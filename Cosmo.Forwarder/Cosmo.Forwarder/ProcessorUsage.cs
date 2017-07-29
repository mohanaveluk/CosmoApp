using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cosmo.Forwarder
{
    public class ProcessorUsage
    {
        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetPhysicallyInstalledSystemMemory(out long TotalMemoryInKilobytes);

        const float SampleFrequencyMillis = 1000;
        protected object SyncLock = new object();
        protected PerformanceCounter CpuUsage;
        protected PerformanceCounter RamUsage;
        protected PerformanceInfo SystemPerformanceInfoInfo;
        protected DateTime LastSampleTime;

        public ProcessorUsage()
        {
            CpuUsage = new PerformanceCounter("Processor", "% Processor Time", "_Total", true);
            RamUsage = new PerformanceCounter("Memory", "Available MBytes", true);
        }


        public PerformanceInfo GetSystemCurrentValue()
        {
            SystemPerformanceInfoInfo = new PerformanceInfo();
            if ((DateTime.UtcNow - LastSampleTime).TotalMilliseconds > SampleFrequencyMillis)
            {
                lock (SyncLock)
                {
                    if ((DateTime.UtcNow - LastSampleTime).TotalMilliseconds > SampleFrequencyMillis)
                    {
                        var cpu = CpuUsage.NextValue();
                        Thread.Sleep(250);
                        SystemPerformanceInfoInfo.Cpu = CpuUsage.NextValue();
                        SystemPerformanceInfoInfo.AvailableMemory = RamUsage.NextValue();

                        long memKb;
                        GetPhysicallyInstalledSystemMemory(out memKb);
                        SystemPerformanceInfoInfo.TotalMemory = memKb;
                        LastSampleTime = DateTime.UtcNow;
                    }
                }
            }

            return SystemPerformanceInfoInfo;
        }
    }
}
