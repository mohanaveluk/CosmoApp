using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Cosmo.Entity;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;

namespace Cosmo.Data
{
    public class ReportDataOrcl : BusinessEntityBaseDAO, IReportData
    {
        #region Constant Variables
        
        private const string PackageNameEnvironment = "COSMO_ENVIRONMENT_PACKAGE.";
        private static readonly string GetSubscriptionUserEmail = string.Format("{0}FN_CWT_GetSubsUserEmail", PackageNameEnvironment);
        private static readonly string SetSubscriptionUserEmail = string.Format("{0}SP_CWT_InsUpdSubscription", PackageNameEnvironment);
        
        #endregion Constant Variables

        public List<DailyReportSubscriptionEntity> GetDailyReportSubscription(int envId)
        {
            var pList = new List<OracleParameter>
            {
                GetParameter("p_ENV_ID", envId, OracleDbType.Int32, ParameterDirection.Input),
            };
            var envEmailList = ReadCompoundEntityList<DailyReportSubscriptionEntity>(GetSubscriptionUserEmail, pList,
                RowToSubscriptionUserEmail);
            return envEmailList;
        }

        public int InsertUpdateEmailSubscription(EmailSubscription emailSubscription)
        {
            var recInsert = 0;
            var nextJobRunTime =
                    Convert.ToDateTime(DateTime.Now.ToShortDateString() + " 00:00:00").AddDays(1)
                        .AddHours(Convert.ToInt32(emailSubscription.Time));

            try
            {
                var pList = new List<OracleParameter>
                {
                    GetParameter("p_SUBSCRIPTION_ID", emailSubscription.Id, OracleDbType.Int32, ParameterDirection.Input),
                    GetParameter("p_SUBSCRIPTION_TYPE", emailSubscription.Type ?? string.Empty, OracleDbType.Varchar2, ParameterDirection.Input),
                    GetParameter("p_SUBSCRIPTION_TIME", emailSubscription.Time ?? string.Empty, OracleDbType.Varchar2, ParameterDirection.Input),
                    GetParameter("p_SUBSCRIPTION_ISACTIVE", emailSubscription.IsActive ? 1 : 0, OracleDbType.Int32, ParameterDirection.Input),
                    GetParameter("p_CREATED_BY", emailSubscription.CreatedBy ?? string.Empty, OracleDbType.Varchar2, ParameterDirection.Input),
                    GetParameter("p_CREATED_DATE", DateTime.Now, OracleDbType.TimeStamp, ParameterDirection.Input),
                    GetParameter("p_UPDATED_BY", emailSubscription.UpdatedBy ?? string.Empty, OracleDbType.Varchar2, ParameterDirection.Input),
                    GetParameter("p_UPDATED_DATE", DateTime.Now, OracleDbType.TimeStamp, ParameterDirection.Input),
                    GetParameter("p_NEXTJOBRUNTIME", nextJobRunTime, OracleDbType.TimeStamp, ParameterDirection.Input),
                    GetParameter("p_SUBSCRIPTION_EMAILS", emailSubscription.EmailList ?? string.Empty, OracleDbType.Varchar2, ParameterDirection.Input),
                    new OracleParameter("p_SCOPE_OUTPUT",OracleDbType.Int32, ParameterDirection.Output)
                };

                ExecuteNonQuery(SetSubscriptionUserEmail, pList);

                foreach (var oracleParameter in pList.Where(oracleParameter => oracleParameter.ParameterName == "p_SCOPE_OUTPUT"))
                {
                    recInsert = OracleDecimalToInt((OracleDecimal)oracleParameter.Value);
                }
            }
            catch (Exception exception)
            {
                recInsert = -1;
                throw new ApplicationException(exception.Message);
            }
            return recInsert;
        }

        private List<DailyReportSubscriptionEntity> RowToSubscriptionUserEmail(OracleDataReader reader)
        {
            var list = new List<DailyReportSubscriptionEntity>();

            while (reader.Read())
            {
                var entity = new DailyReportSubscriptionEntity();
                if (CommonUtility.IsColumnExistsAndNotNull(reader, "USRLST_ID"))
                    entity.UserListId = Convert.ToInt32(reader["USRLST_ID"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "ENV_ID"))
                    entity.EnvId = Convert.ToInt32(reader["ENV_ID"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "USRLST_EMAIL_ADDRESS"))
                    entity.UserListEmailId = Convert.ToString(reader["USRLST_EMAIL_ADDRESS"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "USRLST_TYPE"))
                    entity.UserListType = Convert.ToString(reader["USRLST_TYPE"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "USRLST_IS_ACTIVE"))
                    entity.UserListActive = Convert.ToString(reader["USRLST_IS_ACTIVE"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "USRLST_CREATED_BY"))
                    entity.UserListCreatedBy = Convert.ToString(reader["USRLST_CREATED_BY"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "USRLST_CREATED_DATE"))
                    entity.UserListCreatedDate = Convert.ToString(reader["USRLST_CREATED_DATE"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "USRLST_UPDATED_BY"))
                    entity.UserListUpdatedBy = Convert.ToString(reader["USRLST_UPDATED_BY"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "USRLST_UPDATED_DATE"))
                    entity.UserListUpdatedDate = Convert.ToString(reader["USRLST_UPDATED_DATE"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "SUBSCRIPTION_DETAIL_ID"))
                    entity.SubscriptionDetailId = Convert.ToInt32(reader["SUBSCRIPTION_DETAIL_ID"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "SUBSCRIPTION_ID"))
                    entity.SubscriptionId = Convert.ToInt32(reader["SUBSCRIPTION_ID"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "SUBSCRIPTION_TYPE"))
                    entity.SubscriptionType = Convert.ToString(reader["SUBSCRIPTION_TYPE"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "SUBSCRIPTION_TIME"))
                    entity.SubscriptionTime = Convert.ToString(reader["SUBSCRIPTION_TIME"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "SUBSCRIPTION_ISACTIVE"))
                    entity.SubscriptionIsActive = Convert.ToBoolean(reader["SUBSCRIPTION_ISACTIVE"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "SUBSCRIPTION_USRLST_ID"))
                    entity.SubscriptionUserListId = Convert.ToString(reader["SUBSCRIPTION_USRLST_ID"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "SUBSCRIPTION_EMAILID"))
                    entity.SubscriptionEmail = Convert.ToString(reader["SUBSCRIPTION_EMAILID"]);

                list.Add(entity);
            }

            return list;
        }

    }
}
