using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.ServiceProcess;
using System.Diagnostics;
using System.Configuration;
using System.IO;
using Cog.CSM.MailService;
using Cog.WS.Entity;
using Cog.WS.Data;
using System.Globalization;
using System.Net;
using System.Web;
using System.Web.Script.Serialization;

namespace Cog.WS.Service
{
    public class ManipulationService
    {
        private static JavaScriptSerializer _javaScriptSerializer;

        #region Constants
        private const string SERVICE_RUNNING = "Running";
        private const string SERVICE_STANDBY = "Standby";
        private const string SERVICE_NOT_RUNNING = "Not Running";
        private const string SERVICE_STOPPED = "Stopped";
        private const string SERVICE_NODATA = "No data";
        private const string SERVICE_PAUSED = "Paused";
        private const string SERVICE_STARTING = "Starting";
        private const string SERVICE_STOPPING = "Stopping";
        private const string SERVICE_STATUSCHANGING = "Status Changing";
        private const string SERVICE_NAME_NOTFOUND = "Not Exists!";
        private const string ServiceSkipped = "Skipped";
        private const string CM = "CM";
        private const string DISP = "DISP";

        private const string COMPLETED = "S"; //Success
        private const string TIMEDOUT = "T"; //timedout
        private const string CANCELLED = "C"; //cancelled
        private const string ABONDED = "A"; //abonded
        private const string UNSUCCESSFULL = "U"; //abonded
        private const string NOACTION = "N"; //No Action Required
        private const string OPEN = "O"; //Open Status

        private const string DispNotReady = "not ready";
        private const string DispReady = "Dispatcher is ready";

        private const string CmServiceRunning = "Running";
        private const string CmServiceStandby = "Running as standby";

        #endregion Constants

        #region Objects

        static IWSData _winData;
        static MailService mailService = new MailService();
        static IMonitorData _monitorData;
        #endregion Objects

        #region variables
        //static string fileName = new DirectoryInfo(@AppDomain.CurrentDomain.BaseDirectory + "/../").FullName + @"ServiceControl\WinServiceControl.exe";
        static string fileName = new DirectoryInfo(@AppDomain.CurrentDomain.BaseDirectory).FullName + @"ServiceControl\WinServiceControl.exe";
        static int serviceTimeout = Convert.ToInt32(ConfigurationManager.AppSettings["WindowsServiceTimeout"]);
        static string SERVICEOPERATIONFAILURE = ConfigurationManager.AppSettings["ServiceOperationFailure"].ToString();
        static string SERVICEOPERATIONSUCCESS = ConfigurationManager.AppSettings["ServiceOperationSuccess"].ToString();
        static string SERVICEOPERATIONUNSUCCESS = ConfigurationManager.AppSettings["ServiceOperationUnsuccess"].ToString();
        static string SERVICEOPERATIONSTARTED = ConfigurationManager.AppSettings["ScheduleWindowsServiceOperationStarted"].ToString();
        static string ServiceOperationTimedout = ConfigurationManager.AppSettings["ServiceOperationTimedout"].ToString();
        static string ServiceOperationPartiallyTimedout = ConfigurationManager.AppSettings["ServiceOperationPartiallyTimedout"].ToString();
        private static string RemoteSystemURLForServiceStatus = ConfigurationManager.AppSettings["RemoteSystemURLForServiceStatus"].ToString();
        private static string RemoteSystemURLForServer = ConfigurationManager.AppSettings["RemoteSystemURLForServer"].ToString();
        private static string ServiceOperationBySelfHost = ConfigurationManager.AppSettings["ServiceOperationBySelfHost"].ToString();
        private static string MonitorServerPerformanceInterval = ConfigurationManager.AppSettings["MonitorServerPerformanceInterval"].ToString();

        private static string SystemUser = ConfigurationManager.AppSettings["SystemUser"].ToString();
        private static readonly string DbType = ConfigurationManager.AppSettings["DatabaseType"];

        private static string tableStyle = "'border-collapse:collapse;border:1px solid #eee; width:70%'";
        private static string headerStyle = "'border:1px solid #ddd; padding:5px;background-color:#A1C1D5'";
        private static string bodyStyle = "'border:1px solid #ddd; padding:5px'";
        private static string bodyStyleCenter = "'border:1px solid #ddd; padding:5px;text-align:center'";
        private static string tdHost = "'border:1px solid #ddd; padding:5px;width:30%'";
        private static readonly TimeZone LocalZone = TimeZone.CurrentTimeZone;

        #endregion variables

        public ManipulationService()
        {
            _javaScriptSerializer = new JavaScriptSerializer();
            var iDbType = DbType == DatabaseType.Oracle.ToString()
                ? Convert.ToInt32(DatabaseType.Oracle).ToString()
                : Convert.ToInt32(DatabaseType.SqlServer).ToString();

            _monitorData = new MonitorDataFactory().Create(Convert.ToInt32(iDbType).ToString());
            _winData = new WSDataFactory().Create(Convert.ToInt32(iDbType).ToString());
        }

        #region RunScheduledServiceOperation
        public void RunScheduledServiceOperation()
        {
            var processRunning = true;
            Logger.Log("Service operation in progress...");

            if (File.Exists(fileName))
            {
                while (processRunning)
                {
                    var monitorInProcess = ProcessHelpers.IsRunning("cog.csm");
                    if (monitorInProcess)
                    {
                        Logger.Log("Monitor process status: Running");
                        Thread.Sleep(1000);
                    }
                    else
                        processRunning = false;
                }
                ServerPerformanceSchedule();
                GroupScheduleDetails();
            }
            else
            {
                Logger.Log("WinServiceControl.exe file does not exist in the specified path " + fileName);
            }
        }
        #endregion RunScheduledServiceOperation

        #region windows service operation

        /// <summary>
        /// Get windows status after processing the required operation
        /// </summary>
        /// <param name="serviceName"></param>
        /// <param name="serviceMode"></param>
        /// <param name="systemName"></param>
        /// <param name="serviceID"></param>
        /// <returns></returns>
        public static WindowServiceStatus GetWindowsService(string serviceName, string serviceMode, string systemName,
            string serviceID, string port, string type, string groupName)
        {
            var result = new WindowServiceStatus();
            bool isTimeout = true;
            bool isProcessing = true;
            string serviceAction = string.Empty;
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            CancellationToken token = cancellationTokenSource.Token;
            try
            {
                switch (serviceMode)
                {
                    case "1":
                    case "start":
                        serviceAction = "start";
                        break;
                    case "2":
                    case "stop":
                        serviceAction = "stop";
                        break;
                    case "3":
                    case "restart":
                        serviceAction = "restart";
                        break;
                }
                if (!string.IsNullOrEmpty(serviceAction) && !string.IsNullOrEmpty(serviceName))
                {
                    var t = Task.Factory.StartNew(() =>
                    {
                        try
                        {
                            switch (ServiceOperationBySelfHost)
                            {
                                case "N":
                                    var processInfo = new ProcessStartInfo(fileName);
                                    processInfo.Arguments = "\"" + serviceName + "\" \"" + serviceAction + "\" \"" +
                                                            systemName + "\"";
                                    processInfo.Verb = "runas";
                                    processInfo.UseShellExecute = true;
                                    Process.Start(processInfo);
                                    break;
                                case "Y":
                                    //var uri =
                                    //    string.Format(
                                    //        RemoteSystemURLForServiceStatus + "{3}?svn={1}&t={2}&srn={0}",
                                    //        systemName, serviceName, serviceTimeout,
                                    //        serviceAction == "stop"
                                    //            ? "StopService"
                                    //            : serviceAction == "start"
                                    //                ? "StartService"
                                    //                : serviceAction == "restart" ? "RestartService" : string.Empty);

                                    var uri =
                                        string.Format(
                                            RemoteSystemURLForServiceStatus +
                                            "ServiceProcess?svn={1}&p={2}&srn={0}&t={3}&tm={4}&a={5}&gn={6}", systemName,
                                            serviceName, port, type == CM ? "C" : "D", serviceTimeout, serviceAction,
                                            groupName);

                                    Logger.Log("URI: " + uri);
                                    var wrapper = new WebHttpRequestBuilder();
                                    var request = wrapper.CreateGetRequest(uri);
                                    result = GetWebResponse<WindowServiceStatus>(request);

                                    Logger.Log(
                                        string.Format(
                                            "Response - Status: {0}, ErrorMessage:{1}, ServiceName:{2}, ServiceStatus:{3}, SystemName:{4}, MonitorStatus:{5}",
                                            result.Status, result.ErrorMessage, result.ServiceName, result.ServiceStatus,
                                            result.SystemName, result.MonitorStatus));
                                    break;
                            }

                            isTimeout = false;
                        }
                        catch (AggregateException agex)
                        {
                            Logger.Log("Windows Service operation: " + agex);
                            result.ErrorMessage = CANCELLED;
                            throw;
                        }
                        catch (Exception ex)
                        {
                            if (ex.Message.Contains("timed out"))
                            {
                                Logger.Log("Windows Service general operation: " + ex);
                                result.ErrorMessage = TIMEDOUT;
                            }
                            else
                            {
                                Logger.Log("Windows Service general operation: (Not Timed out)" + ex);
                                result.ErrorMessage = CANCELLED;
                            }
                        }
                        finally
                        {
                            if (cancellationTokenSource != null) cancellationTokenSource.Dispose();
                        }

                    }, token);
                    try
                    {
                        if (!t.Wait(serviceTimeout*1000, token)) //new TimeSpan(0, 0, serviceTimeout)
                        {
                            cancellationTokenSource.Cancel();
                        }
                        Logger.Log("Result status after thread time expiry: " + result.ServiceStatus);
                        //result.ServiceStatus = isTimeout ? TIMEDOUT : COMPLETED;
                    }
                    catch (AggregateException agex)
                    {
                        Logger.Log("Windows Service operation (After Thred): " + agex);
                        if (agex.InnerException != null)
                        {
                            Logger.Log(agex.InnerException.ToString());
                        }
                    }
                    finally
                    {
                        cancellationTokenSource.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                result.ErrorMessage = ABONDED;
                Logger.Log(ex.ToString());
            }
            return result;
        }


        /// <summary>
        /// Get the windows service status 
        /// </summary>
        /// <param name="ServiceName"></param>
        /// <param name="serviceName"></param>
        /// <param name="systemName"></param>
        /// <returns></returns>
        public static string GetWindowsServiceStatus(string serviceName, string systemName)
        {
            string serviceStatus;

            if (ServiceOperationBySelfHost == "Y")
                return GetRemoteWindowsServiceStatus(serviceName, systemName);
            
            var sc = !string.IsNullOrEmpty(systemName) ? new ServiceController(serviceName, systemName) : new ServiceController(serviceName);

            try
            {
                switch (sc.Status)
                {
                    case ServiceControllerStatus.Running:
                        {
                            serviceStatus = SERVICE_RUNNING;
                            break;
                        }
                    case ServiceControllerStatus.Stopped:
                        serviceStatus = SERVICE_STOPPED;
                        break;
                    case ServiceControllerStatus.Paused:
                        serviceStatus = SERVICE_PAUSED;
                        break;
                    case ServiceControllerStatus.StopPending:
                        serviceStatus = SERVICE_STOPPING;
                        break;
                    case ServiceControllerStatus.StartPending:
                        serviceStatus = SERVICE_STARTING;
                        break;
                    default:
                        serviceStatus = SERVICE_STATUSCHANGING;
                        break;
                }
            }
            catch (Exception ex)
            {
                serviceStatus = SERVICE_NAME_NOTFOUND;
                Logger.Log(ex.Message);
                if (ex.Message.Contains("not found"))
                    serviceStatus = SERVICE_NAME_NOTFOUND;
            }
            finally { }
            return serviceStatus;
        }
        #endregion windows service operation

        #region enum ScheduleAction
        public enum ScheduleAction
        {
            Start = 1,
            Stop,
            Restart
        };
        #endregion enum ScheduleAction

        #region Get all Group Schedule scrvice details

        /// <summary>
        /// Run all the service and update the status and update time 
        /// </summary>
        public static void GroupScheduleDetails()
        {
            
            int result;
            int resGroupSchedule;
            var serviceResult = new WindowServiceStatus();
            

            Dictionary<string, string> StatusCompletion = new Dictionary<string, string>();
            StatusCompletion.Add("start", "Started");
            StatusCompletion.Add("stop", "Stopped");
            StatusCompletion.Add("restart", "Restarted");
            StatusCompletion.Add("T", "Timed out");

            var processStatus = "";

            var groupScheduleDetails = GetGroupOpenScheduleDetails();
            try
            {
                if (groupScheduleDetails == null || groupScheduleDetails.Count <= 0) {Logger.Log("Scheduler does not exists"); return;}

                foreach (var groupSd in groupScheduleDetails)
                {
                    Logger.Log("Group Name: " + groupSd.Group_Name);

                    if (groupSd.GroupScheduleDetails != null && groupSd.GroupScheduleDetails.Count > 0)
                    {
                        var tempEntity = new List<GroupScheduleEntity>();
                        foreach (GroupScheduleDetailEntity entity in groupSd.GroupScheduleDetails)
                        {
                            if (entity.ServiceDetails != null && entity.ServiceDetails.Count > 0)
                            {
                                var mailEntiry = new GroupScheduleServiceMailEntity
                                {
                                    Group_Schedule_StartedTime = DateTime.Now,
                                    WindowsServices = "<table style=" + tableStyle + ">"
                                };
                                mailEntiry.WindowsServices += "<tr>";
                                mailEntiry.WindowsServices += "<th style=" + headerStyle + ">Server</th> <th style=" +
                                                              headerStyle + ">Port</th> <th  style=" + headerStyle +
                                                              ">Service Name</th> <th  style=" + headerStyle +
                                                              ">Completed Time</th> " +
                                                              "<th  style=" + headerStyle + ">Service Status</th>" +
                                                              "<th  style=" + headerStyle + ">Monitor Status</th> ";
                                mailEntiry.WindowsServices += "</tr>";

                                var standByServiceList = new List<StandByService>();

                            ProcessAgain:
                                foreach (var grpService in entity.ServiceDetails.ToList())
                                {
                                    if (grpService.Group_Schedule_Action == "0" ||
                                        !string.IsNullOrEmpty(grpService.Group_ActionCompletion_Status)) continue;

                                    serviceResult = new WindowServiceStatus();
                                    processStatus = "";

                                    grpService.Group_Schedule_StartedTime = DateTime.Now;
                                    
                                    if (!standByServiceList.Any(service => string.IsNullOrEmpty(service.Name)) &&
                                        groupSd.Group_Schedule_Action == "3" &&
                                        grpService.Schedule_Action == "1" &&
                                        grpService.ServiceTypeShort == CM)
                                    {
                                        var standByServiceStatus = WaitTillStandByCmToGoActive(standByServiceList, groupSd.Group_Schedule_Action,
                                            grpService.ServiceTypeShort);
                                        if (standByServiceStatus.ContainsIgnoringCase("Running"))
                                        {
                                            standByServiceList = new List<StandByService>();
                                        }
                                    }

                                    if (
                                        CheckCurrentStatus(grpService.WindowsServiceName,
                                            grpService.Schedule_Action, grpService.HostIP))
                                    {
                                        serviceResult = GetWindowsService(grpService.WindowsServiceName,
                                            grpService.Schedule_Action, grpService.HostIP,
                                            Convert.ToString(grpService.WindowsServiceId), grpService.Port,
                                            grpService.ServiceTypeShort, groupSd.Group_Name);

                                        if (!string.IsNullOrEmpty(serviceResult.MonitorStatus) &&
                                            serviceResult.MonitorStatus.ContainsIgnoringCase("standby"))
                                        {
                                            standByServiceList.Add(new StandByService { Name = serviceResult.ServiceName, Host = grpService.HostIP, Port = grpService.Port });
                                        }
                                        grpService.WindowsServiceStatus = serviceResult.ServiceStatus;

                                        if ((grpService.Schedule_Action == "1" ||
                                             grpService.Schedule_Action == "3") &&
                                            (!string.IsNullOrEmpty(serviceResult.Status) &&
                                             serviceResult.Status.Equals("Success")))
                                            SetAcknowledge(grpService, "start");
                                             }
                                    else
                                    {
                                        serviceResult.ErrorMessage = NOACTION;
                                    }

                                    grpService.Schedule_Status = !string.IsNullOrEmpty(serviceResult.Status) &&
                                                                 serviceResult.Status.ContainsIgnoringCase("success")
                                        ? COMPLETED
                                        : serviceResult.ErrorMessage; // == NOACTION ? NOACTION : UNSUCCESSFULL;

                                    Logger.Log("Schedule Status: " + grpService.Schedule_Status);
                                    Logger.Log("Service Status: " + serviceResult.ServiceStatus);
                                    Logger.Log("Monitor Status: " + serviceResult.MonitorStatus);

                                    processStatus = GetProcessStatus(serviceResult, grpService);
                                    
                                    grpService.Schedule_Action = groupSd.Group_Schedule_Action;
                                    grpService.Schedule_UpdatedTime =
                                        (DateTime) (grpService.Group_Schedule_UpdatedDatetime = DateTime.Now);
                                    result = _winData.UpdateScheduleServiceStatus(grpService);
                                    grpService.Group_ActionCompletion_Status =
                                        string.IsNullOrEmpty(serviceResult.ErrorMessage)
                                            ? StatusCompletion[GetAction(groupSd.Group_Schedule_Action).ToLower()]
                                            : StatusCompletion[serviceResult.ErrorMessage];

                                    grpService.Group_Schedule_Action = GetAction(groupSd.Group_Schedule_Action);
                                    grpService.Env_Name = entity.Env_Name;
                                    grpService.Group_Schedule_Timezone =
                                        LocalZone.StandardName.ToString(CultureInfo.InvariantCulture);
                                    grpService.Group_Name = groupSd.Group_Name;

                                    grpService.Group_Schedule_CreatedBy =
                                        !string.IsNullOrEmpty(groupSd.Group_Schedule_CreatedBy)
                                            ? groupSd.Group_Schedule_CreatedBy
                                            : SystemUser;

                                    grpService.ProcessStatus = processStatus;

                                    if (processStatus != COMPLETED)
                                    {
                                        mailService.SendMail(grpService.Env_ID, grpService.Config_ID,
                                            serviceResult.ErrorMessage == UNSUCCESSFULL ||
                                            serviceResult.ErrorMessage == NOACTION
                                                ? SERVICEOPERATIONUNSUCCESS
                                                : SERVICEOPERATIONFAILURE, grpService);

                                        if (serviceResult.ErrorMessage == TIMEDOUT && grpService.ServiceTypeShort == CM && grpService.Schedule_Action == "1")
                                        {
                                            var grpServiceTemp =
                                                entity.ServiceDetails.SingleOrDefault(
                                                    _ =>
                                                        _.ServiceTypeShort == CM &&
                                                        string.IsNullOrEmpty(_.Group_ActionCompletion_Status));
                                            if (grpServiceTemp != null)
                                            {
                                                var ind =
                                                    entity.ServiceDetails.FindIndex(
                                                        _ => _.Config_ID == grpService.Config_ID);

                                                entity.ServiceDetails.Remove(grpServiceTemp);
                                                entity.ServiceDetails.Insert(ind + 1, grpServiceTemp);

                                                goto ProcessAgain;
                                            }
                                        }

                                        if (ValidateToContinueProcess(serviceResult.ErrorMessage,
                                            grpService))
                                        {
                                            break;
                                        }

                                    }

                                }

                                var geroupServiceDetails = new List<GroupScheduleServiceDetailEntity>();
                                Logger.Log(string.Format("Writing details for email"));
                                foreach (var grpService in entity.ServiceDetails)
                                {
                                    Logger.Log(string.Format("Host: {0}, Port: {1}, ServiceName:{2}, CompletionStatus:{3}", grpService.HostIP, grpService.Port, grpService.WindowsServiceName, grpService.Group_ActionCompletion_Status));

                                    if (
                                        geroupServiceDetails.Any(
                                            _ => _.HostIP == grpService.HostIP && _.Port == grpService.Port))
                                    {
                                        Logger.Log(string.Format("Skipped - Host: {0}, Port: {1}, ServiceName:{2}, CompletionStatus:{3}", grpService.HostIP, grpService.Port, grpService.WindowsServiceName, grpService.Group_ActionCompletion_Status));
                                        continue;
                                        
                                    }

                                    geroupServiceDetails.Add(grpService);

                                    var monitorStatus = GetRemoteServiceMonitorStatus(grpService.HostIP,
                                        grpService.Port, grpService.ServiceTypeShort == CM ? "C" : "D",
                                        grpService.Schedule_Action);
                                    if (!string.IsNullOrEmpty(monitorStatus) &&
                                        (monitorStatus.ContainsIgnoringCase("unable to connect") ||
                                         monitorStatus.ContainsIgnoringCase("server unavailable") ||
                                         monitorStatus.ContainsIgnoringCase("returned an error")))
                                        monitorStatus = SERVICE_STOPPED;

                                    grpService.WindowsServiceStatus = grpService.Schedule_Status == OPEN
                                        ? ServiceSkipped
                                        : grpService.WindowsServiceStatus;

                                    monitorStatus = grpService.Schedule_Status == OPEN
                                        ? ServiceSkipped
                                        : monitorStatus;

                                    mailEntiry.WindowsServices += "<tr>";
                                    mailEntiry.WindowsServices += "<td  style=" + bodyStyle + ">" +
                                                                  grpService.HostIP + "</td> <td  style=" +
                                                                  bodyStyleCenter + ">" + grpService.Port +
                                                                  "</td> <td  style=" + bodyStyle + ">" +
                                                                  grpService.WindowsServiceName +
                                                                  "</td> <td  style=" + bodyStyleCenter + ">" +
                                                                  grpService.Schedule_UpdatedTime +
                                                                  "</td> <td  style=" + bodyStyleCenter + ">" +
                                                                  grpService.WindowsServiceStatus + "</td>" +
                                                                  "<td  style=" + bodyStyle + ">" +
                                                                  monitorStatus + "</td>";
                                    mailEntiry.WindowsServices += "</tr>";

                                    Logger.Log(mailEntiry.WindowsServices);

                                    if (processStatus == TIMEDOUT && grpService.Schedule_Status == OPEN)
                                    {
                                        Logger.Log(string.Format("Process skipped to {0} for {1} ",
                                            grpService.Schedule_Action, grpService.WindowsServiceName));

                                        grpService.Schedule_Status = NOACTION;

                                        if (grpService.Schedule_Action == "2" || grpService.Schedule_Action == "3")
                                            SetAcknowledge(grpService, "start");

                                        grpService.Schedule_UpdatedTime =
                                            (DateTime) (grpService.Group_Schedule_UpdatedDatetime = DateTime.Now);
                                        result = _winData.UpdateScheduleServiceStatus(grpService);
                                    }
                                }

                                mailEntiry.WindowsServices += "</table>";

                                processStatus = entity.ServiceDetails.Count == 0
                                    ? NOACTION
                                    : entity.ServiceDetails.All(_ => _.ProcessStatus == COMPLETED)
                                        ? COMPLETED
                                        : entity.ServiceDetails.All(_ => _.ProcessStatus == TIMEDOUT)
                                            ? TIMEDOUT
                                            : entity.ServiceDetails.All(
                                                _ => _.ProcessStatus == NOACTION || _.ProcessStatus == "")
                                                ? NOACTION
                                                : entity.ServiceDetails.Any(_ => _.ProcessStatus == TIMEDOUT)
                                                    ? TIMEDOUT
                                                    : UNSUCCESSFULL;
                                Logger.Log(string.Format("Process status: {0} ", processStatus));

                                //Send mail for env
                                mailEntiry.Env_ID = entity.Env_ID;
                                mailEntiry.Env_Name = entity.Env_Name;
                                mailEntiry.Group_Name = groupSd.Group_Name;
                                mailEntiry.Group_Schedule_Action = GetAction(groupSd.Group_Schedule_Action);
                                mailEntiry.Group_ActionCompletion_Status =
                                    string.IsNullOrEmpty(serviceResult.ErrorMessage)
                                        ? StatusCompletion[GetAction(groupSd.Group_Schedule_Action).ToLower()]
                                        : StatusCompletion[serviceResult.ErrorMessage];

                                //StatusCompletion[mailEntiry.Group_Schedule_Action.ToLower()];
                                mailEntiry.Group_Schedule_Timezone = LocalZone.StandardName.ToString();
                                mailEntiry.Group_Schedule_CreatedDatetime = groupSd.Group_Schedule_CreatedDatetime;
                                // String.Format("{0:dd/MMM/yyyy HH:mm:ss}", groupSD.Group_Schedule_Datatime);
                                mailEntiry.Group_Schedule_Datatime = groupSd.Group_Schedule_Datatime;
                                // String.Format("{0:dd/MMM/yyyy HH:mm:ss}", groupSD.Group_Schedule_Datatime);
                                mailEntiry.Group_Schedule_UpdatedDatetime = DateTime.Now;
                                // String.Format("{0:dd/MMM/yyyy HH:mm:ss}", DateTime.Now);
                                mailEntiry.Group_Schedule_CreatedBy =
                                    !string.IsNullOrEmpty(groupSd.Group_Schedule_CreatedBy)
                                        ? groupSd.Group_Schedule_CreatedBy
                                        : SystemUser;

                                var templateId = processStatus == COMPLETED || processStatus == NOACTION
                                    ? SERVICEOPERATIONSUCCESS
                                    : processStatus == TIMEDOUT &&
                                      entity.ServiceDetails.Count(_ => _.ServiceTypeShort == CM) > 1 &&
                                      entity.ServiceDetails.Any(_ => _.ProcessStatus == COMPLETED)
                                        ? ServiceOperationPartiallyTimedout
                                        : processStatus == TIMEDOUT &&
                                          entity.ServiceDetails.Count(_ => _.ServiceTypeShort == CM) > 1 &&
                                          entity.ServiceDetails.All(_ => _.ProcessStatus == TIMEDOUT)
                                            ? ServiceOperationTimedout
                                            : processStatus == TIMEDOUT &&
                                              entity.ServiceDetails.Count(
                                                  _ => _.ServiceTypeShort == CM && _.ProcessStatus == "T") == 1
                                                ? ServiceOperationTimedout
                                                : processStatus == TIMEDOUT &&
                                                  entity.ServiceDetails.Count(
                                                      _ => _.ServiceTypeShort == CM && _.ProcessStatus == "C") >= 1 &&
                                                  entity.ServiceDetails.Count > 1
                                                    ? ServiceOperationPartiallyTimedout
                                                    : ServiceOperationTimedout;
                                if (entity.ServiceDetails.All(_ => _.ServiceTypeShort == DISP) &&
                                    entity.ServiceDetails.Any(_ => _.ProcessStatus == "T"))
                                    templateId = ServiceOperationPartiallyTimedout;
                                else if (entity.ServiceDetails.All(_ => _.ServiceTypeShort == DISP) &&
                                         entity.ServiceDetails.All(_ => _.ProcessStatus == "T"))
                                    templateId = ServiceOperationTimedout;
                                else if (entity.ServiceDetails.All(_ => _.ServiceTypeShort == DISP) &&
                                         entity.ServiceDetails.All(_ => _.ProcessStatus == "C"))
                                {
                                    templateId = SERVICEOPERATIONSUCCESS;
                                }
                                Logger.Log(string.Format("Mail template: {0} ", templateId));
                                mailService.SendMail(mailEntiry.Env_ID, 0, templateId, mailEntiry);
                            }
                        }
                    }

                    groupSd.Group_Schedule_UpdatedBy = groupSd.Group_Schedule_CreatedBy;
                    groupSd.Group_Schedule_CompletedTime = groupSd.Group_Schedule_UpdatedDatetime = DateTime.Now;

                    if (string.IsNullOrEmpty(processStatus)) processStatus = NOACTION;

                    if (processStatus != COMPLETED)
                    {
                        Logger.Log("Service status " + processStatus == UNSUCCESSFULL
                            ? "Unsuccessful"
                            : processStatus == NOACTION
                                ? "No Action"
                                : "Successfull" + " which is in groupScheduleDetails loop");
                        groupSd.Group_Schedule_Status = processStatus;
                        resGroupSchedule = _winData.UpdateGroupScheduleStatus(groupSd);
                        Logger.Log(resGroupSchedule == 1
                            ? "Group schedule updated"
                            : "Group schedule failed to update");
                    }
                    else
                    {
                        //Update group data
                        groupSd.Group_Schedule_Status = processStatus;

                        Logger.Log("Updating Group schedule details after all service operation completes");

                        resGroupSchedule = _winData.UpdateGroupScheduleStatus(groupSd);
                        
                        Logger.Log(resGroupSchedule == 1
                            ? "Group schedule updated"
                            : "Group schedule failed to update");
                    }

                }
            }
            catch (Exception ex)
            {
                Logger.Log("Error GroupScheduleDetails:" + ex);
                throw;
            }
        }

        private static string GetProcessStatus(WindowServiceStatus serviceResult, GroupScheduleServiceDetailEntity grpService)
        {
            var processStatus = NOACTION;
            if (serviceResult.MonitorStatus == null)
                processStatus = string.IsNullOrEmpty(serviceResult.ErrorMessage)
                    ? NOACTION
                    : serviceResult.ErrorMessage;
            else
            {
                if (grpService.ServiceTypeShort == CM &&
                    (grpService.Schedule_Action.Contains("1") ||
                     grpService.Schedule_Action.Contains("3")))
                {
                    if (serviceResult.MonitorStatus.Contains(CmServiceStandby) ||
                        serviceResult.MonitorStatus.Contains(CmServiceRunning))
                    {
                        processStatus = COMPLETED;
                    }
                }
                else if (grpService.ServiceTypeShort == DISP &&
                         grpService.Schedule_Action.Contains("1") ||
                         grpService.Schedule_Action.Contains("3"))
                {
                    if (serviceResult.MonitorStatus.Contains(DispReady) ||
                        serviceResult.MonitorStatus.Contains(DispNotReady))
                    {
                        processStatus = COMPLETED;
                    }
                }
                else if (grpService.Schedule_Action.Contains("2"))
                {
                    if (serviceResult.MonitorStatus.Contains(SERVICE_STOPPED))
                        processStatus = COMPLETED;
                }
            }
            return processStatus;
        }

        private static bool ValidateToContinueProcess(string errorMessage, GroupScheduleServiceDetailEntity groupService)
        {
            var proceContinue = (errorMessage != NOACTION || errorMessage != TIMEDOUT) &&
                                ((groupService.Schedule_Action == "1") &&
                                 groupService.ServiceTypeShort == CM && groupService.Status == SERVICE_STOPPED ||
                                 (groupService.Schedule_Action == "3") &&
                                 groupService.ServiceTypeShort == CM && groupService.Status == SERVICE_STANDBY);
            return proceContinue;
        }

        private static string WaitTillStandByCmToGoActive(List<StandByService> standByServiceList,
            string action, string type)
        {
            var cmServiceStatus = "";
            if (standByServiceList.Any(service => !string.IsNullOrEmpty(service.Name)) && action == "3" && type == CM)
            {
                var processing = true;

                while (processing)
                {
                    foreach (var standByService in standByServiceList)
                    {
                        cmServiceStatus = GetRemoteServiceMonitorStatus(standByService.Host, standByService.Port, type == CM ? "C" : "D", action);
                        Logger.Log(string.Format("Monitor Status of {0}:{1}: {2}", standByService.Host, standByService.Port, cmServiceStatus));

                        if (!cmServiceStatus.ContainsIgnoringCase("standby") &&
                            cmServiceStatus.ContainsIgnoringCase("running"))
                        {
                            processing = false;
                            break;
                        }

                        Thread.Sleep(2000);
                    }

                }
            }
         
            return cmServiceStatus;
        }

        private static bool CheckCurrentStatus(string serviceName, string serviceAction, string systemName)
        {
            bool status = false;

            var result = GetWindowsServiceStatus(serviceName, systemName).ToLower();
            
            var action = GetAction(serviceAction).ToLower();
            if (action == "start")
            {
                if (result == SERVICE_RUNNING.ToLower() || result == SERVICE_STARTING.ToLower())
                {
                    return false;
                }
            }
            else if(action == "stop")
            {
                if (result == SERVICE_STOPPED.ToLower() || result == SERVICE_STOPPING.ToLower())
                {
                    return false;
                }
            }
            else if (action == "restart")
            {
                if (result != SERVICE_RUNNING.ToLower() && result != SERVICE_STARTING.ToLower())
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Get all open group scheduled services to process
        /// </summary>
        /// <returns></returns>
        public static List<GroupScheduleEntity> GetGroupOpenScheduleDetails()
        {
            List<GroupScheduleEntity> groupScheduleEntity;
            try
            {
                groupScheduleEntity = _winData.GetGroupOpenScheduleDetails(DateTime.Now, "O", "sch", 0, 0);
                if (groupScheduleEntity != null && groupScheduleEntity.Count > 0)
                {
                    foreach (var grpSchedule in groupScheduleEntity)
                    {
                        var groupScheduleDetailEntity = _winData.GetGroupOpenScheduleEnvDetails(DateTime.Now, "O", "env", grpSchedule.Group_Schedule_ID, 0);
                        if (groupScheduleDetailEntity == null || groupScheduleDetailEntity.Count <= 0) continue;
                        grpSchedule.GroupScheduleDetails = new List<GroupScheduleDetailEntity>();
                        foreach (var gsd in groupScheduleDetailEntity)
                        {
                            var groupScheduleServiceDetailEntity = _winData.GetGroupOpenScheduleServiceDetails(DateTime.Now, "O", "cfg", grpSchedule.Group_Schedule_ID, gsd.Env_ID);
                            if (groupScheduleServiceDetailEntity != null && groupScheduleServiceDetailEntity.Count > 0)
                            {
                                //Get monitor service and update
                                var monitorService = new MonitorService();
                                var monitorList = monitorService.GetAllMonitors(0, false);

                                gsd.ServiceDetails = new List<GroupScheduleServiceDetailEntity>();
                                var gsd1 = gsd;
                                foreach (var det in groupScheduleServiceDetailEntity.Where(det => det.Env_ID == gsd1.Env_ID))
                                {
                                    det.WindowsServiceStatus = GetWindowsServiceStatus(det.WindowsServiceName,
                                        det.HostIP);

                                    if (((grpSchedule.Group_Schedule_Action == "3" ||
                                          grpSchedule.Group_Schedule_Action == "2") &&
                                         det.WindowsServiceStatus.ContainsIgnoringCase("stopped")) ||
                                        (grpSchedule.Group_Schedule_Action == "1" &&
                                         (det.WindowsServiceStatus.ContainsIgnoringCase("running") ||
                                          det.WindowsServiceStatus.ContainsIgnoringCase("starting"))))
                                    {
                                        var groupSchedule = new GroupScheduleServiceDetailEntity
                                        {
                                            Env_ID = det.Env_ID,
                                            Config_ID = det.Config_ID,
                                            Group_ActionCompletion_Status = det.WindowsServiceStatus,
                                            Env_Name = gsd.Env_Name, // det.Env_Name,
                                            Group_Name = grpSchedule.Group_Name,
                                            Service_Name = det.Service_Name,
                                            HostIP = det.HostIP,
                                            Port = det.Port,
                                            Group_Schedule_CreatedDatetime = det.Group_Schedule_CreatedDatetime,
                                            Group_Schedule_Action = GetAction(grpSchedule.Group_Schedule_Action),
                                            Group_Schedule_StartedTime = DateTime.Now,
                                            Group_Schedule_UpdatedDatetime = DateTime.Now,
                                            Schedule_UpdatedTime = DateTime.Now,
                                            Group_Schedule_CreatedBy =
                                                !string.IsNullOrEmpty(grpSchedule.Group_Schedule_CreatedBy)
                                                    ? grpSchedule.Group_Schedule_CreatedBy
                                                    : SystemUser,
                                            Group_Schedule_Detail_ID = det.Group_Schedule_Detail_ID,
                                            Schedule_Status = "N",
                                            Schedule_Action = grpSchedule.Group_Schedule_Action,
                                            Status = det.WindowsServiceStatus,

                                        };
                                        var result = _winData.UpdateScheduleServiceStatus(groupSchedule);

                                        mailService.SendMail(det.Env_ID, det.Config_ID, SERVICEOPERATIONUNSUCCESS,
                                            groupSchedule);
                                    }
                                    else
                                    {
                                        gsd.ServiceDetails.Add(det);
                                        SetAcknowledge(det, "stop");
                                    }
                                }

                                if (monitorList != null && monitorList.Count > 0)
                                {
                                    var mailEntity = new GroupScheduleServiceMailEntity
                                    {
                                        Group_Schedule_StartedTime = DateTime.Now,
                                        Env_ID = gsd.Env_ID,
                                        Env_Name = gsd.Env_Name,
                                        Group_Name = grpSchedule.Group_Name,
                                        Group_Schedule_CreatedDatetime = grpSchedule.Group_Schedule_CreatedDatetime,
                                        Group_Schedule_Datatime = grpSchedule.Group_Schedule_Datatime,
                                        Group_Schedule_Action = grpSchedule.Group_Schedule_Action == "1" ? "Start" : grpSchedule.Group_Schedule_Action == "2" ? "Stop" : "Restart",
                                        Group_Schedule_CreatedBy = grpSchedule.Group_Schedule_CreatedBy,
                                        Group_Schedule_Timezone = LocalZone.StandardName.ToString(CultureInfo.InvariantCulture),
                                        WindowsServices = "<table style=" + tableStyle + ">"
                                    };
                                    mailEntity.WindowsServices += "<tr><th style=" + headerStyle + ">Server</th> <th style=" + headerStyle + ">Port</th> <th  style=" + headerStyle + ">Service Type</th> <th  style=" + headerStyle + ">Service Name</th></tr>";

                                    foreach (var service in gsd.ServiceDetails)
                                    {
                                        //GroupScheduleServiceDetailEntity
                                        var serviceMoniterEntity
                                            = monitorList.FirstOrDefault(ml => ml.EnvID == service.Env_ID);
                                        if (serviceMoniterEntity == null) continue;
                                        var firstOrDefault = serviceMoniterEntity
                                            .monitorList.FirstOrDefault(con => con.ConfigID == service.Config_ID);
                                        if (firstOrDefault !=
                                            null)
                                            service.Status =
                                                firstOrDefault
                                                    .MonitorStatus;

                                        mailEntity.WindowsServices += "<tr><td  style=" + tdHost + ">" + service.HostIP + "</td> <td  style=" + bodyStyleCenter + ">" + service.Port + "</td> <td  style=" + bodyStyle + ">" + service.Service_Name + "</td> <td  style=" + bodyStyle + ">" + service.WindowsServiceName + "</td></tr>";
                                    }

                                    mailEntity.WindowsServices += "</table>";

                                    if (gsd.ServiceDetails.Count > 0)
                                        mailService.SendMail(mailEntity.Env_ID, 0, SERVICEOPERATIONSTARTED, mailEntity);
                                }
                            }

                            gsd.ServiceDetails = GetServiceSequence(gsd.ServiceDetails,
                                    grpSchedule);
                            
                            grpSchedule.GroupScheduleDetails.Add(gsd);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex.ToString());
                throw;
            }

            return groupScheduleEntity;
        }

        private static List<GroupScheduleServiceDetailEntity> GetServiceSequence(ICollection<GroupScheduleServiceDetailEntity> serviceDetails, GroupScheduleEntity groupSchedule)
        {
            var sequenceServiceList = new List<GroupScheduleServiceDetailEntity>();
            if (serviceDetails == null || serviceDetails.Count <= 0)
                return sequenceServiceList;


            switch (groupSchedule.Group_Schedule_Action)
            {
                case "1":

                    if (serviceDetails.Any(_ => _.ServiceTypeShort.Equals(CM)))
                    {
                        sequenceServiceList.Add(serviceDetails.FirstOrDefault(_ => _.ServiceTypeShort.Equals(CM)));
                    }
                    if (serviceDetails.Any(_ => _.ServiceTypeShort.Equals(DISP)))
                    {
                        sequenceServiceList.AddRange(serviceDetails.Where(_ => _.ServiceTypeShort.Equals(DISP)));
                    }

                    if (serviceDetails.Any(_ => _.ServiceTypeShort.Equals(CM)) &&
                        serviceDetails.Count(_ => _.ServiceTypeShort.Equals(CM)) > 1)
                    {
                        sequenceServiceList.AddRange(
                            serviceDetails.Where(
                                _ =>
                                {
                                    var groupScheduleServiceDetailEntity =
                                        sequenceServiceList.FirstOrDefault(con => con.ServiceTypeShort.Equals(CM));
                                    return groupScheduleServiceDetailEntity != null &&
                                           (_.ServiceTypeShort.Equals(CM) &&
                                            _.Config_ID !=
                                            groupScheduleServiceDetailEntity
                                                .Config_ID);
                                }));
                    }

                    sequenceServiceList = UpdateScheduleAction(sequenceServiceList, groupSchedule.Group_Schedule_Action);
                    break;

                case "2":
                    if (serviceDetails.Any(_ => _.ServiceTypeShort.Equals(DISP)))
                    {
                        sequenceServiceList.AddRange(serviceDetails.Where(_ => _.ServiceTypeShort.Equals(DISP)));
                    }

                    if (serviceDetails.Any(_ => _.ServiceTypeShort.Equals(CM) && _.Status.Equals(SERVICE_STANDBY)))
                    {
                        sequenceServiceList.AddRange(
                            serviceDetails.Where(_ => _.ServiceTypeShort.Equals(CM) && _.Status.Equals(SERVICE_STANDBY)));
                    }
                    if (serviceDetails.Any(_ => _.ServiceTypeShort.Equals(CM) && !_.Status.Equals(SERVICE_STANDBY)))
                    {
                        sequenceServiceList.AddRange(
                            serviceDetails.Where(_ => _.ServiceTypeShort.Equals(CM) && !_.Status.Equals(SERVICE_STANDBY)));
                    }
                    
                    sequenceServiceList = UpdateScheduleAction(sequenceServiceList, groupSchedule.Group_Schedule_Action);
                    break;

                case "3":
                    var sequenceServiceRestartList = new List<GroupScheduleServiceDetailEntity>();
                    if (serviceDetails.Any(_ => _.ServiceTypeShort.Equals(CM)) &&
                        serviceDetails.Count(_ => _.ServiceTypeShort.Equals(CM)) == 1)
                    {
                        //Stop
                        if (serviceDetails.Any(_ => _.ServiceTypeShort.Equals(DISP)))
                        {
                            sequenceServiceRestartList.AddRange(
                                serviceDetails.Where(_ => _.ServiceTypeShort.Equals(DISP))
                                    .Select(source => source.ShallowCopy()));
                        }

                        var stopCopy = serviceDetails.Where(
                                _ => _.ServiceTypeShort.Equals(CM)).ToList()[0].ShallowCopy();
                        sequenceServiceRestartList.Add(stopCopy);


                        sequenceServiceRestartList = UpdateScheduleAction(sequenceServiceRestartList, "2");
                        sequenceServiceList.AddRange(sequenceServiceRestartList);

                        //Start
                        sequenceServiceRestartList = new List<GroupScheduleServiceDetailEntity>();
                        if (serviceDetails.Any(_ => _.ServiceTypeShort.Equals(CM)))
                        {

                            var startCopy = serviceDetails.Where(
                                _ => _.ServiceTypeShort.Equals(CM)).ToList()[0].ShallowCopy();
                            sequenceServiceRestartList.Add(startCopy);
                        }
                        if (serviceDetails.Any(_ => _.ServiceTypeShort.Equals(DISP)))
                        {
                            sequenceServiceRestartList.AddRange(
                                serviceDetails.Where(_ => _.ServiceTypeShort.Equals(DISP))
                                    .Select(source => source.ShallowCopy()));
                        }

                        sequenceServiceRestartList = UpdateScheduleAction(sequenceServiceRestartList, "1");
                        sequenceServiceList.AddRange(sequenceServiceRestartList);
                    }
                    else if (serviceDetails.Any(_ => _.ServiceTypeShort.Equals(CM)) &&
                             serviceDetails.Count(_ => _.ServiceTypeShort.Equals(CM)) >= 1)
                    {
                        //Restart
                        if (
                            serviceDetails.Any(
                                _ =>
                                    _.ServiceTypeShort.Equals(CM) &&
                                    (_.Status.Equals(SERVICE_STANDBY) ||
                                     _.Status.ContainsIgnoringCase(SERVICE_NOT_RUNNING))))
                        {
                            sequenceServiceRestartList.AddRange(
                                serviceDetails.Where(
                                    _ =>
                                        _.ServiceTypeShort.Equals(CM) &&
                                        (_.Status.Equals(SERVICE_STANDBY) ||
                                         _.Status.ContainsIgnoringCase(SERVICE_NOT_RUNNING))));
                        }
                        if (serviceDetails.Any(_ => _.ServiceTypeShort.Equals(DISP)))
                        {
                            sequenceServiceRestartList.AddRange(
                                serviceDetails.Where(_ => _.ServiceTypeShort.Equals(DISP)));
                        }
                        //sequenceServiceList.AddRange(sequenceServiceRestartList);
                        sequenceServiceList = UpdateScheduleAction(sequenceServiceRestartList,
                            groupSchedule.Group_Schedule_Action);

                        //Stop
                        if (serviceDetails.Any(_ => _.ServiceTypeShort.Equals(CM) && _.Status.Equals(SERVICE_RUNNING)))
                        {
                            var stopCopy = serviceDetails.Where(
                                _ => _.ServiceTypeShort.Equals(CM) && _.Status.Equals(SERVICE_RUNNING)).ToList()[0]
                                .ShallowCopy();

                            stopCopy.Schedule_Action = "2";

                            var startCopy = serviceDetails.Where(
                                _ => _.ServiceTypeShort.Equals(CM) && _.Status.Equals(SERVICE_RUNNING)).ToList()[0]
                                .ShallowCopy();

                            startCopy.Schedule_Action = "1";

                            sequenceServiceList.Add(stopCopy);
                            sequenceServiceList.Add(startCopy);

                        }

                        //Start
                        //if (serviceDetails.Any(_ => _.ServiceTypeShort.Equals(CM) && _.Status.Equals(SERVICE_RUNNING)))
                        //{
                        //    var list = serviceDetails.Where(
                        //        _ => _.ServiceTypeShort.Equals(CM) && _.Status.Equals(SERVICE_RUNNING)).ToList()[0];
                        //    list.Schedule_Action = "1";
                        //    //sequenceServiceRestartList.Add(list);
                        //    sequenceServiceList.Add(list);
                        //}
                    }
                    else if (serviceDetails.Any(_ => _.ServiceTypeShort.Equals(DISP)))
                    {
                        sequenceServiceList.AddRange(
                            serviceDetails.Where(_ => _.ServiceTypeShort.Equals(DISP)));
                        sequenceServiceList = UpdateScheduleAction(sequenceServiceList, groupSchedule.Group_Schedule_Action);
                    }

                    break;
            }

            return sequenceServiceList;
        }

        private static List<GroupScheduleServiceDetailEntity> UpdateScheduleAction(ICollection<GroupScheduleServiceDetailEntity> serviceDetails, string action)
        {
            var serviceDetailsTemp = new List<GroupScheduleServiceDetailEntity>();
            foreach (var serviceDetailsEntity in serviceDetails)
            {
                var detail = serviceDetailsEntity;
                detail.Schedule_Action = action;
                serviceDetailsTemp.Add(serviceDetailsEntity);
            }
            return serviceDetailsTemp;
        }

        #endregion Get all Group Schedule scrvice details

        #region GetAction
        /// <summary>
        /// To return the action name based on the action number passed
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        private static string GetAction(string action)
        {
            string actionName = string.Empty;
            if (action == Convert.ToString(Convert.ToInt32( ScheduleAction.Start)))
                actionName = ScheduleAction.Start.ToString();
            else if (action == Convert.ToString(Convert.ToInt32(ScheduleAction.Stop)))
                actionName = ScheduleAction.Stop.ToString();
            else if (action == Convert.ToString(Convert.ToInt32(ScheduleAction.Restart)))
                actionName = ScheduleAction.Restart.ToString();

            return actionName;
        }
        #endregion GetAction

        private static T GetWebResponse<T>(IWebHttpRequestWrapper request)
        {
            using (var response = request.GetResponse())
            {
                var statusCode = response.StatusCode;

                if (statusCode != HttpStatusCode.OK)
                    throw new ApplicationException(response.StatusDescription);

                string resultText;

                using (var responseStream = response.GetResponseStream())
                {
                    using (var reader = new StreamReader(responseStream))
                    {
                        resultText = reader.ReadToEnd();
                    }
                }
                var javaScriptSerializer = new JavaScriptSerializer();
                var responseRext = javaScriptSerializer.Deserialize<T>(resultText);
                return responseRext;
            }
        }

        private static string GetRemoteWindowsServiceStatus(string serviceName, string systemName)
        {
            var uri = string.Format(RemoteSystemURLForServiceStatus + "/GetStatus?svn={1}&srn={0}", systemName, serviceName);
            var webHttpRequestBuilder = new WebHttpRequestBuilder();
            var request = webHttpRequestBuilder.CreateGetRequest(uri);
            var response = GetWebResponse<WindowServiceStatus>(request);
            return response.ServiceStatus;
        }

        private static string GetRemoteServiceMonitorStatus(string systemName, string port, string type, string action)
        {
            var uri =
                string.Format(RemoteSystemURLForServiceStatus + "/GetMonitorStatus?srn={0}&p={1}&t={2}&a={3}&tm={4}",
                    systemName, port, type, action, 300);
            var webHttpRequestBuilder = new WebHttpRequestBuilder();
            var request = webHttpRequestBuilder.CreateGetRequest(uri);
            var response = GetWebResponse<string>(request);
            return response.ToString();
        }

        private static void SetAcknowledge(GroupScheduleServiceDetailEntity detailsServiceDetailEntity, string mode)
        {
            var setServiceAck = new AcknowledgeEntity
            {
                EnvId = detailsServiceDetailEntity.Env_ID,
                ConfigId = detailsServiceDetailEntity.Config_ID,
                MonId = 0,
                IsAcknowledgeMode = true,
                AcknowledgeAlertChange = mode,
                AcknowledgeComments = "Scheduled service operation",
                CreatedBy = SystemUser,
                CreatedDate = DateTime.Now
            };
            Logger.Log(string.Format("Acknowledge to '{0}' for Env id '{1}', Config Id '{2}', Host '{3}' and Port '{4}' from service operation", mode, detailsServiceDetailEntity.Env_ID, detailsServiceDetailEntity.Config_ID, detailsServiceDetailEntity.HostIP, detailsServiceDetailEntity.Port));
            Logger.Log("Acknowledge Comments: " + setServiceAck.AcknowledgeComments);
            _winData.InsUpdateServiceAcknowledge(setServiceAck, string.Empty);
        }

        #region Get Server Performance

        public static void ServerPerformanceSchedule()
        {
            try
            {
                var serverschedules = _monitorData.GetServerPerformanceSchedules(0);

                if (serverschedules == null || serverschedules.Count <= 0)
                {
                    return;
                }

                var schedules = serverschedules.Where(ns => ns.NextJobRunTime <= DateTime.Now).GroupBy(_ => _.HostIp).Select(g => g.First()).ToList();

                if (schedules.Count <=0) return;

                Logger.Log("Monitor Server Performance begins");
                foreach (var serverSchedule in schedules.Where(_ => _.NextJobRunTime <= DateTime.Now))
                {
                    Logger.Log($"Getting Server details for {serverSchedule.HostIp}");
                    var serverDetails = GetServerPerformance(serverSchedule.HostIp);

                    if (serverDetails?.Performance?.Drives == null || serverDetails.Performance.Drives.Count <= 0)
                    {
                        Logger.Log($"Unable to get server performance details for {serverSchedule.HostIp}");
                        continue;
                    }
                    serverDetails.Performance.EnvId = serverSchedule.EnvId;
                    serverDetails.Performance.ConfigId = serverSchedule.ConfigId;
                    serverDetails.Performance.HostIp = serverSchedule.HostIp;

                    var perfId = _monitorData.SetServerPerformance(serverDetails.Performance);
                    if (string.IsNullOrEmpty(perfId))
                    {
                        Logger.Log($"Unable to update data for {serverDetails.Performance.HostIp}");
                        continue;
                    }

                    foreach (var drive in serverDetails.Performance.Drives)
                    {
                        _monitorData.SetServerPerformanceDrive(drive, Convert.ToInt32(perfId));
                    }

                    serverSchedule.LastJobRunTime = DateTime.Now;
                    serverSchedule.NextJobRunTime =
                        DateTime.Now.AddMinutes(Convert.ToInt32(MonitorServerPerformanceInterval));

                    _monitorData.SetServerPerformanceSchedule(serverSchedule);
                }

                Logger.Log("Monitor Server Performance Ends");
            }
            catch (Exception exception)
            {
                Logger.Log(exception.ToString());
                throw;
            }
        }

        private static PerformanceResponse GetServerPerformance(string systemName)
        {
            var uri =
                string.Format(RemoteSystemURLForServer + "/GetServerDetail?srn={0}",
                    systemName);
            var webHttpRequestBuilder = new WebHttpRequestBuilder();
            var request = webHttpRequestBuilder.CreateGetRequest(uri);
            var response = GetWebResponse<PerformanceResponse>(request);
            return response;
        }
        #endregion Get Server Performance
    }
}
