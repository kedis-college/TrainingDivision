using System;
using System.Collections.Generic;
using System.Text;

namespace TrainingDivisionKedis.Core.Models
{
    public class Message
    {
        public Message()
        {
        }

        public Message(Message message)
        {
            Id = message.Id;
            Sender = message.Sender;
            Recipient = message.Recipient;
            Text = message.Text;
            MessageFileId = message.MessageFileId;
            Received = message.Received;
            Type = message.Type;
            CreatedAt = message.CreatedAt;
            MessageFile = message.MessageFile;
        }

        public int Id { get; set; }
        public int Sender {get; set;}
        public int Recipient { get; set; }
        public string Text { get; set; }
        public int? MessageFileId { get; set; }
        public bool Received { get; set; }
        public byte Type { get; set; }
        public DateTime CreatedAt { get; set; }

        public MessageFile MessageFile { get; set; }
    }
}
