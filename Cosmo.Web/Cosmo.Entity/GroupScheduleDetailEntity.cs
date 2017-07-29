using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Globalization;

namespace Cosmo.Entity
{
    public class GroupScheduleDetailEntity :TRW.NamedBusinessCode, IFill
    {
        #region property

        public int Group_Schedule_ID { get; set; }
        public int Group_ID { get; set; }
        public int Env_ID { get; set; }
        public string Env_Name { get; set; }

        public List<GroupScheduleServiceDetailEntity> ServiceDetails { get; set; }
 
        #endregion property


        #region IFill Environment details
        /// <summary>
        /// Fill Data for Environments
        /// </summary>
        /// <param name="reader"></param>
        public new void Fill(SqlDataReader reader)
        {
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "GROUP_SCH_ID"))
                this.Group_ID = Convert.ToInt32(reader["GROUP_SCH_ID"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "GROUP_ID"))
                this.Group_Schedule_ID = Convert.ToInt32(reader["GROUP_ID"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "ENV_ID"))
                this.Env_ID = Convert.ToInt32(reader["ENV_ID"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "ENV_NAME"))
                this.Env_Name = Convert.ToString(reader["ENV_NAME"]);

        }
        #endregion IFill Environment details


        #region IFill Group schedule service details
        /// <summary>
        /// Fill Data for Environments
        /// </summary>
        /// <param name="reader"></param>
        public GroupScheduleServiceDetailEntity FillService(SqlDataReader reader)
        {
            GroupScheduleServiceDetailEntity serviceDetails = new GroupScheduleServiceDetailEntity();

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "GROUP_ID"))
                this.Group_ID = Convert.ToInt32(reader["GROUP_ID"]);
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "GROUP_SCH_ID"))
                this.Group_Schedule_ID = Convert.ToInt32(reader["GROUP_SCH_ID"]);
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "GROUP_SERVICE_SCH_ID"))
                serviceDetails.Group_Schedule_Detail_ID = Convert.ToInt32(reader["GROUP_SERVICE_SCH_ID"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_ID"))
                serviceDetails.Config_ID = Convert.ToInt32(reader["CONFIG_ID"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "WIN_SERVICE_ID"))
                serviceDetails.WindowsService_ID = Convert.ToInt32(reader["WIN_SERVICE_ID"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_HOST_IP"))
                serviceDetails.HostIP = Convert.ToString(reader["CONFIG_HOST_IP"]).ToLower(CultureInfo.CurrentCulture);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_PORT_NUMBER"))
                serviceDetails.Port = Convert.ToString(reader["CONFIG_PORT_NUMBER"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_SERVICE_TYPE"))
            {
                if (Convert.ToString(reader["CONFIG_SERVICE_TYPE"]) == "1")
                    serviceDetails.Service_Name = "Content Manager";
                else if (Convert.ToString(reader["CONFIG_SERVICE_TYPE"]) == "2")
                    serviceDetails.Service_Name = "Dispatcher";
                else
                    serviceDetails.Service_Name = serviceDetails.HostIP + ":" + serviceDetails.Port;
            }

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "WIN_SERVICENAME"))
                serviceDetails.WindowsService_Name = Convert.ToString(reader["WIN_SERVICENAME"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "GROUP_SCH_STATUS"))
                serviceDetails.Schedule_Status = Convert.ToString(reader["GROUP_SCH_STATUS"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "GROUP_SCH_ISACTIVE"))
                serviceDetails.IsScheduleActive = Convert.ToBoolean(reader["GROUP_SCH_ISACTIVE"]);

            return serviceDetails;
        }
        #endregion IFill Group schedule service details
    }
}
