using System;
using System.Collections.Generic;
using System.Text;
using TrainingDivisionKedis.Core.Models;
using TrainingDivisionKedis.Core.SPModels;

namespace TrainingDivisionKedis.BLL.DTO.TestsAdmin
{
    public class TestResultsListDto
    {
        public TestResultsListDto()
        {
            Groups = new List<TestGroup>();
        }

        public Test Test { get; set; }
        public List<TestGroup> Groups { get; set; }

        
    }
    public class TestGroup
    {
        public TestGroup()
        {
            TestResults = new List<SPTestResultsGetWithStudents>();
        }

        public string GroupName { get; set; }
        public List<SPTestResultsGetWithStudents> TestResults { get; set; }
    }
}
