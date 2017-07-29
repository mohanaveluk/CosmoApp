using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Cosmo.Service;

namespace Cosmo.Web.forms
{
    public partial class ReportServiceRestart : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            CommonService.ValidateUser();
        }
    }
}