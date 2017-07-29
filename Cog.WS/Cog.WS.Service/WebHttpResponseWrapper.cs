using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace Cog.WS.Service
{
    public interface IWebHttpResponseWrapper : IDisposable
    {
        Stream GetResponseStream();
        HttpStatusCode StatusCode { get; }
        string StatusDescription { get; }
        CookieCollection Cookies { get; }
    }

    public class WebHttpResponseWrapper : IWebHttpResponseWrapper
    {private readonly HttpWebResponse _response;

    public WebHttpResponseWrapper(HttpWebResponse response)
        {
            _response = response;
        }

        public CookieCollection Cookies
        {
            get
            {
                return _response.Cookies;
            }
        }

        public Stream GetResponseStream()
        {
            return _response.GetResponseStream();
        }

        public HttpStatusCode StatusCode
        {
            get { return _response.StatusCode; }
        }

        public string StatusDescription
        {
            get { return _response.StatusDescription; }
        }

        public void Dispose()
        {
            ((IDisposable)_response).Dispose();//private api's make ryan sad
        }
    }
}
