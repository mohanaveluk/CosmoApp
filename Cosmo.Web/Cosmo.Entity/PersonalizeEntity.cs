using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cosmo.Entity
{
    public class PersonalizeEntity :TRW.NamedBusinessCode, IFill
    {

        #region property
        
        public int ID { get; set; }
        public int UserId { get; set; }
        public int RefreshTime { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public string SortOrder { get; set; }

        #endregion property

        #region IFill Environment details
        public void Fill(System.Data.SqlClient.SqlDataReader reader)
        {
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "PERS_ID"))
                this.ID = Convert.ToInt32(reader["PERS_ID"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "User_ID"))
                this.UserId = Convert.ToInt32(reader["PERS_ID"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "PERS_DB_REFRESHTIME"))
                this.RefreshTime = Convert.ToInt32(reader["PERS_DB_REFRESHTIME"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "PERS_ISACTIVE"))
                this.IsActive = Convert.ToBoolean(reader["PERS_ISACTIVE"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "PERS_CREATEDDATE"))
                this.CreatedDate = Convert.ToDateTime(reader["PERS_CREATEDDATE"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "PERS_CREATEDBY"))
                this.CreatedBy = Convert.ToString(reader["PERS_CREATEDBY"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "PERS_UPDATEDDATE"))
                this.UpdatedDate = Convert.ToDateTime(reader["PERS_UPDATEDDATE"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "PERS_UPDATEDBY"))
                this.UpdatedBy = Convert.ToString(reader["PERS_UPDATEDBY"]);

        
        }
        #endregion IFill Environment details
    }
}
