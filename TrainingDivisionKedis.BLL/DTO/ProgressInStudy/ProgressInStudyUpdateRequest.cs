using System;
using System.Collections.Generic;
using System.Text;

namespace TrainingDivisionKedis.BLL.DTO.ProgressInStudy
{
    public class ProgressInStudyUpdateRequest
    {
        public int NumRec { get; set; }
        public byte Mod1 { get; set; }
        public byte Mod2 { get; set; }
        public byte Itog { get; set; }
        public byte Dop { get; set; }
        public DateTime? Date { get; set; }
        public short UserId { get; set; }
    }
}
