using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainingDivisionKedis.BLL.Services;
using TrainingDivisionKedis.Core.Models;
using TrainingDivisionKedis.DAL.ApplicationDbContext;
using TrainingDivisionKedis.DAL.Contracts;
using Xunit;

namespace TrainingDivisionKedis.BLL.Tests
{
    public class TermServiceTests
    {
        TermService _sut;

        private static List<TermSeason> GetTestSeasons()
        {
            return new List<TermSeason>
            {
                new TermSeason { Id = 1, Name = "осенний" },
                new TermSeason { Id = 2, Name = "весенний" }
            };
        }

        private static List<Term> GetTestTerms()
        {
            return new List<Term>
            {
                new Term { Id = 1, Season = GetTestSeasons().First()},
                new Term { Id = 2, Season = GetTestSeasons().Last()},
                new Term { Id = 3, Season = GetTestSeasons().First()},
                new Term { Id = 4, Season = GetTestSeasons().Last()},
                new Term { Id = 5, Season = GetTestSeasons().First()},
                new Term { Id = 6, Season = GetTestSeasons().Last()}
            };
        }

        private static Mock<IAppDbContextFactory> SetupContextFactory()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var mockDbContext = new AppDbContext(options);

            mockDbContext.TermSeasons.AddRange(GetTestSeasons());
            mockDbContext.Terms.AddRange(GetTestTerms());
            mockDbContext.SaveChanges();

            var mockDbContextFactory = new Mock<IAppDbContextFactory>();
            mockDbContextFactory.Setup(cf => cf.Create()).Returns(mockDbContext);
            return mockDbContextFactory;
        }

        [Fact]
        public void Constructor_ShouldThrowExceptionWhenArgumentIsNull1()
        {
            Assert.Throws<ArgumentNullException>(() => _sut = new TermService(null));
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnList()
        {
            // ARRANGE
            var mockContextFactory = SetupContextFactory();
            _sut = new TermService(mockContextFactory.Object);
            var expected = GetTestTerms().First();
            expected.SeasonId = 1;

            // ACT
            var actual = await _sut.GetAllAsync();
            
            // ASSERT
            Assert.Equal(GetTestTerms().Count, actual.Entity.Count);
            Assert.Equal(ComparableObject.Convert(expected), ComparableObject.Convert(actual.Entity.First()));
        }

        [Fact]
        public async Task GetSeasonsAllAsync_ShouldReturnList()
        {
            // ARRANGE
            var mockContextFactory = SetupContextFactory();
            _sut = new TermService(mockContextFactory.Object);

            // ACT
            var actual = await _sut.GetSeasonsAllAsync();

            // ASSERT
            Assert.Equal(GetTestSeasons().Count, actual.Entity.Count);
            Assert.Equal(ComparableObject.Convert(GetTestSeasons().First()), ComparableObject.Convert(actual.Entity.First()));
        }
    }
}
