using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography;
using Microsoft.Win32;
using Oracle.ManagedDataAccess.Client;
using WatiN.Core;
using WatiN.Core.Native.Windows;


namespace Cosmo.Entity
{

    public static class CommonUtility
    {
        #region constants

        private const char PROJECT_FIELD_DELIMITER = ',';
        private const string EncryptionKey = "c0$moApp|ica%io#";
        private const string EncryptionKeyMobileApp = "C0530!$@te@3C1uTcH$!@2o!6";
        private const string Subkey = "Svecos";
        private const string SubkeyVersion = "Version01";

        #endregion constants

        /// <summary>
        /// Setting frequency values
        /// </summary>
        public enum Frequency
        {
            Seconds = 1,
            Minutes,
            Hours,
            Daily,
            Weekly,
            Monthly
        }

        public enum WinServiceAction
        {
            Start = 1,
            Stop,
            Restart
        }
        public enum WinServiceStatus
        {
            Scheduled=0,
            Completed,
            Failed,
            Pending
        }

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

        #region GetListFromCommaSeperatedString
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="commmaSeperatedCodes"></param>
        /// <returns></returns>
        public static IList<NamedBusinessCode> CodesToList(string commmaSeperatedCodes)
        {
            IList<NamedBusinessCode> nbcList = new List<NamedBusinessCode>();
            string[] str = commmaSeperatedCodes.Split(PROJECT_FIELD_DELIMITER);
            foreach (string s in str)
            {
                NamedBusinessCode i = new NamedBusinessCode();
                i.Code = s;
                i.Name = string.Empty;
                nbcList.Add(i);
            }
            return nbcList;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commmaSeperatedCodes"></param>
        /// <returns></returns>
        public static IList<NamedBusinessCode> CodesToList(string commmaSeperatedCodes, bool integer)
        {
            IList<NamedBusinessCode> nbcList = new List<NamedBusinessCode>();
            string[] str = commmaSeperatedCodes.Split(PROJECT_FIELD_DELIMITER);
            foreach (string s in str)
            {
                NamedBusinessCode i = new NamedBusinessCode();
                if (integer)
                {
                    if (string.IsNullOrEmpty(s))
                        i.ID = -1;
                    else
                        i.ID = Convert.ToInt32(s);
                }
                else
                    i.Code = s;

                i.Name = string.Empty;
                nbcList.Add(i);
            }
            return nbcList;
        }

        #endregion GetListFromCommaSeperatedString

        #region convert string to Title Case

        /// <summary>
        /// Use the current thread's culture info for conversion
        /// </summary>
        public static string ToTitleCase(this string str)
        {
            var cultureInfo = System.Threading.Thread.CurrentThread.CurrentCulture;
            return cultureInfo.TextInfo.ToTitleCase(str.ToLower());
        }
        #endregion convert string to Title Case

        /// <summary>
        /// conver array to string
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        static string ConvertStringArrayToString(string[] array)
        {
            //
            // Concatenate all the elements into a StringBuilder.
            //
            StringBuilder builder = new StringBuilder();
            foreach (string value in array)
            {
                builder.Append(value);
                builder.Append('.');
            }
            return builder.ToString();
        }

        /// <summary>
        /// Convert list of string to single string using the given delimiter
        /// </summary>
        /// <param name="list"></param>
        /// <param name="delimiter"></param>
        /// <returns></returns>
        public static string ConvertListToString(List<string> list, string delimiter)
        {
            StringBuilder builder = new StringBuilder();
            foreach (string lst in list)
            {
                if (!string.IsNullOrEmpty(builder.ToString()))
                    builder.Append(delimiter);
                builder.Append(lst);
            }
            return builder.ToString();
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

        public static string GetSerialNumberByProcess()
        {
            string serialnumber = null;
            Process process = new Process
            {
                StartInfo = new ProcessStartInfo("cmd", "/c wmic bios get serialnumber")
                {
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };
            // @"D:\Projects\.Net Questions\Samples\ServerSerialNumber\ServerSerialNumber\serial.cmd";
            process.Start();

            // Synchronously read the standard output of the spawned process. 
            StreamReader reader = process.StandardOutput;
            //string output = reader.ReadToEnd();

            process.WaitForExit();
            process.Close();

            string str = null;
            while (reader.Peek() > 0)
            {
                var readLine = reader.ReadLine();
                if (readLine != null) str = readLine.Trim();
                if (!string.IsNullOrEmpty(str) && !str.ToLower().Contains("serialnumber"))
                    serialnumber = str;
            }

            reader.Close();

            return serialnumber;
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

        public static string Encrypt_ActivationKey(string encryptText)
        {

            byte[] clearBytes = Encoding.Unicode.GetBytes(encryptText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x6e, 0x20, 0x76, 0x49, 0x61, 0x64, 0x65, 0x20, });
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

        public static string Decrypt_ActivationKey(string cipherText)
        {

            cipherText = cipherText.Replace(" ", "+");
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x6e, 0x20, 0x76, 0x49, 0x61, 0x64, 0x65, 0x20, });
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

        public static byte[] EncKeyGeneration()
        {
            byte[] salt = new byte[] { 0x6e, 0x20, 0x76, 0x49, 0x61, 0x64, 0x65, 0x20, };
            int iterations = 1024;
            var rfc2898 =
            new System.Security.Cryptography.Rfc2898DeriveBytes(EncryptionKeyMobileApp, salt, iterations);
            byte[] key = rfc2898.GetBytes(16);
            String keyB64 = Convert.ToBase64String(key);
            //System.Console.WriteLine("Key: " + keyB64);
            return key;
        }

        public static EncryptionKeys Encryption_MobileApp(string encryptSting)
        {
            byte[] key = EncKeyGeneration();
            AesManaged aesCipher = new AesManaged();
            aesCipher.KeySize = 128;
            aesCipher.BlockSize = 128;
            aesCipher.Mode = CipherMode.CBC;
            aesCipher.Padding = PaddingMode.PKCS7;
            aesCipher.Key = key;
            byte[] b = System.Text.Encoding.UTF8.GetBytes(encryptSting);
            ICryptoTransform encryptTransform = aesCipher.CreateEncryptor();
            byte[] ctext = encryptTransform.TransformFinalBlock(b, 0, b.Length);
            var iv = Convert.ToBase64String(aesCipher.IV);
            var cipherText = Convert.ToBase64String(ctext);
            //return new EncryptionKeys { IvText = aesCipher.IV, CiperText = ctext };
            return new EncryptionKeys { IvText = iv, CiperText = cipherText };
            
        }

        public static string Decryption_MobileApp(EncryptionKeys keys)
        {
            byte[] key = EncKeyGeneration();
            var ivString = Convert.FromBase64String(keys.IvText);
            var cipString = Convert.FromBase64String(keys.CiperText);


            var aesCipher = new AesManaged
            {
                KeySize = 128,
                BlockSize = 128,
                Mode = CipherMode.CBC,
                Padding = PaddingMode.PKCS7,
                Key = key,
                IV = ivString
            };

            var decryptTransform = aesCipher.CreateDecryptor();
            byte[] plainText = decryptTransform.TransformFinalBlock(cipString, 0, cipString.Length);

            var dc = System.Text.Encoding.UTF8.GetString(plainText);
            return dc;
        }

        public static string EncryptTripleDes(string toEncrypt)
        {
            byte[] saltTripleDesKey =
            {
                0x01, 0x02, 0x03, 0x05, 0x07, 0x0B, 0x0D, 0x11,
                0x12, 0x11, 0x0D, 0x0B, 0x07, 0x02, 0x04, 0x08,
                0x01, 0x02, 0x03, 0x05, 0x07, 0x0B, 0x0D, 0x11
            };
            
            var des = new TripleDESCryptoServiceProvider
            {
                Mode = System.Security.Cryptography.CipherMode.ECB,
                Key = saltTripleDesKey,
                Padding = PaddingMode.PKCS7
            };

            var desEncrypt = des.CreateEncryptor();

            var buffer = System.Text.ASCIIEncoding.ASCII.GetBytes(toEncrypt);

            return Convert.ToBase64String(desEncrypt.TransformFinalBlock(buffer, 0, buffer.Length));
        }

        public static string DecryptTripleDes(string toDecrypt)
        {
            byte[] saltTripleDesKey =
            {
                0x01, 0x02, 0x03, 0x05, 0x07, 0x0B, 0x0D, 0x11,
                0x12, 0x11, 0x0D, 0x0B, 0x07, 0x02, 0x04, 0x08,
                0x01, 0x02, 0x03, 0x05, 0x07, 0x0B, 0x0D, 0x11
            };

            var des = new TripleDESCryptoServiceProvider
            {
                Mode = System.Security.Cryptography.CipherMode.ECB,
                Key = saltTripleDesKey,
                Padding = PaddingMode.PKCS7
            };

            var desEncrypt = des.CreateDecryptor();

            var buffer = Convert.FromBase64String(toDecrypt);

            return System.Text.Encoding.ASCII.GetString(desEncrypt.TransformFinalBlock(buffer, 0, buffer.Length));
        }

        public static string GetInternalKey()
        {
            var key = GetSerialNumberByProcess();
            if (!string.IsNullOrEmpty(key))
            {
                return Encrypt(key);
            }
            return string.Empty;
        }

        public static string GetInternalKeyWithPackaType(string packageType)
        {
            var key = GetSerialNumberByProcess();
            if (!string.IsNullOrEmpty(key))
            {
                return Encrypt(packageType + "@@" + key);
            }
            return string.Empty;
        }


        //Approach 2
        public static string InitialAccessKey()
        {
            var defaultKey = DateTime.Now.AddDays(30).ToShortDateString() + "&001" + "&M";
            
            var key = GetSerialNumberByProcess();

            if (!string.IsNullOrEmpty(key))
            {
                defaultKey = key + "&" + defaultKey;
            }

            return Encrypt_ActivationKey(defaultKey);
        }

        public static void SetRegistry(string keyName, string keyValue)
        {
            RegistryKey softwareKey = Registry.LocalMachine.OpenSubKey("SOFTWARE", true);

            if (softwareKey != null)
            {
                RegistryKey appNameKey = softwareKey.CreateSubKey(Subkey);
                if (appNameKey != null)
                {
                    RegistryKey appVersionKey = appNameKey.CreateSubKey(SubkeyVersion);

                    if (appVersionKey != null) appVersionKey.SetValue(keyName, keyValue);
                }
            }
        }

        public static bool IsKeyDecryptable(ActivationEntity activation)
        {
            try
            {
                var deCrypt = Decrypt_ActivationKey(activation.Key);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
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
                            userid = DecryptString(t.Substring(t.IndexOf("=", StringComparison.Ordinal)+1));
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
                }
                else
                {
                    connectionString = encryptedConnection;
                }

            }

            return connectionString;
        }

        public static string GetWebServiceConfigFilePath(string xmlFilePath)
        {
            const string mobileServiceDir = "Cosmo.WebServices";
            var parent = !Directory.Exists(Path.Combine(xmlFilePath, mobileServiceDir)) ? Directory.GetParent(Directory.GetParent(xmlFilePath).ToString()).FullName : xmlFilePath;

            return  Path.Combine(parent, mobileServiceDir, "Web.config");
        }

        public static string GetWebServicePath(string xmlFilePath)
        {
            const string mobileServiceDir = "Cosmo.WebServices";
            var parent = !Directory.Exists(Path.Combine(xmlFilePath, mobileServiceDir)) ? Directory.GetParent(Directory.GetParent(xmlFilePath).ToString()).FullName : xmlFilePath;

            return Path.Combine(parent, mobileServiceDir);
        }

        
        public static CognosCgiResponse LogWebsiteWithCredential(string url, string match, string username, string password)
        {
            var site = url;// @"http://snowflake/ibmcognos/cgi-bin/cognos.cgi";
            var result = new CognosCgiResponse();
            try
            {
                var stopwatch = new Stopwatch();
                stopwatch.Start();

                //var passwordDecrypt = Decrypt(password);

                var client = new CookieAwareWebClient { BaseAddress = url };
                client.Headers["User-Agent"] = "MOZILLA/5.0 (WINDOWS NT 6.1; WOW64) APPLEWEBKIT/537.1 (KHTML, LIKE GECKO) CHROME/21.0.1180.75 SAFARI/537.1";

                //client.Headers.Add("User-Agent",
                    //@"Mozilla/5.0 (Windows NT 6.2; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/32.0.1667.0 Safari/537.36");
                    //"Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.1; Trident/5.0; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR " +
                    //"3.0.30729; Media Center PC 6.0; .NET4.0C; .NET4.0E; InfoPath.2; AskTbFXTV5/5.15.4.23821; BRI/2)");
                client.Headers.Add("Vary", "Accept-Encoding");
                client.Encoding = Encoding.UTF8;

                var loginPage = client.DownloadString(site);
                if (!loginPage.Contains("Log on")) throw new CognosException(loginPage);
                var loginData = new NameValueCollection { { "CAMUsername", username }, { "CAMPassword", password } };
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
                var responseTime = stopwatch.ElapsedMilliseconds < 1000 ? stopwatch.ElapsedMilliseconds + " ms" : (stopwatch.ElapsedMilliseconds / 1000).ToString("##,##") + " sec";

                if (success)
                    result = new CognosCgiResponse
                    {
                        Status = "Success",
                        Message = string.Empty,
                        ResponseTime = responseTime,
                        PortalUrl = url
                    };
                else
                    result = new CognosCgiResponse
                    {
                        Status = "Failure",
                        Message = "Content '" + match + "' does not match",
                        ResponseTime = responseTime,
                        PortalUrl = url
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

        public static List<Drive> GetDriveInfo(string systenName)
        {
            var driveInfo = DriveInfo.GetDrives();

            return driveInfo.Select(info => new Drive
            {
                Name = info.Name,
                Label = info.VolumeLabel,
                Format = info.DriveFormat,
                Type = info.DriveType,
                TotalSpace = info.TotalSize,
                AvailableSpace = info.AvailableFreeSpace,
                FreeSpace = info.TotalFreeSpace,
                UsedSpace = info.TotalSize - info.AvailableFreeSpace
            }).ToList();
        }

        public static string FormatBytes(decimal bytes)
        {
            const int scale = 1024;
            string[] orders = new string[] { "GB", "MB", "KB", "Bytes" };
            long max = (long)Math.Pow(scale, orders.Length - 1);
            foreach (string order in orders)
            {
                if (bytes > max)
                {
                    return string.Format("{0:##.##} {1}", Decimal.Divide(bytes, max), order);
                }

                max /= scale;
            }
            return "0 Bytes";
        }
    }
}
