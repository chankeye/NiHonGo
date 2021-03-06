﻿using System.Collections.Generic;

namespace NiHonGo.Core.DTO.Video
{
    public class VideoSimpleInfo
    {
        public int Id { get; set; }

        public string YoutubeUrl { get; set; }

        public string Japanese { get; set; }

        public string Chinese { get; set; }

        public List<LevelInfo> Levels { get; set; }

        public string UpdateDateTime { get; set; }
    }
}