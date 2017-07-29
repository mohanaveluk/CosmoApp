using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cosmo.Forwarder
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
#if (DEBUG)
            var serviceHost = new ForwarderService();
            serviceHost.OnDebug();
            Debugger.Launch();
#else
            var servicesToRun = new ServiceBase[]
            {
                new ForwarderService()
            };
            ServiceBase.Run(servicesToRun);
#endif
        }
    }
}
