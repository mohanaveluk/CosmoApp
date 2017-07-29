using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;
using Cosmo.Data;
using Cosmo.Entity;

namespace Cosmo.Service
{
    public class WinService
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

        #endregion Constants

        #region object declaration

        readonly IWinServiceData _winData;
        readonly IMonitorData _monitorData;
        MailService mailService = new MailService();
        MonitorService monitorService = new MonitorService();
        UserService userService;
        #endregion object declaration

        #region Variables
        private static readonly string DbType = ConfigurationManager.AppSettings["DatabaseType"];
        private readonly string _groupscheduleMail = Convert.ToString(ConfigurationManager.AppSettings["GroupSchedule"]);
        private readonly string _scheduleWindowsServiceOperationCancelled = Convert.ToString(ConfigurationManager.AppSettings["ScheduleWindowsServiceOperationCancelled"]);
        private readonly string _servicerestartondemand = Convert.ToString(ConfigurationManager.AppSettings["ScheduleWindowsServiceOnDemand"]);
        private static readonly string RemoteSystemUrlForServiceStatus = ConfigurationManager.AppSettings["RemoteSystemUrlForServiceStatus"].ToString();

        private string tableStyle = "'border-collapse:collapse;border:1px solid #eee; width:70%'";
        private string heaferStyle = "'border:1px solid #ddd; padding:5px;background-color:#A1C1D5'";
        private string tdHost = "'border:1px solid #ddd; padding:5px;width:30%'";
        private string tdCommon = "'border:1px solid #ddd; padding:5px;'";
        private string tdStyleCenter = "'border:1px solid #ddd; padding:5px;text-align:center'";

        string fileName = @AppDomain.CurrentDomain.BaseDirectory + @"ServiceControl\WinServiceControl.exe";
        int serviceTimeout = Convert.ToInt32(ConfigurationManager.AppSettings["WindowsServiceTimeout"]);

        // Get the local time zone and the current local time and year.
        TimeZone localZone = TimeZone.CurrentTimeZone;
        HttpContext context = HttpContext.Current;

        #endregion Variables

        public WinService()
        {
            var iDbType = DbType == DatabaseType.Oracle.ToString()
                ? Convert.ToInt32(DatabaseType.Oracle).ToString()
                : Convert.ToInt32(DatabaseType.SqlServer).ToString();

            _winData = new WinServiceDataFactory().Create(iDbType);
            _monitorData = new MonitorDataFactory().Create(iDbType);

        }

        #region Insert Group
        /// <summary>
        /// 
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        public int InsGroup(GroupEntity group)
        {
            return _winData.InsGroup(group);
        }
        #endregion Insert Group

        #region Insert Group name in to database
        /// <summary>
        /// Get all the group name based on the group ID
        /// </summary>
        /// <param name="grp_ID"></param>
        /// <returns></returns>
        public List<GroupEntity> GetAllGroup(int grp_ID)
        {
            return _winData.GetAllGroup(grp_ID);
        }
        #endregion Insert Group name in to database

        #region Insert Group details in to database
        /// <summary>
        /// Insert Group details in to database
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        public int InsUpdateGroupDetail(int sEnvID, int sGroupID, string sConfigIDs)
        {
            GroupDetailEntity groupDetail = new GroupDetailEntity();
            groupDetail.Env_ID = sEnvID;
            groupDetail.Group_ID = sGroupID;
            groupDetail.Config_ID = 0;
            groupDetail.Comments = string.Empty;
            groupDetail.CreatedBy = HttpContext.Current.Session["_LOGGED_USERD_ID"] != null ? HttpContext.Current.Session["_LOGGED_USERD_ID"].ToString() : string.Empty;
            groupDetail.CreatedDate = System.DateTime.Now;
            groupDetail.IsActive = true;

            return _winData.InsGroupDetail(groupDetail, sConfigIDs);
        }
        #endregion Insert Group details in to database

        #region Insert Group schedule details in to database

        /// <summary>
        /// Insert Group details in to database
        /// </summary>
        /// <param name="group"></param>
        /// <param name="groupId"></param>
        /// <param name="groupName"></param>
        /// <param name="envIDs"></param>
        /// <param name="configIDs"></param>
        /// <param name="winServiceIDs"></param>
        /// <param name="serviceAction"></param>
        /// <param name="actionDateTime"></param>
        /// <param name="requestSource"></param>
        /// <returns></returns>
        public int InsGroupScheduleDetail(int groupId, string groupName, string envIDs, string configIDs, string winServiceIDs, string serviceAction, DateTime actionDateTime, string requestSource)
        {
            if (HttpContext.Current.Session ==null)
            {
                
            }

            char[] delimiterChars = { ',' };
            string[] envArr = envIDs.Split(delimiterChars);
            string[] configArr = configIDs.Split(delimiterChars);
            string[] winServiceArr = winServiceIDs.Split(delimiterChars);
            
            var details = new GroupScheduleDetailEntity();

            int result = 0;
            try
            {
                DateTime utcDate = actionDateTime.ToUniversalTime();
                DateTime localDate = utcDate.ToLocalTime();

                var groupScheduleEntity = new GroupScheduleEntity
                {
                    Group_Schedule_ID = 0,
                    Group_ID = groupId,
                    Group_Name = groupName,
                    Group_Schedule_Action = serviceAction,
                    Group_Schedule_Datatime = localDate,
                    Group_Schedule_Status = "O",
                    Group_Schedule_CreatedBy =
                        HttpContext.Current.Session != null && HttpContext.Current.Session["_LOGGED_USERD_ID"] != null
                            ? HttpContext.Current.Session["_LOGGED_USERD_ID"].ToString()
                            : "1",
                    Group_Schedule_CreatedDatetime = DateTime.Now,
                    Group_Schedule_CompletedTime = DateTime.Now,
                    Group_Schedule_OnDemand = false,
                    Group_Schedule_CompleteStatus = "N/A",
                    RequestSource = requestSource
                };
                result = _winData.InsGroupScheduleDetail(groupScheduleEntity, envIDs, configIDs, winServiceIDs);
                if (result > 0)
                {
                    SendMailGroupSchedule(result, 'O');
                }
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public int InsGroupScheduleDetail(ServiceOnDemandMailEntity demand)
        {
            int result = 0;
            try
            {

                GroupScheduleEntity groupScheduleEntity = new GroupScheduleEntity();
                groupScheduleEntity.Group_Schedule_ID = 0;
                groupScheduleEntity.Group_ID = 0;
                groupScheduleEntity.Group_Name = "OnDemand";

                if(demand.RequestStatus == CommonUtility.WinServiceAction.Start.ToString())
                    groupScheduleEntity.Group_Schedule_Action = ((int)CommonUtility.WinServiceAction.Start).ToString();
                else if (demand.RequestStatus == CommonUtility.WinServiceAction.Stop.ToString())
                    groupScheduleEntity.Group_Schedule_Action = ((int)CommonUtility.WinServiceAction.Stop).ToString();
                else if (demand.RequestStatus == CommonUtility.WinServiceAction.Restart.ToString())
                    groupScheduleEntity.Group_Schedule_Action = ((int)CommonUtility.WinServiceAction.Restart).ToString();

                groupScheduleEntity.Group_Schedule_Datatime = demand.RequestedDateTime;
                groupScheduleEntity.Group_Schedule_Status = demand.GroupScheduledStatus;
                groupScheduleEntity.Group_Schedule_CreatedBy = demand.RequestedBy;
                groupScheduleEntity.Group_Schedule_CreatedDatetime = DateTime.Now;
                groupScheduleEntity.Group_Schedule_CompletedTime = DateTime.Now;
                groupScheduleEntity.Group_Schedule_OnDemand = true;
                groupScheduleEntity.Group_Schedule_CompleteStatus = demand.CompletionStatus ?? string.Empty;
                groupScheduleEntity.RequestSource = demand.RequestSource ?? string.Empty;
                groupScheduleEntity.Group_Schedule_Comments = demand.Comments;

                groupScheduleEntity.ServiceCompletionTime = demand.ServiceCompletionTime;
                groupScheduleEntity.ServiceStartedTime = demand.RequestedDateTime;

                
                result = _winData.InsGroupScheduleDetail(groupScheduleEntity, Convert.ToString(demand.EnvId), Convert.ToString(demand.ConfigId + ","), Convert.ToString(demand.WindowServiceId));

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }
        #endregion Insert Group schedule details in to database

        #region Get group detail
        /// <summary>
        /// Get all the group detail based on the group ID and env id
        /// </summary>
        /// <param name="grp_ID"></param>
        /// <returns></returns>
        public List<GroupDetailEntity> GetGroupDetail(int grp_ID, int env_ID)
        {
            return _winData.GetGroupDetail(grp_ID, env_ID);
        }
        #endregion Get group detail

        #region Get group detail with Service Monitor
        /// <summary>
        /// Get all the group detail with the monitor status based on the group ID and env id
        /// </summary>
        /// <param name="grp_ID"></param>
        /// <returns></returns>
        public List<GroupDetailEntity> GetGroupDetailWithServiveMonitor(int grp_ID, int env_ID)
        {
            List<int> groupEnvID = new List<int>();
            List<ServiceMoniterEntity> monitorList = new List<ServiceMoniterEntity>();
            List<GroupDetailEntity> grpDetails = new List<GroupDetailEntity>();
            grpDetails =  _winData.GetGroupDetail(grp_ID, env_ID);
            groupEnvID = grpDetails.Select(gEnv => gEnv.Env_ID).Distinct().ToList();

            foreach (int eID in groupEnvID)
            {
                monitorList = _monitorData.GetAllMonitors(eID, false);
                if (monitorList != null && monitorList.Count > 0)
                {
                    foreach (GroupDetailEntity tmpGroupDetail in grpDetails)
                    {
                        if (monitorList[0].monitorList.Any(ml => ml.ConfigID == tmpGroupDetail.Config_ID))
                        {
                            tmpGroupDetail.MonitorStatus = monitorList[0].monitorList.Where(ml => ml.ConfigID == tmpGroupDetail.Config_ID).ToList()[0].MonitorStatus;
                            tmpGroupDetail.MonitorComments = monitorList[0].monitorList.Where(ml => ml.ConfigID == tmpGroupDetail.Config_ID).ToList()[0].MonitorComments;
                        }
                        else
                        {
                            tmpGroupDetail.MonitorStatus = string.Empty;
                            tmpGroupDetail.MonitorComments = string.Empty;
                        }
                    }
                }
            }

            return grpDetails;
        }
        #endregion Get group detail with Service Monitor

        #region Get Service Monitor details
        /// <summary>
        /// Get Service Monitor details
        /// </summary>
        /// <param name="env_id"></param>
        /// <param name="isWithServiceName"></param>
        /// <returns></returns>
        public List<ServiceMoniterEntity> GetServiceMonitors(int env_id, bool isWithServiceName)
        {
            List<ServiceMoniterEntity> list = new List<ServiceMoniterEntity>();
            list = _monitorData.GetAllMonitors(env_id, isWithServiceName);
            return list;
        }
        #endregion Get Service Monitor details

        public List<MonitorEntity> getMonitorStatusWithServiceName(int envId, string type)
        {
            return _monitorData.GetMonitorStatusWithServiceName(envId, type);
        }

        #region Get all groups schedule details
        /// <summary>
        /// Get all the group schedules which are based on the start date time provided
        /// </summary>
        /// <param name="grp_ID"></param>
        /// <returns></returns>
        public List<GroupScheduleEntity> GetAllGroupSchedules(int grp_ID, int grp_sch_ID, DateTime startTime, string status)
        {
            List<GroupScheduleEntity> groupSchedule = new List<GroupScheduleEntity>();
            try
            {
                groupSchedule = _winData.GetAllGroupSchedules(grp_ID, grp_sch_ID, startTime);

                if (groupSchedule != null && groupSchedule.Count > 0)
                {
                    groupSchedule.RemoveAll(gs => gs.Group_Schedule_Status == "S");

                    if (groupSchedule.Any(gs => gs.Group_Schedule_Action == status))
                    {
                        groupSchedule = groupSchedule.Where(gs => gs.Group_Schedule_Action == status).ToList();
                    }

                    foreach (GroupScheduleEntity grpSch in groupSchedule)
                    {
                        if (grpSch.Group_Schedule_Action == "1")
                        {
                            grpSch.Group_Schedule_Action = CommonUtility.WinServiceAction.Start.ToString();
                        }
                        else if (grpSch.Group_Schedule_Action == "2")
                        {
                            grpSch.Group_Schedule_Action = CommonUtility.WinServiceAction.Stop.ToString();
                        }
                        else if (grpSch.Group_Schedule_Action == "3")
                        {
                            grpSch.Group_Schedule_Action = CommonUtility.WinServiceAction.Restart.ToString();
                        }

                        if (grpSch.Group_Schedule_Status == "O")
                        {
                            grpSch.Group_Schedule_Status = CommonUtility.WinServiceStatus.Scheduled.ToString();
                        }
                        else if (grpSch.Group_Schedule_Status == "C")
                        {
                            grpSch.Group_Schedule_Status = CommonUtility.WinServiceStatus.Completed.ToString();
                        }
                        else if (grpSch.Group_Schedule_Status == "F")
                        {
                            grpSch.Group_Schedule_Status = CommonUtility.WinServiceStatus.Failed.ToString();
                        }
                        else if (grpSch.Group_Schedule_Status == "P")
                        {
                            grpSch.Group_Schedule_Status = CommonUtility.WinServiceStatus.Pending.ToString();
                        }

                    }
                }

            }
            catch (Exception ex)
            {
                Logger.Log(ex.ToString());
                throw ex;
            }
            return  groupSchedule;
        }

        #endregion Get all groups schedule details

        #region Get all Group Schedule scrvice details

        public List<GroupScheduleEntity> GetGroupScheduleDetails(int groupId, char grpStatus)
        {
            var groupScheduleEntity = _winData.GetGroupScheduleDetails(groupId, "sch",0);
            if (groupScheduleEntity != null && groupScheduleEntity.Count>0)
            {
                var groupScheduleDetailEntity = _winData.GetGroupScheduleEnvDetails(groupId, "env",0);
                if (groupScheduleDetailEntity != null && groupScheduleDetailEntity.Count > 0)
                {
                    groupScheduleEntity[0].GroupScheduleDetails = new List<GroupScheduleDetailEntity>();
                    foreach (GroupScheduleDetailEntity gsd in groupScheduleDetailEntity)
                    {
                        var groupScheduleServiceDetailEntity = _winData.GetGroupScheduleServiceDetails(groupId, "cfg",
                            gsd.Env_ID, grpStatus);
                        if (groupScheduleServiceDetailEntity != null && groupScheduleServiceDetailEntity.Count > 0)
                        {
                            gsd.ServiceDetails = new List<GroupScheduleServiceDetailEntity>();
                            foreach (GroupScheduleServiceDetailEntity det in groupScheduleServiceDetailEntity)
                            {
                                if (det.Env_ID == gsd.Env_ID)
                                {
                                    gsd.ServiceDetails.Add(det);
                                }
                            }
                            
                        }
                        groupScheduleEntity[0].GroupScheduleDetails.Add(gsd);
                    }
                }
            }

            //call GroupScheduleDetailEntity


            return groupScheduleEntity;
        }
        #endregion Get all Group Schedule scrvice details

        #region sendmail for group schedule

        public void SendMailGroupSchedule(int grpSchId, char grpStatus)
        {
            var scheduleDetails = GetGroupScheduleDetails(grpSchId, grpStatus);
            if (scheduleDetails != null && scheduleDetails.Count > 0 && scheduleDetails[0].GroupScheduleDetails != null && scheduleDetails[0].GroupScheduleDetails.Count > 0)
            {

                GroupScheduleServiceMailEntity serviceFormMail = null;
                List<GroupScheduleEntity> tempEntity = new List<GroupScheduleEntity>();
                foreach (GroupScheduleDetailEntity entity in scheduleDetails[0].GroupScheduleDetails)
                {
                    serviceFormMail = new GroupScheduleServiceMailEntity();
                    serviceFormMail.Group_ID = scheduleDetails[0].Group_ID;
                    serviceFormMail.Group_Name = scheduleDetails[0].Group_Name;
                    serviceFormMail.Env_ID = entity.Env_ID;
                    serviceFormMail.Env_Name = entity.Env_Name;
                    var groupScheduleDatatime = scheduleDetails[0].Group_Schedule_Datatime;
                    if (groupScheduleDatatime != null)
                        serviceFormMail.Group_Schedule_Datatime = (DateTime) groupScheduleDatatime;
                    if (scheduleDetails[0].Group_Schedule_Action == "1")
                        serviceFormMail.Group_Schedule_Action = "Start";
                    else if (scheduleDetails[0].Group_Schedule_Action == "2")
                        serviceFormMail.Group_Schedule_Action = "Stop";
                    else if (scheduleDetails[0].Group_Schedule_Action == "3")
                        serviceFormMail.Group_Schedule_Action = "Restart";

                    if (scheduleDetails[0].Group_Schedule_Status == "O")
                        serviceFormMail.Group_Schedule_Status = "Scheduled";
                    else if (scheduleDetails[0].Group_Schedule_Status == "P")
                        serviceFormMail.Group_Schedule_Status = "Pending";
                    else if (scheduleDetails[0].Group_Schedule_Status == "C")
                        serviceFormMail.Group_Schedule_Status = "Completed";

                    serviceFormMail.Group_Schedule_Comments = scheduleDetails[0].Group_Schedule_Comments;
                    var groupScheduleCompletedTime = scheduleDetails[0].Group_Schedule_CompletedTime;
                    if (groupScheduleCompletedTime != null)
                        serviceFormMail.Group_Schedule_CompletedTime = (DateTime) groupScheduleCompletedTime;
                    serviceFormMail.Group_Schedule_CreatedDatetime = scheduleDetails[0].Group_Schedule_CreatedDatetime;

                    serviceFormMail.WindowsServices = "<table style=" + tableStyle + ">";
                    serviceFormMail.WindowsServices += "<tr>";
                    serviceFormMail.WindowsServices += "<th style=" + heaferStyle + ">Server</th> <th style=" + heaferStyle + ">Port</th> <th  style=" + heaferStyle + ">Service Type</th><th  style=" + heaferStyle + ">Service Name</th>";
                    serviceFormMail.WindowsServices += "</tr>";
                    /*
                    foreach (GroupScheduleServiceDetailEntity grpService in entity.ServiceDetails)
                    {
                        serviceFOrmMail.HostIP += grpService.HostIP + ", ";
                    }
                    serviceFOrmMail.HostIP = serviceFOrmMail.HostIP.Substring(0, serviceFOrmMail.HostIP.Length - 2);
                    foreach (GroupScheduleServiceDetailEntity grpService in entity.ServiceDetails)
                    {
                        serviceFOrmMail.WindowsServices += grpService.WindowsService_Name + ", ";
                    }
                    serviceFOrmMail.WindowsServices = serviceFOrmMail.WindowsServices.Substring(0, serviceFOrmMail.WindowsServices.Length - 2);
                    */
                    foreach (GroupScheduleServiceDetailEntity grpService in entity.ServiceDetails)
                    {
                        serviceFormMail.WindowsServices += "<tr>";
                        serviceFormMail.WindowsServices += "<td  style=" + tdHost + ">" + grpService.HostIP + "</td> <td  style=" + tdStyleCenter + ">" + grpService.Port + "</td> <td  style=" + tdCommon + ">" + grpService.Service_Name + "</td> <td  style=" + tdCommon + ">" + grpService.WindowsService_Name + "</td>";
                        serviceFormMail.WindowsServices += "</tr>";
                    }
                    serviceFormMail.WindowsServices += "</table>";
                    context = HttpContext.Current;
                    
                    if (context != null && context.Session["_LOGGED_USERNAME"] != null &&
                        !string.IsNullOrEmpty(context.Session["_LOGGED_USERNAME"].ToString()))
                        serviceFormMail.Group_Schedule_CreatedBy = context.Session["_LOGGED_USERNAME"].ToString();
                    else
                        serviceFormMail.Group_Schedule_CreatedBy = "Unknown";

                    serviceFormMail.TimeZone = localZone.StandardName.ToString();

                    mailService.SendMail(entity.Env_ID, 0,
                        grpStatus == 'C' ? _scheduleWindowsServiceOperationCancelled : _groupscheduleMail, serviceFormMail);
                    Thread.Sleep(1000);
                }
            }
        }

        #endregion

        #region Windows Service Status
        /// <summary>
        /// Get the windows service status 
        /// </summary>
        /// <param name="ServiceName"></param>
        /// <returns></returns>
        public string GetWindowsServiceStatus(string ServiceName, string systemName)
        {
            string serviceStatus = string.Empty;
            using (ServiceController sc = !string.IsNullOrEmpty(systemName) ? new ServiceController(ServiceName, systemName) : new ServiceController(ServiceName))
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
                finally { }
            }
            return serviceStatus;
        }
        #endregion Windows Service Status

        #region windows service operation

        public string WindowServiceFuction(string serviceName, string serviceAction, string systemName = null,
            string requestSource = null)
        {
            if (serviceName.ToLower().Contains("cosmo"))
                systemName = "localhost";

            var uri =
                                    string.Format(
                                        RemoteSystemUrlForServiceStatus + "{3}?svn={1}&t={2}&srn={0}",
                                        systemName, serviceName, serviceTimeout,
                                        serviceAction == "stop"
                                            ? "StopService"
                                            : serviceAction == "start"
                                                ? "StartService"
                                                : serviceAction == "restart" ? "RestartService" : string.Empty);

            Logger.Log("Uri: " + uri);
            var wrapper = new WebHttpRequestBuilder();
            var request = wrapper.CreateGetRequest(uri);
            
            var response = monitorService.GetWebResponse<WindowServiceStatus>(request);
            Logger.Log("ServiceStatus: " + response.ServiceStatus);

            return response.ServiceStatus;
        }

        public string WindowServiceFuction1(string serviceName, string serviceAction, string systemName = null, string requestSource = null)
        {
            string result = null;
            var isProcessing = true;
            var sc = !string.IsNullOrEmpty(systemName) ? new ServiceController(serviceName, systemName) : new ServiceController(serviceName);

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

        #region Get group ID
        /// <summary>
        /// Get group ID based on the group Name
        /// </summary>
        /// <param name="grpName"></param>
        /// <returns></returns>
        public List<GroupEntity> GetGroupID(string grpName)
        {
            return _winData.GetGroupID(grpName);
        }
        #endregion Get group ID

        #region Get Window Service configuration details of OnDemand for Mail

        public List<ServiceOnDemandMailEntity> GetServiceConfigurationOndemand(int winSerId)
        {
            return _winData.GetServiceConfigurationOndemand(winSerId);
        }

        public void SendMailServiceRestartOnDemand(ServiceOnDemandMailEntity demand)
        {
            int insertResult = 0;
            demand.ServerTimeZone = localZone.StandardName.ToString();
            context = HttpContext.Current;
            //to insert the ondemand details to DB
            insertResult = InsGroupScheduleDetail(demand);

            userService = new UserService();
            var user = userService.GetUsers(Convert.ToInt32(demand.RequestedBy));
            if (user != null && user.Count > 0)
                demand.RequestedBy = user[0].FirstName + " " + user[0].LastName;


            //if (context.Session != null && context.Session["_LOGGED_USERNAME"] != null && !string.IsNullOrEmpty(context.Session["_LOGGED_USERNAME"].ToString()))
            //    demand.RequestedBy = context.Session["_LOGGED_USERNAME"].ToString();
            //else

            if (demand.RequestStatus.ToLower() == "start") demand.RequestStatusCompletion = "Started";
            else if (demand.RequestStatus.ToLower() == "stop") demand.RequestStatusCompletion = "Stopped";
            else if (demand.RequestStatus.ToLower() == "restart") demand.RequestStatusCompletion = "Restarted";

            demand.Comments = "Request through Ondemand";
            demand.Comments += demand.RequestSource.ToLowerInvariant().Contains("mobile") ? " (Mobile)" : string.Empty;

            mailService.SendMail(demand.EnvId, demand.ConfigId, _servicerestartondemand, demand);

        }

        #endregion Get Window Service configuration details of OnDemand for Mail

        #region Get report of Group schedule details

        public List<GroupcheduleReportEntity> GetGroupScheduleReport(string schType, DateTime startDate, DateTime endDate)
        {
            var groupSchedules = _winData.GetGroupScheduleReport(schType, startDate,endDate);

            return groupSchedules;
        }
        #endregion Get report of Group schedule details

        #region GetWindowssService
        public string GetWindowssService_old(string serviceName, string serviceMode, string systemName, string serviceId, string requestSource)
        {
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

                    var serviceOnDemand = GetServiceConfigurationOndemand(!string.IsNullOrEmpty(serviceId) ? Convert.ToInt32(serviceId) : 0);

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
                                        RemoteSystemUrlForServiceStatus + "{3}?svn={1}&t={2}&srn={0}",
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

                                var wrapper = new WebHttpRequestBuilder();
                                var request = wrapper.CreateGetRequest(uri);
                                var response = monitorService.GetWebResponse<WindowServiceStatus>(request);

                                result = response.ServiceStatus;

                                isTimeout = false;

                                serviceOnDemand[0].WindowServiceStatus = result;
                                serviceOnDemand[0].CompletionStatus = "Successful";
                            }
                            catch (AggregateException agex)
                            {
                                
                                Logger.Log("Agg - Windows Service operation: " + agex.ToString());
                                Logger.Log("Agg - Windows Service operation: " + agex.StackTrace);
                                result = "cancelled";
                                serviceOnDemand[0].WindowServiceStatus = result;
                                serviceOnDemand[0].CompletionStatus = "Unsuccessful";
                                throw;
                            }
                            catch (Exception ex)
                            {
                                Logger.Log("Windows Service general operation: " + ex);

                                if (ex.Message.Contains("timed out"))
                                {
                                    result = "timedout";
                                    serviceOnDemand[0].WindowServiceStatus = serviceOnDemand[0].CompletionStatus = "Timed out";
                                }
                                else if (ex.Message.Contains("Unable to connect"))
                                {
                                    result = "unabletoconnect";
                                    serviceOnDemand[0].WindowServiceStatus = "cancelled";
                                    serviceOnDemand[0].CompletionStatus = "Unsuccessful";
                                }
                                else
                                {
                                    result = "cancelled";
                                    serviceOnDemand[0].WindowServiceStatus = result;
                                    serviceOnDemand[0].CompletionStatus = "Unsuccessful";
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
                                }
                                else if (result == "timedout")
                                {
                                    result = "timedout";
                                    serviceOnDemand[0].WindowServiceStatus = result;
                                    serviceOnDemand[0].CompletionStatus = "Unsuccessful";
                                }
                                else
                                {
                                    
                                }
                                serviceOnDemand[0].Comments = "Ondemand";
                            }
                            else
                                result = "completed";

                            SendMailServiceRestartOnDemand(serviceOnDemand[0]);
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
        }

        public string GetWindowssService(string serviceName, string serviceMode, string systemName, string serviceId, string requestSource, int requesterId = 0)
        {
            var processRunning = true;
            string result = string.Empty; string currentStatus = string.Empty;
            bool isTimeout = true;
            bool isProcessing = true;
            string serviceAction = string.Empty;
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            CancellationToken token = cancellationTokenSource.Token;

            try
            {
                Logger.Log("Checking monitor process status");
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
                Logger.Log("Monitor process status: Not running");


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
                    var serviceOnDemand = GetServiceConfigurationOndemand(Convert.ToInt32(serviceId));

                    if (serviceOnDemand != null && serviceOnDemand.Count > 0)
                    {
                        serviceOnDemand[0].RequestStatus = serviceAction.ToTitleCase();
                        serviceOnDemand[0].RequestedDateTime = DateTime.Now;
                        serviceOnDemand[0].OnDemand = true;
                        serviceOnDemand[0].RequestSource = requestSource;
                        serviceOnDemand[0].RequestedBy = requesterId.ToString();
                        serviceOnDemand[0].Comments = "OnDemand service operation";


                        var t = Task.Factory.StartNew(() =>
                        {
                            try
                            {
                                var uri =
                                    string.Format(
                                        RemoteSystemUrlForServiceStatus + "{3}?svn={1}&t={2}&srn={0}",
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
                                    CreatedBy = requesterId.ToString(),
                                };

                                SetAcknowledge(groupService, "stop");


                                var wrapper = new WebHttpRequestBuilder();
                                var request = wrapper.CreateGetRequest(uri);
                                var response = monitorService.GetWebResponse<WindowServiceStatus>(request);

                                result = response.ServiceStatus;

                                isTimeout = false;

                                serviceOnDemand[0].WindowServiceStatus = result;
                                serviceOnDemand[0].CompletionStatus = "Successful";
                                serviceOnDemand[0].GroupScheduledStatus = "S";

                                if (serviceAction != "stop")
                                    SetAcknowledge(groupService, "start");

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
                                else if (!string.IsNullOrEmpty(result))
                                {
                                    result = "timedout";
                                    serviceOnDemand[0].WindowServiceStatus = result;
                                    serviceOnDemand[0].CompletionStatus = "Timed out";
                                    serviceOnDemand[0].GroupScheduledStatus = "T";

                                }
                                serviceOnDemand[0].Comments = "Ondemand";
                            }
                            
                            serviceOnDemand[0].ServiceCompletionTime = DateTime.Now;

                            SendMailServiceRestartOnDemand(serviceOnDemand[0]);
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
        }

        #endregion GetWindowssService

        #region Cancel the group schedule and detail based on the GROUP_SCH_ID

        /// <summary>
        /// Soft delete a group scheduler amd detail based on the GROUP_SCH_ID
        /// </summary>
        /// <param name="type"></param>
        /// <param name="vId"></param>

        public void CancelGroupSchedule(string type, string vId)
        {
            try
            {
                _winData.CancelGroupSchedule(type, vId);
                SendMailGroupSchedule(Convert.ToInt32(vId), 'C');
            }
            catch (SqlException exception)
            {
                Logger.Log("Groups Schedule Cancel status: " + exception);
                throw new ApplicationException(exception.Message);
            }
        }

        #endregion Cancel the group schedule and detail based on the GROUP_SCH_ID


        public void SetAcknowledge(GroupScheduleServiceDetailEntity detailsServiceDetailEntity, string mode)
        {
            var setServiceAck = new AcknowledgeEntity
            {
                EnvId = detailsServiceDetailEntity.Env_ID,
                ConfigId = detailsServiceDetailEntity.Config_ID,
                MonId = 0,
                IsAcknowledgeMode = true,
                AcknowledgeAlertChange = mode,
                AcknowledgeComments = "OnDemand service operation",
                CreatedBy = detailsServiceDetailEntity.CreatedBy,
                CreatedDate = DateTime.Now
            };
            Logger.Log(string.Format("Acknowledge to '{0}' for Env id '{1}', Config Id '{2}', Host '{3}' and Port '{4}' from service operation", mode, detailsServiceDetailEntity.Env_ID, detailsServiceDetailEntity.Config_ID, detailsServiceDetailEntity.HostIP, detailsServiceDetailEntity.Port));
            Logger.Log("Acknowledge Comments: " + setServiceAck.AcknowledgeComments);
            _monitorData.InsUpdateServiceAcknowledge(setServiceAck, string.Empty);
        }

    }
}
