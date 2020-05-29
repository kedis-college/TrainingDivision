using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TrainingDivisionKedis.Core.Models
{
    [Table("Raspredelenie")]
    public class Raspredelenie : Entity
    {
        [Key]
        public int Nom { get; set; }
        public byte Year { get; set; }
        public short Group { get; set; }
        public byte Podgruppa { get; set; }
        public int Sub { get; set; }
        public byte Sem { get; set; }
        public byte VidZan { get; set; }
        public int Hours { get; set; }
        public double Exam { get; set; }
        public double Kons { get; set; }
        public double KursRab { get; set; }
        public int? Teacher { get; set; }
        public bool? Obyedinenie { get; set; }
    }
}
