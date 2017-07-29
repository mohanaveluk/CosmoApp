using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace Cog.CSM.Entity
{
    public class SendNotificationEntity:TRW.NamedBusinessCode, IFill
    {
        #region property declaration

        public int Env_ID { get; set; }
        public int ConfigID { get; set; }
        public string ConfigServiceType { get; set; }
        public bool ConfigIsActive { get; set; }
        public bool ConfigIsNotify { get; set; }
        public int ConfigMailFrequency { get; set; }
        public string LastMonitorStatus { get; set; }
        public DateTime LastMonitorUpdated { get; set; }

        #endregion property declaration

        #region IFill Ticket details
        public void Fill(SqlDataReader reader)
        {
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "ENV_ID"))
                this.Env_ID = Convert.ToInt32(reader["ENV_ID"]);
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_ID"))
                this.ConfigID = Convert.ToInt32(reader["CONFIG_ID"]);
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_SERVICE_TYPE"))
                this.ConfigServiceType = Convert.ToString(reader["CONFIG_SERVICE_TYPE"]);
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_IS_ACTIVE"))
                this.ConfigIsActive = Convert.ToBoolean(reader["CONFIG_IS_ACTIVE"]);
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_ISNOTIFY"))
                this.ConfigIsNotify = Convert.ToBoolean(reader["CONFIG_ISNOTIFY"]);
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_MAIL_FREQ"))
                this.ConfigMailFrequency = Convert.ToInt32(reader["CONFIG_MAIL_FREQ"]);
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "MON_STATUS"))
                this.LastMonitorStatus = Convert.ToString(reader["MON_STATUS"]);
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "EMTRAC_CREATED_DATE"))
                this.LastMonitorUpdated = Convert.ToDateTime(reader["EMTRAC_CREATED_DATE"]);

        }

        #endregion
    }
}
