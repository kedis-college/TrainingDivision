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
    public class YearServiceTests
    {
        YearService _sut;

        private static List<Year> GetTestYears()
        {
            return new List<Year>
            {
                new Year { Id = 1, Current = false, Name = "Year1" },
                new Year { Id = 2, Current = false, Name = "Year2" },
                new Year { Id = 3, Current = true, Name = "Year3" },
            };
        }

        private static Mock<IAppDbContextFactory> SetupContextFactory()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var mockDbContext = new AppDbContext(options);

            mockDbContext.Years.AddRange(GetTestYears());
            mockDbContext.SaveChanges();

            var mockDbContextFactory = new Mock<IAppDbContextFactory>();
            mockDbContextFactory.Setup(cf => cf.Create()).Returns(mockDbContext);
            return mockDbContextFactory;
        }

        [Fact]
        public void Constructor_ShouldThrowExceptionWhenArgumentIsNull1()
        {
            Assert.Throws<ArgumentNullException>(() => _sut = new YearService(null));
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnList()
        {
            // ARRANGE
            var mockContextFactory = SetupContextFactory();
            _sut = new YearService(mockContextFactory.Object);

            // ACT
            var actual = await _sut.GetAllAsync();

            // ASSERT
            Assert.Equal(GetTestYears().Count, actual.Entity.Count);
            Assert.Equal(ComparableObject.Convert(GetTestYears().First()), ComparableObject.Convert(actual.Entity.First()));
        }
    }
}
