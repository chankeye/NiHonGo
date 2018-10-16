using System.Collections.Generic;

namespace NiHonGo.Data.Models
{
    public partial class Level
    {
        public Level()
        {
            this.Users = new List<User>();
            this.Videos = new List<Video>();
            this.Words = new List<Word>();
            this.Grammers = new List<Grammer>();
        }

        public int Id { get; set; }
        public string Display { get; set; }
        public virtual ICollection<User> Users { get; set; }
        public virtual ICollection<Video> Videos { get; set; }
        public virtual ICollection<Word> Words { get; set; }
        public virtual ICollection<Grammer> Grammers { get; set; }
    }
}