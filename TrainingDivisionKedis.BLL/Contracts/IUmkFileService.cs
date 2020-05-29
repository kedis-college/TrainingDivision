using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TrainingDivisionKedis.BLL.Common;
using TrainingDivisionKedis.BLL.DTO;
using TrainingDivisionKedis.BLL.DTO.UmkFiles;
using TrainingDivisionKedis.Core.Models;

namespace TrainingDivisionKedis.BLL.Contracts
{
    public interface IUmkFileService
    {
        Task<OperationDetails<GetBySubjectAndTermResponse>> GetBySubjectAndTermAsync(GetBySubjectAndTermRequest request);
        Task<OperationDetails<UmkFile>> CreateAsync(CreateRequest request);
        OperationDetails<FileDto> GetFileById(int id);
        OperationDetails<UmkFile> GetById(int id);
        Task<OperationDetails<UmkFile>> UpdateAsync(UpdateRequest request);
        Task<OperationDetails<bool>> DeleteAsync(int id);
    }
}
