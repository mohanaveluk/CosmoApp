using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;


namespace Cosmo.Entity
{
    public class EnvWindowsServiceEntity : TRW.NamedBusinessCode, IFill
    {
        public int EnvID { get; set; }
        public string EnvName { get; set; }
        //public string EnvHostIP { get; set; }
        //public string EnvLocation { get; set; }
        public string EnvMailFrequency { get; set; }
        public bool EnvIsMonitor { get; set; }
        public bool EnvIsNotify { get; set; }
        public bool EnvIsServiceConsolidated { get; set; }
        public string EnvCreatedBy { get; set; }
        public DateTime? EnvCreatedDate { get; set; }
        public string EnvUpdatedBy { get; set; }
        public DateTime? EnvUpdatedDate { get; set; }
        public string EnvComments { get; set; }
        public string EnvIsActive { get; set; }
        public List<WinServiceEntity> WinServiceList { get; set; }

        /*public EnvironmentEntity(EnvironmentEntity other)
        {
            EnvName = other.EnvName;
            EnvComments=other.EnvComments;
            EnvCreatedBy = other.EnvCreatedBy;
            EnvCreatedDate=other.EnvCreatedDate;
            EnvDetailsList=other.EnvDetailsList;
            EnvID=other.EnvID;
            EnvIsActive=other.EnvIsActive;
            EnvIsMonitor=other.EnvIsMonitor;
            EnvIsNotify=other.EnvIsNotify;
            EnvIsServiceConsolidated=other.EnvIsServiceConsolidated;
            EnvMailFrequency=other.EnvMailFrequency;
            EnvUpdatedBy = other.EnvUpdatedBy;
            EnvUpdatedDate = other.EnvUpdatedDate;
            

        }*/

        #region IFill Environment details

        /// <summary>
        /// Fill Data for Environments
        /// </summary>
        /// <param name="reader"></param>
        public new void Fill(SqlDataReader reader)
        {
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "ENV_ID"))
                this.EnvID = Convert.ToInt32(reader["ENV_ID"]);
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "ENV_NAME"))
                this.EnvName = Convert.ToString(reader["ENV_NAME"]);
            //if (CommonUtility.IsColumnExistsAndNotNull(reader, "ENV_HOST_IP_ADDRESS"))
            //    this.EnvHostIP = Convert.ToString(reader["ENV_HOST_IP_ADDRESS"]);
            //if (CommonUtility.IsColumnExistsAndNotNull(reader, "ENV_LOCATION"))
            //    this.EnvLocation = Convert.ToString(reader["ENV_LOCATION"]);
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "ENV_MAIL_FREQ"))
                this.EnvMailFrequency = Convert.ToString(reader["ENV_MAIL_FREQ"]);
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "ENV_IS_MONITOR"))
                this.EnvIsMonitor = Convert.ToBoolean(reader["ENV_IS_MONITOR"]);
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "ENV_IS_NOTIFY"))
                this.EnvIsNotify = Convert.ToBoolean(reader["ENV_IS_NOTIFY"]);
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "ENV_IS_CONSLTD_MAIL"))
                this.EnvIsServiceConsolidated = Convert.ToBoolean(reader["ENV_IS_CONSLTD_MAIL"]);
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "ENV_CREATED_BY"))
                this.EnvCreatedBy = Convert.ToString(reader["ENV_CREATED_BY"]);
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "ENV_CREATED_DATE"))
                this.EnvCreatedDate = Convert.ToDateTime(reader["ENV_CREATED_DATE"]);
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "ENV_UPDATED_BY"))
                this.EnvUpdatedBy = Convert.ToString(reader["ENV_UPDATED_BY"]);
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "ENV_UPDATED_DATE"))
                this.EnvUpdatedDate = Convert.ToDateTime(reader["ENV_UPDATED_DATE"]);
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "ENV_COMMENTS"))
                this.EnvComments = Convert.ToString(reader["ENV_COMMENTS"]);
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "ENV_ISACTIVE"))
                this.EnvIsActive = Convert.ToString(reader["ENV_ISACTIVE"]);
            WinServiceList = new List<WinServiceEntity>();


        }

        #endregion


    }
}
