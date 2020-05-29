using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TrainingDivisionKedis.BLL.Common;
using TrainingDivisionKedis.BLL.Contracts;
using TrainingDivisionKedis.BLL.DTO.Raspredelenie;
using TrainingDivisionKedis.BLL.DTO.UmkFiles;
using TrainingDivisionKedis.BLL.Services;
using TrainingDivisionKedis.Controllers;
using TrainingDivisionKedis.Core.Contracts.Queries;
using TrainingDivisionKedis.Core.Models;
using TrainingDivisionKedis.Core.SPModels;
using TrainingDivisionKedis.Core.SPModels.Raspredelenie;
using TrainingDivisionKedis.DAL.ApplicationDbContext;
using TrainingDivisionKedis.DAL.Contracts;
using TrainingDivisionKedis.DAL.Extensions;
using TrainingDivisionKedis.Extensions.Alerts;
using TrainingDivisionKedis.Models.Vedomost;
using TrainingDivisionKedis.ViewModels.Umk;
using Xunit;

namespace TrainingDivisionKedis.Tests.ControllersIntegrationTests
{
    public class UmkControllerTests
    {
        UmkController _sut;
        readonly IMapper _mapper;

        public UmkControllerTests()
        {
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfileBLL());
            });
            _mapper = mappingConfig.CreateMapper();
        }

        public static List<Year> GetTestYears()
        {
            return new List<Year> {
                new Year { Id = 1, Name = "2017-2018", Current = false },
                new Year { Id = 2, Name = "2018-2019", Current = false },
                new Year { Id = 3, Name = "2019-2020", Current = false }
            };
        }

        public static List<Term> GetTestTerms()
        {
            return new List<Term>
            {
                new Term { Id = 1 }, new Term { Id = 2 }, new Term { Id = 3 }, new Term { Id = 4 }, new Term { Id = 5 }, new Term { Id = 6 }
            };
        }

        public static IFileService<UmkFilesConfiguration> SetupFileService(string directory = @"C:\Users\E7450\Downloads\VKR\TrainingDivisionKedis\TrainingDivisionKedis.BLL.Tests\bin\Debug\netcoreapp2.2\", int maxSize = 10000)
        {
            var filesConfiguration = new UmkFilesConfiguration { Directory = directory, MaxSize = maxSize };
            IOptions<UmkFilesConfiguration> options = Options.Create(filesConfiguration);
            return new LocalFileService<UmkFilesConfiguration>(options);
        }

        private static Mock<IAppDbContextFactory> SetupContextFactory(IUmkFilesQuery umkFilesQuery, IRaspredelenieQuery raspredelenieQuery, ICurriculumQuery curriculumQuery)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                // don't raise the error warning us that the in memory db doesn't support transactions
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;
            var dbContext = new AppDbContext(options);

            dbContext.Terms.AddRange(GetTestTerms());
            dbContext.Years.AddRange(GetTestYears());
            dbContext.FileTypes.AddRange(GetTestFileTypes());
            dbContext.UmkFiles.AddRange(GetTestUmkFiles());
            dbContext.SaveChanges();

            QueryExtensions.UmkFilesQueryFactory = context => umkFilesQuery;
            QueryExtensions.RaspredelenieQueryFactory = context => raspredelenieQuery;
            QueryExtensions.CurriculumQueryFactory = context => curriculumQuery;

            var mockDbContextFactory = new Mock<IAppDbContextFactory>();
            mockDbContextFactory.Setup(cf => cf.Create()).Returns(dbContext);
            return mockDbContextFactory;
        }

        private static List<FileType> GetTestFileTypes()
        {
            return new List<FileType>
            {
                new FileType { Id = 1, Extension = "txt", Type = "text/plain" },
                new FileType { Id = 2, Extension = "docx", Type = "FileType2" },
                new FileType { Id = 3, Extension = "pdf", Type = "FileType1" }
            };
        }

        private static List<UmkFile> GetTestUmkFiles()
        {
            return new List<UmkFile>
            {
                new UmkFile { Id = 1, Active = true, CreatedAt = new DateTime(2020,05,1), FileName = "TestTextFile.txt", FileSize = 1000, FileType = GetTestFileTypes().First(), Name = "Name1", SubjectId = 1, TermId = 1, UpdatedAt = new DateTime(2020,05,2)},
                new UmkFile { Id = 2, Active = false, CreatedAt = new DateTime(2020,05,1), FileName = "FileName2", FileSize = 2000, FileType = GetTestFileTypes().Last(), Name = "Name2", SubjectId = 1, TermId = 1, UpdatedAt = new DateTime(2020,05,2)},
                new UmkFile { Id = 3, Active = true, CreatedAt = new DateTime(2020,05,1), FileName = "FileName3", FileSize = 3000, FileType = GetTestFileTypes().First(), Name = "Name3", SubjectId = 1, TermId = 2 },
                new UmkFile { Id = 4, Active = true, CreatedAt = new DateTime(2020,05,1), FileName = "FileName4", FileSize = 4000, FileType = GetTestFileTypes().Last(), Name = "Name4", SubjectId = 2, TermId = 1 },
                new UmkFile { Id = 5, Active = true, CreatedAt = new DateTime(2020,05,1), FileName = "FileName5", FileSize = 5000, FileType = GetTestFileTypes().First(), Name = "Name5", SubjectId = 1, TermId = 1 }
            };
        }

        private static List<SPSubjectsGetByYearAndTermAndUser> GetTestSubjects()
        {
            return new List<SPSubjectsGetByYearAndTermAndUser>
            {
                new SPSubjectsGetByYearAndTermAndUser { Semestr = 1, SubjectId = 1, SubjectName = "Subject1", YearId = 1},
                new SPSubjectsGetByYearAndTermAndUser { Semestr = 1, SubjectId = 2, SubjectName = "Subject2", YearId = 1},
                new SPSubjectsGetByYearAndTermAndUser { Semestr = 1, SubjectId = 3, SubjectName = "Subject3", YearId = 1},
                new SPSubjectsGetByYearAndTermAndUser { Semestr = 1, SubjectId = 4, SubjectName = "Subject4", YearId = 1}
            };
        }

        [Fact]
        public async Task GetIndex_ShouldReturnView()
        {
            // Arrange
            var fileService = SetupFileService();

            var yearService = new YearService(SetupContextFactory(null,null,null).Object);
            var termService = new TermService(SetupContextFactory(null,null,null).Object);
            var raspredelenieService = new Mock<IRaspredelenieService>();
            var umkFileService = new Mock<IUmkFileService>();

            _sut = new UmkController(yearService, termService, raspredelenieService.Object, umkFileService.Object);

            // Act
            var response = await _sut.Index();

            // Assert
            var result = Assert.IsType<ViewResult>(response);
            var model = Assert.IsType<UmkIndexViewModel>(result.Model);
            Assert.Equal(GetTestYears().Count, model.Years?.Count);
            Assert.Equal(GetTestTerms().Count, model.Terms?.Count);
        }

        [Fact]
        public async Task GetIndex_ShouldReturnErrorViewWhenErrorInService()
        {
            // Arrange
            var yearService = new YearService(SetupContextFactory(null, null,null).Object);
            var mockTermService = new Mock<ITermService>();
            mockTermService
                .Setup(t => t.GetAllAsync())
                .ReturnsAsync(OperationDetails<List<Term>>.Failure("mock exception", ""));
            var raspredelenieService = new Mock<IRaspredelenieService>();
            var umkFileService = new Mock<IUmkFileService>();

            _sut = new UmkController(yearService, mockTermService.Object, raspredelenieService.Object, umkFileService.Object);

            // Act
            var response = await _sut.Index();

            // Assert
            var result = Assert.IsType<RedirectToActionResult>(response);
            Assert.Equal("Error", result.ActionName);
        }

        [Fact]
        public async Task PostSubjectsSearch_ShouldReturnRedirectWhenSuccess()
        {
            // ARRANGE     
            var mockRaspredelenie = new Mock<IRaspredelenieQuery>();
            mockRaspredelenie
                .Setup(cq => cq.GetSubjectsByYearAndTermAndUser(It.IsAny<byte>(), It.IsAny<byte>(), It.IsAny<int>()))
                .ReturnsAsync(GetTestSubjects());

            var yearService = new Mock<IYearService>();
            var termService = new Mock<ITermService>();
            var umkFileService = new Mock<IUmkFileService>();
            var raspredelenieService = new RaspredelenieService(SetupContextFactory(null, mockRaspredelenie.Object, null).Object);            

            _sut = new UmkController(yearService.Object, termService.Object, raspredelenieService, umkFileService.Object);
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
            var request = new GetVedomostListRequest(1,1,1);
            var response = await _sut.SubjectsSearch(request);

            // Assert
            var result = Assert.IsType<PartialViewResult>(response);
            var model = Assert.IsType<List<SPSubjectsGetByYearAndTermAndUser>>(result.Model);
            Assert.Equal(GetTestSubjects().Count, model.Count);
        }

        [Fact]
        public async Task PostSubjectsSearch_ShouldReturnMessagePartialWhenFailure()
        {
            // ARRANGE     
            var mockRaspredelenie = new Mock<IRaspredelenieQuery>();
            mockRaspredelenie
                .Setup(cq => cq.GetSubjectsByYearAndTermAndUser(It.IsAny<byte>(), It.IsAny<byte>(), It.IsAny<int>()))
                .ThrowsAsync(new Exception("Mock exception"));

            var yearService = new Mock<IYearService>();
            var termService = new Mock<ITermService>();
            var umkFileService = new Mock<IUmkFileService>();
            var raspredelenieService = new RaspredelenieService(SetupContextFactory(null, mockRaspredelenie.Object, null).Object);

            _sut = new UmkController(yearService.Object, termService.Object, raspredelenieService, umkFileService.Object);
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
            var request = new GetVedomostListRequest(1, 1, 1);
            var response = await _sut.SubjectsSearch(request);

            // Assert
            var result = Assert.IsType<PartialViewResult>(response);
            Assert.IsType<string>(result.Model);
        }

        [Fact]
        public async Task PostSubjectsSearch_ShouldReturnMessagePartialWhenNotAuth()
        {
            // ARRANGE     
            var yearService = new Mock<IYearService>();
            var termService = new Mock<ITermService>();
            var umkFileService = new Mock<IUmkFileService>();
            var raspredelenieService = new Mock<IRaspredelenieService>();

            _sut = new UmkController(yearService.Object, termService.Object, raspredelenieService.Object, umkFileService.Object);
            var claims = new List<Claim>();
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
            var request = new GetVedomostListRequest(1, 1, 1);
            var response = await _sut.SubjectsSearch(request);

            // Assert
            var result = Assert.IsType<PartialViewResult>(response);
            var model = Assert.IsType<string>(result.Model);
            Assert.Equal("Ошибка в запросе", model);
        }

        [Fact]
        public async Task GetDetails_ShouldReturnViewWithModel()
        {
            // ARRANGE    
            var mockCurriculum = new Mock<ICurriculumQuery>();
            mockCurriculum.Setup(c => c.GetNameById(It.IsAny<int>()))
                .ReturnsAsync(new SPGetName { Name = "Subject name" });

            var mockRaspredelenie = new Mock<IRaspredelenieQuery>();
            mockRaspredelenie.Setup(r => r.CheckTeacherBySubjectAndTerm(It.IsAny<short>(), It.IsAny<int>(), It.IsAny<byte>()))
                .ReturnsAsync(new SPBoolResult { Result = true });

            var mockContextFactory = SetupContextFactory(null, mockRaspredelenie.Object, mockCurriculum.Object);
            var mockFileService = new Mock<IFileService<UmkFilesConfiguration>>();
            var umkFileService = new UmkFileService(mockContextFactory.Object, _mapper, mockFileService.Object);
            var yearService = new Mock<IYearService>();
            var termService = new Mock<ITermService>();
            var raspredelenieService = new Mock<IRaspredelenieService>();

            _sut = new UmkController(yearService.Object, termService.Object, raspredelenieService.Object, umkFileService);
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
            var response = await _sut.Details(1,1);

            // Assert
            var result = Assert.IsType<ViewResult>(response);
            var model = Assert.IsType<UmkDetailsViewModel>(result.Model);
            Assert.Equal(2, model.UmkFiles.Count);
        }

        [Fact]
        public async Task GetDetails_ShouldReturnErrorAlertWhenDenyInAccess()
        {
            // ARRANGE                
            var mockCurriculum = new Mock<ICurriculumQuery>();
            mockCurriculum.Setup(c => c.GetNameById(It.IsAny<int>()))
                .ReturnsAsync(new SPGetName { Name = "Some name" });
            mockCurriculum.Setup(c => c.CheckStudentBySubjectAndTerm(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<byte>()))
                .ReturnsAsync(new SPBoolResult { Result = false });

            var mockRaspredelenie = new Mock<IRaspredelenieQuery>();
            mockRaspredelenie.Setup(r => r.CheckTeacherBySubjectAndTerm(It.IsAny<short>(), It.IsAny<int>(), It.IsAny<byte>()))
                .ReturnsAsync(new SPBoolResult { Result = false });

            var mockContextFactory = SetupContextFactory(null, mockRaspredelenie.Object, mockCurriculum.Object);
            var mockFileService = new Mock<IFileService<UmkFilesConfiguration>>();
            var umkFileService = new UmkFileService(mockContextFactory.Object, _mapper, mockFileService.Object);
            var yearService = new Mock<IYearService>();
            var termService = new Mock<ITermService>();
            var raspredelenieService = new Mock<IRaspredelenieService>();

            _sut = new UmkController(yearService.Object, termService.Object, raspredelenieService.Object, umkFileService);
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
            var response = await _sut.Details(1,1);

            // Assert
            var result = Assert.IsType<AlertDecoratorResult>(response);
            Assert.Equal("danger",result.Type);
        }

        [Fact]
        public async Task GetDetails_ShouldReturnBadRequestWhenNotAuth()
        {
            // Arrange
            var mockContextFactory = SetupContextFactory(null, null, null);
            var mockFileService = new Mock<IFileService<UmkFilesConfiguration>>();
            var umkFileService = new Mock<IUmkFileService>();
            var yearService = new Mock<IYearService>();
            var termService = new Mock<ITermService>();
            var raspredelenieService = new Mock<IRaspredelenieService>();

            _sut = new UmkController(yearService.Object, termService.Object, raspredelenieService.Object, umkFileService.Object);
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, "John Doe"),
                new Claim(ClaimTypes.NameIdentifier, "John Doe"),
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
            var response = await _sut.Details(1, 1);

            // Assert
            Assert.IsType<BadRequestResult>(response);
        }

        [Fact]
        public async Task PostDetails_ShouldReturnSuccessAlert()
        {
            // Arrange
            var mockUmkQuery = new Mock<IUmkFilesQuery>();
            mockUmkQuery
                .Setup(u => u.Create(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<byte>(), It.IsAny<string>(), It.IsAny<double>(), It.IsAny<string>()))
                .ReturnsAsync(new UmkFile { Id = 10, Active = true, CreatedAt = new DateTime(2020, 05, 1), FileName = "FileNameNew", FileSize = 1000, FileType = GetTestFileTypes().First(), Name = "NameNew", SubjectId = 1, TermId = 1, UpdatedAt = new DateTime(2020, 05, 2) });
            var mockRaspredelenie = new Mock<IRaspredelenieQuery>();
            mockRaspredelenie.Setup(r => r.CheckTeacherBySubjectAndTerm(It.IsAny<short>(), It.IsAny<int>(), It.IsAny<byte>()))
                .ReturnsAsync(new SPBoolResult { Result = true });

            var mockContextFactory = SetupContextFactory(mockUmkQuery.Object, mockRaspredelenie.Object, null);

            var mockFormFile = new Mock<IFormFile>();
            mockFormFile.SetupGet(f => f.ContentType).Returns(GetTestFileTypes().First().Type);
            mockFormFile.SetupGet(f => f.FileName).Returns("FileNameNew");

            var mockFileService = new Mock<IFileService<UmkFilesConfiguration>>();
            mockFileService
                .Setup(f => f.UploadAsync(It.IsAny<IFormFile>()))
                .ReturnsAsync("FileNameNew");

            var umkFileService = new UmkFileService(mockContextFactory.Object, _mapper, mockFileService.Object);
            var yearService = new Mock<IYearService>();
            var termService = new Mock<ITermService>();
            var raspredelenieService = new Mock<IRaspredelenieService>();

            _sut = new UmkController(yearService.Object, termService.Object, raspredelenieService.Object, umkFileService);
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
            var request = new CreateRequest { Name = "FileNameNew", SubjectId = 1, TermId = 1, UmkFile = mockFormFile.Object };
            var response = await _sut.Details(request);

            // Assert
            var result = Assert.IsType<AlertDecoratorResult>(response);
            Assert.Equal("success", result.Type);
        }

        [Fact]
        public async Task PostDetails_ShouldReturnDangerAlertWhenFailure()
        {
            // Arrange            
            var mockRaspredelenie = new Mock<IRaspredelenieQuery>();
            mockRaspredelenie.Setup(r => r.CheckTeacherBySubjectAndTerm(It.IsAny<short>(), It.IsAny<int>(), It.IsAny<byte>()))
                .ReturnsAsync(new SPBoolResult { Result = true });

            var mockContextFactory = SetupContextFactory(null, mockRaspredelenie.Object, null);

            var mockFormFile = new Mock<IFormFile>();
            mockFormFile.SetupGet(f => f.ContentType).Returns(GetTestFileTypes().First().Type);
            mockFormFile.SetupGet(f => f.FileName).Returns("FileNameNew");

            var mockFileService = new Mock<IFileService<UmkFilesConfiguration>>();
            mockFileService
                .Setup(f => f.UploadAsync(It.IsAny<IFormFile>()))
                .ThrowsAsync(new Exception("Mock exception"));

            var umkFileService = new UmkFileService(mockContextFactory.Object, _mapper, mockFileService.Object);
            var yearService = new Mock<IYearService>();
            var termService = new Mock<ITermService>();
            var raspredelenieService = new Mock<IRaspredelenieService>();

            _sut = new UmkController(yearService.Object, termService.Object, raspredelenieService.Object, umkFileService);
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
            var request = new CreateRequest { Name = "FileNameNew", SubjectId = 1, TermId = 1, UmkFile = mockFormFile.Object };
            var response = await _sut.Details(request);

            // Assert
            var result = Assert.IsType<AlertDecoratorResult>(response);
            Assert.Equal("danger", result.Type);
        }

        [Fact]
        public async Task PostDetails_ShouldReturnBadRequestWhenNotAuth()
        {
            // Arrange            
            var mockContextFactory = SetupContextFactory(null, null, null);
            var mockFormFile = new Mock<IFormFile>();
            var mockFileService = new Mock<IFileService<UmkFilesConfiguration>>();

            var umkFileService = new UmkFileService(mockContextFactory.Object, _mapper, mockFileService.Object);
            var yearService = new Mock<IYearService>();
            var termService = new Mock<ITermService>();
            var raspredelenieService = new Mock<IRaspredelenieService>();

            _sut = new UmkController(yearService.Object, termService.Object, raspredelenieService.Object, umkFileService);
            var claims = new List<Claim>();
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
            var request = new CreateRequest { Name = "FileNameNew", SubjectId = 1, TermId = 1, UmkFile = mockFormFile.Object };
            var response = await _sut.Details(request);

            // Assert
            Assert.IsType<BadRequestResult>(response);
        }

        [Fact]
        public void Download_ShouldReturnFile()
        {
            // Arrange
            var mockContextFactory = SetupContextFactory(null, null, null);

            var fileService = SetupFileService();

            var umkFileService = new UmkFileService(mockContextFactory.Object, _mapper, fileService);
            var yearService = new Mock<IYearService>();
            var termService = new Mock<ITermService>();
            var raspredelenieService = new Mock<IRaspredelenieService>();

            _sut = new UmkController(yearService.Object, termService.Object, raspredelenieService.Object, umkFileService);

            // Act
            var response = _sut.Download(1);

            // Assert
            var result = Assert.IsAssignableFrom<FileResult>(response);
            Assert.Equal("TestTextFile.txt", result.FileDownloadName);
        }

        [Fact]
        public void Download_ShouldReturnNotFound()
        {
            // Arrange
            var mockContextFactory = SetupContextFactory(null, null, null);

            var fileService = SetupFileService();

            var umkFileService = new UmkFileService(mockContextFactory.Object, _mapper, fileService);
            var yearService = new Mock<IYearService>();
            var termService = new Mock<ITermService>();
            var raspredelenieService = new Mock<IRaspredelenieService>();

            _sut = new UmkController(yearService.Object, termService.Object, raspredelenieService.Object, umkFileService);

            // Act
            var response = _sut.Download(10);

            // Assert
            Assert.IsAssignableFrom<NotFoundResult>(response);
        }

        [Fact]
        public void GetEdit_ShouldReturnView()
        {
            // Arrange
            var mockContextFactory = SetupContextFactory(null, null, null);
            var fileService = SetupFileService();
            var umkFileService = new UmkFileService(mockContextFactory.Object, _mapper, fileService);
            var yearService = new Mock<IYearService>();
            var termService = new Mock<ITermService>();
            var raspredelenieService = new Mock<IRaspredelenieService>();

            _sut = new UmkController(yearService.Object, termService.Object, raspredelenieService.Object, umkFileService);

            // Act
            var response = _sut.Edit(1);

            // Assert
            var result = Assert.IsType<ViewResult>(response);
            var model = Assert.IsType<UmkFile>(result.Model);
        }

        [Fact]
        public void GetEdit_ShouldReturnNotFound()
        {
            // Arrange
            var mockContextFactory = SetupContextFactory(null, null, null);

            var fileService = SetupFileService();

            var umkFileService = new UmkFileService(mockContextFactory.Object, _mapper, fileService);
            var yearService = new Mock<IYearService>();

            var termService = new Mock<ITermService>();
            var raspredelenieService = new Mock<IRaspredelenieService>();

            _sut = new UmkController(yearService.Object, termService.Object, raspredelenieService.Object, umkFileService);

            // Act
            var response = _sut.Edit(10);

            // Assert
            Assert.IsType<NotFoundResult>(response);
        }

        [Fact]
        public async Task PostEdit_ShouldReturnSuccessAlert()
        {
            // Arrange
            var mockUmkQuery = new Mock<IUmkFilesQuery>();
            mockUmkQuery
                .Setup(u => u.Update(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<double>(), It.IsAny<string>()))
                .ReturnsAsync(new UmkFile { Id = 10, Active = true, CreatedAt = new DateTime(2020, 05, 1), FileName = "FileNameNew", FileSize = 1000, FileType = GetTestFileTypes().First(), Name = "NameNew", SubjectId = 1, TermId = 1, UpdatedAt = new DateTime(2020, 05, 2) });

            var mockRaspredelenie = new Mock<IRaspredelenieQuery>();
            mockRaspredelenie.Setup(r => r.CheckTeacherBySubjectAndTerm(It.IsAny<short>(), It.IsAny<int>(), It.IsAny<byte>()))
                .ReturnsAsync(new SPBoolResult { Result = true });

            var mockContextFactory = SetupContextFactory(mockUmkQuery.Object, mockRaspredelenie.Object, null);

            var mockFormFile = new Mock<IFormFile>();
            mockFormFile.SetupGet(f => f.ContentType).Returns(GetTestFileTypes().First().Type);
            mockFormFile.SetupGet(f => f.FileName).Returns("FileNameNew");

            var mockFileService = new Mock<IFileService<UmkFilesConfiguration>>();
            mockFileService
                .Setup(f => f.UploadAsync(It.IsAny<IFormFile>()))
                .ReturnsAsync("FileNameNew");

            var umkFileService = new UmkFileService(mockContextFactory.Object, _mapper, mockFileService.Object);
            var yearService = new Mock<IYearService>();
            var termService = new Mock<ITermService>();
            var raspredelenieService = new Mock<IRaspredelenieService>();

            _sut = new UmkController(yearService.Object, termService.Object, raspredelenieService.Object, umkFileService);
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
            var request = new UpdateRequest { Id = 1, Name = "FileNameNew", UmkFile = mockFormFile.Object };
            var response = await _sut.Edit(request);

            // Assert
            var result = Assert.IsType<AlertDecoratorResult>(response);
            Assert.Equal("success", result.Type);
        }

        [Fact]
        public async Task PostEdit_ShouldReturnDangerAlertWhenFailure()
        {
            // Arrange            
            var mockRaspredelenie = new Mock<IRaspredelenieQuery>();
            mockRaspredelenie.Setup(r => r.CheckTeacherBySubjectAndTerm(It.IsAny<short>(), It.IsAny<int>(), It.IsAny<byte>()))
                .ReturnsAsync(new SPBoolResult { Result = true });

            var mockContextFactory = SetupContextFactory(null, mockRaspredelenie.Object, null);

            var mockFormFile = new Mock<IFormFile>();
            mockFormFile.SetupGet(f => f.ContentType).Returns(GetTestFileTypes().First().Type);
            mockFormFile.SetupGet(f => f.FileName).Returns("FileNameNew");

            var mockFileService = new Mock<IFileService<UmkFilesConfiguration>>();
            mockFileService
                .Setup(f => f.UploadAsync(It.IsAny<IFormFile>()))
                .ThrowsAsync(new Exception("Mock exception"));

            var umkFileService = new UmkFileService(mockContextFactory.Object, _mapper, mockFileService.Object);
            var yearService = new Mock<IYearService>();
            var termService = new Mock<ITermService>();
            var raspredelenieService = new Mock<IRaspredelenieService>();

            _sut = new UmkController(yearService.Object, termService.Object, raspredelenieService.Object, umkFileService);
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
            var request = new UpdateRequest { Id = 1, Name = "FileNameNew", UmkFile = mockFormFile.Object };
            var response = await _sut.Edit(request);

            // Assert
            var result = Assert.IsType<AlertDecoratorResult>(response);
            Assert.Equal("danger", result.Type);
        }

        [Fact]
        public async Task PostEdit_ShouldReturnBadRequestWhenNotAuth()
        {
            // Arrange            
            var mockContextFactory = SetupContextFactory(null, null, null);
            var mockFormFile = new Mock<IFormFile>();
            var mockFileService = new Mock<IFileService<UmkFilesConfiguration>>();

            var umkFileService = new UmkFileService(mockContextFactory.Object, _mapper, mockFileService.Object);
            var yearService = new Mock<IYearService>();
            var termService = new Mock<ITermService>();
            var raspredelenieService = new Mock<IRaspredelenieService>();

            _sut = new UmkController(yearService.Object, termService.Object, raspredelenieService.Object, umkFileService);
            var claims = new List<Claim>();
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
            var request = new UpdateRequest { Id = 1, Name = "FileNameNew", UmkFile = mockFormFile.Object };
            var response = await _sut.Edit(request);

            // Assert
            Assert.IsType<BadRequestResult>(response);
        }

        [Fact]
        public void GetDelete_ShouldReturnViewWithModel()
        {
            // Arrange
            var mockContextFactory = SetupContextFactory(null, null, null);
            var fileService = SetupFileService();
            var umkFileService = new UmkFileService(mockContextFactory.Object, _mapper, fileService);
            var yearService = new Mock<IYearService>();
            var termService = new Mock<ITermService>();
            var raspredelenieService = new Mock<IRaspredelenieService>();

            _sut = new UmkController(yearService.Object, termService.Object, raspredelenieService.Object, umkFileService);

            // Act
            var response = _sut.ConfirmDelete(1);

            // Assert
            var result = Assert.IsType<ViewResult>(response);
            var model = Assert.IsType<UmkFile>(result.Model);
        }

        [Fact]
        public void GetDelete_ShouldReturnNotFound()
        {
            // Arrange
            var mockContextFactory = SetupContextFactory(null, null, null);

            var fileService = SetupFileService();

            var umkFileService = new UmkFileService(mockContextFactory.Object, _mapper, fileService);
            var yearService = new Mock<IYearService>();

            var termService = new Mock<ITermService>();
            var raspredelenieService = new Mock<IRaspredelenieService>();

            _sut = new UmkController(yearService.Object, termService.Object, raspredelenieService.Object, umkFileService);

            // Act
            var response = _sut.ConfirmDelete(10);

            // Assert
            Assert.IsType<NotFoundResult>(response);
        }

        [Fact]
        public async Task PostDelete_ShouldReturnViewWithSuccessAlert()
        {
            // Arrange
            var mockUmkQuery = new Mock<IUmkFilesQuery>();
            mockUmkQuery
                .Setup(u => u.Delete(It.IsAny<int>()))
                .ReturnsAsync(1);

            var mockContextFactory = SetupContextFactory(mockUmkQuery.Object, null, null);
            var fileService = SetupFileService();
            var umkFileService = new UmkFileService(mockContextFactory.Object, _mapper, fileService);
            var yearService = new Mock<IYearService>();
            var termService = new Mock<ITermService>();
            var raspredelenieService = new Mock<IRaspredelenieService>();

            _sut = new UmkController(yearService.Object, termService.Object, raspredelenieService.Object, umkFileService);

            // Act
            var response = await _sut.Delete(1);

            // Assert
            var result = Assert.IsType<AlertDecoratorResult>(response);
            Assert.Equal( "success" , result.Type );
        }

        [Fact]
        public async Task PostDelete_ShouldReturnViewWithDangerAlertWhenNotFound()
        {
            // Arrange
            var mockUmkQuery = new Mock<IUmkFilesQuery>();
            mockUmkQuery
                .Setup(u => u.Delete(It.IsAny<int>()))
                .ReturnsAsync(1);

            var mockContextFactory = SetupContextFactory(mockUmkQuery.Object, null, null);
            var fileService = SetupFileService();
            var umkFileService = new UmkFileService(mockContextFactory.Object, _mapper, fileService);
            var yearService = new Mock<IYearService>();
            var termService = new Mock<ITermService>();
            var raspredelenieService = new Mock<IRaspredelenieService>();

            _sut = new UmkController(yearService.Object, termService.Object, raspredelenieService.Object, umkFileService);

            // Act
            var response = await _sut.Delete(10);

            // Assert
             Assert.IsType<NotFoundResult>(response);
        }

        [Fact]
        public async Task PostDelete_ShouldReturnViewWithDangerAlertWhenFailure()
        {
            // Arrange
            var mockUmkQuery = new Mock<IUmkFilesQuery>();
            mockUmkQuery
                .Setup(u => u.Delete(It.IsAny<int>()))
                .ThrowsAsync(new Exception("Mock exception"));

            var mockContextFactory = SetupContextFactory(mockUmkQuery.Object, null, null);
            var fileService = SetupFileService();
            var umkFileService = new UmkFileService(mockContextFactory.Object, _mapper, fileService);
            var yearService = new Mock<IYearService>();
            var termService = new Mock<ITermService>();
            var raspredelenieService = new Mock<IRaspredelenieService>();

            _sut = new UmkController(yearService.Object, termService.Object, raspredelenieService.Object, umkFileService);

            // Act
            var response = await _sut.Delete(1);

            // Assert
            var result = Assert.IsType<AlertDecoratorResult>(response);
            Assert.Equal("danger", result.Type);
        }
    }
}
