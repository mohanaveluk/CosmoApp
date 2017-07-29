using System;
using System.Data;
using System.Configuration;
using System.Xml;
using System.IO;



namespace Cog.CSM.MailService
{    
    /// <summary>
    /// Represents common mail property name for a mail message in a template, 
    /// this includes To, Cc, Bcc, From, Subject, Body and IsHtml. 
    /// </summary>
    /// <remarks>
    /// Each email message template is identified by an Id. 
    /// </remarks>   
    /// <example>
    /// This example, create a sample multiple Mail Template in the same file.
    /// <code>
    /// <![CDATA[
    /// <?xml version="1.0" encoding="utf-8" ?>
    /// <ArrayOfMailTemplateField xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
    /// <MailTemplateField>
    /// <Id>Test1</Id>
    /// <IsHtml>false</IsHtml>
    /// <From>Shally.Chong@trw.com</From>
    /// <To>Shally.Chong@trw.com(Shally Chong)</To>
    /// <Cc></Cc>
    /// <Bcc></Bcc>
    /// <Subject>Please ignore this test mail.</Subject>
    /// <Body>
    ///    Hello {{Name}},
    ///    This is a body text for test mail.
    /// </Body>
    /// </MailTemplateField>
    /// 
    /// <MailTemplateField>
    /// <Id>Test2</Id>
    /// <IsHtml>false</IsHtml>
    /// <From>Shally.Chong@trw.com</From>
    /// <To>Shally.Chong@trw.com(Shally Chong)</To>
    /// <Cc></Cc>
    /// <Bcc></Bcc>
    /// <Subject>This is a test mail subject</Subject>
    /// <Body>
    ///    Hello {{Name}},
    ///    Please ignore this test mail.
    /// </Body>
    /// </MailTemplateField>
    /// </ArrayOfMailTemplateField>
    /// ]]>
    /// </code>
    /// </example>
    [Serializable]
    public class MailTemplateField
    {
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the MailTemplateField class. 
        /// </summary>
        public MailTemplateField()
        {
            Reset();
        }

        #endregion

        #region Properties
        
        private string _id;
        /// <summary>
        /// A unique string that identifies email message in a mail template. 
        /// </summary>
        public string Id
        {
            get { return _id; }
            set { _id = value; }
        }

        private string _from;
        /// <summary>
        /// Sender of the email message.
        /// </summary>
        public string From
        {
            get { return _from; }
            set { _from = value; }
        }

        //private string _to;
        /// <summary>
        /// Recipient of the email message. Optional.
        /// </summary>
        [System.Runtime.Serialization.OptionalField]
        public string To;
        

        //private string _cc;
        /// <summary>
        /// Cc Recipient of the email message. Optional.
        /// </summary>
        [System.Runtime.Serialization.OptionalField]
        public string Cc;
        
        // private string _bcc;
        /// <summary>
        /// Bcc Recipient of the email message. Optional.
        /// </summary>
        [System.Runtime.Serialization.OptionalField]
        public string Bcc;

        // private string _replyTo;
        /// <summary>
        /// Bcc Recipient of the email message. Optional.
        /// </summary>
        [System.Runtime.Serialization.OptionalField]
        public string ReplyTo;

        private string _body;
        /// <summary>
        /// Mail template body with mail merge field {{FieldName}} where FieldName
        /// is the DataBound field name. It is case sensitive
        /// </summary>
        public string Body
        {
            get { return _body; }
            set { _body = value; }
        }

        private string _subject;
        /// <summary>
        /// Mail template subject with mail merge field {{FieldName}} where FieldName
        /// is the DataBound field name. It is case sensitive
        /// </summary>
        public string Subject
        {
            get { return _subject; }
            set { _subject = value; }
        }


        private bool _isHtml = false;
        /// <summary>
        /// Flag to indicate if the email message should be sent in a HTML format
        /// If this flag set to true, the body should be provided in a html syntax
        /// </summary>
        public bool IsHtml
        {
            get { return _isHtml; }
            set { _isHtml = value; }
        }

        /// <summary>
        /// Code Page Name for Text Enconding of Body.  By default is UTF8. Optional.
        /// </summary>
        [System.Runtime.Serialization.OptionalField]
        public string BodyEncodingCodePageName;


        /// <summary>
        /// Text Enconding for Body.  By default is UTF8. Optional.
        /// </summary>
        public System.Text.Encoding BodyEncoding
        {
            get
            {
                return ParseEncoding(BodyEncodingCodePageName);

            }

        }


        /// <summary>
        /// Code Page Name for Text Enconding of Subject.  By default is UTF8. Optional.
        /// </summary>
        [System.Runtime.Serialization.OptionalField]
        public string SubjectEncodingCodePageName;


      
        /// <summary>
        /// Text Enconding for Subject.  By default is UTF8. Optional.
        /// </summary>
        public System.Text.Encoding SubjectEncoding
        {
            get {
                return ParseEncoding(SubjectEncodingCodePageName);
                
            }
            
        }

        private string _attachment;

        /// <summary>
        /// Text find the text in Attachment Tag
        /// </summary>
        public string Attachment 
        { 
            get { return _attachment; }
            set { _attachment = value; } 
        }
        #endregion

        #region Methods
        /// <summary>
        /// Defines the initial value of all properties
        /// </summary>
        public void Reset()
        {
            this.Id = string.Empty;
            this.IsHtml = false;
            this.From = string.Empty;
            this.To = string.Empty;
            this.Cc = string.Empty;
            this.Bcc = string.Empty;
            this.Subject = string.Empty;
            this.Body = string.Empty;
            this.ReplyTo = string.Empty;
        }

        /// <summary>
        /// Returns the encoding associated with the special code page name.
        /// </summary>
        /// <param name="name">The code page name of the preferred encoding. Any value return by the system.Text.Encoding.WebName is a valid input. </param>
        /// <returns></returns>
        private System.Text.Encoding ParseEncoding(string name)
        {
            System.Text.Encoding encoding = System.Text.Encoding.UTF8;
            if (!string.IsNullOrEmpty(name))
            {
                try
                {
                    encoding = System.Text.Encoding.GetEncoding(name);
                }
                finally
                {

                }
            }

            return encoding;
        }
        #endregion

        

    }
}
