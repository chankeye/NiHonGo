using NiHonGo.Core.DTO;
using NiHonGo.Core.DTO.Word;
using NiHonGo.Core.Enum;
using NiHonGo.Data.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace NiHonGo.Core.Logic
{
    public class WordLogic : _BaseLogic
    {
        public WordLogic() : base() { }

        public IsSuccessResult Edit(int userId, WordInfo data)
        {
            var log = GetLogger();

            var isAdmin = NiHonGoContext.Users
                .Where(r => r.Type == (int)UserType.Admin)
                .Any(r => r.Id == userId);
            if (isAdmin == false)
                return new IsSuccessResult(ErrorCode.NoAuthentication.ToString());

            if (string.IsNullOrWhiteSpace(data.Japanese))
                return new IsSuccessResult(ErrorCode.JapaneseContentIsNull.ToString());

            if (string.IsNullOrWhiteSpace(data.Chinese))
                return new IsSuccessResult(ErrorCode.ChineseContentIsNull.ToString());

            try
            {
                if (data.Id == 0)
                {
                    var isAny = NiHonGoContext.Words
                    .Any(r => r.Japanese == data.Japanese);
                    if (isAny)
                        return new IsSuccessResult(ErrorCode.AlreadyHadThisWord.ToString());

                    var levels = new List<Level>();
                    foreach (var item in data.Levels)
                    {
                        var level = NiHonGoContext.Levels.SingleOrDefault(r => r.Id == item.Id);
                        if (level != null)
                            levels.Add(level);
                    }
                    var word = new Word
                    {
                        Japanese = data.Japanese,
                        Chinese = data.Chinese,
                        Levels = levels
                    };

                    NiHonGoContext.Words.Add(word);
                    NiHonGoContext.SaveChanges();
                }
                else
                {
                    var isAny = NiHonGoContext.Words
                    .Any(r => r.Japanese == data.Japanese && r.Id != data.Id);
                    if (isAny)
                        return new IsSuccessResult(ErrorCode.AlreadyHadThisWord.ToString());

                    var word = NiHonGoContext.Words
                        .SingleOrDefault(r => r.Id == data.Id);
                    if (word == null)
                        return new IsSuccessResult(ErrorCode.WordNotFound.ToString());

                    word.Japanese = data.Japanese;
                    word.Chinese = data.Chinese;
                    var levels = new List<Level>();
                    foreach (var item in data.Levels)
                    {
                        var level = NiHonGoContext.Levels.SingleOrDefault(r => r.Id == item.Id);
                        if (level != null)
                            levels.Add(level);
                    }
                    word.Levels = levels;

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

        public IsSuccessResult<WordInfo> GetDetail(int wordId)
        {
            var log = GetLogger();

            var word = NiHonGoContext.Words
                .Include(r => r.Levels)
                .SingleOrDefault(r => r.Id == wordId);

            var result = new IsSuccessResult<WordInfo>();
            if (word == null)
                return new IsSuccessResult<WordInfo>(ErrorCode.WordNotFound.ToString());

            try
            {
                result.ReturnObject = new WordInfo
                {
                    Japanese = word.Japanese,
                    Chinese = word.Chinese,
                    Levels = word.Levels
                    .Select(r => new LevelInfo
                    {
                        Id = r.Id,
                        Display = r.Display
                    }).ToList(),
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

        public List<WordInfo> GetList(int videoId)
        {
            var log = GetLogger();

            var video = NiHonGoContext.Videos.SingleOrDefault(r => r.Id == videoId);
            if (video == null)
                return new List<WordInfo>();

            return NiHonGoContext.Words
                .Include(r => r.Levels)
                .Where(r => r.Videos.Contains(video))
                .Select(r => new WordInfo
                {
                    Id = r.Id,
                    Chinese = r.Chinese,
                    Japanese = r.Japanese,
                    Levels = r.Levels.Select(l => new LevelInfo
                    {
                        Id = l.Id,
                        Display = l.Display
                    }).ToList()
                }).ToList();
        }
    }
}