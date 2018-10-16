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
        /// <summary>
        /// User Logic
        /// </summary>
        public UserLogic() : base() { }

        /// <summary>
        /// check email and password
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns>Master.Id</returns>
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
                .Select(r => new
                {
                    r.Id,
                    //r.IsDisable
                })
                .FirstOrDefault();

            // email or password wrong
            if (user == null)
                return new IsSuccessResult<int>(ErrorCode.EmailOrPasswordWrong.ToString());

            return new IsSuccessResult<int>() { ReturnObject = user.Id };
        }

        /// <summary>
        /// get login user info
        /// </summary>
        /// <param name="id">User.Id</param>
        /// <returns>login user info</returns>
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

        /// <summary>
        /// change password
        /// </summary>
        /// <param name="id">User.Id</param>
        /// <param name="oldPassword"></param>
        /// <param name="newPassword"></param>
        /// <returns>IsSuccess</returns>
        public IsSuccessResult ChangePassword(int id, string oldPassword, string newPassword)
        {
            var log = GetLogger();
            log.Debug("id: {0}, oldPassword: {1}, newPassword: {2}", id, oldPassword, newPassword);

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

            #region change db date
            user.Password = Cryptography.EncryptBySHA1(newPassword);

            NiHonGoContext.SaveChanges();
            #endregion // change db date

            log.Info("使用者帳號\"{0}\"變更密碼成功", user.Email);

            return new IsSuccessResult();
        }

        /// <summary>
        /// get user info
        /// </summary>
        /// <returns></returns>
        public IsSuccessResult<CreateUser> GetUser(int id)
        {
            var log = GetLogger();
            log.Debug("id: {0}", id);

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
                }
            };
        }

        /// <summary>
        /// create a new user
        /// </summary>
        /// <returns></returns>
        public IsSuccessResult<UserInfo> AddUser(CreateUser data, UserType type)
        {
            var log = GetLogger();
            log.Debug("Email: {0}, Name: {1}, Phone:{2}, Type:{3}, Password:{4}, Display: {5}, IsDisable: {6}",
                data.Email, data.Name, data.Phone, type, data.Password, data.Display, data.IsDisable);

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

            if ((int)type > 2)
                type = UserType.User;

            try
            {
                var user = NiHonGoContext.Users.Add(new User
                {
                    Name = data.Name,
                    Password = Cryptography.EncryptBySHA1(data.Password),
                    Email = data.Email,
                    Type = (int)type,
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

        public IsSuccessResult UpgrateToMaster(int id)
        {
            var log = GetLogger();
            log.Debug("id: {0}", id);

            var user = NiHonGoContext.Users.SingleOrDefault(r => r.Id == id);
            if (user == null)
                return new IsSuccessResult(ErrorCode.UserNotFound.ToString());

            if (user.Type == (int)UserType.User)
                user.Type = (int)UserType.Master;

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

        public UserListReturn GetList(string email, int index, int count)
        {
            var log = GetLogger();
            log.Debug("email: {0}, index: {1}, count: {2}", email, index, count);

            IQueryable<User> query = NiHonGoContext.Users;
            if (string.IsNullOrWhiteSpace(email) == false)
                query = query.Where(r => r.Email.Contains(email));

            var userCount = query.Count();
            var list = query.OrderByDescending(r => r.Id)
                .Skip(index * count).Take(count)
                .Select(r => new UserItem
                {
                    Id = r.Id,
                    Email = r.Email,
                }).ToList();


            return new UserListReturn
            {
                List = list,
                Count = userCount
            };
        }
    }
}