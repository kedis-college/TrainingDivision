using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TrainingDivisionKedis.BLL.DTO.TestsAdmin
{
    public class TestQuestionDto
    {
        public TestQuestionDto()
        {
            Answers = new List<TestAnswerDto>();
        }

        [Display(Name = "Вопрос")]
        [Required(ErrorMessage = "Поле Вопрос должно быть заполнено")]
        [MinLength(1), MaxLength(500)]
        public string Text { get; set; }
        public int? CorrectAnswerId { get; set; } 
        public List<TestAnswerDto> Answers { get; set; }
    }
}
