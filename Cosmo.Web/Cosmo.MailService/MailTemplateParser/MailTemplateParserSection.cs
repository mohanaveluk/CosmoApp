using System;
using System.Configuration;


namespace Cosmo.MailService
{
    /// <summary>
    /// Configuration Section for MailTemplateParser.
    /// </summary>
    public class MailTemplateParserSection: ConfigurationSection
    {
        /// <summary>
        /// Providers Section Collection.
        /// </summary>
        [ConfigurationProperty("providers")]
        public ProviderSettingsCollection Providers
        {
            get { return (ProviderSettingsCollection)base["providers"]; }
        }

        /// <summary>
        /// Default Provider.
        /// </summary>
        [StringValidator(MinLength = 1)]
        [ConfigurationProperty("defaultProvider",
            DefaultValue = "MailTemplateProvider")]
        public string DefaultProvider
        {
            get { return (string)base["defaultProvider"]; }
            set { base["defaultProvider"] = value; }
        }
    }

}
