using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Cosmo.Entity;
using log4net.Repository.Hierarchy;

namespace Cosmo.Data
{
    public class ReportData: IReportData
    {
        #region Constant variables

        private const string GetSubscriptionUserEmail = "CWT_GetSubscriptionUserEmail";
        private const string InsUpdSubcription = "CWT_InsUpdSubscription";

        #endregion Constant variables


        public List<DailyReportSubscriptionEntity> GetDailyReportSubscription(int envId)
        {
            try
            {
                var pList = new List<SqlParameter>
                {
                    new SqlParameter("@EnvId", envId),
                };
                return UtilityDL.FillData<DailyReportSubscriptionEntity>(GetSubscriptionUserEmail, pList);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public int InsertUpdateEmailSubscription(EmailSubscription emailSubscription)
        {
            var nextJobRunTime =
                    Convert.ToDateTime(DateTime.Now.ToShortDateString() + " 00:00:00").AddDays(1)
                        .AddHours(Convert.ToInt32(emailSubscription.Time));
            try
            {
                var pList = new List<SqlParameter>
                {
                    new SqlParameter("@SUBSCRIPTION_ID", emailSubscription.Id),
                    new SqlParameter("@SUBSCRIPTION_TYPE", emailSubscription.Type ?? string.Empty),
                    new SqlParameter("@SUBSCRIPTION_TIME", emailSubscription.Time ?? string.Empty),
                    new SqlParameter("@SUBSCRIPTION_ISACTIVE", emailSubscription.IsActive),
                    new SqlParameter("@CREATED_BY", emailSubscription.CreatedBy ?? string.Empty),
                    new SqlParameter("@CREATED_DATE", DateTime.Now),
                    new SqlParameter("@UPDATED_BY", emailSubscription.UpdatedBy ?? string.Empty),
                    new SqlParameter("@UPDATED_DATE", DateTime.Now),
                    new SqlParameter("@NEXTJOBRUNTIME", nextJobRunTime),
                    new SqlParameter("@SUBSCRIPTION_EMAILS", emailSubscription.EmailList ?? string.Empty),
                    new SqlParameter("@SCOPE_OUTPUT", SqlDbType.Int) {Direction = ParameterDirection.Output}
                };

                var recInsert = Convert.ToInt32( UtilityDL.ExecuteNonQuery(InsUpdSubcription, pList,true));
               return recInsert;
            }
            catch (Exception exception)
            {
                throw new ApplicationException(exception.Message);
            }
        }
    }
}
