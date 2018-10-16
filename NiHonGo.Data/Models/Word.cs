using System.Collections.Generic;

namespace NiHonGo.Data.Models
{
    public partial class Word
    {
        public Word()
        {
            this.Videos = new List<Video>();
        }

        public int Id { get; set; }
        public string Japanese { get; set; }
        public string Chinese { get; set; }
        public virtual ICollection<Level> Levels { get; set; }
        public virtual ICollection<Video> Videos { get; set; }
    }
}