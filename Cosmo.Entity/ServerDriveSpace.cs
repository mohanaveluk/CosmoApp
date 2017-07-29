using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;

namespace Cosmo.Entity
{
    public class ServerDriveSpace :TRW.NamedBusinessCode, IFill
    {
        public string Name { get; set; }
        public decimal AverageUsedSpace { get; set; }
        public decimal AverageFreeSpace { get; set; }
        public decimal AverageTotalSpace { get; set; }
        public double UsedSpaceInGb { get; set; }
        public double FreeSpaceInGb { get; set; }   
        public double TotalSpaceInGb { get; set; }   
        public int Hour { get; set; }
        public int Date { get; set; }
        public int Month { get; set; }
        public string DateMonth { get; set; }

        public void Fill(SqlDataReader reader)
        {
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "DRIVE_NAME"))
                Name = Convert.ToString(reader["DRIVE_NAME"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "AVGFREESPACE"))
                this.AverageFreeSpace = Convert.ToDecimal(reader["AVGFREESPACE"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "AVGUSEDSPACE"))
                AverageUsedSpace = Convert.ToDecimal(reader["AVGUSEDSPACE"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "AVGTOTALSPACE"))
                AverageTotalSpace = Convert.ToDecimal(reader["AVGTOTALSPACE"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "HOURRT"))
                this.Hour = Convert.ToInt32(reader["HOURRT"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "DATERT"))
                this.Date = Convert.ToInt32(reader["DATERT"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "MONTHRT"))
                Month = Convert.ToInt32(reader["MONTHRT"]);

        }
    }

    public class ServerDriveDetail
    {
        public string Name { get; set; }
        public List<double> FreeSpace { get; set; }
        public List<double> UsedSpace { get; set; }
        public List<double> TotalSpace { get; set; }

        public List<double> FreeSpaceInPercent { get; set; }
        public List<double> UsedSpaceInPercent { get; set; }

        public List<int> Date { get; set; }

    }

    public class Drive
    {
        public string Name { get; set; }
        public string Label { get; set; }
        public DriveType Type { get; set; }
        public string Format { get; set; }
        public double AvailableSpace { get; set; }
        public double FreeSpace { get; set; }
        public double TotalSpace { get; set; }
        public double UsedSpace { get; set; }

        public double UsedInPercent { get; set; }
        public double FreeInPercent { get; set; }

        public double AvailableSpaceInGb { get; set; }
        public double FreeSpaceInGb { get; set; }
        public double TotalSpaceInGb { get; set; }
        public double UsedSpaceInGb { get; set; }

        public string FreeSpaceInText { get; set; }
        public string TotalSpaceInText { get; set; }
        public string UsedSpaceInText { get; set; }
        public string UsedInPercentage { get; set; }

    }
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
    public class PerformanceResponse
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public PerformanceInfo Performance { get; set; }
    }
}