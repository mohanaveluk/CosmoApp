using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Cosmo.Web.error
{
    public partial class PN404 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string[] filePath = new string[] {};
            char[] param = new char[] { '=' };
            if (HttpContext.Current.Request.Url.Query.Contains("="))
            {
                lblEcrrorMessage.Text = "Filename :" + HttpContext.Current.Request.Url.Query.Split(param)[1].ToString();
                
            }
        }
    }
}