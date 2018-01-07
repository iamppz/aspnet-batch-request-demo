using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace BatchRequest.API.Controllers
{
    public class BatchController : ApiController
    {
        [HttpGet]
        public HttpResponseMessage Dispatch()
        {
            var uri = HttpContext.Current.Request.Url;
            var services = GlobalConfiguration.Configuration.Services;
            var request = new HttpRequestMessage(HttpMethod.Get, "/test");
            var controllers = services.GetHttpControllerSelector().SelectController(request);
            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}