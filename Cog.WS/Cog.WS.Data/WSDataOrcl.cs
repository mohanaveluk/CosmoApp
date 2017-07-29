using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using Cog.WS.Entity;
using Oracle.ManagedDataAccess.Client;

namespace Cog.WS.Data
{
    public class WSDataOrcl: BusinessEntityBaseDAO, IWSData
    {
        #region Constant variables
        private const string PackageName = "COSMO_MONITOR_PACKAGE.";
        private const string PackageNameExecutive = "COSMO_CSM_EXECUTIVE_PACKAGE.";
        private const string PackageNameWinService = "COSMO_WINSERVICE_PACKAGE.";
        private const string PackageNameSchedular = "COSMO_SCHEDULER_PACKAGE.";


        private static readonly string FnGroupScheduleServiceDetails = string.Format("{0}FN_GetGroupOpenSchDetails", PackageNameSchedular); //GetGroupOpenScheduleServiceDetails
        private static readonly string SpGroupScheduleStatus = string.Format("{0}SP_CWT_UpdateGroupSchStatus", PackageNameSchedular); //CWT_UpdateGroupScheduleStatus
        private static readonly string SpGroupScheduleServiceStatus = string.Format("{0}SP_CWT_UpdateServiceStatus", PackageNameSchedular); //CWT_UpdateGroupScheduleStatus
        private static readonly string SpSetMailLog = $"{PackageNameExecutive}SP_CWT_InsertMailLog";
        private static readonly string SetSpServiceAcknowledge = string.Format("{0}SP_CWT_SetServiceAcknowledge", PackageName);

        private static readonly string GetUserEmailList = string.Format("{0}FN_CWT_GetUserEmailList", PackageName);

        #region Constants
        private string CONTENTSERVICE = ConfigurationManager.AppSettings["ContentService"].ToString();
        private string DESPATCHER = ConfigurationManager.AppSettings["DespatcherService"].ToString();
        #endregion Constants



        #endregion Constant variables

        public List<GroupScheduleEntity> GetGroupOpenScheduleDetails(DateTime currentDate, string status, string category, int groupSchId, int envId)
        {
            var pList = new List<OracleParameter>
                {
                    GetParameter("p_CURRENTDATETIME", currentDate, OracleDbType.TimeStamp, ParameterDirection.Input),
                    GetParameter("p_SCHEDULE_STATUS", status, OracleDbType.Char, ParameterDirection.Input),
                    GetParameter("p_Category", category, OracleDbType.Varchar2, ParameterDirection.Input),
                    GetParameter("p_GROUP_SCH_ID", groupSchId, OracleDbType.Int32, ParameterDirection.Input),
                    GetParameter("p_ENV_ID", envId, OracleDbType.Int32, ParameterDirection.Input),
                };
            var list = ReadCompoundEntityList<GroupScheduleEntity>(FnGroupScheduleServiceDetails, pList, RowToGroupOpenScheduleDetailsList);

            return list;
        }

        public List<GroupScheduleDetailEntity> GetGroupOpenScheduleEnvDetails(DateTime currentDate, string status, string category, int groupSchId, int envId)
        {
            var pList = new List<OracleParameter>
                {
                    GetParameter("p_CURRENTDATETIME", currentDate, OracleDbType.TimeStamp, ParameterDirection.Input),
                    GetParameter("p_SCHEDULE_STATUS", status, OracleDbType.Char, ParameterDirection.Input),
                    GetParameter("p_Category", category, OracleDbType.Varchar2, ParameterDirection.Input),
                    GetParameter("p_GROUP_SCH_ID", groupSchId, OracleDbType.Int32, ParameterDirection.Input),
                    GetParameter("p_ENV_ID", envId, OracleDbType.Int32, ParameterDirection.Input),
                };
            var list = ReadCompoundEntityList<GroupScheduleDetailEntity>(FnGroupScheduleServiceDetails, pList, RowToGroupScheduleDetailList);

            return list;
        }


        public List<GroupScheduleServiceDetailEntity> GetGroupOpenScheduleServiceDetails(DateTime currentDate, string status, string category, int groupSchID, int envID)
        {
            
            var pList = new List<OracleParameter>
                {
                    GetParameter("p_CURRENTDATETIME", currentDate, OracleDbType.TimeStamp, ParameterDirection.Input),
                    GetParameter("p_SCHEDULE_STATUS", status, OracleDbType.Char, ParameterDirection.Input),
                    GetParameter("p_Category", category, OracleDbType.Varchar2, ParameterDirection.Input),
                    GetParameter("p_GROUP_SCH_ID", groupSchID, OracleDbType.Int32, ParameterDirection.Input),
                    GetParameter("p_ENV_ID", envID, OracleDbType.Int32, ParameterDirection.Input),
                };
            var list = ReadCompoundEntityList<GroupScheduleServiceDetailEntity>(FnGroupScheduleServiceDetails, pList, RowToGroupOpenScheduleServiceDetailsList);

            return list;
        }

        public int UpdateScheduleServiceStatus(GroupScheduleServiceDetailEntity details)
        {
            int result = 0;
            try
            {
                var pList = new List<OracleParameter>
                {
                    GetParameter("p_GROUP_SERVICE_SCH_ID", details.Group_Schedule_Detail_ID, OracleDbType.Int32, ParameterDirection.Input),
                    GetParameter("p_GROUP_SERVICE_SCH_STATUS", details.Schedule_Status, OracleDbType.Char, ParameterDirection.Input),
                    GetParameter("p_GROUP_SCH_UPDATEDTIME", details.Schedule_UpdatedTime, OracleDbType.TimeStamp, ParameterDirection.Input),
                    GetParameter("p_GROUP_SCH_SERVICE_STARTTIME", details.Group_Schedule_StartedTime, OracleDbType.TimeStamp, ParameterDirection.Input),
                    GetParameter("p_GROUP_SCH_SERVICE_COMPTIME", details.Schedule_UpdatedTime, OracleDbType.TimeStamp, ParameterDirection.Input),
                };
                ExecuteNonQuery(SpGroupScheduleServiceStatus, pList);
                result = 1;
            }
            catch (Exception exception)
            {
                throw;
                result = 0;
            }

            return result;
        }

        public int UpdateGroupScheduleStatus(GroupScheduleEntity details)
        {
            int result = 0;
            try
            {
                var pList = new List<OracleParameter>
                {
                    GetParameter("p_GROUP_SCH_ID", details.Group_Schedule_ID, OracleDbType.Int32, ParameterDirection.Input),
                    GetParameter("p_GROUP_SCH_STATUS", details.Group_Schedule_Status, OracleDbType.Char, ParameterDirection.Input),
                    GetParameter("p_GROUP_SCH_COMPLETED_TIME", details.Group_Schedule_CompletedTime, OracleDbType.TimeStamp, ParameterDirection.Input),
                    GetParameter("p_GROUP_SCH_UPDATED_BY", details.Group_Schedule_UpdatedBy, OracleDbType.Varchar2, ParameterDirection.Input),
                    GetParameter("p_GROUP_SCH_UPDATED_DATETIME", details.Group_Schedule_UpdatedDatetime, OracleDbType.TimeStamp, ParameterDirection.Input),
                };
                ExecuteNonQuery(SpGroupScheduleStatus, pList);
                result = 1;
            }
            catch (Exception)
            {
                result = 0;
            }

            return result;
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
                GetParameter("p_EMTRAC_SEND_ERROR", mailLog.Error, OracleDbType.Varchar2, ParameterDirection.Input),
                GetParameter("p_EMTRAC_CONTENT_TYPE", mailLog.ContentType, OracleDbType.Varchar2,
                    ParameterDirection.Input),
                GetParameter("p_EMTRAC_CREATED_BY", "Admin", OracleDbType.Varchar2, ParameterDirection.Input),
                GetParameter("p_EMTRAC_CREATED_DATE", DateTime.Now, OracleDbType.TimeStamp, ParameterDirection.Input),
                GetParameter("p_EMTRAC_COMMENTS", mailLog.Comments, OracleDbType.Varchar2, ParameterDirection.Input),
            };

            ExecuteNonQuery(SpSetMailLog, pList);
        }

        public List<UserEmailEntity> GetUserEMail(int envId, string messageType)
        {
            var pList = new List<OracleParameter>
            {
                GetParameter("p_ENV_ID",envId,OracleDbType.Int32,ParameterDirection.Input),
                GetParameter("p_MessageType",messageType,OracleDbType.Varchar2,ParameterDirection.Input),
            };
            var list = ReadCompoundEntityList<UserEmailEntity>(GetUserEmailList, pList, RowToUserEmailList);

            return list;
        }

        public int InsUpdateServiceAcknowledge(AcknowledgeEntity ackData, string mode)
        {
            int recInsert = 0;
            try
            {
                var pList = new List<OracleParameter>
                {
                    GetParameter("p_ENV_ID", ackData.EnvId, OracleDbType.Int32, ParameterDirection.Input),
                    GetParameter("p_CONFIG_ID", ackData.ConfigId, OracleDbType.Int32, ParameterDirection.Input),
                    GetParameter("p_MON_ID", ackData.MonId, OracleDbType.Int32, ParameterDirection.Input),
                    GetParameter("p_ACK_ISACKNOWLEDGE", ackData.IsAcknowledgeMode ? 1 : 0, OracleDbType.Int32,
                        ParameterDirection.Input),
                    GetParameter("p_ACK_ALERT", ackData.AcknowledgeAlertChange, OracleDbType.Varchar2,
                        ParameterDirection.Input),
                    GetParameter("p_ACK_COMMENTS", ackData.AcknowledgeComments, OracleDbType.Varchar2,
                        ParameterDirection.Input),
                    GetParameter("p_CREATED_BY", ackData.CreatedBy, OracleDbType.Varchar2, ParameterDirection.Input),
                    GetParameter("p_CREATED_DATE", ackData.CreatedDate, OracleDbType.TimeStamp, ParameterDirection.Input),
                };
                ExecuteNonQuery(SetSpServiceAcknowledge, pList);
                recInsert = 0;

            }
            catch (Exception)
            {
                recInsert = 1;
            }

            return recInsert;
        }

        private List<UserEmailEntity> RowToUserEmailList(OracleDataReader reader)
        {
            var list = new List<UserEmailEntity>();

            while (reader.Read())
            {
                var entity = new UserEmailEntity();
                if (CommonUtility.IsColumnExistsAndNotNull(reader, "ENV_ID"))
                    entity.Env_ID = Convert.ToInt32(reader["ENV_ID"]);
                if (CommonUtility.IsColumnExistsAndNotNull(reader, "USRLST_TYPE"))
                    entity.EmailType = Convert.ToString(reader["USRLST_TYPE"]);
                if (CommonUtility.IsColumnExistsAndNotNull(reader, "USRLST_EMAIL_ADDRESS"))
                    entity.EmailAddress = Convert.ToString(reader["USRLST_EMAIL_ADDRESS"]);
                if (CommonUtility.IsColumnExistsAndNotNull(reader, "USRLST_IS_ACTIVE"))
                    entity.IsActive = Convert.ToBoolean(reader["USRLST_IS_ACTIVE"]);
                if (CommonUtility.IsColumnExistsAndNotNull(reader, "USRLST_COMMENTS"))
                    entity.EmailComments = Convert.ToString(reader["USRLST_COMMENTS"]);
                if (CommonUtility.IsColumnExistsAndNotNull(reader, "USRLST_CREATED_BY"))
                    entity.UpdatedBy = Convert.ToString(reader["USRLST_CREATED_BY"]);
                if (CommonUtility.IsColumnExistsAndNotNull(reader, "USRLST_CREATED_DATE"))
                    entity.UpdatedDate = Convert.ToDateTime(reader["USRLST_CREATED_DATE"]);

                list.Add(entity);
            }

            return list;
        }

        private List<GroupScheduleServiceDetailEntity> RowToGroupOpenScheduleServiceDetailsList(OracleDataReader reader)
        {
            var list = new List<GroupScheduleServiceDetailEntity>();

            while (reader.Read())
            {
                var entity = new GroupScheduleServiceDetailEntity();
                if (CommonUtility.IsColumnExistsAndNotNull(reader, "GROUP_ID"))
                    entity.Group_ID = Convert.ToInt32(reader["GROUP_ID"]);
                if (CommonUtility.IsColumnExistsAndNotNull(reader, "ENV_ID"))
                    entity.Env_ID = Convert.ToInt32(reader["ENV_ID"]);
                if (CommonUtility.IsColumnExistsAndNotNull(reader, "GROUP_SCH_ID"))
                    entity.Group_Schedule_ID = Convert.ToInt32(reader["GROUP_SCH_ID"]);
                if (CommonUtility.IsColumnExistsAndNotNull(reader, "GROUP_SERVICE_SCH_ID"))
                    entity.Group_Schedule_Detail_ID = Convert.ToInt32(reader["GROUP_SERVICE_SCH_ID"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_ID"))
                    entity.Config_ID = Convert.ToInt32(reader["CONFIG_ID"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "WIN_SERVICE_ID"))
                    entity.WindowsServiceId = Convert.ToInt32(reader["WIN_SERVICE_ID"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_HOST_IP"))
                    entity.HostIP = Convert.ToString(reader["CONFIG_HOST_IP"]).ToLower(CultureInfo.CurrentCulture);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_PORT_NUMBER"))
                    entity.Port = Convert.ToString(reader["CONFIG_PORT_NUMBER"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_SERVICE_TYPE"))
                {
                    switch (Convert.ToString(reader["CONFIG_SERVICE_TYPE"]))
                    {
                        case "1":
                            entity.Service_Name = CONTENTSERVICE;
                            entity.ServiceTypeShort = "CM";
                            break;
                        case "2":
                            entity.Service_Name = DESPATCHER;
                            entity.ServiceTypeShort = "DISP";
                            break;
                        default:
                            entity.Service_Name = entity.HostIP + ":" + entity.Port;
                            entity.ServiceTypeShort = "CM";
                            break;
                    }
                }

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "WIN_SERVICENAME"))
                    entity.WindowsServiceName = Convert.ToString(reader["WIN_SERVICENAME"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "GROUP_SCH_STATUS"))
                    entity.Schedule_Status = Convert.ToString(reader["GROUP_SCH_STATUS"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "GROUP_SCH_ISACTIVE"))
                    entity.IsScheduleActive = Convert.ToBoolean(reader["GROUP_SCH_ISACTIVE"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "GROUP_SCH_CREATED_DATETIME"))
                    entity.Group_Schedule_CreatedDatetime = Convert.ToDateTime(reader["GROUP_SCH_CREATED_DATETIME"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "GROUP_SCH_UPDATEDTIME"))
                    entity.Schedule_UpdatedTime = (DateTime)(entity.Group_Schedule_UpdatedDatetime = Convert.ToDateTime(reader["GROUP_SCH_UPDATEDTIME"]));

                list.Add(entity);
            }

            return list;
        }


        private List<GroupScheduleDetailEntity> RowToGroupScheduleDetailList(OracleDataReader reader)
        {
            var list = new List<GroupScheduleDetailEntity>();

            while (reader.Read())
            {
                var entity = new GroupScheduleDetailEntity();

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "GROUP_SCH_ID"))
                    entity.Group_ID = Convert.ToInt32(reader["GROUP_SCH_ID"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "GROUP_ID"))
                    entity.Group_Schedule_ID = Convert.ToInt32(reader["GROUP_ID"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "ENV_ID"))
                    entity.Env_ID = Convert.ToInt32(reader["ENV_ID"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "ENV_NAME"))
                    entity.Env_Name = Convert.ToString(reader["ENV_NAME"]);

                list.Add(entity);
            }

            return list;
        }


        private List<GroupScheduleEntity> RowToGroupOpenScheduleDetailsList(OracleDataReader reader)
        {
            var list = new List<GroupScheduleEntity>();

            while (reader.Read())
            {
                var entity = new GroupScheduleEntity();
                if (CommonUtility.IsColumnExistsAndNotNull(reader, "GROUP_SCH_ID"))
                    entity.Group_Schedule_ID = Convert.ToInt32(reader["GROUP_SCH_ID"]);
                if (CommonUtility.IsColumnExistsAndNotNull(reader, "GROUP_ID"))
                    entity.Group_ID = Convert.ToInt32(reader["GROUP_ID"]);
                if (CommonUtility.IsColumnExistsAndNotNull(reader, "GROUP_NAME"))
                    entity.Group_Name = Convert.ToString(reader["GROUP_NAME"]);
                if (CommonUtility.IsColumnExistsAndNotNull(reader, "GROUP_SCH_TIME"))
                    entity.Group_Schedule_Datatime = Convert.ToDateTime(reader["GROUP_SCH_TIME"]);
                if (CommonUtility.IsColumnExistsAndNotNull(reader, "GROUP_SCH_ACTION"))
                    entity.Group_Schedule_Action = Convert.ToString(reader["GROUP_SCH_ACTION"]);
                if (CommonUtility.IsColumnExistsAndNotNull(reader, "GROUP_SCH_STATUS"))
                    entity.Group_Schedule_Status = Convert.ToString(reader["GROUP_SCH_STATUS"]);
                if (CommonUtility.IsColumnExistsAndNotNull(reader, "GROUP_SCH_COMPLETED_TIME"))
                    entity.Group_Schedule_CompletedTime = Convert.ToDateTime(reader["GROUP_SCH_COMPLETED_TIME"]);
                if (CommonUtility.IsColumnExistsAndNotNull(reader, "GROUP_SCH_COMMENTS"))
                    entity.Group_Schedule_Comments = Convert.ToString(reader["GROUP_SCH_COMMENTS"]);
                if (CommonUtility.IsColumnExistsAndNotNull(reader, "USER_FIRST_NAME"))
                    entity.Group_Schedule_CreatedBy = Convert.ToString(reader["USER_FIRST_NAME"]);
                if (CommonUtility.IsColumnExistsAndNotNull(reader, "GROUP_SCH_CREATED_DATETIME"))
                    entity.Group_Schedule_CreatedDatetime = Convert.ToDateTime(reader["GROUP_SCH_CREATED_DATETIME"]);
                if (CommonUtility.IsColumnExistsAndNotNull(reader, "GROUP_SCH_UPDATED_BY"))
                    entity.Group_Schedule_UpdatedBy = Convert.ToString(reader["GROUP_SCH_UPDATED_BY"]);
                if (CommonUtility.IsColumnExistsAndNotNull(reader, "GROUP_SCH_UPDATED_DATETIME"))
                    entity.Group_Schedule_UpdatedDatetime = Convert.ToDateTime(reader["GROUP_SCH_UPDATED_DATETIME"]);

                list.Add(entity);
            }

            return list;
        }


    }
}
//8939969976 - senthilkumar
//