using NiHonGo.Core.DTO;
using NiHonGo.Core.DTO.Profile;
using NiHonGo.Core.DTO.User;
using NiHonGo.Core.Enum;
using NiHonGo.Core.Logic;
using System;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace NiHonGo.Portal.Controllers
{
    [Authorize]
    public class UserController : _BaseController
    {
        UserLogic UserLogic
        {
            get
            {
                if (_userLogic == null)
                    _userLogic = new UserLogic();
                return _userLogic;
            }
        }
        UserLogic _userLogic;

        ProfileLogic ProfileLogic
        {
            get
            {
                if (_profileLogic == null)
                    _profileLogic = new ProfileLogic();
                return _profileLogic;
            }
        }
        ProfileLogic _profileLogic;

        [HttpGet]
        public ActionResult Index() => View();

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Register() => View();

        [HttpGet]
        [AllowAnonymous]
        public ActionResult MasterRegister() => View();

        [HttpPost]
        [AllowAnonymous]
        public ActionResult RegisterSubmit(CreateUser data)
        {
            var result = UserLogic.AddUser(data, UserType.User);

            return Json(result);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult MasterRegisterSubmit(CreateUser data)
        {
            var result = UserLogic.AddUser(data, UserType.Master);

            return Json(result);
        }

        [HttpGet]
        public ActionResult Profile(string id)
        {
            try
            {
                var userId = int.Parse(id);
                return View();
            }
            catch
            {
                var result = new IsSuccessResult(ErrorCode.UserNotFound.ToString());
                return Redirect("/Error/PageNotFind");
            }
        }

        [HttpPost]
        public ActionResult ProfileInit(string id)
        {
            try
            {
                var userId = int.Parse(id);
                var result = ProfileLogic.GetDetail(userId);
                return Json(result);
            }
            catch
            {
                var result = new IsSuccessResult(ErrorCode.UserNotFound.ToString());
                return Json(result);
            }
        }

        [HttpGet]
        public ActionResult ProfileEdit() => View();

        [HttpPost]
        public ActionResult ProfileEditInit()
        {
            var result = ProfileLogic.GetDetail(GetOperation().UserId);

            return Json(result);
        }

        [HttpPost]
        public ActionResult ProfileEditSubmit(EditProfile data)
        {
            var result = ProfileLogic.Edit(GetOperation().UserId, data);

            return Json(result);
        }

        [HttpPost]
        public ActionResult ExperienceEditSubmit(ExperienceInfo data)
        {
            var result = ProfileLogic.EditExperience(GetOperation().UserId, data);

            return Json(result);
        }

        [HttpPost]
        public ActionResult WorksEditSubmit(WorksInfo data)
        {
            var result = ProfileLogic.EditWorks(GetOperation().UserId, data);

            return Json(result);
        }

        [HttpPost]
        public ActionResult SchoolEditSubmit(SchoolInfo data)
        {
            var result = ProfileLogic.EditSchool(GetOperation().UserId, data);

            return Json(result);
        }

        [HttpPost]
        public ActionResult SkillEditSubmit(string display)
        {
            var result = ProfileLogic.EditSkill(GetOperation().UserId, display);

            return Json(result);
        }

        [HttpPost]
        public ActionResult LanguageEditSubmit(string display)
        {
            var result = ProfileLogic.EditLanguage(GetOperation().UserId, display);

            return Json(result);
        }

        [HttpPost]
        public ActionResult LicenseEditSubmit(string display)
        {
            var result = ProfileLogic.EditLicense(GetOperation().UserId, display);

            return Json(result);
        }

        [HttpPost]
        public ActionResult DeleteExperience(string name)
        {
            var result = ProfileLogic.DeleteExperience(GetOperation().UserId, name);

            return Json(result);
        }

        [HttpPost]
        public ActionResult DeleteWorks(string name)
        {
            var result = ProfileLogic.DeleteWorks(GetOperation().UserId, name);

            return Json(result);
        }

        [HttpPost]
        public ActionResult DeleteSchool(string name)
        {
            var result = ProfileLogic.DeleteSchool(GetOperation().UserId, name);

            return Json(result);
        }

        [HttpPost]
        public ActionResult DeleteSkill(int skillId)
        {
            var result = ProfileLogic.DeleteSkill(GetOperation().UserId, skillId);

            return Json(result);
        }

        [HttpPost]
        public ActionResult DeleteLanguage(int languageId)
        {
            var result = ProfileLogic.DeleteLanguage(GetOperation().UserId, languageId);

            return Json(result);
        }

        [HttpPost]
        public ActionResult DeleteLicense(int licenseId)
        {
            var result = ProfileLogic.DeleteLicense(GetOperation().UserId, licenseId);

            return Json(result);
        }

        /// <summary>
        /// login view
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            // 如果已經登入了，就直接幫進入系統，不用看到登入畫面
            if (Request.IsAuthenticated)
                return redirect(returnUrl);

            return View();
        }

        /// <summary>
        /// login
        /// </summary>
        /// <param name="email">Email</param>
        /// <param name="password">密碼</param>
        /// <param name="returnUrl">重導路徑</param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(string email, string password, string returnUrl)
        {
            #region 驗證帳密
            var isValid = UserLogic.IsValid(email, password);
            if (isValid.IsSuccess == false)
            {
                ModelState.AddModelError("", "email or password error");
                return View();
            }
            #endregion //驗證帳密

            // 取得資訊
            var id = isValid.ReturnObject;
            var user = UserLogic.GetLoginInfo(id);

            #region 登入系統
            var userData = ParseToUserDataString(user);

            var ticket = new FormsAuthenticationTicket(
                1,                      // ticket version
                user.Display,           // authenticated username
                DateTime.UtcNow,        // issueDate
                DateTime.MaxValue,      // expiryDate
                false,                  // true to persist across browser sessions
                userData                // can be used to store additional user data
            );
            var encryptedTicket = FormsAuthentication.Encrypt(ticket);

            var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
            authCookie.HttpOnly = true;
            if (ticket.IsPersistent)
                authCookie.Expires = ticket.Expiration;

            Response.Cookies.Add(authCookie);
            #endregion //登入系統

            return redirect(returnUrl);
        }

        /// <summary>
        /// 登出
        /// </summary>
        /// <returns></returns>
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return Redirect("/User/Login");
        }

        #region Private Methods
        /// <summary>
        /// 重新導向
        /// </summary>
        /// <param name="returnUrl">重導路徑</param>
        /// <returns></returns>
        ActionResult redirect(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            else
                return Redirect("/");
        }
        #endregion //Private Methods

        [HttpGet]
        public ActionResult ChangePassword() => View();

        [HttpPost]
        public ActionResult ChangePasswordSubmit(string oldPassword, string newPassword)
        {
            var result = UserLogic.ChangePassword(LoginInfo.Id, oldPassword, newPassword);

            if (result.IsSuccess == false)
                return JsonError(result.ErrorCode);

            return null;
        }

        [HttpPost]
        public ActionResult Photo()
        {
            bool isSavedSuccessfully = true;
            string fName = "";
            try
            {
                foreach (string fileName in Request.Files)
                {
                    HttpPostedFileBase file = Request.Files[fileName];
                    //Save file content goes here
                    fName = file.FileName;
                    if (file != null && file.ContentLength > 0)
                    {

                        var originalDirectory = new DirectoryInfo(string.Format("{0}MediaUpload\\", Server.MapPath(@"\")));

                        string pathString = Path.Combine(originalDirectory.ToString(), "UserPhoto");

                        bool isExists = Directory.Exists(pathString);

                        if (!isExists)
                            Directory.CreateDirectory(pathString);

                        var ext = Path.GetExtension(file.FileName);
                        var fileName1 = GetOperation().UserId.ToString() + ext;
                        var path = string.Format("{0}\\{1}", pathString, fileName1);
                        file.SaveAs(path);

                        var result = ProfileLogic.UpdatePhoto(GetOperation().UserId, fileName1);
                        if (result.IsSuccess == false)
                            isSavedSuccessfully = false;
                    }
                }
            }
            catch (Exception ex)
            {
                isSavedSuccessfully = false;
            }

            if (isSavedSuccessfully)
            {
                return Json(new { Message = fName });
            }
            else
            {
                return Json(new { Message = "Error in saving file" });
            }
        }
    }
}