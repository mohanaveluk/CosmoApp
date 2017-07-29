using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Cosmo.Entity;
using Cosmo.Service;
using Cosmo.Web.controls;

namespace Cosmo.Web.forms
{
    public partial class UrlConfigurationDetails : System.Web.UI.Page
    {
        //private string LOGGEDIN_USER = Convert.ToString(ConfigurationManager.AppSettings["LoggedInUser"]);

        EnvironmentService enviService = new EnvironmentService();
        CommonService commonService = new CommonService();
        SchedulerServices schedulerServices = new SchedulerServices();

        #region Constants
        private const string SUCCESS_MESSAGE = "URL configuration has been successfully saved.";
        private const string FAILURE_MESSAGE = "URL configuration has not been saved.";
        private const string ALREADY_EXISTS_MESSAGE = "URL configuration has already exists. Please try new URL configuration";
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
                if (Request.QueryString["s"] != null)
                {
                    sEnvID = Request.QueryString["s"];
                    if (Request.QueryString["t"] != null)
                    {
                        sType = hidType.Value = Request.QueryString["t"];
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
                var endEntity = new EnvironmentUrlConfiguration();

                var envList = new UrlConfigurationEntity();

                if (!string.IsNullOrEmpty(txtEnvironment.Text))
                    environmentName = txtEnvironment.Text.Trim();
                else if (!string.IsNullOrEmpty(hidEnvironment.Value))
                    environmentName = hidEnvironment.Value.Trim();

                endEntity.EnvName = environmentName;
                envList.EnvName = environmentName;
                if (!string.IsNullOrEmpty(environmentName))
                {
                    envID = enviService.GetEnvironmentID(environmentName);
                    if (envID > 0)
                        endEntity.EnvID = envID;
                    else
                        endEntity.EnvID = -1;
                }

                envList.EnvId = endEntity.EnvID;
                
                if (!string.IsNullOrEmpty(txtAddress.Text.Trim()))
                    envList.Adress = txtAddress.Text.Trim();
                else
                    envList.Adress = string.Empty;

                if (!string.IsNullOrEmpty(txtDisplayName.Text.Trim()))
                    envList.DisplayName = txtDisplayName.Text.Trim();
                else
                    envList.DisplayName = string.Empty;

                envList.MatchContent = !string.IsNullOrEmpty(txtMatchContent.Text.Trim()) ? txtMatchContent.Text.Trim() : string.Empty;

                envList.Interval = !string.IsNullOrEmpty(txtPollingInterval.Text.Trim()) ? Convert.ToInt32(txtPollingInterval.Text.Trim()) : 0;

                envList.Type = !string.IsNullOrEmpty(drpUrlType.SelectedValue) ? drpUrlType.SelectedValue : string.Empty;

                envList.UserName = !string.IsNullOrEmpty(txtUserName.Text.Trim()) ? txtUserName.Text.Trim() : string.Empty;
                envList.Password = !string.IsNullOrEmpty(txtPassword.Text.Trim()) ? CommonUtility.Encrypt(txtPassword.Text.Trim()) : string.Empty;


                if (chkActive.Checked)
                    envList.Status = true;
                else
                    envList.Status = false;


                envList.CreatedBy = HttpContext.Current.Session["_LOGGED_USERD_ID"] != null ? HttpContext.Current.Session["_LOGGED_USERD_ID"].ToString() : string.Empty;

                endEntity.UrlConfiguration = new List<UrlConfigurationEntity>();

                if (sMode == N)
                {
                    //status = enviService.InsUpdateUrlConfiguration(envList, N);
                }
                else if (sMode == U)
                {
                    endEntity.EnvID = CommonService.IsNumeric(hidEnvID.Value) ? Convert.ToInt32(hidEnvID.Value) : 0;
                    //status = enviService.InsUpdEnvironment(endEntity, U);
                }
                else if (sMode == UED)
                {
                    envList.Id = CommonService.IsNumeric(hidUrlID.Value) ? Convert.ToInt32(hidUrlID.Value) : 0;
                    //status = enviService.InsUpdateUrlConfiguration(envList, UED);
                }

                string serviceExists = enviService.IsUrlConfigExists(envList);
                if (!string.IsNullOrEmpty(serviceExists))
                {
                    arrExists = serviceExists.Split(new char[] { '|' }, StringSplitOptions.None);
                    arrExistsText = arrExists[0];
                    arrExistsConfigID = arrExists[2];
                    //arrExistsEnviID = arrExists[1];
                }

                if (!string.IsNullOrEmpty(serviceExists) && (sMode == N || sMode == NED))
                {
                    SetWindowHeight(); 
                    genericMessage.Visible = true;
                    genericMessage.CurMessageType = GenericMessage.MessageType.Failure;
                    genericMessage.ShowMessage(arrExistsText);
                    return;
                }
                else if (!string.IsNullOrEmpty(serviceExists) && (sMode == U || sMode == UED))
                {
                    if (arrExistsConfigID != envList.Id.ToString())
                    {
                        SetWindowHeight();
                        genericMessage.Visible = true;
                        genericMessage.CurMessageType = GenericMessage.MessageType.Failure;
                        genericMessage.ShowMessage(arrExistsText);
                        return;
                    }
                }

                status = enviService.InsUpdateUrlConfiguration(envList, sMode);
                
                
                //end of code udpates on 12312014
                if (txtEnvironment.Text == string.Empty)
                    txtEnvironment.Text = hidEnvironment.Value;
                if (status == -1)
                {
                    SetWindowHeight(); 
                    genericMessage.Visible = true;
                    genericMessage.CurMessageType = GenericMessage.MessageType.Notification;
                    genericMessage.ShowMessage(ALREADY_EXISTS_MESSAGE);
                    //ClientScript.RegisterStartupScript(this.GetType(), "Exists", "alert('This Environment detail is already exists!');", true);
                }
                else if (status >= 0)
                {
                    

                    
                    isDataUpdate = true;
                    hidIsDataUpdated.Value = UPDATED;
                    SetWindowHeight();
                    genericMessage.Visible = true;
                    genericMessage.CurMessageType = GenericMessage.MessageType.Confirmation;
                    genericMessage.ShowMessage(SUCCESS_MESSAGE);
                    Session["SchedulerDetails"] = null;
                    //ClearInputs();
                    
                }
                else if (status == 0)
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
            var entity = new EnvironmentUrlConfiguration();
            try
            {
                if (sMode == UED) // modify environment
                {
                    if (CommonService.IsNumeric(eId))
                    {
                        var detailList = enviService.GetUrlConfiguration(0,Convert.ToInt32(eId));

                        var detail = detailList.Any(_ => _.Id == Convert.ToInt32(eId))
                            ? detailList.Where(_ => _.Id == Convert.ToInt32(eId)).ToList()[0]
                            : new UrlConfigurationEntity();
                        
                        if (detail != null)
                        {
                            EnableControls(true);
                            hidUrlID.Value = CommonService.GetText(detail.Id).ToString();
                            txtEnvironment.Text = CommonService.GetText(detail.EnvName).ToString();
                            hidEnvironment.Value = CommonService.GetText(detail.EnvName).ToString();
                            txtAddress.Text = CommonService.GetText(detail.Adress);
                            drpUrlType.SelectedValue = CommonService.GetText(detail.Type);
                            txtDisplayName.Text = CommonService.GetText(detail.DisplayName);
                            txtMatchContent.Text = CommonService.GetText(detail.MatchContent);
                            txtPollingInterval.Text = CommonService.GetText(detail.Interval).ToString();
                            txtUserName.Text = CommonService.GetText(detail.UserName);
                            var password = CommonUtility.Decrypt(CommonService.GetText(detail.Password));
                            txtPassword.Text = password;
                            txtPassword.Attributes.Add("value", password);
                            chkActive.Checked = detail.Status;
                            tempEnvId = detail.EnvId.ToString();
                            hidEnvID.Value = eId;
                        }
                    }
                }
                ScriptManager.RegisterStartupScript(this, this.GetType(), Guid.NewGuid().ToString(), "clearSessionStore();CanSchedule('" + tempEnvId + "','" + ENVIRONMENT_TYPE + "');", true);
                txtAddress.Focus();
            }
            catch (Exception ex)
            {
                Logger.Log(ex.ToString());
            }

        }

        private void EnableControls(bool mode)
        {

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
            txtAddress.Text = "";
            txtDisplayName.Text = "";
            txtMatchContent.Text = "";
            txtPollingInterval.Text = "";
            txtUserName.Text = "";
            txtPassword.Text = "";
            txtEnvironment.Text = "";
            if (sMode != U) hidEnvID.Value = "";
            //hidEnvironment.Value = "";
            chkActive.Checked = true;
        }

        private void SetWindowHeight()
        {
            ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), "setWindowHeightAlert(true);", true);
        }
    }
}