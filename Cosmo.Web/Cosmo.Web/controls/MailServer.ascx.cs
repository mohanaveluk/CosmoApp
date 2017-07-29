using System;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Web;
using System.Web.Configuration;
using System.Xml;
using Cosmo.Entity;
using Cosmo.Service;

namespace Cosmo.Web.controls
{
    public partial class MailServer : System.Web.UI.UserControl
    {

        #region variables

        #region constant

        private const string WEBCONFIG = "web.config";
        private const string CSMCONFIG = "Cog.CSM.exe.config";
        private const string WSCONFIG = "Cog.WS.exe.config";

        #endregion constant



        string sServerName = string.Empty;
        string sServerPort = string.Empty;
        string sUserId = string.Empty;
        string sPassword = string.Empty;
        string sFromEmailId = string.Empty;
        string connectionString = string.Empty;
        string xmlFilePath = string.Empty;
        Configuration config;

        #endregion variables

        protected void Page_Load(object sender, EventArgs e)
        {
            if (HttpContext.Current.Session["LOGGEDINUSER"] != null)
            {
                btnSkip.Visible = false;
                hidLoggedIn.Value = "true";
                if (!IsPostBack)
                {
                    PopulateSmtpServerDetail();
                }
            }
        }

        protected void btnCreate_Click(object sender, EventArgs e)
        {
            genericMessage.Visible = false;
            genericMessage.ShowMessage("");
            frmSubmit.Text = "Processing...";
            var mailServerEntity = new MailServerEntity();

            if (!string.IsNullOrEmpty(txtServerName.Text.Trim()))
                sServerName = txtServerName.Text.Trim();
            if (!string.IsNullOrEmpty(txtServerName.Text.Trim()))
                sServerPort = txtPort.Text.Trim();
            if (!string.IsNullOrEmpty(txtUserID.Text.Trim()))
                sUserId = txtUserID.Text.Trim();
            if (!string.IsNullOrEmpty(txtPasswrd.Text.Trim()))
                sPassword = txtPasswrd.Text.Trim();
            if (!string.IsNullOrEmpty(txtFromMailId.Text.Trim()))
                sFromEmailId = txtFromMailId.Text.Trim();
            var sslEnable = chkReolace.Checked;

            sUserId = CommonUtility.EnryptString(sUserId);
            sPassword = CommonUtility.EnryptString(sPassword);

            mailServerEntity.ServerName = sServerName;
            mailServerEntity.HostIP = sServerName;
            mailServerEntity.Port = sServerPort;
            mailServerEntity.Username = sUserId;
            mailServerEntity.Password = sPassword;
            mailServerEntity.SslEnable = sslEnable;
            mailServerEntity.FromEmailAddress = sFromEmailId;
            mailServerEntity.IsActive = true;


            try
            {
                var commonService = new CommonService();
                var result = commonService.InsMailServerConfiguration(mailServerEntity);

                if (result == 0)
                {
                    xmlFilePath = GetWebConfigLocation(WEBCONFIG);
                    if (System.IO.Directory.Exists(xmlFilePath + "database"))
                    {
                        SetMailTemplatePath(xmlFilePath, WEBCONFIG, sServerName, sServerPort, sUserId, sPassword,
                            sslEnable, sFromEmailId);
                        SetMailTemplatePath(xmlFilePath + "Service Monitor Tool\\", CSMCONFIG, sServerName, sServerPort,
                            sUserId, sPassword, sslEnable, sFromEmailId);
                        SetMailTemplatePath(xmlFilePath + "Windows Service Tool\\", WSCONFIG, sServerName, sServerPort,
                            sUserId, sPassword, sslEnable, sFromEmailId);

                        var webServiceConfigFilePath = CommonUtility.GetWebServiceConfigFilePath(xmlFilePath);
                        SetMailTemplatePath(webServiceConfigFilePath, string.Empty, sServerName, sServerPort,
                            sUserId, sPassword, sslEnable, sFromEmailId);

                        genericMessage.Visible = true;
                        genericMessage.CurMessageType = GenericMessage.MessageType.Confirmation;
                        genericMessage.ShowMessage("Connection successful");

                        Response.Redirect("~/login/Default.aspx");
                    }
                    else
                    {
                        Logger.Log("Folder  'database' does not exists under " + xmlFilePath);
                        genericMessage.Visible = true;
                        genericMessage.CurMessageType = GenericMessage.MessageType.Failure;
                        genericMessage.ShowMessage("Folder 'database' and SQL scripts does not exists");
                    }
                }
                frmSubmit.Text = "Connect";
            }
            catch (SqlException ex)
            {
                Logger.Log(ex.ToString());
                if (ex.Message.ToLower().Contains("login failed for user"))
                {
                    genericMessage.Visible = true;
                    genericMessage.CurMessageType = GenericMessage.MessageType.Failure;
                    genericMessage.ShowMessage(ex.Message);
                }
                else if (ex.Message.ToLower().Contains("the server was not found"))
                {
                    genericMessage.Visible = true;
                    genericMessage.CurMessageType = GenericMessage.MessageType.Failure;
                    genericMessage.ShowMessage("Invalid server name '" + sServerName + "'");
                }
                frmSubmit.Text = "Connect";
            }
            finally
            {
                frmSubmit.Text = "Connect";
            }
        }

        /// <summary>
        /// Update connectionstring of Web.config file
        /// </summary>
        /// <param name="connectionString"></param>
        private void SetWebConnectionString(string connectionString)
        {
            try
            {
                Configuration config = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~");
                ConnectionStringsSection section = config.GetSection("connectionStrings") as ConnectionStringsSection;
                section.ConnectionStrings["CSMConn"].ConnectionString = connectionString;
                config.Save();
            }
            catch (Exception ex)
            {
                Logger.Log(ex.ToString());
            }
        }

        private void SetMailTemplatePath(string configFilePath, string configFileName, string serverName, string port,
            string userid, string password, bool sslEnable, string fromEmailId)
        {
            XmlNode appNode;

            XmlDocument doc = new XmlDocument();
            bool change = false;
            string configFile = System.IO.Path.Combine(configFilePath, configFileName);
            doc.Load(configFile);

            //get root element
            System.Xml.XmlElement Root = doc.DocumentElement;

            #region Updating mail template path

            appNode = Root["mailConfiguration"]["providers"];
            foreach (XmlNode node in appNode.ChildNodes)
            {
                if (node.Attributes != null)
                {
                    try
                    {
                        string key = node.Attributes.GetNamedItem("name").Value;
                        string value = node.Attributes.GetNamedItem("mailTemplatePath").Value;
                        node.Attributes.GetNamedItem("mailTemplatePath").Value = "mail.config";
                        node.Attributes.GetNamedItem("smtpServerHost").Value = serverName;
                        node.Attributes.GetNamedItem("smtpPort").Value = port;
                        node.Attributes.GetNamedItem("smtpUser").Value = userid;
                        node.Attributes.GetNamedItem("smtpPassword").Value = password;
                        node.Attributes.GetNamedItem("smtpEnableSSL").Value = sslEnable ? "true" : "false";
                        node.Attributes.GetNamedItem("requiredAuthentication").Value = "true";
                        change = true;
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex.ToString());
                    }
                }
            }

            #endregion Updating mail template path

            #region Updating Log file path

            appNode = Root["appSettings"];
            foreach (XmlNode node in appNode.ChildNodes)
            {
                if (node.Attributes != null)
                {
                    try
                    {
                        string key = node.Attributes.GetNamedItem("key").Value;
                        string value = node.Attributes.GetNamedItem("value").Value;
                        if (key == "LogFileLocation")
                        {
                            node.Attributes.GetNamedItem("value").Value = System.IO.Path.Combine(xmlFilePath, "Logs");
                            change = true;
                        }
                        if (key == "AdminAddress")
                        {
                            node.Attributes.GetNamedItem("value").Value = fromEmailId;
                            change = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex.ToString());
                    }
                }
            }

            #endregion Updating Log file path

            //Save xml file
            if (change) //Update web.config only if changes are made to web.config file
            {
                try
                {
                    doc.Save(configFile);

                    Logger.Log(string.Format("Updated config file of {0}",
                        System.IO.Path.Combine(configFilePath, configFileName)));
                }
                catch (IOException ex)
                {
                    Logger.Log("Error occured while saving web.config changes.");

                    string logMsg = "Web.config update failed!  ExMsg: {0} - StackTrace: {2}";
                    Logger.Log(string.Format(logMsg, ex.Message, ex.StackTrace));
                }
            }
            else
            {
                Logger.Log(
                    "No need to update web.config file bcoz appsetting and globalization node values are accurate in web.config file");
            }
        }

        /// <summary>
        /// Update connectionstring of App.config file
        /// </summary>
        /// <param name="connectionString"></param>
        private void SetAppConnectionString(string connectionString)
        {
            string xmlFilePathCon = xmlFilePath;
            try
            {
                xmlFilePath = GetWebConfigLocation(WEBCONFIG);

                //Updating Mail template path
                //SetMailTemplatePath(xmlFilePath, "web.config");
                //SetMailTemplatePath(System.IO.Path.Combine(xmlFilePath, "Service Monitor Tool"), "Cog.CSM.exe.config");

                //updating connection string
                xmlFilePathCon += "Service Monitor Tool\\Cog.CSM.exe";

                // open config
                Configuration configExe = ConfigurationManager.OpenExeConfiguration(xmlFilePathCon);

                // set new connectionstring in config
                configExe.ConnectionStrings.ConnectionStrings["CSMConn"].ConnectionString = connectionString;

                //--http://www.stievens-corner.be/index.php/11-c/17-change-connectionstring-and-save-to-app-config
                // save and refresh the config file
                configExe.Save(ConfigurationSaveMode.Minimal);
                ConfigurationManager.RefreshSection("connectionStrings");
            }
            catch (Exception ex)
            {

                Logger.Log(ex.ToString());
            }

        }

        /// <summary>
        /// Update connectionstring of App.config file
        /// </summary>
        /// <param name="connectionString"></param>
        private void SetWinServiceConnectionString(string connectionString)
        {
            string xmlFilePathCon = xmlFilePath;
            try
            {
                xmlFilePath = GetWebConfigLocation(WEBCONFIG);

                //Updating Mail template path
                //SetMailTemplatePath(xmlFilePath, "web.config");
                //SetMailTemplatePath(System.IO.Path.Combine(xmlFilePath, "Windows Service Tool"), "Cog.WS.exe.config");

                //updating connection string
                xmlFilePathCon += "Windows Service Tool\\Cog.WS.exe";

                // open config
                Configuration configExe = ConfigurationManager.OpenExeConfiguration(xmlFilePathCon);

                // set new connectionstring in config
                configExe.ConnectionStrings.ConnectionStrings["CSMConn"].ConnectionString = connectionString;

                //--http://www.stievens-corner.be/index.php/11-c/17-change-connectionstring-and-save-to-app-config
                // save and refresh the config file
                configExe.Save(ConfigurationSaveMode.Minimal);
                ConfigurationManager.RefreshSection("connectionStrings");
            }
            catch (Exception ex)
            {

                Logger.Log(ex.ToString());
            }

        }

        /// <summary>
        /// Create connection xml file
        /// </summary>
        /// <param name="connectionString"></param>
        private void CreateConnectionXML(string connectionString)
        {
            string xmlFilePathCreate = xmlFilePath;
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;

            try
            {
                config = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~");
                xmlFilePathCreate = config.FilePath.ToLower().Replace(WEBCONFIG, "SqlConnection.xml");
                using (XmlWriter writer = XmlWriter.Create(xmlFilePathCreate, settings))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("connectionStrings");
                    writer.WriteStartElement("add");
                    writer.WriteAttributeString("name", "CSMConn");
                    writer.WriteAttributeString("connectionString", connectionString);
                    writer.WriteAttributeString("providerName", "System.Data.SqlClient");
                    writer.WriteEndDocument();
                    writer.Flush();
                    writer.Close();
                }

            }
            catch (Exception ex)
            {

                Logger.Log(ex.ToString());
            }

        }


        /// <summary>
        /// Get path of the specific file location
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private string GetWebConfigLocation(string fileName)
        {
            xmlFilePath = string.Empty;
            Configuration config = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~");
            xmlFilePath = config.FilePath.Substring(0, (config.FilePath.Length - fileName.Length));
            return xmlFilePath;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private void GetWebServiceConfigLocation(string fileName)
        {
            var xmlFilePathCon = CommonUtility.GetWebServiceConfigFilePath(xmlFilePath); //Path.Combine(parent, mobileServiceDir, "Web.config");
            Logger.Log(xmlFilePathCon);
            FileInfo fileInfo = new FileInfo(xmlFilePathCon);
            var vdm = new VirtualDirectoryMapping(fileInfo.DirectoryName, false, fileInfo.Name);
            var wcFm = new WebConfigurationFileMap();
            wcFm.VirtualDirectories.Add("/", vdm);

        }

        protected void btnSkip_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/login/Default.aspx");
        }

        private void PopulateSmtpServerDetail()
        {
            var commonService = new CommonService();
            var smtpDetail = commonService.GetMailServer();
            if (smtpDetail != null && smtpDetail.Count > 0)
            {
                txtServerName.Text = CommonService.GetText(smtpDetail[0].HostIP);
                txtPort.Text = CommonService.GetText(smtpDetail[0].Port);
                txtUserID.Text = CommonUtility.DecryptString(CommonService.GetText(smtpDetail[0].Username));
                txtPasswrd.Text = CommonUtility.DecryptString(CommonService.GetText(smtpDetail[0].Password));
                chkReolace.Checked = smtpDetail[0].SslEnable;
                txtFromMailId.Text = smtpDetail[0].FromEmailAddress;
            }
            smtpDetail = null;
        }

    }
}