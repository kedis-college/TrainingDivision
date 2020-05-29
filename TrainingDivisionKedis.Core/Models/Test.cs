using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TrainingDivisionKedis.Core.Models
{
    public class Test
    {
        public Test()
        {
            Questions = new List<TestQuestion>();
        }

        public int Id { get; set; }
        [Display(Name = "Наименование")]
        [Required(ErrorMessage = "Поле Наименование должно быть заполнено")]
        [MinLength(1), MaxLength(256)]
        public string Name { get; set; }
        public int SubjectId { get; set; }
        public byte TermId { get; set; }
        [Display(Name = "Всего вопросов")]
        public short QuestionsTotal { get; set; }
        [Display(Name = "Вопросов на тест")]
        [Required(ErrorMessage = "Поле Вопросов на тест должно быть заполнено")]
        public short QuestionsPerTest { get; set; }
        [Display(Name = "Продолжительность")]
        [Required(ErrorMessage = "Поле Продолжительность должно быть заполнено")]
        [Range(1, short.MaxValue, ErrorMessage = "Значение должно быть в промежутке 1 - 32767")]
        public short TimeLimit { get; set; }
        public bool Active { get; set; }
        [Display(Name = "Видимость")]
        public bool Visible { get; set; }
        public bool Draft { get; set; }
        [Display(Name = "Дата добавления")]
        public DateTime CreatedAt { get; set; }
        [Display(Name = "Дата обновления")]
        public DateTime? UpdatedAt { get; set; }

        public List<TestQuestion> Questions { get; set; }
    }
}
