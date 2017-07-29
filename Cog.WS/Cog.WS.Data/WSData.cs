using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using Cog.WS.Entity;

namespace Cog.WS.Data
{
    public class WSData: IWSData
    {
        #region Constant variables

        private const string InsertMaillog = "CWT_InsertMailLog";
        private const string Getuseremail = "CWT_getUserEmailList";
        private const string GetGroupServiceSchedule = "CWT_GetGroupOpenScheduledServceDetails";
        private const string UpdGroupServiceSchedule = "CWT_UpdateServiceStatus";
        private const string UpdGroupSchedule = "CWT_UpdateGroupScheduleStatus";
        private const string SetServiceAcknowledge = "CWT_SetServiceAcknowledge";
        #endregion Constant variables

        #region Get all group Open schedule details for mail

        /// <summary>
        /// Get all the group Open schedules which are based on the start date time provided
        /// </summary>
        /// <param name="grp_ID"></param>
        /// <param name="currentDate"></param>
        /// <param name="status"></param>
        /// <param name="category"></param>
        /// <param name="groupSchId"></param>
        /// <param name="envId"></param>
        /// <returns></returns>
        public List<GroupScheduleEntity> GetGroupOpenScheduleDetails(DateTime currentDate, string status, string category, int groupSchId, int envId)
        {
            List<SqlParameter> pList = new List<SqlParameter>();
            try
            {
                pList.Add(new SqlParameter("@CURRENTDATETIME", currentDate));
                pList.Add(new SqlParameter("@SCHEDULE_STATUS", status));
                pList.Add(new SqlParameter("@Category", category));
                pList.Add(new SqlParameter("@GROUP_SCH_ID", groupSchId));
                pList.Add(new SqlParameter("@ENV_ID", envId));
                return UtilityDL.FillData<GroupScheduleEntity>(GetGroupServiceSchedule, pList);
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
        /// <param name="currentDate"></param>
        /// <param name="status"></param>
        /// <param name="category"></param>
        /// <param name="groupSchId"></param>
        /// <param name="envId"></param>
        /// <returns></returns>
        public List<GroupScheduleDetailEntity> GetGroupOpenScheduleEnvDetails(DateTime currentDate, string status, string category, int groupSchId, int envId)
        {
            List<SqlParameter> pList = new List<SqlParameter>();
            try
            {
                pList.Add(new SqlParameter("@CURRENTDATETIME", currentDate));
                pList.Add(new SqlParameter("@SCHEDULE_STATUS", status));
                pList.Add(new SqlParameter("@Category", category));
                pList.Add(new SqlParameter("@GROUP_SCH_ID", groupSchId));
                pList.Add(new SqlParameter("@ENV_ID", envId));
                return UtilityDL.FillData<GroupScheduleDetailEntity>(GetGroupServiceSchedule, pList);
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
        public List<GroupScheduleServiceDetailEntity> GetGroupOpenScheduleServiceDetails(DateTime currentDate, string status, string category, int groupSchID, int envID)
        {
            List<SqlParameter> pList = new List<SqlParameter>();
            try
            {
                pList.Add(new SqlParameter("@CURRENTDATETIME", currentDate));
                pList.Add(new SqlParameter("@SCHEDULE_STATUS", status));
                pList.Add(new SqlParameter("@Category", category));
                pList.Add(new SqlParameter("@GROUP_SCH_ID", groupSchID));
                pList.Add(new SqlParameter("@ENV_ID", envID));
                string stProc = GetGroupServiceSchedule;
                return UtilityDL.FillData<GroupScheduleServiceDetailEntity>(stProc, pList);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion Get all group Open schedule Service details for mail

        #region Update windows service status

        public int UpdateScheduleServiceStatus(GroupScheduleServiceDetailEntity details)
        {
            int result = 0;
            List<SqlParameter> pList = new List<SqlParameter>();
            try
            {
                pList.Add(new SqlParameter("@GROUP_SERVICE_SCH_ID", details.Group_Schedule_Detail_ID));
                pList.Add(new SqlParameter("@GROUP_SERVICE_SCH_STATUS", details.Schedule_Status));
                pList.Add(new SqlParameter("@GROUP_SCH_UPDATEDTIME", details.Schedule_UpdatedTime));
                pList.Add(new SqlParameter("@GROUP_SCH_SERVICE_STARTTIME", details.Group_Schedule_StartedTime));
                pList.Add(new SqlParameter("@GROUP_SCH_SERVICE_COMPLETEDTIME", details.Schedule_UpdatedTime));
                string stProc = UpdGroupServiceSchedule;
                UtilityDL.ExecuteNonQuery(stProc, pList);
                result = 1;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        #endregion Update windows service status

        #region Update windows service status

        public int UpdateGroupScheduleStatus(GroupScheduleEntity details)
        {
            int result = 0;
            var pList = new List<SqlParameter>();
            try
            {
                pList.Add(new SqlParameter("@GROUP_SCH_ID", details.Group_Schedule_ID));
                pList.Add(new SqlParameter("@GROUP_SCH_STATUS", details.Group_Schedule_Status));
                pList.Add(new SqlParameter("@GROUP_SCH_COMPLETED_TIME", details.Group_Schedule_CompletedTime));
                pList.Add(new SqlParameter("@GROUP_SCH_UPDATED_BY", details.Group_Schedule_UpdatedBy));
                pList.Add(new SqlParameter("@GROUP_SCH_UPDATED_DATETIME", details.Group_Schedule_UpdatedDatetime));
                string stProc = UpdGroupSchedule;
                UtilityDL.ExecuteNonQuery(stProc, pList);
                result = 1;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        #endregion Update windows service status

        #region insert mail log
        /// <summary>
        /// Insert mail log details about the monitor service failures
        /// </summary>
        /// <param name="mailLog"></param>
        public void InserrtMailLog(MailLogEntity mailLog)
        {
            try
            {
                var pList = new List<SqlParameter>
                {
                    new SqlParameter("@ENV_ID", mailLog.ENV_ID),
                    new SqlParameter("@CONFIG_ID", mailLog.Config_ID),
                    new SqlParameter("@EMTRAC_TO_ADDRESS", mailLog.To_Address),
                    new SqlParameter("@EMTRAC_CC_ADDRESS", mailLog.Cc_Address),
                    new SqlParameter("@EMTRAC_BCC_ADDRESS",
                        string.IsNullOrEmpty(mailLog.Bcc_Address) ? string.Empty : mailLog.Bcc_Address),
                    new SqlParameter("@EMTRAC_SUBJECT", mailLog.Subject),
                    new SqlParameter("@EMTRAC_BODY", mailLog.Body),
                    new SqlParameter("@EMTRAC_SEND_STATUS", mailLog.Status),
                    new SqlParameter("@EMTRAC_SEND_ERROR",
                        string.IsNullOrEmpty(mailLog.Error) ? string.Empty : mailLog.Error),
                    new SqlParameter("@EMTRAC_CONTENT_TYPE",
                        string.IsNullOrEmpty(mailLog.ContentType) ? string.Empty : mailLog.ContentType),
                    new SqlParameter("@EMTRAC_CREATED_BY", "Admin"),
                    new SqlParameter("@EMTRAC_CREATED_DATE", DateTime.Now),
                    new SqlParameter("@EMTRAC_COMMENTS", mailLog.Comments)
                };
                UtilityDL.ExecuteNonQuery(InsertMaillog, pList);

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        #endregion insert mail log

        #region getUserEmailList

        /// <summary>
        /// Get user email list with respect to the environment id
        /// </summary>
        /// <param name="envId"></param>
        /// <param name="messageType"></param>
        /// <returns></returns>
        public List<UserEmailEntity> GetUserEMail(int envId, string messageType)
        {
            var pList = new List<SqlParameter>
            {
                new SqlParameter("@Env_ID", envId),
                new SqlParameter("@MessageType", messageType)
            };
            string stProc = Getuseremail;
            return UtilityDL.FillData<UserEmailEntity>(stProc, pList);
        }
        #endregion getUserEmailList


        #region Service acknowledge and stop/start
        /// <summary>
        /// To update the acknowledge and change of email alert type for the service montor
        /// </summary>
        /// <param name="ackData"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public int InsUpdateServiceAcknowledge(AcknowledgeEntity ackData, string mode)
        {
            int recInsert = 0;
            try
            {
                var pList = new List<SqlParameter>
                {
                    new SqlParameter("@ENV_ID", ackData.EnvId),
                    new SqlParameter("@CONFIG_ID", ackData.ConfigId),
                    new SqlParameter("@MON_ID", ackData.MonId),
                    new SqlParameter("@ACK_ISACKNOWLEDGE", ackData.IsAcknowledgeMode),
                    new SqlParameter("@ACK_ALERT", ackData.AcknowledgeAlertChange),
                    new SqlParameter("@ACK_COMMENTS", ackData.AcknowledgeComments),
                    new SqlParameter("@CREATED_BY", ackData.CreatedBy),
                    new SqlParameter("@CREATED_DATE", ackData.CreatedDate)
                };

                UtilityDL.ExecuteNonQuery(SetServiceAcknowledge, pList);
                recInsert = 0;
            }
            catch (Exception ex)
            {
                recInsert = 1;
                throw ex;
            }
            return recInsert;
        }
        
        #endregion Service acknowledge and stop/start

    }
}
