using System.Collections.Generic;
using TrainingDivisionKedis.BLL.DTO.TestsClient;
using TrainingDivisionKedis.Core.SPModels.Tests;

namespace TrainingDivisionKedis.Student.ViewModels.Tests
{
    public class TestsListViewModel
    {
        public TestsListViewModel()
        {
            Tests = new List<SPTestGetByStudentAndSubjectAndTerm>();
        }

        public TestsListViewModel(int subjectId)
        {
            SubjectId = subjectId;
            SubjectName = "";
            Tests = new List<SPTestGetByStudentAndSubjectAndTerm>();
        }

        public List<SPTestGetByStudentAndSubjectAndTerm> Tests { get; set; }
        public int SubjectId { get; set; }
        public string SubjectName { get; set; }
    }
}
