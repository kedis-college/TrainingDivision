using System;
using System.Collections.Generic;
using System.Text;

namespace TrainingDivisionKedis.BLL.DTO.TestsAdmin
{
    public class TestStateDto
    {
        public int Id { get; set; }
        public bool? Visible { get; set; }
        public bool? Draft { get; set; }
        public bool? Active { get; set; }
    }
}
