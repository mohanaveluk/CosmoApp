using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cog.WS.Entity
{
    public class AcknowledgeEntity
    {
        public int AckId { get; set; }
        public int EnvId { get; set; }
        public string EnvName { get; set; }
        public int ConfigId { get; set; }
        public string ServiceName { get; set; }
        public int MonId { get; set; }
        public bool IsAcknowledgeMode { get; set; }
        public string AcknowledgeAlertChange { get; set; }
        public string AcknowledgeComments { get; set; }
        public string CreatedBy { get; set; }
        public string ContentType { get; set; }
        public string Status { get; set; }
        public DateTime CreatedDate { get; set; }

    }
}
