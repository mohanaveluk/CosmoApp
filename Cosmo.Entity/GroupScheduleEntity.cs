using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Globalization;

namespace Cosmo.Entity
{
    public class GroupScheduleEntity : TRW.NamedBusinessCode, IFill
    {
        #region property

        public int Group_Schedule_ID { get; set; }
        public int Group_ID { get; set; }
        public string Group_Name { get; set; }
        public DateTime? Group_Schedule_Datatime { get; set; }
        public string Group_Schedule_DatatimeStr { get; set; }
        public string Group_Schedule_Action { get; set; }
        public string Group_Schedule_Status { get; set; }
        public DateTime? Group_Schedule_CompletedTime { get; set; }
        public string Group_Schedule_Comments { get; set; }
        public string Group_Schedule_CreatedBy { get; set; }
        public DateTime Group_Schedule_CreatedDatetime { get; set; }
        public string Group_Schedule_UpdatedBy { get; set; }
        public DateTime? Group_Schedule_UpdatedDatetime { get; set; }
        public bool Group_Schedule_OnDemand { get; set; }
        public string Group_Schedule_CompleteStatus { get; set; }
        public string RequestSource { get; set; }
        public DateTime? ServiceStartedTime { get; set; }
        public DateTime? ServiceCompletionTime { get; set; }

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
            {
                this.Group_Schedule_Datatime = Convert.ToDateTime(reader["GROUP_SCH_TIME"]);
                Group_Schedule_DatatimeStr =
                    Convert.ToDateTime(reader["GROUP_SCH_TIME"]).ToString(CultureInfo.InvariantCulture);
            }
            else
            {
                this.Group_Schedule_Datatime = null;
                Group_Schedule_DatatimeStr = string.Empty;
            }
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "GROUP_SCH_ACTION"))
                this.Group_Schedule_Action = Convert.ToString(reader["GROUP_SCH_ACTION"]);
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "GROUP_SCH_STATUS"))
                this.Group_Schedule_Status = Convert.ToString(reader["GROUP_SCH_STATUS"]);
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "GROUP_SCH_COMPLETED_TIME"))
                this.Group_Schedule_CompletedTime = Convert.ToDateTime(reader["GROUP_SCH_COMPLETED_TIME"]);
            else
                this.Group_Schedule_CompletedTime = null;

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "GROUP_SCH_COMMENTS"))
                this.Group_Schedule_Comments = Convert.ToString(reader["GROUP_SCH_COMMENTS"]);
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "USERFIRSTNAME"))
                this.Group_Schedule_CreatedBy = Convert.ToString(reader["USERFIRSTNAME"]);
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "GROUP_SCH_CREATED_DATETIME"))
                this.Group_Schedule_CreatedDatetime = Convert.ToDateTime(reader["GROUP_SCH_CREATED_DATETIME"]);
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "USERFIRSTNAME"))
                this.Group_Schedule_UpdatedBy = Convert.ToString(reader["USERFIRSTNAME"]);
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "GROUP_SCH_UPDATED_DATETIME"))
                this.Group_Schedule_UpdatedDatetime = Convert.ToDateTime(reader["GROUP_SCH_UPDATED_DATETIME"]);
            else
            {
                this.Group_Schedule_UpdatedDatetime = null;
            }
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "GROUP_SCH_ONDEMAND"))
                this.Group_Schedule_OnDemand = Convert.ToBoolean(reader["GROUP_SCH_ONDEMAND"]);
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "GROUP_SCH_RESULT"))
                this.Group_Schedule_CompleteStatus = Convert.ToString(reader["GROUP_SCH_RESULT"]);
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "GROUP_SCH_REQUESTSOURCE"))
                RequestSource = Convert.ToString(reader["GROUP_SCH_REQUESTSOURCE"]);


            if (CommonUtility.IsColumnExistsAndNotNull(reader, "@GROUP_SCH_SERVICE_STARTTIME"))
                this.ServiceStartedTime = Convert.ToDateTime(reader["@GROUP_SCH_SERVICE_STARTTIME"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "@GROUP_SCH_SERVICE_COMPLETEDTIME"))
                ServiceCompletionTime = Convert.ToDateTime(reader["@GROUP_SCH_SERVICE_COMPLETEDTIME"]);
        }

        #endregion IFill Environment details
    }
}
