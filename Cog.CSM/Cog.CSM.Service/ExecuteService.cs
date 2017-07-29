using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cog.CSM.Entity;
using Cog.CSM.Data;
using System.Net;
using HtmlAgilityPack;
using Cog.CSM.MailService;
using System.Configuration;
using System.Runtime.Remoting.Channels;
using System.Threading;
using System.Threading.Tasks;

namespace Cog.CSM.Service
{
    public class ExecuteService
    {
        #region Variables
        private const string SERVICE_TYPE_CONTENT = "1";//"Content";
        private const string SERVICE_TYPE_DESPATCHER = "2";//"Despatcher";
        private const string SERVICE_STATUS_RUNNING = "Running";
        private const string SERVICE_STATUS_STANDBY = "Standby";
        private const string SERVICE_NOT_RUNNING = "Not Running";
        private const string SERVICE_NOT_READY = "Not Ready";
        private const string CONTENT_MANAGER = "Content Manager";
        private const string DESPATCHER = "Dispatcher";

        private static readonly string DbType = ConfigurationManager.AppSettings["DatabaseType"];
        private  string SERVICE_STOPPED = Convert.ToString(ConfigurationManager.AppSettings["ServiceStopped"]);
        private  string SERVICE_FAILED = Convert.ToString(ConfigurationManager.AppSettings["ServiceFailure"]);

        private  string CONTENT_SERVICE = Convert.ToString(ConfigurationManager.AppSettings["ContentService"]);
        private  string DESPATCHER_SERVICE = Convert.ToString(ConfigurationManager.AppSettings["DespatcherService"]);
        private string RescanInterval = Convert.ToString(ConfigurationManager.AppSettings["RescanInterval"]);

        #endregion Variables

        #region object instantiation

        readonly ContentManager _contentManager;
        readonly IExecuteServiceData _executeServiceData;
        private readonly MailService _mailService;
        List<SendNotificationEntity> _sendNotificationList;
        private int _rescnaInterval;

        public delegate void AsyncExecuteSingleService(ServiceEntity service);

        #endregion object instantiation

        public ExecuteService(MailService mailService)
        {
            _mailService = mailService;
            _contentManager = new ContentManager();
            _sendNotificationList = new List<SendNotificationEntity>();

            _rescnaInterval = !string.IsNullOrEmpty(RescanInterval) ? Convert.ToInt32(RescanInterval)*1000 : 0;

            var iDbType = DbType == DatabaseType.Oracle.ToString()
                ? Convert.ToInt32(DatabaseType.Oracle).ToString()
                : Convert.ToInt32(DatabaseType.SqlServer).ToString();

            _executeServiceData = new ExecuteServiceDataFactory().Create(Convert.ToInt32(iDbType).ToString());
        }

        /// <summary>
        /// Run all the scheduled services 
        /// </summary>
        public void RunAllScheduledJobs(List<ServiceEntity> serviceList)
        {
            try
            {
                //Get the list of send Notification for all services
                _sendNotificationList = GetAllSendMailNotification();

                foreach (var service in serviceList)
                {
                    var caller = new AsyncExecuteSingleService(ExecuteSingleService);
                    {
                        Logger.Log(
                            $"Env Id: {service.Env_ID}, Name: {service.Env_Name}, Configuration Id: {service.Config_ID}, Scheduler Id: {service.Config_ID} and Executing service uri: {service.Config_ServiceURL}");

                        //caller.BeginInvoke(service, null, null);
                        ExecuteSingleService(service);
                        //Thread.Sleep(1000);
                    }
                }
            }
            catch
                (Exception
                    ex)
            {
                Logger.Log("Error: \n" + ex.ToString());
            }
        }

        /// <summary>
        /// Execute single job / service
        /// </summary>
        /// <param name="service"></param>
        private void ExecuteSingleService(ServiceEntity service)
        {
            try
            {
                if (service.Config_ServiceType != null && service.Config_ServiceType.Contains(SERVICE_TYPE_CONTENT))
                {
                    if (!string.IsNullOrEmpty(service.Config_ServiceURL))
                    {
                        //Logger.Log("Verifying status for " + service.Config_ServiceURL);
                        var content = GetCmStatus(service.Config_ServiceURL);

                        var contentLastStatus = _executeServiceData.GetServiceLastStatus(service) ?? string.Empty;
                        Logger.Log(string.Format("Content manager last status: {0}, current status: {1}",
                            contentLastStatus,
                            content.Status));

                        if (!content.Status.ToLower().Equals(contentLastStatus.ToLower()))
                        {
                            Thread.Sleep(_rescnaInterval);
                            Logger.Log("Verifying status again for " + service.Config_ServiceURL);
                            content = GetCmStatus(service.Config_ServiceURL);
                            Logger.Log(string.Format("Content manager last status: {0}, current status: {1}",
                                contentLastStatus,
                                content.Status));
                        }

                        if (!string.IsNullOrEmpty(service.Env_ID.ToString()))
                        {
                            content.EnvId = service.Env_ID;
                            content.EnvName = service.Env_Name;
                        }

                        _executeServiceData.InsUpdMonitorService(service, content);
                        if (!content.Status.ToLower().Contains(SERVICE_STOPPED.ToLower()) &&
                            !content.Status.ToLower().Contains(SERVICE_FAILED.ToLower()) &&
                            !content.Status.ToLower().Contains(SERVICE_NOT_RUNNING.ToLower())) return;

                        content.ServiceType = CONTENT_MANAGER;
                        content.HostIPPort = service.Config_ServerIP + ":" + service.Config_PortNumber;
                        if (IsSendNotification(_sendNotificationList, service.Config_ID))
                            _mailService.SendMail(service.Env_ID, service.Config_ID, CONTENT_SERVICE, content);
                        else
                            Logger.Log("Mail status: Skipped");
                    }
                }
                else if (service.Config_ServiceType != null &&
                         service.Config_ServiceType.Contains(SERVICE_TYPE_DESPATCHER))
                {
                    if (!string.IsNullOrEmpty(service.Config_ServiceURL))
                    {
                        var despatcher = GetGcStatus(service.Config_ServiceURL);

                        var despatcherLastStatus = _executeServiceData.GetServiceLastStatus(service) ?? string.Empty;
                        Logger.Log(string.Format("Dispatcher last status: {0}, current status: {1}",
                            despatcherLastStatus,
                            despatcher.Status));

                        if (!despatcher.Status.ToLower().Equals(despatcherLastStatus.ToLower()))
                        {
                            Thread.Sleep(_rescnaInterval);
                            Logger.Log("Verifying status again for " + service.Config_ServiceURL);
                            despatcher = GetGcStatus(service.Config_ServiceURL);
                            Logger.Log(string.Format("Dispatcher last status: {0}, current status: {1}",
                                despatcherLastStatus,
                                despatcher.Status));
                        }

                        if (!string.IsNullOrEmpty(service.Env_ID.ToString()))
                        {
                            despatcher.EnvId = service.Env_ID;
                            despatcher.EnvName = service.Env_Name;
                        }

                        _executeServiceData.InsUpdMonitorService(service, despatcher);
                        if (despatcher.Status.ToLower().Contains(SERVICE_STOPPED.ToLower()) ||
                            despatcher.Status.ToLower().Contains(SERVICE_FAILED.ToLower()) ||
                            despatcher.Status.ToLower().Contains(SERVICE_NOT_RUNNING.ToLower()))
                        {
                            despatcher.ServiceType = DESPATCHER;
                            despatcher.HostIPPort = service.Config_ServerIP + ":" + service.Config_PortNumber;
                            if (IsSendNotification(_sendNotificationList, service.Config_ID))
                                _mailService.SendMail(service.Env_ID, service.Config_ID, DESPATCHER_SERVICE,
                                    despatcher);
                            else
                                Logger.Log("Mail status: Skipped");
                        }
                    }
                }
                else
                {
                    Logger.Log("Invalid service type - " + service.Config_ServiceType);
                }
            }
            catch (Exception ex)
            {
                Logger.Log("Error: " + ex);
            }
        }

        /// <summary>
        /// Get Html content from the url that is passed and parse to get the content of the webpage
        /// Content Manager status
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private ContentManager GetCmStatus(string url)
        {
            var currentLine = string.Empty;
            var client = new WebClient();
            var cManager = new ContentManager();

            try
            {
                cManager.Url = url;
                var htmlContent = client.DownloadString(url);

                //client.DownloadStringCompleted += (sender, e) =>
                //{
                //    htmlContent = e.Result;
                //};

                //client.DownloadStringAsync(new Uri(url));

                var htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(htmlContent);
                if (htmlDocument.DocumentNode.Descendants().Any(n => n.Name.ToLower() == "font"))
                {
                    var htmlNodes = htmlDocument.DocumentNode.Descendants().Where(n => n.Name.ToLower() == "font").Select((n, i) => new { Id = i, Content = n.InnerText });

                    foreach (var node in htmlNodes)
                    {


                        if (currentLine.ToLower().Contains("build"))
                        {
                            //Console.WriteLine("Build : " + node.Content);
                            cManager.Build = node.Content;
                        }

                        if (currentLine.ToLower().Contains("start time"))
                        {
                            //Console.WriteLine("Start time : " + node.Content);
                            cManager.StartTime = node.Content;
                        }

                        if (currentLine.ToLower().Contains("current time"))
                        {
                            // Console.WriteLine("Current time : " + node.Content);
                            cManager.CurrentTime = node.Content;
                        }

                        if (currentLine.ToLower().Contains("state"))
                        {
                            //Console.WriteLine("State : " + node.Content)
                            if (node.Content.Contains("Running as standby"))
                                cManager.Status = SERVICE_STATUS_STANDBY;
                            else if (node.Content.ToLower() == SERVICE_STATUS_RUNNING.ToLower())
                                cManager.Status = SERVICE_STATUS_RUNNING;
                            else if (node.Content.ToLower() == SERVICE_NOT_RUNNING.ToLower())
                                cManager.Status = SERVICE_NOT_RUNNING;
                            else
                                cManager.Status = node.Content;
                        }
                        cManager.Comments = "Content manager, Build: " + cManager.Build + " Status:" + node.Content;
                        

                        currentLine = node.Content;
                    }
                }
            }
            catch (Exception ex)
            {
                
                //throw;
                Logger.Log("Error: " + ex.Message.ToString());
                if(ex.InnerException !=null)
                    Logger.Log("Error: " + ex.InnerException.Message.ToString());
                if (ex.Message.Contains("(404) Not Found"))
                {
                    cManager.Comments = "404 - Page not found";
                }
                else if (ex.Message.Contains("(500) Internal Server Error"))
                {
                    cManager.Comments = "(500) Internal Server Error";
                }
                else
                {
                    cManager.Comments = ex.Message;
                }
                cManager.Build = string.Empty;
                cManager.Status = "Stopped";
                cManager.StartTime = string.Empty;
                cManager.CurrentTime = string.Format("{0:F}", DateTime.Now);
            }
            return cManager;
        }

        /// <summary>
        /// Get Html content from the url that is passed and parse to get the content of the webpage
        /// Content Manager status
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private Despatcher GetGcStatus(string url)
        {
            var despatcher = new Despatcher();

            try
            {
                despatcher.Url = url;
                System.Uri uri = new Uri(url);
                string htmlContent;
                using(var client = new WebClient())
                {
                    client.Headers[HttpRequestHeader.UserAgent] = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/535.2 (KHTML, like Gecko) Chrome/15.0.874.121 Safari/535.2";
                    htmlContent = client.DownloadString(uri);
                }

                var htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(htmlContent);
                if (htmlDocument.DocumentNode.Descendants().Any(n => n.Name.ToLower() == "body"))
                {
                    var htmlNodes = htmlDocument.DocumentNode.Descendants().Where(n => n.Name.ToLower() == "body").Select((n, i) => new { Id = i, Content = n.InnerText });

                    foreach (var node in htmlNodes)
                    {
                        if (node.Content == null) continue;
                        if (node.Content.Contains("Dispatcher is ready"))
                            despatcher.Status = SERVICE_STATUS_RUNNING;
                        else if (node.Content.Contains("not ready"))
                            despatcher.Status = SERVICE_NOT_READY;
                        else
                            despatcher.Status = node.Content;

                        despatcher.Comments =  node.Content.Replace("\r\n"," ").Trim();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log("Error: " + ex);
                if (ex.InnerException != null)
                    Logger.Log("Error: " + ex.InnerException.Message.ToString());
                if (ex.Message.Contains("(404) Not Found"))
                {
                    despatcher.Comments = "404 - Page not found";
                }
                else if (ex.Message.Contains("(500) Internal Server Error"))
                {
                    despatcher.Comments = "(500) Internal Server Error";
                }
                else if (ex.Message.Contains("Server Unavailable"))
                {
                    despatcher.Comments = "The remote server returned an error: (503) Server Unavailable.";
                    despatcher.Status = SERVICE_NOT_READY;
                }
                else
                {
                    despatcher.Comments = ex.Message;
                }
                _contentManager.Build = string.Empty;
                if (string.IsNullOrEmpty(despatcher.Status))
                    despatcher.Status = "Stopped";
            }

            return despatcher;
        }

        /// <summary>
        /// Get all services of send notification list 
        /// </summary>
        /// <returns></returns>
        private List<SendNotificationEntity> GetAllSendMailNotification()
        {
            return _executeServiceData.GetAllSendMailNotification(DateTime.Now);
        }

        /// <summary>
        /// to check whether to send a mail notification about the service failure
        /// </summary>
        /// <param name="sendNotificationList"></param>
        /// <param name="configID"></param>
        /// <returns></returns>
        private bool IsSendNotification(List<SendNotificationEntity> sendNotificationList, int configID)
        {
            bool isNotify = false;

            foreach (SendNotificationEntity notify in sendNotificationList)
            {
                if (notify.ConfigID == configID)// &&//(notify.LastMonitorStatus == null || notify.LastMonitorStatus.ToLower().Contains(SERVICE_STOPPED.ToLower()) || notify.LastMonitorStatus.ToLower().Contains(SERVICE_NOT_RUNNING.ToLower())))
                {
                    var tempTime = notify.LastMonitorUpdated;
                    if (DateTime.Now >= tempTime.AddMinutes(notify.ConfigMailFrequency))
                    {
                        isNotify = true;
                    }
                    break;
                }
            }

            return isNotify;
        }
        
    }
}
