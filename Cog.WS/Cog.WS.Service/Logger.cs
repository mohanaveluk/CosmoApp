using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Configuration;

namespace Cog.WS.Service
{
    public static class Logger
    {
        static readonly string LogFileName = Convert.ToString(ConfigurationManager.AppSettings["LogFileName"]);
        static readonly string LogFileLocation = Convert.ToString(ConfigurationManager.AppSettings["LogFileLocation"]);

        static Logger()
        {
            var logDate = DateTime.Now.ToString("MMddyyyy");
            LogFileName = string.IsNullOrEmpty(LogFileName)
                ? "WindowsService_" + logDate + ".log"
                : LogFileName.Replace(".log", "_" + logDate + ".log").Replace(".txt", "_" + logDate + ".log");
        }

        public static void Log(string message)
        {
            var logMmessage = string.Format("{0} {1}", DateTime.Now, ": " + message);
            try
            {
                using (
                    var fs = string.IsNullOrEmpty(LogFileLocation)
                        ? new FileStream(@AppDomain.CurrentDomain.BaseDirectory + "\\" + LogFileName, FileMode.Append,
                            FileAccess.Write)
                        : new FileStream(LogFileLocation + "\\" + LogFileName, FileMode.Append, FileAccess.Write))
                {
                    using (var sw = new StreamWriter(fs))
                    {
                        if (message == "\n" || message == "")
                        {
                            sw.WriteLine("\r\n");
                        }
                        else
                        {
                            sw.WriteLine(logMmessage);
                        }
                        sw.Close();
                    }
                    fs.Close();
                }
            }
            catch (Exception ex)
            {
                // ignored
            }
        }
    }
}
