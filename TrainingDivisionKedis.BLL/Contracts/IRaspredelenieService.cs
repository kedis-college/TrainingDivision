using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TrainingDivisionKedis.BLL.Common;
using TrainingDivisionKedis.BLL.DTO.Raspredelenie;
using TrainingDivisionKedis.Core.SPModels.Raspredelenie;

namespace TrainingDivisionKedis.BLL.Contracts
{
    public interface IRaspredelenieService
    {
        Task<OperationDetails<List<SPRaspredelenieGetByYearAndTermAndUser>>> GetVedomostListAsync(GetVedomostListRequest request);

        Task<OperationDetails<List<SPSubjectsGetByYearAndTermAndUser>>> GetSubjectsByUserAndTermAndYearAsync(GetVedomostListRequest request);

        Task<OperationDetails<List<SPRaspredelenieOfYear>>> GetByYearAsync(int id);
    }
}
