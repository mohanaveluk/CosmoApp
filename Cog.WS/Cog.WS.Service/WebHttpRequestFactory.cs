using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Cog.WS.Service;

namespace Cog.WS.Service
{
    public interface IWebHttpRequestFactory
    {
        IWebHttpRequestWrapper Create(Uri uri);
    }


    public class WebHttpRequestFactory : IWebHttpRequestFactory
    {
        public IWebHttpRequestWrapper Create(Uri uri)
        {
            return new WebHttpRequestWrapper((HttpWebRequest)WebRequest.Create(uri));
        }
    }
}
