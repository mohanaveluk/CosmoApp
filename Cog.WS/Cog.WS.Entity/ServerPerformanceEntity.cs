using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Cog.WS.Entity
{
    public class PerformanceInfo
    {
        public int EnvId { get; set; }
        public int ConfigId { get; set; }
        public string HostIp { get; set; }

        public double Cpu { get; set; }
        public double TotalMemory { get; set; }
        public double AvailableMemory { get; set; }

        public List<Drive> Drives { get; set; }
        public string CreatedBy { get; set; }
    }

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

    public class PerformanceResponse
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public PerformanceInfo Performance { get; set; }
    }
}
