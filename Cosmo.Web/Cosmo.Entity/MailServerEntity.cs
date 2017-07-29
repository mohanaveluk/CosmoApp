using System;
using System.Data.SqlClient;

namespace Cosmo.Entity
{
    public class MailServerEntity: TRW.NamedBusinessCode, IFill
    {
        #region property declaration

        public int ID { get; set; }
        public string ServerName { get; set; }
        public string HostIP { get; set; }
        public string Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool SslEnable { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDateTimeDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDateTime { get; set; }
        public string Comments { get; set; }
        public string FromEmailAddress { get; set; }

        #endregion property declaration

        #region IFill Logging of job details
        
        public void Fill(SqlDataReader reader)
        {
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "EMAIL_SERVER_ID"))
                this.ID = Convert.ToInt32(reader["EMAIL_SERVER_ID"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "EMAIL_SERVER_NAME"))
                this.ServerName = Convert.ToString(reader["EMAIL_SERVER_NAME"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "EMAIL_SERVER_IPADDRESS"))
                this.HostIP = Convert.ToString(reader["EMAIL_SERVER_IPADDRESS"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "EMAIL_SERVER_PORT"))
                this.Port = Convert.ToString(reader["EMAIL_SERVER_PORT"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "EMAIL_AUTH_USER_ID"))
                this.Username = Convert.ToString(reader["EMAIL_AUTH_USER_ID"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "EMAIL_AUTH_USER_PWD"))
                this.Password = Convert.ToString(reader["EMAIL_AUTH_USER_PWD"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "EMAIL_IS_ACTIVE"))
                this.IsActive = Convert.ToBoolean(reader["EMAIL_IS_ACTIVE"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "EMAIL_CREATED_BY"))
                this.CreatedBy = Convert.ToString(reader["EMAIL_CREATED_BY"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "EMAIL_CREATED_DATE"))
                this.CreatedDateTimeDate = Convert.ToDateTime(reader["EMAIL_CREATED_DATE"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "EMAIL_UPDATED_BY"))
                this.UpdatedBy = Convert.ToString(reader["EMAIL_UPDATED_BY"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "EMAIL_UPDATED_DATE"))
                this.UpdatedDateTime = Convert.ToDateTime(reader["EMAIL_UPDATED_DATE"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "EMAIL_SSL_ENABLE"))
                this.SslEnable = Convert.ToBoolean(reader["EMAIL_SSL_ENABLE"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "EMAIL_ADMIN_MAILADRESS"))
                this.FromEmailAddress  = Convert.ToString(reader["EMAIL_ADMIN_MAILADRESS"]);


        }

        #endregion IFill Logging of job details
    }
}
