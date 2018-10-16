﻿using NiHonGo.Core.DTO;
using NiHonGo.Core.DTO.Company;
using NiHonGo.Core.Enum;
using NiHonGo.Data.Models;
using System;
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

                    var currentDate = DateTime.UtcNow;
                    var video = new Video
                    {
                        YoutubeUrl = data.YoutubeUrl,
                        JapaneseTitle = data.JapaneseTitle,
                        JapaneseContent = data.JapaneseContent,
                        ChineseTitle = data.ChineseTitle,
                        ChineseContent = data.ChineseContent,
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

                    video.YoutubeUrl = data.YoutubeUrl;
                    video.JapaneseTitle = data.JapaneseTitle;
                    video.JapaneseContent = data.JapaneseContent;
                    video.ChineseTitle = data.ChineseTitle;
                    video.ChineseContent = data.ChineseContent;
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

        public IsSuccessResult<VideoInfo> GetDetail(int companyId)
        {
            var log = GetLogger();

            var video = NiHonGoContext.Videos
                .SingleOrDefault(r => r.Id == companyId);

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
            log.Debug("Keyword: {0}, index: {1}, count: {2}", keyword, index, count);

            IQueryable<Video> query = NiHonGoContext.Videos;
            if (string.IsNullOrWhiteSpace(keyword) == false)
                query = query
                    .Where(r => r.JapaneseTitle.Contains(keyword) || r.JapaneseContent.Contains(keyword) ||
                        r.ChineseTitle.Contains(keyword) || r.ChineseContent.Contains(keyword));

            var videoCount = query.Count();
            var list = query.OrderByDescending(r => r.UpdateDateTime)
                .Skip(index * count).Take(count)
                .Select(r => new
                {
                    r.Id,
                    r.YoutubeUrl,
                    r.JapaneseTitle,
                    r.ChineseTitle,
                    r.UpdateDateTime
                })
                 .ToList()
                 .Select(r => new VideoSimpleInfo
                 {
                     Id = r.Id,
                     YoutubeUrl = r.YoutubeUrl,
                     Japanese = r.JapaneseTitle.Length > 50 ? r.JapaneseTitle.Remove(50) + "..." : r.JapaneseTitle,
                     Chinese = r.ChineseTitle.Length > 50 ? r.ChineseTitle.Remove(50) + "..." : r.ChineseTitle,
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