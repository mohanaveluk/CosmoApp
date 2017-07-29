using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Cosmo.Web.forms
{
    public partial class ServerPerformanceDetails : System.Web.UI.Page
    {
        public string HostName { get; set; }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request.QueryString["h"] != null)
                {
                    hidHostName.Value = Request.QueryString["h"];
                }
            }
        }
    }
}