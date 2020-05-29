using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TrainingDivisionKedis.BLL.DTO.TestsAdmin
{
    public class TestAnswerDto
    {
        public int Id { get; set; }
        [Display(Name = "Ответ")]
        [Required(ErrorMessage = "Поле Ответ должно быть заполнено")]
        [MinLength(1), MaxLength(200)]
        public string Text { get; set; }
    }
}
