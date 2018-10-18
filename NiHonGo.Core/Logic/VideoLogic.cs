using NiHonGo.Core.DTO;
using NiHonGo.Core.DTO.Grammar;
using NiHonGo.Core.DTO.Video;
using NiHonGo.Core.DTO.Word;
using NiHonGo.Core.Enum;
using NiHonGo.Data.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace NiHonGo.Core.Logic
{
    public class VideoLogic : _BaseLogic
    {
        public VideoLogic() : base() { }

        public IsSuccessResult Edit(int userId, VideoInfo data)
        {
            var log = GetLogger();

            var isAdmin = NiHonGoContext.Users
                .Where(r => r.Type == (int)UserType.Admin)
                .Any(r => r.Id == userId);
            if (isAdmin == false)
                return new IsSuccessResult(ErrorCode.NoAuthentication.ToString());

            if (string.IsNullOrWhiteSpace(data.YoutubeUrl))
                return new IsSuccessResult(ErrorCode.YoutubeUrlIsNull.ToString());

            if (string.IsNullOrWhiteSpace(data.JapaneseTitle))
                return new IsSuccessResult(ErrorCode.JapaneseTitleIsNull.ToString());

            if (string.IsNullOrWhiteSpace(data.JapaneseContent))
                return new IsSuccessResult(ErrorCode.JapaneseContentIsNull.ToString());

            if (string.IsNullOrWhiteSpace(data.ChineseTitle))
                return new IsSuccessResult(ErrorCode.ChineseTitleIsNull.ToString());

            if (string.IsNullOrWhiteSpace(data.ChineseContent))
                return new IsSuccessResult(ErrorCode.ChineseContentIsNull.ToString());

            try
            {
                if (data.Id == 0)
                {
                    var isAny = NiHonGoContext.Videos
                    .Any(r => r.YoutubeUrl == data.YoutubeUrl);
                    if (isAny)
                        return new IsSuccessResult(ErrorCode.AlreadyHadThisVideo.ToString());

                    var levels = new List<Level>();
                    foreach (var item in data.Levels)
                    {
                        var level = NiHonGoContext.Levels.SingleOrDefault(r => r.Id == item.Id);
                        if (level != null)
                            levels.Add(level);
                    }

                    var words = new List<Word>();
                    foreach (var item in data.Words)
                    {
                        var word = NiHonGoContext.Words.SingleOrDefault(r => r.Id == item.Id);
                        if (word != null)
                            words.Add(word);
                    }

                    var grammars = new List<Grammar>();
                    foreach (var item in data.Words)
                    {
                        var grammar = NiHonGoContext.Grammars.SingleOrDefault(r => r.Id == item.Id);
                        if (grammar != null)
                            grammars.Add(grammar);
                    }

                    var currentDate = DateTime.UtcNow;
                    var video = new Video
                    {
                        YoutubeUrl = data.YoutubeUrl,
                        JapaneseTitle = data.JapaneseTitle,
                        JapaneseContent = data.JapaneseContent,
                        ChineseTitle = data.ChineseTitle,
                        ChineseContent = data.ChineseContent,
                        Levels = levels,
                        Words = words,
                        Grammars = grammars,
                        CreateDateTime = currentDate,
                        UpdateDateTime = currentDate
                    };

                    NiHonGoContext.Videos.Add(video);
                    NiHonGoContext.SaveChanges();
                }
                else
                {
                    var isAny = NiHonGoContext.Videos
                    .Any(r => r.YoutubeUrl == data.YoutubeUrl && r.Id != data.Id);
                    if (isAny)
                        return new IsSuccessResult(ErrorCode.AlreadyHadThisVideo.ToString());

                    var video = NiHonGoContext.Videos
                        .SingleOrDefault(r => r.Id == data.Id);
                    if (video == null)
                        return new IsSuccessResult(ErrorCode.VideoNotFound.ToString());

                    var levels = new List<Level>();
                    foreach (var item in data.Levels)
                    {
                        var level = NiHonGoContext.Levels.SingleOrDefault(r => r.Id == item.Id);
                        if (level != null)
                            levels.Add(level);
                    }

                    var words = new List<Word>();
                    foreach (var item in data.Words)
                    {
                        var word = NiHonGoContext.Words.SingleOrDefault(r => r.Id == item.Id);
                        if (word != null)
                            words.Add(word);
                    }

                    var grammars = new List<Grammar>();
                    foreach (var item in data.Words)
                    {
                        var grammar = NiHonGoContext.Grammars.SingleOrDefault(r => r.Id == item.Id);
                        if (grammar != null)
                            grammars.Add(grammar);
                    }

                    video.YoutubeUrl = data.YoutubeUrl;
                    video.JapaneseTitle = data.JapaneseTitle;
                    video.JapaneseContent = data.JapaneseContent;
                    video.ChineseTitle = data.ChineseTitle;
                    video.ChineseContent = data.ChineseContent;
                    video.Levels = levels;
                    video.Words = words;
                    video.Grammars = grammars;
                    video.UpdateDateTime = DateTime.UtcNow;

                    NiHonGoContext.SaveChanges();
                }

                return new IsSuccessResult();
            }
            catch (Exception ex)
            {
                log.Error(ex);

                return new IsSuccessResult(ErrorCode.InternalError.ToString());
            }
        }

        public IsSuccessResult<VideoInfo> GetDetail(int videoId)
        {
            var log = GetLogger();

            var video = NiHonGoContext.Videos
                .Include(r => r.Levels)
                .Include(r => r.Words)
                .Include(r => r.Grammars)
                .SingleOrDefault(r => r.Id == videoId);

            var result = new IsSuccessResult<VideoInfo>();
            if (video == null)
                return new IsSuccessResult<VideoInfo>(ErrorCode.VideoNotFound.ToString());

            try
            {
                result.ReturnObject = new VideoInfo
                {
                    YoutubeUrl = video.YoutubeUrl,
                    JapaneseTitle = video.JapaneseTitle,
                    JapaneseContent = video.JapaneseContent,
                    ChineseTitle = video.ChineseTitle,
                    ChineseContent = video.ChineseContent,
                    Levels = video.Levels.Select(r => new LevelInfo
                    {
                        Id = r.Id,
                        Display = r.Display
                    }).ToList(),
                    Words = video.Words.Select(r => new WordInfo
                    {
                        Id = r.Id,
                        Japanese = r.Japanese,
                        Chinese = r.Chinese,
                        Levels = r.Levels.Select(l => new LevelInfo
                        {
                            Id = l.Id,
                            Display = l.Display
                        }).ToList()
                    }).ToList(),
                    Grammars = video.Grammars.Select(r => new GrammarInfo
                    {
                        Id = r.Id,
                        Title = r.Title,
                        Description = r.Description,
                        Levels = r.Levels.Select(l => new LevelInfo
                        {
                            Id = l.Id,
                            Display = l.Display
                        }).ToList()
                    }).ToList(),
                    CreateDateTime = video.CreateDateTime.ToLocalTime().ToString("yyyy-MM-dd"),
                    UpdateDateTime = video.UpdateDateTime.ToLocalTime().ToString("yyyy-MM-dd")
                };

                return result;
            }
            catch (Exception ex)
            {
                log.Error(ex);

                result.IsSuccess = false;
                result.ErrorCode = ErrorCode.InternalError.ToString();
                return result;
            }
        }

        public VideoSimpleInfoList GetList(string keyword, int index, int count)
        {
            var log = GetLogger();

            IQueryable<Video> query = NiHonGoContext.Videos.Include(r => r.Levels);
            if (string.IsNullOrWhiteSpace(keyword) == false)
            {
                query = query.Where(r => r.JapaneseTitle.Contains(keyword) || r.JapaneseContent.Contains(keyword) ||
                            r.ChineseTitle.Contains(keyword) || r.ChineseContent.Contains(keyword));
            }

            var videoCount = query.Count();
            var list = query.OrderByDescending(r => r.UpdateDateTime)
                .Skip(index * count).Take(count)
                .Select(r => new
                {
                    r.Id,
                    r.YoutubeUrl,
                    r.JapaneseTitle,
                    r.ChineseTitle,
                    r.Levels,
                    r.UpdateDateTime
                })
                 .ToList()
                 .Select(r => new VideoSimpleInfo
                 {
                     Id = r.Id,
                     YoutubeUrl = r.YoutubeUrl,
                     Japanese = r.JapaneseTitle.Length > 50 ? r.JapaneseTitle.Remove(50) + "..." : r.JapaneseTitle,
                     Chinese = r.ChineseTitle.Length > 50 ? r.ChineseTitle.Remove(50) + "..." : r.ChineseTitle,
                     Levels = r.Levels.Select(l => new LevelInfo
                     {
                         Id = l.Id,
                         Display = l.Display
                     }).ToList(),
                     UpdateDateTime = r.UpdateDateTime.ToLocalTime().ToString("yyyy-MM-dd")
                 }).ToList();

            return new VideoSimpleInfoList
            {
                List = list,
                Count = videoCount
            };
        }
    }
}