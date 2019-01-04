using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Routing;

namespace DemoWebApi
{
    public static class WebApiConfig
    {
        public static void Register(RouteCollection routes)
        {
            // Web API configuration and services

            // Web API routes
            routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );


            routes.MapHttpRoute(
                name: "SearchAPI",
                routeTemplate: "api/{controller}/{action}/{dateStart}/{dateEnd}/{location}/{persons}");

            routes.MapHttpRoute(
                name: "ClientAPI",
                routeTemplate: "api/{controller}/{action}/{surname}/{name}/{password}");


        }
    }
}
