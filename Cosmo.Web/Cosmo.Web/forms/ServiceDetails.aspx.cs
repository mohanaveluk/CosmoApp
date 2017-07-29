using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Cosmo.Entity;
using Cosmo.Service;
using Cosmo.Web.controls;

namespace Cosmo.Web.forms
{
    public partial class ServiceDetails : System.Web.UI.Page
    {
        //private string LOGGEDIN_USER = Convert.ToString(ConfigurationManager.AppSettings["LoggedInUser"]);

        EnvironmentService enviService = new EnvironmentService();
        CommonService commonService = new CommonService();
        SchedulerServices schedulerServices = new SchedulerServices();

        #region Constants
        private const string SUCCESS_MESSAGE = "Environment has been successfully saved.";
        private const string FAILURE_MESSAGE = "Environment has not been saved.";
        private const string ALREADY_EXISTS_MESSAGE = "Environment has already exists. Please try new environment";
        private const string N = "add_en";
        private const string NED = "add_ed";
        private const string U = "modify_en";
        private const string UED = "modify_ed";
        private const string UPDATED = "updated";
        private const string CM_TYPE = "1";
        private const string DISP_TYPE = "2";
        private const string ENVIRONMENT_TYPE = "id";

        #endregion Constants

        #region Viewstates
        private string sMode
        {
            get { return ViewState["vstMode"] != null ? ViewState["vstMode"].ToString() : string.Empty; }
            set { ViewState["vstMode"] = value; }
        }
        private bool isDataUpdate
        {
            get { return Convert.ToBoolean(ViewState["vstIsUpdated"]); }
            set { ViewState["vstIsUpdated"] = value; }
        }
        #endregion Viewstates

        string sEnvID = string.Empty;
        string sType = string.Empty;
        int iConfigID = 0;

        protected void Page_Load(object sender, EventArgs e)
        {
            CommonService.ValidateUser();
            PopulateEnvironment();
            if (!IsPostBack)
            {
                Session["emailList"] = null;
                hidIsDataUpdated.Value = string.Empty;
                PopulateTime();
                if (Request.QueryString["s"] != null)
                {
                    sEnvID = Request.QueryString["s"];
                    if (Request.QueryString["t"] != null)
                    {
                        sType = hidServiceType.Value = Request.QueryString["t"];
                        if (sType == "en")
                            sMode = U;
                        else if (sType == "ed")
                            sMode = UED;
                    }
                    PopulateEnvironmentDetails(sEnvID);
                }
                else
                {
                    sEnvID = string.Empty;
                    sMode = N;
                }
            }
        }

        protected void btnCreate_Click(object sender, EventArgs e)
        {
            string environmentName = string.Empty;
            int status = 0, envID = 0; string[] arrExists;

            string arrExistsText = string.Empty;
            string arrExistsConfigID = string.Empty;
            string arrExistsEnviID = string.Empty;

            try
            {
                EnvironmentEntity endEntity = new EnvironmentEntity();

                EnvDetailsEntity envList = new EnvDetailsEntity();

                if (!string.IsNullOrEmpty(txtEnvironment.Text))
                    environmentName = txtEnvironment.Text.Trim();
                else if (!string.IsNullOrEmpty(hidEnvironment.Value))
                    environmentName = hidEnvironment.Value.Trim();

                endEntity.EnvName = environmentName;
                if (!string.IsNullOrEmpty(environmentName))
                {
                    envID = enviService.GetEnvironmentID(environmentName);
                    if (envID > 0)
                        endEntity.EnvID = envID;
                    else
                        endEntity.EnvID = -1;
                }

                envList.EnvID = endEntity.EnvID;
                if (!string.IsNullOrEmpty(txtHostIP.Text.Trim()))
                    envList.EnvDetHostIP = txtHostIP.Text.Trim();
                else
                    envList.EnvDetHostIP = string.Empty;

                if (!string.IsNullOrEmpty(txtPort.Text.Trim()))
                    envList.EnvDetPort = txtPort.Text.Trim();
                else
                    envList.EnvDetPort = string.Empty;

                if (!string.IsNullOrEmpty(txtServerDescripption.Text.Trim()))
                    envList.EnvDetDescription = txtServerDescripption.Text.Trim();
                else
                    envList.EnvDetDescription = string.Empty;

                if (!string.IsNullOrEmpty(txtLocation.Text.Trim()))
                    envList.EnvDetLocation = txtLocation.Text.Trim();
                else
                    envList.EnvDetLocation = string.Empty;

                if (!string.IsNullOrEmpty(drpMailFrequency.SelectedValue))
                    envList.EnvDetMailFrequency = drpMailFrequency.SelectedValue.Trim();
                else
                    envList.EnvDetMailFrequency = string.Empty;

                if (!string.IsNullOrEmpty(drpServiceType.SelectedValue))
                    envList.EnvDetServiceType = drpServiceType.SelectedValue;
                else
                    envList.EnvDetServiceType = string.Empty;

                if (!string.IsNullOrEmpty(hidServiceURL.Value))
                {
                    envList.EnvDetServiceURL = txtURL.Text = hidServiceURL.Value;
                }
                else if (!string.IsNullOrEmpty(txtURL.Text.Trim()))
                {
                    envList.EnvDetServiceURL = txtURL.Text.Trim();
                }

                envList.EnvDetIsMonitor = chkIsMonitor.Checked;

                envList.EnvDetIsServiceConsolidated = chkIsConsolidated.Checked;

                envList.EnvDetIsNotify = chkIsNotify.Checked;

                if (!string.IsNullOrEmpty(txtComments.Text.Trim()))
                    envList.EnvDetComments = txtComments.Text.Trim();

                envList.WindowsServiceName = !string.IsNullOrEmpty(txtWindowsServiceName.Text.Trim()) ? txtWindowsServiceName.Text.Trim() : string.Empty;

                if (string.IsNullOrEmpty(hidWindowsServiceID.Value))
                    envList.WindowsServiceID = 0;
                else
                    envList.WindowsServiceID = Convert.ToInt32(hidWindowsServiceID.Value);

                endEntity.EnvCreatedBy = envList.EnvDetCreatedBy = HttpContext.Current.Session["_LOGGED_USERD_ID"] != null ? HttpContext.Current.Session["_LOGGED_USERD_ID"].ToString() : string.Empty;
                endEntity.EnvCreatedDate = envList.EnvDetCreatedDate = DateTime.Now;
                endEntity.EnvIsActive = envList.EnvDetIsActive = "true";
                envList.EnvDetIsPrimay = true;

                endEntity.EnvDetailsList = new List<EnvDetailsEntity>();

                if (sMode == N)
                {
                    //status = enviService.InsUpdEnvironment(endEntity, N);
                }
                else if (sMode == U)
                {
                    endEntity.EnvID = CommonService.IsNumeric(hidEnvID.Value) ? Convert.ToInt32(hidEnvID.Value) : 0;
                    //status = enviService.InsUpdEnvironment(endEntity, U);
                }
                else if (sMode == UED)
                {
                    envList.EnvDetID = CommonService.IsNumeric(hidEnvID.Value) ? Convert.ToInt32(hidEnvID.Value) : 0;
                    //status = enviService.InsUpdEnvironment(endEntity, UED);
                }
                endEntity.EnvDetailsList.Add(envList);

                string serviceExists = enviService.IsServiceExists(endEntity);
                if (!string.IsNullOrEmpty(serviceExists))
                {
                    arrExists = serviceExists.Split(new char[] { '|' }, StringSplitOptions.None);
                    arrExistsText = arrExists[0];
                    arrExistsConfigID = arrExists[2];
                    arrExistsEnviID = arrExists[1];
                }

                if (!string.IsNullOrEmpty(serviceExists) && (sMode == N || sMode == NED))
                {
                    SetWindowHeight(); 
                    genericMessage.Visible = true;
                    genericMessage.CurMessageType = GenericMessage.MessageType.Failure;
                    genericMessage.ShowMessage(arrExistsText);
                    if (Session["emailList"] != null)
                        ScriptManager.RegisterStartupScript(this, this.GetType(), Guid.NewGuid().ToString(), "EnableSchedule(true);", true);
                    else
                        ScriptManager.RegisterStartupScript(this, this.GetType(), Guid.NewGuid().ToString(), "CanSchedule('" + endEntity.EnvID + "','" + ENVIRONMENT_TYPE + "');", true);
                    return;
                }
                else if (!string.IsNullOrEmpty(serviceExists) && (sMode == U || sMode == UED))
                {
                    if (arrExistsConfigID != endEntity.EnvDetailsList[0].EnvDetID.ToString())
                    {
                        SetWindowHeight();
                        genericMessage.Visible = true;
                        genericMessage.CurMessageType = GenericMessage.MessageType.Failure;
                        genericMessage.ShowMessage(arrExistsText);
                        if (Session["emailList"] != null)
                            ScriptManager.RegisterStartupScript(this, this.GetType(), Guid.NewGuid().ToString(), "EnableSchedule(true);", true);
                        else
                            ScriptManager.RegisterStartupScript(this, this.GetType(), Guid.NewGuid().ToString(), "CanSchedule('" + endEntity.EnvID + "','" + ENVIRONMENT_TYPE + "');", true);
                        return;
                    }
                }

                status = enviService.InsUpdEnvironment(endEntity, sMode);
                //code updated on 12312014
                if (status == 0 && envList.EnvDetServiceType == CM_TYPE)
                {
                    EnvironmentEntity envDispEntity = new EnvironmentEntity();
                    envDispEntity = endEntity;


                    int iDispCongifID = 0;

                    if (envDispEntity.EnvDetailsList[0].EnvDetID > 0)
                    {
                        envDispEntity.EnvDetailsList[0].EnvDetReferenceID = envDispEntity.EnvDetailsList[0].EnvDetID;
                        iDispCongifID = enviService.GetConfigReferenceID(envDispEntity, "REFID");
                        if (iDispCongifID > 0)
                            envDispEntity.EnvDetailsList[0].EnvDetID = iDispCongifID;
                        else
                        {
                            iDispCongifID = enviService.GetConfigReferenceID(envDispEntity, "CMURL");
                            envDispEntity.EnvDetailsList[0].EnvDetID = iDispCongifID;
                        }
                    }
                    else
                    {
                        envID = enviService.GetEnvironmentID(envDispEntity.EnvName);
                        envDispEntity.EnvID = envID;
                        iConfigID = enviService.GetEnvironmentDetailID(envDispEntity);
                        envDispEntity.EnvDetailsList[0].EnvDetReferenceID = iConfigID;
                    }
                    envDispEntity.EnvDetailsList[0].EnvDetServiceType = DISP_TYPE;
                    envDispEntity.EnvDetailsList[0].EnvDetServiceURL = envDispEntity.EnvDetailsList[0].EnvDetServiceURL + "/gc";
                    envDispEntity.EnvDetailsList[0].EnvDetIsPrimay = false;
                    status = enviService.InsUpdEnvironment(envDispEntity, sMode);
                }
                //end of code udpates on 12312014
                if (txtEnvironment.Text == string.Empty)
                    txtEnvironment.Text = hidEnvironment.Value;
                if (status == 1)
                {
                    SetWindowHeight(); 
                    genericMessage.Visible = true;
                    genericMessage.CurMessageType = GenericMessage.MessageType.Notification;
                    genericMessage.ShowMessage(ALREADY_EXISTS_MESSAGE);
                    //ClientScript.RegisterStartupScript(this.GetType(), "Exists", "alert('This Environment detail is already exists!');", true);
                }
                else if (status == 0)
                {
                    if (Session["SchedulerDetails"] != null)
                    {
                        SchedulerEntity scheduler = new SchedulerEntity();
                        scheduler = (SchedulerEntity)Session["SchedulerDetails"];
                        scheduler.EnvID = endEntity.EnvID;
                        //Replacing the CM URL incase if it is primary

                        if (!string.IsNullOrEmpty(drpServiceType.SelectedValue) && drpServiceType.SelectedValue == "1")
                        {
                            endEntity.EnvDetailsList[0].EnvDetServiceType = drpServiceType.SelectedValue;
                            if (!string.IsNullOrEmpty(txtURL.Text.Trim()))
                                endEntity.EnvDetailsList[0].EnvDetServiceURL = txtURL.Text.Trim();
                            
                        }
                        
                        scheduler.ConfigID = enviService.GetEnvironmentDetailID(endEntity);
                        /*
                         * if (endEntity.EnvDetailsList[0].EnvDetID <= 0)
                        {
                            scheduler.ConfigID = enviService.GetEnvironmentDetailID(endEntity);
                        }
                        else if (endEntity.EnvDetailsList[0].EnvDetID > 0)
                         * */
                        status = schedulerServices.InsertUpdateScheduler(scheduler);
                    }

                    //update email list that are added as part of new envionrment if any
                    if (Session["emailList"] != null)
                    {
                        List<ConfigEmailsEntity> emailconfigList = new List<ConfigEmailsEntity>();
                        emailconfigList = (List<ConfigEmailsEntity>)Session["emailList"];
                        if (emailconfigList != null && emailconfigList.Count > 0)
                        {
                            foreach (ConfigEmailsEntity list in emailconfigList)
                            {
                                if (endEntity.EnvID > 0)
                                    list.EnvID = endEntity.EnvID;
                                else
                                    list.EnvID = enviService.GetEnvironmentID(environmentName);
                                list.UserListID = 0;
                                list.IsAvtive = true;
                                status = enviService.InsUpdateEnvUserEmail(list, "");
                            }
                        }
                        ScriptManager.RegisterStartupScript(this, this.GetType(), Guid.NewGuid().ToString(), "clearSessionStore();EnableSchedule(true);", true);
                        Session["emailList"] = null;
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), Guid.NewGuid().ToString(), "clearSessionStore();CanSchedule('" + endEntity.EnvID + "','" + ENVIRONMENT_TYPE + "');", true);
                    }
                    isDataUpdate = true;
                    hidIsDataUpdated.Value = UPDATED;
                    SetWindowHeight();
                    genericMessage.Visible = true;
                    genericMessage.CurMessageType = GenericMessage.MessageType.Confirmation;
                    genericMessage.ShowMessage(SUCCESS_MESSAGE);
                    Session["SchedulerDetails"] = null;
                    ClearInputs();
                    
                    //sMode = UED;
                    //if (iConfigID > 0)
                    //    hidEnvID.Value = iConfigID.ToString();
                    
                    
                }
                else if (status == 2)
                {
                    SetWindowHeight();
                    ScriptManager.RegisterStartupScript(this, this.GetType(), Guid.NewGuid().ToString(), "clearSessionStore();CanSchedule('" + endEntity.EnvID + "','" + ENVIRONMENT_TYPE + "');", true);
                    genericMessage.Visible = true;
                    genericMessage.CurMessageType = GenericMessage.MessageType.Failure;
                    genericMessage.ShowMessage(FAILURE_MESSAGE);
                }
            }
            catch (Exception ex)    
            {
                Logger.Log(ex.ToString());
            }

        }

        private void PopulateTime()
        {
            try
            {
                int timeInMin;
                drpMailFrequency.DataSource = null;
                for (timeInMin = 5; timeInMin <= 60; timeInMin += 5)
                {
                    ListItem newItem = new ListItem();
                    if (timeInMin < 60)
                    {
                        newItem.Text = timeInMin + " min";
                    }
                    else
                    {
                        newItem.Text = timeInMin / 60 + " hour";
                    }
                    newItem.Value = timeInMin.ToString();
                    drpMailFrequency.Items.Add(newItem);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void PopulateEnvironment()
        {
            string strEnvList = string.Empty;
            string envList = "EnvList";
            Type envType = this.GetType();
            ClientScriptManager csm = Page.ClientScript;
            List<NamedBusinessEntity> enviList = new List<NamedBusinessEntity>();
            List<ConfigEmailsEntity> emailconfigList = new List<ConfigEmailsEntity>();

            if (!csm.IsStartupScriptRegistered(envType, envList))
            {
                StringBuilder sb = new StringBuilder();
                enviList = enviService.GetEnvironmentSelect(0);
                sb.Append("<script type=\"text/javascript\" language=\"javascript\">");
                sb.Append("var availableEnvironments = [");
                if (enviList != null && enviList.Count > 0)
                {
                    foreach (NamedBusinessEntity nb in enviList)
                    {
                        if (strEnvList.Length > 0)
                            strEnvList += ", ";
                        strEnvList += "'" + nb.Name + "'";

                    }
                    sb.Append(strEnvList);
                }
                sb.Append("];");

                emailconfigList = enviService.GetEmailConfiguration(0);
                Session["emailconfigList"] = emailconfigList;
                //to get email id related to the environment
                sb.Append("var EnvironmentEmails = [");
                if (emailconfigList != null && emailconfigList.Count > 0)
                {
                    strEnvList = string.Empty;
                    foreach (ConfigEmailsEntity emailEntity in emailconfigList)
                    {
                        if (strEnvList.Length > 0)
                            strEnvList += ", ";
                        strEnvList += "['" + emailEntity.EnvID + "', '" + emailEntity.EnvName + "', '" + emailEntity.EmailAddress + "']";

                    }
                    sb.Append(strEnvList);
                }
                sb.Append("];");
                sb.Append("</script>");
                csm.RegisterStartupScript(envType, envList, sb.ToString());
            }
        }

        private void PopulateEnvironmentDetails(string eId)
        {
            string tempEnvId = string.Empty;
            List<ConfigEmailsEntity> emailconfigList = new List<ConfigEmailsEntity>();
            EnvironmentEntity entity = new EnvironmentEntity();
            try
            {
                if (sMode == U) // modify environment
                {
                    if (CommonService.IsNumeric(eId))
                    {
                        entity = enviService.GetEnvironment(Convert.ToInt32(eId));
                        if (entity != null)
                        {
                            EnableControls(false);
                            hidEnvID.Value = CommonService.GetText(entity.EnvID).ToString(CultureInfo.InvariantCulture);
                            txtEnvironment.Text = CommonService.GetText(entity.EnvName).ToString(CultureInfo.InvariantCulture);
                            hidEnvironment.Value = CommonService.GetText(entity.EnvName).ToString(CultureInfo.InvariantCulture);
                            txtComments.Text = CommonService.GetText(entity.EnvComments);
                            chkIsMonitor.Checked = entity.EnvIsMonitor;
                            chkIsNotify.Checked = entity.EnvIsNotify;
                            chkIsConsolidated.Checked = entity.EnvIsServiceConsolidated;
                            drpMailFrequency.SelectedValue = entity.EnvMailFrequency;
                            hidEnvID.Value = tempEnvId = hidEnvIDEmail.Value = eId;

                        }
                    }
                }
                else if (sMode == UED) // modify environment
                {
                    if (CommonService.IsNumeric(eId))
                    {
                        EnvDetailsEntity detail = new EnvDetailsEntity();
                        detail = enviService.GetEnvironmentDetail(Convert.ToInt32(eId), 0);
                        if (detail != null)
                        {
                            EnableControls(true);
                            hidEnvID.Value = CommonService.GetText(detail.EnvDetID).ToString();
                            txtEnvironment.Text = CommonService.GetText(detail.EnvDet_Name).ToString();
                            hidEnvironment.Value = CommonService.GetText(detail.EnvDet_Name).ToString();
                            txtHostIP.Text = CommonService.GetText(detail.EnvDetHostIP);
                            txtPort.Text = CommonService.GetText(detail.EnvDetPort);
                            txtServerDescripption.Text = CommonService.GetText(detail.EnvDetDescription);
                            txtURL.Text = hidServiceURL.Value = CommonService.GetText(detail.EnvDetServiceURL);
                            txtLocation.Text = CommonService.GetText(detail.EnvDetLocation);
                            txtComments.Text = CommonService.GetText(detail.EnvDetComments);
                            chkIsMonitor.Checked = detail.EnvDetIsMonitor;
                            chkIsNotify.Checked = detail.EnvDetIsNotify;
                            chkIsConsolidated.Checked = detail.EnvDetIsServiceConsolidated;
                            drpMailFrequency.SelectedValue = detail.EnvDetMailFrequency;
                            drpServiceType.SelectedValue = CommonService.GetText(detail.EnvDetServiceType);
                            txtWindowsServiceName.Text = CommonService.GetText(detail.WindowsServiceName);
                            hidWindowsServiceID.Value = detail.WindowsServiceID > 0 ? CommonService.GetText(detail.WindowsServiceID.ToString()) : "0";
                            tempEnvId = hidEnvIDEmail.Value = detail.EnvID.ToString();
                            hidEnvID.Value = eId;
                        }
                    }
                }
                ScriptManager.RegisterStartupScript(this, this.GetType(), Guid.NewGuid().ToString(), "clearSessionStore();CanSchedule('" + tempEnvId + "','" + ENVIRONMENT_TYPE + "');", true);
                txtLocation.Focus();
            }
            catch (Exception ex)
            {
                Logger.Log(ex.ToString());
            }

        }

        private void EnableControls(bool mode)
        {
            txtHostIP.Enabled = mode;
            txtLocation.Enabled = mode;
            txtPort.Enabled = mode;
            txtServerDescripption.Enabled = mode;
            drpServiceType.Enabled = mode;
            txtURL.Enabled = mode;
            txtWindowsServiceName.Enabled = mode;

            lblLocation.Disabled = !mode;
            lblIPAddress.Disabled = !mode;
            lblPort.Disabled = !mode;
            lblServerDescription.Disabled = !mode;
            lblServiceType.Disabled = !mode;
            //lblURL.Disabled = !mode;

            if (mode)
            {
                //txtEnvironment.Enabled = !mode;
                //ScriptManager.RegisterStartupScript(this, this.GetType(), Guid.NewGuid().ToString(), "txtEnvironment", true);
                txtEnvironment.Attributes.Add("disabled", "disabled");
                //txtEnvironment.Attributes.Add("readonly", "true");
            }
        }

        private void ClearInputs()
        {
            //txtEnvironment.Text = "";
            txtComments.Text = "";
            txtHostIP.Text = "";
            txtLocation.Text = "";
            txtPort.Text = "";
            txtServerDescripption.Text = "";
            txtURL.Text = "";
            txtWindowsServiceName.Text = "";
            if (sMode != U) hidEnvID.Value = "";
            //hidEnvironment.Value = "";
            hidServiceType.Value = "";
            hidWindowsServiceID.Value = "";
            hidServiceURL.Value = "";
        }

        private void SetWindowHeight()
        {
            ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), "setWindowHeightAlert(true);", true);
        }
    }
}