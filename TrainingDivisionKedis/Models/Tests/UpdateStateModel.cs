using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrainingDivisionKedis.Models.Tests
{
    public class UpdateStateModel
    {
        public int Id { get; set; }
        public bool? Visible { get; set; }
        public bool? Draft { get; set; }
        public bool? Active { get; set; }
        public int SubjectId { get; set; }
        public byte TermId { get; set; }
    }
}
