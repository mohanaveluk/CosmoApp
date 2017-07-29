using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using Oracle.ManagedDataAccess.Client;

namespace Cog.WS.Entity
{
    public class CommonUtility
    {
        // <summary> 
        /// Method to check whether column exists or not 
        /// </summary> 
        /// <param name="reader">SqlDataReader</param> 
        /// <param name="columnName">columnName</param> 
        /// <returns>true if the columName is exists and false if not</returns> 
        public static bool IsColumnExistsAndNotNull(SqlDataReader reader, string columnName)
        {
            for (int count = 0; count < reader.FieldCount; count++)
            {
                if (reader.GetName(count).Trim().ToUpper() == columnName.Trim().ToUpper() && reader[count] != DBNull.Value)

                    return true;
            }

            return false;
        }

        public static bool IsColumnExistsAndNotNull(OracleDataReader reader, string columnName)
        {
            for (int count = 0; count < reader.FieldCount; count++)
            {
                if (reader.GetName(count).Trim().ToUpper() == columnName.Trim().ToUpper() && reader[count] != DBNull.Value)

                    return true;
            }

            return false;
        }

        /// <summary>
        /// Encrypt password
        /// </summary>
        /// <param name="strEncrypted"></param>
        /// <returns></returns>
        public static string EnryptString(string strEncrypted)
        {
            byte[] b = System.Text.ASCIIEncoding.ASCII.GetBytes(strEncrypted);
            string encryptedPassword = Convert.ToBase64String(b);
            return encryptedPassword;
        }

        /// <summary>
        /// Decrypt password
        /// </summary>
        /// <param name="encrString"></param>
        /// <returns></returns>
        public static string DecryptString(string encrString)
        {
            byte[] b = Convert.FromBase64String(encrString);
            string decryptedPassword = System.Text.ASCIIEncoding.ASCII.GetString(b);
            return decryptedPassword;
        }

        /// <summary>
        /// Get Decrypted connection string 
        /// </summary>
        /// <param name="encryptedConnection"></param>
        /// <returns></returns>
        public static string GetParsedConnectionString(string encryptedConnection)
        {
            string connectionString = string.Empty;
            string userid = null;
            string password = null;
            string dbServer = null;
            string dbName = null;

            if (!string.IsNullOrEmpty(encryptedConnection))
            {
                if (encryptedConnection.ToLower().Contains("user id"))
                {
                    var connectionParameters = encryptedConnection.Split(';');
                    if (connectionParameters.Length <= 0) return connectionString;
                    foreach (var t in connectionParameters)
                    {
                        if (t.ToLower().Contains("user id") || t.ToLower().Contains("userid"))
                        {
                            userid = DecryptString(t.Substring(t.IndexOf("=", StringComparison.Ordinal) + 1));
                        }
                        else if (t.ToLower().Contains("password"))
                        {
                            password = DecryptString(t.Substring(t.IndexOf("=", StringComparison.Ordinal) + 1));
                        }
                        else if (t.ToLower().Contains("data source"))
                        {
                            dbServer = t.Substring(t.IndexOf("=", StringComparison.Ordinal) + 1);
                        }
                        else if (t.ToLower().Contains("catalog"))
                        {
                            dbName = t.Substring(t.IndexOf("=", StringComparison.Ordinal) + 1);
                        }
                    }
                    //connectionString = string.Format("Data Source={0};User ID={1};password={2};Initial Catalog={3};", dbServer, userid, password, dbName);
                    connectionString = string.IsNullOrEmpty(dbName)
                        ? string.Format("Data Source={0};User ID={1};password={2};Connection Timeout=60", dbServer, userid, password)
                        : string.Format("Data Source={0};User ID={1};password={2};Initial Catalog={3};", dbServer,
                            userid, password, dbName);
                }
                else
                {
                    connectionString = encryptedConnection;
                }

            }

            return connectionString;
        }

    }
}
