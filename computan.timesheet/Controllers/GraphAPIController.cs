using computan.timesheet.Contexts;
using computan.timesheet.core;
using computan.timesheet.core.common;
using computan.timesheet.Infrastructure;
using computan.timesheet.Models;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace computan.timesheet.Controllers
{
    public class GraphAPIController : Controller
    {
        //private static readonly string resource = ConfigurationManager.AppSettings["resource"];
        private static readonly string client_secret = ConfigurationManager.AppSettings["client_secret"];

        private static readonly string client_id = ConfigurationManager.AppSettings["client_id"];

        private static readonly string grant_type = ConfigurationManager.AppSettings["grant_type"];

        //private static readonly string username = ConfigurationManager.AppSettings["username"];
        //private static readonly string password = ConfigurationManager.AppSettings["password"];
        private static readonly string redirect_uri = ConfigurationManager.AppSettings["redirect_uri"];

        private readonly ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> Mails()
        {
            //fetch the top one delta link from database that we saved from every request we sent to graph API
            NextLink nextlinkdb = db.NextLink.OrderByDescending(x => x.id).FirstOrDefault();
            string Deltalink = string.Empty;
            string RequestURL = string.Empty;
            string output = string.Empty;
            GraphMessage messages = new GraphMessage();

            Integration graphApiIntegration = db.integration
                .Where(x => x.name == IntegrationSystem.GraphApi.ToString() && x.isenabled).FirstOrDefault();
            //Sent request to microsoft online to get bearer token against our App
            //RestClient client = new RestClient("https://login.microsoftonline.com/computan.onmicrosoft.com/oauth2/token");
            RestClient client = new RestClient("https://login.microsoftonline.com/computan.onmicrosoft.com/oauth2/v2.0/token");
            RestRequest request = new RestRequest(Method.POST);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            //request.AddParameter("resource", resource);
            //request.AddParameter("client_id", client_id);
            //request.AddParameter("client_secret", client_secret);
            //request.AddParameter("grant_type", grant_type);
            //request.AddParameter("username", username);
            //request.AddParameter("password", password);
            request.AddParameter("redirect_uri", redirect_uri);
            request.AddParameter("client_id", client_id);
            request.AddParameter("client_secret", client_secret);
            request.AddParameter("grant_type", grant_type);
            request.AddParameter("refresh_token", graphApiIntegration?.appsettings);
            IRestResponse response = client.Execute(request);

            //convert token response to json to get token
            dynamic Content = JsonConvert.DeserializeObject(response.Content);
            string token = Content.access_token;
            string refresh_token = Content.refresh_token;

            if (!string.IsNullOrEmpty(refresh_token))
            {
                graphApiIntegration.appsettings = refresh_token;
                db.SaveChanges();
            }

            //Assign top one delta link to variable to sent a HTPP request to Graph API
            if (nextlinkdb != null)
            {
                RequestURL = nextlinkdb.url;
            }
            //RequestURL = "https://graph.microsoft.com/v1.0/me/mailFolders/Inbox/messages/delta?$orderby=receivedDateTime+DESC&$top=10";
            else
            {
                RequestURL = "https://graph.microsoft.com/v1.0/me/mailFolders/Inbox/messages/delta";
            }
            //RequestURL = "https://graph.microsoft.com/v1.0/me/mailFolders/Inbox/messages/delta?$select=subject,bodyPreview,header&$orderby=receivedDateTime+DESC&$top=10";

            //Now call the Graph API
            HttpClient clientRequest = new HttpClient();
            HttpRequestMessage httprequest = new HttpRequestMessage(HttpMethod.Get, RequestURL);
            httprequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            HttpResponseMessage httpresponse = await clientRequest.SendAsync(httprequest);
            try
            {
                if (httpresponse.StatusCode == HttpStatusCode.OK)
                {
                    output = await httpresponse.Content.ReadAsStringAsync();
                    messages = JsonConvert.DeserializeObject<GraphMessage>(output);
                    GraphAttachments attachments = new GraphAttachments();
                    foreach (message conversation in messages.value)
                    {
                        HttpClient clientRequestAttachement = new HttpClient();
                        HttpRequestMessage httprequestAttachement = new HttpRequestMessage(HttpMethod.Get,
                            "https://graph.microsoft.com/v1.0/me/messages/" + conversation.id + "/attachments");
                        httprequestAttachement.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                        HttpResponseMessage httpresponseAttachment = await clientRequestAttachement.SendAsync(httprequestAttachement);
                        string output1 = await httpresponseAttachment.Content.ReadAsStringAsync();
                        attachments = JsonConvert.DeserializeObject<GraphAttachments>(output1);
                        conversation.GraphAttachment = attachments.value;
                    }

                    //check if messages response is not null
                    if (messages != null)
                    {
                        //check if messages has values( as emails) and count > 0
                        if (messages.value != null && messages.value.Count > 0)
                        {
                            foreach (message conversation in messages.value)
                            {
                                //to check if email has value
                                if (conversation.from != null)
                                {
                                    //to check if message is not an event meesage
                                    if (conversation.type != "#microsoft.graph.eventMessage")
                                    {
                                        //Validate if conversation already exists.
                                        bool IsAddTicket = false;
                                        bool IsTicketMerge = false;
                                        //string internetticketid = string.Empty;
                                        int statusid = 1;

                                        Ticket ticket = db.Ticket
                                            .Where(t => t.conversationid == conversation.conversationId)
                                            .FirstOrDefault();
                                        if (ticket == null)
                                        {
                                            IsAddTicket = true;

                                            string fromEmail = conversation.from.emailAddress.address;
                                            //var Tempticket = db.Ticket.Where(t => t.fromEmail == fromEmail && t.topic.ToLower().Equals(conversation.subject.ToLower(), StringComparison.OrdinalIgnoreCase)).ToList();
                                            List<Ticket> Tempticket = db.Ticket.Where(t =>
                                                t.fromEmail == fromEmail && t.topic.Equals(conversation.subject,
                                                    StringComparison.OrdinalIgnoreCase)).ToList();
                                            //if (Tempticket.Count()==0)
                                            //{
                                            //    // check if conversation is going on with same sender && topic && today date part
                                            //    Tempticket = db.Ticket.Where(t => t.fromEmail == fromEmail && t.topic.ToLower().Equals(conversation.subject.ToLower(), StringComparison.OrdinalIgnoreCase)).ToList();
                                            //}
                                            //Tempticket = Tempticket.Where(x => x.lastdeliverytime.Date == DateTime.Now.Date).ToList();

                                            // to check if previous sender is now in receiver's list in case of replay
                                            if (Tempticket.Count() == 0)
                                            {
                                                string toEmail = Convert.ToString(string.Join(";",
                                                    conversation.toRecipients.Select(x => x.emailAddress.address)
                                                        .ToList()));
                                                //var Tempticket = db.Ticket.Where(t => (t.fromEmail == fromEmail || t.fromEmail.Contains(toEmail)) && t.topic.ToLower().Equals(conversation.subject.ToLower(), StringComparison.OrdinalIgnoreCase)).ToList();
                                                Tempticket = db.Ticket.Where(t =>
                                                    (t.fromEmail == fromEmail || t.fromEmail.Contains(toEmail)) &&
                                                    t.topic.Equals(conversation.subject,
                                                        StringComparison.OrdinalIgnoreCase)).ToList();
                                            }

                                            // try to fetch the conversation (ticket) by ticket# in subject/topic.
                                            if (Tempticket.Count() == 0)
                                            {
                                                // validate if ticket # exists in the subject. which means email has been replied from timesheet.

                                                //int TicketNoStartIndex = conversation.subject.ToLower().IndexOf("[ticket#");
                                                int TicketNoStartIndex = -1;
                                                if (!string.IsNullOrEmpty(conversation.subject))
                                                {
                                                    TicketNoStartIndex = conversation.subject.ToLower()
                                                        .IndexOf("[ticket#");
                                                }

                                                if (TicketNoStartIndex != -1)
                                                {
                                                    TicketNoStartIndex += 8; // add +8 to exclude [ticket#
                                                    string ticketid = conversation.subject.ToLower()
                                                        .Substring(TicketNoStartIndex,
                                                            conversation.subject.Length - (TicketNoStartIndex + 1));
                                                    if (ticketid != null)
                                                    {
                                                        //if any word has been added after the ticket# Id then we split by ']', after this we will have ticket id
                                                        if (ticketid.Contains(']'))
                                                        {
                                                            ticketid = ticketid.Split(']')[0];
                                                        }

                                                        long TicketId = Convert.ToInt64(ticketid);
                                                        Ticket ticketexist = db.Ticket.Where(t => t.id == TicketId)
                                                            .FirstOrDefault();
                                                        if (ticketexist != null)
                                                        {
                                                            Tempticket.Add(ticketexist);
                                                        }
                                                    }
                                                }
                                            }

                                            if (Tempticket.Count() > 0)
                                            {
                                                IsAddTicket = false;
                                                IsTicketMerge = true;
                                                ticket = Tempticket.First();
                                                //ticket.messagecount = conversation.GlobalMessageCount + ticket.messagecount;
                                            }
                                            else
                                            {
                                                ticket = new Ticket
                                                {
                                                    conversationid = conversation.conversationId,
                                                    messagecount = 1,
                                                    fromEmail = conversation.from.emailAddress.address
                                                };
                                            }
                                        }
                                        else
                                        {
                                            ticket.fromEmail = "default";
                                        }

                                        TicketItemLog temp = (from ticketItems in db.TicketItem
                                                              join ticketItemsLog in db.TicketItemLog on
                                                                  new { ticketitem = ticketItems.id, ticket = ticketItems.ticketid } equals
                                                                  new { ticketitem = ticketItemsLog.ticketitemid, ticket = ticket.id }
                                                              select ticketItemsLog).FirstOrDefault();
                                        if (temp != null)
                                        {
                                            statusid = 2;
                                        }

                                        ticket.tickettypeid = 1;
                                        ticket.uniquesenders = conversation.sender.emailAddress.name;
                                        ticket.topic = conversation.subject;
                                        ticket.lastdeliverytime = conversation.receivedDateTime;
                                        //ticket.size = conversation.;
                                        ticket.hasattachments = conversation.hasAttachments;
                                        switch (conversation.importance)
                                        {
                                            case "important":
                                                ticket.importance = true;
                                                break;

                                            case "normal":
                                                ticket.importance = false;
                                                break;

                                            default:
                                                ticket.importance = false;
                                                break;
                                        }

                                        switch (conversation.flag.flagStatus)
                                        {
                                            case "notFlagged":
                                                ticket.flagstatusid = 1;
                                                break;

                                            case "Flagged":
                                                ticket.flagstatusid = 2;
                                                break;

                                            case "Complete":
                                                ticket.flagstatusid = 3;
                                                break;

                                            default:
                                                ticket.flagstatusid = 1;
                                                break;
                                        }

                                        ticket.lastmodifiedtime = DateTime.Now;
                                        ticket.statusid = statusid;
                                        ticket.statusupdatedbyusersid = User.Identity.GetUserId();
                                        ticket.statusupdatedon = DateTime.Now;
                                        //ticket.LastActivityDate = DateTime.Now;
                                        ticket.createdonutc = DateTime.Now;
                                        ticket.updatedonutc = DateTime.Now;
                                        ticket.ipused = Request.UserHostAddress;
                                        ticket.userid = User.Identity.GetUserId();
                                        ticket.IsArchieved = false;
                                        if (conversation.from.emailAddress.address.Contains("@computan"))
                                        {
                                            ticket.LastActivityDate = DateTime.Now;
                                        }

                                        if (IsAddTicket)
                                        {
                                            db.Ticket.Add(ticket);
                                            await db.SaveChangesAsync();
                                        }
                                        else
                                        {
                                            db.Entry(ticket).State = EntityState.Modified;
                                            db.SaveChanges();
                                        }

                                        // Save Messages
                                        SaveMessages(ticket, conversation, IsAddTicket, token, IsTicketMerge);
                                    }
                                }
                            }
                        }

                        //save nextlink into database
                        NextLink nextlink = new NextLink();
                        if (!string.IsNullOrEmpty(messages.nextLink))
                        {
                            Deltalink = messages.nextLink;
                            nextlink.url = Deltalink;
                            nextlink.createdat = DateTime.Now;
                            db.NextLink.Add(nextlink);
                            db.SaveChanges();
                        }
                        else if (!string.IsNullOrEmpty(messages.deltaLink))
                        {
                            Deltalink = messages.deltaLink;
                            nextlink.url = Deltalink;
                            nextlink.createdat = DateTime.Now;
                            db.NextLink.Add(nextlink);
                            db.SaveChanges();
                        }

                        // Add Contacts (From, To, CC, BCC)
                        if (messages != null)
                        {
                            foreach (message message in messages.value)
                            {
                                // From Address
                                if (message.from != null)
                                {
                                    SaveContactandConactDomain(message.from.emailAddress.address,
                                        message.from.emailAddress.name);
                                }

                                // To Addresses
                                if (message.toRecipients != null)
                                {
                                    foreach (ToRecipient Toemail in message.toRecipients)
                                    {
                                        SaveContactandConactDomain(Toemail.emailAddress.address,
                                            Toemail.emailAddress.name);
                                    }
                                }
                                // CC Addresses
                                if (message.ccRecipients != null)
                                {
                                    foreach (CcRecipient Cc in message.ccRecipients)
                                    {
                                        SaveContactandConactDomain(Cc.emailAddress.address, Cc.emailAddress.name);
                                    }
                                }

                                // BCC Addresses
                                if (message.bccRecipients != null)
                                {
                                    foreach (bccRecipients Bcc in message.bccRecipients)
                                    {
                                        SaveContactandConactDomain(Bcc.emailAddress.address, Bcc.emailAddress.address);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MailService.SendClientEmail(ConfigurationManager.AppSettings["ToNotificatoins"], "Timesheet-Error",
                    "There is an error on timesheet email syncing <br>Please go to websupport owa account and check the issue<br>Here is the stack strace:<br>" +
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

            //ViewBag.Result = output;
            return View();
        }

        private void SaveMessages(Ticket ticket, message messages, bool IsAddTicket, string token, bool IsTicketMerge)
        {
            try
            {
                bool IsAddTicketItem = false;
                TicketItem item = null;

                // If Conversation already exists and message also exists, find if message is already available in database or not.
                if (!IsAddTicket && messages != null)
                {
                    item = db.TicketItem.Where(ti =>
                            ti.ticketid == ticket.id && ti.datetimecreated == messages.createdDateTime &&
                            ti.from == messages.from.emailAddress.address && ti.subject == messages.subject)
                        .FirstOrDefault();
                    if (item != null)
                    {
                        //Item already exists, if ticket merge flag On then we need to m.
                        if (IsTicketMerge)
                        {
                            IsAddTicketItem = true;
                        }
                    }
                    else
                    {
                        IsAddTicketItem = true;
                    }
                }
                else
                {
                    IsAddTicketItem = true;
                }

                if (IsAddTicketItem)
                {
                    item = new TicketItem
                    {
                        ticketid = ticket.id,
                        conversationid = messages.conversationId
                    };
                    foreach (bccRecipients email in messages.bccRecipients)
                    {
                        item.bccrecipients += email.emailAddress.address + ";";
                    }

                    if (!string.IsNullOrEmpty(item.bccrecipients))
                    {
                        item.bccrecipients = item.bccrecipients.TrimEnd(";".ToCharArray());
                    }

                    item.body = messages.bodyPreview;
                    item.emailmessageid = messages.id;
                    //item.mimecontent = messages.miim;
                    item.conversationid = messages.conversationId;
                    //item.conversationindex = System.Text.Encoding.Default.GetString(messages.conver);
                    item.conversationtopic = messages.subject;
                    item.datetimecreated = messages.createdDateTime;
                    item.statusupdatedon = DateTime.Now;
                    item.datetimereceived = messages.receivedDateTime;
                    item.datetimesent = messages.sentDateTime;

                    foreach (CcRecipient email in messages.ccRecipients)
                    {
                        item.displaycc += email.emailAddress.name + ";";
                    }

                    if (!string.IsNullOrEmpty(item.displaycc))
                    {
                        item.displaycc = item.displaycc.TrimEnd(";".ToCharArray());
                    }

                    foreach (replyTo email in messages.replyTo)
                    {
                        item.displayto += email.emailAddress.name + ";";
                    }

                    if (!string.IsNullOrEmpty(item.displayto))
                    {
                        item.displayto = item.displayto.TrimEnd(";".ToCharArray());
                    }

                    item.from = messages.from.emailAddress.address;
                    item.hasattachments = messages.hasAttachments;
                    switch (messages.importance)
                    {
                        case "high":
                            item.importance = 1;
                            break;

                        case "normal":
                            item.importance = 2;
                            break;

                        case "low":
                            item.importance = 3;
                            break;

                        default:
                            item.importance = 2;
                            break;
                    }
                    //item.inreplyto = messages.InReplyTo;
                    //if (messages.messa != null && messages.InternetMessageHeaders.Count > 0)
                    //{
                    //    foreach (InternetMessageHeader internetmsgheader in message.InternetMessageHeaders)
                    //    {
                    //        item.internetmessageheaders += internetmsgheader.Name + "=" + internetmsgheader.Value + ";";
                    //    }
                    //}
                    //if (!string.IsNullOrEmpty(item.internetmessageheaders)) item.internetmessageheaders = item.internetmessageheaders.TrimEnd(";".ToCharArray());

                    item.internetmessageid = messages.internetMessageId;
                    item.lastmodifiedname = messages.from.emailAddress.name;
                    item.lastmodifiedtime = messages.lastModifiedDateTime;
                    //item.mimecontent = message.MimeContent.CharacterSet;
                    foreach (replyTo email in messages.replyTo)
                    {
                        item.replyto += email.emailAddress.address + ";";
                    }

                    if (!string.IsNullOrEmpty(item.replyto))
                    {
                        item.replyto = item.replyto.TrimEnd(";".ToCharArray());
                    }

                    item.subject = messages.subject;
                    foreach (ToRecipient ToRecipients in messages.toRecipients)
                    {
                        item.torecipients += ToRecipients.emailAddress.address + ";";
                    }

                    if (!string.IsNullOrEmpty(item.torecipients))
                    {
                        item.torecipients = item.torecipients.TrimEnd(";".ToCharArray());
                    }

                    item.uniquebody = messages.body.content;
                    item.statusid = 1;
                    item.statusupdatedbyusersid = User.Identity.GetUserId();
                    item.statusupdatedon = DateTime.Now;
                    item.quotedtimeinminutes = 0;
                    item.createdonutc = DateTime.Now;
                    item.updatedonutc = DateTime.Now;
                    foreach (CcRecipient CcRecipients in messages.ccRecipients)
                    {
                        item.ccrecipients += CcRecipients.emailAddress.address + ";";
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
                        DownloadAttachments(messages, item);
                    }
                }
            }
            catch (Exception ex)
            {
                MailService.SendClientEmail(ConfigurationManager.AppSettings["ToNotificatoins"], "Timesheet-Error",
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

        private void DownloadAttachments(message message, TicketItem item)
        {
            try
            {
                // Create Dictionary of all the inline attachments.
                Dictionary<string, string> inlineAttachments = new Dictionary<string, string>();
                if (message.GraphAttachment != null && message.GraphAttachment.Count > 0)
                {
                    foreach (GraphAttachment attachment in message.GraphAttachment)
                    {
                        try
                        {
                            // FileAttachment fileAttachment = attachment as FileAttachment;
                            if (attachment != null)
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

                                TicketItemAttachment itemAttachment = db.TicketItemAttachment
                                    .Where(i => i.ticketitemid == item.id && i.attachmentid == attachment.id)
                                    .FirstOrDefault();
                                if (itemAttachment == null)
                                {
                                    itemAttachment = new TicketItemAttachment();
                                }
                                else
                                {
                                    itemAttachmentExists = true;
                                }

                                itemAttachment.ticketitemid = item.id;
                                itemAttachment.contentid = attachment.contentId;
                                //itemAttachment.contentlocation = attachment.contentLocation;
                                itemAttachment.name = attachment.name;
                                itemAttachment.attachmentid = attachment.id;

                                // Download attachment in appropriate folders and set path for the filename.
                                if (attachment.isInline)
                                {
                                    // Create Inline Directory within Attachment Directory.
                                    if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "/Attachments/" +
                                                          item.id + "/Inline"))
                                    {
                                        Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory +
                                                                  "/Attachments/" + item.id + "/Inline");
                                    }

                                    // Download Attachment into inline folder.
                                    itemAttachment.contenttype = "Inline";
                                    itemAttachment.path = "/Attachments/" + item.id + "/Inline/" + attachment.name;

                                    //  System.IO.File.WriteAllBytes(AppDomain.CurrentDomain.BaseDirectory + itemAttachment.path, Encoding.ASCII.GetBytes(attachment.contentBytes));
                                    byte[] bytes = Convert.FromBase64String(attachment.contentBytes);
                                    using (FileStream imageFile = new FileStream(
                                               AppDomain.CurrentDomain.BaseDirectory + itemAttachment.path,
                                               FileMode.Create))
                                    {
                                        imageFile.Write(bytes, 0, bytes.Length);
                                        imageFile.Flush();
                                    }

                                    inlineAttachments.Add(attachment.contentId, itemAttachment.path);
                                }
                                else
                                {
                                    // Download Attachment into inline folder.
                                    itemAttachment.contenttype = attachment.contentType;
                                    itemAttachment.path = "/Attachments/" + item.id + "/" + attachment.name;

                                    byte[] bytes = Convert.FromBase64String(attachment.contentBytes);
                                    using (FileStream imageFile = new FileStream(
                                               AppDomain.CurrentDomain.BaseDirectory + itemAttachment.path,
                                               FileMode.Create))
                                    {
                                        imageFile.Write(bytes, 0, bytes.Length);
                                        imageFile.Flush();
                                    }
                                    // System.IO.File.WriteAllBytes(AppDomain.CurrentDomain.BaseDirectory + itemAttachment.path, Encoding.ASCII.GetBytes(attachment.contentBytes));
                                }

                                if (itemAttachmentExists)
                                {
                                    db.Entry((object)itemAttachment).State = EntityState.Modified;
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
                                exception_message = ex.Message + "  " + item.id,
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
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult Error(string message, string debug)
        {
            ViewBag.Message = message;
            ViewBag.Debug = debug;
            return View("Error");
        }
    }
}