using System;
using Cosmo.Entity;
using Cosmo.Service;
using Cosmo.Web.controls;

namespace Cosmo.Web.forms
{
    public partial class NotifyAlert : System.Web.UI.Page
    {
        #region serviceObjects
        CommonService commonService = new CommonService();
        EnvironmentService enviService = new EnvironmentService();
        MonitorService monitorService = new MonitorService();
        #endregion serviceObjects

        #region constants
        private const string UPDATED = "updated";
        private const string FAILURE_MESSAGE = "Unable to update service acknowledge";
        private const string FAILURE_MAIL_MESSAGE = "Unable to send mail";

        private const string SERVICE_ACKNOWLEDGED = "Service acknowledged";
        private const string SERVICE_ACKNOWLEDGED_STOPPED = "Service acknowledged and stopped an alert";
        private const string SERVICE_ACKNOWLEDGED_STARTED = "Service acknowledged and started an alert";
        #endregion constants

        #region variables
        int iSrNo = 1;
        string SUCCESS_MESSAGE;

        #endregion constants

        #region viewstate
        /// <summary>
        /// Monitor ID
        /// </summary>
        private string sMonID
        {
            get { return Convert.ToString(ViewState["vstMonID"]); }
            set { ViewState["vstMonID"] = value; }
        }

        /// <summary>
        /// Source ID
        /// </summary>
        private string sEnvID
        {
            get { return Convert.ToString(ViewState["vstEnvID"]); }
            set { ViewState["vstEnvID"] = value; }
        }

        /// <summary>
        /// Source ID
        /// </summary>
        private string sConfigID
        {
            get { return Convert.ToString(ViewState["vstConfigID"]); }
            set { ViewState["vstConfigID"] = value; }
        }
        /// <summary>
        /// Transation whether it is add / update
        /// </summary>
        private string sTransaction
        {
            get { return Convert.ToString(ViewState["vstTrans"]); }
            set { ViewState["vstTrans"] = value; }
        }

        /// <summary>
        /// Service Name
        /// </summary>
        private string sServiceName
        {
            get { return Convert.ToString(ViewState["vstService"]); }
            set { ViewState["vstService"] = value; }
        }

        /// <summary>
        /// Change mode to stop / start
        /// </summary>
        private string sChangeMode
        {
            get { return Convert.ToString(ViewState["vstChangeMode"]); }
            set { ViewState["vstChangeMode"] = value; }
        }

        /// <summary>
        /// Change mode to stop / start
        /// </summary>
        private string sTranx
        {
            get { return Convert.ToString(ViewState["vstTranx"]); }
            set { ViewState["vstTranx"] = value; }
        }

        #endregion viewstate

        protected void Page_Load(object sender, EventArgs e)
        {
            CommonService.ValidateUser();
            if (!IsPostBack)
            {
                if (Request.QueryString["s"] != null)
                {
                    sConfigID = Request.QueryString["s"]; // source
                    if (Request.QueryString["e"] != null)
                    {
                        sEnvID = Request.QueryString["e"];
                    }
                    if (Request.QueryString["mid"] != null)
                    {
                        sMonID = Request.QueryString["mid"];
                    }
                    if (Request.QueryString["n"] != null)
                    {
                        sServiceName = Request.QueryString["n"];
                        lblEnvironmentName.InnerText = sServiceName;
                    }
                    if (Request.QueryString["m"] != null)
                    {
                        sChangeMode = hidMode.Value = Request.QueryString["m"];
                        if (sChangeMode == "sp")
                            opt_Acknowledge_alert.Text = "Acknowledge & Stop Alert";
                        else
                            opt_Acknowledge_alert.Text = "Acknowledge & Start Alert";
                    }
                    if (Request.QueryString["t"] != null)
                        sTranx = Request.QueryString["t"];

                    //PopulateEmailList(Convert.ToInt32(sEnvID));
                }
            }
        }

        protected void btnCreate_Click(object sender, EventArgs e)
        {
            genericMessage.Visible = false;
            int recUpdate = 0;
            try
            {
                //ClientScript.RegisterStartupScript(this.GetType(), "Exists", "ShowProcess(true)", true);
                AcknowledgeEntity ackDetails = new AcknowledgeEntity();
                if (!string.IsNullOrEmpty(sEnvID) && CommonService.IsNumeric(sEnvID))
                {
                    ackDetails.EnvId = Convert.ToInt32(sEnvID);
                    if (!string.IsNullOrEmpty(sMonID) && CommonService.IsNumeric(sMonID))
                        ackDetails.MonId = Convert.ToInt32(sMonID); ;
                    if (!string.IsNullOrEmpty(sConfigID) && CommonService.IsNumeric(sConfigID))
                        ackDetails.ConfigId = Convert.ToInt32(sConfigID);
                    ackDetails.ServiceName = sServiceName;

                    if (!string.IsNullOrEmpty(txtReason.Text.Trim()))
                    {
                        ackDetails.AcknowledgeComments = txtReason.Text;
                    }

                    if (opt_Acknowledge.Checked)
                    {
                        ackDetails.IsAcknowledgeMode = true;
                        ackDetails.AcknowledgeAlertChange = "ack";
                        SUCCESS_MESSAGE = SERVICE_ACKNOWLEDGED;
                    }
                    if (opt_Acknowledge_alert.Checked)
                    {
                        ackDetails.IsAcknowledgeMode = true;
                        if (sChangeMode == "sp")
                        {
                            ackDetails.AcknowledgeAlertChange = "stop";
                            SUCCESS_MESSAGE = SERVICE_ACKNOWLEDGED_STOPPED;
                        }
                        else
                        {
                            ackDetails.AcknowledgeAlertChange = "start";
                            SUCCESS_MESSAGE = SERVICE_ACKNOWLEDGED_STARTED;
                        }
                    }
                    ackDetails.CreatedBy = Session["_LOGGED_USERD_ID"] != null ? Session["_LOGGED_USERD_ID"].ToString() : "1";

                    ackDetails.CreatedByName = Session["_LOGGED_USERNAME"] != null ? Session["_LOGGED_USERNAME"].ToString() : "Admin";

                    ackDetails.CreatedDate = DateTime.Now;
                    recUpdate = monitorService.InsUpdateServiceAcknowledge(ackDetails, sTranx);
                    //ClientScript.RegisterStartupScript(this.GetType(), "Exists", "ShowProcess(false)", true);
                    //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "Exists", "ShowProcess(false)", true);

                    if (recUpdate == 0)
                    {
                        ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), "SetNotifyWindowHeight(true);", true);
                        hidIsDataUpdated.Value = UPDATED;
                        genericMessage.Visible = true;
                        genericMessage.CurMessageType = GenericMessage.MessageType.Confirmation;
                        genericMessage.ShowMessage(SUCCESS_MESSAGE);
                    }
                    else if (recUpdate == -9)
                    {
                        Response.Redirect("Timeout.html");
                    }
                    else if (recUpdate == -3)
                    {
                        ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), "SetNotifyWindowHeight(true);", true);
                        genericMessage.Visible = true;
                        genericMessage.CurMessageType = GenericMessage.MessageType.Notification;
                        genericMessage.ShowMessage(FAILURE_MAIL_MESSAGE);
                    }
                    else
                    {
                        ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), "SetNotifyWindowHeight(true);", true);
                        genericMessage.Visible = true;
                        genericMessage.CurMessageType = GenericMessage.MessageType.Notification;
                        genericMessage.ShowMessage(FAILURE_MESSAGE);

                    }
                }
                else
                {
                    Logger.Log("Config id is missing...");
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex.ToString());
                Logger.Log(ex.ToString());
                if (ex.InnerException != null && !string.IsNullOrEmpty(ex.InnerException.Message))
                    Logger.Log(ex.InnerException.Message);
                ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), "SetNotifyWindowHeight(true);", true);
                genericMessage.Visible = true;
                genericMessage.CurMessageType = GenericMessage.MessageType.Failure;
                genericMessage.ShowMessage(FAILURE_MESSAGE);
            }

        }

    }
}