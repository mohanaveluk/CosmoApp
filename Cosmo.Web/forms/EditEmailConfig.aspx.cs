using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using Cosmo.Entity;
using Cosmo.Service;

namespace Cosmo.Web.forms
{
    public partial class EditEmailConfig : System.Web.UI.Page
    {
        #region serviceObjects
        CommonService commonService = new CommonService();
        EnvironmentService enviService = new EnvironmentService();
        #endregion serviceObjects

        #region constants
        private const string UPDATED = "updated";
        #endregion constants

        #region variables
        int iSrNo = 1;
        bool isUserEMailAddToCollection = true;
        //string sEnvID = string.Empty;
        List<ConfigEmailsEntity> emailconfigList = new List<ConfigEmailsEntity>();
        #endregion constants

        #region viewstate
        private string sEnvID
        {
            get { return Convert.ToString(ViewState["vstEnvID"]); }
            set { ViewState["vstEnvID"] = value; }
        }
        private int vstEnvLessUserEmailID
        {
            get { return Convert.ToInt32(ViewState["vstEnvLessUserEmailID"]); }
            set { ViewState["vstEnvLessUserEmailID"] = value; }
        }
        private string sCreateMode
        {
            get { return Convert.ToString(ViewState["vstsCreateMode"]); }
            set { ViewState["vstsCreateMode"] = value; }
        }

        #endregion viewstate

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)//|| Request.Form["__EVENTTARGET"].Equals("") || Request.Form["__EVENTTARGET"].Contains("lnkEnvDel"))
            {
                InitiateEmailList();
                txtEmail.Focus();
                if (!string.IsNullOrEmpty(Request.QueryString["s"]))
                {
                    sEnvID = Request.QueryString["s"];
                    PopulateEmailList(Convert.ToInt32(sEnvID));
                }
                else
                {
                    //if (emailconfigList==null) emailconfigList = new List<ConfigEmailsEntity>();
                }
                if (!string.IsNullOrEmpty(Request.QueryString["ed"]))
                {
                    sCreateMode = Request.QueryString["ed"];

                    if (!string.IsNullOrEmpty(Request.QueryString["ename"]) && string.IsNullOrEmpty(sEnvID))
                    {
                        hidCreateMode.Value = sCreateMode;
                        lblEnvironmentName.InnerText = Request.QueryString["ename"].ToString();
                    }
                    else
                        hidCreateMode.Value = "edit";

                    populateEMailFromSession();

                }
            }
            else if (Request.Form["__EVENTTARGET"].Equals("") || Request.Form["__EVENTTARGET"].Contains("lnkEnvDel"))
            {
                populateEMailFromSession();
            }
        }

        private void populateEMailFromSession()
        {
            iSrNo = 1;
            if (Session["emailList"] != null && !string.IsNullOrEmpty(Request.QueryString["ed"]) && Request.QueryString["ed"] == "new")
            {
                emailconfigList = new List<ConfigEmailsEntity>();
                emailconfigList = (List<ConfigEmailsEntity>)Session["emailList"];
                if (emailconfigList != null && emailconfigList.Count > 0)
                {
                    rptEmaiList.DataSource = emailconfigList;
                    ClientScript.RegisterStartupScript(this.GetType(), "DataSavedToDB6", "EmailIdExistsStatus('updated');", true);
                }
                else
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "DataSavedToDB5", "EmailIdExistsStatus('noemail');", true);
                    rptEmaiList.DataSource = null;
                }
                rptEmaiList.DataBind();
            }

        }

        private void PopulateEmailList(int envID)
        {
            iSrNo = 1;
            emailconfigList = new List<ConfigEmailsEntity>();
            emailconfigList = enviService.GetEmailConfiguration(envID);
            if (emailconfigList != null && emailconfigList.Count > 0)
            {
                lblEnvironmentName.InnerText = emailconfigList[0].EnvName;
                rptEmaiList.DataSource = new List<ConfigEmailsEntity>();
                rptEmaiList.DataBind();
                rptEmaiList.Visible = true;
                rptEmaiList.DataSource = emailconfigList;
                rptEmaiList.DataBind();
                ClientScript.RegisterStartupScript(this.GetType(), "DataSavedToDB2", "EmailIdExistsStatus('updated');",
                    true);
            }
            else
            {
                rptEmaiList.DataSource = new List<ConfigEmailsEntity>();
                rptEmaiList.DataBind();
                rptEmaiList.Visible = true;
                rptEmaiList.DataBind();
            }
            if (!string.IsNullOrEmpty(Request.QueryString["ed"]) && Request.QueryString["ed"] == "new")
            {
                populateEMailFromSession();
            }
            else
            {
                if (emailconfigList == null)
                {
                    rptEmaiList.DataSource = null;
                    rptEmaiList.DataBind();
                }
                ClientScript.RegisterStartupScript(this.GetType(), "DataSavedToDB3", "EmailIdExistsStatus('noemail');", true);
            }
        }
        protected void btnCreate_Click(object sender, EventArgs e)
        {
            try
            {
            }
            catch (Exception ex)
            {
                Logger.Log(ex.ToString());
            }

        }

        protected void btnAddEmail_Click(object sender, EventArgs e)
        {
            iSrNo = 1;
            if (IsPostBack)
            {
                int recUpdate = 0;
                try
                {
                    //if (!string.IsNullOrEmpty(txtEmail.Text))
                    //{
                    //    if (!IsEmailIDExists(txtEmail.Text.Trim()))
                    //    {

                    //    }
                    //}
                    
                    ConfigEmailsEntity emailEntity = new ConfigEmailsEntity();
                    emailEntity.MessageType = rdoEmail.Checked ? "E" : "T";

                    if (!string.IsNullOrEmpty(sEnvID))
                        emailEntity.EnvID = Convert.ToInt32(sEnvID);

                    if (!string.IsNullOrEmpty(hidUserEmailID.Value.Trim()))
                    {
                        emailEntity.UserListID = Convert.ToInt32(hidUserEmailID.Value);
                    }
                    else
                        emailEntity.UserListID = 0;

                    if (!string.IsNullOrEmpty(txtEmail.Text))
                        emailEntity.EmailAddress = txtEmail.Text.Trim().ToLower();
                    else if (!string.IsNullOrEmpty(Request.Form[txtEmail.ClientID]))
                    {
                        emailEntity.EmailAddress = Request.Form[txtEmail.ClientID].ToLower();
                    }

                    if (!string.IsNullOrEmpty(drpEmailType.SelectedValue))
                        emailEntity.UserListType = CommonUtility.ToTitleCase(drpEmailType.SelectedValue.Trim());
                    else if (!string.IsNullOrEmpty(Request.Form[drpEmailType.ClientID]))
                    {
                        emailEntity.UserListType = CommonUtility.ToTitleCase(Request.Form[drpEmailType.ClientID]);
                    }
                    emailEntity.IsAvtive = true;
                    emailEntity.Created_By = HttpContext.Current.Session["_LOGGED_USERD_ID"] != null ? HttpContext.Current.Session["_LOGGED_USERD_ID"].ToString() : string.Empty;
                    emailEntity.Created_Date = DateTime.Now;
                    emailEntity.Comments = string.Empty;
                    if (!string.IsNullOrEmpty(sEnvID))
                    {
                        recUpdate = enviService.InsUpdateEnvUserEmail(emailEntity, "");

                        if (recUpdate == 0)
                        {
                            hidIsDataUpdated.Value = UPDATED;
                            PopulateEmailList(Convert.ToInt32(sEnvID));
                            txtEmail.Text = string.Empty;
                            Session["emailList"] = null;
                            hidUserEmailID.Value = string.Empty;
                            ClientScript.RegisterStartupScript(this.GetType(), "DataSavedToDB1", "EmailUpdated('updated');EnableEmailType('" + (rdoEmail.Checked ? "email" : "text") + "');", true);
                        }
                    }
                    else
                    {
                        if (Session["emailList"] != null)
                        {
                            emailconfigList = new List<ConfigEmailsEntity>();
                            emailconfigList = (List<ConfigEmailsEntity>)Session["emailList"];
                        }
                        else
                        {
                            //emailconfigList = new List<ConfigEmailsEntity>();
                        }

                        if (emailconfigList != null && emailconfigList.Count > 0)
                        {
                            if (emailconfigList.Any(el => el.UserListID == emailEntity.UserListID))
                            {
                                //emailconfigList.RemoveAll(el => el.UserListID == emailEntity.UserListID);
                                ConfigEmailsEntity singleItem = emailconfigList.Where(el => el.UserListID == emailEntity.UserListID).ToList()[0];
                                singleItem.EmailAddress = emailEntity.EmailAddress;
                                singleItem.UserListType = emailEntity.UserListType;
                                isUserEMailAddToCollection = false;
                            }
                            else
                                isUserEMailAddToCollection = true;
                        }

                        if (isUserEMailAddToCollection)
                        {
                            emailEntity.UserListID = ++vstEnvLessUserEmailID;
                            emailconfigList.Add(emailEntity);
                        }
                        rptEmaiList.DataSource = new List<ConfigEmailsEntity>();
                        rptEmaiList.DataBind();
                        rptEmaiList.Visible = true;
                        rptEmaiList.DataSource = emailconfigList;
                        rptEmaiList.DataBind();
                        txtEmail.Text = string.Empty;
                        hidUserEmailID.Value = string.Empty;
                        Session["emailList"] = emailconfigList;
                        ClientScript.RegisterStartupScript(this.GetType(), "DataNotSavedToDB", "EmailUpdated('dataPassed');", true);
                        ClientScript.RegisterStartupScript(this.GetType(), "DataSavedToDB4", "EmailIdExistsStatus('updated');EnableEmailType('" + (rdoEmail.Checked ? "email":"text") + "');", true);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log(ex.ToString());
                }
            }
        }
        /// <summary>
        /// Get and show all the environment list by ItemBound
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        protected void rptEmaiList_ItemCommand(object source, RepeaterCommandEventArgs e)
        {

        }

        /// <summary>
        /// Get and show all the environment list on ItemDataBound
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void rptEmaiList_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Header)
            {
            }
            else if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Label d_lblSrNo = (Label)e.Item.FindControl("d_lblSrNo");
                Label lblUserEmailID = (Label)e.Item.FindControl("lblUserEmailID");
                Label d_lblEmailAddress = (Label)e.Item.FindControl("d_lblEmailAddress");
                Label d_lblEmailType = (Label)e.Item.FindControl("d_lblEmailType");
                Label d_lblMessageType = (Label)e.Item.FindControl("d_lblMessageType");
                HiddenField hidEmail = (HiddenField)e.Item.FindControl("hidEmail");

                HyperLink lnkEnvEdit = (HyperLink)e.Item.FindControl("lnkEnvEdit");
                LinkButton lnkEnvDel = (LinkButton)e.Item.FindControl("lnkEnvDel");

                lnkEnvEdit.NavigateUrl = "javascript:AlterUserEmail('" + hidEmail.Value + "','" + d_lblEmailAddress.Text + "','" + CommonUtility.ToTitleCase(d_lblEmailType.Text) + "','" + d_lblMessageType.Text + "')"; //Edit email address for specific environment
                lnkEnvDel.Click += new EventHandler(lnkEnvDel_Click);
                lnkEnvDel.Attributes.Add("onclick", "DeleteEmailConfig('" + hidEmail.Value + "','" + d_lblEmailAddress.Text + "');return false;");
                lnkEnvDel.CommandArgument = hidEmail.Value;
                lnkEnvDel.CommandName = "EnvID";
                d_lblSrNo.Text = iSrNo++.ToString();
                d_lblMessageType.Text = d_lblMessageType.Text == "E" ? "Email" : "Text";

            }
        }

        protected void lnkEnvDel_Click(object sender, EventArgs e)
        {
        }

        private bool IsEmailIDExists(string newEmailId)
        {
            bool EmailExists = false;
            if (rptEmaiList.Items != null && rptEmaiList.Items.Count > 0)
            {
                foreach (RepeaterItem item in rptEmaiList.Items)
                {
                    Label lblEmail = (Label)item.FindControl("d_lblEmailAddress");
                    if (lblEmail.Text == newEmailId)
                    {
                        ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), "alert('Oops. EMail address :'" + newEmailId + "' is already exists. Try chhose another email address.');return false;", true);
                        EmailExists = true;
                        break;
                    }
                }
            }
            return EmailExists;
        }

        private void InitiateEmailList()
        {
            rptEmaiList.DataSource = new List<ConfigEmailsEntity>();
            rptEmaiList.DataBind();
            rptEmaiList.Visible = true;
            rptEmaiList.DataBind();
        }
    }
}