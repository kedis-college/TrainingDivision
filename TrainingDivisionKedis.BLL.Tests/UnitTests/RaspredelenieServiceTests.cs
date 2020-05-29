using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TrainingDivisionKedis.BLL.DTO.Raspredelenie;
using TrainingDivisionKedis.BLL.Services;
using TrainingDivisionKedis.Core.SPModels.Raspredelenie;
using TrainingDivisionKedis.DAL.ApplicationDbContext;
using TrainingDivisionKedis.DAL.Contracts;
using TrainingDivisionKedis.Core.Contracts.Queries;
using TrainingDivisionKedis.DAL.Extensions;
using Xunit;

namespace TrainingDivisionKedis.BLL.Tests
{
    public class RaspredelenieServiceTests
    {
        public RaspredelenieService _sut;

        private static Mock<IAppDbContextFactory> SetupContextFactory(IRaspredelenieQuery raspredelenieQuery)
        {
            var options = new DbContextOptionsBuilder<DbContext>().Options;
            var dbContext = new AppDbContext(options);

            QueryExtensions.RaspredelenieQueryFactory = context => raspredelenieQuery;

            var mockDbContextFactory = new Mock<IAppDbContextFactory>();
            mockDbContextFactory.Setup(cf => cf.Create()).Returns(dbContext);
            return mockDbContextFactory;
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

        private static List<SPRaspredelenieGetByYearAndTermAndUser> GetTestVedomost()
        {
            return new List<SPRaspredelenieGetByYearAndTermAndUser>
            {
                new SPRaspredelenieGetByYearAndTermAndUser { Semestr = 1, SubjectId = 1, SubjectName = "Subject1", YearId = 1, Id = 1, GroupId = 1 },
                new SPRaspredelenieGetByYearAndTermAndUser { Semestr = 1, SubjectId = 2, SubjectName = "Subject2", YearId = 1, Id = 2, GroupId = 2 },
                new SPRaspredelenieGetByYearAndTermAndUser { Semestr = 1, SubjectId = 3, SubjectName = "Subject3", YearId = 1, Id = 3, GroupId = 3 }
            };
        }

        private static List<SPRaspredelenieOfYear> GetTestRaspredelenieOfYear()
        {
            return new List<SPRaspredelenieOfYear>
            {
                new SPRaspredelenieOfYear { Id = 1 },
                new SPRaspredelenieOfYear { Id = 2}
            };
        }

        [Fact]
        public void Constructor_ShouldThrowExceptionWhenNullArgument()
        {
            // ASSERT
            Assert.Throws<ArgumentNullException>(() => { var _sut = new RaspredelenieService(null); });
        }

        [Fact]
        public async Task GetSubjectsByUserAndTermAndYearAsync_ShouldReturnList()
        {
            // ARRANGE
            var mockQuery = new Mock<IRaspredelenieQuery>();
            mockQuery
                .Setup(cq => cq.GetSubjectsByYearAndTermAndUser(It.IsAny<byte>(), It.IsAny<byte>(), It.IsAny<int>()))
                .ReturnsAsync(GetTestSubjects());

            var mockContextFactory = SetupContextFactory(mockQuery.Object);
            _sut = new RaspredelenieService(mockContextFactory.Object);

            // ACT
            var result = await _sut.GetSubjectsByUserAndTermAndYearAsync(new GetVedomostListRequest(1, 1, 1));

            // ASSERT
            Assert.Equal(GetTestSubjects().Count, result.Entity.Count);
        }

        [Fact]
        public async Task GetSubjectsByUserAndTermAndYearAsync_ShouldReturnErrorWhenExceptionInQuery()
        {
            // ARRANGE
            var mockQuery = new Mock<IRaspredelenieQuery>();
            mockQuery
                .Setup(cq => cq.GetSubjectsByYearAndTermAndUser(It.IsAny<byte>(), It.IsAny<byte>(), It.IsAny<int>()))
                .ThrowsAsync(new Exception("Mock exception"));

            var mockContextFactory = SetupContextFactory(mockQuery.Object);
            _sut = new RaspredelenieService(mockContextFactory.Object);

            // ACT
            var result = await _sut.GetSubjectsByUserAndTermAndYearAsync(new GetVedomostListRequest(1, 1, 1));

            // ASSERT
            Assert.Equal("Mock exception", result.Error.Message);
        }

        [Fact]
        public async Task GetSubjectsByUserAndTermAndYearAsync_ShouldReturnErrorWhenArgumentIsNull()
        {
            // ARRANGE
            var mockQuery = new Mock<IRaspredelenieQuery>();
            mockQuery
                .Setup(cq => cq.GetSubjectsByYearAndTermAndUser(It.IsAny<byte>(), It.IsAny<byte>(), It.IsAny<int>()))
                .ReturnsAsync(GetTestSubjects());

            var mockContextFactory = SetupContextFactory(mockQuery.Object);
            _sut = new RaspredelenieService(mockContextFactory.Object);

            // ACT
            var result = await _sut.GetSubjectsByUserAndTermAndYearAsync(null);

            // ASSERT
            Assert.NotNull(result.Error);
        }

        [Fact]
        public async Task GetVedomostListAsync_ShouldReturnList()
        {
            // ARRANGE
            var mockQuery = new Mock<IRaspredelenieQuery>();
            mockQuery
                .Setup(cq => cq.GetByYearAndTermAndUser(It.IsAny<byte>(), It.IsAny<byte>(), It.IsAny<int>()))
                .ReturnsAsync(GetTestVedomost());

            var mockContextFactory = SetupContextFactory(mockQuery.Object);
            _sut = new RaspredelenieService(mockContextFactory.Object);

            // ACT
            var result = await _sut.GetVedomostListAsync(new GetVedomostListRequest(1, 1, 1));

            // ASSERT
            Assert.Equal(GetTestVedomost().Count, result.Entity.Count);
        }

        [Fact]
        public async Task GetVedomostListAsync_ShouldReturnErrorWhenExceptionInQuery()
        {
            // ARRANGE
            var mockQuery = new Mock<IRaspredelenieQuery>();
            mockQuery
                .Setup(cq => cq.GetByYearAndTermAndUser(It.IsAny<byte>(), It.IsAny<byte>(), It.IsAny<int>()))
                .ThrowsAsync(new Exception("Mock exception"));

            var mockContextFactory = SetupContextFactory(mockQuery.Object);
            _sut = new RaspredelenieService(mockContextFactory.Object);

            // ACT
            var result = await _sut.GetVedomostListAsync(new GetVedomostListRequest(1, 1, 1));

            // ASSERT
            Assert.Equal("Mock exception", result.Error.Message);
        }

        [Fact]
        public async Task GetVedomostListAsync_ShouldReturnErrorWhenArgumentIsNull()
        {
            // ARRANGE
            var mockQuery = new Mock<IRaspredelenieQuery>();
            mockQuery
                .Setup(cq => cq.GetByYearAndTermAndUser(It.IsAny<byte>(), It.IsAny<byte>(), It.IsAny<int>()))
                .ReturnsAsync(GetTestVedomost());

            var mockContextFactory = SetupContextFactory(mockQuery.Object);
            _sut = new RaspredelenieService(mockContextFactory.Object);

            // ACT
            var result = await _sut.GetVedomostListAsync(null);

            // ASSERT
            Assert.NotNull(result.Error);
        }

        [Fact]
        public async Task GetByYearAsync_ShouldReturnList()
        {
            // ARRANGE
            var mockQuery = new Mock<IRaspredelenieQuery>();
            mockQuery
                .Setup(cq => cq.GetByYear(It.IsAny<int>()))
                .ReturnsAsync(GetTestRaspredelenieOfYear);

            var mockContextFactory = SetupContextFactory(mockQuery.Object);
            _sut = new RaspredelenieService(mockContextFactory.Object);

            // ACT
            var result = await _sut.GetByYearAsync(1);

            // ASSERT
            Assert.Equal(GetTestRaspredelenieOfYear().Count, result.Entity.Count);
        }

        [Fact]
        public async Task GetByYearAsync_ErrorWhenExceptionInQuery()
        {
            // ARRANGE
            var mockQuery = new Mock<IRaspredelenieQuery>();
            mockQuery
                .Setup(cq => cq.GetByYear(It.IsAny<int>()))
                .ThrowsAsync(new Exception("Mock exception"));

            var mockContextFactory = SetupContextFactory(mockQuery.Object);
            _sut = new RaspredelenieService(mockContextFactory.Object);

            // ACT
            var result = await _sut.GetByYearAsync(1);

            // ASSERT
            Assert.Equal("Mock exception", result.Error.Message);
        }
    }
}
