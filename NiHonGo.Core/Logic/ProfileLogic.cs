using NiHonGo.Core.DTO;
using NiHonGo.Core.DTO.Profile;
using NiHonGo.Core.DTO.User;
using NiHonGo.Core.Enum;
using NiHonGo.Core.Utilities;
using NiHonGo.Data.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;

namespace NiHonGo.Core.Logic
{
    public class ProfileLogic : _BaseLogic
    {
        /// <summary>
        /// Profile Logic
        /// </summary>
        public ProfileLogic() : base() { }

        public IsSuccessResult UpdatePhoto(int userId, string photo)
        {
            var log = GetLogger();
            log.Debug("UserID: {0}, Photo: {1}"
                , userId, photo);

            var user = NiHonGoContext.Users
                .SingleOrDefault(r => r.Id == userId);
            if (user == null)
                return new IsSuccessResult(ErrorCode.UserNotFound.ToString());

            if (string.IsNullOrWhiteSpace(photo))
                return new IsSuccessResult(ErrorCode.PhotoIsNull.ToString());

            if (System.Text.RegularExpressions.Regex.IsMatch(photo, Constant.PatternImage, RegexOptions.IgnoreCase) == false)
                return new IsSuccessResult(ErrorCode.PhotoFormatIsWrong.ToString());

            try
            {
                user.Resume.Photo = photo;
                user.Resume.UpdateDate = DateTime.UtcNow;

                NiHonGoContext.SaveChanges();

                return new IsSuccessResult();
            }
            catch (Exception ex)
            {
                log.Error(ex);

                return new IsSuccessResult<UserInfo>(ErrorCode.InternalError.ToString());
            }
        }

        public IsSuccessResult Edit(int userId, EditProfile data)
        {
            var log = GetLogger();
            log.Debug("UserID: {0}, Introduction: {1}, Age: {2}, IsMarried: {3}, Name: {4}, Phone: {5}, Email: {6}, Display: {7}"
                , userId, data.Introduction, data.Age, data.IsMarried, data.Name, data.Phone, data.Email, data.Display);

            var user = NiHonGoContext.Users
                .SingleOrDefault(r => r.Id == userId);
            if (user == null)
                return new IsSuccessResult(ErrorCode.UserNotFound.ToString());

            if (string.IsNullOrWhiteSpace(data.Name))
                return new IsSuccessResult(ErrorCode.NameIsNull.ToString());

            if (string.IsNullOrWhiteSpace(data.Email))
                return new IsSuccessResult(ErrorCode.EmailIsNull.ToString());

            if (System.Text.RegularExpressions.Regex.IsMatch(data.Email, Constant.PatternEmail) == false)
                return new IsSuccessResult(ErrorCode.EmailFormatIsWrong.ToString());

            if (data.VisaId.HasValue)
            {
                var isAnyVisa = NiHonGoContext.Visas.Any(r => r.Id == data.VisaId.Value);
                if (isAnyVisa == false)
                    return new IsSuccessResult(ErrorCode.VisaNotFound.ToString());
            }

            var isAny = NiHonGoContext.Users
                .Any(r => r.Email == data.Email && r.Id != userId);
            if (isAny)
                return new IsSuccessResult(ErrorCode.AlreadyHadThisEmail.ToString());

            try
            {
                user.Email = data.Email;
                user.Name = data.Name;
                user.Phone = data.Phone;
                user.Display = data.Display;
                user.Resume.VisaId = data.VisaId;
                user.Resume.Introduction = data.Introduction;
                user.Resume.Age = data.Age;
                user.Resume.IsMarried = data.IsMarried;
                user.Resume.UpdateDate = DateTime.UtcNow;

                NiHonGoContext.SaveChanges();

                return new IsSuccessResult();
            }
            catch (Exception ex)
            {
                log.Error(ex);

                return new IsSuccessResult(ErrorCode.InternalError.ToString());
            }
        }

        public IsSuccessResult<ProfileInfo> GetDetail(int userId)
        {
            var log = GetLogger();
            log.Debug("userId: {0}", userId);

            var user = NiHonGoContext.Users
                .Include(r => r.Resume)
                .SingleOrDefault(r => r.Id == userId);

            if (user == null)
                return new IsSuccessResult<ProfileInfo>(ErrorCode.UserNotFound.ToString());

            var result = new IsSuccessResult<ProfileInfo>();

            try
            {
                if (user.Resume == null)
                {
                    var currentDate = DateTime.UtcNow;
                    user.Resume = new Video
                    {
                        CreateDate = currentDate,
                        UpdateDate = currentDate
                    };
                    NiHonGoContext.SaveChanges();
                }

                result.ReturnObject = new ProfileInfo
                {
                    Email = user.Email,
                    Name = user.Name,
                    Phone = user.Phone,
                    Display = user.Display,
                    Age = user.Resume.Age,
                    Introduction = user.Resume.Introduction,
                    Photo = user.Resume.Photo,
                    IsMarried = user.Resume.IsMarried,
                    VisaId = user.Resume.VisaId,
                    Visa = user.Resume.Visa?.Display,
                    CreateDate = user.Resume.CreateDate.ToLocalTime().ToString("yyyy-MM-dd"),
                    UpdateDate = user.Resume.UpdateDate.ToLocalTime().ToString("yyyy-MM-dd"),
                    ExperienceList = user.Resume.Experiences.Select(r => new ExperienceInfo
                    {
                        Name = r.Name,
                        JobTitle = r.JobTitle,
                        Detail = r.Detail,
                        StartDate = r.StartDate.HasValue ? r.StartDate.Value.ToLocalTime().ToString("yyyy-MM-dd") : null,
                        EndDate = r.EndDate.HasValue ? r.EndDate.Value.ToLocalTime().ToString("yyyy-MM-dd") : null,
                    }).ToList(),
                    WorksList = user.Resume.Works.Select(r => new WorksInfo
                    {
                        Name = r.Name,
                        Detail = r.Detail,
                        StartDate = r.StartDate.HasValue ? r.StartDate.Value.ToLocalTime().ToString("yyyy-MM-dd") : null,
                        EndDate = r.EndDate.HasValue ? r.EndDate.Value.ToLocalTime().ToString("yyyy-MM-dd") : null,
                    }).ToList(),
                    SchoolList = user.Resume.Schools.Select(r => new SchoolInfo
                    {
                        Name = r.Name,
                        Department = r.Department,
                        Detail = r.Detail,
                        StartDate = r.StartDate.HasValue ? r.StartDate.Value.ToLocalTime().ToString("yyyy-MM-dd") : null,
                        EndDate = r.EndDate.HasValue ? r.EndDate.Value.ToLocalTime().ToString("yyyy-MM-dd") : null,
                    }).ToList(),
                    LanguageList = user.Resume.Languages.Select(r => new KeyValueItem
                    {
                        Id = r.Id,
                        Display = r.Display
                    }).ToList(),
                    SkillList = user.Resume.Skills.Select(r => new KeyValueItem
                    {
                        Id = r.Id,
                        Display = r.Display
                    }).ToList(),
                    LicenseList = user.Resume.Licenses.Select(r => new KeyValueItem
                    {
                        Id = r.Id,
                        Display = r.Display
                    }).ToList()
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

        public IsSuccessResult<ExperienceInfo> EditExperience(int userId, ExperienceInfo data)
        {
            var log = GetLogger();
            log.Debug("UserID: {0}, Detail: {1}, StartDate: {2}, EndDate: {3}, Name: {4}, JobTitle: {5}"
                , userId, data.Detail, data.StartDate, data.EndDate, data.Name, data.JobTitle);

            var result = new IsSuccessResult<ExperienceInfo>();
            var user = NiHonGoContext.Users
                .SingleOrDefault(r => r.Id == userId);
            if (user == null)
            {
                result.IsSuccess = false;
                result.ErrorCode = ErrorCode.UserNotFound.ToString();
                return result;
            }

            if (string.IsNullOrWhiteSpace(data.Name))
            {
                result.IsSuccess = false;
                result.ErrorCode = ErrorCode.ExperienceCompanyNameIsNull.ToString();
                return result;
            }

            if (string.IsNullOrWhiteSpace(data.JobTitle))
            {
                result.IsSuccess = false;
                result.ErrorCode = ErrorCode.ExperienceJobTitleIsNull.ToString();
                return result;
            }

            try
            {
                var experience = user.Resume.Experiences.SingleOrDefault(r => r.Name == data.Name);
                if (experience == null)
                {
                    experience = new Experience
                    {
                        Name = data.Name,
                        Detail = data.Detail,
                        JobTitle = data.JobTitle
                    };

                    if (string.IsNullOrWhiteSpace(data.StartDate) == false)
                        experience.StartDate = DateTimeOffset.Parse(data.StartDate).UtcDateTime;
                    if (string.IsNullOrWhiteSpace(data.EndDate) == false)
                        experience.EndDate = DateTimeOffset.Parse(data.EndDate).UtcDateTime;

                    user.Resume.Experiences.Add(experience);
                }
                else
                {
                    experience.Detail = data.Detail;
                    experience.JobTitle = data.JobTitle;
                    if (string.IsNullOrWhiteSpace(data.StartDate) == false)
                        experience.StartDate = DateTimeOffset.Parse(data.StartDate).UtcDateTime;
                    else
                        experience.StartDate = null;

                    if (string.IsNullOrWhiteSpace(data.EndDate) == false)
                        experience.EndDate = DateTimeOffset.Parse(data.EndDate).UtcDateTime;
                    else
                        experience.EndDate = null;
                }

                NiHonGoContext.SaveChanges();
                result.ReturnObject = new ExperienceInfo
                {
                    Name = experience.Name,
                    JobTitle = experience.JobTitle,
                    Detail = experience.Detail,
                    StartDate = experience.StartDate.HasValue ? experience.StartDate.Value.ToLocalTime().ToString("yyyy-MM-dd") : null,
                    EndDate = experience.EndDate.HasValue ? experience.EndDate.Value.ToLocalTime().ToString("yyyy-MM-dd") : null,
                };
            }
            catch (Exception ex)
            {
                log.Error(ex);

                result.IsSuccess = false;
                result.ErrorCode = ErrorCode.InternalError.ToString();
            }

            return result;
        }

        public IsSuccessResult<WorksInfo> EditWorks(int userId, WorksInfo data)
        {
            var log = GetLogger();
            log.Debug("UserID: {0}, Detail: {1}, StartDate: {2}, EndDate: {3}, Name: {4}"
                , userId, data.Detail, data.StartDate, data.EndDate, data.Name);

            var result = new IsSuccessResult<WorksInfo>();
            var user = NiHonGoContext.Users
                .SingleOrDefault(r => r.Id == userId);
            if (user == null)
            {
                result.IsSuccess = false;
                result.ErrorCode = ErrorCode.UserNotFound.ToString();
                return result;
            }

            if (string.IsNullOrWhiteSpace(data.Name))
            {
                result.IsSuccess = false;
                result.ErrorCode = ErrorCode.WorksNameIsNull.ToString();
                return result;
            }

            try
            {
                var works = user.Resume.Works.SingleOrDefault(r => r.Name == data.Name);
                if (works == null)
                {
                    works = new Work
                    {
                        Name = data.Name,
                        Detail = data.Detail
                    };

                    if (string.IsNullOrWhiteSpace(data.StartDate) == false)
                        works.StartDate = DateTimeOffset.Parse(data.StartDate).UtcDateTime;
                    if (string.IsNullOrWhiteSpace(data.EndDate) == false)
                        works.EndDate = DateTimeOffset.Parse(data.EndDate).UtcDateTime;

                    user.Resume.Works.Add(works);
                }
                else
                {
                    works.Detail = data.Detail;
                    if (string.IsNullOrWhiteSpace(data.StartDate) == false)
                        works.StartDate = DateTimeOffset.Parse(data.StartDate).UtcDateTime;
                    else
                        works.StartDate = null;

                    if (string.IsNullOrWhiteSpace(data.EndDate) == false)
                        works.EndDate = DateTimeOffset.Parse(data.EndDate).UtcDateTime;
                    else
                        works.EndDate = null;
                }

                NiHonGoContext.SaveChanges();
                result.ReturnObject = new WorksInfo
                {
                    Name = works.Name,
                    Detail = works.Detail,
                    StartDate = works.StartDate.HasValue ? works.StartDate.Value.ToLocalTime().ToString("yyyy-MM-dd") : null,
                    EndDate = works.EndDate.HasValue ? works.EndDate.Value.ToLocalTime().ToString("yyyy-MM-dd") : null,
                };
            }
            catch (Exception ex)
            {
                log.Error(ex);

                result.IsSuccess = false;
                result.ErrorCode = ErrorCode.InternalError.ToString();
            }

            return result;
        }

        public IsSuccessResult<SchoolInfo> EditSchool(int userId, SchoolInfo data)
        {
            var log = GetLogger();
            log.Debug("UserID: {0}, Detail: {1}, StartDate: {2}, EndDate: {3}, Name: {4}, Department: {5}"
                , userId, data.Detail, data.StartDate, data.EndDate, data.Name, data.Department);

            var result = new IsSuccessResult<SchoolInfo>();
            var user = NiHonGoContext.Users
                .SingleOrDefault(r => r.Id == userId);
            if (user == null)
            {
                result.IsSuccess = false;
                result.ErrorCode = ErrorCode.UserNotFound.ToString();
                return result;
            }

            if (string.IsNullOrWhiteSpace(data.Name))
            {
                result.IsSuccess = false;
                result.ErrorCode = ErrorCode.SchoolNameIsNull.ToString();
                return result;
            }

            try
            {
                var school = user.Resume.Schools.SingleOrDefault(r => r.Name == data.Name);
                if (school == null)
                {
                    school = new School
                    {
                        Name = data.Name,
                        Detail = data.Detail,
                        Department = data.Department
                    };

                    if (string.IsNullOrWhiteSpace(data.StartDate) == false)
                        school.StartDate = DateTimeOffset.Parse(data.StartDate).UtcDateTime;
                    if (string.IsNullOrWhiteSpace(data.EndDate) == false)
                        school.EndDate = DateTimeOffset.Parse(data.EndDate).UtcDateTime;

                    user.Resume.Schools.Add(school);
                }
                else
                {
                    school.Detail = data.Detail;
                    school.Department = data.Department;
                    if (string.IsNullOrWhiteSpace(data.StartDate) == false)
                        school.StartDate = DateTimeOffset.Parse(data.StartDate).UtcDateTime;
                    else
                        school.StartDate = null;

                    if (string.IsNullOrWhiteSpace(data.EndDate) == false)
                        school.EndDate = DateTimeOffset.Parse(data.EndDate).UtcDateTime;
                    else
                        school.EndDate = null;
                }

                NiHonGoContext.SaveChanges();
                result.ReturnObject = new SchoolInfo
                {
                    Name = school.Name,
                    Department = school.Department,
                    Detail = school.Detail,
                    StartDate = school.StartDate.HasValue ? school.StartDate.Value.ToLocalTime().ToString("yyyy-MM-dd") : null,
                    EndDate = school.EndDate.HasValue ? school.EndDate.Value.ToLocalTime().ToString("yyyy-MM-dd") : null,
                };
            }
            catch (Exception ex)
            {
                log.Error(ex);

                result.IsSuccess = false;
                result.ErrorCode = ErrorCode.InternalError.ToString();
            }

            return result;
        }

        public IsSuccessResult<KeyValueItem> EditSkill(int userId, string skillName)
        {
            var log = GetLogger();
            log.Debug("UserID: {0}, SkillName: {1}", userId, skillName);

            var result = new IsSuccessResult<KeyValueItem>();
            var user = NiHonGoContext.Users
                .SingleOrDefault(r => r.Id == userId);
            if (user == null)
            {
                result.IsSuccess = false;
                result.ErrorCode = ErrorCode.UserNotFound.ToString();
                return result;
            }

            if (string.IsNullOrWhiteSpace(skillName))
            {
                result.IsSuccess = false;
                result.ErrorCode = ErrorCode.SkillNameIsNull.ToString();
                return result;
            }

            try
            {
                var skill = NiHonGoContext.Skills.SingleOrDefault(r => r.Display == skillName);
                if (skill == null)
                {
                    skill = new Skill
                    {
                        Display = skillName
                    };

                    NiHonGoContext.Skills.Add(skill);
                    NiHonGoContext.SaveChanges();

                    user.Resume.Skills.Add(skill);
                }
                else
                    user.Resume.Skills.Add(skill);

                NiHonGoContext.SaveChanges();

                result.ReturnObject = new KeyValueItem
                {
                    Id = skill.Id,
                    Display = skill.Display
                };
            }
            catch (Exception ex)
            {
                log.Error(ex);

                result.IsSuccess = false;
                result.ErrorCode = ErrorCode.InternalError.ToString();
            }

            return result;
        }

        public IsSuccessResult<KeyValueItem> EditLanguage(int userId, string languageName)
        {
            var log = GetLogger();
            log.Debug("UserID: {0}, LanguageName: {1}", userId, languageName);

            var result = new IsSuccessResult<KeyValueItem>();
            var user = NiHonGoContext.Users
                .SingleOrDefault(r => r.Id == userId);
            if (user == null)
            {
                result.IsSuccess = false;
                result.ErrorCode = ErrorCode.UserNotFound.ToString();
                return result;
            }

            if (string.IsNullOrWhiteSpace(languageName))
            {
                result.IsSuccess = false;
                result.ErrorCode = ErrorCode.LanguageNameIsNull.ToString();
                return result;
            }

            try
            {
                var language = NiHonGoContext.Languages.SingleOrDefault(r => r.Display == languageName);
                if (language == null)
                {
                    language = new Language
                    {
                        Display = languageName
                    };

                    NiHonGoContext.Languages.Add(language);
                    NiHonGoContext.SaveChanges();

                    user.Resume.Languages.Add(language);
                }
                else
                    user.Resume.Languages.Add(language);

                NiHonGoContext.SaveChanges();

                result.ReturnObject = new KeyValueItem
                {
                    Id = language.Id,
                    Display = language.Display
                };
            }
            catch (Exception ex)
            {
                log.Error(ex);

                result.IsSuccess = false;
                result.ErrorCode = ErrorCode.InternalError.ToString();
            }

            return result;
        }

        public IsSuccessResult<KeyValueItem> EditLicense(int userId, string licenseName)
        {
            var log = GetLogger();
            log.Debug("UserID: {0}, LicenseName: {1}", userId, licenseName);

            var result = new IsSuccessResult<KeyValueItem>();
            var user = NiHonGoContext.Users
                .SingleOrDefault(r => r.Id == userId);
            if (user == null)
            {
                result.IsSuccess = false;
                result.ErrorCode = ErrorCode.UserNotFound.ToString();
                return result;
            }

            if (string.IsNullOrWhiteSpace(licenseName))
            {
                result.IsSuccess = false;
                result.ErrorCode = ErrorCode.LicenseNameIsNull.ToString();
                return result;
            }

            try
            {
                var license = NiHonGoContext.Licenses.SingleOrDefault(r => r.Display == licenseName);
                if (license == null)
                {
                    license = new License
                    {
                        Display = licenseName
                    };

                    NiHonGoContext.Licenses.Add(license);
                    NiHonGoContext.SaveChanges();

                    user.Resume.Licenses.Add(license);
                }
                else
                    user.Resume.Licenses.Add(license);

                NiHonGoContext.SaveChanges();

                result.ReturnObject = new KeyValueItem
                {
                    Id = license.Id,
                    Display = license.Display
                };
            }
            catch (Exception ex)
            {
                log.Error(ex);

                result.IsSuccess = false;
                result.ErrorCode = ErrorCode.InternalError.ToString();
            }

            return result;
        }

        public IsSuccessResult DeleteExperience(int userId, string name)
        {
            var log = GetLogger();
            log.Debug("UserID: {0}, Name: {1}", userId, name);

            var user = NiHonGoContext.Users
                .SingleOrDefault(r => r.Id == userId);
            if (user == null)
                return new IsSuccessResult(ErrorCode.UserNotFound.ToString());

            if (string.IsNullOrWhiteSpace(name))
                return new IsSuccessResult(ErrorCode.ExperienceNotFound.ToString());

            try
            {
                var experience = user.Resume.Experiences.SingleOrDefault(r => r.Name == name);
                if (experience == null)
                    return new IsSuccessResult(ErrorCode.ExperienceNotFound.ToString());

                var result = user.Resume.Experiences.Remove(experience);
                if (result == false)
                    throw new Exception();

                NiHonGoContext.SaveChanges();

                return new IsSuccessResult();
            }
            catch (Exception ex)
            {
                log.Error(ex);

                return new IsSuccessResult(ErrorCode.InternalError.ToString());
            }
        }

        public IsSuccessResult DeleteWorks(int userId, string name)
        {
            var log = GetLogger();
            log.Debug("UserID: {0}, Name: {1}", userId, name);

            var user = NiHonGoContext.Users
                .SingleOrDefault(r => r.Id == userId);
            if (user == null)
                return new IsSuccessResult(ErrorCode.UserNotFound.ToString());

            if (string.IsNullOrWhiteSpace(name))
                return new IsSuccessResult(ErrorCode.WorksNotFound.ToString());

            try
            {
                var works = user.Resume.Works.SingleOrDefault(r => r.Name == name);
                if (works == null)
                    return new IsSuccessResult(ErrorCode.WorksNotFound.ToString());

                var result = user.Resume.Works.Remove(works);
                if (result == false)
                    throw new Exception();

                NiHonGoContext.SaveChanges();

                return new IsSuccessResult();
            }
            catch (Exception ex)
            {
                log.Error(ex);

                return new IsSuccessResult(ErrorCode.InternalError.ToString());
            }
        }

        public IsSuccessResult DeleteSchool(int userId, string name)
        {
            var log = GetLogger();
            log.Debug("UserID: {0}, Name: {1}", userId, name);

            var user = NiHonGoContext.Users
                .SingleOrDefault(r => r.Id == userId);
            if (user == null)
                return new IsSuccessResult(ErrorCode.UserNotFound.ToString());

            if (string.IsNullOrWhiteSpace(name))
                return new IsSuccessResult(ErrorCode.SchoolNotFound.ToString());

            try
            {
                var school = user.Resume.Schools.SingleOrDefault(r => r.Name == name);
                if (school == null)
                    return new IsSuccessResult(ErrorCode.SchoolNotFound.ToString());

                var result = user.Resume.Schools.Remove(school);
                if (result == false)
                    throw new Exception();

                NiHonGoContext.SaveChanges();

                return new IsSuccessResult();
            }
            catch (Exception ex)
            {
                log.Error(ex);

                return new IsSuccessResult(ErrorCode.InternalError.ToString());
            }
        }

        public IsSuccessResult DeleteSkill(int userId, int skillId)
        {
            var log = GetLogger();
            log.Debug("UserID: {0}, SkillId: {1}", userId, skillId);

            var user = NiHonGoContext.Users
                .SingleOrDefault(r => r.Id == userId);
            if (user == null)
                return new IsSuccessResult(ErrorCode.UserNotFound.ToString());

            try
            {
                var skill = user.Resume.Skills.SingleOrDefault(r => r.Id == skillId);
                if (skill == null)
                    return new IsSuccessResult(ErrorCode.SkillNotFound.ToString());

                var result = user.Resume.Skills.Remove(skill);
                if (result == false)
                    throw new Exception();

                NiHonGoContext.SaveChanges();

                return new IsSuccessResult();
            }
            catch (Exception ex)
            {
                log.Error(ex);

                return new IsSuccessResult(ErrorCode.InternalError.ToString());
            }
        }

        public IsSuccessResult DeleteLanguage(int userId, int languageId)
        {
            var log = GetLogger();
            log.Debug("UserID: {0}, LanguageId: {1}", userId, languageId);

            var user = NiHonGoContext.Users
                .SingleOrDefault(r => r.Id == userId);
            if (user == null)
                return new IsSuccessResult(ErrorCode.UserNotFound.ToString());

            try
            {
                var language = user.Resume.Languages.SingleOrDefault(r => r.Id == languageId);
                if (language == null)
                    return new IsSuccessResult(ErrorCode.LanguageNotFound.ToString());

                var result = user.Resume.Languages.Remove(language);
                if (result == false)
                    throw new Exception();

                NiHonGoContext.SaveChanges();

                return new IsSuccessResult();
            }
            catch (Exception ex)
            {
                log.Error(ex);

                return new IsSuccessResult(ErrorCode.InternalError.ToString());
            }
        }

        public IsSuccessResult DeleteLicense(int userId, int licenseId)
        {
            var log = GetLogger();
            log.Debug("UserID: {0}, LicenseId: {1}", userId, licenseId);

            var user = NiHonGoContext.Users
                .SingleOrDefault(r => r.Id == userId);
            if (user == null)
                return new IsSuccessResult(ErrorCode.UserNotFound.ToString());

            try
            {
                var license = user.Resume.Licenses.SingleOrDefault(r => r.Id == licenseId);
                if (license == null)
                    return new IsSuccessResult(ErrorCode.LicenseNotFound.ToString());

                var result = user.Resume.Licenses.Remove(license);
                if (result == false)
                    throw new Exception();

                NiHonGoContext.SaveChanges();

                return new IsSuccessResult();
            }
            catch (Exception ex)
            {
                log.Error(ex);

                return new IsSuccessResult(ErrorCode.InternalError.ToString());
            }
        }
    }
}