using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Cog.WS.Service;

namespace Cog.WS.Service
{
    public interface IWebHttpWrapperException
    {
        Exception InnerException { get; }
        IWebHttpResponseWrapper Response { get; }
        WebExceptionStatus Status { get; }
        string ExceptionMessage { get; }
    }


    public class WebHttpWrapperException : Exception
    {
        private readonly IWebHttpWrapperException _webException;

        public IWebHttpResponseWrapper Response
        {
            get { return _webException.Response; }
        }

        public WebExceptionStatus Status
        {
            get { return _webException.Status; }
        }

        public override string Message
        {
            get { return _webException.ExceptionMessage; }
        }

        public WebHttpWrapperException(IWebHttpWrapperException webException)
            : base(webException.ExceptionMessage, webException.InnerException)
        {
            _webException = webException;
        }
    }
}
