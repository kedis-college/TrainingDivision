using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TrainingDivisionKedis.BLL.Common;

namespace TrainingDivisionKedis.BLL.Contracts
{
    public interface IFileService<T> where T :FilesConfiguration
    {
        Task<string> UploadAsync(IFormFile formFile);
        byte[] GetFileBytes(string name);
    }
}
