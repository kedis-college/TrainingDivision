using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace TrainingDivisionKedis.BLL.DTO.UmkFiles
{
    public class UpdateRequest
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IFormFile UmkFile { get; set; }
        public int UserId { get; set; }
    }
}
