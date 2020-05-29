using System;
using System.Collections.Generic;
using System.Text;
using TrainingDivisionKedis.Core.Models;

namespace TrainingDivisionKedis.BLL.DTO.ProgressInStudy
{
    public class PrizeDto : Prize
    {
        public PrizeDto() : base()
        {
        }

        public PrizeDto(Prize prizeRecord) : base(prizeRecord)
        {            
        }
    }
}
