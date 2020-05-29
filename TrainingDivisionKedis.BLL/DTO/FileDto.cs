using System;
using System.Collections.Generic;
using System.Text;

namespace TrainingDivisionKedis.BLL.DTO
{
    public class FileDto
    {
        public byte[] FileBytes { get; set; }
        public string FileType { get; set; }
        public string FileName { get; set; }
    }
}
