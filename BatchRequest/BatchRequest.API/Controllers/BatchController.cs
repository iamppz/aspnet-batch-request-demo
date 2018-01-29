using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using Newtonsoft.Json;
using System.Web;
using System;

namespace BatchRequest.API.Controllers
{
    public class BatchController : ApiController
    {
        private static readonly IEnumerable<WebAPIDescriptor> descriptors = new List<WebAPIDescriptor>();
        private static readonly string boundaryPrefix = "--";
        private static readonly string apiPrefix = "/api";
        private static readonly string ns = "BatchRequest.API.Controllers";

        static BatchController()
        {
            var controllers = GlobalConfiguration.Configuration.Services
                .GetHttpControllerSelector().GetControllerMapping();
            foreach (var ctl in controllers)
            {
                var actions = GlobalConfiguration.Configuration.Services.GetActionSelector()
                    .GetActionMapping(ctl.Value);
                descriptors = actions.Aggregate(descriptors, 
                    (current, action) => current.Union(Actions(ctl.Key, action)));
            }
        }

        [HttpPost]
        public HttpResponseMessage Dispatch()
        {
            var request = HttpContext.Current.Request;
            string boundary;
            try
            {
                boundary = request.ContentType.Replace("multipart/related; boundary=", string.Empty);
            }
            catch (Exception)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "INVALID_BOUNDARY");
            }
            if (string.IsNullOrWhiteSpace(boundary))
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "EMPTY_BOUNDARY");
            }
            var content = Request.Content.ReadAsStringAsync().Result;
            var contentParts = content.Split(new [] { boundaryPrefix + boundary }, StringSplitOptions.RemoveEmptyEntries);
            var results = contentParts.Select(part => Execute(part));
            return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(results));
        }

        [HttpGet]
        public HttpResponseMessage PrintAPIs()
        {
            return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(descriptors));
        }

        private object Execute(string requestContentPart)
        {
            var lines = requestContentPart.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            string httpMethod;
            string url;
            try
            {
                string requestInfo = lines[1];
                var fields = requestInfo.Split(' ');
                httpMethod = fields[0];
                url = fields[1];
            }
            catch (Exception)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "INVALID_CONTENT_PART");
            }
            var infos = descriptors.Where(a =>
            string.Equals(a.Url, url, StringComparison.InvariantCultureIgnoreCase)
            && a.Method == httpMethod);
            if (!infos.Any())
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "NOT_FOUND");
            }
            if (infos.Count() > 0)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "AMBIGUOUS");
            }
            var reflectInfo = url.Replace(apiPrefix, string.Empty).Split(new[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
            var type = Type.GetType(ns + "." + reflectInfo[0] + "Controller");
            if (type == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "CLASS_NOT_FOUND");
            }
            var instance = Activator.CreateInstance(type);
            var method = type.GetMethod(reflectInfo[1]);
            if (method == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "METHOD_NOT_FOUND");
            }
            return Request.CreateResponse(HttpStatusCode.BadRequest, method.Invoke(instance, null));
        }

        private static IEnumerable<WebAPIDescriptor> Actions(string controllerName, IEnumerable<HttpActionDescriptor> descriptors)
        {
            return descriptors.Select(desc => new WebAPIDescriptor
            {
                Method = desc.SupportedHttpMethods.First().Method,
                Url = apiPrefix + "/" + controllerName + "/" + desc.ActionName,
                Parameters = string.Join(",", desc.GetParameters().Select(p => p.ParameterName + ":" + p.ParameterType))
            });
        }
    }

    class WebAPIDescriptor
    {
        public string Method { get; set; }
        public string Url { get; set; }
        public string Parameters { get; set; }
    }
}