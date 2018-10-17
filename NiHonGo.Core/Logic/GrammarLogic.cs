using NiHonGo.Core.DTO;
using NiHonGo.Core.DTO.Grammar;
using NiHonGo.Core.Enum;
using NiHonGo.Data.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace NiHonGo.Core.Logic
{
    public class GrammarLogic : _BaseLogic
    {
        public GrammarLogic() : base() { }

        public IsSuccessResult Edit(int userId, GrammarInfo data)
        {
            var log = GetLogger();

            var isAdmin = NiHonGoContext.Users
                .Where(r => r.Type == (int)UserType.Admin)
                .Any(r => r.Id == userId);
            if (isAdmin == false)
                return new IsSuccessResult(ErrorCode.NoAuthentication.ToString());

            if (string.IsNullOrWhiteSpace(data.Title))
                return new IsSuccessResult(ErrorCode.TitleIsNull.ToString());

            if (string.IsNullOrWhiteSpace(data.Description))
                return new IsSuccessResult(ErrorCode.DescriptionIsNull.ToString());

            try
            {
                if (data.Id == 0)
                {
                    var isAny = NiHonGoContext.Grammars
                    .Any(r => r.Title == data.Title);
                    if (isAny)
                        return new IsSuccessResult(ErrorCode.AlreadyHadThisGrammar.ToString());

                    var levels = new List<Level>();
                    foreach (var item in data.Levels)
                    {
                        var level = NiHonGoContext.Levels.SingleOrDefault(r => r.Id == item.Id);
                        if (level != null)
                            levels.Add(level);
                    }
                    var grammar = new Grammar
                    {
                        Title = data.Title,
                        Description = data.Description,
                        Levels = levels
                    };

                    NiHonGoContext.Grammars.Add(grammar);
                    NiHonGoContext.SaveChanges();
                }
                else
                {
                    var isAny = NiHonGoContext.Grammars
                    .Any(r => r.Title == data.Title && r.Id != data.Id);
                    if (isAny)
                        return new IsSuccessResult(ErrorCode.AlreadyHadThisGrammar.ToString());

                    var grammar = NiHonGoContext.Grammars
                        .SingleOrDefault(r => r.Id == data.Id);
                    if (grammar == null)
                        return new IsSuccessResult(ErrorCode.GrammarNotFound.ToString());

                    grammar.Title = data.Title;
                    grammar.Description = data.Description;
                    var levels = new List<Level>();
                    foreach (var item in data.Levels)
                    {
                        var level = NiHonGoContext.Levels.SingleOrDefault(r => r.Id == item.Id);
                        if (level != null)
                            levels.Add(level);
                    }
                    grammar.Levels = levels;

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

        public IsSuccessResult<GrammarInfo> GetDetail(int grammarId)
        {
            var log = GetLogger();

            var grammar = NiHonGoContext.Grammars
                .Include(r => r.Levels)
                .SingleOrDefault(r => r.Id == grammarId);

            if (grammar == null)
                return new IsSuccessResult<GrammarInfo>(ErrorCode.GrammarNotFound.ToString());

            try
            {
                var result = new IsSuccessResult<GrammarInfo>
                {
                    ReturnObject = new GrammarInfo
                    {
                        Id = grammar.Id,
                        Title = grammar.Title,
                        Description = grammar.Description,
                        Levels = grammar.Levels
                    .Select(r => new LevelInfo
                    {
                        Id = r.Id,
                        Display = r.Display
                    }).ToList(),
                    }
                };

                return result;
            }
            catch (Exception ex)
            {
                log.Error(ex);

                return new IsSuccessResult<GrammarInfo>(ErrorCode.InternalError.ToString());
            }
        }

        public List<GrammarInfo> GetList(int videoId)
        {
            var log = GetLogger();

            var video = NiHonGoContext.Videos.SingleOrDefault(r => r.Id == videoId);
            if (video == null)
                return new List<GrammarInfo>();

            return NiHonGoContext.Grammars
                .Include(r => r.Levels)
                .Where(r => r.Videos.Contains(video))
                .Select(r => new GrammarInfo
                {
                    Id = r.Id,
                    Title = r.Title,
                    Description = r.Description,
                    Levels = r.Levels.Select(l => new LevelInfo
                    {
                        Id = l.Id,
                        Display = l.Display
                    }).ToList()
                }).ToList();
        }
    }
}