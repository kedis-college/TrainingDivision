using System;
using System.Collections.Generic;
using System.Text;
using TrainingDivisionKedis.Core.SPModels.Tests;

namespace TrainingDivisionKedis.BLL.DTO.TestsClient
{
    public class TestListDto
    {
        public List<SPTestGetByStudentAndSubjectAndTerm> Tests { get; set; }
        public string SubjectName { get; set; }
    }
}
