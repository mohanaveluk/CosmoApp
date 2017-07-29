using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace Cog.WS.Entity
{
    public class MailLogEntity: IFill
    {
        #region property declaration
        public int ENV_ID { get; set; }
        public int Config_ID { get; set; }
        public string To_Address { get; set; }
        public string Cc_Address{ get; set; }
        public string Bcc_Address { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string Status { get; set; }
        public string Error { get; set; }
        public string ContentType { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedDate { get; set; }
        public string Comments { get; set; }
        #endregion property declaration

        #region IFill Logging of job details
        public void Fill(SqlDataReader reader)
        {
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "ToAddress"))
                this.To_Address = Convert.ToString(reader["ToAddress"]);

        }
        #endregion IFill Logging of job details
    }
}
