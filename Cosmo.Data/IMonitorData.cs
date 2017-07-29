using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cosmo.Entity;

namespace Cosmo.Data
{
    public interface IMonitorData
    {
        List<ServiceMoniterEntity> GetAllMonitors(int envId, bool isWithServiceName);
        List<ServiceMoniterEntity> GetAllIncident(int envId, string type);
        
        List<MonitorEntity> GetMonitorStatusWithServiceName(int envId, string type);

        List<ServiceMoniterEntity> GetServiceAvailability(int env_id, DateTime startTime, DateTime endTime, string sType,
            bool isWithServiceName);

        List<ServiceMoniterEntity> GetServiceDownTime(int env_id, DateTime startTime, DateTime endTime);
        List<ServiceMoniterEntity> GetServiceBuildVersionReport(int env_id);
        
        List<ServiceMoniterEntity> GetServiceBuildHistoryReport(int env_id, DateTime startDate, DateTime endDate);
        List<ServiceMoniterEntity> GetServiceStatusReport(int env_id, DateTime startDate, DateTime endDate);
        int InsUpdateServiceAcknowledge(AcknowledgeEntity ackData, string mode);
        void InserrtMailLog(MailLogEntity mailLog);
        List<MonitorEntity> GetIncidents(int envId, string type);

        int InsIncidentTracking(string monID, string envID, string configID, string issueDesc, string solutionDesc,
            string userID);

        List<ServiceMoniterEntity> GetIncidentTrackingReport(int env_id, DateTime fromTime, DateTime toTime);
        List<MonitorEntity> GetIncidentTracking(int envId, DateTime fromTime, DateTime toTime);
        List<WinServiceEntity> GetWindowsService(int envId);
        int InsUpdPersonalize(PersonalizeEntity personalize);
        List<PersonalizeEntity> GetPersonalize(int userId);
        List<ServerDriveSpace> GetAverageUsedSpace(string host);
        List<ServerCpuUsage> GetCpuMemorySpace(string host);
        List<UserEmailEntity> GetUserEMail(int envId, string messageType);
    }

    public interface IMonitorDataFactory
    {
        IMonitorData Create(string dbType);
    }

    public class MonitorDataFactory : IMonitorDataFactory
    {
        public IMonitorData Create(string dbType)
        {
            return dbType == Convert.ToInt32(DatabaseType.Oracle).ToString()
                ? (IMonitorData) new MonitorDataOrcl()
                : new MonitorData(); 
        }
    }

}
