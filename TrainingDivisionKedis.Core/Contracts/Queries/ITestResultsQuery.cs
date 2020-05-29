using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TrainingDivisionKedis.Core.Models;
using TrainingDivisionKedis.Core.SPModels;

namespace TrainingDivisionKedis.Core.Contracts.Queries
{
    public interface ITestResultsQuery
    {
        Task<TestResult> Create(int testId, int studentId);
        Task<List<Group>> GetGroups(int testId);
        Task<List<SPTestResultsGetWithStudents>> GetWithStudents(int testId, int groupId);
        Task<int> Start(int testResultId, DateTime startedAt);
        Task<int> Finish(int testResultId, DateTime finishedAt);
    }
}
