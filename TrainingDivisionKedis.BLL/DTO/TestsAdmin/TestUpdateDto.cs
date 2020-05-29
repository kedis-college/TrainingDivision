using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TrainingDivisionKedis.BLL.DTO.TestsAdmin
{
    public class TestUpdateDto
    {
        public int Id { get; set; }
        [Display(Name = "Наименование")]
        public string Name { get; set; }
        public int SubjectId { get; set; }
        public byte TermId { get; set; }
        [Display(Name = "Всего вопросов")]
        public short QuestionsTotal { get; set; }
        [Display(Name = "Вопросов на тест")]
        public short QuestionsPerTest { get; set; }
        [Display(Name = "Продолжительность")]
        [Range(1, short.MaxValue, ErrorMessage = "Значение должно быть в промежутке 1 - 32767")]

        public List<QuestionUpdateDto> Questions { get; set; }
    }

    public class QuestionUpdateDto
    {
        public int Id { get; set; }
        [Display(Name = "Вопрос")]
        public string Text { get; set; }
        public int TestId { get; set; }
        public int? CorrectAnswerId { get; set; }
    }
}
