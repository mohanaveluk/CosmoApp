using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Cosmo.Entity
{
    public class DailyReportSubscriptionEntity: TRW.NamedBusinessCode, IFill
    {
        public int UserListId { get; set; }
        public int EnvId { get; set; }
        public string UserListEmailId { get; set; }
        public string UserListType{ get; set; }
        public string UserListActive{ get; set; }
        public string UserListCreatedBy{ get; set; }
        public string UserListCreatedDate{ get; set; }
        public string UserListUpdatedBy { get; set; }
        public string UserListUpdatedDate { get; set; }
        public int SubscriptionId { get; set; }
        public string SubscriptionType { get; set; }
        public string SubscriptionTime { get; set; }
        public bool SubscriptionIsActive { get; set; }
        public int SubscriptionDetailId { get; set; }
        public string SubscriptionUserListId { get; set; }
        public string SubscriptionEmail { get; set; }

        public void Fill(SqlDataReader reader)
        {
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "USRLST_ID"))
                this.UserListId = Convert.ToInt32(reader["USRLST_ID"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "ENV_ID"))
                this.EnvId = Convert.ToInt32(reader["ENV_ID"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "USRLST_EMAIL_ADDRESS"))
                this.UserListEmailId = Convert.ToString(reader["USRLST_EMAIL_ADDRESS"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "USRLST_TYPE"))
                this.UserListType = Convert.ToString(reader["USRLST_TYPE"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "USRLST_IS_ACTIVE"))
                UserListActive = Convert.ToString(reader["USRLST_IS_ACTIVE"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "USRLST_CREATED_BY"))
                UserListCreatedBy = Convert.ToString(reader["USRLST_CREATED_BY"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "USRLST_CREATED_DATE"))
                UserListCreatedDate = Convert.ToString(reader["USRLST_CREATED_DATE"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "USRLST_UPDATED_BY"))
                UserListUpdatedBy = Convert.ToString(reader["USRLST_UPDATED_BY"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "USRLST_UPDATED_DATE"))
                UserListUpdatedDate = Convert.ToString(reader["USRLST_UPDATED_DATE"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "SUBSCRIPTION_DETAIL_ID"))
                this.SubscriptionDetailId = Convert.ToInt32(reader["SUBSCRIPTION_DETAIL_ID"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "SUBSCRIPTION_ID"))
                this.SubscriptionId = Convert.ToInt32(reader["SUBSCRIPTION_ID"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "SUBSCRIPTION_TYPE"))
                SubscriptionType = Convert.ToString(reader["SUBSCRIPTION_TYPE"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "SUBSCRIPTION_TIME"))
                SubscriptionTime = Convert.ToString(reader["SUBSCRIPTION_TIME"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "SUBSCRIPTION_ISACTIVE"))
                SubscriptionIsActive = Convert.ToBoolean(reader["SUBSCRIPTION_ISACTIVE"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "SUBSCRIPTION_USRLST_ID"))
                this.SubscriptionUserListId = Convert.ToString(reader["SUBSCRIPTION_USRLST_ID"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "SUBSCRIPTION_EMAILID"))
                this.SubscriptionEmail = Convert.ToString(reader["SUBSCRIPTION_EMAILID"]);
        }
    }
}
