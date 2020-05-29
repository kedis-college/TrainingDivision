using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TrainingDivisionKedis.Core.SPModels.ActivityOfTeachers
{
    public class SPTeacherGetAll
    {
        public short Id { get; set; }
        [Display(Name = "Преподаватель")]
        public string Name { get; set; }
        public int Nom { get; set; }
        [Display(Name = "Должность")]
        public string Post { get; set; }
    }
}
