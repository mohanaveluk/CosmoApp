using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using Cog.CSM.Entity;

namespace Cog.CSM.Data
{
    public class LogDL
    {
        private const string INSERTLOG = "CWT_InsertCSMLog";

        #region Insert Logging of job status

        /// <summary>
        /// Inser log for all the job run status
        /// </summary>
        /// <param name="logData"></param>
        public void InsertLog(LogEntity logData)
        {
            List<SqlParameter> pList = new List<SqlParameter>();
            pList.Add(new SqlParameter("@LOGID", logData.LOGID));
            pList.Add(new SqlParameter("@SCH_ID", logData.SCH_ID));
            pList.Add(new SqlParameter("@CONFIG_ID", logData.CONFIG_ID));
            pList.Add(new SqlParameter("@ENV_ID", logData.ENV_ID));
            pList.Add(new SqlParameter("@LOGDESCRIPTION", logData.LogDescription));
            pList.Add(new SqlParameter("@LOGERROR", logData.LogError));
            //pList.Add(new SqlParameter("@LOG_UPDATED_DATETIME", logData.UpdatedDateTime));
            pList.Add(new SqlParameter("@LOG_UPDATED_BY", logData.UpdatedBy));
            UtilityDL.ExecuteNonQuery(INSERTLOG, pList);
        }

        #endregion Insert Logging of job status
    }
}
