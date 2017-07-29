using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Cog.CSM.Entity;
using System.Configuration;
using System.Data.Sql;

namespace Cog.CSM.Data
{
    public class ExecuteServiceData: IExecuteServiceData
    {
        #region Constant variables

        private const string SET_MONITORSTATUS = "CWT_SetMonitorStatus";
        private const string INSERT_MAILLOG = "CWT_InsertMailLog";
        private const string GET_SENDNOTIFICATION = "CWT_GetSendNotification";
        private const string Get_ServiceLastStatus = "CWT_GetServiceLastStatus";

        #endregion

        #region Insert or update the content service monitor status
        /// <summary>
        /// Insert or update the content service monitor status
        /// </summary>
        public void InsUpdMonitorService(ServiceEntity service, ContentManager content)
        {
            var pList = new List<SqlParameter>
            {
                new SqlParameter("@SCH_ID", service.Sch_ID),
                new SqlParameter("@CONFIG_ID", service.Config_ID),
                new SqlParameter("@ENV_ID", service.Env_ID),
                new SqlParameter("@MON_STATUS", content.Status),
                new SqlParameter("@MON_START_DATE_TIME", content.StartTime),
                new SqlParameter("@MON_END_DATE_TIME", content.CurrentTime),
                new SqlParameter("@MON_IS_ACTIVE", true),
                new SqlParameter("@MON_CREATED_BY", "Admin"),
                new SqlParameter("@MON_CREATED_DATE", DateTime.Now),
                new SqlParameter("@MON_COMMENTS", content.Comments)
            };
            UtilityDL.ExecuteNonQuery(SET_MONITORSTATUS, pList);
        }
        #endregion Insert or update the content service monitor status

        #region insert / update monitor service
        /// <summary>
        /// Insert or update the despatcher service monitor status
        /// </summary>
        public void InsUpdMonitorService(ServiceEntity service, Despatcher despatcher)
        {
            var pList = new List<SqlParameter>
            {
                new SqlParameter("@SCH_ID", service.Sch_ID),
                new SqlParameter("@CONFIG_ID", service.Config_ID),
                new SqlParameter("@ENV_ID", service.Env_ID),
                new SqlParameter("@MON_STATUS", despatcher.Status),
                new SqlParameter("@MON_START_DATE_TIME", string.Empty),
                new SqlParameter("@MON_END_DATE_TIME", $"{DateTime.Now:F}"),
                new SqlParameter("@MON_IS_ACTIVE", true),
                new SqlParameter("@MON_CREATED_BY", "Admin"),
                new SqlParameter("@MON_CREATED_DATE", DateTime.Now),
                new SqlParameter("@MON_COMMENTS", despatcher.Comments)
            };
            UtilityDL.ExecuteNonQuery(SET_MONITORSTATUS, pList);
        }
        #endregion insert / update monitor service

        #region insert mail log
        /// <summary>
        /// Insert mail log details about the monitor service failures
        /// </summary>
        /// <param name="mailLog"></param>
        public void InserrtMailLog(MailLogEntity mailLog)
        {
            try
            {
                List<SqlParameter> pList = new List<SqlParameter>();
                pList.Add(new SqlParameter("@ENV_ID", mailLog.ENV_ID));
                pList.Add(new SqlParameter("@CONFIG_ID", mailLog.Config_ID));
                pList.Add(new SqlParameter("@EMTRAC_TO_ADDRESS", mailLog.To_Address));
                pList.Add(new SqlParameter("@EMTRAC_CC_ADDRESS", mailLog.Cc_Address));
                pList.Add(new SqlParameter("@EMTRAC_BCC_ADDRESS", mailLog.Bcc_Address));
                pList.Add(new SqlParameter("@EMTRAC_SUBJECT", mailLog.Subject));
                pList.Add(new SqlParameter("@EMTRAC_BODY", mailLog.Body));
                pList.Add(new SqlParameter("@EMTRAC_SEND_STATUS", mailLog.Status));
                pList.Add(new SqlParameter("@EMTRAC_SEND_ERROR", mailLog.Error));
                pList.Add(new SqlParameter("@EMTRAC_CONTENT_TYPE", mailLog.ContentType));
                pList.Add(new SqlParameter("@EMTRAC_CREATED_BY", "Admin"));
                pList.Add(new SqlParameter("@EMTRAC_CREATED_DATE", DateTime.Now));
                pList.Add(new SqlParameter("@EMTRAC_COMMENTS", mailLog.Comments));
                UtilityDL.ExecuteNonQuery(INSERT_MAILLOG, pList);
            
            }
            catch (Exception ex)
            {
                
                throw ex; 
            }
        }
        #endregion insert mail log

        /// <summary>
        /// Get al lthe service details of send notification
        /// </summary>
        /// <param name="beforeDateTime"></param>
        /// <returns></returns>
        public List<SendNotificationEntity> GetAllSendMailNotification(DateTime beforeDateTime)
        {
            List<SqlParameter> pList = new List<SqlParameter>();
            pList.Add(new SqlParameter("@CurrentTimeStamp", beforeDateTime));
            return UtilityDL.FillData<SendNotificationEntity>(GET_SENDNOTIFICATION, pList);
        }

        public string GetServiceLastStatus(ServiceEntity service)
        {
            var pList = new List<SqlParameter>
            {
                new SqlParameter("@SCH_ID", service.Sch_ID),
                new SqlParameter("@CONFIG_ID", service.Config_ID),
                new SqlParameter("@ENV_ID", service.Env_ID)
            };

            return (string) UtilityDL.ReadScalar(Get_ServiceLastStatus, pList);
        }

    }
}
