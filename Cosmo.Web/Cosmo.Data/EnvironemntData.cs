using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cosmo.Entity;
using System.Data.SqlClient;
using System.Data;

namespace Cosmo.Data
{
    internal class ConnectionInfo
    {
        internal static string connectionString = System.Configuration.ConfigurationManager.AppSettings["ConnectionString"];
    }
    public class EnvironemntData : IEnvironemntData
    {

        #region Constant variables
        private const string GET_ENVIRONMENTID = "CWT_GetEnvID";
        private const string GET_ENVIRONMENTIDBYCONFIGID = "CWT_GetEnvIDByConfigID";
        private const string GET_ENVIRONMENTDETAILSID = "CWT_GetEnvConfigID";//"CWT_GetEnvDetailsID"; replaced with CWT_GetEnvConfigID
        private const string SET_ENVIRONMENT = "CWT_InsUpdEnvironmentConfig";//"CWT_InsUpdEnvironment"; earlier it was CWT_InsUpdEnvironment
        private const string GET_ENVIRONMENTS = "CWT_GetEnvironmentList";
        private const string GET_ENVIRONMENTDETAILS = "CWT_GetEnvironmentConfigList";//"CWT_GetEnvironmentDetailsList"; it was used previously
        private const string SET_DELETERECORD = "CWT_DeleteRecord";
        private const string SET_SCHEDULERLASTRUNDATETIME = "UpdateSchedulerLastRunDateTime";
        private const string SET_GETUSEREMAIL = "getUserEmailList";
        private const string SET_GETCONFIGURATIONDETAIL = "CWT_GetConfigurationDetails";
        private const string GET_DISPATCHER_CONFIGID = "CWT_GetDispatcherConfigID";
        private const string ISSERVICEEXISTS = "CWT_ISServiceExists";

        private const string SOURCE_NAME = "env";

        private const string GET_ENVIRONMENTS_EMAILS = "CWT_GetAllConfigEmail";
        private const string SET_INSUPDUSEREMAILS = "CWT_InsUpdEmailUsers";

        private const string GET_WINDOWSDETAILS = "CWT_GetWindowsServiceDetails";

        private const string SET_WINDOWSSERVICE = "CWT_InsUpdWindowsService";
        private const string GetUrlConfiguration = "CWT_GetUrlConfiguration";
        private const string InsUpdUrlConfiguration = "CWT_InsUpdUrlConfiguration";
        private const string IsUrlConfigurationExists = "CWT_ISUrlConfigExists";

        private const string GetUrlPerformanceInHour = "CWT_GetUrlPerformance";
        private const string GetUrlPerformanceIn24Hours = "CWT_GetUrlPerformanceLast24Hours";




        #endregion

        /// <summary>
        /// Get Environment ID if already exists
        /// </summary>
        /// <param name="enviData"></param>
        /// <returns></returns>
        public object GetEnvironmentID(string enviName)
        {
            List<SqlParameter> pList = new List<SqlParameter>();
            pList.Add(new SqlParameter("@ENV_NAME", enviName));
            return UtilityDL.ReadScalar(GET_ENVIRONMENTID, pList);
        }

        /// <summary>
        /// Get Environment ID if already exists
        /// </summary>
        /// <param name="enviData"></param>
        /// <param name="configId"></param>
        /// <returns></returns>
        public object GetEnvironmentID(int configId)
        {
            List<SqlParameter> pList = new List<SqlParameter>();
            pList.Add(new SqlParameter("@CONFIGID", configId));
            return UtilityDL.ReadScalar(GET_ENVIRONMENTIDBYCONFIGID, pList);
        }

        /// <summary>
        /// Get Environment details ID if already exists
        /// </summary>
        /// <param name="enviData"></param>
        /// <returns></returns>
        public object GetEnvironmentDetailsID(EnvironmentEntity enviData)
        {
            List<SqlParameter> pList = new List<SqlParameter>();
            pList.Add(new SqlParameter("@ENV_ID", enviData.EnvID));
            pList.Add(new SqlParameter("@ENV_HOST_IP_ADDRESS", enviData.EnvDetailsList[0].EnvDetHostIP == null ? string.Empty : enviData.EnvDetailsList[0].EnvDetHostIP));
            pList.Add(new SqlParameter("@ENV_PORT", enviData.EnvDetailsList[0].EnvDetPort));
            pList.Add(new SqlParameter("@ENV_SERVICETYPE", enviData.EnvDetailsList[0].EnvDetServiceType));
            //pList.Add(new SqlParameter("@ENV_LOCATION", enviData.EnvDetailsList[0].EnvDetLocation));
            //pList.Add(new SqlParameter("@ENV_SERVICEURL", enviData.EnvDetailsList[0].EnvDetServiceURL));
            return UtilityDL.ReadScalar(GET_ENVIRONMENTDETAILSID, pList);
        }

        /// <summary>
        /// Insert update environment data to database
        /// </summary>
        /// <param name="enviData"></param>
        public void InsUpdateEnvironment(EnvironmentEntity enviData, string mode)
        {
            try
            {
                List<SqlParameter> pList = new List<SqlParameter>();
                pList.Add(new SqlParameter("@ENV_ID", enviData.EnvID));
                pList.Add(new SqlParameter("@CONFIG_ID", enviData.EnvDetailsList[0].EnvDetID));
                pList.Add(new SqlParameter("@ENV_NAME", enviData.EnvName));
                pList.Add(new SqlParameter("@ENV_LOCATION", enviData.EnvDetailsList[0].EnvDetLocation ?? string.Empty));
                pList.Add(new SqlParameter("@ENV_HOST_IP_ADDRESS", enviData.EnvDetailsList[0].EnvDetHostIP ?? string.Empty));
                pList.Add(new SqlParameter("@ENV_PORT", enviData.EnvDetailsList[0].EnvDetPort ?? string.Empty));
                pList.Add(new SqlParameter("@ENV_DESCRIPTION", enviData.EnvDetailsList[0].EnvDetDescription ?? string.Empty));

                pList.Add(new SqlParameter("@ENV_SERVICETYPE", enviData.EnvDetailsList[0].EnvDetServiceType ?? string.Empty));
                pList.Add(new SqlParameter("@ENV_SERVICEURL", enviData.EnvDetailsList[0].EnvDetServiceURL ?? string.Empty));
                pList.Add(new SqlParameter("@ENV_MAIL_FREQ", enviData.EnvDetailsList[0].EnvDetMailFrequency));


                pList.Add(new SqlParameter("@ENV_IS_MONITOR", enviData.EnvDetailsList[0].EnvDetIsMonitor));
                pList.Add(new SqlParameter("@ENV_IS_NOTIFY", enviData.EnvDetailsList[0].EnvDetIsNotify));
                pList.Add(new SqlParameter("@ENV_IS_CONSLTD_MAIL", enviData.EnvDetailsList[0].EnvDetIsServiceConsolidated));
                pList.Add(new SqlParameter("@ENV_COMMENTS", enviData.EnvDetailsList[0].EnvDetComments ?? string.Empty));
                pList.Add(new SqlParameter("@ENV_ISACTIVE", enviData.EnvDetailsList[0].EnvDetIsActive));

                pList.Add(new SqlParameter("@ENV_CREATED_BY", enviData.EnvCreatedBy));
                pList.Add(new SqlParameter("@ENV_CREATED_DATE", enviData.EnvCreatedDate));
                pList.Add(new SqlParameter("@ENV_UPDATED_BY", enviData.EnvUpdatedBy ?? ""));
                pList.Add(new SqlParameter("@ENV_UPDATED_DATE", enviData.EnvUpdatedDate ?? enviData.EnvCreatedDate));
                pList.Add(new SqlParameter("@CATEGORY", mode));
                pList.Add(new SqlParameter("@CONFIG_ISPRIMARY", enviData.EnvDetailsList[0].EnvDetIsPrimay));
                pList.Add(new SqlParameter("@CONFIG_REF_ID", enviData.EnvDetailsList[0].EnvDetReferenceID));

                pList.Add(new SqlParameter("@WINDOWS_SERVICE_NAME", enviData.EnvDetailsList[0].WindowsServiceName));
                pList.Add(new SqlParameter("@WINDOWS_SERVICE_ID", enviData.EnvDetailsList[0].WindowsServiceID));

                UtilityDL.ExecuteNonQuery(SET_ENVIRONMENT, pList);
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
            string scopeOutput = string.Empty;
            try
            {
                SqlParameter retVal = new SqlParameter("@SCOPE_OUTPUT", SqlDbType.VarChar, -1);
                retVal.Direction = ParameterDirection.Output;
                var pList = new List<SqlParameter>
                {
                    new SqlParameter("@ENV_HOST_IP_ADDRESS",
                        enviData.EnvDetailsList[0].EnvDetHostIP ?? string.Empty),
                    new SqlParameter("@ENV_PORT",
                        enviData.EnvDetailsList[0].EnvDetPort ?? string.Empty),
                    retVal
                };
                scopeOutput = Convert.ToString( UtilityDL.ExecuteNonQuery(ISSERVICEEXISTS, pList,true));
            }
            catch(SqlException ex)
            {
                throw ex;
            }

            return scopeOutput;
        }

        /// <summary>
        /// Get all environments
        /// </summary>
        /// <param name="envId"></param>
        /// <returns></returns>
        public List<EnvironmentEntity> GetEnvironments(int envId)
        {
            var envList = new List<EnvironmentEntity>();
            var detList = new List<EnvDetailsEntity>();
            var pList = new List<SqlParameter>();
            try
            {
                pList.Add(new SqlParameter("@ENV_ID", envId));
                string stProc = GET_ENVIRONMENTS;
                envList = UtilityDL.FillData<EnvironmentEntity>(stProc, pList);
                if (envList != null && envList.Count > 0)
                {
                    foreach (var entity in envList)
                    {
                        detList = new List<EnvDetailsEntity>();
                        List<SqlParameter> pListDet = new List<SqlParameter>();
                        pListDet.Add(new SqlParameter("@CONFIG_ID", "0"));
                        pListDet.Add(new SqlParameter("@ENV_ID", entity.EnvID));
                        detList = UtilityDL.FillData<EnvDetailsEntity>(GET_ENVIRONMENTDETAILS, pListDet);
                        if (detList != null)
                            entity.EnvDetailsList = detList;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return envList;
        }

        /// <summary>
        /// Get all environments
        /// </summary>
        /// <param name="envId"></param>
        /// <returns></returns>
        public List<EnvironmentUrlConfiguration> GetEnvironmentUrlConfiguration(int envId, int urlId)
        {
            List<EnvironmentUrlConfiguration> envList = new List<EnvironmentUrlConfiguration>();
            List<UrlConfigurationEntity> urlConfigurationList = new List<UrlConfigurationEntity>();
            List<SqlParameter> pList = new List<SqlParameter>();
            try
            {
                pList.Add(new SqlParameter("@ENV_ID", envId));
                string stProc = GET_ENVIRONMENTS;
                envList = UtilityDL.FillData<EnvironmentUrlConfiguration>(stProc, pList);
                if (envList != null && envList.Count > 0)
                {
                    foreach (EnvironmentUrlConfiguration entity in envList)
                    {
                        urlConfigurationList = new List<UrlConfigurationEntity>();
                        List<SqlParameter> pListDet = new List<SqlParameter>();
                        pListDet.Add(new SqlParameter("@ENVID", entity.EnvID));
                        pListDet.Add(new SqlParameter("@URLID", urlId));
                        urlConfigurationList = UtilityDL.FillData<UrlConfigurationEntity>(GetUrlConfiguration, pListDet);
                        if (urlConfigurationList != null)
                            entity.UrlConfiguration = urlConfigurationList;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return envList;
        }

        public List<UrlConfigurationEntity> GetUrlConfigurationEntity(int envId, int urlId)
        {
            List<SqlParameter> pListDet = new List<SqlParameter>();
            pListDet.Add(new SqlParameter("@ENVID", envId));
            pListDet.Add(new SqlParameter("@URLID", urlId));
            return UtilityDL.FillData<UrlConfigurationEntity>(GetUrlConfiguration, pListDet);

        }

        /// <summary>
        /// Get all environments with Windows services
        /// </summary>
        /// <param name="env_id"></param>
        /// <returns></returns>
        public List<EnvWindowsServiceEntity> GetAllWindowsServices(int envId)
        {
            List<EnvWindowsServiceEntity> envList = new List<EnvWindowsServiceEntity>();
            List<WinServiceEntity> detList = new List<WinServiceEntity>();
            List<SqlParameter> pList = new List<SqlParameter>();
            try
            {
                pList.Add(new SqlParameter("@ENV_ID", envId));
                string stProc = GET_ENVIRONMENTS;
                envList = UtilityDL.FillData<EnvWindowsServiceEntity>(stProc, pList);
                if (envList != null && envList.Count > 0)
                {
                    foreach (EnvWindowsServiceEntity entity in envList)
                    {
                        detList = new List<WinServiceEntity>();
                        List<SqlParameter> pListDet = new List<SqlParameter>();
                        pListDet.Add(new SqlParameter("@ENV_ID", entity.EnvID));
                        detList = UtilityDL.FillData<WinServiceEntity>(GET_WINDOWSDETAILS, pListDet);
                        if (detList != null)
                            entity.WinServiceList = detList;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return envList;
        }

        /// <summary>
        /// To get environment code and name to populate into the dropdown
        /// </summary>
        /// <returns></returns>
        public List<EnvironmentEntity> GetEnvironmentSelect(int envId)
        {
            List<SqlParameter> pList = new List<SqlParameter>();
            pList.Add(new SqlParameter("@ENV_ID", envId));
            string stProc = GET_ENVIRONMENTS;
            return UtilityDL.FillData<EnvironmentEntity>(stProc, pList);
        }

        /// <summary>
        /// To get environment detail based on the code to populate
        /// </summary>
        /// <returns></returns>
        public List<EnvDetailsEntity> GetEnvironmentDetail(int envDetId, int envId)
        {
            List<SqlParameter> pList = new List<SqlParameter>();
            pList.Add(new SqlParameter("@CONFIG_ID", envDetId));
            pList.Add(new SqlParameter("@ENV_ID", envId));
            string stProc = GET_ENVIRONMENTDETAILS;
            return UtilityDL.FillData<EnvDetailsEntity>(stProc, pList);
        }

        /// <summary>
        /// To get environment / configuration details based on the env id 
        /// </summary>
        /// <returns></returns>
        public List<EnvDetailsEntity> GetConfigurationDetails(int envId)
        {
            List<SqlParameter> pList = new List<SqlParameter>();
            pList.Add(new SqlParameter("@ENV_ID", envId));
            string stProc = SET_GETCONFIGURATIONDETAIL;
            return UtilityDL.FillData<EnvDetailsEntity>(stProc, pList);
        }
        
        /// <summary>
        /// Soft delete a environment detail based on the Env ID
        /// </summary>
        /// <param name="vID"></param>
        public void DeleteRecord(string type, string vId)
        {
            List<SqlParameter> pList = new List<SqlParameter>();
            pList.Add(new SqlParameter("@sID", vId));
            pList.Add(new SqlParameter("@sType", type));
            UtilityDL.ExecuteNonQuery(SET_DELETERECORD, pList);
        }

        /// <summary>
        /// To get Reference Id of Service ID
        /// </summary>
        /// <param name="envEntity"></param>
        /// <param name="sType"></param>
        /// <returns></returns>
        public object GetConfigReferenceId(EnvironmentEntity envEntity, string sType)
        {
            List<SqlParameter> pList = new List<SqlParameter>();
            pList.Add(new SqlParameter("@CONFIGREFID", envEntity.EnvDetailsList[0].EnvDetID));
            pList.Add(new SqlParameter("@CMURL", envEntity.EnvDetailsList[0].EnvDetServiceURL));
            pList.Add(new SqlParameter("@STYPE", sType));
            return UtilityDL.ReadScalar(GET_DISPATCHER_CONFIGID, pList);
        }


        /// <summary>
        /// To get environment email configuration details based on the env id 
        /// </summary>
        /// <returns></returns>
        public List<ConfigEmailsEntity> GetEmailConfiguration(int envId)
        {
            List<SqlParameter> pList = new List<SqlParameter>();
            pList.Add(new SqlParameter("@ENV_ID", envId));
            string stProc = GET_ENVIRONMENTS_EMAILS;
            return UtilityDL.FillData<ConfigEmailsEntity>(stProc, pList);
        }

        /// <summary>
        /// Insert update environment based user email address whom would get email alert in case of service failure 
        /// </summary>
        /// <param name="enviData"></param>
        public int InsUpdateEnvUserEmail(ConfigEmailsEntity emailData, string mode)
        {
            int recInsert = 0;
            try
            {
                List<SqlParameter> pList = new List<SqlParameter>();
                pList.Add(new SqlParameter("@USRLST_ID", emailData.UserListID));
                pList.Add(new SqlParameter("@ENV_ID", emailData.EnvID));
                pList.Add(new SqlParameter("@USRLST_EMAIL_ADDRESS", emailData.EmailAddress));
                pList.Add(new SqlParameter("@USRLST_MESSAGETYPE", emailData.MessageType));
                pList.Add(new SqlParameter("@USRLST_TYPE", emailData.UserListType));
                pList.Add(new SqlParameter("@USRLST_IS_ACTIVE", emailData.IsAvtive));
                pList.Add(new SqlParameter("@USRLST_CREATED_BY", emailData.Created_By));
                pList.Add(new SqlParameter("@USRLST_CREATED_DATE", emailData.Created_Date));
                pList.Add(new SqlParameter("@USRLST_COMMENTS", emailData.Comments));

                UtilityDL.ExecuteNonQuery(SET_INSUPDUSEREMAILS, pList);
                recInsert = 0;
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
            List<SqlParameter> pList = new List<SqlParameter>();
            pList.Add(new SqlParameter("@ENV_ID", envId));
            string stProc = GET_ENVIRONMENTS;
            return UtilityDL.FillData<ConfigEmailsEntity>(stProc, pList);
        }

        /// <summary>
        /// Insert update environment data to database
        /// </summary>
        /// <param name="wService"></param>
        public int  InsUpdateWindowsService(WinServiceEntity wService)
        {
            int rowsAffected = 0;
            try
            {
                List<SqlParameter> pList = new List<SqlParameter>();
                pList.Add(new SqlParameter("@ENV_ID", wService.EnvID));
                pList.Add(new SqlParameter("@CONFIG_ID", wService.ConfigID));
                pList.Add(new SqlParameter("@SERVICENAME", wService.ServiceName));
                pList.Add(new SqlParameter("@COMMENTS", wService.Comments));

                pList.Add(new SqlParameter("@CREATED_BY", wService.CreatedBy));
                pList.Add(new SqlParameter("@CREATED_DATE", wService.CreatedDate));

                UtilityDL.ExecuteNonQuery(SET_WINDOWSSERVICE, pList);
                rowsAffected = 1;
            }
            catch (Exception ex)
            {
                rowsAffected = 0;
                throw ex;
            }
            return rowsAffected;
        }


        public int InsUpdateUrlConfiguration(UrlConfigurationEntity urlConfigurationEntity, string mode)
        {
            var scopeOutput = 0;
            try
            {
               var retVal = new SqlParameter("@SCOPE_OUTPUT", SqlDbType.VarChar, -1);
                retVal.Direction = ParameterDirection.Output;
                var pList = new List<SqlParameter>
                {
                    new SqlParameter("@CATEGORY", mode),
                    new SqlParameter("@URL_ID", urlConfigurationEntity.Id),
                    new SqlParameter("@ENVID", urlConfigurationEntity.EnvId),
                    new SqlParameter("@URL_TYPE", urlConfigurationEntity.Type),
                    new SqlParameter("@URL_ADDRESS", urlConfigurationEntity.Adress),
                    new SqlParameter("@URL_DISPLAYNAME", urlConfigurationEntity.DisplayName),
                    new SqlParameter("@URL_MATCHCONTENT", urlConfigurationEntity.MatchContent),
                    new SqlParameter("@URL_INTERVAL", urlConfigurationEntity.Interval),
                    new SqlParameter("@URL_USERNAME", urlConfigurationEntity.UserName),
                    new SqlParameter("@URL_PASSWORD", urlConfigurationEntity.Password),
                    new SqlParameter("@URL_STATUS", urlConfigurationEntity.Status),
                    new SqlParameter("@URL_CREATEDBY", !string.IsNullOrEmpty(urlConfigurationEntity.CreatedBy) ? urlConfigurationEntity.CreatedBy : string.Empty),
                    new SqlParameter("@URL_UPDATEDBY", !string.IsNullOrEmpty(urlConfigurationEntity.UpdatedBy) ? urlConfigurationEntity.UpdatedBy : string.Empty),
                    new SqlParameter("@URL_COMMENTS", !string.IsNullOrEmpty(urlConfigurationEntity.Comments) ? urlConfigurationEntity.Comments : string.Empty),
                    retVal,
                };

                scopeOutput = Convert.ToInt32(UtilityDL.ExecuteNonQuery(InsUpdUrlConfiguration, pList, true));
            }
            catch (SqlException sqex)
            {
                throw sqex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return scopeOutput;
        }


        public string IsUrlConfigExists(UrlConfigurationEntity urlConfigurationEntity)
        {
            string scopeOutput = string.Empty;
            try
            {
                SqlParameter retVal = new SqlParameter("@SCOPE_OUTPUT", SqlDbType.VarChar, -1);
                retVal.Direction = ParameterDirection.Output;
                var pList = new List<SqlParameter>
                {
                    new SqlParameter("@ENVID", urlConfigurationEntity.EnvId),
                    new SqlParameter("@URL_TYPE", urlConfigurationEntity.Type),
                    new SqlParameter("@URL_ADDRESS",
                        urlConfigurationEntity.Adress ?? string.Empty),
                    retVal
                };
                scopeOutput = Convert.ToString(UtilityDL.ExecuteNonQuery(IsUrlConfigurationExists, pList, true));
            }
            catch (SqlException ex)
            {
                throw ex;
            }

            return scopeOutput;
        }

        public List<UrlPerformance> GetUrlPerformance(int envId)
        {
            var pList = new List<SqlParameter> {new SqlParameter("@ENVID", envId)};

            var performanceList = UtilityDL.FillData<UrlPerformance>(GetUrlPerformanceInHour, pList);

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
            List<SqlParameter> pList = new List<SqlParameter>();
            pList.Add(new SqlParameter("@ENVID", envId));
            return UtilityDL.FillData<UrlPerformanceByLast24Hour>(GetUrlPerformanceIn24Hours, pList);
        }
    }
}
