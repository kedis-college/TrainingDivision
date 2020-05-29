using System.Collections.Generic;
using System.Threading.Tasks;
using TrainingDivisionKedis.Core.Models;
using TrainingDivisionKedis.Core.SPModels.Tests;

namespace TrainingDivisionKedis.Core.Contracts.Queries
{
    public interface ITestsQuery
    {
        Task<Test> Create(string name, int subjectId, byte termId, short questionsPerTest, short timeLimit);
        Task<int> UpdateState(int id, bool? visibility, bool? draft, bool? active);
        Task<List<SPTestGetByStudentAndSubjectAndTerm>> GetByStudentAndSubjectAndTerm(int studentId, int subjectId, byte termId);
        Task<int> Delete(int id);
    }
}
