using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cosmo.WebServices.Models
{
    public class AuthenticationResponse
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public string IsAuthenticated { get; set; }
    }
}