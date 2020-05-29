using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainingDivisionKedis.BLL.Common;
using TrainingDivisionKedis.BLL.Contracts;
using TrainingDivisionKedis.BLL.DTO;
using TrainingDivisionKedis.BLL.DTO.UmkFiles;
using TrainingDivisionKedis.BLL.Services;
using TrainingDivisionKedis.Core.Models;
using TrainingDivisionKedis.Core.SPModels;
using TrainingDivisionKedis.DAL.ApplicationDbContext;
using TrainingDivisionKedis.DAL.Contracts;
using TrainingDivisionKedis.Core.Contracts.Queries;
using TrainingDivisionKedis.DAL.Extensions;
using Xunit;

namespace TrainingDivisionKedis.BLL.Tests
{
    public class UmkFileServiceTests
    {
        UmkFileService _sut;
        IMapper _mapper;

        public UmkFileServiceTests()
        {
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfileBLL());
            });
            _mapper = mappingConfig.CreateMapper();
        }

        private static List<FileType> GetTestFileTypes()
        {
            return new List<FileType>
            {
                new FileType { Id = 1, Extension = "pdf", Type = "FileType1" },
                new FileType { Id = 2, Extension = "docx", Type = "FileType2" }
            };
        }

        private static List<UmkFile> GetTestUmkFiles()
        {
            return new List<UmkFile>
            {
                new UmkFile { Id = 1, Active = true, CreatedAt = new DateTime(2020,05,1), FileName = "FileName1", FileSize = 1000, FileType = GetTestFileTypes().First(), Name = "Name1", SubjectId = 1, TermId = 1, UpdatedAt = new DateTime(2020,05,2)},
                new UmkFile { Id = 2, Active = false, CreatedAt = new DateTime(2020,05,1), FileName = "FileName2", FileSize = 2000, FileType = GetTestFileTypes().Last(), Name = "Name2", SubjectId = 1, TermId = 1, UpdatedAt = new DateTime(2020,05,2)},
                new UmkFile { Id = 3, Active = true, CreatedAt = new DateTime(2020,05,1), FileName = "FileName3", FileSize = 3000, FileType = GetTestFileTypes().First(), Name = "Name3", SubjectId = 1, TermId = 2 },
                new UmkFile { Id = 4, Active = true, CreatedAt = new DateTime(2020,05,1), FileName = "FileName4", FileSize = 4000, FileType = GetTestFileTypes().Last(), Name = "Name4", SubjectId = 2, TermId = 1 },
                new UmkFile { Id = 5, Active = true, CreatedAt = new DateTime(2020,05,1), FileName = "FileName5", FileSize = 5000, FileType = GetTestFileTypes().First(), Name = "Name5", SubjectId = 1, TermId = 1 }
            };
        }

        private static Mock<IAppDbContextFactory> SetupContextFactory(IUmkFilesQuery umkFilesQuery, ICurriculumQuery curriculumQuery, IRaspredelenieQuery raspredelenieQuery)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var mockDbContext = new AppDbContext(options);

            mockDbContext.FileTypes.AddRange(GetTestFileTypes());
            mockDbContext.UmkFiles.AddRange(GetTestUmkFiles());

            mockDbContext.SaveChanges();
            
            QueryExtensions.UmkFilesQueryFactory = context => umkFilesQuery;
            QueryExtensions.CurriculumQueryFactory = context => curriculumQuery;
            QueryExtensions.RaspredelenieQueryFactory = context => raspredelenieQuery;

            var mockDbContextFactory = new Mock<IAppDbContextFactory>();
            mockDbContextFactory.Setup(cf => cf.Create()).Returns(mockDbContext);
            return mockDbContextFactory;
        }

        [Fact]
        public void Constructor_ShouldThrowExceptionWhenArgumentIsNull1()
        {
            var mockFileService = new Mock<IFileService<UmkFilesConfiguration>>();
            Assert.Throws<ArgumentNullException>(() => _sut = new UmkFileService(null, _mapper, mockFileService.Object));
        }

        [Fact]
        public void Constructor_ShouldThrowExceptionWhenArgumentIsNull2()
        {
            var mockDbContextFactory = new Mock<IAppDbContextFactory>();
            Assert.Throws<ArgumentNullException>(() => _sut = new UmkFileService(mockDbContextFactory.Object, _mapper, null));
        }

        [Fact]
        public void Constructor_ShouldThrowExceptionWhenArgumentIsNull3()
        {
            var mockDbContextFactory = new Mock<IAppDbContextFactory>();
            var mockFileService = new Mock<IFileService<UmkFilesConfiguration>>();
            Assert.Throws<ArgumentNullException>(() => _sut = new UmkFileService(mockDbContextFactory.Object, null, mockFileService.Object));
        }

        [Fact]
        public async Task GetBySubjectAndTermAsync_ShouldReturnListToTeacher()
        {
            // ARRANGE
            var expected = new GetBySubjectAndTermResponse(new List<UmkFile>(), "SubjectName1");

            var mockCurriculum = new Mock<ICurriculumQuery>();
            mockCurriculum.Setup(c => c.GetNameById(It.IsAny<int>()))
                .ReturnsAsync(new SPGetName { Name = expected.SubjectName });
            
            var mockRaspredelenie = new Mock<IRaspredelenieQuery>();
            mockRaspredelenie.Setup(r => r.CheckTeacherBySubjectAndTerm(It.IsAny<short>(), It.IsAny<int>(), It.IsAny<byte>()))
                .ReturnsAsync(new SPBoolResult { Result = true });

            var mockContextFactory = SetupContextFactory(null, mockCurriculum.Object, mockRaspredelenie.Object);

            var mockFileService = new Mock<IFileService<UmkFilesConfiguration>>();

            _sut = new UmkFileService(mockContextFactory.Object, _mapper, mockFileService.Object );
            var request = new GetBySubjectAndTermRequest(1, 1);            

            // ACT
            var actual = await _sut.GetBySubjectAndTermAsync(request);

            // ASSERT
            Assert.Equal(2, actual.Entity.UmkFiles.Count);
            Assert.Equal(ComparableObject.Convert(expected.SubjectName), ComparableObject.Convert(actual.Entity.SubjectName));
        }

        [Fact]
        public async Task GetBySubjectAndTermAsync_ShouldReturnListToStudent()
        {
            // ARRANGE
            var expected = new GetBySubjectAndTermResponse(new List<UmkFile>(), "SubjectName1");

            var mockCurriculum = new Mock<ICurriculumQuery>();
            mockCurriculum.Setup(c => c.GetNameById(It.IsAny<int>()))
                .ReturnsAsync(new SPGetName { Name = expected.SubjectName });
            mockCurriculum.Setup(c => c.CheckStudentBySubjectAndTerm(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<byte>()))
                .ReturnsAsync(new SPBoolResult { Result = true });

            var mockRaspredelenie = new Mock<IRaspredelenieQuery>();
            mockRaspredelenie.Setup(r => r.CheckTeacherBySubjectAndTerm(It.IsAny<short>(), It.IsAny<int>(), It.IsAny<byte>()))
                .ReturnsAsync(new SPBoolResult { Result = false });

            var mockContextFactory = SetupContextFactory(null, mockCurriculum.Object, mockRaspredelenie.Object);

            var mockFileService = new Mock<IFileService<UmkFilesConfiguration>>();

            _sut = new UmkFileService(mockContextFactory.Object, _mapper, mockFileService.Object);
            var request = new GetBySubjectAndTermRequest(1, 1);

            // ACT
            var actual = await _sut.GetBySubjectAndTermAsync(request);

            // ASSERT
            Assert.Equal(2, actual.Entity.UmkFiles.Count);
            Assert.Equal(ComparableObject.Convert(expected.SubjectName), ComparableObject.Convert(actual.Entity.SubjectName));
        }

        [Fact]
        public async Task GetBySubjectAndTermAsync_ShouldReturnErrorWhenSubjectNotFound()
        {
            // ARRANGE
            var mockCurriculum = new Mock<ICurriculumQuery>();
            mockCurriculum.Setup(c => c.GetNameById(It.IsAny<int>()))
                .ReturnsAsync((SPGetName) null);
            var mockRaspredelenie = new Mock<IRaspredelenieQuery>();
            mockRaspredelenie.Setup(r => r.CheckTeacherBySubjectAndTerm(It.IsAny<short>(), It.IsAny<int>(), It.IsAny<byte>()))
                .ReturnsAsync(new SPBoolResult { Result = true });

            var mockContextFactory = SetupContextFactory(null, mockCurriculum.Object, mockRaspredelenie.Object);

            var mockFileService = new Mock<IFileService<UmkFilesConfiguration>>();

            _sut = new UmkFileService(mockContextFactory.Object, _mapper, mockFileService.Object);
            var request = new GetBySubjectAndTermRequest(1, 1);

            // ACT
            var actual = await _sut.GetBySubjectAndTermAsync(request);

            // ASSERT
            Assert.Equal("Дисциплина с Id " + request.SubjectId + " не найдена", actual.Error.Message);           
        }

        [Fact]
        public async Task GetBySubjectAndTermAsync_ShouldReturnErrorWhenDenyInAccess()
        {
            // ARRANGE
            var mockCurriculum = new Mock<ICurriculumQuery>();
            mockCurriculum.Setup(c => c.GetNameById(It.IsAny<int>()))
                .ReturnsAsync(new SPGetName { Name = "Some name" });
            mockCurriculum.Setup(c => c.CheckStudentBySubjectAndTerm(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<byte>()))
                .ReturnsAsync(new SPBoolResult { Result = false });

            var mockRaspredelenie = new Mock<IRaspredelenieQuery>();
            mockRaspredelenie.Setup(r => r.CheckTeacherBySubjectAndTerm(It.IsAny<short>(), It.IsAny<int>(), It.IsAny<byte>()))
                .ReturnsAsync(new SPBoolResult { Result = false });

            var mockContextFactory = SetupContextFactory(null, mockCurriculum.Object, mockRaspredelenie.Object);

            var mockFileService = new Mock<IFileService<UmkFilesConfiguration>>();

            _sut = new UmkFileService(mockContextFactory.Object, _mapper, mockFileService.Object);
            var request = new GetBySubjectAndTermRequest(1, 1);

            // ACT
            var actual = await _sut.GetBySubjectAndTermAsync(request);

            // ASSERT
            Assert.Equal("Отказ в доступе", actual.Error.Message);
        }

        [Fact]
        public async Task GetBySubjectAndTermAsync_ShouldReturnErrorWhenExceptionInUmkQuery()
        {
            // ARRANGE
            var mockCurriculum = new Mock<ICurriculumQuery>();
            mockCurriculum.Setup(c => c.GetNameById(It.IsAny<int>()))
                .ThrowsAsync(new Exception("Mock exception"));

            var mockRaspredelenie = new Mock<IRaspredelenieQuery>();
            mockRaspredelenie.Setup(r => r.CheckTeacherBySubjectAndTerm(It.IsAny<short>(), It.IsAny<int>(), It.IsAny<byte>()))
                .ReturnsAsync(new SPBoolResult { Result = true });

            var mockContextFactory = SetupContextFactory(null, mockCurriculum.Object, mockRaspredelenie.Object);

            var mockFileService = new Mock<IFileService<UmkFilesConfiguration>>();

            _sut = new UmkFileService(mockContextFactory.Object, _mapper, mockFileService.Object);
            var request = new GetBySubjectAndTermRequest(1, 1);

            // ACT
            var actual = await _sut.GetBySubjectAndTermAsync(request);

            // ASSERT
            Assert.Equal("Mock exception", actual.Error.Message);
        }

        [Fact]
        public async Task CreateAsync_ShouldReturnData()
        {
            // ARRANGE
            var expected = new UmkFile { Id = 10, Active = true, CreatedAt = new DateTime(2020, 05, 1), FileName = "FileNameNew", FileSize = 1000, FileType = GetTestFileTypes().First(), Name = "NameNew", SubjectId = 1, TermId = 1, UpdatedAt = new DateTime(2020, 05, 2) };

            var mockUmkQuery = new Mock<IUmkFilesQuery>();
            mockUmkQuery
                .Setup(u => u.Create(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<byte>(), It.IsAny<string>(), It.IsAny<double>(), It.IsAny<string>()))
                .ReturnsAsync(expected);
            var mockRaspredelenie = new Mock<IRaspredelenieQuery>();
            mockRaspredelenie.Setup(r => r.CheckTeacherBySubjectAndTerm(It.IsAny<short>(), It.IsAny<int>(), It.IsAny<byte>()))
                .ReturnsAsync(new SPBoolResult { Result = true });

            var mockContextFactory = SetupContextFactory(mockUmkQuery.Object, null,mockRaspredelenie.Object);

            var mockFormFile = new Mock<IFormFile>();
            mockFormFile.SetupGet(f => f.ContentType).Returns(GetTestFileTypes().First().Type);
            mockFormFile.SetupGet(f => f.FileName).Returns(expected.FileName);

            var mockFileService = new Mock<IFileService<UmkFilesConfiguration>>();
            mockFileService
                .Setup(f => f.UploadAsync(It.IsAny<IFormFile>()))
                .ReturnsAsync(expected.FileName);

            _sut = new UmkFileService(mockContextFactory.Object, _mapper, mockFileService.Object);
            var request = new CreateRequest { Name = expected.Name, SubjectId = 1, TermId = 1, UmkFile = mockFormFile.Object };

            // ACT
            var actual = await _sut.CreateAsync(request);

            // ASSERT
            Assert.Equal(expected, actual.Entity);
        }

        [Fact]
        public async Task CreateAsync_ShouldReturnErrorWhenExceptionInUmkQuery()
        {
            // ARRANGE
            var expected = new UmkFile { Id = 10, Active = true, CreatedAt = new DateTime(2020, 05, 1), FileName = "FileNameNew", FileSize = 1000, FileType = GetTestFileTypes().First(), Name = "NameNew", SubjectId = 1, TermId = 1, UpdatedAt = new DateTime(2020, 05, 2) };

            var mockUmkQuery = new Mock<IUmkFilesQuery>();
            mockUmkQuery
                .Setup(u => u.Create(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<byte>(), It.IsAny<string>(), It.IsAny<double>(), It.IsAny<string>()))
                .ThrowsAsync(new Exception("Mock exception"));

            var mockRaspredelenie = new Mock<IRaspredelenieQuery>();
            mockRaspredelenie.Setup(r => r.CheckTeacherBySubjectAndTerm(It.IsAny<short>(), It.IsAny<int>(), It.IsAny<byte>()))
                .ReturnsAsync(new SPBoolResult { Result = true });

            var mockContextFactory = SetupContextFactory(mockUmkQuery.Object, null, mockRaspredelenie.Object);

            var mockFormFile = new Mock<IFormFile>();
            mockFormFile.SetupGet(f => f.ContentType).Returns(GetTestFileTypes().First().Type);
            mockFormFile.SetupGet(f => f.FileName).Returns(expected.FileName);

            var mockFileService = new Mock<IFileService<UmkFilesConfiguration>>();
            mockFileService
                .Setup(f => f.UploadAsync(It.IsAny<IFormFile>()))
                .ReturnsAsync(expected.FileName);

            _sut = new UmkFileService(mockContextFactory.Object, _mapper, mockFileService.Object);
            var request = new CreateRequest { Name = expected.Name, SubjectId = 1, TermId = 1, UmkFile = mockFormFile.Object };

            // ACT
            var actual = await _sut.CreateAsync(request);

            // ASSERT
            Assert.Equal("Mock exception", actual.Error.Message);
        }

        [Fact]
        public async Task CreateAsync_ShouldReturnErrorWhenDenyInAccess()
        {
            // ARRANGE
            var expected = new UmkFile { Id = 10, Active = true, CreatedAt = new DateTime(2020, 05, 1), FileName = "FileNameNew", FileSize = 1000, FileType = GetTestFileTypes().First(), Name = "NameNew", SubjectId = 1, TermId = 1, UpdatedAt = new DateTime(2020, 05, 2) };

            var mockUmkQuery = new Mock<IUmkFilesQuery>();
            mockUmkQuery
                .Setup(u => u.Create(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<byte>(), It.IsAny<string>(), It.IsAny<double>(), It.IsAny<string>()))
                .ThrowsAsync(new Exception("Mock exception"));

            var mockRaspredelenie = new Mock<IRaspredelenieQuery>();
            mockRaspredelenie.Setup(r => r.CheckTeacherBySubjectAndTerm(It.IsAny<short>(), It.IsAny<int>(), It.IsAny<byte>()))
                .ReturnsAsync(new SPBoolResult { Result = false });

            var mockContextFactory = SetupContextFactory(mockUmkQuery.Object, null, mockRaspredelenie.Object);

            var mockFormFile = new Mock<IFormFile>();
            mockFormFile.SetupGet(f => f.ContentType).Returns(GetTestFileTypes().First().Type);
            mockFormFile.SetupGet(f => f.FileName).Returns(expected.FileName);

            var mockFileService = new Mock<IFileService<UmkFilesConfiguration>>();
            mockFileService
                .Setup(f => f.UploadAsync(It.IsAny<IFormFile>()))
                .ReturnsAsync(expected.FileName);

            _sut = new UmkFileService(mockContextFactory.Object, _mapper, mockFileService.Object);
            var request = new CreateRequest { Name = expected.Name, SubjectId = 1, TermId = 1, UmkFile = mockFormFile.Object };

            // ACT
            var actual = await _sut.CreateAsync(request);

            // ASSERT
            Assert.Equal("Отказ в доступе", actual.Error.Message);
        }

        [Fact]
        public async Task CreateAsync_ShouldReturnErrorWhenExceptionInFileService()
        {
            // ARRANGE
            var expected = new UmkFile { Id = 10, Active = true, CreatedAt = new DateTime(2020, 05, 1), FileName = "FileNameNew", FileSize = 1000, FileType = GetTestFileTypes().First(), Name = "NameNew", SubjectId = 1, TermId = 1, UpdatedAt = new DateTime(2020, 05, 2) };

            var mockRaspredelenie = new Mock<IRaspredelenieQuery>();
            mockRaspredelenie.Setup(r => r.CheckTeacherBySubjectAndTerm(It.IsAny<short>(), It.IsAny<int>(), It.IsAny<byte>()))
                .ReturnsAsync(new SPBoolResult { Result = true });
            var mockContextFactory = SetupContextFactory(null, null, mockRaspredelenie.Object);

            var mockFormFile = new Mock<IFormFile>();
            mockFormFile.SetupGet(f => f.ContentType).Returns(GetTestFileTypes().First().Type);
            mockFormFile.SetupGet(f => f.FileName).Returns(expected.FileName);

            var mockFileService = new Mock<IFileService<UmkFilesConfiguration>>();
            mockFileService
                .Setup(f => f.UploadAsync(It.IsAny<IFormFile>()))
                .ThrowsAsync(new Exception("File is too big"));

            _sut = new UmkFileService(mockContextFactory.Object, _mapper, mockFileService.Object);
            var request = new CreateRequest { Name = expected.Name, SubjectId = 1, TermId = 1, UmkFile = mockFormFile.Object };

            // ACT
            var actual = await _sut.CreateAsync(request);

            // ASSERT
            Assert.Equal("File is too big", actual.Error.Message);
        }

        [Fact]
        public void GetFileById_ShouldReturnFileDto()
        {
            // ARRANGE
            var expectedFileName = GetTestUmkFiles().First().FileName;
            var expectedFileType = GetTestFileTypes().First().Type;
            var expected = new FileDto { FileBytes = new byte[] { 1, 0, 1, 0 }, FileName = expectedFileName, FileType = expectedFileType };

            var mockFileService = new Mock<IFileService<UmkFilesConfiguration>>();
            mockFileService
                .Setup(f => f.GetFileBytes(It.IsAny<string>()))
                .Returns(expected.FileBytes);

            var mockContextFactory = SetupContextFactory(null, null, null);
            _sut = new UmkFileService(mockContextFactory.Object, _mapper, mockFileService.Object);

            // ACT
            var actual = _sut.GetFileById(1);

            // ASSERT
            Assert.Equal(ComparableObject.Convert(expected), ComparableObject.Convert(actual.Entity));
        }

        [Fact]
        public void GetFileById_ShouldReturnErrorWhenNotFound()
        {
            // ARRANGE
            var mockFileService = new Mock<IFileService<UmkFilesConfiguration>>();

            var mockContextFactory = SetupContextFactory(null, null, null);
            _sut = new UmkFileService(mockContextFactory.Object, _mapper, mockFileService.Object);

            // ACT
            var actual = _sut.GetFileById(100);

            // ASSERT
            Assert.Equal("Файл УМК не найден.", actual.Error.Message);
        }

        [Fact]
        public void GetFileById_ShouldReturnErrorWhenExceptionInFileService()
        {
            // ARRANGE
            var mockFileService = new Mock<IFileService<UmkFilesConfiguration>>();
            mockFileService
                .Setup(f => f.GetFileBytes(It.IsAny<string>()))
                .Throws(new Exception("File was not found"));

            var mockContextFactory = SetupContextFactory(null, null, null);
            _sut = new UmkFileService(mockContextFactory.Object, _mapper, mockFileService.Object);

            // ACT
            var actual = _sut.GetFileById(1);

            // ASSERT
            Assert.Equal("File was not found", actual.Error.Message);
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnDataWhenFileIsNotNull()
        {
            // ARRANGE
            var expected = new UmkFile { Id = 1, Active = true, CreatedAt = new DateTime(2020, 05, 1), FileName = "FileNameNew", FileSize = 1000, FileType = GetTestFileTypes().First(), Name = "NameNew", SubjectId = 1, TermId = 1, UpdatedAt = new DateTime(2020, 05, 2) };

            var mockUmkQuery = new Mock<IUmkFilesQuery>();
            mockUmkQuery
                .Setup(u => u.Update(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<double?>(), It.IsAny<string>()))
                .ReturnsAsync(expected);

            var mockRaspredelenie = new Mock<IRaspredelenieQuery>();
            mockRaspredelenie.Setup(r => r.CheckTeacherBySubjectAndTerm(It.IsAny<short>(), It.IsAny<int>(), It.IsAny<byte>()))
                .ReturnsAsync(new SPBoolResult { Result = true });

            var mockContextFactory = SetupContextFactory(mockUmkQuery.Object, null, mockRaspredelenie.Object);

            var mockFormFile = new Mock<IFormFile>();
            mockFormFile.SetupGet(f => f.ContentType).Returns(GetTestFileTypes().First().Type);
            mockFormFile.SetupGet(f => f.FileName).Returns(expected.FileName);

            var mockFileService = new Mock<IFileService<UmkFilesConfiguration>>();
            mockFileService
                .Setup(f => f.UploadAsync(It.IsAny<IFormFile>()))
                .ReturnsAsync(expected.FileName);

            _sut = new UmkFileService(mockContextFactory.Object, _mapper, mockFileService.Object);
            var request = new UpdateRequest { Id = 1, Name = expected.Name, UmkFile = mockFormFile.Object };

            // ACT
            var actual = await _sut.UpdateAsync(request);

            // ASSERT
            Assert.Equal(expected, actual.Entity);
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnDataWhenFileIsNull()
        {
            // ARRANGE
            var expected = GetTestUmkFiles().First();
            expected.Name = "New name";

            var mockUmkQuery = new Mock<IUmkFilesQuery>();
            mockUmkQuery
                .Setup(u => u.Update(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<double?>(), It.IsAny<string>()))
                .ReturnsAsync(expected);

            var mockRaspredelenie = new Mock<IRaspredelenieQuery>();
            mockRaspredelenie.Setup(r => r.CheckTeacherBySubjectAndTerm(It.IsAny<short>(), It.IsAny<int>(), It.IsAny<byte>()))
                .ReturnsAsync(new SPBoolResult { Result = true });

            var mockContextFactory = SetupContextFactory(mockUmkQuery.Object, null, mockRaspredelenie.Object);

            var mockFileService = new Mock<IFileService<UmkFilesConfiguration>>();

            _sut = new UmkFileService(mockContextFactory.Object, _mapper, mockFileService.Object);
            var request = new UpdateRequest { Id = 1, Name = expected.Name, UmkFile = null };

            // ACT
            var actual = await _sut.UpdateAsync(request);

            // ASSERT
            Assert.Equal(expected, actual.Entity);
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnErrorWhenNotFound()
        {
            // ARRANGE
            var mockUmkQuery = new Mock<IUmkFilesQuery>();

            var mockContextFactory = SetupContextFactory(mockUmkQuery.Object, null, null);

            var mockFormFile = new Mock<IFormFile>();

            var mockFileService = new Mock<IFileService<UmkFilesConfiguration>>();

            _sut = new UmkFileService(mockContextFactory.Object, _mapper, mockFileService.Object);
            var request = new UpdateRequest { Id = 100, Name = "SomeName", UmkFile = mockFormFile.Object };

            // ACT
            var actual = await _sut.UpdateAsync(request);

            // ASSERT
            Assert.Equal("Файл УМК не найден.", actual.Error.Message);
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnErrorWhenExceptionInFileService()
        {
            // ARRANGE
            var mockUmkQuery = new Mock<IUmkFilesQuery>();

            var mockRaspredelenie = new Mock<IRaspredelenieQuery>();
            mockRaspredelenie.Setup(r => r.CheckTeacherBySubjectAndTerm(It.IsAny<short>(), It.IsAny<int>(), It.IsAny<byte>()))
                .ReturnsAsync(new SPBoolResult { Result = true });

            var mockContextFactory = SetupContextFactory(mockUmkQuery.Object, null, mockRaspredelenie.Object);

            var mockFormFile = new Mock<IFormFile>();

            var mockFileService = new Mock<IFileService<UmkFilesConfiguration>>();
            mockFileService.Setup(f => f.UploadAsync(It.IsAny<IFormFile>()))
                .ThrowsAsync(new Exception("File is too big"));

            _sut = new UmkFileService(mockContextFactory.Object, _mapper, mockFileService.Object);
            var request = new UpdateRequest { Id = 1, Name = "SomeName", UmkFile = mockFormFile.Object };

            // ACT
            var actual = await _sut.UpdateAsync(request);

            // ASSERT
            Assert.Equal("File is too big", actual.Error.Message);
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnErrorWhenExceptionInUmkQuery()
        {
            // ARRANGE
            var mockUmkQuery = new Mock<IUmkFilesQuery>();
            mockUmkQuery
                .Setup(u => u.Update(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<double?>(), It.IsAny<string>()))
                .ThrowsAsync(new Exception("Mock exception"));

            var mockRaspredelenie = new Mock<IRaspredelenieQuery>();
            mockRaspredelenie.Setup(r => r.CheckTeacherBySubjectAndTerm(It.IsAny<short>(), It.IsAny<int>(), It.IsAny<byte>()))
                .ReturnsAsync(new SPBoolResult { Result = true });

            var mockContextFactory = SetupContextFactory(mockUmkQuery.Object, null, mockRaspredelenie.Object);

            var mockFormFile = new Mock<IFormFile>();
            mockFormFile.SetupGet(f => f.ContentType).Returns(GetTestFileTypes().First().Type);
            mockFormFile.SetupGet(f => f.FileName).Returns("SomeName");

            var mockFileService = new Mock<IFileService<UmkFilesConfiguration>>();
            mockFileService
                .Setup(f => f.UploadAsync(It.IsAny<IFormFile>()))
                .ReturnsAsync("SomeName");

            _sut = new UmkFileService(mockContextFactory.Object, _mapper, mockFileService.Object);
            var request = new UpdateRequest { Id = 1, Name = "SomeName", UmkFile = mockFormFile.Object };

            // ACT
            var actual = await _sut.UpdateAsync(request);

            // ASSERT
            Assert.Equal("Mock exception", actual.Error.Message);
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnErrorWhenDenyInAccess()
        {
            // ARRANGE
            var mockUmkQuery = new Mock<IUmkFilesQuery>();
            mockUmkQuery
                .Setup(u => u.Update(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<double?>(), It.IsAny<string>()))
                .ThrowsAsync(new Exception("Mock exception"));

            var mockRaspredelenie = new Mock<IRaspredelenieQuery>();
            mockRaspredelenie.Setup(r => r.CheckTeacherBySubjectAndTerm(It.IsAny<short>(), It.IsAny<int>(), It.IsAny<byte>()))
                .ReturnsAsync(new SPBoolResult { Result = false });

            var mockContextFactory = SetupContextFactory(mockUmkQuery.Object, null, mockRaspredelenie.Object);

            var mockFormFile = new Mock<IFormFile>();
            mockFormFile.SetupGet(f => f.ContentType).Returns(GetTestFileTypes().First().Type);
            mockFormFile.SetupGet(f => f.FileName).Returns("SomeName");

            var mockFileService = new Mock<IFileService<UmkFilesConfiguration>>();
            mockFileService
                .Setup(f => f.UploadAsync(It.IsAny<IFormFile>()))
                .ReturnsAsync("SomeName");

            _sut = new UmkFileService(mockContextFactory.Object, _mapper, mockFileService.Object);
            var request = new UpdateRequest { Id = 1, Name = "SomeName", UmkFile = mockFormFile.Object };

            // ACT
            var actual = await _sut.UpdateAsync(request);

            // ASSERT
            Assert.Equal("Отказ в доступе", actual.Error.Message);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnTrue()
        {
            // ARRANGE
            var mockUmkQuery = new Mock<IUmkFilesQuery>();
            mockUmkQuery
                .Setup(u => u.Delete(It.IsAny<int>()))
                .ReturnsAsync(1);

            var mockContextFactory = SetupContextFactory(mockUmkQuery.Object, null, null);

            var mockFileService = new Mock<IFileService<UmkFilesConfiguration>>();

            _sut = new UmkFileService(mockContextFactory.Object, _mapper, mockFileService.Object);

            // ACT
            var actual = await _sut.DeleteAsync(1);

            // ASSERT
            Assert.True(actual.Entity);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnErrorWhenExceptionInUmkQuery()
        {
            // ARRANGE
            var mockUmkQuery = new Mock<IUmkFilesQuery>();
            mockUmkQuery
                .Setup(u => u.Delete(It.IsAny<int>()))
                .ThrowsAsync(new Exception("Mock exception"));

            var mockContextFactory = SetupContextFactory(mockUmkQuery.Object, null, null);

            var mockFileService = new Mock<IFileService<UmkFilesConfiguration>>();

            _sut = new UmkFileService(mockContextFactory.Object, _mapper, mockFileService.Object);

            // ACT
            var actual = await _sut.DeleteAsync(1);

            // ASSERT
            Assert.Equal("Mock exception", actual.Error.Message);
        }

        [Fact]
        public void GetById_ShouldReturnData()
        {
            var mockContextFactory = SetupContextFactory(null, null, null);

            var mockFileService = new Mock<IFileService<UmkFilesConfiguration>>();

            _sut = new UmkFileService(mockContextFactory.Object, _mapper, mockFileService.Object);
            var expected = GetTestUmkFiles().First();
            expected.FileTypeId = 1;

            // ACT
            var actual = _sut.GetById(1);

            // ASSERT
            Assert.Equal(ComparableObject.Convert(expected), ComparableObject.Convert(actual.Entity));
        }

        [Fact]
        public void GetById_ShouldReturnErrorWhenNotFound()
        {
            var mockContextFactory = SetupContextFactory(null, null, null);

            var mockFileService = new Mock<IFileService<UmkFilesConfiguration>>();

            _sut = new UmkFileService(mockContextFactory.Object, _mapper, mockFileService.Object);

            // ACT
            var actual = _sut.GetById(100);

            // ASSERT
            Assert.Equal("Файл УМК не найден.", actual.Error.Message);
        }
    }
}
