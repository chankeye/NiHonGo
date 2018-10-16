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

    public class UserItem
    {
        public int Id { get; set; }

        public string Email { get; set; }

        public bool  IsDisable { get; set; }
    }

    public class UserListReturn
    {
        public List<UserItem> List { get; set; }

        public int Count { get; set; }
    }
}