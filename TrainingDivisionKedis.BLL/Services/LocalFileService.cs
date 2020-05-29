using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using TrainingDivisionKedis.BLL.Common;
using TrainingDivisionKedis.BLL.Contracts;

[assembly: InternalsVisibleTo("TrainingDivisionKedis.BLL.Tests")]
namespace TrainingDivisionKedis.BLL.Services
{
    public class LocalFileService<T> : IFileService<T> where T: FilesConfiguration, new()
    {       
        private static readonly double BYTES_IN_KB = 0.001;
        private readonly T _filesConfiguration;

        public LocalFileService(IOptions<T> filesConfiguration)
        {
            _filesConfiguration = filesConfiguration?.Value ?? throw new ArgumentNullException(nameof(filesConfiguration));
            if (!Directory.Exists(filesConfiguration.Value.Directory))
                throw new DirectoryNotFoundException("Директорий \"" + filesConfiguration.Value.Directory + "\" не найден.");          
        }

        public byte[] GetFileBytes(string fileName)
        {
            var path = Path.Combine(_filesConfiguration.Directory, fileName);
            if (!File.Exists(path))
                throw new FileNotFoundException("Файл \"" + path + "\" не найден.");
            return File.ReadAllBytes(path);
        }

        public async Task<string> UploadAsync(IFormFile formFile)
        {        
            var fileLengthInKB = ConvertBytesToKB(formFile.Length);
            if (fileLengthInKB > _filesConfiguration.MaxSize)
                throw new Exception("Размер файла должен быть не более " + _filesConfiguration.MaxSize + "КБ.");

            var fileName = GetNewFileName(formFile.FileName);
            var path = Path.Combine(_filesConfiguration.Directory, fileName);
            await WriteFile(formFile, path);
            return fileName;
        }

        private double ConvertBytesToKB(double bytes)
        {
            return bytes * BYTES_IN_KB;
        }

        private string GetNewFileName(string fileName)
        {
            var fileNameArray = fileName.Split('.');
            var newFileName = fileNameArray.First();
            var fileExt = fileNameArray.Last();
            return newFileName + DateTime.Now.ToBinary() + "." + fileExt;
        }

        private async Task WriteFile(IFormFile formFile, string path)
        {            
            using (var stream = new FileStream(path, FileMode.Create))
            {
                await formFile.CopyToAsync(stream);
            }
        }
    }
}
