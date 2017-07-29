using System;

namespace Cosmo.Entity
{
    public class CognosCgiResponse
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public string ResponseTime { get; set; }
        public string Exception { get; set; }
        public string PortalUrl { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

    }
}