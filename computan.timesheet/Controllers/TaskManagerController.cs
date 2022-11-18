using computan.timesheet.core;
using computan.timesheet.Helpers;
using computan.timesheet.Infrastructure;
using Microsoft.AspNet.Identity;
using Microsoft.Exchange.WebServices.Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Contact = computan.timesheet.core.Contact;

namespace computan.timesheet.Controllers
{
    [CustomeAuthorizeAttribute]
    public class TaskManagerController : BaseController
    {
        // GET: TaskManager
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SyncNow()
        {
            //try
            //{
            //    // Create an instance of the service.
            //    ExchangeService service = ExchangeServiceInstance.ConnectToService(ExchangeCredentialsFromConfig.GetExchangeCredentials());

            //    // Get Oldest Conversations.
            //    ICollection<Conversation> Conversations = ExchangeManager.GetConversations(service);

            //    // Make sure conversation is not null and have some items in it.
            //    if (Conversations != null && Conversations.Count > 0)
            //    {
            //        foreach (Conversation conversation in Conversations)
            //        {
            //            try
            //            {

            //                // If its a Calender Meeting request, ignore it.
            //                if (conversation.ItemClasses[0] != "IPM.Note")
            //                {
            //                    if (conversation.ItemClasses[0] == "IPM.Schedule.Meeting.Canceled")
            //                    {
            //                        conversation.MoveItemsInConversation(new FolderId(WellKnownFolderName.Inbox), new FolderId(WellKnownFolderName.DeletedItems));
            //                        continue;
            //                    }
            //                    else
            //                    {
            //                        ExchangeManager.DeleteMeetingRequest(service, conversation.ItemIds[0].UniqueId.ToString());
            //                        continue;
            //                    }
            //                }

            //                // Fetch All the available conversation messages in it.
            //                List<EmailMessage> messages = ExchangeManager.BatchGetEmailItems(service, conversation.GlobalItemIds.ToList());

            //                //Validate if conversation already exists.
            //                bool IsAddTicket = false;
            //                int statusid = 1;
            //                var ticket = db.Ticket.Where(t => t.conversationid == conversation.Id).FirstOrDefault();
            //                if (ticket == null)
            //                {
            //                    IsAddTicket = true;

            //                    string fromEmail = Convert.ToString(messages.First().From.Address);
            //                    // check if conversation is going on with same sender && topic && today date part
            //                    var Tempticket = db.Ticket.Where(t => t.fromEmail == fromEmail &&  t.topic.ToLower().Equals(conversation.Topic.ToLower(), StringComparison.OrdinalIgnoreCase)).ToList();
            //                    //Tempticket = Tempticket.Where(x => x.lastdeliverytime.Date == DateTime.Now.Date).ToList();

            //                    // to check if previous sender is now in receiver's list in case of replay
            //                    if (Tempticket.Count() == 0) 
            //                    {
            //                        string toEmail = Convert.ToString(string.Join(";", messages.First().ToRecipients.Select(x => x.Address).ToList()));
            //                        Tempticket = db.Ticket.Where(t => (t.fromEmail == fromEmail || t.fromEmail.Contains(toEmail)) && t.topic.ToLower().Equals(conversation.Topic.ToLower(), StringComparison.OrdinalIgnoreCase)).ToList();
            //                    }

            //                    // try to fetch the conversation (ticket) by ticket# in subject/topic.
            //                    if (Tempticket.Count() == 0)
            //                    {
            //                        // validate if ticket # exists in the subject. which means email has been replied from timesheet.
            //                        int TicketNoStartIndex = conversation.Topic.ToLower().IndexOf("[ticket#");
            //                        if (TicketNoStartIndex != -1)
            //                        {
            //                            TicketNoStartIndex += 8; // add +8 to exclude [ticket#
            //                            string ticketid = conversation.Topic.ToLower().Substring(TicketNoStartIndex, conversation.Topic.Length - (TicketNoStartIndex + 1));
            //                            if(ticketid != null)
            //                            {
            //                                if (ticketid.Contains(']'))
            //                                {
            //                                    ticketid = ticketid.Split(']')[0];
            //                                }
            //                                long TicketId = Convert.ToInt64(ticketid);
            //                                var ticketexist = db.Ticket.Where(t => t.id == TicketId).FirstOrDefault();
            //                                if(ticketexist != null)
            //                                { 
            //                                    Tempticket.Add(ticketexist);
            //                                }
            //                            }
            //                        }
            //                    }

            //                    //merging the ticket, if the topic of ExchangeWebServer ticket and Timesheet ticket  are same 80%
            //                    //if (Tempticket.Count() == 0)
            //                    //{
            //                    //    //fetching the same fromEmail to last 30 days from timesheet database
            //                    //    var fromemail = messages.First().From.Address;
            //                    //    var RecentDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);
            //                    //    var PreviouMonthDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0).AddMonths(-1);
            //                    //    var SameConversations = db.Ticket.Where(t => (t.fromEmail == fromemail && t.createdonutc >= RecentDate) || (t.fromEmail == fromemail && t.createdonutc >= PreviouMonthDate)).OrderByDescending(t=>t.id).ToList();

            //                        //if(SameConversations != null)
            //                        //{ 
            //                        //    foreach (var SameConversation in SameConversations)
            //                        //    {
            //                        //        StringSift2 stringsift2 = new StringSift2();
            //                        //        float percent = stringsift2.Similarity(SameConversation.topic, conversation.Topic);
            //                        //        if(percent >= 0.80)
            //                        //        {
            //                        //            Tempticket.Add(SameConversation);
            //                        //            break;
            //                        //        }
            //                        //    }
            //                        //}
            //                    //}

            //                    if (Tempticket.Count() > 0)
            //                    {
            //                        IsAddTicket = false;
            //                        ticket = Tempticket.First();
            //                        ticket.messagecount = conversation.GlobalMessageCount + ticket.messagecount;
            //                    }
            //                    else
            //                    {
            //                        ticket = new Ticket();
            //                        ticket.conversationid = conversation.Id;
            //                        ticket.messagecount = conversation.GlobalMessageCount;
            //                        ticket.fromEmail = messages.First().From.Address;
            //                    }
            //                }
            //                else
            //                {
            //                    ticket.fromEmail = "default";
            //                }

            //                var temp = (from ticketItems in db.TicketItem
            //                            join ticketItemsLog in db.TicketItemLog on
            //                            new { ticketitem = ticketItems.id, ticket = ticketItems.ticketid } equals
            //                            new { ticketitem = ticketItemsLog.ticketitemid, ticket = ticket.id }
            //                            select ticketItemsLog).FirstOrDefault();
            //                if (temp != null)
            //                {
            //                    statusid = 2;
            //                }

            //                ticket.tickettypeid = 1;
            //                ticket.uniquesenders = conversation.GlobalUniqueSenders.ToString();
            //                ticket.topic = conversation.Topic;
            //                ticket.lastdeliverytime = conversation.GlobalLastDeliveryTime;
            //                ticket.size = conversation.GlobalSize;
            //                ticket.hasattachments = conversation.GlobalHasAttachments;
            //                switch (conversation.GlobalImportance)
            //                {
            //                    case Importance.High:
            //                        ticket.importance = true;
            //                        break;
            //                    default:
            //                        ticket.importance = false;
            //                        break;
            //                }
            //                switch (conversation.GlobalFlagStatus)
            //                {
            //                    case ConversationFlagStatus.NotFlagged:
            //                        ticket.flagstatusid = 1;
            //                        break;
            //                    case ConversationFlagStatus.Flagged:
            //                        ticket.flagstatusid = 2;
            //                        break;
            //                    case ConversationFlagStatus.Complete:
            //                        ticket.flagstatusid = 3;
            //                        break;
            //                }
            //                ticket.lastmodifiedtime = System.DateTime.Now;
            //                ticket.statusid = statusid;
            //                ticket.statusupdatedbyusersid = User.Identity.GetUserId();
            //                ticket.statusupdatedon = System.DateTime.Now;
            //                ticket.createdonutc = System.DateTime.Now;
            //                ticket.updatedonutc = System.DateTime.Now;
            //                ticket.ipused = Request.UserHostAddress;
            //                ticket.userid = User.Identity.GetUserId();
            //                ticket.IsArchieved = false;

            //                if (IsAddTicket)
            //                {
            //                    db.Ticket.Add(ticket);
            //                    db.SaveChanges();                                
            //                }
            //                else
            //                {
            //                    db.Entry(ticket).State = EntityState.Modified;
            //                    db.SaveChanges();
            //                }

            //                // Save Messages
            //                SaveMessages(ticket, messages, IsAddTicket);

            //                // Add Contacts (From, To, CC, BCC)
            //                if (messages != null)
            //                {
            //                    foreach (EmailMessage message in messages)
            //                    {
            //                        // From Address
            //                        SaveContactandConactDomain(message.From.Address, message.From.Name);

            //                        // To Addresses
            //                        if (message.ToRecipients != null)
            //                        {
            //                            foreach (EmailAddress To in message.ToRecipients)
            //                            {
            //                                SaveContactandConactDomain(To.Address, To.Name);
            //                            }
            //                        }

            //                        // CC Addresses
            //                        if (message.CcRecipients != null)
            //                        {
            //                            foreach (EmailAddress Cc in message.CcRecipients)
            //                            {
            //                                SaveContactandConactDomain(Cc.Address, Cc.Name);
            //                            }
            //                        }

            //                        // BCC Addresses
            //                        if (message.BccRecipients != null)
            //                        {
            //                            foreach (EmailAddress Bcc in message.BccRecipients)
            //                            {
            //                                SaveContactandConactDomain(Bcc.Address, Bcc.Name);
            //                            }
            //                        }

            //                    }
            //                }

            //            }
            //            catch (Exception ex)
            //            {
            //                MailService.SendClientEmail(ConfigurationManager.AppSettings["ToNotificatoins"].ToString(), "Timesheet-Error", "There is an error on timesheet email syncing.<br>email subject is" + conversation.Topic + "<br>Please go to websupport owa account and check the issue<br>Here is the stack strace:<br>" + ex.ToString(), null);
            //                var rd = ControllerContext.RouteData;
            //                var currentAction = rd.GetRequiredString("action");
            //                var currentController = rd.GetRequiredString("controller");
            //                MyExceptions myex = new MyExceptions();
            //                myex.action = currentAction;
            //                myex.exceptiondate = DateTime.Now;
            //                myex.controller = currentController;
            //                myex.exception_message = ex.Message;
            //                myex.exception_source = ex.Source;
            //                myex.exception_stracktrace = ex.StackTrace;
            //                myex.exception_targetsite = ex.TargetSite.DeclaringType.FullName + ", " + ex.TargetSite.Name;
            //                myex.ipused = Request.UserHostAddress;
            //                myex.userid = User.Identity.GetUserId();
            //                db.MyExceptions.Add(myex);
            //                db.SaveChanges();
            //                continue;
            //            }
            //        }
            //    }

            //    ViewBag.exception = "Done!";
            //    return RedirectToAction("Index","tickets", new { @id=1});
            //}
            //catch (Exception ex)
            //{
            //    MailService.SendClientEmail(ConfigurationManager.AppSettings["ToNotificatoins"].ToString(), "Timesheet-Error", "There is an error on timesheet email syncing.<br>Please go to websupport owa account and check the issue.<br>Here is the stack strace:<br>" + ex.ToString(), null);
            //    var rd = ControllerContext.RouteData;
            //    var currentAction = rd.GetRequiredString("action");
            //    var currentController = rd.GetRequiredString("controller");
            //    MyExceptions myex = new MyExceptions();
            //    myex.action = currentAction;
            //    myex.exceptiondate = DateTime.Now;
            //    myex.controller = currentController;
            //    myex.exception_message = ex.Message;
            //    myex.exception_source = ex.Source;
            //    myex.exception_stracktrace = ex.StackTrace;
            //    myex.exception_targetsite = ex.TargetSite.DeclaringType.FullName + ", " + ex.TargetSite.Name;
            //    myex.ipused = Request.UserHostAddress;
            //    myex.userid = User.Identity.GetUserId();
            //    db.MyExceptions.Add(myex);
            //    db.SaveChanges();
            //    ViewBag.exception = ex.Message;
            //    return View();
            //}
            return RedirectToAction("Index", "tickets", new { id = 1 });
        }

        private void SaveMessages(Ticket ticket, List<EmailMessage> messages, bool IsAddTicket)
        {
            try
            {
                // Validate if any conversation messages are available.
                if (messages != null && messages.Count > 0)
                {
                    foreach (EmailMessage message in messages.OrderBy(x => x.DateTimeCreated))
                    {
                        try
                        {
                            int a = 1;
                            if (a == 1)
                            {
                                bool IsAddTicketItem = false;
                                TicketItem item = null;

                                // If Conversation already exists and message also exists, find if message is already available in database or not.
                                if (!IsAddTicket && message != null)
                                {
                                    item = db.TicketItem.Where(ti =>
                                            ti.ticketid == ticket.id && ti.datetimecreated == message.DateTimeCreated &&
                                            ti.from == message.From.Address && ti.subject == message.Subject)
                                        .FirstOrDefault();
                                    if (item != null)
                                    {
                                        //Item already exists, simply delete it and move to next message.
                                        message.Delete(DeleteMode.MoveToDeletedItems);
                                        continue;
                                    }

                                    IsAddTicketItem = true;
                                }
                                else
                                {
                                    IsAddTicketItem = true;
                                }

                                if (IsAddTicketItem)
                                {
                                    item = new TicketItem();
                                }

                                item.ticketid = ticket.id;
                                item.conversationid = message.ConversationId;
                                foreach (EmailAddress emailmsg in message.BccRecipients)
                                {
                                    item.bccrecipients += emailmsg.Address + ";";
                                }

                                if (!string.IsNullOrEmpty(item.bccrecipients))
                                {
                                    item.bccrecipients = item.bccrecipients.TrimEnd(";".ToCharArray());
                                }

                                item.body = message.Body;
                                item.emailmessageid = message.Id.UniqueId;
                                item.mimecontent = message.MimeContent.CharacterSet;
                                item.conversationid = message.ConversationId;
                                item.conversationindex = Encoding.Default.GetString(message.ConversationIndex);
                                item.conversationtopic = message.ConversationTopic;
                                item.datetimecreated = message.DateTimeCreated;
                                item.datetimereceived = message.DateTimeReceived;
                                item.datetimesent = message.DateTimeSent;
                                item.displaycc = message.DisplayCc;
                                item.displayto = message.DisplayTo;
                                item.from = message.From.Address;
                                item.hasattachments = message.HasAttachments;
                                switch (message.Importance)
                                {
                                    case Importance.High:
                                        item.importance = 1;
                                        break;
                                    case Importance.Normal:
                                        item.importance = 2;
                                        break;
                                    case Importance.Low:
                                        item.importance = 3;
                                        break;
                                    default:
                                        item.importance = 2;
                                        break;
                                }

                                item.inreplyto = message.InReplyTo;
                                if (message.InternetMessageHeaders != null && message.InternetMessageHeaders.Count > 0)
                                {
                                    foreach (InternetMessageHeader internetmsgheader in message.InternetMessageHeaders)
                                    {
                                        item.internetmessageheaders += internetmsgheader.Name + "=" +
                                                                       internetmsgheader.Value + ";";
                                    }
                                }

                                if (!string.IsNullOrEmpty(item.internetmessageheaders))
                                {
                                    item.internetmessageheaders =
                                        item.internetmessageheaders.TrimEnd(";".ToCharArray());
                                }

                                item.internetmessageid = message.InternetMessageId;
                                item.lastmodifiedname = message.LastModifiedName;
                                item.lastmodifiedtime = message.LastModifiedTime;
                                item.mimecontent = message.MimeContent.CharacterSet;
                                item.replyto = message.InReplyTo;
                                switch (message.Sensitivity)
                                {
                                    case Sensitivity.Normal:
                                        item.sensitivity = 1;
                                        break;
                                    case Sensitivity.Personal:
                                        item.sensitivity = 2;
                                        break;
                                    case Sensitivity.Private:
                                        item.sensitivity = 3;
                                        break;
                                    case Sensitivity.Confidential:
                                        item.sensitivity = 4;
                                        break;
                                }

                                item.size = message.Size;
                                item.subject = message.Subject;
                                foreach (EmailAddress ToRecipients in message.ToRecipients)
                                {
                                    item.torecipients += ToRecipients.Address + ";";
                                }

                                if (!string.IsNullOrEmpty(item.torecipients))
                                {
                                    item.torecipients = item.torecipients.TrimEnd(";".ToCharArray());
                                }

                                item.uniquebody = message.UniqueBody;
                                item.statusid = 1;
                                item.statusupdatedbyusersid = User.Identity.GetUserId();
                                item.statusupdatedon = DateTime.Now;
                                item.quotedtimeinminutes = 0;
                                item.createdonutc = DateTime.Now;
                                item.updatedonutc = DateTime.Now;
                                foreach (EmailAddress CcRecipients in message.CcRecipients)
                                {
                                    item.ccrecipients += CcRecipients.Address + ";";
                                }

                                if (!string.IsNullOrEmpty(item.ccrecipients))
                                {
                                    item.ccrecipients = item.ccrecipients.TrimEnd(";".ToCharArray());
                                }

                                item.ipused = Request.UserHostAddress;
                                item.userid = User.Identity.GetUserId();

                                if (IsAddTicketItem)
                                {
                                    db.TicketItem.Add(item);
                                    db.SaveChanges();

                                    // Fetch Attachments if its a new item.
                                    DownloadAttachments(message, item);
                                }

                                // Delete Message From Conversation.
                                message.Delete(DeleteMode.MoveToDeletedItems);
                            }
                        }
                        catch (Exception ex)
                        {
                            MailService.SendClientEmail(ConfigurationManager.AppSettings["ToNotificatoins"],
                                "Timesheet-Error",
                                "There is an error on timesheet email syncing.<br>Email subject is" + ticket.topic +
                                "<br>Please go to websupport owa account and check the issue.<br>Here is the stack strace:<br>" +
                                ex, null);
                            System.Web.Routing.RouteData rd = ControllerContext.RouteData;
                            string currentAction = rd.GetRequiredString("action");
                            string currentController = rd.GetRequiredString("controller");
                            MyExceptions myex = new MyExceptions
                            {
                                action = currentAction,
                                exceptiondate = DateTime.Now,
                                controller = currentController,
                                exception_message = ex.Message,
                                exception_source = ex.Source,
                                exception_stracktrace = ex.StackTrace,
                                exception_targetsite = ex.TargetSite.DeclaringType.FullName + ", " + ex.TargetSite.Name,
                                ipused = Request.UserHostAddress,
                                userid = User.Identity.GetUserId()
                            };
                            db.MyExceptions.Add(myex);
                            db.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public void SaveContactandConactDomain(string emailAddress, string displayName)
        {
            try
            {
                // Add/Update ContactCompany
                if (!string.IsNullOrEmpty(emailAddress))
                {
                    string[] emailElement = emailAddress.Split('@');

                    string Contactdomain = string.Empty;
                    if (emailElement.Length > 0)
                    {
                        Contactdomain = emailElement[1];
                    }

                    // Add ContactCompany if doesn't exists.
                    ContactCompany objContactCompany = db.ContactCompany.Where(x => x.name == Contactdomain).FirstOrDefault();
                    if (objContactCompany == null)
                    {
                        objContactCompany = new ContactCompany
                        {
                            name = Contactdomain
                        };
                        db.ContactCompany.Add(objContactCompany);
                        db.SaveChanges();
                    }

                    // Add Contact if doesn't exists.
                    Contact objContact = db.Contact.Where(x => x.Email == emailAddress).FirstOrDefault();

                    if (objContact == null)
                    {
                        Contact objcontact = new Contact
                        {
                            Email = emailAddress,
                            DisplayName = displayName,
                            contactdomainid = objContactCompany.id,
                            createdonutc = DateTime.Now,
                            ipused = Request.UserHostAddress,
                            isactive = true,
                            updatedonutc = DateTime.Now,
                            userid = User.Identity.GetUserId()
                        };
                        db.Contact.Add(objcontact);
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        private void DownloadAttachments(EmailMessage message, TicketItem item)
        {
            try
            {
                if (message.Attachments != null && message.Attachments.Count > 0)
                {
                    // Create Dictionary of all the inline attachments.
                    Dictionary<string, string> inlineAttachments = new Dictionary<string, string>();

                    foreach (Attachment attachment in message.Attachments)
                    {
                        try
                        {
                            FileAttachment fileAttachment = attachment as FileAttachment;
                            if (fileAttachment != null)
                            {
                                // Create Attachment Directory.
                                if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "/Attachments/" +
                                                      item.id))
                                {
                                    Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "/Attachments/" +
                                                              item.id);
                                }

                                // Validate if Attachment already exists in database.
                                bool itemAttachmentExists = false;

                                TicketItemAttachment itemAttachment = db.TicketItemAttachment.Where(i =>
                                    i.ticketitemid == item.id && i.attachmentid == fileAttachment.Id).FirstOrDefault();
                                if (itemAttachment == null)
                                {
                                    itemAttachment = new TicketItemAttachment();
                                }
                                else
                                {
                                    itemAttachmentExists = true;
                                }

                                itemAttachment.ticketitemid = item.id;
                                itemAttachment.contentid = fileAttachment.ContentId;
                                itemAttachment.contentlocation = fileAttachment.ContentLocation;
                                itemAttachment.name = fileAttachment.Name;
                                itemAttachment.attachmentid = fileAttachment.Id;

                                // Download attachment in appropriate folders and set path for the filename.
                                if (fileAttachment.IsInline)
                                {
                                    // Create Inline Directory within Attachment Directory.
                                    if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "/Attachments/" +
                                                          item.id + "/Inline"))
                                    {
                                        Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory +
                                                                  "/Attachments/" + item.id + "/Inline");
                                    }

                                    // Download Attachment into inline folder.
                                    fileAttachment.Load(AppDomain.CurrentDomain.BaseDirectory + "/Attachments/" +
                                                        item.id + "/Inline/" + fileAttachment.Name);

                                    itemAttachment.contenttype = "Inline";
                                    itemAttachment.path = "/Attachments/" + item.id + "/Inline/" + fileAttachment.Name;

                                    inlineAttachments.Add(fileAttachment.ContentId, itemAttachment.path);
                                }
                                else
                                {
                                    // Download Attachment into inline folder.
                                    fileAttachment.Load(AppDomain.CurrentDomain.BaseDirectory + "/Attachments/" +
                                                        item.id + "/" + fileAttachment.Name);

                                    itemAttachment.contenttype = fileAttachment.ContentType;
                                    itemAttachment.path = "/Attachments/" + item.id + "/" + fileAttachment.Name;
                                }

                                if (itemAttachmentExists)
                                {
                                    db.Entry(itemAttachment).State = EntityState.Modified;
                                    db.SaveChanges();
                                }
                                else
                                {
                                    db.TicketItemAttachment.Add(itemAttachment);
                                    db.SaveChanges();
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            System.Web.Routing.RouteData rd = ControllerContext.RouteData;
                            string currentAction = rd.GetRequiredString("action");
                            string currentController = rd.GetRequiredString("controller");
                            MyExceptions myex = new MyExceptions
                            {
                                action = currentAction,
                                exceptiondate = DateTime.Now,
                                controller = currentController,
                                exception_message = ex.Message,
                                exception_source = ex.Source,
                                exception_stracktrace = ex.StackTrace,
                                exception_targetsite = ex.TargetSite.DeclaringType.FullName + ", " + ex.TargetSite.Name,
                                ipused = Request.UserHostAddress,
                                userid = User.Identity.GetUserId()
                            };
                            db.MyExceptions.Add(myex);
                            db.SaveChanges();
                        }
                    }

                    // Find All inline attachments and replace with image paths.
                    if (inlineAttachments != null && inlineAttachments.Count > 0)
                    {
                        foreach (KeyValuePair<string, string> iattach in inlineAttachments)
                        {
                            item.body = item.body.Replace("cid:" + iattach.Key, iattach.Value);
                            item.uniquebody = item.uniquebody.Replace("cid:" + iattach.Key, iattach.Value);
                        }

                        db.Entry(item).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Manually checking the percentage of Similarity between two strings
        //public double CompareStrings(String string1, String string2)
        //{
        //    StringSift2 stringSift2 = new StringSift2();
        //    double percentage = stringSift2.Similarity(string1, string2);
        //    return percentage;
        //}
    }
}

//Comparing Two String Percentage wise
//public class StringSift2 
//{
//    //1st Technique
//    private int maxOffset;
//    public StringSift2() : this(5) { }
//    public StringSift2(int maxOffset)
//    {
//        this.maxOffset = maxOffset;
//    }
//    public float Distance(string s1, string s2)
//    {
//        if (String.IsNullOrEmpty(s1))
//            return
//            String.IsNullOrEmpty(s2) ? 0 : s2.Length;
//        if (String.IsNullOrEmpty(s2))
//            return s1.Length;
//        int c = 0;
//        int offset1 = 0;
//        int offset2 = 0;
//        int dist = 0;
//        while ((c + offset1 < s1.Length)
//        && (c + offset2 < s2.Length))
//        {
//            if (s1[c + offset1] != s2[c + offset2])
//            {
//                offset1 = 0;
//                offset2 = 0;
//                for (int i = 0; i < maxOffset; i++)
//                {
//                    if ((c + i < s1.Length)
//                    && (s1[c + i] == s2[c]))
//                    {
//                        if (i > 0)
//                        {
//                            dist++;
//                            offset1 = i;
//                        }
//                        goto ender;
//                    }
//                    if ((c + i < s2.Length)
//                    && (s1[c] == s2[c + i]))
//                    {
//                        if (i > 0)
//                        {
//                            dist++;
//                            offset2 = i;
//                        }
//                        goto ender;
//                    }
//                }
//                dist++;
//            }
//        ender:
//            c++;
//        }
//        return dist + (s1.Length - offset1
//        + s2.Length - offset2) / 2 - c;
//    }
//    public float Similarity(string s1, string s2)
//        {
//            float dis = Distance(s1, s2);
//            int maxLen = Math.Max(s1.Length, s2.Length);
//            if (maxLen == 0) return 1;
//            else
//            return 1 - dis / maxLen;
//        }

//    //2nd Technique
//    public double Compare(string str1, string str2)
//    {
//        int count = str1.Length > str2.Length ? str1.Length : str2.Length;
//        int hits = 0;
//        int i, j; i = 0; j = 0;
//        for (i = 0; i <= str1.Length - 1; i++)
//        {
//            if (str1[i] == ' ')
//            {
//                i += 1; j = str2.IndexOf(' ', j) + 1; hits += 1;
//            }
//            while (j < str2.Length && str2[j] != ' ')
//            {
//                if (str1[i] == str2[j])
//                {
//                    hits += 1;
//                    j += 1;
//                    break;
//                }
//                else
//                    j += 1;
//            }
//            if (!(j < str2.Length && str2[j] != ' '))
//                j -= 1;
//        }
//        return Math.Round((hits / (double)count), 2);
//    }

//    //3rd Technique
//    public double CalculateSimilarity(string source, string target)
//    {
//        if ((source == null) || (target == null)) return 0.0;
//        if ((source.Length == 0) || (target.Length == 0)) return 0.0;
//        if (source == target) return 1.0;

//        int stepsToSame = ComputeLevenshteinDistance(source, target);
//        return (1.0 - ((double)stepsToSame / (double)Math.Max(source.Length, target.Length)));
//    }
//    int ComputeLevenshteinDistance(string source, string target)
//    {
//        if ((source == null) || (target == null)) return 0;
//        if ((source.Length == 0) || (target.Length == 0)) return 0;
//        if (source == target) return source.Length;

//        int sourceWordCount = source.Length;
//        int targetWordCount = target.Length;

//        // Step 1
//        if (sourceWordCount == 0)
//            return targetWordCount;

//        if (targetWordCount == 0)
//            return sourceWordCount;

//        int[,] distance = new int[sourceWordCount + 1, targetWordCount + 1];

//        // Step 2
//        for (int i = 0; i <= sourceWordCount; distance[i, 0] = i++) ;
//        for (int j = 0; j <= targetWordCount; distance[0, j] = j++) ;

//        for (int i = 1; i <= sourceWordCount; i++)
//        {
//            for (int j = 1; j <= targetWordCount; j++)
//            {
//                // Step 3
//                int cost = (target[j - 1] == source[i - 1]) ? 0 : 1;

//                // Step 4
//                distance[i, j] = Math.Min(Math.Min(distance[i - 1, j] + 1, distance[i, j - 1] + 1), distance[i - 1, j - 1] + cost);
//            }
//        }

//        return distance[sourceWordCount, targetWordCount];
//    }

//}