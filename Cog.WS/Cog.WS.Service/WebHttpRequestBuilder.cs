using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using Cog.WS.Service;

namespace Cog.WS.Service
{
    public class WebHttpRequestBuilder
    {
        static int serviceTimeout = Convert.ToInt32(ConfigurationManager.AppSettings["WindowsServiceTimeout"]);

        public IWebHttpRequestWrapper CreateGetRequest(string inUri)
        {
            var uri = new Uri(inUri);
            var requestWrapper = new WebHttpRequestFactory();
            var request = requestWrapper.Create(uri);
            request.Method = "GET";
            request.Timeout = serviceTimeout*1000;
            request.ContentType = "application/json;charset=utf-8";
            request.Accept = "application/json";

            return request;
        }

        public IWebHttpRequestWrapper CreatePostRequest<T>(string inUri, T criteria, string contract)
        {
            var uri = new Uri(inUri);
            var requestWrapper = new WebHttpRequestFactory();
            var request = requestWrapper.Create(uri);
            request.Method = "POST";
            request.Timeout = serviceTimeout * 1000;
            request.ContentType = "application/json;charset=utf-8";
            request.Accept = "application/json";

            var javaScriptSerializer = new JavaScriptSerializer();
            var membercsRequest = javaScriptSerializer.Serialize(criteria);

            var encoding = new System.Text.UTF8Encoding();
            byte[] bytes = encoding.GetBytes(membercsRequest);

            using (var requestStream = request.GetRequestStream())
            {
                requestStream.Write(bytes, 0, bytes.Length);
                requestStream.Close();
            }
            /*
            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                streamWriter.Write(membercsRequest);
                streamWriter.Flush();
                streamWriter.Close();
            }
            */
            return request;
        }
    }
}
