using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TrainingDivisionKedis.Core.Models
{
    [Table("Term")]
    public class TermSeason : Entity
    {
        [Key]
        [Column("N")]
        public byte Id { get; set; }

        [Column("Term")]
        public string Name { get; set; }

        ICollection<Term> SeasonTerms { get; set; }
    }
}
