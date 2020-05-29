using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;

namespace TrainingDivisionKedis.BLL.DTO.Chat
{
    public class MessageCreateRequest
    {
        public int Sender { get; set; }
        public int[] Recipients { get; set; }
        public string Text { get; set; }
        public IFormFile AppliedFile { get; set; }
       
        public void Validate()
        {
            if (Recipients == null) throw new ValidationException("Получатели не указаны");
            if (Recipients.Length < 1)
                throw new ValidationException("Количество получателей должно быть минимум 1", new MinLengthAttribute(1), Recipients);
            if (Recipients.Length > 1000)
                throw new ValidationException("Количество получателей должно быть максимум 1000", new MaxLengthAttribute(1000), Recipients);
            if (Text != null && Text.Length > 500)
                throw new ValidationException("Длина сообщения должно быть менее 500 символов", new MaxLengthAttribute(500), Text);
        }
    }
}
