using System.Collections.Generic;

namespace NiHonGo.Core.DTO.Grammar
{
    public class GrammarInfo
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public List<LevelInfo> Levels { get; set; }
    }
}