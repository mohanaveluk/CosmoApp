using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using Cog.WS.Entity;
using Oracle.ManagedDataAccess.Client;

namespace Cog.WS.Data
{
    public class MonitorDataOrcl: BusinessEntityBaseDAO, IMonitorData
    {
        #region Constant variables
        private const string PackageName = "COSMO_MONITOR_PACKAGE.";

        private const string PackageNameEnvironment = "COSMO_ENVIRONMENT_PACKAGE.";
        private static readonly string FnGetEnvironments = $"{PackageNameEnvironment}FN_CWT_GetEnvironmentList";

        private static readonly string FnGetMonitors = $"{PackageName}FN_CWT_GetMonitorStatus";
        private static readonly string FnGetMonitorsWithServicename = $"{PackageName}FN_CWT_GetMonStatusWithSName";
        private static readonly string FnGetMonitorsWithServicenameConfigid = $"{PackageName}FN_CWT_GetMonStatusWithSN_CID";

        private static readonly string FnServerPerformanceSchedule = $"{PackageName}FN_CWT_GetServerPerfSchedule"; //CWT_GetServerPerformanceSchedule
        private static readonly string SpSetServerPerformance = $"{PackageName}SP_CWT_InsUpdServerPerformance"; //CWT_InsUpdServerPerformance
        private static readonly string SpSetServerPerformanceDrive = $"{PackageName}SP_CWT_InsUpdServerPerfDrive"; //CWT_InsUpdServerPerformanceDrive
        private static readonly string SpSetServerPerformanceSchedule = $"{PackageName}SP_CWT_InsUpdServerPerfSch"; //CWT_InsUpdServerPerformanceSchedule

        #endregion Constant variables

        public List<ServiceMoniterEntity> GetAllMonitors(int envId, bool isWithServiceName)
        {
            var envList = new List<ServiceMoniterEntity>();
            var monitorList = new List<MonitorEntity>();

            var pList = new List<OracleParameter>
            {
                GetParameter("p_ENV_ID", envId, OracleDbType.Int32, ParameterDirection.Input),
            };
            envList = ReadCompoundEntityList<ServiceMoniterEntity>(FnGetEnvironments, pList, RowToEnvironmentList);

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

        public List<MonitorEntity> GetMonitorStatus(int envId)
        {
            var pList = new List<OracleParameter>
            {
                GetParameter("p_ENV_ID",envId,OracleDbType.Int32,ParameterDirection.Input),
            };
            var list = ReadCompoundEntityList<MonitorEntity>(FnGetMonitors, pList, RowToMonitorList);

            return list;
        }

        public List<MonitorEntity> GetMonitorStatusWithServiceName(int envId, string type)
        {
            var stProc = string.Empty;
            var pList = new List<OracleParameter>();
            if (type == "e")
            {
                pList.Add(GetParameter("p_ENV_ID", envId, OracleDbType.Int32));
                stProc = FnGetMonitorsWithServicename;
            }
            else if (type == "c")
            {
                pList.Add(GetParameter("p_CONFIG_ID", envId, OracleDbType.Int32));
                stProc = FnGetMonitorsWithServicenameConfigid;
            }

            var list = ReadCompoundEntityList<MonitorEntity>(stProc, pList, RowToMonitorList);
            return list;
        }

        public List<ServerSchedule> GetServerPerformanceSchedules(int envId)
        {
            var pList = new List<OracleParameter>
            {
                GetParameter("p_ENV_ID",envId,OracleDbType.Int32,ParameterDirection.Input),
            };
            var list = ReadCompoundEntityList<ServerSchedule>(FnServerPerformanceSchedule, pList, RowToServerPerformanceSchedulesList);

            return list;
        }
        
        public string SetServerPerformance(PerformanceInfo pInfo)
        {
            var scopeOutput = string.Empty;

            try
            {
                var pList = new List<OracleParameter>
                {
                    GetParameter("p_ENVID", pInfo.EnvId, OracleDbType.Int32, ParameterDirection.Input),
                    GetParameter("p_CONFIGID", pInfo.ConfigId, OracleDbType.Int32, ParameterDirection.Input),
                    GetParameter("p_PER_HOSTIP", !string.IsNullOrEmpty(pInfo.HostIp) ? pInfo.HostIp : string.Empty,
                        OracleDbType.Varchar2, ParameterDirection.Input),
                    GetParameter("p_PER_CPU_USAGE", pInfo.Cpu, OracleDbType.Double, ParameterDirection.Input),
                    GetParameter("p_PER_AVAILABLEMEMORY", pInfo.AvailableMemory, OracleDbType.Double,
                        ParameterDirection.Input),
                    GetParameter("p_PER_TOTALMEMORY", pInfo.TotalMemory, OracleDbType.Double, ParameterDirection.Input),
                    GetParameter("p_PER_CREATED_BY", "System", OracleDbType.Varchar2, ParameterDirection.Input),
                    GetParameter("p_PER_COMMENTS", string.Empty, OracleDbType.Varchar2, ParameterDirection.Input),
                    
                    new OracleParameter("p_SCOPE_OUTPUT", OracleDbType.Int32, ParameterDirection.Output)
                };

                ExecuteNonQuery(SpSetServerPerformance, pList).ToString();

                foreach (var oracleParameter in pList.Where(oracleParameter => oracleParameter.ParameterName == "p_SCOPE_OUTPUT"))
                {
                    scopeOutput = oracleParameter.Value.ToString();
                }

            }
            catch (Exception)
            {
                
                throw;
            }
            return scopeOutput;
        }

        public void SetServerPerformanceDrive(Drive drive, int perfId)
        {
            var pList = new List<OracleParameter>
            {
                GetParameter("p_PER_ID", perfId, OracleDbType.Int32, ParameterDirection.Input),
                GetParameter("p_DRIVE_NAME", !string.IsNullOrEmpty(drive.Name) ? drive.Name : string.Empty,
                    OracleDbType.Varchar2, ParameterDirection.Input),
                GetParameter("p_DRIVE_LABEL", !string.IsNullOrEmpty(drive.Label) ? drive.Label : string.Empty,
                    OracleDbType.Varchar2, ParameterDirection.Input),
                GetParameter("p_DRIVE_FORMAT", !string.IsNullOrEmpty(drive.Format) ? drive.Format : string.Empty,
                    OracleDbType.Varchar2, ParameterDirection.Input),
                GetParameter("p_DRIVE_TYPE",
                    !string.IsNullOrEmpty(drive.Type.ToString()) ? drive.Type.ToString() : string.Empty,
                    OracleDbType.Varchar2, ParameterDirection.Input),
                GetParameter("p_DRIVE_FREESPACE", drive.FreeSpace, OracleDbType.Double, ParameterDirection.Input),
                GetParameter("p_DRIVE_USEDSPACE", drive.UsedSpace, OracleDbType.Double, ParameterDirection.Input),
                GetParameter("p_DRIVE_TOTALSPACE", drive.TotalSpace, OracleDbType.Double, ParameterDirection.Input),
                GetParameter("p_DRIVE_COMMENTS", string.Empty, OracleDbType.Varchar2, ParameterDirection.Input),
            };
            ExecuteNonQuery(SpSetServerPerformanceDrive, pList);
        }

        public void SetServerPerformanceSchedule(ServerSchedule serverSchedule)
        {
            var pList = new List<OracleParameter>
            {
                GetParameter("p_ENVID", serverSchedule.EnvId, OracleDbType.Int32, ParameterDirection.Input),
                GetParameter("p_CONFIGID", serverSchedule.ConfigId, OracleDbType.Int32, ParameterDirection.Input),
                GetParameter("p_HOSTIP",
                    !string.IsNullOrEmpty(serverSchedule.HostIp) ? serverSchedule.HostIp : string.Empty,
                    OracleDbType.Varchar2, ParameterDirection.Input),
                GetParameter("p_PORT", !string.IsNullOrEmpty(serverSchedule.Port) ? serverSchedule.Port : string.Empty,
                    OracleDbType.Varchar2, ParameterDirection.Input),
                GetParameter("p_LASTJOBRUNTIME", serverSchedule.LastJobRunTime, OracleDbType.TimeStamp,
                    ParameterDirection.Input),
                GetParameter("p_NEXTJOBRUNTIME", serverSchedule.NextJobRunTime, OracleDbType.TimeStamp,
                    ParameterDirection.Input),
                GetParameter("p_MODE", "NS", OracleDbType.Varchar2, ParameterDirection.Input),
            };

            ExecuteNonQuery(SpSetServerPerformanceSchedule, pList);
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

        private List<ServerSchedule> RowToServerPerformanceSchedulesList(OracleDataReader reader)
        {
            var list = new List<ServerSchedule>();

            while (reader.Read())
            {
                var entity = new ServerSchedule();
                if (CommonUtility.IsColumnExistsAndNotNull(reader, "SVR_ID"))
                    entity.Id = Convert.ToInt32(reader["SVR_ID"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "ENVID"))
                    entity.EnvId = Convert.ToInt32(reader["ENVID"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIGID"))
                    entity.ConfigId = Convert.ToInt32(reader["CONFIGID"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "ENV_NAME"))
                    entity.EnvName = Convert.ToString(reader["ENV_NAME"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_HOST_IP"))
                    entity.HostIp = Convert.ToString(reader["CONFIG_HOST_IP"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_PORT_NUMBER"))
                    entity.Port = Convert.ToString(reader["CONFIG_PORT_NUMBER"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIGID"))
                    entity.ConfigId = Convert.ToInt32(reader["CONFIGID"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "SVR_LASTJOBRAN_TIME"))
                    entity.LastJobRunTime = Convert.ToDateTime(reader["SVR_LASTJOBRAN_TIME"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "SVR_NEXTJOBRAN_TIME"))
                    entity.NextJobRunTime = Convert.ToDateTime(reader["SVR_NEXTJOBRAN_TIME"]);

                list.Add(entity);
            }

            return list;
        }

    }
}
