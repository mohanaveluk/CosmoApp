using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cog.CSM.Entity;
using Cog.CSM.Data;
using Cog.CSM.MailService;
using System.Configuration;

namespace Cog.CSM.Service
{
    public class SchedulerService
    {
        private static readonly string DbType = ConfigurationManager.AppSettings["DatabaseType"];
        private const string SubscriptionMailTypeId = "SubscriptionReport";
        private const string MonitorAlertMailTypeId = "UrlMonitorAlert";
        private static string _tableStyle = "'border-collapse:collapse;border:1px solid #eee; width:90%'";
        private static string _tableOutage = "'border-collapse:collapse;border:1px solid #eee; width:70%'";
        private static string _headerStyle = "'border:1px solid #ddd; padding:5px;background-color:#A1C1D5'";
        private static string _headerOutage = "'border:1px solid #ddd; padding:5px;background-color:#168eea; color:#fff'"; //#3AA6C4
        private static string _outageTitle = "'color:#ce1126;font-weight:bold'";

        private static string _headerStyleTime = "'border:1px solid #ddd; width:30px;background-color:#A1C1D7'";
        private static string _bodyStyle = "'border:1px solid #ddd; padding:5px'";
        private static string _bodyStyleRight = "'border:1px solid #ddd; padding:5px;text-align:right'";
        private static string _bodyStyleCenter = "'border:1px solid #ddd; padding:5px;text-align:center'";
        private static string _tdHost = "'border:1px solid #ddd; padding:5px;width:30%'";
        private static string _bgGreen = "'border:1px solid #ddd;;background-color:#00a100'"; //#36A44A
        private static string _bgRed = "'border:1px solid #ddd;background-color:#C41D20'";
        private static string _bgGray = "'border:1px solid #ddd;background-color:#cccccc'";

        readonly ISchedularData _schedulerData;
        readonly ExecuteService _executeService;
        readonly MailService _mailService;

        public SchedulerService(MailService mailService)
        {
            _mailService = mailService;
            _executeService = new ExecuteService(mailService);

            var iDbType = DbType == DatabaseType.Oracle.ToString()
                ? Convert.ToInt32(DatabaseType.Oracle).ToString()
                : Convert.ToInt32(DatabaseType.SqlServer).ToString();

            _schedulerData = new SchedularDataFactory().Create(Convert.ToInt32(iDbType).ToString());
        }

        /// <summary>
        /// Get all the schedules avaialbe based on the time since last job run
        /// </summary>
        /// <returns></returns>
        private List<Scheduler_details> getSchedules()
        {
            return _schedulerData.GetSchedules(DateTime.Now);
        }

        /// <summary>
        /// Get all the schedules avaialbe based on the time since last job run
        /// </summary>
        /// <returns></returns>
        private List<ServiceEntity> GetAllScheduledServices()
        {
            return _schedulerData.GetAllScheduledServices(DateTime.Now);
        }

        /// <summary>
        /// Method to monitor all services
        /// </summary>
        public void MonitorAllServices()
        {
            try
            {
                //Despatcher desp = new Despatcher();
                //mail1.GetMailMessage("Project Submitted", "Content Manager", desp);
                //MailTemplate.GetMailMessage(System.Configuration.ConfigurationManager.AppSettings["mailTemplatePath"].ToString());
                var currentTime = DateTime.Now;

                var schList = _schedulerData.GetSchedules(currentTime);

                if (schList != null && schList.Count > 0)
                {
                    var servicesLise = GetAllScheduledServices();
                    if (servicesLise != null && servicesLise.Count > 0)
                    {
                        //Execute all the available services
                        _executeService.RunAllScheduledJobs(servicesLise);
                        //Update date time of service run for next time
                        UpdateServiceRunNextTime(schList, currentTime);
                    }
                }
                else
                    Logger.Log("Scheduler does not exists");

                SendStatusReport();
                MonitorPotalStatus();
            }
            catch (Exception ex)
            {
                Logger.Log(ex.ToString());
                throw;
            }
        }

        private void MonitorPotalStatus()
        {
            try
            {
                var monitorPortal = _schedulerData.GetPortalToMonitor(0);
                if(monitorPortal == null || monitorPortal.Count <= 0) return;

                Logger.Log("Portal monitor begins");

                var monitorAlert = CommonUtility.GetMonitorAlert("url");

                foreach (
                    var monitor in monitorPortal.Where(monitor => monitor.NextJobRunTime <= DateTime.Now)
                    )
                {
                    Logger.Log(monitor.Adress);
                    
                    var result = CommonUtility.LogWebsiteWithCredential(monitor.Adress, monitor.MatchContent,
                        monitor.UserName, monitor.Password);
                    Logger.Log(result.Status == "Success"
                        ? $"Status: {result.Status}, ResponseTime: {result.ResponseTime}"
                        : $"Status: {result.Status}, Message: {result.Message}, Exception: {result.Exception.Substring(0, 100)}");

                    result.UrlId = monitor.Id;
                    result.EnvId = monitor.EnvId;
                    result.EnvName = monitor.EnvName;
                    result.Address = monitor.Adress;
                    result.ResponseTimeInSec = CommonUtility.ConvertMillisecondsToSeconds(result.ResponseTime).ToString("0.##");
                    

                    if (!string.IsNullOrEmpty(monitorAlert.Unit) && !string.IsNullOrEmpty(monitorAlert.Value))
                    {
                        if ((monitorAlert.Unit.ToLower().StartsWith("sec") &&
                             Convert.ToInt32(monitorAlert.Value)*1000 < result.ResponseTime) ||
                            ((monitorAlert.Unit.ToLower().Equals("ms") || monitorAlert.Unit.ToLower().Equals("msec")) &&
                            Convert.ToInt32(monitorAlert.Value) < result.ResponseTime))
                        {
                            result.ThresholdTime = monitorAlert.Value;
                            result.ThresholdUnit = monitorAlert.Unit;

                            _mailService.SendMail(result.EnvId, 0, MonitorAlertMailTypeId, result);
                        }
                    }

                    _schedulerData.InsertPortalStatus(result);
                    Logger.Log($"Status updated for {monitor.DisplayName}");

                    _schedulerData.SetPortalNextJobRunTime(monitor);
                    Logger.Log($"Next monitor time has updated for {monitor.DisplayName}");

                }
                Logger.Log("Portal monitor ends");
            }
            catch (Exception exception)
            {
                Logger.Log(exception.ToString());
                throw;
            }
        }

        private void SendStatusReport()
        {
            try
            {
                var subscriptions = GetSubscription(0);
                if (subscriptions == null || subscriptions.Count <= 0) return;

                foreach (var subscription in subscriptions.Where(subscription => subscription.NextJonRunTime <= DateTime.Now))
                {
                    Logger.Log("Subscription report begins");

                    CalculteTimePeriod(subscription);

                    Logger.Log(
                        $"Subscription Type: {subscription.Type}, Time: {subscription.StartTime.ToString("h:mm:ss tt")}");
                    var monitorStaturReport = GetMonitorStatusReport(0, false, subscription);

                    var emailList = subscription.SubscriptionDetails.Select(subscriptionDetail => subscriptionDetail.EmailAddress).ToList();

                    var subscriptionMail = new SubscriptionMailService
                    {
                        Id = subscription.Id,
                        Time = subscription.NextJonRunTime !=null ? Convert.ToDateTime(subscription.NextJonRunTime).ToShortTimeString() : "",
                        Type = subscription.Type,
                        NextJonRunTime = subscription.NextJonRunTime,
                        LastJobRanTime = subscription.LastJobRanTime,
                        ServiceStatusDetails = monitorStaturReport,
                        StartDateTime = subscription.StartTime,
                        EndDateTime = subscription.EndTime
                    };

                    Logger.Log("Sending mail");
                    _mailService.SendMail(0, emailList, SubscriptionMailTypeId, subscriptionMail);

                    //update next job run time
                    Logger.Log(
                        $"Updating next job run time: {Convert.ToDateTime(DateTime.Now.ToShortDateString() + " 00:00:00").AddDays(1).AddHours(Convert.ToInt32(subscription.Time))}");

                    _schedulerData.SetNextJobRunTime(subscription);
                    Logger.Log("Subscription report ends");
                }
            }
            catch (Exception exception)
            {
                Logger.Log("Error: " + exception);
            }

        }

        private List<ServiceMoniterEntity> GetMonitorHistory(Subscription subscription)
        {
            var monitorHistory = _schedulerData.GetSubscriptionStatusList(0, subscription);
            return monitorHistory;
        }

        private void CalculteTimePeriod(Subscription subscription)
        {
            switch (subscription.Type)
            {
                case "Daily":
                {
                    /*
                        subscription.StartTime =
                            Convert.ToDateTime(DateTime.Now.ToShortDateString() + " 00:00:00")
                            .AddDays(-1)
                                .AddHours(Convert.ToInt32(subscription.Time));

                        subscription.EndTime =
                            Convert.ToDateTime(DateTime.Now.ToShortDateString() + " 00:00:00")
                                .AddHours(Convert.ToInt32(subscription.Time));
                        */

                    if (subscription.NextJonRunTime != null)
                        subscription.StartTime = (DateTime) Convert.ToDateTime(subscription.NextJonRunTime).AddDays(-1);
                    else
                    {
                        subscription.StartTime =
                            Convert.ToDateTime(DateTime.Now.ToShortDateString() + " 00:00:00")
                                .AddDays(-1)
                                .AddHours(Convert.ToInt32(subscription.Time));
                    }

                    subscription.EndTime = Convert.ToDateTime(subscription.NextJonRunTime);

                    break;
                }
            }
        }

        private string GetMonitorStatusReport(int envId, bool statusWithServiceName, Subscription subscription)
        {
            var monitorList = _schedulerData.GetAllMonitors(envId, statusWithServiceName);
            if (monitorList == null || monitorList.Count == 0) return "Service status report is not avaialble";

            var monitorHistory = GetMonitorHistory(subscription); 
            var envMonitorHistory = new ServiceMoniterEntity();
            var sb = new StringBuilder();

            foreach (var monitorService in monitorList)
            {
                var envMonitorStatus = "No issues";
                var isOtage = false;
                if (monitorHistory != null && monitorHistory.Any(_ => _.EnvID == monitorService.EnvID))
                {
                    envMonitorHistory = monitorHistory.FirstOrDefault(_ => _.EnvID == monitorService.EnvID);

                    if (envMonitorHistory != null && (envMonitorHistory.monitorList != null && envMonitorHistory.monitorList.Count > 0))
                    {
                        if (
                            envMonitorHistory.monitorList.Any(
                                _ =>
                                    (_.MonitorStatus.ToLowerInvariant().Equals("stopped") ||
                                     _.MonitorStatus.ToLowerInvariant().Contains("not"))))
                        {
                            isOtage = true;
                            envMonitorStatus = "One or More issues";
                        }
                    }
                }

                sb.Append("Environment: " + monitorService.EnvName);// + " - " + envMonitorStatus);
                sb.Append("<br/>");
                sb.Append("<table style=" + _tableStyle + ">");
                sb.Append(GetReportTableHeader(subscription));
                sb.Append(GetReportTableBody(monitorService, subscription, envMonitorHistory));
                sb.Append("</table>");

                sb.Append("<br/br/>");
                if (isOtage)
                {
                    sb.Append("Environment: " + monitorService.EnvName + " - <span style=" + _outageTitle + ">Outage Details</span>");
                    sb.Append("<br/>");
                    sb.Append("<table style=" + _tableOutage + ">");
                    sb.Append(GetReportOutageDetails(monitorService, subscription, envMonitorHistory));
                    sb.Append("</table>");
                }
                sb.Append("<br/br/>");

            }

            return sb.ToString();
        }

        private string GetReportTableHeader(Subscription subscription)
        {
            var sb = new StringBuilder();
            sb.Append("<th style=" + _headerStyle + ">Service Type</th>");
            sb.Append("<th style=" + _headerStyle + ">Host/IP</th>");
            sb.Append("<th style=" + _headerStyle + ">Port</th>");
            sb.Append("<th style=" + _headerStyle + ">Description</th>");

            if (subscription.Type == SubscptionType.Daily.ToString())
            {
                //var startTime =
                //    Convert.ToDateTime(DateTime.Now.ToShortDateString() + " 00:00:00")
                //        .AddDays(-1)
                //        .AddHours(Convert.ToInt32(subscription.Time));

                var startTime = subscription.StartTime;

                while (startTime != subscription.EndTime)
                {
                    sb.Append("<th style=" + _headerStyleTime + ">" + startTime.ToString("ht").ToLower() + "</th>");
                    startTime = startTime.AddHours(1);
                }
            }
            return sb.ToString();
        }

        private string GetReportOutageDetails(ServiceMoniterEntity monitorService, Subscription subscription, ServiceMoniterEntity monitorHistory)
        {
            var sb = new StringBuilder();

            sb.Append("<th style=" + _headerOutage + ">Service Type</th>");
            sb.Append("<th style=" + _headerOutage + ">Host/IP</th>");
            sb.Append("<th style=" + _headerOutage + ">Port</th>");
            sb.Append("<th style=" + _headerOutage + ">Description</th>");
            sb.Append("<th style=" + _headerOutage + ">Outage Start Time</th>");
            sb.Append("<th style=" + _headerOutage + ">Outage End Time</th>");
            sb.Append("<th style=" + _headerOutage + ">Duration (In min)</th>");

            var outageList = monitorHistory.monitorList.Any(
                            _ => (_.MonitorStatus.ToLowerInvariant().Equals("stopped") || _.MonitorStatus.ToLowerInvariant().Contains("not")) && _.EnvID == monitorService.EnvID)
                            ? monitorHistory.monitorList.Where(
                                _ =>
                                    (_.MonitorStatus.ToLowerInvariant().Equals("stopped") || _.MonitorStatus.ToLowerInvariant().Contains("not")) &&
                                    _.EnvID == monitorService.EnvID)
                            : new List<MonitorEntity>();

            foreach (var monitorEntity in outageList)
            {
                var duration = monitorEntity.MonitorUpdatedDateTime != null
                    ? Convert.ToDateTime(monitorEntity.MonitorUpdatedDateTime)
                        .Subtract(monitorEntity.MonitorCreatedDateTime).TotalMinutes
                    : 0;
                var dispatcherPrimary = !monitorEntity.ConfigIsPrimary ? "(" + monitorEntity.ConfigPort  + ")" : string.Empty;

                sb.Append("<tr>");
                sb.Append("<td style=" + _bodyStyle + ">" + $"{monitorEntity.ConfigServiceType} {dispatcherPrimary}" + "</td>");
                sb.Append("<td style=" + _bodyStyle + ">" + monitorEntity.ConfigHostIP + "</td>");
                sb.Append("<td style=" + _bodyStyle + ">" + monitorEntity.ConfigPort + "</td>");
                sb.Append("<td style=" + _bodyStyle + ">" + monitorEntity.ConfigServiceDescription + "</td>");
                sb.Append("<td style=" + _bodyStyle + ">" + monitorEntity.MonitorCreatedDateTime + "</td>");
                sb.Append("<td style=" + _bodyStyle + ">" + monitorEntity.MonitorUpdatedDateTime + "</td>");
                sb.Append("<td style=" + _bodyStyleRight + ">" + $"{duration:N1}" + "</td>");
                sb.Append("</tr>");
            }

            return sb.ToString();
        }

        private string GetReportTableBody(ServiceMoniterEntity monitorService, Subscription subscription, ServiceMoniterEntity monitorHistory)
        {
            var sb = new StringBuilder();

            foreach (var monitorEntity in monitorService.monitorList)
            {
                var serviceType = monitorEntity.ConfigServiceType == "1"
                    ? "Content Manager"
                    : "Dispatcher";
                sb.Append("<tr>");
                sb.Append("<td style=" + _bodyStyle + ">" + serviceType + "</td>");
                sb.Append("<td style=" + _bodyStyle + ">" + monitorEntity.ConfigHostIP + "</td>");
                sb.Append("<td style=" + _bodyStyle + ">" + monitorEntity.ConfigPort + "</td>");
                sb.Append("<td style=" + _bodyStyle + ">" + monitorEntity.ConfigServiceDescription + "</td>");
                
                if (subscription.Type == SubscptionType.Daily.ToString())
                {
                    var entity = monitorEntity;
                    var serviceMonitorHistory =
                        (Dictionary<DateTime, string>)
                            GetServiceMonitorHistory(monitorEntity, monitorHistory, subscription);

                    //var startTime =
                    //    Convert.ToDateTime(DateTime.Now.ToShortDateString() + " 00:00:00")
                    //        .AddDays(-1)
                    //        .AddHours(Convert.ToInt32(subscription.Time));

                    var startTime = subscription.StartTime;
                    while (startTime != subscription.EndTime)
                    {
                        var colorCode = serviceMonitorHistory[startTime] == "green"
                            ? _bgGreen
                            : _bgRed;
                        sb.Append("<td style=" + colorCode + ">&nbsp;</td>");
                        startTime = startTime.AddHours(1);
                    }
                }
                sb.Append("</tr>"); 
            }

            return sb.ToString();
        }

        private object GetServiceMonitorHistory(MonitorEntity monitorEntity, ServiceMoniterEntity monitorHistory, Subscription subscription)
        {
            var statusDict = new Dictionary<DateTime,string>();

            var runningList = monitorHistory.monitorList.Any(
                                        _ => (_.MonitorStatus.ToLowerInvariant().Equals("running") || _.MonitorStatus.ToLowerInvariant().Contains("standby")) && _.ConfigID == monitorEntity.ConfigID)
                                        ? monitorHistory.monitorList.Where(
                                            _ =>
                                                (_.MonitorStatus.ToLowerInvariant().Equals("running") || _.MonitorStatus.ToLowerInvariant().Contains("standby")) &&
                                                _.ConfigID == monitorEntity.ConfigID).ToList()
                                        : new List<MonitorEntity>();

            var outageList = monitorHistory.monitorList.Any(
                _ => (_.MonitorStatus.ToLowerInvariant().Equals("stopped") || _.MonitorStatus.ToLowerInvariant().Contains("not")) && _.ConfigID == monitorEntity.ConfigID)
                ? monitorHistory.monitorList.Where(
                    _ =>
                        (_.MonitorStatus.ToLowerInvariant().Equals("stopped") || _.MonitorStatus.ToLowerInvariant().Contains("not")) &&
                        _.ConfigID == monitorEntity.ConfigID).ToList()
                : new List<MonitorEntity>();


            var grayStatus = "gray";//(!runningList.Any() && !outageList.Any()) ? "gray" : "green";
            
            if (subscription.Type == SubscptionType.Daily.ToString())
            {
                var startTime = subscription.StartTime;
                    //Convert.ToDateTime(DateTime.Now.ToShortDateString() + " 00:00:00")
                    //    .AddDays(-1)
                    //    .AddHours(Convert.ToInt32(subscription.Time));

                while (startTime != subscription.EndTime)
                {
                    statusDict.Add(startTime, grayStatus);

                    startTime = startTime.AddHours(1);
                }
            }

            if (!runningList.Any() && !outageList.Any()) return statusDict;

            foreach (var statusTime in from statusTime in statusDict.Keys.ToList()
                from outage in outageList
                                       let outageStart = outage.MonitorCreatedDateTime
                let outageEnd = outage.MonitorUpdatedDateTime
                where TimeBetween(statusTime, Convert.ToDateTime(outage.MonitorCreatedDateTime),
                    Convert.ToDateTime(outage.MonitorUpdatedDateTime))
                select statusTime)
            {
                statusDict[statusTime] = "red";
            }

            foreach (var statusTime in from statusTime in statusDict.Keys.ToList()
                                       from running in runningList
                                       let outageStart = running.MonitorCreatedDateTime
                                       let outageEnd = running.MonitorUpdatedDateTime
                                       where TimeBetween(statusTime, Convert.ToDateTime(running.MonitorCreatedDateTime),
                                           Convert.ToDateTime(running.MonitorUpdatedDateTime))
                                       select statusTime)
            {
                statusDict[statusTime] = "green";
            }

            return statusDict;
        }


        /// <summary>
        /// Update the last and next service run time in the schedular table
        /// </summary>
        /// <param name="schedulers"></param>
        private void UpdateServiceRunNextTime(List<Scheduler_details> schedulers, DateTime LastServiceRunTime)
        {
            try
            {
                foreach (Scheduler_details scheduler in schedulers)
                {
                    //Update Last and next service run time in the scheduler table
                    _schedulerData.UPdateSchedule(scheduler.Scheduler_id, LastServiceRunTime);
                    Logger.Log("Last and next service run time updated for schedular :" + scheduler.Scheduler_id + " - " + scheduler.Env_name);
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex.ToString());
            }
        }

        #region Private Properties

        /// <summary>
        ///  Gets the TRW.Common.Net.Mail.MailTemplateParser object
        /// </summary>        
        /// <returns>TRW.Common.Net.Mail.MailTemplateParser object</returns>
        private MailTemplateParser MailTemplate
        {
            get
            {
                MailTemplateParser mp = new MailTemplateParser();
                // Get the mail templates
                mp.Parse();
                return mp;
            }
        }

        #endregion

        #region Get Subscription

        private List<Subscription> GetSubscription(int id)
        {
            var subscriptions = new List<Subscription>();

            if (id >= 0)
            {
                subscriptions = _schedulerData.GetSubscription(id);

                if (subscriptions == null || subscriptions.Count == 0) return subscriptions;

                foreach (var subscription in subscriptions)
                {
                    var detail = _schedulerData.GetSubscriptionDetails(subscription.Id);
                    subscription.SubscriptionDetails = detail;
                }
            }
            return subscriptions;
        }

        #endregion Get Subscription

        private static bool TimeBetween(DateTime time, DateTime startDateTime, DateTime endDateTime)
        {
            // get TimeSpan
            var start = new TimeSpan(startDateTime.Hour, startDateTime.Minute, 0);
            var end = new TimeSpan(endDateTime.Hour, endDateTime.Minute, 0);

            // convert datetime to a TimeSpan
            var now = time.TimeOfDay;
            // see if start comes before end
            
            //if (start < end)
            //    return start <= now && now <= end;
            
            // start is after end, so do the inverse comparison

            //return !(end < now && now < start);

            var bo = startDateTime < time.AddHours(1) && endDateTime > time;
            return bo;
        }
    }

    public enum SubscptionType
    {
        Daily = 0,
        Weekly,
        Monthly
    }
}
