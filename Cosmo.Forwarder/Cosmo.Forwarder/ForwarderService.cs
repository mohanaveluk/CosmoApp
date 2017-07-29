using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin.Hosting;

namespace Cosmo.Forwarder
{
    public partial class ForwarderService : ServiceBase
    {
        private IDisposable _server = null;

        public ForwarderService()
        {
            InitializeComponent();
        }

        public void OnDebug()
        {
            Logger.Log("Debug mode");
            OnStart(null);
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                var baseUri = Convert.ToString(ConfigurationManager.AppSettings["baseUri"]);
                _server = WebApp.Start<StartService>(url: baseUri);
                Logger.Log("Cosmo forwarder self host service has started");
                Logger.Log("Base Uri: " + baseUri);
            }
            catch (Exception ex)
            {
                Logger.Log("OnStart: " + ex);
            }
        }

        protected override void OnStop()
        {
            _server?.Dispose();
            base.OnStop();
            Logger.Log("Cosmo forwarder self host service has stopped");
        }
    }
}
