using System.Collections.Generic;
using System.Threading.Tasks;
using TrainingDivisionKedis.BLL.Common;
using TrainingDivisionKedis.BLL.DTO.TestsClient;
using TrainingDivisionKedis.BLL.DTO.UmkFiles;
using TrainingDivisionKedis.Core.Models;

namespace TrainingDivisionKedis.BLL.Contracts
{
    public interface ITestClientService
    {
        Task<OperationDetails<TestListDto>> GetBySubjectAndTermAsync(GetBySubjectAndTermRequest request);
        Task<OperationDetails<TestDto>> GetByIdAsync(int id, int userId);
        Task<OperationDetails<bool>> Start(TestStartRequest request);
        Task<OperationDetails<TestResult>> Finish(int id);
        Task<OperationDetails<bool>> CreateResultItem(TestResultItem testResultItem);
    }
}
