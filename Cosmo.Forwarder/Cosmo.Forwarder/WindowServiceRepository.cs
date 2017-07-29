using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cosmo.Forwarder
{
    public interface IWindowServiceRepository
    {
        WindowService GetStatus(string serviceName, string serverName);
        List<WindowService> GetAllServiceStatus(string serverName);
        WindowService StartService(string serviceName, int timeoutMilliseconds, string systemName);
        WindowService StopService(string serviceName, int timeoutMilliseconds, string systemName);
        WindowService RestartService(string serviceName, int timeoutMilliseconds, string systemName);
        WindowService ServiceProcess(string svn, string p, string srn, string t, int tm, string a, string gn);
        string GetMonitorStatus(string systemName, string port, string type, string action, TimeSpan timeout);
    }


    public class WindowServiceRepository : IWindowServiceRepository
    {
        #region Constants
        private const string ServiceRunning = "Running";
        private const string ServiceStandby = "Standby";
        private const string ServiceNotRunning = "Not Running";
        private const string ServiceStopped = "Stopped";
        private const string ServiceNodata = "No data";
        private const string ServicePaused = "Paused";
        private const string ServiceStarting = "Starting";
        private const string ServiceStopping = "Stopping";
        private const string ServiceStatuschanging = "ServiceStatus Changing";
        private const string ServiceNameNotfound = "Not Exists!";

        private const string DispNotReady = "not ready";
        private const string DispReady = "Dispatcher is ready";

        private const string CmServiceRunning = "Running";
        private const string CmServiceStandby = "Running as standby";

        private const string CONTENT_MANAGER = "Content Manager";
        private const string DISPATCHER = "Dispatcher";

        #endregion Constants

        public WindowService GetStatus(string serviceName, string serverName)
        {
            var currentStatus = GetWindowsServiceStatus(serviceName, serverName);
            Console.WriteLine("Service ServiceStatus for " + serviceName + " : " + currentStatus);
            Logger.Log("Service ServiceStatus for " + serviceName + " : " + currentStatus);
            return new WindowService
            {
                Status = ResponseStatus.Success.ToString(),
                ErrorMessage = string.Empty,
                ServiceName = serviceName,
                SystemName = serverName,
                ServiceStatus = currentStatus
            };
        }

        public List<WindowService> GetAllServiceStatus(string serverName)
        {
            var serviceList = new List<WindowService>();

            var services = ServiceController.GetServices(serverName);
            Logger.Log("Get status of all services on " + serverName);

            if (services.Length > 0)
            {
                serviceList.AddRange(services.Select(service => new WindowService
                {
                    Status = ResponseStatus.Success.ToString(),
                    ErrorMessage = string.Empty,
                    ServiceName = service.ServiceName,
                    ServiceStatus = service.Status.ToString(),
                    SystemName = serverName,
                }));
            }
            return serviceList;
        }

        public WindowService StartService(string serviceName, int ms, string systemName)
        {
            var result = ServiceOperation(serviceName, "0", systemName, null, ms, HandleException, "start");
            return result;
        }

        public WindowService StopService(string serviceName, int ms, string systemName)
        {
            var result = ServiceOperation(serviceName, "0", systemName, null, ms, HandleException, "stop");
            return result;
        }

        public WindowService RestartService(string serviceName, int ms, string systemName)
        {
            var result = ServiceOperation(serviceName, "0", systemName, null, ms, HandleException, "restart");
            return result;
        }

        public WindowService ServiceProcess(string svn, string p, string srn, string t, int tm, string a, string gn)
        {
            Logger.Log($"Group Name: {gn}");

            var result = (a == "1" || a == "start")
                ? ServiceOperation(svn, p, srn, t, tm, HandleException, "start")
                : (a == "2" || a == "stop")
                    ? ServiceOperation(svn, p, srn, t, tm, HandleException, "stop")
                    : (a == "3" || a == "restart")
                        ? ServiceOperation(svn, p, srn, t, tm, HandleException, "restart")
                        : new WindowService();
            return result;
        }

        private WindowService ServiceOperation(string serviceName, string port, string systemName, string type,
            int timeoutMilliseconds, Action<Exception> handleException, string actionName)
        {
            try
            {
                var service = !string.IsNullOrEmpty(systemName)
                    ? new ServiceController(serviceName, systemName)
                    : new ServiceController(serviceName);

                Logger.Log("Service Name: " + service.ServiceName.ToString());
                Logger.Log(
                    $"Service Type: {(!string.IsNullOrEmpty(type) ? (type.ToLower() == "c" ? CONTENT_MANAGER : DISPATCHER) : string.Empty)}");
                Logger.Log("Host Name: " + systemName);
                Logger.Log("Port: " + port);
                Logger.Log("Service action: " + actionName);
                Logger.Log("Service ServiceStatus: " + service.Status.ToString());

                var timeout = TimeSpan.FromSeconds(timeoutMilliseconds);
                var millisec1 = Environment.TickCount;

                if (actionName.ToLower(CultureInfo.CurrentCulture).Equals("stop"))
                {
                    if (service.Status != ServiceControllerStatus.Stopped)
                    {
                        service.Stop();
                        Logger.Log("Stopping service...");
                        service.WaitForStatus(ServiceControllerStatus.Stopped, timeout);
                        Logger.Log("Service Status: " + service.Status.ToString());
                    }
                    else
                    {
                        Logger.Log("Service ServiceStatus: Already " + service.Status.ToString());
                    }
                }
                else if (actionName.ToLower(CultureInfo.CurrentCulture).Equals("start"))
                {
                    if (service.Status == ServiceControllerStatus.Stopped)
                    {
                        service.Start();
                        Logger.Log("Starting service...");
                        service.WaitForStatus(ServiceControllerStatus.Running, timeout);
                        Logger.Log("Service Status: " + service.Status.ToString());
                    }
                    else
                        Logger.Log("Service ServiceStatus: Already " + service.Status.ToString());
                }
                else if (actionName.ToLower(CultureInfo.CurrentCulture).Equals("restart"))
                {
                    if (service.Status != ServiceControllerStatus.Stopped)
                    {
                        service.Stop();
                        Logger.Log("stopping service to restart...");
                        while (service.Status != ServiceControllerStatus.Stopped)
                        {
                            service.Refresh();
                            service.WaitForStatus(ServiceControllerStatus.Stopped, timeout);
                        }
                        Logger.Log("Service Status: " + service.Status.ToString());
                    }
                    else
                        Logger.Log("Service ServiceStatus during stop: " + service.Status.ToString());

                    // count the rest of the timeout
                    var millisec2 = Environment.TickCount;
                    timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds * 1000 - (millisec2 - millisec1));
                    if (service.Status == ServiceControllerStatus.Stopped)
                    {
                        service.Start();
                        Logger.Log("Restarting service...");
                        Logger.Log($"Pending time for service restart: {timeout:c}");
                        //service.WaitForStatus(ServiceControllerStatus.Running, timeout);
                        while (service.Status == ServiceControllerStatus.Stopped)
                        {
                            service.Refresh();
                            service.WaitForStatus(ServiceControllerStatus.Running, timeout);
                        }
                        Logger.Log("Service Status: " + service.Status.ToString());
                    }
                    else
                        Logger.Log("Service ServiceStatus during start: " + service.Status.ToString());
                }

                var millisec3 = Environment.TickCount;
                timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds * 1000 - (millisec3 - millisec1));
                Logger.Log($"Pending time for Monitor service: {timeout:c}");

                var monitorStatus = actionName.ToLower(CultureInfo.CurrentCulture).Contains("start") &&
                                    !string.IsNullOrEmpty(type)
                    ? GetMonitorStatus(systemName, port, type, actionName, timeout)
                    : ServiceStopped;

                Logger.Log("Monitor Status: " + monitorStatus);

                return new WindowService
                {
                    Status = ResponseStatus.Success.ToString(),
                    ErrorMessage = string.Empty,
                    ServiceName = serviceName,
                    SystemName = systemName,
                    ServiceStatus = service.Status.ToString(),
                    MonitorStatus = monitorStatus
                };
            }
            catch (Exception ex)
            {
                handleException(ex);
                return new WindowService
                {
                    Status = ResponseStatus.Failure.ToString(),
                    ErrorMessage = ex.Message,
                    ServiceName = serviceName,
                    SystemName = systemName,
                    MonitorStatus = string.Empty
                };
            }
        }


        private static void HandleException(Exception message)
        {
            Logger.Log("Error: " + message.Message);
            Logger.Log("Error: " + message.StackTrace);
        }

        /// <summary>
        /// Get the windows service status 
        /// </summary>
        /// <param name="serviceName"></param>
        /// <param name="serverName"></param>
        /// <returns></returns>
        private static string GetWindowsServiceStatus(string serviceName, string serverName)
        {
            var serviceStatus = string.Empty;

            var sc = !string.IsNullOrEmpty(serverName) ? new ServiceController(serviceName, serverName) : new ServiceController(serviceName);

            try
            {
                switch (sc.Status)
                {
                    case ServiceControllerStatus.Running:
                        {
                            serviceStatus = ServiceRunning;
                            break;
                        }
                    case ServiceControllerStatus.Stopped:
                        serviceStatus = ServiceStopped;
                        break;
                    case ServiceControllerStatus.Paused:
                        serviceStatus = ServicePaused;
                        break;
                    case ServiceControllerStatus.StopPending:
                        serviceStatus = ServiceStopping;
                        break;
                    case ServiceControllerStatus.StartPending:
                        serviceStatus = ServiceStarting;
                        break;
                    case ServiceControllerStatus.ContinuePending:
                        break;
                    case ServiceControllerStatus.PausePending:
                        break;
                    default:
                        serviceStatus = ServiceStatuschanging;
                        break;
                }
            }
            catch (Exception ex)
            {
                serviceStatus = ServiceNameNotfound;
                Console.WriteLine(ex.Message);
                Logger.Log(ex.Message);
                if (ex.Message.Contains("not found"))
                    serviceStatus = ServiceNameNotfound;
            }
            finally { }
            return serviceStatus;
        }

        public string GetMonitorStatus(string systemName, string port, string type, string action, TimeSpan timeout)
        {
            var processing = true;
            var monitorStatus = string.Empty;

            var t = Task.Run(() =>
            {
                var url = "http://" + systemName + ":" + port + "/p2pd/servlet";

                if (type.ToLower() == "d") url += "/gc";

                using (var webClient = new WebClient())
                {
                    webClient.Headers.Add("user-agent",
                        "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
                    try
                    {

                        // ReSharper disable once AccessToModifiedClosure
                        while (processing == true)
                        {
                            var data = webClient.DownloadString(url);
                            Logger.Log(data);

                            switch (type.ToLower())
                            {
                                case "d":
                                    if (data.ToLower().Contains(DispReady.ToLower()))
                                    {
                                        monitorStatus = DispReady;
                                        processing = false;
                                    }
                                    else if (data.ToLower().Contains(DispNotReady.ToLower()))
                                    {
                                        monitorStatus = DispNotReady;
                                        processing = true;
                                    }
                                    break;
                                case "c":
                                    if (data.ToLower().Contains(ServiceNotRunning.ToLower()))
                                    {
                                        monitorStatus = ServiceNotRunning;
                                        processing = true;
                                    }
                                    else if (data.ToLower().Contains(CmServiceStandby.ToLower()))
                                    {
                                        monitorStatus = CmServiceStandby;
                                        processing = false;
                                    }
                                    else if (data.ToLower().Contains(CmServiceRunning.ToLower()))
                                    {
                                        monitorStatus = ServiceRunning;
                                        processing = false;
                                    }
                                    else
                                    {
                                        monitorStatus = ServiceNotRunning;
                                    }
                                    break;
                            }
                            if (processing) Thread.Sleep(2000);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex.ToString());
                        monitorStatus = ex.Message;
                    }

                }
            });

            try
            {
                var result = t.Wait(timeout);
                processing = false;
                if (!result && string.IsNullOrEmpty(monitorStatus)) monitorStatus = ServiceNotRunning;
                return monitorStatus;
            }
            catch (Exception ex)
            {
                Logger.Log(ex.ToString());
                throw;
            }
        }
    }
}
