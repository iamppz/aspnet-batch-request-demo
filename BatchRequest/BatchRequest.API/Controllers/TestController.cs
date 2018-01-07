using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace BatchRequest.API.Controllers
{
    public class TestController : ApiController
    {
        [HttpGet]
        public HttpResponseMessage Index()
        {
            return Request.CreateResponse(HttpStatusCode.OK, "OK");
        }
    }
}