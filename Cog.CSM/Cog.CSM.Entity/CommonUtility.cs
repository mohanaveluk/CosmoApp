using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Xml;
using Oracle.ManagedDataAccess.Client;

namespace Cog.CSM.Entity
{
    public class CommonUtility
    {
        #region constants

        private const string EncryptionKey = "c0$moApp|ica%io#";

        #endregion constants

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

        public static string Encrypt(string encryptText)
        {

            byte[] clearBytes = Encoding.Unicode.GetBytes(encryptText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    encryptText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return encryptText;
        }

        public static string Decrypt(string cipherText)
        {
            cipherText = cipherText.Replace(" ", "+");
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
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
                    connectionString = string.IsNullOrEmpty(dbName)
                        ? string.Format("Data Source={0};User ID={1};password={2};Connection Timeout=60", dbServer, userid, password)
                        : string.Format("Data Source={0};User ID={1};password={2};Initial Catalog={3};", dbServer,
                            userid, password, dbName);
                    //connectionString = string.Format("Data Source={0};User ID={1};password={2};Initial Catalog={3};", dbServer, userid, password, dbName);
                }
                else
                {
                    connectionString = encryptedConnection;
                }

            }

            return connectionString;
        }

        public static CognosCgiResponse LogWebsiteWithCredential(string url, string match, string username, string password)
        {
            var site = url; //@"http://snowflake/ibmcognos/cgi-bin/cognos.cgi";
            var result = new CognosCgiResponse();
            try
            {
                var stopwatch = new Stopwatch();
                stopwatch.Start();

                var passwordDecrypt = Decrypt(password);

                var client = new CookieAwareWebClient { BaseAddress = url };
                var loginPage = client.DownloadString(site);
                if (!loginPage.Contains("Log on")) throw new CognosException(loginPage);
                var loginData = new NameValueCollection { { "CAMUsername", username }, { "CAMPassword", passwordDecrypt } };
                client.UploadValues("", "POST", loginData);

                var source =
                    client.DownloadString(
                        url);//+ "?b_action=xts.run&m=portal/cc.xts&gohome=");
                var success = source.ToLower().Contains("log on");

                if (success) throw new CognosException("login failure");

                source =
                    client.DownloadString(
                        url + "?b_action=xts.run&m=portal/cc.xts&gohome=");
                success = source.ToLower().Contains(match.ToLower());

                stopwatch.Stop();
                var responseTime = stopwatch.ElapsedMilliseconds ;//< 1000 ? stopwatch.ElapsedMilliseconds + " ms" : (stopwatch.ElapsedMilliseconds / 1000).ToString("##,##") + " sec";

                if (success)
                    result = new CognosCgiResponse
                    {
                        Status = "Success",
                        Message = string.Empty,
                        ResponseTime = responseTime
                    };
                else
                    result = new CognosCgiResponse
                    {
                        Status = "Failure",
                        Message = "Content '" + match + "' does not match",
                        ResponseTime = responseTime
                    };
            }
            catch (CognosException exception)
            {
                if (exception.Message.Contains("dispatcher is still initializing"))
                {
                    result = new CognosCgiResponse { Status = "Failure", Message = "The dispatcher is still initializing", Exception = exception.Message };
                }
                else if (exception.Message.Contains("error has occurred"))
                {
                    result = new CognosCgiResponse { Status = "Failure", Message = "An error has occurred", Exception = exception.Message };
                }
                else if (exception.Message.Contains("unable to connect") || exception.Message.Contains("Bad Gateway"))
                {
                    result = new CognosCgiResponse { Status = "Failure", Message = "Unable to connect to the server", Exception = exception.Message };
                }
                else if (exception.Message.Contains("Bad Gateway"))
                {
                    result = new CognosCgiResponse { Status = "Failure", Message = "Bad Gateway", Exception = exception.Message };
                }
                else if (exception.Message.Contains("login failure"))
                {
                    result = new CognosCgiResponse { Status = "Failure", Message = "Unable to log on the user because of invalid credential", Exception = exception.Message };
                }
            }
            catch (Exception exception)
            {
                result = new CognosCgiResponse { Status = "Failure", Message = exception.Message, Exception = exception.Message };
            }

            return result;
        }

        public static MonitorAlertConfig GetMonitorAlert(string name)
        {
            var xmlFilePath = @AppDomain.CurrentDomain.BaseDirectory + "\\alertconfiguration.xml";

            var xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlFilePath);

            if (xmlDoc.DocumentElement == null) return new MonitorAlertConfig();

            var nodeList = xmlDoc.DocumentElement.SelectNodes("/Monitor/UrlPerformance/config");

            if (nodeList == null || nodeList.Count < 0) return new MonitorAlertConfig();

            var xmlAttributeCollection = nodeList [0].Attributes;
            if (xmlAttributeCollection != null)
            {
                return new MonitorAlertConfig
                {
                    Name = xmlAttributeCollection["name"].InnerText,
                    Unit = xmlAttributeCollection["unit"].InnerText,
                    Value = xmlAttributeCollection["value"].InnerText
                };
            }

            return new MonitorAlertConfig();
        }

        #region Convert ms To seconds
        public static double ConvertMillisecondsToSeconds(double milliseconds)
        {
            return TimeSpan.FromMilliseconds(milliseconds).TotalSeconds;
        }
        #endregion Convert ms To seconds
    }

    public class CognosCgiResponse
    {
        public int UrlId { get; set; }
        public int EnvId { get; set; }
        public string EnvName { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
        public long ResponseTime { get; set; }
        public string Exception { get; set; }
        public string Address { get; set; }
        public string ResponseTimeInSec { get; set; }
        public string ThresholdTime { get; set; }
        public string ThresholdUnit { get; set; }
    }

    public class MonitorAlertConfig
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public string Unit { get; set; }
    }

    public class CognosException : Exception
    {
        public CognosException()
        {

        }
        public CognosException(string message)
            : base(message)
        {
        }
    }

    public class CookieAwareWebClient : WebClient
    {
        private readonly CookieContainer _cookie = new CookieContainer();

        protected override WebRequest GetWebRequest(Uri address)
        {
            var request = base.GetWebRequest(address);
            if (request is HttpWebRequest)
            {
                (request as HttpWebRequest).CookieContainer = _cookie;
            }
            return request;
        }
    }
}
