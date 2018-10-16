using NiHonGo.Core.DTO;
using NiHonGo.Core.DTO.Company;
using NiHonGo.Core.Enum;
using NiHonGo.Core.Utilities;
using NiHonGo.Data.Models;
using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace NiHonGo.Core.Logic
{
    public class CompanyLogic : _BaseLogic
    {
        /// <summary>
        /// CompanyLogic Logic
        /// </summary>
        public CompanyLogic() : base() { }

        public IsSuccessResult UpdatePhoto(int companyId, string photo)
        {
            var log = GetLogger();
            log.Debug("CompanyID: {0}, Photo: {1}"
                , companyId, photo);

            var company = NiHonGoContext.Companies
                .SingleOrDefault(r => r.Id == companyId);
            if (company == null)
                return new IsSuccessResult(ErrorCode.CompanyNotFound.ToString());

            if (string.IsNullOrWhiteSpace(photo))
                return new IsSuccessResult(ErrorCode.PhotoIsNull.ToString());

            if (System.Text.RegularExpressions.Regex.IsMatch(photo, Constant.PatternImage, RegexOptions.IgnoreCase) == false)
                return new IsSuccessResult(ErrorCode.PhotoFormatIsWrong.ToString());

            try
            {
                company.Photo = photo;
                company.UpdateDate = DateTime.UtcNow;

                NiHonGoContext.SaveChanges();

                return new IsSuccessResult();
            }
            catch (Exception ex)
            {
                log.Error(ex);

                return new IsSuccessResult(ErrorCode.InternalError.ToString());
            }
        }

        public IsSuccessResult Edit(int userId, CompanyInfo data)
        {
            var log = GetLogger();
            log.Debug("UserID: {0}, Introduction: {1}, Address: {2}, MemberCount: {3}, Name: {4}, Phone: {5}, StartYear: {6}, Url: {7}"
                , userId, data.Introduction, data.Address, data.MemberCount, data.Name, data.Phone, data.StartYear, data.Url);

            var user = NiHonGoContext.Users
                .Include(r => r.Company)
                .SingleOrDefault(r => r.Id == userId);
            if (user == null)
                return new IsSuccessResult(ErrorCode.UserNotFound.ToString());

            if ((UserType)user.Type != UserType.Master)
                return new IsSuccessResult(ErrorCode.NoAuthentication.ToString());

            if (string.IsNullOrWhiteSpace(data.Name))
                return new IsSuccessResult(ErrorCode.CompanyNameIsNull.ToString());

            if (string.IsNullOrWhiteSpace(data.Introduction))
                return new IsSuccessResult(ErrorCode.CompanyIntroductionIsNull.ToString());

            try
            {
                if (user.Company == null)
                {
                    var isAny = NiHonGoContext.Companies
                    .Any(r => r.Name == data.Name);
                    if (isAny)
                        return new IsSuccessResult(ErrorCode.AlreadyHadThisCompany.ToString());

                    var currentDate = DateTime.UtcNow;
                    user.Company = new Company
                    {
                        Name = data.Name,
                        Phone = data.Phone,
                        Address = data.Address,
                        Introduction = data.Introduction,
                        StartYear = data.StartYear,
                        Url = data.Url,
                        MemberCount = data.MemberCount,
                        CreateDate = currentDate,
                        UpdateDate = currentDate
                    };

                    NiHonGoContext.SaveChanges();

                    if (!string.IsNullOrWhiteSpace(data.Photo))
                    {
                        var oldPhotoName = data.Photo.Split('\\').Last();
                        var newPhotoName = user.Company.Id + Path.GetExtension(oldPhotoName);
                        File.Move(data.Photo, data.Photo.Replace(oldPhotoName, newPhotoName));

                        user.Company.Photo = newPhotoName;
                        NiHonGoContext.SaveChanges();
                    }
                }
                else
                {
                    var isAny = NiHonGoContext.Companies
                    .Any(r => r.Name == data.Name && r.Id != user.CompanyId);
                    if (isAny)
                        return new IsSuccessResult(ErrorCode.AlreadyHadThisEmail.ToString());

                    user.Company.Name = data.Name;
                    user.Company.Phone = data.Phone;
                    user.Company.Address = data.Address;
                    user.Company.Introduction = data.Introduction;
                    user.Company.StartYear = data.StartYear;
                    user.Company.Url = data.Url;
                    user.Company.MemberCount = data.MemberCount;
                    user.Company.UpdateDate = DateTime.UtcNow;

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

        public IsSuccessResult<CompanyInfo> GetDetail(int companyId)
        {
            var log = GetLogger();
            log.Debug("companyId: {0}", companyId);

            var company = NiHonGoContext.Companies
                .SingleOrDefault(r => r.Id == companyId);

            var result = new IsSuccessResult<CompanyInfo>();
            if (company == null)
                return new IsSuccessResult<CompanyInfo>(ErrorCode.CompanyNotFound.ToString());

            try
            {
                result.ReturnObject = new CompanyInfo
                {
                    Address = company.Address,
                    Name = company.Name,
                    Phone = company.Phone,
                    Introduction = company.Introduction,
                    MemberCount = company.MemberCount,
                    StartYear = company.StartYear,
                    Url = company.Url,
                    CreateDate = company.CreateDate.ToLocalTime().ToString("yyyy-MM-dd"),
                    UpdateDate = company.UpdateDate.ToLocalTime().ToString("yyyy-MM-dd"),
                    Photo = company.Photo
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

        public CompanyListReturn GetList(string keyword, int index, int count)
        {
            var log = GetLogger();
            log.Debug("Keyword: {0}, index: {1}, count: {2}", keyword, index, count);

            IQueryable<Company> query = NiHonGoContext.Companies;
            if (string.IsNullOrWhiteSpace(keyword) == false)
                query = query.Where(r => r.Name.Contains(keyword) || r.Introduction.Contains(keyword));

            var companyCount = query.Count();
            var list = query.OrderByDescending(r => r.UpdateDate)
                .Skip(index * count).Take(count)
                .Select(r => new
                {
                    r.Id,
                    r.Name,
                    r.Introduction,
                    r.Photo,
                    r.Address,
                    r.UpdateDate
                })
                 .ToList()
                 .Select(r => new CompanySimpleInfo
                 {
                     Id = r.Id,
                     Name = r.Name,
                     Introduction = r.Introduction.Length > 150 ? r.Introduction.Remove(150) + "..." : r.Introduction,
                     Photo = r.Photo,
                     UpdateDate = r.UpdateDate.ToLocalTime().ToString("yyyy-MM-dd"),
                 }).ToList();

            return new CompanyListReturn
            {
                List = list,
                Count = companyCount
            };
        }
    }
}