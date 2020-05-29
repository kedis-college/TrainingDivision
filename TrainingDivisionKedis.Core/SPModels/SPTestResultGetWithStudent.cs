using System;
using System.Collections.Generic;
using System.Text;
using TrainingDivisionKedis.Core.Models;

namespace TrainingDivisionKedis.Core.SPModels
{
    public class SPTestResultsGetWithStudents
    {
        public int Id { get; set; }
        public string StudentName { get; set; }
        public int StudentId { get; set; }
        public int TestId { get; set; }
        public byte YearId { get; set; }
        public int Ball { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? FinishedAt { get; set; }

        public List<TestResultItem> TestResultItems { get; set; }
        public Test Test { get; set; }

        public SPTestResultsGetWithStudents()
        {
            TestResultItems = new List<TestResultItem>();
        }
    }
}
