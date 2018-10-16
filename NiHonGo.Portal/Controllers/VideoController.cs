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
    public class CompanyController : _BaseController
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

        CompanyLogic CompanyLogic
        {
            get
            {
                if (_companyLogic == null)
                    _companyLogic = new CompanyLogic();
                return _companyLogic;
            }
        }
        CompanyLogic _companyLogic;

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

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Index() => View();

        [HttpGet]
        public ActionResult Master()
        {
            if (LoginInfo.UserType == UserType.User)
                return Redirect("/Error/PageNotFind");

            return View();
        }

        [HttpPost]
        public ActionResult MasterInit()
        {
            var result = UserLogic.GetMasterCompanyId(GetOperation().UserId);
            if (result.IsSuccess == false)
                return Json(result);
            else
            {
                var result2 = CompanyLogic.GetDetail(result.ReturnObject);
                return Json(result2);
            }
        }

        [HttpGet]
        public ActionResult Edit()
        {
            if (LoginInfo.UserType == UserType.User)
                return Redirect("/Error/PageNotFind");

            return View();
        }

        [HttpPost]
        public ActionResult EditInit()
        {
            var result = UserLogic.GetMasterCompanyId(GetOperation().UserId);
            if (result.IsSuccess == false)
                return Json(result);
            else
            {
                var result2 = CompanyLogic.GetDetail(result.ReturnObject);
                return Json(result2);
            }
        }

        [HttpPost]
        public ActionResult EditSubmit(CompanyInfo data)
        {
            if (!string.IsNullOrWhiteSpace(data.Photo))
            {
                var originalDirectory = new DirectoryInfo(string.Format("{0}MediaUpload\\", Server.MapPath(@"\")));
                var pathString = Path.Combine(originalDirectory.ToString(), "CompanyPhoto");
                var ext = Path.GetExtension(data.Photo);
                var photoName = "temp" + GetOperation().UserId + ext;
                data.Photo = string.Format("{0}\\{1}", pathString, photoName);
            }
            var result = CompanyLogic.Edit(GetOperation().UserId, data);

            return Json(result);
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
                var companyId = int.Parse(id);
                var result = CompanyLogic.GetDetail(companyId);
                return Json(result);
            }
            catch
            {
                var result = new IsSuccessResult(ErrorCode.CompanyNotFound.ToString());
                return Json(result);
            }
        }

        [HttpPost]
        public ActionResult GetMasterJobList(int index, int count)
        {
            var result = UserLogic.GetMasterCompanyId(GetOperation().UserId);
            if (result.IsSuccess == false)
                return Json(new JobListReturn());
            else
            {
                var filter = new SearchJob();
                filter.CompanyId = result.ReturnObject;
                filter.IsOnlyRecruiting = false;
                var result2 = JobLogic.GetList(filter, index, count);
                return Json(result2);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult GetJobList(int id, int index, int count)
        {
            var filter = new SearchJob();
            filter.CompanyId = id;
            var result2 = JobLogic.GetList(filter, index, count);
            return Json(result2);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult GetCompanyList(string keyword, int index, int count)
        {
            var result = CompanyLogic.GetList(keyword, index, count);

            return Json(result);
        }

        [HttpPost]
        public ActionResult Photo()
        {
            var isSavedSuccessfully = true;
            var fName = "";
            var defaultPhotoName = "temp" + GetOperation().UserId;
            var photoName = defaultPhotoName;
            var companyIdResult = UserLogic.GetMasterCompanyId(GetOperation().UserId);
            if (companyIdResult.IsSuccess)
                photoName = companyIdResult.ReturnObject.ToString();

            foreach (string fileName in Request.Files)
            {
                var file = Request.Files[fileName];
                fName = file.FileName;
                if (file != null && file.ContentLength > 0)
                {
                    var originalDirectory = new DirectoryInfo(string.Format("{0}MediaUpload\\", Server.MapPath(@"\")));
                    var pathString = Path.Combine(originalDirectory.ToString(), "CompanyPhoto");
                    var isExists = Directory.Exists(pathString);

                    if (!isExists)
                        Directory.CreateDirectory(pathString);

                    var ext = Path.GetExtension(file.FileName);
                    var fileName1 = photoName + ext;
                    var path = string.Format("{0}\\{1}", pathString, fileName1);
                    file.SaveAs(path);

                    if (photoName.Contains(defaultPhotoName) == false && companyIdResult.IsSuccess)
                    {
                        var result = CompanyLogic.UpdatePhoto(companyIdResult.ReturnObject, fileName1);
                        if (result.IsSuccess == false)
                            isSavedSuccessfully = false;
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
    }
}