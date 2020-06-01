using NiHonGo.Core.Logic;
using System;
using System.Globalization;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

namespace NiHonGo.Portal
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            GlobalConfiguration.Configure(WebApiConfig.Register);
            new UserLogic().HasAnyUser();
        }

        protected void Application_BeginRequest(Object sender, EventArgs e)
        {
            var language = Request.Cookies["language"];

            if (language != null)
            {
                var ci = new CultureInfo(language.Value);
                Thread.CurrentThread.CurrentCulture = ci;
                Thread.CurrentThread.CurrentUICulture = ci;
            }
            else
            {
                language = new HttpCookie("language", "ja-JP");
                var ci = new CultureInfo(language.Value);
                Thread.CurrentThread.CurrentCulture = ci;
                Thread.CurrentThread.CurrentUICulture = ci;
                Response.Cookies.Add(language);
            }
        }
    }
}