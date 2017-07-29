using System;
using System.Configuration;
using Cosmo.Service;

namespace Cosmo.Web.setup
{
    public partial class Initialize : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                var enviService = new EnvironmentService();
                var connectionString = ConfigurationManager.ConnectionStrings["CSMConn"].ConnectionString;
                if (string.IsNullOrEmpty(connectionString))
                {
                    Server.Transfer("DBConfig.aspx",false);
                }
                else if (!string.IsNullOrEmpty(connectionString))
                {
                    var enviList = enviService.GetEnvironments(0);
                    Response.Redirect("../login/Default.aspx", false);
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.ToLower().Contains("ConnectionString property has not been initialized".ToLower()) || ex.Message.ToLower().Contains("cannot open database"))
                {
                    Server.Transfer("DBConfig.aspx");
                    Logger.Log(ex.ToString());
                }
                else if (ex.Message.ToLower().Contains("Could not find stored procedure".ToLower()))
                {
                    Logger.Log(ex.ToString());
                    Response.Redirect("~/error/Oops.aspx?e=1");
                }
                else if (ex.Message.ToLower().Contains("network-related or instance-specific error".ToLower()))
                {
                    Logger.Log(ex.ToString());
                    //Response.Redirect("~/error/Oops.aspx?e=2");
                }

                throw new ApplicationException(ex.ToString());
            }

        }
    }


    public class InitializeException : Exception
    {
        public InitializeException(string message) : base(message)
        {

        }

        public InitializeException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}