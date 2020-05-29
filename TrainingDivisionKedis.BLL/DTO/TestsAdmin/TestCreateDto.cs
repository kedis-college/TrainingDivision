using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using TrainingDivisionKedis.Core.Models;

namespace TrainingDivisionKedis.BLL.DTO.TestsAdmin
{
    public class TestCreateDto
    {
        public TestCreateDto()
        {
            Questions = new List<TestQuestionDto>();
        }

        [Display(Name = "Наименование")]
        [Required(ErrorMessage = "Поле Наименование должно быть заполнено")]
        [MinLength(1),MaxLength(256)]
        public string Name { get; set; }
        public int SubjectId { get; set; }
        public byte TermId { get; set; }
        [Display(Name = "Всего вопросов")]
        public short QuestionsTotal { get; set; }
        [Display(Name = "Вопросов на тест")]
        [Required(ErrorMessage = "Поле Вопросов на тест должно быть заполнено")]
        public short QuestionsPerTest { get; set; }
        [Display(Name= "Продолжительность")]
        [Required(ErrorMessage = "Поле Продолжительность должно быть заполнено")]
        [Range(1,short.MaxValue,ErrorMessage = "Значение должно быть в промежутке 1 - 32767")]
        public short TimeLimit { get; set; }
        public int UserId { get; set; }

        public List<TestQuestionDto> Questions { get; set; }
    }
}
