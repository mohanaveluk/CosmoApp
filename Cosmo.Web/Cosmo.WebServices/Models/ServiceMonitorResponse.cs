using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Cosmo.Entity;

namespace Cosmo.WebServices.Models
{
    public class ServiceMonitorResponse
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public CosmoService CosmoService { get; set; }
        public IEnumerable<ServiceMoniterEntity> ServiceMoniters { get; set; }
    }
}