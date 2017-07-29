using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace Cog.CSM.Entity
{
    public class Scheduler_details :TRW.BusinessCode, IFill
    {
        public int Scheduler_id { get; set; }
        public int Env_ID { get; set; }
        public string Env_name { get; set; }
        public int Sch_interval { get; set; }
        public string Sch_duration { get; set; }
        public DateTime? Sch_lastJobRunTime { get; set; }
        public DateTime? Sch_nextJobRunTime { get; set; }
        public string Sch_comments { get; set; }


        #region IFill Ticket details
        public void Fill(SqlDataReader reader)
        {
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "SCH_ID"))
                this.Scheduler_id = Convert.ToInt32(reader["SCH_ID"]);
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "ENV_ID"))
                this.Env_ID = Convert.ToInt32(reader["ENV_ID"]);
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "ENV_NAME"))
                this.Env_name = Convert.ToString(reader["ENV_NAME"]);
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "SCH_INTERVAL"))
                this.Sch_interval = Convert.ToInt32(reader["SCH_INTERVAL"]);
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "SCH_DURATION"))
                this.Sch_duration = Convert.ToString(reader["SCH_DURATION"]);
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "SCH_LASTJOBRAN_TIME"))
                this.Sch_lastJobRunTime = Convert.ToDateTime(reader["SCH_LASTJOBRAN_TIME"]);
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "SCH_NEXTJOBRAN_TIME"))
                this.Sch_nextJobRunTime = Convert.ToDateTime(reader["SCH_NEXTJOBRAN_TIME"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "SCH_COMMENTS"))
                this.Sch_comments = reader["SCH_COMMENTS"].ToString().Trim();

        }

        #endregion

    }
}
