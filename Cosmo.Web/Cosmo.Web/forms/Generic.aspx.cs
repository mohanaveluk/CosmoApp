using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net.NetworkInformation;
using System.ServiceProcess;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Security;
using Cosmo.Entity;
using Cosmo.Service;

namespace Cosmo.Web.forms
{
    public partial class Generic : System.Web.UI.Page
    {

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

        private const string INCODENT_TYPE = "pnd";

        private static string LocationUnavailableError_Message = ConfigurationManager.AppSettings["LocationUnavailableError"].ToString();
        private static string NoServiceAvailable_Message = ConfigurationManager.AppSettings["NoServiceAvailable"].ToString();
        private static string AllServiceScheduled_Message = ConfigurationManager.AppSettings["AllServiceScheduled"].ToString();
        private static string NoServiceScheduled_Message = ConfigurationManager.AppSettings["NoServiceScheduled"].ToString();
        private static string UnableToScheduleService_Message = ConfigurationManager.AppSettings["UnableToScheduleService"].ToString();
        private static string NOENVIRONMENT = ConfigurationManager.AppSettings["NoEnvironmentAvailable"].ToString();
        private static string RemoteSystemURLForServiceStatus = ConfigurationManager.AppSettings["RemoteSystemURLForServiceStatus"].ToString();

        #endregion Constants

        #region ogjects
        static EnvironmentService enviService = new EnvironmentService();
        static WinService winService = new WinService();
        static JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
        static UserService userService = new UserService();
        static MonitorService monitorService = new MonitorService();
        static ReportService reportService = new ReportService();
        #endregion ogjects

        #region variables
        static string fileName = @AppDomain.CurrentDomain.BaseDirectory + @"ServiceControl\WinServiceControl.exe";
        static string fileNameServiceStatus = @AppDomain.CurrentDomain.BaseDirectory + @"Status\RemoteServiceStatus.exe";
        static int serviceTimeout = Convert.ToInt32(ConfigurationManager.AppSettings["WindowsServiceTimeout"]);
        private static string CONTENT_SERVICE = ConfigurationManager.AppSettings["ContentService"].ToString();
        private static string DESPATCHER_SERVICE = ConfigurationManager.AppSettings["DispatcherService"].ToString();

        #endregion variables

        public Generic()
        {
            

        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        [System.Web.Services.WebMethod(Description = "email", EnableSession = true)]
        public static void DeleteEnvConfig(string type, string configID)
        {
            enviService.DeleteRecord(type, configID);
        }

        [System.Web.Services.WebMethod]
        public static void DeleteeMailFromSession(string type, string configID)
        {
            if (type == "email")
            {
                if (HttpContext.Current.Session["emailList"] == null) return;
                var emailconfigList = (List<ConfigEmailsEntity>)HttpContext.Current.Session["emailList"];
                if (emailconfigList != null && emailconfigList.Count > 0)
                {
                    if (emailconfigList.Any(el => el.EmailAddress == configID))
                    {
                        emailconfigList.RemoveAll(el => el.EmailAddress == configID);
                    }
                }
            }
        }

        [System.Web.Services.WebMethod(Description = "emial", EnableSession = true)]
        public static void DeleteEnvEmail(string type, string emailAddress)
        {
            if (type == "email")
            {
                if (HttpContext.Current.Session["emailList"] != null)
                {
                    List<ConfigEmailsEntity> emailconfigList = new List<ConfigEmailsEntity>();
                    emailconfigList = (List<ConfigEmailsEntity>)HttpContext.Current.Session["emailList"];
                    if (emailconfigList.Any(ml => ml.EmailAddress == emailAddress))
                    {
                        //ConfigEmailsEntity mail = new ConfigEmailsEntity();
                        //mail.EmailAddress = emailAddress;
                        //emailconfigList.Remove(mail);
                        emailconfigList.RemoveAll(ml => ml.EmailAddress == emailAddress);
                    }

                    HttpContext.Current.Session["emailList"] = emailconfigList;
                }
            }
        }

        [System.Web.Services.WebMethod]
        public static void CancelGroupSchedule(string type, string groupScheduleId)
        {
            if (!string.IsNullOrEmpty(groupScheduleId))
            {
                winService.CancelGroupSchedule(type, groupScheduleId);
            }
        }

        [System.Web.Services.WebMethod]
        public static int InsUpdateWindowsService(string serviceName, string comments, string envID, string configID)
        {
            int rowAffected = enviService.InsUpdateWindowsService(serviceName, comments, Convert.ToInt32(envID), Convert.ToInt32(configID));
            return rowAffected;
        }


        [System.Web.Services.WebMethod]
        public static int InsUpdateGroupDetail(string sEnvID, string sGroupID, string sConfigIDs)
        {
            int rowAffected = winService.InsUpdateGroupDetail(Convert.ToInt32(sEnvID), Convert.ToInt32(sGroupID), sConfigIDs);
            return rowAffected;
            /*if (rowAffected == 0)
            {
                List<GroupDetailEntity> grpDetail = new List<GroupDetailEntity>();
                grpDetail = winService.GetGroupDetail(Convert.ToInt32(grp_ID), Convert.ToInt32(env_ID));
                return grpDetail;
            }*/
        }

        /// <summary>
        /// to update the issue and solution details in order to track the incident
        /// </summary>
        [System.Web.Services.WebMethod]
        public static void IncidentTracking1(string monitorId, string environmentId, string serviceId, string serviceIssue, string serviceSolution)
        {
        }

        [System.Web.Services.WebMethod]
        public static void IncidentTracking(string monitorId, string environmentId, string serviceId, string serviceIssue, string serviceSolution)
        {
            int insertRec = 0;
            char[] delimiter1 = new char[] { '^' };
            string[] mID;
            string[] sIssue;
            string[] sSolution;

            try
            {

                var monitorService = new MonitorService();
                var userId = HttpContext.Current.Session["_LOGGED_USERD_ID"].ToString();
                if (monitorId.Contains("^"))
                {
                    mID = monitorId.Split(delimiter1);
                    sIssue = serviceIssue.Split(delimiter1);
                    sSolution = serviceSolution.Split(delimiter1);
                    for (var iItem = 0; iItem < mID.Length - 1; iItem++)
                    {
                        insertRec = monitorService.InsIncidentTracking(mID[iItem], environmentId, serviceId, sIssue[iItem], sSolution[iItem], userId);
                    }
                }
                else
                    insertRec = monitorService.InsIncidentTracking(monitorId, environmentId, serviceId, serviceIssue, serviceSolution, userId);
            }
            catch (Exception ex)
            {
                Logger.Log(ex.ToString());
                if (ex.InnerException != null)
                {
                    Logger.Log(ex.InnerException.Message);
                }
            }
        }

        [System.Web.Services.WebMethod]
        public static string GetPingServer(string host)
        {
            string serverStatus = string.Empty;

            Ping P = new Ping();
            PingReply reply = null;
            try
            {
                reply = P.Send(host, 3000);
                if (reply != null && reply.Status == IPStatus.Success)
                {
                    serverStatus = reply.Status.ToString() + " : Server responded in " + reply.RoundtripTime + " ms";
                }
                else if (reply != null)
                {
                    serverStatus = "Failure: " + reply.Status;
                }
            }
            catch (PingException pingExc)
            {
                serverStatus = "Error: " + pingExc.Message;
            }
            catch (ObjectDisposedException objDisposedExc)
            {
                serverStatus = "Object Disposed Exception: " + objDisposedExc.Message;
            }
            catch (ArgumentNullException argNullExc)
            {
                serverStatus = "Argument Null Exception: " + argNullExc.Message;
            }
            catch (ArgumentOutOfRangeException argOutOfRangeExc)
            {
                serverStatus = "Argument Out Of Range Exception: " + argOutOfRangeExc.Message;
            }
            catch (InvalidOperationException invOperationExc)
            {
                serverStatus = "Invalid Operation Exception: " + invOperationExc.Message;
            }

            return serverStatus;
        }

        [System.Web.Services.WebMethod]
        public static string GetGroupDetail(string grp_ID, string env_ID)
        {
            List<GroupDetailEntity> grpDetail = new List<GroupDetailEntity>();
            grpDetail = winService.GetGroupDetail(Convert.ToInt32(grp_ID), Convert.ToInt32(env_ID));
            var json = javaScriptSerializer.Serialize(grpDetail);
            return json;
        }

        [System.Web.Services.WebMethod]
        public static List<GroupDetailEntity> GetGroupDetails(string grpId, string envId)
        {
            var groupDetail = new List<GroupDetailEntity>();
            try
            {
                groupDetail = winService.GetGroupDetail(Convert.ToInt32(grpId), Convert.ToInt32(envId));
                if (groupDetail != null && groupDetail.Count > 0)
                {
                    groupDetail = groupDetail
                        .OrderByDescending(
                            ml =>
                                (!String.Equals(ml.MonitorStatus, SERVICE_RUNNING, StringComparison.CurrentCultureIgnoreCase) &&
                                 !String.Equals(ml.MonitorStatus, SERVICE_STANDBY, StringComparison.CurrentCultureIgnoreCase)) || (ml.MonitorStatus != null))
                        .ThenBy(
                            ml => String.Equals(ml.MonitorStatus, SERVICE_RUNNING, StringComparison.CurrentCultureIgnoreCase) || (ml.MonitorStatus != null))
                        .ThenBy(
                            ml => String.Equals(ml.MonitorStatus, SERVICE_STANDBY, StringComparison.CurrentCultureIgnoreCase) || (ml.MonitorStatus != null))
                        .ToList();
                }

                return groupDetail;
            }
            catch (Exception ex)
            {
                if (groupDetail != null)
                {
                    groupDetail.Add(new GroupDetailEntity {Comments = "Error: " + ex.Message});
                }
                return groupDetail;
            }
        }

        #region Insert group schedule details
        [System.Web.Services.WebMethod]
        public static string InsUpdateGroupSchedule(string groupId, string groupName, string envID, string configID, string winServiceID, string serviceAction, string actionDateTime, string requestSource)
        {
            var json = "";
            var commonService = new CommonService();
            if (HttpContext.Current.Session == null) commonService.Logout();
                
            try
            {
                List<GroupScheduleEntity> groupSchedule = new List<GroupScheduleEntity>();
                int result = 0;
                result = winService.InsGroupScheduleDetail(Convert.ToInt32(groupId), groupName.Trim(), envID, configID,
                    winServiceID, serviceAction, Convert.ToDateTime(actionDateTime), requestSource);
                if (result > 0)
                {
                    groupSchedule = GetAllGroupSchedules(0, 0, DateTime.Now, "O");
                    json = javaScriptSerializer.Serialize(groupSchedule);
                }
            }
            catch (Exception ex)
            {

                json = ex.Message;
            }
            return json;
        }
        #endregion Insert group schedule details

        #region Get all groups schedule details
        /// <summary>
        /// Get all the group schedules which are based on the start date time provided
        /// </summary>
        /// <param name="grp_ID"></param>
        /// <returns></returns>
        [System.Web.Services.WebMethod]
        public static List<GroupScheduleEntity> GetAllGroupSchedules(int grp_ID, int grp_sch_ID, DateTime startTime, string status)
        {
            List<GroupScheduleEntity> groupSchedule = new List<GroupScheduleEntity>();
            try
            {
                groupSchedule = winService.GetAllGroupSchedules(Convert.ToInt32(grp_ID), Convert.ToInt32(grp_sch_ID), Convert.ToDateTime(startTime), status);
            }
            catch (Exception ex)
            {
                Logger.Log(ex.ToString());
                throw;
            }

            return groupSchedule;
        }
        #endregion Get all groups schedule details

        /// <summary>
        /// Get all schedule based on the status and start time passed from ajax call
        /// </summary>
        /// <param name="groupID"></param>
        /// <param name="grp_sch_ID"></param>
        /// <param name="startTime"></param>
        /// <param name="serviceStatus"></param>
        /// <returns></returns>
        [System.Web.Services.WebMethod]
        public static string GetOpenGroupSchedule1(string groupID, string grp_sch_ID, string startTime, string serviceStatus)
        {
            List<GroupScheduleEntity> groupSchedule = new List<GroupScheduleEntity>();
            groupSchedule = GetAllGroupSchedules(Convert.ToInt32(groupID), Convert.ToInt32(grp_sch_ID), Convert.ToDateTime(startTime), serviceStatus);
            var json = javaScriptSerializer.Serialize(groupSchedule);
            return json;
        }

        [System.Web.Services.WebMethod]
        public static List<GroupScheduleEntity> GetOpenGroupSchedule(string groupID, string grp_sch_ID, string startTime, string serviceStatus)
        {
            var groupSchedule = new List<GroupScheduleEntity>();
            groupSchedule = GetAllGroupSchedules(Convert.ToInt32(groupID), Convert.ToInt32(grp_sch_ID), DateTime.Now, serviceStatus);
            return groupSchedule;
        }

        #region windows service operation
        [System.Web.Services.WebMethod]
        public static string GetWindowssService(string serviceName, string serviceMode, string systemName, string serviceId, string requestSource)
        {
            
            var loggedUserId = (HttpContext.Current.Session != null &&
                                HttpContext.Current.Session["_LOGGED_USERD_ID"] != null)
                ? Convert.ToInt32(HttpContext.Current.Session["_LOGGED_USERD_ID"])
                : 1;

            return winService.GetWindowssService(serviceName, serviceMode, systemName, serviceId, requestSource, loggedUserId);

            /*
            string result = string.Empty; string currentStatus = string.Empty;
            bool isTimeout = true;
            bool isProcessing = true;
            string serviceAction = string.Empty;
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            CancellationToken token = cancellationTokenSource.Token;

            try
            {
                if (serviceMode == "1" || serviceMode.ToLower(CultureInfo.CurrentCulture) == "start")
                {
                    serviceAction = "start";
                    currentStatus = SERVICE_RUNNING;
                }
                else if (serviceMode == "2" || serviceMode.ToLower(CultureInfo.CurrentCulture) == "stop")
                {
                    serviceAction = "stop";
                    currentStatus = SERVICE_STOPPED;
                }
                else if (serviceMode == "3" || serviceMode.ToLower(CultureInfo.CurrentCulture) == "restart")
                    serviceAction = "restart";

                if (!string.IsNullOrEmpty(serviceAction) && !string.IsNullOrEmpty(serviceName))
                {
                    var loggedUserId = (HttpContext.Current.Session["_LOGGED_USERD_ID"] != null)
                        ? HttpContext.Current.Session["_LOGGED_USERD_ID"].ToString()
                        : "1";
                    var serviceOnDemand = winService.GetServiceConfigurationOndemand(Convert.ToInt32(serviceId));
                    
                    if (serviceOnDemand != null && serviceOnDemand.Count > 0)
                    {
                        serviceOnDemand[0].RequestStatus = serviceAction.ToTitleCase();
                        serviceOnDemand[0].RequestedDateTime = DateTime.Now;
                        serviceOnDemand[0].OnDemand = true;
                        serviceOnDemand[0].RequestSource = requestSource;

                        var t = Task.Factory.StartNew(() =>
                        {
                            try
                            {
                                var uri =
                                    string.Format(
                                        RemoteSystemURLForServiceStatus + "{3}?svn={1}&t={2}&srn={0}",
                                        systemName, serviceName, serviceTimeout,
                                        serviceAction == "stop"
                                            ? "StopService"
                                            : serviceAction == "start"
                                                ? "StartService"
                                                : serviceAction == "restart" ? "RestartService" : string.Empty);

                                Logger.Log("URI: " + uri);

                                var groupService = new GroupScheduleServiceDetailEntity
                                {
                                    Service_Name = serviceName,
                                    HostIP = systemName,
                                    Config_ID = serviceOnDemand[0].ConfigId,
                                    Env_ID = serviceOnDemand[0].EnvId,
                                    CreatedBy = loggedUserId,
                                };

                                winService.SetAcknowledge(groupService, "stop");

                                
                                var wrapper = new WebHttpRequestBuilder();
                                var request = wrapper.CreateGetRequest(uri);
                                var response = monitorService.GetWebResponse<WindowServiceStatus>(request);
                                
                                result = response.ServiceStatus;

                                isTimeout = false;

                                serviceOnDemand[0].WindowServiceStatus = result;
                                serviceOnDemand[0].CompletionStatus = "Successful";
                                serviceOnDemand[0].GroupScheduledStatus = "S";

                                if (serviceAction != "stop")
                                    winService.SetAcknowledge(groupService, "start");

                            }
                            catch (AggregateException agex)
                            {
                                Logger.Log("Agg - Windows Service operation: " + agex);
                                result = "cancelled";
                                serviceOnDemand[0].WindowServiceStatus = result;
                                serviceOnDemand[0].CompletionStatus = "Cancelled";
                                serviceOnDemand[0].GroupScheduledStatus = "C";
                                throw;
                            }
                            catch (Exception ex)
                            {
                                Logger.Log("Windows Service general operation: " + ex);

                                if (ex.Message.Contains("timed out"))
                                {
                                    result = "timedout";
                                    serviceOnDemand[0].WindowServiceStatus = serviceOnDemand[0].CompletionStatus = "Timed out";
                                    serviceOnDemand[0].GroupScheduledStatus = "T";
                                }
                                else if (ex.Message.Contains("Unable to connect"))
                                {
                                    result = "unabletoconnect";
                                    serviceOnDemand[0].WindowServiceStatus = "cancelled";
                                    serviceOnDemand[0].CompletionStatus = "Unsuccessful";
                                    serviceOnDemand[0].GroupScheduledStatus = "U";
                                }
                                else
                                {
                                    result = "cancelled";
                                    serviceOnDemand[0].WindowServiceStatus = result;
                                    serviceOnDemand[0].CompletionStatus = "Cancelled";
                                    serviceOnDemand[0].GroupScheduledStatus = "C";
                                }
                            }
                            finally
                            {
                                if (cancellationTokenSource != null) cancellationTokenSource.Dispose();
                            }

                        }, token);

                        try
                        {
                            t.Wait(new TimeSpan(0, 0, serviceTimeout));
                            //cancellationTokenSource.Cancel();

                            if (isTimeout)
                            {
                                if (result == "cancelled")
                                {
                                    serviceOnDemand[0].WindowServiceStatus = result;
                                    serviceOnDemand[0].CompletionStatus = "Cancelled";
                                    serviceOnDemand[0].GroupScheduledStatus = "C";
                                }
                                else if (result == "timedout")
                                {
                                    result = "timedout";
                                    serviceOnDemand[0].WindowServiceStatus = result;
                                    serviceOnDemand[0].CompletionStatus = "Timed out";
                                    serviceOnDemand[0].GroupScheduledStatus = "T";
                                }
                                else if(!string.IsNullOrEmpty(result))
                                {
                                    result = "timedout";
                                    serviceOnDemand[0].WindowServiceStatus = result;
                                    serviceOnDemand[0].CompletionStatus = "Timed out";
                                    serviceOnDemand[0].GroupScheduledStatus = "T";

                                }
                                serviceOnDemand[0].Comments = "Ondemand";
                            }

                            winService.SendMailServiceRestartOnDemand(serviceOnDemand[0]);
                        }
                        catch (AggregateException agex)
                        {
                            Logger.Log("File name: " + fileName);
                            Logger.Log("Windows Service operation: " + agex.StackTrace);
                            throw;
                        }
                        finally
                        {
                            cancellationTokenSource.Dispose();
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                result = "abonded";
                Logger.Log(ex.ToString());
                Logger.Log(ex.ToString());

            }
            return result;
            */
        }

        /// <summary>
        /// Get the windows service status 
        /// </summary>
        /// <param name="serviceName"></param>
        /// <param name="systemName"></param>
        /// <returns></returns>
        [System.Web.Services.WebMethod]
        public static string GetWindowsServiceStatus(string serviceName)
        {
            return GetWindowsServiceStatus(serviceName, null);
        }

        /// <summary>
        /// Get the windows service status 
        /// </summary>
        /// <param name="serviceName"></param>
        /// <param name="systemName"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        [System.Web.Services.WebMethod]
        public static string GetWindowsServiceStatus(string serviceName, string systemName)
        {
            if (systemName == null) throw new ArgumentNullException("systemName");
            string serviceStatus;

            try
            {
                var action = "status";
                Logger.Log("Calling RemoteServiceStatus");
                Logger.Log(fileNameServiceStatus + " \"" + serviceName + "\" \"" + systemName + "\" \"" + action + "\"");

                var processInfo = new ProcessStartInfo(fileNameServiceStatus)
                {
                    Arguments = "\"" + serviceName + "\" \"" + systemName + "\" \"" + action + "\"",
                    Verb = "runas",
                    UseShellExecute = true,
                    WindowStyle = ProcessWindowStyle.Minimized
                };

                var p = Process.Start(processInfo);
                p.WaitForExit();
                var currentStatus = p.ExitCode;
                Logger.Log("Status: " + currentStatus);

                switch (currentStatus)
                {
                    case 1:
                        serviceStatus = SERVICE_RUNNING;
                        break;
                    case 11:
                        serviceStatus = SERVICE_STARTING;
                        break;
                    case 2:
                        serviceStatus = SERVICE_STOPPED;
                        break;
                    case 21:
                        serviceStatus = SERVICE_STOPPING;
                        break;
                    case 3:
                    case 31:
                        serviceStatus = SERVICE_PAUSED;
                        break;
                    default:
                        serviceStatus = SERVICE_NAME_NOTFOUND;
                        break;
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex.ToString());
                serviceStatus = SERVICE_NAME_NOTFOUND;
            }
            return serviceStatus;
        }

        [System.Web.Services.WebMethod]
        public static string GetWindowsServiceStatusLocal(string serviceName, string systemName = null)
        {
            if (systemName == null) throw new ArgumentNullException("systemName");
            string serviceStatus = string.Empty;
            using (var sc = !string.IsNullOrEmpty(systemName) ? new ServiceController(serviceName, systemName) : new ServiceController(serviceName))
            {
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
                    Logger.Log(ex.ToString());
                    if (ex.Message.Contains("not found"))
                        serviceStatus = SERVICE_NAME_NOTFOUND;
                }
            }
            return serviceStatus;
        }

        [System.Web.Services.WebMethod]
        public static string WindowServiceFuction(string serviceName, string serviceAction, string systemName = null)
        {
            string result = null;
            var isProcessing = true;
            try
            {
                Logger.Log(fileName + " \"" + serviceName + "\" \"" + serviceAction + "\" \"" + systemName + "\"");

                var processInfo = new ProcessStartInfo(fileName)
                {
                    Arguments = "\"" + serviceName + "\" \"" + serviceAction + "\" \"" + systemName + "\"",
                    Verb = "runas",
                    UseShellExecute = true,
                    WindowStyle = ProcessWindowStyle.Minimized
                };
                Process.Start(processInfo);

                #region Chedking the service status
                while (isProcessing)
                {
                    Thread.Sleep(2000);
                    result = GetWindowsServiceStatus(serviceName, systemName);
                    if (serviceAction == "start")
                    {
                        if (String.Equals(result, SERVICE_STOPPING, StringComparison.CurrentCultureIgnoreCase) ||
                                String.Equals(result, SERVICE_STARTING, StringComparison.CurrentCultureIgnoreCase) ||
                                String.Equals(result, SERVICE_STOPPED, StringComparison.CurrentCultureIgnoreCase))
                        {
                        }
                        else
                            isProcessing = false;
                    }
                    else if (serviceAction == "stop")
                    {
                        if (result.ToLower() == SERVICE_STOPPING.ToLower() ||
                                result.ToLower() == SERVICE_STARTING.ToLower() ||
                                result.ToLower() == SERVICE_RUNNING.ToLower())
                        {
                            isProcessing = true;
                        }
                        else
                            isProcessing = false;

                    }
                    else if (serviceAction == "restart")
                    {
                        if (result.ToLower() == SERVICE_STOPPING.ToLower() ||
                                result.ToLower() == SERVICE_STARTING.ToLower())
                        {
                            isProcessing = true;
                        }
                        else
                            isProcessing = false;
                    }
                }
                #endregion Chedking the service status


            }
            catch (Exception e)
            {
                Logger.Log(e.ToString());
                return e.Message;
            }
            return result;
        }

        #endregion windows service operation

        #region get windows service details by ENV / Config details
        [System.Web.Services.WebMethod]
        public static List<MonitorEntity> getMonitorStatusWithServiceName(string id, string type)
        {
            List<MonitorEntity> monStatusList = new List<MonitorEntity>();
            monStatusList = winService.getMonitorStatusWithServiceName(Convert.ToInt32(id), type);
            if (monStatusList != null && monStatusList.Count > 0)
            {
                foreach (MonitorEntity monitor in monStatusList)
                {
                    if (monitor.ConfigServiceType == "1")
                        monitor.ConfigServiceType = CONTENT_SERVICE;
                    if (monitor.ConfigServiceType == "2")
                        monitor.ConfigServiceType = DESPATCHER_SERVICE;

                    if (!string.IsNullOrEmpty(monitor.MonitorStatus))
                    {
                        if (monitor.MonitorStatus.ToLower() == "stopped")
                        {
                            monitor.Incident_Solution = "../images/red1_icon.jpg";
                            monitor.MonitorStatus = monitor.MonitorComments;
                        }
                        else if (monitor.MonitorStatus.ToLower() == "running")
                            monitor.Incident_Solution = "../images/green_icon.jpg";
                        else if (monitor.MonitorStatus.ToLower() == "standby")
                            monitor.Incident_Solution = "../images/orange_icon.jpg";
                        else if (monitor.MonitorStatus.ToLower() == "not running")
                            monitor.Incident_Solution = "../images/red_icon.jpg";
                        else if (monitor.MonitorStatus.ToLower() == "not ready")
                            monitor.Incident_Solution = "../images/blue_icon.jpg";
                        else
                            monitor.Incident_Solution = "../images/gray_icon.png";
                    }
                    else
                    {
                        monitor.Incident_Solution = "../images/gray_icon.png";
                        monitor.MonitorStatus = "Monitor is off";
                    }
                    monitor.Incident_Issue = !string.IsNullOrEmpty(monitor.WindowsServiceName)
                        ? monitorService.GetRemoteWindowsServiceStatus(monitor.WindowsServiceName, monitor.ConfigHostIP)
                        : string.Empty;
                }
            }
            return monStatusList;

        }
        #endregion get windows service details by ENV / Config details

        #region GetAll groups
        [System.Web.Services.WebMethod]
        public static string GetAllGroups(string grpId)
        {
            var groupList = winService.GetAllGroup(Convert.ToInt32(grpId));
            //groupList.Insert(0, new GroupEntity { Group_ID = 0, Group_Name = "Select" });
            var json = javaScriptSerializer.Serialize(groupList);
            return json;
        }
        #endregion GetAll groups

        #region Get Group schedule service report

        #region Get report of Group schedule details
        [System.Web.Services.WebMethod]
        public static List<GroupcheduleReportEntity> GetGroupScheduleReport(string schType, DateTime startDate, DateTime endDate)
        {
            var groupSchedules = winService.GetGroupScheduleReport(schType, startDate, endDate);

            return groupSchedules;
        }
        #endregion Get report of Group schedule details


        #endregion Get Group schedule service report

        #region Insert user
        [System.Web.Services.WebMethod]
        public static string InsertUpdateUser(string id, string firstName, string lastName, string email, string password, string[] roles, string status)
        {
            string record = string.Empty;
            try
            {
                string encryptPassword = FormsAuthentication.HashPasswordForStoringInConfigFile(password, "SHA1");
                UserEntity user = new UserEntity();
                user.UserID = Convert.ToInt32(id);
                user.FirstName = firstName;
                user.LastName = lastName;
                user.Email = email;
                user.Password = CommonUtility.EnryptString(password);
                user.Roles = string.Join(",", roles); // convert aarry to comma seperated value
                //user.IsActive = Convert.ToBoolean(status);
                if (status == "active")
                    user.IsActive = true;
                else
                    user.IsActive = false;
                //user.IsDeleted = false;
                record = userService.InsertUpdateUser(user);
            }
            catch (Exception)
            {
                throw;
            }

            return record.ToString();
        }
        #endregion Insert user

        #region Edit user profile
        [System.Web.Services.WebMethod]
        public static string EditProfile(string id, string email, string currentPassword, string newPassword)
        {
            string newUserPassword = string.Empty;
            string record = string.Empty;
            try
            {
                UserEntity user = new UserEntity();
                user.UserID = Convert.ToInt32(id);
                user.Email = email;
                user.Password = CommonUtility.EnryptString(currentPassword);
                newUserPassword = CommonUtility.EnryptString(newPassword);

                record = userService.UpdateUserPassword(user, newUserPassword);

            }
            catch (Exception ex)
            {
                Logger.Log(ex.ToString());
                throw;
            }
            return record.ToString();
        }
        #endregion Edit user profile

        #region Get user
        [System.Web.Services.WebMethod]
        public static List<UserEntity> GetUser(string uid, string mode)
        {
            int userID = 0; int logUserID = 0;
            UserEntity user = new UserEntity();
            List<UserEntity> users = new List<UserEntity>();

            userID = Convert.ToInt32(uid);


            if (HttpContext.Current.Session["_LOGGED_USERD_ID"] != null)
            {
                logUserID = Convert.ToInt32(HttpContext.Current.Session["_LOGGED_USERD_ID"]);
            }

            //Get user details from database
            users = userService.GetUsers(userID);

            if (users != null && users.Count > 0 && mode.ToLower() == "all") // all - all users case
            {
                if (users.Any(us => us.UserID == logUserID))
                {
                    users = users.Except(users.Where(ur => ur.UserID == logUserID)).ToList();
                }
            }

            return users;

        }
        #endregion Get user

        #region delete user
        [System.Web.Services.WebMethod]
        public static int DeleteUser(string uid)
        {
            int userID = 0;
            int status = 0;
            userID = Convert.ToInt32(uid);

            status = userService.DeleteUser(userID);
            //record = userService.InsertUpdateUser(user);

            return status;
        }
        #endregion delete user

        #region Login user
        [System.Web.Services.WebMethod]
        public static string ValidateLoginUser(string userId, string password, bool rememberMe)
        {
            int loginStatus = 0; string roles = string.Empty;
            string loginMessage = string.Empty;
            List<RoleMenuEntity> rolewiseMenu = new List<RoleMenuEntity>();
            List<UserEntity> validUser = new List<UserEntity>();
            try
            {

                loginStatus = userService.LoginUsers(userId, password);
                if (loginStatus >= 1)
                {
                    validUser = userService.GetUsers(loginStatus);
                    if (validUser != null && validUser.Count > 0)
                    {
                        HttpContext.Current.Session["LOGGEDINUSER"] = validUser[0];
                        HttpContext.Current.Session["_LOGGED_USERD_ID"] = validUser[0].UserID;
                        HttpContext.Current.Session["_LOGGED_USERD_EMAIL"] = validUser[0].Email;
                        HttpContext.Current.Session["_LOGGED_USERFIRSTNAME"] =
                            HttpContext.Current.Session["_LOGGED_USERNAME"] = validUser[0].FirstName;
                        
                        if (!string.IsNullOrEmpty(validUser[0].LastName))
                        {
                            HttpContext.Current.Session["_LOGGED_USERNAME"] += " " + validUser[0].LastName;
                        }

                        if (validUser[0].UserRoles.Any(x => x.RoleID == 1))
                        {
                            HttpContext.Current.Session["_LOGGED_USERROLE"] = "Administrator";
                        }

                        //GetRoleMenu details
                        roles = string.Join(",", validUser[0].UserRoles.Select(x => x.RoleID));
                        rolewiseMenu = userService.GetRoleMenu(roles);
                        HttpContext.Current.Session["ROLEMENU"] = rolewiseMenu;
                        if (rolewiseMenu != null && rolewiseMenu.Count > 0)
                        {
                            foreach (var menuEntity in rolewiseMenu.Where(menuEntity => menuEntity.MenuPath != "-"))
                            {
                                loginMessage = menuEntity.MenuPath;
                                break;
                            }
                        }
                        else
                        {
                            Logger.Log("Menu details are not setup in database");
                            loginMessage = "Menu details are not avaialble";
                        }

                        //userService.SetRememberMe(userId,password, rememberMe);
                    }
                }

                //if (loginStatus >= 1)
                //    loginMessage = loginMessage;
                //else 
                if (loginStatus == 0)
                    loginMessage = "Invalid user / email address";
                else if (loginStatus == -1)
                    loginMessage = "Invalid password";

            }
            catch (Exception ex)
            {

                Logger.Log(ex.ToString());
            }
            return loginMessage;
        }
        #endregion Login user

        #region Get current build version
        [System.Web.Services.WebMethod]
        public static List<ServiceMoniterEntity> GetCurrentBuileDetails(int envID)
        {
            List<ServiceMoniterEntity> monitorList = new List<ServiceMoniterEntity>();
            monitorList = monitorService.GetServiceBuildVersionReport(envID);

            return monitorList;
        }
        #endregion Get current build version

        #region Get build history
        [System.Web.Services.WebMethod]
        public static List<ServiceMoniterEntity> GetBuildHistoryReport(int envID, DateTime startDate, DateTime endDate)
        {
            List<ServiceMoniterEntity> monitorList = new List<ServiceMoniterEntity>();

            monitorList = monitorService.GetServiceBuildHistoryReport(envID, startDate, endDate);

            return monitorList;
        }
        #endregion Get build history

        #region Get service status history
        [System.Web.Services.WebMethod]
        public static List<ServiceMoniterEntity> GetServiceStatusHistoryReport(int envID, DateTime startDate, DateTime endDate)
        {
            var monitorList = monitorService.GetServiceStatusReport(envID, startDate, endDate);

            return monitorList;
        }

        #endregion region Get service status history

        #region Get All environments
        [System.Web.Services.WebMethod]
        public static List<EnvironmentEntity> GetAllEnvironments(string envId)
        {
            var environmentList = enviService.GetEnvironments(Convert.ToInt32(envId));
            var emailList = enviService.GetEmailConfiguration(0);
            
            if (environmentList != null && environmentList.Count > 0)
            {
                foreach (var environment in environmentList)
                {
                    if (environment.EnvDetailsList != null && environment.EnvDetailsList.Count > 0)
                    {
                        if (environment.EnvID > 0 && emailList.Any(emList => emList.EnvID == environment.EnvID))
                        {
                            environment.IsEmailExists = true;
                            if (environment.EnvDetailsList.Any(li => li.SchedulerSummary == null))
                            {
                                var envNotScheduledList = environment.EnvDetailsList.Where(li => li.SchedulerSummary == null).ToList<EnvDetailsEntity>();
                                var envScheduleList = environment.EnvDetailsList.Where(li => li.SchedulerSummary != null).ToList<EnvDetailsEntity>();
                                //var envDetailsEntities = envScheduleist as EnvDetailsEntity[] ?? envScheduleist.ToArray();


                                environment.UnScheduledCount = envNotScheduledList.Count;
                                environment.ScheduledCount = envScheduleList.Count;

                                if (envNotScheduledList.Count == environment.EnvDetailsList.Count)
                                {
                                    environment.SchedularStatus = "2";
                                    environment.SchedularTitle = NoServiceScheduled_Message;
                                }
                                else if (envNotScheduledList.Count < environment.EnvDetailsList.Count)
                                {
                                    environment.SchedularStatus = "3";
                                    environment.SchedularTitle = envScheduleList.Count == 1 ? string.Format("Only {0} service is scheduled", envScheduleList.Count) : string.Format("{0} services are scheduled", envScheduleList.Count);
                                }

                                foreach (var envDetailsEntity in environment.EnvDetailsList.Where(li => string.IsNullOrEmpty( li.SchedulerSummary) ))
                                {
                                    envDetailsEntity.SchedulerSummary = "Yet to schedule";
                                }
                            }
                            else
                            {
                                environment.SchedularStatus = "1";
                                environment.SchedularTitle = AllServiceScheduled_Message;
                            }
                        }
                        else
                        {
                            environment.IsEmailExists = false;
                            environment.SchedularStatus = "2";
                            environment.SchedularTitle = UnableToScheduleService_Message;
                        }
                    }
                    else
                    {
                        environment.SchedularStatus = "0";
                        environment.SchedularTitle = NoServiceAvailable_Message;
                    }
                }
            }

            return environmentList;
        }
        #endregion Get All environments

        #region Get incident report history
        [System.Web.Services.WebMethod]
        public static List<ServiceMoniterEntity> GetIncidentTrackingReport(int envId, DateTime startDate, DateTime endDate)
        {
            var enviIncidentList = monitorService.GetIncidentTrackingReport(envId, startDate, endDate);

            return enviIncidentList;
        }
        #endregion Get incident report history

        #region Get Server datetime

        [System.Web.Services.WebMethod]
        public static string GetServerDateTime()
        {
            return DateTime.Now.ToString("MM/dd/yyy HH:m:s");
        }
        #endregion Get server datetime

        #region Get all monitor details for dashboard

        [System.Web.Services.WebMethod]
        public static List<ServiceMoniterEntity> GetAllMonitors(string envId, bool isWithServiceName)
        {
            var monitorList = monitorService.GetAllMonitors(Convert.ToInt32(envId), isWithServiceName);
            HttpContext.Current.Session["ServiceMonitor"] = monitorList;
            return monitorList;
        }

        [System.Web.Services.WebMethod]
        public static string GetAllMonitorsJson(string envId, bool isWithServiceName)
        {
            var monitorList = monitorService.GetAllMonitors(Convert.ToInt32(envId), isWithServiceName);
            HttpContext.Current.Session["ServiceMonitor"] = monitorList;
            var json = javaScriptSerializer.Serialize(monitorList);
            return json;
        }

        [System.Web.Services.WebMethod]
        public static ServiceMoniterEntity GetServiceStatusByEnvironment(string envId)
        {
            var monitorList = monitorService.GetAllMonitors(Convert.ToInt32(envId), true);
            if (monitorList != null && monitorList.Count > 0)
            {
                var monitor = monitorList.FirstOrDefault(_ => _.EnvID == Convert.ToInt32(envId));
                var statusList = monitorService.UpdateWindowsServicetatus(monitor);
                return statusList;
            }
            return new ServiceMoniterEntity();
        }

        #endregion Get all monitor details for dashboard

        #region UrlConfiguration

        [System.Web.Services.WebMethod]
        public static List<UrlConfigurationEntity> GetAllUrlConfiguration(string envId, string urlId)
        {
            var urlList = enviService.GetUrlConfiguration(Convert.ToInt32(envId), Convert.ToInt32(urlId));
            HttpContext.Current.Session["ServiceUrls"] = urlList;
            return urlList;
        }

        #endregion UrlConfiguration

        #region GetPersonalize setting

        [System.Web.Services.WebMethod]
        public static string GetPersonaliseSetting()
        {
            var refreshTime = 0;
            var userId = Convert.ToInt32(HttpContext.Current.Session["_LOGGED_USERD_ID"]);
            var personalise = monitorService.GetPersonalize(userId);
            if (personalise != null && personalise.Count > 0)
            {
                HttpContext.Current.Session["_PERSONALIZE"] = personalise[0];
                refreshTime = personalise[0].RefreshTime;
            }

            return refreshTime.ToString();
        }

        #endregion GetPersonalize setting

        #region Get Environment Emails
        [System.Web.Services.WebMethod]
        public static List<ConfigEmailsEntity> GetEnvironmentEmails()
        {
            var emailConfiglist = enviService.GetEnvEmailConfiguration(0);
            return emailConfiglist;
        }

        #endregion Get Environment Emails


        #region Get Environment Email configuration
        [System.Web.Services.WebMethod]
        public static List<ConfigEmailsEntity> GetEmailConfiguration()
        {
            var emailConfiglist = enviService.GetEmailConfiguration(0);
            return emailConfiglist;
        }

        #endregion Get Environment Email configuration

        #region Get all incidents

        [System.Web.Services.WebMethod]
        public static List<ServiceMoniterEntity> GetAllIncident(string envId)
        {
            var enviList = monitorService.GetAllIncident(Convert.ToInt32(envId), INCODENT_TYPE);
            if (enviList != null && enviList.Count > 0)
            {
                enviList.RemoveAll(el => el.monitorList.Count <= 0);
            }

            return enviList;
        }

        #endregion Get all incidents

        #region EmailSubscription

        [System.Web.Services.WebMethod]
        public static List<DailyReportSubscriptionEntity> GetReportSubscription(string envId)
        {
            var dailyReportSubscription = new List<DailyReportSubscriptionEntity>();
            try
            {
                if (!string.IsNullOrEmpty(envId))
                    dailyReportSubscription = reportService.GetDailyReportSubscription(Convert.ToInt32(envId));

                if (dailyReportSubscription != null && dailyReportSubscription.Count > 0)
                {
                    for (var index = 0; index < dailyReportSubscription.Count; index++)
                    {
                        var subscription = dailyReportSubscription[index];
                        subscription.UserListId = (index + 1);
                    }
                }
                else
                    dailyReportSubscription = new List<DailyReportSubscriptionEntity>();

            }
            catch (Exception exception)
            {
                Logger.Log(exception.ToString());
                dailyReportSubscription = new List<DailyReportSubscriptionEntity>();
            }

            return dailyReportSubscription;
        }

        [System.Web.Services.WebMethod]
        public static SubscriptionResult InsUpdReportSubscription(string subscriptionId, string reportType, string reportTime, bool isDisable, string selectedUserEmails)
        {
            int currentSubcriptionId = 0;
            try
            {
                if (HttpContext.Current.Session["_LOGGED_USERD_ID"] == null)
                    throw new ApplicationException("Application Session has expired. Please login again to proceed");

                var emailSubscription = new EmailSubscription
                {
                    Id = !string.IsNullOrEmpty(subscriptionId) ? Convert.ToInt32(subscriptionId) : 0,
                    Type = reportType,
                    Time = reportTime,
                    EmailList = selectedUserEmails,
                    IsActive = !isDisable,
                    CreatedBy = HttpContext.Current.Session["_LOGGED_USERD_ID"].ToString(),
                    UpdatedBy = HttpContext.Current.Session["_LOGGED_USERD_ID"].ToString(),
                };

                currentSubcriptionId = reportService.InsertUpdateEmailSubscription(emailSubscription);
                return new SubscriptionResult { Status = reportTime = "Success", Message = string.Empty, SubscriptionId = currentSubcriptionId };
            }
            catch (Exception exception)
            {
                return new SubscriptionResult
                {
                    Status = reportTime = "Failure",
                    Message = exception.Message,
                    SubscriptionId = currentSubcriptionId
                };
            }

        }

        #endregion EmailSubscription

        #region ValidatePortal using Credential
        [System.Web.Services.WebMethod]
        public static CognosCgiResponse LogWebsiteWithCredential(string url, string match, string username,
            string password)
        {
            var result = CommonUtility.LogWebsiteWithCredential(url, match, username, password);
            return result;
        }

        #endregion ValidatePortal using Credential

        #region Get Url Performance

        [System.Web.Services.WebMethod]
        public static CognosCgiResponse LogCognosPortalResponse(string envId, string urlId)
        {
            var urlConfiguration = enviService.GetUrlConfiguration(Convert.ToInt32(envId), Convert.ToInt32(urlId));
            if (urlConfiguration == null || urlConfiguration.Count <= 0)
                return new CognosCgiResponse
                {
                    Status = "Failure",
                    Message = "Failure to login Cognos portal",
                    Exception = string.Empty
                };
            var configuration = urlConfiguration.FirstOrDefault(_ => !string.IsNullOrEmpty(_.Adress));

            if (configuration != null)
            {
                var passwordDecrypt = CommonUtility.Decrypt(configuration.Password);
                var response = LogWebsiteWithCredential(configuration.Adress, "log off",
                    configuration.UserName, passwordDecrypt);
                response.UserName = configuration.UserName;
                response.Password = passwordDecrypt;

                return response;
            }

            return new CognosCgiResponse
            {
                Status = "Failure",
                Message = "URL configuration detail does not available for this environment" ,
                Exception = string.Empty
            };
        }


        [System.Web.Services.WebMethod]
        public static List<UrlPerformance> GetUrlPerformance(string envId)
        {
            var performance = enviService.GetUrlPerformance(Convert.ToInt32(envId));
            
            return performance;
        }

        #endregion Get Url Performance

        #region Get Server Performance
        [System.Web.Services.WebMethod]
        public static List<ServerDriveDetail> GetAverageUsedSpace(string host, string mode)
        {
            var averageUsedSpace = monitorService.GetAverageUsedSpace(host, mode);
            
            return averageUsedSpace;
        }

        [System.Web.Services.WebMethod]
        public static List<ServerCpuUsage> GetAverageCpuUsage(string host)
        {
            var averageCpuUsage = monitorService.GetCpuMemorySpace(host);

            return averageCpuUsage;
        }

        [System.Web.Services.WebMethod]
        public static PerformanceResponse GetCurrentDriveInfos(string host)
        {
            var response = monitorService.GetCurrentDriveInfos(host);
            return response;
        }

        #endregion Get Server Performance

    }

    public class ServiceException : Exception
    {
        public ServiceException(string message)
            : base(message)
        {
            Logger.Log(message);
        }
    }



}