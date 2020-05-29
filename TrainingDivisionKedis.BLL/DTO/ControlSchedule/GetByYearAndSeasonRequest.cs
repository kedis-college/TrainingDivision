using System;
using System.Collections.Generic;
using System.Text;

namespace TrainingDivisionKedis.BLL.DTO.ControlSchedule
{
    public class GetByYearAndSeasonRequest
    {
        public GetByYearAndSeasonRequest(byte yearId, byte seasonId)
        {
            YearId = yearId;
            SeasonId = seasonId;
        }

        public byte YearId { get; set; }
        public byte SeasonId { get; set; }
    }
}
