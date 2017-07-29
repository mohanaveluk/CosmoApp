using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using Cosmo.Entity;

namespace Cosmo.Data
{
    public class SchedulerData : ISchedulerData
    {
        #region Constant variables
        private const string GetEnvironmentid = "CWT_GetEnvID";
        private const string GetSchedulerdetails = "CWT_GetSchedulerDetails";
        private const string SetScheduler = "CWT_InsUpdScheduler";//"CWT_InsUpdEnvironment"; earlier it was CWT_InsUpdEnvironment
        #endregion Constant variables

        /// <summary>
        /// To insert or update the scheduler details
        /// </summary>
        /// <param name="scheduler"></param>
        public void InsertUpdateScheduler(SchedulerEntity scheduler)
        {
            try
            {
                List<SqlParameter> pList = new List<SqlParameter>();
                pList.Add(new SqlParameter("@ENV_ID", scheduler.EnvID));
                pList.Add(new SqlParameter("@CONFIG_ID", scheduler.ConfigID));
                pList.Add(new SqlParameter("@SCH_INTERVAL", scheduler.Interval <= 0 ? 0 : scheduler.Interval));
                pList.Add(new SqlParameter("@SCH_DURATION", scheduler.Duration == null ? string.Empty : scheduler.Duration));
                pList.Add(new SqlParameter("@SCH_REPEATS", scheduler.RepeatsOn == null ? string.Empty : scheduler.RepeatsOn));
                pList.Add(new SqlParameter("@SCH_STARTBY", scheduler.StartDateTime == null ? DateTime.Now : scheduler.StartDateTime));
                pList.Add(new SqlParameter("@SCH_ENDAS", scheduler.EndAs == null ? string.Empty : scheduler.EndAs));
                pList.Add(new SqlParameter("@SCH_END_OCCURANCE", scheduler.EndOfOccurance <= 0 ? 0 : scheduler.EndOfOccurance));
                pList.Add(new SqlParameter("@SCH_ENDBY", scheduler.EndDateTime ==null? Convert.ToDateTime("1/1/2999") : scheduler.EndDateTime));
                pList.Add(new SqlParameter("@SCH_IS_ACTIVE", scheduler.IsActive));
                pList.Add(new SqlParameter("@SCH_CREATED_BY", scheduler.CreatedBy == null ? string.Empty : scheduler.CreatedBy));
                pList.Add(new SqlParameter("@SCH_CREATED_DATE", scheduler.CreatedDate == null ? DateTime.Now : scheduler.CreatedDate));
                pList.Add(new SqlParameter("@SCH_UPDATED_BY", scheduler.CreatedBy == null ? string.Empty : scheduler.CreatedBy));
                pList.Add(new SqlParameter("@SCH_UPDATED_DATE", scheduler.CreatedDate == null ? DateTime.Now : scheduler.CreatedDate));
                pList.Add(new SqlParameter("@SCH_COMMENTS", scheduler.Comments == null ? string.Empty : scheduler.Comments));

                UtilityDL.ExecuteNonQuery(SetScheduler, pList);
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
        }

        /// <summary>
        /// Insert update multiple scheduler
        /// </summary>
        /// <param name="scheduler"></param>
        public void InsertUpdateMultipleScheduler(SchedulerEntity scheduler)
        {
            var eData = new EnvironemntData();
            if (scheduler.ConfigID != 0) return;
            var configDetails = eData.GetConfigurationDetails(scheduler.EnvID);
            if (configDetails == null || configDetails.Count <= 0) return;
            foreach (var detail in configDetails)
            {
                scheduler.ConfigID = detail.EnvDetID;
                InsertUpdateScheduler(scheduler);
            }
        }

        /// <summary>
        /// Get the scheduler details using env id and config id
        /// </summary>
        /// <param name="scheduler"></param>
        public List<SchedulerEntity> GetSchedulerDetails(int envID, int configID)
        {
            try
            {
                List<SqlParameter> pList = new List<SqlParameter>();
                pList.Add(new SqlParameter("@ENVID", envID));
                pList.Add(new SqlParameter("@CONFIGID", configID));

                return UtilityDL.FillData<SchedulerEntity>(GetSchedulerdetails, pList);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
