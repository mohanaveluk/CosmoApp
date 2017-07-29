using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace Cog.CSM.Entity
{
    public class LogEntity: IFill
    {
        public int LOGID { get; set; }
        public int ENV_ID { get; set; }
        public int SCH_ID { get; set; }
        public int CONFIG_ID { get; set; }
        public string LogDescription { get; set; }
        public string LogError { get; set; }
        public DateTime? UpdatedDateTime{ get; set; }
        public string UpdatedBy { get; set; }


        #region IFill Logging of job details
        public void Fill(SqlDataReader reader)
        {
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "LOGID"))
                this.LOGID = Convert.ToInt32(reader["LOGID"]);
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "ENV_ID"))
                this.ENV_ID = Convert.ToInt32(reader["ENV_ID"]);
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "SCH_ID"))
                this.SCH_ID = Convert.ToInt32(reader["SCH_ID"]);
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_ID"))
                this.CONFIG_ID = Convert.ToInt32(reader["CONFIG_ID"]);
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "LOG_UPDATED_DATETIME"))
                this.UpdatedDateTime = Convert.ToDateTime(reader["LOG_UPDATED_DATETIME"]);
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "LOGDESCRIPTION"))
                this.LogDescription = Convert.ToString(reader["LOGDESCRIPTION"]);
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "LOGERROR"))
                this.LogError = Convert.ToString(reader["LOGERROR"]);
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "LOG_UPDATED_BY"))
                this.UpdatedBy = Convert.ToString(reader["LOG_UPDATED_BY"]);
        }
        #endregion IFill Logging of job details
    }
}
