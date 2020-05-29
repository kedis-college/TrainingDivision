using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrainingDivisionKedis.BLL.Common;
using TrainingDivisionKedis.BLL.DTO.ControlSchedule;
using TrainingDivisionKedis.BLL.Services;
using TrainingDivisionKedis.Core.Models;
using TrainingDivisionKedis.DAL.ApplicationDbContext;
using TrainingDivisionKedis.DAL.Contracts;
using TrainingDivisionKedis.Core.Contracts.Queries;
using TrainingDivisionKedis.DAL.Extensions;
using Xunit;

namespace TrainingDivisionKedis.BLL.Tests
{
    public class ControlScheduleServiceTests
    {
        public ControlScheduleService _sut;
        public IMapper _mapper;

        public ControlScheduleServiceTests()
        {
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfileBLL());
            });
            _mapper = mappingConfig.CreateMapper();
        }

        public static List<ControlSchedule> GetTestControlSchedules()
        {
            return new List<ControlSchedule> {
                new ControlSchedule {Id = 1, DateStart = new DateTime(2020,04,28), YearId = 1, SeasonId = 1},
                new ControlSchedule {Id = 2, DateStart = new DateTime(2020,04,28), YearId = 2, SeasonId = 2 },
                new ControlSchedule {Id = 3, DateStart = new DateTime(2020,04,28), YearId = 3, SeasonId = 1 }
            };
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

        private static Mock<IAppDbContextFactory> SetupContextFactory(IControlScheduleQuery conScheduleQuery)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var dbContext = new AppDbContext(options);           

            dbContext.Years.AddRange(GetTestYears());
            dbContext.TermSeasons.AddRange(GetTestSeasons());
            dbContext.SaveChanges();

            QueryExtensions.ControlScheduleQueryFactory = context => conScheduleQuery;

            var mockDbContextFactory = new Mock<IAppDbContextFactory>();
            mockDbContextFactory.Setup(cf => cf.Create()).Returns(dbContext);
            return mockDbContextFactory;
        }

        [Fact]
        public void Constructor_ShouldThrowExceptionWhenNullArgument1()
        {
            // ASSERT
            Assert.Throws<ArgumentNullException>(() => { var _sut = new ControlScheduleService(null,_mapper); });
        }

        [Fact]
        public void Constructor_ShouldThrowExceptionWhenNullArgument2()
        {
            // ARRANGE
            var dbContext = new Mock<IAppDbContextFactory>();
            // ASSERT
            Assert.Throws<ArgumentNullException>(() => { var _sut = new ControlScheduleService(dbContext.Object, null); });
        }

        [Fact]
        public async Task Create_ShouldReturnDto()
        {
            // ARRANGE
            var mockQuery  = new Mock<IControlScheduleQuery>();
            mockQuery
                .Setup(tq => tq.Create(
                    It.IsAny<byte>(), It.IsAny<byte>(), It.IsAny<short>(), It.IsAny<DateTime>(),
                    It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(),
                    It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>()))
                .ReturnsAsync((byte a, byte b, short c, DateTime d, DateTime? e, DateTime? f,
                    DateTime? g, DateTime? h, DateTime? i, DateTime? j, DateTime? k) =>
                        new ControlSchedule
                        {
                            Id = 1,
                            YearId = a,
                            SeasonId = b,
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
            var mockContextFactory = SetupContextFactory(mockQuery.Object);
            _sut = new ControlScheduleService(mockContextFactory.Object, _mapper);

            var testModel = GetTestControlSchedules().First();
            var dto = _mapper.Map<ControlScheduleDto>(testModel);

            // ACT
            var result = await _sut.CreateAsync(dto);

            // ASSERT
            Assert.Equal(ComparableObject.Convert(dto), ComparableObject.Convert(result.Entity));
        }

        [Fact]
        public async Task Create_ShouldReturnErrorWhenNull()
        {
            // ARRANGE
            var mockQuery = new Mock<IControlScheduleQuery>();
            mockQuery
                .Setup(tq => tq.Create(
                    It.IsAny<byte>(), It.IsAny<byte>(), It.IsAny<short>(), It.IsAny<DateTime>(),
                    It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(),
                    It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>()))
                .ReturnsAsync(()=>null);
            var mockContextFactory = SetupContextFactory(mockQuery.Object);
            _sut = new ControlScheduleService(mockContextFactory.Object, _mapper);

            var testModel = GetTestControlSchedules().First();
            var dto = _mapper.Map<ControlScheduleDto>(testModel);

            // ACT
            var result = await _sut.CreateAsync(dto);

            // ASSERT
            Assert.Equal("Ошибка при добавлении записи", result.Error.Message);
        }

        [Fact]
        public async Task Create_ShouldReturnErrorWhenExceptionInQuery()
        {
            // ARRANGE
            var mockQuery = new Mock<IControlScheduleQuery>();
            mockQuery
                .Setup(tq => tq.Create(
                    It.IsAny<byte>(), It.IsAny<byte>(), It.IsAny<short>(), It.IsAny<DateTime>(),
                    It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(),
                    It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>()))
                .ThrowsAsync(new Exception("Mock exception"));
            var mockContextFactory = SetupContextFactory(mockQuery.Object);
            _sut = new ControlScheduleService(mockContextFactory.Object, _mapper);

            var testModel = GetTestControlSchedules().First();
            var dto = _mapper.Map<ControlScheduleDto>(testModel);

            // ACT
            var result = await _sut.CreateAsync(dto);

            // ASSERT
            Assert.Equal("Mock exception", result.Error.Message);
        }

        [Fact]
        public async Task GetByYearAndSeasonAsync_ShouldReturnList()
        {
            // ARRANGE
            var mockQuery = new Mock<IControlScheduleQuery>();
            mockQuery
                .Setup(tq => tq.GetByYearAndSeason(
                    It.IsAny<byte>(), It.IsAny<byte>()))
                .ReturnsAsync((byte year,byte season)=>GetTestControlSchedules().Where(c => c.YearId == year && c.SeasonId == season).ToList());
            var mockContextFactory = SetupContextFactory(mockQuery.Object);
            _sut = new ControlScheduleService(mockContextFactory.Object, _mapper);

            byte yearId = 1, seasonId = 1;
            var expectedDtoList = _mapper.Map<List<ControlScheduleListDto>>(GetTestControlSchedules().GetRange(0,1));

            // ACT
            var result = await _sut.GetByYearAndSeasonAsync(new GetByYearAndSeasonRequest(yearId,seasonId));

            // ASSERT
            Assert.Equal(expectedDtoList.Count, result.Entity.Count);
            Assert.Equal(ComparableObject.Convert(expectedDtoList.First()), ComparableObject.Convert(result.Entity.First()));
        }

        [Fact]
        public async Task GetByYearAndSeasonAsync_ShouldReturnErrorWhenExceptionInQuery()
        {
            // ARRANGE
            var mockQuery = new Mock<IControlScheduleQuery>();
            mockQuery
                .Setup(tq => tq.GetByYearAndSeason(
                    It.IsAny<byte>(), It.IsAny<byte>()))
                .ThrowsAsync(new Exception("Mock exception"));
            var mockContextFactory = SetupContextFactory(mockQuery.Object);
            _sut = new ControlScheduleService(mockContextFactory.Object, _mapper);

            byte yearId = 1, seasonId = 1;
            var expectedDtoList = _mapper.Map<List<ControlScheduleDto>>(GetTestControlSchedules().GetRange(0, 1));

            // ACT
            var result = await _sut.GetByYearAndSeasonAsync(new GetByYearAndSeasonRequest(yearId, seasonId));

            // ASSERT
            Assert.Equal("Mock exception", result.Error.Message);            
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnValue()
        {
            // ARRANGE
            var mockQuery = new Mock<IControlScheduleQuery>();
            mockQuery
                .Setup(tq => tq.GetById(It.IsAny<int>()))
                .ReturnsAsync((int id) => GetTestControlSchedules().First(c => c.Id == id));
            var mockContextFactory = SetupContextFactory(mockQuery.Object);
            _sut = new ControlScheduleService(mockContextFactory.Object, _mapper);

            var expectedDto = _mapper.Map<ControlScheduleDto>(GetTestControlSchedules().First());
            expectedDto.Year = GetTestYears().First();
            expectedDto.Season = GetTestSeasons().First();

            // ACT
            var result = await _sut.GetByIdAsync(1);

            // ASSERT
            Assert.Equal(ComparableObject.Convert(expectedDto), ComparableObject.Convert(result.Entity));
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnErrorWhenNotFound()
        {
            // ARRANGE
            var mockQuery = new Mock<IControlScheduleQuery>();
            mockQuery
                .Setup(tq => tq.GetById(It.IsAny<int>()))
                .ReturnsAsync((int id) => GetTestControlSchedules().FirstOrDefault(c => c.Id == id));
            var mockContextFactory = SetupContextFactory(mockQuery.Object);
            _sut = new ControlScheduleService(mockContextFactory.Object, _mapper);

            // ACT
            var result = await _sut.GetByIdAsync(10);

            // ASSERT
            Assert.Equal("Запись не найдена",result.Error.Message);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnErrorWhenExceptionInQuery()
        {
            // ARRANGE
            var mockQuery = new Mock<IControlScheduleQuery>();
            mockQuery
                .Setup(tq => tq.GetById(It.IsAny<int>()))
                .ThrowsAsync(new Exception("Mock exception"));
            var mockContextFactory = SetupContextFactory(mockQuery.Object);
            _sut = new ControlScheduleService(mockContextFactory.Object, _mapper);

            // ACT
            var result = await _sut.GetByIdAsync(10);

            // ASSERT
            Assert.Equal("Mock exception", result.Error.Message);
        }

        [Fact]
        public async Task Update_ShouldReturnValue()
        {
            // ARRANGE
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
            var mockContextFactory = SetupContextFactory(mockQuery.Object);
            _sut = new ControlScheduleService(mockContextFactory.Object, _mapper);

            var testModel = GetTestControlSchedules().First();
            var dto = _mapper.Map<ControlScheduleDto>(testModel);
            dto.Year = GetTestYears().First();
            dto.Season = GetTestSeasons().First();

            // ACT
            var result = await _sut.UpdateAsync(dto);

            // ASSERT
            Assert.Equal(ComparableObject.Convert(dto), ComparableObject.Convert(result.Entity));

        }

        [Fact]
        public async Task Update_ShouldReturnErrorWhenExceptionInQuery()
        {
            // ARRANGE
            var mockQuery = new Mock<IControlScheduleQuery>();
            mockQuery
                .Setup(tq => tq.Update(
                    It.IsAny<int>(), It.IsAny<short>(), It.IsAny<DateTime>(),
                    It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(),
                    It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>()))
                .ThrowsAsync(new Exception("Mock exception"));
            var mockContextFactory = SetupContextFactory(mockQuery.Object);
            _sut = new ControlScheduleService(mockContextFactory.Object, _mapper);

            var testModel = GetTestControlSchedules().First();
            var dto = _mapper.Map<ControlScheduleDto>(testModel);

            // ACT
            var result = await _sut.UpdateAsync(dto);

            // ASSERT
            Assert.Equal("Mock exception", result.Error.Message);

        }
    }
}
