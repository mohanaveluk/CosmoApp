using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Mail;
using System.Collections.Specialized;
using System.Net;
using System.Configuration.Provider;
using Cog.CSM.Entity;

namespace Cog.CSM.MailService
{
    /// <summary>
    /// Represents provider for MailTemplateParser that  inherit from MailProvider
    /// </summary>
    /// <remarks>
    /// The mailConfiguration to be defined in the application config file is as below<br/><br/>
    /// 1. Adds a new custom section
    /// <code>
    /// <![CDATA[ 
    /// <!--
    ///   Custom Config Section.
    ///   This element is useful when adding new config section.    
    /// -->
    /// <configSections>
    ///	...
    ///      <section name="mailConfiguration"     type="TRW.Net.MailTemplateParserSection,TRW.Net"       />
    /// </configSections>
    /// ]]>
    /// </code>
    /// 
    /// 2. Adds the mailConfiguration Section
    /// <code>
    /// <![CDATA[ 
    /// <!--
    ///         mailConfiguration - configure mail settings
    ///         smtpServerHost        : SMTP (SendMail Transfer Protocol) server host name, accept ip and any host name
    ///         smtpPort              : SMTP Port, standard smtp is 25
    ///         requiredAuthentication: Is the connection to SMTP server required any authentication, set true if yes, otherwise set to false
    ///         smtpUser              : Credential user for the SMTP authententication
    ///         smtpPassword          : Credential Password  of the smtpUser
    ///         mailTemplatePath      : The complete file path of the mail template
    ///     -->
    ///   <mailConfiguration defaultProvider="MailTemplateProvider">
    ///     <providers>
    ///       <add name="MailTemplateProvider" type="TRW.Net.MailTemplateProvider,TRW.Net"
    ///            smtpServerHost="gwia.livmi.trw.com"
    ///            smtpPort="25"
    ///            requiredAuthentication="false"
    ///            smtpUser=""
    ///            smtpPassword=""
    ///            mailTemplatePath="DRIVE:\mail.config"
    ///            />
    /// 
    ///     </providers>
    ///   </mailConfiguration>
    /// ]]>
    /// </code>
    /// </remarks>
    public class MailTemplateProvider : MailProvider
    {
        private string _smtpServerHost = "gwia.livmi.trw.com";
        /// <summary>
        /// SMTP server host name including localhost, qualified domain namew or IP address.
        /// </summary>
        /// <remarks>
        /// Default SMTP server is gwia.livmi.trw.com.
        /// </remarks>
        public override string SmtpServerHost
        {
            get
            {
                return _smtpServerHost;
            }
            set
            {
                _smtpServerHost = value;
            }
        }

        private int _smtpPort = 25;
        /// <summary>
        /// SMTP Port is a network port of the SMTP server where the SMTP client will communicate to.         
        /// </summary>
        /// <remarks>Default SMTP port is 25.</remarks>
        public override int SmtpPort
        {
            get
            {
                return _smtpPort;
            }
            set
            {
                _smtpPort = value;
            }
        }

        private bool _requiredAuthentication = false;
        /// <summary>
        /// Gets or Sets flag to indicate if the SMTP communication requires authentication.
        /// </summary>
        /// <remarks>Default setting is false.
        /// true if the SMTP communication requires authentication, otherwise false.
        /// </remarks>
        public override bool RequiredAuthentication
        {
            get
            {
                return _requiredAuthentication;
            }
            set
            {
                _requiredAuthentication = value;
            }
        }

        private bool _smtpEnableSSL = false;
        /// <summary>
        /// Gets or Sets flag to indicate if the SMTP communication requires authentication.
        /// </summary>
        /// <remarks>Default setting is false.
        /// true if the SMTP communication requires authentication, otherwise false.
        /// </remarks>
        public override bool SmtpEnableSsl
        {
            get
            {
                return _smtpEnableSSL;
            }
            set
            {
                _smtpEnableSSL = value;
            }
        }
 
        private string _smtpUser = string.Empty;
        /// <summary>
        /// SMTP login user name. This is an email address use for verification when the SMTP
        /// server requires authentication prior to using any sendmail protocol.
        /// </summary>
        public override string SmtpUser
        {
            get
            {
                return _smtpUser;
            }
            set
            {
                _smtpUser = value;
            }
        }

        private string _smtpPassword = string.Empty;
        /// <summary>
        /// SMTP credential password for the SmtpUser.
        /// </summary>
        public override string SmtpPassword
        {
            get
            {
                return _smtpPassword;
            }
            set
            {
                _smtpPassword = value;
            }
        }


        private string _mailTemplatePath = string.Empty;
        /// <summary>
        /// The complete path of the mail template file. 
        /// </summary>
        public override string MailTemplatePath
        {
            get
            {
                return _mailTemplatePath;
            }
            set
            {
                _mailTemplatePath = value;
            }
        }

        /// <summary>
        /// Initialize application configuration.
        /// </summary>
        /// <param name="name">Provider name. This is a unique name.</param>
        /// <param name="config">Configuration name-value set</param>
        public override void Initialize(string name, NameValueCollection config)
        {
            // Verify that config isn't null
            if (config == null)
                throw new ArgumentNullException("config");

            // Assign the provider a default name if it doesn't have one
            if (String.IsNullOrEmpty(name))
                name = "MailTemplateProvider";

            // Add a default "description" attribute to config if the
            // attribute doesn't exist or is empty
            if (string.IsNullOrEmpty(config["description"]))
            {
                config.Remove("description");
                config.Add("description",
                    "mail template provider");
            }

            // Call the base class's Initialize method
            base.Initialize(name, config);

            // Initialize _applicationName
            if (!string.IsNullOrEmpty(config["smtpServerHost"]))
            {
                _smtpServerHost = config["smtpServerHost"];
            }
            else
            {
                throw new ProviderException
                                  ("Empty or missing smtpServerHost");
            }

            config.Remove("smtpServerHost");

            // Initialize _port
            if (!string.IsNullOrEmpty(config["smtpPort"]))
            {
                Int32.TryParse(config["smtpPort"], out _smtpPort);
            }
            else
            {
                throw new ProviderException
                   ("Empty or missing smtpPort");
            }


            config.Remove("smtpPort");


            // Initialize _requiredAuthentication
            string requiredAuthentication = config["requiredAuthentication"];

            if (!String.IsNullOrEmpty(requiredAuthentication))
            {               
           
                bool isPassed = bool.TryParse(requiredAuthentication, out _requiredAuthentication);
                if (!isPassed)
                {
                    throw new ProviderException
                   ("requiredAuthentication is a boolean. Please set true or false.");
                }
                
            }

            config.Remove("requiredAuthentication");

            // Initialize _requiredAuthentication
            string enableSsl = config["smtpEnableSSL"];

            if (!String.IsNullOrEmpty(enableSsl))
            {

                bool isPassed = bool.TryParse(enableSsl, out _smtpEnableSSL);
                if (!isPassed)
                {
                    throw new ProviderException
                   ("Smtp enableSsl is a boolean. Please set true or false.");
                }

            }

            config.Remove("smtpEnableSSL");

            // Initialize _smtpUser
            _smtpUser = CommonUtility.DecryptString(config["smtpUser"]);

            if (String.IsNullOrEmpty(_smtpUser) &&  _requiredAuthentication)
            {               
           
                    throw new ProviderException
                   ("Empty or missing smtpUser");
            }

            config.Remove("smtpUser");


            // Initialize _smtpPassword
            _smtpPassword = CommonUtility.DecryptString(config["smtpPassword"]);

            if (String.IsNullOrEmpty(_smtpPassword) && _requiredAuthentication)
            {

                throw new ProviderException
               ("Empty or missing smtpPassword");
            }

            config.Remove("smtpPassword");

            // Initialize _mailTemplatePath
            _mailTemplatePath = config["mailTemplatePath"];

            if (String.IsNullOrEmpty(_mailTemplatePath))
            {

                throw new ProviderException
               ("Empty or missing mailTemplatePath");
            }

            config.Remove("mailTemplatePath");


            // Throw an exception if unrecognized attributes remain
            if (config.Count > 0)
            {
                string attr = config.GetKey(0);
                if (!String.IsNullOrEmpty(attr))
                    throw new ProviderException
                        ("Unrecognized attribute: " + attr);
            }

        }



        /// <summary>
        /// Configure SMTP Client based on the provided information.
        /// </summary>
        /// <returns>SmtpClient object</returns>
        public override SmtpClient Configure()
        {
            SmtpClient smtp = new SmtpClient();
            smtp.Host = _smtpServerHost;
            smtp.Port = _smtpPort;
            smtp.EnableSsl = _smtpEnableSSL;
            if (_requiredAuthentication)
            {
                if (!String.IsNullOrEmpty(_smtpUser))
                {
                    smtp.Credentials = new NetworkCredential(_smtpUser, _smtpPassword);
                }
            }
            else
            {
                smtp.UseDefaultCredentials = true;
            }


            return smtp;
        }
    }
}
