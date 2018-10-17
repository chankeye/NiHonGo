using NiHonGo.Core.DTO.Word;
using System.Collections.Generic;

namespace NiHonGo.Core.DTO.Video
{
    public class VideoInfo
    {
        public int Id { get; set; }

        public string YoutubeUrl { get; set; }

        public string JapaneseTitle { get; set; }

        public string JapaneseContent { get; set; }

        public string ChineseTitle { get; set; }

        public string ChineseContent { get; set; }

        public List<LevelInfo> Levels { get; set; }

        public List<WordInfo> Words { get; set; }

        public List<GrammerInfo> Grammers { get; set; }

        public string CreateDateTime { get; set; }

        public string UpdateDateTime { get; set; }
    }
}