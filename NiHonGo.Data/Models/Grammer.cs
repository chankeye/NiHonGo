using System.Collections.Generic;

namespace NiHonGo.Data.Models
{
    public partial class Grammer
    {
        public Grammer()
        {
            this.Levels = new List<Level>();
            this.Videos = new List<Video>();
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public virtual ICollection<Level> Levels { get; set; }
        public virtual ICollection<Video> Videos { get; set; }
    }
}