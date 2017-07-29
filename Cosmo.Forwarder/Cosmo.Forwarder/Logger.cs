using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmo.Forwarder
{
    public static class Logger
    {
        private static string LogFileName = Convert.ToString(ConfigurationManager.AppSettings["LogFileName"]);
        private static readonly string LogFileLocation = Convert.ToString(ConfigurationManager.AppSettings["LogFileLocation"]);

        private static string GetLoggerFileName()
        {
            var logDate = DateTime.Now.ToString("MMddyyyy");
            var logFileName = string.IsNullOrEmpty(LogFileName)
                ? "CosmoService_" + logDate + ".log"
                : LogFileName.Replace(".log", "_" + logDate + ".log").Replace(".txt", "_" + logDate + ".log");
            return logFileName;
        }

        public static async void Log(string message)
        {
            FileStream fs;
            string logMessage = $"{DateTime.Now} {": " + message}";
            var logFileName = GetLoggerFileName();

            try
            {
                if (string.IsNullOrEmpty(LogFileLocation))
                {
                    if (!Directory.Exists(@AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\"))
                        Directory.CreateDirectory(@AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\");
                    fs = new FileStream(@AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\" + logFileName, FileMode.Append, FileAccess.Write);
                }
                else
                    fs = new FileStream(LogFileLocation + "\\" + logFileName, FileMode.Append, FileAccess.Write);

                using (var sw = new StreamWriter(fs))
                {
                    if (message == "\n" || message == "")
                    {
                        sw.WriteLine("\r\n");
                    }
                    else
                    {
                        await sw.WriteLineAsync(logMessage);
                    }
                    sw.Close();
                }
                fs.Close();
            }
            catch (Exception ex)
            {
                if (!Directory.Exists(@AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\"))
                    Directory.CreateDirectory(@AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\");
                fs = new FileStream(@AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\" + logFileName, FileMode.Append, FileAccess.Write);
                StreamWriter sw = new StreamWriter(fs);
                sw.WriteLine(ex.Message + " Unable to find folder location");
                sw.Close();
                fs.Close();
            }
        }

    }
}
