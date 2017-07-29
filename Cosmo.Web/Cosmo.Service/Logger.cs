using System;
using System.IO;
using System.Configuration;

namespace Cosmo.Service
{
    public static class Logger
    {
        static readonly string LogFileName = Convert.ToString(ConfigurationManager.AppSettings["LogFileName"]);
        static readonly string LogFileLocation = Convert.ToString(ConfigurationManager.AppSettings["LogFileLocation"]);
        static Logger()
        {
            var logDate = DateTime.Now.ToString("MMddyyyy");
            LogFileName = string.IsNullOrEmpty(LogFileName)
                ? "CosmoService_" + logDate + ".log"
                : LogFileName.Replace(".log", "_" + logDate + ".log").Replace(".txt", "_" + logDate + ".log");
        }

        public static void Log(string message)
        {
            FileStream fs;
            try
            {
                if (string.IsNullOrEmpty(LogFileLocation))
                {
                    if (!Directory.Exists(@AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\"))
                        Directory.CreateDirectory(@AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\");
                    fs = new FileStream(@AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\" + LogFileName, FileMode.Append, FileAccess.Write);
                }
                else
                    fs = new FileStream(LogFileLocation + "\\" + LogFileName, FileMode.Append, FileAccess.Write);

                using (var sw = new StreamWriter(fs))
                {

                    if (message == "\n" || message == "")
                    {
                        sw.WriteLine("\r\n");
                    }
                    else
                    {
                        string _message = string.Format("{0} {1}", DateTime.Now, ": " + message);
                        sw.WriteLine(_message);
                    }
                    sw.Close();
                }
                fs.Close();
            }
            catch (Exception ex)
            {
                if (!Directory.Exists(@AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\"))
                    Directory.CreateDirectory(@AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\");
                fs = new FileStream(@AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\" + LogFileName, FileMode.Append, FileAccess.Write);
                StreamWriter sw = new StreamWriter(fs);
                sw.WriteLine(ex.Message + " Unable to find folder location");
                sw.Close();
                fs.Close();
            }
         }

    }
}
