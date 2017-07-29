using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Cosmo.Entity;

namespace Cosmo.Data
{
    public class CommonData : ICommonData
    {
        private const string SetMailserverdetail = "CWT_InsMailServer";
        private const string GetMailserverdetail = "CWT_GetMailServerDetail";
        private const string SetActivationKey = "CWT_InsUserAccess";
        private const string GetActivationKey = "CWT_GetUserAccess";

        public int InsMailServerConfiguration(MailServerEntity mailServer)
        {
            var recInsert = 1;

            var pList = new List<SqlParameter>
            {
                new SqlParameter("@ServerName", mailServer.HostIP),
                new SqlParameter("@ServerPort", mailServer.Port),
                new SqlParameter("@Username", mailServer.Username),
                new SqlParameter("@Password", mailServer.Password),
                new SqlParameter("@SSLEnable", mailServer.SslEnable),
                new SqlParameter("@IsActive", mailServer.IsActive),
                new SqlParameter("@FromEmailId", mailServer.FromEmailAddress)
            };
            UtilityDL.ExecuteNonQuery(SetMailserverdetail, pList);
            recInsert = 0;

            return recInsert;
        }

        public List<MailServerEntity> GetMailServer()
        {
            return UtilityDL.FillData<MailServerEntity>(GetMailserverdetail);
        }

        public ActivationEntity GetUserAccess()
        {
            return UtilityDL.FillEntity<ActivationEntity>(GetActivationKey, new List<SqlParameter>());
        }

        public int InsActivation(ActivationEntity activation)
        {
            var recInsert = 1;

            var pList = new List<SqlParameter>
            {
                new SqlParameter("@ACCESS_CODE", activation.Key),
            };
            UtilityDL.ExecuteNonQuery(SetActivationKey, pList);
            recInsert = 0;

            return recInsert;
        }

    }
}
