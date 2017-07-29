using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace Cog.WS.Entity
{
    public class UserEmailEntity: TRW.NamedBusinessCode, IFill
    {
        #region property declaration
        
        public int Env_ID { get; set; }
        public string EmailType { get; set; }
        public string EmailAddress { get; set; }
        public bool IsActive { get; set; }
        public string EmailComments { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        
        #endregion property declaration

         #region IFill Logging of job details
        public void Fill(SqlDataReader reader)
        {
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "ENV_ID"))
                this.Env_ID = Convert.ToInt32(reader["ENV_ID"]);
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "USRLST_TYPE"))
                this.EmailType = Convert.ToString(reader["USRLST_TYPE"]);
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "USRLST_EMAIL_ADDRESS"))
                this.EmailAddress = Convert.ToString(reader["USRLST_EMAIL_ADDRESS"]);
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "USRLST_IS_ACTIVE"))
                this.IsActive = Convert.ToBoolean(reader["USRLST_IS_ACTIVE"]);
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "USRLST_COMMENTS"))
                this.EmailComments = Convert.ToString(reader["USRLST_COMMENTS"]);
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "USRLST_CREATED_BY"))
                this.UpdatedBy = Convert.ToString(reader["USRLST_CREATED_BY"]);
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "USRLST_CREATED_DATE"))
                this.UpdatedDate = Convert.ToDateTime(reader["USRLST_CREATED_DATE"]);
        }
        #endregion IFill Logging of job details
    }
}
