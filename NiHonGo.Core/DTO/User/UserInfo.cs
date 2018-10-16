using System.Collections.Generic;

namespace NiHonGo.Core.DTO.User
{
    /// <summary>
    /// user info
    /// </summary>
    public class UserInfo
    {
        public int Id { get; set; }

        public string Email { get; set; }
    }

    public class UserInfoList
    {
        public List<UserInfo> List { get; set; }

        public int Count { get; set; }
    }
}