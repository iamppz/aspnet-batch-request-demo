using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using Newtonsoft.Json;

namespace BatchRequest.API.Controllers
{
    public class BatchController : ApiController
    {
        [HttpGet]
        public HttpResponseMessage Dispatch()
        {
            var controllers = GlobalConfiguration.Configuration.Services.GetHttpControllerSelector()
                .GetControllerMapping();
            IEnumerable<string> result = new List<string>();
            foreach (var controller in controllers)
            {
                var actions = GlobalConfiguration.Configuration.Services.GetActionSelector()
                    .GetActionMapping(controller.Value);
                result = actions.Aggregate(result, (current, action) => current.Union(Actions(controller.Key, action)));
            }

            return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(result));
        }

        private static IEnumerable<string> Actions(string controllerName, IEnumerable<HttpActionDescriptor> action)
        {
            return action.Select(i => Method(i) + " /" + controllerName + "/" + i.ActionName + "?" + Parameters(i));
        }

        private static string Method(HttpActionDescriptor action)
        {
            return action.SupportedHttpMethods.First().Method;
        }

        private static string Parameters(HttpActionDescriptor action)
        {
            var result = action.GetParameters().Aggregate("",
                (last, p) =>
                    last + "," + p.ParameterName + ":" + p.ParameterType.Name);
            return result.Any() ? result.Remove(0, 1) : result;
        }
    }
}