using System;
using System.Collections.Generic;

namespace NiHonGo.Data.Models
{
    public partial class Video
    {
        public Video()
        {
            this.Levels = new List<Level>();
            this.Words = new List<Word>();
            this.Grammers = new List<Grammer>();
        }

        public int Id { get; set; }
        public string YoutubeUrl { get; set; }
        public string JapaneseTitle { get; set; }
        public string JapaneseContent { get; set; }
        public string ChineseTitle { get; set; }
        public string ChineseContent { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public int UserId { get; set; }
        public virtual ICollection<Level> Levels { get; set; }
        public virtual ICollection<Word> Words { get; set; }
        public virtual ICollection<Grammer> Grammers { get; set; }
        public virtual User User { get; set; }
    }
}