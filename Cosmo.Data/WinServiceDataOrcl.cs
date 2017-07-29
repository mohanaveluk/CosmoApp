using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using Cosmo.Entity;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;

namespace Cosmo.Data
{
    public class WinServiceDataOrcl : BusinessEntityBaseDAO, IWinServiceData
    {
        #region Constant Variables

        private const string PackageName = "COSMO_WINSERVICE_PACKAGE.";
        private const string PackageNameEnvironment = "COSMO_ENVIRONMENT_PACKAGE.";

        private static readonly string SetGroup = string.Format("{0}SP_CWT_InsGroup", PackageName);
        private static readonly string SetGroupDetail = string.Format("{0}SP_CWT_InsGroupDetail", PackageName);
        private static readonly string SetGroupSchedule = string.Format("{0}SP_CWT_InsGroupSchedule", PackageName);

        private static readonly string GetGroup = string.Format("{0}FN_CWT_GetGroup", PackageName);
        private static readonly string GetGroupId = string.Format("{0}FN_CWT_GetGroupID", PackageName);
        private static readonly string FnGetGroupDetail = string.Format("{0}FN_CWT_GetGroupDetail", PackageName);
        private static readonly string FnGetGroupSchedule = string.Format("{0}FN_CWT_GetGroupSchedule", PackageName);
        private static readonly string FnGetGroupScheduleServceDetails = string.Format("{0}FN_CWT_GetGroupSchServiceDet", PackageName); //FN_CWT_GetGroupScheduleServceDetails
        private static readonly string FnGetWindowsServiceConfigurationOnDemand = string.Format("{0}FN_CWT_GetWSConfigOnDemand", PackageName); //CWT_GetWindowsServiceConfigurationOnDemand
        private static readonly string ReportResatrService = string.Format("{0}FN_CWT_ReportRestartService", PackageName);

        private static readonly string SetDeleterecord = string.Format("{0}SP_CWT_DeleteRecord", PackageNameEnvironment);

        #endregion Constant Variables

        public int InsGroup(GroupEntity group)
        {
            int recInsert = 0;
            try
            {


                var pList = new List<OracleParameter>
                {
                    GetParameter("p_GROUP_ID", group.Group_ID, OracleDbType.Int32, ParameterDirection.Input),
                    GetParameter("p_GROUP_NAME", group.Group_Name, OracleDbType.Varchar2, ParameterDirection.Input),
                    GetParameter("p_GROUP_CREATED_BY", group.CreatedBy, OracleDbType.Varchar2, ParameterDirection.Input),
                    GetParameter("p_GROUP_CREATED_DATE", System.DateTime.Now, OracleDbType.TimeStamp,
                        ParameterDirection.Input),
                    GetParameter("p_GROUP_COMMENTS",
                        string.IsNullOrEmpty(group.Comments) ? string.Empty : group.Comments, OracleDbType.Varchar2,
                        ParameterDirection.Input),
                    GetParameter("p_GROUP_ISACTIVE", 1, OracleDbType.Int32, ParameterDirection.Input),
                };
                ExecuteNonQuery(SetGroup, pList);
                recInsert = 0;
            }
            catch (Exception)
            {
                recInsert = 1;
                
            }
            return recInsert;
        }

        public int InsGroupDetail(GroupDetailEntity groupDetail, string configIDs)
        {
            int recInsert = 0;
            try
            {
                var pList = new List<OracleParameter>
                {
                    GetParameter("p_GROUP_ID", groupDetail.Group_ID, OracleDbType.Int32, ParameterDirection.Input),
                    GetParameter("p_ENV_ID", groupDetail.Env_ID, OracleDbType.Int32, ParameterDirection.Input),
                    GetParameter("p_SERVICE_IDS", configIDs, OracleDbType.Int32, ParameterDirection.Input),
                    GetParameter("p_WIN_SERVICE_ID", groupDetail.WinService_ID, OracleDbType.Int32,
                        ParameterDirection.Input),
                    GetParameter("p_GROUP_DETAIL_COMMENTS",
                        string.IsNullOrEmpty(groupDetail.Comments) ? string.Empty : groupDetail.Comments,
                        OracleDbType.Varchar2, ParameterDirection.Input),
                    GetParameter("p_GROUP_CREATED_BY", groupDetail.CreatedBy, OracleDbType.Int32,
                        ParameterDirection.Input),
                    GetParameter("p_GROUP_CREATED_DATE", System.DateTime.Now, OracleDbType.Int32,
                        ParameterDirection.Input),
                    GetParameter("p_GROUP_ISACTIVE", 1, OracleDbType.Int32, ParameterDirection.Input),
                };
                ExecuteNonQuery(SetGroupDetail, pList);
                recInsert = 0;
            }
            catch (Exception)
            {
                recInsert = 1;
                
            }
            return recInsert;
        }

        public int InsGroupScheduleDetail(GroupScheduleEntity groupScheduleEntity, string envIDs, string configIDs,
            string winServiceIDs)
        {
            int? recInsert = 0;
            try
            {
                var pList = new List<OracleParameter>
                {
                    GetParameter("p_GROUP_SCH_ID", groupScheduleEntity.Group_Schedule_ID, OracleDbType.Int32,
                        ParameterDirection.Input),
                    GetParameter("p_GROUP_ID", groupScheduleEntity.Group_ID, OracleDbType.Int32,
                        ParameterDirection.Input),
                    GetParameter("p_GROUP_NAME", groupScheduleEntity.Group_Name, OracleDbType.Varchar2,
                        ParameterDirection.Input),
                    GetParameter("p_ENV_IDS", envIDs, OracleDbType.Varchar2, ParameterDirection.Input),
                    GetParameter("p_CONFIG_IDS", configIDs, OracleDbType.Varchar2, ParameterDirection.Input),
                    GetParameter("p_WIN_SERVICE_IDS", winServiceIDs, OracleDbType.Varchar2, ParameterDirection.Input),
                    GetParameter("p_GROUP_SCH_ACTION", groupScheduleEntity.Group_Schedule_Action, OracleDbType.Varchar2,
                        ParameterDirection.Input),
                    GetParameter("p_GROUP_SCH_STATUS", groupScheduleEntity.Group_Schedule_Status, OracleDbType.Varchar2,
                        ParameterDirection.Input),
                    GetParameter("p_GROUP_SCH_TIME", groupScheduleEntity.Group_Schedule_Datatime, OracleDbType.TimeStamp,
                        ParameterDirection.Input),
                    GetParameter("p_GROUP_SCH_COMMENTS",
                        string.IsNullOrEmpty(groupScheduleEntity.Group_Schedule_Comments)
                            ? string.Empty
                            : groupScheduleEntity.Group_Schedule_Comments, OracleDbType.Varchar2,
                        ParameterDirection.Input),
                    GetParameter("p_GROUP_SCH_CREATED_BY", groupScheduleEntity.Group_Schedule_CreatedBy,
                        OracleDbType.Varchar2, ParameterDirection.Input),
                    GetParameter("p_GROUP_SCH_CREATED_DATETIME", groupScheduleEntity.Group_Schedule_CreatedDatetime,
                        OracleDbType.TimeStamp, ParameterDirection.Input),
                    GetParameter("p_GROUP_SCH_ISACTIVE", 1, OracleDbType.Int32, ParameterDirection.Input),
                    GetParameter("p_GROUP_SCH_ONDEMAND", groupScheduleEntity.Group_Schedule_OnDemand ? 1 : 0,
                        OracleDbType.Int32, ParameterDirection.Input),
                    GetParameter("p_GROUP_SCH_COMPLETESTATUS", groupScheduleEntity.Group_Schedule_CompleteStatus,
                        OracleDbType.Varchar2, ParameterDirection.Input),
                    GetParameter("p_GROUP_SCH_COMPLETEDTIME", groupScheduleEntity.Group_Schedule_CompletedTime,
                        OracleDbType.TimeStamp, ParameterDirection.Input),
                    GetParameter("p_GROUP_SCH_REQUESTSOURCE", groupScheduleEntity.RequestSource ?? "Desktop",
                        OracleDbType.Varchar2, ParameterDirection.Input),
                    GetParameter("p_GROUP_SCH_SERVICE_STARTTIME", groupScheduleEntity.ServiceStartedTime ?? DateTime.Now,
                        OracleDbType.TimeStamp, ParameterDirection.Input),
                    GetParameter("p_GROUP_SCH_SERVICE_COMPLETEDT",
                        groupScheduleEntity.ServiceCompletionTime ?? DateTime.Now, OracleDbType.TimeStamp,
                        ParameterDirection.Input),
                };
                pList.Add(new OracleParameter("p_SCOPE_OUTPUT", OracleDbType.Int32, ParameterDirection.Output));

                ExecuteNonQuery(SetGroupSchedule, pList);

                foreach (var oracleParameter in pList.Where(oracleParameter => oracleParameter.ParameterName == "p_SCOPE_OUTPUT"))
                {
                    var rec = OracleDecimalToInt((OracleDecimal)oracleParameter.Value);
                    recInsert = rec;
                }
            }
            catch (Exception)
            {
                recInsert = -1;
            }

            if (recInsert != null) return (int) recInsert;

            return -1;
        }

        public List<GroupEntity> GetGroupID(string grpName)
        {
            var pList = new List<OracleParameter>
            {
                GetParameter("p_GROUP_NAME", grpName, OracleDbType.Varchar2, ParameterDirection.Input),
            };
            var groupList = ReadCompoundEntityList<GroupEntity>(GetGroupId, pList, RowToGroupIdList);
            return groupList;
        }


        public List<GroupEntity> GetAllGroup(int grp_ID)
        {
            var pList = new List<OracleParameter>
            {
                GetParameter("p_GRP_ID", grp_ID, OracleDbType.Int32, ParameterDirection.Input),
            };
            var groupList = ReadCompoundEntityList<GroupEntity>(GetGroup, pList, RowToGroupIdList);
            return groupList;
        }

        public List<GroupDetailEntity> GetGroupDetail(int grp_ID, int env_ID)
        {
            var pList = new List<OracleParameter>
            {
                GetParameter("p_GROUP_ID", grp_ID, OracleDbType.Int32, ParameterDirection.Input),
                GetParameter("p_ENV_ID", env_ID, OracleDbType.Int32, ParameterDirection.Input),
            };
            var groupList = ReadCompoundEntityList<GroupDetailEntity>(FnGetGroupDetail, pList, RowToGroupDetailList);
            return groupList;
        }

        public List<GroupScheduleEntity> GetAllGroupSchedules(int grp_ID, int grp_sch_ID, DateTime startTime)
        {
            var pList = new List<OracleParameter>
            {
                GetParameter("p_GROUP_ID", grp_ID, OracleDbType.Int32, ParameterDirection.Input),
                GetParameter("p_GROUP_SCH_ID", grp_sch_ID, OracleDbType.Int32, ParameterDirection.Input),
                GetParameter("p_CURRENTDATETIME", startTime, OracleDbType.TimeStamp, ParameterDirection.Input),
            };
            var groupList = ReadCompoundEntityList<GroupScheduleEntity>(FnGetGroupSchedule, pList, RowToGroupScheduleList);
            return groupList;
        }

        public List<GroupScheduleEntity> GetGroupScheduleDetails(int grpId, string category, int envId)
        {
            var pList = new List<OracleParameter>
            {
                GetParameter("p_GROUP_SCH_ID", grpId, OracleDbType.Int32, ParameterDirection.Input),
                GetParameter("p_Category", category, OracleDbType.Varchar2, ParameterDirection.Input),
                GetParameter("p_ENV_ID", envId, OracleDbType.Int32, ParameterDirection.Input),
                GetParameter("p_GROUP_SCH_STATUS", string.Empty, OracleDbType.Char, ParameterDirection.Input),
            };
            var groupList = ReadCompoundEntityList<GroupScheduleEntity>(FnGetGroupScheduleServceDetails, pList, RowToGroupScheduleList);
            return groupList;
        }

        public List<GroupScheduleDetailEntity> GetGroupScheduleEnvDetails(int grpId, string category, int envId)
        {
            var pList = new List<OracleParameter>
            {
                GetParameter("p_GROUP_SCH_ID", grpId, OracleDbType.Int32, ParameterDirection.Input),
                GetParameter("p_Category", category, OracleDbType.Varchar2, ParameterDirection.Input),
                GetParameter("p_ENV_ID", envId, OracleDbType.Int32, ParameterDirection.Input),
                GetParameter("p_GROUP_SCH_STATUS", string.Empty, OracleDbType.Char, ParameterDirection.Input),
            };
            var groupList = ReadCompoundEntityList<GroupScheduleDetailEntity>(FnGetGroupScheduleServceDetails, pList, RowToGroupScheduleDetailList);
            return groupList;
        }
        
        public List<GroupScheduleServiceDetailEntity> GetGroupScheduleServiceDetails(int grpId, string category, int envId, char grpStatus)
        {
            var pList = new List<OracleParameter>
            {
                GetParameter("p_GROUP_SCH_ID", grpId, OracleDbType.Int32, ParameterDirection.Input),
                GetParameter("p_Category", category, OracleDbType.Varchar2, ParameterDirection.Input),
                GetParameter("p_ENV_ID", envId, OracleDbType.Int32, ParameterDirection.Input),
                GetParameter("p_GROUP_SCH_STATUS", grpStatus, OracleDbType.Char, ParameterDirection.Input),
            };
            var groupList = ReadCompoundEntityList<GroupScheduleServiceDetailEntity>(FnGetGroupScheduleServceDetails, pList, RowToGroupScheduleServiceDetailList);
            return groupList;
        }


        public List<ServiceOnDemandMailEntity> GetServiceConfigurationOndemand(int winSerId)
        {
            var pList = new List<OracleParameter>
            {
                GetParameter("p_WIN_SERVICE_ID", winSerId, OracleDbType.Int32, ParameterDirection.Input),
            };
            var groupList = ReadCompoundEntityList<ServiceOnDemandMailEntity>(FnGetWindowsServiceConfigurationOnDemand, pList, RowToWindowsServiceConfigOnDemandMailList);
            return groupList;
        }

        public List<GroupcheduleReportEntity> GetGroupScheduleReport(string schType, DateTime startDate, DateTime endDate)
        {
            var pList = new List<OracleParameter>
            {
                GetParameter("p_SCHEDULETYPE", schType, OracleDbType.Varchar2, ParameterDirection.Input),
                GetParameter("p_STARTDATE", startDate, OracleDbType.TimeStamp, ParameterDirection.Input),
                GetParameter("p_ENDDATE", endDate, OracleDbType.TimeStamp, ParameterDirection.Input),
            };
            var groupList = ReadCompoundEntityList<GroupcheduleReportEntity>(ReportResatrService, pList, RowToGroupScheduleReportList);
            return groupList;
        }



        public void CancelGroupSchedule(string type, string vId)
        {
            var pList = new List<OracleParameter>
            {
                GetParameter("p_sID", vId, OracleDbType.Int32, ParameterDirection.Input),
                GetParameter("p_sType", type, OracleDbType.Varchar2, ParameterDirection.Input),
            };

            ExecuteNonQuery(SetDeleterecord, pList);
        }

        private List<GroupEntity> RowToGroupIdList(OracleDataReader reader)
        {
            var list = new List<GroupEntity>();

            while (reader.Read())
            {
                var entity = new GroupEntity();
                if (CommonUtility.IsColumnExistsAndNotNull(reader, "GROUP_ID"))
                    entity.Group_ID = Convert.ToInt32(reader["GROUP_ID"]);
                if (CommonUtility.IsColumnExistsAndNotNull(reader, "GROUP_NAME"))
                    entity.Group_Name = Convert.ToString(reader["GROUP_NAME"]);
                if (CommonUtility.IsColumnExistsAndNotNull(reader, "USERFIRSTNAME"))
                    entity.CreatedBy = Convert.ToString(reader["USERFIRSTNAME"]);
                if (CommonUtility.IsColumnExistsAndNotNull(reader, "GROUP_CREATED_DATE"))
                    entity.CreatedDate = Convert.ToDateTime(reader["GROUP_CREATED_DATE"]);
                if (CommonUtility.IsColumnExistsAndNotNull(reader, "USERFIRSTNAME"))
                    entity.UpdatedBy = Convert.ToString(reader["USERFIRSTNAME"]);
                if (CommonUtility.IsColumnExistsAndNotNull(reader, "GROUP_UPDATED_DATE"))
                    entity.UpdatedDate = Convert.ToDateTime(reader["GROUP_UPDATED_DATE"]);
                if (CommonUtility.IsColumnExistsAndNotNull(reader, "GROUP_COMMENTS"))
                    entity.Comments = Convert.ToString(reader["GROUP_COMMENTS"]);
                if (CommonUtility.IsColumnExistsAndNotNull(reader, "GROUP_IS_ACTIVE"))
                    entity.IsActive = Convert.ToBoolean(reader["GROUP_IS_ACTIVE"]);

                list.Add(entity);
            }

            return list;
        }


        private List<GroupDetailEntity> RowToGroupDetailList(OracleDataReader reader)
        {
            var list = new List<GroupDetailEntity>();

            while (reader.Read())
            {
                var entity = new GroupDetailEntity();
                if (CommonUtility.IsColumnExistsAndNotNull(reader, "GROUP_DETAIL_ID"))
                    entity.Group_Detail_ID = Convert.ToInt32(reader["GROUP_DETAIL_ID"]);
                if (CommonUtility.IsColumnExistsAndNotNull(reader, "GROUP_ID"))
                    entity.Group_ID = Convert.ToInt32(reader["GROUP_ID"]);
                if (CommonUtility.IsColumnExistsAndNotNull(reader, "ENV_ID"))
                    entity.Env_ID = Convert.ToInt32(reader["ENV_ID"]);
                if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_ID"))
                    entity.Config_ID = Convert.ToInt32(reader["CONFIG_ID"]);
                if (CommonUtility.IsColumnExistsAndNotNull(reader, "WIN_SERVICE_ID"))
                    entity.WinService_ID = Convert.ToInt32(reader["WIN_SERVICE_ID"]);
                if (CommonUtility.IsColumnExistsAndNotNull(reader, "USERFIRSTNAME"))
                    entity.CreatedBy = Convert.ToString(reader["USERFIRSTNAME"]);
                if (CommonUtility.IsColumnExistsAndNotNull(reader, "GROUP_CREATED_DATE"))
                    entity.CreatedDate = Convert.ToDateTime(reader["GROUP_CREATED_DATE"]);
                if (CommonUtility.IsColumnExistsAndNotNull(reader, "USERFIRSTNAME"))
                    entity.UpdatedBy = Convert.ToString(reader["USERFIRSTNAME"]);
                if (CommonUtility.IsColumnExistsAndNotNull(reader, "GROUP_UPDATED_DATE"))
                    entity.UpdatedDate = Convert.ToDateTime(reader["GROUP_UPDATED_DATE"]);
                if (CommonUtility.IsColumnExistsAndNotNull(reader, "GROUP_DETAIL_COMMENTS"))
                    entity.Comments = Convert.ToString(reader["GROUP_DETAIL_COMMENTS"]);
                if (CommonUtility.IsColumnExistsAndNotNull(reader, "GROUP_ISACTIVE"))
                    entity.IsActive = Convert.ToBoolean(reader["GROUP_ISACTIVE"]);
                if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_HOST_IP"))
                    entity.HostIP = Convert.ToString(reader["CONFIG_HOST_IP"]).ToLower(CultureInfo.CurrentCulture);
                if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_PORT_NUMBER"))
                    entity.Port = Convert.ToString(reader["CONFIG_PORT_NUMBER"]);
                if (CommonUtility.IsColumnExistsAndNotNull(reader, "WIN_SERVICENAME"))
                    entity.WinServiceName = Convert.ToString(reader["WIN_SERVICENAME"]);
                else
                {
                    entity.WinServiceName = string.Empty;
                }
                if (CommonUtility.IsColumnExistsAndNotNull(reader, "GROUP_NAME"))
                    entity.GroupName = Convert.ToString(reader["GROUP_NAME"]);
                else
                {
                    entity.GroupName = string.Empty;
                }
                if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_LOCATION"))
                    entity.Location = Convert.ToString(reader["CONFIG_LOCATION"]);
                else
                {
                    entity.Location = string.Empty;
                }
                if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_SERVICE_TYPE"))
                    entity.ServiceType = Convert.ToString(reader["CONFIG_SERVICE_TYPE"]);
                else
                {
                    entity.ServiceType = string.Empty;
                }
                if (CommonUtility.IsColumnExistsAndNotNull(reader, "ENV_NAME"))
                    entity.Env_Name = Convert.ToString(reader["ENV_NAME"]);

                list.Add(entity);
            }

            return list;
        }

        private List<GroupScheduleEntity> RowToGroupScheduleList(OracleDataReader reader)
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
                {
                    entity.Group_Schedule_Datatime = Convert.ToDateTime(reader["GROUP_SCH_TIME"]);
                    entity.Group_Schedule_DatatimeStr =
                        Convert.ToDateTime(reader["GROUP_SCH_TIME"]).ToString(CultureInfo.InvariantCulture);
                }
                else
                {
                    entity.Group_Schedule_Datatime = null;
                    entity.Group_Schedule_DatatimeStr = string.Empty;
                }
                if (CommonUtility.IsColumnExistsAndNotNull(reader, "GROUP_SCH_ACTION"))
                    entity.Group_Schedule_Action = Convert.ToString(reader["GROUP_SCH_ACTION"]);
                if (CommonUtility.IsColumnExistsAndNotNull(reader, "GROUP_SCH_STATUS"))
                    entity.Group_Schedule_Status = Convert.ToString(reader["GROUP_SCH_STATUS"]);
                if (CommonUtility.IsColumnExistsAndNotNull(reader, "GROUP_SCH_COMPLETED_TIME"))
                    entity.Group_Schedule_CompletedTime = Convert.ToDateTime(reader["GROUP_SCH_COMPLETED_TIME"]);
                else
                    entity.Group_Schedule_CompletedTime = null;

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "GROUP_SCH_COMMENTS"))
                    entity.Group_Schedule_Comments = Convert.ToString(reader["GROUP_SCH_COMMENTS"]);
                if (CommonUtility.IsColumnExistsAndNotNull(reader, "USERFIRSTNAME"))
                    entity.Group_Schedule_CreatedBy = Convert.ToString(reader["USERFIRSTNAME"]);
                if (CommonUtility.IsColumnExistsAndNotNull(reader, "GROUP_SCH_CREATED_DATETIME"))
                    entity.Group_Schedule_CreatedDatetime = Convert.ToDateTime(reader["GROUP_SCH_CREATED_DATETIME"]);
                if (CommonUtility.IsColumnExistsAndNotNull(reader, "USERFIRSTNAME"))
                    entity.Group_Schedule_UpdatedBy = Convert.ToString(reader["USERFIRSTNAME"]);
                if (CommonUtility.IsColumnExistsAndNotNull(reader, "GROUP_SCH_UPDATED_DATETIME"))
                    entity.Group_Schedule_UpdatedDatetime = Convert.ToDateTime(reader["GROUP_SCH_UPDATED_DATETIME"]);
                else
                {
                    entity.Group_Schedule_UpdatedDatetime = null;
                }
                if (CommonUtility.IsColumnExistsAndNotNull(reader, "GROUP_SCH_ONDEMAND"))
                    entity.Group_Schedule_OnDemand = Convert.ToBoolean(reader["GROUP_SCH_ONDEMAND"]);
                if (CommonUtility.IsColumnExistsAndNotNull(reader, "GROUP_SCH_RESULT"))
                    entity.Group_Schedule_CompleteStatus = Convert.ToString(reader["GROUP_SCH_RESULT"]);
                if (CommonUtility.IsColumnExistsAndNotNull(reader, "GROUP_SCH_REQUESTSOURCE"))
                    entity.RequestSource = Convert.ToString(reader["GROUP_SCH_REQUESTSOURCE"]);


                if (CommonUtility.IsColumnExistsAndNotNull(reader, "@GROUP_SCH_SERVICE_STARTTIME"))
                    entity.ServiceStartedTime = Convert.ToDateTime(reader["@GROUP_SCH_SERVICE_STARTTIME"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "@GROUP_SCH_SERVICE_COMPLETEDTIME"))
                    entity.ServiceCompletionTime = Convert.ToDateTime(reader["@GROUP_SCH_SERVICE_COMPLETEDTIME"]);

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

                var serviceDetails = new GroupScheduleServiceDetailEntity();

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "ENV_ID"))
                    entity.Env_ID = Convert.ToInt32(reader["ENV_ID"]);
                if (CommonUtility.IsColumnExistsAndNotNull(reader, "GROUP_ID"))
                    entity.Group_ID = Convert.ToInt32(reader["GROUP_ID"]);
                if (CommonUtility.IsColumnExistsAndNotNull(reader, "GROUP_SCH_ID"))
                    entity.Group_Schedule_ID = Convert.ToInt32(reader["GROUP_SCH_ID"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "GROUP_SERVICE_SCH_ID"))
                    serviceDetails.Group_Schedule_Detail_ID = Convert.ToInt32(reader["GROUP_SERVICE_SCH_ID"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_ID"))
                    serviceDetails.Config_ID = Convert.ToInt32(reader["CONFIG_ID"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "WIN_SERVICE_ID"))
                    serviceDetails.WindowsService_ID = Convert.ToInt32(reader["WIN_SERVICE_ID"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_HOST_IP"))
                    serviceDetails.HostIP = Convert.ToString(reader["CONFIG_HOST_IP"]).ToLower(CultureInfo.CurrentCulture);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_PORT_NUMBER"))
                    serviceDetails.Port = Convert.ToString(reader["CONFIG_PORT_NUMBER"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_SERVICE_TYPE"))
                {
                    if (Convert.ToString(reader["CONFIG_SERVICE_TYPE"]) == "1")
                        serviceDetails.Service_Name = "Content Manager";
                    else if (Convert.ToString(reader["CONFIG_SERVICE_TYPE"]) == "2")
                        serviceDetails.Service_Name = "Dispatcher";
                    else
                        serviceDetails.Service_Name = serviceDetails.HostIP + ":" + serviceDetails.Port;
                }

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "WIN_SERVICENAME"))
                    serviceDetails.WindowsService_Name = Convert.ToString(reader["WIN_SERVICENAME"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "GROUP_SCH_STATUS"))
                    serviceDetails.Schedule_Status = Convert.ToString(reader["GROUP_SCH_STATUS"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "GROUP_SCH_ISACTIVE"))
                    serviceDetails.IsScheduleActive = Convert.ToBoolean(reader["GROUP_SCH_ISACTIVE"]);

                entity.ServiceDetails = new List<GroupScheduleServiceDetailEntity> {serviceDetails};

                list.Add(entity);
            }

            return list;
        }

        private List<GroupScheduleServiceDetailEntity> RowToGroupScheduleServiceDetailList(OracleDataReader reader)
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
                    entity.WindowsService_ID = Convert.ToInt32(reader["WIN_SERVICE_ID"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_HOST_IP"))
                    entity.HostIP = Convert.ToString(reader["CONFIG_HOST_IP"]).ToLower(CultureInfo.CurrentCulture);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_PORT_NUMBER"))
                    entity.Port = Convert.ToString(reader["CONFIG_PORT_NUMBER"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_SERVICE_TYPE"))
                {
                    if (Convert.ToString(reader["CONFIG_SERVICE_TYPE"]) == "1")
                        entity.Service_Name = "Content Manager";
                    else if (Convert.ToString(reader["CONFIG_SERVICE_TYPE"]) == "2")
                        entity.Service_Name = "Dispatcher";
                    else
                        entity.Service_Name = entity.HostIP + ":" + entity.Port;
                }

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "WIN_SERVICENAME"))
                    entity.WindowsService_Name = Convert.ToString(reader["WIN_SERVICENAME"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "GROUP_SCH_STATUS"))
                    entity.Schedule_Status = Convert.ToString(reader["GROUP_SCH_STATUS"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "GROUP_SCH_ISACTIVE"))
                    entity.IsScheduleActive = Convert.ToBoolean(reader["GROUP_SCH_ISACTIVE"]);

                list.Add(entity);
            }

            return list;
        }


        private List<ServiceOnDemandMailEntity> RowToWindowsServiceConfigOnDemandMailList(OracleDataReader reader)
        {
            var list = new List<ServiceOnDemandMailEntity>();

            while (reader.Read())
            {
                var entity = new ServiceOnDemandMailEntity();

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "WIN_SERVICE_ID"))
                    entity.WindowServiceId = Convert.ToInt32(reader["WIN_SERVICE_ID"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "WIN_SERVICENAME"))
                    entity.WindowServiceName = reader["WIN_SERVICENAME"].ToString();

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "WIN_SERVICE_STATUS"))
                    entity.WindowServiceStatus = reader["WIN_SERVICE_STATUS"].ToString();
                else
                    entity.WindowServiceStatus = string.Empty;

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_ID"))
                    entity.ConfigId = Convert.ToInt32(reader["CONFIG_ID"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_HOST_IP"))
                    entity.HostIP = Convert.ToString(reader["CONFIG_HOST_IP"]).ToLower(CultureInfo.CurrentCulture);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_PORT_NUMBER"))
                    entity.Port = reader["CONFIG_PORT_NUMBER"].ToString();

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_SERVICE_TYPE"))
                    entity.ServiceTypeId = reader["CONFIG_SERVICE_TYPE"].ToString();

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "SERVICETYPE"))
                    entity.ServiceType = reader["SERVICETYPE"].ToString();

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "ENV_NAME"))
                    entity.Env_Name = reader["ENV_NAME"].ToString();

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "ENV_ID"))
                    entity.EnvId = Convert.ToInt32(reader["ENV_ID"]);

                entity.RequestStatus = CommonUtility.IsColumnExistsAndNotNull(reader, "WIN_SERVICE_REQUEST_STATUS") ? reader["WIN_SERVICE_REQUEST_STATUS"].ToString() : string.Empty;

                entity.RequestStatusCompletion = CommonUtility.IsColumnExistsAndNotNull(reader, "WIN_SERVICE_REQUEST_COMPLETION") ? reader["WIN_SERVICE_REQUEST_COMPLETION"].ToString() : string.Empty;

                entity.CompletionStatus = CommonUtility.IsColumnExistsAndNotNull(reader, "WIN_SERVICE_REQUEST_COMPLETION_STATUS") ? reader["WIN_SERVICE_REQUEST_COMPLETION_STATUS"].ToString() : string.Empty;

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "WIN_SERVICE_REQUESTED_TIME"))
                    entity.RequestedDateTime = Convert.ToDateTime(reader["WIN_SERVICE_REQUESTED_TIME"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "GROUP_SCH_ONDEMAND"))
                    entity.OnDemand = Convert.ToBoolean(reader["GROUP_SCH_ONDEMAND"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "GROUP_SCH_RESULT"))
                    entity.Result = Convert.ToString(reader["GROUP_SCH_RESULT"]);


                entity.ServerTimeZone = string.Empty;
                entity.Comments = string.Empty;
                entity.RequestedBy = string.Empty;

                list.Add(entity);
            }

            return list;
        }

        private List<GroupcheduleReportEntity> RowToGroupScheduleReportList(OracleDataReader reader)
        {
            var list = new List<GroupcheduleReportEntity>();

            while (reader.Read())
            {
                var entity = new GroupcheduleReportEntity();
                if (CommonUtility.IsColumnExistsAndNotNull(reader, "GROUP_SCH_ID"))
                    entity.GROUP_SCH_ID = Convert.ToInt32(reader["GROUP_SCH_ID"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "GROUP_ID"))
                    entity.GROUP_ID = Convert.ToInt32(reader["GROUP_ID"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "GROUP_NAME"))
                    entity.GROUP_NAME = Convert.ToString(reader["GROUP_NAME"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "ENV_ID"))
                    entity.ENV_ID = Convert.ToInt32(reader["ENV_ID"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "ENV_NAME"))
                    entity.ENV_NAME = Convert.ToString(reader["ENV_NAME"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_HOST_IP"))
                    entity.CONFIG_HOST_IP = Convert.ToString(reader["CONFIG_HOST_IP"]).ToLower(CultureInfo.CurrentCulture);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_PORT_NUMBER"))
                    entity.CONFIG_PORT_NUMBER = Convert.ToString(reader["CONFIG_PORT_NUMBER"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_DESCRIPTION"))
                    entity.CONFIG_SERVICE_NAME = Convert.ToString(reader["CONFIG_DESCRIPTION"]);

                if (string.IsNullOrEmpty(entity.CONFIG_SERVICE_NAME))
                {
                    if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_HOST_IP"))
                        entity.CONFIG_SERVICE_NAME = entity.CONFIG_HOST_IP + ":" + entity.CONFIG_PORT_NUMBER;
                }

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_LOCATION"))
                    entity.CONFIG_LOCATION = Convert.ToString(reader["CONFIG_LOCATION"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_SERVICE_TYPE"))
                    entity.CONFIG_SERVICE_TYPE = Convert.ToString(reader["CONFIG_SERVICE_TYPE"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "GROUP_SCH_TIME"))
                    entity.GROUP_SCH_TIME = Convert.ToDateTime(reader["GROUP_SCH_TIME"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "GROUP_SCH_ACTION"))
                    entity.GROUP_SCH_ACTION = Convert.ToString(reader["GROUP_SCH_ACTION"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "GROUP_SCH_TYPE"))
                    entity.GROUP_SCH_TYPE = Convert.ToString(reader["GROUP_SCH_TYPE"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "GROUP_SCH_DETAIL_TYPE"))
                    entity.GROUP_SCH_DETAIL_TYPE = Convert.ToString(reader["GROUP_SCH_DETAIL_TYPE"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "GROUP_SCH_STATUS"))
                    entity.GROUP_SCH_STATUS = Convert.ToString(reader["GROUP_SCH_STATUS"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "GROUP_SCH_DETAIL_STATUS"))
                    entity.GROUP_SCH_DETAIL_STATUS = Convert.ToString(reader["GROUP_SCH_DETAIL_STATUS"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "GROUP_SCH_COMPLETED_TIME"))
                    entity.GROUP_SCH_COMPLETED_TIME = Convert.ToDateTime(reader["GROUP_SCH_COMPLETED_TIME"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "GROUP_SCH_COMMENTS"))
                    entity.GROUP_SCH_COMMENTS = Convert.ToString(reader["GROUP_SCH_COMMENTS"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "GROUP_SCH_CREATED_BY"))
                    entity.GROUP_SCH_CREATED_BY = Convert.ToString(reader["GROUP_SCH_CREATED_BY"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "GROUP_SCH_CREATED_DATETIME"))
                    entity.GROUP_SCH_CREATED_DATETIME = Convert.ToDateTime(reader["GROUP_SCH_CREATED_DATETIME"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "USERNAME"))
                    entity.GROUP_SCH_CREATED_BY_USERNAME = Convert.ToString(reader["USERNAME"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "GROUP_SCH_ONDEMAND"))
                    entity.GROUP_SCH_ONDEMAND = Convert.ToString(reader["GROUP_SCH_ONDEMAND"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "GROUP_SCH_RESULT"))
                    entity.GROUP_SCH_RESULT = Convert.ToString(reader["GROUP_SCH_RESULT"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "GROUP_SCH_REQUESTSOURCE"))
                    entity.RequestSource = Convert.ToString(reader["GROUP_SCH_REQUESTSOURCE"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "GROUP_SCH_UPDATEDTIME"))
                    entity.ServiceUpdatedTime = Convert.ToDateTime(reader["GROUP_SCH_UPDATEDTIME"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "GROUP_SCH_SERVICE_STARTTIME"))
                    entity.ServiceStartedTime = Convert.ToDateTime(reader["GROUP_SCH_SERVICE_STARTTIME"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "GROUP_SCH_SERVICE_COMPTIME"))
                    entity.ServiceCompletionTime = Convert.ToDateTime(reader["GROUP_SCH_SERVICE_COMPTIME"]);

                list.Add(entity);
            }

            return list;
        }
    }
}
