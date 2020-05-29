using System;
using System.Collections.Generic;
using System.Text;
using TrainingDivisionKedis.Core.Models;
using TrainingDivisionKedis.Core.SPModels.Students;

namespace TrainingDivisionKedis.BLL.DTO.TestsAdmin
{
    public class TestResultDto
    {
        public TestResult TestResult { get; set; }
        public SPStudentsGetWithSpeciality Student { get; set; } 
    }
}
