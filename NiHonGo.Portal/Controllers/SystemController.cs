using NiHonGo.Core.DTO;
using NiHonGo.Core.Logic;
using System.Web;
using System.Web.Mvc;

namespace NiHonGo.Portal.Controllers
{
    public class SystemController : _BaseController
    {
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

        [HttpPost]
        public ActionResult LevelList()
        {
            var result = SystemLogic.GetLevels();
            return Json(result);
        }

        [HttpPost]
        public ActionResult ChangeLanguage(string language)
        {
            try
            {
                var languageCookie = new HttpCookie("language", language);
                Response.Cookies.Add(languageCookie);
                return Json(new IsSuccessResult());
            }
            catch { return Json(new IsSuccessResult("failed")); }

        }
    }
}