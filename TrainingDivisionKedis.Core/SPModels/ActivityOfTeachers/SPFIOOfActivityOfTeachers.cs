using System.ComponentModel.DataAnnotations;

namespace TrainingDivisionKedis.Core.SPModels.ActivityOfTeachers
{
    public class SPFIOOfActivityOfTeachers
    {
        public int Nom { get; set; }
        [Display(Name = "Преподаватель")]
        public string FIO { get; set; }
        [Display(Name = "Преподаватель")]
        public string FIO_Short { get; set; }
        [Display(Name = "Должность")]
        public string Post { get; set; }
    }
}
