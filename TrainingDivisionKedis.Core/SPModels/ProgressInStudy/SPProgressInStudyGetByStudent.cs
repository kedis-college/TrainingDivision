using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TrainingDivisionKedis.Core.SPModels.ProgressInStudy
{
    public class SPProgressInStudyGetByStudent
    {
        public int NumRec { get; set; }
        public int Student { get; set; }
        public int Subject { get; set; }
        public byte Term { get; set; }
        [Column("1-mod")]
        public byte Mod1 { get; set; }
        [Column("2-mod")]
        public byte Mod2 { get; set; }
        public byte Itog { get; set; }
        public byte Dop { get; set; }
        public byte Prize { get; set; }
        public byte Ball { get; set; }
        public DateTime? Date { get; set; }
        public string PrizeName { get; set; }
        public string SubjectName { get; set; }
        public short Credits { get; set; }
    }
}
