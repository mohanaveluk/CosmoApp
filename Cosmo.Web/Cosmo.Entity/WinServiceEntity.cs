using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Cosmo.Entity
{
    public class WinServiceEntity:TRW.NamedBusinessCode, IFill
    {

        #region properties
        public int ID { get; set; }
        public int EnvID { get; set; }
        public string EnvName { get; set; }
        public int ConfigID { get; set; }
        public string HostIP { get; set; }
        public string Port { get; set; }
        public string Location { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string ServiceName { get; set; }
        public string Comments { get; set; }
        public string SchedulerSummary { get; set; }
        public string ServiceType { get; set; }
        public string MonitorStatus { get; set; }
        public string MonitorComments { get; set; }

        #endregion properties

        #region IFill Environment details
        /// <summary>
        /// Fill Data for Environment details 
        /// </summary>
        /// <param name="reader"></param>
        public void Fill(System.Data.SqlClient.SqlDataReader reader)
        {
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "ENV_ID"))
                this.EnvID = Convert.ToInt32(reader["ENV_ID"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "ENV_NAME"))
                this.EnvName = Convert.ToString(reader["ENV_NAME"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_ID"))
                this.ConfigID = Convert.ToInt32(reader["CONFIG_ID"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_HOST_IP"))
                this.HostIP = Convert.ToString(reader["CONFIG_HOST_IP"]).ToLower(CultureInfo.CurrentCulture);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_PORT_NUMBER"))
                this.Port = Convert.ToString(reader["CONFIG_PORT_NUMBER"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_LOCATION"))
                this.Location = Convert.ToString(reader["CONFIG_LOCATION"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "WIN_SERVICE_ID"))
                this.ID = Convert.ToInt32(reader["WIN_SERVICE_ID"]);
            
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "WIN_SERVICENAME"))
                this.ServiceName = Convert.ToString(reader["WIN_SERVICENAME"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "WIN_COMMENTS"))
                this.Comments = Convert.ToString(reader["WIN_COMMENTS"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_CREATED_BY"))
                this.CreatedBy = Convert.ToString(reader["CONFIG_CREATED_BY"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_CREATED_DATE"))
                this.CreatedDate = Convert.ToDateTime(reader["CONFIG_CREATED_DATE"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_UPDATED_BY"))
                this.UpdatedBy = Convert.ToString(reader["CONFIG_UPDATED_BY"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_UPDATED_DATE"))
                this.UpdatedDate = Convert.ToDateTime(reader["CONFIG_UPDATED_DATE"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_SERVICE_TYPE"))
                this.ServiceType = Convert.ToString(reader["CONFIG_SERVICE_TYPE"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "MONITOR_STATUS"))
                this.MonitorStatus = Convert.ToString(reader["MONITOR_STATUS"]);
            else
                this.MonitorStatus = string.Empty;
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "MONITOR_COMMENTS"))
                this.MonitorComments = Convert.ToString(reader["MONITOR_COMMENTS"]);
            else
                this.MonitorComments = string.Empty;
        }
        #endregion IFill Environment details
    }
}
