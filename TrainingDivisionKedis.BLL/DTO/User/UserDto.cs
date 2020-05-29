using System;
using System.Collections.Generic;
using System.Text;

namespace TrainingDivisionKedis.BLL.DTO.User
{
    public class UserDto
    {
        public UserDto()
        {
            Roles = new List<string>();
        }

        public UserDto(string login, string password)
        {
            Login = login ?? throw new ArgumentNullException(nameof(login));
            Password = password ?? throw new ArgumentNullException(nameof(password));
            Roles = new List<string>();
        }

        public int Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public List<string> Roles { get; set; }
    }
}
