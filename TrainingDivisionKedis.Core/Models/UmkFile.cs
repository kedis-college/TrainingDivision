using System;
using System.Collections.Generic;
using System.Text;

namespace TrainingDivisionKedis.Core.Models
{
    public class UmkFile
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int SubjectId { get; set; }
        public byte TermId { get; set; }
        public string FileName { get; set; }
        public double? FileSize { get; set; }
        public byte FileTypeId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool Active { get; set; }

        public FileType FileType { get; set; }
    }
}
