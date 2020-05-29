using System;
using System.Collections.Generic;
using System.Text;

namespace TrainingDivisionKedis.BLL.DTO.Raspredelenie
{
    public class GetVedomostListRequest
    {
        public GetVedomostListRequest(byte yearId, byte termId, int userId)
        {
            YearId = yearId;
            TermId = termId;
            UserId = userId;
        }

        public byte YearId { get; set; }
        public byte TermId { get; set; }
        public int UserId { get; set; }
    }
}
