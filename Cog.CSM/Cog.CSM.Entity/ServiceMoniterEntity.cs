using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using Cog.CSM.Entity;

namespace Cog.CSM.Entity
{
    public class ServiceMoniterEntity:TRW.NamedBusinessCode, IFill
    {
        #region properties
        public int EnvID { get; set; }
        public string EnvName { get; set; }
        public bool IsActive { get; set; }
        public bool IsMonitor { get; set; }
        public bool IsNotify { get; set; }
        public bool IsConsolidated { get; set; }
        public int SortOrder { get; set; }

        public List<MonitorEntity> monitorList { get; set; }
        public EnvironmentMonitorStatus OverallStatus { get; set; }

        #endregion properties

        #region IFill Environment details
        /// <summary>
        /// Fill Data for Environment details 
        /// </summary>
        /// <param name="reader"></param>
        public new void Fill(SqlDataReader reader)
        {
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "ENV_ID"))
                this.EnvID = Convert.ToInt32(reader["ENV_ID"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "ENV_NAME"))
                this.EnvName = Convert.ToString(reader["ENV_NAME"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "ENV_IS_MONITOR"))
                this.IsMonitor = Convert.ToBoolean(reader["ENV_IS_MONITOR"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "ENV_IS_NOTIFY"))
                this.IsNotify = Convert.ToBoolean(reader["ENV_IS_NOTIFY"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "ENV_IS_CONSLTD_MAIL"))
                this.IsConsolidated = Convert.ToBoolean(reader["ENV_IS_CONSLTD_MAIL"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "ENV_SORTORDER"))
                this.SortOrder = Convert.ToInt32(reader["ENV_SORTORDER"]);

            monitorList = new List<MonitorEntity>();

        }

        #endregion

    }

    public class EnvironmentMonitorStatus
    {
        public int Id { get; set; }
        public string ImagePath { get; set; }
        public string Description { get; set; }

        public enum StatusCode
        {
            Running=1,
            Stopped,
            OneOrMoreStopped,
            NotAvailable,
            PartiallyAvailable,
        }
    }
}
