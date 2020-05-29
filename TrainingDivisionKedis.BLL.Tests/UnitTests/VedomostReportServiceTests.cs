using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainingDivisionKedis.BLL.Common;
using TrainingDivisionKedis.BLL.Contracts;
using TrainingDivisionKedis.BLL.Services;
using TrainingDivisionKedis.Core.Models;
using TrainingDivisionKedis.Core.SPModels.ProgressInStudy;
using TrainingDivisionKedis.DAL.ApplicationDbContext;
using TrainingDivisionKedis.DAL.Contracts;
using TrainingDivisionKedis.Core.Contracts.Queries;
using TrainingDivisionKedis.DAL.Extensions;
using Xunit;

namespace TrainingDivisionKedis.BLL.Tests
{
    public class VedomostReportServiceTests
    {
        VedomostReportService _sut;

        private static Mock<IAppDbContextFactory> SetupContextFactory(IProgressInStudyQuery progressInStudyQuery)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var mockDbContext = new AppDbContext(options);

            mockDbContext.DirectorNames.Add(new DirectorName { Nom = 1, Director = "Director", NachUchChasti = "NachUchChasti", ZamDirector = "ZamDirector" });
            mockDbContext.SaveChanges();

            QueryExtensions.ProgressInStudyQueryFactory = context => progressInStudyQuery;

            var mockDbContextFactory = new Mock<IAppDbContextFactory>();
            mockDbContextFactory.Setup(cf => cf.Create()).Returns(mockDbContext);
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

        [Fact]
        public void Constructor_ShouldThrowExceptionWhenArgumentIsNull1()
        {
            var mockFileService = new Mock<IOptions<ReportsConfiguration>>();
            mockFileService
                .SetupGet(f => f.Value)
                .Returns(new ReportsConfiguration { BaseDirectory = @"C:\Users\E7450\Downloads\VKR\TrainingDivisionKedis\TrainingDivisionKedis.BLL.Tests\bin\Debug\netcoreapp2.2\ReportTemplates\", VedomostTemplate = @"VedomostTemplate.txt" });
            Assert.Throws<ArgumentNullException>(() => _sut = new VedomostReportService(null, mockFileService.Object));
        }

        [Fact]
        public void Constructor_ShouldThrowExceptionWhenArgumentIsNull2()
        {
            var mockDbContextFactory = new Mock<IAppDbContextFactory>();
            Assert.Throws<ArgumentNullException>(() => _sut = new VedomostReportService(mockDbContextFactory.Object, null));
        }

        [Fact]
        public void Constructor_ShouldThrowExceptionWhenDirectoryNotFound()
        {
            var mockDbContextFactory = new Mock<IAppDbContextFactory>();
            var mockFileService = new Mock<IOptions<ReportsConfiguration>>();
            mockFileService
                .SetupGet(f => f.Value)
                .Returns(new ReportsConfiguration { BaseDirectory = @"C:\Users\", VedomostTemplate = @"VedomostTemplate.txt" });
            Assert.Throws<FileNotFoundException>(() => _sut = new VedomostReportService(mockDbContextFactory.Object, mockFileService.Object));
        }

        [Fact]
        public async Task GetReportAsync_ShouldReturnFileDto()
        {
            // ARRANGE
            var mockProgressInStudyQuery = new Mock<IProgressInStudyQuery>();
            mockProgressInStudyQuery
                .Setup(p => p.GetByRaspredelenie(It.IsAny<int>()))
                .ReturnsAsync(GetTestProgressInStudy());
            mockProgressInStudyQuery
                .Setup(p => p.GetTotalsByRaspredelenie(It.IsAny<int>()))
                .ReturnsAsync(GetTestTotals());

            var reportsConfiguration = new ReportsConfiguration { BaseDirectory = @"C:\Users\E7450\Downloads\VKR\TrainingDivisionKedis\TrainingDivisionKedis.BLL.Tests\bin\Debug\netcoreapp2.2\ReportTemplates\", VedomostTemplate = @"VedomostTemplate.docx" };
            IOptions<ReportsConfiguration> options = Options.Create(reportsConfiguration);

            var mockContextFactory = SetupContextFactory(mockProgressInStudyQuery.Object);
            _sut = new VedomostReportService(mockContextFactory.Object, options);

            // ACT
            var actual = await _sut.GetReportAsync(1);

            // ASSERT
            Assert.True(actual.Succedeed);
            Assert.True(actual.Entity.FileBytes.Length > 0);
            Assert.Contains(GetTestProgressInStudy().First().SubjectName, actual.Entity.FileName);
        }

        [Fact]
        public async Task GetReportAsync_ShouldReturnErrorWhenPrInStEmpty()
        {
            // ARRANGE
            var mockProgressInStudyQuery = new Mock<IProgressInStudyQuery>();
            mockProgressInStudyQuery
                .Setup(p => p.GetByRaspredelenie(It.IsAny<int>()))
                .ReturnsAsync(new List<SPProgressInStudyGetByRaspredelenieAndUser>());
            mockProgressInStudyQuery
                .Setup(p => p.GetTotalsByRaspredelenie(It.IsAny<int>()))
                .ReturnsAsync(GetTestTotals());

            var reportsConfiguration = new ReportsConfiguration { BaseDirectory = @"C:\Users\E7450\Downloads\VKR\TrainingDivisionKedis\TrainingDivisionKedis.BLL.Tests\bin\Debug\netcoreapp2.2\ReportTemplates\", VedomostTemplate = @"VedomostTemplate.docx" };
            IOptions<ReportsConfiguration> options = Options.Create(reportsConfiguration);

            var mockContextFactory = SetupContextFactory(mockProgressInStudyQuery.Object);
            _sut = new VedomostReportService(mockContextFactory.Object, options);

            // ACT
            var actual = await _sut.GetReportAsync(1);

            // ASSERT
            Assert.Equal("Неполная информации для формирования отчета", actual.Error.Message);
        }

        [Fact]
        public async Task GetReportAsync_ShouldReturnErrorWhenTotalsEmpty()
        {
            // ARRANGE
            var mockProgressInStudyQuery = new Mock<IProgressInStudyQuery>();
            mockProgressInStudyQuery
                .Setup(p => p.GetByRaspredelenie(It.IsAny<int>()))
                .ReturnsAsync(GetTestProgressInStudy());
            mockProgressInStudyQuery
                .Setup(p => p.GetTotalsByRaspredelenie(It.IsAny<int>()))
                .ReturnsAsync(new List<SPProgressInStudyGetTotals>());

            var reportsConfiguration = new ReportsConfiguration { BaseDirectory = @"C:\Users\E7450\Downloads\VKR\TrainingDivisionKedis\TrainingDivisionKedis.BLL.Tests\bin\Debug\netcoreapp2.2\ReportTemplates\", VedomostTemplate = @"VedomostTemplate.docx" };
            IOptions<ReportsConfiguration> options = Options.Create(reportsConfiguration);

            var mockContextFactory = SetupContextFactory(mockProgressInStudyQuery.Object);
            _sut = new VedomostReportService(mockContextFactory.Object, options);

            // ACT
            var actual = await _sut.GetReportAsync(1);

            // ASSERT
            Assert.Equal("Неполная информации для формирования отчета", actual.Error.Message);
        }

        [Fact]
        public async Task GetReportAsync_ShouldReturnErrorWhenExceptionInQuery1()
        {
            // ARRANGE
            var mockProgressInStudyQuery = new Mock<IProgressInStudyQuery>();
            mockProgressInStudyQuery
                .Setup(p => p.GetByRaspredelenie(It.IsAny<int>()))
                .ThrowsAsync(new Exception("Mock exception"));
            mockProgressInStudyQuery
                .Setup(p => p.GetTotalsByRaspredelenie(It.IsAny<int>()))
                .ReturnsAsync(new List<SPProgressInStudyGetTotals>());

            var reportsConfiguration = new ReportsConfiguration { BaseDirectory = @"C:\Users\E7450\Downloads\VKR\TrainingDivisionKedis\TrainingDivisionKedis.BLL.Tests\bin\Debug\netcoreapp2.2\ReportTemplates\", VedomostTemplate = @"VedomostTemplate.docx" };
            IOptions<ReportsConfiguration> options = Options.Create(reportsConfiguration);

            var mockContextFactory = SetupContextFactory(mockProgressInStudyQuery.Object);
            _sut = new VedomostReportService(mockContextFactory.Object, options);

            // ACT
            var actual = await _sut.GetReportAsync(1);

            // ASSERT
            Assert.Equal("Mock exception", actual.Error.Message);
        }

        [Fact]
        public async Task GetReportAsync_ShouldReturnErrorWhenExceptionInQuery2()
        {
            // ARRANGE
            var mockProgressInStudyQuery = new Mock<IProgressInStudyQuery>();
            mockProgressInStudyQuery
                .Setup(p => p.GetByRaspredelenie(It.IsAny<int>()))
                .ReturnsAsync(GetTestProgressInStudy());
            mockProgressInStudyQuery
                .Setup(p => p.GetTotalsByRaspredelenie(It.IsAny<int>()))
                .ThrowsAsync(new Exception("Mock exception"));

            var reportsConfiguration = new ReportsConfiguration { BaseDirectory = @"C:\Users\E7450\Downloads\VKR\TrainingDivisionKedis\TrainingDivisionKedis.BLL.Tests\bin\Debug\netcoreapp2.2\ReportTemplates\", VedomostTemplate = @"VedomostTemplate.docx" };
            IOptions<ReportsConfiguration> options = Options.Create(reportsConfiguration);

            var mockContextFactory = SetupContextFactory(mockProgressInStudyQuery.Object);
            _sut = new VedomostReportService(mockContextFactory.Object, options);

            // ACT
            var actual = await _sut.GetReportAsync(1);

            // ASSERT
            Assert.Equal("Mock exception", actual.Error.Message);
        }
    }
}
