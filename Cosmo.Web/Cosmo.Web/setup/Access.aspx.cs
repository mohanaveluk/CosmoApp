using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Cosmo.Entity;
using Cosmo.Service;

namespace Cosmo.Web.setup
{
    public partial class Access : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            InsertInitialAccessKey();
            Response.Redirect("MailServerConfig.aspx");
        }

        private void InsertInitialAccessKey()
        {
            try
            {
                var activation = new ActivationEntity
                {
                    Key = CommonUtility.InitialAccessKey(),
                };
                var commonService = new CommonService();
                var result = commonService.InsActivation(activation);

            }
            catch (Exception exception)
            {
                Logger.Log(exception.Message);
                throw new ApplicationException(exception.Message);
            }
        }
    }
}