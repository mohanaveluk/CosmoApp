using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Globalization;

namespace Cosmo.Entity
{
    public class EnvDetailsEntity : TRW.NamedBusinessEntity, IFill
    {
        private string CONTENT_SERVICE = ConfigurationManager.AppSettings["ContentService"].ToString();
        private string DESPATCHER_SERVICE = ConfigurationManager.AppSettings["DispatcherService"].ToString();

        #region properties
        public int EnvDetID { get; set; }
        public int EnvID { get; set; }
        public string EnvDetHostIP { get; set; }
        public string EnvDetPort { get; set; }
        public string EnvDetDescription { get; set; }
        public string EnvDetLocation { get; set; }
        public string EnvDetMailFrequency { get; set; }
        public bool EnvDetIsMonitor { get; set; }
        public bool EnvDetIsNotify { get; set; }
        public bool EnvDetIsValidated { get; set; }
        public bool EnvDetIsServiceConsolidated { get; set; }
        public bool EnvDetIsLocked { get; set; }
        public string EnvDetCreatedBy { get; set; }
        public DateTime? EnvDetCreatedDate { get; set; }
        public string EnvDetUpdatedBy { get; set; }
        public DateTime? EnvDetUpdatedDate { get; set; }
        public string EnvDetComments { get; set; }
        public string EnvDetIsActive { get; set; }
        public string EnvDetServiceType { get; set; }
        public string EnvDetServiceTypeDesc { get; set; }
        public string EnvDetServiceURL { get; set; }
        public string EnvDet_Name { get; set; }
        public int SchedulerID { get; set; }
        public string SchedulerSummary { get; set; }
        public bool EnvDetIsPrimay { get; set; }
        public int EnvDetReferenceID { get; set; }
        public int WindowsServiceID { get; set; }
        public string WindowsServiceName { get; set; }

        #endregion properties

        #region IFill Environment details
        /// <summary>
        /// Fill Data for Environment details 
        /// </summary>
        /// <param name="reader"></param>
        public new void Fill(SqlDataReader reader)
        {
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_ID"))
                this.EnvDetID = Convert.ToInt32(reader["CONFIG_ID"]);
            
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "ENV_ID"))
                this.EnvID = Convert.ToInt32(reader["ENV_ID"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "ENV_NAME"))
                this.EnvDet_Name = Convert.ToString(reader["ENV_NAME"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_HOST_IP"))
                this.EnvDetHostIP = Convert.ToString(reader["CONFIG_HOST_IP"]).ToLower(CultureInfo.CurrentCulture);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_PORT_NUMBER"))
                this.EnvDetPort = Convert.ToString(reader["CONFIG_PORT_NUMBER"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_DESCRIPTION"))
                EnvDetDescription = Convert.ToString(reader["CONFIG_DESCRIPTION"]);

            if (string.IsNullOrEmpty(EnvDetDescription))
            {
                if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_HOST_IP"))
                    EnvDetDescription = EnvDetHostIP + ":" + EnvDetPort;
            }

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_SERVICE_TYPE"))
            {
                this.EnvDetServiceType = Convert.ToString(reader["CONFIG_SERVICE_TYPE"]);
                if (Convert.ToString(reader["CONFIG_SERVICE_TYPE"]) == "1")
                    EnvDetServiceTypeDesc = CONTENT_SERVICE;
                else if (Convert.ToString(reader["CONFIG_SERVICE_TYPE"]) == "2")
                    EnvDetServiceTypeDesc = DESPATCHER_SERVICE;
                else
                    EnvDetServiceTypeDesc = "Service Type does not available";
            }

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_URL_ADDRESS"))
                this.EnvDetServiceURL = Convert.ToString(reader["CONFIG_URL_ADDRESS"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_LOCATION"))
                this.EnvDetLocation = Convert.ToString(reader["CONFIG_LOCATION"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_MAIL_FREQ"))
                this.EnvDetMailFrequency = Convert.ToString(reader["CONFIG_MAIL_FREQ"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_IS_MONITORED"))
                this.EnvDetIsMonitor = Convert.ToBoolean(reader["CONFIG_IS_MONITORED"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_ISNOTIFY"))
                this.EnvDetIsNotify = Convert.ToBoolean(reader["CONFIG_ISNOTIFY"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_IS_VALIDATED"))
                this.EnvDetIsValidated = Convert.ToBoolean(reader["CONFIG_IS_VALIDATED"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_IS_LOCKED"))
                this.EnvDetIsLocked = Convert.ToBoolean(reader["CONFIG_IS_LOCKED"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_ISCONSOLIDATED"))
                this.EnvDetIsServiceConsolidated = Convert.ToBoolean(reader["CONFIG_ISCONSOLIDATED"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_IS_ACTIVE"))
                this.EnvDetIsActive = Convert.ToString(reader["CONFIG_IS_ACTIVE"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "USERFIRSTNAME"))
                this.EnvDetCreatedBy = Convert.ToString(reader["USERFIRSTNAME"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_CREATED_DATE"))
                this.EnvDetCreatedDate = Convert.ToDateTime(reader["CONFIG_CREATED_DATE"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "USERFIRSTNAME"))
                this.EnvDetUpdatedBy = Convert.ToString(reader["USERFIRSTNAME"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_UPDATED_DATE"))
                this.EnvDetUpdatedDate = Convert.ToDateTime(reader["CONFIG_UPDATED_DATE"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_COMMENTS"))
                this.EnvDetComments = Convert.ToString(reader["CONFIG_COMMENTS"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "SCH_ID"))
                this.SchedulerID = Convert.ToInt32(reader["SCH_ID"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "SCH_COMMENTS"))
                this.SchedulerSummary = Convert.ToString(reader["SCH_COMMENTS"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_ISPRIMARY"))
                this.EnvDetIsPrimay = Convert.ToBoolean(reader["CONFIG_ISPRIMARY"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_REF_ID"))
                this.EnvDetReferenceID = Convert.ToInt32(reader["CONFIG_REF_ID"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "WIN_SERVICE_ID"))
                this.WindowsServiceID = Convert.ToInt32(reader["WIN_SERVICE_ID"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "WIN_SERVICENAME"))
                this.WindowsServiceName = Convert.ToString(reader["WIN_SERVICENAME"]);
        
        }

        #endregion

    }

}
