using NiHonGo.Data.Models;
using System;
using System.Collections.Generic;

namespace NiHonGo.Core.DTO.Company
{
    public class VideoInfo
    {
        public int Id { get; set; }

        public string YoutubeUrl { get; set; }

        public string JapaneseTitle { get; set; }

        public string JapaneseContent { get; set; }

        public string ChineseTitle { get; set; }

        public string ChineseContent { get; set; }

        public List<Level> Levels { get; set; }

        public List<Word> Words { get; set; }

        public List<Grammer> Grammers { get; set; }

        public string CreateDateTime { get; set; }

        public string UpdateDateTime { get; set; }
    }
}