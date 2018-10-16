using NiHonGo.Core.Enum;

namespace NiHonGo.Core.DTO.User
{
    public class CreateUser
    {
        public string Email { get; set; }

        public string Name { get; set; }

        public string Phone { get; set; }

        public string Password { get; set; }

        public string Display { get; set; }

        public bool IsDisable { get; set; }
    }
}