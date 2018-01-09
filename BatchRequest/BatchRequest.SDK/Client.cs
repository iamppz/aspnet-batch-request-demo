using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;

namespace BatchRequest.SDK
{
    public class Client
    {
        public void Request()
        {
            var boundary = Guid.NewGuid().ToString();
            var uri = new Uri("http://localhost:53517/api/batch/dispatch");
            var content = new StringBuilder();
            content.Append("--").Append(boundary).Append(Environment.NewLine);
            content.Append("Content-Type: application/http; msgtype=request").Append(Environment.NewLine);
            content.Append("\n");
            content.Append("GET /").Append(uri.ToString()).Append(" ").Append("HTTP/1.1").Append(Environment.NewLine);
            content.Append(Environment.NewLine);
            content.Append(Environment.NewLine);

            var client = new HttpClient();
            var stringContent = new StringContent(content.ToString());
            var result = client.PostAsync(uri, stringContent).Result;
        }
    }
}
