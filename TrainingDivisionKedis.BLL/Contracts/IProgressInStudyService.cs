using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TrainingDivisionKedis.BLL.Common;
using TrainingDivisionKedis.BLL.DTO.ProgressInStudy;
using TrainingDivisionKedis.Core.SPModels.ProgressInStudy;

namespace TrainingDivisionKedis.BLL.Contracts
{
    public interface IProgressInStudyService
    {
        Task<OperationDetails<ProgressInStudyListDto>> GetByRaspredelenieAndUserAsync(ProgressInStudyRequest request);

        Task<OperationDetails<List<SPProgressInStudyGetByRaspredelenieAndUser>>> GetByRaspredelenieAsync(int id);

        Task<OperationDetails<SPProgressInStudyGetByRaspredelenieAndUser>> UpdateAsync(ProgressInStudyUpdateRequest request);

        Task<OperationDetails<List<TotalsDto>>> GetTotalsByRaspredelenieAsync(int id);

        Task<OperationDetails<List<ProgressInStudyOfStudent>>> GetByStudentAsync(int id);

        Task<OperationDetails<SPProgressInStudyGetTotalsByStudent>> GetTotalsOfStudentAsync(int id);
    }
}
