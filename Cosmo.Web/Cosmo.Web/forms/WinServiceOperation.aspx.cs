using System;
using Cosmo.Service;

namespace Cosmo.Web.forms
{
    public partial class WinServiceOperation : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            CommonService.ValidateUser();
            lblCurrentTime.InnerText = lblNoteServerDateTime.InnerText = DateTime.Now.AddSeconds(1).ToString("MM/dd/yyyy HH:mm:ss");
            var timezone = TimeZone.CurrentTimeZone;
            lblNoteServerTimezone.InnerText = timezone.DaylightName;
        }
    }
}