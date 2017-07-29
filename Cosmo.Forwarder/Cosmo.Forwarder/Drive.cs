using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Cosmo.Forwarder
{
    public class Drive
    {
        public string Name { get; set; }
        public string Label { get; set; }
        public string Format { get; set; }
        public DriveType Type { get; set; }
        public double AvailableSpace { get; set; }
        public double FreeSpace { get; set; }
        public double TotalSpace { get; set; }
        public double UsedSpace { get; set; }
    }

    public class PerformanceInfo
    {

        public double Cpu { get; set; }
        public double TotalMemory { get; set; }
        public double AvailableMemory { get; set; }
        public List<Drive> Drives { get; set; }
    }

    public class PerformanceResponse
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public PerformanceInfo Performance { get; set; }
    }
}
