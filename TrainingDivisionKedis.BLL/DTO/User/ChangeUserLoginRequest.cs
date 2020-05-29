using System;
using System.Collections.Generic;
using System.Text;

namespace TrainingDivisionKedis.BLL.DTO.User
{
    public class ChangeUserLoginRequest
    {
        public int Id { get; set; }
        public string NewLogin { get; set; }
    }
}
