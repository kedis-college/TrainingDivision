using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using TrainingDivisionKedis.BLL.Common;
using TrainingDivisionKedis.BLL.Contracts;
using TrainingDivisionKedis.BLL.DTO.Chat;
using TrainingDivisionKedis.BLL.Services;
using TrainingDivisionKedis.Controllers;
using TrainingDivisionKedis.Core.Contracts.Queries;
using TrainingDivisionKedis.Core.Enums;
using TrainingDivisionKedis.Core.Models;
using TrainingDivisionKedis.Core.SPModels.Messages;
using TrainingDivisionKedis.Core.SPModels.Students;
using TrainingDivisionKedis.DAL.ApplicationDbContext;
using TrainingDivisionKedis.DAL.Contracts;
using TrainingDivisionKedis.DAL.Extensions;
using TrainingDivisionKedis.Extensions.Alerts;
using Xunit;

namespace TrainingDivisionKedis.Tests.ControllersIntegrationTests
{
    public class ChatControllerTests
    {
        ChatController _sut;
        readonly IMapper _mapper;

        private static List<Message> GetTestMessages()
        {
            return new List<Message>
            {
                new Message { Id = 1, CreatedAt = new DateTime(2020,05,01), Received = false, Recipient = 1, Sender = 2, Text = "Message1 text", Type = (byte) MessageTypeEnum.TeacherToStudent, MessageFile = GetTestMessageFiles().First() },
                new Message { Id = 2, CreatedAt = new DateTime(2020,05,01), Received = true, Recipient = 3, Sender = 2, Text = "Message1 text", Type = (byte) MessageTypeEnum.TeacherToStudent, MessageFile = GetTestMessageFiles().First() },
                new Message { Id = 3, CreatedAt = new DateTime(2020,05,01), Received = false, Recipient = 2, Sender = 1, Text = "Message2 text", Type = (byte) MessageTypeEnum.StudentToTeacher, MessageFile = GetTestMessageFiles().Last() },
                new Message { Id = 4, CreatedAt = new DateTime(2020,05,01), Received = true, Recipient = 4, Sender = 1, Text = "Message2 text", Type = (byte) MessageTypeEnum.StudentToTeacher, MessageFile = GetTestMessageFiles().Last() },
                new Message { Id = 5, CreatedAt = new DateTime(2020,05,01), Received = false, Recipient = 1, Sender = 2, Text = "Message3 text", Type = (byte) MessageTypeEnum.TeacherToStudent },
                new Message { Id = 6, CreatedAt = new DateTime(2020,05,01), Received = false, Recipient = 2, Sender = 1, Text = "Message4 text", Type = (byte) MessageTypeEnum.StudentToTeacher }
            };
        }

        private static List<MessageFile> GetTestMessageFiles()
        {
            return new List<MessageFile>
            {
                new MessageFile { Id = 1, Name = "MessageFile1.pdf", FileType = GetTestFileTypes().First() },
                new MessageFile { Id = 2, Name = "MessageFile2.docx", FileType = GetTestFileTypes().Last()  }
            };
        }

        private static List<FileType> GetTestFileTypes()
        {
            return new List<FileType>
            {
                new FileType { Id = 1, Extension = "pdf", Type = "FileType1" },
                new FileType { Id = 2, Extension = "docx", Type = "FileType2" },
                new FileType { Id = 3, Extension = "png", Type = "FileType3" }
            };
        }

        private static List<SPStudentsGetWithSpeciality> GetTestStudents()
        {
            return new List<SPStudentsGetWithSpeciality>
            {
                new SPStudentsGetWithSpeciality{ Id = 1, FullName = "StudentName1", Course = 1, Group = "Group1", Speciality = "Speciality1" },
                new SPStudentsGetWithSpeciality{ Id = 2, FullName = "TeacherName2", Course = 2, Group = "Group2", Speciality = "Speciality2" },
                new SPStudentsGetWithSpeciality{ Id = 3, FullName = "TeacherName3", Course = 3, Group = "Group3", Speciality = "Speciality3" },
                new SPStudentsGetWithSpeciality{ Id = 4, FullName = "TeacherName4", Course = 4, Group = "Group4", Speciality = "Speciality4" }
            };
        }

        private static List<SPMessagesGetByUsers> GetTestMessagesByUsers()
        {
            return new List<SPMessagesGetByUsers>
            {
                new SPMessagesGetByUsers { Id = 1, CreatedAt = new DateTime(2020,05,01), MessageFileId = 1, Received = false, Recipient = 1, Sender = 2, Text = "Message1 text", Type = (byte) MessageTypeEnum.TeacherToStudent, MessageFileName = "MessageFile1", SenderName = "TeacherName1" },
                new SPMessagesGetByUsers { Id = 3, CreatedAt = new DateTime(2020,05,01), MessageFileId = 2, Received = false, Recipient = 2, Sender = 1, Text = "Message2 text", Type = (byte) MessageTypeEnum.StudentToTeacher, MessageFileName = "MessageFile2", SenderName = "StudentName1" },
                new SPMessagesGetByUsers { Id = 5, CreatedAt = new DateTime(2020,05,01), Received = false, Recipient = 1, Sender = 2, Text = "Message3 text", Type = (byte) MessageTypeEnum.TeacherToStudent, SenderName = "TeacherName1" },
                new SPMessagesGetByUsers { Id = 6, CreatedAt = new DateTime(2020,05,01), Received = false, Recipient = 2, Sender = 1, Text = "Message4 text", Type = (byte) MessageTypeEnum.StudentToTeacher, SenderName = "StudentName1" }

            };
        }

        private static List<SPMessagesGetContactsByUser> GetTestContactsByUser()
        {
            return new List<SPMessagesGetContactsByUser>
            {
                new SPMessagesGetContactsByUser { Id = 1, Name = "ContactName1", Group = "Group1", NewMessages = 2},
                new SPMessagesGetContactsByUser { Id = 2, Name = "ContactName2", Group = "Group2", NewMessages = 1}
            };
        }

        private static Mock<IAppDbContextFactory> SetupContextFactory(IMessagesQuery messagesQuery, IMessageFileQuery messageFileQuery, IStudentsQuery studentsQuery)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                // don't raise the error warning us that the in memory db doesn't support transactions
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;
            var dbContext = new AppDbContext(options);

            dbContext.FileTypes.AddRange(GetTestFileTypes());
            dbContext.MessageFiles.AddRange(GetTestMessageFiles());
            dbContext.Messages.AddRange(GetTestMessages());

            dbContext.SaveChanges();

            QueryExtensions.MessagesQueryFactory = context => messagesQuery;
            QueryExtensions.MessageFileQueryFactory = context => messageFileQuery;
            QueryExtensions.StudentsQueryFactory = context => studentsQuery;

            var mockDbContextFactory = new Mock<IAppDbContextFactory>();
            mockDbContextFactory.Setup(cf => cf.Create()).Returns(dbContext);
            return mockDbContextFactory;
        }

        public static IFileService<ChatFilesConfiguration> SetupFileService(string directory = @"C:\Users\E7450\Downloads\VKR\TrainingDivisionKedis\TrainingDivisionKedis.BLL.Tests\bin\Debug\netcoreapp2.2\", int maxSize = 10000)
        {
            var filesConfiguration = new ChatFilesConfiguration { Directory = directory, MaxSize = maxSize };
            IOptions<ChatFilesConfiguration> options = Options.Create(filesConfiguration);
            return new LocalFileService<ChatFilesConfiguration>(options);
        }

        public ChatControllerTests()
        {        
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfileBLL());
            });
            _mapper = mappingConfig.CreateMapper();           
        }

        [Fact]
        public void Index_ShouldReturnView()
        {
            // Act           
            var response = _sut.Index();

            // Assert
            var result = Assert.IsType<ViewResult>(response);
        }

        [Fact]
        public async Task GetCreate_ShouldReturnView()
        {
            // Arrange
            var mockStudentsQuery = new Mock<IStudentsQuery>();
            mockStudentsQuery
                .Setup(t => t.GetWithSpeciality())
                .ReturnsAsync(GetTestStudents());

            var mockContextFactory = SetupContextFactory(null, null, mockStudentsQuery.Object);
            var fileService = SetupFileService();
            var chatService = new TeacherChatService(mockContextFactory.Object, fileService, _mapper);
            _sut = new ChatController(chatService);

            // Act           
            var response = await _sut.Create();

            // Assert
            var result = Assert.IsType<ViewResult>(response);
            Assert.NotNull(result.Model);
        }

        [Fact]
        public async Task GetCreate_ShouldReturnRedirectWithWarningWhenArrayIsEmpty()
        {
            // Arrange
            var mockStudentsQuery = new Mock<IStudentsQuery>();
            mockStudentsQuery
                .Setup(t => t.GetWithSpeciality())
                .ReturnsAsync(new List<SPStudentsGetWithSpeciality>());

            var mockContextFactory = SetupContextFactory(null, null, mockStudentsQuery.Object);
            var fileService = SetupFileService();
            var chatService = new TeacherChatService(mockContextFactory.Object, fileService, _mapper);
            _sut = new ChatController(chatService);

            // Act           
            var response = await _sut.Create();

            // Assert
            var result = Assert.IsType<AlertDecoratorResult>(response);
            Assert.Equal("warning", result.Type);
        }

        [Fact]
        public async Task PostCreate_ShouldReturnRedirectWithSuccessWhenFileIsNotNull()
        {
            // Arrange
            var newMessageFile = new MessageFile { Id = 10, Name = "NewFileName", FileTypeId = 3 };
            var mockMessageFilesQuery = new Mock<IMessageFileQuery>();
            mockMessageFilesQuery
                .Setup(m => m.Create(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(newMessageFile);

            var mockMessagesQuery = new Mock<IMessagesQuery>();
            mockMessagesQuery
                .Setup(m => m.Create(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<byte>(), It.IsAny<int?>()))
                .ReturnsAsync(0);

            var mockContextFactory = SetupContextFactory(mockMessagesQuery.Object, mockMessageFilesQuery.Object, null);
            var fileService = SetupFileService();
            var chatService = new TeacherChatService(mockContextFactory.Object, fileService, _mapper);
            _sut = new ChatController(chatService);

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, "John Doe"),
                new Claim(ClaimTypes.NameIdentifier, "1307"),
                new Claim(ClaimTypes.Role, "директор")
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            var context = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = claimsPrincipal
                }
            };
            _sut.ControllerContext = context;

            var mockFile = new Mock<IFormFile>();
            var sourceImg = File.OpenRead(@"C:\Users\E7450\Pictures\handMade\64d735876ce855d858a742001e0585ea.jpg");
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms);
            writer.Write(sourceImg);
            writer.Flush();
            ms.Position = 0;
            var fileName = "QQ.png";
            mockFile.Setup(f => f.FileName).Returns(fileName).Verifiable();
            mockFile.Setup(f => f.ContentType).Returns("FileType3").Verifiable();
            mockFile.Setup(f => f.Length).Returns(9999000).Verifiable();
            mockFile.Setup(_ => _.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
                .Returns((Stream stream, CancellationToken token) => ms.CopyToAsync(stream))
                .Verifiable();

            // Act
            var request = new MessageCreateRequest { Text = "", Recipients = new int[] {1,2,3}, Sender = 1, AppliedFile = mockFile.Object };
            var response = await _sut.Create(request);

            // Assert
            var result = Assert.IsType<AlertDecoratorResult>(response);
            Assert.Equal("success",result.Type);
            mockFile.Verify();
        }

        [Fact]
        public async Task PostCreate_ShouldReturnRedirectWithSuccessWhenFileIsNull()
        {
            // Arrange
            var mockMessagesQuery = new Mock<IMessagesQuery>();
            mockMessagesQuery
                .Setup(m => m.Create(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<byte>(), It.IsAny<int?>()))
                .ReturnsAsync(0);

            var mockContextFactory = SetupContextFactory(mockMessagesQuery.Object, null, null);
            var fileService = SetupFileService();
            var chatService = new TeacherChatService(mockContextFactory.Object, fileService, _mapper);
            _sut = new ChatController(chatService);

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, "John Doe"),
                new Claim(ClaimTypes.NameIdentifier, "1307"),
                new Claim(ClaimTypes.Role, "директор")
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            var context = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = claimsPrincipal
                }
            };
            _sut.ControllerContext = context;

            // Act
            var request = new MessageCreateRequest { Text = "Some text", Recipients = new int[] { 1, 2, 3 }, Sender = 1 };
            var response = await _sut.Create(request);

            // Assert
            var result = Assert.IsType<AlertDecoratorResult>(response);
            Assert.Equal("success", result.Type);
        }

        [Fact]
        public async Task PostCreate_ShouldReturnRedirectWithDangerWhenExpectionInQuery()
        {
            // Arrange
            var mockMessagesQuery = new Mock<IMessagesQuery>();
            mockMessagesQuery
                .Setup(m => m.Create(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<byte>(), It.IsAny<int?>()))
                .ThrowsAsync(new Exception("Mock exception"));

            var mockContextFactory = SetupContextFactory(mockMessagesQuery.Object, null, null);
            var fileService = SetupFileService();
            var chatService = new TeacherChatService(mockContextFactory.Object, fileService, _mapper);
            _sut = new ChatController(chatService);

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, "John Doe"),
                new Claim(ClaimTypes.NameIdentifier, "1307"),
                new Claim(ClaimTypes.Role, "директор")
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            var context = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = claimsPrincipal
                }
            };
            _sut.ControllerContext = context;

            // Act
            var request = new MessageCreateRequest { Text = null, AppliedFile = null, Recipients = new int[] { 1, 2, 3 }, Sender = 1 };
            var response = await _sut.Create(request);

            // Assert
            var result = Assert.IsType<AlertDecoratorResult>(response);
            Assert.Equal("Mock exception", result.Body);
        }

        [Fact]
        public async Task PostCreate_ShouldReturnBadRequestWhenNotAuth()
        {
            // Arrange
            var mockContextFactory = SetupContextFactory(null, null, null);
            var fileService = SetupFileService();
            var chatService = new TeacherChatService(mockContextFactory.Object, fileService, _mapper);
            _sut = new ChatController(chatService);

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, "John Doe"),
                new Claim(ClaimTypes.Role, "директор")
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            var context = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
            _sut.ControllerContext = context;

            // Act
            var request = new MessageCreateRequest { Text = null, AppliedFile = null, Recipients = new int[] { 1, 2, 3 }, Sender = 1 };
            var response = await _sut.Create(request);

            // Assert
            var result = Assert.IsType<BadRequestResult>(response);
        }

        [Fact]
        public async Task PostCreate_ShouldReturnRedirectWithDangerWhenRecipientsAreEmpty()
        {
            // Arrange
            var mockContextFactory = SetupContextFactory(null, null, null);
            var fileService = SetupFileService();
            var chatService = new TeacherChatService(mockContextFactory.Object, fileService, _mapper);
            _sut = new ChatController(chatService);

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, "John Doe"),
                new Claim(ClaimTypes.NameIdentifier, "1307"),
                new Claim(ClaimTypes.Role, "директор")
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            var context = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = claimsPrincipal
                }
            };
            _sut.ControllerContext = context;

            // Act
            var request = new MessageCreateRequest { Text = null, AppliedFile = null, Recipients = null, Sender = 1 };
            var response = await _sut.Create(request);

            // Assert
            var result = Assert.IsType<AlertDecoratorResult>(response);
            Assert.Equal("Получатели не указаны", result.Body);
        }

        [Fact]
        public async Task PostCreateAjax_ShouldReturnRedirectWithSuccessWhenFileIsNull()
        {
            // Arrange
            var mockMessagesQuery = new Mock<IMessagesQuery>();
            mockMessagesQuery
                .Setup(m => m.Create(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<byte>(), It.IsAny<int?>()))
                .ReturnsAsync(0);

            var mockContextFactory = SetupContextFactory(mockMessagesQuery.Object, null, null);
            var fileService = SetupFileService();
            var chatService = new TeacherChatService(mockContextFactory.Object, fileService, _mapper);
            _sut = new ChatController(chatService);

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, "John Doe"),
                new Claim(ClaimTypes.NameIdentifier, "1307"),
                new Claim(ClaimTypes.Role, "директор")
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            var context = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = claimsPrincipal
                }
            };
            _sut.ControllerContext = context;

            // Act
            var request = new MessageCreateRequest { Text = "Some text", Recipients = new int[] { 1, 2, 3 }, Sender = 1 };
            var response = await _sut.CreateAjax(request);

            // Assert
            var result = Assert.IsType<JsonResult>(response);
            Assert.Equal(ComparableObject.Convert(OperationDetails<bool>.Success(true)),ComparableObject.Convert(result.Value));
        }

        [Fact]
        public async Task PostCreateAjax_ShouldReturnRedirectWithDangerWhenExpectionInQuery()
        {
            // Arrange
            var mockMessagesQuery = new Mock<IMessagesQuery>();
            mockMessagesQuery
                .Setup(m => m.Create(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<byte>(), It.IsAny<int?>()))
                .ThrowsAsync(new Exception("Mock exception"));

            var mockContextFactory = SetupContextFactory(mockMessagesQuery.Object, null, null);
            var fileService = SetupFileService();
            var chatService = new TeacherChatService(mockContextFactory.Object, fileService, _mapper);
            _sut = new ChatController(chatService);

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, "John Doe"),
                new Claim(ClaimTypes.NameIdentifier, "1307"),
                new Claim(ClaimTypes.Role, "директор")
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            var context = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = claimsPrincipal
                }
            };
            _sut.ControllerContext = context;

            // Act
            var request = new MessageCreateRequest { Text = null, AppliedFile = null, Recipients = new int[] { 1, 2, 3 }, Sender = 1 };
            var response = await _sut.CreateAjax(request);

            // Assert
            var result = Assert.IsType<JsonResult>(response);
            Assert.Equal(ComparableObject.Convert(OperationDetails<bool>.Failure("Mock exception", "System.Private.CoreLib")), ComparableObject.Convert(result.Value));
        }

        [Fact]
        public async Task PostCreateAjax_ShouldReturnBadRequestWhenNotAuth()
        {
            // Arrange
            var mockMessagesQuery = new Mock<IMessagesQuery>();
            mockMessagesQuery
                .Setup(m => m.Create(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<byte>(), It.IsAny<int?>()))
                .ThrowsAsync(new Exception("Mock exception"));

            var mockContextFactory = SetupContextFactory(mockMessagesQuery.Object, null, null);
            var fileService = SetupFileService();
            var chatService = new TeacherChatService(mockContextFactory.Object, fileService, _mapper);
            _sut = new ChatController(chatService);

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, "John Doe"),
                new Claim(ClaimTypes.NameIdentifier, "NameIdentifier"),
                new Claim(ClaimTypes.Role, "директор")
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            var context = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = claimsPrincipal
                }
            };
            _sut.ControllerContext = context;

            // Act
            var request = new MessageCreateRequest { Text = null, AppliedFile = null, Recipients = new int[] { 1, 2, 3 }, Sender = 1 };
            var response = await _sut.CreateAjax(request);

            // Assert
            var result = Assert.IsType<BadRequestResult>(response);           
        }

        [Fact]
        public void GetMessagesByUsers_ShouldReturnViewComponent()
        {
            var mockContextFactory = SetupContextFactory(null, null, null);
            var fileService = SetupFileService();
            var chatService = new TeacherChatService(mockContextFactory.Object, fileService, _mapper);
            _sut = new ChatController(chatService);
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, "John Doe"),
                new Claim(ClaimTypes.NameIdentifier, "1307"),
                new Claim(ClaimTypes.Role, "директор")
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            var context = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = claimsPrincipal
                }
            };
            _sut.ControllerContext = context;
            var response = _sut.GetMessagesByUsers(1);
            var result = Assert.IsType<ViewComponentResult>(response);
        }

        [Fact]
        public void GetMessagesByUsers_ShouldReturnMessagePartialWhenNotAuth()
        {
            var mockContextFactory = SetupContextFactory(null, null, null);
            var fileService = SetupFileService();
            var chatService = new TeacherChatService(mockContextFactory.Object, fileService, _mapper);
            _sut = new ChatController(chatService);
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, "John Doe"),
                new Claim(ClaimTypes.Role, "директор")
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            var context = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = claimsPrincipal
                }
            };
            _sut.ControllerContext = context;
            var response = _sut.GetMessagesByUsers(1);
            var result = Assert.IsType<PartialViewResult>(response);
        }

        [Fact]
        public void GetContacts_ShouldReturnViewComponent()
        {
            var mockContextFactory = SetupContextFactory(null, null, null);
            var fileService = SetupFileService();
            var chatService = new TeacherChatService(mockContextFactory.Object, fileService, _mapper);
            _sut = new ChatController(chatService);
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, "John Doe"),
                new Claim(ClaimTypes.NameIdentifier, "1307"),
                new Claim(ClaimTypes.Role, "директор")
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            var context = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = claimsPrincipal
                }
            };
            _sut.ControllerContext = context;
            var response = _sut.GetContacts();
            var result = Assert.IsType<ViewComponentResult>(response);
        }

        [Fact]
        public void GetContacts_ShouldReturnMessagePartialWhenNotAuth()
        {
            var mockContextFactory = SetupContextFactory(null, null, null);
            var fileService = SetupFileService();
            var chatService = new TeacherChatService(mockContextFactory.Object, fileService, _mapper);
            _sut = new ChatController(chatService);
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, "John Doe"),
                new Claim(ClaimTypes.Role, "директор")
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            var context = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = claimsPrincipal
                }
            };
            _sut.ControllerContext = context;
            var response = _sut.GetContacts();
            var result = Assert.IsType<PartialViewResult>(response);
        }
    }
}
