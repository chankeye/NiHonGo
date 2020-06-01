using System.Web.Mvc;

namespace NiHonGo.Portal.Controllers
{
    public class HomeController : _BaseController
    {
        public ActionResult Index() {
            return Redirect("/Video/Index");
        }
    }
}