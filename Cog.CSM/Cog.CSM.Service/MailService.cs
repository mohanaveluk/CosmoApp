using System;
using System.Collections.Generic;
using System.Collections;
using System.Configuration;
using System.Net.Mail;
using Cog.CSM.MailService;
using System.Reflection;
using Cog.CSM.Entity;
using Cog.CSM.Data;

namespace Cog.CSM.Service
{
    public class MailService
    {
        readonly IExecuteServiceData _executeData;
        readonly ISchedularData _schedularData;
        #region variables

        //string MAILTEMPLATE_PATH = Convert.ToString(ConfigurationManager.AppSettings["mailTemplatePath"]);
        //private static Configuration config = WebConfigurationManager.OpenWebConfiguration(HttpContext.Current.Request.ApplicationPath);
        //MailSettingsSectionGroup settings = (MailSettingsSectionGroup)config.GetSectionGroup("system.net/mailSettings");
        private static readonly string DbType = ConfigurationManager.AppSettings["DatabaseType"];

        string ADMIN_ADDRESS = ConfigurationManager.AppSettings["AdminAddress"].ToString();
        string TO_ADDRESS = ConfigurationManager.AppSettings["ToAddress"].ToString();
        string CC_ADDRESS = ConfigurationManager.AppSettings["CcAddress"].ToString();
        string TEST_TO_ADDRESS = ConfigurationManager.AppSettings["TestToAddress"].ToString();

        string SMTP_SERVER = ConfigurationManager.AppSettings["smtpServerHost"].ToString();

        private const string UNABLETOSEND = "Unable to send to all recipients";
        private const string MAILBOXUNAVAILABLE = "Mailbox unavailable";
        private const string AUTHENTICATIONREQUIRED = "Authentication required";
        private const string RECIPIENTMISSING = "recipient must be specified";
        private const string FAILURE_SENDMAIL = "Failure sending mail";
        private const string UNABLETOCONNECT_MAILSERVER = "Unable to connect to the remote server";
        private const string INVALID_CERTIFICATE = "remote certificate is invalid";
        private const string INVALID_FROMADDRESS = "from address not verified";

        #endregion variables

        #region Variable Declarations

        #region Private Fields

        private string _applicationName;
        private string _objectName;
        private string _assemblyFilePath;
        private string _assemblyName;
        private string _businessClass;
        private string _getMethod;
        private string _entityClass;
        private string _idProperty;
        private string _connectionString;
        private string _insertMethod;
        protected Guid? uniqueId;
        private Assembly _assembly = null;
        private Object _currentObject = null;
        private Type _type = null;

        public MailTemplateParser MailTemplateFromObject { get; set; }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or Sets the Name of the Data Service 
        /// </summary>
        public string ApplicationName
        {
            get { return _applicationName; }
            set { _applicationName = value; }
        }

        /// <summary>
        /// Gets or sets the name of the Object Type
        /// </summary>
        public string ObjectName
        {
            get { return _objectName; }
            set { _objectName = value; }
        }

        /// <summary>
        /// Gets or Sets the Assembly File Path of the Data Service
        /// </summary>
        public string AssemblyFilePath
        {
            get { return _assemblyFilePath; }
            set { _assemblyFilePath = value; }
        }

        /// <summary>
        /// Gets or Sets the Assembly Name of the Data Service
        /// </summary>
        public string AssemblyName
        {
            get { return _assemblyName; }
            set { _assemblyName = value; }
        }

        /// <summary>
        /// Gets or Sets the Business Class Name in the Assembly of the Data Service
        /// </summary>
        public string BusinessClass
        {
            get { return _businessClass; }
            set { _businessClass = value; }
        }

        /// <summary>
        /// Gets or Sets the Method which returns Data Service's Entity Object
        /// </summary>
        public string GetMethod
        {
            get { return _getMethod; }
            set { _getMethod = value; }
        }

        /// <summary>
        /// Gets or Sets the Entity Class of the Data Service
        /// </summary>
        public string EntityClass
        {
            get { return _entityClass; }
            set { _entityClass = value; }
        }

        /// <summary>
        /// Gets or Sets the Name of the Object Identifier
        /// </summary>
        public string IdProperty
        {
            get { return _idProperty; }
            set { _idProperty = value; }
        }

        /// <summary>
        /// Gets or Sets the Connection String Property 
        /// </summary>
        public string ConnectionString
        {
            get { return _connectionString; }
            set { _connectionString = value; }
        }

        /// <summary>
        /// Gets or Sets the Name of the Method which inserts mail in the database
        /// </summary>
        public string InsertMethod
        {
            get { return _insertMethod; }
            set { _insertMethod = value; }
        }

        ///// <summary>
        ///// Gets or Sets the type of the mail that is being sent
        ///// </summary>
        //public string MailType { get; set; }

        /// <summary>
        /// Gets or Sets UniqueId property for every business object
        /// </summary>
        public Guid? UniqueId
        {
            get
            {
                return uniqueId;
            }
            set
            {
                uniqueId = value;
            }
        }

        public string HostName
        {
            get;
            set;
        }

        public List<int> AttachmentID { get; set; }

        #endregion

        #endregion

        #region Private Properties

        /// <summary>
        ///  Gets the TRW.Common.Net.Mail.MailTemplateParser object
        /// </summary>        
        /// <returns>TRW.Common.Net.Mail.MailTemplateParser object</returns>
        private MailTemplateParser MailTemplate
        {
            get
            {
                MailTemplateParser mp = new MailTemplateParser();
                // Get the mail templates
                mp.Parse();
                return mp;
            }
        }

        #endregion

        public MailService()
        {
            var iDbType = DbType == DatabaseType.Oracle.ToString()
               ? Convert.ToInt32(DatabaseType.Oracle).ToString()
               : Convert.ToInt32(DatabaseType.SqlServer).ToString();

            _schedularData = new SchedularDataFactory().Create(Convert.ToInt32(iDbType).ToString());
            _executeData = new ExecuteServiceDataFactory().Create(Convert.ToInt32(iDbType).ToString());
        }
        #region SendMail

        /// <summary>
        /// 
        /// </summary>
        /// <param name="envId"></param>
        /// <param name="emaiList"></param>
        /// <param name="mailTypeId"></param>
        /// <param name="objectName"></param>
        public void SendMail(int envId, List<string> emaiList,  string mailTypeId, object objectName)
        {
            try
            {
                var smsMailTypeId = mailTypeId + "_sms";
                var error = string.Empty;
                var smtpClientInfo = MailTemplate.SmtpClientInfo;

                var mailMessage = GetStatusReportMailMessage<object>(envId, emaiList, mailTypeId, objectName);
                if (!string.IsNullOrEmpty(mailMessage.Body))
                    error = MailUtilities.SendMail(smtpClientInfo, mailMessage);

                if (!string.IsNullOrEmpty(error)) Logger.Log("Email status: " + error);

                if (!string.IsNullOrEmpty(error))
                    Logger.Log(error);

                var mailEntity = new MailLogEntity
                {
                    ENV_ID = envId,
                    Config_ID = 0,
                    To_Address = mailMessage.To.ToString(),
                    Cc_Address = mailMessage.CC.ToString(),
                    Bcc_Address = mailMessage.Bcc.ToString(),
                    Subject = mailMessage.Subject.ToString(),
                    Body = mailMessage.Body.ToString(),
                    Error = error
                };

                if (!string.IsNullOrEmpty(error) && error.ToLower().Contains(UNABLETOSEND.ToLower()))
                    mailEntity.Status = "Not sent";
                else if (!string.IsNullOrEmpty(error) && error.ToLower().Contains(MAILBOXUNAVAILABLE.ToLower()))
                    mailEntity.Status = "Partially sent";
                else if (!string.IsNullOrEmpty(error) && (error.ToLower().Contains(AUTHENTICATIONREQUIRED.ToLower()) || error.ToLower().Contains(RECIPIENTMISSING.ToLower()) || error.ToLower().Contains(FAILURE_SENDMAIL.ToLower()) || error.ToLower().Contains(UNABLETOCONNECT_MAILSERVER.ToLower()) || error.ToLower().Contains(INVALID_CERTIFICATE.ToLower()) || error.ToLower().Contains(INVALID_FROMADDRESS.ToLower())))
                    mailEntity.Status = "Not sent";
                else
                    mailEntity.Status = "Sent";

                if (mailMessage.IsBodyHtml)
                    mailEntity.ContentType = "Html";
                else
                    mailEntity.ContentType = "Text";
                mailEntity.Comments = string.Empty;
                Logger.Log("Mail status : " + mailEntity.Status);
                _executeData.InserrtMailLog(mailEntity);
            }
            catch (Exception ex)
            {
                Logger.Log(ex.Message);
                if (ex.InnerException != null && !string.IsNullOrEmpty(ex.InnerException.Message))
                    Logger.Log(ex.InnerException.Message);
            }

        }

        private MailMessage GetStatusReportMailMessage<T>(int envId, List<string> emaiList, string id, T typeObject)
        {
            string toAddress = string.Empty;
            string ccAddress = string.Empty;
            string bccAddress = string.Empty;
            string replyToAddress = string.Empty;
            var mailMessage = new MailMessage();

            var independentValues = new Hashtable();
            if (emaiList == null || emaiList.Count <= 0) return mailMessage;

            foreach (var emailAddress in emaiList)
            {
                if (!string.IsNullOrEmpty(toAddress))
                    toAddress += ";" + emailAddress;
                else
                    toAddress += emailAddress;
            }

            if (independentValues.ContainsKey("AdminAddress")) return mailMessage;

            independentValues.Add("AdminAddress", ADMIN_ADDRESS);
            independentValues.Add("DistributionEmail", toAddress);
            independentValues.Add("CCAddress", ccAddress);
            independentValues.Add("BCCAddress", bccAddress);

            mailMessage = MailTemplate.GetMailMessage<T>(id, typeObject, independentValues);

            return mailMessage;
        }

        /// <summary>
        /// Send mail to the admin in case of service failures
        /// </summary>
        /// <param name="objName"></param>
        public void SendMail(int envId, int ConfigId, string mailTypeId, object objectName)
        {
            try
            {
                var smsMailTypeId = mailTypeId + "_sms";
                var error = string.Empty;

                SmtpClient smtpClientInfo = MailTemplate.SmtpClientInfo;
                MailMessage mailMessage = GetMailMessage<object>(envId, smsMailTypeId, objectName);
                if (!string.IsNullOrEmpty(mailMessage.Body))
                    error = MailUtilities.SendMail(smtpClientInfo, mailMessage);

                if (!string.IsNullOrEmpty(error)) Logger.Log("SMS status: " + error);

                mailMessage = GetMailMessage<object>(envId, mailTypeId, objectName);
                if (!string.IsNullOrEmpty(mailMessage.Body))
                    error = MailUtilities.SendMail(smtpClientInfo, mailMessage);

                if (!string.IsNullOrEmpty(error)) Logger.Log("Email status: " + error);

                if (!string.IsNullOrEmpty(error))
                    Logger.Log(error);
                MailLogEntity mailEntity = new MailLogEntity();
                mailEntity.ENV_ID = envId;
                mailEntity.Config_ID = ConfigId;
                mailEntity.To_Address = mailMessage.To.ToString();
                mailEntity.Cc_Address = mailMessage.CC.ToString();
                mailEntity.Bcc_Address = mailMessage.Bcc.ToString();
                mailEntity.Subject = mailMessage.Subject.ToString();
                mailEntity.Body = mailMessage.Body.ToString();
                mailEntity.Error = error;
                if (!string.IsNullOrEmpty(error) && error.ToLower().Contains(UNABLETOSEND.ToLower()))
                    mailEntity.Status = "Not sent";
                else if (!string.IsNullOrEmpty(error) && error.ToLower().Contains(MAILBOXUNAVAILABLE.ToLower()))
                    mailEntity.Status = "Partially sent";
                else if (!string.IsNullOrEmpty(error) && (error.ToLower().Contains(AUTHENTICATIONREQUIRED.ToLower()) || error.ToLower().Contains(RECIPIENTMISSING.ToLower()) || error.ToLower().Contains(FAILURE_SENDMAIL.ToLower()) || error.ToLower().Contains(UNABLETOCONNECT_MAILSERVER.ToLower()) || error.ToLower().Contains(INVALID_CERTIFICATE.ToLower()) || error.ToLower().Contains(INVALID_FROMADDRESS.ToLower())))
                    mailEntity.Status = "Not sent";
                else
                    mailEntity.Status = "Sent";

                if (mailMessage.IsBodyHtml)
                    mailEntity.ContentType = "Html";
                else
                    mailEntity.ContentType = "Text";
                mailEntity.Comments = string.Empty;// mailMessage.DeliveryNotificationOptions.ToString();
                Logger.Log("Mail status : " + mailEntity.Status);
                _executeData.InserrtMailLog(mailEntity);
            }
            catch (Exception ex)
            {
                Logger.Log(ex.Message);
                if (ex.InnerException != null && !string.IsNullOrEmpty(ex.InnerException.Message))
                    Logger.Log(ex.InnerException.Message);
            }
        }
        #endregion

        #region create mail message
        private MailMessage GetMailMessage<T>(int envId, string id, T typeObject)
        {
            string toAddress = string.Empty;
            string ccAddress = string.Empty;
            string bccAddress = string.Empty;
            string replyToAddress = string.Empty;
            MailMessage mailMessage = new MailMessage();
            try
            {
                //Get email id list for respctive environment 
                List<UserEmailEntity> userEmailLst = new List<UserEmailEntity>();

                userEmailLst = _schedularData.GetUserEMail(envId, id.Contains("sms") ? "T" : "E");
                //MailTemplateParser MailTemplate = new MailTemplateParser();
                Hashtable independentValues = new Hashtable();

                if (userEmailLst != null && userEmailLst.Count > 0)
                {
                    foreach (UserEmailEntity user in userEmailLst)
                    {
                        switch (user.EmailType.ToLower())
                        {
                            case "to":
                                if (!string.IsNullOrEmpty(toAddress))
                                    toAddress += ";" + user.EmailAddress;
                                else
                                    toAddress += user.EmailAddress;
                                break;
                            case "cc":
                                if (!string.IsNullOrEmpty(ccAddress))
                                    ccAddress += ";" + user.EmailAddress;
                                else
                                    ccAddress += user.EmailAddress;
                                break;
                            case "bcc":
                                if (!string.IsNullOrEmpty(bccAddress))
                                    bccAddress += ";" + user.EmailAddress;
                                else
                                    bccAddress += user.EmailAddress;
                                break;
                        }
                        if (user.EmailType.ToLower() == "reply")
                        {
                            if (!string.IsNullOrEmpty(replyToAddress))
                                replyToAddress += ";" + user.EmailAddress;
                            else
                                replyToAddress += user.EmailAddress;
                        }
                    }
                }
                if (!independentValues.ContainsKey("AdminAddress"))
                {
                    independentValues.Add("AdminAddress", ADMIN_ADDRESS);
                    independentValues.Add("DistributionEmail", toAddress);
                    independentValues.Add("CCAddress", ccAddress);
                    independentValues.Add("BCCAddress", bccAddress);
                }
                //Pass MailType and Class Object to the Dll
                mailMessage = MailTemplate.GetMailMessage<T>(id, typeObject, independentValues);
                //mailMessage = MailTemplate.GetMailMessage(mailTemplatePath);
            }
            catch (Exception ex)
            {
                Logger.Log(ex.Message);
            }
            return mailMessage;
        }
        #endregion create mail message

        //public void GetMailMessage(string mailType, string id, object tObject)
        //{
        //    MailMessage mailMessage = GetMailMessage<object>(MAILTEMPLATE_PATH, mailType, id, tObject);
        //}

        #region Get Data For ID
        /// <summary>
        /// Retrieves Data based on the Object Id from the Data Service
        /// </summary>
        /// <param name="objectId">Object Id for which Data needs to be retrieved</param>
        /// <returns>Object containing data based on Object Id </returns>
        public Object GetDataForID(int objectId, string mailType, string hostName)
        {

            try
            {
                //Set the object Id property in the object
                //PropertyInfo propertyInfo = _type.GetProperty(_idProperty, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
                //propertyInfo.SetValue(_currentObject, objectId, null);

                object[] parameters = { objectId };

                // propertyInfo = _type.GetProperty(MailType, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
                //propertyInfo.SetValue(_currentObject, mailType, null);

                PropertyInfo propertyInfo = _type.GetProperty(HostName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
                propertyInfo.SetValue(_currentObject, hostName, null);

                //Invoke GetMethod in the object
                MethodInfo method = _type.GetMethod(_getMethod);
                //Object classObject = method.Invoke(_currentObject, null);
                Object classObject = method.Invoke(_currentObject, parameters);

                return classObject;
            }
            catch (ArgumentNullException argNullEx)
            {
                return argNullEx.ToString();
            }
            catch
            {
                throw;
            }
        }

        #endregion


    }
}
