using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Routing;
using System.Web.Routing;

namespace DemoWebApi
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            WebApiConfig.Register(RouteTable.Routes);
            //GlobalConfiguration.Configuration.Routes.Add("default", new HttpRoute("{controller}"));
            //GlobalConfiguration.Configuration.Routes.Add("Search", new HttpRoute("{controller}/dateStart/dateEnd/location/{persons:int}"));
        }
    }
}
