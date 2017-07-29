using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cog.WS.Service;
using System.Threading;

namespace Cog.WS
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //MonitorAllServices();
            try
            {
                using (var mutex = new Mutex(false, "CosmoWindowsServiceActivator"))
                {
                    try
                    {
//                        Logger.Log("Validating if any instance is running");
                        if (!mutex.WaitOne(5000, false))
                        {
                             Logger.Log("Instance is running already");
                            //Console.ReadLine();
                            return;
                        }
                        Logger.Log("Verifying scheduler");
                        #region Windows service process

                        var manipulationService = new ManipulationService();
                        manipulationService.RunScheduledServiceOperation();

                        Logger.Log("Process has finished");
                        //Thread.Sleep(10000);

                        #endregion Windows service process
                        //Console.ReadLine();
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex.ToString());
                        if (ex.InnerException != null)
                        {
                            Logger.Log(ex.InnerException.Message.ToString());
                        }
                    }
                }
            }
            catch (Exception exAll)
            {
                if (exAll.InnerException != null)
                {
                    Logger.Log(exAll.InnerException.Message.ToString());
                }
                Logger.Log(exAll.Message);
            }
        }


    }
}
