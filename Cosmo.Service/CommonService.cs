using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Web;
using System.Web.UI;
using Cosmo.Data;
using Cosmo.Entity;

namespace Cosmo.Service
{
    public class CommonService
    {
        private static readonly string DbType = ConfigurationManager.AppSettings["DatabaseType"];
        private const string KeyFolderName = "line";
        private const string KeyFileName = "planline.lxd";
        private readonly ICommonData _commonData;

        public CommonService()
        {
            var iDbType = DbType == DatabaseType.Oracle.ToString()
                ? Convert.ToInt32(DatabaseType.Oracle).ToString()
                : Convert.ToInt32(DatabaseType.SqlServer).ToString();

            _commonData = new CommonDataFactory().Create(Convert.ToInt32(iDbType).ToString());
        }

        /// <summary>
        /// Method used to convert the string to numeric
        /// </summary>
        /// <param name="tString"></param>
        /// <returns></returns>
        public static bool IsNumeric(string tString)
        {
            Double num;
            return double.TryParse(tString, out num);
        }

        /// <summary>
        /// Check and Get data from the passed parameter and return either value or empty
        /// </summary>
        /// <param name="tString"></param>
        /// <returns></returns>
        public static string GetText(string tString)
        {
            string newValue = string.Empty;
            if (!string.IsNullOrEmpty(tString))
            {
                newValue = tString;
            }
            return newValue;
        }

        /// <summary>
        /// Check and Get data from the passed parameter and return either value or empty
        /// </summary>
        /// <param name="tString"></param>
        /// <returns></returns>
        public static int GetText(int tValue)
        {
            int newValue = 0;
            if (tValue != null)
            {
                newValue = tValue;
            }
            return newValue;
        }

        public static void ValidateUser()
        {
            //if (HttpContext.Current.Session["LicenseStatus"] != null)
            //{
            //    var licenseStatus = (LicenseStatus)HttpContext.Current.Session["LicenseStatus"];
            //    if (licenseStatus.Type == "Trial" && licenseStatus.Status == "Failure")
            //    {
            //        HttpContext.Current.Response.Redirect("../BuyLicense.aspx");
            //    }
            //}
            if (HttpContext.Current.Session["LOGGEDINUSER"] == null || HttpContext.Current.Session.Count <= 0)
            {
                HttpContext.Current.Response.Redirect("../login/Default.aspx");
                //ScriptManager.RegisterStartupScript(GetText(KeyFileName), this.GetType(), Guid.NewGuid().ToString(), "EnableSchedule(true);", true);

            }
        }

        public static string GetAllMonitors()
        {
            var monitorList = new List<ServiceMoniterEntity>();
            try
            {
                var monitorService = new MonitorService();
                monitorList = monitorService.GetAllMonitors(0, false);
            }
            catch (Exception ex)
            {
                if (ex.Message.ToLower().Contains("ConnectionString property has not been initialized".ToLower()) ||
                    ex.Message.ToLower().Contains("cannot open database"))
                {
                    Logger.Log(ex.ToString());
                    return "../setup/DBConfig.aspx";
                }
                else if (ex.Message.ToLower().Contains("Could not find stored procedure".ToLower()))
                {
                    Logger.Log(ex.ToString());
                    //this.Page.ClientScript.RegisterStartupScript(this.GetType(), "ex", "alert('" + ex.Message + "');", true);
                    return "../error/Oops.aspx?e=1";
                }
            }
            return "Success";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mailServer"></param>
        /// <returns></returns>
        public int InsMailServerConfiguration(MailServerEntity mailServer)
        {
            return _commonData.InsMailServerConfiguration(mailServer);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<MailServerEntity> GetMailServer()
        {
            return _commonData.GetMailServer();
        }

        public int InsActivation(ActivationEntity activation)
        {
            return _commonData.InsActivation(activation);
        }

        public LicenseStatus GetUserAccess()
        {
            try
            {

                var licenseStatus = new LicenseStatus();
                var activationEntity = _commonData.GetUserAccess();
                if (activationEntity == null || activationEntity.Key == null)
                    return new LicenseStatus
                    {
                        Status = "Failure",
                        Type = "Trial",
                        Message = "Oops. License Key has not found in the system"
                    };
                var decryptedActivationKey = CommonUtility.Decrypt_ActivationKey(activationEntity.Key);
                var keyDetails = decryptedActivationKey.Split('&');
                var systemKey = CommonUtility.GetSerialNumberByProcess();
                if (keyDetails.Length > 0)
                {
                    if (keyDetails[0] == systemKey)
                    {
                        if (!string.IsNullOrEmpty(keyDetails[2]))
                        {
                            licenseStatus.PackageMode = !string.IsNullOrEmpty(keyDetails[3])
                                ? keyDetails[3]
                                : string.Empty;

                            if (keyDetails[2] == "4")
                            {
                                licenseStatus.Status = "Success";
                                licenseStatus.Type = "Licensed";
                                licenseStatus.Message = string.Empty;

                                return licenseStatus;
                            }
                        }

                        var currentDate = Convert.ToDateTime(DateTime.Now.ToShortDateString());
                        if (!string.IsNullOrEmpty(keyDetails[1]))
                        {
                            TimeSpan timsSpan = Convert.ToDateTime(keyDetails[1]) - currentDate;
                            licenseStatus.ExpiryInDays = (int) timsSpan.TotalDays;
                            licenseStatus.Type = "Trial";
                            if (Convert.ToDateTime(keyDetails[1]) < currentDate)
                            {
                                licenseStatus.Status = "Failure";
                                licenseStatus.Message = "Trial Version has Expired on " + keyDetails[1] +
                                                        ". Please contact <a href='mailto:hellocosmo@teamclutch.com' target='_blank'>hellocosmo@teamclutch.com</a> to purchase license";
                            }
                            else
                            {
                                licenseStatus.Status = "Success";
                                licenseStatus.Message = licenseStatus.ExpiryInDays > 1
                                    ? "Trial Version and Expires in <font color='#FF1352'>" + licenseStatus.ExpiryInDays +
                                      "</font> days"
                                    : licenseStatus.Message = licenseStatus.ExpiryInDays == 1
                                        ? "Trial Version and Expires Tomorrow"
                                        : licenseStatus.Message = licenseStatus.ExpiryInDays == 0
                                            ? "Trial Version and Expires Today"
                                            : "Trial Version Expired. Please contact us to buy license";
                            }
                        }
                    }
                    else
                    {
                        licenseStatus.Type = "Full";
                        licenseStatus.Status = "Failure";
                        licenseStatus.Message = "Invalid License Key";
                    }
                }
                else
                {
                    licenseStatus.Type = "Full";
                    licenseStatus.Status = "Failure";
                    licenseStatus.Message = "Invalid License Key";
                }
                return licenseStatus;
            }
            catch (Exception exception)
            {
                Logger.Log(exception.Message);
                throw;
            }

        }

        public void SaveLicenseKey(string encryptedKey)
        {
            if (!Directory.Exists(@AppDomain.CurrentDomain.BaseDirectory + "\\" + KeyFolderName + "\\"))
                Directory.CreateDirectory(@AppDomain.CurrentDomain.BaseDirectory + "\\" + KeyFolderName + "\\");

            using (
                var fs =
                    new FileStream(@AppDomain.CurrentDomain.BaseDirectory + "\\" + KeyFolderName + "\\" + KeyFileName,
                        FileMode.Create, FileAccess.Write))
            {

                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.WriteLine(encryptedKey);
                }
            }
        }

        public string GetLicenseKey()
        {
            var thisLicense = string.Empty;
            try
            {
                if (Directory.Exists(@AppDomain.CurrentDomain.BaseDirectory + "\\" + KeyFolderName + "\\"))
                {
                    if (!File.Exists(@AppDomain.CurrentDomain.BaseDirectory + "\\" + KeyFolderName + "\\" + KeyFileName))
                        throw new ActivaionException("File does not exists");

                    using (
                        var fs =
                            new FileStream(
                                @AppDomain.CurrentDomain.BaseDirectory + "\\" + KeyFolderName + "\\" + KeyFileName,
                                FileMode.Open, FileAccess.Read))
                    {
                        using (var sr = new StreamReader(fs))
                        {
                            thisLicense = sr.ReadToEnd().TrimEnd();
                        }
                    }
                }
                else
                {
                    throw new ActivaionException("Folder does not exists");
                }
            }
            catch (ActivaionException e)
            {
                throw new ActivaionException(e.Message);
            }


            return thisLicense;
        }

        public void Logout()
        {
            HttpContext.Current.Session.Abandon();
            HttpContext.Current.Response.Redirect("Default.aspx");
        }

    }

    public class ActivaionException : Exception
    {
        public ActivaionException()
        {
            
        }

        public ActivaionException(string message):base(message)
        {
            Logger.Log(message);
            //throw new Exception(message);
            
        }

        public ActivaionException(string message, Exception innerMessage)
            : base(message, innerMessage)
        {
            Logger.Log(message);
            //throw new Exception(message);
        }

    }

}
