using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Configuration;
using System.Globalization;

namespace Cog.WS.Entity
{
    public class GroupScheduleServiceDetailEntity : TRW.NamedBusinessCode, IFill
    {
        #region Constants
        private string CONTENTSERVICE = ConfigurationManager.AppSettings["ContentService"].ToString();
        private string DESPATCHER = ConfigurationManager.AppSettings["DespatcherService"].ToString();
        #endregion Constants

        #region property

        public int Group_ID { get; set; }
        public int Env_ID { get; set; }
        public string Env_Name { get; set; }
        public int Group_Schedule_ID { get; set; }
        public int Group_Schedule_Detail_ID { get; set; }
        public int Config_ID { get; set; }
        public int WindowsServiceId { get; set; }
        public string HostIP { get; set; }
        public string Port { get; set; }
        public string Status { get; set; }
        public string Service_Name { get; set; }
        public string WindowsServiceName { get; set; }
        public string WindowsServiceStatus { get; set; }
        public string Schedule_Status { get; set; }
        public bool IsScheduleActive { get; set; }
        public DateTime Schedule_UpdatedTime { get; set; }
        public string Schedule_Action { get; set; }
        public string Group_Schedule_Timezone { get; set; }
        public string Group_ActionCompletion_Status { get; set; }
        public string Group_Schedule_Action { get; set; }
        public string Group_Name { get; set; }
        public string Group_Schedule_CreatedBy { get; set; }
        public string ServiceTypeShort { get; set; }
        public DateTime? Group_Schedule_CreatedDatetime { get; set; }
        public DateTime? Group_Schedule_StartedTime { get; set; }
        public DateTime? Group_Schedule_UpdatedDatetime { get; set; }

        public string ProcessStatus { get; set; }

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
                this.WindowsServiceId = Convert.ToInt32(reader["WIN_SERVICE_ID"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_HOST_IP"))
                this.HostIP = Convert.ToString(reader["CONFIG_HOST_IP"]).ToLower(CultureInfo.CurrentCulture);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_PORT_NUMBER"))
                this.Port = Convert.ToString(reader["CONFIG_PORT_NUMBER"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_SERVICE_TYPE"))
            {
                switch (Convert.ToString(reader["CONFIG_SERVICE_TYPE"]))
                {
                    case "1":
                        this.Service_Name = CONTENTSERVICE;
                        ServiceTypeShort = "CM";
                        break;
                    case "2":
                        this.Service_Name = DESPATCHER;
                        ServiceTypeShort = "DISP";
                        break;
                    default:
                        this.Service_Name = this.HostIP + ":" + this.Port;
                        ServiceTypeShort = "CM";
                        break;
                }
            }

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "WIN_SERVICENAME"))
                this.WindowsServiceName = Convert.ToString(reader["WIN_SERVICENAME"]);
            
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "GROUP_SCH_STATUS"))
                this.Schedule_Status = Convert.ToString(reader["GROUP_SCH_STATUS"]);
            
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "GROUP_SCH_ISACTIVE"))
                this.IsScheduleActive = Convert.ToBoolean(reader["GROUP_SCH_ISACTIVE"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "GROUP_SCH_CREATED_DATETIME"))
                this.Group_Schedule_CreatedDatetime = Convert.ToDateTime(reader["GROUP_SCH_CREATED_DATETIME"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "GROUP_SCH_UPDATEDTIME"))
                this.Schedule_UpdatedTime = (DateTime) (this.Group_Schedule_UpdatedDatetime = Convert.ToDateTime(reader["GROUP_SCH_UPDATEDTIME"]));
        }
        #endregion IFill Group schedule service details

        public GroupScheduleServiceDetailEntity ShallowCopy()
        {
            return (GroupScheduleServiceDetailEntity) MemberwiseClone();
        }
    }
}
