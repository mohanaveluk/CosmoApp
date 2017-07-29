using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace Cosmo.Entity
{
    public class GroupEntity:TRW.NamedBusinessCode, IFill
    {
        #region property
        public int Group_ID { get; set; }
        public string Group_Name { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string Comments { get; set; }
        public bool IsActive { get; set; }
        
        #endregion property


        #region IFill Environment details
        /// <summary>
        /// Fill Data for Environments
        /// </summary>
        /// <param name="reader"></param>
        public new void Fill(SqlDataReader reader)
        {
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "GROUP_ID"))
                this.Group_ID = Convert.ToInt32(reader["GROUP_ID"]);
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "GROUP_NAME"))
                this.Group_Name = Convert.ToString(reader["GROUP_NAME"]);
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "USERFIRSTNAME"))
                this.CreatedBy = Convert.ToString(reader["USERFIRSTNAME"]);
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "GROUP_CREATED_DATE"))
                this.CreatedDate = Convert.ToDateTime(reader["GROUP_CREATED_DATE"]);
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "USERFIRSTNAME"))
                this.UpdatedBy = Convert.ToString(reader["USERFIRSTNAME"]);
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "GROUP_UPDATED_DATE"))
                this.UpdatedDate = Convert.ToDateTime(reader["GROUP_UPDATED_DATE"]);
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "GROUP_COMMENTS"))
                this.Comments = Convert.ToString(reader["GROUP_COMMENTS"]);
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "GROUP_IS_ACTIVE"))
                this.IsActive = Convert.ToBoolean(reader["GROUP_IS_ACTIVE"]);
        }

        #endregion IFill Environment details
    }
}
