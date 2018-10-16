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
    public class BlogController : _BaseController
    {
        BlogLogic BlogLogic
        {
            get
            {
                if (_blogLogic == null)
                    _blogLogic = new BlogLogic();
                return _blogLogic;
            }
        }
        BlogLogic _blogLogic;

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Index() => View();

        [HttpGet]
        public ActionResult Edit()
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
                var blogId = int.Parse(id);
                var result = BlogLogic.GetDetail(blogId);
                return Json(result);
            }
            catch
            {
                var result = new IsSuccessResult(ErrorCode.BlogNotFound.ToString());
                return Json(result);
            }
        }

        [HttpPost]
        public ActionResult EditSubmit(EditBlog data, int? blogId)
        {
            if (!string.IsNullOrWhiteSpace(data.Photo))
            {
                var originalDirectory = new DirectoryInfo(string.Format("{0}MediaUpload\\", Server.MapPath(@"\")));
                var pathString = Path.Combine(originalDirectory.ToString(), "BlogPhoto");
                var ext = Path.GetExtension(data.Photo);
                var photoName = "temp" + GetOperation().UserId + ext;
                data.Photo = string.Format("{0}\\{1}", pathString, photoName);
            }
            var result = BlogLogic.Edit(data, GetOperation().UserId, blogId);

            return Json(result);
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Detail(string id) => View();

        [HttpPost]
        [AllowAnonymous]
        public ActionResult DetailInit(string id)
        {
            try
            {
                var jobId = int.Parse(id);
                var result = BlogLogic.GetDetail(jobId);
                return Json(result);
            }
            catch
            {
                var result = new IsSuccessResult(ErrorCode.BlogNotFound.ToString());
                return Json(result);
            }
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
                var result = BlogLogic.GetDetail(jobId);
                return Json(result);
            }
            catch
            {
                var result = new IsSuccessResult(ErrorCode.BlogNotFound.ToString());
                return Json(result);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult GetList(string keyword, int index, int count, int? catalogId)
        {
            var result = BlogLogic.GetList(keyword, index, count, catalogId);

            return Json(result);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Photo(string id)
        {
            var isSavedSuccessfully = true;
            var fName = "";
            var defaultPhotoName = "temp" + GetOperation().UserId;
            var photoName = defaultPhotoName;
            try
            {
                photoName = int.Parse(id).ToString();
            }
            catch (Exception)
            {
            }

            if (LoginInfo.UserType == UserType.User)
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
                        var pathString = Path.Combine(originalDirectory.ToString(), "BlogPhoto");
                        var isExists = Directory.Exists(pathString);

                        if (!isExists)
                            Directory.CreateDirectory(pathString);

                        var ext = Path.GetExtension(file.FileName);
                        var fileName1 = photoName + ext;
                        var path = string.Format("{0}\\{1}", pathString, fileName1);
                        file.SaveAs(path);

                        if (photoName.Contains(defaultPhotoName) == false)
                        {
                            var result = BlogLogic.UpdatePhoto(int.Parse(id), fileName1);
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

        [HttpPost]
        [AllowAnonymous]
        public ActionResult GetCatalogList()
        {
            var result = BlogLogic.GetCataloges();
            return Json(result);
        }
    }
}