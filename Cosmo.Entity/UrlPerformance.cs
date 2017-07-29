using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.AccessControl;
using System.Text;

namespace Cosmo.Entity
{
    public class UrlPerformance :TRW.NamedBusinessCode, IFill
    {
        public int Id { get; set; }
        public int EnvId { get; set; }
        public string EnvName { get; set; }
        public string Type { get; set; }
        public string Adress { get; set; }
        public string DisplayName { get; set; }
        public double ResponseTimeLastPing { get; set; }
        public double ResponseTimeLastHour { get; set; }
        public DateTime LastPingDateTime { get; set; }
        public string ChartDataSource { get; set; }

        public List<UrlPerformanceByLast24Hour> ResponseTimeLast24Hour { get; set; }

        public void Fill(SqlDataReader reader)
        {
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "URL_ID"))
                this.Id = Convert.ToInt32(reader["URL_ID"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "ENV_ID"))
                this.EnvId = Convert.ToInt32(reader["ENV_ID"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "ENV_NAME"))
                EnvName = Convert.ToString(reader["ENV_NAME"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "URL_TYPE"))
                this.Type = Convert.ToString(reader["URL_TYPE"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "URL_ADDRESS"))
                this.Adress = Convert.ToString(reader["URL_ADDRESS"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "URL_DISPLAYNAME"))
                DisplayName = Convert.ToString(reader["URL_DISPLAYNAME"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "RESPONSETIME"))
                this.ResponseTimeLastPing = Convert.ToDouble(reader["RESPONSETIME"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "RESPONSETIMEINHOUR"))
                this.ResponseTimeLastHour = Convert.ToDouble(reader["RESPONSETIMEINHOUR"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "LASTPINGDATETIME"))
                this.LastPingDateTime = Convert.ToDateTime(reader["LASTPINGDATETIME"]);

            ResponseTimeLast24Hour = new List<UrlPerformanceByLast24Hour>();
        }
    }
}
