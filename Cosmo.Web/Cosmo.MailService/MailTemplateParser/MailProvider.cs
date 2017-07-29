using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration.Provider;
using System.Net.Mail;

namespace Cosmo.MailService
{
    /// <summary>
    /// Provides a base class for collecting basic SendMail Transfer Protocol (SMTP) information.
    /// </summary>
    public abstract class MailProvider : ProviderBase
    {
        #region Properties
        /// <summary>
        /// SMTP server host name including localhost, qualified domain name or IP address.
        /// </summary>
        public abstract string SmtpServerHost { get; set; }

        /// <summary>
        /// SMTP Port is a network port of the SMTP server where the SMTP client will communicate to. 
        /// Default SMTP port is 25.
        /// </summary>
        public abstract int SmtpPort { get; set; }

        /// <summary>
        /// Flag to indicate if the SMTP communication requires authentication.
        /// </summary>
        public abstract bool RequiredAuthentication { get;set; }

        /// <summary>
        /// Flag to indicate if the SMTP flows over Secure layer.
        /// </summary>
        public abstract bool SmtpEnableSsl { get; set; }

        /// <summary>
        /// SMTP login user name. This is an email address use for verification when the SMTP
        /// server requires authentication prior to using any sendmail protocol.
        /// </summary>
        public abstract string SmtpUser { get; set; }

        /// <summary>
        /// SMTP credential password for the SmtpUser.
        /// </summary>
        public abstract string SmtpPassword { get; set; }

        /// <summary>
        /// Physical path of the mail template file. 
        /// </summary>
        public abstract string MailTemplatePath { get;set;}
        #endregion

        #region Methods

        /// <summary>
        /// Configure SMTP Client based on the provided information.
        /// </summary>
        /// <returns>SmtpClient object</returns>
        public abstract SmtpClient Configure();
        #endregion

    }
}
