using System.Collections.Generic;
using System.Threading.Tasks;
using TrainingDivisionKedis.BLL.Common;
using TrainingDivisionKedis.BLL.DTO.ControlSchedule;

namespace TrainingDivisionKedis.BLL.Contracts
{
    public interface IControlScheduleService
    {
        Task<OperationDetails<List<ControlScheduleListDto>>> GetByYearAndSeasonAsync(GetByYearAndSeasonRequest request);
        Task<OperationDetails<ControlScheduleDto>> GetByIdAsync(int id);
        Task<OperationDetails<ControlScheduleDto>> CreateAsync(ControlScheduleDto request);
        Task<OperationDetails<ControlScheduleDto>> UpdateAsync(ControlScheduleDto request);
    }
}
