using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.ServiceProcess;
using System.Web;
using System.Web.Script.Serialization;
using Cosmo.Data;
using Cosmo.Entity;

namespace Cosmo.Service
{
    public class MonitorService
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
        private const string ServiceStatuschanging = "Status Changing";
        private const string ServiceNameNotfound = "Not Exists!";
        private const string ServiceNameNotreachable = "Not Reachable";
        #endregion Constants

        #region object declaration

        IMonitorData monitorData;
        MailService mailService = new MailService();
        SchedulerServices schedulerServices = new SchedulerServices();

        private readonly JavaScriptSerializer _javaScriptSerializer;
        private readonly WebHttpRequestBuilder _webHttpRequestBuilder;

        #endregion object declaration

        #region Variables

        private static readonly string DbType = ConfigurationManager.AppSettings["DatabaseType"];
        private string ACKNOWLEDGE_MAIL = Convert.ToString(ConfigurationManager.AppSettings["Acknowledge"]);
        private string ACKNOWLEDGE_ALERT_STOPPED_MAIL = Convert.ToString(ConfigurationManager.AppSettings["Acknowledge_Alert_Stopped"]);
        private string ACKNOWLEDGE_ALERT_STARTED_MAIL = Convert.ToString(ConfigurationManager.AppSettings["Acknowledge_Alert_Started"]);
        private string CONTENT_MANAGER = Convert.ToString(ConfigurationManager.AppSettings["ContentService"]);
        private string DESPATCHER = Convert.ToString(ConfigurationManager.AppSettings["DispatcherService"]);
        private static string RemoteSystemURLForServiceStatus = ConfigurationManager.AppSettings["RemoteSystemURLForServiceStatus"].ToString();
        private static string RemoteSystemURLForServerPerformance = ConfigurationManager.AppSettings["RemoteSystemURLForServerPerformance"].ToString();

        private const string ALL_SERVICES_STOPPED = "All services are stopped";
        private const string SOME_SERVICES_STOPPED = "One or more services are stopped";
        private const string ALL_SERVICES_RUNNING = "All services are running";
        private const string SOME_SERVICES_NOTREADY = "One or more services are not ready";
        private const string SERVICE_NOTAVAILABLE= "Not Available";
        private const string SERVICE_PARTIALAVAILABLE = "One or more arevices are Not Available";

        #endregion Variables

        public MonitorService() : this(new JavaScriptSerializer(), new WebHttpRequestBuilder())
        {
            var iDbType = DbType == DatabaseType.Oracle.ToString()
                ? Convert.ToInt32(DatabaseType.Oracle).ToString()
                : Convert.ToInt32(DatabaseType.SqlServer).ToString();

           monitorData = new MonitorDataFactory().Create(Convert.ToInt32(iDbType).ToString());
        }

        private MonitorService(JavaScriptSerializer javaScriptSerializer, WebHttpRequestBuilder webHttpRequestBuilder)
        {
            _javaScriptSerializer = javaScriptSerializer;
            _webHttpRequestBuilder = webHttpRequestBuilder;
        }

        /// <summary>
        /// Get current monitor status all environments
        /// </summary>
        /// <param name="env_id"></param>
        /// <returns></returns>
        public List<ServiceMoniterEntity> GetAllMonitors(int envIid, bool isWithServiceName)
        {
            var isNotify = false;
            try
            {
                var monList = monitorData.GetAllMonitors(envIid, isWithServiceName);
                var monListWithStatusIcon = UpdateMonitorStatus(monList, isWithServiceName);
                return monListWithStatusIcon;
            }
            catch (Exception ex)
            {
                Logger.Log(ex.ToString());
                throw ex;
            }

        }

        public List<ServiceMoniterEntity> UpdateMonitorStatus(List<ServiceMoniterEntity> monList, bool isWithServiceName)
        {
            var isNotify = false;

            //var windowServiceStatusList = isWithServiceName ? GetRemoteWindowsServiceStatus(monList) : new List<WindowServiceStatus>();
            var windowServiceStatusList = new List<WindowServiceStatus>();
            
            foreach (var monitor in monList)
            {
                if (monitor == null || monitor.monitorList == null || monitor.monitorList.Count <= 0) continue;
                if (monitor.monitorList.All(at => at.MonitorStatus.ToLower() == "stopped"))
                {
                    monitor.OverallStatus = new EnvironmentMonitorStatus
                    {
                        Id = Convert.ToInt32( EnvironmentMonitorStatus.StatusCode.Stopped),
                        ImagePath = "../images/red1_icon.jpg",
                        Description = ALL_SERVICES_STOPPED
                    };
                }
                else if (
                    monitor.monitorList.Any(
                        at =>
                            at.MonitorStatus.ToLower().Contains("stopped") ||
                            at.MonitorStatus.ToLower().Contains("not ready")))
                {
                    monitor.OverallStatus = new EnvironmentMonitorStatus
                    {
                        Id = Convert.ToInt32(EnvironmentMonitorStatus.StatusCode.OneOrMoreStopped),
                        ImagePath = "../images/red_icon.jpg",
                        Description = SOME_SERVICES_STOPPED
                    };
                }
                else if (monitor.monitorList.All(at => at.MonitorStatus.ToLower().Contains("running")))
                {
                    monitor.OverallStatus = new EnvironmentMonitorStatus
                    {
                        Id = Convert.ToInt32(EnvironmentMonitorStatus.StatusCode.Running),
                        ImagePath = "../images/green_icon.jpg",
                        Description = ALL_SERVICES_RUNNING
                    };
                }
                else if (monitor.monitorList.Any(at => at.MonitorStatus.ToLower().Contains("standby")) &&
                         monitor.monitorList.Any(at => at.MonitorStatus.ToLower().Contains("running")))
                {
                    monitor.OverallStatus = new EnvironmentMonitorStatus
                    {
                        Id = Convert.ToInt32(EnvironmentMonitorStatus.StatusCode.Running),
                        ImagePath = "../images/green_icon.jpg",
                        Description = ALL_SERVICES_RUNNING
                    };
                }
                else if (monitor.monitorList.All(at => at.MonitorStatus.ToLower() == string.Empty))
                {
                    monitor.OverallStatus = new EnvironmentMonitorStatus
                    {
                        Id = Convert.ToInt32(EnvironmentMonitorStatus.StatusCode.NotAvailable),
                        ImagePath = "../images/gray_icon.png",
                        Description = SERVICE_NOTAVAILABLE
                    };
                }
                else if (monitor.monitorList.Any(at => at.MonitorStatus.ToLower() == string.Empty) ||
                         monitor.monitorList.Any(at => at.MonitorStatus.ToLower().Contains("running")))
                {
                    monitor.OverallStatus = new EnvironmentMonitorStatus
                    {
                        Id = Convert.ToInt32(EnvironmentMonitorStatus.StatusCode.PartiallyAvailable),
                        ImagePath = "../images/yellow_icon.png",
                        Description = SERVICE_PARTIALAVAILABLE
                    };
                }
                else
                {
                    monitor.OverallStatus = new EnvironmentMonitorStatus
                    {
                        Id = Convert.ToInt32(EnvironmentMonitorStatus.StatusCode.NotAvailable),
                        ImagePath = "../images/blue_icon1.jpg",
                        Description = SERVICE_NOTAVAILABLE
                    };
                }

                foreach (var monitorEntity in monitor.monitorList)
                {
                    isNotify = false;

                    switch (monitorEntity.MonitorStatus.ToLower())
                    {
                        case "stopped":
                            monitorEntity.MonitorStatusIcon = "red1_icon.jpg";
                            isNotify = true;
                            break;
                        case "running":
                            monitorEntity.MonitorStatusIcon = "green_icon.jpg";
                            break;
                        case "standby":
                            monitorEntity.MonitorStatusIcon = "orange_icon.jpg";
                            break;
                        default:
                            if (monitorEntity.MonitorStatus.ToLower().Contains("not running"))
                            {
                                monitorEntity.MonitorStatusIcon = "red_icon.jpg";
                                isNotify = true;
                            }
                            else if (monitorEntity.MonitorStatus.ToLower() == "not ready")
                            {
                                monitorEntity.MonitorStatusIcon = "blue_icon.jpg";
                                isNotify = true;
                            }
                            else if (string.IsNullOrEmpty(monitorEntity.MonitorStatus))
                            {
                                monitorEntity.MonitorStatusIcon = "gray_icon.png";
                                monitorEntity.MonitorStatus = "Not avaialble";
                                isNotify = true;
                            }
                            break;
                    }


                    if (monitorEntity.ScheduleID > 0)
                    {
                        if (isNotify && monitorEntity.ConfigIsNotify)
                        {
                            if (monitorEntity.IsAcknowledge)
                            {
                                monitorEntity.MonitorNotifyIcon = "../images/email-acknowledge-icon_24.png";
                                monitorEntity.MonitorNotifyTooltip = "Acknowledged and alert being sent";
                            }
                            else
                            {
                                monitorEntity.MonitorNotifyIcon = "../images/email-send-icon_24.png";
                                monitorEntity.MonitorNotifyTooltip = "Alert being sent";
                            }

                        }
                        else if (isNotify && monitorEntity.ConfigIsNotify == false)
                        {
                            monitorEntity.MonitorNotifyIcon = "../images/email-delete-icon_24.png";
                            monitorEntity.MonitorNotifyTooltip = "Alert has stopped";
                        }
                    }

                    if (isWithServiceName)
                    {
                        monitorEntity.WindowsServiceStatus = string.Empty;
                    }

                    /*
                    if (isWithServiceName)
                    {
                        if (string.IsNullOrEmpty(monitorEntity.WindowsServiceName)) continue;
                        
                        var entity = monitorEntity;
                        var currentServiceStatus = new WindowServiceStatus();

                        if (windowServiceStatusList.Any(
                            sn =>
                                sn.ServiceName.ToLower(CultureInfo.CurrentCulture)
                                    .Equals(entity.WindowsServiceName.ToLower(CultureInfo.CurrentCulture)) && (!string.IsNullOrEmpty(sn.SystemName)) &&
                                (sn.SystemName.ToLower(CultureInfo.CurrentCulture))
                                    .Equals(entity.ConfigHostIP.ToLower(CultureInfo.CurrentCulture))))
                            currentServiceStatus = windowServiceStatusList.Where(
                                sn =>
                                    sn.ServiceName.ToLower(CultureInfo.CurrentCulture)
                                        .Equals(entity.WindowsServiceName.ToLower(CultureInfo.CurrentCulture)) &&
                                    sn.SystemName.ToLower(CultureInfo.CurrentCulture)
                                        .Equals(entity.ConfigHostIP.ToLower(CultureInfo.CurrentCulture))).ToList()[0];

                        else if (windowServiceStatusList.Any(
                            sn =>
                                (!string.IsNullOrEmpty(sn.SystemName)) &&
                                (sn.SystemName.ToLower(CultureInfo.CurrentCulture))
                                    .Equals(entity.ConfigHostIP.ToLower(CultureInfo.CurrentCulture))))
                        {
                            currentServiceStatus.ServiceStatus = ServiceNameNotfound;
                        }
                        else 
                        {
                            currentServiceStatus.ServiceStatus = ServiceNameNotreachable;
                        }

                        Logger.Log(monitorEntity.WindowsServiceName + " - " + monitorEntity.ConfigHostIP + " - " + currentServiceStatus.ServiceStatus + " - ");
                        
                        if (string.IsNullOrEmpty(currentServiceStatus.ServiceStatus)) continue;
                        
                        //monitorEntity.WindowsServiceStatus = currentServiceStatus.ServiceStatus;
                        monitorEntity.WindowsServiceStatus = string.Empty;

                        switch (currentServiceStatus.ServiceStatus)
                        {
                            case ServiceRunning:
                                monitorEntity.ServiceStrategy = new WindowsServiceStrategy { Start = false, Restart = true, Stop = true };
                                break;
                            case ServiceStarting:
                            case ServiceStatuschanging:
                                monitorEntity.ServiceStrategy = new WindowsServiceStrategy { Start = false, Restart = false, Stop = true };
                                break;
                            case ServiceStopped:
                                monitorEntity.ServiceStrategy = new WindowsServiceStrategy { Start = true, Restart = false, Stop = false };
                                break;
                            case ServiceNameNotfound:
                                monitorEntity.ServiceStrategy = new WindowsServiceStrategy { Start = false, Restart = false, Stop = false };
                                break;
                            default:
                                monitorEntity.ServiceStrategy = new WindowsServiceStrategy { Start = false, Restart = false, Stop = false };
                                break;
                        }

                        
                    }
                    */
                }
            }
            return monList;
        }


        public ServiceMoniterEntity UpdateWindowsServicetatus(ServiceMoniterEntity monitor)
        {
            Logger.Log(string.Format("Getting windows service status for {0}", monitor.EnvName));
            
            var windowServiceStatusList = GetRemoteWindowsServiceStatus(monitor);

            foreach (var monitorEntity in monitor.monitorList)
            {
                if (string.IsNullOrEmpty(monitorEntity.WindowsServiceName)) continue;
                var entity = monitorEntity;
                var currentServiceStatus = new WindowServiceStatus();

                if (windowServiceStatusList.Any(
                    sn =>
                        sn.ServiceName.ToLower(CultureInfo.CurrentCulture)
                            .Equals(entity.WindowsServiceName.ToLower(CultureInfo.CurrentCulture)) && (!string.IsNullOrEmpty(sn.SystemName)) &&
                        (sn.SystemName.ToLower(CultureInfo.CurrentCulture))
                            .Equals(entity.ConfigHostIP.ToLower(CultureInfo.CurrentCulture))))
                    currentServiceStatus = windowServiceStatusList.Where(
                        sn =>
                            sn.ServiceName.ToLower(CultureInfo.CurrentCulture)
                                .Equals(entity.WindowsServiceName.ToLower(CultureInfo.CurrentCulture)) &&
                            sn.SystemName.ToLower(CultureInfo.CurrentCulture)
                                .Equals(entity.ConfigHostIP.ToLower(CultureInfo.CurrentCulture))).ToList()[0];

                else if (windowServiceStatusList.Any(
                    sn =>
                        (!string.IsNullOrEmpty(sn.SystemName)) &&
                        (sn.SystemName.ToLower(CultureInfo.CurrentCulture))
                            .Equals(entity.ConfigHostIP.ToLower(CultureInfo.CurrentCulture))))
                {
                    currentServiceStatus.ServiceStatus = ServiceNameNotfound;
                }
                else
                {
                    currentServiceStatus.ServiceStatus = ServiceNameNotreachable;
                }

                Logger.Log(monitorEntity.WindowsServiceName + " - " + monitorEntity.ConfigHostIP + " - " + currentServiceStatus.ServiceStatus + " - ");

                if (string.IsNullOrEmpty(currentServiceStatus.ServiceStatus)) continue;

                monitorEntity.WindowsServiceStatus = currentServiceStatus.ServiceStatus;

                switch (currentServiceStatus.ServiceStatus)
                {
                    case ServiceRunning:
                        monitorEntity.ServiceStrategy = new WindowsServiceStrategy { Start = false, Restart = true, Stop = true };
                        break;
                    case ServiceStarting:
                    case ServiceStatuschanging:
                        monitorEntity.ServiceStrategy = new WindowsServiceStrategy { Start = false, Restart = false, Stop = true };
                        break;
                    case ServiceStopped:
                        monitorEntity.ServiceStrategy = new WindowsServiceStrategy { Start = true, Restart = false, Stop = false };
                        break;
                    case ServiceNameNotfound:
                        monitorEntity.ServiceStrategy = new WindowsServiceStrategy { Start = false, Restart = false, Stop = false };
                        break;
                    default:
                        monitorEntity.ServiceStrategy = new WindowsServiceStrategy { Start = false, Restart = false, Stop = false };
                        break;
                }
            }

            return monitor;
        }

        /// <summary>
        /// Get Service availability for all the environments
        /// </summary>
        /// <param name="env_id"></param>
        /// <param name="startTime"></param>
        /// <returns></returns>
        public List<ServiceMoniterEntity> GetServiceAvailability(int env_id, DateTime startTime, DateTime endTime, string sType, bool isWithServiceName)
        {
            return monitorData.GetServiceAvailability(env_id, startTime, endTime, sType, isWithServiceName);
        }

        /// <summary>
        /// Get Service downtime for all the environments
        /// </summary>
        /// <param name="env_id"></param>
        /// <param name="startTime"></param>
        /// <returns></returns>
        public List<UpDownTimeEntity> GetServiceUpDownTime(int env_id, DateTime startTime, DateTime endTime)
        {
            List<UpDownTimeEntity> upDownTimeList = new List<UpDownTimeEntity>();
            UpDownTimeEntity  upDownTime = new UpDownTimeEntity();
            List<ServiceMoniterEntity> endServiceEntity = new List<ServiceMoniterEntity>();
            List<MonitorEntity> configIDList = new List<MonitorEntity>();
            List<MonitorEntity> tempMonitorList = new List<MonitorEntity>();
            
            
            TimeSpan span;
            double totalDownTimeInMinutes = 0;
            double totalAvailableMinutes = 0;
            DateTime[] timeDuration = new DateTime[2];
            DateTime envSchStartTime = DateTime.Now;
            try
            {
                span = endTime.Subtract(startTime);
                totalAvailableMinutes = (int)span.TotalMinutes;
                endServiceEntity = monitorData.GetServiceDownTime(env_id, startTime, endTime);
                if (endServiceEntity != null && endServiceEntity.Count > 0)
                {
                    foreach (ServiceMoniterEntity list in endServiceEntity)
                    {

                        totalDownTimeInMinutes = 0;
                        timeDuration = new DateTime[2];

                        #region//find out environment schedule start date time

                        envSchStartTime = GetScheduleStartDateTime(list.EnvID);
                        if (envSchStartTime > startTime)
                        {
                            span = endTime.Subtract(envSchStartTime);
                        }
                        else
                        {
                            span = endTime.Subtract(startTime);
                        }
                        totalAvailableMinutes = (int)span.TotalMinutes;

                        #endregion//find out environment schedule start date time

                        if (list.monitorList != null && list.monitorList.Count > 0)
                        {
                            configIDList = list.monitorList.GroupBy(x => x.ConfigID).Select(y => y.First()).ToList<MonitorEntity>();
                            foreach (MonitorEntity monitor in configIDList)
                            {
                                tempMonitorList = new List<MonitorEntity>();
                                if (list.monitorList.Any(mo => mo.ConfigID == monitor.ConfigID))
                                {
                                    tempMonitorList = list.monitorList.Where(mo => mo.ConfigID == monitor.ConfigID).ToList<MonitorEntity>();
                                    if (tempMonitorList.Count > 0)
                                    {
                                        if (tempMonitorList[0].MonitorCreatedDateTime >= timeDuration[1])
                                        {
                                            if (tempMonitorList[0].MonitorCreatedDateTime <= startTime)
                                                timeDuration[0] = startTime;
                                            else
                                            {
                                                var monitorCreatedDateTime = tempMonitorList[0].MonitorCreatedDateTime;
                                                if (monitorCreatedDateTime != null)
                                                    timeDuration[0] = (DateTime) monitorCreatedDateTime;
                                            }
                                            var monitorUpdatedDateTime = tempMonitorList[tempMonitorList.Count - 1].MonitorUpdatedDateTime;
                                            if (monitorUpdatedDateTime !=
                                                null)
                                                timeDuration[1] = (DateTime) monitorUpdatedDateTime;
                                            span = new TimeSpan();
                                            span = timeDuration[1].Subtract(timeDuration[0]);
                                            totalDownTimeInMinutes += (int)span.TotalMinutes;
                                        }
                                        else
                                        {
                                            foreach (MonitorEntity findDate in tempMonitorList)
                                            {
                                                if (findDate.MonitorCreatedDateTime > timeDuration[1])
                                                {
                                                    if (findDate.MonitorCreatedDateTime != null)
                                                        timeDuration[0] = (DateTime) findDate.MonitorCreatedDateTime;
                                                    var monitorUpdatedDateTime = tempMonitorList[tempMonitorList.Count - 1].MonitorUpdatedDateTime;
                                                    if (
                                                        monitorUpdatedDateTime != null)
                                                        timeDuration[1] = (DateTime) monitorUpdatedDateTime;
                                                    span = new TimeSpan();
                                                    span = timeDuration[1].Subtract(timeDuration[0]);
                                                    totalDownTimeInMinutes += (int)span.TotalMinutes;
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else {
                            totalDownTimeInMinutes = totalAvailableMinutes;
                        }

                        upDownTime = new UpDownTimeEntity();
                        upDownTime.Env_Id = list.EnvID;
                        upDownTime.Env_name = list.EnvName;
                        upDownTime.TotalTime = totalAvailableMinutes;
                        upDownTime.DownTime = totalDownTimeInMinutes;
                        upDownTime.UpTime = totalAvailableMinutes - totalDownTimeInMinutes;
                        if (totalAvailableMinutes <= 0)
                        {
                            upDownTime.DownTimePercent = 0;
                            upDownTime.UpTimePercent = 0;
                        }
                        else
                        {
                            upDownTime.DownTimePercent = Math.Round((totalDownTimeInMinutes / totalAvailableMinutes) * 100, 0);
                            upDownTime.UpTimePercent = Math.Round(((totalAvailableMinutes - totalDownTimeInMinutes) / totalAvailableMinutes) * 100, 0);
                        }
                        upDownTimeList.Add(upDownTime);
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return upDownTimeList;
        }

        /// <summary>
        /// Get current build version details for all the services that are available in all the environment
        /// </summary>
        /// <param name="env_id"></param>
        /// <returns></returns>
        public List<ServiceMoniterEntity> GetServiceBuildVersionReport(int env_id)
        {
            var monitorList =  monitorData.GetServiceBuildVersionReport(env_id);

            if (monitorList != null && monitorList.Count > 0)
            {
                foreach (var serviceMoniterEntity in monitorList)
                {
                    foreach (var monitorEntity in serviceMoniterEntity.monitorList)
                    {
                        monitorEntity.EnvName = serviceMoniterEntity.EnvName;
                    }
                }
            }
            return monitorList;
        }

        /// <summary>
        /// Get build history version details for all the services that are available in all the environment
        /// </summary>
        /// <param name="env_id"></param>
        /// <returns></returns>
        public List<ServiceMoniterEntity> GetServiceBuildHistoryReport(int env_id, DateTime startDate, DateTime endDate)
        {
            MonitorEntity tempMonitor = new MonitorEntity();
            List<ServiceMoniterEntity> monitorHistory = new List<ServiceMoniterEntity>();
            List<int> tempConfig = new List<int>();
            List<MonitorEntity> tempMonitorEtity = new List<MonitorEntity>();

            monitorHistory =  monitorData.GetServiceBuildHistoryReport(env_id, startDate, endDate);
            if (monitorHistory != null && monitorHistory.Count > 0)
            { 
                foreach(ServiceMoniterEntity envHistory in monitorHistory)
                {
                    tempMonitor = new MonitorEntity();
                    if (envHistory.monitorList != null && envHistory.monitorList.Count > 0)
                    {
                        if (envHistory.monitorList.Count > 1)
                        {
                            //var uniqueConfig = from uConfig in envHistory.monitorList
                            //                   group uConfig by new { uConfig.ConfigID }
                            //                       into myConfig
                            //                       select myConfig.Distinct();
                            tempConfig = new List<int>();
                            tempMonitorEtity = new List<MonitorEntity>();
                            tempConfig = envHistory.monitorList.Select(ml => ml.ConfigID).Distinct().ToList();

                            foreach (int i in tempConfig)
                            {
                                tempMonitorEtity = new List<MonitorEntity>();
                                tempMonitorEtity = envHistory.monitorList.Where(ml => ml.ConfigID == i).ToList<MonitorEntity>();
                                if (tempMonitorEtity.Count > 0)
                                {
                                    if (tempMonitorEtity.Count > 1)
                                    {
                                        foreach (MonitorEntity monitor in tempMonitorEtity)
                                        {
                                            if (tempMonitor != null && tempMonitor.ConfigID > 0)
                                            {
                                                tempMonitor.MonitorUpdatedDateTime = monitor.MonitorCreatedDateTime.AddHours(-1);
                                                monitor.MonitorUpdatedDateTime = DateTime.Now;
                                            }
                                            tempMonitor = monitor;
                                        }
                                    }
                                    else
                                    {
                                        tempMonitorEtity[0].MonitorUpdatedDateTime = DateTime.Now;
                                    }

                                }
                            }

                        }
                        else
                        {
                            envHistory.monitorList[0].MonitorUpdatedDateTime = DateTime.Now;
                        }
                    }
                }
            }

            return monitorHistory;
        }

        
        /// <summary>
        /// To update the acknowledge and change of email alert type for the service montor
        /// </summary>
        /// <param name="ackData"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public int InsUpdateServiceAcknowledge(AcknowledgeEntity ackData, string mode)
        {
            int recInsert = 0;
            try
            {
                recInsert = monitorData.InsUpdateServiceAcknowledge(ackData, mode);
                if (recInsert == 0)
                {
                    var context = HttpContext.Current;
                    ackData.CreatedBy = ackData.CreatedByName;

                    if (context.Session["ServiceMonitor"] != null)
                    {
                        var mList = (List<ServiceMoniterEntity>) context.Session["ServiceMonitor"];
                        if (mList != null && mList.Count > 0)
                        {
                            if (mList.Any(ml => ml.EnvID == ackData.EnvId))
                            {
                                ServiceMoniterEntity serviceMonitor =
                                    mList.SingleOrDefault(ml => ml.EnvID == ackData.EnvId);
                                if (serviceMonitor != null)
                                {
                                    if (mList.Count > 0 && mList[0].monitorList != null &&
                                        mList[0].monitorList.Count > 0)
                                    {
                                        ackData.Env_Name = serviceMonitor.EnvName;
                                        var singleOrDefault = serviceMonitor.monitorList.SingleOrDefault(
                                            con => con.ConfigID == ackData.ConfigId);
                                        if (
                                            singleOrDefault != null && singleOrDefault.ConfigServiceType == "1")
                                            ackData.ContentType = CONTENT_MANAGER;
                                        else
                                        {
                                            var monitorEntity = serviceMonitor.monitorList.SingleOrDefault(
                                                con => con.ConfigID == ackData.ConfigId);
                                            if (
                                                monitorEntity != null && monitorEntity.ConfigServiceType == "2")
                                                ackData.ContentType = DESPATCHER;
                                        }
                                    }
                                    var orDefault = serviceMonitor.monitorList.SingleOrDefault(
                                        con => con.ConfigID == ackData.ConfigId);
                                    if (orDefault != null)
                                        ackData.Status =
                                            orDefault.MonitorStatus;
                                }
                            }
                        }
                    }
                    else if (context.Session.Contents.Count <= 0)
                    {
                        recInsert = -9;
                    }
                    //Send mail for an information to the user about the change 
                    //ACKNOWLEDGE_ALERT_MAIL
                    switch (ackData.AcknowledgeAlertChange)
                    {
                        case "ack":
                            mailService.SendMail(ackData.EnvId, ackData.ConfigId, ACKNOWLEDGE_MAIL, ackData);
                            break;
                        case "stop":
                            mailService.SendMail(ackData.EnvId, ackData.ConfigId, ACKNOWLEDGE_ALERT_STOPPED_MAIL, ackData);
                            break;
                        case "start":
                            mailService.SendMail(ackData.EnvId, ackData.ConfigId, ACKNOWLEDGE_ALERT_STARTED_MAIL, ackData);
                            break;
                    }
                }
            }
            catch (ArgumentOutOfRangeException arg)
            {
                if (arg.Message.Contains("Specified argument was out of the range"))
                {
                    recInsert = -3; //Mail error
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex.ToString());
                recInsert = -4; //Other error
            }
            return recInsert;
        }

        /// <summary>
        /// Get  open incidents for all environments
        /// </summary>
        /// <param name="env_id"></param>
        /// <returns></returns>
        public List<ServiceMoniterEntity> GetAllIncident(int envId, string type)
        {
            var monList = monitorData.GetAllIncident(envId, type);
            var monListWithStatusIcon = UpdateMonitorStatus(monList, false);

            return monListWithStatusIcon;
        }

                /// <summary>
        /// To insert incident tracking details
        /// </summary>
        /// <param name="ackData"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public int InsIncidentTracking(string monID, string envID, string configID, string issueDesc, string solutionDesc, string userID)
        {
            int recInsert = 0;
            try
            {
                recInsert = monitorData.InsIncidentTracking(monID,envID,configID,issueDesc,solutionDesc,userID);
            }
            catch (Exception ex)
            {
                recInsert = 1;
                throw ex;
            }
            return recInsert;
        }

        public List<ServiceMoniterEntity> GetIncidentTrackingReport(int envId, DateTime fromTime, DateTime toTime)
        {
            List<ServiceMoniterEntity> monitorList = new List<ServiceMoniterEntity>();
            try
            {
                monitorList = monitorData.GetIncidentTrackingReport(envId, fromTime, toTime);
                if (monitorList != null && monitorList.Count > 0)
                {
                    foreach (var serviceMoniterEntity in monitorList)
                    {
                        foreach (var monitorEntity in serviceMoniterEntity.monitorList)
                        {
                            monitorEntity.EnvName = serviceMoniterEntity.EnvName;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex.ToString());
            }
            return monitorList;
        }
    
        private DateTime GetScheduleStartDateTime(int schEnvId)
        {
            DateTime schStartDateTime = DateTime.Now;
            SchedulerEntity schedulerList = new SchedulerEntity();
            schedulerList = schedulerServices.GetSchedulerDetails(schEnvId, 0);
            if(schedulerList!=null && schedulerList.EnvID>0)
            {
                schStartDateTime = Convert.ToDateTime(schedulerList.StartDateTime);
            }
            return schStartDateTime;
        }

        /// <summary>
        /// Get All windows service list with configuration
        /// </summary>
        /// <param name="envID"></param>
        /// <returns></returns>
        public List<WinServiceEntity> GetWindowsService(int env_id)
        {
            List<WinServiceEntity> winServiceList = new List<WinServiceEntity>();
            List<ServiceMoniterEntity> monitorList = new List<ServiceMoniterEntity>();
            monitorList = monitorData.GetAllMonitors(env_id, false);
            winServiceList = monitorData.GetWindowsService(env_id);

            if (monitorList != null && monitorList.Count > 0)
            {
                if (monitorList[0].monitorList != null && monitorList[0].monitorList.Count > 0)
                {
                    foreach (WinServiceEntity win in winServiceList)
                    {
                        if (monitorList[0].monitorList.Any(ml => ml.ConfigID == win.ConfigID))
                        {
                            win.MonitorStatus = monitorList[0].monitorList.Where(ml => ml.ConfigID == win.ConfigID).ToList()[0].MonitorStatus;
                            win.MonitorComments = monitorList[0].monitorList.Where(ml => ml.ConfigID == win.ConfigID).ToList()[0].MonitorComments;
                        }
                    }
                }
            }
            return winServiceList;
        }

        #region Get daily service status report (Dasboard)
        /// <summary>
        /// Get daily service status report
        /// </summary>
        /// <param name="env_id"></param>
        /// <returns></returns>
        public List<ServiceMoniterEntity> GetServiceStatusReport(int env_id, DateTime startDate, DateTime endDate)
        {
            List<int> tempConfig = new List<int>();
            List<MonitorEntity> dayMonitorList = new List<MonitorEntity>();
            List<MonitorEntity> configList = new List<MonitorEntity>();
            List<ServiceMoniterEntity> statusMonitorList = new List<ServiceMoniterEntity>();

            MonitorEntity newMonitorEntity = new MonitorEntity();
            ServiceMoniterEntity newServiceMonitor = new ServiceMoniterEntity();
            StringBuilder sbStatus = new StringBuilder();
            List<MonitorEntity> monitorListEntity = new List<MonitorEntity>();
            List<ServiceMoniterEntity> newStatusMonitorList = new List<ServiceMoniterEntity>();

            statusMonitorList = monitorData.GetServiceStatusReport(env_id, startDate, endDate);
            if (statusMonitorList != null && statusMonitorList.Count > 0)
            {
                
                foreach (ServiceMoniterEntity statusMonitor in statusMonitorList)
                {
                    newServiceMonitor = new ServiceMoniterEntity
                    {
                        EnvID = statusMonitor.EnvID,
                        EnvName = statusMonitor.EnvName
                    };

                    monitorListEntity = new List<MonitorEntity>();
                    if (statusMonitor.monitorList != null && statusMonitor.monitorList.Count > 0)
                    {
                        tempConfig = new List<int>();
                        tempConfig = statusMonitor.monitorList.Select(ml => ml.ConfigID).Distinct().ToList();
                        newMonitorEntity = new MonitorEntity();
                        foreach (int iCon in tempConfig)
                        {
                            configList = new List<MonitorEntity>();
                            if (statusMonitor.monitorList.Any(con => con.ConfigID == iCon))
                            {
                                newMonitorEntity = new MonitorEntity();
                                newMonitorEntity.ConfigID = iCon;

                                configList = statusMonitor.monitorList.Where(con => con.ConfigID == iCon).ToList<MonitorEntity>();
                                newMonitorEntity.ConfigHostIP = configList[0].ConfigHostIP;
                                newMonitorEntity.ConfigPort = configList[0].ConfigPort;
                                newMonitorEntity.ConfigServiceDescription = configList[0].ConfigServiceDescription;
                                newMonitorEntity.EnvName = statusMonitor.EnvName;

                                dayMonitorList = new List<MonitorEntity>();
                                sbStatus = new StringBuilder();
                                //sbStatus.Append("[");
                                //foreach (MonitorEntity monitor in statusMonitor.monitorList)
                                foreach (DateTime day in EachDay(startDate, endDate))
                                {
                                    if(!string.IsNullOrEmpty(sbStatus.ToString()))
                                        sbStatus.Append(",");
                                    dayMonitorList = new List<MonitorEntity>();

                                    if (configList.Any(ml => ml.MonitorCreatedDateTime == day))
                                    {
                                        dayMonitorList = configList.Where(ml => ml.MonitorCreatedDateTime == day).ToList<MonitorEntity>();
                                        if (dayMonitorList.Count > 0)
                                        {
                                            List<string> statusCode = new List<string>();
                                            if (dayMonitorList.Any(sc => sc.ConfigServiceType == "1"))
                                            {
                                                statusCode = dayMonitorList.Where(sc => sc.ConfigServiceType == "1").Select(sc => sc.MonitorStatus).ToList<string>();
                                                newMonitorEntity.ConfigServiceType = CONTENT_MANAGER;
                                            } //check for either content manager / Dispatcher
                                            else if (dayMonitorList.Any(sc => sc.ConfigServiceType == "2"))
                                            {
                                                statusCode = dayMonitorList.Where(sc => sc.ConfigServiceType == "2").Select(sc => sc.MonitorStatus).ToList<string>();
                                                newMonitorEntity.ConfigServiceType = DESPATCHER;
                                            } //check for either content manager / Dispatcher
                                            if (statusCode != null && statusCode.Count > 0)
                                            {
                                                statusCode = statusCode.Distinct().ToList();
                                                if (statusCode.Any(sc => sc == "D" || sc == "A" || sc == "R"))
                                                    statusCode.RemoveAll(sc => sc == "N/A");
                                                sbStatus.Append(CommonUtility.ConvertListToString(statusCode, "/"));
                                            }
                                        }

                                    }
                                    else
                                    {
                                        sbStatus.Append("N/A");
                                    }
                                }
                                //sbStatus.Append("]");
                                newMonitorEntity.MonitorComments = sbStatus.ToString();
                                monitorListEntity.Add(newMonitorEntity);
                                newServiceMonitor.monitorList = monitorListEntity;
                            }
                        }
                    }
                    newStatusMonitorList.Add(newServiceMonitor);
                }

            }
            return newStatusMonitorList;
        }

        public static IEnumerable<DateTime> EachDay(DateTime from, DateTime thru)
        {
            for (var day = from.Date; day.Date <= thru.Date; day = day.AddDays(1))
                yield return day;
        }
        #endregion Get daily service status report (Dasboard)

        #region Personalize setting
        
        public int InsUpdPersonalize(PersonalizeEntity personalize)
        {
            return monitorData.InsUpdPersonalize(personalize);
        }

        public List<PersonalizeEntity> GetPersonalize(int userId)
        {
            return monitorData.GetPersonalize(userId);
        }

        #endregion Personalize setting


        /// <summary>
        /// Get the windows service status 
        /// </summary>
        /// <param name="ServiceName"></param>
        /// <returns></returns>
        private string GetWindowsServiceStatus(string ServiceName, string systemName)
        {
            string serviceStatus = string.Empty;
            using (var sc = !string.IsNullOrEmpty(systemName) ? new ServiceController(ServiceName, systemName) : new ServiceController(ServiceName))
            {
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
                        default:
                            serviceStatus = ServiceStatuschanging;
                            break;
                    }
                }
                catch (Exception ex)
                {
                    serviceStatus = ServiceNameNotfound;
                    Logger.Log(ex.ToString());
                    if (ex.Message.Contains("not found"))
                        serviceStatus = ServiceNameNotfound;
                }
                finally { }
            }
            return serviceStatus;
        }

        public string GetRemoteWindowsServiceStatus(string serviceName, string systemName)
        {
            var uri = string.Format(RemoteSystemURLForServiceStatus + "/GetStatus?svn={1}&srn={0}", systemName, serviceName);
            var request = _webHttpRequestBuilder.CreateGetRequest(uri);
            var response = GetWebResponse<WindowServiceStatus>(request);
            return response.ServiceStatus;
        }

        public List<WindowServiceStatus> GetRemoteWindowsServiceStatus(List<ServiceMoniterEntity> monList)
        {
            var serviceList = new List<WindowServiceStatus>();
            try
            {
                var hostNames = GetHostNames(monList);
                //foreach (var response in hostNames.Select(hostName => string.Format("http://{0}:7777/owin/Status/GetAllServiceStatus?srn={0}", 
                //    hostName)).Select(uri => wrapper.CreateGetRequest(uri)).Select(GetWebResponseList<WindowServiceStatus>).Where(response => response != null))
                //{
                //    serviceList.AddRange(response);
                //}
                foreach (var request in hostNames.Select(hostName => string.Format(RemoteSystemURLForServiceStatus + "/GetAllServiceStatus?srn={0}", hostName)).Select(uri => _webHttpRequestBuilder.CreateGetRequest(uri)))
                 {
                    try
                    {
                        Logger.Log(request.Uri);
                        var response = GetWebResponseList<WindowServiceStatus>(request);
                        if (response != null)
                            serviceList.AddRange(response);
                    }
                    catch (Exception exception)
                    {
                        Logger.Log(exception.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex.ToString());
            }

            return serviceList;
        }

        public List<WindowServiceStatus> GetRemoteWindowsServiceStatus(ServiceMoniterEntity monitor)
        {
            var serviceList = new List<WindowServiceStatus>();
            try
            {
                var hostNames = GetHostNames(monitor);
                //foreach (var response in hostNames.Select(hostName => string.Format("http://{0}:7777/owin/Status/GetAllServiceStatus?srn={0}", 
                //    hostName)).Select(uri => wrapper.CreateGetRequest(uri)).Select(GetWebResponseList<WindowServiceStatus>).Where(response => response != null))
                //{
                //    serviceList.AddRange(response);
                //}
                foreach (var request in hostNames.Select(hostName => string.Format(RemoteSystemURLForServiceStatus + "/GetAllServiceStatus?srn={0}", hostName)).Select(uri => _webHttpRequestBuilder.CreateGetRequest(uri)))
                {
                    try
                    {
                        Logger.Log(request.Uri);
                        var response = GetWebResponseList<WindowServiceStatus>(request);
                        if (response != null)
                            serviceList.AddRange(response);
                    }
                    catch (Exception exception)
                    {
                        Logger.Log(exception.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex.ToString());
            }

            return serviceList;
        }
        private IEnumerable<string> GetHostNames(List<ServiceMoniterEntity> monList)
        {
            var listIp = new List<string>();
            foreach (var serviceMoniterEntity in monList)
            {
                if (serviceMoniterEntity.monitorList.Count > 0)
                    listIp.AddRange(serviceMoniterEntity.monitorList.Select(m => m.ConfigHostIP).Distinct().ToList());
            }

            return listIp.Distinct().ToList();
        }

        private IEnumerable<string> GetHostNames(ServiceMoniterEntity monitor)
        {
            var listIp = new List<string>();
            if (monitor.monitorList.Count > 0)
                listIp.AddRange(monitor.monitorList.Select(m => m.ConfigHostIP).Distinct().ToList());

            return listIp.Distinct().ToList();
        }

        public IEnumerable<T> GetWebResponseList<T>(IWebHttpRequestWrapper request)
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

                var responseRext = _javaScriptSerializer.Deserialize<IEnumerable<T>>(resultText);
                return responseRext;
            }
        }

        public T GetWebResponse<T>(IWebHttpRequestWrapper request)
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

                var responseRext = _javaScriptSerializer.Deserialize<T>(resultText);
                return responseRext;
            }
        }

        public List<ServerDriveDetail> GetAverageUsedSpace(string host, string mode)
        {
            var lists = new List<ServerDriveDetail>();
            var averageUsedSpace = monitorData.GetAverageUsedSpace(host);
            if (averageUsedSpace != null && averageUsedSpace.Count > 0)
            {
                foreach (var serverDriveSpace in averageUsedSpace)
                {
                    if (serverDriveSpace.AverageFreeSpace <= 0 || serverDriveSpace.AverageUsedSpace <= 0 ||
                        serverDriveSpace.AverageTotalSpace < 0) continue;

                    var freeSpace = CommonUtility.FormatBytes(serverDriveSpace.AverageFreeSpace);
                    var usedSpace = CommonUtility.FormatBytes(serverDriveSpace.AverageUsedSpace);
                    var totalSpace = CommonUtility.FormatBytes(serverDriveSpace.AverageTotalSpace);

                    serverDriveSpace.FreeSpaceInGb = Convert.ToDouble(freeSpace.Substring(0, freeSpace.Length - 3));
                    serverDriveSpace.UsedSpaceInGb = Convert.ToDouble(usedSpace.Substring(0, usedSpace.Length - 3));
                    serverDriveSpace.TotalSpaceInGb = Convert.ToDouble(totalSpace.Substring(0, totalSpace.Length - 3));

                }
            }

            if (averageUsedSpace != null)
            {
                var drives = averageUsedSpace.Select(_ => _.Name).Distinct();
                var dates = averageUsedSpace.Select(_ => _.Date).Distinct();
                var enumerable = dates as int[] ?? dates.ToArray();
                
                foreach (var drive in drives)
                {
                    var list = new ServerDriveDetail
                    {
                        Name = drive,
                        Date = new List<int>(),
                        FreeSpace = new List<double>(),
                        TotalSpace = new List<double>(),
                        UsedSpace = new List<double>(),
                        UsedSpaceInPercent = new List<double>(),
                        FreeSpaceInPercent = new List<double>()
                    };

                    foreach (var date in enumerable)
                    {
                        list.Date.Add(date);
                        var serverDriveSpace = averageUsedSpace.FirstOrDefault(_ => _.Name == drive && _.Date == date);
                        if (serverDriveSpace != null)
                        {
                            list.FreeSpace.Add(
                                serverDriveSpace.FreeSpaceInGb);
                            list.UsedSpace.Add(
                                serverDriveSpace.UsedSpaceInGb);
                            list.TotalSpace.Add(
                                serverDriveSpace.TotalSpaceInGb);

                            if (serverDriveSpace.UsedSpaceInGb <= 0 || serverDriveSpace.TotalSpaceInGb < 0)
                                list.UsedSpaceInPercent.Add(0);
                            else
                                list.UsedSpaceInPercent.Add((serverDriveSpace.UsedSpaceInGb/
                                                             serverDriveSpace.TotalSpaceInGb)*100);

                            if (serverDriveSpace.FreeSpaceInGb <= 0 || serverDriveSpace.TotalSpaceInGb <= 0)
                                list.FreeSpaceInPercent.Add(0);
                            else
                                list.FreeSpaceInPercent.Add((serverDriveSpace.FreeSpaceInGb/
                                                             serverDriveSpace.TotalSpaceInGb)*100);

                        }

                    }
                    lists.Add(list);
                }
            }

            return lists;
        }

        public List<ServerCpuUsage> GetCpuMemorySpace(string host)
        {
            return monitorData.GetCpuMemorySpace(host);
        }

        public PerformanceResponse GetCurrentDriveInfos(string hostName)
        {
            var uri =
                   string.Format(RemoteSystemURLForServerPerformance + "/GetServerDetail?srn={0}",
                       hostName);
            var webHttpRequestBuilder = new WebHttpRequestBuilder();
            var request = webHttpRequestBuilder.CreateGetRequest(uri);
            var response = GetWebResponse<PerformanceResponse>(request);
            response = ConvertToGbPerformanceResponse(response);
            return response;
        }

        public PerformanceResponse ConvertToGbPerformanceResponse(PerformanceResponse response)
        {
            if (response != null && response.Performance != null && response.Performance.Drives != null &&
                response.Performance.Drives.Count > 0)
            {
                foreach (var drive in response.Performance.Drives)
                {
                    drive.FreeSpaceInText = CommonUtility.FormatBytes(Convert.ToDecimal(drive.FreeSpace));
                    drive.UsedSpaceInText = CommonUtility.FormatBytes(Convert.ToDecimal(drive.UsedSpace));
                    drive.TotalSpaceInText = CommonUtility.FormatBytes(Convert.ToDecimal(drive.TotalSpace));

                    drive.FreeSpaceInGb = Convert.ToDouble(drive.FreeSpaceInText.Substring(0, drive.FreeSpaceInText.Length - 3));
                    drive.UsedSpaceInGb = Convert.ToDouble(drive.UsedSpaceInText.Substring(0, drive.UsedSpaceInText.Length - 3));
                    drive.TotalSpaceInGb = Convert.ToDouble(drive.TotalSpaceInText.Substring(0, drive.TotalSpaceInText.Length - 3));
                }
            }
            return response;
        }
    }
}
