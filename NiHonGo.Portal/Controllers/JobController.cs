using NiHonGo.Core.DTO;
using NiHonGo.Core.DTO.Company;
using NiHonGo.Core.Enum;
using NiHonGo.Core.Logic;
using System;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace NiHonGo.Portal.Controllers
{
    [Authorize]
    public class JobController : _BaseController
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

        JobLogic JobLogic
        {
            get
            {
                if (_jobLogic == null)
                    _jobLogic = new JobLogic();
                return _jobLogic;
            }
        }
        JobLogic _jobLogic;

        SystemLogic SystemLogic
        {
            get
            {
                if (_systemLogic == null)
                    _systemLogic = new SystemLogic();
                return _systemLogic;
            }
        }
        SystemLogic _systemLogic;

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Index() => View();

        [HttpPost]
        [AllowAnonymous]
        public ActionResult GetList(SearchJob filter, int index, int count)
        {
            filter.IsOnlyRecruiting = true;
            var result = JobLogic.GetList(filter, index, count);

            return Json(result);
        }

        [HttpGet]
        public ActionResult Edit(string id)
        {
            if (LoginInfo.UserType == UserType.User)
                return Redirect("/Error/PageNotFind");

            return View();
        }

        [HttpPost]
        public ActionResult EditInit(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return Json(null);

            try
            {
                var jobId = int.Parse(id);
                var checkResult = UserLogic.IsJobMaster(GetOperation().UserId, jobId);
                if (checkResult.IsSuccess == false)
                    return Json(checkResult);

                var result = JobLogic.GetDetail(jobId);
                return Json(result);
            }
            catch
            {
                var result = new IsSuccessResult(ErrorCode.JobNotFound.ToString());
                return Json(result);
            }
        }

        [HttpPost]
        public ActionResult EditSubmit(EditJob data, int? jobId)
        {
            if (!string.IsNullOrWhiteSpace(data.Photo))
            {
                var originalDirectory = new DirectoryInfo(string.Format("{0}MediaUpload\\", Server.MapPath(@"\")));
                var pathString = Path.Combine(originalDirectory.ToString(), "JobPhoto");
                var ext = Path.GetExtension(data.Photo);
                var photoName = "temp" + GetOperation().UserId + ext;
                data.Photo = string.Format("{0}\\{1}", pathString, photoName);
            }
            var result = JobLogic.Edit(GetOperation().UserId, data, jobId);

            return Json(result);
        }

        [HttpGet]
        public ActionResult Master(string id)
        {
            if (LoginInfo.UserType == UserType.User)
                return Redirect("/Error/PageNotFind");

            return View();
        }

        [HttpPost]
        public ActionResult MasterInit(string id)
        {
            try
            {
                var jobId = int.Parse(id);
                var checkResult = UserLogic.IsJobMaster(GetOperation().UserId, jobId);
                if (checkResult.IsSuccess == false)
                    return Json(checkResult);

                var result = JobLogic.GetDetail(jobId);
                return Json(result);
            }
            catch
            {
                var result = new IsSuccessResult(ErrorCode.JobNotFound.ToString());
                return Json(result);
            }
        }

        [HttpGet]
        public ActionResult Detail(string id)
        {
            try
            {
                int.Parse(id);
                return View();
            }
            catch
            {
                return Redirect("/Error/PageNotFind");
            }
        }

        [HttpPost]
        public ActionResult DetailInit(string id)
        {
            try
            {
                var jobId = int.Parse(id);
                var result = JobLogic.GetDetail(jobId);
                return Json(result);
            }
            catch
            {
                var result = new IsSuccessResult(ErrorCode.JobNotFound.ToString());
                return Json(result);
            }
        }

        [HttpPost]
        public ActionResult Photo(string id)
        {
            var isSavedSuccessfully = true;
            var fName = "";
            var defaultPhotoName = "temp" + GetOperation().UserId;
            var photoName = defaultPhotoName;
            var checkResult = new IsSuccessResult();
            try
            {
                photoName = int.Parse(id).ToString();
                checkResult = UserLogic.IsJobMaster(GetOperation().UserId, int.Parse(id));
            }
            catch (Exception)
            {
            }

            if (checkResult.IsSuccess == false)
                isSavedSuccessfully = false;
            else
            {
                foreach (string fileName in Request.Files)
                {
                    var file = Request.Files[fileName];
                    fName = file.FileName;
                    if (file != null && file.ContentLength > 0)
                    {
                        var originalDirectory = new DirectoryInfo(string.Format("{0}MediaUpload\\", Server.MapPath(@"\")));
                        var pathString = Path.Combine(originalDirectory.ToString(), "JobPhoto");
                        var isExists = Directory.Exists(pathString);

                        if (!isExists)
                            Directory.CreateDirectory(pathString);

                        var ext = Path.GetExtension(file.FileName);
                        var fileName1 = photoName + ext;
                        var path = string.Format("{0}\\{1}", pathString, fileName1);
                        file.SaveAs(path);

                        if (photoName.Contains(defaultPhotoName) == false)
                        {
                            var result = JobLogic.UpdatePhoto(int.Parse(id), fileName1);
                            if (result.IsSuccess == false)
                                isSavedSuccessfully = false;
                        }
                    }
                }
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

        public ActionResult Apply(string id)
        {
            try
            {
                var jobId = int.Parse(id);
                var mailTO = JobLogic.GetCompanyEmail(jobId);
                var emailBody = "<h3>TRAILSから応募者がきました</h3>";
                var url = "http://" + HttpContext.Request.Url.Authority + "/User/Profile/" + LoginInfo.Id;
                emailBody += "<p>応募者の情報はこちら：<a href=\"" + url + "\">" + url + "</a></p>";
                emailBody += "<p>直接応募者に返事お願いします。</p>";
                var result = SystemLogic.SendMail(Settigns.SMTP, Settigns.MailForm, mailTO, "TRAILSから応募者がきました", emailBody);
                if (result)
                    return Json(new IsSuccessResult());
            }
            catch { }

            return Json(new IsSuccessResult("false"));
        }
    }
}