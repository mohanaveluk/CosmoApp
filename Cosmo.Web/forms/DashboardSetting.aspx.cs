using System;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using Cosmo.Entity;
using Cosmo.Service;
using Cosmo.Web.controls;

namespace Cosmo.Web.forms
{
    public partial class DashboardSetting : System.Web.UI.Page
    {
        #region constants

        private const string UPDATED = "updated";
        private const string SUCCESS_MESSAGE = "Settings updated successfully";
        private const string FAILURE_MESSAGE = "Unable to update service acknowledge";

        #endregion constants

        protected void Page_Load(object sender, EventArgs e)
        {
            if (HttpContext.Current.Session["_PERSONALIZE"] != null)
            {
                var personalise = (PersonalizeEntity)HttpContext.Current.Session["_PERSONALIZE"];
                lblCurrentRefreshTime.InnerText = personalise.RefreshTime + " min";
                if (string.IsNullOrEmpty(txtNewRefreshTime.Text))
                    txtNewRefreshTime.Text = personalise.RefreshTime.ToString();
                lblLastUpdatedTime.InnerText = personalise.UpdatedDate.ToString("MM/dd/yyyy HH:mm:ss tt");
            }

            if (!IsPostBack)
            {
                LoadAllEnvironments();
            }
        }

        protected void btnCreate_Click(object sender, EventArgs e)
        {
            string listOrder = string.Empty;
            var monitorService = new MonitorService();
            var personalize = new PersonalizeEntity();
            if (!string.IsNullOrEmpty(txtNewRefreshTime.Text))
            {
                personalize.UserId = Convert.ToInt32(HttpContext.Current.Session["_LOGGED_USERD_ID"]);
                personalize.CreatedDate = DateTime.Now;
                personalize.CreatedBy = personalize.UserId.ToString(CultureInfo.InvariantCulture);
                personalize.RefreshTime = Convert.ToInt32(txtNewRefreshTime.Text);
                personalize.IsActive = true;

                if (lstEnvironment.Items.Count > 0)
                {
                    foreach (ListItem item in lstEnvironment.Items)
                    {
                        listOrder += item.Value + ",";
                    }
                }
                personalize.SortOrder = listOrder;
                var result = monitorService.InsUpdPersonalize(personalize);
                if (result == 0)
                {
                    ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), "SetSettingWindowHeight(true);", true);
                    hidIsDataUpdated.Value = UPDATED;
                    genericMessage.Visible = true;
                    genericMessage.CurMessageType = GenericMessage.MessageType.Confirmation;
                    genericMessage.ShowMessage(SUCCESS_MESSAGE);
                }
                else if (result != 0)
                {
                    ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), "SetSettingWindowHeight(true);", true);
                    genericMessage.Visible = true;
                    genericMessage.CurMessageType = GenericMessage.MessageType.Notification;
                    genericMessage.ShowMessage(FAILURE_MESSAGE);
                }
            }

        }

        private void LoadAllEnvironments()
        {
            var environmentService = new EnvironmentService();
            var enviList = environmentService.GetEnvironments(0);
            if (enviList != null && enviList.Count > 0)
            {
                lstEnvironment.Items.Clear();
                lstEnvironment.DataTextField = "EnvName"; lstEnvironment.DataValueField = "EnvID";
                lstEnvironment.DataSource = enviList;
                lstEnvironment.DataBind();
            }
        }

        protected void btnUp_Click(object sender, EventArgs e)
        {
            MoveItem(-1);
        }

        protected void btnDown_Click(object sender, EventArgs e)
        {
            MoveItem(1);

        }

        private void MoveItem(int direction)
        {
            // Checking selected item
            if (lstEnvironment.SelectedItem == null || lstEnvironment.SelectedIndex < 0)
                return; // No selected item - nothing to do

            // Calculate new index using move direction
            int newIndex = lstEnvironment.SelectedIndex + direction;

            // Checking bounds of the range
            if (newIndex < 0 || newIndex >= lstEnvironment.Items.Count)
                return; // Index out of range - nothing to do

            ListItem selected = lstEnvironment.SelectedItem;

            // Removing removable element
            lstEnvironment.Items.Remove(selected);
            // Insert it in new position
            lstEnvironment.Items.Insert(newIndex, selected);
            // Restore selection
            
        }

    }
}