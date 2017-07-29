using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cog.CSM.Entity;

namespace Cog.CSM.Data
{
    public interface IExecuteServiceData
    {
        void InsUpdMonitorService(ServiceEntity service, ContentManager content);
        void InsUpdMonitorService(ServiceEntity service, Despatcher despatcher);
        void InserrtMailLog(MailLogEntity mailLog);
        List<SendNotificationEntity> GetAllSendMailNotification(DateTime beforeDateTime);
        string GetServiceLastStatus(ServiceEntity service);
    }

    public interface IExecuteServiceDataFactory
    {
        IExecuteServiceData Create(string dbType);
    }

    public class ExecuteServiceDataFactory : IExecuteServiceDataFactory
    {
        public IExecuteServiceData Create(string dbType)
        {
            return dbType == Convert.ToInt32(DatabaseType.Oracle).ToString()
                ? (IExecuteServiceData)new ExecuteServiceDataOrcl()
                : new ExecuteServiceData();
        }
    }
}
