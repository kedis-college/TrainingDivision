using System;
using System.Collections.Generic;
using System.Text;

namespace TrainingDivisionKedis.BLL.DTO.Curriculum
{
    public class GetSubjectsOfStudentRequest
    {
        public GetSubjectsOfStudentRequest(byte termId, int userId)
        {
            TermId = termId;
            UserId = userId;
        }

        public byte TermId { get; set; }
        public int UserId { get; set; }
    }
}
