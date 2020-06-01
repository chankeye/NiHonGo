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
            this.Grammars = new List<Grammar>();
        }

        public int Id { get; set; }
        public string Display { get; set; }
        public virtual ICollection<User> Users { get; set; }
        public virtual ICollection<Video> Videos { get; set; }
        public virtual ICollection<Word> Words { get; set; }
        public virtual ICollection<Grammar> Grammars { get; set; }
    }
}