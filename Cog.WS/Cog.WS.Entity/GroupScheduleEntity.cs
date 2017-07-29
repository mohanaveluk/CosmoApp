using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace Cog.WS.Entity
{
    public class GroupScheduleEntity: TRW.NamedBusinessCode, IFill
    {
        #region property

        public int Group_Schedule_ID { get; set; }
        public int Group_ID { get; set; }
        public string Group_Name { get; set; }
        public DateTime Group_Schedule_Datatime { get; set; }
        public string Group_Schedule_Action { get; set; }
        public string Group_Schedule_Status { get; set; }
        public DateTime Group_Schedule_CompletedTime { get; set; }
        public string Group_Schedule_Comments { get; set; }
        public string Group_Schedule_CreatedBy { get; set; }
        public DateTime Group_Schedule_CreatedDatetime { get; set; }
        public string Group_Schedule_UpdatedBy { get; set; }
        public DateTime Group_Schedule_UpdatedDatetime { get; set; }

        public List<GroupScheduleDetailEntity> GroupScheduleDetails { get; set; }
        #endregion property


        #region IFill Environment details
        /// <summary>
        /// Fill Data for Environments
        /// </summary>
        /// <param name="reader"></param>
        public new void Fill(SqlDataReader reader)
        {
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "GROUP_SCH_ID"))
                this.Group_Schedule_ID = Convert.ToInt32(reader["GROUP_SCH_ID"]);
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "GROUP_ID"))
                this.Group_ID = Convert.ToInt32(reader["GROUP_ID"]);
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "GROUP_NAME"))
                this.Group_Name = Convert.ToString(reader["GROUP_NAME"]);
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "GROUP_SCH_TIME"))
                this.Group_Schedule_Datatime = Convert.ToDateTime(reader["GROUP_SCH_TIME"]);
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "GROUP_SCH_ACTION"))
                this.Group_Schedule_Action = Convert.ToString(reader["GROUP_SCH_ACTION"]);
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "GROUP_SCH_STATUS"))
                this.Group_Schedule_Status = Convert.ToString(reader["GROUP_SCH_STATUS"]);
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "GROUP_SCH_COMPLETED_TIME"))
                this.Group_Schedule_CompletedTime = Convert.ToDateTime(reader["GROUP_SCH_COMPLETED_TIME"]);
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "GROUP_SCH_COMMENTS"))
                this.Group_Schedule_Comments = Convert.ToString(reader["GROUP_SCH_COMMENTS"]);
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "USER_FIRST_NAME"))
                this.Group_Schedule_CreatedBy = Convert.ToString(reader["USER_FIRST_NAME"]);
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "GROUP_SCH_CREATED_DATETIME"))
                this.Group_Schedule_CreatedDatetime = Convert.ToDateTime(reader["GROUP_SCH_CREATED_DATETIME"]);
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "GROUP_SCH_UPDATED_BY"))
                this.Group_Schedule_UpdatedBy = Convert.ToString(reader["GROUP_SCH_UPDATED_BY"]);
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "GROUP_SCH_UPDATED_DATETIME"))
                this.Group_Schedule_UpdatedDatetime = Convert.ToDateTime(reader["GROUP_SCH_UPDATED_DATETIME"]);

        }
        #endregion IFill Environment details
    }
}
