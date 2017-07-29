using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Resources;
using System.Text;

namespace Cog.WS.Data
{
    public class ConfigurationUtility
    {
        #region constants

        private const string ErrorMessageWebconfigNullValue = "{0}: is null and cannot be retrived from web.config!";
        private const string ErrorMessageWebconfigEmptyValue = "{0}: is empty and cannot be retrived from web.config!";

        private const string ErrorMessageResourceNotFound = "The resource file for {0} named {1} could not be found!";
        private const string ErrorMessageResourceEmptyValue = "{0}: is empty and cannot be retrived from the resource file for {1}:{2}!";

        #endregion

        #region GetConfigValue public static methods

        /// <summary>
        /// This method gets the value associated with the key
        /// from the Configuration.AppSettings collection. It assumes 
        /// that a value is required and will throw an error if
        /// the key is not found or the value is empty.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetConfigValue(string key)
        {
            return GetConfigValue(ConfigurationManager.AppSettings, key, true);
        }

        /// <summary>
        /// This method returns the value associated with the key 
        /// from the Configuration.AppSettings collection.  If the
        /// key is not found or the value is empty, the given default
        /// value will be returned.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static string GetConfigValue(string key, string defaultValue)
        {
            return GetConfigValue(ConfigurationManager.AppSettings, key, defaultValue);
        }

        /// <summary>
        /// This method returns the value associated with the key
        /// from the Configuration.AppSettings collection.  If the
        /// required boolean is set to true, an error will be thrown
        /// if the key is not found or the value is empty.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="required"></param>
        /// <returns></returns>
        public static string GetConfigValue(string key, bool required)
        {
            return GetConfigValue(ConfigurationManager.AppSettings, key, required);
        }

        /// <summary>
        /// Returns the application settings located in the dll's config file,
        /// i.e. dllName.dll.config.
        /// </summary>
        /// <param name="dllType"></param>
        /// <returns></returns>
        public static NameValueCollection GetDLLSettings(Type dllType)
        {
            string assemblyPath = dllType.Assembly.CodeBase.Replace("file:///", string.Empty);
            Configuration config = ConfigurationManager.OpenExeConfiguration(assemblyPath);
            NameValueCollection appSettings = new NameValueCollection();

            foreach (KeyValueConfigurationElement e in config.AppSettings.Settings)
            {
                appSettings.Add(e.Key, e.Value);
            }

            return appSettings;
        }

        /// <summary>
        /// This method gets the value associated with the key
        /// from the Configuration.AppSettings collection. It assumes 
        /// that a value is required and will throw an error if
        /// the key is not found or the value is empty.
        /// </summary>
        /// <param name="dllType"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetConfigValue(Type dllType, string key)
        {
            return GetConfigValue(GetDLLSettings(dllType), key, true);
        }

        /// <summary>
        /// This method returns the value associated with the key 
        /// from the Configuration.AppSettings collection.  If the
        /// key is not found or the value is empty, the given default
        /// value will be returned.
        /// </summary>
        /// <param name="dllType"></param>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static string GetConfigValue(Type dllType, string key, string defaultValue)
        {
            return GetConfigValue(GetDLLSettings(dllType), key, defaultValue);
        }

        /// <summary>
        /// This method returns the value associated with the key
        /// from the Configuration.AppSettings collection.  If the
        /// required boolean is set to true, an error will be thrown
        /// if the key is not found or the value is empty.
        /// </summary>
        /// <param name="dllType"></param>
        /// <param name="key"></param>
        /// <param name="required"></param>
        /// <returns></returns>
        public static string GetConfigValue(Type dllType, string key, bool required)
        {
            return GetConfigValue(GetDLLSettings(dllType), key, required);
        }

        /// <summary>
        /// This method returns the value associated with the key 
        /// in the given NameValueCollection object. It assumes the
        /// value is required.  If the key can not be found or the 
        /// value is empty, an error will be thrown.
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetConfigValue(NameValueCollection settings, string key)
        {
            return GetConfigValue(settings, key, true);
        }

        /// <summary>
        /// This method returns the value associated with the key 
        /// from the given NameValueCollection object.  If the key
        /// can not be found or the value is empty, it returns
        /// the given default value.
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static string GetConfigValue(NameValueCollection settings, string key, string defaultValue)
        {
            string value = GetConfigValue(settings, key, false);

            if (String.IsNullOrEmpty(value))
            {
                return defaultValue;
            }

            return value;
        }

        /// <summary>
        /// This method gets the value associated with the key from the
        /// given NameValueCollection object.  If the required boolean 
        /// value is true, the method will throw an error if the key
        /// can not be found or the value is empty.
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="key"></param>
        /// <param name="required"></param>
        /// <returns></returns>
        public static string GetConfigValue(NameValueCollection settings, string key, bool required)
        {
            object value = null;
            string setting = string.Empty;

            try
            {
                value = settings[key];

                if (value != null)
                {
                    setting = value.ToString();

                    if (setting.Trim().Length == 0 && required)
                    {
                        throw new Exception(string.Format(ErrorMessageWebconfigEmptyValue, key));
                    }
                }
                else if (required)
                {
                    throw new Exception(string.Format(ErrorMessageWebconfigNullValue, key));
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return setting;
        }

        #endregion

        #region GetIntegerConfigValue public static methods

        /// <summary>
        /// This method gets the integer value associated with the key
        /// from the Configuration.AppSettings collection. It assumes 
        /// that a value is required and will throw an error if
        /// the key is not found or the value is empty.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static int GetIntegerConfigValue(string key)
        {
            return GetIntegerConfigValue(ConfigurationManager.AppSettings, key, true);
        }

        /// <summary>
        /// This method returns the value associated with the key 
        /// from the Configuration.AppSettings collection.  If the
        /// key is not found or the value is empty, the given default
        /// value will be returned.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static int GetIntegerConfigValue(string key, int defaultValue)
        {
            return GetIntegerConfigValue(ConfigurationManager.AppSettings, key, defaultValue);
        }

        /// <summary>
        /// This method returns the value associated with the key
        /// from the Configuration.AppSettings collection.  If the
        /// required boolean is set to true, an error will be thrown
        /// if the key is not found or the value is empty.  If a value
        /// is not required and does not exist, a value of zero is returned
        /// </summary>
        /// <param name="key"></param>
        /// <param name="required"></param>
        /// <returns></returns>
        public static int GetIntegerConfigValue(string key, bool required)
        {
            return GetIntegerConfigValue(ConfigurationManager.AppSettings, key, required);
        }

        /// <summary>
        /// This method returns the value associated with the key 
        /// in the given NameValueCollection object. It assumes the
        /// value is required.  If the key can not be found or the 
        /// value is empty, an error will be thrown.
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static int GetIntegerConfigValue(NameValueCollection settings, string key)
        {
            return GetIntegerConfigValue(settings, key, true);
        }

        /// <summary>
        /// This method returns the value associated with the key 
        /// from the given NameValueCollection object.  If the key
        /// can not be found or the value is empty, it returns
        /// the given default value.
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static int GetIntegerConfigValue(NameValueCollection settings, string key, int defaultValue)
        {
            string value = GetConfigValue(settings, key, false);

            if (String.IsNullOrEmpty(value))
            {
                return defaultValue;
            }

            return Int32.Parse(value);
        }

        /// <summary>
        /// This method gets the value associated with the key from the
        /// given NameValueCollection object.  If the required boolean 
        /// value is true, the method will throw an error if the key
        /// can not be found or the value is empty.  If a value is not
        /// required and is not found, a value of zero is returned
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="key"></param>
        /// <param name="required"></param>
        /// <returns></returns>
        public static int GetIntegerConfigValue(NameValueCollection settings, string key, bool required)
        {
            string value = GetConfigValue(settings, key, required);

            if (String.IsNullOrEmpty(value))
            {
                return 0;
            }

            return Int32.Parse(value);
        }

        #endregion

        #region GetResource public static methods

        /// <summary>
        /// Gets the named string from the named resource file associated with the
        /// assembly the given class type is in.  Assumes that a value is required.
        /// </summary>
        /// <param name="classType">Type of executing class</param>
        /// <param name="resourceName">Name of resource file including namespace path</param>
        /// <param name="stringName">string name in resource file</param>
        /// <returns></returns>
        public static string GetResourceString(Type classType, string resourceName, string stringName)
        {
            return GetResourceString(classType, resourceName, stringName, true);
        }

        /// <summary>
        /// Gets the named string from the named resource file associated with the
        /// assembly the given class type is in.  If a value is not found, the given
        /// default value is used.
        /// </summary>
        /// <param name="classType">Type of executing class</param>
        /// <param name="resourceName">Name of resource file including namespace path</param>
        /// <param name="stringName">string name in resource file</param>
        /// <param name="defaultValue">value that will be returned if the named
        /// string is not found</param>
        /// <returns></returns>
        public static string GetResourceString(Type classType, string resourceName, string stringName, string defaultValue)
        {
            string stringValue = GetResourceString(classType, resourceName, stringName, false);

            if (String.IsNullOrEmpty(stringValue))
            {
                stringValue = defaultValue;
            }

            return stringValue;
        }

        /// <summary>
        /// Retrieves the value associated with the string name in the named
        /// resource file associated with the assembly that contains the given
        /// class type.  The boolean value says whether an error should be
        /// generated if a value is not found.  Returns an empty string if the
        /// value is not found and is not required.
        /// </summary>
        /// <param name="classType">Type of executing class</param>
        /// <param name="resourceName">Name of resource file including namespace path</param>
        /// <param name="stringName">string name in resource file</param>
        /// <param name="required">boolean value signifying if an error should be thrown
        /// if a value is not found</param>
        /// <returns></returns>
        public static string GetResourceString(Type classType, string resourceName, string stringName, bool required)
        {
            string stringValue = string.Empty;

            ResourceManager m = new ResourceManager(resourceName, classType.Assembly);

            if (m != null)
            {
                stringValue = m.GetString(stringName);

                if (required && String.IsNullOrEmpty(stringValue))
                {
                    throw new Exception(string.Format(ErrorMessageResourceEmptyValue,
                                                      stringName,
                                                      classType.ToString(),
                                                      resourceName));
                }
            }

            else
            {
                throw new Exception(string.Format(ErrorMessageResourceNotFound,
                                                  classType.ToString(),
                                                  resourceName));
            }

            return stringValue;

        }

        #endregion

        #region public properties

        /// <summary>
        /// Flag to indicates the appliation is currently on testing mode.
        /// True if the testing mode is on otherwise false.
        /// </summary>
        public static bool IsTestEnvironment
        {
            get
            {
                bool isTest = false;
                string valueText = string.Empty;
                if (ConfigurationManager.AppSettings["IsTestEnvironment"] != null)
                {
                    valueText = ConfigurationManager.AppSettings["IsTestEnvironment"].ToString();
                }
                else if (ConfigurationManager.AppSettings["isTestEnvironment"] != null)
                {
                    valueText = ConfigurationManager.AppSettings["isTestEnvironment"].ToString();
                }
                else if (ConfigurationManager.AppSettings["istestenvironment"] != null)
                {
                    valueText = ConfigurationManager.AppSettings["istestenvironment"].ToString();
                }
                else if (ConfigurationManager.AppSettings["ISTESTENVIRONMENT"] != null)
                {
                    valueText = ConfigurationManager.AppSettings["ISTESTENVIRONMENT"].ToString();
                }

                if (!string.IsNullOrEmpty(valueText))
                {
                    bool valid = bool.TryParse(valueText, out isTest);
                }


                return isTest;

            }


        }


        /// <summary>
        /// Default email recipients (MailTo) in test environment (when the IsTestEnvironment is set to true).
        /// </summary>
        /// <remarks>
        /// The email addresses can be provided in the following format:
        /// a. emailaddress[name], i.e. email address with optional name
        /// b. If more than one email address to be provided, separate two emaill addresses with a comma
        /// </remarks>
        /// <example>
        /// 1. abc@abc.com,abc2@abc.com(abc2 name)
        /// 2. abc@abc.com(abc1 name),abc2@abc.com,abc3@abc.com
        /// </example>
        public static string DefaultTestRecipients
        {
            get
            {
                string valueText = string.Empty;
                if (ConfigurationManager.AppSettings["DefaultTestRecipients"] != null)
                {
                    valueText = ConfigurationManager.AppSettings["DefaultTestRecipients"].ToString();
                }



                return valueText;

            }


        }

        #endregion

        #region private methods

        #endregion
    }

}
