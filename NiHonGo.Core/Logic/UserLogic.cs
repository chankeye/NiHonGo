using NiHonGo.Core.DTO;
using NiHonGo.Core.DTO.User;
using NiHonGo.Core.Enum;
using NiHonGo.Core.Utilities;
using NiHonGo.Data.Models;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace NiHonGo.Core.Logic
{
    public class UserLogic : _BaseLogic
    {
        public UserLogic() : base() { }

        public IsSuccessResult<int> IsValid(string email, string password)
        {
            #region check parameter
            // email
            if (string.IsNullOrWhiteSpace(email))
                return new IsSuccessResult<int>(ErrorCode.EmailIsNull.ToString());

            // password
            if (string.IsNullOrWhiteSpace(password))
                return new IsSuccessResult<int>(ErrorCode.PasswordIsNull.ToString());
            #endregion // check parameter

            // password after encryption
            var passwordEncrypt = Cryptography.EncryptBySHA1(password);

            var user = NiHonGoContext.Users
                .Where(r =>
                    r.Email == email &&
                    r.Password == passwordEncrypt
                )
                .Select(r => new { r.Id })
                .FirstOrDefault();

            // email or password wrong
            if (user == null)
                return new IsSuccessResult<int>(ErrorCode.EmailOrPasswordWrong.ToString());

            return new IsSuccessResult<int>() { ReturnObject = user.Id };
        }

        public LoginInfo GetLoginInfo(int id)
        {
            var user = NiHonGoContext.Users
                .Where(r => r.Id == id)
                .Select(r => new
                {
                    r.Id,
                    r.Name,
                    r.Email,
                    r.Type
                }).Single();

            return new LoginInfo
            {
                Id = user.Id,
                Display = user.Name,
                Email = user.Email,
                UserType = (UserType)user.Type
            };
        }

        public IsSuccessResult ChangePassword(int id, string oldPassword, string newPassword)
        {
            var log = GetLogger();

            #region check parameter

            if (string.IsNullOrWhiteSpace(oldPassword))
                return new IsSuccessResult(ErrorCode.OldPasswordIsNull.ToString());
            oldPassword = oldPassword.Trim();

            if (string.IsNullOrWhiteSpace(newPassword))
                return new IsSuccessResult(ErrorCode.NewPasswordIsNull.ToString());
            newPassword = newPassword.Trim();

            // get user
            var user = NiHonGoContext.Users
                .SingleOrDefault(r => r.Id == id);

            // user not found
            if (user == null)
                return new IsSuccessResult(ErrorCode.UserNotFound.ToString());

            // check old password
            if (user.Password != Cryptography.EncryptBySHA1(oldPassword))
                return new IsSuccessResult(ErrorCode.OldPasswordWrong.ToString());

            // old pwd and new pwd is the same
            if (oldPassword == newPassword)
                return new IsSuccessResult();
            #endregion // check parameter

            try
            {
                user.Password = Cryptography.EncryptBySHA1(newPassword);
                NiHonGoContext.SaveChanges();
            }
            catch (Exception ex)
            {
                log.Error(ex);

                return new IsSuccessResult(ErrorCode.ChangePasswordFailed.ToString());
            }

            return new IsSuccessResult();
        }

        public IsSuccessResult<CreateUser> GetUser(int id)
        {
            var log = GetLogger();

            var user = NiHonGoContext.Users
                .SingleOrDefault(r => r.Id == id);
            if (user == null)
                return new IsSuccessResult<CreateUser>(ErrorCode.UserNotFound.ToString());

            return new IsSuccessResult<CreateUser>
            {
                ReturnObject = new CreateUser
                {
                    Email = user.Email,
                    Name = user.Name,
                    Password = user.Password,
                    CreateDateTime = user.CreateDateTime.ToLocalTime().ToString("yyyy-MM-dd")
                }
            };
        }

        public IsSuccessResult<UserInfo> AddUser(CreateUser data, UserType type)
        {
            var log = GetLogger();

            if (string.IsNullOrWhiteSpace(data.Email))
                return new IsSuccessResult<UserInfo>(ErrorCode.EmailIsNull.ToString());
            data.Email = data.Email.Trim();
            if (Regex.IsMatch(data.Email, Constant.PatternEmail) == false)
                return new IsSuccessResult<UserInfo>(ErrorCode.EmailFormatIsWrong.ToString());

            if (string.IsNullOrWhiteSpace(data.Name))
                return new IsSuccessResult<UserInfo>(ErrorCode.NameIsNull.ToString());
            data.Name = data.Name.Trim();

            if (string.IsNullOrWhiteSpace(data.Password))
                return new IsSuccessResult<UserInfo>(ErrorCode.PasswordIsNull.ToString());
            data.Password = data.Password.Trim();

            var isAny = NiHonGoContext.Users
                .Any(r => r.Email == data.Email);
            if (isAny)
                return new IsSuccessResult<UserInfo>(ErrorCode.AlreadyHadThisEmail.ToString());

            if ((int)type > 1)
                type = UserType.User;

            try
            {
                var user = NiHonGoContext.Users.Add(new User
                {
                    Name = data.Name,
                    Password = Cryptography.EncryptBySHA1(data.Password),
                    Email = data.Email,
                    Type = (int)type,
                    CreateDateTime = DateTime.UtcNow
                });
                NiHonGoContext.SaveChanges();

                return new IsSuccessResult<UserInfo>
                {
                    ReturnObject = new UserInfo
                    {
                        Id = user.Id,
                        Email = user.Email
                    }
                };
            }
            catch (Exception ex)
            {
                log.Error(ex);

                return new IsSuccessResult<UserInfo>(ErrorCode.InternalError.ToString());
            }
        }

        /// <summary>
        /// add admin if no user in db
        /// </summary>
        public void HasAnyUser()
        {
            var isAny = NiHonGoContext.Users.Any();

            if (isAny == false)
            {
                NiHonGoContext.Users.Add(new User
                {
                    Email = Constant.DefaultAdminEmail,
                    Password = Cryptography.EncryptBySHA1(Constant.DefaultPassword),
                    Name = "管理員",
                    CreateDateTime = DateTime.UtcNow
                });

                NiHonGoContext.SaveChanges();

                new SystemLogic().DBDataInit();
            }
        }

        public IsSuccessResult UpgrateToAdmin(int id)
        {
            var log = GetLogger();

            var user = NiHonGoContext.Users.SingleOrDefault(r => r.Id == id);
            if (user == null)
                return new IsSuccessResult(ErrorCode.UserNotFound.ToString());

            if (user.Type == (int)UserType.User)
                user.Type = (int)UserType.Admin;

            try
            {
                NiHonGoContext.SaveChanges();
                return new IsSuccessResult();
            }
            catch (Exception ex)
            {
                log.Error(ex);

                return new IsSuccessResult(ErrorCode.InternalError.ToString());
            }
        }

        public UserInfoList GetList(string email, int index, int count)
        {
            var log = GetLogger();
            log.Debug("email: {0}, index: {1}, count: {2}", email, index, count);

            IQueryable<User> query = NiHonGoContext.Users;
            if (string.IsNullOrWhiteSpace(email) == false)
                query = query.Where(r => r.Email.Contains(email));

            var userCount = query.Count();
            var list = query.OrderByDescending(r => r.Id)
                .Skip(index * count).Take(count)
                .Select(r => new UserInfo
                {
                    Id = r.Id,
                    Email = r.Email,
                }).ToList();


            return new UserInfoList
            {
                List = list,
                Count = userCount
            };
        }
    }
}