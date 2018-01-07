using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace BatchRequest.API.Filter
{
    /// <summary>
    /// 参数验证过滤器
    /// </summary>
    public class ParameterCheckAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (!actionContext.ModelState.IsValid)
            {
                var error = actionContext.ModelState.First(pair => pair.Value.Errors.Any());
                actionContext.Response = actionContext.ControllerContext.Request.CreateResponse(HttpStatusCode.BadRequest,
                        new {Message = error.Value.Errors.First().ErrorMessage, Status = "invalid-" + error.Key});
            }
        }
    }
}