using System;
using System.Collections.Generic;
using System.Text;
using TrainingDivisionKedis.Core.Models;

namespace TrainingDivisionKedis.BLL.DTO.TestsAdmin
{
    public class TestListDto
    {
        public List<Test> Tests { get; set; }
        public string SubjectName { get; set; }
    }
}
