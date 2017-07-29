using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Globalization;

namespace Cosmo.Entity
{
    public class GroupDetailEntity :TRW.NamedBusinessCode, IFill
    {
        #region property
        public int Group_Detail_ID { get; set; }
        public int Group_ID { get; set; }
        public int Env_ID { get; set; }
        public int Config_ID { get; set; }
        public int WinService_ID { get; set; }
        public string GroupName { get; set; }
        public string Env_Name { get; set; }
        public string WinServiceName { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string Comments { get; set; }
        public bool IsActive { get; set; }
        public string HostIP { get; set; }
        public string Port { get; set; }
        public string Location { get; set; }
        public string ServiceType { get; set; }
        public string MonitorStatus { get; set; }
        public string MonitorComments { get; set; }

        #endregion property


        #region IFill Environment details
        /// <summary>
        /// Fill Data for Environments
        /// </summary>
        /// <param name="reader"></param>
        public new void Fill(SqlDataReader reader)
        {
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "GROUP_DETAIL_ID"))
                this.Group_Detail_ID = Convert.ToInt32(reader["GROUP_DETAIL_ID"]);
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "GROUP_ID"))
                this.Group_ID = Convert.ToInt32(reader["GROUP_ID"]);
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "ENV_ID"))
                this.Env_ID = Convert.ToInt32(reader["ENV_ID"]);
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_ID"))
                this.Config_ID = Convert.ToInt32(reader["CONFIG_ID"]);
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "WIN_SERVICE_ID"))
                this.WinService_ID = Convert.ToInt32(reader["WIN_SERVICE_ID"]);
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "USERFIRSTNAME"))
                this.CreatedBy = Convert.ToString(reader["USERFIRSTNAME"]);
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "GROUP_CREATED_DATE"))
                this.CreatedDate = Convert.ToDateTime(reader["GROUP_CREATED_DATE"]);
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "USERFIRSTNAME"))
                this.UpdatedBy = Convert.ToString(reader["USERFIRSTNAME"]);
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "GROUP_UPDATED_DATE"))
                this.UpdatedDate = Convert.ToDateTime(reader["GROUP_UPDATED_DATE"]);
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "GROUP_DETAIL_COMMENTS"))
                this.Comments = Convert.ToString(reader["GROUP_DETAIL_COMMENTS"]);
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "GROUP_ISACTIVE"))
                this.IsActive = Convert.ToBoolean(reader["GROUP_ISACTIVE"]);
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_HOST_IP"))
                this.HostIP = Convert.ToString(reader["CONFIG_HOST_IP"]).ToLower(CultureInfo.CurrentCulture);
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_PORT_NUMBER"))
                this.Port = Convert.ToString(reader["CONFIG_PORT_NUMBER"]);
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "WIN_SERVICENAME"))
                this.WinServiceName = Convert.ToString(reader["WIN_SERVICENAME"]);
            else
            {
                this.WinServiceName = string.Empty;
            }
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "GROUP_NAME"))
                this.GroupName = Convert.ToString(reader["GROUP_NAME"]);
            else
            {
                this.GroupName = string.Empty;
            }
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_LOCATION"))
                this.Location = Convert.ToString(reader["CONFIG_LOCATION"]);
            else
            {
                this.Location  = string.Empty;
            }
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_SERVICE_TYPE"))
                this.ServiceType = Convert.ToString(reader["CONFIG_SERVICE_TYPE"]);
            else
            {
                this.ServiceType = string.Empty;
            }
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "ENV_NAME"))
                this.Env_Name = Convert.ToString(reader["ENV_NAME"]);
        }

        #endregion IFill Environment details
    }
}
