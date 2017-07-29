using System;
using Cosmo.Service;

namespace Cosmo.Web.forms
{
    public partial class UrlConfiguration : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            CommonService.ValidateUser();
        }
    }
}