using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cog.CSM.Entity;

namespace Cog.CSM.Data
{
    public interface ISchedularData
    {
        List<Scheduler_details> GetSchedules(DateTime beforeDateTime);
        List<ServiceEntity> GetAllScheduledServices(DateTime beforeDateTime);
        void UPdateSchedule(int schedulerId, DateTime dateLastJobRan);
        List<UserEmailEntity> GetUserEMail(int envId, string messageType);
        List<Subscription> GetSubscription(int id);
        List<SubscriptionDetail> GetSubscriptionDetails(int id);
        void SetNextJobRunTime(Subscription subscription);
        void SetPortalNextJobRunTime(Portal portal);
        void InsertPortalStatus(CognosCgiResponse response);
        List<Portal> GetPortalToMonitor(int id);
        List<ServiceMoniterEntity> GetAllMonitors(int envId, bool isWithServiceName);
        List<ServiceMoniterEntity> GetSubscriptionStatusList(int envId, Subscription subscription);


    }

    public interface ISchedularDataFactory
    {
        ISchedularData Create(string dbType);
    }

    public class SchedularDataFactory : ISchedularDataFactory
    {
        public ISchedularData Create(string dbType)
        {
            return dbType == Convert.ToInt32(DatabaseType.Oracle).ToString()
                ? (ISchedularData) new SchedularDataOrcl()
                : new SchedularData();
        }
    }
}