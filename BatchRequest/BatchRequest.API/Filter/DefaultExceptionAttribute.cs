using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;

namespace BatchRequest.API.Filter
{
    /// <summary>
    /// 组件异常处理
    /// </summary>
    public class DefaultExceptionAttribute : ExceptionFilterAttribute
    {
        /// <summary>
        /// Raises the exception event.
        /// </summary>
        /// <param name="actionExecutedContext">The context for the action.</param>
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            actionExecutedContext.Response =
                actionExecutedContext.Request.CreateResponse(HttpStatusCode.InternalServerError,
                new { Status = "server-error", Message = "服务器异常" });
        }
    }
}