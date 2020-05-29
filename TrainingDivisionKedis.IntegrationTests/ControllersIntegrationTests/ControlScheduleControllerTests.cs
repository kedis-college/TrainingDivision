using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TrainingDivisionKedis.BLL.Common;
using TrainingDivisionKedis.BLL.Contracts;
using TrainingDivisionKedis.BLL.DTO.ControlSchedule;
using TrainingDivisionKedis.BLL.Services;
using TrainingDivisionKedis.Controllers;
using TrainingDivisionKedis.Core.Contracts.Queries;
using TrainingDivisionKedis.Core.Models;
using TrainingDivisionKedis.DAL.ApplicationDbContext;
using TrainingDivisionKedis.DAL.Contracts;
using TrainingDivisionKedis.DAL.Extensions;
using TrainingDivisionKedis.Extensions.Alerts;
using TrainingDivisionKedis.ViewModels.ControlSchedule;
using Xunit;

namespace TrainingDivisionKedis.Tests.ControllersIntegrationTests
{
    public class ControlScheduleControllerTests
    {
        ControlScheduleController _sut;
        readonly IMapper _mapper;

        public ControlScheduleControllerTests()
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

        public static List<TermSeason> GetTestSeasons()
        {
            return new List<TermSeason>
            {
                new TermSeason { Id = 1, Name = "осенний"},
                new TermSeason { Id = 2, Name = "весенний" }
            };
        }

        private static Mock<IAppDbContextFactory> SetupContextFactory(IControlScheduleQuery controlScheduleQuery)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                // don't raise the error warning us that the in memory db doesn't support transactions
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;
            var dbContext = new AppDbContext(options);

            dbContext.TermSeasons.AddRange(GetTestSeasons());
            dbContext.Years.AddRange(GetTestYears());

            dbContext.SaveChanges();

            QueryExtensions.ControlScheduleQueryFactory = context => controlScheduleQuery;

            var mockDbContextFactory = new Mock<IAppDbContextFactory>();
            mockDbContextFactory.Setup(cf => cf.Create()).Returns(dbContext);
            return mockDbContextFactory;
        }

        public static List<ControlSchedule> GetTestControlSchedules()
        {
            return new List<ControlSchedule> {
                new ControlSchedule {Id = 1, DateStart = new DateTime(2020,04,28), YearId = 1, SeasonId = 1},
                new ControlSchedule {Id = 2, DateStart = new DateTime(2020,04,28), YearId = 2, SeasonId = 2 },
                new ControlSchedule {Id = 3, DateStart = new DateTime(2020,04,28), YearId = 3, SeasonId = 1 }
            };
        }

        [Fact]
        public async Task GetIndex_ShouldReturnView()
        {
            // Arrange
            var yearService = new YearService(SetupContextFactory(null).Object);
            var termService = new TermService(SetupContextFactory(null).Object);
            var controlScheduleService = new ControlScheduleService(SetupContextFactory(null).Object, _mapper);

            _sut = new ControlScheduleController(yearService, termService, controlScheduleService);

            // Act
            var response = await _sut.Index();

            // Assert
            var result = Assert.IsType<ViewResult>(response);
            var model = Assert.IsType<CSchIndexViewModel>(result.Model);
            Assert.Equal(GetTestYears().Count, model.Years?.Count);
            Assert.Equal(GetTestSeasons().Count, model.TermSeasons?.Count);
        }

        [Fact]
        public async Task GetIndex_ShouldReturnErrorViewWhenErrorInService()
        {
            // Arrange
            var yearService = new YearService(SetupContextFactory(null).Object);
            var mockTermService = new Mock<ITermService>();
            mockTermService
                .Setup(t => t.GetSeasonsAllAsync())
                .ReturnsAsync(OperationDetails<List<TermSeason>>.Failure("mock exception",""));
            var controlScheduleService = new ControlScheduleService(SetupContextFactory(null).Object, _mapper);

            _sut = new ControlScheduleController(yearService, mockTermService.Object, controlScheduleService);

            // Act
            var response = await _sut.Index();

            // Assert
            var result = Assert.IsType<RedirectToActionResult>(response);
            Assert.Equal("Error", result.ActionName);
        }

        [Fact]
        public async Task PostControlScheduleSearch_ShouldReturnRedirectWhenSuccess()
        {
            // ARRANGE           
            var yearService = new YearService(SetupContextFactory(null).Object);
            var termService = new TermService(SetupContextFactory(null).Object);

            var mockQuery = new Mock<IControlScheduleQuery>();
            mockQuery
                .Setup(tq => tq.GetByYearAndSeason(
                    It.IsAny<byte>(), It.IsAny<byte>()))
                .ReturnsAsync((byte year, byte season) => GetTestControlSchedules().Where(c => c.YearId == year && c.SeasonId == season).ToList());
            var mockContextFactory = SetupContextFactory(mockQuery.Object);
            var controlScheduleService = new ControlScheduleService(mockContextFactory.Object, _mapper);

            _sut = new ControlScheduleController(yearService, termService, controlScheduleService);

            // Act
            var request = new GetByYearAndSeasonRequest(1, 1);
            var response = await _sut.ControlScheduleSearch(request);

            // Assert
            var result = Assert.IsType<PartialViewResult>(response);
            var model = Assert.IsType<List<ControlScheduleListDto>>(result.Model);
            Assert.Single(model);
        }

        [Fact]
        public async Task PostControlScheduleSearch_ShouldReturnRedirectWhenFailure()
        {
            // ARRANGE           
            var yearService = new YearService(SetupContextFactory(null).Object);
            var termService = new TermService(SetupContextFactory(null).Object);

            var mockQuery = new Mock<IControlScheduleQuery>();
            mockQuery
                .Setup(tq => tq.GetByYearAndSeason(
                    It.IsAny<byte>(), It.IsAny<byte>()))
                .ThrowsAsync(new Exception("mock exception"));
            var mockContextFactory = SetupContextFactory(mockQuery.Object);
            var controlScheduleService = new ControlScheduleService(mockContextFactory.Object, _mapper);

            _sut = new ControlScheduleController(yearService, termService, controlScheduleService);

            // Act
            var request = new GetByYearAndSeasonRequest(1, 1);
            var response = await _sut.ControlScheduleSearch(request);

            // Assert
            var result = Assert.IsType<PartialViewResult>(response);
            var model = Assert.IsType<string>(result.Model);
        }

        [Fact]
        public async Task GetCreate_ShouldReturnView()
        {
            // Arrange
            var yearService = new YearService(SetupContextFactory(null).Object);
            var termService = new TermService(SetupContextFactory(null).Object);
            var controlScheduleService = new ControlScheduleService(SetupContextFactory(null).Object, _mapper);

            _sut = new ControlScheduleController(yearService, termService, controlScheduleService);

            // Act
            var response = await _sut.Create();

            // Assert
            var result = Assert.IsType<ViewResult>(response);
            var model = Assert.IsType<CSchCreateViewModel>(result.Model);
            Assert.Equal(GetTestYears().Count, model.Years.Count());
            Assert.Equal(GetTestSeasons().Count, model.TermSeasons.Count());
        }

        [Fact]
        public async Task GetCreate_ShouldReturnRedirectWhenErrorInService()
        {
            // Arrange
            var yearService = new YearService(SetupContextFactory(null).Object);
            var mockTermService = new Mock<ITermService>();
            mockTermService
                .Setup(t => t.GetSeasonsAllAsync())
                .ReturnsAsync(OperationDetails<List<TermSeason>>.Failure("mock exception", ""));
            var controlScheduleService = new ControlScheduleService(SetupContextFactory(null).Object, _mapper);

            _sut = new ControlScheduleController(yearService, mockTermService.Object, controlScheduleService);

            // Act
            var response = await _sut.Create();

            // Assert
            var result = Assert.IsType<RedirectToActionResult>(response);
            Assert.Equal("Error", result.ActionName);
        }

        [Fact]
        public async Task PostCreate_ShouldReturnRedirectWithSuccessAlert()
        {
            // Arrange
            var yearService = new YearService(SetupContextFactory(null).Object);
            var termService = new TermService(SetupContextFactory(null).Object);

            var mockQuery = new Mock<IControlScheduleQuery>();
            mockQuery
                .Setup(tq => tq.Create(
                    It.IsAny<byte>(), It.IsAny<byte>(), It.IsAny<short>(), It.IsAny<DateTime>(),
                    It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(),
                    It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>()))
                .ReturnsAsync((byte a, byte b, short c, DateTime d, DateTime? e, DateTime? f,
                    DateTime? g, DateTime? h, DateTime? i, DateTime? j, DateTime? k) =>
                        new ControlSchedule { Id = 1, YearId = a, SeasonId = b, UserId = c, DateStart = d, DateEnd = e,
                            Mod1DateStart = f, Mod1DateEnd = g, Mod2DateStart = h, Mod2DateEnd = i, ItogDateStart = j, ItogDateEnd = k }
                );
            var controlScheduleService = new ControlScheduleService(SetupContextFactory(mockQuery.Object).Object, _mapper);

            _sut = new ControlScheduleController(yearService, termService, controlScheduleService);
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
            var request = new CSchCreateViewModel { ControlSchedule = new ControlScheduleDto { YearId = 1, SeasonId = 1, DateStart = new DateTime(2020, 05, 17) } };
            var response = await _sut.Create(request);

            // Assert
            var result = Assert.IsType<AlertDecoratorResult>(response);
            Assert.Equal("success", result.Type);
        }

        [Fact]
        public async Task PostCreate_ShouldReturnViewWithDangerAlert()
        {
            // Arrange
            var yearService = new YearService(SetupContextFactory(null).Object);
            var termService = new TermService(SetupContextFactory(null).Object);

            var mockQuery = new Mock<IControlScheduleQuery>();
            mockQuery
                .Setup(tq => tq.Create(
                    It.IsAny<byte>(), It.IsAny<byte>(), It.IsAny<short>(), It.IsAny<DateTime>(),
                    It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(),
                    It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>()))
                .ThrowsAsync(new Exception("Mock exception"));
            var controlScheduleService = new ControlScheduleService(SetupContextFactory(mockQuery.Object).Object, _mapper);

            _sut = new ControlScheduleController(yearService, termService, controlScheduleService);
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
            var request = new CSchCreateViewModel { ControlSchedule = new ControlScheduleDto { YearId = 1, SeasonId = 1, DateStart = new DateTime(2020, 05, 17) } };
            var response = await _sut.Create(request);

            // Assert
            var result = Assert.IsType<AlertDecoratorResult>(response);
            Assert.Equal("danger", result.Type);
        }

        [Fact]
        public async Task PostCreate_ShouldReturnBadRequestWhenNotAuth()
        {
            // Arrange
            var yearService = new YearService(SetupContextFactory(null).Object);
            var termService = new TermService(SetupContextFactory(null).Object);
            var controlScheduleService = new ControlScheduleService(SetupContextFactory(null).Object, _mapper);

            _sut = new ControlScheduleController(yearService, termService, controlScheduleService);
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

            // Act
            var request = new CSchCreateViewModel { ControlSchedule = new ControlScheduleDto { YearId = 1, SeasonId = 1, DateStart = new DateTime(2020, 05, 17) } };
            var response = await _sut.Create(request);

            // Assert
            Assert.IsType<BadRequestResult>(response);
        }

        [Fact]
        public async Task PostCreate_ShouldReturnViewWhenModelIsNotValid()
        {
            // Arrange
            var yearService = new YearService(SetupContextFactory(null).Object);
            var termService = new TermService(SetupContextFactory(null).Object);
            var controlScheduleService = new ControlScheduleService(SetupContextFactory(null).Object, _mapper);

            _sut = new ControlScheduleController(yearService, termService, controlScheduleService);
            _sut.ModelState.AddModelError("DateStart", "Required");

            // Act
            var request = new CSchCreateViewModel { ControlSchedule = new ControlScheduleDto { YearId = 1, SeasonId = 1, DateStart = null } };
            var response = await _sut.Create(request);

            // Assert
            var result = Assert.IsType<ViewResult>(response);
            var model = Assert.IsType<CSchCreateViewModel>(result.Model);
            Assert.Equal(ComparableObject.Convert(request), ComparableObject.Convert(model));
        }

        [Fact]
        public async Task PostCreate_ShouldReturnErrorWhenModelIsNotValidAndErrorInService()
        {
            // Arrange
            var yearService = new YearService(SetupContextFactory(null).Object);
            var mockTermService = new Mock<ITermService>();
            mockTermService
                .Setup(t => t.GetSeasonsAllAsync())
                .ReturnsAsync(OperationDetails<List<TermSeason>>.Failure("mock exception", ""));
            var controlScheduleService = new ControlScheduleService(SetupContextFactory(null).Object, _mapper);

            _sut = new ControlScheduleController(yearService, mockTermService.Object, controlScheduleService);
            _sut.ModelState.AddModelError("DateStart", "Required");

            // Act
            var request = new CSchCreateViewModel { ControlSchedule = new ControlScheduleDto { YearId = 1, SeasonId = 1, DateStart = null } };
            var response = await _sut.Create(request);

            // Assert
            var result = Assert.IsType<RedirectToActionResult>(response);
            Assert.Equal("Error", result.ActionName);
        }

        [Fact]
        public async Task GetEdit_ShouldReturnView()
        {
            // Arrange
            var yearService = new YearService(SetupContextFactory(null).Object);
            var termService = new TermService(SetupContextFactory(null).Object);

            var mockQuery = new Mock<IControlScheduleQuery>();
            mockQuery
               .Setup(tq => tq.GetById(It.IsAny<int>()))
               .ReturnsAsync((int id) => GetTestControlSchedules().FirstOrDefault(c => c.Id == id));
            var controlScheduleService = new ControlScheduleService(SetupContextFactory(mockQuery.Object).Object, _mapper);

            _sut = new ControlScheduleController(yearService, termService, controlScheduleService);

            var expectedDto = _mapper.Map<ControlScheduleDto>(GetTestControlSchedules().First());
            expectedDto.Year = GetTestYears().First();
            expectedDto.Season = GetTestSeasons().First();

            // Act
            var response = await _sut.Edit(1);

            // Assert
            var result = Assert.IsType<ViewResult>(response);
            Assert.Equal(ComparableObject.Convert(expectedDto), ComparableObject.Convert(result.Model));
        }

        [Fact]
        public async Task GetEdit_ShouldReturnNotFoundResultWhenError()
        {
            // Arrange
            var yearService = new YearService(SetupContextFactory(null).Object);
            var termService = new TermService(SetupContextFactory(null).Object);

            var mockQuery = new Mock<IControlScheduleQuery>();
            mockQuery
               .Setup(tq => tq.GetById(It.IsAny<int>()))
               .ReturnsAsync((int id) => GetTestControlSchedules().FirstOrDefault(c => c.Id == id));
            var controlScheduleService = new ControlScheduleService(SetupContextFactory(mockQuery.Object).Object, _mapper);

            _sut = new ControlScheduleController(yearService, termService, controlScheduleService);

            var expectedDto = _mapper.Map<ControlScheduleDto>(GetTestControlSchedules().First());
            expectedDto.Year = GetTestYears().First();
            expectedDto.Season = GetTestSeasons().First();

            // Act
            var response = await _sut.Edit(10);

            // Assert
            Assert.IsType<NotFoundResult>(response);
        }

        [Fact]
        public async Task PostEdit_ShouldReturnSuccessAlertWhenSucceeded()
        {
            var yearService = new YearService(SetupContextFactory(null).Object);
            var termService = new TermService(SetupContextFactory(null).Object);

            var mockQuery = new Mock<IControlScheduleQuery>();
            mockQuery
                .Setup(tq => tq.Update(
                    It.IsAny<int>(), It.IsAny<short>(), It.IsAny<DateTime>(),
                    It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(),
                    It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>()))
                .ReturnsAsync((int a, short c, DateTime d, DateTime? e, DateTime? f,
                    DateTime? g, DateTime? h, DateTime? i, DateTime? j, DateTime? k) =>
                        new ControlSchedule
                        {
                            Id = a,
                            YearId = 1,
                            SeasonId = 1,
                            UserId = c,
                            DateStart = d,
                            DateEnd = e,
                            Mod1DateStart = f,
                            Mod1DateEnd = g,
                            Mod2DateStart = h,
                            Mod2DateEnd = i,
                            ItogDateStart = j,
                            ItogDateEnd = k
                        }
                );
            var controlScheduleService = new ControlScheduleService(SetupContextFactory(mockQuery.Object).Object, _mapper);

            _sut = new ControlScheduleController(yearService, termService, controlScheduleService);
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
            var request = new ControlScheduleDto { YearId = 1, SeasonId = 1, DateStart = new DateTime(2020, 05, 17), DateEnd = new DateTime(2020,05,18) };
            var response = await _sut.Edit(request);

            // Assert
            var result = Assert.IsType<AlertDecoratorResult>(response);
            Assert.Equal("success", result.Type);
        }

        [Fact]
        public async Task PostEdit_ShouldReturnRedirectWithDangerAlertWhenErrorInService()
        {
            var yearService = new YearService(SetupContextFactory(null).Object);
            var termService = new TermService(SetupContextFactory(null).Object);

            var mockQuery = new Mock<IControlScheduleQuery>();
            mockQuery
                .Setup(tq => tq.Update(
                    It.IsAny<int>(), It.IsAny<short>(), It.IsAny<DateTime>(),
                    It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(),
                    It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>()))
                .ThrowsAsync(new Exception("Mock exception"));
            var controlScheduleService = new ControlScheduleService(SetupContextFactory(mockQuery.Object).Object, _mapper);

            _sut = new ControlScheduleController(yearService, termService, controlScheduleService);
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
            var request = new ControlScheduleDto { YearId = 1, SeasonId = 1, DateStart = new DateTime(2020, 05, 17), DateEnd = new DateTime(2020, 05, 18) };
            var response = await _sut.Edit(request);

            // Assert
            var alertResult = Assert.IsType<AlertDecoratorResult>(response);
            Assert.Equal("danger", alertResult.Type);

            var redirectResult = Assert.IsType<RedirectToActionResult>(alertResult.Result);
            Assert.Equal("Edit", redirectResult.ActionName);
        }

        [Fact]
        public async Task PostEdit_ShouldReturnBadRequestWhenNotAuth()
        {
            var yearService = new YearService(SetupContextFactory(null).Object);
            var termService = new TermService(SetupContextFactory(null).Object);
            var controlScheduleService = new ControlScheduleService(SetupContextFactory(null).Object, _mapper);

            _sut = new ControlScheduleController(yearService, termService, controlScheduleService);
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
            var request = new ControlScheduleDto { YearId = 1, SeasonId = 1, DateStart = new DateTime(2020, 05, 17), DateEnd = new DateTime(2020, 05, 18) };
            var response = await _sut.Edit(request);

            // Assert
            Assert.IsType<BadRequestResult>(response);
        }
    }
}       
    