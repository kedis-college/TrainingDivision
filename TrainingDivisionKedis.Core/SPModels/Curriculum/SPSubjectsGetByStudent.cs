using System;
using System.Collections.Generic;
using System.Text;

namespace TrainingDivisionKedis.Core.SPModels.Curriculum
{
    public class SPSubjectsGetByStudent
    {
        public int Id { get; set; }
        public string Subject { get; set; }
        public byte Semestr { get; set; }
    }
}
