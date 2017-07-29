using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Windows.Forms;
using Cosmo.Entity;
using Cosmo.Service;

namespace Cosmo.Web.login
{
    public partial class ActivateProduct : System.Web.UI.Page
    {
        private const string KeyFolderName = "line";
        private const string KeyFileName = "planline.lxd";
        private static string _Val;

        public string LicenseStatus { get; set; }
        public Dictionary<string, string> licTypeDict = new Dictionary<string, string>();

        public static string Val
        {
            get { return _Val; }
            set { _Val = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["LicenseStatus"] != null)
            {
                var licenseStatus = (LicenseStatus)Session["LicenseStatus"];
                if (licenseStatus.Type == "Licensed")
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), Guid.NewGuid().ToString(),
                        "$(function () { ActivatedAlready();});", true);
                }
            }

            if (!IsPostBack)
            {
                var internalKey = CommonUtility.GetInternalKeyWithPackaType("W");
                txtInternalKey.Value = internalKey;
                //ClientScript.RegisterStartupScript(GetType(), Guid.NewGuid().ToString(), "alert('internalKey ');",true);

                //if (IsLicenseExists())
                //{
                //    if (LicenseStatus.ToLower().Contains("expired"))
                //    {
                //        lblExpiry.InnerText = LicenseStatus;
                //        return;
                //    }

                //    ClientScript.RegisterStartupScript(GetType(), Guid.NewGuid().ToString(),
                //        "$(function () { ProductActivated('" + LicenseStatus + "');});", true);
                //}
            }
            //ScriptManager.RegisterStartupScript(this, this.GetType(), Guid.NewGuid().ToString(), "ActivatedAlready();", true);
        }

        [WebMethod]
        public static string rdoPackage_CheckedChanged(string type)
        {
            var internalKey = CommonUtility.GetInternalKeyWithPackaType(type);
            return internalKey;
        }

        [WebMethod]
        public static string ValidateActivationKey2(string activationKey, string confirmationNumber)
        {
            //var activationKey = txtActivationKey.Value;

            string status = string.Empty;
            try
            {
                var activation = new ActivationEntity
                {
                    Key = activationKey,
                };
                var commonService = new CommonService();
                if (!CommonUtility.IsKeyDecryptable(activation)) throw new ApplicationException("Invalid Key!");
                var result = commonService.InsActivation(activation);

                if (!string.IsNullOrEmpty(activationKey))
                {
                    var licenseStatus = commonService.GetUserAccess();
                    HttpContext.Current.Session["LicenseStatus"] = licenseStatus;
                    if (licenseStatus.Status == "Success" && licenseStatus.Type != "Trial")
                    {
                        status = "Great: Cosmo product has been activated and enjoy using this product";
                    }
                    else if (licenseStatus.Status == "Success" && licenseStatus.Type == "Trial")
                    {
                        status = "Trial version has been extended. <a href='../login/Default.aspx' title='Click here to login Cosmo'><b>Login</b></a> to Cosmo";
                    }
                    else
                        status = "Oops: Invalid Activation Key";
                }
            }
            catch (CryptographicException cryExpException)
            {
                status = "Oops: Invalid Activation Key";
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Invalid length") || ex.Message.Contains("Invalid Key"))
                    status = "Oops: Invalid Activation Key";
                else
                    status = "Oops: " + ex.Message;
            }
            return status;
        }

        private bool IsLicenseExists()
        {
            var typeDict = new Dictionary<string, string>
            {
                {"1", "30 Days"},
                {"2", "60 Days"},
                {"3", "90 Days"},
                {"4", "Full License"}
            };
            try
            {
                var commonService = new CommonService();
                var licenseKey = commonService.GetLicenseKey();
                var decryptedLicenseKey = CommonUtility.Decrypt_ActivationKey(licenseKey);
                var licenseKeyDetails = decryptedLicenseKey.Split('&');
                if (licenseKeyDetails.Length > 0)
                {
                    var licenseType = Convert.ToInt32(licenseKeyDetails[1]);
                    if (licenseType == 4)
                    {
                        LicenseStatus = "Cosmo product was activated with full version on " + DateTime.Parse(licenseKeyDetails[2]).ToShortDateString();
                        return true;
                    }
                    if (DateTime.Parse(licenseKeyDetails[2]).AddMonths(licenseType) >= DateTime.Now)
                    {
                        LicenseStatus = "Cosmo product was activated with " + typeDict[licenseKeyDetails[1]] +
                                        " of Evaluation version on " +
                                        DateTime.Parse(licenseKeyDetails[2]).ToShortDateString();
                        return true;
                    }
                    else
                    {
                        LicenseStatus = "Evaluation period has expired on " + DateTime.Parse(licenseKeyDetails[2]).AddMonths(licenseType).ToLongDateString();
                        return true;
                    }

                }
                LicenseStatus = "Product was not validated";
            }
            catch (Exception e)
            {
                LicenseStatus = e.Message;
            }
            return false;
        }

        private static void MyMethod()
        {
            Clipboard.SetText(Val);
        }

        protected void btnClip_Click(object sender, EventArgs e)
        {
            Val = hixKey.Value;
            Thread staThread = new Thread(new ThreadStart(MyMethod));
            staThread.ApartmentState = ApartmentState.STA;
            staThread.Start();
        }
    }


    public class ApplicationException : Exception
    {
        public ApplicationException(string message)
            : base(message)
        {

        }

        public ApplicationException(string message, Exception innException)
            : base(message, innException)
        {

        }
    }
}