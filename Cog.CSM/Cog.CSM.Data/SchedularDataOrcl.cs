using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using Cog.CSM.Entity;
using Oracle.ManagedDataAccess.Client;

namespace Cog.CSM.Data
{
    public class SchedularDataOrcl: BusinessEntityBaseDAO, ISchedularData
    {
        #region Constant variables

        private const string PackageName = "COSMO_CSM_SCHEDULE_PACKAGE.";
        private const string EnvironmentPackageName = "COSMO_ENVIRONMENT_PACKAGE.";
        private const string MonitorPackageName = "COSMO_MONITOR_PACKAGE.";

        private static readonly string FnGetSchedules = $"{PackageName}FN_CWT_GetAllSchedule_NJS";
        private static readonly string FnGetAllScheduledServices = $"{PackageName}FN_CWT_GetAllScheduledServices";
        private static readonly string FnGetUserEmailList = $"{PackageName}FN_CWT_GetUserEmailList";
        private static readonly string FnGetSubscription = $"{PackageName}FN_CWT_GetSubscription";
        private static readonly string FnGetSubscriptiondetail = $"{PackageName}FN_CWT_GetSubscriptionDetail";
        private static readonly string FnGetPortals = $"{PackageName}FN_CWT_GetPortelToMonitor";
        private static readonly string FnGetSubscriptionMonitorStatus = $"{PackageName}FN_CWT_GetSubsMonitorStatus"; //CWT_GetSubscriptionMonitorStatus

        private static readonly string GetEnvironments = $"{EnvironmentPackageName}FN_CWT_GetEnvironmentList";
        private static readonly string GetMonitors = $"{MonitorPackageName}FN_CWT_GetMonitorStatus";
        private static readonly string GetMonitorsWithServicename = $"{MonitorPackageName}FN_CWT_GetMonStatusWithSName";
        private static readonly string GetMonitorsWithServicenameConfigid =
            $"{MonitorPackageName}FN_CWT_GetMonStatusWithSN_CID";



        private static readonly string SpSetUpdateSchedulerLastRunDateTime = $"{PackageName}SP_CWT_UpdateScheduleLastRunDt"; //CWT_UpdateSchedulerLastRunDateTime
        private static readonly string SpSetSubscriptionnextjobRuntime = $"{PackageName}SP_CWT_SetSubsNextJobRunTime"; //CWT_SetSubscriptionNextJobRunTime
        private static readonly string SpSetPortalnextjonruntime = $"{PackageName}SP_CWT_SetPortalNextJobRunTime"; //CWT_SetSubscriptionNextJobRunTime
        private static readonly string SpSetPortalStatus = $"{PackageName}SP_CWT_InsertPortalStatus";


        #endregion Constant variables


        public List<Scheduler_details> GetSchedules(DateTime beforeDateTime)
        {
            var pList = new List<OracleParameter>
            {
                GetParameter("p_DateNextJobRunStartBefore", beforeDateTime, OracleDbType.TimeStamp,
                    ParameterDirection.Input),
            };
            var scheduleList = ReadCompoundEntityList<Scheduler_details>(FnGetSchedules, pList, RowToScheduleList);
            return scheduleList;
        }

        public List<ServiceEntity> GetAllScheduledServices(DateTime beforeDateTime)
        {
            var pList = new List<OracleParameter>
            {
                GetParameter("p_DateNextJobRunStartBefore", beforeDateTime, OracleDbType.TimeStamp,
                    ParameterDirection.Input),
            };
            var serviceList = ReadCompoundEntityList<ServiceEntity>(FnGetAllScheduledServices, pList, RowToServiceList);
            return serviceList;
        }

        
        public void UPdateSchedule(int schedulerId, DateTime dateLastJobRan)
        {
            var pList = new List<OracleParameter>
            {
                GetParameter("p_SchedulerID", schedulerId, OracleDbType.Int32, ParameterDirection.Input),
                GetParameter("p_DateLastJobRun", dateLastJobRan, OracleDbType.TimeStamp, ParameterDirection.Input),
            };

            ExecuteNonQuery(SpSetUpdateSchedulerLastRunDateTime, pList);
            
        }

        public List<UserEmailEntity> GetUserEMail(int envId, string messageType)
        {
            var pList = new List<OracleParameter>
            {
                GetParameter("p_Env_ID", envId, OracleDbType.Int32, ParameterDirection.Input),
                GetParameter("p_MessageType", messageType, OracleDbType.Varchar2, ParameterDirection.Input),
            };

            var emailList = ReadCompoundEntityList<UserEmailEntity>(FnGetUserEmailList, pList, RowToUserEmailList);
            return emailList;
        }

        
        public List<Subscription> GetSubscription(int id)
        {
            var pList = new List<OracleParameter>
            {
                GetParameter("p_ID", id, OracleDbType.Int32, ParameterDirection.Input),
            };
            var subcriptionList = ReadCompoundEntityList<Subscription>(FnGetSubscription, pList, RowToSubscriptionList);
            return subcriptionList;
        }

        public List<SubscriptionDetail> GetSubscriptionDetails(int id)
        {
            var pList = new List<OracleParameter>
            {
                GetParameter("p_ID", id, OracleDbType.Int32, ParameterDirection.Input),
            };
            var subcriptionList = ReadCompoundEntityList<SubscriptionDetail>(FnGetSubscriptiondetail, pList, RowToSubscriptionDetailList);
            return subcriptionList;
        }

        public void SetNextJobRunTime(Subscription subscription)
        {
            try
            {
                var nextJobRunTime =
                    Convert.ToDateTime(DateTime.Now.ToShortDateString() + " 00:00:00").AddDays(1)
                        .AddHours(Convert.ToInt32(subscription.Time));

                var pList = new List<OracleParameter>
                {
                    GetParameter("p_ID", subscription.Id, OracleDbType.Int32, ParameterDirection.Input),
                    GetParameter("p_LASTJOBRANTIME", DateTime.Now, OracleDbType.TimeStamp, ParameterDirection.Input),
                    GetParameter("p_NEXTJOBRUNTIME", nextJobRunTime, OracleDbType.TimeStamp, ParameterDirection.Input),
                };

                ExecuteNonQuery(SpSetSubscriptionnextjobRuntime, pList);
            }
            catch (Exception)
            {
                
                throw;
            }
            
        }

        public void SetPortalNextJobRunTime(Portal portal)
        {
            try
            {
                var nextJobRunTime = DateTime.Now.AddMinutes(portal.Interval);
                var pList = new List<OracleParameter>
                {
                    GetParameter("p_ID", portal.Id, OracleDbType.Int32, ParameterDirection.Input),
                    GetParameter("p_LASTJOBRANTIME", DateTime.Now, OracleDbType.TimeStamp, ParameterDirection.Input),
                    GetParameter("p_NEXTJOBRUNTIME", nextJobRunTime, OracleDbType.TimeStamp, ParameterDirection.Input),
                };
                ExecuteNonQuery(SpSetPortalnextjonruntime, pList);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void InsertPortalStatus(CognosCgiResponse response)
        {
            var pList = new List<OracleParameter>
                {
                    GetParameter("p_URL_ID", response.UrlId, OracleDbType.Int32, ParameterDirection.Input),
                    GetParameter("p_ENV_ID", response.EnvId, OracleDbType.Int32, ParameterDirection.Input),
                    GetParameter("p_PMON_STATUS", !string.IsNullOrEmpty(response.Status) ? response.Status: string.Empty, OracleDbType.Varchar2, ParameterDirection.Input),
                    GetParameter("p_PMON_MESSAGE", !string.IsNullOrEmpty(response.Message) ? response.Message:string.Empty, OracleDbType.Varchar2, ParameterDirection.Input),
                    GetParameter("p_PMON_RESPONSETIME", response.ResponseTime > 0 ? response.ResponseTime: 0, OracleDbType.Int32, ParameterDirection.Input),
                    GetParameter("p_PMON_EXCEPTION", !string.IsNullOrEmpty(response.Exception) ? response.Exception: string.Empty, OracleDbType.Varchar2, ParameterDirection.Input),
                };

            ExecuteNonQuery(SpSetPortalStatus, pList);
        }

        public List<Portal> GetPortalToMonitor(int id)
        {
            //FnGetPortals
            var pList = new List<OracleParameter>
            {
                GetParameter("p_ID", id, OracleDbType.Int32, ParameterDirection.Input),
            };
            var portalToMonitor = ReadCompoundEntityList<Portal>(FnGetPortals, pList, RowToPortalToMonitorList);
            return portalToMonitor;
        }

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

        public List<ServiceMoniterEntity> GetSubscriptionStatusList(int envId, Subscription subscription)
        {
            var pList = new List<OracleParameter>
            {
                GetParameter("p_ENV_ID", envId, OracleDbType.Int32, ParameterDirection.Input),
            };
            var envList = ReadCompoundEntityList<ServiceMoniterEntity>(GetEnvironments, pList, RowToEnvironmentList);

            if (envList == null || envList.Count <= 0) return envList;

            foreach (ServiceMoniterEntity entity in envList)
            {
                var monitorList = GetSubscriptionMonitorStatus(entity.EnvID, subscription);
                entity.monitorList = monitorList;
            }

            return envList;
        }

        private List<MonitorEntity> GetSubscriptionMonitorStatus(int envId, Subscription subscription)
        {
            var pList = new List<OracleParameter>
            {
                GetParameter("p_ENV_ID", envId, OracleDbType.Int32, ParameterDirection.Input),
                GetParameter("p_TYPE", subscription.Type, OracleDbType.Varchar2, ParameterDirection.Input),
                GetParameter("p_STARTTIME", subscription.StartTime, OracleDbType.TimeStamp, ParameterDirection.Input),
                GetParameter("p_ENDTIME", subscription.EndTime, OracleDbType.TimeStamp, ParameterDirection.Input),
            };

            var monitorList = ReadCompoundEntityList<MonitorEntity>(GetEnvironments, pList, RowToMonitorList);
            return monitorList;

        }
        
        private static List<Scheduler_details> RowToScheduleList(OracleDataReader reader)
        {
            var list = new List<Scheduler_details>();

            while (reader.Read())
            {
                var entity = new Scheduler_details();

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "SCH_ID"))
                    entity.Scheduler_id = Convert.ToInt32(reader["SCH_ID"]);
                if (CommonUtility.IsColumnExistsAndNotNull(reader, "ENV_ID"))
                    entity.Env_ID = Convert.ToInt32(reader["ENV_ID"]);
                if (CommonUtility.IsColumnExistsAndNotNull(reader, "ENV_NAME"))
                    entity.Env_name = Convert.ToString(reader["ENV_NAME"]);
                if (CommonUtility.IsColumnExistsAndNotNull(reader, "SCH_INTERVAL"))
                    entity.Sch_interval = Convert.ToInt32(reader["SCH_INTERVAL"]);
                if (CommonUtility.IsColumnExistsAndNotNull(reader, "SCH_DURATION"))
                    entity.Sch_duration = Convert.ToString(reader["SCH_DURATION"]);
                if (CommonUtility.IsColumnExistsAndNotNull(reader, "SCH_LASTJOBRAN_TIME"))
                    entity.Sch_lastJobRunTime = Convert.ToDateTime(reader["SCH_LASTJOBRAN_TIME"]);
                if (CommonUtility.IsColumnExistsAndNotNull(reader, "SCH_NEXTJOBRAN_TIME"))
                    entity.Sch_nextJobRunTime = Convert.ToDateTime(reader["SCH_NEXTJOBRAN_TIME"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "SCH_COMMENTS"))
                    entity.Sch_comments = reader["SCH_COMMENTS"].ToString().Trim();

                list.Add(entity);
            }

            return list;
        }

        private List<ServiceEntity> RowToServiceList(OracleDataReader reader)
        {
            var list = new List<ServiceEntity>();

            while (reader.Read())
            {
                var entity = new ServiceEntity();

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_ID"))
                    entity.Config_ID = Convert.ToInt32(reader["CONFIG_ID"]);
                if (CommonUtility.IsColumnExistsAndNotNull(reader, "ENV_ID"))
                    entity.Env_ID = Convert.ToInt32(reader["ENV_ID"]);
                if (CommonUtility.IsColumnExistsAndNotNull(reader, "SCH_ID"))
                    entity.Sch_ID = Convert.ToInt32(reader["SCH_ID"]);
                if (CommonUtility.IsColumnExistsAndNotNull(reader, "ENV_NAME"))
                    entity.Env_Name = Convert.ToString(reader["ENV_NAME"]);
                if (CommonUtility.IsColumnExistsAndNotNull(reader, "ENV_LOCATION"))
                    entity.Env_Location = Convert.ToString(reader["ENV_LOCATION"]);
                if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_SERVICE_TYPE"))
                    entity.Config_ServiceType = Convert.ToString(reader["CONFIG_SERVICE_TYPE"]);
                if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_HOST_IP"))
                    entity.Config_ServerIP = Convert.ToString(reader["CONFIG_HOST_IP"]);
                if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_PORT_NUMBER"))
                    entity.Config_PortNumber = Convert.ToString(reader["CONFIG_PORT_NUMBER"]);
                if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_URL_ADDRESS"))
                    entity.Config_ServiceURL = Convert.ToString(reader["CONFIG_URL_ADDRESS"]);
                if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_DESCRIPTION"))
                    entity.Config_ServiceDescription = Convert.ToString(reader["CONFIG_DESCRIPTION"]);
                if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_IS_VALIDATED"))
                    entity.Config_IsValidated = Convert.ToBoolean(reader["CONFIG_IS_VALIDATED"]);
                if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_IS_ACTIVE"))
                    entity.Config_IsActive = Convert.ToBoolean(reader["CONFIG_IS_ACTIVE"]);
                if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_IS_MONITORED"))
                    entity.Config_IsMonitored = Convert.ToBoolean(reader["CONFIG_IS_MONITORED"]);
                if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_IS_LOCKED"))
                    entity.Config_IsLocked = Convert.ToBoolean(reader["CONFIG_IS_LOCKED"]);
                if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_COMMENTS"))
                    entity.Config_ServiceComments = Convert.ToString(reader["CONFIG_COMMENTS"]);

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


        private List<Subscription> RowToSubscriptionList(OracleDataReader reader)
        {
            var list = new List<Subscription>();

            while (reader.Read())
            {
                var entity = new Subscription();

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "SUBSCRIPTION_ID"))
                    entity.Id = Convert.ToInt32(reader["SUBSCRIPTION_ID"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "SUBSCRIPTION_TYPE"))
                    entity.Type = Convert.ToString(reader["SUBSCRIPTION_TYPE"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "SUBSCRIPTION_TIME"))
                    entity.Time = Convert.ToString(reader["SUBSCRIPTION_TIME"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "SUBSCRIPTION_ISACTIVE"))
                    entity.IsActive = Convert.ToBoolean(reader["SUBSCRIPTION_ISACTIVE"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "CREATEDBY_NAME"))
                    entity.CreatedBy = Convert.ToString(reader["CREATEDBY_NAME"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "CREATED_DATE"))
                    entity.CreatedDate = Convert.ToDateTime(reader["CREATED_DATE"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "UPDATEDBY_NAME"))
                    entity.UpdatedBy = Convert.ToString(reader["UPDATEDBY_NAME"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "UPDATED_DATE"))
                    entity.UpdatedDate = Convert.ToDateTime(reader["UPDATED_DATE"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "SCH_LASTJOBRAN_TIME"))
                    entity.LastJobRanTime = Convert.ToDateTime(reader["SCH_LASTJOBRAN_TIME"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "SCH_NEXTJOBRAN_TIME"))
                    entity.NextJonRunTime = Convert.ToDateTime(reader["SCH_NEXTJOBRAN_TIME"]);

                list.Add(entity);
            }

            return list;
        }


        private List<SubscriptionDetail> RowToSubscriptionDetailList(OracleDataReader reader)
        {
            var list = new List<SubscriptionDetail>();

            while (reader.Read())
            {
                var entity = new SubscriptionDetail();
                if (CommonUtility.IsColumnExistsAndNotNull(reader, "SUBSCRIPTION_ID"))
                    entity.Id = Convert.ToInt32(reader["SUBSCRIPTION_ID"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "SUBSCRIPTION_DETAIL_ID"))
                    entity.DetailId = Convert.ToInt32(reader["SUBSCRIPTION_DETAIL_ID"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "USRLST_ID"))
                    entity.UserListId = Convert.ToInt32(reader["USRLST_ID"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "SUBSCRIPTION_EMAILID"))
                    entity.EmailAddress = Convert.ToString(reader["SUBSCRIPTION_EMAILID"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "SUBSCRIPTION_ISACTIVE"))
                    entity.IsActive = Convert.ToBoolean(reader["SUBSCRIPTION_ISACTIVE"]);

                list.Add(entity);
            }

            return list;
        }

        private List<Portal> RowToPortalToMonitorList(OracleDataReader reader)
        {
            var list = new List<Portal>();

            while (reader.Read())
            {
                var entity = new Portal();

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "URL_ID"))
                    entity.Id = Convert.ToInt32(reader["URL_ID"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "ENV_ID"))
                    entity.EnvId = Convert.ToInt32(reader["ENV_ID"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "ENV_NAME"))
                    entity.EnvName = Convert.ToString(reader["ENV_NAME"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "URL_TYPE"))
                    entity.Type = Convert.ToString(reader["URL_TYPE"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "URL_ADDRESS"))
                    entity.Adress = Convert.ToString(reader["URL_ADDRESS"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "URL_DISPLAYNAME"))
                    entity.DisplayName = Convert.ToString(reader["URL_DISPLAYNAME"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "URL_MATCHCONTENT"))
                    entity.MatchContent = Convert.ToString(reader["URL_MATCHCONTENT"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "URL_INTERVAL"))
                    entity.Interval = Convert.ToInt32(reader["URL_INTERVAL"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "URL_USERNAME"))
                    entity.UserName = Convert.ToString(reader["URL_USERNAME"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "URL_PASSWORD"))
                    entity.Password = Convert.ToString(reader["URL_PASSWORD"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "URL_ISACTIVE"))
                    entity.IsActive = Convert.ToBoolean(reader["URL_ISACTIVE"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "URL_STATUS"))
                    entity.Status = Convert.ToBoolean(reader["URL_STATUS"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "URL_CREATEDBY"))
                    entity.CreatedBy = Convert.ToString(reader["URL_CREATEDBY"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "URL_CREATEDDATE"))
                    entity.CreatedDateTime = Convert.ToDateTime(reader["URL_CREATEDDATE"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "URL_UPDATEDBY"))
                    entity.UpdatedBy = Convert.ToString(reader["URL_UPDATEDBY"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "URL_UPDATEDDATE"))
                    entity.UpdatedDateTime = Convert.ToDateTime(reader["URL_UPDATEDDATE"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "URL_COMMENTS"))
                    entity.Comments = Convert.ToString(reader["URL_COMMENTS"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "URL_CREATEDDATE"))
                    entity.CreatedDateTime = Convert.ToDateTime(reader["URL_CREATEDDATE"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "URL_CREATEDDATE"))
                    entity.CreatedDateTime = Convert.ToDateTime(reader["URL_CREATEDDATE"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "URL_LASTJOBRUNTIME"))
                    entity.LastJobRunTime = Convert.ToDateTime(reader["URL_LASTJOBRUNTIME"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "URL_NEXTJOBRUNTIME"))
                    entity.NextJobRunTime = Convert.ToDateTime(reader["URL_NEXTJOBRUNTIME"]);

                list.Add(entity);
            }

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
                pList.Add(GetParameter("p_CONFIG_ID", envId, OracleDbType.Int32));
                stProc = GetMonitorsWithServicenameConfigid;
            }

            var list = ReadCompoundEntityList<MonitorEntity>(stProc, pList, RowToMonitorList);
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
            }

            return list;
        }

    }
}
