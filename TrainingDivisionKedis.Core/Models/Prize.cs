using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TrainingDivisionKedis.Core.Models
{
    [Table("Prizes")]
    public class Prize
    {
        public Prize()
        {
        }

        public Prize(Prize prizeRecord)
        {
            ID = prizeRecord.ID;
            Name = prizeRecord.Name;
            Ot = prizeRecord.Ot;
            Do = prizeRecord.Do;
        }

        [Key]
        public byte ID { get; set; }
        [Column("Prize")]
        public string Name { get; set; }
        public byte? Ot { get; set; }
        public byte? Do { get; set; }
    }
}
