using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrainingDivisionKedis.BLL.Services;
using TrainingDivisionKedis.Core.SPModels.ActivityOfTeachers;
using TrainingDivisionKedis.DAL.ApplicationDbContext;
using TrainingDivisionKedis.DAL.Contracts;
using TrainingDivisionKedis.Core.Contracts.Queries;
using TrainingDivisionKedis.DAL.Extensions;
using Xunit;

namespace TrainingDivisionKedis.BLL.Tests
{
    public class ActivityOfTeacherServiceTests
    {
        public ActivityOfTeacherService _sut;

        public static IAppDbContextFactory SetupContextFactory(ITeachersQuery teachersQuery)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>().Options;
            QueryExtensions.TeachersQueryFactory = context => teachersQuery;

            var mockDbContextFactory = new AppDbContextFactory(options);
            return mockDbContextFactory;  
        }

        [Fact]
        public void Constructor_ShouldThrowExceptionWhenNullArgument()
        { 
            // ASSERT
            Assert.Throws<ArgumentNullException>(()=> { var _sut = new ActivityOfTeacherService(null); });
        }

        [Fact]
        public async Task GetTeachersWithPostAsync_ShouldReturnList()
        {
            // ARRANGE 
            var mockTeachersQuery = new Mock<ITeachersQuery>();
            mockTeachersQuery.Setup(tq => tq.GetActivityAll()).ReturnsAsync(GetTestData());

            var dbContextFactory = SetupContextFactory(mockTeachersQuery.Object);
            _sut = new ActivityOfTeacherService(dbContextFactory);

            // ACT
            var result = await _sut.GetTeachersWithPostAsync();

            // ASSERT
            Assert.Equal(GetTestData().Count, result.Entity.Count);
        }

        public List<SPFIOOfActivityOfTeachers> GetTestData()
        {
            return new List<SPFIOOfActivityOfTeachers> {
                new SPFIOOfActivityOfTeachers {Nom = 1, FIO = "Surname1 Name1 MiddleName1", FIO_Short = "Surname1 N.M.", Post = "Post1"},
                new SPFIOOfActivityOfTeachers {Nom = 2, FIO = "Surname2 Name2 MiddleName2", FIO_Short = "Surname2 N.M.", Post = "Post2" },
                new SPFIOOfActivityOfTeachers {Nom = 3, FIO = "Surname3 Name3 MiddleName3", FIO_Short = "Surname3 N.M.", Post = "Post3" }
            };
        }
    }
}
