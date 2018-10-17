using System.Collections.Generic;

namespace NiHonGo.Core.DTO.Word
{
    public class WordInfo
    {
        public int Id { get; set; }

        public string Japanese { get; set; }

        public string Chinese { get; set; }

        public List<LevelInfo> Levels { get; set; }
    }
}