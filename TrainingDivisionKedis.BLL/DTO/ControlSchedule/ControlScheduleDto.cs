using System;
using System.ComponentModel.DataAnnotations;
using TrainingDivisionKedis.Core.Models;

namespace TrainingDivisionKedis.BLL.DTO.ControlSchedule
{
    public class ControlScheduleDto
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "Учебный год")]
        public byte YearId { get; set; }
        [Required]
        [Display(Name = "Семестр")]
        public byte SeasonId { get; set; }
        [Required]
        [Display(Name = "Дата начала семестра")]
        public DateTime? DateStart { get; set; }
        [Display(Name = "Дата конца семестра")]
        public DateTime? DateEnd { get; set; }
        [Display(Name = "Начало 1 модуля")]
        public DateTime? Mod1DateStart { get; set; }
        [Display(Name = "Конец 1 модуля")]
        public DateTime? Mod1DateEnd { get; set; }
        [Display(Name = "Начало 2 модуля")]
        public DateTime? Mod2DateStart { get; set; }
        [Display(Name = "Конец 2 модуля")]
        public DateTime? Mod2DateEnd { get; set; }
        [Display(Name = "Начало итогового контроля")]
        public DateTime? ItogDateStart { get; set; }
        [Display(Name = "Конец итогового контроля")]
        public DateTime? ItogDateEnd { get; set; }
        public short UserId { get; set; }

        public Year Year { get; set; }
        public TermSeason Season { get; set; }
    }
}
