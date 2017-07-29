using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Globalization;

namespace Cosmo.Entity
{
    public class MonitorEntity:TRW.NamedBusinessCode, IFill
    {
        #region properties
        public int MonID { get; set; }
        public int EnvID { get; set; }
        public int ConfigID { get; set; }
        public int ScheduleID { get; set; }
        public string ConfigHostIP { get; set; }
        public string ConfigPort { get; set; }
        public string ConfigServiceDescription { get; set; }
        public string ConfigServiceType { get; set; }
        public string ConfigServiceURL { get; set; }
        public string ConfigMailFrequency { get; set; }
        public string ConfigLocation { get; set; }
        public bool ConfigIsMonitor { get; set; }
        public bool ConfigIsNotify { get; set; }
        public bool ConfigIsPrimary { get; set; }
        public string MonitorStatus { get; set; }
        public string MonitorStatusIcon { get; set; }
        public string MonitorNotifyIcon { get; set; }
        public string MonitorNotifyTooltip { get; set; }
        public DateTime? LastMoniterTime { get; set; }
        public string MonitorStartDateTime { get; set; }
        public string MonitorEndDateTime { get; set; }
        public DateTime MonitorCreatedDateTime { get; set; }
        public DateTime? MonitorUpdatedDateTime { get; set; }
        public string MonitorUpTime { get; set; }
        public string MonitorComments { get; set; }
        public bool IsActive { get; set; }
        public bool IsAcknowledge { get; set; }
        public string Incident_Issue { get; set; }
        public string Incident_Solution { get; set; }
        public DateTime ResolutionCreatedDateTime { get; set; }
        public string ResolutionCreatedBy { get; set; }
        public string WindowsServiceName { get; set; }
        public int WindowsServiceID { get; set; }
        public string WindowsServiceStatus { get; set; }
        public string BuildVersion { get; set; }
        public string EnvName { get; set; }
        public string StatusClass { get; set; }

        public WindowsServiceStrategy ServiceStrategy { get; set; }

        #endregion properties

        #region IFill Environment details
        /// <summary>
        /// Fill Data for Environment details 
        /// </summary>
        /// <param name="reader"></param>
        public  void Fill(SqlDataReader reader)
        {
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "MON_ID"))
                this.MonID = Convert.ToInt32(reader["MON_ID"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "ENV_ID"))
                this.EnvID = Convert.ToInt32(reader["ENV_ID"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_ID"))
                this.ConfigID = Convert.ToInt32(reader["CONFIG_ID"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "SCH_ID"))
                this.ScheduleID = Convert.ToInt32(reader["SCH_ID"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_HOST_IP"))
                this.ConfigHostIP = Convert.ToString(reader["CONFIG_HOST_IP"]).ToLower(CultureInfo.CurrentCulture);
            
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_PORT_NUMBER"))
                this.ConfigPort = Convert.ToString(reader["CONFIG_PORT_NUMBER"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_DESCRIPTION"))
                ConfigServiceDescription = Convert.ToString(reader["CONFIG_DESCRIPTION"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_SERVICE_TYPE"))
                this.ConfigServiceType = Convert.ToString(reader["CONFIG_SERVICE_TYPE"]);
            
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_URL_ADDRESS"))
                this.ConfigServiceURL = Convert.ToString(reader["CONFIG_URL_ADDRESS"]);
            
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_IS_MONITORED"))
                this.ConfigIsMonitor = Convert.ToBoolean(reader["CONFIG_IS_MONITORED"]);
            
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_ISNOTIFY"))
                this.ConfigIsNotify = Convert.ToBoolean(reader["CONFIG_ISNOTIFY"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_ISPRIMARY"))
                ConfigIsPrimary = Convert.ToBoolean(reader["CONFIG_ISPRIMARY"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_MAIL_FREQ"))
                this.ConfigMailFrequency = Convert.ToString(reader["CONFIG_MAIL_FREQ"]);
           
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_LOCATION"))
                this.ConfigLocation = Convert.ToString(reader["CONFIG_LOCATION"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "MON_STATUS"))
                this.MonitorStatus = Convert.ToString(reader["MON_STATUS"]);
            else
            {
                this.MonitorStatus = string.Empty;
            }

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "MON_UPDATED_DATE"))
                this.LastMoniterTime = Convert.ToDateTime(reader["MON_UPDATED_DATE"]);
            else
            {
                this.LastMoniterTime = null;
            }

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "MON_START_DATE_TIME"))
                this.MonitorStartDateTime = Convert.ToString(reader["MON_START_DATE_TIME"]);
            else
            {
                this.MonitorStartDateTime = string.Empty;
            }

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "MON_END_DATE_TIME"))
                this.MonitorEndDateTime = Convert.ToString(reader["MON_END_DATE_TIME"]);
            else
            {
                MonitorEndDateTime = string.Empty;
            }

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "MON_CREATED_DATE"))
                this.MonitorCreatedDateTime = Convert.ToDateTime(reader["MON_CREATED_DATE"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "MON_UPDATED_DATE"))
                this.MonitorUpdatedDateTime = Convert.ToDateTime(reader["MON_UPDATED_DATE"]);
            else
            {
                this.MonitorUpdatedDateTime = null;
            }

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "MON_UPTIME"))
                this.MonitorUpTime = Convert.ToString(reader["MON_UPTIME"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "MON_COMMENTS"))
                this.MonitorComments = Convert.ToString(reader["MON_COMMENTS"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "MON_IS_ACTIVE"))
                this.IsActive = Convert.ToBoolean(reader["MON_IS_ACTIVE"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "MON_ISACKNOWLEDGE"))
                this.IsAcknowledge = Convert.ToBoolean(reader["MON_ISACKNOWLEDGE"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "TRK_ISSUE"))
                this.Incident_Issue = Convert.ToString(reader["TRK_ISSUE"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "TRK_SOLUTION"))
                this.Incident_Solution = Convert.ToString(reader["TRK_SOLUTION"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "TRK_CREATED_DATE"))
                ResolutionCreatedDateTime = Convert.ToDateTime(reader["TRK_CREATED_DATE"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "TRK_CREATED_BY"))
                ResolutionCreatedBy = Convert.ToString(reader["TRK_CREATED_BY"]);
            
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "WIN_SERVICENAME"))
                this.WindowsServiceName = Convert.ToString(reader["WIN_SERVICENAME"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "WIN_SERVICE_ID"))
                this.WindowsServiceID = Convert.ToInt32(reader["WIN_SERVICE_ID"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "BUILD_VERSION"))
                this.BuildVersion = Convert.ToString(reader["BUILD_VERSION"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "ENV_NAME"))
                EnvName = Convert.ToString(reader["ENV_NAME"]);

            if (string.IsNullOrEmpty(ConfigServiceDescription))
            {
                if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_HOST_IP"))
                    ConfigServiceDescription = ConfigHostIP + ":" + ConfigPort;
            }

        }

        #endregion

    }
}
