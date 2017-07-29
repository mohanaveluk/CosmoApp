using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Web;
using System.Web.UI;
using Cosmo.Service;

namespace Cosmo.Web.forms
{
    public partial class Dashboard : System.Web.UI.Page
    {
        #region service objects

        MonitorService _monitorService;
        WinService _winService;

        #endregion service objects

        #region variable initialisation
        public int RefreshTime { get; set; }
        #endregion variable initialisation


        protected Dashboard():this(new MonitorService(), new WinService())
        {
            
        }

        private Dashboard(MonitorService monitorService, WinService winService)
        {
            _monitorService = monitorService;
            _winService = winService;
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            hidCurrentTime.Value = DateTime.Now.AddSeconds(1).ToString("MM/dd/yyyy HH:mm:ss");
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            CommonService.ValidateUser();
            Response.AppendHeader("Access-Control-Allow-Origin", "*");
            

            SetWebHostName();

            GetPersonaliseSetting();
        }

        private void GetPersonaliseSetting()
        {
            var userId = Convert.ToInt32(HttpContext.Current.Session["_LOGGED_USERD_ID"]);
            var personalise = _monitorService.GetPersonalize(userId);
            if (personalise != null && personalise.Count > 0)
            {
                HttpContext.Current.Session["_PERSONALIZE"] = personalise[0];
                RefreshTime = personalise[0].RefreshTime;
            }

        }

        private void SetWebHostName()
        {
            ClientScriptManager csm = Page.ClientScript;
            var systemType = this.GetType();
            var webHostName = "webHostName";

            var hostName = Dns.GetHostName();
            var host = System.Net.Dns.GetHostEntry(hostName);
            var ip = host.AddressList.FirstOrDefault(n => n.AddressFamily == AddressFamily.InterNetwork);
            Logger.Log(string.Format("HostName: {0}, IP: {1}", hostName, ip));

            var sb = new StringBuilder();
            sb.Append("<script>");
            sb.Append("systemName = '" + hostName + "';");
            sb.Append("</script>");

            csm.RegisterStartupScript(systemType, webHostName, sb.ToString());
        }
    }
}