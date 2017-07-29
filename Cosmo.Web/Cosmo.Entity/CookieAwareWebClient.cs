using System;
using System.Net;

namespace Cosmo.Entity
{
    public class CookieAwareWebClient : WebClient
    {
        private readonly CookieContainer _cookie = new CookieContainer();

        protected override WebRequest GetWebRequest(Uri address)
        {
            var request = base.GetWebRequest(address);
            
            if (request is HttpWebRequest)
            {
                (request as HttpWebRequest).CookieContainer = _cookie;
            }
            return request;
        }
    }
}