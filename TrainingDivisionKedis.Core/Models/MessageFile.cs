using System;
using System.Collections.Generic;
using System.Text;

namespace TrainingDivisionKedis.Core.Models
{
    public class MessageFile
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public byte FileTypeId { get; set; }
        public FileType FileType { get; set; }
    }
}
