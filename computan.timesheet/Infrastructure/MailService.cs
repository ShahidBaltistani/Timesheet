using computan.timesheet.Contexts;
using computan.timesheet.core;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Net.Configuration;
using System.Net.Mail;
using System.Text;

namespace computan.timesheet.Infrastructure
{
    public static class MailService
    {
        private static readonly ApplicationDbContext db = new ApplicationDbContext();

        public static void SendEmail(MailMessage mailMessage)
        {
            try
            {
                SmtpSection settings = (SmtpSection)ConfigurationManager.GetSection("system.net/mailSettings/smtp");

                if (settings != null)
                {
                    mailMessage.IsBodyHtml = true;
                    mailMessage.BodyEncoding = Encoding.UTF8;
                    mailMessage.SubjectEncoding = Encoding.UTF8;
                    // Add Alternate views for clients that doesn't support HTML.
                    //mailMessage.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(HTML_BODY, new ContentType("text/html")));
                    //mailMessage.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(PLAIN_BODY, new ContentType("text/html")));

                    // Add BCC Addresses
                    string bccAddresses = ConfigurationManager.AppSettings["BCCAddresses"];
                    if (!string.IsNullOrEmpty(bccAddresses))
                    {
                        string[] bccList = bccAddresses.Split(';');
                        foreach (string bcc in bccList)
                        {
                            if (string.IsNullOrEmpty(bcc.Trim()))
                            {
                                mailMessage.Bcc.Add(bcc);
                            }
                        }
                    }

                    SmtpClient client = new SmtpClient(settings.Network.Host, settings.Network.Port)
                    {
                        DeliveryMethod = SmtpDeliveryMethod.Network
                    };
                    if (!settings.Network.DefaultCredentials)
                    {
                        client.Credentials =
                            new NetworkCredential(settings.Network.UserName, settings.Network.Password);
                        client.EnableSsl = true;
                    }

                    client.Send(mailMessage);
                }
            }
            catch (Exception ex)
            {
                MyExceptions myex = new MyExceptions
                {
                    action = "MailService.SendEmail",
                    exceptiondate = DateTime.Now,
                    controller = "Infrastructure.MailService Class",
                    exception_message = ex.Message,
                    exception_source = ex.Source,
                    exception_stracktrace = ex.StackTrace,
                    exception_targetsite = ex.TargetSite.DeclaringType.FullName + ", " + ex.TargetSite.Name,
                    ipused = string.Empty,
                    userid = string.Empty
                };
                db.MyExceptions.Add(myex);
                db.SaveChanges();
            }
        }

        public static bool SendEmailStatus(MailMessage mailMessage)
        {
            try
            {
                SmtpSection settings = (SmtpSection)ConfigurationManager.GetSection("system.net/mailSettings/smtp");

                if (settings != null)
                {
                    mailMessage.IsBodyHtml = true;
                    mailMessage.BodyEncoding = Encoding.UTF8;
                    mailMessage.SubjectEncoding = Encoding.UTF8;
                    // Add Alternate views for clients that doesn't support HTML.
                    //mailMessage.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(HTML_BODY, new ContentType("text/html")));
                    //mailMessage.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(PLAIN_BODY, new ContentType("text/html")));

                    // Add BCC Addresses
                    string BCCAddresses = ConfigurationManager.AppSettings["BCCAddresses"];
                    if (!string.IsNullOrEmpty(BCCAddresses))
                    {
                        string[] BCCList = BCCAddresses.Split(';');
                        foreach (string bcc in BCCList)
                        {
                            if (string.IsNullOrEmpty(bcc.Trim()))
                            {
                                mailMessage.Bcc.Add(bcc);
                            }
                        }
                    }

                    SmtpClient client = new SmtpClient(settings.Network.Host, settings.Network.Port)
                    {
                        DeliveryMethod = SmtpDeliveryMethod.Network
                    };
                    if (!settings.Network.DefaultCredentials)
                    {
                        client.Credentials =
                            new NetworkCredential(settings.Network.UserName, settings.Network.Password);
                        client.EnableSsl = true;
                    }

                    client.Send(mailMessage);
                }

                return true;
            }
            catch (Exception ex)
            {
                MyExceptions myex = new MyExceptions
                {
                    action = "MailService.SendEmail",
                    exceptiondate = DateTime.Now,
                    controller = "Infrastructure.MailService Class",
                    exception_message = ex.Message,
                    exception_source = ex.Source,
                    exception_stracktrace = ex.StackTrace,
                    exception_targetsite = ex.TargetSite.DeclaringType.FullName + ", " + ex.TargetSite.Name,
                    ipused = string.Empty,
                    userid = string.Empty
                };
                db.MyExceptions.Add(myex);
                db.SaveChanges();
                return false;
            }
        }

        public static void SendClientEmail(string email, string subject, string body, List<string> ccEmails, Boolean isOrphanedRequest = false)
        {
            //SmtpSection settings = (SmtpSection)ConfigurationManager.GetSection("system.net/mailSettings/smtp");
            //MailMessage mailMessage = new MailMessage
            //{
            //    From = new MailAddress(settings.From)
            //};
            SmtpSection settings = (SmtpSection)ConfigurationManager.GetSection("system.net/mailSettings/smtp");
            SmtpSection orphanedSettings = (SmtpSection)ConfigurationManager.GetSection("mailSettings/smtp_1");
            MailMessage mailMessage = new MailMessage();
            mailMessage.From = isOrphanedRequest ? new MailAddress(orphanedSettings.From) : new MailAddress(settings.From);
            //mailMessage.To.Add(new MailAddress(email));
            //mailMessage.To.Add("jhassan@computan.net"); commented by Abdul Khaliq
            mailMessage.To.Add(email);
            //mailMessage.Bcc.Add("rashed@computan.net");
            //mailMessage.Bcc.Add("jhassan@computan.net");
            if (ccEmails != null && ccEmails.Count > 0)
            {
                foreach (string ccemail in ccEmails)
                {
                    mailMessage.CC.Add(ccemail);
                }
            }

            mailMessage.Subject = subject;
            mailMessage.IsBodyHtml = true;
            mailMessage.Body = body;
            mailMessage.BodyEncoding = Encoding.UTF8;
            mailMessage.SubjectEncoding = Encoding.UTF8;
            SmtpClient client = new SmtpClient(settings.Network.Host, settings.Network.Port)
            {
                DeliveryMethod = SmtpDeliveryMethod.Network
            };
            if (!settings.Network.DefaultCredentials)
            {
                client.Credentials = new NetworkCredential(settings.Network.UserName, settings.Network.Password);
                client.EnableSsl = true;
            }

            client.Send(mailMessage);
        }
    }
}