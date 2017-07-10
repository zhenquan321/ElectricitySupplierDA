using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Web;

namespace IW2S.Helpers
{
    public class EmailHelper
    {
        /// <summary>
        /// Sends an email
        /// </summary>
        public void SendMail(string To, string CC, string Bcc, string Subject, string Body,
            LinkedResource[] EmbeddedResources,Attachment[] Attachments
            , string From, string UserName, string Password, string Server, int Port, bool UseSSL)
        {
            using (System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage())
            {
                char[] Splitter = { ',', ';' };
                string[] AddressCollection = To.Split(Splitter);
                for (int x = 0; x < AddressCollection.Length; ++x)
                {
                    if (!string.IsNullOrEmpty(AddressCollection[x].Trim()))
                        message.To.Add(AddressCollection[x]);
                }
                if (!string.IsNullOrEmpty(CC))
                {
                    AddressCollection = CC.Split(Splitter);
                    for (int x = 0; x < AddressCollection.Length; ++x)
                    {
                        if (!string.IsNullOrEmpty(AddressCollection[x].Trim()))
                            message.CC.Add(AddressCollection[x]);
                    }
                }
                if (!string.IsNullOrEmpty(Bcc))
                {
                    AddressCollection = Bcc.Split(Splitter);
                    for (int x = 0; x < AddressCollection.Length; ++x)
                    {
                        if (!string.IsNullOrEmpty(AddressCollection[x].Trim()))
                            message.Bcc.Add(AddressCollection[x]);
                    }
                }
                message.Subject = Subject;
                message.From = new System.Net.Mail.MailAddress((From));
                AlternateView BodyView = AlternateView.CreateAlternateViewFromString(Body, null, MediaTypeNames.Text.Html);
                if (EmbeddedResources != null)
                {
                    foreach (LinkedResource Resource in EmbeddedResources)
                    {
                        BodyView.LinkedResources.Add(Resource);
                    }
                }
                message.AlternateViews.Add(BodyView);
                //message.Body = Body;
                message.Priority = MailPriority.High;
                message.SubjectEncoding = System.Text.Encoding.GetEncoding("UTF-8");
                message.BodyEncoding = System.Text.Encoding.GetEncoding("UTF-8");
                message.IsBodyHtml = true;
                if (Attachments != null)
                {
                    
                    foreach (Attachment TempAttachment in Attachments)
                    {
                        message.Attachments.Add(TempAttachment);
                    }
                }
                System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient(Server, Port);
                if (!string.IsNullOrEmpty(UserName) && !string.IsNullOrEmpty(Password))
                {
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new System.Net.NetworkCredential(UserName, Password);
                   
                }
                if (UseSSL)
                    smtp.EnableSsl = true;
                else
                    smtp.EnableSsl = false;

                try
                {
                    smtp.Send(message);
                   
                }
                catch (Exception ex)
                {
                   
                }
            }
        }
    }
}