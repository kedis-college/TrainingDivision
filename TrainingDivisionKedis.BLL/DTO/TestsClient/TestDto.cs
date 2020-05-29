using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using TrainingDivisionKedis.Core.SPModels.Tests;

namespace TrainingDivisionKedis.BLL.DTO.TestsClient
{
    public class TestDto
    {
        public TestDto()
        {
            Questions = new List<SPTestQuestionsGetRandomSet>();
        }

        public int Id { get; set; }
        [Display(Name = "Наименование")]
        public string Name { get; set; }
        public int SubjectId { get; set; }
        public byte TermId { get; set; }
        [Display(Name = "Всего вопросов")]
        public short QuestionsTotal { get; set; }
        [Display(Name = "Продолжительность")]
        public short TimeLimit { get; set; }
        public DateTime? StartedAt { get; set; }
        public int TestResultId { get; set; }
        [Display(Name = "Вопросы")]
        public List<SPTestQuestionsGetRandomSet> Questions { get; set; }
    }
}
