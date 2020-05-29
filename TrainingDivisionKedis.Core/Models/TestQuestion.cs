using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TrainingDivisionKedis.Core.Models
{
    public class TestQuestion
    {
        public TestQuestion()
        {
            Answers = new List<TestAnswer>();
        }

        public int Id { get; set; }
        [Display(Name = "Вопрос")]
        [Required(ErrorMessage = "Поле Вопрос должно быть заполнено")]
        [MinLength(1), MaxLength(500)]
        public string Text { get; set; }
        public int TestId { get; set; }
        public int? CorrectAnswerId { get; set; }
        
        public List<TestAnswer> Answers { get; set; }
        public Test Test { get; set; }
    }
}
