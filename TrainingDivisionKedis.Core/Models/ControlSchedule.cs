using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TrainingDivisionKedis.Core.Models
{
    public class ControlSchedule
    {
        public int Id { get; set; }
        [ForeignKey("Year")]
        public byte YearId { get; set; }
        [ForeignKey("Season")]
        public byte SeasonId { get; set; }       
        public DateTime DateStart { get; set; }
        public DateTime? DateEnd { get; set; }
        public short UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? Mod1DateStart { get; set; }
        public DateTime? Mod1DateEnd { get; set; }
        public DateTime? Mod2DateStart { get; set; }
        public DateTime? Mod2DateEnd { get; set; }
        public DateTime? ItogDateStart { get; set; }
        public DateTime? ItogDateEnd { get; set; }

        public Year Year { get; set; }
        public TermSeason Season { get; set; }
    }
}
