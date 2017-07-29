using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Security.Cryptography;

namespace Cosmo.Entity
{
    public class ConfigEmailsEntity : TRW.NamedBusinessEntity, IFill
    {
        public int EnvID { get; set; }
        public string EnvName { get; set; }
        public int UserListID { get; set; }
        public string UserListType { get; set; }
        public string EmailAddress { get; set; }
        public string MessageType { get; set; }
        public string FromAddress { get; set; }
        public string ToAddress { get; set; }
        public string CcAddress { get; set; }
        public string BccAddress { get; set; }
        public bool IsAvtive { get; set; }
        public string Created_By { get; set; }
        public DateTime Created_Date { get; set; }
        public string Comments { get; set; }

        #region Fill user email configuration details
        /// <summary>
        /// Get all email configuration details for the environment
        /// </summary>
        /// <param name="reader"></param>
        public new void Fill(SqlDataReader reader)
        {
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "USRLST_ID"))
                this.UserListID = Convert.ToInt32(reader["USRLST_ID"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "ENV_ID"))
                this.EnvID = Convert.ToInt32(reader["ENV_ID"]);
            
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "ENV_NAME"))
                this.EnvName = Convert.ToString(reader["ENV_NAME"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "USRLST_TYPE"))
            {
                this.UserListType = Convert.ToString(reader["USRLST_TYPE"]);
            }

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "USRLST_EMAIL_ADDRESS"))
            {
                this.EmailAddress = Convert.ToString(reader["USRLST_EMAIL_ADDRESS"]);
            }

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "USRLST_MESSAGETYPE"))
            {
                MessageType = Convert.ToString(reader["USRLST_MESSAGETYPE"]);
            }

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "USRLST_IS_ACTIVE"))
            {
                this.IsAvtive = Convert.ToBoolean(reader["USRLST_IS_ACTIVE"]);
            }

        }
        #endregion Fill user email configuration details
    }
}
