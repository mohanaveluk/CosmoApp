using System;
using System.Net.Mime;
using System.Threading;
using Cog.CSM.Service;


namespace Cog.CSM
{
    public class Program
    {
        public static void Main(string[] args)
        {
            MonitorAllServices();
        }

        private static void MonitorAllServices()
        {
            try
            {
                using (var mutex = new Mutex(false, "CosmoService"))
                {
                    try
                    {
                        Logger.Log("Checking for waitone"); 
                        if (!mutex.WaitOne(5000, false))
                        {
                            Logger.Log("Monitoring instance is already running");
                            Environment.Exit(1);
                        }
                        //Console.WriteLine("Processing. Please wait....");
                        var scheduleService = new SchedulerService(new MailService());
                        scheduleService.MonitorAllServices();
                    }
                    catch (Exception exMutex)
                    {
                        Logger.Log(exMutex.Message);
                        if (exMutex.InnerException != null)
                        {
                            Logger.Log(exMutex.InnerException.Message.ToString());
                        }
                    }
                }
                
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Could not open a connection"))
                    Console.WriteLine("Unable to connect to SQL Server Database. Please check the connection setting");
                else if (ex.Message.Contains("Login failed"))
                    Console.WriteLine(ex.Message);
                else
                    Console.WriteLine(ex.Message);
                Console.ReadLine();
            }
        }
    }
}
