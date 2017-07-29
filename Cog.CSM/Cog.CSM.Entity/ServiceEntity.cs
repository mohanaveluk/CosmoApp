using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace Cog.CSM.Entity
{
    public class ServiceEntity : TRW.BusinessCode, IFill
    {
        public int Config_ID { get; set; }
        public int Env_ID { get; set; }
        public int Sch_ID { get; set; }
        public string Env_Name { get; set; }
        public string Env_Location { get; set; }
        public string Config_ServiceType { get; set; }
        public string Config_ServerIP { get; set; }
        public string Config_PortNumber { get; set; }
        public string Config_ServiceURL { get; set; }
        public string Config_ServiceDescription { get; set; }
        public bool Config_IsValidated { get; set; }
        public bool Config_IsActive { get; set; }
        public bool Config_IsMonitored { get; set; }
        public bool Config_IsLocked { get; set; }
        public string Config_ServiceComments { get; set; }
        
        #region IFill Ticket details
        /// <summary>
        /// Fill Data for configuration details
        /// </summary>
        /// <param name="reader"></param>
        public void Fill(SqlDataReader reader)
        {
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_ID"))
                this.Config_ID = Convert.ToInt32(reader["CONFIG_ID"]);
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "ENV_ID"))
                this.Env_ID = Convert.ToInt32(reader["ENV_ID"]);
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "SCH_ID"))
                this.Sch_ID = Convert.ToInt32(reader["SCH_ID"]);
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "ENV_NAME"))
                this.Env_Name = Convert.ToString(reader["ENV_NAME"]);
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "ENV_LOCATION"))
                this.Env_Location = Convert.ToString(reader["ENV_LOCATION"]);
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_SERVICE_TYPE"))
                this.Config_ServiceType = Convert.ToString(reader["CONFIG_SERVICE_TYPE"]);
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_HOST_IP"))
                this.Config_ServerIP = Convert.ToString(reader["CONFIG_HOST_IP"]);
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_PORT_NUMBER"))
                this.Config_PortNumber = Convert.ToString(reader["CONFIG_PORT_NUMBER"]);
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_URL_ADDRESS"))
                this.Config_ServiceURL = Convert.ToString(reader["CONFIG_URL_ADDRESS"]);
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_DESCRIPTION"))
                this.Config_ServiceDescription = Convert.ToString(reader["CONFIG_DESCRIPTION"]);
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_IS_VALIDATED"))
                this.Config_IsValidated = Convert.ToBoolean(reader["CONFIG_IS_VALIDATED"]);
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_IS_ACTIVE"))
                this.Config_IsActive = Convert.ToBoolean(reader["CONFIG_IS_ACTIVE"]);
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_IS_MONITORED"))
                this.Config_IsMonitored = Convert.ToBoolean(reader["CONFIG_IS_MONITORED"]);
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_IS_LOCKED"))
                this.Config_IsLocked = Convert.ToBoolean(reader["CONFIG_IS_LOCKED"]);
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_COMMENTS"))
                this.Config_ServiceComments = Convert.ToString(reader["CONFIG_COMMENTS"]);
        }

        #endregion


    }
}
