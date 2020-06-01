using NiHonGo.Core.DTO;
using NiHonGo.Core.Enum;
using NiHonGo.Core.Logic;
using System.Web.Mvc;

namespace NiHonGo.Portal.Controllers
{
    [Authorize]
    public class VideoController : _BaseController
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

        VideoLogic VideoLogic
        {
            get
            {
                if (_videoLogic == null)
                    _videoLogic = new VideoLogic();
                return _videoLogic;
            }
        }
        VideoLogic _videoLogic;

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Index() => View();

        //[HttpGet]
        //public ActionResult Edit()
        //{
        //    if (LoginInfo.UserType == UserType.User)
        //        return Redirect("/Error/PageNotFind");

        //    return View();
        //}

        //[HttpPost]
        //public ActionResult EditInit()
        //{
        //    var result = UserLogic.GetMasterCompanyId(GetOperation().UserId);
        //    if (result.IsSuccess == false)
        //        return Json(result);
        //    else
        //    {
        //        var result2 = VideoLogic.GetDetail(result.ReturnObject);
        //        return Json(result2);
        //    }
        //}

        //[HttpPost]
        //public ActionResult EditSubmit(CompanyInfo data)
        //{
        //    if (!string.IsNullOrWhiteSpace(data.Photo))
        //    {
        //        var originalDirectory = new DirectoryInfo(string.Format("{0}MediaUpload\\", Server.MapPath(@"\")));
        //        var pathString = Path.Combine(originalDirectory.ToString(), "CompanyPhoto");
        //        var ext = Path.GetExtension(data.Photo);
        //        var photoName = "temp" + GetOperation().UserId + ext;
        //        data.Photo = string.Format("{0}\\{1}", pathString, photoName);
        //    }
        //    var result = VideoLogic.Edit(GetOperation().UserId, data);

        //    return Json(result);
        //}

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
                var result = VideoLogic.GetDetail(companyId);
                return Json(result);
            }
            catch
            {
                var result = new IsSuccessResult(ErrorCode.VideoNotFound.ToString());
                return Json(result);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult GetVideoList(string keyword, int index, int count)
        {
            var result = VideoLogic.GetList(keyword, index, count);

            return Json(result);
        }

        //[HttpPost]
        //public ActionResult Photo()
        //{
        //    var isSavedSuccessfully = true;
        //    var fName = "";
        //    var defaultPhotoName = "temp" + GetOperation().UserId;
        //    var photoName = defaultPhotoName;
        //    var companyIdResult = UserLogic.GetMasterCompanyId(GetOperation().UserId);
        //    if (companyIdResult.IsSuccess)
        //        photoName = companyIdResult.ReturnObject.ToString();

        //    foreach (string fileName in Request.Files)
        //    {
        //        var file = Request.Files[fileName];
        //        fName = file.FileName;
        //        if (file != null && file.ContentLength > 0)
        //        {
        //            var originalDirectory = new DirectoryInfo(string.Format("{0}MediaUpload\\", Server.MapPath(@"\")));
        //            var pathString = Path.Combine(originalDirectory.ToString(), "CompanyPhoto");
        //            var isExists = Directory.Exists(pathString);

        //            if (!isExists)
        //                Directory.CreateDirectory(pathString);

        //            var ext = Path.GetExtension(file.FileName);
        //            var fileName1 = photoName + ext;
        //            var path = string.Format("{0}\\{1}", pathString, fileName1);
        //            file.SaveAs(path);

        //            if (photoName.Contains(defaultPhotoName) == false && companyIdResult.IsSuccess)
        //            {
        //                var result = VideoLogic.UpdatePhoto(companyIdResult.ReturnObject, fileName1);
        //                if (result.IsSuccess == false)
        //                    isSavedSuccessfully = false;
        //            }
        //        }
        //    }

        //    if (isSavedSuccessfully)
        //    {
        //        return Json(new { Message = fName });
        //    }
        //    else
        //    {
        //        return Json(new { Message = "Error in saving file" });
        //    }
        //}
    }
}