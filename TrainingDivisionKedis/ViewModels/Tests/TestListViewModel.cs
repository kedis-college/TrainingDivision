using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrainingDivisionKedis.Core.Models;

namespace TrainingDivisionKedis.ViewModels.Tests
{
    public class TestListViewModel
    {
        public TestListViewModel()
        {
            Tests = new List<Test>();
        }

        public TestListViewModel(int subjectId, byte termId)
        {
            SubjectId = subjectId;
            SubjectName = "";
            TermId = termId;
            Tests = new List<Test>();
        }

        public List<Test> Tests { get; set; }
        public int SubjectId { get; set; }
        public byte TermId { get; set; }
        public string SubjectName { get; set; }
    }
}
