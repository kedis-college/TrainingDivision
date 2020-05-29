using System;
using System.Collections.Generic;
using System.Text;

namespace TrainingDivisionKedis.Core.SPModels.Messages
{
    public class SPMessagesGetContactsByUser
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Group { get; set; }
        public int NewMessages { get; set; }
    }
}
