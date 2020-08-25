using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using TrainingDivisionKedis.BLL.Common;
using TrainingDivisionKedis.BLL.Services;
using Xunit;

namespace TrainingDivisionKedis.BLL.Tests
{
    public class LocalFileServiceTests 
    {
        private LocalFileService<FilesConfiguration> _sut;

        [Fact]
        public void Constructor_ShouldThrowExceptionWhenNullArgument()
        {
            Assert.Throws<ArgumentNullException>(()=> { var _sut = new LocalFileService<FilesConfiguration>(null); });
        }

        [Fact]
        public void Constructor_ShouldThrowExceptionWhenDirectoryNotExists()
        {
            // ARRANGE
            var filesConfiguration = new FilesConfiguration { Directory = @"C:\ABC\", MaxSize = 10000 };
            IOptions<FilesConfiguration> options = Options.Create(filesConfiguration);

            // ASSERT
            Assert.Throws<DirectoryNotFoundException>(() => { var _sut = new LocalFileService<FilesConfiguration>(options); });
        }

        [Fact]
        public void GetFileBytes_ShouldReturnBytes()
        {
            // ARRANGE
            var filesConfiguration = new FilesConfiguration { Directory = @"C:\Users\E7450\source\repos\TrainingDivision\TrainingDivisionKedis.BLL.Tests\bin\Debug\netcoreapp2.2\", MaxSize = 10000 };
            IOptions<FilesConfiguration> options = Options.Create(filesConfiguration);
            _sut = new LocalFileService<FilesConfiguration>(options);

            // ACT
            var actual =_sut.GetFileBytes("TestTextFile.txt");

            // ASSERT
            Assert.True(actual.Length > 0);
        }

        [Fact]
        public void GetFileBytes_ShouldThrowErrorWhenFileNotFound()
        {
            // ARRANGE
            var path = @"C:\Users\E7450\source\repos\TrainingDivision\TrainingDivisionKedis.BLL.Tests\bin\Debug\netcoreapp2.2\";
            var filesConfiguration = new FilesConfiguration { Directory = path, MaxSize = 10000 };
            IOptions<FilesConfiguration> options = Options.Create(filesConfiguration);
            _sut = new LocalFileService<FilesConfiguration>(options);

            // ACT
            Assert.Throws<FileNotFoundException>(()=>_sut.GetFileBytes("SomeFileName.txt"));
        }

        [Fact]
        public async Task UploadAsync_ShouldReturnFileNameWhenFileLenghtIsLessThanMax()
        {
            // ARRANGE
            var filesConfiguration = new FilesConfiguration { Directory = @"C:\Users\E7450\source\repos\TrainingDivision\TrainingDivisionKedis.BLL.Tests\bin\Debug\netcoreapp2.2\", MaxSize = 10000 };
            IOptions<FilesConfiguration> options = Options.Create(filesConfiguration);
            _sut = new LocalFileService<FilesConfiguration>(options);

            var file = new Mock<IFormFile>();
            var sourceImg = File.OpenRead(@"C:\Users\E7450\Pictures\handMade\64d735876ce855d858a742001e0585ea.jpg");
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms);
            writer.Write(sourceImg);
            writer.Flush();
            ms.Position = 0;
            var fileName = "QQ.png";
            file.Setup(f => f.FileName).Returns(fileName).Verifiable();
            file.Setup(f => f.Length).Returns(9999000).Verifiable();
            file.Setup(_ => _.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
                .Returns((Stream stream, CancellationToken token) => ms.CopyToAsync(stream))
                .Verifiable();

            // ACT
            var actual = await _sut.UploadAsync(file.Object);

            // ASSERT
            Assert.Contains("QQ", actual);
            Assert.Contains(".png", actual);
        }

        [Fact]
        public async Task UploadAsync_ShouldThrowExceptionWhenFileLenghtIsBiggerThanMax()
        {
            // ARRANGE
            var filesConfiguration = new FilesConfiguration { Directory = @"C:\Users\E7450\source\repos\TrainingDivision\TrainingDivisionKedis.BLL.Tests\bin\Debug\netcoreapp2.2\", MaxSize = 10000 };
            IOptions<FilesConfiguration> options = Options.Create(filesConfiguration);
            _sut = new LocalFileService<FilesConfiguration>(options);

            var file = new Mock<IFormFile>();
            file.Setup(f => f.Length).Returns(10001000).Verifiable();

            try
            {
                // ACT
                var actual = await _sut.UploadAsync(file.Object);
            }
            catch (Exception ex)
            {
                // ASSERT
                Assert.Equal("Размер файла должен быть не более " + filesConfiguration.MaxSize + "КБ.", ex.Message);
            }
        }

        [Fact]
        public async Task UploadAsync_ShouldReturnFileNameWhenFileLenghtIsEqualToMax()
        {
            // ARRANGE
            var filesConfiguration = new FilesConfiguration { Directory = @"C:\Users\E7450\source\repos\TrainingDivision\TrainingDivisionKedis.BLL.Tests\bin\Debug\netcoreapp2.2\", MaxSize = 10000 };
            IOptions<FilesConfiguration> options = Options.Create(filesConfiguration);
            _sut = new LocalFileService<FilesConfiguration>(options);

            var file = new Mock<IFormFile>();
            var sourceImg = File.OpenRead(@"C:\Users\E7450\Pictures\handMade\64d735876ce855d858a742001e0585ea.jpg");
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms);
            writer.Write(sourceImg);
            writer.Flush();
            ms.Position = 0;
            var fileName = "QQ.png";
            file.Setup(f => f.FileName).Returns(fileName).Verifiable();
            file.Setup(f => f.Length).Returns(10000000).Verifiable();
            file.Setup(_ => _.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
                .Returns((Stream stream, CancellationToken token) => ms.CopyToAsync(stream))
                .Verifiable();

            // ACT
            var actual = await _sut.UploadAsync(file.Object);

            // ASSERT
            Assert.Contains("QQ", actual);
            Assert.Contains(".png", actual);
        }
    }
}
