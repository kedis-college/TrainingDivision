using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrainingDivisionKedis.Core.Models
{
    [Table("Year")]
    public class Year : Entity
    {
        [Key]
        [Column("Nom")]
        public byte Id { get; set; }

        [Column("Year")]
        public string Name { get; set; }

        [Column("Currient")]
        public bool? Current { get; set; }
    }
}
