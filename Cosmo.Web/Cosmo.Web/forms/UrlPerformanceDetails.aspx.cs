using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Cosmo.Web.forms
{
    public partial class UrlPerformanceDetails : System.Web.UI.Page
    {
        public string EnvId { get; set; }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request.QueryString["e"] != null)
                {
                    hidEnvId.Value = Request.QueryString["e"];
                }
            }
        }
    }
}