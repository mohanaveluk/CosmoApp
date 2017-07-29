using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cog.CSM.Entity;
using Cog.CSM.Data;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;

namespace Cog.CSM.Service
{
    public class WindowsService
    {
        #region objects
        public WindowsServiceData winData = new WindowsServiceData();
        #endregion objects

        #region Variables
        
        #endregion Variables

        #region Get all Group Schedule scrvice details

        public void GroupScheduleDetails()
        {
            string serviceResukt = string.Empty;
            List<GroupScheduleEntity> groupScheduleDetails = GetGroupOpenScheduleDetails();

            if (groupScheduleDetails != null && groupScheduleDetails.Count > 0)
            {
                foreach (GroupScheduleEntity groupSD in groupScheduleDetails)
                {
                    List<GroupScheduleEntity> tempEntity = new List<GroupScheduleEntity>();
                    foreach (GroupScheduleDetailEntity entity in groupSD.GroupScheduleDetails)
                    {
                        foreach (GroupScheduleServiceDetailEntity grpService in entity.ServiceDetails)
                        {
                            serviceResukt = GetWindowssService(grpService.WindowsService_Name, groupSD.Group_Schedule_Action, grpService.HostIP, grpService.WindowsService_ID.ToString());
                            if (serviceResukt == string.Empty || serviceResukt.ToLower() == "completed")
                            {

                            }
                        }
                    }
                }
            }
        }

        public List<GroupScheduleEntity> GetGroupOpenScheduleDetails()
        {
            List<GroupScheduleEntity> groupScheduleEntity = new List<GroupScheduleEntity>();
            List<GroupScheduleDetailEntity> groupScheduleDetailEntity = new List<GroupScheduleDetailEntity>();
            List<GroupScheduleServiceDetailEntity> groupScheduleServiceDetailEntity = new List<Entity.GroupScheduleServiceDetailEntity>();

            groupScheduleEntity = winData.GetGroupOpenScheduleDetails(DateTime.Now,"O", "sch");
            if (groupScheduleEntity != null && groupScheduleEntity.Count > 0)
            {
                groupScheduleDetailEntity = winData.GetGroupOpenScheduleEnvDetails(DateTime.Now,"O", "env");
                if (groupScheduleDetailEntity != null && groupScheduleDetailEntity.Count > 0)
                {
                    groupScheduleEntity[0].GroupScheduleDetails = new List<GroupScheduleDetailEntity>();
                    foreach (GroupScheduleDetailEntity gsd in groupScheduleDetailEntity)
                    {
                        groupScheduleServiceDetailEntity = winData.GetGroupOpenScheduleServiceDetails(DateTime.Now,"O", "cfg");
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

        #region Run Windows scrvice 

        public static string GetWindowssService(string serviceName, string serviceMode, string systemName, string serviceID)
        {
            string result = string.Empty;
            bool isTimeout = true;
            string serviceAction = string.Empty;
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            CancellationToken token = cancellationTokenSource.Token;
            try
            {
                if (serviceMode == "1" || serviceMode == "start")
                    serviceAction = "start";
                else if (serviceMode == "2" || serviceMode == "stop")
                    serviceAction = "stop";
                else if (serviceMode == "3" || serviceMode == "restart")
                    serviceAction = "restart";
                if (!string.IsNullOrEmpty(serviceAction) && !string.IsNullOrEmpty(serviceName))
                {
                    /*Task t = new Task(() =>
                    {
                        try
                        {

                        ProcessStartInfo processInfo = new ProcessStartInfo(fileName);
                        processInfo.Arguments = "\"" + serviceName + "\" \"" + serviceAction + "\" \"" + systemName + "\"";
                        processInfo.Verb = "runas";
                        processInfo.UseShellExecute = true;
                        Process.Start(processInfo);
                        }
                        catch (Exception exTask)
                        {
                            Logger.Log(exTask.Message);
                        }
                    });
                    t.Start();*/
                    //To be uncommented
                    /*ProcessStartInfo processInfo = new ProcessStartInfo(fileName);
                    processInfo.Arguments = "\"" + serviceName + "\" \"" + serviceAction + "\" \"" + systemName + "\"";
                    processInfo.Verb = "runas";
                    processInfo.UseShellExecute = true;
                    Process.Start(processInfo);*/
                    long sum = 0;
                    int i;
                    Task t = Task.Factory.StartNew(() =>
                    {
                        //token.ThrowIfCancellationRequested();

                        Random rnd = new Random();
                        for (i = 0; i < 90000000; i++)
                        {
                            if (token.IsCancellationRequested)
                            {
                                //token.ThrowIfCancellationRequested();
                                result = "cancelled";
                                break;
                            }
                            int number = rnd.Next(0, 101);
                            sum += number;

                        }
                        isTimeout = false;
                    }, token);
                    try
                    {
                        t.Wait(2000, token);
                        cancellationTokenSource.Cancel();

                        if (isTimeout)
                        {
                            result = "timedout";
                        }
                        else
                            result = sum.ToString();// "processing";

                    }
                    catch (AggregateException agex)
                    {
                        Logger.Log("Windows Service operation: " + agex.ToString());
                        throw;
                    }
                    finally
                    {
                        cancellationTokenSource.Dispose();
                    }


                }
            }
            catch (Exception ex)
            {
                result = "abonded";
                Logger.Log(ex.Message);
            }
            return result;
        }

        #endregion Run Windows scrvice
    }
}
