using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrainingDivisionKedis.BLL.Common;
using TrainingDivisionKedis.BLL.Contracts;
using TrainingDivisionKedis.BLL.DTO;
using TrainingDivisionKedis.BLL.DTO.Chat;
using TrainingDivisionKedis.BLL.Services;
using TrainingDivisionKedis.Core.Enums;
using TrainingDivisionKedis.Core.Models;
using TrainingDivisionKedis.Core.SPModels.ActivityOfTeachers;
using TrainingDivisionKedis.Core.SPModels.Messages;
using TrainingDivisionKedis.DAL.ApplicationDbContext;
using TrainingDivisionKedis.DAL.Contracts;
using TrainingDivisionKedis.Core.Contracts.Queries;
using TrainingDivisionKedis.DAL.Extensions;
using Xunit;

namespace TrainingDivisionKedis.BLL.Tests
{
    public class StudentChatServiceTests
    {
        public StudentChatService _sut;
        public IMapper _mapper;

        public StudentChatServiceTests()
        {
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfileBLL());
            });
            _mapper = mappingConfig.CreateMapper();
        }

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
                new FileType { Id = 2, Extension = "docx", Type = "FileType2" }                
            };
        }

        private static List<SPTeacherGetAll> GetTestTeachers()
        {
            return new List<SPTeacherGetAll>
            {
                new SPTeacherGetAll{ Id = 1, Name = "TeacherName1", Nom = 1, Post = "Post1" },
                new SPTeacherGetAll{ Id = 2, Name = "TeacherName2", Nom = 2, Post = "Post2" },
                new SPTeacherGetAll{ Id = 3, Name = "TeacherName3", Nom = 3, Post = "Post3" },
                new SPTeacherGetAll{ Id = 4, Name = "TeacherName4", Nom = 4, Post = "Post4" }
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

        private static Mock<IAppDbContextFactory> SetupContextFactory(IMessagesQuery messagesQuery, IMessageFileQuery messageFileQuery, ITeachersQuery teachersQuery)
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
            QueryExtensions.TeachersQueryFactory = context => teachersQuery;

            var mockDbContextFactory = new Mock<IAppDbContextFactory>();
            mockDbContextFactory.Setup(cf => cf.Create()).Returns(dbContext);
            return mockDbContextFactory;
        }

        private static int[] GetTestRecipientsArray()
        {
            Random rnd = new Random();
            var result = new int[1001];
            for (int i = 0; i < 1001; i++)
                result[i] = rnd.Next(1, 1000);
            return result;
        }

        [Fact]
        public void Constructor_ShouldThrowExceptionWhenArgumentIsNull1()
        {
            var mockFileService = new Mock<IFileService<ChatFilesConfiguration>>();           
            Assert.Throws<ArgumentNullException>(()=>_sut = new StudentChatService(null, mockFileService.Object, _mapper));
        }

        [Fact]
        public void Constructor_ShouldThrowExceptionWhenArgumentIsNull2()
        {
            var mockDbContextFactory = new Mock<IAppDbContextFactory>();
            Assert.Throws<ArgumentNullException>(() => _sut = new StudentChatService(mockDbContextFactory.Object, null, _mapper));
        }

        [Fact]
        public void Constructor_ShouldThrowExceptionWhenArgumentIsNull3()
        {
            var mockDbContextFactory = new Mock<IAppDbContextFactory>();
            var mockFileService = new Mock<IFileService<ChatFilesConfiguration>>();
            Assert.Throws<ArgumentNullException>(() => _sut = new StudentChatService(mockDbContextFactory.Object, mockFileService.Object, null));
        }

        [Fact]
        public async Task GetFileByMessageAsync_ShouldReturnDto()
        {
            // ARRANGE
            var expectedFileName = GetTestMessageFiles().First().Name;
            var expectedFileType = GetTestFileTypes().First().Type;
            var expected = new FileDto { FileBytes = new byte[] { 1, 0, 1, 0 }, FileName = expectedFileName, FileType = expectedFileType };

            var mockFileService = new Mock<IFileService<ChatFilesConfiguration>>();
            mockFileService
                .Setup(f => f.GetFileBytes(It.IsAny<string>()))
                .Returns(expected.FileBytes);

            var mockContextFactory = SetupContextFactory(null,null,null);
            _sut = new StudentChatService(mockContextFactory.Object, mockFileService.Object, _mapper);     

            // ACT
            var actual = await _sut.GetFileByMessageAsync(1);

            // ASSERT
            Assert.Equal(ComparableObject.Convert(expected), ComparableObject.Convert(actual.Entity));
        }

        [Fact]
        public async Task GetFileByMessageAsync_ShouldReturnErrorWhenMessageNotFound()
        {
            // ARRANGE
            var mockFileService = new Mock<IFileService<ChatFilesConfiguration>>();
            var mockContextFactory = SetupContextFactory(null, null, null);
            _sut = new StudentChatService(mockContextFactory.Object, mockFileService.Object, _mapper);

            // ACT
            var actual = await _sut.GetFileByMessageAsync(10);

            // ASSERT
            Assert.Equal("Сообщение не найдено", actual.Error.Message);
        }

        [Fact]
        public async Task GetFileByMessageAsync_ShouldReturnErrorWhenMessageFileNotFound()
        {
            // ARRANGE
            var mockFileService = new Mock<IFileService<ChatFilesConfiguration>>();
            var mockContextFactory = SetupContextFactory(null, null, null);
            _sut = new StudentChatService(mockContextFactory.Object, mockFileService.Object, _mapper);

            // ACT
            var actual = await _sut.GetFileByMessageAsync(6);

            // ASSERT
            Assert.Equal("Файл отсутствует в данном сообщении", actual.Error.Message);
        }

        [Fact]
        public async Task GetFileByMessageAsync_ShouldReturnErrorWhenExceptionInQuery()
        {
            // ARRANGE
            var mockFileService = new Mock<IFileService<ChatFilesConfiguration>>();
            mockFileService.Setup(f => f.GetFileBytes(It.IsAny<string>()))
                .Throws(new Exception("Mock exception"));

            var mockContextFactory = SetupContextFactory(null, null, null);
            _sut = new StudentChatService(mockContextFactory.Object, mockFileService.Object, _mapper);

            // ACT
            var actual = await _sut.GetFileByMessageAsync(1);

            // ASSERT
            Assert.Equal("Mock exception", actual.Error.Message);
        }

        [Fact]
        public async Task GetMessagesByUsersAsync_ShouldReturnList()
        {
            // ARRANGE
            var mockMessagesQuery = new Mock<IMessagesQuery>();
            mockMessagesQuery
                .Setup(m => m.GetMessagesByUsers(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(GetTestMessagesByUsers());
            var mockFileService = new Mock<IFileService<ChatFilesConfiguration>>();

            var mockContextFactory = SetupContextFactory(mockMessagesQuery.Object, null, null);
            _sut = new StudentChatService(mockContextFactory.Object, mockFileService.Object, _mapper);

            // ACT
            var actual = await _sut.GetMessagesByUsersAsync(1,2);

            // ASSERT
            Assert.Equal(GetTestMessagesByUsers().Count, actual.Entity.Count);
            Assert.Equal(ComparableObject.Convert(GetTestMessagesByUsers().First()), ComparableObject.Convert(actual.Entity.First()));
        }

        [Fact]
        public async Task GetMessagesByUsersAsync_ShouldReturnErrorWhenExceptionInQuery()
        {
            // ARRANGE
            var mockMessagesQuery = new Mock<IMessagesQuery>();
            mockMessagesQuery
                .Setup(m => m.GetMessagesByUsers(It.IsAny<int>(), It.IsAny<int>()))
                .ThrowsAsync(new Exception("Mock exception"));
            var mockFileService = new Mock<IFileService<ChatFilesConfiguration>>();

            var mockContextFactory = SetupContextFactory(mockMessagesQuery.Object, null, null);
            _sut = new StudentChatService(mockContextFactory.Object, mockFileService.Object, _mapper);

            // ACT
            var actual = await _sut.GetMessagesByUsersAsync(1, 2);

            // ASSERT
            Assert.Equal("Mock exception", actual.Error.Message);
        }

        [Fact]
        public async Task GetRecipientsAsync_ShouldReturnList()
        {
            // ARRANGE
            var mockTeachersQuery = new Mock<ITeachersQuery>();
            mockTeachersQuery
                .Setup(t => t.GetAll())
                .ReturnsAsync(GetTestTeachers());

            var mockFileService = new Mock<IFileService<ChatFilesConfiguration>>();

            var mockContextFactory = SetupContextFactory(null, null, mockTeachersQuery.Object);
            _sut = new StudentChatService(mockContextFactory.Object, mockFileService.Object, _mapper);

            // ACT
            var actual = await _sut.GetRecipientsAsync();

            // ASSERT
            Assert.Equal(GetTestTeachers().Count, actual.Entity.Count);
            Assert.Equal(ComparableObject.Convert(GetTestTeachers().First()), ComparableObject.Convert(actual.Entity.First()));
        }

        [Fact]
        public async Task GetRecipientsAsync_ShouldReturnErrorWhenExceptionInQuery()
        {
            // ARRANGE
            var mockTeachersQuery = new Mock<ITeachersQuery>();
            mockTeachersQuery
                .Setup(t => t.GetAll())
                .ThrowsAsync(new Exception("Mock exception"));

            var mockFileService = new Mock<IFileService<ChatFilesConfiguration>>();

            var mockContextFactory = SetupContextFactory(null, null, mockTeachersQuery.Object);
            _sut = new StudentChatService(mockContextFactory.Object, mockFileService.Object, _mapper);

            // ACT
            var actual = await _sut.GetRecipientsAsync();

            // ASSERT
            Assert.Equal("Mock exception", actual.Error.Message);
        }

        [Fact]
        public async Task CreateAsync_ShouldReturnTrueWhenAplliedFileNotNull()
        {
            var newMessageFile = new MessageFile { Id = 10, Name = "NewFileName", FileTypeId = 1 };

            var mockMessageFilesQuery = new Mock<IMessageFileQuery>();
            mockMessageFilesQuery
                .Setup(m => m.Create(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(newMessageFile);

            var mockMessagesQuery = new Mock<IMessagesQuery>();
            mockMessagesQuery
                .Setup(m => m.Create(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<byte>(), It.IsAny<int?>()))
                .ReturnsAsync(0);

            var mockFormFile = new Mock<IFormFile>();
            mockFormFile.SetupGet(f => f.ContentType).Returns(GetTestFileTypes().First().Type);
            mockFormFile.SetupGet(f => f.FileName).Returns(newMessageFile.Name);

            var mockFileService = new Mock<IFileService<ChatFilesConfiguration>>();
            mockFileService
                .Setup(f => f.UploadAsync(It.IsAny<IFormFile>()))
                .ReturnsAsync(newMessageFile.Name);

            var mockContextFactory = SetupContextFactory(mockMessagesQuery.Object, mockMessageFilesQuery.Object, null);
            _sut = new StudentChatService(mockContextFactory.Object, mockFileService.Object, _mapper);
            var request = new MessageCreateRequest { Sender = 1, Recipients = new int[] { 2, 3 }, Text = "NewMessageText", AppliedFile = mockFormFile.Object };

            // ACT
            var actual = await _sut.CreateAsync(request);

            // ASSERT
            Assert.True(actual.Entity);
        }

        [Fact]
        public async Task CreateAsync_ShouldReturnTrueWhenAplliedFileIsNull()
        {
            var mockMessagesQuery = new Mock<IMessagesQuery>();
            mockMessagesQuery
                .Setup(m => m.Create(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<byte>(), It.IsAny<int?>()))
                .ReturnsAsync(0);

            var mockFileService = new Mock<IFileService<ChatFilesConfiguration>>();

            var mockContextFactory = SetupContextFactory(mockMessagesQuery.Object, null, null);
            _sut = new StudentChatService(mockContextFactory.Object, mockFileService.Object, _mapper);
            var request = new MessageCreateRequest { Sender = 1, Recipients = new int[] { 2, 3 }, Text = "NewMessageText" };

            // ACT
            var actual = await _sut.CreateAsync(request);

            // ASSERT
            Assert.True(actual.Entity);
        }

        [Fact]
        public async Task CreateAsync_ShouldReturnErrorWhenAplliedFileTypeIsNotAllowed()
        {          
            var mockMessagesQuery = new Mock<IMessagesQuery>();

            var mockFormFile = new Mock<IFormFile>();
            mockFormFile.SetupGet(f => f.ContentType).Returns("application/bat");
            mockFormFile.SetupGet(f => f.FileName).Returns("NewFileName.bat");

            var mockFileService = new Mock<IFileService<ChatFilesConfiguration>>();

            var mockContextFactory = SetupContextFactory(mockMessagesQuery.Object, null, null);
            _sut = new StudentChatService(mockContextFactory.Object, mockFileService.Object, _mapper);
            var request = new MessageCreateRequest { Sender = 1, Recipients = new int[] { 2, 3 }, Text = "NewMessageText", AppliedFile = mockFormFile.Object };

            // ACT
            var actual = await _sut.CreateAsync(request);

            // ASSERT
            Assert.Equal("Файл \"" + request.AppliedFile.FileName + "\" не может быть загружен", actual.Error.Message);
        }

        [Fact]
        public async Task CreateAsync_ShouldReturnErrorWhenRecipientsIsNull()
        {
            var mockMessagesQuery = new Mock<IMessagesQuery>();

            var mockFileService = new Mock<IFileService<ChatFilesConfiguration>>();

            var mockContextFactory = SetupContextFactory(mockMessagesQuery.Object, null, null);
            _sut = new StudentChatService(mockContextFactory.Object, mockFileService.Object, _mapper);
            var request = new MessageCreateRequest { Sender = 1, Recipients = null, Text = "NewMessageText" };

            // ACT
            var actual = await _sut.CreateAsync(request);

            // ASSERT
            Assert.Equal("Получатели не указаны", actual.Error.Message);
        }

        [Fact]
        public async Task CreateAsync_ShouldReturnErrorWhenRecipientsNumberIsZero()
        {
            var mockMessagesQuery = new Mock<IMessagesQuery>();

            var mockFileService = new Mock<IFileService<ChatFilesConfiguration>>();

            var mockContextFactory = SetupContextFactory(mockMessagesQuery.Object, null, null);
            _sut = new StudentChatService(mockContextFactory.Object, mockFileService.Object, _mapper);
            var request = new MessageCreateRequest { Sender = 1, Recipients = new int[] { }, Text = "NewMessageText" };

            // ACT
            var actual = await _sut.CreateAsync(request);

            // ASSERT
            Assert.Equal("Количество получателей должно быть минимум 1", actual.Error.Message);
        }

        [Fact]
        public async Task CreateAsync_ShouldReturnErrorWhenRecipientsNumberIsLarge()
        {
            var mockMessagesQuery = new Mock<IMessagesQuery>();

            var mockFileService = new Mock<IFileService<ChatFilesConfiguration>>();

            var mockContextFactory = SetupContextFactory(mockMessagesQuery.Object, null, null);
            _sut = new StudentChatService(mockContextFactory.Object, mockFileService.Object, _mapper);
            var request = new MessageCreateRequest { Sender = 1, Recipients = GetTestRecipientsArray(), Text = "NewMessageText" };

            // ACT
            var actual = await _sut.CreateAsync(request);

            // ASSERT
            Assert.Equal("Количество получателей должно быть максимум 1000", actual.Error.Message);
        }

        [Fact]
        public async Task CreateAsync_ShouldReturnErrorWhenExceptionInQuery1()
        {
            var mockMessageFilesQuery = new Mock<IMessageFileQuery>();
            mockMessageFilesQuery
                .Setup(m => m.Create(It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(new Exception("Mock exception 1"));

            var mockFileService = new Mock<IFileService<ChatFilesConfiguration>>();
            mockFileService
                .Setup(f => f.UploadAsync(It.IsAny<IFormFile>()))
                .ReturnsAsync("Some name");

            var mockFormFile = new Mock<IFormFile>();
            mockFormFile.SetupGet(f => f.ContentType).Returns(GetTestFileTypes().First().Type);
            mockFormFile.SetupGet(f => f.FileName).Returns("Some name");

            var mockContextFactory = SetupContextFactory(null, mockMessageFilesQuery.Object, null);
            _sut = new StudentChatService(mockContextFactory.Object, mockFileService.Object, _mapper);
            var request = new MessageCreateRequest { Sender = 1, Recipients = new int[] { 1, 2, 3 }, Text = "NewMessageText", AppliedFile = mockFormFile.Object };

            // ACT
            var actual = await _sut.CreateAsync(request);

            // ASSERT
            Assert.Equal("Mock exception 1", actual.Error.Message);
        }

        [Fact]
        public async Task CreateAsync_ShouldReturnErrorWhenExceptionInQuery2()
        {
            var newMessageFile = new MessageFile { Id = 10, Name = "NewFileName", FileTypeId = 1 };

            var mockMessageFilesQuery = new Mock<IMessageFileQuery>();
            mockMessageFilesQuery
                .Setup(m => m.Create(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(newMessageFile);

            var mockMessagesQuery = new Mock<IMessagesQuery>();
            mockMessagesQuery.Setup(m => m.Create(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<byte>(), It.IsAny<int?>()))
                .ThrowsAsync(new Exception("Mock exception"));

            var mockFileService = new Mock<IFileService<ChatFilesConfiguration>>();

            var mockContextFactory = SetupContextFactory(mockMessagesQuery.Object, mockMessageFilesQuery.Object, null);
            _sut = new StudentChatService(mockContextFactory.Object, mockFileService.Object, _mapper);
            var request = new MessageCreateRequest { Sender = 1, Recipients = new int[] { 1, 2, 3 }, Text = "NewMessageText" };

            // ACT
            var actual = await _sut.CreateAsync(request);

            // ASSERT
            Assert.Equal("Mock exception", actual.Error.Message);
        }

        [Fact]
        public async Task GetMessageContactsByUserAsync_ShouldReturnList()
        {
            var mockMessagesQuery = new Mock<IMessagesQuery>();
            mockMessagesQuery.Setup(m => m.GetContactsByUser(It.IsAny<int>(), It.IsAny<byte>()))
                .ReturnsAsync(GetTestContactsByUser());

            var mockFileService = new Mock<IFileService<ChatFilesConfiguration>>();

            var mockContextFactory = SetupContextFactory(mockMessagesQuery.Object, null, null);
            _sut = new StudentChatService(mockContextFactory.Object, mockFileService.Object, _mapper);

            // ACT
            var actual = await _sut.GetMessageContactsByUserAsync(1);

            // ASSERT
            Assert.Equal(GetTestContactsByUser().Count, actual.Entity.Count);
            Assert.Equal(ComparableObject.Convert(GetTestContactsByUser().First()), ComparableObject.Convert(actual.Entity.First()));
        }

        [Fact]
        public async Task GetMessageContactsByUserAsync_ShouldReturnErrorWhenExceptionInQuery()
        {
            var mockMessagesQuery = new Mock<IMessagesQuery>();
            mockMessagesQuery.Setup(m => m.GetContactsByUser(It.IsAny<int>(), It.IsAny<byte>()))
                .ThrowsAsync(new Exception("Mock exception"));

            var mockFileService = new Mock<IFileService<ChatFilesConfiguration>>();

            var mockContextFactory = SetupContextFactory(mockMessagesQuery.Object, null, null);
            _sut = new StudentChatService(mockContextFactory.Object, mockFileService.Object, _mapper);

            // ACT
            var actual = await _sut.GetMessageContactsByUserAsync(1);

            // ASSERT
            Assert.Equal("Mock exception", actual.Error.Message);
        }
    }
}
