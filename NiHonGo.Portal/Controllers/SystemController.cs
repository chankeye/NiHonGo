using NiHonGo.Core.DTO;
using NiHonGo.Core.DTO.Company;
using NiHonGo.Core.Enum;
using NiHonGo.Core.Logic;
using System.Globalization;
using System.Threading;
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
        public ActionResult AreaList()
        {
            var result = SystemLogic.GetAreas();
            return Json(result);
        }

        [HttpPost]
        public ActionResult JobTypeList()
        {
            var result = SystemLogic.GetJobTypes();
            return Json(result);
        }

        [HttpPost]
        public ActionResult JobStatusList()
        {
            var result = SystemLogic.GetJobStatuses();
            return Json(result);
        }

        [HttpPost]
        public ActionResult VisaList()
        {
            var result = SystemLogic.GetVisas();
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