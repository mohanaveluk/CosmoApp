using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cog.WS.Entity;

namespace Cog.WS.Data
{
    public interface IWSDataFactory
    {
        IWSData Create(string dbType);
    }

    public class WSDataFactory : IWSDataFactory
    {
        public IWSData Create(string dbType)
        {
            return dbType == Convert.ToInt32(DatabaseType.Oracle).ToString()
                ? (IWSData)new WSDataOrcl()
                : new WSData();
        }
    }


    public interface IWSData
    {
        List<GroupScheduleEntity> GetGroupOpenScheduleDetails(DateTime currentDate, string status, string category,
            int groupSchId, int envId);

        List<GroupScheduleDetailEntity> GetGroupOpenScheduleEnvDetails(DateTime currentDate, string status,
            string category, int groupSchId, int envId);

        List<GroupScheduleServiceDetailEntity> GetGroupOpenScheduleServiceDetails(DateTime currentDate, string status,
            string category, int groupSchID, int envID);

        int UpdateScheduleServiceStatus(GroupScheduleServiceDetailEntity details);
        int UpdateGroupScheduleStatus(GroupScheduleEntity details);
        void InserrtMailLog(MailLogEntity mailLog);
        List<UserEmailEntity> GetUserEMail(int envId, string messageType);
        int InsUpdateServiceAcknowledge(AcknowledgeEntity ackData, string mode);
    }
}
