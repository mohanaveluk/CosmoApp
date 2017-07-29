using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Cog.CSM.Entity
{
    public class Subscription:TRW.NamedBusinessCode, IFill
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Time { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public DateTime? LastJobRanTime { get; set; }
        public DateTime? NextJonRunTime { get; set; }
        public List<SubscriptionDetail> SubscriptionDetails { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public void Fill(SqlDataReader reader)
        {

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "SUBSCRIPTION_ID"))
                Id = Convert.ToInt32(reader["SUBSCRIPTION_ID"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "SUBSCRIPTION_TYPE"))
                Type = Convert.ToString(reader["SUBSCRIPTION_TYPE"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "SUBSCRIPTION_TIME"))
                Time = Convert.ToString(reader["SUBSCRIPTION_TIME"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "SUBSCRIPTION_ISACTIVE"))
                IsActive = Convert.ToBoolean(reader["SUBSCRIPTION_ISACTIVE"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "CREATEDBY_NAME"))
                CreatedBy = Convert.ToString(reader["CREATEDBY_NAME"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "CREATED_DATE"))
                CreatedDate = Convert.ToDateTime(reader["CREATED_DATE"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "UPDATEDBY_NAME"))
                UpdatedBy = Convert.ToString(reader["UPDATEDBY_NAME"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "UPDATED_DATE"))
                UpdatedDate = Convert.ToDateTime(reader["UPDATED_DATE"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "SCH_LASTJOBRAN_TIME"))
                LastJobRanTime = Convert.ToDateTime(reader["SCH_LASTJOBRAN_TIME"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "SCH_NEXTJOBRAN_TIME"))
                NextJonRunTime = Convert.ToDateTime(reader["SCH_NEXTJOBRAN_TIME"]);

        }
    }


    public class SubscriptionDetail: TRW.NamedBusinessCode, IFill
    {
        public int DetailId { get; set; }
        public int Id { get; set; }
        public int UserListId { get; set; }
        public string EmailAddress { get; set; }
        public bool IsActive { get; set; }

        public void Fill(SqlDataReader reader)
        {
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "SUBSCRIPTION_ID"))
                Id = Convert.ToInt32(reader["SUBSCRIPTION_ID"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "SUBSCRIPTION_DETAIL_ID"))
                DetailId = Convert.ToInt32(reader["SUBSCRIPTION_DETAIL_ID"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "USRLST_ID"))
                UserListId = Convert.ToInt32(reader["USRLST_ID"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "SUBSCRIPTION_EMAILID"))
                EmailAddress = Convert.ToString(reader["SUBSCRIPTION_EMAILID"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "SUBSCRIPTION_ISACTIVE"))
                IsActive = Convert.ToBoolean(reader["SUBSCRIPTION_ISACTIVE"]);
        }
    }

    public class SubscriptionMailService
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Time { get; set; }
        public DateTime? LastJobRanTime { get; set; }
        public DateTime? NextJonRunTime { get; set; }
        public string ServiceStatusDetails { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }

    }
}
