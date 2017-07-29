using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace Cog.CSM.Entity
{
    public class GroupScheduleServiceDetailEntity : IFill
    {
        #region property

        public int Group_ID { get; set; }
        public int Env_ID { get; set; }
        public int Group_Schedule_ID { get; set; }
        public int Group_Schedule_Detail_ID { get; set; }
        public int Config_ID { get; set; }
        public int WindowsService_ID { get; set; }
        public string HostIP { get; set; }
        public string Port { get; set; }
        public string Service_Name { get; set; }
        public string WindowsService_Name { get; set; }
        public string Schedule_Status { get; set; }
        public bool IsScheduleActive { get; set; }

        #endregion property


        #region IFill Group schedule service details
        /// <summary>
        /// Fill Data for Environments
        /// </summary>
        /// <param name="reader"></param>
        public new void Fill(SqlDataReader reader)
        {

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "GROUP_ID"))
                this.Group_ID = Convert.ToInt32(reader["GROUP_ID"]);
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "ENV_ID"))
                this.Env_ID = Convert.ToInt32(reader["ENV_ID"]);
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "GROUP_SCH_ID"))
                this.Group_Schedule_ID = Convert.ToInt32(reader["GROUP_SCH_ID"]);
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "GROUP_SERVICE_SCH_ID"))
                this.Group_Schedule_Detail_ID = Convert.ToInt32(reader["GROUP_SERVICE_SCH_ID"]);
           
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_ID"))
                this.Config_ID = Convert.ToInt32(reader["CONFIG_ID"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "WIN_SERVICE_ID"))
                this.WindowsService_ID = Convert.ToInt32(reader["WIN_SERVICE_ID"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_HOST_IP"))
                this.HostIP = Convert.ToString(reader["CONFIG_HOST_IP"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_PORT_NUMBER"))
                this.Port = Convert.ToString(reader["CONFIG_PORT_NUMBER"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_SERVICE_TYPE"))
            {
                if (Convert.ToString(reader["CONFIG_SERVICE_TYPE"]) == "1")
                    this.Service_Name = "Content Manager";
                else if(Convert.ToString(reader["CONFIG_SERVICE_TYPE"]) == "2")
                    this.Service_Name = "Despatcher";
                else
                    this.Service_Name = this.HostIP + ":" + this.Port;
            }

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "WIN_SERVICENAME"))
                this.WindowsService_Name = Convert.ToString(reader["WIN_SERVICENAME"]);
            
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "GROUP_SCH_STATUS"))
                this.Schedule_Status = Convert.ToString(reader["GROUP_SCH_STATUS"]);
            
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "GROUP_SCH_ISACTIVE"))
                this.IsScheduleActive = Convert.ToBoolean(reader["GROUP_SCH_ISACTIVE"]);
        }
        #endregion IFill Group schedule service details
    }
}
