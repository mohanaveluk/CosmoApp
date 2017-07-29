using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Cog.CSM.Entity
{
    public class Portal:TRW.NamedBusinessCode, IFill
    {
        public int Id { get; set; }
        public int EnvId { get; set; }
        public string EnvName { get; set; }
        public string Type { get; set; }
        public string Adress { get; set; }
        public string DisplayName { get; set; }
        public string MatchContent { get; set; }
        public int Interval { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool IsActive { get; set; }
        public bool Status { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedDateTime { get; set; }
        public string UpdatedBy { get; set; }
        public string Comments { get; set; }
        public DateTime? LastJobRunTime { get; set; }
        public DateTime? NextJobRunTime { get; set; }

        public void Fill(SqlDataReader reader)
        {
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "URL_ID"))
                this.Id = Convert.ToInt32(reader["URL_ID"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "ENV_ID"))
                this.EnvId = Convert.ToInt32(reader["ENV_ID"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "ENV_NAME"))
                EnvName = Convert.ToString(reader["ENV_NAME"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "URL_TYPE"))
                this.Type = Convert.ToString(reader["URL_TYPE"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "URL_ADDRESS"))
                this.Adress = Convert.ToString(reader["URL_ADDRESS"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "URL_DISPLAYNAME"))
                DisplayName = Convert.ToString(reader["URL_DISPLAYNAME"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "URL_MATCHCONTENT"))
                MatchContent = Convert.ToString(reader["URL_MATCHCONTENT"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "URL_INTERVAL"))
                Interval = Convert.ToInt32(reader["URL_INTERVAL"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "URL_USERNAME"))
                UserName = Convert.ToString(reader["URL_USERNAME"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "URL_PASSWORD"))
                Password = Convert.ToString(reader["URL_PASSWORD"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "URL_ISACTIVE"))
                IsActive = Convert.ToBoolean(reader["URL_ISACTIVE"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "URL_STATUS"))
                Status = Convert.ToBoolean(reader["URL_STATUS"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "URL_CREATEDBY"))
                CreatedBy = Convert.ToString(reader["URL_CREATEDBY"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "URL_CREATEDDATE"))
                CreatedDateTime = Convert.ToDateTime(reader["URL_CREATEDDATE"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "URL_UPDATEDBY"))
                UpdatedBy = Convert.ToString(reader["URL_UPDATEDBY"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "URL_UPDATEDDATE"))
                UpdatedDateTime = Convert.ToDateTime(reader["URL_UPDATEDDATE"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "URL_COMMENTS"))
                Comments = Convert.ToString(reader["URL_COMMENTS"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "URL_CREATEDDATE"))
                CreatedDateTime = Convert.ToDateTime(reader["URL_CREATEDDATE"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "URL_CREATEDDATE"))
                CreatedDateTime = Convert.ToDateTime(reader["URL_CREATEDDATE"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "URL_LASTJOBRUNTIME"))
                LastJobRunTime = Convert.ToDateTime(reader["URL_LASTJOBRUNTIME"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "URL_NEXTJOBRUNTIME"))
                NextJobRunTime = Convert.ToDateTime(reader["URL_NEXTJOBRUNTIME"]);

        }
    }
}
