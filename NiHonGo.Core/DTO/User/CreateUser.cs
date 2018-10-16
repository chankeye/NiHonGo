using System.Collections.Generic;

namespace NiHonGo.Core.DTO.User
{
    public class CreateUser
    {
        public string Email { get; set; }

        public string Name { get; set; }

        public string Password { get; set; }

        public List<string> Levels { get; set; }

        public string CreateDateTime { get; set; }
    }
}