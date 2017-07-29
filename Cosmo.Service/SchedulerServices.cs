using System;
using System.Collections.Generic;
using System.Configuration;
using Cosmo.Data;
using Cosmo.Entity;

namespace Cosmo.Service
{
    public class SchedulerServices
    {
        ISchedulerData _schedulerData;
        private static readonly string DbType = ConfigurationManager.AppSettings["DatabaseType"];

        public SchedulerServices()
        {
            var iDbType = DbType == DatabaseType.Oracle.ToString()
                ? Convert.ToInt32(DatabaseType.Oracle).ToString()
                : Convert.ToInt32(DatabaseType.SqlServer).ToString();

            _schedulerData = new SchedulerDataFactory().Create(iDbType);
        }
        /// <summary>
        /// To insert or update the scheduler details
        /// </summary>
        /// <param name="scheduler"></param>
        public int InsertUpdateScheduler(SchedulerEntity scheduler)
        {
            int updateStatus = 0;
            try
            {
                if (scheduler.ConfigID == 0)
                {
                    _schedulerData.InsertUpdateMultipleScheduler(scheduler);
                }
                else if(scheduler.ConfigID > 0)
                {
                    _schedulerData.InsertUpdateScheduler(scheduler);
                }
            }
            catch (Exception ex)
            {
                updateStatus=2;
                Logger.Log(ex.ToString());
            }
            return updateStatus;
        }
        
        /// <summary>
        /// Get the scheduler details using env id and config id
        /// </summary>
        /// <param name="scheduler"></param>
        public SchedulerEntity GetSchedulerDetails(int envID, int configID)
        {
            List<SchedulerEntity> schedulerList = new List<SchedulerEntity>();
            SchedulerEntity scheduler = new SchedulerEntity();
            try
            {
                schedulerList = _schedulerData.GetSchedulerDetails(envID, configID);
                if (schedulerList != null && schedulerList.Count > 0)
                {
                    //scheduler = new SchedulerEntity();
                    scheduler = schedulerList[0];
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex.ToString());                
            }
            return scheduler;
        }
    }
}
