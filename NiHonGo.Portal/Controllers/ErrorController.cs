using System.Web.Mvc;

namespace NiHonGo.Portal.Controllers
{
    public class ErrorController : _BaseController
    {
        // GET: Error
        public ActionResult PageNotFind() => View();

        public ActionResult InternalError() => View();
    }
}