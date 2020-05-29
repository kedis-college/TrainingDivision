using System;
using System.Collections.Generic;
using System.Text;

namespace TrainingDivisionKedis.BLL.DTO.UmkFiles
{
    public class GetBySubjectAndTermRequest
    {
        public GetBySubjectAndTermRequest()
        {
        }

        public GetBySubjectAndTermRequest(int subjectId, byte termId)
        {
            SubjectId = subjectId;
            TermId = termId;
        }

        public int UserId { get; set; }
        public int SubjectId { get; set; }
        public byte TermId { get; set; }
    }
}
