using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmo.Forwarder
{
    public interface IWindowService
    {
        string Status { get; set; }
        string ErrorMessage { get; set; }
        string ServiceName { get; set; }
        string ServiceStatus { get; set; }
        string SystemName { get; set; }
        string MonitorStatus { get; set; }
    }

    public class WindowService : IWindowService
    {
        public string Status { get; set; }
        public string ErrorMessage { get; set; }
        public string ServiceName { get; set; }
        public string ServiceStatus { get; set; }
        public string SystemName { get; set; }
        public string MonitorStatus { get; set; }
    }

    public enum ResponseStatus
    {
        Success = 0,
        Failure
    }
}
