using System;
using System.Collections.Generic;
using System.EnterpriseServices;
using System.Linq;
using System.Web;

namespace Cosmo.WebServices.Models
{
    public class ResponseStatus
    {
        public enum Status
        {
            Success = 0,
            Failure
        }
    }
}