using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TrainingDivisionKedis.BLL.Common;
using TrainingDivisionKedis.BLL.DTO.TestsAdmin;
using TrainingDivisionKedis.BLL.DTO.UmkFiles;
using TrainingDivisionKedis.Core.Models;

namespace TrainingDivisionKedis.BLL.Contracts
{
    public interface ITestAdminService
    {
        Task<OperationDetails<bool>> CreateAsync(TestCreateDto request);
        Task<OperationDetails<TestListDto>> GetBySubjectAndTermAsync(GetBySubjectAndTermRequest request);
        Task<OperationDetails<Test>> GetByIdAsync(int id);
        Task<OperationDetails<bool>> UpdateVisibilityAsync(TestStateDto request);
        Task<OperationDetails<bool>> DeleteAsync(int id);
        Task<OperationDetails<TestResultsListDto>> GetResultsByTestIdAsync(int id);
        Task<OperationDetails<TestResultDto>> GetResultByIdAsync(int id);
        Task<OperationDetails<bool>> UpdateAsync(Test test);
        Task<OperationDetails<List<TestResult>>> GetResultsBySubjectAndTermAndStudentAsync(GetBySubjectAndTermRequest request);
    }
}
