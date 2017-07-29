using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Cosmo.Web.error
{
    public partial class Oops : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            divRedirect.Visible = false;
            if (Request.QueryString["e"] != null)
            {
                if (Request.QueryString["e"] == "1")
                {
                    lblErrorMessage.Text = "It seems the database that you have configured is invalid. <br>Please make sure you connect to appropriate database";
                    divRedirect.Visible = true;
                }
                else if (Request.QueryString["e"] == "2")
                {
                    lblErrorMessage.Text = "Unable to connect to Database. Please make sure the Service and Agent are running";
                    divRedirect.Visible = true;
                }
                else
                {
                    lblErrorMessage.Text = "An unexpected error has occurred. please contact administrator";
                }
            }
            else
            {
                lblErrorMessage.Text = "An unexpected error has occurred. please contact administrator";
            }
        }
    }
}