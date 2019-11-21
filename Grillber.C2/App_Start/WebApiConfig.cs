using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Grillber.C2.App_Start;

namespace Grillber.C2
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            MappingRegistrations.BuildRegistrations();
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
