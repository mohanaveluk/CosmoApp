using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmo.Forwarder
{
    public interface IServerRepository
    {
        PerformanceResponse GeterverPerformance(string srn);
    }

    public class ServerRepository: IServerRepository
    {
        public PerformanceResponse GeterverPerformance(string srn)
        {
            try
            {
                var performance = new PerformanceInfo {Drives = new List<Drive>()};

                var list = GetDriveInfo("snowflake");
                foreach (var driveInfo in list.Select(drive => new Drive
                {
                    Name = drive.Name,
                    Label = drive.Label,
                    Format = drive.Format,
                    Type = drive.Type,
                    AvailableSpace = drive.AvailableSpace,
                    FreeSpace = drive.FreeSpace,
                    UsedSpace = drive.UsedSpace,
                    TotalSpace = drive.TotalSpace
                }))
                {
                    performance.Drives.Add(driveInfo);
                }

                var cpuUsage = new ProcessorUsage();
                var cpuInfo = cpuUsage.GetSystemCurrentValue();
                if (cpuInfo.AvailableMemory >= 0) cpuInfo.AvailableMemory = cpuInfo.AvailableMemory / 1024;

                performance.Cpu = cpuInfo.Cpu;
                performance.AvailableMemory =  cpuInfo.AvailableMemory;
                performance.TotalMemory = Convert.ToDouble(FormatKiloBytesToBM(Convert.ToDecimal(cpuInfo.TotalMemory)));

                return new PerformanceResponse {Status = "Success", Message = string.Empty, Performance = performance};
            }
            catch (Exception exception)
            {
                Logger.Log(exception.ToString());
                return new PerformanceResponse {Status = "Failure", Message = exception.Message, Performance = null};
            }
            
        }

        static List<Drive> GetDriveInfo(string systenName)
        {
            var driveInfo = DriveInfo.GetDrives();

            return driveInfo.Select(info => new Drive
            {
                Name = info.Name,
                Label = info.VolumeLabel,
                Format = info.DriveFormat,
                Type = info.DriveType,
                TotalSpace = info.TotalSize,
                AvailableSpace = info.AvailableFreeSpace,
                FreeSpace = info.TotalFreeSpace,
                UsedSpace = info.TotalSize - info.AvailableFreeSpace
            }).ToList();
        }

        public static string FormatBytes(long bytes)
        {
            const int scale = 1024;
            string[] orders = new string[] { "GB", "MB", "KB", "Bytes" };
            long max = (long)Math.Pow(scale, orders.Length - 1);
            foreach (string order in orders)
            {
                if (bytes > max)
                {
                    return string.Format("{0:##.##} {1}", Decimal.Divide(bytes, max), order);
                }

                max /= scale;
            }
            return "0 Bytes";
        }

        public static string FormatKiloBytes(long bytes)
        {
            const int scale = 1024;
            string[] orders = new string[] { "GB", "MB", "KB" };
            long max = (long)Math.Pow(scale, orders.Length - 1);
            foreach (string order in orders)
            {
                if (bytes > max)
                {
                    return string.Format("{0:##.##} {1}", Decimal.Divide(bytes, max), order);
                }

                max /= scale;
            }
            return "0 Bytes";
        }

        public static decimal FormatKiloBytesToBM(decimal bytes)
        {
            const int scale = 1024;
            string[] orders = new string[] { "GB", "MB", "KB" };
            long max = (long)Math.Pow(scale, orders.Length - 1);
            foreach (string order in orders)
            {
                if (bytes > max)
                {
                    return Decimal.Divide(bytes, max);
                }

                max /= scale;
            }
            return 0;
        }
    }
}
