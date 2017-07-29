using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration.Provider;


namespace Cog.CSM.MailService
{
    /// <summary>
    /// Represents the collection of MailProvider that  inherit from ProviderCollection
    /// </summary>
    /// <remarks>
    /// The MailProviderCollection class utilizes an underlying Hashtable object to store the 
    /// provider name/value pairs 
    ///</remarks>
    public class MailProviderCollection : ProviderCollection
    {
        /// <summary>
        /// Gets the MailProvider with the specified name
        /// </summary>
        /// <param name="name">The key by which the provider is identified.</param>
        public new MailProvider this[string name]
        {
            get { return (MailProvider)base[name]; }
        }

        /// <summary>
        /// Adds a provider to the collection. 
        /// </summary>
        /// <param name="provider">The provider to be added. </param>
        public override void Add(ProviderBase provider)
        {
            if (provider == null)
                throw new ArgumentNullException("provider");

            if (!(provider is MailProvider))
                throw new ArgumentException
                    ("Invalid provider type", "provider");

            base.Add(provider);
        }

    }
}
