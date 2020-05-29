using System;
using System.Collections.Generic;
using System.Text;
using TrainingDivisionKedis.Core.Models;

namespace TrainingDivisionKedis.Core.SPModels.Tests
{
    public class SPTestQuestionsGetRandomSet
    {
        public SPTestQuestionsGetRandomSet()
        {
            Answers = new List<TestAnswer>();
        }

        public int Id { get; set; }
        public string Text { get; set; }
        public List<TestAnswer> Answers { get; set; }
        public int? CheckedAnswer { get; set; }
    }
}
