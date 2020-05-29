using System.ComponentModel.DataAnnotations;

namespace TrainingDivisionKedis.Core.SPModels.Students
{
    public class SPStudentsGetWithSpeciality
    {
        public int Id { get; set; }
        [Display(Name="Студент")]
        public string FullName { get; set; }
        [Display(Name = "Группа")]
        public string Group { get; set; }
        [Display(Name = "Специальность")]
        public string Speciality { get; set; }
        [Display(Name = "Курс")]
        public byte? Course { get; set; }
    }
}
