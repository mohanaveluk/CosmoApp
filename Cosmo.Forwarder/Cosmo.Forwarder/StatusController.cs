using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Cosmo.Forwarder
{
    public class StatusController: ApiController
    {
        private static readonly IWindowServiceRepository Repository = new WindowServiceRepository();

        [HttpGet]
        [AcceptVerbs("Get")]
        public WindowService GetStatus(string svn, string srn)
        {
            return Repository.GetStatus(svn, srn);
        }

        [HttpGet]
        [AcceptVerbs("Get")]
        public List<WindowService> GetAllServiceStatus(string srn)
        {
            return Repository.GetAllServiceStatus(srn);
        }

        [HttpGet]
        [AcceptVerbs("Get")]
        public WindowService StartService(string svn, int t, string srn)
        {
            return Repository.StartService(svn, t, srn);
        }

        [HttpGet]
        [AcceptVerbs("Get")]
        public WindowService StopService(string svn, int t, string srn)
        {
            return Repository.StopService(svn, t, srn);
        }

        [HttpGet]
        [AcceptVerbs("Get")]
        public WindowService RestartService(string svn, int t, string srn)
        {
            return Repository.RestartService(svn, t, srn);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="svn">Service Name</param>
        /// <param name="p">Port</param>
        /// <param name="srn">Target Server Name</param>
        /// <param name="t">Type of service</param>
        /// <param name="tm">Time in millisecond</param>
        /// <param name="a">Action</param>
        /// <param name="gn"></param>
        /// <returns></returns>
        [HttpGet]
        [AcceptVerbs("Get")]
        public WindowService ServiceProcess(string svn, string p, string srn, string t, int tm, string a, string gn)
        {
            return Repository.ServiceProcess(svn, p, srn, t, tm, a, gn);
        }

        [HttpGet]
        [AcceptVerbs("Get")]
        public string GetMonitorStatus(string srn, string p, string t, string a, int tm)
        {
            var timeout = TimeSpan.FromMilliseconds(tm * 1000);
            return Repository.GetMonitorStatus(srn, p, t, a, timeout);
        }

    }
}
