using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cosmo.Data;
using Cosmo.Entity;

namespace Cosmo.Data
{
    public interface IEnvironemntData
    {
        object GetEnvironmentID(string enviName);
        object GetEnvironmentID(int configId);
        object GetEnvironmentDetailsID(EnvironmentEntity enviData);
        void InsUpdateEnvironment(EnvironmentEntity enviData, string mode);
        string IsServiceExists(EnvironmentEntity enviData);
        List<EnvironmentEntity> GetEnvironments(int envId);
        List<EnvironmentUrlConfiguration> GetEnvironmentUrlConfiguration(int envId, int urlId);
        List<UrlConfigurationEntity> GetUrlConfigurationEntity(int envId, int urlId);
        List<EnvWindowsServiceEntity> GetAllWindowsServices(int envId);
        List<EnvironmentEntity> GetEnvironmentSelect(int envId);
        List<EnvDetailsEntity> GetEnvironmentDetail(int envDetId, int envId);
        List<EnvDetailsEntity> GetConfigurationDetails(int envId);
        void DeleteRecord(string type, string vID);
        object GetConfigReferenceId(EnvironmentEntity envEntity, string sType);
        List<ConfigEmailsEntity> GetEmailConfiguration(int envId);
        int InsUpdateEnvUserEmail(ConfigEmailsEntity emailData, string mode);
        List<ConfigEmailsEntity> GetEnvironmentsForEmail(int envId);
        int InsUpdateWindowsService(WinServiceEntity wService);
        int InsUpdateUrlConfiguration(UrlConfigurationEntity urlConfigurationEntity, string mode);
        string IsUrlConfigExists(UrlConfigurationEntity urlConfigurationEntity);
        List<UrlPerformance> GetUrlPerformance(int envId);
        List<UrlPerformanceByLast24Hour> GetUrlPerformanceLast24Hours(int envId);
    }

    public class EnvironemntDataFactory : IEnvironemntDataFactory
    {
        public IEnvironemntData Create(string dbType)
        {
            return dbType == Convert.ToInt32(DatabaseType.Oracle).ToString()
                ? (IEnvironemntData)new EnvironemntDataOrcl()
                : new EnvironemntData();
        }
    }
    
    public interface IEnvironemntDataFactory
    {
        IEnvironemntData Create(string dbType);
    }
}
