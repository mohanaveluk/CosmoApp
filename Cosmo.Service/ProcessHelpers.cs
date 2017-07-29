using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Cosmo.Service
{
    public static class ProcessHelpers
    {
        public static bool IsRunning(string name)
        {
            return Process.GetProcessesByName(name).Length > 0;
        }
    }
}
