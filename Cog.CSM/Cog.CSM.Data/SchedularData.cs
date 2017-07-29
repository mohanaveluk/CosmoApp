using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Cog.CSM.Entity;
using System.Configuration;
using System.Data.Sql;

namespace Cog.CSM.Data
{
    internal class ConnectionInfo
    {
        internal static string connectionString = System.Configuration.ConfigurationManager.AppSettings["ConnectionString"];
    }

    /// <summary>
    /// Summary description for JobAccess.
    /// </summary>
    public class SchedularData: ISchedularData
    {

        #region Constant variables
        private const string GET_ENVIRONMENTS = "CWT_GetEnvironmentList";
        private const string GET_MONITORS = "CWT_GetMonitorStatus";                                     //updated on 06/12/15
        private const string GET_MONITORS_WITH_SERVICENAME = "CWT_GetMonitorStatusWithServiceName";     //updated on 06/12/15
        private const string GET_MONITORS_WITH_SERVICENAME_CONFIGID = "CWT_GetMonitorStatusWithServiceName_ConID";     //updated on 06/12/15

        private const string GET_SCHEDULES = "CWT_GetAllSchedule_NextJobStartBefore";
        private const string GET_ALLSCHEDULEDSERVICES = "CWT_GetAllScheduledServices";
        private const string SET_SCHEDULERLASTRUNDATETIME = "CWT_UpdateSchedulerLastRunDateTime";
        private const string SET_GETUSEREMAIL = "CWT_getUserEmailList";

        private const string GET_SUBSCRIPTION = "CWT_GetSubscription";
        private const string GetSubscriptiondetail = "CWT_GetSubscriptionDetail";
        private const string SetSubscriptionnextjobRuntime = "CWT_SetSubscriptionNextJobRunTime";
        private const string Get_SubscriptionMonitorStatus = "CWT_GetSubscriptionMonitorStatus";

        private const string GetPortals = "CWT_GetPortelToMonitor";
        private const string SetPortals = "CWT_InsertPortalStatus";
        private const string SetPortalnextjonruntime = "CWT_SetPortalNextJobRunTime";

        #endregion

        public SchedularData()
        {
            ///Constructor
        }

        //UtilityDL utilityDL = new UtilityDL();

        /// <summary>
        /// Get all the schedules avaialbe based on the time since last job run
        /// </summary>
        /// <param name="beforeDateTime"></param>
        /// <returns></returns>
        public List<Scheduler_details> GetSchedules(DateTime beforeDateTime)
        {
            List<SqlParameter> pList = new List<SqlParameter>();
            pList.Add(new SqlParameter("@DateNextJobRunStartBefore", beforeDateTime));
            string stProc = GET_SCHEDULES;
            return UtilityDL.FillData<Scheduler_details>(stProc, pList);
        }

        public List<ServiceEntity> GetAllScheduledServices(DateTime beforeDateTime)
        {
            List<SqlParameter> pList = new List<SqlParameter>();
            pList.Add(new SqlParameter("@DateNextJobRunStartBefore", beforeDateTime));
            string stProc = GET_ALLSCHEDULEDSERVICES;
            return UtilityDL.FillData<ServiceEntity>(stProc, pList);
        }

        public void UPdateSchedule(int schedulerId, DateTime dateLastJobRan)
        {
            List<SqlParameter> pList = new List<SqlParameter>
            {
                new SqlParameter("@SchedulerID", schedulerId),
                new SqlParameter("@DateLastJobRun", dateLastJobRan)
            };
            string stProc = SET_SCHEDULERLASTRUNDATETIME;
            UtilityDL.ExecuteNonQuery(stProc, pList);
        }

        #region getUserEmailList

        /// <summary>
        /// Get user email list with respect to the environment id
        /// </summary>
        /// <param name="envId"></param>
        /// <param name="messageType"></param>
        /// <returns></returns>
        public List<UserEmailEntity> GetUserEMail(int envId, string messageType)
        {
            List<SqlParameter> pList = new List<SqlParameter>();
            pList.Add(new SqlParameter("@Env_ID", envId));
            pList.Add(new SqlParameter("@MessageType", messageType));
            string stProc = SET_GETUSEREMAIL;
            return UtilityDL.FillData<UserEmailEntity>(stProc, pList);
        }
        #endregion getUserEmailList

        #region Get Subscription

        public List<Subscription> GetSubscription(int id)
        {
            var pList = new List<SqlParameter>
            {
                new SqlParameter("@ID", id)
            };
            return UtilityDL.FillData<Subscription>(GET_SUBSCRIPTION, pList);
        }

        public List<SubscriptionDetail> GetSubscriptionDetails(int id)
        {
            var pList = new List<SqlParameter>
            {
                new SqlParameter("@ID", id)
            };
            return UtilityDL.FillData<SubscriptionDetail>(GetSubscriptiondetail, pList);
        }
        #endregion Get Subscription

        public void SetNextJobRunTime(Subscription subscription)
        {
            try
            {
                var nextJobRunTime =
                    Convert.ToDateTime(DateTime.Now.ToShortDateString() + " 00:00:00").AddDays(1)
                        .AddHours(Convert.ToInt32(subscription.Time));

                var pList = new List<SqlParameter>
                {
                    new SqlParameter("@ID", subscription.Id),
                    new SqlParameter("@LASTJOBRANTIME", DateTime.Now),
                    new SqlParameter("@NEXTJOBRUNTIME", nextJobRunTime),
                };
                UtilityDL.ExecuteNonQuery(SetSubscriptionnextjobRuntime, pList);
            }
            catch (Exception exception)
            {
                throw;
            }
        }

        public void SetPortalNextJobRunTime(Portal portal)
        {
            try
            {
                var nextJobRunTime = DateTime.Now.AddMinutes(portal.Interval);

                var pList = new List<SqlParameter>
                {
                    new SqlParameter("@ID", portal.Id),
                    new SqlParameter("@LASTJOBRANTIME", DateTime.Now),
                    new SqlParameter("@NEXTJOBRUNTIME", nextJobRunTime),
                };
                UtilityDL.ExecuteNonQuery(SetPortalnextjonruntime, pList);
            }
            catch (Exception exception)
            {
                throw;
            }
        }

        public void InsertPortalStatus(CognosCgiResponse response)
        {
            try
            {
                var pList = new List<SqlParameter>
                {
                    new SqlParameter("@URL_ID", response.UrlId),
                    new SqlParameter("@ENV_ID", response.EnvId),
                    new SqlParameter("@PMON_STATUS", !string.IsNullOrEmpty(response.Status) ? response.Status: string.Empty),
                    new SqlParameter("@PMON_MESSAGE", !string.IsNullOrEmpty(response.Message) ? response.Message:string.Empty),
                    new SqlParameter("@PMON_RESPONSETIME", response.ResponseTime > 0 ? response.ResponseTime: 0),
                    new SqlParameter("@PMON_EXCEPTION", !string.IsNullOrEmpty(response.Exception) ? response.Exception: string.Empty),
                };
                UtilityDL.ExecuteNonQuery(SetPortals, pList);
            }
            catch (Exception exception)
            {
                throw;
            }
        }

        #region Get current monitor status all environments


        public List<Portal> GetPortalToMonitor(int id)
        {
            var pList = new List<SqlParameter>
            {
                new SqlParameter("@ID", id)
            };
            return UtilityDL.FillData<Portal>(GetPortals, pList);
        }


        /// <summary>
        /// Get current monitor status all environments
        /// </summary>
        /// <param name="env_id"></param>
        /// <param name="envId"></param>
        /// <param name="isWithServiceName"></param>
        /// <returns></returns>
        public List<ServiceMoniterEntity> GetAllMonitors(int envId, bool isWithServiceName)
        {
            var pList = new List<SqlParameter> {new SqlParameter("@ENV_ID", envId)};
            string stProc = GET_ENVIRONMENTS;
            var envList = UtilityDL.FillData<ServiceMoniterEntity>(stProc, pList);

            if (envList == null || envList.Count <= 0) return envList;

            if (!isWithServiceName)
                //envList = envList.Where(el => el.IsMonitor == true).ToList();
            foreach (ServiceMoniterEntity entity in envList)
            {
                var monitorList = isWithServiceName ? GetMonitorStatusWithServiceName(entity.EnvID, "e") : GetMonitorStatus(entity.EnvID);
                entity.monitorList = monitorList;
            }

            return envList;
        }

        /// <summary>
        /// Get current monitor status by environment
        /// </summary>
        /// <param name="envId"></param>
        /// <returns></returns>
        private List<MonitorEntity> GetMonitorStatus(int envId)
        {
            var pList = new List<SqlParameter> {new SqlParameter("@ENV_ID", envId)};
            string stProc = GET_MONITORS;
            return UtilityDL.FillData<MonitorEntity>(stProc, pList);
        }

        /// <summary>
        /// Get current monitor status by environment with Service name
        /// </summary>
        /// <param name="envId"></param>
        /// <returns></returns>
        private List<MonitorEntity> GetMonitorStatusWithServiceName(int envId, string type)
        {
            var stProc = String.Empty;
            var pList = new List<SqlParameter>();
            if (type == "e")
            {
                pList.Add(new SqlParameter("@ENV_ID", envId));
                stProc = GET_MONITORS_WITH_SERVICENAME;
            }
            else if (type == "c")
            {
                pList.Add(new SqlParameter("@CONFIG_ID", envId));
                stProc = GET_MONITORS_WITH_SERVICENAME_CONFIGID;
            }
            return UtilityDL.FillData<MonitorEntity>(stProc, pList);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="envId"></param>
        /// <param name="type"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public List<ServiceMoniterEntity> GetSubscriptionStatusList(int envId, Subscription subscription)
        {
            var pList = new List<SqlParameter> { new SqlParameter("@ENV_ID", envId) };

            var envList = UtilityDL.FillData<ServiceMoniterEntity>(GET_ENVIRONMENTS, pList);

            if (envList == null || envList.Count <= 0) return envList;

            foreach (ServiceMoniterEntity entity in envList)
            {
                var monitorList = GetSubscriptionMonitorStatus(entity.EnvID, subscription);
                entity.monitorList = monitorList;
            }

            return envList;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="envId"></param>
        /// <param name="type"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        private List<MonitorEntity> GetSubscriptionMonitorStatus(int envId, Subscription subscription)
        {
            var pList = new List<SqlParameter>
            {
                new SqlParameter("@ENV_ID", envId),
                new SqlParameter("@TYPE", subscription.Type),
                new SqlParameter("@STARTTIME", subscription.StartTime),
                new SqlParameter("@ENDTIME", subscription.EndTime),
            };
            return UtilityDL.FillData<MonitorEntity>(Get_SubscriptionMonitorStatus, pList);
        }

        #endregion Get current monitor status all environments
    }
}
