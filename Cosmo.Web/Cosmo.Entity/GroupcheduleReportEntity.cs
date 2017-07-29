using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Cosmo.Entity
{
    public class GroupcheduleReportEntity: TRW.NamedBusinessCode, IFill
    {
        #region property for Restart service report

        public int GROUP_SCH_ID { get; set; }
        public int GROUP_ID { get; set; }
        public string GROUP_NAME { get; set; }
        public int ENV_ID { get; set; }
        public string ENV_NAME { get; set; }
        public string CONFIG_HOST_IP { get; set; }
        public string CONFIG_PORT_NUMBER { get; set; }
        public string CONFIG_SERVICE_NAME { get; set; }
        public string CONFIG_LOCATION { get; set; }
        public string CONFIG_SERVICE_TYPE { get; set; }
        public DateTime GROUP_SCH_TIME { get; set; }
        public string GROUP_SCH_ACTION { get; set; }
        public string GROUP_SCH_TYPE { get; set; }
        public string GROUP_SCH_DETAIL_TYPE { get; set; }
        public string GROUP_SCH_STATUS { get; set; }
        public string GROUP_SCH_DETAIL_STATUS { get; set; }
        public DateTime? GROUP_SCH_COMPLETED_TIME { get; set; }
        public string GROUP_SCH_COMMENTS { get; set; }
        public string GROUP_SCH_CREATED_BY { get; set; }
        public DateTime GROUP_SCH_CREATED_DATETIME { get; set; }
        public string GROUP_SCH_CREATED_BY_USERNAME { get; set; }
        public string GROUP_SCH_ONDEMAND { get; set; }
        public string GROUP_SCH_RESULT { get; set; }
        public string RequestSource { get; set; }
        public DateTime? ServiceUpdatedTime { get; set; }
        public DateTime? ServiceStartedTime { get; set; }
        public DateTime? ServiceCompletionTime { get; set; }
        #endregion property for Restart service report



        #region IFill Group schedule service details
        /// <summary>
        /// Fill Data for Environments
        /// </summary>
        /// <param name="reader"></param>
        public void Fill(System.Data.SqlClient.SqlDataReader reader)
        {
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "GROUP_SCH_ID"))
                this.GROUP_SCH_ID = Convert.ToInt32(reader["GROUP_SCH_ID"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "GROUP_ID"))
                this.GROUP_ID = Convert.ToInt32(reader["GROUP_ID"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "GROUP_NAME"))
                this.GROUP_NAME = Convert.ToString(reader["GROUP_NAME"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "ENV_ID"))
                this.ENV_ID = Convert.ToInt32(reader["ENV_ID"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "ENV_NAME"))
                this.ENV_NAME = Convert.ToString(reader["ENV_NAME"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_HOST_IP"))
                this.CONFIG_HOST_IP = Convert.ToString(reader["CONFIG_HOST_IP"]).ToLower(CultureInfo.CurrentCulture);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_PORT_NUMBER"))
                this.CONFIG_PORT_NUMBER = Convert.ToString(reader["CONFIG_PORT_NUMBER"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_DESCRIPTION"))
                this.CONFIG_SERVICE_NAME = Convert.ToString(reader["CONFIG_DESCRIPTION"]);
            
            if (string.IsNullOrEmpty(CONFIG_SERVICE_NAME))
            {
                if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_HOST_IP"))
                    CONFIG_SERVICE_NAME = CONFIG_HOST_IP + ":" + CONFIG_PORT_NUMBER;
            }

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_LOCATION"))
                this.CONFIG_LOCATION = Convert.ToString(reader["CONFIG_LOCATION"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_SERVICE_TYPE"))
                this.CONFIG_SERVICE_TYPE = Convert.ToString(reader["CONFIG_SERVICE_TYPE"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "GROUP_SCH_TIME"))
                this.GROUP_SCH_TIME = Convert.ToDateTime(reader["GROUP_SCH_TIME"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "GROUP_SCH_ACTION"))
                this.GROUP_SCH_ACTION = Convert.ToString(reader["GROUP_SCH_ACTION"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "GROUP_SCH_TYPE"))
                this.GROUP_SCH_TYPE = Convert.ToString(reader["GROUP_SCH_TYPE"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "GROUP_SCH_DETAIL_TYPE"))
                this.GROUP_SCH_DETAIL_TYPE = Convert.ToString(reader["GROUP_SCH_DETAIL_TYPE"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "GROUP_SCH_STATUS"))
                this.GROUP_SCH_STATUS = Convert.ToString(reader["GROUP_SCH_STATUS"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "GROUP_SCH_DETAIL_STATUS"))
                this.GROUP_SCH_DETAIL_STATUS = Convert.ToString(reader["GROUP_SCH_DETAIL_STATUS"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "GROUP_SCH_COMPLETED_TIME"))
                this.GROUP_SCH_COMPLETED_TIME = Convert.ToDateTime(reader["GROUP_SCH_COMPLETED_TIME"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "GROUP_SCH_COMMENTS"))
                this.GROUP_SCH_COMMENTS = Convert.ToString(reader["GROUP_SCH_COMMENTS"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "GROUP_SCH_CREATED_BY"))
                this.GROUP_SCH_CREATED_BY = Convert.ToString(reader["GROUP_SCH_CREATED_BY"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "GROUP_SCH_CREATED_DATETIME"))
                this.GROUP_SCH_CREATED_DATETIME = Convert.ToDateTime(reader["GROUP_SCH_CREATED_DATETIME"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "USERNAME"))
                this.GROUP_SCH_CREATED_BY_USERNAME = Convert.ToString(reader["USERNAME"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "GROUP_SCH_ONDEMAND"))
                this.GROUP_SCH_ONDEMAND = Convert.ToString(reader["GROUP_SCH_ONDEMAND"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "GROUP_SCH_RESULT"))
                this.GROUP_SCH_RESULT = Convert.ToString(reader["GROUP_SCH_RESULT"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "GROUP_SCH_REQUESTSOURCE"))
                RequestSource = Convert.ToString(reader["GROUP_SCH_REQUESTSOURCE"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "GROUP_SCH_UPDATEDTIME"))
                ServiceUpdatedTime = Convert.ToDateTime(reader["GROUP_SCH_UPDATEDTIME"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "GROUP_SCH_SERVICE_STARTTIME"))
                ServiceStartedTime = Convert.ToDateTime(reader["GROUP_SCH_SERVICE_STARTTIME"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "GROUP_SCH_SERVICE_COMPLETEDTIME"))
                ServiceCompletionTime = Convert.ToDateTime(reader["GROUP_SCH_SERVICE_COMPLETEDTIME"]);

        }
        #endregion IFill Group schedule service details
    }
}
