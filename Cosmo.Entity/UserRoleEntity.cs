using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cosmo.Entity
{
    public class UserRoleEntity : TRW.NamedBusinessCode, IFill
    {
        #region property declaration
        public int UserRoleID { get; set; }
        public int UserID { get; set; }
        public int RoleID { get; set; }
        public string RoleName { get; set; }

        #endregion property declaration

        #region IFill Logging of job details

        public void Fill(System.Data.SqlClient.SqlDataReader reader)
        {
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "USER_ID"))
                this.UserID = Convert.ToInt32(reader["USER_ID"]);
            else
                this.UserID = 0;

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "USER_ROLE_ID"))
                this.UserRoleID = Convert.ToInt32(reader["USER_ROLE_ID"]);
            else
                this.UserRoleID = 0;
            
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "ROLE_ID"))
                this.RoleID = Convert.ToInt32(reader["ROLE_ID"]);
            else
                this.RoleID = 0;
            
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "ROLE_NAME"))
                this.RoleName = Convert.ToString(reader["ROLE_NAME"]);
            else
                this.RoleName = string.Empty;

        }
        
        #endregion IFill Logging of job details
    }
}
