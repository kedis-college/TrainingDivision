using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TrainingDivisionKedis.Core.Models
{
    public class Group
    {
        [Column("Code")]
        public short Id { get; set; }
        [Column("Grup")]
        public string Name { get; set; }
        public short Specialty { get; set; }
        public bool Graduated { get; set; }
    }
}
