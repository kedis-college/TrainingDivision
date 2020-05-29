using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TrainingDivisionKedis.Core.Models
{
    [Table("Director_Name")]
    public class DirectorName
    {
        [Key]
        public byte Nom { get; set; }
        public string Director { get; set; }
        public string ZamDirector { get; set; }
        public string NachUchChasti { get; set; }
    }
}
