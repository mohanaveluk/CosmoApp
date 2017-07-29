using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Cosmo.Entity;
using Oracle.ManagedDataAccess.Client;

namespace Cosmo.Data
{
    public class CommonDataOrcl : BusinessEntityBaseDAO,ICommonData
    {

        private const string PackageName = "COSMO_COMMON_PACKAGE.";
        private static readonly string SetMailserverdetail = string.Format("{0}SP_CWT_InsMailServer",PackageName);
        private static readonly string GetMailserverdetail = string.Format("{0}FN_CWT_GetMailServerDetail", PackageName);
        private static readonly string SetActivationKey = string.Format("{0}SP_CWT_InsUserAccess", PackageName);
        private static readonly string GetActivationKey = string.Format("{0}FN_CWT_GetUserAccess", PackageName);

        public int InsMailServerConfiguration(MailServerEntity mailServer)
        {
            var recInsert = 1;
            try
            {
                var pList = new List<OracleParameter>
                {
                    GetParameter("p_ServerName", mailServer.HostIP, OracleDbType.Varchar2, ParameterDirection.Input),
                    GetParameter("p_ServerPort", mailServer.Port, OracleDbType.Varchar2, ParameterDirection.Input),
                    GetParameter("p_Username", mailServer.Username, OracleDbType.Varchar2, ParameterDirection.Input),
                    GetParameter("p_Password", mailServer.Password, OracleDbType.Varchar2, ParameterDirection.Input),
                    GetParameter("p_SSLEnable", mailServer.SslEnable, OracleDbType.Int32, ParameterDirection.Input),
                    GetParameter("p_IsActive", mailServer.IsActive, OracleDbType.Int32, ParameterDirection.Input),
                    GetParameter("p_FromEmailId", mailServer.FromEmailAddress, OracleDbType.Varchar2,
                        ParameterDirection.Input),
                };
                ExecuteNonQuery(SetMailserverdetail, pList);

            }
            catch (Exception)
            {
                recInsert = -1;
            }

            return recInsert;
        }

        public List<MailServerEntity> GetMailServer()
        {
            var pList = new List<OracleParameter>();
            var mailServerDetail = ReadCompoundEntityList<MailServerEntity>(GetMailserverdetail, pList, RowToMailServerList);

            return mailServerDetail;
        }

        
        public ActivationEntity GetUserAccess()
        {
            var access = ReadEntityList<ActivationEntity>(GetActivationKey, new List<OracleParameter>(), RowToUserAccess);
            return access != null && access.Count > 0 ? access[0] : new ActivationEntity();
        }

        public int InsActivation(ActivationEntity activation)
        {
            var result = 0;
            try
            {
                var pList = new List<OracleParameter>
                {
                    GetParameter("p_ACCESS_CODE", activation.Key, OracleDbType.Varchar2, ParameterDirection.Input),
                };
                ExecuteNonQuery(SetActivationKey, pList);
            }
            catch (Exception)
            {

                result = 1;
            }

            return result;
        }

        private ActivationEntity RowToUserAccess(OracleDataReader reader)
        {
            var entity = new ActivationEntity();
            if (reader != null)
            {
                entity = new ActivationEntity().Fill(reader);
            }
            return entity;
        }

        private List<MailServerEntity> RowToMailServerList(OracleDataReader reader)
        {
            var list = new List<MailServerEntity>();

            while (reader.Read())
            {
                var entity = new MailServerEntity();
                if (CommonUtility.IsColumnExistsAndNotNull(reader, "EMAIL_SERVER_ID"))
                    entity.ID = Convert.ToInt32(reader["EMAIL_SERVER_ID"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "EMAIL_SERVER_NAME"))
                    entity.ServerName = Convert.ToString(reader["EMAIL_SERVER_NAME"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "EMAIL_SERVER_IPADDRESS"))
                    entity.HostIP = Convert.ToString(reader["EMAIL_SERVER_IPADDRESS"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "EMAIL_SERVER_PORT"))
                    entity.Port = Convert.ToString(reader["EMAIL_SERVER_PORT"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "EMAIL_AUTH_USER_ID"))
                    entity.Username = Convert.ToString(reader["EMAIL_AUTH_USER_ID"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "EMAIL_AUTH_USER_PWD"))
                    entity.Password = Convert.ToString(reader["EMAIL_AUTH_USER_PWD"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "EMAIL_IS_ACTIVE"))
                    entity.IsActive = Convert.ToBoolean(reader["EMAIL_IS_ACTIVE"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "EMAIL_CREATED_BY"))
                    entity.CreatedBy = Convert.ToString(reader["EMAIL_CREATED_BY"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "EMAIL_CREATED_DATE"))
                    entity.CreatedDateTimeDate = Convert.ToDateTime(reader["EMAIL_CREATED_DATE"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "EMAIL_UPDATED_BY"))
                    entity.UpdatedBy = Convert.ToString(reader["EMAIL_UPDATED_BY"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "EMAIL_UPDATED_DATE"))
                    entity.UpdatedDateTime = Convert.ToDateTime(reader["EMAIL_UPDATED_DATE"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "EMAIL_SSL_ENABLE"))
                    entity.SslEnable = Convert.ToBoolean(reader["EMAIL_SSL_ENABLE"]);

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "EMAIL_ADMIN_MAILADRESS"))
                    entity.FromEmailAddress = Convert.ToString(reader["EMAIL_ADMIN_MAILADRESS"]);

                list.Add(entity);
            }

            return list;
        }

    }
}
