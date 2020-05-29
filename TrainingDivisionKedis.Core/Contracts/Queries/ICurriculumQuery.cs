using System.Collections.Generic;
using System.Threading.Tasks;
using TrainingDivisionKedis.Core.SPModels;
using TrainingDivisionKedis.Core.SPModels.Curriculum;

namespace TrainingDivisionKedis.Core.Contracts.Queries
{
    public interface ICurriculumQuery
    {
        Task<List<SPSubjectsGetByStudent>> GetByStudentAndTerm(int studentId, byte termId);

        Task<SPGetName> GetNameById(int id);

        Task<SPBoolResult> CheckStudentBySubjectAndTerm(int studentId, int subjectId, byte termId);
    }
}
