using System;
using System.Collections.Generic;
using System.Text;

namespace TrainingDivisionKedis.Core.SPModels.Messages
{
    public class SPMessagesGetByUsers
    {
        public int Id { get; set; }
        public int Sender { get; set; }
        public int Recipient { get; set; }
        public string Text { get; set; }
        public int? MessageFileId { get; set; }
        public bool Received { get; set; }
        public byte Type { get; set; }
        public DateTime CreatedAt { get; set; }
        public string MessageFileName { get; set; }
        public string SenderName { get; set; }
    }
}
