using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TrainingDivisionKedis.BLL.DTO.ControlSchedule
{
    public class ControlScheduleListDto
    {
        public int Id { get; set; }
        public byte YearId { get; set; }
        public byte SeasonId { get; set; }
        [Display(Name = "Дата начала семестра")]
        public DateTime? DateStart { get; set; }
        [Display(Name = "Дата конца семестра")]
        public DateTime? DateEnd { get; set; }
        public short UserId { get; set; }
        [Display(Name = "Дата создания")]
        public DateTime CreatedAt { get; set; }
        [Display(Name = "Дата обновления")]
        public DateTime? UpdatedAt { get; set; }
    }
}
