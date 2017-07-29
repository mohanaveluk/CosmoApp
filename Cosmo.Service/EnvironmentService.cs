using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using Cosmo.Data;
using Cosmo.Entity;

namespace Cosmo.Service
{
    public class EnvironmentService
    {
        private readonly IEnvironemntData _enviDataLayer;

        private const string ADD = "add_en";
        private const string ADD_ED = "add_ed";
        private const string UPDATE = "modify_en";
        private const string UPDATE_ED = "modify_ed";

        #region Variables

        private static readonly string DbType = ConfigurationManager.AppSettings["DatabaseType"];

        #endregion Variables

        public EnvironmentService()
        {
            var iDbType = DbType == DatabaseType.Oracle.ToString()
                ? Convert.ToInt32(DatabaseType.Oracle).ToString()
                : Convert.ToInt32(DatabaseType.SqlServer).ToString();

            _enviDataLayer = new EnvironemntDataFactory().Create(Convert.ToInt32(iDbType).ToString());
        }
        /// <summary>
        /// To get Environment ID
        /// </summary>
        /// <param name="enviData"></param>
        /// <returns></returns>
        public int GetEnvironmentID(string enviName)
        {
            int envID=0;
            object environment = _enviDataLayer.GetEnvironmentID(enviName);
            if (environment != null)
            {
                envID = Convert.ToInt32(environment);
            }
            return envID;
        }

        public int GetEnvironmentID(int configID)
        {
            int envID = 0;

            try
            {
                object environment = _enviDataLayer.GetEnvironmentID(configID);
                if (environment != null)
                {
                    envID = Convert.ToInt32(environment);
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex.ToString());
                throw;
            }
            return envID;
        }

        /// <summary>
        /// To get Environment details ID
        /// </summary>
        /// <param name="enviData"></param>
        /// <returns></returns>
        public int GetEnvironmentDetailID(EnvironmentEntity enviData)
        {
            int envID = 0;
            object environment = _enviDataLayer.GetEnvironmentDetailsID(enviData);
            if (environment != null)
            {
                envID = Convert.ToInt32(environment);
            }
            return envID;
        }

        /// <summary>
        /// Check whether the given Config details like Host and Port number is already exists in the database
        /// </summary>
        /// <param name="enviData"></param>
        /// <returns></returns>
        public string IsServiceExists(EnvironmentEntity enviData)
        {
            return _enviDataLayer.IsServiceExists(enviData);
        }
        /// <summary>
        /// To insert update the Environment and its details
        /// </summary>
        /// <param name="enviData"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public int InsUpdEnvironment(EnvironmentEntity enviData, string mode)
        {
            int endStatus = 0;
            string HostExists = string.Empty;
            try
            {
                if (mode == ADD)
                {
                    if (GetEnvironmentID(enviData.EnvName.ToString()) == 0)
                        _enviDataLayer.InsUpdateEnvironment(enviData, ADD);
                    else if (GetEnvironmentDetailID(enviData) == 0)
                        _enviDataLayer.InsUpdateEnvironment(enviData, ADD_ED);
                    else
                        endStatus = 1;
                }
                else if (mode == UPDATE)
                {
                    _enviDataLayer.InsUpdateEnvironment(enviData, UPDATE);
                }
                else if (mode == UPDATE_ED)
                {
                    _enviDataLayer.InsUpdateEnvironment(enviData, UPDATE_ED);
                }
            }
            catch (Exception ex)
            {
                endStatus = 2;
                Logger.Log(ex.ToString());
            }

            return endStatus;
        }

        /// <summary>
        /// Get all environments
        /// </summary>
        /// <param name="envId"></param>
        /// <returns></returns>
        public List<EnvironmentEntity> GetEnvironments(int envId)
        {
            try
            {
                return _enviDataLayer.GetEnvironments(envId);
            }
            catch (Exception ex)
            {
                Logger.Log(ex.StackTrace);
                //return new List<EnvironmentEntity>();
                throw ex;
            }

        }


        /// <summary>
        /// Get all url configuration for selected / all environment
        /// </summary>
        /// <param name="envId"></param>
        /// <param name="urlId"></param>
        /// <returns></returns>
        public List<EnvironmentUrlConfiguration> GetEnvironmentUrlConfiguration(int envId, int urlId)
        {
            return _enviDataLayer.GetEnvironmentUrlConfiguration(envId, urlId);
        }

        /// <summary>
        /// Get all url configuration for selected / all environment
        /// </summary>
        /// <param name="envId"></param>
        /// <param name="urlId"></param>
        /// <returns></returns>
        public List<UrlConfigurationEntity> GetUrlConfiguration(int envId, int urlId)
        {
            var detail = _enviDataLayer.GetUrlConfigurationEntity(envId, urlId);
            if (detail != null && detail.Count > 0)
            {
                return detail;
            }

            return new List<UrlConfigurationEntity>();
        }
        /// <summary>
        /// Get all environments with windows services
        /// </summary>
        /// <param name="env_id"></param>
        /// <returns></returns>
        public List<EnvWindowsServiceEntity> GetAllWindowsServices(int env_id)
        {
            try
            {
                return _enviDataLayer.GetAllWindowsServices(env_id);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        /// <summary>
        /// To get environment code and name to populate into the dropdown
        /// </summary>
        /// <param name="env_id"></param>
        /// <returns></returns>
        public List<NamedBusinessEntity> GetEnvironmentSelect(int env_id)
        {
            List<EnvironmentEntity> enviList = new List<EnvironmentEntity>();
            List<NamedBusinessEntity> namedlist = new List<NamedBusinessEntity>();
            enviList = _enviDataLayer.GetEnvironmentSelect(env_id);
            //namedlist.Add(new NamedBusinessEntity(-1, "Select"));
            if (enviList != null && enviList.Count > 0)
            {

                foreach (EnvironmentEntity env in enviList)
                {
                    NamedBusinessEntity list = new NamedBusinessEntity();
                    list.ID = env.EnvID;
                    list.Name = env.EnvName;
                    namedlist.Add(list);
                }
            }
            return namedlist;
        }

        /// <summary>
        /// Get specific environment details based on the Environment ID
        /// </summary>
        /// <param name="env_id"></param>
        /// <returns></returns>
        public EnvironmentEntity GetEnvironment(int env_id)
        {
            List<EnvironmentEntity> entityList = new List<EnvironmentEntity>();
            EnvironmentEntity entity = new EnvironmentEntity();
            try
            {
                entityList = _enviDataLayer.GetEnvironments(env_id);
                if (entityList != null && entityList.Count > 0)
                {
                    entity = entityList[0];
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex.ToString());
                throw;
            }
            return entity;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="env_id"></param>
        /// <returns></returns>
        public EnvDetailsEntity GetEnvironmentDetail(int envDetID, int envID)
        {
            List<EnvDetailsEntity> envDetails = new List<EnvDetailsEntity>();
            EnvDetailsEntity detail = new EnvDetailsEntity();
            try
            {
                envDetails = _enviDataLayer.GetEnvironmentDetail(envDetID, envID);
                if (envDetails != null && envDetails.Count > 0)
                    detail = envDetails[0];
            }
            catch (Exception)
            {
                
                throw;
            }

            return detail;
        }

        /// <summary>
        /// To get environment / configuration details based on the env id 
        /// </summary>
        /// <param name="envId"></param>
        /// <returns></returns>
        public List<EnvDetailsEntity> GetConfigurationDetails(int envId)
        {
            return _enviDataLayer.GetConfigurationDetails(envId);
        }

        /// <summary>
        /// Delete record based on the env id
        /// </summary>
        /// <param name="type"></param>
        /// <param name="vID"></param>
        public void DeleteRecord(string type, string vID)
        {
            _enviDataLayer.DeleteRecord(type,vID);
        }

        /// <summary>
        /// To get Reference Id of Service ID
        /// </summary>
        /// <param name="envEntity"></param>
        /// <param name="sType"></param>
        /// <returns></returns>
        public int GetConfigReferenceID(EnvironmentEntity envEntity, string sType)
        {
            int iConfigRefID=0;
            object oID = _enviDataLayer.GetConfigReferenceId(envEntity, sType);
            if (oID !=null)
                iConfigRefID = (int)oID;

            return iConfigRefID;
        }

        /// <summary>
        /// To get environment email configuration details based on the env id 
        /// </summary>
        /// <returns></returns>
        public List<ConfigEmailsEntity> GetEmailConfiguration(int envID)
        {
            return _enviDataLayer.GetEmailConfiguration(envID);
        }

        /// <summary>
        /// To get unique environment email configuration details based on the env id 
        /// </summary>
        /// <returns></returns>
        public List<ConfigEmailsEntity> GetEnvEmailConfiguration(int envID)
        {
            List<ConfigEmailsEntity> envList = new List<ConfigEmailsEntity>();
            envList = _enviDataLayer.GetEnvironmentsForEmail(0);

            List<ConfigEmailsEntity> emailList = new List<ConfigEmailsEntity>();
            List<ConfigEmailsEntity> envEmailList = new List<ConfigEmailsEntity>();
            ConfigEmailsEntity emailEntity = new ConfigEmailsEntity();

            foreach (ConfigEmailsEntity envnt in envList)
            {
                emailList = new List<ConfigEmailsEntity>();
                emailList = _enviDataLayer.GetEmailConfiguration(envnt.EnvID);
                if (emailList != null && emailList.Count > 0)
                {
                    foreach (ConfigEmailsEntity list in emailList)
                    {
                        emailEntity = new ConfigEmailsEntity();
                        if (envEmailList.Any(em => em.EnvID == list.EnvID))
                        {
                            emailEntity = envEmailList.Where(em => em.EnvID == list.EnvID).ToList<ConfigEmailsEntity>()[0];
                            if (list.UserListType.ToLower() == "to")
                                emailEntity.ToAddress = string.IsNullOrEmpty(emailEntity.ToAddress) ? list.EmailAddress : emailEntity.ToAddress + ", " + list.EmailAddress;
                            else if (list.UserListType.ToLower() == "cc")
                                emailEntity.CcAddress = string.IsNullOrEmpty(emailEntity.CcAddress) ? list.EmailAddress : emailEntity.CcAddress + ", " + list.EmailAddress;
                            else if (list.UserListType.ToLower() == "bcc")
                                emailEntity.BccAddress = string.IsNullOrEmpty(emailEntity.BccAddress) ? list.EmailAddress : emailEntity.BccAddress + ", " + list.EmailAddress;
                        }
                        else
                        {
                            emailEntity.EnvID = list.EnvID;
                            emailEntity.EnvName = list.EnvName;

                            if (list.UserListType.ToLower() == "to")
                                emailEntity.ToAddress = list.EmailAddress;
                            else if (list.UserListType.ToLower() == "cc")
                                emailEntity.CcAddress = list.EmailAddress;
                            else if (list.UserListType.ToLower() == "bcc")
                                emailEntity.BccAddress = list.EmailAddress;

                            emailEntity.FromAddress = list.FromAddress;

                            envEmailList.Add(emailEntity);
                        }
                    }
                }
                else
                {
                    emailEntity = new ConfigEmailsEntity();
                    emailEntity.EnvID = envnt.EnvID;
                    emailEntity.EnvName = envnt.EnvName;
                    envEmailList.Add(emailEntity);
                }
            }
            return envEmailList;
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
                recInsert = _enviDataLayer.InsUpdateEnvUserEmail(emailData, mode);
            }
            catch (Exception ex)
            {
                recInsert = 1;
                Logger.Log(ex.ToString());
            }
            return recInsert;
        }

        /// <summary>
        /// Insert update Windows service details
        /// </summary>
        /// <param name="enviData"></param>
        public int InsUpdateWindowsService(string serviceName, string comments, int EnvID, int ConfigID)
        {
            try
            {
                WinServiceEntity wService = new WinServiceEntity();
                wService.ConfigID = ConfigID;
                wService.EnvID = EnvID;
                wService.Comments = comments;
                wService.ServiceName = serviceName;
                wService.CreatedBy = HttpContext.Current.Session["_LOGGED_USERD_ID"] != null ? HttpContext.Current.Session["_LOGGED_USERD_ID"].ToString() : string.Empty;
                wService.CreatedDate = DateTime.Now;
                return _enviDataLayer.InsUpdateWindowsService(wService);
            }
            catch (Exception ex)
            {
                Logger.Log(ex.ToString());
                throw ex;
            }
        }

        public int InsUpdateUrlConfiguration(UrlConfigurationEntity urlConfigurationEntity, string mode)
        {
            urlConfigurationEntity.CreatedBy = urlConfigurationEntity.UpdatedBy = HttpContext.Current.Session["_LOGGED_USERD_ID"] != null ? HttpContext.Current.Session["_LOGGED_USERD_ID"].ToString() : string.Empty;
            
            return _enviDataLayer.InsUpdateUrlConfiguration(urlConfigurationEntity, mode);
        }

        public string IsUrlConfigExists(UrlConfigurationEntity urlConfigurationEntity)
        {
            return _enviDataLayer.IsUrlConfigExists(urlConfigurationEntity);
        }

        public List<UrlPerformance> GetUrlPerformance(int envId)
        {
            return _enviDataLayer.GetUrlPerformance(envId);
        }
    }
}
