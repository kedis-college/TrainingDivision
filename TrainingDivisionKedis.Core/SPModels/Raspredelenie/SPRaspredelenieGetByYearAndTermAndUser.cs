using System;
using System.Collections.Generic;
using System.Text;

namespace TrainingDivisionKedis.Core.SPModels.Raspredelenie
{
    public class SPRaspredelenieGetByYearAndTermAndUser
    {
        public int Id { get; set; }
        public byte YearId { get; set; }
        public string YearName { get; set; }
        public string SpecialityName { get; set; }
        public string GroupName { get; set; }
        public short GroupId { get; set; }
        public string SubjectName { get; set; }
        public int SubjectId { get; set; }
        public int TeacherId { get; set; }
        public byte Semestr { get; set; }
    }
}
