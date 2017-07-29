using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cog.WS.Entity;

namespace Cog.WS.Data
{
    public interface IMonitorDataFactory
    {
        IMonitorData Create(string dbType);
    }

    public class MonitorDataFactory : IMonitorDataFactory
    {
        public IMonitorData Create(string dbType)
        {
            return dbType == Convert.ToInt32(DatabaseType.Oracle).ToString()
                ? (IMonitorData)new MonitorDataOrcl()
                : new MonitorData();
        }
    }

    public interface IMonitorData
    {
        List<ServiceMoniterEntity> GetAllMonitors(int env_id, bool isWithServiceName);
        List<MonitorEntity> GetMonitorStatus(int envId);
        List<MonitorEntity> GetMonitorStatusWithServiceName(int envId, string type);
        List<ServerSchedule> GetServerPerformanceSchedules(int envId);
        string SetServerPerformance(PerformanceInfo pInfo);
        void SetServerPerformanceDrive(Drive drive, int perfId);
        void SetServerPerformanceSchedule(ServerSchedule serverSchedule);

    }
}
