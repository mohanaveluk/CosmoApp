using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cosmo.Entity
{
    public class GroupScheduleServiceMailEntity
    {
        public int Group_Schedule_ID { get; set; }
        public int Group_ID { get; set; }
        public string Group_Name { get; set; }
        public DateTime Group_Schedule_Datatime { get; set; }
        public string Group_Schedule_Action { get; set; }
        public string Group_Schedule_Status { get; set; }
        public DateTime Group_Schedule_CompletedTime { get; set; }
        public string Group_Schedule_Comments { get; set; }
        public string Group_Schedule_CreatedBy { get; set; }
        public DateTime Group_Schedule_CreatedDatetime { get; set; }
        public string Group_Schedule_UpdatedBy { get; set; }
        public DateTime Group_Schedule_UpdatedDatetime { get; set; }
        public int Env_ID { get; set; }
        public string Env_Name { get; set; }
        public string HostIP { get; set; }
        public string Port { get; set; }
        public string WindowsServices { get; set; }
        public string TimeZone { get; set; }

    }
}
