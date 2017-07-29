using System;
using System.Collections.Generic;
using System.Configuration;
using Cog.WS.Data;
using Cog.WS.Entity;

namespace Cog.WS.Service
{
    public class MonitorService
    {
        IMonitorData _monitorData;
        private static readonly string DbType = ConfigurationManager.AppSettings["DatabaseType"];

        public MonitorService()
        {
            var iDbType = DbType == DatabaseType.Oracle.ToString()
               ? Convert.ToInt32(DatabaseType.Oracle).ToString()
               : Convert.ToInt32(DatabaseType.SqlServer).ToString();

            _monitorData = new MonitorDataFactory().Create(Convert.ToInt32(iDbType).ToString());
        }
        /// <summary>
        /// Get current monitor status all environments
        /// </summary>
        /// <param name="envId"></param>
        /// <param name="isWithServiceName"></param>
        /// <returns></returns>
        public List<ServiceMoniterEntity> GetAllMonitors(int envId, bool isWithServiceName)
        {
            var monitorList = new List<ServiceMoniterEntity>();
            try
            {
                monitorList = _monitorData.GetAllMonitors(envId, isWithServiceName);

            }
            catch (Exception ex)
            {
                Logger.Log(ex.ToString());
            }
            return monitorList;
        }
    }
}
