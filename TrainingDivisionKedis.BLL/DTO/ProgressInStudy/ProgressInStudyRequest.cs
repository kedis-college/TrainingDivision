using System;
using System.Collections.Generic;
using System.Text;

namespace TrainingDivisionKedis.BLL.DTO.ProgressInStudy
{
    public class ProgressInStudyRequest
    {
        public ProgressInStudyRequest(int raspredelenieId, int userId)
        {
            RaspredelenieId = raspredelenieId;
            UserId = userId;
        }

        public int RaspredelenieId { get; set; }
        public int UserId { get; set; }
    }
}
