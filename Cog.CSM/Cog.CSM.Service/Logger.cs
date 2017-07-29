using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Configuration;
using System.Threading.Tasks;

namespace Cog.CSM.Service
{
    public static class Logger
    {
        static readonly string LogFileName = Convert.ToString(ConfigurationManager.AppSettings["LogFileName"]);
        static readonly string LogFileLocation = Convert.ToString(ConfigurationManager.AppSettings["LogFileLocation"]);
        private static object _locker;

        static Logger()
        {
            var logDate = DateTime.Now.ToString("MMddyyyy");
            LogFileName = string.IsNullOrEmpty(LogFileName)
                ? "MonitorService_" + logDate + ".log"
                : LogFileName.Replace(".log", "_" + logDate + ".log").Replace(".txt", "_" + logDate + ".log");
            _locker = new Object();
        }

        public static void Log(string message)
        {
            string logMessage = $"{DateTime.Now} {": " + message}";

            //if (message.Contains("Could not load file"))
            //    throw new ApplicationException("Error Loading");

            try
            {
                lock (_locker)
                {
                    using (var fs = string.IsNullOrEmpty(LogFileLocation)
                        ? new FileStream(@AppDomain.CurrentDomain.BaseDirectory + "\\" + LogFileName,
                            FileMode.Append,
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
                                sw.WriteLine(logMessage);
                            }
                            sw.Close();
                        }
                        fs.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
                // ignored
            }

        }
    }
}
