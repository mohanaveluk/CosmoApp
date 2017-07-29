using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Cosmo.Entity
{
    public class ServiceOnDemandMailEntity: TRW.NamedBusinessCode, IFill
    {
        public int WindowServiceId { get; set; }
        public string WindowServiceName { get; set; }
        public string WindowServiceStatus { get; set; }
        public int ConfigId { get; set; }
        public string HostIP { get; set; }
        public string Port { get; set; }
        public string ServiceTypeId { get; set; }
        public string ServiceType { get; set; }
        public string Env_Name { get; set; }
        public int EnvId { get; set; }
        public string RequestStatus { get; set; }
        public string RequestStatusCompletion { get; set; } //Stopped / Started / Restarted}
        public string CompletionStatus { get; set; } //Successful / Unsuccessful}
        public DateTime RequestedDateTime { get; set; } //
        public string ServerTimeZone { get; set; }
        public string Comments { get; set; }
        public string RequestedBy { get; set; }
        public bool OnDemand { get; set; }
        public string Result { get; set; }
        public string RequestSource { get; set; }
        public string GroupScheduledStatus { get; set; }
        public DateTime? ServiceStartedTime { get; set; }
        public DateTime? ServiceCompletionTime { get; set; }

        #region Fill windows service configuration details for Ondemand trigger
        
        public void Fill(System.Data.SqlClient.SqlDataReader reader)
        {
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "WIN_SERVICE_ID"))
                this.WindowServiceId = Convert.ToInt32(reader["WIN_SERVICE_ID"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "WIN_SERVICENAME"))
                this.WindowServiceName = reader["WIN_SERVICENAME"].ToString();

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "WIN_SERVICE_STATUS"))
                this.WindowServiceStatus = reader["WIN_SERVICE_STATUS"].ToString();
            else
                this.WindowServiceStatus = string.Empty;

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_ID"))
                this.ConfigId = Convert.ToInt32(reader["CONFIG_ID"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_HOST_IP"))
                this.HostIP = Convert.ToString(reader["CONFIG_HOST_IP"]).ToLower(CultureInfo.CurrentCulture);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_PORT_NUMBER"))
                this.Port = reader["CONFIG_PORT_NUMBER"].ToString();

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_SERVICE_TYPE"))
                this.ServiceTypeId = reader["CONFIG_SERVICE_TYPE"].ToString();

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "SERVICETYPE"))
                this.ServiceType = reader["SERVICETYPE"].ToString();

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "ENV_NAME"))
                this.Env_Name = reader["ENV_NAME"].ToString();

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "ENV_ID"))
                this.EnvId = Convert.ToInt32(reader["ENV_ID"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "WIN_SERVICE_REQUEST_STATUS"))
                this.RequestStatus = reader["WIN_SERVICE_REQUEST_STATUS"].ToString();
            else
                this.RequestStatus = string.Empty;

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "WIN_SERVICE_REQUEST_COMPLETION"))
                this.RequestStatusCompletion = reader["WIN_SERVICE_REQUEST_COMPLETION"].ToString();
            else
                this.RequestStatusCompletion = string.Empty;

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "WIN_SERVICE_REQUEST_COMPLETION_STATUS"))
                this.CompletionStatus = reader["WIN_SERVICE_REQUEST_COMPLETION_STATUS"].ToString();
            else
                this.CompletionStatus = string.Empty;

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "WIN_SERVICE_REQUESTED_TIME"))
                this.RequestedDateTime = Convert.ToDateTime(reader["WIN_SERVICE_REQUESTED_TIME"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "GROUP_SCH_ONDEMAND"))
                this.OnDemand = Convert.ToBoolean(reader["GROUP_SCH_ONDEMAND"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "GROUP_SCH_RESULT"))
                this.Result = Convert.ToString(reader["GROUP_SCH_RESULT"]);


            this.ServerTimeZone = string.Empty;
            this.Comments = string.Empty;
            this.RequestedBy = string.Empty;

        }

        #endregion Fill windows service configuration details for Ondemand trigger

    }
}
