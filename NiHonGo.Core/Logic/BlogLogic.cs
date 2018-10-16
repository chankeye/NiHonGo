using NiHonGo.Core.DTO;
using NiHonGo.Core.DTO.Company;
using NiHonGo.Core.DTO.User;
using NiHonGo.Core.Enum;
using NiHonGo.Core.Utilities;
using NiHonGo.Data.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace NiHonGo.Core.Logic
{
    public class BlogLogic : _BaseLogic
    {
        /// <summary>
        /// BlogLogic Logic
        /// </summary>
        public BlogLogic() : base() { }

        public IsSuccessResult UpdatePhoto(int blogId, string photo)
        {
            var log = GetLogger();
            log.Debug("BlogId: {0}, Photo: {1}"
                , blogId, photo);

            var blog = NiHonGoContext.Blogs
                .SingleOrDefault(r => r.Id == blogId);
            if (blog == null)
                return new IsSuccessResult(ErrorCode.BlogNotFound.ToString());

            if (string.IsNullOrWhiteSpace(photo))
                return new IsSuccessResult(ErrorCode.PhotoIsNull.ToString());

            if (Regex.IsMatch(photo, Constant.PatternImage, RegexOptions.IgnoreCase) == false)
                return new IsSuccessResult(ErrorCode.PhotoFormatIsWrong.ToString());

            try
            {
                blog.Photo = photo;

                NiHonGoContext.SaveChanges();

                return new IsSuccessResult();
            }
            catch (Exception ex)
            {
                log.Error(ex);

                return new IsSuccessResult(ErrorCode.InternalError.ToString());
            }
        }

        public IsSuccessResult<int> Edit(EditBlog data, int userId, int? blogId)
        {
            var log = GetLogger();
            log.Debug("Title: {0}, Detail: {1}, Tag: {2}, UserId: {3}, BlogId: {4}, Catalog: {5}"
                , data.Title, data.Detail, data.Tag, userId, blogId, data.Catalog);

            var user = NiHonGoContext.Users
                .SingleOrDefault(r => r.Id == userId);
            if (user == null)
                return new IsSuccessResult<int>(ErrorCode.UserNotFound.ToString());

            if ((UserType)user.Type != UserType.Admin)
                return new IsSuccessResult<int>(ErrorCode.NoAuthentication.ToString());

            if (string.IsNullOrWhiteSpace(data.Title))
                return new IsSuccessResult<int>(ErrorCode.BlogTitleIsNull.ToString());

            if (string.IsNullOrWhiteSpace(data.Detail))
                return new IsSuccessResult<int>(ErrorCode.BlogDetailIsNull.ToString());

            if (string.IsNullOrWhiteSpace(data.Catalog))
                return new IsSuccessResult<int>(ErrorCode.CatalogIsNull.ToString());

            var blog = NiHonGoContext.Blogs
                .SingleOrDefault(r => r.Id == blogId);

            var catalog = NiHonGoContext.Catalogs
                .SingleOrDefault(r => r.Display == data.Catalog);

            try
            {
                if (catalog == null)
                {
                    catalog = new Grammer
                    {
                        Display = data.Catalog
                    };
                    NiHonGoContext.Catalogs.Add(catalog);
                    NiHonGoContext.SaveChanges();
                }

                if (blog == null)
                {
                    var currentDate = DateTime.UtcNow;

                    blog = new Blog
                    {
                        Title = data.Title,
                        Detail = data.Detail,
                        UserId = userId,
                        Tag = data.Tag,
                        Catalog = catalog,
                        CreateDate = currentDate,
                    };

                    NiHonGoContext.Blogs.Add(blog);
                    NiHonGoContext.SaveChanges();

                    if (!string.IsNullOrWhiteSpace(data.Photo))
                    {
                        var oldPhotoName = data.Photo.Split('\\').Last();
                        var newPhotoName = blog.Id + Path.GetExtension(oldPhotoName);
                        File.Move(data.Photo, data.Photo.Replace(oldPhotoName, newPhotoName));

                        blog.Photo = newPhotoName;
                        NiHonGoContext.SaveChanges();
                    }
                }
                else
                {
                    blog.Title = data.Title;
                    blog.Detail = data.Detail;
                    blog.Tag = data.Tag;
                    blog.UserId = userId;
                    blog.Catalog = catalog;
                    NiHonGoContext.SaveChanges();
                }

                return new IsSuccessResult<int>
                {
                    ReturnObject = blog.Id
                };
            }
            catch (Exception ex)
            {
                log.Error(ex);

                return new IsSuccessResult<int>(ErrorCode.InternalError.ToString());
            }
        }

        public IsSuccessResult<BlogInfo> GetDetail(int blogId)
        {
            var log = GetLogger();
            log.Debug("blogId: {0}", blogId);

            var blog = NiHonGoContext.Blogs
                .SingleOrDefault(r => r.Id == blogId);

            var result = new IsSuccessResult<BlogInfo>();
            if (blog == null)
            {
                result.IsSuccess = false;
                result.ErrorCode = ErrorCode.BlogNotFound.ToString();
                return result;
            }

            try
            {
                result.ReturnObject = new BlogInfo
                {
                    Title = blog.Title,
                    Detail = blog.Detail,
                    CreateDate = blog.CreateDate.ToLocalTime().ToString("yyyy-MM-dd"),
                    Photo = blog.Photo,
                    Tag = blog.Tag,
                    Catalog = blog.Catalog.Display
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

        public BlogListReturn GetList(string keyword, int index, int count, int? catalogId)
        {
            var log = GetLogger();
            log.Debug("Keyword: {0}, index: {1}, count: {2}, catalogId", keyword, index, count, catalogId);

            IQueryable<Blog> query = NiHonGoContext.Blogs;
            if (string.IsNullOrWhiteSpace(keyword) == false)
                query = query.Where(r => r.Title.Contains(keyword) || r.Detail.Contains(keyword) || r.Tag.Contains(keyword));

            if (catalogId.HasValue)
                query = query.Where(r => r.CatalogId == catalogId.Value);

            var blogCount = query.Count();
            var list = query.OrderByDescending(r => r.CreateDate)
                .Skip(index * count).Take(count)
                .Select(r => new
                {
                    r.Id,
                    r.Title,
                    r.Detail,
                    r.Photo,
                    Catalog = r.Catalog.Display
                })
                 .ToList()
                 .Select(r => new BlogSimpleInfo
                 {
                     Id = r.Id,
                     Title = r.Title,
                     Detail = r.Detail.Length > 150 ?
                         (r.Detail.Contains("<img") ? r.Detail.Remove(r.Detail.IndexOf("<img")) + "..." : r.Detail.Remove(150) + "...") :
                         r.Detail,
                     Photo = r.Photo,
                     Catalog = r.Catalog
                 }).ToList();

            return new BlogListReturn
            {
                List = list,
                Count = blogCount
            };
        }

        public List<ListItem> GetCataloges()
        {
            return NiHonGoContext.Catalogs.Select(r => new ListItem
            {
                Display = r.Display,
                Value = r.Id.ToString()
            }).ToList();
        }
    }
}