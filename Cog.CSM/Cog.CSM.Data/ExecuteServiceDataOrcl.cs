using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Cog.CSM.Entity;
using Oracle.ManagedDataAccess.Client;

namespace Cog.CSM.Data
{
    public class ExecuteServiceDataOrcl: BusinessEntityBaseDAO, IExecuteServiceData
    {
        #region Constant variables

        private const string PackageName = "COSMO_CSM_EXECUTIVE_PACKAGE.";
        private static readonly string FnGetSendNotifications = $"{PackageName}FN_CWT_GetSendNotification";
        private static readonly string FnGetServiceLastStatus = $"{PackageName}FN_CWT_GetServiceLastStatus";
        private static readonly string SpSetMonitorStatus = $"{PackageName}SP_CWT_SetMonitorStatus";
        private static readonly string SpSetMailLog = $"{PackageName}SP_CWT_InsertCSMLog";

        #endregion Constant variables

        public void InsUpdMonitorService(ServiceEntity service, ContentManager content)
        {
            var pList = new List<OracleParameter>
            {
                GetParameter("p_SCH_ID", service.Sch_ID, OracleDbType.Int32, ParameterDirection.Input),
                GetParameter("p_CONFIG_ID", service.Config_ID, OracleDbType.Int32, ParameterDirection.Input),
                GetParameter("p_ENV_ID", service.Env_ID, OracleDbType.Int32, ParameterDirection.Input),
                GetParameter("p_MON_STATUS", content.Status, OracleDbType.Varchar2, ParameterDirection.Input),
                GetParameter("p_MON_START_DATE_TIME", content.StartTime, OracleDbType.Varchar2, ParameterDirection.Input),
                GetParameter("p_MON_END_DATE_TIME", content.CurrentTime, OracleDbType.Varchar2, ParameterDirection.Input),
                GetParameter("p_MON_IS_ACTIVE", 1, OracleDbType.Int32, ParameterDirection.Input),
                GetParameter("p_MON_CREATED_BY", "Admin", OracleDbType.Varchar2, ParameterDirection.Input),
                GetParameter("p_MON_CREATED_DATE", DateTime.Now, OracleDbType.TimeStamp, ParameterDirection.Input),
                GetParameter("p_MON_COMMENTS", content.Comments, OracleDbType.Varchar2, ParameterDirection.Input),
            };

            ExecuteNonQuery(SpSetMonitorStatus, pList);
        }

        public void InsUpdMonitorService(ServiceEntity service, Despatcher despatcher)
        {
            var pList = new List<OracleParameter>
            {
                GetParameter("p_SCH_ID", service.Sch_ID, OracleDbType.Int32, ParameterDirection.Input),
                GetParameter("p_CONFIG_ID", service.Config_ID, OracleDbType.Int32, ParameterDirection.Input),
                GetParameter("p_ENV_ID", service.Env_ID, OracleDbType.Int32, ParameterDirection.Input),
                GetParameter("p_MON_STATUS", despatcher.Status, OracleDbType.Varchar2, ParameterDirection.Input),
                GetParameter("p_MON_START_DATE_TIME", string.Empty, OracleDbType.Varchar2, ParameterDirection.Input),
                GetParameter("p_MON_END_DATE_TIME", $"{DateTime.Now:F}", OracleDbType.Varchar2, ParameterDirection.Input),
                GetParameter("p_MON_IS_ACTIVE", 1, OracleDbType.Int32, ParameterDirection.Input),
                GetParameter("p_MON_CREATED_BY", "Admin", OracleDbType.Varchar2, ParameterDirection.Input),
                GetParameter("p_MON_CREATED_DATE", DateTime.Now, OracleDbType.TimeStamp, ParameterDirection.Input),
                GetParameter("p_MON_COMMENTS", despatcher.Comments, OracleDbType.Varchar2, ParameterDirection.Input),
            };

            ExecuteNonQuery(SpSetMonitorStatus, pList);
        }

        public void InserrtMailLog(MailLogEntity mailLog)
        {
            var pList = new List<OracleParameter>
            {
                GetParameter("p_ENV_ID", mailLog.ENV_ID, OracleDbType.Int32, ParameterDirection.Input),
                GetParameter("p_CONFIG_ID", mailLog.Config_ID, OracleDbType.Int32, ParameterDirection.Input),
                GetParameter("p_EMTRAC_TO_ADDRESS", mailLog.To_Address, OracleDbType.Varchar2, ParameterDirection.Input),
                GetParameter("p_EMTRAC_CC_ADDRESS", mailLog.Cc_Address, OracleDbType.Varchar2, ParameterDirection.Input),
                GetParameter("p_EMTRAC_BCC_ADDRESS", mailLog.Bcc_Address, OracleDbType.Varchar2,
                    ParameterDirection.Input),
                GetParameter("p_EMTRAC_SUBJECT", mailLog.Subject, OracleDbType.Varchar2, ParameterDirection.Input),
                GetParameter("p_EMTRAC_BODY", mailLog.Body, OracleDbType.Varchar2, ParameterDirection.Input),
                GetParameter("p_EMTRAC_SEND_STATUS", mailLog.Status, OracleDbType.Varchar2, ParameterDirection.Input),
                GetParameter("p_EMTRAC_SEND_ERROR", mailLog.Error, OracleDbType.TimeStamp, ParameterDirection.Input),
                GetParameter("p_EMTRAC_CONTENT_TYPE", mailLog.ContentType, OracleDbType.Varchar2,
                    ParameterDirection.Input),
                GetParameter("p_EMTRAC_CREATED_BY", "Admin", OracleDbType.Varchar2, ParameterDirection.Input),
                GetParameter("p_EMTRAC_CREATED_DATE", DateTime.Now, OracleDbType.TimeStamp, ParameterDirection.Input),
                GetParameter("p_EMTRAC_COMMENTS", mailLog.Comments, OracleDbType.Varchar2, ParameterDirection.Input),
            };

            ExecuteNonQuery(SpSetMailLog, pList);
        }

        public List<SendNotificationEntity> GetAllSendMailNotification(DateTime beforeDateTime)
        {
            var pList = new List<OracleParameter>
            {
                GetParameter("p_CurrentTimeStamp", beforeDateTime, OracleDbType.TimeStamp,
                    ParameterDirection.Input),
            };
            var notificationList = ReadCompoundEntityList<SendNotificationEntity>(FnGetSendNotifications, pList, RowToSendNotificationList);
            return notificationList;
        }

        public string GetServiceLastStatus(ServiceEntity service)
        {
            var pList = new List<OracleParameter>
            {
                GetParameter("p_SCH_ID", service.Sch_ID, OracleDbType.Int32, ParameterDirection.Input),
                GetParameter("p_CONFIG_ID", service.Config_ID, OracleDbType.Int32, ParameterDirection.Input),
                GetParameter("p_ENV_ID", service.Env_ID, OracleDbType.Int32, ParameterDirection.Input),
            };
            var monitorStatus = ReadScalarValue(FnGetServiceLastStatus, pList, OracleDbType.Varchar2, 500).ToString();
            return monitorStatus == "null" ? string.Empty : monitorStatus;
        }

        private List<SendNotificationEntity> RowToSendNotificationList(OracleDataReader reader)
        {
            var list = new List<SendNotificationEntity>();

            while (reader.Read())
            {
                var entity = new SendNotificationEntity();
                if (CommonUtility.IsColumnExistsAndNotNull(reader, "ENV_ID"))
                    entity.Env_ID = Convert.ToInt32(reader["ENV_ID"]);
                if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_ID"))
                    entity.ConfigID = Convert.ToInt32(reader["CONFIG_ID"]);
                if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_SERVICE_TYPE"))
                    entity.ConfigServiceType = Convert.ToString(reader["CONFIG_SERVICE_TYPE"]);
                if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_IS_ACTIVE"))
                    entity.ConfigIsActive = Convert.ToBoolean(reader["CONFIG_IS_ACTIVE"]);
                if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_ISNOTIFY"))
                    entity.ConfigIsNotify = Convert.ToBoolean(reader["CONFIG_ISNOTIFY"]);
                if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_MAIL_FREQ"))
                    entity.ConfigMailFrequency = Convert.ToInt32(reader["CONFIG_MAIL_FREQ"]);
                if (CommonUtility.IsColumnExistsAndNotNull(reader, "MON_STATUS"))
                    entity.LastMonitorStatus = Convert.ToString(reader["MON_STATUS"]);
                if (CommonUtility.IsColumnExistsAndNotNull(reader, "EMTRAC_CREATED_DATE"))
                    entity.LastMonitorUpdated = Convert.ToDateTime(reader["EMTRAC_CREATED_DATE"]);


                list.Add(entity);
            }

            return list;
        }
    }
}
