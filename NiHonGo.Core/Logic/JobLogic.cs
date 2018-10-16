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
    public class JobLogic : _BaseLogic
    {
        /// <summary>
        /// JobLogic Logic
        /// </summary>
        public JobLogic() : base() { }

        public IsSuccessResult UpdatePhoto(int jobId, string photo)
        {
            var log = GetLogger();
            log.Debug("JobID: {0}, Photo: {1}"
                , jobId, photo);

            var job = NiHonGoContext.Jobs
                .SingleOrDefault(r => r.Id == jobId);
            if (job == null)
                return new IsSuccessResult(ErrorCode.JobNotFound.ToString());

            if (string.IsNullOrWhiteSpace(photo))
                return new IsSuccessResult(ErrorCode.PhotoIsNull.ToString());

            if (System.Text.RegularExpressions.Regex.IsMatch(photo, Constant.PatternImage, RegexOptions.IgnoreCase) == false)
                return new IsSuccessResult(ErrorCode.PhotoFormatIsWrong.ToString());

            try
            {
                job.Photo = photo;
                job.UpdateDate = DateTime.UtcNow;

                NiHonGoContext.SaveChanges();

                return new IsSuccessResult();
            }
            catch (Exception ex)
            {
                log.Error(ex);

                return new IsSuccessResult(ErrorCode.InternalError.ToString());
            }
        }

        public IsSuccessResult<int> Edit(int userId, EditJob data, int? jobId = null)
        {
            var log = GetLogger();
            log.Debug("UserID: {0}, Name: {1}, What: {2}, Why: {3}, How: {4}, Detail: {5}, TypeId: {6}, StartSalary: {7}, EndSalary: {8}, StatusId: {9}, Tag: {10}, AreaId: {11}"
                , userId, data.Name, data.What, data.Why, data.How, data.Detail, data.TypeId, data.StartSalary, data.EndSalary, data.StatusId, data.Tag, data.AreaId);

            var user = NiHonGoContext.Users
                .SingleOrDefault(r => r.Id == userId);
            if (user == null)
                return new IsSuccessResult<int>(ErrorCode.UserNotFound.ToString());

            if ((UserType)user.Type != UserType.Master)
                return new IsSuccessResult<int>(ErrorCode.NoAuthentication.ToString());

            if (user.Company == null)
                return new IsSuccessResult<int>(ErrorCode.NoCompany.ToString());

            if (string.IsNullOrWhiteSpace(data.Name))
                return new IsSuccessResult<int>(ErrorCode.JobNameIsNull.ToString());

            if (string.IsNullOrWhiteSpace(data.What))
                return new IsSuccessResult<int>(ErrorCode.JobWhatIsNull.ToString());

            if (string.IsNullOrWhiteSpace(data.Why))
                return new IsSuccessResult<int>(ErrorCode.JobWhyIsNull.ToString());

            if (string.IsNullOrWhiteSpace(data.How))
                return new IsSuccessResult<int>(ErrorCode.JobHowIsNull.ToString());

            if (string.IsNullOrWhiteSpace(data.Detail))
                return new IsSuccessResult<int>(ErrorCode.JobDetailIsNull.ToString());

            var isAnyJobType = NiHonGoContext.JobTypes.Any(r => r.Id == data.TypeId);
            if (isAnyJobType == false)
                return new IsSuccessResult<int>(ErrorCode.JobTypeIsNull.ToString());

            var isAnyJobStatus = NiHonGoContext.JobStatus.Any(r => r.Id == data.StatusId);
            if (isAnyJobStatus == false)
                return new IsSuccessResult<int>(ErrorCode.JobStatusIsNull.ToString());

            var isAnyArea = NiHonGoContext.Areas.Any(r => r.Id == data.AreaId);
            if (isAnyArea == false)
                return new IsSuccessResult<int>(ErrorCode.AreaIsNull.ToString());

            try
            {
                var result = new IsSuccessResult<int>();
                if (jobId == null)
                {
                    var currentDate = DateTime.UtcNow;
                    var job = new Job
                    {
                        Name = data.Name,
                        What = data.What,
                        Why = data.Why,
                        How = data.How,
                        Detail = data.Detail,
                        JobTypeId = data.TypeId,
                        JobStatusId = data.StatusId,
                        EndSalary = data.EndSalary,
                        StartSalary = data.StartSalary,
                        StartDate = DateTime.UtcNow,
                        Tag = data.Tag,
                        AreaId = data.AreaId,
                        CreateDate = currentDate,
                        UpdateDate = currentDate
                    };
                    user.Company.Jobs.Add(job);

                    NiHonGoContext.SaveChanges();

                    if (!string.IsNullOrWhiteSpace(data.Photo))
                    {
                        var oldPhotoName = data.Photo.Split('\\').Last();
                        var newPhotoName = job.Id + Path.GetExtension(oldPhotoName);
                        File.Move(data.Photo, data.Photo.Replace(oldPhotoName, newPhotoName));

                        job.Photo = newPhotoName;
                        NiHonGoContext.SaveChanges();
                    }

                    result.ReturnObject = job.Id;
                }
                else
                {
                    var job = NiHonGoContext.Jobs.SingleOrDefault(r => r.Id == jobId);
                    if (job == null)
                        return new IsSuccessResult<int>(ErrorCode.JobNotFound.ToString());

                    if (job.CompanyId != user.CompanyId)
                        return new IsSuccessResult<int>(ErrorCode.NoAuthentication.ToString());

                    job.Name = data.Name;
                    job.What = data.What;
                    job.Why = data.Why;
                    job.How = data.How;
                    job.Detail = data.Detail;
                    job.JobTypeId = data.TypeId;
                    job.JobStatusId = data.StatusId;
                    job.EndSalary = data.EndSalary;
                    job.StartSalary = data.StartSalary;
                    job.StartDate = DateTime.UtcNow;
                    job.Tag = data.Tag;
                    job.AreaId = data.AreaId;
                    job.UpdateDate = DateTime.UtcNow;

                    NiHonGoContext.SaveChanges();
                    result.ReturnObject = job.Id;
                }

                return result;
            }
            catch (Exception ex)
            {
                log.Error(ex);

                return new IsSuccessResult<int>(ErrorCode.InternalError.ToString());
            }
        }

        public IsSuccessResult<JobInfo> GetDetail(int jobId)
        {
            var log = GetLogger();
            log.Debug("JobId: {0}", jobId);

            var result = new IsSuccessResult<JobInfo>();
            var job = NiHonGoContext.Jobs
                .Include(r => r.Company)
                .Include(r => r.JobStatus)
                .Include(r => r.JobType)
                .Include(r => r.Area)
                .SingleOrDefault(r => r.Id == jobId);

            if (job == null)
            {
                result.IsSuccess = false;
                result.ErrorCode = ErrorCode.JobNotFound.ToString();
                return result;
            }

            try
            {
                result.ReturnObject = new JobInfo
                {
                    CompanyId = job.Company.Id,
                    CompanyName = job.Company.Name,
                    Name = job.Name,
                    What = job.What,
                    Why = job.Why,
                    How = job.How,
                    Detail = job.Detail,
                    StartSalary = job.StartSalary,
                    EndSalary = job.EndSalary,
                    Type = job.JobType.Display,
                    Status = job.JobStatus.Display,
                    Area = job.Area.Display,
                    TypeId = job.JobTypeId,
                    StatusId = job.JobStatusId,
                    AreaId = job.AreaId,
                    Tag = job.Tag,
                    Photo = job.Photo,
                    CreateDate = job.CreateDate.ToLocalTime().ToString("yyyy-MM-dd"),
                    UpdateDate = job.UpdateDate.ToLocalTime().ToString("yyyy-MM-dd")
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

        public JobListReturn GetList(SearchJob search, int index, int count)
        {
            var log = GetLogger();
            log.Debug("Keyword: {0}, StartSalary: {1}, EndSalary: {2}, index: {3}, count: {4}, CompanyId: {5}",
                search.Keyword, search.StartSalary, search.EndSalary, index, count, search.CompanyId);

            IQueryable<Job> query = NiHonGoContext.Jobs;
            if (search.IsOnlyRecruiting)
                query = query.Where(r => r.JobStatus.Display == "募集中");

            if (search.StartSalary.HasValue)
                query = query.Where(r => r.StartSalary >= search.StartSalary);

            if (search.EndSalary.HasValue)
                query = query.Where(r => r.EndSalary <= search.EndSalary);

            if (search.CompanyId.HasValue)
                query = query.Where(r => r.CompanyId == search.CompanyId.Value);

            if (string.IsNullOrWhiteSpace(search.Keyword) == false)
                query = query.Where(r => r.Name.Contains(search.Keyword) || r.Tag.Contains(search.Keyword) || r.Detail.Contains(search.Keyword));

            if (search.TypeIds != null && search.TypeIds.Count != 0)
                query = query.Where(r => search.TypeIds.Contains(r.JobTypeId));

            if (search.AreaIds != null && search.AreaIds.Count != 0)
                query = query.Where(r => search.AreaIds.Contains(r.AreaId));

            var jobCount = query.Count();
            var list = query.OrderByDescending(r => r.UpdateDate)
                .Skip(index * count).Take(count)
                .Select(r => new
                {
                    Id = r.Id,
                    Name = r.Name,
                    Detail = r.Detail,
                    Photo = r.Photo,
                    Area = r.Area.Display,
                    Type = r.JobType.Display,
                    StartSalary = r.StartSalary,
                    EndSalary = r.EndSalary,
                    UpdateDate = r.UpdateDate,
                    CompanyId = r.Company.Id,
                    CompanyName = r.Company.Name
                })
                 .ToList()
                 .Select(r => new JobSimpleInfo
                 {
                     Id = r.Id,
                     Name = r.Name,
                     Detail = r.Detail.Length > 150 ? r.Detail.Remove(150) + "..." : r.Detail,
                     Photo = r.Photo,
                     Area = r.Area,
                     Type = r.Type,
                     StartSalary = r.StartSalary,
                     EndSalary = r.EndSalary,
                     UpdateDate = r.UpdateDate.ToLocalTime().ToString("yyyy-MM-dd"),
                     CompanyId = r.CompanyId,
                     CompanyName = r.CompanyName
                 }).ToList();

            return new JobListReturn
            {
                List = list,
                Count = jobCount
            };
        }

        public string GetCompanyEmail(int jobId)
        {
            try
            {
                return NiHonGoContext.Jobs.SingleOrDefault(r => r.Id == jobId)?.Company.Users.FirstOrDefault()?.Email;
            }
            catch
            {
                return "";
            }

        }
    }
}