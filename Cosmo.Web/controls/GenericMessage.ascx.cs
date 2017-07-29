using System;
using System.Data;

namespace Cosmo.Web.controls
{
    public partial class GenericMessage : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        #region Declarations

        #region Enum

        /// <summary>
        /// Message Type Enumeration
        /// </summary>
        public enum MessageType
        {
            Alert,
            Confirmation,
            Failure,
            Notification
        }

        /// <summary>
        /// Return Status Enumeration
        /// </summary>
        public enum ReturnStatus
        {
            Success,
            Failure
        }

        #endregion Enum

        #region Variables

        private MessageType enMessageType = MessageType.Alert;

        #endregion Variables

        #region Properties

        /// <summary>
        /// Gets or Sets the Current Message Type
        /// </summary>
        public MessageType CurMessageType
        {
            get { return enMessageType; }
            set { enMessageType = value; }
        }

        #endregion Properties

        #endregion Declarations

        #region Other Methods

        /// <summary>
        /// This function shows the Error or success status message 
        /// </summary>
        /// <param name="message"> Message to be displayed</param>
        public void ShowMessage(string message)
        {

            switch (enMessageType)
            {
                case MessageType.Failure:
                    imgMessage.ImageUrl = Page.ResolveUrl("~/images/failure.gif");
                    lblStatus.Style.Add("color", "#a94442");
                    break;
                case MessageType.Alert:
                    imgMessage.ImageUrl = Page.ResolveUrl("~/images/alert.gif");
                    lblStatus.Style.Add("color", "#D6992F");
                    break;
                case MessageType.Confirmation:
                    imgMessage.ImageUrl = Page.ResolveUrl("~/images/success.gif");
                    lblStatus.Style.Add("color", "#85C23D");
                    break;
                case MessageType.Notification:
                    imgMessage.ImageUrl = Page.ResolveUrl("~/images/notify.gif");
                    lblStatus.Style.Add("color", "#34A9E3");
                    break;
                default:
                    imgMessage.ImageUrl = Page.ResolveUrl("~/images/notify.gif");
                    lblStatus.Style.Add("color", "#34A9E3");
                    break;
            }

            tblMessage.Visible = true;
            imgMessage.Visible = true;
            lblStatus.Visible = true;
            lblStatus.Text = message;
        }

        /// <summary>
        /// Method to read the message string from the xml file for the given message code
        /// </summary>
        /// <param name="messageCode">Code of the message string defined in the xml file</param>
        /// <returns></returns>
        public string GetApplicationMessage(string messageCode)
        {
            DataView dvMessage;
            string message = string.Empty;
            DataSet dsMessages = new DataSet();
            dsMessages.ReadXml(Server.MapPath("~/XML/ApplicationMessages.xml"));
            dvMessage = dsMessages.Tables[0].DefaultView;
            dvMessage.RowFilter = "Code='" + messageCode + "'";
            if (dvMessage.Count > 0)
            {
                message = dvMessage[0]["Value"].ToString();
            }
            return message;
        }

        /// <summary>
        /// The method used to show the application message for the given message code
        /// </summary>
        /// <param name="messageCode">String  message code which is defined in the application message xml file</param>
        public void ShowApplicationMessage(string messageCode)
        {
            ShowMessage(GetApplicationMessage(messageCode));
        }

        /// <summary>
        /// Used to get the message string for 
        /// </summary>
        /// <param name="returnStatus">input </param>
        public void ShowApplicationMessage(ReturnStatus returnStatus)
        {
            switch (returnStatus)
            {
                case ReturnStatus.Success:
                    ShowMessage(GetApplicationMessage(ReturnStatus.Success.ToString()));
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// This function will hide the status message table
        /// </summary>
        public void HideMessageTable()
        {
            tblMessage.Visible = false;
            imgMessage.Visible = false;
            lblStatus.Visible = false;
        }

        #endregion Other Methods

    }
}