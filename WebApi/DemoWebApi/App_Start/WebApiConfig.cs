using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace DemoWebApi
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute("Search", "api/{controller}/{dateStart}/{dateEnd}/{location}/{persons:int}",
                    new
                    {
                        id = RouteParameter.Optional,
                        dateStart = RouteParameter.Optional,
                        dateEnd = RouteParameter.Optional,
                        location = RouteParameter.Optional,
                        persons = RouteParameter.Optional
                    });

        }
    }
}
