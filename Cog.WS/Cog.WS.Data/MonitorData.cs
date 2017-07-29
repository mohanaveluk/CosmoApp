using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Cog.WS.Entity;

namespace Cog.WS.Data
{
    public class MonitorData: IMonitorData
    {
        #region Constant variables

        private const string GET_ENVIRONMENTS = "CWT_GetEnvironmentList";
        private const string GET_MONITORS = "CWT_GetMonitorStatus"; //updated on 06/12/15

        private const string GET_MONITORS_WITH_SERVICENAME = "CWT_GetMonitorStatusWithServiceName";
            //updated on 06/12/15

        private const string GET_MONITORS_WITH_SERVICENAME_CONFIGID = "CWT_GetMonitorStatusWithServiceName_ConID";
            //updated on 06/12/15

        private const string ServerPerformanceSchedule = "CWT_GetServerPerformanceSchedule"; //updated on 06/12/15
        private const string InsUpdServerPerformance = "CWT_InsUpdServerPerformance"; //updated on 06/12/15
        private const string InsUpdServerPerformanceDrive = "CWT_InsUpdServerPerformanceDrive"; //updated on 06/12/15
        private const string InsUpdServerPerformanceSchedule = "CWT_InsUpdServerPerformanceSchedule"; //updated on 06/12/15

        #endregion Constant variables


        #region Get current monitor status all environments

        /// <summary>
        /// Get current monitor status all environments
        /// </summary>
        /// <param name="env_id"></param>
        /// <returns></returns>
        public List<ServiceMoniterEntity> GetAllMonitors(int env_id, bool isWithServiceName)
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
                    if (!isWithServiceName)
                        envList = envList.Where(el => el.IsMonitor == true).ToList();
                    foreach (ServiceMoniterEntity entity in envList)
                    {
                        monitorList = new List<MonitorEntity>();
                        if (isWithServiceName)
                            monitorList = GetMonitorStatusWithServiceName(entity.EnvID, "e");
                        else
                            monitorList = GetMonitorStatus(entity.EnvID);
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

        #endregion Get current monitor status all environments

        /// <summary>
        /// Get current monitor status by environment
        /// </summary>
        /// <param name="envId"></param>
        /// <returns></returns>
        public List<MonitorEntity> GetMonitorStatus(int envId)
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
        /// <returns></returns>
        public List<MonitorEntity> GetMonitorStatusWithServiceName(int envId, string type)
        {
            string stProc = string.Empty;
            List<SqlParameter> pList = new List<SqlParameter>();
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

        #region Get Server performance schedule details

        /// <summary>
        /// Get current monitor status by environment
        /// </summary>
        /// <param name="envId"></param>
        /// <returns></returns>
        public List<ServerSchedule> GetServerPerformanceSchedules(int envId)
        {
            var pList = new List<SqlParameter> {new SqlParameter("@ENVID", envId)};
 
            return UtilityDL.FillData<ServerSchedule>(ServerPerformanceSchedule, pList);
        }

        public string SetServerPerformance(PerformanceInfo pInfo)
        {
            string scopeOutput = string.Empty;

            SqlParameter retVal = new SqlParameter("@SCOPE_OUTPUT", SqlDbType.VarChar, -1);
            retVal.Direction = ParameterDirection.Output;
            var pList = new List<SqlParameter>
            {
                new SqlParameter("@ENVID", pInfo.EnvId),
                new SqlParameter("@CONFIGID", pInfo.ConfigId),
                new SqlParameter("@PER_HOSTIP", !string.IsNullOrEmpty(pInfo.HostIp) ? pInfo.HostIp : string.Empty),
                new SqlParameter("@PER_CPU_USAGE", pInfo.Cpu),
                new SqlParameter("@PER_AVAILABLEMEMORY", pInfo.AvailableMemory),
                new SqlParameter("@PER_TOTALMEMORY", pInfo.TotalMemory),
                new SqlParameter("@PER_CREATED_BY", "System"),
                new SqlParameter("@PER_COMMENTS", string.Empty),
                retVal
            };

            scopeOutput = Convert.ToString(UtilityDL.ExecuteNonQuery(InsUpdServerPerformance, pList, true));

            return scopeOutput;
        }


        public void SetServerPerformanceDrive(Drive drive, int perfId)
        {

            var pList = new List<SqlParameter>
            {
                new SqlParameter("@PER_ID", perfId),
                new SqlParameter("@DRIVE_NAME", !string.IsNullOrEmpty(drive.Name) ? drive.Name : string.Empty),
                new SqlParameter("@DRIVE_LABEL", !string.IsNullOrEmpty(drive.Label) ? drive.Label : string.Empty),
                new SqlParameter("@DRIVE_FORMAT", !string.IsNullOrEmpty(drive.Format) ? drive.Format : string.Empty),
                new SqlParameter("@DRIVE_TYPE",
                    !string.IsNullOrEmpty(drive.Type.ToString()) ? drive.Type.ToString() : string.Empty),
                new SqlParameter("@DRIVE_FREESPACE", drive.FreeSpace),
                new SqlParameter("@DRIVE_USEDSPACE", drive.UsedSpace),
                new SqlParameter("@DRIVE_TOTALSPACE", drive.TotalSpace),
                new SqlParameter("@DRIVE_COMMENTS", string.Empty),
            };

            UtilityDL.ExecuteNonQuery(InsUpdServerPerformanceDrive, pList);
        }

        public void SetServerPerformanceSchedule(ServerSchedule serverSchedule)
        {
            var pList = new List<SqlParameter>
            {
                new SqlParameter("@ENVID", serverSchedule.EnvId),
                new SqlParameter("@CONFIGID", serverSchedule.ConfigId),
                new SqlParameter("@HOSTIP",
                    !string.IsNullOrEmpty(serverSchedule.HostIp) ? serverSchedule.HostIp : string.Empty),
                new SqlParameter("@PORT",
                    !string.IsNullOrEmpty(serverSchedule.Port) ? serverSchedule.Port : string.Empty),
                new SqlParameter("@LASTJOBRUNTIME", serverSchedule.LastJobRunTime),
                new SqlParameter("@NEXTJOBRUNTIME", serverSchedule.NextJobRunTime),
                new SqlParameter("@MODE", "NS"),
            };

            UtilityDL.ExecuteNonQuery(InsUpdServerPerformanceSchedule, pList);
        }

        #endregion Get Server performance schedule details
    }
}
