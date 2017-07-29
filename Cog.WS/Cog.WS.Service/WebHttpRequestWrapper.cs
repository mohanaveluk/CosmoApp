using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;


namespace Cog.WS.Service
{
    public interface IWebHttpRequestWrapper
    {
        IWebHttpResponseWrapper GetResponse();
        string Uri { get; set; }
        string Method { set; }
        ICredentials Credentials { set; }
        string ContentType { set; }
        bool PreAuthenticate { set; }
        long ContentLength { set; }
        string UserAgent { set; }
        WebHeaderCollection Headers { get; }
        string Accept { set; }
        string Referer { set; }
        CookieContainer Cookies { set; }
        bool AllowAutoRedirect { set; }
        Stream GetRequestStream();
        int Timeout { get; set; }
    }


    public class WebHttpRequestWrapper : IWebHttpRequestWrapper
    {
        private readonly HttpWebRequest _request;

        public WebHttpRequestWrapper(HttpWebRequest request)
        {
            _request = request;
        }

        public string Uri
        {
            get { return _request.RequestUri.AbsoluteUri; }
            set { }
        }

        public string Method
        {
            set { _request.Method = value; }
        }

        public bool PreAuthenticate
        {
            set { _request.PreAuthenticate = value; }
        }

        public long ContentLength
        {
            set { _request.ContentLength = value; }
        }

        public string UserAgent
        {
            set { _request.UserAgent = value; }
        }

        public WebHeaderCollection Headers
        {
            get { return _request.Headers; }
        }

        public string Accept
        {
            set { _request.Accept = value; }
        }

        public string Referer
        {
            set { _request.Referer = value; }
        }

        public bool AllowAutoRedirect
        {
            set { _request.AllowAutoRedirect = value; }
        }

        public Stream GetRequestStream()
        {
            return _request.GetRequestStream();
        }

        public ICredentials Credentials
        {
            set { _request.Credentials = value; }
        }

        public string ContentType
        {
            set { _request.ContentType = value; }
        }

        public CookieContainer Cookies
        {
            set { _request.CookieContainer = value; }
        }

        public System.Security.Cryptography.X509Certificates.X509CertificateCollection ClientCertificates
        {
            set { _request.ClientCertificates = value; }
        }

        public IWebHttpResponseWrapper GetResponse()
        {
            try
            {
                return new WebHttpResponseWrapper((HttpWebResponse)_request.GetResponse());
            }
            catch (WebException webException)
            {
                throw new WebHttpWrapperException(new InternalWebExceptionWrapper(webException));
            }
        }

        public int Timeout
        {
            get { return _request.Timeout; }
            set { _request.Timeout = value; }
        }

        private class InternalWebExceptionWrapper : IWebHttpWrapperException
        {
            private readonly WebException _exception;
            private readonly IWebHttpResponseWrapper _response;

            public Exception InnerException
            {
                get { return _exception; }
            }

            public IWebHttpResponseWrapper Response
            {
                get { return _response; }
            }

            public WebExceptionStatus Status
            {
                get { return _exception.Status; }
            }

            public string ExceptionMessage
            {
                get { return _exception.Message; }
            }

            public InternalWebExceptionWrapper(WebException exception)
            {
                _exception = exception;
                _response = new WebHttpResponseWrapper((HttpWebResponse) exception.Response);
                
            }
        }
    }
}
