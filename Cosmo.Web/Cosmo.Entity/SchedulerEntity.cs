using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace Cosmo.Entity
{
    public class SchedulerEntity:TRW.NamedBusinessCode, IFill
    {
        public int SchedulerID { get; set; }
        public int EnvID { get; set; }
        public int ConfigID { get; set; }
        public int Interval { get; set; } // Duration
        public string Duration { get; set; }
        public string RepeatsOn { get; set; }
        public DateTime? StartDateTime { get; set; } // Duration
        public string EndAs { get; set; }
        public int EndOfOccurance { get; set; }
        public DateTime? EndDateTime { get; set; }
        public bool  IsActive { get; set; }
        public string Comments { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }


        #region IFill Environment details
        /// <summary>
        /// Fill Data for Environment details 
        /// </summary>
        /// <param name="reader"></param>
        public new void Fill(SqlDataReader reader)
        {
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "SCH_ID"))
                this.SchedulerID = Convert.ToInt32(reader["SCH_ID"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "ENV_ID"))
                this.EnvID = Convert.ToInt32(reader["ENV_ID"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONGIG_ID"))
                this.ConfigID = Convert.ToInt32(reader["CONGIG_ID"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "SCH_INTERVAL")) // frequency
                this.Interval = Convert.ToInt32(reader["SCH_INTERVAL"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "SCH_DURATION"))
                this.Duration = Convert.ToString(reader["SCH_DURATION"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "SCH_REPEATS"))
                this.RepeatsOn = Convert.ToString(reader["SCH_REPEATS"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "SCH_STARTBY"))
                this.StartDateTime = Convert.ToDateTime(reader["SCH_STARTBY"]);
            
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "SCH_ENDAS"))
                this.EndAs = Convert.ToString(reader["SCH_ENDAS"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "SCH_END_OCCURANCE"))
                this.EndOfOccurance = Convert.ToInt32(reader["SCH_END_OCCURANCE"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "SCH_ENDBY"))
                this.EndDateTime = Convert.ToDateTime(reader["SCH_ENDBY"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "SCH_IS_ACTIVE"))
                this.IsActive = Convert.ToBoolean(reader["SCH_IS_ACTIVE"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "SCH_COMMENTS"))
                this.Comments = Convert.ToString(reader["SCH_COMMENTS"]);
        
        #endregion IFill Environment details
        }

    }
}
