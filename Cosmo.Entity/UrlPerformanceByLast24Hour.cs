using System;
using System.Data.SqlClient;

namespace Cosmo.Entity
{
    public class UrlPerformanceByLast24Hour : TRW.NamedBusinessCode, IFill
    {
        public double Total { get; set; }
        public double Average { get; set; }
        public int Hour { get; set; }
        public int Date { get; set; }

        public void Fill(SqlDataReader reader)
        {
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "TOTALRT"))
                this.Total = Convert.ToDouble(reader["TOTALRT"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "AVGRT"))
                this.Average = Convert.ToDouble(reader["AVGRT"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "HOURRT"))
                this.Hour = Convert.ToInt32(reader["HOURRT"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "DATERT"))
                this.Date = Convert.ToInt32(reader["DATERT"]);
            

        }
    }
}