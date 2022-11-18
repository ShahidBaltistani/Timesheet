using computan.timesheet.core;
using computan.timesheet.Helpers;
using computan.timesheet.Infrastructure;
using computan.timesheet.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace computan.timesheet.Controllers
{
    [CustomeAuthorizeAttribute]
    public class EmailController : BaseController
    {
        public ActionResult Send(long? id)
        {
            List<TicketStatusViewModel> ticketstatuses = db.Database.SqlQuery<TicketStatusViewModel>("exec ticketstatus_loadticketscount")
                .ToList();
            int totaltask = 0;
            foreach (TicketStatusViewModel status in ticketstatuses)
            {
                totaltask += status.ticketcount;
            }

            TicketStatusViewModel tsvm = new TicketStatusViewModel
            {
                id = 0,
                isactive = true,
                name = "All",
                ticketcount = totaltask
            };
            ticketstatuses.Add(tsvm);
            List<TicketStatusViewModel> newticketstatusesOrder = new List<TicketStatusViewModel>
            {
                ticketstatuses.SingleOrDefault(x => x.name.Equals("All")),
                ticketstatuses.SingleOrDefault(x => x.name.Equals("New Task")),
                ticketstatuses.SingleOrDefault(x => x.name.Equals("Assigned")),
                ticketstatuses.SingleOrDefault(x => x.name.Equals("In Progress")),
                ticketstatuses.SingleOrDefault(x => x.name.Equals("On Hold")),
                ticketstatuses.SingleOrDefault(x => x.name.Equals("QC")),
                ticketstatuses.SingleOrDefault(x => x.name.Equals("In Review")),
                ticketstatuses.SingleOrDefault(x => x.name.Equals("Done")),
                ticketstatuses.SingleOrDefault(x => x.name.Equals("Trash"))
            };
            ViewBag.conversationstatus = newticketstatusesOrder;
            ViewBag.currentSubTab = GetCurrentActiveSubTab();

            /****************************************************
             * Fetch EmailReplySignature of loged in user
             ****************************************************/
            ApplicationUser LoginUser = db.Users.Find(User.Identity.GetUserId());
            ViewBag.EmailSignature = LoginUser.EmailReplySignature;
            SentItemLog SentItemLog = new SentItemLog();
            if (id != null)
            {
                SentItemLog sentlog = db.SentItemLog.Find(id);
                SentItemLog.To = sentlog.To;
                SentItemLog.Cc = sentlog.Cc;
                SentItemLog.Bcc = sentlog.Bcc;
                SentItemLog.subject = sentlog.subject;
                SentItemLog.body = sentlog.body;
                SentItemLog.id = sentlog.id;
            }

            TicketViewModel tvm = new TicketViewModel
            {
                SentItemLog = SentItemLog
            };
            return View("send", tvm);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SendEmail(string type, string TO, string CC, string BCC, string Subject, string body,
            string Attach)
        {
            try
            {
                // Fetch current user.
                string userID = User.Identity.GetUserId();
                string username = User.Identity.GetUserName();
                ApplicationUserManager userManager = System.Web.HttpContext.Current.GetOwinContext()
                    .GetUserManager<ApplicationUserManager>();
                ApplicationUser user = userManager.FindById(userID);
                // Prepare mailmessage.
                MailMessage emailMessage = new MailMessage
                {
                    From = new MailAddress(username, user.FullName)
                };

                if (!string.IsNullOrEmpty(TO))
                {
                    string[] ToEmails = TO.Split(',');
                    if (ToEmails.Length > 0)
                    {
                        foreach (string toEmail in ToEmails)
                        {
                            if (!string.IsNullOrEmpty(toEmail))
                            {
                                emailMessage.To.Add(new MailAddress(toEmail));
                            }
                        }
                    }
                }

                if (!string.IsNullOrEmpty(CC))
                {
                    string[] CCEmails = CC.Split(',');
                    if (CCEmails.Length > 0)
                    {
                        foreach (string ccEmail in CCEmails)
                        {
                            if (!string.IsNullOrEmpty(ccEmail))
                            {
                                emailMessage.CC.Add(new MailAddress(ccEmail));
                            }
                        }
                    }
                }

                if (!string.IsNullOrEmpty(BCC))
                {
                    string[] BCCEmails = BCC.Split(',');
                    if (BCCEmails.Length > 0)
                    {
                        foreach (string bccEmail in BCCEmails)
                        {
                            if (!string.IsNullOrEmpty(bccEmail))
                            {
                                emailMessage.Bcc.Add(new MailAddress(bccEmail));
                            }
                        }
                    }
                }

                if (Attach != "")
                {
                    Attach = Attach.Remove(Attach.Length - 1);
                    string[] Attachments = Attach.Split(',');

                    foreach (string attachment in Attachments)
                    {
                        string[] attachinfo = attachment.Split('_');
                        if (attachinfo.Length > 0)
                        {
                            int id = Convert.ToInt32(attachinfo[0]);
                            switch (attachinfo[1])
                            {
                                case "N":
                                    TicketReplay newAttachment = db.TicketReplay.Where(tr => tr.id == id).FirstOrDefault();
                                    if (newAttachment != null)
                                    {
                                        emailMessage.Attachments.Add(
                                            new Attachment(Server.MapPath(newAttachment.Attatchment)));
                                    }

                                    break;
                                case "E":
                                    TicketItemAttachment existingAttachment = db.TicketItemAttachment.Where(tr => tr.id == id)
                                        .FirstOrDefault();
                                    if (existingAttachment != null)
                                    {
                                        emailMessage.Attachments.Add(
                                            new Attachment(Server.MapPath(existingAttachment.path)));
                                    }

                                    break;
                            }
                        }
                    }
                }

                string removeRE = Subject.Substring(0, 3);
                if (removeRE.ToUpper() == "RE:")
                {
                    Subject = Subject.Remove(0, 3);
                }

                emailMessage.Subject = Subject;
                emailMessage.Body = body;
                emailMessage.IsBodyHtml = true;
                emailMessage.Priority = MailPriority.Normal;
                MailService.SendEmail(emailMessage);
                return Json(new { error = false, response = "success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = true, response = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        private string GetCurrentActiveSubTab()
        {
            System.Web.Routing.RouteData rd = ControllerContext.RouteData;
            string currentAction = rd.GetRequiredString("action");
            string currentController = rd.GetRequiredString("controller");
            string currentSubTab = string.Empty;

            switch (currentController.ToLower())
            {
                case "tickets":
                    switch (currentAction.ToLower())
                    {
                        case "index":
                            object id = rd.Values["id"];
                            if (id != null)
                            {
                                switch (id.ToString())
                                {
                                    case "0":
                                        currentSubTab = "All";
                                        break;
                                    case "1":
                                        currentSubTab = "New Task";
                                        break;
                                    case "2":
                                        currentSubTab = "In Progress";
                                        break;
                                    case "3":
                                        currentSubTab = "Closed";
                                        break;
                                    case "4":
                                        currentSubTab = "Wont Fix";
                                        break;
                                }
                            }
                            else
                            {
                                currentSubTab = "";
                            }

                            break;
                    }

                    break;
            }

            return currentSubTab;
        }

        // GET: Email

        #region Constructors

        private ApplicationUserManager _userManager;

        public ApplicationUserManager UserManager
        {
            get => _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            private set => _userManager = value;
        }

        #endregion
    }
}