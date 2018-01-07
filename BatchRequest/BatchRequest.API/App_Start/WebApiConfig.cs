using System.Web.Http;

namespace BatchRequest.API
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.LocalOnly;
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "{controller}/{id}",
                defaults: new
                {
                    controller = "default",
                    id = RouteParameter.Optional
                }
            );
           
        }
    }
}
