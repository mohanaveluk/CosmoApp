using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cosmo.Entity
{
    public class ServiceAvailableEntity
    {
        public int EnvID { get; set; }
        public int ConfigID { get; set; }
        public int SchID { get; set; }
        public int MonID { get; set; }
        public string Environment { get; set; }
        public string Host { get; set; }
        public string Port { get; set; }
        public string ServiceType { get; set; }
        public string Status { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
