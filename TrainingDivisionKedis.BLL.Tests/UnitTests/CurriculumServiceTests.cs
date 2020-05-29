using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrainingDivisionKedis.BLL.DTO.Curriculum;
using TrainingDivisionKedis.BLL.Services;
using TrainingDivisionKedis.Core.SPModels.Curriculum;
using TrainingDivisionKedis.DAL.ApplicationDbContext;
using TrainingDivisionKedis.DAL.Contracts;
using TrainingDivisionKedis.Core.Contracts.Queries;
using TrainingDivisionKedis.DAL.Extensions;
using Xunit;

namespace TrainingDivisionKedis.BLL.Tests
{
    public class CurriculumServiceTests
    {
        public CurriculumService _sut;

        private static IAppDbContextFactory SetupContextFactory(ICurriculumQuery curriculumQuery)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            QueryExtensions.CurriculumQueryFactory = context => curriculumQuery;

            var mockDbContextFactory = new AppDbContextFactory(options);
            return mockDbContextFactory;
        }

        private static List<SPSubjectsGetByStudent> GetTestSubjects()
        {
            return new List<SPSubjectsGetByStudent>
            {
                new SPSubjectsGetByStudent {Id = 1, Semestr = 1, Subject = "Subject1"},
                new SPSubjectsGetByStudent {Id = 2, Semestr = 1, Subject = "Subject2"},
                new SPSubjectsGetByStudent {Id = 3, Semestr = 2, Subject = "Subject1"},
                new SPSubjectsGetByStudent {Id = 4, Semestr = 3, Subject = "Subject3"}
            };
        }

        [Fact]
        public void Constructor_ShouldThrowExceptionWhenNullArgument()
        {
            // ASSERT
            Assert.Throws<ArgumentNullException>(() => { var _sut = new CurriculumService(null); });
        }

        [Fact]
        public async Task GetSubjectsOfStudentAsync_ShouldReturnList()
        {
            // ARRANGE
            var mockQuery = new Mock<ICurriculumQuery>();
            mockQuery
                .Setup(cq => cq.GetByStudentAndTerm(It.IsAny<int>(), It.IsAny<byte>()))
                .ReturnsAsync((int student, byte term) => GetTestSubjects());

            var contextFactory = SetupContextFactory(mockQuery.Object);
            _sut = new CurriculumService(contextFactory);

            // ACT
            var result = await _sut.GetSubjectsOfStudentAsync(new GetSubjectsOfStudentRequest(1, 1));

            // ASSERT
            Assert.Equal(GetTestSubjects().Count, result.Entity.Count);
        }

        [Fact]
        public async Task GetSubjectsOfStudentAsync_ShouldReturnErrorWhenNullRequest()
        {
            // ARRANGE
            var mockQuery = new Mock<ICurriculumQuery>();
            mockQuery
                .Setup(cq => cq.GetByStudentAndTerm(It.IsAny<int>(), It.IsAny<byte>()))
                .ReturnsAsync((int student, byte term) => GetTestSubjects());

            var contextFactory = SetupContextFactory(mockQuery.Object);
            _sut = new CurriculumService(contextFactory);

            // ACT
            var result = await _sut.GetSubjectsOfStudentAsync(null);

            // ASSERT
            Assert.NotNull(result.Error);
        }
    }
}
