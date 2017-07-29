using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using Cosmo.Entity;
using log4net.Repository.Hierarchy;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;

namespace Cosmo.Data
{
    public class EnvironemntDataOrcl : BusinessEntityBaseDAO, IEnvironemntData
    {
        #region Constant Variables

        private string CONTENT_SERVICE = ConfigurationManager.AppSettings["ContentService"].ToString();
        private string DESPATCHER_SERVICE = ConfigurationManager.AppSettings["DispatcherService"].ToString();

        private const string PackageNameWinService = "COSMO_WINSERVICE_PACKAGE.";
        private const string PackageNameEnvironment = "COSMO_ENVIRONMENT_PACKAGE.";
        private static readonly string Get_Environments = string.Format("{0}FN_CWT_GetEnvironmentList", PackageNameEnvironment);
        private static readonly string GetEnvironmentdetails = string.Format("{0}FN_CWT_GetEnvConfigList", PackageNameEnvironment);
        private static readonly string GetAllConfigEmails = string.Format("{0}FN_CWT_GetAllConfigEmail", PackageNameEnvironment);
        private static readonly string GetEnvironmentId = string.Format("{0}FN_CWT_GetEnvId", PackageNameEnvironment);
        private static readonly string GetEnvironmentIdByConfigId = string.Format("{0}FN_CWT_GetEnvIDByConfigId", PackageNameEnvironment);
        private static readonly string GetEnvironmentConfigId = string.Format("{0}FN_CWT_GetEnvConfigID", PackageNameEnvironment);
        private static readonly string GetServiceExists = string.Format("{0}FN_CWT_ISServiceExists", PackageNameEnvironment);
        private static readonly string GetDispatcherConfigID = string.Format("{0}FN_CWT_GetDispatcherConfigID", PackageNameEnvironment);
        private static readonly string GetGetConfigurationDetails = string.Format("{0}FN_CWT_GetConfigurationDetails", PackageNameEnvironment);

        private static readonly string GetGetUrlConfiguration = string.Format("{0}FN_CWT_GetUrlConfiguration", PackageNameEnvironment);
        private static readonly string IsUrlConfigurationExists = string.Format("{0}FN_CWT_ISUrlConfigExists", PackageNameEnvironment);
        private static readonly string SetUrlConfiguration = string.Format("{0}SP_CWT_InsUpdUrlConfiguration", PackageNameEnvironment);

        private static readonly string GetWindowsServiceDetails = string.Format("{0}FN_CWT_GetWinServiceDetails", PackageNameEnvironment);

        private static readonly string SetEnvironment = string.Format("{0}SP_CWT_InsUpdEnvironmentConfig", PackageNameEnvironment);
        private static readonly string SetEnvironmentEmails = string.Format("{0}SP_CWT_InsUpdEmailUsers", PackageNameEnvironment);
        private static readonly string SetDeleteRecord = string.Format("{0}SP_CWT_DeleteRecord", PackageNameEnvironment);
        private static readonly string SetWindowsService = string.Format("{0}SP_CWT_InsUpdWindowsService", PackageNameEnvironment);

        private static readonly string GetUrlPerformanceInHour = string.Format("{0}FN_CWT_GetUrlPerformance", PackageNameWinService);
        private static readonly string GetUrlPerformanceIn24Hours = string.Format("{0}FN_CWT_GetUrlPerfLast24Hrs", PackageNameWinService);

        #endregion Constant Variables

        public object GetEnvironmentID(string enviName)
        {

            var pList = new List<OracleParameter>
                {
                    GetParameter("p_ENV_NAME", enviName, OracleDbType.Varchar2, ParameterDirection.Input),
                };
            var envId =
                OracleDecimalToInt((OracleDecimal) ReadScalarValue(GetEnvironmentId, pList, OracleDbType.Decimal));

            return envId;
        }

        public object GetEnvironmentID(int configId)
        {
            var pList = new List<OracleParameter>
                {
                    GetParameter("p_CONFIGID", configId, OracleDbType.Int32, ParameterDirection.Input),
                };
            var envId = OracleDecimalToInt((OracleDecimal)ReadScalarValue(GetEnvironmentIdByConfigId, pList, OracleDbType.Decimal));
            return envId;
        }

        public object GetEnvironmentDetailsID(EnvironmentEntity enviData)
        {
            var pList = new List<OracleParameter>
            {
                GetParameter("p_ENV_ID", enviData.EnvID, OracleDbType.Int32, ParameterDirection.Input),
                GetParameter("p_ENV_HOST_IP_ADDRESS", enviData.EnvDetailsList[0].EnvDetHostIP == null ? string.Empty : enviData.EnvDetailsList[0].EnvDetHostIP, OracleDbType.Varchar2, ParameterDirection.Input),
                GetParameter("p_ENV_PORT", enviData.EnvDetailsList[0].EnvDetPort, OracleDbType.Varchar2, ParameterDirection.Input),
                GetParameter("p_ENV_SERVICETYPE", enviData.EnvDetailsList[0].EnvDetServiceType, OracleDbType.Varchar2, ParameterDirection.Input),
            };

            var configId = OracleDecimalToInt((OracleDecimal)ReadScalarValue(GetEnvironmentConfigId, pList, OracleDbType.Decimal));
            return configId;
        }

        public void InsUpdateEnvironment(EnvironmentEntity enviData, string mode)
        {
            try
            {
                var pList = new List<OracleParameter>
                {
                    GetParameter("p_ENV_ID", enviData.EnvID, OracleDbType.Int32, ParameterDirection.Input),
                    GetParameter("p_CONFIG_ID", enviData.EnvDetailsList[0].EnvDetID, OracleDbType.Int32,
                        ParameterDirection.Input),
                    GetParameter("p_ENV_NAME", enviData.EnvName, OracleDbType.Varchar2, ParameterDirection.Input),
                    GetParameter("p_ENV_LOCATION", enviData.EnvDetailsList[0].EnvDetLocation ?? string.Empty,
                        OracleDbType.Varchar2, ParameterDirection.Input),
                    GetParameter("p_ENV_HOST_IP_ADDRESS", enviData.EnvDetailsList[0].EnvDetHostIP ?? string.Empty,
                        OracleDbType.Varchar2, ParameterDirection.Input),
                    GetParameter("p_ENV_PORT", enviData.EnvDetailsList[0].EnvDetPort ?? string.Empty,
                        OracleDbType.Varchar2, ParameterDirection.Input),
                    GetParameter("p_ENV_DESCRIPTION", enviData.EnvDetailsList[0].EnvDetDescription ?? string.Empty,
                        OracleDbType.Varchar2, ParameterDirection.Input),
                    GetParameter("p_ENV_SERVICETYPE", enviData.EnvDetailsList[0].EnvDetServiceType ?? string.Empty,
                        OracleDbType.Varchar2, ParameterDirection.Input),
                    GetParameter("p_ENV_SERVICEURL", enviData.EnvDetailsList[0].EnvDetServiceURL ?? string.Empty,
                        OracleDbType.Varchar2, ParameterDirection.Input),
                    GetParameter("p_ENV_MAIL_FREQ",
                        (object) enviData.EnvDetailsList[0].EnvDetMailFrequency ??
                        Convert.ToInt32(enviData.EnvDetailsList[0].EnvDetMailFrequency), OracleDbType.Int32,
                        ParameterDirection.Input),
                    GetParameter("p_ENV_IS_MONITOR", enviData.EnvDetailsList[0].EnvDetIsMonitor ? 1 : 0,
                        OracleDbType.Int32, ParameterDirection.Input),
                    GetParameter("p_ENV_IS_NOTIFY", enviData.EnvDetailsList[0].EnvDetIsNotify ? 1 : 0,
                        OracleDbType.Int32, ParameterDirection.Input),
                    GetParameter("p_ENV_IS_CONSLTD_MAIL", enviData.EnvDetailsList[0].EnvDetIsServiceConsolidated ? 1 : 0,
                        OracleDbType.Int32, ParameterDirection.Input),
                    GetParameter("p_ENV_COMMENTS", enviData.EnvDetailsList[0].EnvDetComments ?? string.Empty,
                        OracleDbType.Varchar2, ParameterDirection.Input),
                    GetParameter("p_ENV_ISACTIVE", enviData.EnvDetailsList[0].EnvDetIsActive == "true" ? 1 : 0,
                        OracleDbType.Int32, ParameterDirection.Input),
                    GetParameter("p_ENV_CREATED_BY", enviData.EnvCreatedBy, OracleDbType.Int32, ParameterDirection.Input),
                    GetParameter("p_ENV_CREATED_DATE", enviData.EnvCreatedDate, OracleDbType.TimeStamp,
                        ParameterDirection.Input),
                    GetParameter("p_ENV_UPDATED_BY", enviData.EnvUpdatedBy ?? string.Empty, OracleDbType.Varchar2,
                        ParameterDirection.Input),
                    GetParameter("p_ENV_UPDATED_DATE", enviData.EnvUpdatedDate ?? enviData.EnvCreatedDate,
                        OracleDbType.TimeStamp, ParameterDirection.Input),
                    GetParameter("p_CATEGORY", mode, OracleDbType.Varchar2, ParameterDirection.Input),
                    GetParameter("p_CONFIG_ISPRIMARY", enviData.EnvDetailsList[0].EnvDetIsPrimay ? 1 : 0,
                        OracleDbType.Int32, ParameterDirection.Input),
                    GetParameter("p_CONFIG_REF_ID", enviData.EnvDetailsList[0].EnvDetReferenceID, OracleDbType.Int32,
                        ParameterDirection.Input),
                    GetParameter("p_WINDOWS_SERVICE_NAME", enviData.EnvDetailsList[0].WindowsServiceName,
                        OracleDbType.Varchar2, ParameterDirection.Input),
                    GetParameter("p_WINDOWS_SERVICE_ID", enviData.EnvDetailsList[0].WindowsServiceID, OracleDbType.Int32,
                        ParameterDirection.Input),
                    //new OracleParameter("cur", OracleDbType.RefCursor, ParameterDirection.Output)
                };


                ExecuteNonQuery(SetEnvironment, pList);

            }
            catch (SqlException sqex)
            {
                throw sqex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string IsServiceExists(EnvironmentEntity enviData)
        {
            var pList = new List<OracleParameter>
            {
                GetParameter("p_ENV_HOST_IP_ADDRESS", enviData.EnvDetailsList[0].EnvDetHostIP ?? string.Empty,
                    OracleDbType.Varchar2, ParameterDirection.Input),
                GetParameter("p_ENV_PORT", enviData.EnvDetailsList[0].EnvDetPort ?? string.Empty, OracleDbType.Varchar2,
                    ParameterDirection.Input),
            };

            var scopeOutput = ReadScalarValue(GetServiceExists, pList, OracleDbType.Varchar2, 500).ToString();
            return scopeOutput == "null" ? string.Empty : scopeOutput;
        }

        public List<EnvironmentEntity> GetEnvironments(int envId)
        {
            List<EnvironmentEntity> envList;
            try
            {
                var pList = new List<OracleParameter>
                {
                    GetParameter("p_ENV_ID", envId, OracleDbType.Int32, ParameterDirection.Input),
                };

                envList = ReadCompoundEntityList<EnvironmentEntity>(Get_Environments, pList, RowToEnvironmentList);

                if (envList != null && envList.Count > 0)
                {
                    foreach (var entity in envList)
                    {
                        var pListDet = new List<OracleParameter>
                        {
                            GetParameter("p_CONFIG_ID", 0, OracleDbType.Int32, ParameterDirection.Input),
                            GetParameter("p_ENV_ID", entity.EnvID, OracleDbType.Int32, ParameterDirection.Input),
                        };

                        var detList = ReadCompoundEntityList<EnvDetailsEntity>(GetEnvironmentdetails, pListDet, RowToEnvironmentConfigList);
                        if (detList != null)
                            entity.EnvDetailsList = detList;
                    }
                }

            }
            catch (Exception exception)
            {
                throw;
            }

            return envList;
        }
        

        public List<EnvironmentUrlConfiguration> GetEnvironmentUrlConfiguration(int envId, int urlId)
        {
            List<EnvironmentUrlConfiguration> envList;
            try
            {
                var pList = new List<OracleParameter>
                {
                    GetParameter("p_ENV_ID", envId, OracleDbType.Int32, ParameterDirection.Input),
                };

                envList = ReadCompoundEntityList<EnvironmentUrlConfiguration>(Get_Environments, pList, RowToEnvironmentUrlConfigurationList);

                if (envList != null && envList.Count > 0)
                {
                    foreach (var entity in envList)
                    {
                        var pListDet = new List<OracleParameter>
                        {
                            GetParameter("p_ENVID", entity.EnvID, OracleDbType.Int32, ParameterDirection.Input),
                            GetParameter("p_URLID", 0, OracleDbType.Int32, ParameterDirection.Input),
                        };

                        var detList = ReadCompoundEntityList<UrlConfigurationEntity>(GetGetUrlConfiguration, pListDet, RowToURLConfigList);
                        if (detList != null)
                            entity.UrlConfiguration = detList;
                    }
                }

            }
            catch (Exception exception)
            {
                throw;
            }

            return envList;
        }

        private List<UrlConfigurationEntity> RowToURLConfigList(OracleDataReader reader)
        {
            var list = new List<UrlConfigurationEntity>();

            while (reader.Read())
            {
                var entity = new UrlConfigurationEntity();

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "URL_ID"))
                    entity.Id = Convert.ToInt32(reader["URL_ID"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "ENV_ID"))
                    entity.EnvId = Convert.ToInt32(reader["ENV_ID"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "ENV_NAME"))
                    entity.EnvName = Convert.ToString(reader["ENV_NAME"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "URL_TYPE"))
                    entity.Type = Convert.ToString(reader["URL_TYPE"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "URL_ADDRESS"))
                    entity.Adress = Convert.ToString(reader["URL_ADDRESS"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "URL_DISPLAYNAME"))
                    entity.DisplayName = Convert.ToString(reader["URL_DISPLAYNAME"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "URL_MATCHCONTENT"))
                    entity.MatchContent = Convert.ToString(reader["URL_MATCHCONTENT"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "URL_INTERVAL"))
                    entity.Interval = Convert.ToInt32(reader["URL_INTERVAL"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "URL_USERNAME"))
                    entity.UserName = Convert.ToString(reader["URL_USERNAME"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "URL_PASSWORD"))
                    entity.Password = Convert.ToString(reader["URL_PASSWORD"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "URL_ISACTIVE"))
                    entity.IsActive = Convert.ToBoolean(reader["URL_ISACTIVE"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "URL_STATUS"))
                    entity.Status = Convert.ToBoolean(reader["URL_STATUS"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "URL_CREATEDBY"))
                    entity.CreatedBy = Convert.ToString(reader["URL_CREATEDBY"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "URL_CREATEDDATE"))
                    entity.CreatedDateTime = Convert.ToDateTime(reader["URL_CREATEDDATE"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "URL_UPDATEDBY"))
                    entity.UpdatedBy = Convert.ToString(reader["URL_UPDATEDBY"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "URL_LASTJOBRUNTIME"))
                    entity.LastJobRunTime = Convert.ToDateTime(reader["URL_LASTJOBRUNTIME"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "URL_NEXTJOBRUNTIME"))
                    entity.NextJobRunTime = Convert.ToDateTime(reader["URL_NEXTJOBRUNTIME"]);
           
                list.Add(entity);
            }

            return list;
        }


        public List<UrlConfigurationEntity> GetUrlConfigurationEntity(int envId, int urlId)
        {
            var pList = new List<OracleParameter>
            {
                GetParameter("p_ENV_ID", envId, OracleDbType.Int32, ParameterDirection.Input),
                GetParameter("p_URLID", urlId, OracleDbType.Int32, ParameterDirection.Input),
            };
            var envEmailList = ReadCompoundEntityList<UrlConfigurationEntity>(GetGetUrlConfiguration, pList, RowToUrlConfigurationList);
            return envEmailList;

        }

        public List<EnvWindowsServiceEntity> GetAllWindowsServices(int envId)
        {
            List<EnvWindowsServiceEntity> envList;
            try
            {
                var pList = new List<OracleParameter>
                {
                    GetParameter("p_ENV_ID", envId, OracleDbType.Int32, ParameterDirection.Input),
                };

                envList = ReadCompoundEntityList<EnvWindowsServiceEntity>(Get_Environments, pList, RowToEnvWindowsServiceList);

                if (envList != null && envList.Count > 0)
                {
                    foreach (var entity in envList)
                    {
                        var pListDet = new List<OracleParameter>
                        {
                            GetParameter("p_ENVID", entity.EnvID, OracleDbType.Int32, ParameterDirection.Input),
                        };

                        var detList = ReadCompoundEntityList<WinServiceEntity>(GetWindowsServiceDetails, pListDet, RowToWinServiceEntityList);
                        if (detList != null)
                            entity.WinServiceList = detList;
                    }
                }
            }
            catch (Exception)
            {
                
                throw;
            }
            return envList;
        }

        
        public List<EnvironmentEntity> GetEnvironmentSelect(int envId)
        {
            try
            {
                var pList = new List<OracleParameter>
                {
                    GetParameter("p_ENV_ID", envId, OracleDbType.Int32, ParameterDirection.Input),
                };

                var envList = ReadCompoundEntityList<EnvironmentEntity>(Get_Environments, pList, RowToEnvironmentList);
                return envList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<EnvDetailsEntity> GetEnvironmentDetail(int envDetId, int envId)
        {
            var pList = new List<OracleParameter>
            {
                GetParameter("p_CONFIG_ID", envDetId, OracleDbType.Int32, ParameterDirection.Input),
                GetParameter("p_ENV_ID", envId, OracleDbType.Int32, ParameterDirection.Input),
            };
            var envConfig= ReadCompoundEntityList<EnvDetailsEntity>(GetEnvironmentdetails, pList,
                RowToEnvironmentConfigList);
            return envConfig;
        }

        public List<EnvDetailsEntity> GetConfigurationDetails(int envId)
        {
            var pList = new List<OracleParameter>
            {
                GetParameter("p_ENV_ID", envId, OracleDbType.Int32, ParameterDirection.Input),
            };
            var envConfig = ReadCompoundEntityList<EnvDetailsEntity>(GetGetConfigurationDetails, pList,
                RowToEnvironmentConfigList);
            return envConfig;
        }

        public void DeleteRecord(string type, string vID)
        {
            var pList = new List<OracleParameter>
            {
                GetParameter("p_sID", Convert.ToInt32(vID), OracleDbType.Int32, ParameterDirection.Input),
                GetParameter("p_sType", type, OracleDbType.Varchar2, ParameterDirection.Input),
            };
            
            ExecuteNonQuery(SetDeleteRecord, pList);
        }

        public object GetConfigReferenceId(EnvironmentEntity envEntity, string sType)
        {
            var pList = new List<OracleParameter>
            {
                GetParameter("p_CONFIGREFID", envEntity.EnvDetailsList[0].EnvDetID, OracleDbType.Int32, ParameterDirection.Input),
                GetParameter("p_CMURL", envEntity.EnvDetailsList[0].EnvDetServiceURL ?? string.Empty, OracleDbType.Varchar2, ParameterDirection.Input),
                GetParameter("p_STYPE", sType, OracleDbType.Varchar2, ParameterDirection.Input),
            };

            var configId = OracleDecimalToInt((OracleDecimal)ReadScalarValue(GetDispatcherConfigID, pList, OracleDbType.Decimal));
            return configId;

        }

        public List<ConfigEmailsEntity> GetEmailConfiguration(int envId)
        {
            var pList = new List<OracleParameter>
            {
                GetParameter("p_ENV_ID", envId, OracleDbType.Int32, ParameterDirection.Input),
            };
            var envEmailList = ReadCompoundEntityList<ConfigEmailsEntity>(GetAllConfigEmails, pList, RowToEnvironmentEmailList);
            return envEmailList;
        }

        public int InsUpdateEnvUserEmail(ConfigEmailsEntity emailData, string mode)
        {
            int recInsert = 0;
            try
            {
                var pList = new List<OracleParameter>
                {
                    GetParameter("p_USRLST_ID", emailData.UserListID, OracleDbType.Int32, ParameterDirection.Input),
                    GetParameter("p_ENV_ID", emailData.EnvID, OracleDbType.Int32, ParameterDirection.Input),
                    GetParameter("p_USRLST_EMAIL_ADDRESS", emailData.EmailAddress, OracleDbType.Varchar2, ParameterDirection.Input),
                    GetParameter("p_USRLST_MESSAGETYPE", emailData.MessageType, OracleDbType.Varchar2, ParameterDirection.Input),
                    GetParameter("p_USRLST_TYPE", emailData.UserListType, OracleDbType.Varchar2, ParameterDirection.Input),
                    GetParameter("p_USRLST_IS_ACTIVE", emailData.IsAvtive, OracleDbType.Int32, ParameterDirection.Input),
                    GetParameter("p_USRLST_CREATED_BY", emailData.Created_By, OracleDbType.Varchar2, ParameterDirection.Input),
                    GetParameter("p_USRLST_CREATED_DATE", emailData.Created_Date, OracleDbType.TimeStamp, ParameterDirection.Input),
                    GetParameter("p_USRLST_COMMENTS", emailData.Comments, OracleDbType.Varchar2, ParameterDirection.Input),
                };

                ExecuteNonQuery(SetEnvironmentEmails, pList);
                
            }
            catch (SqlException sqex)
            {
                recInsert = 1;
                throw sqex;
            }
            catch (Exception ex)
            {
                recInsert = 1;
                throw ex;
            }
            return recInsert;

            
        }

        public List<ConfigEmailsEntity> GetEnvironmentsForEmail(int envId)
        {
            var pList = new List<OracleParameter>
                {
                    GetParameter("p_ENV_ID", envId, OracleDbType.Int32, ParameterDirection.Input),
                };

            var envList = ReadCompoundEntityList<ConfigEmailsEntity>(Get_Environments, pList, RowToEnvironmentForEmail);
            return envList;
        }



        public int InsUpdateWindowsService(WinServiceEntity wService)
        {
            int rowsAffected = 0;
            try
            {
                var pList = new List<OracleParameter>
                {
                    GetParameter("p_ENV_ID", wService.EnvID, OracleDbType.Int32, ParameterDirection.Input),
                    GetParameter("p_CONFIG_ID", wService.ConfigID, OracleDbType.Int32, ParameterDirection.Input),
                    GetParameter("p_SERVICENAME", wService.ServiceName, OracleDbType.Varchar2, ParameterDirection.Input),
                    GetParameter("p_COMMENTS", wService.Comments, OracleDbType.Varchar2, ParameterDirection.Input),
                    GetParameter("p_CREATED_BY", wService.CreatedBy, OracleDbType.Varchar2, ParameterDirection.Input),
                    GetParameter("p_CREATED_DATE", wService.CreatedDate, OracleDbType.TimeStamp, ParameterDirection.Input),
                };
                ExecuteNonQuery(SetWindowsService, pList);
                rowsAffected = 1;
            }
            catch (Exception)
            {
                rowsAffected = 0;
                throw;
            }
            return rowsAffected;
        }

        public int InsUpdateUrlConfiguration(UrlConfigurationEntity urlConfigurationEntity, string mode)
        {
            //SetUrlConfiguration
            var pList = new List<OracleParameter>
            {
                GetParameter("p_CATEGORY", mode, OracleDbType.Varchar2, ParameterDirection.Input),
                GetParameter("p_URL_ID", urlConfigurationEntity.Id, OracleDbType.Int32, ParameterDirection.Input),
                GetParameter("p_ENVID", urlConfigurationEntity.EnvId, OracleDbType.Int32, ParameterDirection.Input),
                GetParameter("p_URL_TYPE", urlConfigurationEntity.Type ?? string.Empty, OracleDbType.Varchar2, ParameterDirection.Input),
                GetParameter("p_URL_ADDRESS", urlConfigurationEntity.Adress ?? string.Empty, OracleDbType.Varchar2, ParameterDirection.Input),
                GetParameter("p_URL_DISPLAYNAME", urlConfigurationEntity.DisplayName ?? string.Empty, OracleDbType.Varchar2, ParameterDirection.Input),
                GetParameter("p_URL_MATCHCONTENT", urlConfigurationEntity.MatchContent ?? string.Empty, OracleDbType.Varchar2, ParameterDirection.Input),
                GetParameter("p_URL_INTERVAL", urlConfigurationEntity.Interval, OracleDbType.Int32, ParameterDirection.Input),
                GetParameter("p_URL_USERNAME", urlConfigurationEntity.UserName ?? string.Empty, OracleDbType.Varchar2, ParameterDirection.Input),
                GetParameter("p_URL_PASSWORD", urlConfigurationEntity.Password ?? string.Empty, OracleDbType.Varchar2, ParameterDirection.Input),
                GetParameter("p_URL_STATUS", urlConfigurationEntity.Status, OracleDbType.Int32, ParameterDirection.Input),
                GetParameter("p_URL_CREATEDBY", !string.IsNullOrEmpty(urlConfigurationEntity.CreatedBy) ? urlConfigurationEntity.CreatedBy : string.Empty, OracleDbType.Varchar2, ParameterDirection.Input),
                GetParameter("p_URL_UPDATEDBY", !string.IsNullOrEmpty(urlConfigurationEntity.UpdatedBy) ? urlConfigurationEntity.UpdatedBy : string.Empty, OracleDbType.Varchar2, ParameterDirection.Input),
                GetParameter("p_URL_COMMENTS", !string.IsNullOrEmpty(urlConfigurationEntity.Comments) ? urlConfigurationEntity.Comments : string.Empty, OracleDbType.Varchar2, ParameterDirection.Input),
            };

            ExecuteNonQuery(SetUrlConfiguration, pList);
            return 1;
        }

        public string IsUrlConfigExists(UrlConfigurationEntity urlConfigurationEntity)
        {
            var pList = new List<OracleParameter>
            {
                GetParameter("p_ENVID", urlConfigurationEntity.EnvId, OracleDbType.Int32, ParameterDirection.Input),
                GetParameter("p_URL_TYPE", urlConfigurationEntity.Type, OracleDbType.Varchar2, ParameterDirection.Input),
                GetParameter("p_URL_ADDRESS", urlConfigurationEntity.Adress ?? string.Empty, OracleDbType.Varchar2, ParameterDirection.Input),
            };
            
            var scopeOutput = ReadScalarValue(IsUrlConfigurationExists, pList, OracleDbType.Varchar2, 500).ToString();
            return scopeOutput == "null" ? string.Empty : scopeOutput;
            
        }

        public List<UrlPerformance> GetUrlPerformance(int envId)
        {
            var pList = new List<OracleParameter>
            {
                GetParameter("p_ENVID", envId, OracleDbType.Varchar2, ParameterDirection.Input),
            };

            var performanceList = ReadCompoundEntityList<UrlPerformance>(GetUrlPerformanceInHour, pList, RowToUrlPerformanceList);
            if (performanceList == null || performanceList.Count <= 0) return new List<UrlPerformance>();

            foreach (var urlPerformance in performanceList)
            {
                var last24Hour = GetUrlPerformanceLast24Hours(urlPerformance.EnvId);
                urlPerformance.ResponseTimeLast24Hour = last24Hour;
            }

            return performanceList;
        }

        public List<UrlPerformanceByLast24Hour> GetUrlPerformanceLast24Hours(int envId)
        {
            var pList = new List<OracleParameter>
            {
                GetParameter("p_ENVID", envId, OracleDbType.Int32, ParameterDirection.Input),
            };

            var performanceList = ReadCompoundEntityList<UrlPerformanceByLast24Hour>(GetUrlPerformanceIn24Hours, pList, RowToUrlPerformanceIn24HoursList);

            return performanceList;
        }

        private List<EnvDetailsEntity> RowToEnvironmentConfigList(OracleDataReader reader)
        {
            var list = new List<EnvDetailsEntity>();

            while (reader.Read())
            {
                var entuty = new EnvDetailsEntity();
                if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_ID"))
                    entuty.EnvDetID = Convert.ToInt32(reader["CONFIG_ID"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "ENV_ID"))
                    entuty.EnvID = Convert.ToInt32(reader["ENV_ID"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "ENV_NAME"))
                    entuty.EnvDet_Name = Convert.ToString(reader["ENV_NAME"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_HOST_IP"))
                    entuty.EnvDetHostIP = Convert.ToString(reader["CONFIG_HOST_IP"]).ToLower(CultureInfo.CurrentCulture);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_PORT_NUMBER"))
                    entuty.EnvDetPort = Convert.ToString(reader["CONFIG_PORT_NUMBER"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_DESCRIPTION"))
                    entuty.EnvDetDescription = Convert.ToString(reader["CONFIG_DESCRIPTION"]);

                if (string.IsNullOrEmpty(entuty.EnvDetDescription))
                {
                    if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_HOST_IP"))
                        entuty.EnvDetDescription = entuty.EnvDetHostIP + ":" + entuty.EnvDetPort;
                }

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_SERVICE_TYPE"))
                {
                    entuty.EnvDetServiceType = Convert.ToString(reader["CONFIG_SERVICE_TYPE"]);
                    if (Convert.ToString(reader["CONFIG_SERVICE_TYPE"]) == "1")
                        entuty.EnvDetServiceTypeDesc = CONTENT_SERVICE;
                    else if (Convert.ToString(reader["CONFIG_SERVICE_TYPE"]) == "2")
                        entuty.EnvDetServiceTypeDesc = DESPATCHER_SERVICE;
                    else
                        entuty.EnvDetServiceTypeDesc = "Service Type does not available";
                }

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_URL_ADDRESS"))
                    entuty.EnvDetServiceURL = Convert.ToString(reader["CONFIG_URL_ADDRESS"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_LOCATION"))
                    entuty.EnvDetLocation = Convert.ToString(reader["CONFIG_LOCATION"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_MAIL_FREQ"))
                    entuty.EnvDetMailFrequency = Convert.ToString(reader["CONFIG_MAIL_FREQ"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_IS_MONITORED"))
                    entuty.EnvDetIsMonitor = Convert.ToBoolean(reader["CONFIG_IS_MONITORED"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_ISNOTIFY"))
                    entuty.EnvDetIsNotify = Convert.ToBoolean(reader["CONFIG_ISNOTIFY"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_IS_VALIDATED"))
                    entuty.EnvDetIsValidated = Convert.ToBoolean(reader["CONFIG_IS_VALIDATED"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_IS_LOCKED"))
                    entuty.EnvDetIsLocked = Convert.ToBoolean(reader["CONFIG_IS_LOCKED"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_ISCONSOLIDATED"))
                    entuty.EnvDetIsServiceConsolidated = Convert.ToBoolean(reader["CONFIG_ISCONSOLIDATED"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_IS_ACTIVE"))
                    entuty.EnvDetIsActive = Convert.ToString(reader["CONFIG_IS_ACTIVE"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "USERFIRSTNAME"))
                    entuty.EnvDetCreatedBy = Convert.ToString(reader["USERFIRSTNAME"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_CREATED_DATE"))
                    entuty.EnvDetCreatedDate = Convert.ToDateTime(reader["CONFIG_CREATED_DATE"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "USERFIRSTNAME"))
                    entuty.EnvDetUpdatedBy = Convert.ToString(reader["USERFIRSTNAME"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_UPDATED_DATE"))
                    entuty.EnvDetUpdatedDate = Convert.ToDateTime(reader["CONFIG_UPDATED_DATE"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_COMMENTS"))
                    entuty.EnvDetComments = Convert.ToString(reader["CONFIG_COMMENTS"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "SCH_ID"))
                    entuty.SchedulerID = Convert.ToInt32(reader["SCH_ID"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "SCH_COMMENTS"))
                    entuty.SchedulerSummary = Convert.ToString(reader["SCH_COMMENTS"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_ISPRIMARY"))
                    entuty.EnvDetIsPrimay = Convert.ToBoolean(reader["CONFIG_ISPRIMARY"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_REF_ID"))
                    entuty.EnvDetReferenceID = Convert.ToInt32(reader["CONFIG_REF_ID"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "WIN_SERVICE_ID"))
                    entuty.WindowsServiceID = Convert.ToInt32(reader["WIN_SERVICE_ID"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "WIN_SERVICENAME"))
                    entuty.WindowsServiceName = Convert.ToString(reader["WIN_SERVICENAME"]);
                list.Add(entuty);
            }

            return list;
        }

        private List<EnvironmentEntity> RowToEnvironmentList(OracleDataReader reader)
        {
            var list = new List<EnvironmentEntity>();

            while (reader.Read())
            {
                var entity = new EnvironmentEntity();
                if (CommonUtility.IsColumnExistsAndNotNull(reader, "ENV_ID"))
                    entity.EnvID = Convert.ToInt32(reader["ENV_ID"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "ENV_NAME"))
                    entity.EnvName = Convert.ToString(reader["ENV_NAME"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "ENV_IS_MONITOR"))
                    entity.EnvIsMonitor = Convert.ToBoolean(reader["ENV_IS_MONITOR"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "ENV_IS_NOTIFY"))
                    entity.EnvIsNotify = Convert.ToBoolean(reader["ENV_IS_NOTIFY"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "ENV_IS_CONSLTD_MAIL"))
                    entity.EnvIsServiceConsolidated = Convert.ToBoolean(reader["ENV_IS_CONSLTD_MAIL"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "ENV_MAIL_FREQ"))
                    entity.EnvMailFrequency = Convert.ToString(reader["ENV_MAIL_FREQ"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "ENV_CREATED_BY"))
                    entity.EnvCreatedBy = Convert.ToString(reader["ENV_CREATED_BY"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "ENV_CREATED_DATE"))
                    entity.EnvCreatedDate = Convert.ToDateTime(reader["ENV_CREATED_DATE"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "ENV_UPDATED_BY"))
                    entity.EnvUpdatedBy = Convert.ToString(reader["ENV_UPDATED_BY"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "ENV_UPDATED_DATE"))
                    entity.EnvUpdatedDate = Convert.ToDateTime(reader["ENV_UPDATED_DATE"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "ENV_COMMENTS"))
                    entity.EnvComments = Convert.ToString(reader["ENV_COMMENTS"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "ENV_ISACTIVE"))
                    entity.EnvIsActive = Convert.ToString(reader["ENV_ISACTIVE"]);


                entity.EnvDetailsList = new List<EnvDetailsEntity>();
                list.Add(entity);
            }
            return list;
        }

        private List<ConfigEmailsEntity> RowToEnvironmentForEmail(OracleDataReader reader)
        {
            var list = new List<ConfigEmailsEntity>();

            while (reader.Read())
            {
                var entity = new ConfigEmailsEntity
                {
                    EnvID =
                        CommonUtility.IsColumnExistsAndNotNull(reader, "ENV_ID") ? Convert.ToInt32(reader["ENV_ID"]) : 0,
                    EnvName =
                        CommonUtility.IsColumnExistsAndNotNull(reader, "ENV_NAME")
                            ? Convert.ToString(reader["ENV_NAME"])
                            : string.Empty,

                };

                list.Add(entity);
            }
            return list;
        }

        private List<ConfigEmailsEntity> RowToEnvironmentEmailList(OracleDataReader reader)
        {
            var list = new List<ConfigEmailsEntity>();

            while (reader.Read())
            {
                var entity = new ConfigEmailsEntity
                {
                    UserListID =
                        CommonUtility.IsColumnExistsAndNotNull(reader, "USRLST_ID")
                            ? Convert.ToInt32(reader["USRLST_ID"])
                            : 0,
                    EnvID =
                        CommonUtility.IsColumnExistsAndNotNull(reader, "ENV_ID") ? Convert.ToInt32(reader["ENV_ID"]) : 0,
                    EnvName =
                        CommonUtility.IsColumnExistsAndNotNull(reader, "ENV_NAME")
                            ? Convert.ToString(reader["ENV_NAME"])
                            : string.Empty,
                    UserListType =
                        CommonUtility.IsColumnExistsAndNotNull(reader, "USRLST_TYPE")
                            ? Convert.ToString(reader["USRLST_TYPE"])
                            : string.Empty,
                    EmailAddress =
                        CommonUtility.IsColumnExistsAndNotNull(reader, "USRLST_EMAIL_ADDRESS")
                            ? Convert.ToString(reader["USRLST_EMAIL_ADDRESS"])
                            : string.Empty,
                    MessageType =
                        CommonUtility.IsColumnExistsAndNotNull(reader, "USRLST_MESSAGETYPE")
                            ? Convert.ToString(reader["USRLST_MESSAGETYPE"])
                            : string.Empty,
                    IsAvtive =
                        CommonUtility.IsColumnExistsAndNotNull(reader, "USRLST_IS_ACTIVE") &&
                        Convert.ToBoolean(reader["USRLST_IS_ACTIVE"])
                };

                list.Add(entity);
            }
            
            return list;
        }

        private static List<UrlConfigurationEntity> RowToUrlConfigurationList(OracleDataReader reader)
        {
            var list = new List<UrlConfigurationEntity>();

            while (reader.Read())
            {
                var entity = new UrlConfigurationEntity();

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "URL_ID"))
                    entity.Id = Convert.ToInt32(reader["URL_ID"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "ENV_ID"))
                    entity.EnvId = Convert.ToInt32(reader["ENV_ID"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "ENV_NAME"))
                    entity.EnvName = Convert.ToString(reader["ENV_NAME"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "URL_TYPE"))
                    entity.Type = Convert.ToString(reader["URL_TYPE"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "URL_ADDRESS"))
                    entity.Adress = Convert.ToString(reader["URL_ADDRESS"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "URL_DISPLAYNAME"))
                    entity.DisplayName = Convert.ToString(reader["URL_DISPLAYNAME"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "URL_MATCHCONTENT"))
                    entity.MatchContent = Convert.ToString(reader["URL_MATCHCONTENT"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "URL_INTERVAL"))
                    entity.Interval = Convert.ToInt32(reader["URL_INTERVAL"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "URL_USERNAME"))
                    entity.UserName = Convert.ToString(reader["URL_USERNAME"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "URL_PASSWORD"))
                    entity.Password = Convert.ToString(reader["URL_PASSWORD"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "URL_ISACTIVE"))
                    entity.IsActive = Convert.ToBoolean(reader["URL_ISACTIVE"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "URL_STATUS"))
                    entity.Status = Convert.ToBoolean(reader["URL_STATUS"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "URL_CREATEDBY"))
                    entity.CreatedBy = Convert.ToString(reader["URL_CREATEDBY"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "URL_CREATEDDATE"))
                    entity.CreatedDateTime = Convert.ToDateTime(reader["URL_CREATEDDATE"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "URL_UPDATEDBY"))
                    entity.UpdatedBy = Convert.ToString(reader["URL_UPDATEDBY"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "URL_LASTJOBRUNTIME"))
                    entity.LastJobRunTime = Convert.ToDateTime(reader["URL_LASTJOBRUNTIME"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "URL_NEXTJOBRUNTIME"))
                    entity.NextJobRunTime = Convert.ToDateTime(reader["URL_NEXTJOBRUNTIME"]);

                list.Add(entity);
            }

            return list;
        }

        private static List<EnvironmentUrlConfiguration> RowToEnvironmentUrlConfigurationList(OracleDataReader reader)
        {
            var list = new List<EnvironmentUrlConfiguration>();

            while (reader.Read())
            {
                var entity = new EnvironmentUrlConfiguration();
                if (CommonUtility.IsColumnExistsAndNotNull(reader, "ENV_ID"))
                    entity.EnvID = Convert.ToInt32(reader["ENV_ID"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "ENV_NAME"))
                    entity.EnvName = Convert.ToString(reader["ENV_NAME"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "ENV_IS_MONITOR"))
                    entity.IsMonitor = Convert.ToBoolean(reader["ENV_IS_MONITOR"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "ENV_IS_NOTIFY"))
                    entity.IsNotify = Convert.ToBoolean(reader["ENV_IS_NOTIFY"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "ENV_IS_CONSLTD_MAIL"))
                    entity.IsConsolidated = Convert.ToBoolean(reader["ENV_IS_CONSLTD_MAIL"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "ENV_SORTORDER"))
                    entity.SortOrder = Convert.ToInt32(reader["ENV_SORTORDER"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "ENV_ISACTIVE"))
                    entity.IsActive = Convert.ToBoolean(reader["ENV_ISACTIVE"]);

                entity.UrlConfiguration = new List<UrlConfigurationEntity>();
                list.Add(entity);
            }
            return list;
        }


        private List<EnvWindowsServiceEntity> RowToEnvWindowsServiceList(OracleDataReader reader)
        {
            var list = new List<EnvWindowsServiceEntity>();

            while (reader.Read())
            {
                var entity = new EnvWindowsServiceEntity();

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "ENV_ID"))
                    entity.EnvID = Convert.ToInt32(reader["ENV_ID"]);
                if (CommonUtility.IsColumnExistsAndNotNull(reader, "ENV_NAME"))
                    entity.EnvName = Convert.ToString(reader["ENV_NAME"]);
                
                if (CommonUtility.IsColumnExistsAndNotNull(reader, "ENV_MAIL_FREQ"))
                    entity.EnvMailFrequency = Convert.ToString(reader["ENV_MAIL_FREQ"]);
                if (CommonUtility.IsColumnExistsAndNotNull(reader, "ENV_IS_MONITOR"))
                    entity.EnvIsMonitor = Convert.ToBoolean(reader["ENV_IS_MONITOR"]);
                if (CommonUtility.IsColumnExistsAndNotNull(reader, "ENV_IS_NOTIFY"))
                    entity.EnvIsNotify = Convert.ToBoolean(reader["ENV_IS_NOTIFY"]);
                if (CommonUtility.IsColumnExistsAndNotNull(reader, "ENV_IS_CONSLTD_MAIL"))
                    entity.EnvIsServiceConsolidated = Convert.ToBoolean(reader["ENV_IS_CONSLTD_MAIL"]);
                if (CommonUtility.IsColumnExistsAndNotNull(reader, "ENV_CREATED_BY"))
                    entity.EnvCreatedBy = Convert.ToString(reader["ENV_CREATED_BY"]);
                if (CommonUtility.IsColumnExistsAndNotNull(reader, "ENV_CREATED_DATE"))
                    entity.EnvCreatedDate = Convert.ToDateTime(reader["ENV_CREATED_DATE"]);
                if (CommonUtility.IsColumnExistsAndNotNull(reader, "ENV_UPDATED_BY"))
                    entity.EnvUpdatedBy = Convert.ToString(reader["ENV_UPDATED_BY"]);
                if (CommonUtility.IsColumnExistsAndNotNull(reader, "ENV_UPDATED_DATE"))
                    entity.EnvUpdatedDate = Convert.ToDateTime(reader["ENV_UPDATED_DATE"]);
                if (CommonUtility.IsColumnExistsAndNotNull(reader, "ENV_COMMENTS"))
                    entity.EnvComments = Convert.ToString(reader["ENV_COMMENTS"]);
                if (CommonUtility.IsColumnExistsAndNotNull(reader, "ENV_ISACTIVE"))
                    entity.EnvIsActive = Convert.ToString(reader["ENV_ISACTIVE"]);
                entity.WinServiceList = new List<WinServiceEntity>();

                list.Add(entity);
            }

            return list;
        }

        private static List<WinServiceEntity> RowToWinServiceEntityList(OracleDataReader reader)
        {
            var list = new List<WinServiceEntity>();

            while (reader.Read())
            {
                var entity = new WinServiceEntity();
                if (CommonUtility.IsColumnExistsAndNotNull(reader, "ENV_ID"))
                    entity.EnvID = Convert.ToInt32(reader["ENV_ID"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "ENV_NAME"))
                    entity.EnvName = Convert.ToString(reader["ENV_NAME"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_ID"))
                    entity.ConfigID = Convert.ToInt32(reader["CONFIG_ID"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_HOST_IP"))
                    entity.HostIP = Convert.ToString(reader["CONFIG_HOST_IP"]).ToLower(CultureInfo.CurrentCulture);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_PORT_NUMBER"))
                    entity.Port = Convert.ToString(reader["CONFIG_PORT_NUMBER"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_LOCATION"))
                    entity.Location = Convert.ToString(reader["CONFIG_LOCATION"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "WIN_SERVICE_ID"))
                    entity.ID = Convert.ToInt32(reader["WIN_SERVICE_ID"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "WIN_SERVICENAME"))
                    entity.ServiceName = Convert.ToString(reader["WIN_SERVICENAME"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "WIN_COMMENTS"))
                    entity.Comments = Convert.ToString(reader["WIN_COMMENTS"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_CREATED_BY"))
                    entity.CreatedBy = Convert.ToString(reader["CONFIG_CREATED_BY"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_CREATED_DATE"))
                    entity.CreatedDate = Convert.ToDateTime(reader["CONFIG_CREATED_DATE"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_UPDATED_BY"))
                    entity.UpdatedBy = Convert.ToString(reader["CONFIG_UPDATED_BY"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_UPDATED_DATE"))
                    entity.UpdatedDate = Convert.ToDateTime(reader["CONFIG_UPDATED_DATE"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_SERVICE_TYPE"))
                    entity.ServiceType = Convert.ToString(reader["CONFIG_SERVICE_TYPE"]);

                entity.MonitorStatus = CommonUtility.IsColumnExistsAndNotNull(reader, "MONITOR_STATUS") ? Convert.ToString(reader["MONITOR_STATUS"]) : string.Empty;

                entity.MonitorComments = CommonUtility.IsColumnExistsAndNotNull(reader, "MONITOR_COMMENTS") ? Convert.ToString(reader["MONITOR_COMMENTS"]) : string.Empty;

                list.Add(entity);
            }

            return list;
        }

        private List<UrlPerformance> RowToUrlPerformanceList(OracleDataReader reader)
        {
            var list = new List<UrlPerformance>();

            while (reader.Read())
            {
                var entity = new UrlPerformance();

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "URL_ID"))
                    entity.Id = Convert.ToInt32(reader["URL_ID"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "ENV_ID"))
                    entity.EnvId = Convert.ToInt32(reader["ENV_ID"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "ENV_NAME"))
                    entity.EnvName = Convert.ToString(reader["ENV_NAME"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "URL_TYPE"))
                    entity.Type = Convert.ToString(reader["URL_TYPE"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "URL_ADDRESS"))
                    entity.Adress = Convert.ToString(reader["URL_ADDRESS"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "URL_DISPLAYNAME"))
                    entity.DisplayName = Convert.ToString(reader["URL_DISPLAYNAME"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "RESPONSETIME"))
                    entity.ResponseTimeLastPing = Convert.ToDouble(reader["RESPONSETIME"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "RESPONSETIMEINHOUR"))
                    entity.ResponseTimeLastHour = Convert.ToDouble(reader["RESPONSETIMEINHOUR"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "LASTPINGDATETIME"))
                    entity.LastPingDateTime = Convert.ToDateTime(reader["LASTPINGDATETIME"]);

                list.Add(entity);
            }

            return list;
        }

        private List<UrlPerformanceByLast24Hour> RowToUrlPerformanceIn24HoursList(OracleDataReader reader)
        {
            var list = new List<UrlPerformanceByLast24Hour>();

            while (reader.Read())
            {
                var entity = new UrlPerformanceByLast24Hour();

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "TOTALRT"))
                    entity.Total = Convert.ToDouble(reader["TOTALRT"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "AVGRT"))
                    entity.Average = Convert.ToDouble(reader["AVGRT"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "HOURRT"))
                    entity.Hour = Convert.ToInt32(reader["HOURRT"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "DATERT"))
                    entity.Date = Convert.ToInt32(reader["DATERT"]);

                list.Add(entity);
            }

            return list;
        }

    }
}
