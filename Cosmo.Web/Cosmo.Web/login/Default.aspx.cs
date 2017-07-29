using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Cosmo.Entity;
using Cosmo.Service;

namespace Cosmo.Web.login
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            HttpContext.Current.Session.Abandon();
            //ValidateLicenseKey();
            ValidateDatabase();
            ValidateLicense();
        }

        private void ValidateLicense()
        {
            if (Master != null)
            {
                //Label lblStatus = (Label) Master.FindControl("lblStatus");

                try
                {
                    var commonService = new CommonService();
                    var licenseStatus = commonService.GetUserAccess();
                    Session["LicenseStatus"] = licenseStatus;
                    hidLicenseStatus.Value = licenseStatus.Status;
                    lblLicStatus.InnerHtml = licenseStatus.Message;
                }
                catch (Exception exception)
                {
                    Response.Redirect("~/error/Oops.aspx?e=2");
                }
            }
        }

        private void ValidateDatabase()
        {
            var status = CommonService.GetAllMonitors();
            if (!string.IsNullOrEmpty(status) && status != "Success")
            {
                Response.Redirect(status);
            }

        }

        private void ValidateLicenseKey()
        {
            try
            {
                var commonService = new CommonService();
                var licenseKey = commonService.GetLicenseKey();
                var decryptedLicenseKey = CommonUtility.Decrypt_ActivationKey(licenseKey);
                var licenseKeyDetails = decryptedLicenseKey.Split('&');

                if (licenseKeyDetails.Length > 0)
                {
                    var licenseType = Convert.ToInt32(licenseKeyDetails[1]);
                    if (licenseType == 4) return;
                    if (DateTime.Parse(licenseKeyDetails[2]).AddMonths(licenseType) < DateTime.Now)
                    {
                        Logger.Log("Product was installed on " + licenseKeyDetails[2] + " and Expired on " + DateTime.Parse(licenseKeyDetails[2]).AddMonths(licenseType).ToLongDateString());
                        Response.Redirect("Activate.aspx", false);
                    }
                    else
                    {
                        lblExpiry.InnerText = "Product expires on " +
                                              DateTime.Parse(licenseKeyDetails[2]).AddMonths(licenseType).ToLongDateString();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex.ToString());
                Response.Redirect("Activate.aspx");

            }
        }

    }
}