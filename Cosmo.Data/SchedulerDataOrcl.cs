using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Cosmo.Entity;
using Oracle.ManagedDataAccess.Client;

namespace Cosmo.Data
{
    public class SchedulerDataOrcl : BusinessEntityBaseDAO, ISchedulerData
    {
        #region Constant variables
        private const string PackageName = "COSMO_SCHEDULER_PACKAGE.";
        private static readonly string GetEnvironmentid = string.Format("{0}CWT_GetEnvID", PackageName);
        private static readonly string GetSchedulerdetails = string.Format("{0}FN_CWT_GetSchedulerDetails", PackageName);
        private static readonly string SetScheduler = string.Format("{0}SP_CWT_InsUpdScheduler", PackageName);

        #endregion Constant variables

        public void InsertUpdateScheduler(SchedulerEntity scheduler)
        {
            var pList = new List<OracleParameter>
            {
                GetParameter("p_ENV_ID", scheduler.EnvID, OracleDbType.Int32, ParameterDirection.Input),
                GetParameter("p_CONFIG_ID", scheduler.ConfigID, OracleDbType.Int32, ParameterDirection.Input),
                GetParameter("p_SCH_INTERVAL", scheduler.Interval <= 0 ? 0 : scheduler.Interval, OracleDbType.Int32, ParameterDirection.Input),
                GetParameter("p_SCH_DURATION", scheduler.Duration ?? string.Empty, OracleDbType.Varchar2, ParameterDirection.Input),
                GetParameter("p_SCH_REPEATS", scheduler.RepeatsOn ?? string.Empty, OracleDbType.Varchar2, ParameterDirection.Input),
                GetParameter("p_SCH_STARTBY", scheduler.StartDateTime ?? DateTime.Now, OracleDbType.TimeStamp, ParameterDirection.Input),
                GetParameter("p_SCH_ENDAS", scheduler.EndAs ?? string.Empty, OracleDbType.Varchar2, ParameterDirection.Input),
                GetParameter("p_SCH_END_OCCURANCE", scheduler.EndOfOccurance <= 0 ? 0 : scheduler.EndOfOccurance, OracleDbType.Int32, ParameterDirection.Input),
                GetParameter("p_SCH_ENDBY", scheduler.EndDateTime ?? Convert.ToDateTime("1/1/2999"), OracleDbType.TimeStamp, ParameterDirection.Input),
                GetParameter("p_SCH_IS_ACTIVE", scheduler.IsActive ? 1 : 0, OracleDbType.Int32, ParameterDirection.Input),
                GetParameter("p_SCH_CREATED_BY", scheduler.CreatedBy ?? string.Empty, OracleDbType.Varchar2, ParameterDirection.Input),
                GetParameter("p_SCH_CREATED_DATE", scheduler.CreatedDate ?? DateTime.Now, OracleDbType.TimeStamp, ParameterDirection.Input),
                GetParameter("p_SCH_UPDATED_BY", scheduler.CreatedBy ?? string.Empty, OracleDbType.Varchar2, ParameterDirection.Input),
                GetParameter("p_SCH_UPDATED_DATE", scheduler.CreatedDate ?? DateTime.Now, OracleDbType.TimeStamp, ParameterDirection.Input),
                GetParameter("p_SCH_COMMENTS", scheduler.Comments ?? string.Empty, OracleDbType.Varchar2, ParameterDirection.Input),
            };

            ExecuteNonQuery(SetScheduler, pList);
        }

        public void InsertUpdateMultipleScheduler(SchedulerEntity scheduler)
        {
            var eData = new EnvironemntDataOrcl();
            if (scheduler.ConfigID != 0) return;
            var configDetails = eData.GetConfigurationDetails(scheduler.EnvID);
            if (configDetails == null || configDetails.Count <= 0) return;
            foreach (var detail in configDetails)
            {
                scheduler.ConfigID = detail.EnvDetID;
                InsertUpdateScheduler(scheduler);
            }
        }

        public List<SchedulerEntity> GetSchedulerDetails(int envID, int configID)
        {
            var pList = new List<OracleParameter>
            {
                GetParameter("p_ENVID", envID, OracleDbType.Int32, ParameterDirection.Input),
                GetParameter("p_CONFIGID", configID, OracleDbType.Int32, ParameterDirection.Input),
            };
            var scheduleList = ReadCompoundEntityList<SchedulerEntity>(GetSchedulerdetails, pList, RowToSchedulerList);
            return scheduleList;
        }

        private List<SchedulerEntity> RowToSchedulerList(OracleDataReader reader)
        {
            var list = new List<SchedulerEntity>();

            while (reader.Read())
            {
                var entity = new SchedulerEntity();
                if (CommonUtility.IsColumnExistsAndNotNull(reader, "ENV_ID"))
                    entity.EnvID = Convert.ToInt32(reader["ENV_ID"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "SCH_ID"))
                    entity.SchedulerID = Convert.ToInt32(reader["SCH_ID"]);

               if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONGIG_ID"))
                   entity.ConfigID = Convert.ToInt32(reader["CONGIG_ID"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "SCH_INTERVAL")) // frequency
                    entity.Interval = Convert.ToInt32(reader["SCH_INTERVAL"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "SCH_DURATION"))
                    entity.Duration = Convert.ToString(reader["SCH_DURATION"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "SCH_REPEATS"))
                    entity.RepeatsOn = Convert.ToString(reader["SCH_REPEATS"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "SCH_STARTBY"))
                    entity.StartDateTime = Convert.ToDateTime(reader["SCH_STARTBY"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "SCH_ENDAS"))
                    entity.EndAs = Convert.ToString(reader["SCH_ENDAS"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "SCH_END_OCCURANCE"))
                    entity.EndOfOccurance = Convert.ToInt32(reader["SCH_END_OCCURANCE"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "SCH_ENDBY"))
                    entity.EndDateTime = Convert.ToDateTime(reader["SCH_ENDBY"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "SCH_IS_ACTIVE"))
                    entity.IsActive = Convert.ToBoolean(reader["SCH_IS_ACTIVE"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "SCH_COMMENTS"))
                    entity.Comments = Convert.ToString(reader["SCH_COMMENTS"]);

                list.Add(entity);
            }

            return list;
        }
    }
}
