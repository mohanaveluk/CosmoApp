using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Cosmo.Entity;

namespace Cosmo.Web
{
    public partial class Setup : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["LicenseStatus"] != null)
            {
                var licenseStatus = (LicenseStatus)Session["LicenseStatus"];
                //lblStatus.Text = licenseStatus.Message;

            }
            else
            {
                //Session["LicenseStatus"] = "<span class='lic-trial'> TRIAL VERSION EXPIRES IN <font color='#FF1352'> 30</font> DAYS</span>";
                Session["LicenseStatus"] = new LicenseStatus { ExpiryInDays = 30, Message = "TRIAL VERSION EXPIRES IN", Status = "Failure", Type = "Trial" };
                //Todo
            }
        }
    }
}