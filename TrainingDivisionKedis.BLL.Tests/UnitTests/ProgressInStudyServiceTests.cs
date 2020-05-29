using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainingDivisionKedis.BLL.Common;
using TrainingDivisionKedis.BLL.DTO.ProgressInStudy;
using TrainingDivisionKedis.BLL.Services;
using TrainingDivisionKedis.Core.SPModels.ControlSchedule;
using TrainingDivisionKedis.Core.SPModels.ProgressInStudy;
using TrainingDivisionKedis.DAL.ApplicationDbContext;
using TrainingDivisionKedis.DAL.Contracts;
using TrainingDivisionKedis.Core.Contracts.Queries;
using TrainingDivisionKedis.DAL.Extensions;
using Xunit;

namespace TrainingDivisionKedis.BLL.Tests
{
    public class ProgressInStudyServiceTests
    {
        public ProgressInStudyService _sut;
        public IMapper _mapper;

        public ProgressInStudyServiceTests()
        {
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfileBLL());
            });
            _mapper = mappingConfig.CreateMapper();
        }

        private static Mock<IAppDbContextFactory> SetupContextFactory(IProgressInStudyQuery progressInStudyQuery, IControlScheduleQuery controlScheduleQuery)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>().Options;
            var dbContext = new AppDbContext(options);

            QueryExtensions.ProgressInStudyQueryFactory = context => progressInStudyQuery;
            QueryExtensions.ControlScheduleQueryFactory = context => controlScheduleQuery;

            var mockDbContextFactory = new Mock<IAppDbContextFactory>();
            mockDbContextFactory.Setup(cf => cf.Create()).Returns(dbContext);
            return mockDbContextFactory;
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

        public static List<SPProgressInStudyGetByStudent> GetTestProgressInStudyOfStudent()
        {
            return new List<SPProgressInStudyGetByStudent>
            {
                new SPProgressInStudyGetByStudent { NumRec = 1, Student = 1, Subject = 1, Term = 1, Mod1 = 20, Mod2 = 25, Itog = 35, Dop = 0, Prize = 4, Ball = 80, Date = new DateTime(2018,10,20), PrizeName = "хорошо", SubjectName = "Subject1" },
                new SPProgressInStudyGetByStudent { NumRec = 2, Student = 1, Subject = 2, Term = 1, Mod1 = 20, Mod2 = 25, Itog = 35, Dop = 0, Prize = 4, Ball = 80, Date = new DateTime(2018,10,20), PrizeName = "хорошо", SubjectName = "Subject2" },
                new SPProgressInStudyGetByStudent { NumRec = 3, Student = 1, Subject = 3, Term = 2, Mod1 = 20, Mod2 = 25, Itog = 35, Dop = 0, Prize = 4, Ball = 80, Date = new DateTime(2018,10,20), PrizeName = "хорошо", SubjectName = "Subject3" },
                new SPProgressInStudyGetByStudent { NumRec = 4, Student = 1, Subject = 4, Term = 3, Mod1 = 20, Mod2 = 25, Itog = 35, Dop = 0, Prize = 4, Ball = 80, Date = new DateTime(2018,10,20), PrizeName = "хорошо", SubjectName = "Subject4" },
                new SPProgressInStudyGetByStudent { NumRec = 5, Student = 1, Subject = 5, Term = 4, Mod1 = 20, Mod2 = 25, Itog = 35, Dop = 0, Prize = 4, Ball = 80, Date = new DateTime(2018,10,20), PrizeName = "хорошо", SubjectName = "Subject5" }               
            };
        }

        public static List<SPProgressInStudyGetTotals> GetTestTotals()
        {
            return new List<SPProgressInStudyGetTotals>
            {
                new SPProgressInStudyGetTotals { Id = 1, Name = "Name1", Value = 1 },
                new SPProgressInStudyGetTotals { Id = 2, Name = "Name2", Value = 2 },
                new SPProgressInStudyGetTotals { Id = 3, Name = "Name3", Value = 3 }
            };
        }

        [Fact]
        public void Constructor_ShouldThrowExceptionWhenNullArgument1()
        {
            // ASSERT
            Assert.Throws<ArgumentNullException>(() => { var _sut = new ProgressInStudyService(null, _mapper); });
        }

        [Fact]
        public void Constructor_ShouldThrowExceptionWhenNullArgument2()
        {
            // ARRANGE
            var dbContext = new Mock<IAppDbContextFactory>();
            // ASSERT
            Assert.Throws<ArgumentNullException>(() => { var _sut = new ProgressInStudyService(dbContext.Object, null); });
        }

        [Fact]
        public async Task GetByRaspredelenieAndUserAsync_ShouldReturnList()
        {
            // ARRANGE
            var mockPrQuery = new Mock<IProgressInStudyQuery>();
            mockPrQuery.Setup(p => p.GetByRaspredelenieAndUser(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(GetTestProgressInStudy());

            var mockConQuery = new Mock<IControlScheduleQuery>();
            mockConQuery.Setup(p => p.GetEditable(It.IsAny<int>()))
                .ReturnsAsync(new SPControlScheduleGetEditable { Mod1 = false, Mod2 = false, Itog = true });

            var mockContextFactory = SetupContextFactory(mockPrQuery.Object, mockConQuery.Object);
            _sut = new ProgressInStudyService(mockContextFactory.Object, _mapper);

            var expected = new ProgressInStudyListDto
            {
                ProgressInStudy = GetTestProgressInStudy(),
                ControlSchedule = new SPControlScheduleGetEditable { Mod1 = true, Mod2 = true, Itog = false }
            };

            // ACT 
            var actual = await _sut.GetByRaspredelenieAndUserAsync(new ProgressInStudyRequest(1, 1));

            // ASSERT
            Assert.Equal(expected.ProgressInStudy.Count, actual.Entity.ProgressInStudy.Count);
            Assert.Equal(ComparableObject.Convert(expected.ControlSchedule), ComparableObject.Convert(actual.Entity.ControlSchedule));
        }

        [Fact]
        public async Task GetByRaspredelenieAndUserAsync_ShouldReturnErrorWhenExceptionInQuery1()
        {
            // ARRANGE
            var mockPrQuery = new Mock<IProgressInStudyQuery>();
            mockPrQuery.Setup(p => p.GetByRaspredelenieAndUser(It.IsAny<int>(), It.IsAny<int>()))
                .ThrowsAsync(new Exception("Mock exception"));

            var mockConQuery = new Mock<IControlScheduleQuery>();
            mockConQuery.Setup(p => p.GetEditable(It.IsAny<int>()))
                .ReturnsAsync(new SPControlScheduleGetEditable { Mod1 = false, Mod2 = false, Itog = true });

            var mockContextFactory = SetupContextFactory(mockPrQuery.Object, mockConQuery.Object);
            _sut = new ProgressInStudyService(mockContextFactory.Object, _mapper);

            // ACT 
            var actual = await _sut.GetByRaspredelenieAndUserAsync(new ProgressInStudyRequest(1, 1));

            // ASSERT
            Assert.Equal("Mock exception", actual.Error.Message);
        }

        [Fact]
        public async Task GetByRaspredelenieAndUserAsync_ShouldReturnErrorWhenExceptionInQuery2()
        {
            // ARRANGE
            var mockPrQuery = new Mock<IProgressInStudyQuery>();
            mockPrQuery.Setup(p => p.GetByRaspredelenieAndUser(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(GetTestProgressInStudy());

            var mockConQuery = new Mock<IControlScheduleQuery>();
            mockConQuery.Setup(p => p.GetEditable(It.IsAny<int>()))               
                .ThrowsAsync(new Exception("Mock exception"));
            var mockContextFactory = SetupContextFactory(mockPrQuery.Object, mockConQuery.Object);
            _sut = new ProgressInStudyService(mockContextFactory.Object, _mapper);

            // ACT 
            var actual = await _sut.GetByRaspredelenieAndUserAsync(new ProgressInStudyRequest(1, 1));

            // ASSERT
            Assert.Equal("Mock exception", actual.Error.Message);
        }

        [Fact]
        public async Task GetByRaspredelenieAndUserAsync_ShouldReturnErrorWhenArgumentIsNull()
        {
            // ARRANGE
            var mockPrQuery = new Mock<IProgressInStudyQuery>();
            mockPrQuery.Setup(p => p.GetByRaspredelenieAndUser(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(GetTestProgressInStudy());

            var mockConQuery = new Mock<IControlScheduleQuery>();
            mockConQuery.Setup(p => p.GetEditable(It.IsAny<int>()))
                .ReturnsAsync(new SPControlScheduleGetEditable { Mod1 = false, Mod2 = false, Itog = true });
            var mockContextFactory = SetupContextFactory(mockPrQuery.Object, mockConQuery.Object);
            _sut = new ProgressInStudyService(mockContextFactory.Object, _mapper);

            // ACT 
            var actual = await _sut.GetByRaspredelenieAndUserAsync(null);

            // ASSERT
            Assert.NotNull(actual.Error);
        }

        [Fact]
        public async Task GetByRaspredelenieAsync_ShouldReturnList()
        {
            // ARRANGE
            var mockPrQuery = new Mock<IProgressInStudyQuery>();
            mockPrQuery.Setup(p => p.GetByRaspredelenie(It.IsAny<int>()))
                .ReturnsAsync(GetTestProgressInStudy());        

            var mockContextFactory = SetupContextFactory(mockPrQuery.Object, null);
            _sut = new ProgressInStudyService(mockContextFactory.Object, _mapper);

            var expected = GetTestProgressInStudy();

            // ACT 
            var actual = await _sut.GetByRaspredelenieAsync(1);

            // ASSERT
            Assert.Equal(expected.Count, actual.Entity.Count);           
        }

        [Fact]
        public async Task GetByRaspredelenieAsync_ShouldReturnErrorWhenExceptionInQuery()
        {
            // ARRANGE
            var mockPrQuery = new Mock<IProgressInStudyQuery>();
            mockPrQuery.Setup(p => p.GetByRaspredelenie(It.IsAny<int>()))
                .ThrowsAsync(new Exception("Mock exception"));

            var mockContextFactory = SetupContextFactory(mockPrQuery.Object, null);
            _sut = new ProgressInStudyService(mockContextFactory.Object, _mapper);

            var expected = GetTestProgressInStudy();

            // ACT 
            var actual = await _sut.GetByRaspredelenieAsync(1);

            // ASSERT
            Assert.Equal("Mock exception", actual.Error.Message);
        }

        [Fact]
        public async Task GetByStudentAsync_ShouldReturnList()
        {
            // ARRANGE
            var mockPrQuery = new Mock<IProgressInStudyQuery>();
            mockPrQuery.Setup(p => p.GetByStudentAndTerm(It.IsAny<int>(), It.IsAny<byte>()))
                .ReturnsAsync((int student, byte term) => GetTestProgressInStudyOfStudent().Where(p => p.Student == student && p.Term == term).ToList());

            var mockContextFactory = SetupContextFactory(mockPrQuery.Object, null);
            _sut = new ProgressInStudyService(mockContextFactory.Object, _mapper);

            var expected = new List<ProgressInStudyOfStudent> {
                new ProgressInStudyOfStudent(1, GetTestProgressInStudyOfStudent().GetRange(0, 2)),
                new ProgressInStudyOfStudent(2, GetTestProgressInStudyOfStudent().GetRange(2, 1)),
                new ProgressInStudyOfStudent(3, GetTestProgressInStudyOfStudent().GetRange(3, 1)),
                new ProgressInStudyOfStudent(4, GetTestProgressInStudyOfStudent().GetRange(4, 1)),
                new ProgressInStudyOfStudent(5, new List<SPProgressInStudyGetByStudent>()),
                new ProgressInStudyOfStudent(6, new List<SPProgressInStudyGetByStudent>())
            };

            // ACT 
            var actual = await _sut.GetByStudentAsync(1);

            // ASSERT
            for (int i = 0; i < 6; i++)
            {
                Assert.Equal(expected[i].Term, actual.Entity[i].Term);
                Assert.Equal(expected[i].ProgressInStudy.Count, actual.Entity[i].ProgressInStudy.Count);
            }
        }

        [Fact]
        public async Task GetByStudentAsync_ShouldReturnErrorWhenExceptionInQuery()
        {
            // ARRANGE
            var mockPrQuery = new Mock<IProgressInStudyQuery>();
            mockPrQuery.Setup(p => p.GetByStudentAndTerm(It.IsAny<int>(), It.IsAny<byte>()))
                .ThrowsAsync(new Exception("Mock exception"));

            var mockContextFactory = SetupContextFactory(mockPrQuery.Object, null);
            _sut = new ProgressInStudyService(mockContextFactory.Object, _mapper);

            // ACT 
            var actual = await _sut.GetByStudentAsync(1);

            // ASSERT
            Assert.Equal("Mock exception", actual.Error.Message);
        }

        [Fact]
        public async Task GetTotalsByRaspredelenieAsync_ShouldReturnList()
        {
            // ARRANGE
            var mockPrQuery = new Mock<IProgressInStudyQuery>();
            mockPrQuery.Setup(p => p.GetTotalsByRaspredelenie(It.IsAny<int>()))
                .ReturnsAsync(GetTestTotals());

            var mockContextFactory = SetupContextFactory(mockPrQuery.Object, null);
            _sut = new ProgressInStudyService(mockContextFactory.Object, _mapper);
            var expected = _mapper.Map<List<TotalsDto>>(GetTestTotals());

            // ACT
            var actual = await _sut.GetTotalsByRaspredelenieAsync(1);

            // ASSERT
            Assert.Equal(expected.Count, actual.Entity.Count);
        }

        [Fact]
        public async Task GetTotalsByRaspredelenieAsync_ShouldReturnErrorWhenExceptionInQuery()
        {
            // ARRANGE
            var mockPrQuery = new Mock<IProgressInStudyQuery>();
            mockPrQuery.Setup(p => p.GetTotalsByRaspredelenie(It.IsAny<int>()))
                .ThrowsAsync(new Exception("Mock exception"));

            var mockContextFactory = SetupContextFactory(mockPrQuery.Object, null);
            _sut = new ProgressInStudyService(mockContextFactory.Object, _mapper);

            // ACT
            var actual = await _sut.GetTotalsByRaspredelenieAsync(1);

            // ASSERT
            Assert.Equal("Mock exception", actual.Error.Message);
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnData()
        {
            // ARRANGE
            var mockPrQuery = new Mock<IProgressInStudyQuery>();
            mockPrQuery.Setup(p => p.Update(It.IsAny<int>(),It.IsAny<byte>(), It.IsAny<byte>(), It.IsAny<byte>(), It.IsAny<byte>(), It.IsAny<short>(), It.IsAny<DateTime?>()))
                .ReturnsAsync(GetTestProgressInStudy().First());

            var mockContextFactory = SetupContextFactory(mockPrQuery.Object, null);
            _sut = new ProgressInStudyService(mockContextFactory.Object, _mapper);

            // ACT
            var request = new ProgressInStudyUpdateRequest { NumRec = 1, Mod1 = 30, Mod2 = 30, Itog = 30, Dop = 5, Date = new DateTime(2020, 05, 1) };
            var actual = await _sut.UpdateAsync(request);

            // ASSERT
            Assert.Equal(ComparableObject.Convert(GetTestProgressInStudy().First()), ComparableObject.Convert(actual.Entity));
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnErrorWhenExceptionInQuery()
        {
            // ARRANGE
            var mockPrQuery = new Mock<IProgressInStudyQuery>();
            mockPrQuery.Setup(p => p.Update(It.IsAny<int>(), It.IsAny<byte>(), It.IsAny<byte>(), It.IsAny<byte>(), It.IsAny<byte>(), It.IsAny<short>(), It.IsAny<DateTime?>()))
                .ThrowsAsync(new Exception("Mock exception"));

            var mockContextFactory = SetupContextFactory(mockPrQuery.Object, null);
            _sut = new ProgressInStudyService(mockContextFactory.Object, _mapper);

            // ACT
            var request = new ProgressInStudyUpdateRequest { NumRec = 1, Mod1 = 30, Mod2 = 30, Itog = 30, Dop = 5, Date = new DateTime(2020, 05, 1) };
            var actual = await _sut.UpdateAsync(request);

            // ASSERT
            Assert.Equal("Mock exception", actual.Error.Message);
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnErrorWhenArgumentIsNull()
        {
            // ARRANGE
            var mockPrQuery = new Mock<IProgressInStudyQuery>();
            mockPrQuery.Setup(p => p.Update(It.IsAny<int>(), It.IsAny<byte>(), It.IsAny<byte>(), It.IsAny<byte>(), It.IsAny<byte>(), It.IsAny<short>(), It.IsAny<DateTime?>()))
                .ReturnsAsync(GetTestProgressInStudy().First());

            var mockContextFactory = SetupContextFactory(mockPrQuery.Object, null);
            _sut = new ProgressInStudyService(mockContextFactory.Object, _mapper);

            // ACT         
            var actual = await _sut.UpdateAsync(null);

            // ASSERT
            Assert.NotNull(actual.Error);
        }

        [Fact]
        public async Task GetTotalsOfStudentAsync_ShouldReturnList()
        {
            // ARRANGE
            var expected = new SPProgressInStudyGetTotalsByStudent { AverageGrade = 4.5, CreditsSum = 50, Gpa = 3.8 };
            var mockPrQuery = new Mock<IProgressInStudyQuery>();
            mockPrQuery.Setup(p => p.GetTotalsByStudent(It.IsAny<int>()))
                .ReturnsAsync(expected);

            var mockContextFactory = SetupContextFactory(mockPrQuery.Object, null);
            _sut = new ProgressInStudyService(mockContextFactory.Object, _mapper);

            // ACT
            var actual = await _sut.GetTotalsOfStudentAsync(1);

            // ASSERT
            Assert.Equal(ComparableObject.Convert(expected), ComparableObject.Convert(actual.Entity));
        }

        [Fact]
        public async Task GetTotalsOfStudentAsync_ShouldReturnErrorWhenExceptionInQuery()
        {
            // ARRANGE
            var mockPrQuery = new Mock<IProgressInStudyQuery>();
            mockPrQuery.Setup(p => p.GetTotalsByStudent(It.IsAny<int>()))
                .ThrowsAsync(new Exception("Mock exception"));

            var mockContextFactory = SetupContextFactory(mockPrQuery.Object, null);
            _sut = new ProgressInStudyService(mockContextFactory.Object, _mapper);

            // ACT
            var actual = await _sut.GetTotalsOfStudentAsync(1);

            // ASSERT
            Assert.Equal("Mock exception", actual.Error.Message);
        }
    }
}
