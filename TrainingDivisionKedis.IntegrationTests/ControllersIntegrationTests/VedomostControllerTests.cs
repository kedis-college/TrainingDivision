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
using TrainingDivisionKedis.BLL.DTO.ProgressInStudy;
using TrainingDivisionKedis.BLL.DTO.Raspredelenie;
using TrainingDivisionKedis.BLL.Services;
using TrainingDivisionKedis.Controllers;
using TrainingDivisionKedis.Core.Contracts.Queries;
using TrainingDivisionKedis.Core.Models;
using TrainingDivisionKedis.Core.SPModels.ControlSchedule;
using TrainingDivisionKedis.Core.SPModels.ProgressInStudy;
using TrainingDivisionKedis.Core.SPModels.Raspredelenie;
using TrainingDivisionKedis.DAL.ApplicationDbContext;
using TrainingDivisionKedis.DAL.Contracts;
using TrainingDivisionKedis.DAL.Extensions;
using TrainingDivisionKedis.Extensions.Alerts;
using TrainingDivisionKedis.ViewModels.Vedomost;
using Xunit;

namespace TrainingDivisionKedis.Tests.ControllersIntegrationTests
{
    public class VedomostControllerTests
    {
        VedomostController _sut;
        readonly IMapper _mapper;

        public VedomostControllerTests()
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

        public static List<SPProgressInStudyGetByRaspredelenieAndUser> GetTestProgressInStudy()
        {
            return new List<SPProgressInStudyGetByRaspredelenieAndUser> {
                new SPProgressInStudyGetByRaspredelenieAndUser { NumRec = 1, Student = 1, Subject = 1, Term = 1, Mod1 = 20, Mod2 = 25, Itog = 35, Dop = 0, Prize = 4, Ball = 80, Date = new DateTime(2018,10,20),GroupName = "GroupName1",SpecialityName = "SpecialityName1", StudentFio = "StudentFio1", StudentCode = "123456", GroupId = 1, PrizeName = "хорошо", SubjectName = "Subject1"},
                new SPProgressInStudyGetByRaspredelenieAndUser { NumRec = 2, Student = 1, Subject = 2, Term = 2, Mod1 = 25, Mod2 = 20, Itog = 30, Dop = 0, Prize = 3, Ball = 75, Date = new DateTime(2019,10,20),GroupName = "GroupName1",SpecialityName = "SpecialityName1", StudentFio = "StudentFio1", StudentCode = "123456", GroupId = 1, PrizeName = "хорошо", SubjectName = "Subject2"},
                new SPProgressInStudyGetByRaspredelenieAndUser { NumRec = 3, Student = 1, Subject = 3, Term = 1, Mod1 = 30, Mod2 = 25, Itog = 40, Dop = 5, Prize = 5, Ball = 100, Date = new DateTime(2018,10,20),GroupName = "GroupName1",SpecialityName = "SpecialityName1", StudentFio = "StudentFio1", StudentCode = "123456", GroupId = 1, PrizeName = "хорошо", SubjectName = "Subject3"},
                new SPProgressInStudyGetByRaspredelenieAndUser { NumRec = 4, Student = 2, Subject = 1, Term = 1, Mod1 = 15, Mod2 = 30, Itog = 35, Dop = 0, Prize = 4, Ball = 80, Date = new DateTime(2018,10,20),GroupName = "GroupName1",SpecialityName = "SpecialityName1", StudentFio = "StudentFio2", StudentCode = "12345", GroupId = 1, PrizeName = "хорошо", SubjectName = "Subject1"},
                new SPProgressInStudyGetByRaspredelenieAndUser { NumRec = 5, Student = 2, Subject = 2, Term = 1, Mod1 = 20, Mod2 = 20, Itog = 30, Dop = 5, Prize = 3, Ball = 75, Date = new DateTime(2018,10,20),GroupName = "GroupName1",SpecialityName = "SpecialityName1", StudentFio = "StudentFio2", StudentCode = "12345", GroupId = 1, PrizeName = "хорошо", SubjectName = "Subject2"}
            };
        }

        public static List<SPProgressInStudyGetTotals> GetTestTotals()
        {
            return new List<SPProgressInStudyGetTotals>
            {
                new SPProgressInStudyGetTotals { Id = 1, Name = "всего", Value = 1 },
                new SPProgressInStudyGetTotals { Id = 2, Name = "неявка", Value = 2 },
                new SPProgressInStudyGetTotals { Id = 3, Name = "отлично", Value = 3 },
                new SPProgressInStudyGetTotals { Id = 3, Name = "хорошо", Value = 3 },
                new SPProgressInStudyGetTotals { Id = 3, Name = "удовл.", Value = 3 },
                new SPProgressInStudyGetTotals { Id = 3, Name = "неуд.", Value = 3 }
            };
        }

        private static List<SPRaspredelenieGetByYearAndTermAndUser> GetTestVedomost()
        {
            return new List<SPRaspredelenieGetByYearAndTermAndUser>
            {
                new SPRaspredelenieGetByYearAndTermAndUser { Semestr = 1, SubjectId = 1, SubjectName = "Subject1", YearId = 1, Id = 1, GroupId = 1 },
                new SPRaspredelenieGetByYearAndTermAndUser { Semestr = 1, SubjectId = 2, SubjectName = "Subject2", YearId = 1, Id = 2, GroupId = 2 },
                new SPRaspredelenieGetByYearAndTermAndUser { Semestr = 1, SubjectId = 3, SubjectName = "Subject3", YearId = 1, Id = 3, GroupId = 3 }
            };
        }

        public static IOptions<ReportsConfiguration> SetupReportConfiguration(string baseDirectory = @"C:\Users\E7450\source\repos\TrainingDivision\TrainingDivisionKedis.BLL.Tests\bin\Debug\netcoreapp2.2\ReportTemplates\", string vedomostTemplate = @"VedomostTemplate.docx")
        {
            var reportsConfiguration = new ReportsConfiguration { BaseDirectory = baseDirectory, VedomostTemplate = vedomostTemplate };
            return Options.Create(reportsConfiguration);
        }

        private static Mock<IAppDbContextFactory> SetupContextFactory(IProgressInStudyQuery progressInStudyQuery, IRaspredelenieQuery raspredelenieQuery, IControlScheduleQuery controlScheduleQuery)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                // don't raise the error warning us that the in memory db doesn't support transactions
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;
            var dbContext = new AppDbContext(options);

            dbContext.Terms.AddRange(GetTestTerms());
            dbContext.Years.AddRange(GetTestYears());
            dbContext.DirectorNames.Add(new DirectorName { Nom = 1, Director = "Director", NachUchChasti = "NachUchChasti", ZamDirector = "ZamDirector" });
            dbContext.SaveChanges();

            QueryExtensions.ProgressInStudyQueryFactory = context => progressInStudyQuery;
            QueryExtensions.RaspredelenieQueryFactory = context => raspredelenieQuery;
            QueryExtensions.ControlScheduleQueryFactory = context => controlScheduleQuery;

            var mockDbContextFactory = new Mock<IAppDbContextFactory>();
            mockDbContextFactory.Setup(cf => cf.Create()).Returns(dbContext);
            return mockDbContextFactory;
        }

        [Fact]
        public async Task GetIndex_ShouldReturnViewWithModel()
        {
            var yearService = new YearService(SetupContextFactory(null, null, null).Object);
            var termService = new TermService(SetupContextFactory(null, null, null).Object);
            var raspredelenieService = new Mock<IRaspredelenieService>();
            var progressInStudyService = new Mock<IProgressInStudyService>();
            var reportService = new Mock<IVedomostReportService>();

            _sut = new VedomostController(yearService, termService, raspredelenieService.Object, progressInStudyService.Object, reportService.Object);

            // Act
            var response = await _sut.Index();

            // Assert
            var result = Assert.IsType<ViewResult>(response);
            var model = Assert.IsType<VedIndexViewModel>(result.Model);
            Assert.Equal(GetTestYears().Count, model.Years?.Count);
            Assert.Equal(GetTestTerms().Count, model.Terms?.Count);
        }

        [Fact]
        public async Task GetIndex_ShouldReturnErrorViewWhenErrorInService()
        {
            // Arrange
            var yearService = new YearService(SetupContextFactory(null, null, null).Object);
            var mockTermService = new Mock<ITermService>();
            mockTermService
                .Setup(t => t.GetAllAsync())
                .ReturnsAsync(OperationDetails<List<Term>>.Failure("mock exception", ""));
            var raspredelenieService = new Mock<IRaspredelenieService>();
            var progressInStudyService = new Mock<IProgressInStudyService>();
            var reportService = new Mock<IVedomostReportService>();

            _sut = new VedomostController(yearService, mockTermService.Object, raspredelenieService.Object, progressInStudyService.Object, reportService.Object);

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
                .Setup(cq => cq.GetByYearAndTermAndUser(It.IsAny<byte>(), It.IsAny<byte>(), It.IsAny<int>()))
                .ReturnsAsync(GetTestVedomost());

            var yearService = new Mock<IYearService>();
            var termService = new Mock<ITermService>();
            var progressInStudyService = new Mock<IProgressInStudyService>();
            var raspredelenieService = new RaspredelenieService(SetupContextFactory(null, mockRaspredelenie.Object, null).Object);
            var reportService = new Mock<IVedomostReportService>();

            _sut = new VedomostController(yearService.Object, termService.Object, raspredelenieService, progressInStudyService.Object, reportService.Object);
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
            var response = await _sut.VedomostSearch(request);

            // Assert
            var result = Assert.IsType<PartialViewResult>(response);
            var model = Assert.IsType<List<SPRaspredelenieGetByYearAndTermAndUser>>(result.Model);
            Assert.Equal(GetTestVedomost().Count, model.Count);
        }

        [Fact]
        public async Task PostSubjectsSearch_ShouldReturnMessagePartialWhenFailure()
        {
            // ARRANGE     
            var mockRaspredelenie = new Mock<IRaspredelenieQuery>();
            mockRaspredelenie
                .Setup(cq => cq.GetByYearAndTermAndUser(It.IsAny<byte>(), It.IsAny<byte>(), It.IsAny<int>()))
                .ThrowsAsync(new Exception("Mock exception"));

            var yearService = new Mock<IYearService>();
            var termService = new Mock<ITermService>();
            var progressInStudyService = new Mock<IProgressInStudyService>();
            var raspredelenieService = new RaspredelenieService(SetupContextFactory(null, mockRaspredelenie.Object, null).Object);
            var reportService = new Mock<IVedomostReportService>();

            _sut = new VedomostController(yearService.Object, termService.Object, raspredelenieService, progressInStudyService.Object, reportService.Object);
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
            var response = await _sut.VedomostSearch(request);

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
            var progressInStudyService = new Mock<IProgressInStudyService>();
            var raspredelenieService = new Mock<IRaspredelenieService>();
            var reportService = new Mock<IVedomostReportService>();

            _sut = new VedomostController(yearService.Object, termService.Object, raspredelenieService.Object, progressInStudyService.Object, reportService.Object);
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
            var response = await _sut.VedomostSearch(request);

            // Assert
            var result = Assert.IsType<PartialViewResult>(response);
            var model = Assert.IsType<string>(result.Model);
            Assert.Equal("Ошибка в авторизации", model);
        }

        [Fact]
        public async Task GetEdit_ShouldReturnView()
        {
            // Arrange
            var mockPrQuery = new Mock<IProgressInStudyQuery>();
            mockPrQuery.Setup(p => p.GetByRaspredelenieAndUser(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(GetTestProgressInStudy());

            var mockConQuery = new Mock<IControlScheduleQuery>();
            mockConQuery.Setup(p => p.GetEditable(It.IsAny<int>()))
                .ReturnsAsync(new SPControlScheduleGetEditable { Mod1 = false, Mod2 = false, Itog = true });

            var mockContextFactory = SetupContextFactory(mockPrQuery.Object, null, mockConQuery.Object);
            var yearService = new Mock<IYearService>();
            var termService = new Mock<ITermService>();
            var raspredelenieService = new Mock<IRaspredelenieService>();
            var reportService = new Mock<IVedomostReportService>();
            var progressInStudyService = new ProgressInStudyService(mockContextFactory.Object, _mapper);

            _sut = new VedomostController(yearService.Object, termService.Object, raspredelenieService.Object, progressInStudyService, reportService.Object);
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
            var response = await _sut.Edit(1);

            // Assert
            var result = Assert.IsType<ViewResult>(response);
            var model = Assert.IsType<VedDetailsViewModel>(result.Model);
        }

        [Fact]
        public async Task GetEdit_ShouldReturnNotFoundWhenFailure()
        {
            // Arrange
            var mockPrQuery = new Mock<IProgressInStudyQuery>();
            mockPrQuery.Setup(p => p.GetByRaspredelenieAndUser(It.IsAny<int>(), It.IsAny<int>()))
                .ThrowsAsync(new Exception("Mock exception"));

            var mockContextFactory = SetupContextFactory(mockPrQuery.Object, null, null);
            var progressInStudyService = new ProgressInStudyService(mockContextFactory.Object, _mapper);
            var yearService = new Mock<IYearService>();
            var termService = new Mock<ITermService>();
            var raspredelenieService = new Mock<IRaspredelenieService>();
            var reportService = new Mock<IVedomostReportService>();

            _sut = new VedomostController(yearService.Object, termService.Object, raspredelenieService.Object, progressInStudyService, reportService.Object);
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
            var response = await _sut.Edit(10);

            // Assert
            Assert.IsType<NotFoundResult>(response);
        }

        [Fact]
        public async Task GetEdit_ShouldReturnNotFoundWhenProgressInStudyArrayIsEmpty()
        {
            // Arrange
            var mockPrQuery = new Mock<IProgressInStudyQuery>();
            mockPrQuery.Setup(p => p.GetByRaspredelenieAndUser(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(new List<SPProgressInStudyGetByRaspredelenieAndUser>());

            var mockContextFactory = SetupContextFactory(mockPrQuery.Object, null, null);
            var progressInStudyService = new ProgressInStudyService(mockContextFactory.Object, _mapper);
            var yearService = new Mock<IYearService>();
            var termService = new Mock<ITermService>();
            var raspredelenieService = new Mock<IRaspredelenieService>();
            var reportService = new Mock<IVedomostReportService>();

            _sut = new VedomostController(yearService.Object, termService.Object, raspredelenieService.Object, progressInStudyService, reportService.Object);
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
            var response = await _sut.Edit(10);

            // Assert
            Assert.IsType<NotFoundResult>(response);
        }

        [Fact]
        public async Task GetEdit_ShouldReturnBadRequestWhenNotAuth()
        {
            // Arrange
            var mockContextFactory = SetupContextFactory(null, null, null);
            var progressInStudyService = new ProgressInStudyService(mockContextFactory.Object, _mapper);
            var yearService = new Mock<IYearService>();
            var termService = new Mock<ITermService>();
            var raspredelenieService = new Mock<IRaspredelenieService>();
            var reportService = new Mock<IVedomostReportService>();

            _sut = new VedomostController(yearService.Object, termService.Object, raspredelenieService.Object, progressInStudyService, reportService.Object);
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
            var response = await _sut.Edit(10);

            // Assert
            Assert.IsType<BadRequestResult>(response);
        }

        [Fact]
        public async Task PutEdit_ShouldReturnJsonSuccess()
        {
            // Arrange
            var mockPrQuery = new Mock<IProgressInStudyQuery>();
            mockPrQuery.Setup(p => p.Update(It.IsAny<int>(), It.IsAny<byte>(), It.IsAny<byte>(), It.IsAny<byte>(), It.IsAny<byte>(), It.IsAny<short>(), It.IsAny<DateTime?>()))
                .ReturnsAsync(GetTestProgressInStudy().First());

            var mockContextFactory = SetupContextFactory(mockPrQuery.Object, null, null);
            var yearService = new Mock<IYearService>();
            var termService = new Mock<ITermService>();
            var raspredelenieService = new Mock<IRaspredelenieService>();
            var reportService = new Mock<IVedomostReportService>();
            var progressInStudyService = new ProgressInStudyService(mockContextFactory.Object, _mapper);

            _sut = new VedomostController(yearService.Object, termService.Object, raspredelenieService.Object, progressInStudyService, reportService.Object);
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
            var request = new ProgressInStudyUpdateRequest { NumRec = 1, Mod1 = 30, Mod2 = 30, Itog = 30, Dop = 5, Date = new DateTime(2020, 05, 1), UserId = 1 };
            var response = await _sut.Edit(request);

            // Assert
            var result = Assert.IsType<JsonResult>(response);
            var model = Assert.IsType<OperationDetails<SPProgressInStudyGetByRaspredelenieAndUser>>(result.Value);
            Assert.True(model.Succedeed);
        }

        [Fact]
        public async Task PutEdit_ShouldReturnDangerAlertWhenFailure()
        {
            // Arrange
            var mockPrQuery = new Mock<IProgressInStudyQuery>();
            mockPrQuery.Setup(p => p.Update(It.IsAny<int>(), It.IsAny<byte>(), It.IsAny<byte>(), It.IsAny<byte>(), It.IsAny<byte>(), It.IsAny<short>(), It.IsAny<DateTime?>()))
                .ThrowsAsync(new Exception("Mock exception"));

            var mockContextFactory = SetupContextFactory(mockPrQuery.Object, null, null);
            var yearService = new Mock<IYearService>();
            var termService = new Mock<ITermService>();
            var raspredelenieService = new Mock<IRaspredelenieService>();
            var reportService = new Mock<IVedomostReportService>();
            var progressInStudyService = new ProgressInStudyService(mockContextFactory.Object, _mapper);

            _sut = new VedomostController(yearService.Object, termService.Object, raspredelenieService.Object, progressInStudyService, reportService.Object);
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
            var request = new ProgressInStudyUpdateRequest { NumRec = 1, Mod1 = 30, Mod2 = 30, Itog = 30, Dop = 5, Date = new DateTime(2020, 05, 1), UserId = 1 };
            var response = await _sut.Edit(request);

            // Assert
            var result = Assert.IsType<AlertDecoratorResult>(response);
            Assert.Equal("Mock exception", result.Body);
        }

        [Fact]
        public async Task PutEdit_ShouldReturnDangerAlertWhenNotAuth()
        {
            // Arrange
            var mockContextFactory = SetupContextFactory(null, null, null);
            var yearService = new Mock<IYearService>();
            var termService = new Mock<ITermService>();
            var raspredelenieService = new Mock<IRaspredelenieService>();
            var reportService = new Mock<IVedomostReportService>();
            var progressInStudyService = new ProgressInStudyService(mockContextFactory.Object, _mapper);

            _sut = new VedomostController(yearService.Object, termService.Object, raspredelenieService.Object, progressInStudyService, reportService.Object);
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
            var request = new ProgressInStudyUpdateRequest { NumRec = 1, Mod1 = 30, Mod2 = 30, Itog = 30, Dop = 5, Date = new DateTime(2020, 05, 1), UserId = 1 };
            var response = await _sut.Edit(request);

            // Assert
            var result = Assert.IsType<AlertDecoratorResult>(response);
            Assert.Equal("Ошибка в авторизации", result.Body);
        }

        [Fact]
        public void GetPrizesTotal_ShouldReturnViewComponent()
        {
            // Arrange
            var mockContextFactory = SetupContextFactory(null, null, null);
            var yearService = new Mock<IYearService>();
            var termService = new Mock<ITermService>();
            var raspredelenieService = new Mock<IRaspredelenieService>();
            var reportService = new Mock<IVedomostReportService>();
            var progressInStudyService = new ProgressInStudyService(mockContextFactory.Object, _mapper);

            _sut = new VedomostController(yearService.Object, termService.Object, raspredelenieService.Object, progressInStudyService, reportService.Object);
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
            var response = _sut.GetPrizesTotal(1);

            // Assert
            var result = Assert.IsType<ViewComponentResult>(response);
        }

        [Fact]
        public async Task GetReport_ShouldReturnFile()
        {
            // Arrange
            var mockPrQuery = new Mock<IProgressInStudyQuery>();
            mockPrQuery.Setup(p => p.GetByRaspredelenie(It.IsAny<int>()))
                .ReturnsAsync(GetTestProgressInStudy());
            mockPrQuery
                .Setup(p => p.GetTotalsByRaspredelenie(It.IsAny<int>()))
                .ReturnsAsync(GetTestTotals());

            var mockContextFactory = SetupContextFactory(mockPrQuery.Object, null, null);
            var yearService = new Mock<IYearService>();
            var termService = new Mock<ITermService>();
            var raspredelenieService = new Mock<IRaspredelenieService>();
            var progressInStudyService = new Mock<IProgressInStudyService>();

            var reportConfiguration = SetupReportConfiguration();
            var reportService = new VedomostReportService(mockContextFactory.Object, reportConfiguration);

            _sut = new VedomostController(yearService.Object, termService.Object, raspredelenieService.Object, progressInStudyService.Object, reportService);

            // Act           
            var response = await _sut.GetReport(1);

            // Assert
            var result = Assert.IsAssignableFrom<FileResult>(response);
            Assert.Equal("application/vnd.openxmlformats-officedocument.wordprocessingml.document", result.ContentType);
        }

        [Fact]
        public async Task GetReport_ShouldReturnBadRequestWhenFailure()
        {
            // Arrange
            var mockPrQuery = new Mock<IProgressInStudyQuery>();
            mockPrQuery.Setup(p => p.GetByRaspredelenie(It.IsAny<int>()))
                .ReturnsAsync(new List<SPProgressInStudyGetByRaspredelenieAndUser>());
            mockPrQuery
                .Setup(p => p.GetTotalsByRaspredelenie(It.IsAny<int>()))
                .ReturnsAsync(GetTestTotals());

            var mockContextFactory = SetupContextFactory(mockPrQuery.Object, null, null);
            var yearService = new Mock<IYearService>();
            var termService = new Mock<ITermService>();
            var raspredelenieService = new Mock<IRaspredelenieService>();
            var progressInStudyService = new Mock<IProgressInStudyService>();

            var reportConfiguration = SetupReportConfiguration();
            var reportService = new VedomostReportService(mockContextFactory.Object, reportConfiguration);

            _sut = new VedomostController(yearService.Object, termService.Object, raspredelenieService.Object, progressInStudyService.Object, reportService);

            // Act           
            var response = await _sut.GetReport(1);

            // Assert
            Assert.IsType<BadRequestResult>(response);
        }
    }
}
