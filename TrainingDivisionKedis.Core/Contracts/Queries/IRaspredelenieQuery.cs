using System.Collections.Generic;
using System.Threading.Tasks;
using TrainingDivisionKedis.Core.SPModels;
using TrainingDivisionKedis.Core.SPModels.Raspredelenie;

namespace TrainingDivisionKedis.Core.Contracts.Queries
{
    public interface IRaspredelenieQuery
    {
        Task<List<SPSubjectsGetByYearAndTermAndUser>> GetSubjectsByYearAndTermAndUser(byte yearId, byte termId, int userId);

        Task<List<SPRaspredelenieGetByYearAndTermAndUser>> GetByYearAndTermAndUser(byte yearId, byte termId, int userId);

        Task<List<SPRaspredelenieOfYear>> GetByYear(int id);

        Task<SPBoolResult> CheckTeacherBySubjectAndTerm(short teacherId, int subjectId, byte termId);
    }
}
