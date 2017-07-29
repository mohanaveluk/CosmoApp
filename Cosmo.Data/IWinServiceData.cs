using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cosmo.Entity;

namespace Cosmo.Data
{
    public interface IWinServiceData
    {
        int InsGroup(GroupEntity group);
        int InsGroupDetail(GroupDetailEntity groupDetail, string configIDs);

        int InsGroupScheduleDetail(GroupScheduleEntity groupScheduleEntity, string envIDs, string configIDs, string winServiceIDs);
        List<GroupEntity> GetGroupID(string grpName);
        List<GroupEntity> GetAllGroup(int grp_ID);
        List<GroupDetailEntity> GetGroupDetail(int grp_ID, int env_ID);
        List<GroupScheduleEntity> GetAllGroupSchedules(int grp_ID, int grp_sch_ID, DateTime startTime);
        List<GroupScheduleEntity> GetGroupScheduleDetails(int grpId, string category, int envId);
        List<GroupScheduleDetailEntity> GetGroupScheduleEnvDetails(int grpId, string category, int envId);

        List<GroupScheduleServiceDetailEntity> GetGroupScheduleServiceDetails(int grpId, string category, int envId,
            char grpStatus);

        List<ServiceOnDemandMailEntity> GetServiceConfigurationOndemand(int winSerId);
        List<GroupcheduleReportEntity> GetGroupScheduleReport(string schType, DateTime startDate, DateTime endDate);
        void CancelGroupSchedule(string type, string vId);
    }

    public interface IWinServiceDataFactory
    {
        IWinServiceData Create(string dbType);
    }

    public class WinServiceDataFactory : IWinServiceDataFactory
    {
        public IWinServiceData Create(string dbType)
        {
            return dbType == Convert.ToInt32(DatabaseType.Oracle).ToString()
                ? (IWinServiceData)new WinServiceDataOrcl()
                : new WinServiceData();
        }
    }

}
