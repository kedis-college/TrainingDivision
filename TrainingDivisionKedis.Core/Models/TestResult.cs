using System;
using System.Collections.Generic;
using System.Text;

namespace TrainingDivisionKedis.Core.Models
{
    public class TestResult
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int TestId { get; set; }
        public byte YearId { get; set; }
        public int Ball { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? FinishedAt { get; set; }

        public List<TestResultItem> TestResultItems { get; set; }
        public Test Test { get; set; }

        public TestResult()
        {
            TestResultItems = new List<TestResultItem>();
        }

        public bool IsOpened
        {
            get
            {
                if (Test == null)
                    throw new Exception("Тест не указан");
                if (FinishedAt.HasValue)
                    return false;
                if (!StartedAt.HasValue)
                    return true;
                var maxFinishTime = StartedAt.Value.AddMinutes(Test.TimeLimit);
                var now = DateTime.Now;
                return DateTime.Compare(now, maxFinishTime) < 0;
            }
        }
    }
}
