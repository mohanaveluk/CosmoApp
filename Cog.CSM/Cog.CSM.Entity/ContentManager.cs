using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cog.CSM.Entity
{
    public class ContentManager
    {
        public string Build 
        {
            get;
            set;
        }

        public string StartTime
        {
            get;
            set;
        }

        public string CurrentTime
        {
            get;
            set;
        }

        public string Status
        {
            get;
            set;
        }
        public string Comments
        {
            get;
            set;
        }
        public string Url
        {
            get;
            set;
        }
        public int EnvId
        {
            get;
            set;
        }
        public string EnvName
        {
            get;
            set;
        }
        public string HostIPPort
        {
            get;
            set;
        }
        public string ServiceType
        {
            get;
            set;
        }
    }
}
