using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cog.CSM.Entity;
using System.Data.SqlClient;

namespace Cog.CSM.Data
{
    public class WindowsServiceData
    {
        #region Constant variables

        private const string GET_GROUP_SERVICE_SCHEDULE = "CWT_GetGroupOpenScheduledServceDetails";

        #endregion Constant variables

        #region Get all group Open schedule details for mail
        /// <summary>
        /// Get all the group Open schedules which are based on the start date time provided
        /// </summary>
        /// <param name="grp_ID"></param>
        /// <returns></returns>
        public List<GroupScheduleEntity> GetGroupOpenScheduleDetails(DateTime currentDate, string status, string category)
        {
            List<SqlParameter> pList = new List<SqlParameter>();
            try
            {
                pList.Add(new SqlParameter("@CURRENTDATETIME", currentDate));
                pList.Add(new SqlParameter("@SCHEDULE_STATUS", status));
                pList.Add(new SqlParameter("@Category", category));
                string stProc = GET_GROUP_SERVICE_SCHEDULE;
                return UtilityDL.FillData<GroupScheduleEntity>(stProc, pList);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion Get all group Open schedule details for mail

        #region Get all group Open schedule environment details for mail
        /// <summary>
        /// Get all the group schedules which are based on the start date time provided
        /// </summary>
        /// <param name="grp_ID"></param>
        /// <returns></returns>
        public List<GroupScheduleDetailEntity> GetGroupOpenScheduleEnvDetails(DateTime currentDate, string status, string category)
        {
            List<SqlParameter> pList = new List<SqlParameter>();
            try
            {
                pList.Add(new SqlParameter("@CURRENTDATETIME", currentDate));
                pList.Add(new SqlParameter("@SCHEDULE_STATUS", status));
                pList.Add(new SqlParameter("@Category", category));
                string stProc = GET_GROUP_SERVICE_SCHEDULE;
                return UtilityDL.FillData<GroupScheduleDetailEntity>(stProc, pList);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion Get all group Open schedule environment details for mail

        #region Get all group Open schedule Service details for mail
        /// <summary>
        /// Get all the group Open schedules which are based on the start date time provided
        /// </summary>
        /// <param name="grp_ID"></param>
        /// <returns></returns>
        public List<GroupScheduleServiceDetailEntity> GetGroupOpenScheduleServiceDetails(DateTime currentDate, string status, string category)
        {
            List<SqlParameter> pList = new List<SqlParameter>();
            try
            {
                pList.Add(new SqlParameter("@CURRENTDATETIME", currentDate));
                pList.Add(new SqlParameter("@SCHEDULE_STATUS", status));
                pList.Add(new SqlParameter("@Category", category));
                string stProc = GET_GROUP_SERVICE_SCHEDULE;
                return UtilityDL.FillData<GroupScheduleServiceDetailEntity>(stProc, pList);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion Get all group Open schedule Service details for mail

    }
}
