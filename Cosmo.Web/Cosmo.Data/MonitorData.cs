using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cosmo.Entity;
using System.Data.SqlClient;
using System.Web;

namespace Cosmo.Data
{
    public class MonitorData : IMonitorData
    {
        #region Constant variables
        private const string GET_ENVIRONMENTS = "CWT_GetEnvironmentList";
        private const string GET_MONITORS = "CWT_GetMonitorStatus";                                     //updated on 06/12/15
        private const string GET_MONITORS_WITH_SERVICENAME = "CWT_GetMonitorStatusWithServiceName";     //updated on 06/12/15
        private const string GET_MONITORS_WITH_SERVICENAME_CONFIGID = "CWT_GetMonitorStatusWithServiceName_ConID";     //updated on 06/12/15
        private const string GET_CONFIGURATION_WITH_SERVICENAME = "CWT_GetConfigurationWithServiceName";     ////updated on 06/12/15
        private const string GET_SERVICE_AVAILABILITY = "CWT_GetServiceAvailability";
        private const string GET_SERVICE_DOWNTIME = "CWT_GetServiceAvailabilityForDowntime";
        private const string GET_SERVICE_BUILD_REPORT  = "CWT_GetCurrentBuildReport";
        private const string GET_SERVICE_BUILD_HISTORY_REPORT = "CWT_ReportBuildHistory";

        private const string SET_SERVICE_ACKNOWLEDGE = "CWT_SetServiceAcknowledge";
        private const string INSERT_MAILLOG = "CWT_InsertMailLog";
        private const string SET_GETUSEREMAIL = "CWT_GetUserEmailList";
        private const string GETALLINCIDENT = "CWT_GetAllIncident";
        private const string SETINCIDENTTRACKING = "CWT_InsIncidentTracking";
        private const string GETINCIDENTTRACKING = "CWT_GetIncidentTracking";
        //private const string GET_REPORTSERVICESTATUS = "CWT_GetReportServiceStatus";
        private const string GET_REPORTSERVICESTATUS = "CWT_ReportServiceStatus";
        private const string SET_PERSONALIZE = "CWT_InsUpdPersonalize";
        private const string GET_PERSONALIZE = "CWT_GetPersonaliseSetting";

        private const string AverageUsedSpace = "CWT_GetAverageUsedSpace";
        private const string CpuMemorySpace = "CWT_GetCpuMemorySpace";

        #endregion Constant variables

        #region Get current monitor status all environments

        /// <summary>
        /// Get current monitor status all environments
        /// </summary>
        /// <param name="envId"></param>
        /// <param name="isWithServiceName"></param>
        /// <returns></returns>
        public List<ServiceMoniterEntity> GetAllMonitors(int envId, bool isWithServiceName)
        {
            List<ServiceMoniterEntity> envList = new List<ServiceMoniterEntity>();
            List<MonitorEntity> monitorList = new List<MonitorEntity>();
            List<SqlParameter> pList = new List<SqlParameter>();
            pList.Add(new SqlParameter("@ENV_ID", envId));
            string stProc = GET_ENVIRONMENTS;
            envList = UtilityDL.FillData<ServiceMoniterEntity>(stProc, pList);
            
            if (envList == null || envList.Count <= 0) return envList;
            
            if (!isWithServiceName)
                envList = envList.Where(el => el.IsMonitor == true).ToList();
            foreach (ServiceMoniterEntity entity in envList)
            {
                monitorList = new List<MonitorEntity>();
                monitorList = isWithServiceName ? GetMonitorStatusWithServiceName(entity.EnvID, "e") : getMonitorStatus(entity.EnvID);
                entity.monitorList = monitorList;
            }

            return envList;
        }
        
        #endregion Get current monitor status all environments

        /// <summary>
        /// Get current monitor status by environment
        /// </summary>
        /// <param name="envId"></param>
        /// <returns></returns>
        private List<MonitorEntity> getMonitorStatus(int envId)
        {
            List<SqlParameter> pList = new List<SqlParameter>();
            pList.Add(new SqlParameter("@ENV_ID", envId));
            string stProc = GET_MONITORS;
            return UtilityDL.FillData<MonitorEntity>(stProc, pList);
        }

        /// <summary>
        /// Get current monitor status by environment with Service name
        /// </summary>
        /// <param name="envId"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        //public List<MonitorEntity> GetMonitorStatusWithServiceName(int envId, string type)
        public List<MonitorEntity> GetMonitorStatusWithServiceName(int envId, string type)
        {
            string stProc = string.Empty;
            List<SqlParameter> pList = new List<SqlParameter>();
            if (type == "e")
            {
                pList.Add(new SqlParameter("@ENV_ID", envId));
                stProc = GET_MONITORS_WITH_SERVICENAME;
            }
            else if(type == "c")
            {
                pList.Add(new SqlParameter("@CONFIG_ID", envId));
                stProc = GET_MONITORS_WITH_SERVICENAME_CONFIGID;
            }
            return UtilityDL.FillData<MonitorEntity>(stProc, pList);
        }

        /// <summary>
        /// Get Service availability for all the environments
        /// </summary>
        /// <param name="env_id"></param>
        /// <param name="startTime"></param>
        /// <returns></returns>
        public List<ServiceMoniterEntity> GetServiceAvailability(int env_id, DateTime startTime, DateTime endTime, string sType, bool isWithServiceName)
        {
            List<ServiceMoniterEntity> envList = new List<ServiceMoniterEntity>();
            List<MonitorEntity> monitorList = new List<MonitorEntity>();
            List<SqlParameter> pList = new List<SqlParameter>();
            pList.Add(new SqlParameter("@ENV_ID", env_id));
            string stProc = GET_ENVIRONMENTS;
            envList = UtilityDL.FillData<ServiceMoniterEntity>(stProc, pList);
            if (envList != null && envList.Count > 0)
            {
                if (!isWithServiceName)
                    envList = envList.Where(el => el.IsMonitor == true).ToList();
                foreach (ServiceMoniterEntity entity in envList)
                {
                    monitorList = new List<MonitorEntity>();
                    monitorList = getAvailabilityByEnv(entity.EnvID, startTime,endTime, sType);
                    entity.monitorList = monitorList;
                }
            }
            return envList;
        }

        private List<MonitorEntity> getAvailabilityByEnv(int envId, DateTime startTime, DateTime endTime, string sType)
        {
            List<SqlParameter> pList = new List<SqlParameter>();
            pList.Add(new SqlParameter("@ENV_ID", envId));
            pList.Add(new SqlParameter("@FromTime", startTime));
            pList.Add(new SqlParameter("@ToTime", endTime));
            pList.Add(new SqlParameter("@Type", sType));
            string stProc = GET_SERVICE_AVAILABILITY;
            return UtilityDL.FillData<MonitorEntity>(stProc, pList);
        }

        /// <summary>
        /// Get Service availability for all the environments
        /// </summary>
        /// <param name="env_id"></param>
        /// <param name="startTime"></param>
        /// <returns></returns>
        public List<ServiceMoniterEntity> GetServiceDownTime(int env_id, DateTime startTime, DateTime endTime)
        {
            List<ServiceMoniterEntity> envList = new List<ServiceMoniterEntity>();
            List<MonitorEntity> monitorList = new List<MonitorEntity>();
            List<SqlParameter> pList = new List<SqlParameter>();
            try
            {
                pList.Add(new SqlParameter("@ENV_ID", env_id));
                string stProc = GET_ENVIRONMENTS;
                envList = UtilityDL.FillData<ServiceMoniterEntity>(stProc, pList);
                if (envList != null && envList.Count > 0)
                {
                    foreach (ServiceMoniterEntity entity in envList)
                    {
                        monitorList = new List<MonitorEntity>();
                        monitorList = getServiceDownTimeByEnv(entity.EnvID, startTime, endTime);
                        entity.monitorList = monitorList;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return envList;
        }

        /// <summary>
        /// Get Servce list that are listed as Stopped, Not running or Not ready status os all services for give environment
        /// </summary>
        /// <param name="envId"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        private List<MonitorEntity> getServiceDownTimeByEnv(int envId, DateTime startTime, DateTime endTime)
        {
            List<SqlParameter> pList = new List<SqlParameter>();
            pList.Add(new SqlParameter("@ENV_ID", envId));
            pList.Add(new SqlParameter("@FromTime", startTime));
            pList.Add(new SqlParameter("@ToTime", endTime));
            string stProc = GET_SERVICE_DOWNTIME;
            return UtilityDL.FillData<MonitorEntity>(stProc, pList);
        }

        /// <summary>
        /// Get current build version details for all the services that are available in all the environment
        /// </summary>
        /// <param name="env_id"></param>
        /// <returns></returns>
        public List<ServiceMoniterEntity> GetServiceBuildVersionReport(int env_id)
        {
            List<ServiceMoniterEntity> envList = new List<ServiceMoniterEntity>();
            List<MonitorEntity> monitorList = new List<MonitorEntity>();
            List<SqlParameter> pList = new List<SqlParameter>();
            pList.Add(new SqlParameter("@ENV_ID", env_id));
            string stProc = GET_ENVIRONMENTS;
            envList = UtilityDL.FillData<ServiceMoniterEntity>(stProc, pList);
            if (envList != null && envList.Count > 0)
            {
                foreach (ServiceMoniterEntity entity in envList)
                {
                    monitorList = new List<MonitorEntity>();
                    monitorList = GetServiceBuildVersionReportEnv(entity.EnvID);
                    entity.monitorList = monitorList;
                }
            }
            return envList;
        }

        /// <summary>
        /// Get current build version of all service based on the environment
        /// </summary>
        /// <param name="envId"></param>
        /// <returns></returns>
        private List<MonitorEntity> GetServiceBuildVersionReportEnv(int envId)
        {
            List<SqlParameter> pList = new List<SqlParameter>();
            pList.Add(new SqlParameter("@ENV_ID", envId));
            string stProc = GET_SERVICE_BUILD_REPORT;
            return UtilityDL.FillData<MonitorEntity>(stProc, pList);
        }

        /// <summary>
        /// Get build history version details for all the services that are available in all the environment
        /// </summary>
        /// <param name="env_id"></param>
        /// <returns></returns>
        public List<ServiceMoniterEntity> GetServiceBuildHistoryReport(int env_id, DateTime startDate, DateTime endDate)
        {
            List<ServiceMoniterEntity> envList = new List<ServiceMoniterEntity>();
            List<MonitorEntity> monitorList = new List<MonitorEntity>();
            List<SqlParameter> pList = new List<SqlParameter>();
            pList.Add(new SqlParameter("@ENV_ID", env_id));
            string stProc = GET_ENVIRONMENTS;
            envList = UtilityDL.FillData<ServiceMoniterEntity>(stProc, pList);
            if (envList != null && envList.Count > 0)
            {
                foreach (ServiceMoniterEntity entity in envList)
                {
                    monitorList = new List<MonitorEntity>();
                    monitorList = GetServiceBuildHistoryReportEnv(entity.EnvID, startDate, endDate);
                    entity.monitorList = monitorList;
                }
            }
            return envList;
        }

        /// <summary>
        /// Get build version history of all service based on the environment
        /// </summary>
        /// <param name="envId"></param>
        /// <returns></returns>
        private List<MonitorEntity> GetServiceBuildHistoryReportEnv(int envId, DateTime startDate, DateTime endDate)
        {
            List<SqlParameter> pList = new List<SqlParameter>();
            pList.Add(new SqlParameter("@ENV_ID", envId));
            pList.Add(new SqlParameter("@STARTDATE", startDate));
            pList.Add(new SqlParameter("@ENDDATE", endDate));
            string stProc = GET_SERVICE_BUILD_HISTORY_REPORT;
            return UtilityDL.FillData<MonitorEntity>(stProc, pList);
        }

        #region Get daily service status report (Dasboard)
        /// <summary>
        /// Get daily service status report
        /// </summary>
        /// <param name="env_id"></param>
        /// <returns></returns>
        public List<ServiceMoniterEntity> GetServiceStatusReport(int env_id, DateTime startDate, DateTime endDate) //GET_REPORTSERVICESTATUS
        {
            List<ServiceMoniterEntity> envList = new List<ServiceMoniterEntity>();
            List<MonitorEntity> monitorList = new List<MonitorEntity>();
            List<SqlParameter> pList = new List<SqlParameter>();
            pList.Add(new SqlParameter("@ENV_ID", env_id));
            string stProc = GET_ENVIRONMENTS;
            envList = UtilityDL.FillData<ServiceMoniterEntity>(stProc, pList);
            if (envList != null && envList.Count > 0)
            {
                foreach (ServiceMoniterEntity entity in envList)
                {
                    monitorList = new List<MonitorEntity>();
                    monitorList = GetServiceStatusReportEnv(entity.EnvID, startDate,endDate);
                    entity.monitorList = monitorList;
                }
            }
            return envList;
        }

        /// <summary>
        /// Get current build version of all service based on the environment
        /// </summary>
        /// <param name="envId"></param>
        /// <returns></returns>
        private List<MonitorEntity> GetServiceStatusReportEnv(int envId, DateTime startDate, DateTime endDate)
        {
            List<SqlParameter> pList = new List<SqlParameter>();
            pList.Add(new SqlParameter("@ENV_ID", envId));
            pList.Add(new SqlParameter("@STARTDATE", startDate));
            pList.Add(new SqlParameter("@ENDDATE", endDate));
            string stProc = GET_REPORTSERVICESTATUS;
            return UtilityDL.FillData<MonitorEntity>(stProc, pList);
        }

        #endregion Get daily service status report

        #region insert update ServiceAcknowledge
        /// <summary>
        /// To update the acknowledge and change of email alert type for the service montor
        /// </summary>
        /// <param name="ackData"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public int InsUpdateServiceAcknowledge(AcknowledgeEntity ackData, string mode)
        {
            int recInsert = 0;
            try
            {
                List<SqlParameter> pList = new List<SqlParameter>
                {
                    new SqlParameter("@ENV_ID", ackData.EnvId),
                    new SqlParameter("@CONFIG_ID", ackData.ConfigId),
                    new SqlParameter("@MON_ID", ackData.MonId),
                    new SqlParameter("@ACK_ISACKNOWLEDGE", ackData.IsAcknowledgeMode),
                    new SqlParameter("@ACK_ALERT", ackData.AcknowledgeAlertChange),
                    new SqlParameter("@ACK_COMMENTS", ackData.AcknowledgeComments),
                    new SqlParameter("@CREATED_BY", ackData.CreatedBy),
                    new SqlParameter("@CREATED_DATE", ackData.CreatedDate)
                };


                UtilityDL.ExecuteNonQuery(SET_SERVICE_ACKNOWLEDGE, pList);
                recInsert = 0;
            }
            catch (Exception ex)
            {
                recInsert = 1;
                throw ex;
            }
            return recInsert;
        }
        
        #endregion insert update ServiceAcknowledge

        #region insert mail log
        /// <summary>
        /// Insert mail log details about the monitor service failures
        /// </summary>
        /// <param name="mailLog"></param>
        public void InserrtMailLog(MailLogEntity mailLog)
        {
            HttpContext context = HttpContext.Current;
            try
            {
                List<SqlParameter> pList = new List<SqlParameter>();
                pList.Add(new SqlParameter("@ENV_ID", mailLog.ENV_ID));
                pList.Add(new SqlParameter("@CONFIG_ID", mailLog.Config_ID));
                pList.Add(new SqlParameter("@EMTRAC_TO_ADDRESS", mailLog.To_Address));
                pList.Add(new SqlParameter("@EMTRAC_CC_ADDRESS", mailLog.Cc_Address));
                pList.Add(new SqlParameter("@EMTRAC_BCC_ADDRESS", string.IsNullOrEmpty(mailLog.Bcc_Address) ? string.Empty: mailLog.Bcc_Address));
                pList.Add(new SqlParameter("@EMTRAC_SUBJECT", mailLog.Subject));
                pList.Add(new SqlParameter("@EMTRAC_BODY", mailLog.Body));
                pList.Add(new SqlParameter("@EMTRAC_SEND_STATUS", mailLog.Status));
                pList.Add(new SqlParameter("@EMTRAC_SEND_ERROR", string.IsNullOrEmpty(mailLog.Error) ? string.Empty : mailLog.Error));
                pList.Add(new SqlParameter("@EMTRAC_CONTENT_TYPE", string.IsNullOrEmpty(mailLog.ContentType) ? string.Empty : mailLog.ContentType));
                pList.Add(new SqlParameter("@EMTRAC_CREATED_BY",
                    context.Session != null && context.Session["_LOGGED_USERD_ID"] != null
                        ? context.Session["_LOGGED_USERD_ID"].ToString()
                        : "1"));
                pList.Add(new SqlParameter("@EMTRAC_CREATED_DATE", DateTime.Now));
                pList.Add(new SqlParameter("@EMTRAC_COMMENTS", mailLog.Comments));
                UtilityDL.ExecuteNonQuery(INSERT_MAILLOG, pList);

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        #endregion insert mail log

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

        /// <summary>
        /// To get all the incidents that has occured and which was wither addressed / not
        /// </summary>
        /// <param name="envId"></param>
        /// <returns></returns>
        public List<ServiceMoniterEntity> GetAllIncident(int envId, string type)
        {
            List<ServiceMoniterEntity> envList = new List<ServiceMoniterEntity>();
            List<MonitorEntity> monitorList = new List<MonitorEntity>();
            List<SqlParameter> pList = new List<SqlParameter>();
            pList.Add(new SqlParameter("@ENV_ID", envId));
            string stProc = GET_ENVIRONMENTS;
            envList = UtilityDL.FillData<ServiceMoniterEntity>(stProc, pList);
            
            if (envList != null && envList.Count > 0)
            {
                foreach (ServiceMoniterEntity entity in envList)
                {
                    monitorList = new List<MonitorEntity>();
                    monitorList = GetIncidents(entity.EnvID, type);
                    entity.monitorList = monitorList;
                }
            }
            return envList;
        }


        /// <summary>
        /// Get current monitor status by environment
        /// </summary>
        /// <param name="envId"></param>
        /// <returns></returns>
        public List<MonitorEntity> GetIncidents(int envId, string type)
        {
            List<SqlParameter> pList = new List<SqlParameter>();
            pList.Add(new SqlParameter("@ENV_ID", envId));
            pList.Add(new SqlParameter("@TYPE", type));
            string stProc = GETALLINCIDENT;
            return UtilityDL.FillData<MonitorEntity>(stProc, pList);
        }

        /// <summary>
        /// To insert incident tracking details
        /// </summary>
        /// <param name="ackData"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public int InsIncidentTracking(string monID, string envID, string configID, string issueDesc, string solutionDesc, string userID)
        {
            int recInsert = 0;
            try
            {
                List<SqlParameter> pList = new List<SqlParameter>();
                pList.Add(new SqlParameter("@MON_ID", monID));
                pList.Add(new SqlParameter("@ENV_ID", envID));
                pList.Add(new SqlParameter("@CONFIG_ID", configID));
                pList.Add(new SqlParameter("@TRK_ISSUE", issueDesc));
                pList.Add(new SqlParameter("@TRK_SOLUTION", solutionDesc));
                pList.Add(new SqlParameter("@TRK_CREATED_BY", userID));
                pList.Add(new SqlParameter("@TRK_CREATED_DATE", DateTime.Now));
                pList.Add(new SqlParameter("@TRK_COMMENTS", string.Empty));

                UtilityDL.ExecuteNonQuery(SETINCIDENTTRACKING, pList);
                recInsert = 0;
            }
            catch (Exception ex)
            {
                recInsert = 1;
                throw ex;
            }
            return recInsert;
        }

        /// <summary>
        /// To get all the incidents report that has updated based on the given period
        /// </summary>
        /// <param name="env_id"></param>
        /// <param name="fromTime"></param>
        /// <param name="toTime"></param>
        /// <returns></returns>
        public List<ServiceMoniterEntity> GetIncidentTrackingReport(int env_id, DateTime fromTime, DateTime toTime)
        {
            List<ServiceMoniterEntity> envList = new List<ServiceMoniterEntity>();
            List<MonitorEntity> monitorList = new List<MonitorEntity>();
            List<SqlParameter> pList = new List<SqlParameter>();
            pList.Add(new SqlParameter("@ENV_ID", env_id));
            string stProc = GET_ENVIRONMENTS;
            envList = UtilityDL.FillData<ServiceMoniterEntity>(stProc, pList);

            if (envList != null && envList.Count > 0)
            {
                foreach (ServiceMoniterEntity entity in envList)
                {
                    monitorList = new List<MonitorEntity>();
                    monitorList = GetIncidentTracking(entity.EnvID, fromTime, toTime);
                    entity.monitorList = monitorList;
                }
            }
            return envList;
        }

        /// <summary>
        /// to get incident tracking details based in the given time period
        /// </summary>
        /// <param name="envId"></param>
        /// <param name="fromTime"></param>
        /// <param name="toTime"></param>
        /// <returns></returns>
        public List<MonitorEntity> GetIncidentTracking(int envId, DateTime fromTime, DateTime toTime)
        {
            try
            {
                List<SqlParameter> pList = new List<SqlParameter>();
                pList.Add(new SqlParameter("@ENV_ID", envId));
                pList.Add(new SqlParameter("@FromTime", fromTime));
                pList.Add(new SqlParameter("@ToTime", toTime));
                string stProc = GETINCIDENTTRACKING;
                return UtilityDL.FillData<MonitorEntity>(stProc, pList);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        
        }

        /// <summary>
        /// Get All windows service list with configuration
        /// </summary>
        /// <param name="envID"></param>
        /// <returns></returns>
        public List<WinServiceEntity> GetWindowsService(int envId)
        {
            List<SqlParameter> pList = new List<SqlParameter>();
            pList.Add(new SqlParameter("@ENV_ID", envId));
            string stProc = GET_CONFIGURATION_WITH_SERVICENAME;
            return UtilityDL.FillData<WinServiceEntity>(stProc, pList);
        }

        public int InsUpdPersonalize(PersonalizeEntity personalize)
        {
            int recInsert = 0;
            try
            {
                var pList = new List<SqlParameter>();
                pList.Add(new SqlParameter("@PERS_ID", personalize.ID));
                pList.Add(new SqlParameter("@USER_ID", personalize.UserId));
                pList.Add(new SqlParameter("@PERS_DB_REFRESHTIME", personalize.RefreshTime));
                pList.Add(new SqlParameter("@PERS_ISACTIVE", personalize.IsActive));
                pList.Add(new SqlParameter("@PERS_CREATEDDATE", personalize.CreatedDate));
                pList.Add(new SqlParameter("@PERS_CREATEDBY", personalize.CreatedBy));
                pList.Add(new SqlParameter("@PERS_SORTORDER", personalize.SortOrder));

                UtilityDL.ExecuteNonQuery(SET_PERSONALIZE, pList);
                recInsert = 0;
            }
            catch (Exception ex)
            {
                recInsert = 1;
                throw ex;
            }
            return recInsert;
        }

        public List<PersonalizeEntity> GetPersonalize(int userId)
        {
            List<SqlParameter> pList = new List<SqlParameter>();
            pList.Add(new SqlParameter("@User_ID", userId));
            string stProc = GET_PERSONALIZE;
            return UtilityDL.FillData<PersonalizeEntity>(stProc, pList);
        }

        public List<ServerDriveSpace> GetAverageUsedSpace(string host)
        {
            var pList = new List<SqlParameter> {new SqlParameter("@HOSTIP", host)};
            return UtilityDL.FillData<ServerDriveSpace>(AverageUsedSpace, pList);
        }

        public List<ServerCpuUsage> GetCpuMemorySpace(string host)
        {
            var pList = new List<SqlParameter>
            {
                new SqlParameter("@HOSTIP", host)
            };
            return UtilityDL.FillData<ServerCpuUsage>(CpuMemorySpace, pList);
        }
    }
}
