using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Cosmo.Entity
{
    public class ServerCpuUsage:TRW.NamedBusinessCode, IFill
    {
        public double AverageCpuUsage { get; set; }
        public double AverageAvailableMemory { get; set; }
        public double AverageTotalMemory { get; set; }
        public int Hour { get; set; }
        public int Date { get; set; }

        public void Fill(SqlDataReader reader)
        {
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "AVGCPUUSAGE"))
                this.AverageCpuUsage = Convert.ToDouble(reader["AVGCPUUSAGE"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "AVGAVAILABLEMEMORY"))
                this.AverageAvailableMemory = Convert.ToDouble(reader["AVGAVAILABLEMEMORY"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "TOTALMEMORY"))
                this.AverageTotalMemory = Convert.ToDouble(reader["TOTALMEMORY"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "HOURRT"))
                this.Hour = Convert.ToInt32(reader["HOURRT"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "DATERT"))
                this.Date = Convert.ToInt32(reader["DATERT"]);
        
        }
    }
}
