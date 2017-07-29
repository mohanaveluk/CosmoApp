using System;
using System.Data.SqlClient;

namespace Cog.WS.Entity
{
    public class ServerSchedule: TRW.NamedBusinessCode, IFill
    {
        public int Id { get; set; }
        public int EnvId { get; set; }
        public string EnvName { get; set; }
        public int ConfigId { get; set; }
        public string HostIp { get; set; }
        public string Port { get; set; }
        public DateTime LastJobRunTime { get; set; }
        public DateTime NextJobRunTime { get; set; }


        public void Fill(SqlDataReader reader)
        {
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "SVR_ID"))
                this.Id = Convert.ToInt32(reader["SVR_ID"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "ENVID"))
                this.EnvId = Convert.ToInt32(reader["ENVID"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIGID"))
                this.ConfigId = Convert.ToInt32(reader["CONFIGID"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "ENV_NAME"))
                EnvName = Convert.ToString(reader["ENV_NAME"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_HOST_IP"))
                HostIp = Convert.ToString(reader["CONFIG_HOST_IP"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIG_PORT_NUMBER"))
                Port = Convert.ToString(reader["CONFIG_PORT_NUMBER"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIGID"))
                this.ConfigId = Convert.ToInt32(reader["CONFIGID"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "SVR_LASTJOBRAN_TIME"))
                LastJobRunTime = Convert.ToDateTime(reader["SVR_LASTJOBRAN_TIME"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "SVR_NEXTJOBRAN_TIME"))
                NextJobRunTime = Convert.ToDateTime(reader["SVR_NEXTJOBRAN_TIME"]);

        }
    }
}