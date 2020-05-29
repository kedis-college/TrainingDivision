using System;
using System.Collections.Generic;
using System.Text;

namespace TrainingDivisionKedis.Core.Models
{
    public class TestResultItem
    {
        public int Id { get; set; }
        public int TestQuestionId { get; set; }
        public int? AnswerId { get; set; }
        public bool IsCorrect { get; set; }
        public int TestResultId { get; set; }

        public TestQuestion TestQuestion { get; set; }
    }
}
