using System;
using System.Collections.Generic;
using System.Text;

namespace TrainingDivisionKedis.Core.SPModels.Raspredelenie
{
    public class SPSubjectsGetByYearAndTermAndUser
    {
        public byte YearId { get; set; }
        public string SubjectName { get; set; }
        public int SubjectId { get; set; }
        public byte Semestr { get; set; }
    }
}
