using System;
using System.Collections.Generic;
using System.Configuration;
using Cosmo.Entity;
using Cosmo.Service;
using Cosmo.Web.controls;

namespace Cosmo.Web.forms
{
    public partial class Schedule : System.Web.UI.Page
    {
        #region constants & variables
        private string CONTENTSERVICE = Convert.ToString(ConfigurationManager.AppSettings["ContentService"]);
        private string DISPATCHERSERVICE = Convert.ToString(ConfigurationManager.AppSettings["DispatcherService"]);
        private string SCHEDULERENDTIME = Convert.ToString(ConfigurationManager.AppSettings["SchedulerEndTime"]);

        private const string ALLDAYS = "alldays";
        string selectedWeekDays = string.Empty;
        string schID = string.Empty;
        string sConfigHostPort = string.Empty;
        bool isPageClose = false;
        #region Constants

        private const string SUCCESS_MESSAGE = "Scheduler has been successfully saved.";
        private const string FAILURE_MESSAGE = "Scheduler has not been saved.";
        private const string ALREADY_EXISTS_MESSAGE = "Scheduler has already exists. Please try new environment";
        private const string UPDATED = "updated";
        private const string FAILED = "failed";

        #endregion Constants

        #endregion constants & variables

        #region Object instantiation

        SchedulerEntity scheduler = new SchedulerEntity();
        EnvironmentService enviService = new EnvironmentService();
        SchedulerServices schedulerServices = new SchedulerServices();

        #endregion Object instantiation

        #region viewstate
        private bool isDataUpdate
        {
            get { return Convert.ToBoolean(ViewState["vstIsUpdated"]); }
            set { ViewState["vstIsUpdated"] = value; }
        }
        private string sEnvID
        {
            get { return Convert.ToString(ViewState["vstEnvID"]); }
            set { ViewState["vstEnvID"] = value; }
        }
        private string sEnvConfigID
        {
            get { return Convert.ToString(ViewState["vstEnvConfigID"]); }
            set { ViewState["vstEnvConfigID"] = value; }
        }
        private string sEditSchedule
        {
            get { return Convert.ToString(ViewState["vstEditSchedule"]); }
            set { ViewState["vstEditSchedule"] = value; }
        }
        private string envName
        {
            get { return Convert.ToString(ViewState["vstEnvName"]); }
            set { ViewState["vstEnvName"] = value; }
        }
        #endregion viewstate

        protected void Page_Load(object sender, EventArgs e)
        {
            CommonService.ValidateUser();
            if (!IsPostBack)
            {
                rdoFrequency.Focus();
                hidIsDataUpdated.Value = string.Empty;
                InitialseValues();

                if (Request.QueryString["e"] != null && !string.IsNullOrEmpty(Request.QueryString["e"]))
                {
                    sEnvID = Request.QueryString["e"];
                    if (Request.QueryString["c"] != null)
                    {
                        sEnvConfigID = Request.QueryString["c"];
                    }
                    else
                        sEnvConfigID = "0";

                    if (Request.QueryString["ed"] != null)
                    {
                        sEditSchedule = Request.QueryString["ed"];
                    }

                    if (!string.IsNullOrEmpty(sEnvConfigID) && !string.IsNullOrEmpty(sEnvID))
                    {
                        if (int.Parse(sEnvConfigID) >= 0 && int.Parse(sEnvID) >= 0)
                            GetConfigDetails(Convert.ToInt32(sEnvConfigID), Convert.ToInt32(sEnvID));
                    }

                    //Get exising scheduler details
                    if (!string.IsNullOrEmpty(sEnvConfigID) && int.Parse(sEnvConfigID) > 0 && int.Parse(sEnvID) > 0)
                    {
                        scheduler = schedulerServices.GetSchedulerDetails(Convert.ToInt32(sEnvID), Convert.ToInt32(sEnvConfigID));
                        if (scheduler != null && scheduler.SchedulerID > 0)
                        {
                            PopulateScheduler(scheduler);
                        }
                    }
                }
                else
                {
                    if (Request.QueryString["ed"] != null)
                    {
                        sEditSchedule = Request.QueryString["ed"];
                    }
                    if (Request.QueryString["ename"] != null)
                    {
                        envName = Request.QueryString["ename"];
                        txtEnvironmentName.InnerText = envName;
                    }
                    if (Request.QueryString["sp"] != null)
                    {
                        envName = Request.QueryString["sp"];
                        txtPort.InnerText = envName;
                    }
                }
            }
        }

        private void PopulateScheduler(SchedulerEntity schedulerDetail)
        {
            selectedWeekDays = string.Empty;
            if (schedulerDetail.Interval > 0)
                hidIntervalEdit.Value = Convert.ToString(schedulerDetail.Interval);
            if (!string.IsNullOrEmpty(schedulerDetail.Duration))
            {
                if (schedulerDetail.Duration == "Seconds")
                    rdoFrequency.Items[0].Selected = true;
                else if (schedulerDetail.Duration == "Minutes")
                    rdoFrequency.Items[1].Selected = true;
                else if (schedulerDetail.Duration == "Hours")
                    rdoFrequency.Items[2].Selected = true;
                else if (schedulerDetail.Duration == "Daily")
                    rdoFrequency.Items[3].Selected = true;
                else if (schedulerDetail.Duration == "Weekly")
                    rdoFrequency.Items[4].Selected = true;
                else if (schedulerDetail.Duration == "Monthly")
                    rdoFrequency.Items[5].Selected = true;

            }
            if (schedulerDetail.StartDateTime != null)
            {
                txtStartDate.Text = String.Format("{0:MM/dd/yyyy HH:mm}", schedulerDetail.StartDateTime); //Convert.ToString(schedulerDetail.StartDateTime);
                //txtStartDate.Text = String.Format("{0:MM/dd/yyyy}", schedulerDetail.StartDateTime); //Convert.ToString(schedulerDetail.StartDateTime);




            }
            if (!string.IsNullOrEmpty(schedulerDetail.RepeatsOn))
            {
                selectedWeekDays = schedulerDetail.RepeatsOn;
                PopulateWeekdays(selectedWeekDays);
            }


            if (!string.IsNullOrEmpty(schedulerDetail.EndAs))
            {
                if (schedulerDetail.EndAs == "never")
                    rdoEndsTime_1.Checked = true;
                else if (schedulerDetail.EndAs == "after")
                {
                    rdoEndsTime_2.Checked = true;
                    txtOccurance.Text = Convert.ToString(schedulerDetail.EndOfOccurance);
                }
                else if (schedulerDetail.EndAs == "on")
                {
                    rdoEndsTime_3.Checked = true;
                    txtEndOn.Text = String.Format("{0:MM/dd/yyyy HH:mm}", schedulerDetail.EndDateTime); // Convert.ToString(schedulerDetail.EndDateTime); 
                }
            }
        }

        /// <summary>
        /// Populate the selected days when edit the scheduler
        /// </summary>
        /// <param name="selectedDays"></param>
        private void PopulateWeekdays(string selectedDays)
        {
            IList<NamedBusinessCode> days = CommonUtility.CodesToList(selectedDays);
            foreach (NamedBusinessCode day in days)
            {
                if (day.Code == chkSunday.Value) chkSunday.Checked = true;
                else if (day.Code == chkMonday.Value) chkMonday.Checked = true;
                else if (day.Code == chkTuesday.Value) chkTuesday.Checked = true;
                else if (day.Code == chkWednesday.Value) chkWednesday.Checked = true;
                else if (day.Code == chkThursday.Value) chkThursday.Checked = true;
                else if (day.Code == chkFriday.Value) chkFriday.Checked = true;
                else if (day.Code == chkSaturday.Value) chkSaturday.Checked = true;

            }
        }

        private void InitialseValues()
        {
            txtStartDate.Text = DateTime.Now.ToString("MM/dd/yyyy HH:mm");
            //txtStartDate.Text = DateTime.Now.ToString("MM/dd/yyyy HH:mm");
            txtEndOn.Text = DateTime.Now.AddDays(1).ToString("MM/dd/yyyy HH:mm");
            Session["SchedulerDetails"] = null;
        }

        protected void btnCreate_Click(object sender, EventArgs e)
        {
            btnCreate_Save();
        }

        protected void btnCreate_Save()
        {
            int status;
            scheduler = new SchedulerEntity();
            try
            {

                scheduler.EnvID = !string.IsNullOrEmpty(sEnvID) ? Convert.ToInt32(sEnvID) : 0;

                scheduler.ConfigID = !string.IsNullOrEmpty(sEnvConfigID) ? Convert.ToInt32(sEnvConfigID) : 0;

                scheduler.Duration = !string.IsNullOrEmpty(rdoFrequency.SelectedItem.Value) ? Convert.ToString(rdoFrequency.SelectedItem.Text) : "";

                if (!string.IsNullOrEmpty(hidInterval.Value))
                {
                    scheduler.Interval = Convert.ToInt32(hidInterval.Value);
                    hidIntervalEdit.Value = hidInterval.Value;
                }
                else
                    scheduler.Interval = 0;

                if (!string.IsNullOrEmpty(txtStartDate.Text))
                {
                    scheduler.StartDateTime = Convert.ToDateTime(txtStartDate.Text);
                    
                }
                else
                    scheduler.StartDateTime = null;

                //Get all the selected day(s) in case of frequency weekly 
                if (!string.IsNullOrEmpty(rdoFrequency.SelectedItem.Value) && rdoFrequency.SelectedItem.Value == "5")
                {
                    scheduler.RepeatsOn = GetSlectedWeekDays();
                }
                else
                    scheduler.RepeatsOn = string.Empty;

                if (rdoEndsTime_1.Checked)
                {
                    scheduler.EndAs = "never";
                }
                else if (rdoEndsTime_2.Checked)
                {
                    scheduler.EndAs = "after";
                    if (!string.IsNullOrEmpty(txtOccurance.Text))
                    {
                        scheduler.EndOfOccurance = Convert.ToInt32(txtOccurance.Text);
                        scheduler.EndDateTime = GetScheduleEndTime(Convert.ToInt32(txtOccurance.Text), Convert.ToInt32(hidInterval.Value), Convert.ToString(rdoFrequency.SelectedItem.Text), Convert.ToDateTime(scheduler.StartDateTime));
                    }
                    else
                    {
                        scheduler.EndOfOccurance = 0;
                    }
                }
                else if (rdoEndsTime_3.Checked)
                {
                    scheduler.EndAs = "on";
                    scheduler.EndDateTime = Convert.ToDateTime(txtEndOn.Text);// + " " + SCHEDULERENDTIME);
                }

                scheduler.IsActive = true;
                if (!string.IsNullOrEmpty(hidSchSummary.Value))
                {
                    scheduler.Comments = hidSchSummary.Value.Substring(10, hidSchSummary.Value.Length - 10); //txtComments.Text;
                }

                if (string.IsNullOrEmpty(sEditSchedule))
                {

                    status = schedulerServices.InsertUpdateScheduler(scheduler);
                    if (status == 0)
                    {
                        ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), "setWindowHeight(true);", true);
                        isDataUpdate = true;
                        hidIsDataUpdated.Value = UPDATED;
                        genericMessage.Visible = true;
                        genericMessage.CurMessageType = GenericMessage.MessageType.Confirmation;
                        genericMessage.ShowMessage(SUCCESS_MESSAGE);

                    }
                    else if (status == 1)
                    {
                        ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), "setWindowHeight(true);", true);
                        genericMessage.Visible = true;
                        hidIsDataUpdated.Value = FAILED;
                        genericMessage.CurMessageType = GenericMessage.MessageType.Notification;
                        genericMessage.ShowMessage(ALREADY_EXISTS_MESSAGE);

                    }
                    else if (status == 2)
                    {
                        ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), "setWindowHeight(true);", true);
                        genericMessage.Visible = true;
                        hidIsDataUpdated.Value = FAILED;
                        genericMessage.CurMessageType = GenericMessage.MessageType.Failure;
                        genericMessage.ShowMessage(FAILURE_MESSAGE);

                    }
                    else if (status == 3)
                    {
                    }
                }
                else if (sEditSchedule == "new")
                {
                    Session["SchedulerDetails"] = scheduler;
                    ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), "alert('Schedule detail will be updated with the Service Configuration. Please Close this Schedule window and click Save in Environment Window');", true);
                    //ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), "setWindowHeight(true);", true);
                    //isDataUpdate = true;
                    hidIsDataUpdated.Value = "dataPassed";
                    //genericMessage.Visible = true;
                    //genericMessage.CurMessageType = GenericMessage.MessageType.Confirmation;
                    //genericMessage.ShowMessage(SUCCESS_MESSAGE);

                }
            }
            catch (Exception ex)
            {
                genericMessage.Visible = true;
                genericMessage.CurMessageType = GenericMessage.MessageType.Failure;
                genericMessage.ShowMessage(FAILURE_MESSAGE);
                Logger.Log(ex.ToString());
            }
        }

        private string GetSlectedWeekDays()
        {
            string selectedWD = string.Empty;
            if (!string.IsNullOrEmpty(rdoFrequency.SelectedItem.Value) && rdoFrequency.SelectedItem.Value == "5")
            {
                if (chkSunday.Checked && chkMonday.Checked && chkTuesday.Checked && chkWednesday.Checked && chkThursday.Checked && chkFriday.Checked && chkSaturday.Checked)
                {
                    selectedWD = ALLDAYS;
                }
                else
                {
                    if (chkSunday.Checked)
                        selectedWD = "sun";
                    if (chkMonday.Checked)
                    {
                        if (!string.IsNullOrEmpty(selectedWD))
                            selectedWD += ",mon";
                        else
                            selectedWD += "mon";
                    }
                    if (chkTuesday.Checked)
                    {
                        if (!string.IsNullOrEmpty(selectedWD))
                            selectedWD += ",tue";
                        else
                            selectedWD += "tue";
                    }
                    if (chkWednesday.Checked)
                    {
                        if (!string.IsNullOrEmpty(selectedWD))
                            selectedWD += ",wed";
                        else
                            selectedWD += "wed";
                    }
                    if (chkThursday.Checked)
                    {
                        if (!string.IsNullOrEmpty(selectedWD))
                            selectedWD += ",thu";
                        else
                            selectedWD += "thu";
                    }
                    if (chkFriday.Checked)
                    {
                        if (!string.IsNullOrEmpty(selectedWD))
                            selectedWD += ",fri";
                        else
                            selectedWD += "fri";
                    }
                    if (chkSaturday.Checked)
                    {
                        if (!string.IsNullOrEmpty(selectedWD))
                            selectedWD += ",sat";
                        else
                            selectedWD += "sat";
                    }
                }
            }

            return selectedWD;
        }

        private void PopulateEnvironments()
        {
            List<NamedBusinessEntity> enviList = new List<NamedBusinessEntity>();
            enviList = enviService.GetEnvironmentSelect(0);
            if (enviList != null && enviList.Count > 0)
            {

            }
        }

        private void GetConfigDetails(int configID, int envID)
        {
            EnvDetailsEntity detail = new EnvDetailsEntity();
            detail = enviService.GetEnvironmentDetail(Convert.ToInt32(configID), Convert.ToInt32(envID));
            if (detail != null)
            {
                txtEnvironmentName.InnerText= detail.EnvDet_Name;
                if (configID > 0)
                    txtPort.InnerText = detail.EnvDetHostIP + ":" + detail.EnvDetPort;
            }
        }

        private DateTime GetScheduleEndTime(int ocurance, int interval, string duration, DateTime startTime)
        {
            DateTime tempEndTime = new DateTime();
            if (duration == "Seconds")
            {
                tempEndTime = startTime.AddSeconds(interval * ocurance);
            }
            else if (duration == "Minutes")
            {
                tempEndTime = startTime.AddMinutes(interval * ocurance);
            }
            else if (duration == "Hours")
            {
                tempEndTime = startTime.AddHours(interval * ocurance);
            }
            else if (duration == "Daily")
            {
                tempEndTime = startTime.AddDays(interval * ocurance);
            }
            else if (duration == "Weekly")
            {
                tempEndTime = startTime.AddDays(interval * 7 * ocurance);
            }
            else if (duration == "Monthly")
            {
                tempEndTime = startTime.AddMonths(interval * ocurance);
            }


            return tempEndTime;
        }

        protected void btnCreateClose_Click(object sender, EventArgs e)
        {
            btnCreate_Save();
            isPageClose = true;
        }

        void Page_LoadComplete(object sender, EventArgs e)
        {
            // call your download function
            if (isPageClose)
                ClientScript.RegisterStartupScript(this.GetType(), "newScheduler", "fnGetValue('dataPassed');", true);

        }
    }
}