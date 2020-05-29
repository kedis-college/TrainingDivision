using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrainingDivisionKedis.Core.SPModels.ProgressInStudy;

namespace TrainingDivisionKedis.Core.Contracts.Queries
{
    public interface IProgressInStudyQuery 
    {
        Task<List<SPProgressInStudyGetByRaspredelenieAndUser>> GetByRaspredelenieAndUser(int raspredelenieId, int userId);

        Task<List<SPProgressInStudyGetByRaspredelenieAndUser>> GetByRaspredelenie(int raspredelenieId);

        Task<List<SPProgressInStudyGetByStudent>> GetByStudentAndTerm(int id, byte term);

        Task<List<SPProgressInStudyGetTotals>> GetTotalsByRaspredelenie(int id);

        Task<SPProgressInStudyGetByRaspredelenieAndUser> Update(int id, byte mod1, byte mod2, byte dop, byte itog, short userId, DateTime? date);

        Task<SPProgressInStudyGetTotalsByStudent> GetTotalsByStudent(int id);
    }
}
