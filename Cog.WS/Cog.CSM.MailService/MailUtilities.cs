using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Net;
using System.Configuration;

namespace Cog.CSM.MailService
{
    /// <summary>
	/// Mail utilities for SMTP protocol RFC 2821 (RFC 821)(Send Mail).
    /// </summary>
    public class MailUtilities
    {
        public static int SMTP_PORT = 0;// Convert.ToInt32( ConfigurationManager.AppSettings["smtpServerPort"]);
        public static string SMTP_USERID = "";// ConfigurationManager.AppSettings["UserId"].ToString();
        public static string SMTP_PASSWORD = "";//ConfigurationManager.AppSettings["UserPassword"].ToString();

        /// <summary>
        /// Initializes an instance of MailUtilities class.
        /// </summary>
        public MailUtilities()
        {
        }

        #region SMTP / Send Mail

        #region Non Authentication SendMail
        /// <summary>
        /// Sends the specified message to an SMTP Server to deliver
        /// </summary>
        /// <param name="smtpServer">A System.String that contains the smtp server information.</param>
        /// <param name="fromAddress">A System.String that contains the addresses information of the message sender.</param>
        /// <param name="toAddress">A System.String that contains the addresses information that the message is sent to.</param>
        /// <param name="subject">A System.String that contains the subject line of the message.</param>
        /// <param name="body">A System.String that contains the message body</param>
        /// <returns>A System.String that contacin error message if exception thrown</returns>  
        /// <remarks>
        /// Return empty string if the email sent successfully.
        /// </remarks>
        /// <example>
        /// This example, a Console application, send a test mail and display the message, whether success or error occurred. 
        /// <code>
        /// <![CDATA[
        /// static void Main()
        /// {
        ///    // Set the mail server
        ///    string mailServerHost = "gwia.livmi.trw.com";
        ///    // Set the sender's email
        ///    string emailFrom = "shally.chong@trw.com";
        ///    // set the recipient's email
        ///    string mailTo = "shally.chong@trw.com";
        ///    // set the subject of the email
        ///    string subject = "Test";
        ///    // set the content of the email
        ///    string body = "This is a test email.";
        /// 
        ///    try
        ///    {
        ///         // send the mail 
        ///        string errorMsg = MailUtilities.SendMail(mailServerHost, emailFrom, mailTo, subject, body);
        /// 
        ///        if(errorMsg.Length > 0)
        ///        {
        ///             Console.WriteLine("The mail was not sent due to error:  " + errorMsg);
        ///        }
        ///        else
        ///        {
        ///            Console.WriteLine("Email sent successfully");
        ///        }
        ///    }
        ///    catch (Exception ex)
        ///    {
        ///        Console.WriteLIne("Error:" + ex.Message);
        ///    }
        /// 
        ///    Console.Write("\nPress any key to continue...");
        ///    Console.ReadKey(true);
        /// }
        /// ]]>
        /// </code>
        /// </example>
        public static string SendMail(string smtpServer, string fromAddress, string toAddress, string subject, string body) 
        {
            // declare variables
             string errorMessage = string.Empty;
            
             errorMessage = SendMail(smtpServer, SMTP_PORT, true, SMTP_USERID, SMTP_PASSWORD, fromAddress, toAddress, subject, body);
             return errorMessage;
        }
 
        /// <summary>
        /// Sends the specified message to an SMTP Server to deliver
        /// </summary>
        /// <param name="smtpServer">A System.String that contains the smtp server information.</param>
        /// <param name="fromAddress">A System.String that contains the addresses information of the message sender.</param>
        /// <param name="toAddress">A System.String that contains the addresses information that the message is sent to.</param>
        /// <param name="subject">A System.String that contains the subject line of the message.</param>
        /// <param name="body">A System.String that contains the message body</param>
        /// <param name="isHtml">A System.Boolean that define the message format. Either Html or Text</param>
        /// <returns>A System.String that contacin error message if exception thrown</returns>    
        /// <remarks>
        /// Return empty string if the email sent successfully.
        /// </remarks>
        /// <example>
        /// This example, a Console application, send a test mail and display the message, whether success or error occurred. 
        /// <code>
        /// <![CDATA[
        /// static void Main()
        /// {
        ///    // Set the mail server
        ///    string mailServerHost = "gwia.livmi.trw.com";
        ///    // Set the sender's email
        ///    string emailFrom = "shally.chong@trw.com";
        ///    // set the recipient's email
        ///    string mailTo = "shally.chong@trw.com";
        ///    // set the subject of the email
        ///    string subject = "Test";
        ///    // set the content of the email
        ///    string body = "This is a test email.";
        /// 
        ///    try
        ///    {
        ///         // send the mail 
        ///        string errorMsg = MailUtilities.SendMail(mailServerHost, emailFrom, mailTo, subject, body, false);
        /// 
        ///        if(errorMsg.Length > 0)
        ///        {
        ///             Console.WriteLine("The mail was not sent due to error:  " + errorMsg);
        ///        }
        ///        else
        ///        {
        ///            Console.WriteLine("Email sent successfully");
        ///        }
        ///    }
        ///    catch (Exception ex)
        ///    {
        ///        Console.WriteLIne("Error:" + ex.Message);
        ///    }
        /// 
        ///    Console.Write("\nPress any key to continue...");
        ///    Console.ReadKey(true);
        /// }
        /// ]]>
        /// </code>
        /// </example>
        public static string SendMail(string smtpServer, string fromAddress, string toAddress, string subject, string body,bool isHtml)
        {
            // declare variables
            string errorMessage = string.Empty;
            
            errorMessage = SendMail(smtpServer, fromAddress, toAddress, null, null, null, subject, body, null, MailPriority.Normal, isHtml);
            return errorMessage;
        }
        
        /// <summary>
        /// Sends the specified message to an SMTP Server to deliver
        /// </summary>
        /// <param name="smtpServer">A System.String that contains the smtp server information.</param>
        /// <param name="fromAddress">A System.String that contains the addresses information of the message sender.</param>
        /// <param name="toAddress">A System.String that contains the addresses information that the message is sent to.</param>
        /// <param name="ccAddress">A System.String that contains the addresses information that the message is cc to.</param>
        /// <param name="bccAddress">A System.String that contains the addresses information that the message is bcc to.</param>
        /// <param name="subject">A System.String that contains the subject line of the message.</param>
        /// <param name="body">A System.String that contains the message body</param>
        /// <param name="isHtml">A System.Boolean that define the message format. Either Html or Text</param>
        /// <returns>A System.String that contacin error message if exception thrown</returns>    
        /// <remarks>
        /// Return empty string if the email sent successfully.
        /// </remarks>
        /// <example>
        /// This example, a Console application, send a test mail and display the message, whether success or error occurred. 
        /// <code>
        /// <![CDATA[
        /// static void Main()
        /// {
        ///    // Set the mail server
        ///    string mailServerHost = "gwia.livmi.trw.com";
        ///    // Set the sender's email
        ///    string emailFrom = "shally.chong@trw.com";
        ///    // set the recipient's email
        ///    string mailTo = "shally.chong@trw.com";
        ///    // set the CC's email
        ///    string mailCC = "";   
        ///    // set the BCC's mail
        ///    string mailBcc = "";
        ///    // set the subject of the email
        ///    string subject = "Test";
        ///    // set the content of the email
        ///    string body = "This is a test email.";
        /// 
        ///    try
        ///    {
        ///         // send the mail 
        ///        string errorMsg = MailUtilities.SendMail(mailServerHost, emailFrom, mailTo, mailCC, mailBcc, subject, body, false);
        /// 
        ///        if(errorMsg.Length > 0)
        ///        {
        ///             Console.WriteLine("The mail was not sent due to error:  " + errorMsg);
        ///        }
        ///        else
        ///        {
        ///            Console.WriteLine("Email sent successfully");
        ///        }
        ///    }
        ///    catch (Exception ex)
        ///    {
        ///        Console.WriteLIne("Error:" + ex.Message);
        ///    }
        /// 
        ///    Console.Write("\nPress any key to continue...");
        ///    Console.ReadKey(true);
        /// }
        /// ]]>
        /// </code>
        /// </example>
        public static string SendMail(string smtpServer, string fromAddress, string toAddress, string ccAddress, string bccAddress, string subject, string body, bool isHtml)
        {
            // declare variables
            string errorMessage = string.Empty;
            errorMessage = SendMail(smtpServer, fromAddress, toAddress, ccAddress, bccAddress, null, subject, body, null, MailPriority.Normal, isHtml);
            return errorMessage;
        }
 
        /// <summary>
        /// Sends the specified message to an SMTP Server to deliver
        /// </summary>
        /// <param name="smtpServer">A System.String that contains the smtp server information.</param>
        /// <param name="fromAddress">A System.String that contains the addresses information of the message sender.</param>
        /// <param name="toAddress">A System.String that contains the addresses information that the message is sent to.</param>
        /// <param name="ccAddress">A System.String that contains the addresses information that the message is cc to.</param>
        /// <param name="bccAddress">A System.String that contains the addresses information that the message is bcc to.</param>
        /// <param name="replyToAddress">A System.String that contains the addresses information that the message is reply to.</param>
        /// <param name="subject">A System.String that contains the subject line of the message.</param>
        /// <param name="body">A System.String that contains the message body</param>
        /// <param name="isHtml">A System.Boolean that define the message format. Either Html or Text</param>
        /// <returns>A System.String that contacin error message if exception thrown</returns>        
        /// <remarks>
        /// Return empty string if the email sent successfully.
        /// </remarks>
        /// <example>
        /// This example, a Console application, send a test mail and display the message, whether success or error occurred. 
        /// <code>
        /// <![CDATA[
        /// static void Main()
        /// {
        ///    // Set the mail server
        ///    string mailServerHost = "gwia.livmi.trw.com";
        ///    // Set the sender's email
        ///    string emailFrom = "shally.chong@trw.com";
        ///    // set the recipient's email
        ///    string mailTo = "shally.chong@trw.com";
        ///    // set the CC's email
        ///    string mailCC = "";   
        ///    // set the BCC's mail
        ///    string mailBcc = "";
        ///    // set the reply email address
        ///    string replyTo = "";
        ///    // set the subject of the email
        ///    string subject = "Test";
        ///    // set the content of the email
        ///    string body = "This is a test email.";
        /// 
        ///    try
        ///    {
        ///         // send the mail 
        ///        string errorMsg = MailUtilities.SendMail(mailServerHost, emailFrom, mailTo, mailCC, mailBcc, replyTo, subject, body, false);
        /// 
        ///        if(errorMsg.Length > 0)
        ///        {
        ///             Console.WriteLine("The mail was not sent due to error:  " + errorMsg);
        ///        }
        ///        else
        ///        {
        ///            Console.WriteLine("Email sent successfully");
        ///        }
        ///    }
        ///    catch (Exception ex)
        ///    {
        ///        Console.WriteLIne("Error:" + ex.Message);
        ///    }
        /// 
        ///    Console.Write("\nPress any key to continue...");
        ///    Console.ReadKey(true);
        /// }
        /// ]]>
        /// </code>
        /// </example>
        public static string SendMail(string smtpServer, string fromAddress, string toAddress, string ccAddress, string bccAddress, string replyToAddress, string subject, string body, bool isHtml)
        {
            // declare variables
            string errorMessage = string.Empty;            
            errorMessage = SendMail(smtpServer, fromAddress, toAddress, ccAddress, bccAddress, replyToAddress, subject, body, null, MailPriority.Normal, isHtml);
            return errorMessage;
        }
          
        /// <summary>
        /// Sends the specified message to an SMTP Server to deliver
        /// </summary>
        /// <param name="smtpServer">A System.String that contains the smtp server information.</param>
        /// <param name="fromAddress">A System.String that contains the addresses information of the message sender.</param>
        /// <param name="toAddress">A System.String that contains the addresses information that the message is sent to.</param>
        /// <param name="ccAddress">A System.String that contains the addresses information that the message is cc to.</param>
        /// <param name="bccAddress">A System.String that contains the addresses information that the message is bcc to.</param>
        /// <param name="replyToAddress">A System.String that contains the addresses information that the message is reply to.</param>
        /// <param name="subject">A System.String that contains the subject line of the message.</param>
        /// <param name="body">A System.String that contains the message body</param>
        /// <param name="attachments">A generic list of System.Net.Mail.Attachment</param>
        /// <param name="isHtml">A System.Boolean that define the message format. Either Html or Text</param>
        /// <returns>A System.String that contacin error message if exception thrown</returns>
        /// <remarks>
        /// Return empty string if the email sent successfully.
        /// </remarks>
        /// <example>
        /// This example, a Console application, send a test mail with attachment and display the message, whether success or error occurred. 
        /// <code>
        /// <![CDATA[
        /// static void Main()
        /// {
        ///    // Set the mail server
        ///    string mailServerHost = "gwia.livmi.trw.com";
        ///    // Set the sender's email
        ///    string emailFrom = "shally.chong@trw.com";
        ///    // set the recipient's email
        ///    string mailTo = "shally.chong@trw.com";
        ///    // set the CC's email
        ///    string mailCC = "";   
        ///    // set the BCC's mail
        ///    string mailBcc = "";
        ///    // set the reply email address
        ///    string replyTo = "";
        ///    // set the subject of the email
        ///    string subject = "Test";
        ///    // set the content of the email
        ///    string body = "This is a test email.";
        ///    // set the list of attachment 
        ///    List<Attachment> lst = SetAttachment();
        /// 
        ///    try
        ///    {
        ///         // send the mail 
        ///        string errorMsg = MailUtilities.SendMail(mailServerHost, emailFrom, mailTo, mailCC, mailBcc, replyTo, subject, body, lst, false);
        /// 
        ///        if(errorMsg.Length > 0)
        ///        {
        ///             Console.WriteLine("The mail was not sent due to error:  " + errorMsg);
        ///        }
        ///        else
        ///        {
        ///            Console.WriteLine("Email sent successfully");
        ///        }
        ///    }
        ///    catch (Exception ex)
        ///    {
        ///        Console.WriteLIne("Error:" + ex.Message);
        ///    }
        /// 
        ///    Console.Write("\nPress any key to continue...");
        ///    Console.ReadKey(true);
        /// }
        /// 
        /// private static List<Attachment> SetAttachment()
        /// {
        ///    List<Attachment> lst = new List<Attachment>();
        /// 
        ///     // Specify the file to be attached and sent.
        ///    // This example assumes that a file named data.txt exists in the current working directory.
        ///    string file = "data.txt";
        /// 
        ///    // Create  the file attachment for this e-mail message.
        ///    Attachment data = new Attachment(file, MediaTypeNames.Application.Octet);
        ///    lst.Add(data);
        /// 
        ///    return lst;
        /// }
        /// ]]>
        /// </code>
        /// </example>
        public static string SendMail(string smtpServer, string fromAddress, string toAddress, string ccAddress, string bccAddress, string replyToAddress, string subject, string body, List<Attachment> attachments,  bool isHtml)
        {
            // declare variables
            string errorMessage = string.Empty;
            
            errorMessage = SendMail(smtpServer, fromAddress, toAddress, ccAddress, bccAddress, replyToAddress, subject, body, attachments, MailPriority.Normal, isHtml);
            return errorMessage;
        }
        
        /// <summary>
        /// Sends the specified message to an SMTP Server to deliver
        /// </summary>
        /// <param name="smtpServer">A System.String that contains the smtp server information.</param>
        /// <param name="fromAddress">A System.String that contains the addresses information of the message sender.</param>
        /// <param name="toAddress">A System.String that contains the addresses information that the message is sent to.</param>
        /// <param name="ccAddress">A System.String that contains the addresses information that the message is cc to.</param>
        /// <param name="bccAddress">A System.String that contains the addresses information that the message is bcc to.</param>
        /// <param name="replyToAddress">A System.String that contains the addresses information that the message is reply to.</param>
        /// <param name="subject">A System.String that contains the subject line of the message.</param>
        /// <param name="body">A System.String that contains the message body</param>
        /// <param name="attachments">A generic list of System.Net.Mail.Attachment</param>
        /// <param name="mailPriority">A System.Net.Mail.MailPriority that set the priority of  the message</param>
        /// <param name="isHtml">A System.Boolean that define the message format. Either Html or Text</param>
        /// <returns>A System.String that contacin error message if exception thrown</returns>
        /// <remarks>
        /// Return empty string if the email sent successfully.
        /// </remarks>
        /// <example>
        /// This example, a Console application, send a test mail with attachment and display the message, whether success or error occurred. 
        /// <code>
        /// <![CDATA[
        /// static void Main()
        /// {
        ///    // Set the mail server
        ///    string mailServerHost = "gwia.livmi.trw.com";
        ///    // Set the sender's email
        ///    string emailFrom = "shally.chong@trw.com";
        ///    // set the recipient's email
        ///    string mailTo = "shally.chong@trw.com";
        ///    // set the CC's email
        ///    string mailCC = "";   
        ///    // set the BCC's mail
        ///    string mailBcc = "";
        ///    // set the reply email address
        ///    string replyTo = "";
        ///    // set the subject of the email
        ///    string subject = "Test";
        ///    // set the content of the email
        ///    string body = "This is a test email.";
        ///    // set the list of attachment 
        ///    List<Attachment> lst = SetAttachment();
        /// 
        ///    try
        ///    {
        ///         // send the mail 
        ///        string errorMsg = MailUtilities.SendMail(mailServerHost, emailFrom, mailTo, mailCC, mailBcc, replyTo, subject, body, lst, MailPriority.Normal, false);
        /// 
        ///        if(errorMsg.Length > 0)
        ///        {
        ///             Console.WriteLine("The mail was not sent due to error:  " + errorMsg);
        ///        }
        ///        else
        ///        {
        ///            Console.WriteLine("Email sent successfully");
        ///        }
        ///    }
        ///    catch (Exception ex)
        ///    {
        ///        Console.WriteLIne("Error:" + ex.Message);
        ///    }
        /// 
        ///    Console.Write("\nPress any key to continue...");
        ///    Console.ReadKey(true);
        /// }
        /// 
        /// private static List<Attachment> SetAttachment()
        /// {
        ///    List<Attachment> lst = new List<Attachment>();
        /// 
        ///     // Specify the file to be attached and sent.
        ///    // This example assumes that a file named data.txt exists in the current working directory.
        ///    string file = "data.txt";
        /// 
        ///    // Create  the file attachment for this e-mail message.
        ///    Attachment data = new Attachment(file, MediaTypeNames.Application.Octet);
        ///    lst.Add(data);
        /// 
        ///    return lst;
        /// }
        /// ]]>
        /// </code>
        /// </example>
        public static string SendMail(string smtpServer, string fromAddress, string toAddress,string ccAddress, string bccAddress, string replyToAddress, string subject, string body, List<Attachment> attachments,MailPriority mailPriority, bool isHtml)
        {
            // declare variables
            string errorMessage = string.Empty;
            
            // configure smtp
            SmtpClient smtp = new SmtpClient();
            
            try
            {
                smtp.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                smtp.UseDefaultCredentials = true;
                smtp.Host = smtpServer;
                MailMessage message = new MailMessage();

                // Set Sender
                string[] fromAddrs = Regex.Split(fromAddress, @",");
                for (int i = 0; i < fromAddrs.Length; i++)
                {
                    string fromAddr = fromAddrs[i];
                    if (fromAddr.Trim().Length > 0)
                    {
                        MailAddress mailFrom = new MailAddress(fromAddr);
                        message.From = mailFrom;
                    }
                }

                // Set Recipient
                if (toAddress != null && !String.IsNullOrEmpty(toAddress.Trim()))
                {
                    string[] toAddrs = Regex.Split(toAddress, @",");
                    for (int i = 0; i < toAddrs.Length; i++)
                    {
                        string toAddr = toAddrs[i];
                        if (toAddr.Trim().Length > 0)
                        {
                            MailAddress mailAddr = new MailAddress(toAddr);
                            message.To.Add(mailAddr);

                        }
                    }


                }

                // Set Cc
                if (ccAddress != null && !String.IsNullOrEmpty(ccAddress.Trim()))
                {
                    string[] ccAddrs = Regex.Split(ccAddress, @",");
                    for (int i = 0; i < ccAddrs.Length; i++)
                    {
                        string ccAddr = ccAddrs[i];
                        if (ccAddr.Trim().Length > 0)
                        {
                            MailAddress mailAddr = new MailAddress(ccAddr);
                            message.CC.Add(mailAddr);
                        }
                    }


                }


                // Set BCc
                if (bccAddress != null && !String.IsNullOrEmpty(bccAddress.Trim()))
                {
                    string[] bccAddrs = Regex.Split(bccAddress, @",");

                    for (int i = 0; i < bccAddrs.Length; i++)
                    {
                        string bccAddr = bccAddrs[i];
                        if (bccAddr.Trim().Length > 0)
                        {
                            MailAddress mailAddr = new MailAddress(bccAddr);
                            message.Bcc.Add(mailAddr);

                        }
                    }

                }

                // Set ReplyTo
                if (replyToAddress != null && !String.IsNullOrEmpty(replyToAddress.Trim()))
                {
                    string[] replyToAddrs = Regex.Split(replyToAddress, @",");
                    for (int i = 0; i < replyToAddrs.Length; i++)
                    {
                        string replyToAddr = replyToAddrs[i];
                        if (replyToAddr.Trim().Length > 0)
                        {
                            MailAddress mailreplyTo = new MailAddress(replyToAddr);
                            message.ReplyToList.Add(mailreplyTo);
                        }
                    }
                }

                // Set attachment
                if (attachments != null)
                {
                    for (int i = 0; i < attachments.Count; i++)
                    {
                        Attachment data = attachments[i] as Attachment;
                        message.Attachments.Add(data);
                        
                    }
                }
                // Set subject
                message.Subject = subject;

                // Set body
                message.Body = body;

                // Set Priority
                message.Priority = mailPriority;

                // set isHTML
                message.IsBodyHtml = isHtml;

                smtp.Send(message);
                
            }
            catch (SmtpException smtpEx)
            {
                errorMessage = smtpEx.Message;
            }
            catch (Exception ex)
            {
                errorMessage = ex.ToString();
            }
            finally
            {
                
                smtp = null;
            }

            return errorMessage;
        }
        
        /// <summary>
        /// Sends the specified message to an SMTP Server to deliver
        /// </summary>
        /// <param name="smtpServer">A System.String that contains the smtp server information.</param>
        /// <param name="mailMessage">A System.NET.Mail.MailMessage that contains message to send.</param>
        /// <returns>A System.String that contacin error message if exception thrown</returns>
        /// <remarks>
        /// Return empty string if the email sent successfully.
        /// </remarks>
        /// <example>
        /// This example, a Console application, send a test mail and display the message, whether success or error occurred. 
        /// <code>
        /// <![CDATA[
        /// static void Main()
        /// {
        ///    // Set the mail server
        ///    string mailServerHost = "gwia.livmi.trw.com";
        ///    // Set the details of the email message
        ///    MailMessage mailMsg = new MailMessage("shally.chong@trw.com", "shally.chong@trw.com", "Test", "This is a Test Mail Body");
        /// 
        ///    try
        ///    {
        ///        // send the mail                    
        ///        string errorMsg = MailUtilities.SendMail(mailServerHost, mailMsg);
        /// 
        ///         if (errorMsg.Length > 0)
        ///        {
        ///            Console.WriteLine("The mail was not sent due to error:  " + errorMsg);
        ///        }
        ///        else
        ///        {
        ///            Console.WriteLine("Email sent successfully");
        ///        }
        ///    }
        ///    catch (Exception ex)
        ///    {
        ///        Console.WriteLine("Error:" + ex.Message);
        ///    }
        /// 
        ///    Console.Write("\nPress any key to continue...");
        ///    Console.ReadKey(true);
        /// }
        /// ]]>
        /// </code>
        /// </example>
        public static string SendMail(SmtpClient smtpInfo, MailMessage mailMessage)
        {
            // declare variables
            string errorMessage = string.Empty;

            try
            {
                //errorMessage = SendMail(smtpInfo.Host, smtpInfo.Port, true, SMTP_USERID, SMTP_PASSWORD, mailMessage);
                errorMessage = SendMail(smtpInfo, true, mailMessage);

                return errorMessage;
            }
            catch(Exception ex)
            {
                throw ex;
            }

        }
          
        /// <summary>
        /// Sends the specified message to an SMTP Server to deliver
        /// </summary>
        /// <param name="smtpServer">A System.String that contains the smtp server information.</param>
        /// <param name="mailMessage">A System.NET.Mail.MailMessage that contains message to send.</param>
        /// <param name="attachments">A Generic List of Attachment that to be attached in the message.</param>
        /// <returns>A System.String that contacin error message if exception thrown</returns>
        /// <remarks>
        ///  //Create  the file attachment for this e-mail message.
        ///  Attachment data = new Attachment(file, MediaTypeNames.Application.Octet);
        ///  //Add time stamp information for the file.
        ///  ContentDisposition disposition = data.ContentDisposition;
        ///  disposition.CreationDate = System.IO.File.GetCreationTime(file);
        ///  disposition.ModificationDate = System.IO.File.GetLastWriteTime(file);
        ///  disposition.ReadDate = System.IO.File.GetLastAccessTime(file);
        ///</remarks>
        ///<example>
        /// This example, a Console application, send a test mail with attachment and display the message, whether success or error occurred. 
        /// <code>
        /// <![CDATA[
        /// static void Main()
        /// {
        ///    // Set the mail server
        ///    string mailServerHost = "gwia.livmi.trw.com";
        ///    // Set the details of the email message
        ///    MailMessage mailMsg = new MailMessage("shally.chong@trw.com", "shally.chong@trw.com", "Test", "This is a Test Mail Body");
        ///    // set the list of attachment 
        ///    List<Attachment> lst = SetAttachment();
        /// 
        ///    try
        ///    {
        ///        // send the mail                    
        ///        string errorMsg = MailUtilities.SendMail(mailServerHost, mailMsg, lst);
        /// 
        ///         if (errorMsg.Length > 0)
        ///        {
        ///            Console.WriteLine("The mail was not sent due to error:  " + errorMsg);
        ///        }
        ///        else
        ///        {
        ///            Console.WriteLine("Email sent successfully");
        ///        }
        ///    }
        ///    catch (Exception ex)
        ///    {
        ///        Console.WriteLine("Error:" + ex.Message);
        ///    }
        /// 
        ///    Console.Write("\nPress any key to continue...");
        ///    Console.ReadKey(true);
        /// }
        ///  
        /// private static List<Attachment> SetAttachment()
        /// {
        ///    List<Attachment> lst = new List<Attachment>();
        /// 
        ///     // Specify the file to be attached and sent.
        ///    // This example assumes that a file named data.txt exists in the current working directory.
        ///    string file = "data.txt";
        /// 
        ///    // Create  the file attachment for this e-mail message.
        ///    Attachment data = new Attachment(file, MediaTypeNames.Application.Octet);
        ///    lst.Add(data);
        /// 
        ///    return lst;
        /// }
        /// ]]>
        /// </code>
        /// </example>
        public static string SendMail(string smtpServer, MailMessage mailMessage,List<Attachment> attachments)
        {
            // declare variables
            string errorMessage = string.Empty;
            
            errorMessage = SendMail(smtpServer, 25, false, null, null, mailMessage, attachments);

            return errorMessage;

        }
        #endregion
        
        #region With Authentication SendMail
        /// <summary>
        /// Sends the specified message to an SMTP Server to deliver
        /// </summary>
        /// <param name="smtpServer">A System.String that contains the smtp server information.</param>
        /// <param name="smtpPort">A System.Int32 that contains the smtp port information.</param>
        /// <param name="isRequireAuthentication">A System.Boolean that define the authentication is required for smtp connection</param>
        ///  <param name="mailUserId">A System.String that contains the smtp login userId information.</param>
        /// <param name="mailPassword">A System.String that contains the smtp login password information.</param>
        /// <param name="fromAddress">A System.String that contains the addresses information of the message sender.</param>
        /// <param name="toAddress">A System.String that contains the addresses information that the message is sent to.</param>
        /// <param name="subject">A System.String that contains the subject line of the message.</param>
        /// <param name="body">A System.String that contains the message body</param>
        /// <returns>A System.String that contacin error message if exception thrown</returns>    
        /// <remarks>
        /// Return empty string if the email sent successfully.
        ///</remarks>
        ///<example>
        /// This example, a Console application, send a test mail and display the message, whether success or error occurred. 
        /// <code>
        /// <![CDATA[
        /// static void Main()
        /// {
        ///    // Set the mail server
        ///    string mailServerHost = "gwia.livmi.trw.com";
        ///    // Set the smtp port
        ///    int smtpPort = 25;
        ///    // Set the email Id
        ///    string emailId = "Shally.Chong@trw.com";
        ///    // Set the email password
        ///    string emailPwd = "xxxx";
        ///    // Set the sender's emai
        ///    string emailFrom = "Shally.Chong@trw.com";
        ///    // set the recipient's email
        ///    string mailTo = "Shally.Chong@trw.com";
        ///    // set the subject of the email
        ///    string subject = "Test";
        ///    // set the content of the email
        ///    string body = "This is a test mail.";
        /// 
        ///    try
        ///    {
        ///        // send the mail  
        ///        string errorMsg = MailUtilities.SendMail(mailServerHost, smtpPort, true, emailId, emailPwd, emailFrom, mailTo, subject, body);
        /// 
        ///        if (errorMsg.Length > 0)
        ///        {
        ///            Console.WriteLine("The mail was not sent due to error:  " + errorMsg);
        ///        }
        ///    }
        ///    catch (Exception ex)
        ///    {
        ///        Console.WriteLine("Error:" + ex.Message);
        ///    }
        ///    else
        ///    {
        ///        Console.WriteLine("Email sent successfully");
        ///    }
        /// 
        ///    Console.Write("\nPress any key to continue...");
        ///    Console.ReadKey(true);
        /// }
        /// ]]>
        /// </code>
        /// </example>
        public static string SendMail(string smtpServer, int smtpPort, bool isRequireAuthentication, string mailUserId, string mailPassword, string fromAddress, string toAddress, string subject, string body)
        {
            // declare variables
            string errorMessage = string.Empty;
            

            // configure smtp
            SmtpClient smtp = new SmtpClient();
            try
            {
                smtp.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                smtp.UseDefaultCredentials = true;
                smtp.Host = smtpServer;
                smtp.Port = smtpPort;
                if (isRequireAuthentication)
                {
                    if (!String.IsNullOrEmpty(mailUserId))
                    {
                        smtp.Credentials = new NetworkCredential(mailUserId, mailPassword);
                    }
                }
                else
                {
                    smtp.UseDefaultCredentials = true;
                }
                smtp.Send(fromAddress, toAddress, subject, body);
                
            }
            catch (SmtpException smtpEx)
            {
                errorMessage = smtpEx.Message;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
            finally
            {
                smtp = null;
            }

            return errorMessage;
        }

        /// <summary>
        /// Sends the specified message to an SMTP Server to deliver
        /// </summary>
        /// <param name="smtpServer">A System.String that contains the smtp server information.</param>
        /// <param name="smtpPort">A System.Int32 that contains the smtp port information.</param>
        /// <param name="isRequireAuthentication">A System.Boolean that define the authentication is required for smtp connection</param>
        ///  <param name="mailUserId">A System.String that contains the smtp login userId information.</param>
        /// <param name="mailPassword">A System.String that contains the smtp login password information.</param>
        /// <param name="fromAddress">A System.String that contains the addresses information of the message sender.</param>
        /// <param name="toAddress">A System.String that contains the addresses information that the message is sent to.</param>
        /// <param name="subject">A System.String that contains the subject line of the message.</param>
        /// <param name="body">A System.String that contains the message body</param>
        /// <param name="isHtml">A System.Boolean that define the message format. Either Html or Text</param>
        /// <returns>A System.String that contacin error message if exception thrown</returns>   
        /// <remarks>
        /// Return empty string if the email sent successfully.
        ///</remarks>
        ///<example>
        /// This example, a Console application, send a test mail and display the message, whether success or error occurred. 
        /// <code>
        /// <![CDATA[
        /// static void Main()
        /// {
        ///    // Set the mail server
        ///    string mailServerHost = "gwia.livmi.trw.com";
        ///    // Set the smtp port
        ///    int smtpPort = 25;
        ///    // Set the email Id
        ///    string emailId = "Shally.Chong@trw.com";
        ///    // Set the email password
        ///    string emailPwd = "xxxx";
        ///    // Set the sender's emai
        ///    string emailFrom = "Shally.Chong@trw.com";
        ///    // set the recipient's email
        ///    string mailTo = "Shally.Chong@trw.com";
        ///    // set the subject of the email
        ///    string subject = "Test";
        ///    // set the content of the email
        ///    string body = "This is a test mail.";
        /// 
        ///    try
        ///    {
        ///        // send the mail  
        ///        string errorMsg = MailUtilities.SendMail(mailServerHost, smtpPort, true, emailId, emailPwd, emailFrom, mailTo, subject, body, true);
        /// 
        ///        if (errorMsg.Length > 0)
        ///        {
        ///            Console.WriteLine("The mail was not sent due to error:  " + errorMsg);
        ///        }
        ///    }
        ///    catch (Exception ex)
        ///    {
        ///        Console.WriteLine("Error:" + ex.Message);
        ///    }
        ///    else
        ///    {
        ///        Console.WriteLine("Email sent successfully");
        ///    }
        /// 
        ///    Console.Write("\nPress any key to continue...");
        ///    Console.ReadKey(true);
        /// }
        /// ]]>
        /// </code>
        /// </example>
        public static string SendMail(string smtpServer, int smtpPort, bool isRequireAuthentication, string mailUserId, string mailPassword, string fromAddress, string toAddress, string subject, string body, bool isHtml)
        {
            // declare variables
            string errorMessage = string.Empty;
            
            errorMessage = SendMail(smtpServer, smtpPort, isRequireAuthentication, mailUserId, mailPassword, fromAddress, toAddress, null, null, null, subject, body, null, MailPriority.Normal, isHtml);
            return errorMessage;
        }
        
        /// <summary>
        /// Sends the specified message to an SMTP Server to deliver
        /// </summary>
        /// <param name="smtpServer">A System.String that contains the smtp server information.</param>
        /// <param name="smtpPort">A System.Int32 that contains the smtp port information.</param>
        /// <param name="isRequireAuthentication">A System.Boolean that define the authentication is required for smtp connection</param>
        ///  <param name="mailUserId">A System.String that contains the smtp login userId information.</param>
        /// <param name="mailPassword">A System.String that contains the smtp login password information.</param>
        /// <param name="fromAddress">A System.String that contains the addresses information of the message sender.</param>
        /// <param name="toAddress">A System.String that contains the addresses information that the message is sent to.</param>
        /// <param name="ccAddress">A System.String that contains the addresses information that the message is cc to.</param>
        /// <param name="bccAddress">A System.String that contains the addresses information that the message is bcc to.</param>
        /// <param name="subject">A System.String that contains the subject line of the message.</param>
        /// <param name="body">A System.String that contains the message body</param>
        /// <param name="isHtml">A System.Boolean that define the message format. Either Html or Text</param>
        /// <returns>A System.String that contacin error message if exception thrown</returns>
        /// <remarks>
        /// Return empty string if the email sent successfully.
        ///</remarks>
        ///<example>
        /// This example, a Console application, send a test mail and display the message, whether success or error occurred. 
        /// <code>
        /// <![CDATA[
        /// static void Main()
        /// {
        ///    // Set the mail server
        ///    string mailServerHost = "gwia.livmi.trw.com";
        ///    // Set the smtp port
        ///    int smtpPort = 25;
        ///    // Set the email Id
        ///    string emailId = "Shally.Chong@trw.com";
        ///    // Set the email password
        ///    string emailPwd = "xxxx";
        ///    // Set the sender's emai
        ///    string emailFrom = "Shally.Chong@trw.com";
        ///    // set the recipient's email
        ///    string mailTo = "Shally.Chong@trw.com";
        ///    // set the CC's email
        ///    string mailCC = "shally.chong@trw.com";
        ///    // set the BCC's mail
        ///    string mailBcc = "";
        ///    // set the subject of the email
        ///    string subject = "Test";
        ///    // set the content of the email
        ///    string body = "This is a test mail.";
        /// 
        ///    try
        ///    {
        ///        // send the mail  
        ///        string errorMsg = MailUtilities.SendMail(mailServerHost, smtpPort, true, emailId, emailPwd, emailFrom, mailTo, mailCC, mailBcc, subject, body, true);
        /// 
        ///        if (errorMsg.Length > 0)
        ///        {
        ///            Console.WriteLine("The mail was not sent due to error:  " + errorMsg);
        ///        }
        ///    }
        ///    catch (Exception ex)
        ///    {
        ///        Console.WriteLine("Error:" + ex.Message);
        ///    }
        ///    else
        ///    {
        ///        Console.WriteLine("Email sent successfully");
        ///    }
        /// 
        ///    Console.Write("\nPress any key to continue...");
        ///    Console.ReadKey(true);
        /// }
        /// ]]>
        /// </code>
        /// </example>
        public static string SendMail(string smtpServer, int smtpPort, bool isRequireAuthentication, string mailUserId, string mailPassword, string fromAddress, string toAddress, string ccAddress, string bccAddress, string subject, string body, bool isHtml)
        {
            // declare variables
            string errorMessage = string.Empty;
            
            errorMessage = SendMail(smtpServer, smtpPort, isRequireAuthentication, mailUserId, mailPassword, fromAddress, toAddress, ccAddress, bccAddress, null, subject, body, null, MailPriority.Normal, isHtml);
            return errorMessage;
        }

        /// <summary>
        /// Sends the specified message to an SMTP Server to deliver
        /// </summary>
        /// <param name="smtpServer">A System.String that contains the smtp server information.</param>
        /// <param name="smtpPort">A System.Int32 that contains the smtp port information.</param>
        /// <param name="isRequireAuthentication">A System.Boolean that define the authentication is required for smtp connection</param>
        ///  <param name="mailUserId">A System.String that contains the smtp login userId information.</param>
        /// <param name="mailPassword">A System.String that contains the smtp login password information.</param>
        /// <param name="fromAddress">A System.String that contains the addresses information of the message sender.</param>
        /// <param name="toAddress">A System.String that contains the addresses information that the message is sent to.</param>
        /// <param name="ccAddress">A System.String that contains the addresses information that the message is cc to.</param>
        /// <param name="bccAddress">A System.String that contains the addresses information that the message is bcc to.</param>
        /// <param name="replyToAddress">A System.String that contains the addresses information that the message is reply to.</param>
        /// <param name="subject">A System.String that contains the subject line of the message.</param>
        /// <param name="body">A System.String that contains the message body</param>
        /// <param name="isHtml">A System.Boolean that define the message format. Either Html or Text</param>
        /// <returns>A System.String that contacin error message if exception thrown</returns>
        /// <remarks>
        /// Return empty string if the email sent successfully.
        ///</remarks>
        ///<example>
        /// This example, a Console application, send a test mail and display the message, whether success or error occurred. 
        /// <code>
        /// <![CDATA[
        /// static void Main()
        /// {
        ///    // Set the mail server
        ///    string mailServerHost = "gwia.livmi.trw.com";
        ///    // Set the smtp port
        ///    int smtpPort = 25;
        ///    // Set the email Id
        ///    string emailId = "Shally.Chong@trw.com";
        ///    // Set the email password
        ///    string emailPwd = "xxxx";
        ///    // Set the sender's emai
        ///    string emailFrom = "Shally.Chong@trw.com";
        ///    // set the recipient's email
        ///    string mailTo = "Shally.Chong@trw.com";
        ///    // set the CC's email
        ///    string mailCC = "shally.chong@trw.com";
        ///    // set the BCC's mail
        ///    string mailBcc = "";
        ///    // set the reply email address
        ///    string replyTo = "shally.chong@trw.com";
        ///    // set the subject of the email
        ///    string subject = "Test";
        ///    // set the content of the email
        ///    string body = "This is a test mail.";
        /// 
        ///    try
        ///    {
        ///        // send the mail  
        ///        string errorMsg = MailUtilities.SendMail(mailServerHost, smtpPort, true, emailId, emailPwd, emailFrom, mailTo, mailCC, mailBcc, replyTo, subject, body, true);
        /// 
        ///        if (errorMsg.Length > 0)
        ///        {
        ///            Console.WriteLine("The mail was not sent due to error:  " + errorMsg);
        ///        }
        ///    }
        ///    catch (Exception ex)
        ///    {
        ///        Console.WriteLine("Error:" + ex.Message);
        ///    }
        ///    else
        ///    {
        ///        Console.WriteLine("Email sent successfully");
        ///    }
        /// 
        ///    Console.Write("\nPress any key to continue...");
        ///    Console.ReadKey(true);
        /// }
        /// ]]>
        /// </code>
        /// </example>
        public static string SendMail(string smtpServer, int smtpPort, bool isRequireAuthentication, string mailUserId, string mailPassword, string fromAddress, string toAddress, string ccAddress, string bccAddress, string replyToAddress, string subject, string body, bool isHtml)
        {
            // declare variables
            string errorMessage = string.Empty;
            
            errorMessage = SendMail(smtpServer, smtpPort, isRequireAuthentication, mailUserId, mailPassword, fromAddress, toAddress, ccAddress, bccAddress, replyToAddress, subject, body, null, MailPriority.Normal, isHtml);
            return errorMessage;
        }

        /// <summary>
        /// Sends the specified message to an SMTP Server to deliver
        /// </summary>
        /// <param name="smtpServer">A System.String that contains the smtp server information.</param>
        /// <param name="smtpPort">A System.Int32 that contains the smtp port information.</param>
        /// <param name="isRequireAuthentication">A System.Boolean that define the authentication is required for smtp connection</param>
        ///  <param name="mailUserId">A System.String that contains the smtp login userId information.</param>
        /// <param name="mailPassword">A System.String that contains the smtp login password information.</param>
        /// <param name="fromAddress">A System.String that contains the addresses information of the message sender.</param>
        /// <param name="toAddress">A System.String that contains the addresses information that the message is sent to.</param>
        /// <param name="ccAddress">A System.String that contains the addresses information that the message is cc to.</param>
        /// <param name="bccAddress">A System.String that contains the addresses information that the message is bcc to.</param>
        /// <param name="replyToAddress">A System.String that contains the addresses information that the message is reply to.</param>
        /// <param name="subject">A System.String that contains the subject line of the message.</param>
        /// <param name="body">A System.String that contains the message body</param>
        /// <param name="attachments">A generic list of System.Net.Mail.Attachment</param>
        /// <param name="isHtml">A System.Boolean that define the message format. Either Html or Text</param>
        /// <returns>A System.String that contacin error message if exception thrown</returns>
        /// <remarks>
        /// Return empty string if the email sent successfully.
        ///</remarks>
        ///<example>
        /// This example, a Console application, send a test mail and display the message, whether success or error occurred. 
        /// <code>
        /// <![CDATA[
        /// static void Main()
        /// {
        ///    // Set the mail server
        ///    string mailServerHost = "gwia.livmi.trw.com";
        ///    // Set the smtp port
        ///    int smtpPort = 25;
        ///    // Set the email Id
        ///    string emailId = "Shally.Chong@trw.com";
        ///    // Set the email password
        ///    string emailPwd = "xxxx";
        ///    // Set the sender's emai
        ///    string emailFrom = "Shally.Chong@trw.com";
        ///    // set the recipient's email
        ///    string mailTo = "Shally.Chong@trw.com";
        ///    // set the CC's email
        ///    string mailCC = "shally.chong@trw.com";
        ///    // set the BCC's mail
        ///    string mailBcc = "";
        ///    // set the reply email address
        ///    string replyTo = "shally.chong@trw.com";
        ///    // set the subject of the email
        ///    string subject = "Test";
        ///    // set the content of the email
        ///    string body = "This is a test mail.";
        ///    // set the list of attachment 
        ///    List<Attachment> lst = SetAttachment();
        /// 
        ///    try
        ///    {
        ///        // send the mail  
        ///        string errorMsg =  MailUtilities.SendMail(mailServerHost, smtpPort, true, emailId, emailPwd, emailFrom, mailTo, mailCC, mailBcc, replyTo, subject, body, lst, true);
        /// 
        ///        if (errorMsg.Length > 0)
        ///        {
        ///            Console.WriteLine("The mail was not sent due to error:  " + errorMsg);
        ///        }
        ///    }
        ///    catch (Exception ex)
        ///    {
        ///        Console.WriteLine("Error:" + ex.Message);
        ///    }
        ///    else
        ///    {
        ///        Console.WriteLine("Email sent successfully");
        ///    }
        /// 
        ///    Console.Write("\nPress any key to continue...");
        ///    Console.ReadKey(true);
        /// }
        /// 
        ///  private static List<Attachment> SetAttachment()
        /// {
        ///    List<Attachment> lst = new List<Attachment>();
        /// 
        ///     // Specify the file to be attached and sent.
        ///    // This example assumes that a file named data.txt exists in the current working directory.
        ///    string file = "data.txt";
        /// 
        ///    // Create  the file attachment for this e-mail message.
        ///    Attachment data = new Attachment(file, MediaTypeNames.Application.Octet);
        ///    lst.Add(data);
        /// 
        ///    return lst;
        /// }
        /// ]]>
        /// </code>
        /// </example>
        public static string SendMail(string smtpServer, int smtpPort, bool isRequireAuthentication, string mailUserId, string mailPassword, string fromAddress, string toAddress, string ccAddress, string bccAddress, string replyToAddress, string subject, string body, List<Attachment> attachments, bool isHtml)
        {
            // declare variables
            string errorMessage = string.Empty;
            

            errorMessage = SendMail(smtpServer, smtpPort, isRequireAuthentication, mailUserId, mailPassword, fromAddress, toAddress, ccAddress, bccAddress, replyToAddress, subject, body, attachments, MailPriority.Normal, isHtml);
            return errorMessage;
        }

       /// <summary>
        /// Sends the specified message to an SMTP Server to deliver
        /// </summary>
        /// <param name="smtpServer">A System.String that contains the smtp server information.</param>
        /// <param name="smtpPort">A System.Int32 that contains the smtp port information.</param>
        /// <param name="isRequireAuthentication">A System.Boolean that define the authentication is required for smtp connection</param>
        ///  <param name="mailUserId">A System.String that contains the smtp login userId information.</param>
        /// <param name="mailPassword">A System.String that contains the smtp login password information.</param>
        /// <param name="fromAddress">A System.String that contains the addresses information of the message sender.</param>
        /// <param name="toAddress">A System.String that contains the addresses information that the message is sent to.</param>
        /// <param name="ccAddress">A System.String that contains the addresses information that the message is cc to.</param>
        /// <param name="bccAddress">A System.String that contains the addresses information that the message is bcc to.</param>
        /// <param name="replyToAddress">A System.String that contains the addresses information that the message is reply to.</param>
        /// <param name="subject">A System.String that contains the subject line of the message.</param>
        /// <param name="body">A System.String that contains the message body</param>
        /// <param name="attachments">A generic list of System.Net.Mail.Attachment</param>
        /// <param name="mailPriority">A System.Net.Mail.MailPriority that set the priority of  the message</param>
        /// <param name="isHtml">A System.Boolean that define the message format. Either Html or Text</param>
        /// <returns>A System.String that contacin error message if exception thrown </returns>
        /// <remarks>
        /// Return empty string if the email sent successfully.
        ///</remarks>
        ///<example>
        /// This example, a Console application, send a test mail and display the message, whether success or error occurred. 
        /// <code>
        /// <![CDATA[
        /// static void Main()
        /// {
        ///    // Set the mail server
        ///    string mailServerHost = "gwia.livmi.trw.com";
        ///    // Set the smtp port
        ///    int smtpPort = 25;
        ///    // Set the email Id
        ///    string emailId = "Shally.Chong@trw.com";
        ///    // Set the email password
        ///    string emailPwd = "xxxx";
        ///    // Set the sender's emai
        ///    string emailFrom = "Shally.Chong@trw.com";
        ///    // set the recipient's email
        ///    string mailTo = "Shally.Chong@trw.com";
        ///    // set the CC's email
        ///    string mailCC = "shally.chong@trw.com";
        ///    // set the BCC's mail
        ///    string mailBcc = "";
        ///    // set the reply email address
        ///    string replyTo = "shally.chong@trw.com";
        ///    // set the subject of the email
        ///    string subject = "Test";
        ///    // set the content of the email
        ///    string body = "This is a test mail.";
        ///    // set the list of attachment 
        ///    List<Attachment> lst = SetAttachment();
        /// 
        ///    try
        ///    {
        ///        // send the mail  
        ///        string errorMsg =  MailUtilities.SendMail(mailServerHost, smtpPort, true, emailId, emailPwd, emailFrom, mailTo, mailCC, mailBcc, replyTo, subject, body, lst, MailPriority.High, true);
        /// 
        ///        if (errorMsg.Length > 0)
        ///        {
        ///            Console.WriteLine("The mail was not sent due to error:  " + errorMsg);
        ///        }
        ///    }
        ///    catch (Exception ex)
        ///    {
        ///        Console.WriteLine("Error:" + ex.Message);
        ///    }
        ///    else
        ///    {
        ///        Console.WriteLine("Email sent successfully");
        ///    }
        /// 
        ///    Console.Write("\nPress any key to continue...");
        ///    Console.ReadKey(true);
        /// }
        /// 
        ///  private static List<Attachment> SetAttachment()
        /// {
        ///    List<Attachment> lst = new List<Attachment>();
        /// 
        ///     // Specify the file to be attached and sent.
        ///    // This example assumes that a file named data.txt exists in the current working directory.
        ///    string file = "data.txt";
        /// 
        ///    // Create  the file attachment for this e-mail message.
        ///    Attachment data = new Attachment(file, MediaTypeNames.Application.Octet);
        ///    lst.Add(data);
        /// 
        ///    return lst;
        /// }
        /// ]]>
        /// </code>
        /// </example>
        public static string SendMail(string smtpServer, int smtpPort, bool isRequireAuthentication, string mailUserId, string mailPassword, string fromAddress, string toAddress, string ccAddress, string bccAddress, string replyToAddress, string subject, string body, List<Attachment> attachments, MailPriority mailPriority, bool isHtml)
        {
            // declare variables
            string errorMessage = string.Empty;
            
            // configure smtp
            SmtpClient smtp = new SmtpClient();
            Attachment data;
            try
            {
                smtp.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;                
                smtp.Host = smtpServer;
                smtp.Port = smtpPort;
                if (isRequireAuthentication)
                {
                    if (!String.IsNullOrEmpty(mailUserId))
                    {
                        smtp.Credentials = new NetworkCredential(mailUserId, mailPassword);
                    }
                }
                else
                {
                    smtp.UseDefaultCredentials = true;
                }

                MailMessage message = new MailMessage();

                // Set Sender
                string[] fromAddrs = Regex.Split(fromAddress, @",");
                for (int i = 0; i < fromAddrs.Length; i++)
                {
                    string fromAddr = fromAddrs[i];
                    if (fromAddr.Trim().Length > 0)
                    {
                        MailAddress mailFrom = new MailAddress(fromAddr);
                        message.From = mailFrom;
                    }
                }

                // Set Recipient
                if (toAddress != null && !String.IsNullOrEmpty(toAddress.Trim()))
                {
                    string[] toAddrs = Regex.Split(toAddress, @",");                   
                    for (int i = 0; i < toAddrs.Length; i++)
                    {
                        string toAddr = toAddrs[i];
                        if (toAddr.Trim().Length > 0)
                        {
                            MailAddress mailAddr = new MailAddress(toAddr);
                            message.To.Add(mailAddr);

                        }
                    }
                   

                }

                // Set Cc
                if (ccAddress != null && !String.IsNullOrEmpty(ccAddress.Trim()))
                {
                    string[] ccAddrs = Regex.Split(ccAddress, @",");                    
                    for (int i = 0; i < ccAddrs.Length; i++)
                    {
                        string ccAddr = ccAddrs[i];
                        if (ccAddr.Trim().Length > 0)
                        {
                            MailAddress mailAddr = new MailAddress(ccAddr);
                            message.CC.Add(mailAddr);
                        }
                    }
                   

                }


                // Set BCc
                if (bccAddress != null && !String.IsNullOrEmpty(bccAddress.Trim()))
                {
                    string[] bccAddrs = Regex.Split(bccAddress, @",");
                    
                    for (int i = 0; i < bccAddrs.Length; i++)
                    {
                        string bccAddr = bccAddrs[i];
                        if (bccAddr.Trim().Length > 0)
                        {
                            MailAddress mailAddr = new MailAddress(bccAddr);
                            message.Bcc.Add(mailAddr);

                        }
                    }
     
                }

 

                // Set ReplyTo
                if (replyToAddress != null && !String.IsNullOrEmpty(replyToAddress.Trim()))
                {
                    string[] replyToAddrs = Regex.Split(replyToAddress, @",");
                    for (int i = 0; i < replyToAddrs.Length; i++)
                    {
                        string replyToAddr = replyToAddrs[i];
                        if (replyToAddr.Trim().Length > 0)
                        {
                            MailAddress mailreplyTo = new MailAddress(replyToAddr);
                            message.ReplyToList.Add(mailreplyTo);
                        }
                    }
                }

                // Set attachment
                if (attachments != null)
                {
                    for (int i = 0; i < attachments.Count; i++)
                    {
                        data = attachments[i] as Attachment;
                        message.Attachments.Add(data);

                    }
                }

                // Set subject
                message.Subject = subject;

                // Set body
                message.Body = body;

                // Set Priority
                message.Priority = mailPriority;

                // set isHTML
                message.IsBodyHtml = isHtml;


                smtp.Send(message);
                
            }
            catch (SmtpException smtpEx)
            {
                errorMessage = smtpEx.Message;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
            finally
            {

                
                smtp = null;
            }

            return errorMessage;
        }

        /// <summary>
        /// Sends the specified message to an SMTP Server to deliver
        /// </summary>
        /// <param name="smtpServer">A System.String that contains the smtp server information.</param>
        /// <param name="smtpPort">A System.Int32 that contains the smtp port information.</param>
        /// <param name="isSecureConnection">A System.Boolean that define the secure connection is required for smtp connection</param>
        /// <param name="isRequireAuthentication">A System.Boolean that define the authentication is required for smtp connection</param>
        ///  <param name="mailUserId">A System.String that contains the smtp login userId information.</param>
        /// <param name="mailPassword">A System.String that contains the smtp login password information.</param>
        /// <param name="fromAddress">A System.String that contains the addresses information of the message sender.</param>
        /// <param name="toAddress">A System.String that contains the addresses information that the message is sent to.</param>
        /// <param name="ccAddress">A System.String that contains the addresses information that the message is cc to.</param>
        /// <param name="bccAddress">A System.String that contains the addresses information that the message is bcc to.</param>
        /// <param name="replyToAddress">A System.String that contains the addresses information that the message is reply to.</param>
        /// <param name="subject">A System.String that contains the subject line of the message.</param>
        /// <param name="body">A System.String that contains the message body</param>
        /// <param name="attachments">A generic list of System.Net.Mail.Attachment</param>
        /// <param name="mailPriority">A System.Net.Mail.MailPriority that set the priority of  the message</param>
        /// <param name="isHtml">A System.Boolean that define the message format. Either Html or Text</param>
        /// <returns>A System.String that contacin error message if exception thrown</returns>
        /// <remarks>
        /// Return empty string if the email sent successfully.
        ///</remarks>
        ///<example>
        /// This example, a Console application, send a test mail and display the message, whether success or error occurred. 
        /// <code>
        /// <![CDATA[
        /// static void Main()
        /// {
        ///    // Set the mail server
        ///    string mailServerHost = "smtp.gmail.com";
        ///    // Set the smtp port
        ///    int smtpPort = 587;
        ///    // Set the email Id
        ///    string emailId = "Shally.Chong@gmail.com";
        ///    // Set the email password
        ///    string emailPwd = "xxxx";
        ///    // Set the sender's emai
        ///    string emailFrom = "Shally.Chong@gmail.com";
        ///    // set the recipient's email
        ///    string mailTo = "Shally.Chong@gmail.com";
        ///    // set the CC's email
        ///    string mailCC = "shally.chong@trw.com";
        ///    // set the BCC's mail
        ///    string mailBcc = "";
        ///    // set the reply email address
        ///    string replyTo = "shally.chong@gmail.com";
        ///    // set the subject of the email
        ///    string subject = "Test";
        ///    // set the content of the email
        ///    string body = "This is a test mail.";
        ///    // set the list of attachment 
        ///    List<Attachment> lst = SetAttachment();
        /// 
        ///    try
        ///    {
        ///        // send the mail  
        ///        string errorMsg =  
        ///            MailUtilities.SendMail(mailServerHost, smtpPort, true, true, emailId, emailPwd, emailFrom, mailTo, mailCC, mailBcc, replyTo, subject, body, lst, MailPriority.High, true);
        /// 
        ///        if (errorMsg.Length > 0)
        ///        {
        ///            Console.WriteLine("The mail was not sent due to error:  " + errorMsg);
        ///        }
        ///    }
        ///    catch (Exception ex)
        ///    {
        ///        Console.WriteLine("Error:" + ex.Message);
        ///    }
        ///    else
        ///    {
        ///        Console.WriteLine("Email sent successfully");
        ///    }
        /// 
        ///    Console.Write("\nPress any key to continue...");
        ///    Console.ReadKey(true);
        /// }
        /// 
        ///  private static List<Attachment> SetAttachment()
        /// {
        ///    List<Attachment> lst = new List<Attachment>();
        /// 
        ///     // Specify the file to be attached and sent.
        ///    // This example assumes that a file named data.txt exists in the current working directory.
        ///    string file = "data.txt";
        /// 
        ///    // Create  the file attachment for this e-mail message.
        ///    Attachment data = new Attachment(file, MediaTypeNames.Application.Octet);
        ///    lst.Add(data);
        /// 
        ///    return lst;
        /// }
        /// ]]>
        /// </code>
        /// </example>
        public static string SendMail(string smtpServer, int smtpPort,bool isSecureConnection, bool isRequireAuthentication, string mailUserId, string mailPassword, string fromAddress, string toAddress, string ccAddress, string bccAddress, string replyToAddress, string subject, string body, List<Attachment> attachments, MailPriority mailPriority, bool isHtml)
        {
            // declare variables
            string errorMessage = string.Empty;
            
            // configure smtp
            SmtpClient smtp = new SmtpClient();
            Attachment data;
            try
            {
                smtp.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                smtp.EnableSsl = isSecureConnection;
                smtp.Host = smtpServer;
                smtp.Port = smtpPort;
                if (isRequireAuthentication)
                {
                    if (!String.IsNullOrEmpty(mailUserId))
                    {
                        smtp.Credentials = new NetworkCredential(mailUserId, mailPassword);
                    }
                }
                else
                {
                    smtp.UseDefaultCredentials = true;
                }

                MailMessage message = new MailMessage();

                // Set Sender
                string[] fromAddrs = Regex.Split(fromAddress, @",");
                for (int i = 0; i < fromAddrs.Length; i++)
                {
                    string fromAddr = fromAddrs[i];
                    if (fromAddr.Trim().Length > 0)
                    {
                        MailAddress mailFrom = new MailAddress(fromAddr);
                        message.From = mailFrom;
                    }
                }

                // Set Recipient
                if (toAddress != null && !String.IsNullOrEmpty(toAddress.Trim()))
                {
                    string[] toAddrs = Regex.Split(toAddress, @",");
                    for (int i = 0; i < toAddrs.Length; i++)
                    {
                        string toAddr = toAddrs[i];
                        if (toAddr.Trim().Length > 0)
                        {
                            MailAddress mailAddr = new MailAddress(toAddr);
                            message.To.Add(mailAddr);

                        }
                    }


                }

                // Set Cc
                if (ccAddress != null && !String.IsNullOrEmpty(ccAddress.Trim()))
                {
                    string[] ccAddrs = Regex.Split(ccAddress, @",");
                    for (int i = 0; i < ccAddrs.Length; i++)
                    {
                        string ccAddr = ccAddrs[i];
                        if (ccAddr.Trim().Length > 0)
                        {
                            MailAddress mailAddr = new MailAddress(ccAddr);
                            message.CC.Add(mailAddr);
                        }
                    }


                }


                // Set BCc
                if (bccAddress != null && !String.IsNullOrEmpty(bccAddress.Trim()))
                {
                    string[] bccAddrs = Regex.Split(bccAddress, @",");

                    for (int i = 0; i < bccAddrs.Length; i++)
                    {
                        string bccAddr = bccAddrs[i];
                        if (bccAddr.Trim().Length > 0)
                        {
                            MailAddress mailAddr = new MailAddress(bccAddr);
                            message.Bcc.Add(mailAddr);

                        }
                    }

                }



                // Set ReplyTo
                if (replyToAddress != null && !String.IsNullOrEmpty(replyToAddress.Trim()))
                {
                    string[] replyToAddrs = Regex.Split(replyToAddress, @",");
                    for (int i = 0; i < replyToAddrs.Length; i++)
                    {
                        string replyToAddr = replyToAddrs[i];
                        if (replyToAddr.Trim().Length > 0)
                        {
                            MailAddress mailreplyTo = new MailAddress(replyToAddr);
                            message.ReplyToList.Add(mailreplyTo);
                        }
                    }
                }

                // Set attachment
                if (attachments != null)
                {
                    for (int i = 0; i < attachments.Count; i++)
                    {
                        data = attachments[i] as Attachment;
                        message.Attachments.Add(data);

                    }
                }

                // Set subject
                message.Subject = subject;

                // Set body
                message.Body = body;

                // Set Priority
                message.Priority = mailPriority;

                // set isHTML
                message.IsBodyHtml = isHtml;


                smtp.Send(message);
                
            }
            catch (SmtpException smtpEx)
            {
                errorMessage = smtpEx.Message;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
            finally
            {


                smtp = null;
            }

            return errorMessage;
        }
        
        /// <summary>
        /// Sends the specified message to an SMTP Server to deliver
        /// </summary>
        /// <param name="smtpServer">A System.String that contains the smtp server information.</param>
        /// <param name="smtpPort">A System.Int32 that contains the smtp port information.</param>
        /// <param name="isRequireAuthentication">A System.Boolean that define the authentication is required for smtp connection</param>
        /// <param name="mailUserId">A System.String that contains the smtp login userId information.</param>
        /// <param name="mailPassword">A System.String that contains the smtp login password information.</param>
        /// <param name="mailMessage">A System.NET.Mail.MailMessage that contains message to send.</param>
        /// <returns>A System.String that contacin error message if exception thrown.</returns>
        /// <remarks>
        /// Return empty string if the email sent successfully.
        ///</remarks>
        ///<example>
        /// This example, a Console application, send a test mail and display the message, whether success or error occurred. 
        /// <code>
        /// <![CDATA[
        /// static void Main()
        /// {
        ///    // Set the mail server
        ///   string mailServerHost = "gwia.livmi.trw.com";
        ///    // Set the smtp port
        ///    int smtpPort = 25;
        ///    // Set the email Id
        ///    string emailId = "Shally.Chong@trw.com";
        ///    // Set the email password
        ///    string emailPwd = "xxxx";
        ///    // set the details of the email message
        ///    MailMessage mailMsg = new MailMessage("shally.chong@trw.com", "shally.chong@trw.com", "Test", "This is a Test Mail Body");
        /// 
        ///    try
        ///    {
        ///        // send the mail  
        ///        string errorMsg = MailUtilities.SendMail(mailServerHost, smtpPort, true, emailId, emailPwd, mailMsg);
        /// 
        ///        if (errorMsg.Length > 0)
        ///        {
        ///            Console.WriteLine("The mail was not sent due to error:  " + errorMsg);
        ///        }
        ///    }
        ///    catch (Exception ex)
        ///    {
        ///        Console.WriteLine("Error:" + ex.Message);
        ///    }
        ///    else
        ///    {
        ///        Console.WriteLine("Email sent successfully");
        ///    }
        /// 
        ///    Console.Write("\nPress any key to continue...");
        ///    Console.ReadKey(true);
        /// }
        /// ]]>
        /// </code>
        /// </example>
        public static string SendMail(string smtpServer, int smtpPort, bool isRequireAuthentication, string mailUserId, string mailPassword, MailMessage mailMessage)
        {
            // declare variables
            string errorMessage = string.Empty;
            
            try
            {
                using (SmtpClient smtp = new SmtpClient(smtpServer, smtpPort))
                {
                    //smtp.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                    if (isRequireAuthentication)
                    {
                        if (!String.IsNullOrEmpty(mailUserId))
                        {
                            smtp.UseDefaultCredentials = false;
                            smtp.Credentials = new NetworkCredential(mailUserId, mailPassword);
                            smtp.EnableSsl = true;
                        }
                    }
                    else
                    {
                        smtp.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                        smtp.UseDefaultCredentials = true;
                    }
                    smtp.Send(mailMessage);

                }
            }
            catch(SmtpFailedRecipientsException smtpFailedRecipient)
            {
                if (((System.Net.Mail.SmtpFailedRecipientException)(smtpFailedRecipient)).FailedRecipient != null)
                    errorMessage = smtpFailedRecipient.Message + " Failed receipient:" + ((System.Net.Mail.SmtpFailedRecipientException)(smtpFailedRecipient)).FailedRecipient;
                /*
                for (int i = 0; i < ex.InnerExceptions.Length; i++)
				{
					SmtpStatusCode status = ex.InnerExceptions[i].StatusCode;
					if (status == SmtpStatusCode.MailboxBusy ||
						status == SmtpStatusCode.MailboxUnavailable)
					{
						Console.WriteLine("Delivery failed - retrying in 5 seconds.");
						System.Threading.Thread.Sleep(5000);
						client.Send(message);
					}
					else
					{
						Console.WriteLine("Failed to deliver message to {0}", 
						    ex.InnerExceptions[i].FailedRecipient);
					}
				}
                 * */
            }
            catch (SmtpException smtpEx)
            {
                //if (!smtpEx.Message.Contains("not authenticated"))
                //{
                //    if (((System.Net.Mail.SmtpFailedRecipientException)(smtpEx)).FailedRecipient != null)
                //        errorMessage = smtpEx.Message + " Failed receipient:" + ((System.Net.Mail.SmtpFailedRecipientException)(smtpEx)).FailedRecipient;
                //}
                //else
                errorMessage = smtpEx.Message;
                if (smtpEx.InnerException != null && !string.IsNullOrEmpty(smtpEx.InnerException.Message))
                    errorMessage += " - " + smtpEx.InnerException.Message;
                //if (smtpEx.InnerException.InnerException != null && !string.IsNullOrEmpty(smtpEx.InnerException.InnerException.Message))
                //    errorMessage += " - " + smtpEx.InnerException.InnerException.Message;
                    
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
            finally
            {
                //smtp = null;
            }

            return errorMessage;

        }



        /// <summary>
        /// Send mail by passing th smtp infor
        /// </summary>
        /// <param name="smtpInfo"></param>
        /// <param name="isRequireAuthentication"></param>
        /// <param name="mailMessage"></param>
        /// <returns></returns>
        public static string SendMail(SmtpClient smtpInfo, bool isRequireAuthentication, MailMessage mailMessage)
        {
            // declare variables
            string errorMessage = string.Empty;

            try
            {
                using (SmtpClient smtp = new SmtpClient(smtpInfo.Host, smtpInfo.Port))
                {
                    //smtp.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                    if (isRequireAuthentication)
                    {
                        if (smtpInfo.Credentials != null)
                        {
                            smtp.UseDefaultCredentials = false;
                            smtp.Credentials = smtpInfo.Credentials;
                            smtp.EnableSsl = smtpInfo.EnableSsl;
                        }
                    }
                    else
                    {
                        smtp.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                        smtp.UseDefaultCredentials = true;
                    }
                    smtp.Send(mailMessage);

                }
            }
            catch (SmtpFailedRecipientsException smtpFailedRecipients)
            {
                if (((System.Net.Mail.SmtpFailedRecipientException)(smtpFailedRecipients)).FailedRecipient != null)
                    errorMessage = smtpFailedRecipients.Message + " Failed receipient:" + ((System.Net.Mail.SmtpFailedRecipientException)(smtpFailedRecipients)).FailedRecipient;
            }
            catch (SmtpFailedRecipientException smtpFailedRecipient)
            {
                if (((System.Net.Mail.SmtpFailedRecipientException)(smtpFailedRecipient)).FailedRecipient != null)
                    errorMessage = smtpFailedRecipient.Message + " Failed receipient:" + ((System.Net.Mail.SmtpFailedRecipientException)(smtpFailedRecipient)).FailedRecipient;
            }
            catch (SmtpException smtpEx)
            {
                //if (!smtpEx.Message.Contains("not authenticated"))
                //{
                //    if (((System.Net.Mail.SmtpFailedRecipientException)(smtpEx)).FailedRecipient != null)
                //        errorMessage = smtpEx.Message + " Failed receipient:" + ((System.Net.Mail.SmtpFailedRecipientException)(smtpEx)).FailedRecipient;
                //}
                //else
                errorMessage = smtpEx.Message;
                if (smtpEx.InnerException != null && !string.IsNullOrEmpty(smtpEx.InnerException.Message))
                {
                    errorMessage += " - " + smtpEx.InnerException.Message;
                    if (smtpEx.InnerException.InnerException != null && !string.IsNullOrEmpty(smtpEx.InnerException.InnerException.Message))
                        errorMessage += " - " + smtpEx.InnerException.InnerException.Message;
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
            finally
            {
                //smtp = null;
            }

            return errorMessage;

        }


        /// <summary>
        /// Sends the specified message to an SMTP Server to deliver
        /// </summary>
        /// <param name="smtpServer">A System.String that contains the smtp server information.</param>
        /// <param name="smtpPort">A System.Int32 that contains the smtp port information.</param>
        /// <param name="isSecureConnection">A System.Boolean that defines if the connection is through a secure channel.</param>
        /// <param name="isRequireAuthentication">A System.Boolean that define the authentication is required for smtp connection</param>
        ///  <param name="mailUserId">A System.String that contains the smtp login userId information.</param>
        /// <param name="mailPassword">A System.String that contains the smtp login password information.</param>
        /// <param name="mailMessage">A System.NET.Mail.MailMessage that contains message to send.</param>
        /// <param name="attachments">A Generic List of Attachment that to be attached in the message.</param>
        /// <returns>A System.String that contacin error message if exception thrown.</returns>
        /// <remarks>
        /// Return empty string if the email sent successfully.
        ///</remarks>
        ///<example>
        /// This example, a Console application, send a test mail and display the message, whether success or error occurred. 
        /// <code>
        /// <![CDATA[
        /// static void Main()
        /// {
        ///    // Set the mail server
        ///   string mailServerHost = "gwia.livmi.trw.com";
        ///    // Set the smtp port
        ///    int smtpPort = 25;
        ///    // Set the email Id
        ///    string emailId = "Shally.Chong@trw.com";
        ///    // Set the email password
        ///    string emailPwd = "xxxx";
        ///    // set the details of the email message
        ///    MailMessage mailMsg = new MailMessage("shally.chong@trw.com", "shally.chong@trw.com", "Test", "This is a Test Mail Body");
        ///     // set the list of attachment 
        ///    List<Attachment> lst = SetAttachment();
        /// 
        ///    try
        ///    {
        ///        // send the mail  
        ///        string errorMsg = MailUtilities.SendMail(mailServerHost, smtpPort, true, true, emailId, emailPwd, mailMsg, lst);
        /// 
        ///        if (errorMsg.Length > 0)
        ///        {
        ///            Console.WriteLine("The mail was not sent due to error:  " + errorMsg);
        ///        }
        ///    }
        ///    catch (Exception ex)
        ///    {
        ///        Console.WriteLine("Error:" + ex.Message);
        ///    }
        ///    else
        ///    {
        ///        Console.WriteLine("Email sent successfully");
        ///    }
        /// 
        ///    Console.Write("\nPress any key to continue...");
        ///    Console.ReadKey(true);
        /// }
        ///  private static List<Attachment> SetAttachment()
        /// {
        ///    List<Attachment> lst = new List<Attachment>();
        /// 
        ///     // Specify the file to be attached and sent.
        ///    // This example assumes that a file named data.txt exists in the current working directory.
        ///    string file = "data.txt";
        /// 
        ///    // Create  the file attachment for this e-mail message.
        ///    Attachment data = new Attachment(file, MediaTypeNames.Application.Octet);
        ///    lst.Add(data);
        /// 
        ///    return lst;
        /// }
        /// ]]>
        /// </code>
        /// </example>
        public static string SendMail(string smtpServer, int smtpPort, bool isSecureConnection, bool isRequireAuthentication, string mailUserId, string mailPassword, MailMessage mailMessage, List<Attachment> attachments)
        {
            // declare variables
            string errorMessage = string.Empty;
            
            // configure smtp
            SmtpClient smtp = new SmtpClient();
            try
            {
                smtp.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;

                smtp.Host = smtpServer;
                smtp.Port = smtpPort;
                if (isRequireAuthentication)
                {
                    if (!String.IsNullOrEmpty(mailUserId))
                    {
                        smtp.Credentials = new NetworkCredential(mailUserId, mailPassword);
                    }
                }
                else
                {
                    smtp.UseDefaultCredentials = true;
                }

                // check if has any attachment
                if (attachments != null)
                {
                    for (int i = 0; i < attachments.Count; i++)
                    {
                        Attachment data = attachments[i] as Attachment;
                        mailMessage.Attachments.Add(data);

                    }
                }
                
                smtp.Send(mailMessage);
                
            }
            catch (SmtpException smtpEx)
            {
                errorMessage = smtpEx.Message;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
            finally
            {
                smtp = null;
            }

            return errorMessage;

        }

        /// <summary>
        /// Sends the specified message to an SMTP Server to deliver
        /// </summary>
        /// <param name="smtpServer">A System.String that contains the smtp server information.</param>
        /// <param name="smtpPort">A System.Int32 that contains the smtp port information.</param>
        /// <param name="isRequireAuthentication">A System.Boolean that define the authentication is required for smtp connection</param>
        ///  <param name="mailUserId">A System.String that contains the smtp login userId information.</param>
        /// <param name="mailPassword">A System.String that contains the smtp login password information.</param>
        /// <param name="mailMessage">A System.NET.Mail.MailMessage that contains message to send.</param>
        /// <param name="attachments">A Generic List of Attachment that to be attached in the message.</param>
        /// <returns>A System.String that contacin error message if exception thrown. </returns>
        /// <remarks>
        /// Return empty string if the email sent successfully.
        ///</remarks>
        ///<example>
        /// This example, a Console application, send a test mail and display the message, whether success or error occurred. 
        /// <code>
        /// <![CDATA[
        /// static void Main()
        /// {
        ///    // Set the mail server
        ///   string mailServerHost = "gwia.livmi.trw.com";
        ///    // Set the smtp port
        ///    int smtpPort = 25;
        ///    // Set the email Id
        ///    string emailId = "Shally.Chong@trw.com";
        ///    // Set the email password
        ///    string emailPwd = "xxxx";
        ///    // set the details of the email message
        ///    MailMessage mailMsg = new MailMessage("shally.chong@trw.com", "shally.chong@trw.com", "Test", "This is a Test Mail Body");
        ///     // set the list of attachment 
        ///    List<Attachment> lst = SetAttachment();
        /// 
        ///    try
        ///    {
        ///        // send the mail  
        ///        string errorMsg = MailUtilities.SendMail(mailServerHost, smtpPort, true, emailId, emailPwd, mailMsg, lst);
        /// 
        ///        if (errorMsg.Length > 0)
        ///        {
        ///            Console.WriteLine("The mail was not sent due to error:  " + errorMsg);
        ///        }
        ///    }
        ///    catch (Exception ex)
        ///    {
        ///        Console.WriteLine("Error:" + ex.Message);
        ///    }
        ///    else
        ///    {
        ///        Console.WriteLine("Email sent successfully");
        ///    }
        /// 
        ///    Console.Write("\nPress any key to continue...");
        ///    Console.ReadKey(true);
        /// }
        /// 
        ///  private static List<Attachment> SetAttachment()
        /// {
        ///    List<Attachment> lst = new List<Attachment>();
        /// 
        ///     // Specify the file to be attached and sent.
        ///    // This example assumes that a file named data.txt exists in the current working directory.
        ///    string file = "data.txt";
        /// 
        ///    // Create  the file attachment for this e-mail message.
        ///    Attachment data = new Attachment(file, MediaTypeNames.Application.Octet);
        ///    lst.Add(data);
        /// 
        ///    return lst;
        /// }
        /// ]]>
        /// </code>
        /// </example>
        public static string SendMail(string smtpServer, int smtpPort, bool isRequireAuthentication, string mailUserId, string mailPassword, MailMessage mailMessage, List<Attachment> attachments)
        {
            // declare variables
            string errorMessage = string.Empty;
            
            // configure smtp
            SmtpClient smtp = new SmtpClient();
            try
            {
                smtp.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                smtp.EnableSsl = false;
                smtp.Host = smtpServer;
                smtp.Port = smtpPort;
                if (isRequireAuthentication)
                {
                    if (!String.IsNullOrEmpty(mailUserId))
                    {
                        smtp.Credentials = new NetworkCredential(mailUserId, mailPassword);
                    }
                }
                else
                {
                    smtp.UseDefaultCredentials = true;
                }

                // check if has any attachment
                if (attachments != null)
                {
                    for (int i = 0; i < attachments.Count; i++)
                    {
                        Attachment data = attachments[i] as Attachment;
                        mailMessage.Attachments.Add(data);

                    }
                }

                smtp.Send(mailMessage);
                
            }
            catch (SmtpException smtpEx)
            {
                errorMessage = smtpEx.Message;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
            finally
            {
                smtp = null;
            }

            return errorMessage;

        }
           
         #endregion

        #region Set Mail Message
        /// <summary>
        /// Sets mail message with AlternatViews
        /// </summary>
        /// <param name="fromAddress">A System.String that contains the addresses information of the message sender.</param>
        /// <param name="toAddress">A System.String that contains the addresses information that the message is sent to.</param>
        /// <param name="ccAddress">A System.String that contains the addresses information that the message is cc to.</param>
        /// <param name="bccAddress">A System.String that contains the addresses information that the message is bcc to.</param>
        /// <param name="replyToAddress">A System.String that contains the addresses information that the message is reply to.</param>
        /// <param name="subject">A System.String that contains the subject line of the message.</param>
        /// <param name="body">A System.String that contains the message body</param>
        /// <param name="attachments">A generic list of System.Net.Mail.Attachment</param>
        /// <param name="mailPriority">A System.Net.Mail.MailPriority that set the priority of  the message</param>
        /// <param name="isHtml">A System.Boolean that define the message format. Either Html or Text</param>
        /// <returns>A System.NET.Mail.MailMessage that contains message to send. </returns>
        /// <remarks>
        /// Creates a mail message accordingly.
        ///</remarks>
        ///<example>
        /// This example, a Console application, send a test mail and display the message, whether success or error occurred. 
        /// <code>
        /// <![CDATA[
        /// static void Main()
        /// {
        ///    // Set the mail server
        ///    string mailServerHost = "gwia.livmi.trw.com";
        ///    // set the details of the email message
        ///    MailMessage mailMsg = CreateMailMsg();
        /// 
        ///    try
        ///    {
        ///        // send the mail  
        ///        string errorMsg = MailUtilities.SendMail(mailServerHost, mailMsg);
        /// 
        ///        if (errorMsg.Length > 0)
        ///        {
        ///            Console.WriteLine("The mail was not sent due to error:  " + errorMsg);
        ///        }
        ///    }
        ///    catch (Exception ex)
        ///    {
        ///        Console.WriteLine("Error:" + ex.Message);
        ///    }
        ///    else
        ///    {
        ///        Console.WriteLine("Email sent successfully");
        ///    }
        /// 
        ///    Console.Write("\nPress any key to continue...");
        ///    Console.ReadKey(true);
        /// }
        /// 
        /// private static List<Attachment> SetAttachment()
        /// {
        ///    List<Attachment> lst = new List<Attachment>();
        /// 
        ///     // Specify the file to be attached and sent.
        ///    // This example assumes that a file named data.txt exists in the current working directory.
        ///    string file = "data.txt";
        /// 
        ///    // Create  the file attachment for this e-mail message.
        ///    Attachment data = new Attachment(file, MediaTypeNames.Application.Octet);
        ///    lst.Add(data);
        /// 
        ///    return lst;
        /// }
        /// 
        ///  private static MailMessage CreateMailMsg()
        /// {
        ///    // Create a sample MailMessage with the following details.
        ///    MailMessage msg = new MailMessage();
        /// 
        ///    // Set the sender's email
        ///    string mailFrom = "shally.chong@trw.com";
        ///    // set the recipient's email
        ///    string mailTo = "shally.chong@trw.com";
        ///    // set the CC's email
        ///    string mailCC = "shally.chong@trw.com";
        ///    // set the BCC's mail
        ///    string mailBcc = ""; 
        ///    // set the reply email address
        ///    string replyTo = "";
        ///    // set the subject of the email
        ///    string subject = "Test";
        ///    // set the content of the email
        ///    string body = "This is a test email.";
        ///    // set the list of attachment
        ///    List<Attachment> lst = SetAttachment();
        /// 
        ///    // Create  the mail message.
        ///    msg = MailUtilities.SetMailMessage(mailFrom, mailTo, mailCC, mailBcc, replyTo, subject, body, lst, MailPriority.High, true);
        ///    return msg;
        /// }
        /// ]]>
        /// </code>
        /// </example>
        public static MailMessage SetMailMessage(string fromAddress, string toAddress, string ccAddress, string bccAddress, string replyToAddress, string subject, string body, List<Attachment> attachments, MailPriority mailPriority, bool isHtml)
        {
            MailMessage message = new MailMessage();

            // Set Sender
            string[] fromAddrs = Regex.Split(fromAddress, @",");
            for (int i = 0; i < fromAddrs.Length; i++)
            {
                string fromAddr = fromAddrs[i];
                if (fromAddr.Trim().Length > 0)
                {
                    MailAddress mailFrom = new MailAddress(fromAddr);
                    message.From = mailFrom;
                }
            }

            // Set Recipient
            if (toAddress != null && !String.IsNullOrEmpty(toAddress.Trim()))
            {
                string[] toAddrs = Regex.Split(toAddress, @",");
                for (int i = 0; i < toAddrs.Length; i++)
                {
                    string toAddr = toAddrs[i];
                    if (toAddr.Trim().Length > 0)
                    {
                        MailAddress mailAddr = new MailAddress(toAddr);
                        message.To.Add(mailAddr);

                    }
                }


            }

            // Set Cc
            if (ccAddress != null && !String.IsNullOrEmpty(ccAddress.Trim()))
            {
                string[] ccAddrs = Regex.Split(ccAddress, @",");
                for (int i = 0; i < ccAddrs.Length; i++)
                {
                    string ccAddr = ccAddrs[i];
                    if (ccAddr.Trim().Length > 0)
                    {
                        MailAddress mailAddr = new MailAddress(ccAddr);
                        message.CC.Add(mailAddr);
                    }
                }


            }


            // Set BCc
            if (bccAddress != null && !String.IsNullOrEmpty(bccAddress.Trim()))
            {
                string[] bccAddrs = Regex.Split(bccAddress, @",");

                for (int i = 0; i < bccAddrs.Length; i++)
                {
                    string bccAddr = bccAddrs[i];
                    if (bccAddr.Trim().Length > 0)
                    {
                        MailAddress mailAddr = new MailAddress(bccAddr);
                        message.Bcc.Add(mailAddr);

                    }
                }

            }



            // Set ReplyTo
            if (replyToAddress != null && !String.IsNullOrEmpty(replyToAddress.Trim()))
            {
                string[] replyToAddrs = Regex.Split(replyToAddress, @",");
                for (int i = 0; i < replyToAddrs.Length; i++)
                {
                    string replyToAddr = replyToAddrs[i];
                    if (replyToAddr.Trim().Length > 0)
                    {
                        MailAddress mailreplyTo = new MailAddress(replyToAddr);
                        message.ReplyToList.Add(mailreplyTo);
                    }
                }
            }

            // Set attachment
            if (attachments != null)
            {
                for (int i = 0; i < attachments.Count; i++)
                {
                    Attachment data = attachments[i] as Attachment;
                    message.Attachments.Add(data);

                }
            }

            // Set subject
            message.Subject = subject;

            // Set body
            message.Body = body;

            // Set Priority
            message.Priority = mailPriority;

            // set isHTML
            message.IsBodyHtml = isHtml;

            return message;
        }

        /// <summary>
        /// Set mail message with AlternatViews
        /// </summary>
        /// <param name="fromAddress">A System.String that contains the addresses information of the message sender.</param>
        /// <param name="toAddress">A System.String that contains the addresses information that the message is sent to.</param>
        /// <param name="ccAddress">A System.String that contains the addresses information that the message is cc to.</param>
        /// <param name="bccAddress">A System.String that contains the addresses information that the message is bcc to.</param>
        /// <param name="replyToAddress">A System.String that contains the addresses information that the message is reply to.</param>
        /// <param name="subject">A System.String that contains the subject line of the message.</param>
        /// <param name="plainBody">A System.String that contains the message plain body</param>
        /// <param name="htmlBody">A System.String that contains the message Html body</param>
        /// <param name="attachments">A generic list of System.Net.Mail.Attachment</param>
        /// <param name="mailPriority">A System.Net.Mail.MailPriority that set the priority of  the message</param>
        /// <returns>A System.NET.Mail.MailMessage that contains message to send. </returns> 
        /// <remarks>
        /// Creates a mail message accordingly.
        ///</remarks>
        ///<example>
        /// This example, a Console application, send a test mail and display the message, whether success or error occurred. 
        /// <code>
        /// <![CDATA[
        /// static void Main()
        /// {
        ///    // Set the mail server
        ///    string mailServerHost = "gwia.livmi.trw.com";
        ///    // set the details of the email message
        ///    MailMessage mailMsg = CreateMailMsg();
        /// 
        ///    try
        ///    {
        ///        // send the mail  
        ///        string errorMsg = MailUtilities.SendMail(mailServerHost, mailMsg);
        /// 
        ///        if (errorMsg.Length > 0)
        ///        {
        ///            Console.WriteLine("The mail was not sent due to error:  " + errorMsg);
        ///        }
        ///    }
        ///    catch (Exception ex)
        ///    {
        ///        Console.WriteLine("Error:" + ex.Message);
        ///    }
        ///    else
        ///    {
        ///        Console.WriteLine("Email sent successfully");
        ///    }
        /// 
        ///    Console.Write("\nPress any key to continue...");
        ///    Console.ReadKey(true);
        /// }
        /// 
        /// private static List<Attachment> SetAttachment()
        /// {
        ///    List<Attachment> lst = new List<Attachment>();
        /// 
        ///     // Specify the file to be attached and sent.
        ///    // This example assumes that a file named data.txt exists in the current working directory.
        ///    string file = "data.txt";
        /// 
        ///    // Create  the file attachment for this e-mail message.
        ///    Attachment data = new Attachment(file, MediaTypeNames.Application.Octet);
        ///    lst.Add(data);
        /// 
        ///    return lst;
        /// }
        /// 
        ///  private static MailMessage CreateMailMsg()
        /// {
        ///    // Create a sample MailMessage with the following details.
        ///    MailMessage msg = new MailMessage();
        /// 
        ///    // Set the sender's email
        ///    string mailFrom = "shally.chong@trw.com";
        ///    // set the recipient's email
        ///    string mailTo = "shally.chong@trw.com";
        ///    // set the CC's email
        ///    string mailCC = "shally.chong@trw.com";
        ///    // set the BCC's mail
        ///    string mailBcc = ""; 
        ///    // set the reply email address
        ///    string replyTo = "";
        ///    // set the subject of the email
        ///    string subject = "Test";
        ///   // set the content of the plain body
        ///    string plainBody = "This is a plain test body.";
        ///   // set the content of the html body
        ///    string htmlBody = "<b>This is a html test body.</b>";
        ///    // set the list of attachment
        ///    List<Attachment> lst = SetAttachment();
        /// 
        ///    // Create  the mail message.
        ///    msg = MailUtilities.SetMailMessage(mailFrom, mailTo, mailCC, mailBcc, replyTo, subject, plainBody, htmlBody, lst, MailPriority.High);
        ///    return msg;
        /// }
        /// ]]>
        /// </code>
        /// </example>
        public static MailMessage SetMailMessage(string fromAddress, string toAddress, string ccAddress, string bccAddress, string replyToAddress, string subject, string plainBody, string htmlBody, List<Attachment> attachments, MailPriority mailPriority)
        {
            MailMessage message = new MailMessage();

            // Set Sender
            string[] fromAddrs = Regex.Split(fromAddress, @",");
            for (int i = 0; i < fromAddrs.Length; i++)
            {
                string fromAddr = fromAddrs[i];
                if (fromAddr.Trim().Length > 0)
                {
                    MailAddress mailFrom = new MailAddress(fromAddr);
                    message.From = mailFrom;
                }
            }

            // Set Recipient
            if (toAddress != null && !String.IsNullOrEmpty(toAddress.Trim()))
            {
                string[] toAddrs = Regex.Split(toAddress, @",");
                for (int i = 0; i < toAddrs.Length; i++)
                {
                    string toAddr = toAddrs[i];
                    if (toAddr.Trim().Length > 0)
                    {
                        MailAddress mailAddr = new MailAddress(toAddr);
                        message.To.Add(mailAddr);

                    }
                }


            }

            // Set Cc
            if (ccAddress != null && !String.IsNullOrEmpty(ccAddress.Trim()))
            {
                string[] ccAddrs = Regex.Split(ccAddress, @",");
                for (int i = 0; i < ccAddrs.Length; i++)
                {
                    string ccAddr = ccAddrs[i];
                    if (ccAddr.Trim().Length > 0)
                    {
                        MailAddress mailAddr = new MailAddress(ccAddr);
                        message.CC.Add(mailAddr);
                    }
                }


            }


            // Set BCc
            if (bccAddress != null && !String.IsNullOrEmpty(bccAddress.Trim()))
            {
                string[] bccAddrs = Regex.Split(bccAddress, @",");

                for (int i = 0; i < bccAddrs.Length; i++)
                {
                    string bccAddr = bccAddrs[i];
                    if (bccAddr.Trim().Length > 0)
                    {
                        MailAddress mailAddr = new MailAddress(bccAddr);
                        message.Bcc.Add(mailAddr);

                    }
                }

            }



            // Set ReplyTo
            if (replyToAddress != null && !String.IsNullOrEmpty(replyToAddress.Trim()))
            {
                string[] replyToAddrs = Regex.Split(replyToAddress, @",");
                for (int i = 0; i < replyToAddrs.Length; i++)
                {
                    string replyToAddr = replyToAddrs[i];
                    if (replyToAddr.Trim().Length > 0)
                    {
                        MailAddress mailreplyTo = new MailAddress(replyToAddr);
                        message.ReplyToList.Add(mailreplyTo);
                    }
                }
            }

            // Set attachment
            if (attachments != null)
            {
                for (int i = 0; i < attachments.Count; i++)
                {
                    Attachment data = attachments[i] as Attachment;
                    message.Attachments.Add(data);

                }
            }

            // Set subject
            message.Subject = subject;

            // Set body
            AlternateView plainView = System.Net.Mail.AlternateView.CreateAlternateViewFromString
            (System.Text.RegularExpressions.Regex.Replace(plainBody, @"<(.|\n)*?>", string.Empty), null, "text/plain");
            message.AlternateViews.Add(plainView);
            
            System.Net.Mail.AlternateView htmlView = System.Net.Mail.AlternateView.CreateAlternateViewFromString(htmlBody, null, "text/html");
            message.AlternateViews.Add(htmlView);

            // Set Priority
            message.Priority = mailPriority;



            return message;
        }


        #endregion

        #region Manipulator
        /// <summary>
        /// Converts a list of mail addresses which are separated by a delimiter (Comma or Semi-Colon) to
        /// a collection of MailAddress type.
        /// </summary>
        /// <param name="mailAddressList">A list of mail addresses separated either by comma or semi-colon</param>
        /// <returns>A collection of System.Net.Mail.MailAddress</returns>
        /// <remarks>The maill address in a the list can be defined in the following format:
        /// USER@DOMAIN(DISPLAYNAME)
        /// </remarks>
        /// <example>
        /// This example, a Console application, send a test mail with sender's display name, and multiple recipients
        /// and display the message, whether success or error occurred. 
        /// <code>
        /// <![CDATA[
        /// static void Main()
        /// {
        ///    // Set the mail server
        ///    string mailServerHost = "gwia.livmi.trw.com";
        ///    // Set the sender's email with display name
        ///    MailAddress address = MailUtilities.ConvertToMailAddress("shally.chong@trw.com(Shally Chong)");
        ///    // Set the mutilple recipient's email with display name
        ///    MailAddressCollection lstAdd = MailUtilities.ConvertToMailAddressCollection("shally.chong@trw.com(Shally Chong), shally.chong@gmail.com(Chong Shally)");
        /// 
        ///     if (lstAdd.Count > 0)
        ///    {
        ///        MailMessage mailMsg = new MailMessage();
        ///        mailMsg.From = address;
        ///         for (int i = 0; i < lstAdd.Count; i++)
        ///        {
        ///            mailMsg.To.Add(lstAdd[i]);
        ///        }
        /// 
        ///        // set the details of the email message   
        ///        mailMsg.Subject = "Test";
        ///        mailMsg.Body = "This is a test mail.";
        /// 
        ///        try
        ///        {
        ///            // send the mail  
        ///            string errorMsg = MailUtilities.SendMail(mailServerHost, mailMsg);
        /// 
        ///            if (errorMsg.Length > 0)
        ///            {
        ///                Console.WriteLine("The mail was not sent due to error:  " + errorMsg);
        ///            }
        ///        }
        ///        catch (Exception ex)
        ///        {
        ///            Console.WriteLine("Error:" + ex.Message);
        ///        }
        ///        else
        ///        {
        ///            Console.WriteLine("Email sent successfully");
        ///        }
        /// 
        ///    Console.Write("\nPress any key to continue...");
        ///    Console.ReadKey(true);
        ///    }
        /// }
        /// ]]>
        /// </code>
        /// </example>
        public static MailAddressCollection ConvertToMailAddressCollection(string mailAddressList)
        {
            MailAddressCollection mailAddressCol = new MailAddressCollection();
            mailAddressList = mailAddressList.Trim();
            if (!string.IsNullOrEmpty(mailAddressList))
            {
                char[] delimiter = new char[] { ',', ';' };
                string[] addrs = mailAddressList.Split( delimiter);

                // Loop through the array
                for (int i = 0; i < addrs.Length; i++)
                {
                    string addr = addrs[i].Trim();
                    MailAddress mailAddr = ConvertToMailAddress(addr);
                    if (mailAddr != null)
                    {
                        mailAddressCol.Add(mailAddr);
                    }                     
                }               
            }
            return mailAddressCol;
        }

        /// <summary>
        /// Convert a System.String that contains an email address ( and display name ) to
        /// a System.Net.Mail.MailAddress type.
        /// </summary>
        /// <param name="mailAddress"> a System.String that contains an email address (and display name)</param>
        /// <returns>A System.Net.Mail.MailAddress type</returns>
        /// <remarks>The maill address can be defined in the following format:
        /// USER@DOMAIN(DISPLAYNAME)
        /// </remarks>
        /// <example>
        /// This example, a Console application, send a test mail with sender's display name and display the message, whether success or error occurred. 
        /// <code>
        /// <![CDATA[
        /// static void Main()
        /// {
        ///    // Set the mail server
        ///    string mailServerHost = "gwia.livmi.trw.com";
        ///    // Set the email address with display name
        ///    MailAddress address = MailUtilities.ConvertToMailAddress("shally.chong@trw.com(Shally Chong)");
        ///    // set the details of the email message
        ///    MailMessage mailMsg = new MailMessage(address, address);
        ///    mailMsg.Subject = "Test";
        ///    mailMsg.Body = "This is a test mail.";
        /// 
        ///    try
        ///    {
        ///        // send the mail  
        ///        string errorMsg = MailUtilities.SendMail(mailServerHost, mailMsg);
        /// 
        ///        if (errorMsg.Length > 0)
        ///        {
        ///            Console.WriteLine("The mail was not sent due to error:  " + errorMsg);
        ///        }
        ///    }
        ///    catch (Exception ex)
        ///    {
        ///        Console.WriteLine("Error:" + ex.Message);
        ///    }
        ///    else
        ///    {
        ///        Console.WriteLine("Email sent successfully");
        ///    }
        /// 
        ///    Console.Write("\nPress any key to continue...");
        ///    Console.ReadKey(true);
        /// }
        /// ]]>
        /// </code>
        /// </example>
        public static MailAddress ConvertToMailAddress(string mailAddress)
        {            
            mailAddress = mailAddress.Trim();
            if (!string.IsNullOrEmpty(mailAddress))
            {
                MailAddress mailAddr;

                string delimiter = @"\(";
                string[] addrs = Regex.Split(mailAddress, delimiter);
                string addr = addrs[0].Trim();
                if (addrs.Length > 1)
                {
                    string displayName = Regex.Replace(addrs[1].Trim(), @"\)", "");
                    mailAddr = new MailAddress(addr,displayName);
                }
                else
                {
                    mailAddr = new MailAddress(addr);
                }

                return mailAddr;
            }
            else
            {
                return null;
            }
        }

        #endregion

        #endregion
    }
}
