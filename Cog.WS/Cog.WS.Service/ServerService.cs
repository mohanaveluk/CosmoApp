using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Cog.WS.Service
{
    public class ServerService
    {
        #region variables
        private static string _remoteSystemUrlForServiceStatus = ConfigurationManager.AppSettings["_remoteSystemUrlForServiceStatus"].ToString();
        private static string _serviceOperationBySelfHost = ConfigurationManager.AppSettings["_serviceOperationBySelfHost"].ToString();
        #endregion variables
        public ServerService()
        {
            
        }
        

    }
}
