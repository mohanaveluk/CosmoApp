using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cosmo.Entity;
using System.Data.SqlClient;
using System.Data;

namespace Cosmo.Data
{
    public class WinServiceData : IWinServiceData
    {
        #region Constant variables
        private const string SET_GROUP = "CWT_InsGroup";
        private const string GET_GROUP = "CWT_GetGroup";
        private const string GET_GROUPID = "CWT_GetGroupID";
        private const string SET_GROUP_DETAIL = "CWT_InsGroupDetail";
        private const string GET_GROUP_DETAIL = "CWT_GetGroupDetail";
        private const string SET_GROUP_SCHEDULE = "CWT_InsGroupSchedule";
        private const string GET_GROUP_SCHEDULE = "CWT_GetGroupSchedule";
        private const string GET_GROUP_SERVICE_SCHEDULE = "CWT_GetGroupScheduleServceDetails";
        private const string GET_GETSERVICECONFIGURATIONONDEMAND = "CWT_GetWindowsServiceConfigurationOnDemand";
        private const string GET_REPORTGROUPSCHEDULE = "CWT_ReportRestartService";
        private const string SetDeleterecord = "CWT_DeleteRecord";

        #endregion Constant variables

        #region Insert Group name in to database
        /// <summary>
        /// Insert Group name in to database
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        public int InsGroup(GroupEntity group)
        {
            int recInsert = 0;
            try
            {
                List<SqlParameter> pList = new List<SqlParameter>();
                pList.Add(new SqlParameter("@GROUP_ID", group.Group_ID));
                pList.Add(new SqlParameter("@GROUP_NAME", group.Group_Name));
                pList.Add(new SqlParameter("@GROUP_CREATED_BY", group.CreatedBy));
                pList.Add(new SqlParameter("@GROUP_CREATED_DATE", System.DateTime.Now));
                pList.Add(new SqlParameter("@GROUP_COMMENTS", string.IsNullOrEmpty( group.Comments) ? string.Empty: group.Comments));
                pList.Add(new SqlParameter("@GROUP_ISACTIVE", true));
                UtilityDL.ExecuteNonQuery(SET_GROUP, pList);
                recInsert = 0;
            }
            catch (Exception)
            {
                recInsert = 1;
                throw;
            }
            return recInsert;
        }
        #endregion Insert Group name in to database

        #region Insert Group details in to database
        /// <summary>
        /// Insert Group schedule details in to database
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        public int InsGroupDetail(GroupDetailEntity groupDetail, string configIDs)
        {
            int recInsert = 0;
            try
            {
                List<SqlParameter> pList = new List<SqlParameter>();
                pList.Add(new SqlParameter("@GROUP_ID", groupDetail.Group_ID));
                pList.Add(new SqlParameter("@ENV_ID", groupDetail.Env_ID));
                pList.Add(new SqlParameter("@SERVICE_IDS", configIDs));
                pList.Add(new SqlParameter("@WIN_SERVICE_ID", groupDetail.WinService_ID));
                pList.Add(new SqlParameter("@GROUP_CREATED_BY", groupDetail.CreatedBy));
                pList.Add(new SqlParameter("@GROUP_CREATED_DATE", System.DateTime.Now));
                pList.Add(new SqlParameter("@GROUP_DETAIL_COMMENTS", string.IsNullOrEmpty(groupDetail.Comments) ? string.Empty : groupDetail.Comments));
                pList.Add(new SqlParameter("@GROUP_ISACTIVE", true));
                UtilityDL.ExecuteNonQuery(SET_GROUP_DETAIL, pList);
                recInsert = 0;
            }
            catch (Exception)
            {
                recInsert = 1;
                throw;
            }
            return recInsert;
        }
        #endregion Insert Group details in to database

        #region Insert Group schedule details in to database

        /// <summary>
        /// Insert Group details in to database
        /// </summary>
        /// <param name="groupScheduleEntity"></param>
        /// <param name="envIDs"></param>
        /// <param name="configIDs"></param>
        /// <param name="winServiceIDs"></param>
        /// <param name="group"></param>
        /// <returns></returns>
        public int InsGroupScheduleDetail(GroupScheduleEntity groupScheduleEntity, string envIDs, string configIDs, string winServiceIDs)
        {
            int recInsert = 0;
            try
            {
                SqlParameter retVal = new SqlParameter("@SCOPE_OUTPUT", SqlDbType.Int);
                retVal.Direction = ParameterDirection.Output;
                List<SqlParameter> pList = new List<SqlParameter>();
                pList.Add(new SqlParameter("@GROUP_SCH_ID", groupScheduleEntity.Group_Schedule_ID));
                pList.Add(new SqlParameter("@GROUP_ID", groupScheduleEntity.Group_ID));
                pList.Add(new SqlParameter("@GROUP_NAME", groupScheduleEntity.Group_Name)); 
                pList.Add(new SqlParameter("@ENV_IDS", envIDs));
                pList.Add(new SqlParameter("@CONFIG_IDS", configIDs));
                pList.Add(new SqlParameter("@WIN_SERVICE_IDS", winServiceIDs));
                pList.Add(new SqlParameter("@GROUP_SCH_ACTION", groupScheduleEntity.Group_Schedule_Action));
                pList.Add(new SqlParameter("@GROUP_SCH_STATUS", groupScheduleEntity.Group_Schedule_Status.Trim()));
                pList.Add(new SqlParameter("@GROUP_SCH_TIME", groupScheduleEntity.Group_Schedule_Datatime));
                pList.Add(new SqlParameter("@GROUP_SCH_COMMENTS", string.IsNullOrEmpty(groupScheduleEntity.Group_Schedule_Comments) ? string.Empty : groupScheduleEntity.Group_Schedule_Comments));
                pList.Add(new SqlParameter("@GROUP_SCH_CREATED_BY", groupScheduleEntity.Group_Schedule_CreatedBy));
                pList.Add(new SqlParameter("@GROUP_SCH_CREATED_DATETIME", groupScheduleEntity.Group_Schedule_CreatedDatetime));
                pList.Add(new SqlParameter("@GROUP_SCH_ISACTIVE", true));
                pList.Add(new SqlParameter("@GROUP_SCH_ONDEMAND", groupScheduleEntity.Group_Schedule_OnDemand));
                pList.Add(new SqlParameter("@GROUP_SCH_COMPLETESTATUS", groupScheduleEntity.Group_Schedule_CompleteStatus));
                pList.Add(new SqlParameter("@GROUP_SCH_COMPLETEDTIME", groupScheduleEntity.Group_Schedule_CompletedTime));
                pList.Add(new SqlParameter("@GROUP_SCH_REQUESTSOURCE", groupScheduleEntity.RequestSource ?? "Desktop"));
                pList.Add(new SqlParameter("@GROUP_SCH_SERVICE_STARTTIME", groupScheduleEntity.ServiceStartedTime ?? DateTime.Now));
                pList.Add(new SqlParameter("@GROUP_SCH_SERVICE_COMPLETEDTIME", groupScheduleEntity.ServiceCompletionTime ?? DateTime.Now));
                pList.Add(retVal);
                recInsert = Convert.ToInt32(UtilityDL.ExecuteNonQuery(SET_GROUP_SCHEDULE, pList, true));
            }
            catch (Exception)
            {
                recInsert = -1;
                throw;
            }
            return recInsert;
        }
        #endregion Insert Group schedule details in to database


        #region Get group ID
        /// <summary>
        /// Get group ID based on the group Name
        /// </summary>
        /// <param name="grpName"></param>
        /// <returns></returns>
        public List<GroupEntity> GetGroupID(string grpName)
        {
            List<SqlParameter> pList = new List<SqlParameter>();
            try
            {
                pList.Add(new SqlParameter("@GROUP_NAME", grpName));
                string stProc = GET_GROUPID;
                return UtilityDL.FillData<GroupEntity>(stProc, pList);
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion Get group ID


        #region Get all groups
        /// <summary>
        /// Get all the group name based on the group ID
        /// </summary>
        /// <param name="grp_ID"></param>
        /// <returns></returns>
        public List<GroupEntity> GetAllGroup(int grp_ID)
        {
            List<SqlParameter> pList = new List<SqlParameter>();
            try
            {
                pList.Add(new SqlParameter("@GRP_ID", grp_ID));
                string stProc = GET_GROUP;
                return UtilityDL.FillData<GroupEntity>(stProc, pList);
            }
            catch (Exception)
            {
                
                throw;
            }
        }
        #endregion Get all groups

        #region Get group detail
        /// <summary>
        /// Get all the group detail based on the group ID and env id
        /// </summary>
        /// <param name="grp_ID"></param>
        /// <returns></returns>
        public List<GroupDetailEntity> GetGroupDetail(int grp_ID, int env_ID)
        {
            List<SqlParameter> pList = new List<SqlParameter>();
            try
            {
                pList.Add(new SqlParameter("@GROUP_ID", grp_ID));
                pList.Add(new SqlParameter("@ENV_ID", env_ID));
                string stProc = GET_GROUP_DETAIL;
                return UtilityDL.FillData<GroupDetailEntity>(stProc, pList);
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion Get all groups

        #region Get all groups schedule details
        /// <summary>
        /// Get all the group schedules which are based on the start date time provided
        /// </summary>
        /// <param name="grp_ID"></param>
        /// <returns></returns>
        public List<GroupScheduleEntity> GetAllGroupSchedules(int grp_ID, int grp_sch_ID, DateTime startTime)
        {
            List<SqlParameter> pList = new List<SqlParameter>();
            try
            {
                pList.Add(new SqlParameter("GROUP_ID", grp_ID));
                pList.Add(new SqlParameter("@GROUP_SCH_ID", grp_sch_ID));
                pList.Add(new SqlParameter("@CURRENTDATETIME", startTime));
                string stProc = GET_GROUP_SCHEDULE;
                return UtilityDL.FillData<GroupScheduleEntity>(stProc, pList);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion Get all groups schedule details

        #region Get all groups schedule details for mail

        /// <summary>
        /// Get all the group schedules which are based on the start date time provided
        /// </summary>
        /// <param name="grpId"></param>
        /// <param name="category"></param>
        /// <param name="envId"></param>
        /// <returns></returns>
        public List<GroupScheduleEntity> GetGroupScheduleDetails(int grpId, string category, int envId)
        {
            List<SqlParameter> pList = new List<SqlParameter>();
            try
            {
                pList.Add(new SqlParameter("@GROUP_SCH_ID", grpId));
                pList.Add(new SqlParameter("@Category", category));
                pList.Add(new SqlParameter("@ENV_ID", envId));
                pList.Add(new SqlParameter("@GROUP_SCH_STATUS", string.Empty));
                string stProc = GET_GROUP_SERVICE_SCHEDULE;
                return UtilityDL.FillData<GroupScheduleEntity>(stProc, pList);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion Get all groups schedule details for mail

        #region Get all groups schedule environment details for mail

        /// <summary>
        /// Get all the group schedules which are based on the start date time provided
        /// </summary>
        /// <param name="grpId"></param>
        /// <param name="category"></param>
        /// <param name="envId"></param>
        /// <returns></returns>
        public List<GroupScheduleDetailEntity> GetGroupScheduleEnvDetails(int grpId, string category, int envId)
        {
            List<SqlParameter> pList = new List<SqlParameter>();
            try
            {
                pList.Add(new SqlParameter("@GROUP_SCH_ID", grpId));
                pList.Add(new SqlParameter("@Category", category));
                pList.Add(new SqlParameter("@ENV_ID", envId));
                pList.Add(new SqlParameter("@GROUP_SCH_STATUS", string.Empty));
                string stProc = GET_GROUP_SERVICE_SCHEDULE;
                return UtilityDL.FillData<GroupScheduleDetailEntity>(stProc, pList);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion Get all groups schedule environment details for mail

        #region Get all groups schedule Service details for mail

        /// <summary>
        /// Get all the group schedules which are based on the start date time provided
        /// </summary>
        /// <param name="grpId"></param>
        /// <param name="category"></param>
        /// <param name="envId"></param>
        /// <param name="grpStatus"></param>
        /// <returns></returns>
        public List<GroupScheduleServiceDetailEntity> GetGroupScheduleServiceDetails(int grpId, string category, int envId, char grpStatus)
        {
            List<SqlParameter> pList = new List<SqlParameter>();
            try
            {
                pList.Add(new SqlParameter("@GROUP_SCH_ID", grpId));
                pList.Add(new SqlParameter("@Category", category));
                pList.Add(new SqlParameter("@ENV_ID", envId));
                pList.Add(new SqlParameter("@GROUP_SCH_STATUS", grpStatus));
                string stProc = GET_GROUP_SERVICE_SCHEDULE;
                return UtilityDL.FillData<GroupScheduleServiceDetailEntity>(stProc, pList);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion Get all groups schedule Service details for mail

        #region Get Window Service configuration details of OnDemand for Mail

        public List<ServiceOnDemandMailEntity> GetServiceConfigurationOndemand(int winSerId)
        {
            List<SqlParameter> pList = new List<SqlParameter>();
            try
            {
                pList.Add(new SqlParameter("@WIN_SERVICE_ID", winSerId));
                string stProc = GET_GETSERVICECONFIGURATIONONDEMAND;
                return UtilityDL.FillData<ServiceOnDemandMailEntity>(stProc, pList);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        #endregion Get Window Service configuration details of OnDemand for Mail

        #region Get report of Group schedule details

        public List<GroupcheduleReportEntity> GetGroupScheduleReport(string schType, DateTime startDate, DateTime endDate)
        {
            try
            {
                var pList = new List<SqlParameter>
                {
                    new SqlParameter("@SCHEDULETYPE", schType),
                    new SqlParameter("@STARTDATE", startDate),
                    new SqlParameter("@ENDDATE", endDate)
                };
                return UtilityDL.FillData<GroupcheduleReportEntity>(GET_REPORTGROUPSCHEDULE, pList);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion Get report of Group schedule details

        #region Cancel the group schedule and detail based on the GROUP_SCH_ID
        /// <summary>
        /// Soft delete a group scheduler amd detail based on the GROUP_SCH_ID
        /// </summary>
        /// <param name="type"></param>
        /// <param name="vId"></param>
        public void CancelGroupSchedule(string type, string vId)
        {
            List<SqlParameter> pList = new List<SqlParameter>
            {
                new SqlParameter("@sID", vId),
                new SqlParameter("@sType", type)
            };
            UtilityDL.ExecuteNonQuery(SetDeleterecord, pList);
        }
        #endregion Cancel the group schedule and detail based on the GROUP_SCH_ID
    }
}
