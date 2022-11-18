using Microsoft.Exchange.WebServices.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace computan.exchange.web.services
{
    public static class ExchangeEmailer
    {

        public static string ReplyEmail(ExchangeService service, string ItemId, bool replyToAll, string from, string Subject, string ReplyBody, string TO, string CC, string BCC)
        {
            // Bind to the email message to reply to by using the ItemId.
            // This method call results in a GetItem call to EWS.
            EmailMessage message = EmailMessage.Bind(service, new ItemId(ItemId), BasePropertySet.IdOnly);

            message.From = from;

            if (!string.IsNullOrEmpty(TO))
            {
                string[] ToEmails = TO.Split(';');
                if (ToEmails.Length > 0)
                {
                    foreach (string toEmail in ToEmails)
                    {
                        if (!string.IsNullOrEmpty(toEmail))
                        {
                            message.ToRecipients.Add(new EmailAddress(toEmail));
                        }
                    }
                }
            }

            if (!string.IsNullOrEmpty(CC))
            {
                string[] CCEmails = CC.Split(';');
                if (CCEmails.Length > 0)
                {
                    foreach (string ccEmail in CCEmails)
                    {
                        if (!string.IsNullOrEmpty(ccEmail))
                        {
                            message.CcRecipients.Add(new EmailAddress(ccEmail));
                        }
                    }
                }
            }

            if (!string.IsNullOrEmpty(BCC))
            {
                string[] BCCEmails = BCC.Split(';');
                if (BCCEmails.Length > 0)
                {
                    foreach (string bccEmail in BCCEmails)
                    {
                        if (!string.IsNullOrEmpty(bccEmail))
                        {
                            message.CcRecipients.Add(new EmailAddress(bccEmail));
                        }
                    }
                }
            }

            // Create the reply response message from the original email message.
            // Indicate whether the message is a reply or reply all type of reply.
            ResponseMessage responseMessage = message.CreateReply(replyToAll);


            // Set subject of the reply message.
            responseMessage.Subject = Subject;


            // Prepend the reply to the message body. 
            responseMessage.BodyPrefix = ReplyBody;

            // Send the response message.
            // This method call results in a CreateItem call to EWS.
            responseMessage.SendAndSaveCopy();

            //// Check that the response was sent by calling FindRecentlySent.
            //FindRecentlySent(message);

            //EmailMessage reply = responseMessage.Save();
            //reply.Attachments.AddFileAttachment("attachmentname.txt");
            //reply.Update(ConflictResolutionMode.AutoResolve);
            //reply.SendAndSaveCopy();
            return string.Empty;
        }

        public static string SendEmail(string type, string to, string cc, string bcc, string subject, string body, List<string> fileAttachment = null)
        {
            try
            {
                ExchangeService service = ExchangeServiceInstance.ConnectToService(ExchangeCredentialsFromConfig.GetExchangeCredentials());
                EmailMessage message = new EmailMessage(service);

                if (fileAttachment != null && fileAttachment.Count() > 0)
                {
                    foreach (string file in fileAttachment)
                    {
                        if (file != null && System.IO.File.Exists(file))
                        {
                            message.Attachments.AddFileAttachment(file);
                        }
                    }
                }


                type = type ?? "";
                if (type == "Replay" || type == "ReplayAll")
                {
                    subject = subject.Remove(0, 3);
                    message.Subject = subject;

                    List<string> temp = to.Split(';').ToList();
                    if (temp.Count == 1 && to != "")
                    { message.ToRecipients.Add(to); }
                    else if (temp.Count > 1)
                    { message.ToRecipients.AddRange(temp); }


                    temp = cc.Split(';').ToList();
                    if (temp.Count == 1 && cc != "")
                    { message.CcRecipients.Add(cc); }
                    else if (temp.Count > 1)
                    { message.CcRecipients.AddRange(temp); }


                    temp = bcc.Split(';').ToList();
                    if (temp.Count == 1 && bcc != "")
                    { message.BccRecipients.Add(bcc); }
                    else if (temp.Count > 1)
                    { message.BccRecipients.AddRange(temp); }

                    //var index = body.IndexOf("____________________Right Above This Line and do not remove this line____________________");
                    //string messageBody = "";
                    //if (index > 0)
                    //{
                    //    messageBody = body.Substring(0, index);
                    //}
                    //else
                    //{  }
                    //messageBody = body;
                    message.Body = body;

                    message.SendAndSaveCopy();
                }

                //message.SendAndSaveCopy(); //this will send a new email except replay
                return "success";
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
        }
    }
}
