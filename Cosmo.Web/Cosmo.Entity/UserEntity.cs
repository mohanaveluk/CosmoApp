using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;


namespace Cosmo.Entity
{
    public class UserEntity:TRW.NamedBusinessCode, IFill
    {
        #region property declaration

        public int UserID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Roles { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public List<UserRoleEntity> UserRoles { get; set; }

        #endregion property declaration

        #region IFill Logging of job details

        public void Fill(SqlDataReader reader)
        {
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "USER_ID"))
                this.UserID = Convert.ToInt32(reader["USER_ID"]);
            else
                this.UserID = 0;
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "USER_FIRST_NAME"))
                this.FirstName = Convert.ToString(reader["USER_FIRST_NAME"]);
            else
                this.FirstName = string.Empty;

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "USER_LAST_NAME"))
                this.LastName = Convert.ToString(reader["USER_LAST_NAME"]);
            else
                this.LastName = string.Empty;

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "USER_EMAIL_ADDRESS"))
                this.Email = Convert.ToString(reader["USER_EMAIL_ADDRESS"]);
            else
                this.Email = string.Empty;

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "USER_PASSWORD"))
                this.Password = Convert.ToString(reader["USER_PASSWORD"]);
            else
                this.Password = string.Empty;

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "USER_IS_ACTIVE"))
                this.IsActive = Convert.ToBoolean(reader["USER_IS_ACTIVE"]);
            else
                this.IsActive = false;

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "USER_IS_DELETED"))
                this.IsDeleted = Convert.ToBoolean(reader["USER_IS_DELETED"]);
            else
                this.IsDeleted = false;

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "USER_CREATED_DATE"))
                this.CreatedDate = Convert.ToDateTime(reader["USER_CREATED_DATE"]);
            else
                this.CreatedDate = null;

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "USER_UPDATED_DATE"))
                this.UpdatedDate = Convert.ToDateTime(reader["USER_UPDATED_DATE"]);
            else
                this.UpdatedDate = null;

        }

        #endregion IFill Logging of job details
    }
}
