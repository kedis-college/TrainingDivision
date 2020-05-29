using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace TrainingDivisionKedis.BLL.DTO.UmkFiles
{
    public class CreateRequest
    {
        public string Name { get; set; }
        public int SubjectId { get; set; }
        public byte TermId { get; set; }
        public IFormFile UmkFile { get; set; }
        public int UserId { get; set; }
    }
}
