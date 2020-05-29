using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrainingDivisionKedis.Core.Models;
using TrainingDivisionKedis.Core.SPModels.ControlSchedule;

namespace TrainingDivisionKedis.Core.Contracts.Queries
{
    public interface IControlScheduleQuery 
    {
        Task<ControlSchedule> Create(byte yearId, byte seasonId, short userId,
            DateTime dateStart, DateTime? dateEnd, DateTime? mod1DateStart, DateTime? mod1DateEnd,
            DateTime? mod2DateStart, DateTime? mod2DateEnd, DateTime? itogDateStart, DateTime? itogDateEnd);

        Task<List<ControlSchedule>> GetByYearAndSeason(byte yearId, byte seasonId);

        Task<ControlSchedule> GetById(int id);

        Task<ControlSchedule> Update(int id, short userId,
            DateTime dateStart, DateTime? dateEnd, DateTime? mod1DateStart, DateTime? mod1DateEnd,
            DateTime? mod2DateStart, DateTime? mod2DateEnd, DateTime? itogDateStart, DateTime? itogDateEnd);

        Task<SPControlScheduleGetEditable> GetEditable(int raspredelenieId);
    }
}
