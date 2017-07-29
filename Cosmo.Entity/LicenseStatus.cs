using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cosmo.Entity
{
    public class LicenseStatus
    {
        private string _message;
        public int ExpiryInDays { get; set; }
        public string Status { get; set; }
        public string Type { get; set; } //Trial / Full
        public string PackageMode { get; set; } //Web Only / Web With Mobile
        public string Message
        {
            get
            {
                return _message; 
            }
            set
            {
                _message = value;
            }
        }


    }
}
