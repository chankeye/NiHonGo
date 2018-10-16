using System;
using System.Collections.Generic;

namespace NiHonGo.Data.Models
{
    public partial class User
    {
        public User()
        {
            this.Levels = new List<Level>();
            this.Videos = new List<Video>();
        }

        public int Id { get; set; }
        public string FBToken { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public int Type { get; set; }
        public DateTime CreateDateTime { get; set; }
        public virtual ICollection<Level> Levels { get; set; }
        public virtual ICollection<Video> Videos { get; set; }
    }
}