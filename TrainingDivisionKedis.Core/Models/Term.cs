using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TrainingDivisionKedis.Core.Models
{
    [Table("Terms")]
    public class Term : Entity
    {
        [Key]
        [Column("Term")]
        public byte Id { get; set; }

        [ForeignKey("Season")]
        public byte SeasonId { get; set; }

        public TermSeason Season { get; set; }
    }
}
