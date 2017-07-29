using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Xml;
using System.Diagnostics;
using System.Text;
using System.IO;
using System.Linq;
using System.Web.Configuration;
using Cosmo.Entity;
using Cosmo.Service;
using Cosmo.Web.controls;
using Oracle.ManagedDataAccess.Client;

namespace Cosmo.Web.setup
{
    public partial class DBConfig : System.Web.UI.Page
    {
        #region constant
        private const string WEBCONFIG = "web.config";
        private const string ServiceMonitorTool = "Service Monitor Tool";
        private const string WindowsServiceTool = "Windows Service Tool";
        private const string CosmoWebservice = "Cosmo.WebServices";
        #endregion constant
        
        #region variables

        string sDBType = string.Empty;
        string sServerName = string.Empty;
        string sDatabaseName = string.Empty;
        string sPort = string.Empty;
        string sUserId = string.Empty;
        string sPassword = string.Empty;
        string connectionString = string.Empty;
        string xmlFilePath = string.Empty;
        Configuration config;
        #endregion variables

        protected void Page_Load(object sender, EventArgs e)
        {
            //ValidateLicenseKey();
        }

        protected void btnCreate_Click(object sender, EventArgs e)
        {
            genericMessage.Visible = false;
            genericMessage.ShowMessage("");
            frmSubmit.Text = "Processing...";

            if (!string.IsNullOrEmpty(drpDatabaseType.SelectedValue))
                sDBType = drpDatabaseType.SelectedValue;
            if (!string.IsNullOrEmpty(txtServerName.Text.Trim()))
                sServerName = txtServerName.Text.Trim();
            if (!string.IsNullOrEmpty(txtPort.Text.Trim()))
                sPort = txtPort.Text.Trim();
            if (!string.IsNullOrEmpty(txtDatabaseName.Text.Trim()))
                sDatabaseName = txtDatabaseName.Text.Trim();
            if (!string.IsNullOrEmpty(txtUserID.Text.Trim()))
                sUserId = txtUserID.Text.Trim();
            if (!string.IsNullOrEmpty(txtPasswrd.Text.Trim()))
                sPassword = txtPasswrd.Text.Trim();

            var encryptedUserId = CommonUtility.EnryptString(sUserId);
            var encryptedPassword = CommonUtility.EnryptString(sPassword);

            if (sDBType == Convert.ToInt32(DatabaseType.Oracle).ToString())
            {
                connectionString =
                    "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=" + sServerName + ")(PORT=" + sPort + ")))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=" + sDatabaseName + "))); user id=" + encryptedUserId + "; password=" + encryptedPassword + ";Connection Timeout=60;";
                UpdateConfigurationDetailsOracle(connectionString, sServerName, sDatabaseName, encryptedUserId, encryptedPassword);
            }
            else if (sDBType == Convert.ToInt32(DatabaseType.SqlServer).ToString())
            {
                if (drpAuthenticationType.SelectedValue == "2")
                {
                    connectionString = "Data Source=" + sServerName + ";User ID=" + encryptedUserId + ";password=" +
                                       encryptedPassword + ";Initial Catalog=" + sDatabaseName + ";";
                }
                else
                {
                    connectionString = "Data Source=" + sServerName + ";Initial Catalog=" + sDatabaseName +
                                       ";Integrated Security=True";
                }
                UpdateConfigurationDetailsSql(connectionString, sServerName, sDatabaseName, encryptedUserId, encryptedPassword);
            }

        }

        /// <summary>
        /// Update connectionstring of Web.config file
        /// </summary>
        /// <param name="connectionStr"></param>
        private void SetWebConnectionString(string connectionStr)
        {
            try
            {
                Configuration config = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~");
                ConnectionStringsSection section = config.GetSection("connectionStrings") as ConnectionStringsSection;
                if (section != null)
                {
                    section.ConnectionStrings["CSMConn"].ConnectionString = connectionStr;
                    section.ConnectionStrings["CSMConn"].ProviderName =
                        sDBType == Convert.ToInt32(DatabaseType.Oracle).ToString()
                            ? "System.Data.OracleClient"
                            : "System.Data.SqlClient";
                }
                config.Save();
            }
            catch (Exception ex)
            {

                Logger.Log(ex.ToString());
            }
        }

        private void SetMailTemplatePath(string configFilePath, string configFileName)
        {
            XmlNode appNode;

            XmlDocument doc = new XmlDocument();
            bool change = false;
            string configFile = Path.Combine(configFilePath, configFileName);
            doc.Load(configFile);

            //get root element
            XmlElement Root = doc.DocumentElement;

            #region Updating mail template path
            
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
                            node.Attributes.GetNamedItem("value").Value = Path.Combine(xmlFilePath, "Logs");
                            change = true;
                            break;
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
            if (change)  //Update web.config only if changes are made to web.config file
            {
                try
                {
                    doc.Save(configFile);

                    Logger.Log(string.Format("Updated config file of {0}", Path.Combine(configFilePath, configFileName)));
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
                Logger.Log("No need to update web.config file bcoz appsetting and globalization node values are accurate in web.config file");
            }
        }


        private void SetDatabaseType(string configFilePath, string configFileName)
        {
            XmlNode appNode = null;

            var doc = new XmlDocument();
            var change = false;
            var configFile = Path.Combine(configFilePath, configFileName);
            doc.Load(configFile);

            //get root element
            var root = doc.DocumentElement;

            #region Updating Log file path

            if (root != null) appNode = root["appSettings"];
            if (appNode != null)
                foreach (var node in appNode.ChildNodes.Cast<XmlNode>().Where(node => node.Attributes != null))
                {
                    try
                    {
                        if (node.Attributes == null) continue;
                        var key = node.Attributes.GetNamedItem("key").Value;
                        var value = node.Attributes.GetNamedItem("value").Value;

                        if (key != "DatabaseType") continue;
                        node.Attributes.GetNamedItem("value").Value = sDBType == "1"
                            ? DatabaseType.Oracle.ToString()
                            : DatabaseType.SqlServer.ToString();
                        change = true;
                        break;
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex.ToString());
                    }
                }

            #endregion Updating Log file path

            //Save xml file
            if (change)  //Update web.config only if changes are made to web.config file
            {
                try
                {
                    doc.Save(configFile);

                    Logger.Log(string.Format("Updated config file of {0}", Path.Combine(configFilePath, configFileName)));
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
                Logger.Log("No need to update web.config file bcoz appsetting and globalization node values are accurate in web.config file");
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

                SetDatabaseType(xmlFilePath, "web.config");

                //updating connection string
                xmlFilePathCon += ServiceMonitorTool + "\\Cog.CSM.exe";

                SetDatabaseType(Path.Combine(xmlFilePath, ServiceMonitorTool), "Cog.CSM.exe.config");

                // open config
                Configuration configExe = ConfigurationManager.OpenExeConfiguration(xmlFilePathCon);

                // set new connectionstring in config
                configExe.ConnectionStrings.ConnectionStrings["CSMConn"].ConnectionString = connectionString;
                configExe.ConnectionStrings.ConnectionStrings["CSMConn"].ProviderName =
                    sDBType == Convert.ToInt32(DatabaseType.Oracle).ToString()
                        ? "System.Data.OracleClient"
                        : "System.Data.SqlClient";
                

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
        /// <param name="connectionStr"></param>
        private void SetWinServiceConnectionString(string connectionStr)
        {
            string xmlFilePathCon = xmlFilePath;
            try
            {
                xmlFilePath = GetWebConfigLocation(WEBCONFIG);

                //Updating Mail template path
                //SetMailTemplatePath(xmlFilePath, "web.config");
                //SetMailTemplatePath(System.IO.Path.Combine(xmlFilePath, "Windows Service Tool"), "Cog.WS.exe.config");

                //updating connection string
                xmlFilePathCon += WindowsServiceTool + "\\Cog.WS.exe";

                SetDatabaseType(Path.Combine(xmlFilePath, WindowsServiceTool), "Cog.WS.exe.config");

                // open config
                Configuration configExe = ConfigurationManager.OpenExeConfiguration(xmlFilePathCon);

                // set new connectionstring in config
                configExe.ConnectionStrings.ConnectionStrings["CSMConn"].ConnectionString = connectionStr;
                configExe.ConnectionStrings.ConnectionStrings["CSMConn"].ProviderName =
                    sDBType == Convert.ToInt32(DatabaseType.Oracle).ToString()
                        ? "System.Data.OracleClient"
                        : "System.Data.SqlClient";
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

        private void SetWebServiceConnectionString(string connectionStr)
        {
            try
            {
                var xmlFilePathCon = CommonUtility.GetWebServiceConfigFilePath(xmlFilePath); //Path.Combine(parent, mobileServiceDir, "Web.config");
                Logger.Log(xmlFilePathCon);

                FileInfo fileInfo = new FileInfo(xmlFilePathCon);
                var vdm = new VirtualDirectoryMapping(fileInfo.DirectoryName, false, fileInfo.Name);
                var wcFm = new WebConfigurationFileMap();
                wcFm.VirtualDirectories.Add("/", vdm);

                SetDatabaseType(CommonUtility.GetWebServicePath(xmlFilePath), "web.config");

                Configuration webConfiguration = System.Web.Configuration.WebConfigurationManager.OpenMappedWebConfiguration(wcFm, "/");
                ConnectionStringsSection section = webConfiguration.GetSection("connectionStrings") as ConnectionStringsSection;
                if (section != null)
                {
                    section.ConnectionStrings["CSMConn"].ConnectionString = connectionStr;
                    section.ConnectionStrings["CSMConn"].ProviderName =
                        sDBType == Convert.ToInt32(DatabaseType.Oracle).ToString()
                            ? "System.Data.OracleClient"
                            : "System.Data.SqlClient";
                }
                webConfiguration.Save();

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
        private void CreateConnectionXML(string connectionStr)
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
                    writer.WriteAttributeString("connectionString", connectionStr);
                    writer.WriteAttributeString("providerName", sDBType == Convert.ToInt32(DatabaseType.Oracle).ToString() ? "System.Data.OracleClient" : "System.Data.SqlClient");
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
        /// to create batch file with sql file location to run and load the SQL scripts
        /// </summary>
        /// <param name="serverInstance"></param>
        /// <param name="database"></param>
        /// <param name="userID"></param>
        /// <param name="password"></param>
        /// <param name="dbType"></param>
        private void CreateSQLBatchFile(string serverInstance, string database, string userID, string password, string dbType)
        {
            xmlFilePath = string.Empty; string sqlcmdPath = string.Empty;
            string sqlcmdCommand = string.Empty;
            string SqlBatchFile = string.Empty;
            string SqlToExecute = string.Empty;
            string OutSqlFileName = string.Empty;

            StringBuilder sb = new StringBuilder();
            try
            {
                xmlFilePath = GetWebConfigLocation(WEBCONFIG);
                SqlBatchFile = xmlFilePath + string.Format("database\\{0}\\RunSMonitor.bat", dbType);
                SqlToExecute = xmlFilePath + string.Format("database\\{0}\\ServiceManagerScript.sql", dbType);
                OutSqlFileName = xmlFilePath + string.Format("database\\{0}\\ServiceManageOutput.txt", dbType);
                sqlcmdPath = @xmlFilePath + string.Format("database\\{0}\\sqlcmd.exe", dbType);

                if (dbType == DatabaseType.SqlServer.ToString())
                {
                    sqlcmdCommand = !string.IsNullOrEmpty(userID)
                        ? "\"" + sqlcmdPath + "\" -U " + userID + " -P " + password + " -S " + serverInstance +
                          " -d " + database + " -i %1 -o \"" + @OutSqlFileName + "\""
                        : "\"" + sqlcmdPath + "\" -S " + serverInstance + " -d " + database + " -i %1 -o \"" +
                          @OutSqlFileName + "\"";

                    sb.AppendLine(xmlFilePath.Substring(0, 2));
                    sb.AppendLine(@"cd " + string.Format("{0}database\\{1}",xmlFilePath,dbType));
                    sb.AppendLine("@Echo Off");
                    sb.AppendLine("FOR /f %%i IN ('DIR ServiceManagerScript.sql /B') do call :RunScript %%i");
                    sb.AppendLine("GOTO :END");
                    sb.AppendLine(":RunScript");
                    sb.AppendLine("Echo Executing %1");
                    sb.AppendLine(sqlcmdCommand);
                    sb.AppendLine("Echo Completed %1");
                    sb.AppendLine(":END");
                }
                else
                {
                    sqlcmdCommand = !string.IsNullOrEmpty(userID)
                        ? "sqlplus " + userID + "/" + password + "@" + database + " @\"%1\"" : "";
                    sb.AppendLine(xmlFilePath.Substring(0, 2));
                    sb.AppendLine(@"cd " + string.Format("{0}database\\{1}", xmlFilePath, dbType));
                    sb.AppendLine("@Echo Off");
                    sb.AppendLine("FOR /f %%i IN (ServiceManager.sql) do call :RunScript %%i");
                    sb.AppendLine("GOTO :END");
                    sb.AppendLine(":RunScript");
                    sb.AppendLine("Echo Executing %1");
                    sb.AppendLine(sqlcmdCommand);
                    sb.AppendLine("Echo Completed %1");
                    sb.AppendLine(":END");
                }

                File.WriteAllText(SqlBatchFile, sb.ToString());

            }
            catch (Exception ex)
            {
                Logger.Log(ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// Load sql scripts of tables and procedures in the given database
        /// </summary>
        private void LoadDatabaseScript(string serverInstance, string database, string userID, string password,
            string dbType)
        {
            xmlFilePath = string.Empty;
            var SqlToExecute = string.Empty;
            var OutFileName = string.Empty;
            try
            {
                xmlFilePath = GetWebConfigLocation(WEBCONFIG);

                SqlToExecute = xmlFilePath + "database\\ServiceManagerScript.sql";
                OutFileName = xmlFilePath + string.Format("database\\{0}\\Output.txt", dbType);
                using (Process process = new Process())
                {
                    process.StartInfo.FileName = @xmlFilePath +
                                                 string.Format("database\\{0}\\RunSMonitor.bat", dbType);
                    //process.StartInfo.Arguments = @"-U " + userID + " -P " + password + " -S " + serverInstance + " -d " + database + @" -i """ + @SqlToExecute + @""" -o """ + @OutSqlFileName + @"""";
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.RedirectStandardError = true;
                    process.StartInfo.CreateNoWindow = true;
                    process.Start();
                    File.AppendAllText(@OutFileName, process.StandardOutput.ReadToEnd());
                    process.WaitForExit();
                    File.AppendAllText(@OutFileName, process.StandardOutput.ReadToEnd());
                    process.Close();
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

        private void UpdateConfigurationDetailsSql(string connStr, string serverName, string database, string userId, string password)
        {
            var connection = new SqlConnection(CommonUtility.GetParsedConnectionString(connectionString));

            try
            {
                if (sDBType == Convert.ToInt32(DatabaseType.SqlServer).ToString())
                {
                    Logger.Log(connection.State.ToString());
                    //Logger.Log(connectionString);
                    if (connection.State == ConnectionState.Closed)
                    {
                        //Logger.Log("inside");
                        connection.Open();
                    }
                    Logger.Log(connection.State.ToString());
                    if (connection.State == ConnectionState.Open)
                    {
                        xmlFilePath = GetWebConfigLocation(WEBCONFIG);
                        if (Directory.Exists(xmlFilePath + "database"))
                        {
                            CreateSQLBatchFile(sServerName, sDatabaseName, sUserId, sPassword, DatabaseType.SqlServer.ToString());
                            CreateConnectionXML(connectionString);
                            SetAppConnectionString(connectionString);
                            SetWinServiceConnectionString(connectionString);
                            SetWebConnectionString(connectionString);
                            SetWebServiceConnectionString(connectionString);
                            if (chkReolace.Checked)
                                LoadDatabaseScript(sServerName, sDatabaseName, sUserId, sPassword, DatabaseType.SqlServer.ToString());

                            genericMessage.Visible = true;
                            genericMessage.CurMessageType = GenericMessage.MessageType.Confirmation;
                            genericMessage.ShowMessage("Connection successful");
                        }
                        else
                        {
                            Logger.Log("Folder  'database' does not exists under " + xmlFilePath);
                            genericMessage.Visible = true;
                            genericMessage.CurMessageType = GenericMessage.MessageType.Failure;
                            genericMessage.ShowMessage("Folder 'database' and SQL scripts does not exists");
                        }
                        connection.Close();
                    }
                }
                Response.Redirect(chkReolace.Checked ? "Access.aspx" : "MailServerConfig.aspx");

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
                else if (ex.Message.ToLower().Contains("cannot open database"))
                {
                    genericMessage.Visible = true;
                    genericMessage.CurMessageType = GenericMessage.MessageType.Failure;
                    genericMessage.ShowMessage("Invalid database name '" + sDatabaseName + "'");
                }
                frmSubmit.Text = "Connect";
            }
            finally
            {
                frmSubmit.Text = "Connect";
                connection.Close();
            }

        }

        private void UpdateConfigurationDetailsOracle(string connStr, string serverName, string database, string userId, string password)
        {
            var connection = new OracleConnection(CommonUtility.GetParsedConnectionString(connectionString));

            try
            {
                if (sDBType == Convert.ToInt32(DatabaseType.Oracle).ToString())
                {
                    Logger.Log(connection.State.ToString());
                    //Logger.Log(connectionString);
                    if (connection.State == ConnectionState.Closed)
                    {
                        //Logger.Log("inside");
                        
                        connection.Open();
                    }
                    Logger.Log(connection.State.ToString());
                    if (connection.State == ConnectionState.Open)
                    {
                        xmlFilePath = GetWebConfigLocation(WEBCONFIG);
                        if (Directory.Exists(xmlFilePath + "database"))
                        {
                            CreateSQLBatchFile(sServerName, sDatabaseName, sUserId, sPassword, DatabaseType.Oracle.ToString());
                            CreateConnectionXML(connectionString);
                            SetAppConnectionString(connectionString);
                            SetWinServiceConnectionString(connectionString);
                            SetWebConnectionString(connectionString);
                            SetWebServiceConnectionString(connectionString);
                            if (chkReolace.Checked)
                                LoadDatabaseScript(sServerName, sDatabaseName, sUserId, sPassword, DatabaseType.Oracle.ToString());

                            genericMessage.Visible = true;
                            genericMessage.CurMessageType = GenericMessage.MessageType.Confirmation;
                            genericMessage.ShowMessage("Connection successful");
                        }
                        else
                        {
                            Logger.Log("Folder  'database' does not exists under " + xmlFilePath);
                            genericMessage.Visible = true;
                            genericMessage.CurMessageType = GenericMessage.MessageType.Failure;
                            genericMessage.ShowMessage("Folder 'database' and SQL scripts does not exists");
                        }
                        connection.Close();
                    }
                }
                Response.Redirect(chkReolace.Checked ? "Access.aspx" : "MailServerConfig.aspx");

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
                else if (ex.Message.ToLower().Contains("cannot open database"))
                {
                    genericMessage.Visible = true;
                    genericMessage.CurMessageType = GenericMessage.MessageType.Failure;
                    genericMessage.ShowMessage("Invalid database name '" + sDatabaseName + "'");
                }
                frmSubmit.Text = "Connect";
            }
            finally
            {
                frmSubmit.Text = "Connect";
                connection.Close();
            }
        }

        private void ValidateLicenseKey()
        {
            try
            {
                var commonService = new CommonService();
                var licenseKey = commonService.GetLicenseKey();
                var decryptedLicenseKey = CommonUtility.Decrypt_ActivationKey(licenseKey);
                var licenseKeyDetails = decryptedLicenseKey.Split('&');

                if (licenseKeyDetails.Length > 0)
                {
                    var licenseType = Convert.ToInt32(licenseKeyDetails[1]);
                    if (licenseType == 4) return;
                    if (DateTime.Parse(licenseKeyDetails[2]).AddMonths(licenseType) < DateTime.Now)
                    {
                        Logger.Log("Product was installed on " + licenseKeyDetails[2] + " and Expired on " + DateTime.Parse(licenseKeyDetails[2]).AddMonths(licenseType).ToLongDateString());
                        Response.Redirect("~/login/ActivateProduct.aspx", false);
                    }
                    else
                    {
                        lblExpiry.InnerText = "Product expires on " +
                                              DateTime.Parse(licenseKeyDetails[2]).AddMonths(licenseType).ToLongDateString();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex.ToString());
                Response.Redirect("~/login/ActivateProduct.aspx");
            }
        }
    }
}