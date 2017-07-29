using System;
using System.Data;
using System.IO;
using System.Xml;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Net.Mail;
using System.Web.Configuration;
using System.Configuration.Provider;
using System.Linq;



namespace Cog.CSM.MailService
{
    /// <summary>
    /// Parses a mail template and binds the data to the merge fields.
    /// </summary>
    /// <remarks>
    /// Merge fields are defined in a {{FIELDNAME}} format.  The data source can be a T type object or
    /// a System.Collection.HashTable.
    /// 
    /// </remarks>
    /// <example>
    /// The mail template should be configured as below:
    /// <code>
    /// <![CDATA[
    /// <?xml version="1.0" encoding="utf-8" ?>
    /// <ArrayOfMailTemplateField xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
    /// <MailTemplateField>
    /// <Id>IdKey</Id>
    /// <IsHtml>true|false</IsHtml>
    /// <From>{{MAILFROM}}</From>
    /// <To>{{MAILTO1}},{{MAILTO2}}</To>
    /// <Cc>{{MAILCC1}},{{MAILCC2}}</Cc>
    /// <Bcc>{{MAILBCC1}},{{MAILBCC2}}</Bcc>
    /// <Subject>{{SUBJECT}}</Subject>
    /// <Body></Body>
    /// </MailTemplateField>
    /// ...
    /// </ArrayOfMailTemplateField>
    /// ]]>
    /// </code>
    /// 
    /// The mailConfiguration to be defined in the application config file is as below
    /// 1. Adds a new custom section
    /// <code>
    /// <![CDATA[
    ///  <!--
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
    ///        mailTemplatePath      : The complete file path of the mail template
    ///     -->
    /// 
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
    /// <br/><br/>
    /// This example, create multiple mail template, and send the template mail according to the mail template id.<br/><br/>
    /// 1. Create a mail template:
    ///  <code>
    /// <![CDATA[
    /// <?xml version="1.0" encoding="utf-8" ?>
    /// <ArrayOfMailTemplateField xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
    /// <MailTemplateField>
    ///    <Id>Test1</Id>
    ///    <IsHtml>false</IsHtml>
    ///    <From>Shally.Chong@trw.com</From>
    ///    <To>Shally.Chong@trw.com(Shally Chong)</To>
    ///    <Cc></Cc>
    ///    <Bcc></Bcc>
    ///    <Subject>Please ignore this test mail.</Subject>
    ///    <Body>
    ///        Hello {{Name}},
    ///        This is a body text for test mail.
    ///    </Body>
    /// </MailTemplateField>
    /// 
    /// <MailTemplateField>
    ///    <Id>Test2</Id>
    ///    <IsHtml>false</IsHtml>
    ///    <From>Shally.Chong@trw.com</From>
    ///    <To>Shally.Chong@trw.com(Shally Chong)</To>
    ///    <Cc></Cc>
    ///    <Bcc></Bcc>
    ///    <Subject>This is a test mail subject</Subject>
    ///    <Body>
    ///        Hello {{Name}},
    ///        Please ignore this test mail.
    ///    </Body>
    /// </MailTemplateField>
    /// </ArrayOfMailTemplateField>
    /// ]]>
    /// </code>
    /// 2. Create a console application to use the mail template to send the email:
    ///  <code>
    /// <![CDATA[
    /// static void Main()
    /// {
    ///     // Set the mail server
    ///    string mailServerHost = "gwia.livmi.trw.com";
    ///    // Set the mail template Id
    ///    string id = "Test1";
    ///    // Set the mail template path
    ///    string mailConfigPath = "C:/Documents and Settings/Chongs/My Documents/Visual Studio 2005/Projects/CommonTest/CommonTestConsole/mail.config";
    ///    // Get the customer's details
    ///     List<EntityCustomer> lstCust = GetCustomers();
    ///    // Set the first index of the customer: Id = 1; Name = Mary
    ///    EntityCustomer entityCust = lstCust[0];
    ///    
    ///    MailTemplateParser xmlTemp = new MailTemplateParser();
    ///    xmlTemp.ParseXml(mailConfigPath);
    /// 
    ///    MailTemplateField entity = xmlTemp.GetMailTemplate(id);
    /// 
    ///     if (entity.Id.Length > 0)
    ///    {
    ///        // Set the sender's email
    ///        string emailFrom = entity.From;
    ///        // Set the recipient's email
    ///        string mailTo = entity.To;
    ///        // set the CC's email
    ///        string mailCC = entity.Cc;
    ///        // set the BCC's mail
    ///        string mailBcc = entity.Bcc;
    ///        // set the reply email address
    ///        string replyTo = "";
    ///        // set the subject of the email
    ///        string subject = xmlTemp.DataBind<EntityCustomer>(entityCust, entity.Subject);
    ///        // set the content of the email
    ///        // The variable {{Name}} in mail template will be replaced by Mary
    ///        string body = xmlTemp.DataBind<EntityCustomer>(entityCust, entity.Body);
    /// 
    ///        try
    ///        {
    ///            // send the mail  
    ///            string errorMsg = MailUtilities.SendMail(mailServerHost, emailFrom, mailTo, mailCC, mailBcc, replyTo, subject, body, true);
    ///            
    ///             if (errorMsg.Length > 0)
    ///            {
    ///                Console.WriteLine("The mail was not sent due to error:  " + errorMsg);
    ///            }
    ///            else
    ///            {
    ///                Console.WriteLine("Email sent successfully");
    ///            }
    ///        }
    ///         catch (Exception ex)
    ///        {
    ///            Console.WriteLine("Error:" + ex.Message);
    ///        }
    ///    }
    /// 
    ///        Console.Write("\nPress any key to continue...");
    ///        Console.ReadKey(true);
    /// } 
    /// 
    /// private static List<EntityCustomer> GetCustomers()
    /// {
    ///    // Create sample Customers list in order to demonstrate the GetListById.
    ///    List<EntityCustomer> customers = new List<EntityCustomer>();
    ///    EntityCustomer cust1 = new EntityCustomer();
    ///    EntityCustomer cust2 = new EntityCustomer();
    ///    EntityCustomer cust3 = new EntityCustomer();
    /// 
    ///    // Add item into customers list.
    ///    cust1.Id = 1;
    ///    cust1.Name = "Mary";
    ///    customers.Add(cust1);
    ///  
    ///    cust2.Id = 2;
    ///    cust2.Name = "Andy";
    ///    customers.Add(cust2);
    /// 
    ///    cust3.Id = 3;
    ///    cust3.Name = "Kary";
    ///    customers.Add(cust3);
    /// 
    ///    return customers;
    /// }
    /// 
    /// #region Properties
    /// public class EntityCustomer
    /// {
    ///     private int _id;
    ///    public int Id
    ///    {
    ///        get { return _id; }
    ///        set { _id = value; }
    ///    }
    /// 
    ///    private string _name;
    ///    public string Name
    ///    {
    ///        get { return _name; }
    ///        set { _name = value; }
    ///    }
    /// }
    ///  #endregion
    /// ]]>
    /// </code>
    /// </example>
    public class MailTemplateParser
    {

        private static MailProvider _provider = null;
        private static MailProviderCollection _providers = null;
        private static object _lock = new object();
        private const int _maxChildNode = 8; // to avoid buffer overrun
        private string mailConfigPath = @AppDomain.CurrentDomain.BaseDirectory;
        string[] propertiesOmit = new[] { "Chars", "Length", "Item", "Date", "Now", "UtcNow", "Today" };
        

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the MailTemplateParser class. 
        /// </summary>
        public MailTemplateParser()
        {
        }

        /// <summary>
        /// Initializes a new instance of the MailTemplateParser  class for the specified file name. 
        /// </summary>
        /// <param name="path">The complete file path of the mail template config file to be read. </param>
        public MailTemplateParser(string path)
        {
            ParseXml(path);
        }

        #endregion

        #region Parse
        /// <summary>
        /// Parses the Mail Template configuration based on 
        /// the configuration in the mailConfiguration section of an application config file.
        /// </summary>
        public void Parse()
        {
            LoadProviders();

            if (_provider != null)
            {
                _smtpClient = _provider.Configure();
                string path = mailConfigPath + _provider.MailTemplatePath;
                ParseXml(path);
            }

        }

        /// <summary>
        /// Parses the Mail Template configuration based on 
        /// the configuration in the mailConfiguration section of an application config file.
        /// </summary>
        /// <param name="providerName">Provider Name to which the template should parse.</param>
        public void Parse(string providerName)
        {
            LoadProviders(providerName);

            if (_provider != null)
            {
                _smtpClient = _provider.Configure();
                string path = _provider.MailTemplatePath;
                ParseXml(path);
            }

        }

        /// <summary>
        /// Parses the Mail Template configuration from a config file.
        /// </summary>
        /// <param name="path">The complete file path of the mail template config file to be read.</param>
        public void ParseXml(string path) 
        {
            if (File.Exists(path))
            {
                _mailTemplateList = DeSerializeData(path);

            }
        }

        /// <summary>
        /// Parses the Mail Template configuration from a XML structured string.
        /// </summary>
        /// <param name="xmlString">A System.String that contains XML structure</param>
        ///  <example>
        /// The mail template should be configured as below:
        /// <code>
        /// <![CDATA[
        /// <?xml version="1.0" encoding="utf-8" ?>
        /// <ArrayOfMailTemplateField xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
        /// <MailTemplateField>
        /// <Id>IdKey</Id>
        /// <IsHtml>true|false</IsHtml>
        /// <From>{{MAILFROM}}</From>
        /// <To>{{MAILTO1}},{{MAILTO2}}</To>
        /// <Cc>{{MAILCC1}},{{MAILCC2}}</Cc>
        /// <Bcc>{{MAILBCC1}},{{MAILBCC2}}</Bcc>
        /// <Subject>{{SUBJECT}}</Subject>
        /// <Body></Body>
        /// </MailTemplateField>
        /// ...
        /// </ArrayOfMailTemplateField>
        /// ]]>
        /// </code>
        /// </example>
        public void ParseXmlString(string xmlString)
        {
            if (xmlString.Length > 0 )
            {  
                _mailTemplateList = DeSerializeStringData(xmlString);

            }
        }

        /// <summary>
        /// Parses the Mail Template configuration from a System.Xml.XmlDocument.
        /// </summary>
        /// <param name="xmlDoc">A System.Xml.XmlDocument that contains mail template</param>
        public void ParseXmlDocument(XmlDocument xmlDoc)
        {
            if (xmlDoc != null)
            {
               
                _mailTemplateList = DeSerializeData(xmlDoc);
            }
        }

        #endregion

        #region Deserialization

        /// <summary>
        /// Deserialize object (MailTemplateField) from an XML Document, which read from a XML file.
        /// </summary>
        /// <param name="path">The complete file path of the mail template config file to be read.</param>
        /// <returns>An array of MailTemplateField</returns>
        public List<MailTemplateField> DeSerializeData(string path)
        {
            List<MailTemplateField> dataList = new List<MailTemplateField>();
            System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(dataList.GetType());

            Stream stream = File.OpenRead(path);
            dataList = x.Deserialize(stream) as List<MailTemplateField>;
            stream.Close();
            return dataList;
        }

        /// <summary>
        /// Deserialize object (MailTemplateField) from an XML Document.
        /// </summary>
        /// <param name="xmlDoc">A System.Xml.XmlDocument that contains mail template</param>
        /// <returns>An array of MailTemplateField</returns>
        public List<MailTemplateField> DeSerializeData(XmlDocument xmlDoc)
        {
            List<MailTemplateField> dataList = new List<MailTemplateField>();
           
            MemoryStream ms = new MemoryStream();                       
            xmlDoc.Save(ms);
            string xmlString = System.Text.Encoding.Default.GetString(ms.GetBuffer());
            dataList = DeSerializeStringData(xmlString);
            ms.Close();
            return dataList;
        }

        /// <summary>
        /// Deserialize object (MailTemplateField) from an XML Document.
        /// </summary>
        /// <param name="xmlString">A System.String that contains XML structure</param>
        /// <returns>An array of MailTemplateField</returns>
        public List<MailTemplateField> DeSerializeStringData(string xmlString)
        {
            List<MailTemplateField> dataList = new List<MailTemplateField>();
            System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(dataList.GetType());

            StringReader sr = new StringReader(xmlString);
            dataList = x.Deserialize(sr) as List<MailTemplateField>;
            
            return dataList;
        }

        /// <summary>
        /// Deserialize object (MailTemplateField) from the stream.
        /// </summary>
        /// <param name="stream">A System.IO.Stream that contains XML structure</param>
        /// <returns>An array of MailTemplateField</returns>
        public List<MailTemplateField> DeSerializeStringData(Stream stream)
        {
            List<MailTemplateField> dataList = new List<MailTemplateField>();
            System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(dataList.GetType());
            dataList = x.Deserialize(stream) as List<MailTemplateField>;
            stream.Close();
            return dataList;
        }

        #endregion
        
        #region DataBind

        /// <summary>
        /// Queries the array of MailTemplateField based on the specified Id
        /// </summary>
        /// <param name="id">The specified Id of the MailTemplateField</param>
        /// <returns>A MailTemplateField</returns>
        public MailTemplateField GetMailTemplate(string id)
        {
            List<MailTemplateField> subList = null;
            subList = MailTemplateList.FindAll(delegate(MailTemplateField record)
                                    {
                                        if (record.Id.ToLower().Equals(id.ToLower()))
                                        { return true; }
                                          return false;
                                    });
            MailTemplateField entity = new MailTemplateField();
            if (subList.Count > 0)
            {
                entity = subList[0];
            }

            return entity;

        }

        /// <summary>
        /// Gets all Merge Fields from a raw data.
        /// </summary>
        /// <param name="rawData">A System.String that contains raw data with a Merge Fields</param>
        /// <returns>An array of System.String of all Merge Fields</returns>
        /// <remarks>
        /// The Merge Fields are defined in a format as below:
        /// {{FIELDNAME}}
        /// where FIELDNAME is the Merge Field Name. Merge Fields are fields to be replaced based on the FIELDNAME.
        /// </remarks>
        public List<string> GetMergeFields(string rawData)
        {
            List<string> list = new List<string>();
            string tag = @"{{([^}]*)}}";
            Regex r = new Regex(tag, RegexOptions.Multiline | RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
            Match m = r.Match(rawData);

            if (m.Success)
            {
                 while (m.Success)
                {                    
                    for (int i = 1; i <= 2; i++)
                    {
                        Group g = m.Groups[i];
                        
                        CaptureCollection cc = g.Captures;
                        for (int j = 0; j < cc.Count; j++)
                        {
                            Capture c = cc[j];
                            string key = c.ToString();
                            if (!list.Contains(key))
                            {
                                list.Add(key);
                            }
                        }
                    }
                    m = m.NextMatch();
                }


            }

            return list;
        }
                
        /// <summary>
        ///  Binds a data source to the raw data.
        /// </summary>
        /// <typeparam name="T">T type object</typeparam>
        /// <param name="t">An instance of T type object</param>
        /// <param name="rawData">A System.String that contains raw data with a Merge Fields</param>
        /// <returns>A System.String that contains merged data.</returns>
        private string DoDataBind<T>(T t, string rawData)
        {
            string newData = rawData;

            List<string> list = new List<string>();
            Hashtable ht = new Hashtable();
            list = GetMergeFields(rawData);
            Type objType = t.GetType();


            if (list.Count > 0)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    string fieldName = list[i];

                    // try to map the fields
                    // get field info
                    
                    PropertyInfo pInfo = objType.GetProperty(fieldName,
                    BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                    string fieldValue = "";
                    if (pInfo != null)
                    {
                        // get setter
                        if (pInfo.GetValue(t, null) != null)
                        {
                            fieldValue = pInfo.GetValue(t, null).ToString();
                            if (ht.Contains(fieldName))
                            {
                                ht.Remove(fieldName);

                            }
                        }

                    }

                    if (!ht.Contains(fieldName))
                    {
                        ht.Add(fieldName, fieldValue);
                    }
                }
            }

            if (ht.Count > 0)
            {
                MailParser mp = new MailParser(ht);
                mp.SetTemplate(rawData);
                newData = mp.Parse();
            }

            return newData;
        }

        /// <summary>
        ///  Binds a data source to the raw data.
        /// </summary>
        /// <typeparam name="T">T type object</typeparam>
        /// <param name="t">An instance of T type object</param>
        /// <param name="rawData">A System.String that contains raw data with a Merge Fields</param>
        /// <param name="hashVariable">A System.Collections.HashTable that contains definition of all Merge Fields</param>
        /// <returns>A System.String that contains merged data.</returns>
        
        private string DoDataBind<T>(T t, string rawData, Hashtable hashVariable)
        {
            string newData = rawData;

            List<string> list = new List<string>();
            Hashtable ht = hashVariable;
            list = GetMergeFields(rawData);
            Type objType = t.GetType();


            if (list.Count > 0)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    string fieldName = list[i];                    
                   
                    // try to map the fields
                    // get field info
                    PropertyInfo pInfo = objType.GetProperty(fieldName,
                    BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                    string fieldValue = "";
                    if (pInfo != null)
                    {
                        // get setter
                        if (pInfo.GetValue(t, null) != null)
                        {
                            fieldValue = pInfo.GetValue(t, null).ToString();
                            if (ht.Contains(fieldName))
                            {
                                ht.Remove(fieldName);

                            }
                        }
                        
                    }

                   if (!ht.Contains(fieldName))
                   {
                     ht.Add(fieldName,fieldValue);
                   }
                }
            }

            if (ht.Count > 0)
            {
                MailParser mp = new MailParser(ht);
                mp.SetTemplate(rawData);
                newData = mp.Parse();
            }

            return newData;
        }

        /// <summary>
        /// Binds a data source to the raw data from a T property and its associative properties
        /// </summary>
        /// <typeparam name="T">T type object</typeparam>
        /// <param name="t">An instance of T type object</param>
        /// <param name="hashVariable">A System.Collections.HashTable that contains definition of all Merge Fields</param>
        /// <param name="parentPropertyName">Upper level of associative property of a T type object</param>
        private void DataBind<T>(T t, Hashtable hashVariable, string parentPropertyName)
        {
            Hashtable ht = hashVariable;
            Type objType = t.GetType();

            //int childSize = 1;
            //if (parentPropertyName.Length > 0)
            //{
            //    string[] childNode = Regex.Split(parentPropertyName, @"\.");
            //    childSize = childNode.Length;
            //}

            // get field info
            // loop through 
            foreach (PropertyInfo property in objType.GetProperties())
            {
                if (!propertiesOmit.Any(po => po == property.Name))
                {
                    // get property name
                    string fieldName = property.Name;

                    // get property type
                    Type type = property.PropertyType;
                    // if from TRW namespace, then nested loop the class
                    //if (Regex.IsMatch(type.c, ".", RegexOptions.IgnoreCase))
                    //{
                    //if (childSize <= _maxChildNode)
                    //{
                    //object obj = property.GetValue(t, null);
                    //if (childSize <= _maxChildNode && obj != null)
                    //{
                    //    if (parentPropertyName.Length > 0)
                    //    {
                    //        fieldName = parentPropertyName + "." + fieldName;
                    //    }
                    //    DataBind(obj, hashVariable, fieldName);
                    //}
                    //}

                    //}
                    //else
                    //{
                    try
                    {
                        //string fieldValue = Convert.ToString(property.GetValue(t, null));
                        object fieldValue = property.GetValue(t, null);
                        List<PropertyInfo> propertiesChild = new List<PropertyInfo>();
                        GetChildProperties(fieldValue, propertiesChild);

                        if (parentPropertyName.Length > 0)
                            fieldName = parentPropertyName + "." + fieldName;

                        if (fieldValue != null && (propertiesChild.Count() > 0))
                            DataBind(fieldValue, hashVariable, fieldName);
                        else
                        {
                            if (fieldValue == null)
                                fieldValue = string.Empty;

                            this.GetFieldValue(ht, fieldName, Convert.ToString(fieldValue));

                            if (ht.Contains(fieldName))
                                ht.Remove(fieldName);

                            if (!ht.ContainsKey(fieldName) && fieldValue != null)
                                ht.Add(fieldName, fieldValue);
                        }
                    }

                    catch (TargetParameterCountException e)
                    {
                        // do nothing, just skip this field
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    //}
                }
            }

            hashVariable = ht;

        }

        /// <summary>
        /// Binds a data source to the raw data from a property of an object and its associative properties
        /// </summary>
        /// <param name="objParam">An instance of object</param>
        /// <param name="hashVariable">A System.Collections.HashTable that contains definition of all Merge Fields</param>
        /// <param name="parentPropertyName">Upper level of associative property of a T type object</param>
        private void DataBind(Object objParam, Hashtable hashVariable, string parentPropertyName)
        {
            Hashtable ht = hashVariable;
            int childSize = 1;
            if (parentPropertyName.Length > 0)
            {
                string[] childNode = Regex.Split(parentPropertyName, @"\.");
                childSize = childNode.Length;
            }


            // get field info
            // loop through 
            foreach (PropertyInfo property in objParam.GetType().GetProperties())
            {
                //if (!propertiesOmit.Any(po => po == objParam.GetType().FullName))
                //{
                if (!propertiesOmit.Any(po => po == property.Name))
                {
                    // get property name
                    string fieldName = property.Name;

                    // get property type
                    Type type = property.PropertyType;
                    // if from TRW namespace, then loop the class
                    //if (Regex.IsMatch(type.Namespace, @"^TRW\.", RegexOptions.IgnoreCase))
                    //{
                    //    if (childSize <= _maxChildNode)
                    //    {
                    //        object obj = property.GetValue(objParam, null);
                    //        if (obj != null)
                    //        {
                    //            if (parentPropertyName.Length > 0)
                    //            {
                    //                fieldName = parentPropertyName + "." + fieldName;
                    //            }
                    //            DataBind(obj, hashVariable, fieldName);
                    //        }
                    //    }

                    //}
                    //else
                    //{
                    object fieldValue = property.GetValue(objParam, null);
                    List<PropertyInfo> propertiesChild = new List<PropertyInfo>();
                    GetChildProperties(fieldValue, propertiesChild);

                    if (parentPropertyName.Length > 0)
                        fieldName = parentPropertyName + "." + fieldName;

                    if (fieldValue != null && (propertiesChild.Count() > 0))
                        DataBind(fieldValue, hashVariable, fieldName);
                    else
                    {
                        if (fieldValue == null)
                            fieldValue = string.Empty;

                        this.GetFieldValue(ht, fieldName, Convert.ToString(fieldValue));

                        if (ht.Contains(fieldName))
                            ht.Remove(fieldName);

                        if (!ht.ContainsKey(fieldName))
                            ht.Add(fieldName, fieldValue);
                        //}
                    }
                }
            }

            hashVariable = ht;
        }

        /// <summary>
        ///  Binds a data source to the raw data.
        /// </summary>
        /// <typeparam name="T">T type object</typeparam>
        /// <param name="t">An instance of T type object</param>
        /// <param name="rawData">A System.String that contains raw data with a Merge Fields</param>
        /// <param name="hashVariable">A System.Collections.HashTable that contains definition of all Merge Fields</param>
        /// <returns>A System.String that contains merged data.</returns>
        public string DataBind<T>(T t, string rawData, Hashtable hashVariable)
        {
            string newData = rawData;
            if (hashVariable == null)
            {
                hashVariable = new Hashtable();
            }

            DataBind<T>(t, hashVariable, string.Empty);
            List<string> list = new List<string>();
            Hashtable ht = hashVariable;
            list = GetMergeFields(rawData);

            if (list.Count > 0)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    string fieldName = list[i];
                    if (!ht.ContainsKey(fieldName))
                    {
                        ht.Add(fieldName, string.Empty);
                    }
                }
            }

            if (ht.Count > 0)
            {
                MailParser mp = new MailParser(ht);
                mp.SetTemplate(rawData);
                newData = mp.Parse();
            }

            return newData;
        }

        /// <summary>
        ///  Binds a data source to the raw data.
        /// </summary>
        /// <param name="rawData">A System.String that contains raw data with a Merge Fields</param>
        /// <param name="hashVariable">A System.Collections.HashTable that contains definition of all Merge Fields</param>
        /// <returns>A System.String that contains merged data.</returns>
        public string DataBind(string rawData, Hashtable hashVariable)
        {
            string newData = rawData; 
            List<string> list = new List<string>();
            list = GetMergeFields(rawData);
            Hashtable ht = hashVariable;

            if (list.Count > 0)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    string fieldName = list[i];
                    if (!ht.ContainsKey(fieldName))
                    {
                        ht.Add(fieldName, string.Empty);
                    }
                }
            }

            if (ht.Count > 0)
            {
                MailParser mp = new MailParser(ht);
                mp.SetTemplate(rawData);
                newData = mp.Parse();
            }

            return newData;

        }

        /// <summary>
        ///  Binds a data source to the raw data.
        /// </summary>
        /// <typeparam name="T">T type object</typeparam>
        /// <param name="t">An instance of T type object</param>
        /// <param name="rawData">A System.String that contains raw data with a Merge Fields</param>
        /// <returns>A System.String that contains merged data.</returns>
        public string DataBind<T>(T t, string rawData)
        {
            return DataBind<T>(t, rawData, new Hashtable());
        }

        /// <summary>
        ///  Binds a data source to the raw data.
        /// </summary>
        /// <typeparam name="T">T type object</typeparam>
        /// <param name="rawData">A System.String that contains raw data with a Merge Fields</param>
        /// <param name="hashVariable">A System.Collections.HashTable that contains definition of all Merge Fields</param>
        /// <returns>A System.String that contains merged data.</returns>
        public string DataBind<T>(string rawData, Hashtable hashVariable)
        {
            T t = Activator.CreateInstance<T>();
            return DataBind<T>(t, rawData, hashVariable);
        }

        /// <summary>
        /// Gets the System.Net.Mail.MailMessage from the MailTemplateField. 
        /// </summary>
        /// <typeparam name="T">T type object</typeparam>
        /// <param name="id">The specified Id of the MailTemplateField</param>
        /// <param name="t">An instance of T type object</param>
        /// <param name="hashVariable">A System.Collections.HashTable that contains definition of all Merge Fields</param>
        /// <returns>A System.Net.Mail.MailMessage that contains information about From, To, CC, BCC, Subject, Body and IsBodyHtml</returns>
        public MailMessage GetMailMessage<T>(string id, T t, Hashtable hashVariable)
        {
            MailMessage mail = new MailMessage();  
            MailTemplateField field = this.GetMailTemplate(id);

            if (field != null && field.Id.Length > 0)
            {
                string mailFrom = this.DataBind<T>(t, field.From, hashVariable);
                MailAddress mFrom = new MailAddress(mailFrom);
                mail.From = mFrom;

                if (!string.IsNullOrEmpty(field.To.Trim()))
                {
                    string mailTo = DataBind<T>(t, field.To.Trim(), hashVariable);
                    MailAddressCollection maddrToList = MailUtilities.ConvertToMailAddressCollection(mailTo);
                    foreach (MailAddress maddr in maddrToList)
                    {
                        mail.To.Add(maddr);
                    }
                }
                if (!string.IsNullOrEmpty(field.Cc.Trim()))
                {
                    string mailCc = DataBind<T>(t, field.Cc.Trim(), hashVariable);
                    MailAddressCollection maddrCcList = MailUtilities.ConvertToMailAddressCollection(mailCc);
                    foreach (MailAddress maddr in maddrCcList)
                    {
                        mail.CC.Add(maddr);
                    }
                }
                if (!string.IsNullOrEmpty(field.Bcc.Trim()))
                {
                    string mailBcc =  DataBind<T>(t, field.Bcc.Trim(), hashVariable);
                    MailAddressCollection maddrBccList = MailUtilities.ConvertToMailAddressCollection(mailBcc);
                    foreach (MailAddress maddr in maddrBccList)
                    {
                        mail.Bcc.Add(maddr);
                    }
                }

                if (!string.IsNullOrEmpty(field.ReplyTo.Trim()))
                {
                    string mailReplyTo =  DataBind<T>(t, field.ReplyTo.Trim(), hashVariable);
                    MailAddressCollection maddrReplyToList = MailUtilities.ConvertToMailAddressCollection(mailReplyTo);
                    foreach (MailAddress maddr in maddrReplyToList)
                    {
                        mail.ReplyToList.Add(maddr);
                    }
                }


                if (field.SubjectEncoding != null)
                {
                    mail.SubjectEncoding = field.SubjectEncoding;
                }

                if (field.BodyEncoding != null)
                {
                    mail.BodyEncoding = field.BodyEncoding;
                }

                mail.IsBodyHtml = field.IsHtml;
                mail.Subject = this.DataBind<T>(t, field.Subject, hashVariable);
                mail.Body = this.DataBind<T>(t, field.Body, hashVariable);

            }
            else
            {
                throw new Exception(string.Format("Mail Template <{0}> could not be located!", id));
            }

            return mail;

        }

  
        /// <summary>
        /// Gets the System.Net.Mail.MailMessage from the MailTemplateField. 
        /// </summary>
        /// <typeparam name="T">T type object</typeparam>
        /// <param name="id">The specified Id of the MailTemplateField</param>
        /// <param name="hashVariable">A System.Collections.HashTable that contains definition of all Merge Fields</param>
        /// <returns>A System.Net.Mail.MailMessage that contains information about From, To, CC, BCC, Subject, Body and IsBodyHtml</returns>

        public MailMessage GetMailMessage<T>(Hashtable hashVariable, string id)
        {
            T t = Activator.CreateInstance<T>();
            return GetMailMessage<T>(id, t, hashVariable);
        }

        /// <summary>
        /// Gets the System.Net.Mail.MailMessage from the MailTemplateField. 
        /// </summary>
        /// <typeparam name="T">T type object</typeparam>
        /// <param name="id">The specified Id of the MailTemplateField</param>
        /// <param name="t">An instance of T type object</param>
        /// <returns>A System.Net.Mail.MailMessage that contains information about From, To, CC, BCC, Subject, Body and IsBodyHtml</returns>
 
        public  MailMessage GetMailMessage<T>( T t, string id )
        {
            return GetMailMessage<T>( id, t, new Hashtable());
        }


        /// <summary>
        /// Gets the System.Net.Mail.MailMessage from the MailTemplateField. 
        /// </summary>
        /// <param name="id">The specified Id of the MailTemplateField</param>
        /// <param name="hashVariable">A System.Collections.HashTable that contains definition of all Merge Fields</param>
        /// <returns>A System.Net.Mail.MailMessage that contains information about From, To, CC, BCC, Subject, Body and IsBodyHtml</returns>
        public MailMessage GetMailMessage(Hashtable hashVariable, string id)
        {
            MailMessage mail = new MailMessage();
            MailTemplateField field = this.GetMailTemplate(id);

            if (field != null && field.Id.Length > 0)
            {
                string mailFrom = this.DataBind(field.From, hashVariable);
                MailAddress mFrom = new MailAddress(mailFrom);
                mail.From = mFrom;

                if (!string.IsNullOrEmpty(field.To.Trim()))
                {
                    string mailTo = DataBind(field.To.Trim(), hashVariable);
                    MailAddressCollection maddrToList = MailUtilities.ConvertToMailAddressCollection(mailTo);
                    foreach (MailAddress maddr in maddrToList)
                    {
                        mail.To.Add(maddr);
                    }
                }
                if (!string.IsNullOrEmpty(field.Cc.Trim()))
                {
                    string mailCc = DataBind(field.Cc.Trim(), hashVariable);
                    MailAddressCollection maddrCcList = MailUtilities.ConvertToMailAddressCollection(mailCc);
                    foreach (MailAddress maddr in maddrCcList)
                    {
                        mail.CC.Add(maddr);
                    }
                }
                if (!string.IsNullOrEmpty(field.Bcc.Trim()))
                {
                    string mailBcc = DataBind(field.Bcc.Trim(), hashVariable);
                    MailAddressCollection maddrBccList = MailUtilities.ConvertToMailAddressCollection(mailBcc);
                    foreach (MailAddress maddr in maddrBccList)
                    {
                        mail.Bcc.Add(maddr);
                    }
                }

                if (field.SubjectEncoding != null)
                {
                    mail.SubjectEncoding = field.SubjectEncoding;
                }

                if (field.BodyEncoding != null)
                {
                    mail.BodyEncoding = field.BodyEncoding;
                }

                mail.IsBodyHtml = field.IsHtml;
                mail.Subject = this.DataBind( field.Subject, hashVariable);
                mail.Body = this.DataBind(field.Body, hashVariable);

            }
            else
            {
                throw new Exception(string.Format("Mail Template <{0}> could not be located!", id));
            }

            return mail;

        }

        /// <summary>
        /// Gets the Child Properties from the property List given
        /// </summary>
        /// <param name="fieldValue"></param>
        /// <param name="propertiesChild"></param>
        public void GetChildProperties(object fieldValue, IList<PropertyInfo> propertiesChild)
        {
            if (fieldValue != null)
            {
                foreach (PropertyInfo propertyChild in fieldValue.GetType().GetProperties())
                {
                    DateTime date = DateTime.Now;
                    if (!propertiesOmit.Any(po => po == propertyChild.Name) && !(date.GetType().GetProperties().Any(ps => ps.Name == propertyChild.Name)))
                    {
                        propertiesChild.Add(propertyChild);
                    }
                }
            }
        }

        /// <summary>
        /// Get the comma Separated string for the fieldValue given
        /// </summary>
        /// <param name="ht"></param>
        /// <param name="fieldName"></param>
        /// <param name="fieldValue"></param>
        private void GetFieldValue(Hashtable ht, string fieldName, string fieldValue)
        {
            PropertyInfo[] properties = fieldName.GetType().GetProperties();

            foreach (PropertyInfo propertyHash in properties)
            {
                if (propertyHash.Name == fieldName)
                {
                    string value = Convert.ToString(propertyHash.GetValue(fieldName, null));

                    if (value != fieldValue)
                        fieldValue = fieldValue + "," + value;
                }
            }
        }

        #endregion
  
        #region Provider
        private static void LoadProviders()
        {
            // Avoid claiming lock if providers are already loaded
            if (_provider == null)
            {
                lock (_lock)
                {
                    // Do this again to make sure _provider is still null
                    if (_provider == null)
                    {
                        // Get a reference to the <imageService> section
                        MailTemplateParserSection section = (MailTemplateParserSection)
                            System.Configuration.ConfigurationManager.GetSection("mailConfiguration");

                        // Load registered providers and point _provider
                        // to the default provider
                        _providers = new MailProviderCollection();
                        ProvidersHelper.InstantiateProviders
                            (section.Providers, _providers,
                            typeof(MailProvider));
                        _provider = _providers[section.DefaultProvider];

                        if (_provider == null)
                            throw new ProviderException
                                ("Unable to load default MailProvider");
                    }
                }
            }
        }


        private static void LoadProviders(string providerName)
        {
            // Avoid claiming lock if providers are already loaded
            if (_provider == null)
            {
                lock (_lock)
                {
                    // Do this again to make sure _provider is still null
                    if (_provider == null)
                    {
                        // Get a reference to the <imageService> section
                        MailTemplateParserSection section = (MailTemplateParserSection)
                            System.Configuration.ConfigurationManager.GetSection
                            ("mailConfiguration");

                        // Load registered providers and point _provider
                        // to the default provider
                        _providers = new MailProviderCollection();
                        ProvidersHelper.InstantiateProviders
                            (section.Providers, _providers,
                            typeof(MailProvider));
                        _provider = _providers[providerName];

                        if (_provider == null)
                            throw new ProviderException
                                (string.Format("Unable to load MailProvider:{0}",providerName));
                    }
                }
            }
        }
        #endregion

        #region static methods
        /// <summary>
        /// Gets the System.Net.Mail.MailMessage from a XML MailTemplate structure.  
        /// </summary>
        /// <typeparam name="T">T type object</typeparam>
        /// <param name="xmlTemplateString">A System.String that contains a list of XML MailTemplate</param>
        /// <param name="id">The specified Id of the MailTemplateField</param>
        /// <param name="t">An instance of T type object</param>
        /// <param name="hashVariable">A System.Collections.HashTable that contains definition of all Merge Fields</param>
        /// <returns>A System.Net.Mail.MailMessage that contains information about From, To, CC, BCC, Subject, Body and IsBodyHtml</returns>
        //public MailMessage GetMailMessage(string xmlTemplateString, string id, Hashtable hashVariable)
        public static MailMessage GetMailMessage<T>(string xmlTemplateString, string id, T t, Hashtable hashVariable)
        {
            MailMessage mail = new MailMessage();
            MailTemplateParser mtp = new MailTemplateParser();
            mtp.ParseXmlString(xmlTemplateString);
            MailTemplateField field = mtp.GetMailTemplate(id);

            if (field != null && field.Id.Length > 0)
            {
                string mailFrom = mtp.DataBind<T>(t, field.From, hashVariable);
                MailAddress mFrom = new MailAddress(mailFrom);                
                mail.From = mFrom;
                if (!string.IsNullOrEmpty(field.To.Trim()))
                {
                    string mailTo = mtp.DataBind<T>(t, field.To.Trim(), hashVariable);
                    MailAddressCollection maddrToList = MailUtilities.ConvertToMailAddressCollection(mailTo);
                    foreach (MailAddress maddr in maddrToList)
                    {
                        mail.To.Add(maddr);
                    }
                }
                if (!string.IsNullOrEmpty(field.Cc.Trim()))
                {
                    string mailCc = mtp.DataBind<T>(t, field.Cc.Trim(), hashVariable);
                    MailAddressCollection maddrCcList = MailUtilities.ConvertToMailAddressCollection(mailCc);
                    foreach (MailAddress maddr in maddrCcList)
                    {
                        mail.CC.Add(maddr);
                    }
                }
                if (!string.IsNullOrEmpty(field.Bcc.Trim()))
                {
                    string mailBcc = mtp.DataBind<T>(t, field.Bcc.Trim(), hashVariable);
                    MailAddressCollection maddrBccList = MailUtilities.ConvertToMailAddressCollection(mailBcc);
                    foreach (MailAddress maddr in maddrBccList)
                    {
                        mail.Bcc.Add(maddr);
                    }
                }

                if (field.SubjectEncoding != null)
                {
                    mail.SubjectEncoding = field.SubjectEncoding;
                }

                if (field.BodyEncoding != null)
                {
                    mail.BodyEncoding = field.BodyEncoding;
                }
                mail.IsBodyHtml = field.IsHtml;
                mail.Subject = mtp.DataBind<T>(t, field.Subject, hashVariable);
                mail.Body = mtp.DataBind<T>(t, field.Body, hashVariable);
                
            }
            else
            {
                throw new Exception(string.Format("Mail Template <{0}> could not be located!", id));
            }

            return mail;
            
        }

        /// <summary>
        /// Gets the System.Net.Mail.MailMessage from a XML MailTemplate structure. 
        /// </summary>
        /// <typeparam name="T">T type object</typeparam>
        /// <param name="xmlTemplateString">A System.String that contains a list of XML MailTemplate</param>
        /// <param name="id">The specified Id of the MailTemplateField</param>
        /// <param name="hashVariable">A System.Collections.HashTable that contains definition of all Merge Fields</param>
        /// <returns>A System.Net.Mail.MailMessage that contains information about From, To, CC, BCC, Subject, Body and IsBodyHtml</returns>
 
        public static MailMessage GetMailMessage<T>(string xmlTemplateString, Hashtable hashVariable, string id)
        {
            T t = Activator.CreateInstance<T>();
            return GetMailMessage<T>(xmlTemplateString, id, t,  hashVariable);
        }


      
        /// <summary>
        /// Gets the System.Net.Mail.MailMessage from a XML MailTemplate structure. 
        /// </summary>
        /// <typeparam name="T">T type object</typeparam>
        /// <param name="xmlTemplateString">A System.String that contains a list of XML MailTemplate</param>
        /// <param name="id">The specified Id of the MailTemplateField</param>
        /// <param name="t">An instance of T type object</param>
        /// <returns>A System.Net.Mail.MailMessage that contains information about From, To, CC, BCC, Subject, Body and IsBodyHtml</returns>
 
        public static MailMessage GetMailMessage<T>(string xmlTemplateString, T t, string id )
        {
            return GetMailMessage<T>(xmlTemplateString,id, t,  new Hashtable());
        }

        #endregion

        #region Properties
        private DataSet _mailTemplateDataSet = new DataSet();

        private List<MailTemplateField> _mailTemplateList = new List<MailTemplateField>();
        /// <summary>
        /// Gets and sets an array (list) of MailTemplateField
        /// </summary>
        public List<MailTemplateField> MailTemplateList
        {
            get
            {
                return _mailTemplateList;
            }
            set
            {
                _mailTemplateList = value;
            }
        }

        /// <summary>
        /// Gets a base Provider class for collecting basic SendMail Transfer Protocol (SMTP) information.
        /// </summary>
        public MailProvider Provider
        {
            get
            {
                return _provider;
            }
        }

        /// <summary>
        /// Gets A collection of MailProvider that  inherit from ProviderCollection
        /// </summary>
        public MailProviderCollection Providers
        {
            get
            {
                return _providers;
            }
        }

        private SmtpClient _smtpClient = new SmtpClient();
        /// <summary>
        /// Gets SmtpClient information
        /// </summary>
        /// SmptClient information such as smtpServerHost, smtpPort, 
        /// and SMTP Network Credentials are defined in mailConfiguration section of an Application Config file.
        
        public SmtpClient SmtpClientInfo
        {
            get
            {
                return _smtpClient;
            }
        }


        #endregion


        public MailMessage GetMailMessage(string xmlTemplateString)
        {
            MailMessage mail = new MailMessage();
            MailTemplateParser mtp = new MailTemplateParser();
            mtp.ParseXmlString(xmlTemplateString);
            //MailTemplateField field = mtp.GetMailTemplate(id);
            return mail;
        }
    }
}
