using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web;
using Cosmo.Entity;
using Oracle.ManagedDataAccess.Client;

namespace Cosmo.Data
{
    public class MonitorDataOrcl : BusinessEntityBaseDAO, IMonitorData      
    {
        #region Constant variables

        private const string PackageName = "COSMO_MONITOR_PACKAGE.";
        private const string UserPackageName = "COSMO_USER_PACKAGE.";
        private const string ReportPackageName = "COSMO_REPORT_PACKAGE.";
        private const string PackageNameEnvironment = "COSMO_ENVIRONMENT_PACKAGE.";
        private static readonly string GetEnvironments = string.Format("{0}FN_CWT_GetEnvironmentList", PackageNameEnvironment);
        private static readonly string GetMonitors = string.Format("{0}FN_CWT_GetMonitorStatus", PackageName);
        private static readonly string GetMonitorsWithServicename = string.Format("{0}FN_CWT_GetMonStatusWithSName", PackageName);
        private static readonly string GetMonitorsWithServicenameConfigid = string.Format("{0}FN_CWT_GetMonStatusWithSN_CID", PackageName);
        private static readonly string GetConfigurationWithServiceName = string.Format("{0}FN_CWT_GetConfigServiceName", PackageName); //CWT_GetConfigurationWithServiceName
        private static readonly string GetPersonalizeSetting = string.Format("{0}FN_CWT_GetPersonaliseSetting", UserPackageName);

        private static readonly string GetFnServiceAvailability = string.Format("{0}FN_CWT_GetServiceAvailability", PackageName);
        private static readonly string GetFnServiceDownTime = string.Format("{0}FN_CWT_GetServiceAvailDowntime", PackageName);


        private static readonly string GetAllIncidents = string.Format("{0}FN_CWT_GetAllIncident", PackageName);
        private static readonly string GetUserEmailList = string.Format("{0}FN_CWT_GetUserEmailList", PackageName);
        private static readonly string FnGetIncidentTracking = string.Format("{0}FN_CWT_GetIncidentTracking", PackageName);

        private static readonly string SetSpServiceAcknowledge = string.Format("{0}SP_CWT_SetServiceAcknowledge", PackageName);
        private static readonly string SetSpMailLog = string.Format("{0}SP_CWT_InsertMailLog", PackageName);
        private static readonly string SetInsIncidentTracking = string.Format("{0}SP_CWT_InsIncidentTracking", PackageName);
        private static readonly string SetPersonalizeSetting = string.Format("{0}SP_CWT_InsUpdPersonalize", PackageName);

        private static readonly string AverageUsedSpace = string.Format("{0}FN_CWT_GetAverageUsedSpace", PackageName);
        private static readonly string CpuMemorySpace = string.Format("{0}FN_CWT_GetCpuMemorySpace", PackageName);

        //Reports
        private static readonly string GetFnCurrentBuildReport = string.Format("{0}FN_CWT_GetCurrentBuildReport", PackageName);
        private static readonly string FnGetReportBuildHistory = string.Format("{0}FN_CWT_ReportBuildHistory", ReportPackageName);
        private static readonly string FnGetReportServiceStatus = string.Format("{0}FN_CWT_ReportServiceStatus", ReportPackageName);


        #endregion Constant variables

        public List<ServiceMoniterEntity> GetAllMonitors(int envId, bool isWithServiceName)
        {
            var envList = new List<ServiceMoniterEntity>();
            var monitorList = new List<MonitorEntity>();

            var pList = new List<OracleParameter>
            {
                GetParameter("p_ENV_ID", envId, OracleDbType.Int32, ParameterDirection.Input),
            };
            envList = ReadCompoundEntityList<ServiceMoniterEntity>(GetEnvironments, pList, RowToEnvironmentList);

            if (envList == null || envList.Count <= 0) return envList;

            if (!isWithServiceName)
                envList = envList.Where(el => el.IsMonitor == true).ToList();

            foreach (ServiceMoniterEntity entity in envList)
            {
                monitorList = new List<MonitorEntity>();
                monitorList = isWithServiceName ? GetMonitorStatusWithServiceName(entity.EnvID, "e") : GetMonitorStatus(entity.EnvID);
                entity.monitorList = monitorList;
            }

            return envList;
        }
        
        public List<ServiceMoniterEntity> GetAllIncident(int envId, string type)
        {
            var pList = new List<OracleParameter>
            {
                GetParameter("p_ENV_ID", envId, OracleDbType.Int32, ParameterDirection.Input),
            };
            var envList = ReadCompoundEntityList<ServiceMoniterEntity>(GetEnvironments, pList, RowToEnvironmentList);

            if (envList == null || envList.Count <= 0) return envList;
            foreach (ServiceMoniterEntity entity in envList)
            {
                var monitorList = GetIncidents(entity.EnvID, type);
                entity.monitorList = monitorList;
            }

            return envList;
        }

        public List<MonitorEntity> GetMonitorStatusWithServiceName(int envId, string type)
        {
            var stProc = string.Empty;
            var pList = new List<OracleParameter>();
            if (type == "e")
            {
                pList.Add(GetParameter("p_ENV_ID", envId, OracleDbType.Int32));
                stProc = GetMonitorsWithServicename;
            }
            else if (type == "c")
            {
                pList.Add(GetParameter("p_CONFIG_ID",envId,OracleDbType.Int32));
                stProc = GetMonitorsWithServicenameConfigid;
            }

            var list = ReadCompoundEntityList<MonitorEntity>(stProc, pList, RowToMonitorList);
            return list;
        }

        public List<ServiceMoniterEntity> GetServiceAvailability(int env_id, DateTime startTime, DateTime endTime, string sType, bool isWithServiceName)
        {
            var pList = new List<OracleParameter>
            {
                GetParameter("p_ENV_ID", env_id, OracleDbType.Int32, ParameterDirection.Input),
            };
            var envList = ReadCompoundEntityList<ServiceMoniterEntity>(GetEnvironments, pList, RowToEnvironmentList);

            if (envList == null || envList.Count <= 0) return envList;
            foreach (ServiceMoniterEntity entity in envList)
            {
                var monitorList = GetAvailabilityByEnv(entity.EnvID, startTime, endTime, sType);
                entity.monitorList = monitorList;
            }

            return envList;
        }

        private List<MonitorEntity> GetAvailabilityByEnv(int envId, DateTime startTime, DateTime endTime, string sType)
        {
            var pList = new List<OracleParameter>
            {
                GetParameter("p_ENV_ID", envId, OracleDbType.Int32, ParameterDirection.Input),
                GetParameter("p_FromTime", startTime, OracleDbType.TimeStamp, ParameterDirection.Input),
                GetParameter("p_ToTime", endTime, OracleDbType.TimeStamp, ParameterDirection.Input),
                GetParameter("p_Type", sType, OracleDbType.Varchar2, ParameterDirection.Input),
            };
            
            var serviceAvailability = ReadCompoundEntityList<MonitorEntity>(GetFnServiceAvailability, pList, RowToMonitorList);
            return serviceAvailability;
        }

        public List<ServiceMoniterEntity> GetServiceDownTime(int env_id, DateTime startTime, DateTime endTime)
        {
            var pList = new List<OracleParameter>
            {
                GetParameter("p_ENV_ID", env_id, OracleDbType.Int32, ParameterDirection.Input),
            };
            var envList = ReadCompoundEntityList<ServiceMoniterEntity>(GetEnvironments, pList, RowToEnvironmentList);

            if (envList == null || envList.Count <= 0) return envList;
            foreach (ServiceMoniterEntity entity in envList)
            {
                var monitorList = GetServiceDownTimeByEnv(entity.EnvID, startTime, endTime);
                entity.monitorList = monitorList;
            }

            return envList;
        }

        private List<MonitorEntity> GetServiceDownTimeByEnv(int envId, DateTime startTime, DateTime endTime)
        {
            var pList = new List<OracleParameter>
            {
                GetParameter("p_ENV_ID", envId, OracleDbType.Int32, ParameterDirection.Input),
                GetParameter("p_FromTime", startTime, OracleDbType.TimeStamp, ParameterDirection.Input),
                GetParameter("p_ToTime", endTime, OracleDbType.TimeStamp, ParameterDirection.Input),
            };

            var serviceDownTime = ReadCompoundEntityList<MonitorEntity>(GetFnServiceDownTime, pList, RowToMonitorList);
            return serviceDownTime;
        }
        
        public List<ServiceMoniterEntity> GetServiceBuildVersionReport(int env_id)
        {
            var pList = new List<OracleParameter>
            {
                GetParameter("p_ENV_ID", env_id, OracleDbType.Int32, ParameterDirection.Input),
            };
            var envList = ReadCompoundEntityList<ServiceMoniterEntity>(GetEnvironments, pList, RowToEnvironmentList);

            if (envList == null || envList.Count <= 0) return envList;
            foreach (ServiceMoniterEntity entity in envList)
            {
                var monitorList = GetServiceBuildVersionReportEnv(entity.EnvID);
                entity.monitorList = monitorList;
            }

            return envList;
        }

        private List<MonitorEntity> GetServiceBuildVersionReportEnv(int envId)
        {
            var pList = new List<OracleParameter>
            {
                GetParameter("p_ENV_ID", envId, OracleDbType.Int32, ParameterDirection.Input),
            };
            var envList = ReadCompoundEntityList<MonitorEntity>(GetFnCurrentBuildReport, pList, RowToMonitorList);

            return envList;
        }

        public List<ServiceMoniterEntity> GetServiceBuildHistoryReport(int env_id, DateTime startDate, DateTime endDate)
        {
            var pList = new List<OracleParameter>
            {
                GetParameter("p_ENV_ID", env_id, OracleDbType.Int32, ParameterDirection.Input),
            };
            var envList = ReadCompoundEntityList<ServiceMoniterEntity>(GetEnvironments, pList, RowToEnvironmentList);

            if (envList == null || envList.Count <= 0) return envList;

            foreach (ServiceMoniterEntity entity in envList)
            {
                var monitorList = GetServiceBuildHistoryReportEnv(entity.EnvID, startDate, endDate);
                entity.monitorList = monitorList;
            }

            return envList;
        }

        public List<MonitorEntity> GetServiceBuildHistoryReportEnv(int envId, DateTime startDate, DateTime endDate)
        {
            var pList = new List<OracleParameter>
            {
                GetParameter("p_ENV_ID", envId, OracleDbType.Int32, ParameterDirection.Input),
                GetParameter("p_STARTDATE", startDate, OracleDbType.TimeStamp, ParameterDirection.Input),
                GetParameter("p_ENDDATE", endDate, OracleDbType.TimeStamp, ParameterDirection.Input),
            };
            var envList = ReadCompoundEntityList<MonitorEntity>(FnGetReportBuildHistory, pList, RowToMonitorList);

            return envList;
            
        }

        public List<ServiceMoniterEntity> GetServiceStatusReport(int env_id, DateTime startDate, DateTime endDate)
        {
            var pList = new List<OracleParameter>
            {
                GetParameter("p_ENV_ID", env_id, OracleDbType.Int32, ParameterDirection.Input),
            };
            var envList = ReadCompoundEntityList<ServiceMoniterEntity>(GetEnvironments, pList, RowToEnvironmentList);

            if (envList == null || envList.Count <= 0) return envList;

            foreach (ServiceMoniterEntity entity in envList)
            {
                var monitorList = GetServiceStatusReportEnv(entity.EnvID, startDate, endDate);
                entity.monitorList = monitorList;
            }

            return envList;
        }

        private List<MonitorEntity> GetServiceStatusReportEnv(int envId, DateTime startDate, DateTime endDate)
        {
            var pList = new List<OracleParameter>
            {
                GetParameter("p_ENV_ID", envId, OracleDbType.Int32, ParameterDirection.Input),
                GetParameter("p_STARTDATE", startDate, OracleDbType.TimeStamp, ParameterDirection.Input),
                GetParameter("p_ENDDATE", endDate, OracleDbType.TimeStamp, ParameterDirection.Input),
            };
            var envList = ReadCompoundEntityList<MonitorEntity>(FnGetReportServiceStatus, pList, RowToMonitorList);

            return envList;
        }


        public int InsUpdateServiceAcknowledge(AcknowledgeEntity ackData, string mode)
        {
            int recInsert = 0;
            try
            {
                var pList = new List<OracleParameter>
                {
                    GetParameter("p_ENV_ID", ackData.EnvId, OracleDbType.Int32, ParameterDirection.Input),
                    GetParameter("p_CONFIG_ID", ackData.ConfigId, OracleDbType.Int32, ParameterDirection.Input),
                    GetParameter("p_MON_ID", ackData.MonId, OracleDbType.Int32, ParameterDirection.Input),
                    GetParameter("p_ACK_ISACKNOWLEDGE", ackData.IsAcknowledgeMode ? 1 : 0, OracleDbType.Int32,
                        ParameterDirection.Input),
                    GetParameter("p_ACK_ALERT", ackData.AcknowledgeAlertChange, OracleDbType.Varchar2,
                        ParameterDirection.Input),
                    GetParameter("p_ACK_COMMENTS", ackData.AcknowledgeComments, OracleDbType.Varchar2,
                        ParameterDirection.Input),
                    GetParameter("p_CREATED_BY", ackData.CreatedBy, OracleDbType.Varchar2, ParameterDirection.Input),
                    GetParameter("p_CREATED_DATE", ackData.CreatedDate, OracleDbType.TimeStamp, ParameterDirection.Input),
                };
                ExecuteNonQuery(SetSpServiceAcknowledge, pList);
                 recInsert = 0;

            }
            catch (Exception)
            {
                recInsert = 1;
            }

            return recInsert;
        }

        public void InserrtMailLog(MailLogEntity mailLog)
        {
            var context = HttpContext.Current;

            var pList = new List<OracleParameter>
            {
                GetParameter("p_ENV_ID", mailLog.ENV_ID, OracleDbType.Int32, ParameterDirection.Input),
                GetParameter("p_CONFIG_ID", mailLog.Config_ID, OracleDbType.Int32, ParameterDirection.Input),
                GetParameter("p_EMTRAC_TO_ADDRESS", mailLog.To_Address, OracleDbType.Varchar2, ParameterDirection.Input),
                GetParameter("p_EMTRAC_CC_ADDRESS", mailLog.Cc_Address, OracleDbType.Varchar2, ParameterDirection.Input),
                GetParameter("p_EMTRAC_BCC_ADDRESS", mailLog.Bcc_Address, OracleDbType.Varchar2,
                    ParameterDirection.Input),
                GetParameter("p_EMTRAC_SUBJECT", mailLog.Subject, OracleDbType.Varchar2, ParameterDirection.Input),
                GetParameter("p_EMTRAC_BODY", mailLog.Body, OracleDbType.Varchar2, ParameterDirection.Input),
                GetParameter("p_EMTRAC_SEND_STATUS", mailLog.Status, OracleDbType.Varchar2, ParameterDirection.Input),
                GetParameter("p_EMTRAC_SEND_ERROR", mailLog.Error, OracleDbType.Varchar2, ParameterDirection.Input),
                GetParameter("p_EMTRAC_CONTENT_TYPE", mailLog.ContentType, OracleDbType.Varchar2,
                    ParameterDirection.Input),
                GetParameter("p_EMTRAC_CREATED_BY",
                    context.Session != null && context.Session["_LOGGED_USERD_ID"] != null
                        ? context.Session["_LOGGED_USERD_ID"].ToString()
                        : "1", OracleDbType.Varchar2, ParameterDirection.Input),
                GetParameter("p_EMTRAC_CREATED_DATE", mailLog.CreatedDate, OracleDbType.TimeStamp,
                    ParameterDirection.Input),
                GetParameter("p_EMTRAC_COMMENTS", mailLog.Comments, OracleDbType.Varchar2, ParameterDirection.Input),
            };

            ExecuteNonQuery(SetSpMailLog, pList);
        }

        public List<MonitorEntity> GetIncidents(int envId, string type)
        {
            var pList = new List<OracleParameter>
            {
                GetParameter("p_ENV_ID", envId, OracleDbType.Int32, ParameterDirection.Input),
                GetParameter("p_Type", type, OracleDbType.Varchar2, ParameterDirection.Input),
            };
            var incidentList = ReadCompoundEntityList<MonitorEntity>(GetAllIncidents, pList, RowToMonitorList);

            return incidentList;
        }

        public int InsIncidentTracking(string monID, string envID, string configID, string issueDesc,
            string solutionDesc,
            string userID)
        {
            int recInsert = 0;
            try
            {
                var pList = new List<OracleParameter>
                {
                    GetParameter("p_MON_ID", monID, OracleDbType.Int32, ParameterDirection.Input),
                    GetParameter("p_ENV_ID", envID, OracleDbType.Int32, ParameterDirection.Input),
                    GetParameter("p_CONFIG_ID", configID, OracleDbType.Int32, ParameterDirection.Input),
                    GetParameter("p_TRK_ISSUE", issueDesc, OracleDbType.Varchar2, ParameterDirection.Input),
                    GetParameter("p_TRK_SOLUTION", solutionDesc, OracleDbType.Varchar2, ParameterDirection.Input),
                    GetParameter("p_TRK_CREATED_BY", userID, OracleDbType.Varchar2, ParameterDirection.Input),
                    GetParameter("p_TRK_CREATED_DATE", DateTime.Now, OracleDbType.TimeStamp, ParameterDirection.Input),
                    GetParameter("p_TRK_COMMENTS", string.Empty, OracleDbType.Varchar2, ParameterDirection.Input),
                };

                ExecuteNonQuery(SetInsIncidentTracking, pList);

                recInsert = 0;
            }
            catch (Exception)
            {
                recInsert = 1;
                throw;
            }
            return recInsert;
        }

        public List<ServiceMoniterEntity> GetIncidentTrackingReport(int env_id, DateTime fromTime, DateTime toTime)
        {
            var pList = new List<OracleParameter>
            {
                GetParameter("p_ENV_ID", env_id, OracleDbType.Int32, ParameterDirection.Input),
            };
            var envList = ReadCompoundEntityList<ServiceMoniterEntity>(GetEnvironments, pList, RowToEnvironmentList);

            if (envList == null || envList.Count <= 0) return envList;

            foreach (ServiceMoniterEntity entity in envList)
            {
                var monitorList = GetIncidentTracking(entity.EnvID, fromTime, toTime);
                entity.monitorList = monitorList;
            }

            return envList;
        }

        public List<MonitorEntity> GetIncidentTracking(int envId, DateTime fromTime, DateTime toTime)
        {
            
            var pList = new List<OracleParameter>
            {
                GetParameter("p_ENV_ID", envId, OracleDbType.Int32, ParameterDirection.Input),
                GetParameter("p_FromTime", fromTime, OracleDbType.TimeStamp, ParameterDirection.Input),
                GetParameter("p_ToTime", toTime, OracleDbType.TimeStamp, ParameterDirection.Input),
            };
            var envList = ReadCompoundEntityList<MonitorEntity>(FnGetIncidentTracking, pList, RowToMonitorList);

            return envList;
        }

        public List<WinServiceEntity> GetWindowsService(int envId)
        {
            var pList = new List<OracleParameter>
            {
                GetParameter("p_ENV_ID", envId, OracleDbType.Int32, ParameterDirection.Input),
            };
            var envList = ReadCompoundEntityList<WinServiceEntity>(GetConfigurationWithServiceName, pList, RowToWinServiceList);

            return envList;
        }

        public int InsUpdPersonalize(PersonalizeEntity personalize)
        {
            int recInsert = 0;
            try
            {
                var pList = new List<OracleParameter>
                {
                    GetParameter("p_PERS_ID", personalize.ID, OracleDbType.Int32, ParameterDirection.Input),
                    GetParameter("p_USER_ID", personalize.UserId, OracleDbType.Int32, ParameterDirection.Input),
                    GetParameter("p_PERS_DB_REFRESHTIME", personalize.RefreshTime, OracleDbType.Int32,
                        ParameterDirection.Input),
                    GetParameter("p_PERS_ISACTIVE", personalize.IsActive ? 1 : 0, OracleDbType.Int32,
                        ParameterDirection.Input),
                    GetParameter("p_PERS_CREATEDDATE", personalize.CreatedDate, OracleDbType.TimeStamp,
                        ParameterDirection.Input),
                    GetParameter("p_PERS_CREATEDBY", personalize.CreatedBy, OracleDbType.Varchar2,
                        ParameterDirection.Input),
                    GetParameter("p_PERS_SORTORDER", personalize.SortOrder, OracleDbType.Varchar2,
                        ParameterDirection.Input),
                };

                ExecuteNonQuery(SetPersonalizeSetting, pList);
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
            var pList = new List<OracleParameter>
            {
                GetParameter("p_ENV_ID", userId, OracleDbType.Int32, ParameterDirection.Input),
            };

            var list = ReadCompoundEntityList<PersonalizeEntity>(GetPersonalizeSetting, pList, RowToPersonalizeList);
            return list;
        }

        public List<ServerDriveSpace> GetAverageUsedSpace(string host)
        {
            var pList = new List<OracleParameter>
            {
                GetParameter("p_HOSTIP", host, OracleDbType.Varchar2, ParameterDirection.Input),
            };
            var driveList = ReadCompoundEntityList<ServerDriveSpace>(AverageUsedSpace, pList, RowToDriveSpceList);

            return driveList;
        }

        public List<ServerCpuUsage> GetCpuMemorySpace(string host)
        {
            var pList = new List<OracleParameter>
            {
                GetParameter("p_HOSTIP", host, OracleDbType.Varchar2, ParameterDirection.Input),
            };
            var driveList = ReadCompoundEntityList<ServerCpuUsage>(CpuMemorySpace, pList, RowToCpuUsageList);

            return driveList;
        }
        

        public List<UserEmailEntity> GetUserEMail(int envId, string messageType)
        {
            var pList = new List<OracleParameter>
            {
                GetParameter("p_ENV_ID",envId,OracleDbType.Int32,ParameterDirection.Input),
                GetParameter("p_MessageType",messageType,OracleDbType.Varchar2,ParameterDirection.Input),
            };
            var list = ReadCompoundEntityList<UserEmailEntity>(GetUserEmailList, pList, RowToUserEmailList);

            return list;
        }

        private List<MonitorEntity> GetMonitorStatus(int envId)
        {
            var pList = new List<OracleParameter>
            {
                GetParameter("p_ENV_ID",envId,OracleDbType.Int32,ParameterDirection.Input),
            };
            var list = ReadCompoundEntityList<MonitorEntity>(GetMonitors, pList, RowToMonitorList);

            return list;
        }
        
        private List<ServiceMoniterEntity> RowToEnvironmentList(OracleDataReader reader)
        {
            var list = new List<ServiceMoniterEntity>();

            while (reader.Read())
            {
                var entity = new ServiceMoniterEntity();
                if (CommonUtility.IsColumnExistsAndNotNull(reader, "ENV_ID"))
                    entity.EnvID = Convert.ToInt32(reader["ENV_ID"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "ENV_NAME"))
                    entity.EnvName = Convert.ToString(reader["ENV_NAME"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "ENV_IS_MONITOR"))
                    entity.IsMonitor = Convert.ToBoolean(reader["ENV_IS_MONITOR"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "ENV_IS_NOTIFY"))
                    entity.IsNotify = Convert.ToBoolean(reader["ENV_IS_NOTIFY"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "ENV_IS_CONSLTD_MAIL"))
                    entity.IsConsolidated = Convert.ToBoolean(reader["ENV_IS_CONSLTD_MAIL"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "ENV_SORTORDER"))
                    entity.SortOrder = Convert.ToInt32(reader["ENV_SORTORDER"]);

                entity.monitorList = new List<MonitorEntity>();
                list.Add(entity);
            }
            return list;
        }

        private List<MonitorEntity> RowToMonitorList(OracleDataReader reader)
        {
            var list = new List<MonitorEntity>();
            while (reader.Read())
            {
                var entity = new MonitorEntity();

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "MON_ID"))
                    entity.MonID = Convert.ToInt32(reader["MON_ID"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "ENV_ID"))
                    entity.EnvID = Convert.ToInt32(reader["ENV_ID"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_ID"))
                    entity.ConfigID = Convert.ToInt32(reader["CONFIG_ID"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "SCH_ID"))
                    entity.ScheduleID = Convert.ToInt32(reader["SCH_ID"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_HOST_IP"))
                    entity.ConfigHostIP = Convert.ToString(reader["CONFIG_HOST_IP"]).ToLower(CultureInfo.CurrentCulture);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_PORT_NUMBER"))
                    entity.ConfigPort = Convert.ToString(reader["CONFIG_PORT_NUMBER"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_DESCRIPTION"))
                    entity.ConfigServiceDescription = Convert.ToString(reader["CONFIG_DESCRIPTION"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_SERVICE_TYPE"))
                    entity.ConfigServiceType = Convert.ToString(reader["CONFIG_SERVICE_TYPE"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_URL_ADDRESS"))
                    entity.ConfigServiceURL = Convert.ToString(reader["CONFIG_URL_ADDRESS"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_IS_MONITORED"))
                    entity.ConfigIsMonitor = Convert.ToBoolean(reader["CONFIG_IS_MONITORED"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_ISNOTIFY"))
                    entity.ConfigIsNotify = Convert.ToBoolean(reader["CONFIG_ISNOTIFY"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_ISPRIMARY"))
                    entity.ConfigIsPrimary = Convert.ToBoolean(reader["CONFIG_ISPRIMARY"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_MAIL_FREQ"))
                    entity.ConfigMailFrequency = Convert.ToString(reader["CONFIG_MAIL_FREQ"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_LOCATION"))
                    entity.ConfigLocation = Convert.ToString(reader["CONFIG_LOCATION"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "MON_STATUS"))
                    entity.MonitorStatus = Convert.ToString(reader["MON_STATUS"]);
                else
                {
                    entity.MonitorStatus = string.Empty;
                }

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "MON_UPDATED_DATE"))
                    entity.LastMoniterTime = Convert.ToDateTime(reader["MON_UPDATED_DATE"]);
                else
                {
                    entity.LastMoniterTime = null;
                }

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "MON_START_DATE_TIME"))
                    entity.MonitorStartDateTime = Convert.ToString(reader["MON_START_DATE_TIME"]);
                else
                {
                    entity.MonitorStartDateTime = string.Empty;
                }

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "MON_END_DATE_TIME"))
                    entity.MonitorEndDateTime = Convert.ToString(reader["MON_END_DATE_TIME"]);
                else
                {
                    entity.MonitorEndDateTime = string.Empty;
                }

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "MON_CREATED_DATE"))
                    entity.MonitorCreatedDateTime = Convert.ToDateTime(reader["MON_CREATED_DATE"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "MON_UPDATED_DATE"))
                    entity.MonitorUpdatedDateTime = Convert.ToDateTime(reader["MON_UPDATED_DATE"]);
                else
                {
                    entity.MonitorUpdatedDateTime = null;
                }

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "MON_UPTIME"))
                    entity.MonitorUpTime = Convert.ToString(reader["MON_UPTIME"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "MON_COMMENTS"))
                    entity.MonitorComments = Convert.ToString(reader["MON_COMMENTS"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "MON_IS_ACTIVE"))
                    entity.IsActive = Convert.ToBoolean(reader["MON_IS_ACTIVE"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "MON_ISACKNOWLEDGE"))
                    entity.IsAcknowledge = Convert.ToBoolean(reader["MON_ISACKNOWLEDGE"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "TRK_ISSUE"))
                    entity.Incident_Issue = Convert.ToString(reader["TRK_ISSUE"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "TRK_SOLUTION"))
                    entity.Incident_Solution = Convert.ToString(reader["TRK_SOLUTION"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "TRK_CREATED_DATE"))
                    entity.ResolutionCreatedDateTime = Convert.ToDateTime(reader["TRK_CREATED_DATE"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "TRK_CREATED_BY"))
                    entity.ResolutionCreatedBy = Convert.ToString(reader["TRK_CREATED_BY"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "WIN_SERVICENAME"))
                    entity.WindowsServiceName = Convert.ToString(reader["WIN_SERVICENAME"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "WIN_SERVICE_ID"))
                    entity.WindowsServiceID = Convert.ToInt32(reader["WIN_SERVICE_ID"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "BUILD_VERSION"))
                    entity.BuildVersion = Convert.ToString(reader["BUILD_VERSION"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "ENV_NAME"))
                    entity.EnvName = Convert.ToString(reader["ENV_NAME"]);

                if (string.IsNullOrEmpty(entity.ConfigServiceDescription))
                {
                    if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_HOST_IP"))
                        entity.ConfigServiceDescription = entity.ConfigHostIP + ":" + entity.ConfigPort;
                }

                list.Add(entity);
            }

            return list;
        }

        private List<PersonalizeEntity> RowToPersonalizeList(OracleDataReader reader)
        {
            var list = new List<PersonalizeEntity>();
            while (reader.Read())
            {
                var entity = new PersonalizeEntity();
                if (CommonUtility.IsColumnExistsAndNotNull(reader, "PERS_ID"))
                    entity.ID = Convert.ToInt32(reader["PERS_ID"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "User_ID"))
                    entity.UserId = Convert.ToInt32(reader["PERS_ID"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "PERS_DB_REFRESHTIME"))
                    entity.RefreshTime = Convert.ToInt32(reader["PERS_DB_REFRESHTIME"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "PERS_ISACTIVE"))
                    entity.IsActive = Convert.ToBoolean(reader["PERS_ISACTIVE"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "PERS_CREATEDDATE"))
                    entity.CreatedDate = Convert.ToDateTime(reader["PERS_CREATEDDATE"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "PERS_CREATEDBY"))
                    entity.CreatedBy = Convert.ToString(reader["PERS_CREATEDBY"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "PERS_UPDATEDDATE"))
                    entity.UpdatedDate = Convert.ToDateTime(reader["PERS_UPDATEDDATE"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "PERS_UPDATEDBY"))
                    entity.UpdatedBy = Convert.ToString(reader["PERS_UPDATEDBY"]);

                list.Add(entity);
            }

            return list;
        }

        private List<UserEmailEntity> RowToUserEmailList(OracleDataReader reader)
        {
            var list = new List<UserEmailEntity>();

            while (reader.Read())
            {
                var entity = new UserEmailEntity();
                if (CommonUtility.IsColumnExistsAndNotNull(reader, "ENV_ID"))
                    entity.Env_ID = Convert.ToInt32(reader["ENV_ID"]);
                if (CommonUtility.IsColumnExistsAndNotNull(reader, "USRLST_TYPE"))
                    entity.EmailType = Convert.ToString(reader["USRLST_TYPE"]);
                if (CommonUtility.IsColumnExistsAndNotNull(reader, "USRLST_EMAIL_ADDRESS"))
                    entity.EmailAddress = Convert.ToString(reader["USRLST_EMAIL_ADDRESS"]);
                if (CommonUtility.IsColumnExistsAndNotNull(reader, "USRLST_IS_ACTIVE"))
                    entity.IsActive = Convert.ToBoolean(reader["USRLST_IS_ACTIVE"]);
                if (CommonUtility.IsColumnExistsAndNotNull(reader, "USRLST_COMMENTS"))
                    entity.EmailComments = Convert.ToString(reader["USRLST_COMMENTS"]);
                if (CommonUtility.IsColumnExistsAndNotNull(reader, "USRLST_CREATED_BY"))
                    entity.UpdatedBy = Convert.ToString(reader["USRLST_CREATED_BY"]);
                if (CommonUtility.IsColumnExistsAndNotNull(reader, "USRLST_CREATED_DATE"))
                    entity.UpdatedDate = Convert.ToDateTime(reader["USRLST_CREATED_DATE"]);

                list.Add(entity);
            }

            return list;
        }

        private List<WinServiceEntity> RowToWinServiceList(OracleDataReader reader)
        {
            var list = new List<WinServiceEntity>();

            while (reader.Read())
            {
                var entity = new WinServiceEntity();

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "ENV_ID"))
                    entity.EnvID = Convert.ToInt32(reader["ENV_ID"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "ENV_NAME"))
                    entity.EnvName = Convert.ToString(reader["ENV_NAME"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_ID"))
                    entity.ConfigID = Convert.ToInt32(reader["CONFIG_ID"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_HOST_IP"))
                    entity.HostIP = Convert.ToString(reader["CONFIG_HOST_IP"]).ToLower(CultureInfo.CurrentCulture);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_PORT_NUMBER"))
                    entity.Port = Convert.ToString(reader["CONFIG_PORT_NUMBER"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_LOCATION"))
                    entity.Location = Convert.ToString(reader["CONFIG_LOCATION"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "WIN_SERVICE_ID"))
                    entity.ID = Convert.ToInt32(reader["WIN_SERVICE_ID"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "WIN_SERVICENAME"))
                    entity.ServiceName = Convert.ToString(reader["WIN_SERVICENAME"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "WIN_COMMENTS"))
                    entity.Comments = Convert.ToString(reader["WIN_COMMENTS"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_CREATED_BY"))
                    entity.CreatedBy = Convert.ToString(reader["CONFIG_CREATED_BY"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_CREATED_DATE"))
                    entity.CreatedDate = Convert.ToDateTime(reader["CONFIG_CREATED_DATE"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_UPDATED_BY"))
                    entity.UpdatedBy = Convert.ToString(reader["CONFIG_UPDATED_BY"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_UPDATED_DATE"))
                    entity.UpdatedDate = Convert.ToDateTime(reader["CONFIG_UPDATED_DATE"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_SERVICE_TYPE"))
                    entity.ServiceType = Convert.ToString(reader["CONFIG_SERVICE_TYPE"]);

                entity.MonitorStatus = CommonUtility.IsColumnExistsAndNotNull(reader, "MONITOR_STATUS") ? Convert.ToString(reader["MONITOR_STATUS"]) : string.Empty;
                entity.MonitorComments = CommonUtility.IsColumnExistsAndNotNull(reader, "MONITOR_COMMENTS") ? Convert.ToString(reader["MONITOR_COMMENTS"]) : string.Empty;

                list.Add(entity);
            }

            return list;
        }

        private List<ServerDriveSpace> RowToDriveSpceList(OracleDataReader reader)
        {
            var list = new List<ServerDriveSpace>();

            while (reader.Read())
            {
                var entity = new ServerDriveSpace();

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "DRIVE_NAME"))
                    entity.Name = Convert.ToString(reader["DRIVE_NAME"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "AVGFREESPACE"))
                    entity.AverageFreeSpace = Convert.ToDecimal(reader["AVGFREESPACE"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "AVGUSEDSPACE"))
                    entity.AverageUsedSpace = Convert.ToDecimal(reader["AVGUSEDSPACE"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "AVGTOTALSPACE"))
                    entity.AverageTotalSpace = Convert.ToDecimal(reader["AVGTOTALSPACE"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "HOURRT"))
                    entity.Hour = Convert.ToInt32(reader["HOURRT"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "DATERT"))
                    entity.Date = Convert.ToInt32(reader["DATERT"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "MONTHRT"))
                    entity.Month = Convert.ToInt32(reader["MONTHRT"]);

                list.Add(entity);
            }

            return list;
        }

        private List<ServerCpuUsage> RowToCpuUsageList(OracleDataReader reader)
        {
            var list = new List<ServerCpuUsage>();

            while (reader.Read())
            {
                var entity = new ServerCpuUsage();

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "AVGCPUUSAGE"))
                    entity.AverageCpuUsage = Convert.ToDouble(reader["AVGCPUUSAGE"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "AVGAVAILABLEMEMORY"))
                    entity.AverageAvailableMemory = Convert.ToDouble(reader["AVGAVAILABLEMEMORY"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "TOTALMEMORY"))
                    entity.AverageTotalMemory = Convert.ToDouble(reader["TOTALMEMORY"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "HOURRT"))
                    entity.Hour = Convert.ToInt32(reader["HOURRT"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "DATERT"))
                    entity.Date = Convert.ToInt32(reader["DATERT"]);

                list.Add(entity);
            }

            return list;
        }
    }
}