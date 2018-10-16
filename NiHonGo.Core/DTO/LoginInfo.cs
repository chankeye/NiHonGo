using NiHonGo.Core.Enum;

namespace NiHonGo.Core.DTO
{
    public class LoginInfo
    {
        public int Id { get; set; }

        public string Email { get; set; }

        public string Display { get; set; }

        public UserType UserType { get; set; }
    }
}