using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cosmo.Entity;

namespace Cosmo.Data
{
    public interface ICommonDataFactory
    {
        ICommonData Create(string dbType);
    }

    public class CommonDataFactory : ICommonDataFactory
    {
        public ICommonData Create(string dbType)
        {
            return dbType == Convert.ToInt32(DatabaseType.Oracle).ToString()
                ? (ICommonData) new CommonDataOrcl()
                : new CommonData();
        }
    }

    public interface ICommonData
    {
        int InsMailServerConfiguration(MailServerEntity mailServer);
        List<MailServerEntity> GetMailServer();
        ActivationEntity GetUserAccess();
        int InsActivation(ActivationEntity activation);
    }
}
