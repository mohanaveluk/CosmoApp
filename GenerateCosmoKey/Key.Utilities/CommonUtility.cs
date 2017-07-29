using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using Key.Utilities;


namespace GenerateCosmoKey
{
    public static class CommonUtility
    {
        #region constants
        
        private const char PROJECT_FIELD_DELIMITER = ',';
        private const string EncryptionKey = "c0$moApp|ica%io#";

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

            string str;
            while (reader.Peek() > 0)
            {
                str = reader.ReadLine();
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

        public static string GetInternalKey()
        {
            var key = GetSerialNumberByProcess();
            if (!string.IsNullOrEmpty(key))
            {
                return Encrypt(key);
            }
            return string.Empty;
        }
    }
}

namespace Key.Utilities
{
    public class NamedBusinessCode
    {
        public int ID { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
    }
}
