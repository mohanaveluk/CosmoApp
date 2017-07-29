using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cosmo.Entity
{
    public class UpDownTimeEntity
    {
        public int Env_Id { get; set; }
        public string Env_name { get; set; }
        public double TotalTime { get; set; }
        public double UpTime { get; set; }
        public double DownTime { get; set; }
        public double DownTimePercent { get; set; }
        public double UpTimePercent { get; set; }
    }
}
