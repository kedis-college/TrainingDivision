using System;
using System.Collections.Generic;
using System.Text;

namespace TrainingDivisionKedis.Core.SPModels.Tests
{
    public class SPTestGetByStudentAndSubjectAndTerm
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int SubjectId { get; set; }
        public byte TermId { get; set; }
        public short QuestionsTotal { get; set; }
        public short TimeLimit { get; set; }
        public int? Ball { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? FinishedAt { get; set; }
        public int? TestResultId { get; set; }

        public bool IsOpened
        {
            get
            {
                if (!TestResultId.HasValue || !StartedAt.HasValue)
                    return true;
                if (FinishedAt.HasValue)
                    return false;
                var maxFinishTime = StartedAt.Value.AddMinutes(TimeLimit);
                var now = DateTime.Now;
                return DateTime.Compare(now, maxFinishTime) < 0;
            }
        }
    }
}
