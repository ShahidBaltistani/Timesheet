using Attachment = System.Net.Mail.Attachment;
using computan.timesheet.core.common;
using computan.timesheet.core.custom;
using computan.timesheet.core;
using computan.timesheet.Extensions;
using computan.timesheet.Helpers;
using computan.timesheet.Infrastructure;
using computan.timesheet.Models;
using EmailAddress = SendGrid.Helpers.Mail.EmailAddress;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;
using SendGrid.Helpers.Mail;
using SendGrid;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Configuration;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web;
using System;

namespace computan.timesheet.Controllers
{
    [CustomeAuthorizeAttribute]
    public class TicketsController : BaseController
    {
        private const int recordsPerPage = 100;

        #region Inbox Actions

        public ActionResult Inbox(string mailbox = "")
        {
            return View();
        }

        #endregion Inbox Actions

        #region Ticket Assignment

        public ActionResult UpdateTicketTeam(long TicketId, int TeamId)
        {
            if (TicketId < 1 || TeamId < 1)
            {
                return Json(new { error = 1, flag = 1, errortext = "Sorry, ticket or team information is missing!" },
                    JsonRequestBehavior.AllowGet);
            }

            TicketTeamLogs TicketTeamIDs = db.TicketTeamLogs.Where(TTL => TTL.ticketid == TicketId && TTL.teamid == TeamId)
                .SingleOrDefault();
            if (TicketTeamIDs != null)
            {
                return Json(new { error = 1, flag = 2, errortext = "ticket already assigned to this team" },
                    JsonRequestBehavior.AllowGet);
            }

            TicketTeamLogs ticketTeamlogs = new TicketTeamLogs();
            //string Ticid = TicketId.ToString();
            //string TId = TeamId.ToString();
            ticketTeamlogs.ticketid = TicketId;
            ticketTeamlogs.teamid = TeamId;
            ticketTeamlogs.assignedbyusersid = User.Identity.GetUserId();
            ticketTeamlogs.assignedon = DateTime.Now;
            ticketTeamlogs.statusid = 6;
            ticketTeamlogs.statusupdatedbyusersid = User.Identity.GetUserId();
            ticketTeamlogs.statusupdatedon = DateTime.Now;
            ticketTeamlogs.displayorder = 1;
            db.TicketTeamLogs.Add(ticketTeamlogs);
            db.SaveChanges();
            ApplicationUser userAdmin = UserManager.FindById(Convert.ToString(User.Identity.GetUserId()));
            Team team = db.Team.Find(TeamId);
            /*Muhammad Nasir 29-11-2018*/
            TicketLogs TicketLogs = new TicketLogs
            {
                ticketid = Convert.ToInt64(TicketId),
                actiontypeid = 3,
                actiondate = DateTime.Now,
                actionbyuserId = User.Identity.GetUserId(),
                ActionDescription = "<a href='/tickets/ticketitem/" + Convert.ToString(TicketId) + "'>Ticket #" +
                                    Convert.ToString(TicketId) + "</a> is assigned to team <b>[" + team.name +
                                    "]</b> by " + userAdmin.FirstName + " " + userAdmin.LastName + " on " +
                                    Convert.ToString(DateTime.Now)
            };
            db.TicketLogs.Add(TicketLogs);
            db.SaveChanges();
            return Json(new { error = 0 }, JsonRequestBehavior.AllowGet);
        }

        #endregion Ticket Assignment

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AddFavouriteTickets(long ticketid)
        {
            try
            {
                string user = User.Identity.GetUserId();
                TicketUserFlagged uft = db.TicketUserFlagged.Where(u => u.userid == user && u.ticketid == ticketid).FirstOrDefault();
                if (uft != null)
                {
                    uft.isactive = true;
                    db.Entry(uft).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    TicketUserFlagged newuft = new TicketUserFlagged
                    {
                        userid = user,
                        ticketid = ticketid,
                        isactive = true
                    };
                    db.TicketUserFlagged.Add(newuft);
                    db.SaveChanges();
                }

                return Json(new { success = 1 }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { success = 0 }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult RemoveFavouriteTickets(long ticketid)
        {
            try
            {
                string user = User.Identity.GetUserId();
                TicketUserFlagged uft = db.TicketUserFlagged.Where(u => u.userid == user && u.ticketid == ticketid).FirstOrDefault();
                uft.isactive = false;
                db.Entry(uft).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = 1 }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { success = 0 }, JsonRequestBehavior.AllowGet);
            }
        }

        //public ActionResult ConversationRow(int? pageNum,string topic)
        //{
        //    if (!IsLoggedIn())
        //    {
        //        return RedirectToAction("login", "account");
        //    }
        //    var page = pageNum ?? 0;
        //    if (Request.IsAjaxRequest())
        //    {
        //        var items = GetPaginatedTickets(page,topic);
        //        int itemcount = items.tickets.Count();
        //        String tickets = PartialView("_ConversationRow", items).RenderToString();
        //        return Json(new { tickets = tickets, itemcount = itemcount }, JsonRequestBehavior.AllowGet);
        //        //return PartialView("_ConversationRow", GetPaginatedTickets(page));
        //    }
        //    ViewBag.users = UserManager.Users.OrderBy(f => f.FirstName).Where(u => u.isactive == true).ToList();
        //    var ticket = (from tt in db.Ticket
        //                  join ticketi in db.TicketItem on tt.id equals ticketi.ticketid
        //                  select tt
        //           ).Include(t => t.FlagStatus).Include(tt => tt.TicketType).Include(t => t.StatusUpdatedByUser).Include(t => t.ConversationStatus).Where(t => t.statusid == 1).GroupBy(t => t.id).Select(t => t.FirstOrDefault()).OrderByDescending(t => t.lastdeliverytime).Take(recordsPerPage);
        //    List<TicketItem> ti = new List<TicketItem>();
        //    TicketViewModel tvm = new TicketViewModel();
        //    tvm.tickets = ticket.ToList();
        //    tvm.ticketitems = ti;
        //    foreach (var items in ticket)
        //    {
        //        var ticketitem = db.TicketItem.Where(t => t.ticketid == items.id).OrderByDescending(t => t.createdonutc).FirstOrDefault();
        //        ti.Add(ticketitem);
        //    }
        //    return View("Index", tvm);
        //}

        // GET: Tickets/MyTickets
        //public ActionResult MyTickets(long id)
        //{
        //    // Make sure user is logged in.
        //    if (!IsLoggedIn())
        //    {
        //        return RedirectToAction("login", "account");
        //    }

        //    string currentuserid = User.Identity.GetUserId();
        //    var ticket = (from t in db.Ticket
        //                  join ti in db.TicketItem on t.id equals ti.ticketid
        //                  join til in db.TicketItemLog on ti.id equals til.ticketitemid
        //                  join tstatus in db.ConversationStatus on t.statusid equals tstatus.id into status
        //                  where t.statusid == id && til.assignedtousersid == currentuserid
        //                  select t).Include(tt => tt.TicketType).Distinct();

        //    var mytickettake = ticket.OrderByDescending(t => t.lastdeliverytime).Take(recordsPerPage);

        //    ViewBag.status = (db.ConversationStatus);
        //    var projectlist = db.Database.SqlQuery<CombinedEntity>("exec projects_loadcombined").Select(x => new SelectListItem
        //    {
        //        Text = x.name,
        //        Value = x.id.ToString(),
        //    }).ToList();

        //    ViewBag.projects = new SelectList(projectlist, "Value", "Text");
        //    ViewBag.skills = (db.Skill.ToList());
        //    ViewBag.ticketid = id;
        //    ViewBag.statusid = id;
        //    ViewBag.tickettype = 1;
        //    return View("Index", mytickettake.ToList());
        //}
        public ActionResult MyTicketsAjax(int id, int? pagenum)
        {
            int page = pagenum ?? 0;

            if (Request.IsAjaxRequest())
            {
                List<Ticket> items = MyTicketGetPaginatedTickets(id, page);
                int itemcount = items.Count();
                string tickets = PartialView("_ConversationRow", items).RenderToString();
                return Json(new { tickets, itemcount }, JsonRequestBehavior.AllowGet);
                //return PartialView("_ConversationRow", MyTicketGetPaginatedTickets(id, page));
            }

            ViewBag.status = db.ConversationStatus;
            List<SelectListItem> projectlist = db.Database.SqlQuery<CombinedEntity>("exec projects_loadcombined").Select(x =>
                new SelectListItem
                {
                    Text = x.name,
                    Value = x.id.ToString()
                }).ToList();
            ViewBag.projects = new SelectList(projectlist, "Value", "Text");
            ViewBag.skills = db.Skill.ToList();
            ViewBag.ticketid = id;
            return View("Index",
                db.Ticket.Include(t => t.FlagStatus).Include(tt => tt.TicketType).Include(t => t.StatusUpdatedByUser)
                    .Include(t => t.ConversationStatus).Where(t => t.statusid == id)
                    .OrderByDescending(t => t.lastdeliverytime).Take(recordsPerPage));
        }

        private List<Ticket> MyTicketGetPaginatedTickets(int id, int page = 1)
        {
            int skipRecords = page * recordsPerPage;
            string currentuserid = User.Identity.GetUserId();
            IQueryable<Ticket> listOftickets = (from t in db.Ticket
                                                join ti in db.TicketItem on t.id equals ti.ticketid
                                                join til in db.TicketItemLog on ti.id equals til.ticketitemid
                                                join tstatus in db.ConversationStatus on t.statusid equals tstatus.id into status
                                                where t.statusid == id && til.assignedtousersid == currentuserid
                                                select t).Include(tt => tt.TicketType).Distinct();

            List<Ticket> myticketpaginated = listOftickets.OrderByDescending(t => t.lastdeliverytime).Skip(skipRecords)
                .Take(recordsPerPage).ToList();

            ViewBag.status = db.ConversationStatus;
            List<SelectListItem> projectlist = db.Database.SqlQuery<CombinedEntity>("exec projects_loadcombined").Select(x =>
                new SelectListItem
                {
                    Text = x.name,
                    Value = x.id.ToString()
                }).ToList();
            ViewBag.projects = new SelectList(projectlist, "Value", "Text");
            ViewBag.skills = db.Skill.ToList();

            return myticketpaginated.ToList();
        }

        // GET: Tickets/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Ticket ticket = db.Ticket.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }

            return View(ticket);
        }

        // GET: Tickets/Create
        public ActionResult Create()
        {
            ViewBag.flagstatusid = new SelectList(db.FlagStatus, "id", "name");
            ViewBag.statusupdatedbyusersid =
                new SelectList(UserManager.Users.OrderBy(f => f.FirstName).Where(u => u.isactive == true).ToList(),
                    "Id", "Email");
            ViewBag.statusid = new SelectList(db.TicketStatus, "id", "name");
            return View();
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(
            [Bind(Include =
                "id,conversationid,uniquesenders,topic,lastdeliverytime,size,messagecount,hasattachments,importance,flagstatusid,lastmodifiedtime,statusid,statusupdatedbyusersid,statusupdatedon,createdonutc,updatedonutc,ipused,userid")]
            Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                db.Ticket.Add(ticket);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.flagstatusid = new SelectList(db.FlagStatus, "id", "name", ticket.flagstatusid);
            ViewBag.statusupdatedbyusersid =
                new SelectList(UserManager.Users.OrderBy(f => f.FirstName).Where(u => u.isactive == true).ToList(),
                    "Id", "Email", ticket.statusupdatedbyusersid);
            ViewBag.statusid = new SelectList(db.TicketStatus, "id", "name", ticket.statusid);
            return View(ticket);
        }

        // GET: Tickets/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Ticket ticket = db.Ticket.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }

            ViewBag.flagstatusid = new SelectList(db.FlagStatus, "id", "name", ticket.flagstatusid);
            ViewBag.statusupdatedbyusersid =
                new SelectList(UserManager.Users.OrderBy(f => f.FirstName).Where(u => u.isactive == true).ToList(),
                    "Id", "Email", ticket.statusupdatedbyusersid);
            ViewBag.statusid = new SelectList(db.TicketStatus, "id", "name", ticket.statusid);
            return View(ticket);
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(
            [Bind(Include =
                "id,conversationid,uniquesenders,topic,lastdeliverytime,size,messagecount,hasattachments,importance,flagstatusid,lastmodifiedtime,statusid,statusupdatedbyusersid,statusupdatedon,createdonutc,updatedonutc,ipused,userid")]
            Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ticket).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.flagstatusid = new SelectList(db.FlagStatus, "id", "name", ticket.flagstatusid);
            ViewBag.statusupdatedbyusersid =
                new SelectList(UserManager.Users.OrderBy(f => f.FirstName).Where(u => u.isactive == true).ToList(),
                    "Id", "Email", ticket.statusupdatedbyusersid);
            ViewBag.statusid = new SelectList(db.TicketStatus, "id", "name", ticket.statusid);
            return View(ticket);
        }

        // GET: Tickets/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Ticket ticket = db.Ticket.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }

            return View(ticket);
        }

        // POST: Tickets/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            Ticket ticket = db.Ticket.Find(id);
            db.Ticket.Remove(ticket);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }

            base.Dispose(disposing);
        }

        #region Change Ticket Status Methods

        //TODO: Delete after verifying that it is not used anywhere in the system.
        public ActionResult TicketStatusUpdate(string id, string status)
        {
            try
            {
                long tid = Convert.ToInt64(id);
                int statusid = Convert.ToInt32(status);
                Ticket ticket = db.Ticket.Find(tid);
                if (ticket.statusid == 2 && statusid == 2)
                {
                    return Json(new { error = true, errortext = "Sorry this task already in progress." });
                }

                ticket.statusid = statusid;
                ticket.statusupdatedbyusersid = User.Identity.GetUserId();
                ticket.updatedonutc = DateTime.Now;
                ticket.statusupdatedon = DateTime.Now;
                ticket.LastActivityDate = DateTime.Now;
                ticket.ipused = Request.UserHostAddress;
                ticket.userid = User.Identity.GetUserId();
                db.Entry(ticket).State = EntityState.Modified;
                db.SaveChanges();

                List<TicketItem> TicketItem = db.TicketItem.Where(t => t.ticketid == ticket.id).ToList();
                if (TicketItem.Count > 0 && TicketItem != null)
                {
                    foreach (TicketItem items in TicketItem)
                    {
                        if (items.statusid == 1 || items.statusid == 2)
                        {
                            TicketItem ticketitem = db.TicketItem.Where(i => i.id == items.id).FirstOrDefault();
                            if (statusid == 4)
                            {
                                ticketitem.statusid = 4;
                            }
                            else if (statusid == 3)
                            {
                                ticketitem.statusid = 3;
                            }
                            else
                            {
                                ticketitem.statusid = statusid;
                            }

                            ticketitem.statusupdatedbyusersid = User.Identity.GetUserId();
                            ticketitem.updatedonutc = DateTime.Now;
                            ticketitem.statusupdatedon = DateTime.Now;
                            ticketitem.ipused = Request.UserHostAddress;
                            ticketitem.userid = User.Identity.GetUserId();
                            db.Entry(ticketitem).State = EntityState.Modified;
                            db.SaveChanges();
                        }

                        List<TicketItemLog> ticketitemlog = db.TicketItemLog.Where(t => t.ticketitemid == items.id).ToList();
                        if (ticketitemlog.Count > 0 && ticketitemlog != null)
                        {
                            foreach (TicketItemLog logs in ticketitemlog)
                            {
                                if (logs.statusid == 1 || logs.statusid == 2)
                                {
                                    if (statusid == 4)
                                    {
                                        logs.statusid = 4;
                                    }
                                    else if (statusid == 3)
                                    {
                                        logs.statusid = 3;
                                    }
                                    else
                                    {
                                        logs.statusid = statusid;
                                    }

                                    logs.ticketitemid = items.id;
                                    logs.statusupdatedbyusersid = User.Identity.GetUserId();
                                    logs.statusupdatedon = DateTime.Now;
                                    db.Entry(logs).State = EntityState.Modified;
                                    db.SaveChanges();
                                }
                            }
                        }
                    }
                }

                AddNotification(tid, 1);
                return Json(new { success = true, successtext = "The ticket status updated." });
            }
            catch (Exception ex)
            {
                return Json(new { error = true, errortext = ex.Message });
            }
        }

        #endregion Change Ticket Status Methods

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SendEmail(string type, string TO, string CC, string BCC, string Subject, string body,
            string ticketID, string Attach, long tcktitemid, string pid, string sid, int spenttime, int? billtime,
            DateTime workdate, string title, string description, string comments, bool ischecked)
        {
            try
            {
                // Make sure, ticket is found.
                long ticketid = Convert.ToInt64(ticketID);
                //var ticket = db.Ticket.SingleOrDefault(x => x.id == ticketid.);
                TicketItem ticket = db.TicketItem.Where(t => t.id == ticketid).FirstOrDefault();

                if (ticket == null)
                {
                    return Json(
                        new
                        {
                            error = true,
                            response = "Sorry, ticket is not found, please refresh your page and try again!"
                        }, JsonRequestBehavior.AllowGet);
                }

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

                SentItemLog sentitem = new SentItemLog
                {
                    To = TO,
                    Cc = CC,
                    Bcc = BCC,
                    subject = Subject,
                    body = body,
                    ticketId = ticketid,
                    ticket_title = ticket.conversationtopic,
                    updatedonutc = DateTime.Now,
                    createdonutc = DateTime.Now,
                    Sentdate = DateTime.Now,
                    ipused = Request.UserHostAddress,
                    userid = User.Identity.GetUserId()
                };
                db.SentItemLog.Add(sentitem);
                db.SaveChanges();

                TicketItem ticketitemEntry = db.TicketItem.Find(ticketid);
                long ticketIdEntry = ticketitemEntry.ticketid;
                TicketLogs TicketLogs = new TicketLogs
                {
                    ticketid = ticketIdEntry,
                    actiontypeid = 7,
                    actiondate = DateTime.Now,
                    actionbyuserId = User.Identity.GetUserId()
                };
                //The ticket # 160101 is assigned to [Shaban Sarfraz] by [Bratislav].

                ApplicationUser userAdmin = UserManager.FindById(Convert.ToString(User.Identity.GetUserId()));
                TicketLogs.ActionDescription = "The <a href='/tickets/ticketitem/" + ticketid + "'>ticket #" +
                                               ticketid + "</a> response has been sent to <b>[" + TO + "]</b> by <b>[" +
                                               user.FullName + "]</b> on " + Convert.ToString(DateTime.Now);
                db.TicketLogs.Add(TicketLogs);
                db.SaveChanges();

                emailMessage.Subject = Subject;
                emailMessage.Body = body;
                emailMessage.IsBodyHtml = true;
                emailMessage.Priority = MailPriority.Normal;
                bool result = MailService.SendEmailStatus(emailMessage);

                sentitem.IsSent = result;
                db.Entry(sentitem).State = EntityState.Modified;
                db.SaveChanges();

                if (ischecked)
                {
                    if (!string.IsNullOrEmpty(pid) && !string.IsNullOrEmpty(sid))
                    {
                        //Add time for that ticket item
                        AddTimeWithEmail(tcktitemid, pid, sid, spenttime, billtime, workdate, title, description,
                            comments);
                    }
                }

                //List<Int64> AttachmentsID = new List<long>();
                //if (!string.IsNullOrEmpty(Attach))
                //{
                //    foreach (var file in Attach.TrimEnd(',').Split(',').ToList())
                //    {
                //        AttachmentsID.Add(Convert.ToInt64(file));
                //    }
                //}

                //var attatchment = db.TicketReplay.Where(x => x.TicketID == ticketid && x.UserID == userID).ToList();

                //response = Send(HttpContext.Request.ApplicationPath);

                //if (response == "success")
                //{
                //    if (attatchment.Count > 0)
                //    {
                //        db.TicketReplay.RemoveRange(attatchment);
                //        db.SaveChanges();
                //        try
                //        {
                //            foreach (var item in attatchment)
                //            {
                //                if (System.IO.File.Exists(item.Attatchment))
                //                {
                //                    System.IO.File.Delete(item.Attatchment);
                //                }
                //            }
                //        }
                //        catch { }
                //    }
                //    return Json(new { error = false, response = response }, JsonRequestBehavior.AllowGet);
                //}

                return Json(new { error = false, response = "success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = true, response = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SendEmail_backup(string type, string TO, string CC, string BCC, string Subject, string body,
            string ticketID, string Attach)
        {
            try
            {
                // Make sure, ticket is found.
                long ticketid = Convert.ToInt64(ticketID);
                //var ticket = db.Ticket.SingleOrDefault(x => x.id == ticketid.);
                TicketItem ticket = db.TicketItem.Where(t => t.id == ticketid).FirstOrDefault();

                if (ticket == null)
                {
                    return Json(
                        new
                        {
                            error = true,
                            response = "Sorry, ticket is not found, please refresh your page and try again!"
                        }, JsonRequestBehavior.AllowGet);
                }

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

                //List<Int64> AttachmentsID = new List<long>();
                //if (!string.IsNullOrEmpty(Attach))
                //{
                //    foreach (var file in Attach.TrimEnd(',').Split(',').ToList())
                //    {
                //        AttachmentsID.Add(Convert.ToInt64(file));
                //    }
                //}

                //var attatchment = db.TicketReplay.Where(x => x.TicketID == ticketid && x.UserID == userID).ToList();

                //response = Send(HttpContext.Request.ApplicationPath);

                //if (response == "success")
                //{
                //    if (attatchment.Count > 0)
                //    {
                //        db.TicketReplay.RemoveRange(attatchment);
                //        db.SaveChanges();
                //        try
                //        {
                //            foreach (var item in attatchment)
                //            {
                //                if (System.IO.File.Exists(item.Attatchment))
                //                {
                //                    System.IO.File.Delete(item.Attatchment);
                //                }
                //            }
                //        }
                //        catch { }
                //    }
                //    return Json(new { error = false, response = response }, JsonRequestBehavior.AllowGet);
                //}

                return Json(new { error = false, response = "success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = true, response = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        //Send grid method
        //[HttpPost]
        //[ValidateInput(false)]
        //public async Task<ActionResult> SendEmailtest(string type, string TO, string CC, string BCC, string Subject, string body, string ticketID, string Attach)
        //{
        //    try
        //    {
        //        // Make sure, ticket is found.
        //        long ticketid = Convert.ToInt64(ticketID);
        //        //var ticket = db.Ticket.SingleOrDefault(x => x.id == ticketid.);
        //        var ticket = db.TicketItem.Where(t => t.id == ticketid).FirstOrDefault();

        //        if (ticket == null)
        //        {
        //            return Json(new { error = true, response = "Sorry, ticket is not found, please refresh your page and try again!" }, JsonRequestBehavior.AllowGet);
        //        }

        //        // Fetch current user.
        //        string userID = User.Identity.GetUserId();
        //        string username = User.Identity.GetUserName();
        //        var userManager = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
        //        var user = userManager.FindById(userID);
        //        var client = new SendGridClient("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");
        //        var msg = new SendGridMessage();

        //        // Prepare mailmessage.
        //        List<SendGrid.Helpers.Mail.EmailAddress> to_email = new List<SendGrid.Helpers.Mail.EmailAddress>();
        //        List<SendGrid.Helpers.Mail.EmailAddress> cc_email = new List<SendGrid.Helpers.Mail.EmailAddress>();
        //        List<SendGrid.Helpers.Mail.EmailAddress> bcc_email = new List<SendGrid.Helpers.Mail.EmailAddress>();
        //        List<SendGrid.Helpers.Mail.Attachment> attach_email = new List<SendGrid.Helpers.Mail.Attachment>();

        //        msg.From = new SendGrid.Helpers.Mail.EmailAddress("zohaib@computan.net", user.FullName);

        //        if (!string.IsNullOrEmpty(TO))
        //        {
        //            string[] ToEmails = TO.Split(',');
        //            if (ToEmails.Length > 0)
        //            {
        //                foreach (string toEmail in ToEmails)
        //                {
        //                    if (!string.IsNullOrEmpty(toEmail))
        //                    {
        //                        to_email.Add(new SendGrid.Helpers.Mail.EmailAddress(toEmail));
        //                    }
        //                }
        //            }
        //        }
        //        msg.AddTos(to_email);

        //        if (!string.IsNullOrEmpty(CC))
        //        {
        //            string[] CCEmails = CC.Split(',');
        //            if (CCEmails.Length > 0)
        //            {
        //                foreach (string ccEmail in CCEmails)
        //                {
        //                    if (!string.IsNullOrEmpty(ccEmail))
        //                    {
        //                        cc_email.Add(new SendGrid.Helpers.Mail.EmailAddress(ccEmail));
        //                    }
        //                }
        //            }
        //        }
        //        if (cc_email.Count > 0)
        //        {
        //            msg.AddCcs(cc_email);
        //        }
        //        if (!string.IsNullOrEmpty(BCC))
        //        {
        //            string[] BCCEmails = BCC.Split(',');
        //            if (BCCEmails.Length > 0)
        //            {
        //                foreach (string bccEmail in BCCEmails)
        //                {
        //                    if (!string.IsNullOrEmpty(bccEmail))
        //                    {
        //                        bcc_email.Add(new SendGrid.Helpers.Mail.EmailAddress(bccEmail));
        //                    }
        //                }
        //            }
        //        }
        //        if (bcc_email.Count > 0)
        //        {
        //            msg.AddBccs(bcc_email);
        //        }

        //        //if (Attach != "")
        //        //{
        //        //    Attach = Attach.Remove(Attach.Length - 1);
        //        //    string[] Attachments = Attach.Split(',');

        //        //    foreach (string attachment in Attachments)
        //        //    {
        //        //        string[] attachinfo = attachment.Split('_');
        //        //        if (attachinfo.Length > 0)
        //        //        {
        //        //            int id = (Convert.ToInt32(attachinfo[0]));
        //        //            switch (attachinfo[1].ToString())
        //        //            {
        //        //                case "N":
        //        //                    var newAttachment = db.TicketReplay.Where(tr => tr.id == id).FirstOrDefault();
        //        //                    if (newAttachment != null)
        //        //                    {
        //        //                        attach_email.Add(new SendGrid.Helpers.Mail.Attachment()
        //        //                        {
        //        //                            Filename= newAttachment.Attatchment
        //        //                        });
        //        //                    }
        //        //                    break;
        //        //                case "E":
        //        //                    var existingAttachment = db.TicketItemAttachment.Where(tr => tr.id == id).FirstOrDefault();
        //        //                    if (existingAttachment != null)
        //        //                    {
        //        //                        attach_email.Add(new SendGrid.Helpers.Mail.Attachment()
        //        //                        {
        //        //                            Filename = Server.MapPath(existingAttachment.path)
        //        //                        });
        //        //                    }
        //        //                    break;
        //        //            }
        //        //        }
        //        //    }
        //        //}
        //        var removeRE = Subject.Substring(0, 3);
        //        if (removeRE.ToUpper() == "RE:")
        //        {
        //            Subject = Subject.Remove(0, 3);
        //        }
        //        msg.Subject = Subject;
        //        msg.HtmlContent = body;

        //        var response = await client.SendEmailAsync(msg);

        //        //List<Int64> AttachmentsID = new List<long>();
        //        //if (!string.IsNullOrEmpty(Attach))
        //        //{
        //        //    foreach (var file in Attach.TrimEnd(',').Split(',').ToList())
        //        //    {
        //        //        AttachmentsID.Add(Convert.ToInt64(file));
        //        //    }
        //        //}

        //        //var attatchment = db.TicketReplay.Where(x => x.TicketID == ticketid && x.UserID == userID).ToList();

        //        //response = Send(HttpContext.Request.ApplicationPath);

        //        //if (response == "success")
        //        //{
        //        //    if (attatchment.Count > 0)
        //        //    {
        //        //        db.TicketReplay.RemoveRange(attatchment);
        //        //        db.SaveChanges();
        //        //        try
        //        //        {
        //        //            foreach (var item in attatchment)
        //        //            {
        //        //                if (System.IO.File.Exists(item.Attatchment))
        //        //                {
        //        //                    System.IO.File.Delete(item.Attatchment);
        //        //                }
        //        //            }
        //        //        }
        //        //        catch { }
        //        //    }
        //        //    return Json(new { error = false, response = response }, JsonRequestBehavior.AllowGet);
        //        //}

        //        return Json(new { error = false, response = "success" }, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex) { return Json(new { error = true, response = ex.Message.ToString() }, JsonRequestBehavior.AllowGet); }
        //}

        //[HttpPost]
        //public ActionResult EmailAttachment(long TicketID, string Type, string To, string CC, string BCC, string Body)
        //{
        //    List<string> filename = new List<string>();
        //    try
        //    {
        //        var files = Request.Files;
        //        if (files.Count > 0)
        //        {
        //            for (int i = 0; i < files.Count; i++)
        //            {
        //                if (!Directory.Exists(Server.MapPath("~/Contexts/TempForUploading")))
        //                {
        //                    Directory.CreateDirectory(Server.MapPath("~/Contexts/TempForUploading"));
        //                }
        //                HttpPostedFileBase file = files[i] as HttpPostedFileBase;
        //                if (file != null)
        //                {
        //                    string pathfilename = Server.MapPath("~/Contexts/TempForUploading/") + System.Guid.NewGuid().ToString().Replace("-", "") + file.FileName;
        //                    file.SaveAs(pathfilename);
        //                    TicketReplay obj = new TicketReplay();
        //                    obj.UserID = User.Identity.GetUserId();
        //                    obj.createdon = DateTime.Now;
        //                    obj.Attatchment = pathfilename;
        //                    obj.TicketID = TicketID;
        //                    obj.Type = Type;
        //                    obj.To = To;
        //                    obj.CC = CC;
        //                    obj.BCC = BCC;
        //                    obj.Body = Body;
        //                    db.TicketReplay.Add(obj);
        //                    db.SaveChanges();
        //                    filename.Add(file.FileName + ";" + obj.id);
        //                }
        //            }
        //            return Json(new { error = false, response = "success", filename = filename }, JsonRequestBehavior.AllowGet);
        //        }
        //    }
        //    catch (Exception ex) { return Json(new { error = true, response = ex.Message, filename = filename }, JsonRequestBehavior.AllowGet); }
        //    return Json(new { error = true, response = "Unknown error ", filename = filename }, JsonRequestBehavior.AllowGet);
        //}
        //public JsonResult DeleteAttachment(string TicketReplayID)
        //{
        //    try
        //    {
        //        var id = Convert.ToInt64(TicketReplayID);
        //        var objToDelete = db.TicketReplay.FirstOrDefault(x => x.id == id);
        //        if (objToDelete != null)
        //        {
        //            db.TicketReplay.Remove(objToDelete);
        //            System.IO.File.Delete(objToDelete.Attatchment);
        //            db.SaveChanges();
        //            return Json(new { error = false, response = "success" }, JsonRequestBehavior.AllowGet);
        //        }
        //    }
        //    catch (Exception ex) { return Json(new { error = true, response = ex.Message }, JsonRequestBehavior.AllowGet); }
        //    return Json(new { error = true, response = "File not found" }, JsonRequestBehavior.AllowGet);
        //}

        //public string SendSmtpEmail(string type, string to, string cc, string bcc, string subject, string body, List<string> fileAttachment = null)
        //{
        //    try
        //    {
        //        string host = Convert.ToString(ConfigurationManager.AppSettings["host"]);
        //        string Email = Convert.ToString(ConfigurationManager.AppSettings["EmailAddress"]);
        //        string Domain = Convert.ToString(ConfigurationManager.AppSettings["Domain"]);
        //        string Password = Convert.ToString(ConfigurationManager.AppSettings["EmailAddress"]);
        //        string EmailAddress = Email + "@" + Domain + ".com";

        //        MailMessage mail = new MailMessage(EmailAddress, "zubair@computan.com");
        //        SmtpClient client = new SmtpClient();
        //        client.Port = 25;
        //        client.DeliveryMethod = SmtpDeliveryMethod.Network;
        //        client.UseDefaultCredentials = true;
        //        client.Credentials = new System.Net.NetworkCredential(EmailAddress, Password);
        //        client.Host = host;
        //        mail.Subject = subject;
        //        mail.Body = body;
        //        client.Send(mail);

        //        return "success";
        //    }
        //    catch (Exception ex) { return ex.Message; }
        //}
        public MailSettingsSectionGroup GetMailSettings(string ApplicationPath)
        {
            System.Configuration.Configuration config = WebConfigurationManager.OpenWebConfiguration(ApplicationPath);
            MailSettingsSectionGroup settings = (MailSettingsSectionGroup)config.GetSectionGroup("system.net/mailSettings");
            return settings;
        }

        public async Task<ActionResult> Send(MailMessage emailMessage)
        {
            try
            {
                SendGridClient client =
                    new SendGridClient("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");
                EmailAddress from = new EmailAddress("Zohaib@computan.net", "Zohaib Khalid");
                string subject = "Sending with SendGrid is Fun";
                EmailAddress to = new EmailAddress("rashed@computan.net", "Rashed Alee");
                string plainTextContent = "and easy to do anywhere, even with C#";
                string htmlContent = "<strong>and easy to do anywhere, even with C#</strong>";
                SendGridMessage msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
                Response response = await client.SendEmailAsync(msg);

                return View();
                //  return "success";
            }
            catch (Exception)
            {
                return View();
            }
        }

        public string Message(Exception ex)
        {
            string temp = "";
            if (ex.InnerException != null)
            {
                temp += ";" + Message(ex.InnerException);
            }
            else
            {
                temp += ex.Message;
            }

            return temp;
        }

        //#region Send Email Functionality

        //[HttpPost]
        //public ActionResult UploadAttachment(long TicketID, string Type)
        //{
        //    List<string> fileList = new List<string>();
        //    try
        //    {
        //        var files = Request.Files;
        //        if (files.Count > 0)
        //        {
        //            for (int i = 0; i < files.Count; i++)
        //            {
        //                // Create Outgoing directory, if not exists.
        //                if (!Directory.Exists(Server.MapPath("~/Attachments/Outgoing")))
        //                {
        //                    Directory.CreateDirectory(Server.MapPath("~/Attachments/Outgoing"));
        //                }

        //                // Create ticket directory if not exists.
        //                if (!Directory.Exists(Server.MapPath("~/Attachments/Outgoing/" + TicketID)))
        //                {
        //                    Directory.CreateDirectory(Server.MapPath("~/Attachments/Outgoing/" + TicketID));
        //                }

        //                HttpPostedFileBase file = files[i] as HttpPostedFileBase;
        //                if (file != null)
        //                {
        //                    string filename = "/Attachments/Outgoing/" + TicketID + "/" + file.FileName;
        //                    string physicalpath = Server.MapPath("~/Attachments/Outgoing/" + TicketID + "/") + file.FileName;

        //                    if (System.IO.File.Exists(physicalpath))
        //                    {
        //                        string uid = System.Guid.NewGuid().ToString().Replace("-", "");
        //                        // create new guid folder
        //                        if (!Directory.Exists(Server.MapPath("~/Attachments/Outgoing/" + TicketID + "/" + uid)))
        //                        {
        //                            Directory.CreateDirectory(Server.MapPath("~/Attachments/Outgoing/" + TicketID + "/" + uid));
        //                        }

        //                        filename = "/Attachments/Outgoing/" + TicketID + "/" + uid + "/" + file.FileName;
        //                        physicalpath = Server.MapPath("~/Attachments/Outgoing/" + TicketID + "/" + uid + "/") + file.FileName;
        //                    }

        //                    file.SaveAs(physicalpath);
        //                    TicketReplay obj = new TicketReplay();
        //                    obj.UserID = User.Identity.GetUserId();
        //                    obj.createdon = DateTime.Now;
        //                    obj.Attatchment = filename;
        //                    obj.TicketID = TicketID;
        //                    obj.Type = Type;
        //                    obj.To = string.Empty;
        //                    obj.CC = string.Empty;
        //                    obj.BCC = string.Empty;
        //                    obj.Body = string.Empty;
        //                    db.TicketReplay.Add(obj);
        //                    db.SaveChanges();
        //                    fileList.Add(file.FileName + ";" + obj.id + ";" + filename);
        //                }
        //            }
        //            return Json(new { error = false, response = "success", filename = fileList }, JsonRequestBehavior.AllowGet);
        //        }
        //    }
        //    catch (Exception ex) { return Json(new { error = true, response = ex.Message, filename = fileList }, JsonRequestBehavior.AllowGet); }
        //    return Json(new { error = true, response = "Unknown error ", filename = fileList }, JsonRequestBehavior.AllowGet);
        //}

        //#endregion
        [HttpGet]
        public ActionResult Searchbysubject(string subject, string criteria)
        {
            IQueryable<Ticket> tickets;
            switch (criteria)
            {
                case "id":
                    long id = Convert.ToInt64(subject);
                    tickets = db.Ticket.Where(t => t.id == id).ToList().AsQueryable();
                    break;

                case "subject":
                    tickets = db.Ticket.Where(t => t.topic == subject).ToList().AsQueryable();
                    break;

                default:
                    tickets = db.Ticket.Where(t => t.topic == subject).ToList().AsQueryable();
                    break;
            }

            try
            {
                TicketViewModel tvModel = new TicketViewModel
                {
                    tickets = tickets
                };
                string ticketitems = PartialView("_MergeAbleTicketsRow", tvModel).RenderToString();
                return Json(new { Tickets = ticketitems }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = true, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult MergeMultipleTickets(long firstticketid, long[] selectedtickets)
        {
            try
            {
                int failed = 0;
                string ticketTitle = null;
                string Message = null;
                string ticketids = null;
                foreach (long item in selectedtickets)
                {
                    Ticket removeableticket = db.Ticket.Find(item);
                    if (removeableticket != null && item != firstticketid)
                    {
                        try
                        {
                            //Remove Faaviourat flag Before Merge
                            List<TicketUserFlagged> uft = db.TicketUserFlagged.Where(u => u.ticketid == item).ToList();
                            foreach (TicketUserFlagged flag in uft)
                            {
                                db.TicketUserFlagged.Remove(flag);
                                db.SaveChanges();
                            }
                        }
                        catch (Exception ex)
                        {
                            failed++;
                            if (!string.IsNullOrEmpty(ticketTitle))
                            {
                                ticketTitle = ticketTitle + "-" + removeableticket;
                                Message = Message + "- Exception Removing faviourat ticket : " + ex.Message;
                            }
                            else
                            {
                                ticketTitle = removeableticket.topic;
                                Message = "Exception Removing faviourat ticket : " + ex.Message;
                            }

                            break;
                        }

                        TicketMergeLog log = new TicketMergeLog();
                        try
                        {
                            List<TicketItem> listofticketitems =
                                (from ti in db.TicketItem where ti.ticketid == item select ti).ToList();
                            foreach (TicketItem item2 in listofticketitems)
                            {
                                TicketItem ticketitem = db.TicketItem.Find(item2.id);
                                ticketitem.ticketid = firstticketid;
                                db.Entry(ticketitem).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                        }
                        catch (Exception ex)
                        {
                            failed++;
                            if (!string.IsNullOrEmpty(ticketTitle))
                            {
                                ticketTitle = ticketTitle + "-" + removeableticket;
                                Message = Message + "- Exception when ticket items are merging : " + ex.Message;
                            }
                            else
                            {
                                ticketTitle = removeableticket.topic;
                                Message = "Exception when ticket items are merging : " + ex.Message;
                            }

                            break;
                        }

                        try
                        {
                            List<TicketTeamLogs> ticketteamlog = db.TicketTeamLogs.Where(tl => tl.ticketid == item).ToList();
                            foreach (TicketTeamLogs ttl in ticketteamlog)
                            {
                                TicketTeamLogs removeableticketteamlog = db.TicketTeamLogs.Find(ttl.id);
                                db.TicketTeamLogs.Remove(removeableticketteamlog);
                                db.SaveChanges();
                            }
                        }
                        catch (Exception ex)
                        {
                            failed++;
                            if (!string.IsNullOrEmpty(ticketTitle))
                            {
                                ticketTitle = ticketTitle + "-" + removeableticket;
                                Message = Message + "- Exception while removing from ticket team logs :" + ex.Message;
                            }
                            else
                            {
                                ticketTitle = removeableticket.topic;
                                Message = "Exception while removing from ticket team logs : " + ex.Message;
                            }

                            break;
                            //throw new Exception("Exception while removing from ticket team logs : " + ex.Message.ToString());
                        }

                        #region Move ticket to mergelog table

                        try
                        {
                            log.ticketid = removeableticket.id;
                            log.conversationid = removeableticket.conversationid;
                            log.uniquesenders = removeableticket.uniquesenders;
                            log.topic = removeableticket.topic;
                            log.lastdeliverytime = removeableticket.lastdeliverytime;
                            log.size = removeableticket.size;
                            log.messagecount = removeableticket.messagecount;
                            log.hasattachments = removeableticket.hasattachments;
                            log.importance = removeableticket.importance;
                            log.flagstatusid = removeableticket.flagstatusid;
                            log.lastmodifiedtime = removeableticket.lastmodifiedtime;
                            log.statusid = removeableticket.statusid;
                            log.statusupdatedbyusersid = removeableticket.statusupdatedbyusersid;
                            log.statusupdatedon = removeableticket.statusupdatedon;
                            log.createdonutc = removeableticket.createdonutc;
                            log.updatedonutc = removeableticket.updatedonutc;
                            log.userid = removeableticket.userid;
                            log.ipused = removeableticket.ipused;
                            log.tickettypeid = removeableticket.tickettypeid;
                            log.fromEmail = removeableticket.fromEmail;
                            log.startdate = removeableticket.startdate;
                            log.enddate = removeableticket.enddate;
                            log.projectid = removeableticket.projectid;
                            log.skillid = removeableticket.skillid;
                            log.mergebyuserid = User.Identity.GetUserId();
                            log.mergedinticketid = firstticketid;
                            db.TicketMergeLog.Add(log);
                            db.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            failed++;
                            if (!string.IsNullOrEmpty(ticketTitle))
                            {
                                ticketTitle = ticketTitle + "-" + removeableticket;
                                Message = Message + "- Exception while move ticket to mergelog table : " + ex.Message;
                            }
                            else
                            {
                                ticketTitle = removeableticket.topic;
                                Message = "Exception while move ticket to mergelog table : " + ex.Message;
                            }

                            break;
                            //throw new Exception("Exception while move ticket to mergelog table : " + ex.Message.ToString());
                        }

                        #endregion Move ticket to mergelog table

                        try
                        {
                            db.Ticket.Remove(removeableticket);
                            db.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            failed++;
                            if (!string.IsNullOrEmpty(ticketTitle))
                            {
                                ticketTitle = ticketTitle + "-" + removeableticket;
                                Message = Message + "- Exception while removing merged ticket : " + ex.Message;
                            }
                            else
                            {
                                ticketTitle = removeableticket.topic;
                                Message = "Exception while removing merged ticket : " + ex.Message;
                            }

                            break;
                            //throw new Exception("Exception while removing merged ticket : " + ex.Message.ToString());
                        }
                    }

                    if (string.IsNullOrEmpty(ticketids))
                    {
                        ticketids = item.ToString();
                    }
                    else
                    {
                        ticketids = ticketids + "-" + item;
                    }
                }

                return Json(
                    new
                    {
                        error = false,
                        fails = failed,
                        message = Message,
                        tickets = ticketTitle,
                        successfullmergeids = ticketids
                    }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = true, ex.Message, stacks = ex.StackTrace }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Draft()
        {
            List<TicketStatusViewModel> ticketstatuses = db.Database.SqlQuery<TicketStatusViewModel>("exec ticketstatus_loadticketscount")
                .ToList();
            List<TicketStatusViewModel> ticketstatusesCountAll =
                db.Database.SqlQuery<TicketStatusViewModel>("exec ticketstatus_loadticketscountAll").ToList();
            int totaltask = 0;
            foreach (TicketStatusViewModel status in ticketstatusesCountAll)
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
                ticketstatuses.SingleOrDefault(x => x.name.Equals("In Review")),
                ticketstatuses.SingleOrDefault(x => x.name.Equals("QC")),
                ticketstatuses.SingleOrDefault(x => x.name.Equals("Done")),
                ticketstatuses.SingleOrDefault(x => x.name.Equals("Trash")),
                ticketstatuses.SingleOrDefault(x => x.name.Equals("IsArchive"))
            };
            ViewBag.conversationstatus = newticketstatusesOrder;
            string usersid = User.Identity.GetUserId();
            return View(db.SentItemLog.Where(x => x.userid == usersid && !x.IsSent).ToList());
        }

        public ActionResult SentItem()
        {
            List<TicketStatusViewModel> ticketstatuses = db.Database.SqlQuery<TicketStatusViewModel>("exec ticketstatus_loadticketscount")
                .ToList();
            List<TicketStatusViewModel> ticketstatusesCountAll =
                db.Database.SqlQuery<TicketStatusViewModel>("exec ticketstatus_loadticketscountAll").ToList();
            int totaltask = 0;
            foreach (TicketStatusViewModel status in ticketstatusesCountAll)
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
                ticketstatuses.SingleOrDefault(x => x.name.Equals("In Review")),
                ticketstatuses.SingleOrDefault(x => x.name.Equals("QC")),
                ticketstatuses.SingleOrDefault(x => x.name.Equals("Done")),
                ticketstatuses.SingleOrDefault(x => x.name.Equals("Trash")),
                ticketstatuses.SingleOrDefault(x => x.name.Equals("IsArchive"))
            };
            ViewBag.conversationstatus = newticketstatusesOrder;
            string userid = User.Identity.GetUserId();
            return View(db.SentItemLog.Where(x => x.userid == userid && x.IsSent).ToList());
        }

        //ADD Notification
        private void AddNotification(long entityid, long actionid)
        {
            try
            {
                List<string> Managerslist = new List<string>();
                Notification notification = new Notification
                {
                    commentid = 0
                };
                List<string> listusers = new List<string>();

                Ticket ticket = (from t in db.Ticket where t.id == entityid select t).FirstOrDefault();
                long ticketitem = (from t in db.TicketItem where t.ticketid == entityid select t.id).FirstOrDefault();
                SendNotificationViewModel send = new SendNotificationViewModel();
                string userid = User.Identity.GetUserId();
                ApplicationUser activeuser = db.Users.Where(u => u.Id == userid).FirstOrDefault();
                switch (actionid)
                {
                    case 1:
                        // ticket status change
                        send.title = "Ticket status updated";
                        notification.description = activeuser.FullName + " updated status to  " +
                                                   ticket.ConversationStatus.name + " '" + ticket.topic + "'";
                        break;

                    case 6:
                        //Assign to user
                        notification.description =
                            activeuser.FullName + " assigned you a new ticket. '" + ticket.topic + "'";
                        send.title = "New Ticket Assigned";
                        break;

                    case 7:
                        // ticket status change
                        notification.description = User.Identity.Name + " added a new comment. '" + ticket.topic + "'";
                        send.title = "New comment added";
                        break;
                }

                List<string> assignedUsers =
                    (from t in db.TicketItemLog where t.ticketitemid == ticketitem select t.assignedtousersid)
                    .Distinct().ToList();
                if (assignedUsers.Count() > 0)
                {
                    notification.actorid = User.Identity.GetUserId();
                    notification.entityid = entityid;
                    notification.entityactionid = actionid;
                    notification.createdon = DateTime.Now;
                    db.Notification.Add(notification);
                    db.SaveChanges();
                    // find relevent members of ticket
                    foreach (string user in assignedUsers)
                    {
                        if (user != User.Identity.GetUserId())
                        {
                            NotificationUsers notificationuser = new NotificationUsers
                            {
                                notification_Id = notification.id,
                                notifierid = user,
                                status = false
                            };
                            db.NotificationUsers.Add(notificationuser);
                            db.SaveChanges();
                            listusers = assignedUsers;
                        }
                    }
                }

                send.users = listusers;
                send.notification = notification;
                if (listusers.Count() > 0)
                {
                    NotificatonViewmodel.SendPushNotification(send);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void AddNotification(long entityid, long actionid, List<string> users)
        {
            try
            {
                bool notifymanager = false;
                List<string> Managerslist = new List<string>();
                Notification notification = new Notification
                {
                    commentid = 0
                };
                List<string> listusers = new List<string>();

                Ticket ticket = (from t in db.Ticket where t.id == entityid select t).FirstOrDefault();
                long ticketitem = (from t in db.TicketItem where t.ticketid == entityid select t.id).FirstOrDefault();
                SendNotificationViewModel send = new SendNotificationViewModel();
                string userid = User.Identity.GetUserId();
                ApplicationUser activeuser = db.Users.Where(u => u.Id == userid).FirstOrDefault();
                switch (actionid)
                {
                    case 1:
                        // ticket status change
                        send.title = "Ticket status updated";
                        notification.description = activeuser.FullName + " updated status to  " +
                                                   ticket.ConversationStatus.name + " '" + ticket.topic + "'";
                        break;

                    case 6:
                        //Assign to user
                        notification.description =
                            activeuser.FullName + " assigned you a new ticket. '" + ticket.topic + "'";
                        send.title = "New Ticket Assigned";
                        notifymanager = true;
                        break;

                    case 7:
                        // ticket status change
                        notification.description = User.Identity.Name + " added a new comment. '" + ticket.topic + "'";
                        send.title = "New comment added";
                        break;
                }

                if (users.Count() <= 0 && users == null)
                {
                    List<string> assignedUsers =
                        (from t in db.TicketItemLog where t.ticketitemid == ticketitem select t.assignedtousersid)
                        .Distinct().ToList();
                    if (assignedUsers.Count() > 0)
                    {
                        notification.actorid = User.Identity.GetUserId();
                        notification.entityid = entityid;
                        notification.entityactionid = actionid;
                        notification.createdon = DateTime.Now;
                        db.Notification.Add(notification);
                        db.SaveChanges();
                        // find relevent members of ticket
                        foreach (string user in assignedUsers)
                        {
                            if (notifymanager)
                            {
                                long teamid = (from x in db.TeamMember where x.usersid == user select x.teamid)
                                    .FirstOrDefault();
                                TeamMember managerid =
                                    (from t in db.TeamMember where t.IsManager && t.teamid == teamid select t)
                                    .FirstOrDefault();
                                if (managerid != null)
                                {
                                    Managerslist.Add(managerid.usersid);
                                }
                            }

                            if (user != User.Identity.GetUserId())
                            {
                                NotificationUsers notificationuser = new NotificationUsers
                                {
                                    notification_Id = notification.id,
                                    notifierid = user,
                                    status = false
                                };
                                db.NotificationUsers.Add(notificationuser);
                                db.SaveChanges();
                                listusers = assignedUsers;
                            }
                        }
                    }
                }
                else
                {
                    notification.actorid = User.Identity.GetUserId();
                    notification.entityid = entityid;
                    notification.entityactionid = actionid;
                    notification.createdon = DateTime.Now;
                    db.Notification.Add(notification);
                    db.SaveChanges();
                    //var userobj = db.Users.Where(u => u.Id == users).FirstOrDefault();
                    foreach (string user in users)
                    {
                        if (notifymanager)
                        {
                            long teamid = (from x in db.TeamMember where x.usersid == user select x.teamid)
                                .FirstOrDefault();
                            TeamMember managerid = (from t in db.TeamMember where t.IsManager && t.teamid == teamid select t)
                                .FirstOrDefault();
                            if (managerid != null)
                            {
                                Managerslist.Add(managerid.usersid);
                            }
                        }

                        // find relevent members of ticket
                        if (user != User.Identity.GetUserId())
                        {
                            NotificationUsers notificationuser = new NotificationUsers
                            {
                                notification_Id = notification.id,
                                notifierid = user,
                                status = false
                            };
                            db.NotificationUsers.Add(notificationuser);
                            db.SaveChanges();
                            listusers.Add(user);
                        }
                    }
                }

                //Send notification to managers
                if (Managerslist != null)
                {
                    //create a new notification for manger
                    Notification managernotification = new Notification
                    {
                        //person who create that notification
                        actorid = User.Identity.GetUserId(),
                        //item id which has been modified
                        entityid = entityid,
                        //the action implements on above item
                        entityactionid = actionid,
                        description = activeuser.FullName + " assigned a new ticket to "
                    };
                    //notification description
                    foreach (string user in users)
                    {
                        ApplicationUser userobj = db.Users.Where(u => u.Id == user).FirstOrDefault();
                        managernotification.description += "'" + userobj.FullName + "',";
                    }

                    managernotification.description += " '" + ticket.topic + "'";
                    managernotification.createdon = DateTime.Now;
                    db.Notification.Add(managernotification);
                    db.SaveChanges();
                    foreach (string manager in Managerslist.Distinct())
                    {
                        if (manager != User.Identity.GetUserId())
                        {
                            ApplicationUser notifiedManagers = db.Users
                                .Where(u => u.Id == manager && u.IsNotifyManagerOnTaskAssignment).FirstOrDefault();
                            //add notification to manager
                            if (notifiedManagers != null)
                            {
                                NotificationUsers notificationuser = new NotificationUsers
                                {
                                    notification_Id = managernotification.id,
                                    notifierid = manager,
                                    status = false
                                };
                                db.NotificationUsers.Add(notificationuser);
                                db.SaveChanges();

                                //Send Push notification for managers
                                List<string> managerlist = new List<string>
                                {
                                    manager
                                };
                                SendNotificationViewModel send1 = new SendNotificationViewModel
                                {
                                    title = "New ticket assigned to team member",
                                    notification = managernotification,
                                    users = managerlist
                                };
                                NotificatonViewmodel.SendPushNotification(send1);
                            }
                        }
                    }
                }

                send.users = listusers;
                send.notification = notification;
                if (listusers.Count() > 0)
                {
                    NotificatonViewmodel.SendPushNotification(send);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void AddNotification(long entityid, long actionid, long commentid, string users = "")
        {
            try
            {
                Notification notification = new Notification();
                List<string> listusers = new List<string>();
                Ticket ticket = (from t in db.Ticket where t.id == entityid select t).FirstOrDefault();
                long ticketitem = (from t in db.TicketItem where t.ticketid == entityid select t.id).FirstOrDefault();
                SendNotificationViewModel send = new SendNotificationViewModel();
                string userid = User.Identity.GetUserId();
                ApplicationUser activeuser = db.Users.Where(u => u.Id == userid).FirstOrDefault();
                switch (actionid)
                {
                    case 1:
                        // ticket status change
                        send.title = "Ticket status updated";
                        notification.description = activeuser.FullName + " updated status to  " +
                                                   ticket.ConversationStatus.name + " '" + ticket.topic + "'";
                        break;

                    case 6:
                        //Assign to user
                        notification.description =
                            activeuser.FullName + " assigned you a new ticket. '" + ticket.topic + "'";
                        send.title = "New Ticket Assigned";
                        break;

                    case 7:
                        // ticket status change
                        notification.description = User.Identity.Name + " added a new comment. '" + ticket.topic + "'";
                        send.title = "New comment added";
                        notification.commentid = commentid;
                        break;
                }

                if (string.IsNullOrEmpty(users))
                {
                    List<string> assignedUsers =
                        (from t in db.TicketItemLog
                         where t.ticketitemid == ticketitem && t.assignedtousersid != userid
                         select t.assignedtousersid).Distinct().ToList();
                    if (assignedUsers.Count() > 0)
                    {
                        notification.actorid = User.Identity.GetUserId();
                        notification.entityid = entityid;
                        notification.entityactionid = actionid;
                        notification.createdon = DateTime.Now;
                        db.Notification.Add(notification);
                        db.SaveChanges();
                        // find relevent members of ticket
                        foreach (string user in assignedUsers)
                        {
                            if (user != User.Identity.GetUserId())
                            {
                                NotificationUsers notificationuser = new NotificationUsers
                                {
                                    notification_Id = notification.id,
                                    notifierid = user,
                                    status = false
                                };
                                db.NotificationUsers.Add(notificationuser);
                                db.SaveChanges();
                                listusers = assignedUsers;
                            }
                        }
                    }
                }
                else
                {
                    notification.actorid = User.Identity.GetUserId();
                    notification.entityid = entityid;
                    notification.entityactionid = actionid;
                    notification.createdon = DateTime.Now;
                    db.Notification.Add(notification);
                    db.SaveChanges();
                    // find relevent members of ticket
                    if (users != User.Identity.GetUserId())
                    {
                        NotificationUsers notificationuser = new NotificationUsers
                        {
                            notification_Id = notification.id,
                            notifierid = users,
                            status = false
                        };
                        db.NotificationUsers.Add(notificationuser);
                        db.SaveChanges();
                        listusers.Add(users);
                    }
                }

                send.users = listusers;
                send.notification = notification;
                if (listusers.Count() > 0)
                {
                    NotificatonViewmodel.SendPushNotification(send);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private string Base64Decode(string input)
        {
            string base64Decoded;
            byte[] data = Convert.FromBase64String(input);
            base64Decoded = Encoding.ASCII.GetString(data);
            return base64Decoded;
        }

        #region Constructors

        private ApplicationUserManager _userManager;

        public ApplicationUserManager UserManager
        {
            get => _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            private set => _userManager = value;
        }

        public TicketsController()
        {
        }

        public TicketsController(ApplicationUserManager userManager)
        {
            UserManager = userManager;
        }

        #endregion Constructors

        #region Inbox

        //public ActionResult ChangeStatus(int newstatusid, long ticketid)
        //{
        //    Ticket ticket = db.Ticket.Where(x => x.id == ticketid).SingleOrDefault();
        //    if (ticket != null)
        //    {
        //        TicketStatus TicketstatusBefore = db.TicketStatus.Find(db.Ticket.Find(ticketid).statusid);
        //        ticket.statusid = newstatusid;
        //        db.SaveChanges();
        //        /*By Muhammad Nasir on 28-11-2018*/
        //        TicketLogs TicketLogs = new TicketLogs
        //        {
        //            ticketid = Convert.ToInt64(ticketid),
        //            actiontypeid = 9,
        //            actiondate = DateTime.Now,
        //            actionbyuserId = User.Identity.GetUserId()
        //        };

        //        //The ticket # 160101 is assigned to [Shaban Sarfraz] by [Bratislav].
        //        TicketStatus Ticketstatus = db.TicketStatus.Find(newstatusid);
        //        ApplicationUser userAdmin = UserManager.FindById(Convert.ToString(User.Identity.GetUserId()));

        //        TicketLogs.ActionDescription = "The <a href='/tickets/ticketitem/" + Convert.ToString(ticketid) + "'>ticket #" + Convert.ToString(ticketid) + "</a> status is changed from <b>[" + TicketstatusBefore.name + "]</b> to <b>[" + Ticketstatus.name + "]</b> by [" + userAdmin.FirstName + " " + userAdmin.LastName + "] on " + Convert.ToString(DateTime.Now);
        //        db.TicketLogs.Add(TicketLogs);
        //        db.SaveChanges();

        //        AddNotification(ticketid, 1);
        //        return Json("Success", JsonRequestBehavior.AllowGet);
        //    }
        //    return Json("item not found", JsonRequestBehavior.AllowGet);
        //}

        public ActionResult ChangeStatus(int newstatusid, long ticketid)
        {
            Ticket ticket = db.Ticket.Find(ticketid);
            TicketStatus Oldticketstaus = db.TicketStatus.Find(ticket.statusid);
            try
            {
                if (ticket.statusid == (int)TicketsStatus.InProgress && newstatusid == (int)TicketsStatus.InProgress)
                {
                    return Json(new { error = true, errortext = "Sorry this task already in progress." });
                }

                ticket.statusid = newstatusid;
                ticket.statusupdatedbyusersid = User.Identity.GetUserId();
                ticket.updatedonutc = DateTime.Now;
                ticket.statusupdatedon = DateTime.Now;
                ticket.LastActivityDate = DateTime.Now;
                ticket.ipused = Request.UserHostAddress;
                ticket.userid = User.Identity.GetUserId();
                ticket.LastActivityDate = DateTime.Now;
                db.Entry(ticket).State = EntityState.Modified;
                db.SaveChanges();
                IQueryable<TicketItem> TicketItem = db.TicketItem.Where(t => t.ticketid == ticket.id).ToList().AsQueryable();
                if (TicketItem.Count() != 0 && TicketItem != null)
                {
                    foreach (TicketItem items in TicketItem)
                    {
                        if (items.statusid == (int)TicketsStatus.NewTask ||
                            items.statusid == (int)TicketsStatus.InProgress) //2
                        {
                            TicketItem ticketitem = db.TicketItem.Where(i => i.id == items.id).FirstOrDefault();
                            if (newstatusid == (int)TicketsStatus.OnHold) //4
                            {
                                ticketitem.statusid = 4; //4
                            }
                            else if (newstatusid == (int)TicketsStatus.Done) //3
                            {
                                ticketitem.statusid = 3; //3
                            }
                            else
                            {
                                ticketitem.statusid = newstatusid;
                            }

                            ticketitem.statusupdatedbyusersid = User.Identity.GetUserId();
                            ticketitem.updatedonutc = DateTime.Now;
                            ticketitem.statusupdatedon = DateTime.Now;
                            ticketitem.ipused = Request.UserHostAddress;
                            ticketitem.userid = User.Identity.GetUserId();
                            db.Entry(ticketitem).State = EntityState.Modified;
                            db.SaveChanges();
                        }

                        List<TicketItemLog> ticketitemlog = db.TicketItemLog.Where(t => t.ticketitemid == items.id).ToList();
                        if (ticketitemlog.Count > 0)
                        {
                            foreach (TicketItemLog logs in ticketitemlog)
                            {
                                if (logs.statusid == (int)TicketsStatus.NewTask ||
                                    logs.statusid == (int)TicketsStatus.InProgress)
                                {
                                    if (newstatusid == (int)TicketsStatus.OnHold) //4
                                    {
                                        logs.statusid = 4;
                                    }
                                    else if (newstatusid == (int)TicketsStatus.Done) //3
                                    {
                                        logs.statusid = 3;
                                    }
                                    else
                                    {
                                        logs.statusid = newstatusid;
                                    }

                                    logs.ticketitemid = items.id;
                                    logs.statusupdatedbyusersid = User.Identity.GetUserId();
                                    logs.statusupdatedon = DateTime.Now;
                                    db.Entry(logs).State = EntityState.Modified;
                                    db.SaveChanges();
                                }
                            }
                        }
                    }
                }

                TicketLogs TicketLogs = new TicketLogs
                {
                    ticketid = Convert.ToInt64(ticketid),
                    actiontypeid = 9,
                    actiondate = DateTime.Now,
                    actionbyuserId = User.Identity.GetUserId()
                };
                TicketStatus Newticketstatus = db.TicketStatus.Find(newstatusid);
                ApplicationUser userAdmin = UserManager.FindById(Convert.ToString(User.Identity.GetUserId()));
                TicketLogs.ActionDescription = "The <a href='/tickets/ticketitem/" + Convert.ToString(ticketid) +
                                               "'>ticket #" + Convert.ToString(ticketid) +
                                               "</a> status is changed from <b>[" + Oldticketstaus.name +
                                               "]</b> to <b>[" + Newticketstatus.name + "]</b> by [" +
                                               userAdmin.FirstName + " " + userAdmin.LastName + "] on " +
                                               Convert.ToString(DateTime.Now);
                ;
                db.TicketLogs.Add(TicketLogs);
                db.SaveChanges();
                return Json(new { success = true, successtext = "The ticket status updated." });
            }
            catch (Exception)
            {
                return Json("item not found", JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ChangeDates(string startDate, string endDate, long ticketid)
        {
            Ticket ticket = db.Ticket.Where(x => x.id == ticketid).SingleOrDefault();
            if (ticket != null)
            {
                ticket.startdate = Convert.ToDateTime(startDate);
                ticket.enddate = Convert.ToDateTime(endDate);
                int i = db.SaveChanges();
                return Json("Success", JsonRequestBehavior.AllowGet);
            }

            return Json("item not found", JsonRequestBehavior.AllowGet);
        }

        // GET: Tickets
        [ValidateInput(false)]
        public ActionResult Index(int? id, string searchvalue = "")
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Tickets", new { id = 1 });
            }

            if (!string.IsNullOrEmpty(searchvalue))
            {
                searchvalue = Base64Decode(searchvalue);
            }

            TicketViewModel tvm = new TicketViewModel();
            //ViewBag.status = db.ConversationStatus.Where(i => i.id != 2);
            List<SelectListItem> projectlist = db.Database.SqlQuery<CombinedEntity>("exec projects_loadcombined").Select(x =>
                new SelectListItem
                {
                    Text = x.name,
                    Value = x.id.ToString()
                }).ToList();
            List<SelectListItem> ClientSelectList = new SelectList(db.Client.Where(x => x.isactive).ToList(), "id", "name").ToList();
            ViewBag.clients = ClientSelectList;
            ViewBag.projects = new SelectList(projectlist, "Value", "Text");
            ViewBag.skills = db.Skill.ToList().AsQueryable();
            ViewBag.statusid = id;
            ViewBag.tickettype = 0;
            List<TicketStatusViewModel> ticketstatuses = db.Database.SqlQuery<TicketStatusViewModel>("exec ticketstatus_loadticketscount")
                .ToList();
            List<TicketStatusViewModel> ticketstatusesCountAll =
                db.Database.SqlQuery<TicketStatusViewModel>("exec ticketstatus_loadticketscountAll").ToList();
            int totaltask = 0;
            foreach (TicketStatusViewModel status in ticketstatusesCountAll)
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
                ticketstatuses.SingleOrDefault(x => x.name.Equals("In Review")),
                ticketstatuses.SingleOrDefault(x => x.name.Equals("QC")),
                ticketstatuses.SingleOrDefault(x => x.name.Equals("Done")),
                ticketstatuses.SingleOrDefault(x => x.name.Equals("Trash")),
                ticketstatuses.SingleOrDefault(x => x.name.Equals("IsArchive"))
            };
            ViewBag.conversationstatus = newticketstatusesOrder;
            ViewBag.IsArchieved = db.Ticket.Where(x => x.IsArchieved == true).AsQueryable().Count();

            ViewBag.currentSubTab = GetCurrentActiveSubTab();

            //ViewBag.counter = db.Ticket.Include(t => t.FlagStatus).Include(t => t.StatusUpdatedByUser).Include(t => t.ConversationStatus).Where(t => t.statusid == id).OrderByDescending(t => t.lastdeliverytime).Where(i => i.statusid == id).Count();
            ViewBag.users = UserManager.Users.OrderBy(f => f.FirstName).Where(u => u.isactive == true).ToList().AsQueryable();
            IQueryable<Team> ActiveTeams = db.Team.OrderBy(f => f.name).Where(f => f.isactive == true).ToList().AsQueryable();
            ViewBag.teams = ActiveTeams;
            string userid = User.Identity.GetUserId();
            IQueryable<TicketUserFlagged> flag = db.TicketUserFlagged.Where(f => f.isactive && f.userid == userid).ToList().AsQueryable();
            if (string.IsNullOrEmpty(searchvalue))
            {
                //IQueryable<Ticket> ticket = new IQueryable<Ticket>();
                IQueryable<Ticket> ticket;
                if (id == 0)
                {
                    ticket = (from tt in db.Ticket
                              join ticketi in db.TicketItem on tt.id equals ticketi.ticketid
                              select tt
                        ).Include(t => t.FlagStatus).Include(t => t.StatusUpdatedByUser).Include(tt => tt.TicketType)
                        .Include(t => t.ConversationStatus).GroupBy(t => t.id).Select(t => t.FirstOrDefault())
                        .OrderByDescending(t => t.lastdeliverytime).Take(recordsPerPage).ToList().AsQueryable();
                }
                else
                {
                    ticket = (from tt in db.Ticket
                              join ticketi in db.TicketItem on tt.id equals ticketi.ticketid
                              select tt
                        ).Include(t => t.FlagStatus).Include(t => t.StatusUpdatedByUser).Include(tt => tt.TicketType)
                        .Include(t => t.ConversationStatus).Where(t => t.statusid == id).GroupBy(t => t.id)
                        .Select(t => t.FirstOrDefault()).OrderByDescending(t => t.lastdeliverytime).Take(recordsPerPage)
                        .ToList().AsQueryable();
                }

                List<TicketItem> ti = new List<TicketItem>();
                foreach (Ticket items in ticket)
                {
                    TicketItem ticketitem = db.TicketItem.Where(t => t.ticketid == items.id)
                        .OrderByDescending(t => t.createdonutc).AsQueryable().FirstOrDefault();
                    ti.Add(ticketitem);
                }

                tvm.tickets = ticket;
                tvm.ticketitems = ti;
                tvm.flaggeditems = flag;
                tvm.teams = ActiveTeams;
            }
            else
            {
                //List<Ticket> ticket = new List<Ticket>();
                IQueryable<Ticket> ticket;
                if (id == 0)
                {
                    ticket = (from tt in db.Ticket
                              join ticketi in db.TicketItem on tt.id equals ticketi.ticketid
                              select tt
                        ).Where(t =>
                            t.topic.Contains(searchvalue) || t.uniquesenders.Contains(searchvalue) ||
                            t.fromEmail.Contains(searchvalue)).Include(t => t.FlagStatus)
                        .Include(t => t.StatusUpdatedByUser).Include(tt => tt.TicketType)
                        .Include(t => t.ConversationStatus).GroupBy(t => t.id).Select(t => t.FirstOrDefault())
                        .OrderByDescending(t => t.lastdeliverytime).Take(recordsPerPage).ToList().AsQueryable();
                }
                else
                {
                    ticket = (from tt in db.Ticket
                              join ticketi in db.TicketItem on tt.id equals ticketi.ticketid
                              select tt
                        ).Where(t =>
                            t.topic.Contains(searchvalue) || t.uniquesenders.Contains(searchvalue) ||
                            t.fromEmail.Contains(searchvalue)).Include(t => t.FlagStatus)
                        .Include(t => t.StatusUpdatedByUser).Include(tt => tt.TicketType)
                        .Include(t => t.ConversationStatus).Where(t => t.statusid == id).GroupBy(t => t.id)
                        .Select(t => t.FirstOrDefault()).OrderByDescending(t => t.lastdeliverytime).Take(recordsPerPage)
                        .ToList().AsQueryable();
                }

                List<TicketItem> ti = new List<TicketItem>();
                foreach (Ticket items in ticket)
                {
                    TicketItem ticketitem = db.TicketItem.Where(t => t.ticketid == items.id)
                        .OrderByDescending(t => t.createdonutc).AsQueryable().FirstOrDefault();
                    ti.Add(ticketitem);
                }

                tvm.tickets = ticket;
                tvm.ticketitems = ti;
                tvm.flaggeditems = flag;
                tvm.teams = ActiveTeams;
                ViewBag.search = searchvalue;
            }

            return View("Index", tvm);
        }

        public ActionResult FetchUsersAndTeams(long? Ticketid)
        {
            // If not logged in, redirect to login page.
            //if (!IsLoggedIn()) return RedirectToAction("login", "account");

            List<ApplicationUser> users = (from t in db.Ticket
                                           join ti in db.TicketItem on t.id equals ti.ticketid
                                           join til in db.TicketItemLog on ti.id equals til.ticketitemid
                                           join u in db.Users on til.assignedtousersid equals u.Id
                                           where t.id == Ticketid
                                           select u).ToList();

            List<Team> teams = (from t in db.Ticket
                                join ttl in db.TicketTeamLogs on t.id equals ttl.ticketid
                                join tm in db.Team on ttl.teamid equals tm.id
                                where t.id == Ticketid
                                select tm).ToList();
            Ticket ticket = db.Ticket.Find(Ticketid);

            if (users != null || teams != null)
            {
                return Json(
                    new
                    {
                        error = 0,
                        Users = users,
                        Teams = teams,
                        projectId = ticket.projectid,
                        skillId = ticket.skillid,
                        Ticketstatus = ticket.statusid
                    }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { error = 1, TextContext = "Sorry! no ticket found with this User or Team" },
                JsonRequestBehavior.AllowGet);
        }

        public ActionResult LoadTicketDetail(long id)
        {
            try
            {
                var ticketsQuery = (from tt in db.Ticket
                                    join teamlog in db.TicketTeamLogs on tt.id equals teamlog.ticketid
                                    join team in db.Team on teamlog.teamid equals team.id
                                    join ti in db.TicketItem on tt.id equals ti.ticketid
                                    join til in db.TicketItemLog on ti.id equals til.ticketitemid
                                    where tt.id == id
                                    select new
                                    {
                                        ticketid = tt.id,
                                        createddate = tt.createdonutc,
                                        teamname = team.name,
                                        AssignedToTeamDate = teamlog.assignedon,
                                        AssignedByTeamMember = teamlog.assignedbyusersid,
                                        AssignedToTeamMember = til.assignedtousersid,
                                        AssignedToTeamMemberByuserId = til.assignbyuser
                                    }).FirstOrDefault();

                return Json(new
                {
                    ticketsQuery.ticketid,
                    ticketsQuery.createddate,
                    teamname = ticketsQuery.AssignedToTeamDate,
                    AssigntoteamByUser = ticketsQuery.AssignedByTeamMember
                });
            }
            catch
            {
                return PartialView("~/Views/Shared/_error.cshtml");
            }
        }

        [HttpGet]
        [ValidateInput(false)]
        public ActionResult SearchTickets(string searchstring, int statusid, int pagenum)
        {
            try
            {
                // trim user search string.
                searchstring = searchstring.Trim();

                int skipRecords = pagenum * recordsPerPage;
                // Fetch Tickets.
                IQueryable<Ticket> SearchResults = null;
                if (!string.IsNullOrEmpty(searchstring))
                {
                    if (statusid == 0)
                    {
                        SearchResults = db.Ticket.Include(ti => ti.TicketItems).Include(tt => tt.TicketType)
                            .Include(t => t.ConversationStatus)
                            .Where(t => t.topic.Contains(searchstring) || t.uniquesenders.Contains(searchstring))
                            .OrderByDescending(t => t.lastdeliverytime).Skip(skipRecords).Take(recordsPerPage).ToList().AsQueryable();
                    }
                    else
                    {
                        //SearchResults = db.Ticket.Include(ti => ti.TicketItems).Include(tt => tt.TicketType).Include(t => t.ConversationStatus).Where(t => t.fromEmail.Contains(searchstring) || t.statusid == statusid).Where(t => t.topic.Contains(searchstring) || t.uniquesenders.Contains(searchstring)).OrderByDescending(t => t.lastdeliverytime).Skip(skipRecords).Take(recordsPerPage).ToList();
                        SearchResults = db.Ticket.Include(ti => ti.TicketItems).Include(tt => tt.TicketType)
                            .Include(t => t.ConversationStatus)
                            .Where(t => t.topic.Contains(searchstring) || t.uniquesenders.Contains(searchstring) ||
                                        t.fromEmail.Contains(searchstring)).Where(t => t.statusid == statusid)
                            .OrderByDescending(t => t.lastdeliverytime).Skip(skipRecords).Take(recordsPerPage).ToList().AsQueryable();
                    }
                }
                else
                {
                    if (statusid == 0)
                    {
                        SearchResults = db.Ticket.Include(ti => ti.TicketItems).Include(tt => tt.TicketType)
                            .Include(t => t.ConversationStatus).OrderByDescending(t => t.lastdeliverytime).Take(50)
                            .ToList().AsQueryable();
                    }
                    else
                    {
                        SearchResults = db.Ticket.Include(ti => ti.TicketItems).Include(tt => tt.TicketType)
                            .Include(t => t.ConversationStatus).Where(t => t.statusid == statusid)
                            .OrderByDescending(t => t.lastdeliverytime).Take(50).ToList().AsQueryable();
                    }
                }

                IQueryable<Team> ActiveTeams = db.Team.OrderBy(f => f.name).Where(f => f.isactive == true).ToList().AsQueryable();

                TicketViewModel tvModel = new TicketViewModel
                {
                    tickets = SearchResults,
                    teams = ActiveTeams
                };
                ViewBag.status = db.ConversationStatus;
                //return PartialView("_ConversationRow", tvModel);
                int itemcount = SearchResults.Count();
                //ViewBag.tickettype = 0;
                int totalcount = 0;
                if (statusid == 0)
                {
                    totalcount = db.Ticket
                        .Where(t => t.fromEmail.Contains(searchstring) || t.topic.Contains(searchstring) ||
                                    t.uniquesenders.Contains(searchstring)).OrderByDescending(t => t.lastdeliverytime)
                        .Count();
                }
                else
                {
                    totalcount = db.Ticket.Where(s => s.statusid == statusid)
                        .Where(t => t.fromEmail.Contains(searchstring) || t.topic.Contains(searchstring) ||
                                    t.uniquesenders.Contains(searchstring)).OrderByDescending(t => t.lastdeliverytime)
                        .Count();
                }

                string tickets = PartialView("_ConversationRow", tvModel).RenderToString();
                return Json(new { tickets, itemcount, totalcount }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return PartialView("_ConversationRow", new TicketViewModel());
            }
        }

        [ValidateInput(false)]
        [HttpPost]
        public ActionResult IndexAjax(int id, int? pagenum, string topic = "")
        {
            int page = pagenum ?? 0;

            if (Request.IsAjaxRequest())
            {
                TicketViewModel items = GetPaginatedTickets(id, topic, page);
                int itemcount = items.tickets.Count();
                int totalcount = 0;
                if (!string.IsNullOrEmpty(topic))
                {
                    if (id == 0)
                    {
                        totalcount = db.Ticket.Where(t => t.topic.Contains(topic)).Count();
                    }
                    else
                    {
                        totalcount = db.Ticket.Where(s =>
                            s.statusid == id && s.topic.Contains(topic) && s.IsArchieved == false).Count();
                    }
                }
                else
                {
                    if (id == 0)
                    {
                        totalcount = db.Ticket.Count();
                    }
                    else if (id == 9)
                    {
                        totalcount = db.Ticket.Where(s => s.IsArchieved == true).Count();
                    }
                    else
                    {
                        totalcount = db.Ticket.Where(s => s.statusid == id && s.IsArchieved == false).Count();
                    }
                }

                //List<Team> ActiveTeams = db.Team.OrderBy(f => f.name).Where(f => f.isactive == true).ToList().AsQueryable();
                items.teams = db.Team.OrderBy(f => f.name).Where(f => f.isactive == true).ToList().AsQueryable();
                string tickets = PartialView("_ConversationRow", items).RenderToString();
                return Json(new { tickets, itemcount, totalcount }, JsonRequestBehavior.AllowGet);
                //return PartialView("_ConversationRow", GetPaginatedTickets(id, page));
            }

            ViewBag.status = db.ConversationStatus;
            List<SelectListItem> projectlist = db.Database.SqlQuery<CombinedEntity>("exec projects_loadcombined").Select(x =>
                new SelectListItem
                {
                    Text = x.name,
                    Value = x.id.ToString()
                }).ToList();
            ViewBag.projects = new SelectList(projectlist, "Value", "Text");
            ViewBag.skills = db.Skill.ToList();
            ViewBag.users = UserManager.Users.OrderBy(f => f.FirstName).Where(u => u.isactive == true).ToList();
            IQueryable<Ticket> ticket = db.Ticket.Include(t => t.FlagStatus).Include(tt => tt.TicketType)
                .Include(t => t.StatusUpdatedByUser).Include(t => t.ConversationStatus).Where(t => t.statusid == id)
                .GroupBy(t => t.id).Select(t => t.FirstOrDefault()).OrderByDescending(t => t.lastdeliverytime)
                .Take(recordsPerPage).AsQueryable();
            List<TicketItem> ti = new List<TicketItem>();
            TicketViewModel tvm = new TicketViewModel
            {
                tickets = ticket,
                ticketitems = ti
            };
            IQueryable<TicketUserFlagged> flaggedticket = db.TicketUserFlagged
                .Where(x => x.userid == User.Identity.GetUserId() && x.isactive == true).ToList().AsQueryable();
            tvm.flaggeditems = flaggedticket;
            foreach (Ticket items in ticket)
            {
                TicketItem ticketitem = db.TicketItem.Where(t => t.ticketid == items.id).OrderByDescending(t => t.createdonutc)
                    .FirstOrDefault();
                ti.Add(ticketitem);
            }

            return View("Index", tvm);
        }

        public ActionResult AssignTickets(string ticketcsv, long projectid, long skillid, string StartDate,
            string EndDate, string usercsv, string teamcsv, string comment, int? EstimatedTime)
        {
            try
            {
                // validate required information.
                if (string.IsNullOrEmpty(ticketcsv))
                {
                    return Json(new { error = true, errortext = "Please select at least one ticket for assignment." });
                }

                if (projectid < 1)
                {
                    return Json(new { error = true, errortext = "Sorry, project is required." });
                }

                if (skillid < 1)
                {
                    return Json(new { error = true, errortext = "Sorry, skill is required." });
                }

                if (string.IsNullOrEmpty(usercsv) && string.IsNullOrEmpty(teamcsv))
                {
                    return Json(new { error = true, errortext = "Please assign ticket to at least one user or team." });
                }

                // Make sure at least one ticket has been selected.
                string[] Tickets = ticketcsv.Split(',');
                if (Tickets == null)
                {
                    return Json(new { error = true, errortext = "Please select at least one ticket for assignment." });
                }

                TicketAssignmentResult ticketResult = new TicketAssignmentResult
                { ResultType = TicketAssignmentResultType.Error, ResultMessage = "Sorry, no tickets processed." };

                foreach (string ticketitem in Tickets)
                {
                    //Removing Already Exists assigned teams Related to same Ticket
                    long ticketid = Convert.ToInt64(ticketitem);

                    //TicketTeamLogs ticketteamlogs = db.TicketTeamLogs.Find(ticketid);
                    //db.TicketTeamLogs.Remove(ticketteamlogs);
                    //db.SaveChanges();

                    ticketResult = AssignTicketItem(ticketid, projectid, skillid, StartDate, EndDate, usercsv, teamcsv,
                        comment, EstimatedTime);

                    if (ticketResult.ResultType == TicketAssignmentResultType.Error)
                    {
                        break;
                    }
                }

                // Check latest result, if no error occured it should be success.
                if (ticketResult.ResultType == TicketAssignmentResultType.Success)
                {
                    db.SaveChanges();
                    /* Ticket Logs by Muhammad Nasir 19-11-2018 */
                    string[] assignedUsers = usercsv.Split(',');
                    string assignedUserNames = string.Empty;
                    if (assignedUsers.Length > 0)
                    {
                        for (int counter = 0; counter < assignedUsers.Length; counter = counter + 1)
                        {
                            ApplicationUser user = UserManager.FindById(Convert.ToString(assignedUsers[counter]));
                            assignedUserNames = user.FirstName + " " + user.LastName + "," + assignedUserNames;
                        }
                    }

                    assignedUserNames = assignedUserNames.TrimEnd(',');

                    TicketLogs TicketLogs = new TicketLogs
                    {
                        ticketid = Convert.ToInt64(ticketcsv),
                        actiontypeid = 7,
                        actiondate = DateTime.Now,
                        actionbyuserId = User.Identity.GetUserId()
                    };
                    //The ticket # 160101 is assigned to [Shaban Sarfraz] by [Bratislav].

                    ApplicationUser userAdmin = UserManager.FindById(Convert.ToString(User.Identity.GetUserId()));
                    TicketLogs.ActionDescription = "The <a href='/tickets/ticketitem/" + Convert.ToString(ticketcsv) +
                                                   "'>ticket #" + Convert.ToString(ticketcsv) +
                                                   "</a> is assigned to [" + assignedUserNames + "] by [" +
                                                   userAdmin.FirstName + " " + userAdmin.LastName + "] on " +
                                                   Convert.ToString(DateTime.Now);
                    db.TicketLogs.Add(TicketLogs);
                    db.SaveChanges();
                    return Json(new
                    {
                        success = true,
                        successtext = "The selected tickets have been assigned to the users successfully.",
                        tickets = Tickets
                    });
                }

                return Json(new { error = true, errortext = ticketResult.ResultMessage });
            }
            catch (Exception ex)
            {
                return Json(new { error = true, errortext = ex.Message });
            }
        }

        public ActionResult ArchivedTicket(string ticketcsv)
        {
            try
            {
                // validate required information.
                if (string.IsNullOrEmpty(ticketcsv))
                {
                    return Json(new { error = true, errortext = "Please select at least one ticket for assignment." });
                }

                // Make sure at least one ticket has been selected.
                string[] Tickets = ticketcsv.Split(',');
                if (Tickets == null)
                {
                    return Json(new { error = true, errortext = "Please select at least one ticket for assignment." });
                }

                TicketAssignmentResult ticketResult = new TicketAssignmentResult
                { ResultType = TicketAssignmentResultType.Error, ResultMessage = "Sorry, no tickets processed." };
                foreach (string ticketitem in Tickets)
                {
                    //Removing Already Exists assigned teams Related to same Ticket
                    long ticketid = Convert.ToInt64(ticketitem);
                    Ticket ticket = db.Ticket.Find(ticketid);
                    ticket.IsArchieved = true;
                    db.Entry(ticket).State = EntityState.Modified;
                    db.SaveChanges();
                }

                return Json(new { success = true, tickets = Tickets });
            }
            catch (Exception ex)
            {
                return Json(new { error = true, errortext = ex.Message });
            }
        }

        //Asign Ticket To User
        public ActionResult AssignUserTickets(string ticketcsv, long projectid, long skillid, string StartDate,
            string EndDate, string usercsv, string comment)
        {
            try
            {
                // validate required information.
                if (string.IsNullOrEmpty(ticketcsv))
                {
                    return Json(new { error = true, errortext = "Please select at least one ticket for assignment." });
                }

                if (projectid < 1)
                {
                    return Json(new { error = true, errortext = "Sorry, project is required." });
                }

                if (skillid < 1)
                {
                    return Json(new { error = true, errortext = "Sorry, skill is required." });
                }

                if (string.IsNullOrEmpty(usercsv))
                {
                    return Json(new { error = true, errortext = "Please assign ticket to at least one user." });
                }

                // Make sure at least one ticket has been selected.
                string[] Tickets = ticketcsv.Split(',');
                if (Tickets == null)
                {
                    return Json(new { error = true, errortext = "Please select at least one ticket for assignment." });
                }

                TicketAssignmentResult ticketResult = new TicketAssignmentResult
                { ResultType = TicketAssignmentResultType.Error, ResultMessage = "Sorry, no tickets processed." };
                foreach (string ticketitem in Tickets)
                {
                    long ticketid = Convert.ToInt64(ticketitem);
                    ticketResult = AssignUserTicketItem(ticketid, projectid, skillid, StartDate, EndDate, usercsv,
                        comment);

                    if (ticketResult.ResultType == TicketAssignmentResultType.Error)
                    {
                        break;
                    }
                }

                // Check latest result, if no error occured it should be success.
                if (ticketResult.ResultType == TicketAssignmentResultType.Success)
                {
                    db.SaveChanges();
                    return Json(new
                    {
                        success = true,
                        successtext = "The selected tickets have been assigned to the users successfully.",
                        tickets = Tickets
                    });
                }

                return Json(new { error = true, errortext = ticketResult.ResultMessage });
            }
            catch (Exception ex)
            {
                return Json(new { error = true, errortext = ex.Message });
            }
        }

        //Fetch Users Which Have to be Assigned Tasks
        public ActionResult FetchAssignedUsers(long? Ticketid)
        {
            TeamDashboardViewModel teamdashboardviewmodel = new TeamDashboardViewModel
            {
                users = new List<SingleUser>()
            };

            //Fethcing the assigned users of same ticketitems
            if (Ticketid != null)
            {
                IQueryable<TicketTeamLogs> ticketteamlogs = db.TicketTeamLogs.Where(ttl => ttl.ticketid == Ticketid);
                //foreach (var ticketteamlog in ticketteamlogs)
                //{
                //    SingleUser singleuser = new SingleUser();
                //    singleuser.FullName = ticketitemlog.user.FullName;
                //    singleuser.ID = ticketitemlog.user.Id;
                //    teamdashboardviewmodel.users.Add(singleuser);
                //}

                TicketItem ticketitem = db.TicketItem.Where(ti => ti.ticketid == Ticketid).FirstOrDefault();
                List<TicketItemLog> tiketitemlogs = db.TicketItemLog.Where(til => til.ticketitemid == ticketitem.id).ToList();
                foreach (TicketItemLog ticketitemlog in tiketitemlogs)
                {
                    SingleUser singleuser = new SingleUser
                    {
                        FullName = ticketitemlog.user.FullName,
                        ID = ticketitemlog.user.Id
                    };
                    teamdashboardviewmodel.users.Add(singleuser);
                }

                return Json(new { error = 0, teamdashboardviewmodel.users }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { error = 1, TextContext = "Sorry! no ticket found with this name" },
                JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult loadCommentByTicketId(long id)
        {
            /****************************************************
            * Get Ticket Comment.
            ****************************************************/
            List<TicketComment> ticketcommentlist = db.TicketComment.Where(x => x.ticketid == id).ToList();
            TicketItemViewModel TicketItemView = new TicketItemViewModel
            {
                TicketComment = ticketcommentlist
            };
            int count = ticketcommentlist.Count();
            return Json(new { commentlist = TicketItemView.TicketComment.OrderByDescending(x => x.id) },
                JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult DeleteCommentById(long id)
        {
            TicketComment commentToDelete = db.TicketComment.Where(x => x.id == id).SingleOrDefault();
            string msg = "Error";
            if (commentToDelete != null)
            {
                db.TicketComment.Remove(commentToDelete);
                db.SaveChanges();
                msg = "Deleted";
            }

            return Json(msg, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AddCommentByTicketId(long ticketId, string comments)
        {
            try
            {
                ApplicationUser CurrentUser = (ApplicationUser)Session[Role.User.ToString()];
                TicketComment ticketComment = new TicketComment
                {
                    ticketid = ticketId,
                    commenton = DateTime.Now,
                    commentbyuserid = User.Identity.GetUserId(),
                    commentbyusername = CurrentUser.FirstName + " " + CurrentUser.LastName,
                    commentbody = comments,
                    createdonutc = DateTime.Now,
                    updatedonutc = DateTime.Now,
                    ipused = Request.UserHostAddress,
                    userid = User.Identity.GetUserId()
                };
                db.TicketComment.Add(ticketComment);
                db.SaveChanges();

                AddNotification(ticketId, 7, ticketComment.id);

                return Json(ticketComment, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { ticketComment = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult EditCommentById(long Id, string comments)
        {
            ApplicationUser CurrentUser = (ApplicationUser)Session[Role.User.ToString()];
            TicketComment commentToEdit = db.TicketComment.Where(x => x.id == Id).SingleOrDefault();
            string msg = "Failed";
            if (commentToEdit != null)
            {
                commentToEdit.commentbody = comments;
                commentToEdit.updatedonutc = DateTime.Now;
                db.SaveChanges();
                msg = "Success";
            }

            return Json(msg, JsonRequestBehavior.AllowGet);
        }

        private TicketAssignmentResult AssignTicketItem(long ticketid, long projectid, long skillid, string StartDate,
            string EndDate, string usercsv, string teamcsv, string comment, int? EstimatedTime)
        {
            try
            {
                // Step 1. Validate if ticket exists.
                Ticket ticket = db.Ticket.Where(i => i.id == ticketid).FirstOrDefault();
                if (ticket == null)
                {
                    return new TicketAssignmentResult
                    {
                        ResultType = TicketAssignmentResultType.Error,
                        ResultMessage = "Sorry, the ticket you are trying to assign was not found."
                    };
                }

                // Step 2. Validate if ticket emails exists.
                db.Entry(ticket).Collection(p => p.TicketItems).Load();
                if (ticket.TicketItems == null)
                {
                    return new TicketAssignmentResult
                    {
                        ResultType = TicketAssignmentResultType.Error,
                        ResultMessage =
                            "Sorry, the ticket you are trying to assign has no emails that can be assigned to selected users."
                    };
                }

                UpdateClientProjectStatus(projectid);
                // Step 3. Validate at least one team Member has been selected to assign.
                string[] teams = teamcsv.Split(',');
                string[] assignedUsers = usercsv.Split(',');

                if (assignedUsers.Length == 0 && teams.Length == 0)
                {
                    return new TicketAssignmentResult
                    {
                        ResultType = TicketAssignmentResultType.Error,
                        ResultMessage = "Sorry, no user has been assigned to the ticket."
                    };
                }
                //Step 4 validate users and teame both are selected
                if (teams.Count() == 1 && teams[0].Equals("") ||
                    assignedUsers.Count() == 1 && assignedUsers[0].Equals(""))
                {
                    return new TicketAssignmentResult
                    {
                        ResultType = TicketAssignmentResultType.Error,
                        ResultMessage = "Sorry, user or teams has not been assigned to the ticket."
                    };
                }

                // Step 4. Find unique team members.
                List<string> teamMembers = new List<string>();
                if (assignedUsers.Length > 0)
                {
                    foreach (string user in assignedUsers)
                    {
                        if (!string.IsNullOrEmpty(user))
                        {
                            teamMembers.Add(user);
                        }
                    }
                }
                //removed on ajax call
                //db.TicketTeamLogs.RemoveRange(db.TicketTeamLogs.Where(x=>x.ticketid == ticket.id).ToList());
                string Teams = string.Empty;

                foreach (string team in teams)
                {
                    if (!string.IsNullOrEmpty(team))
                    {
                        long teamid = Convert.ToInt64(team);
                        TicketTeamLogs teamLog =
                            db.TicketTeamLogs.FirstOrDefault(x => x.teamid == teamid && x.ticketid == ticket.id);
                        if (teamLog == null)
                        {
                            TicketTeamLogs newteamLog = new TicketTeamLogs
                            {
                                displayorder = 1,
                                teamid = teamid,
                                ticketid = ticket.id,
                                assignedbyusersid = User.Identity.GetUserId(),
                                assignedon = DateTime.Now,
                                statusid = 6,
                                statusupdatedbyusersid = User.Identity.GetUserId(),
                                statusupdatedon = DateTime.Now
                            };
                            db.TicketTeamLogs.Add(newteamLog);
                            db.SaveChanges();
                            /*Muhammad Nasir 30-11-2018*/
                            Teams = Teams + db.Team.Find(teamid).name + ",";
                        }
                        else
                        {
                            teamLog.statusupdatedbyusersid = User.Identity.GetUserId();
                            teamLog.statusupdatedon = DateTime.Now;
                            teamLog.assignedon = DateTime.Now;
                            db.SaveChanges();
                        }
                    }
                }

                //if (!string.IsNullOrEmpty(team))
                //{
                //    long teamid = Convert.ToInt64(team);
                //    List<string> memberCollection = db.TeamMember.Where(f => f.teamid == teamid).Select(tm => tm.usersid).ToList();
                //    teamMembers.AddRange(memberCollection);
                //}

                teamMembers = teamMembers.Distinct().ToList();

                // Step 5. Iterate through all the available Ticket Items and assign users to these tickets.
                TicketItem ticketItem = ticket.TicketItems.OrderBy(t => t.id).FirstOrDefault();
                if (ticketItem != null)
                {
                    //Notification variables
                    long ticketidfornotification = ticketItem.ticketid;
                    List<string> usertonotify = new List<string>();
                    // Validate if user is assigned to current ticketitem.
                    db.Entry(ticketItem).Collection(p => p.TicketItemLog).Load();

                    // Initialize TicketItemLog if not available.
                    if (ticketItem.TicketItemLog == null)
                    {
                        ticketItem.TicketItemLog = new List<TicketItemLog>();
                    }

                    // Iterate current email for all the users assigned.
                    foreach (string teamMember in teamMembers)
                    {
                        // If current user is not found.
                        if (ticketItem.TicketItemLog
                                .Where(t => t.ticketitemid == ticketItem.id && t.assignedtousersid == teamMember)
                                .FirstOrDefault() == null)
                        {
                            TicketItemLog newItemLog = new TicketItemLog
                            {
                                displayorder = 1,
                                ticketitemid = ticketItem.id,
                                assignedbyusersid = User.Identity.GetUserId(),
                                assignedtousersid = teamMember,
                                assignedon = DateTime.Now,
                                statusid = 6,
                                statusupdatedbyusersid = User.Identity.GetUserId(),
                                statusupdatedon = DateTime.Now
                            };
                            db.TicketItemLog.Add(newItemLog);
                            db.SaveChanges();
                            ticketidfornotification = newItemLog.TicketItem.ticketid;
                            usertonotify.Add(teamMember);
                        }
                    }

                    // Update TicketItem Skill and Project
                    ticketItem.projectid = projectid;
                    ticketItem.skillid = skillid;
                    if (ticketItem.statusid == 0 || ticketItem.statusid == 1)
                    {
                        ticketItem.statusid = 6;
                    }

                    ticketItem.statusupdatedbyusersid = User.Identity.GetUserId();
                    ticketItem.statusupdatedon = DateTime.Now;
                    ticketItem.updatedonutc = DateTime.Now;
                    ticketItem.ipused = Request.UserHostAddress;
                    db.Entry(ticketItem).State = EntityState.Modified;
                    db.SaveChanges();
                    AddNotification(ticketidfornotification, 6, usertonotify);
                }
                else
                {
                    return new TicketAssignmentResult
                    { ResultType = TicketAssignmentResultType.Error, ResultMessage = "Sorry, no ticket item found." };
                }

                // Step 6. Update Ticket.
                if (!string.IsNullOrEmpty(StartDate) && !string.IsNullOrEmpty(EndDate))
                {
                    ticket.startdate = Convert.ToDateTime(StartDate);
                    ticket.enddate = Convert.ToDateTime(EndDate);
                }

                ticket.projectid = projectid;
                ticket.skillid = skillid;
                if (ticket.statusid == 0 || ticket.statusid == 1)
                {
                    ticket.statusid = 6;
                }

                ticket.statusupdatedbyusersid = User.Identity.GetUserId();
                ticket.statusupdatedon = DateTime.Now;
                ticket.LastActivityDate = DateTime.Now;
                ticket.updatedonutc = DateTime.Now;
                ticket.ipused = Request.UserHostAddress;
                db.Entry(ticket).State = EntityState.Modified;
                db.SaveChanges();
                // Step 7. Add comment.
                if (!string.IsNullOrEmpty(comment))
                {
                    ApplicationUser CurrentUser = (ApplicationUser)Session[Role.User.ToString()];
                    TicketComment ticketComment = new TicketComment
                    {
                        ticketid = ticket.id,
                        commenton = DateTime.Now,
                        commentbyuserid = User.Identity.GetUserId(),
                        commentbyusername = CurrentUser.FirstName + " " + CurrentUser.LastName,
                        commentbody = comment,
                        createdonutc = DateTime.Now,
                        updatedonutc = DateTime.Now,
                        ipused = Request.UserHostAddress,
                        userid = User.Identity.GetUserId()
                    };
                    db.TicketComment.Add(ticketComment);
                }

                db.SaveChanges();

                /*Muhammad Nasir 29-11-2018*/
                Teams = Teams.TrimEnd(',');
                ApplicationUser userAdmin = UserManager.FindById(Convert.ToString(User.Identity.GetUserId()));
                if (Teams.Trim() != string.Empty)
                {
                    TicketLogs TicketLogs = new TicketLogs
                    {
                        ticketid = Convert.ToInt64(ticketid),
                        actiontypeid = 3,
                        actiondate = DateTime.Now,
                        actionbyuserId = User.Identity.GetUserId(),
                        ActionDescription = "Team[s] <b>[" + Teams +
                                            "]</b> is/are assigned <a href='/tickets/ticketitem/" +
                                            Convert.ToString(ticket.id) + "'>ticket #" + Convert.ToString(ticket.id) +
                                            "</a> by " + userAdmin.FirstName + " " + userAdmin.LastName + " on " +
                                            Convert.ToString(DateTime.Now)
                    };
                    db.TicketLogs.Add(TicketLogs);
                    db.SaveChanges();
                }

                // Step 7. Add Estimated Time.
                if (EstimatedTime != null && EstimatedTime > 0)
                {
                    TicketEstimateTimeLog estimateTimeLog = new TicketEstimateTimeLog
                    {
                        ticketid = ticketid,
                        ticketitemcount = ticket.TicketItems.Count,
                        timeestimateinminutes = (int)EstimatedTime,
                        ticketusers = usercsv,
                        createdonutc = DateTime.Now,
                        updatedonutc = DateTime.Now,
                        ipused = Request.UserHostAddress,
                        userid = User.Identity.GetUserId()
                    };
                    db.TicketEstimateTimeLog.Add(estimateTimeLog);
                    db.SaveChanges();
                }

                return new TicketAssignmentResult
                {
                    ResultType = TicketAssignmentResultType.Success,
                    ResultMessage = "The ticket has been assigned successfully to selected users."
                };
            }
            catch (Exception ex)
            {
                return new TicketAssignmentResult
                { ResultType = TicketAssignmentResultType.Error, ResultMessage = ex.Message };
            }
        }

        //Ticket Assigned to Single User
        private TicketAssignmentResult AssignUserTicketItem(long ticketid, long projectid, long skillid,
            string StartDate, string EndDate, string usercsv, string comment)
        {
            try
            {
                // Step 1. Validate if ticket exists.
                Ticket ticket = db.Ticket.Where(i => i.id == ticketid).FirstOrDefault();
                if (ticket == null)
                {
                    return new TicketAssignmentResult
                    {
                        ResultType = TicketAssignmentResultType.Error,
                        ResultMessage = "Sorry, the ticket you are trying to assign was not found."
                    };
                }

                // Step 2. Validate if ticket emails exists.
                db.Entry(ticket).Collection(p => p.TicketItems).Load();
                if (ticket.TicketItems == null)
                {
                    return new TicketAssignmentResult
                    {
                        ResultType = TicketAssignmentResultType.Error,
                        ResultMessage =
                            "Sorry, the ticket you are trying to assign has no emails that can be assigned to selected users."
                    };
                }

                // Step 3. Validate at least one team Member has been selected to assign.
                string[] assignedUsers = usercsv.Split(',');

                if (assignedUsers.Length == 0)
                {
                    return new TicketAssignmentResult
                    {
                        ResultType = TicketAssignmentResultType.Error,
                        ResultMessage = "Sorry, no user has been assigned to the ticket."
                    };
                }
                //Step 4 validate users and teame both are selected
                if (assignedUsers.Count() == 1 && assignedUsers[0].Equals(""))
                {
                    return new TicketAssignmentResult
                    {
                        ResultType = TicketAssignmentResultType.Error,
                        ResultMessage = "Sorry, user or teams has not been assigned to the ticket."
                    };
                }
                // Step 4. Find unique team members.
                List<string> teamMembers = new List<string>();
                if (assignedUsers.Length > 0)
                {
                    foreach (string user in assignedUsers)
                    {
                        if (!string.IsNullOrEmpty(user))
                        {
                            teamMembers.Add(user);
                        }
                    }
                }
                //removed on ajax call
                //db.TicketTeamLogs.RemoveRange(db.TicketTeamLogs.Where(x=>x.ticketid == ticket.id).ToList());
                //foreach (string team in teams)
                //{
                //    if (!string.IsNullOrEmpty(team))
                //    {
                //        var teamid = Convert.ToInt64(team);
                //        var teamLog = db.TicketTeamLogs.FirstOrDefault(x => x.teamid == teamid && x.ticketid == ticket.id);
                //        if (teamLog == null)
                //        {
                //            TicketTeamLogs newteamLog = new TicketTeamLogs();
                //            newteamLog.displayorder = 1;
                //            newteamLog.teamid = teamid;
                //            newteamLog.ticketid = ticket.id;
                //            newteamLog.assignedbyusersid = User.Identity.GetUserId();
                //            newteamLog.assignedon = DateTime.Now;
                //            newteamLog.statusid = 6;
                //            newteamLog.statusupdatedbyusersid = User.Identity.GetUserId();
                //            newteamLog.statusupdatedon = DateTime.Now;
                //            db.TicketTeamLogs.Add(newteamLog);
                //        }
                //        else
                //        {
                //            teamLog.statusupdatedbyusersid = User.Identity.GetUserId();
                //            teamLog.statusupdatedon = DateTime.Now;
                //            teamLog.assignedon = DateTime.Now;
                //        }
                //    }

                //    //if (!string.IsNullOrEmpty(team))
                //    //{
                //    //    long teamid = Convert.ToInt64(team);
                //    //    List<string> memberCollection = db.TeamMember.Where(f => f.teamid == teamid).Select(tm => tm.usersid).ToList();
                //    //    teamMembers.AddRange(memberCollection);
                //    //}
                //}
                teamMembers = teamMembers.Distinct().ToList();

                // Step 5. Iterate through all the available Ticket Items and assign users to these tickets.
                TicketItem ticketItem = ticket.TicketItems.OrderBy(t => t.id).FirstOrDefault();
                if (ticketItem != null)
                {
                    //Notification variables
                    long ticketidfornotification = ticketItem.ticketid;
                    List<string> usertonotify = new List<string>();
                    // Validate if user is assigned to current ticketitem.
                    db.Entry(ticketItem).Collection(p => p.TicketItemLog).Load();

                    // Initialize TicketItemLog if not available.
                    if (ticketItem.TicketItemLog == null)
                    {
                        ticketItem.TicketItemLog = new List<TicketItemLog>();
                    }

                    // Iterate current email for all the users assigned.
                    foreach (string teamMember in teamMembers)
                    {
                        // If current user is not found.
                        if (ticketItem.TicketItemLog
                                .Where(t => t.ticketitemid == ticketItem.id && t.assignedtousersid == teamMember)
                                .FirstOrDefault() == null)
                        {
                            TicketItemLog newItemLog = new TicketItemLog
                            {
                                displayorder = 1,
                                ticketitemid = ticketItem.id,
                                assignedbyusersid = User.Identity.GetUserId(),
                                assignedtousersid = teamMember,
                                assignedon = DateTime.Now,
                                statusid = 6,
                                statusupdatedbyusersid = User.Identity.GetUserId(),
                                statusupdatedon = DateTime.Now
                            };
                            db.TicketItemLog.Add(newItemLog);
                            ticketidfornotification = newItemLog.TicketItem.ticketid;
                            usertonotify.Add(teamMember);
                        }
                    }

                    // Update TicketItem Skill and Project
                    ticketItem.projectid = projectid;
                    ticketItem.skillid = skillid;
                    if (ticketItem.statusid == 0 || ticketItem.statusid == 1)
                    {
                        ticketItem.statusid = 6;
                    }

                    ticketItem.statusupdatedbyusersid = User.Identity.GetUserId();
                    ticketItem.statusupdatedon = DateTime.Now;
                    ticketItem.updatedonutc = DateTime.Now;
                    ticketItem.ipused = Request.UserHostAddress;
                    db.Entry(ticketItem).State = EntityState.Modified;
                    AddNotification(ticketidfornotification, 6, usertonotify);
                }
                else
                {
                    return new TicketAssignmentResult
                    { ResultType = TicketAssignmentResultType.Error, ResultMessage = "Sorry, no ticket item found." };
                }

                // Step 6. Update Ticket.
                if (!string.IsNullOrEmpty(StartDate) && !string.IsNullOrEmpty(EndDate))
                {
                    ticket.startdate = Convert.ToDateTime(StartDate);
                    ticket.enddate = Convert.ToDateTime(EndDate);
                }

                ticket.projectid = projectid;
                ticket.skillid = skillid;
                if (ticket.statusid == 0 || ticket.statusid == 1)
                {
                    ticket.statusid = 6;
                }

                ticket.statusupdatedbyusersid = User.Identity.GetUserId();
                ticket.statusupdatedon = DateTime.Now;
                ticket.LastActivityDate = DateTime.Now;
                ticket.updatedonutc = DateTime.Now;
                ticket.ipused = Request.UserHostAddress;
                db.Entry(ticket).State = EntityState.Modified;

                // Step 7. Add comment.
                if (!string.IsNullOrEmpty(comment))
                {
                    ApplicationUser CurrentUser = (ApplicationUser)Session[Role.User.ToString()];
                    TicketComment ticketComment = new TicketComment
                    {
                        ticketid = ticket.id,
                        commenton = DateTime.Now,
                        commentbyuserid = User.Identity.GetUserId(),
                        commentbyusername = CurrentUser.FirstName + " " + CurrentUser.LastName,
                        commentbody = comment,
                        createdonutc = DateTime.Now,
                        updatedonutc = DateTime.Now,
                        ipused = Request.UserHostAddress,
                        userid = User.Identity.GetUserId()
                    };
                    db.TicketComment.Add(ticketComment);
                }

                db.SaveChanges();
                return new TicketAssignmentResult
                {
                    ResultType = TicketAssignmentResultType.Success,
                    ResultMessage = "The ticket has been assigned successfully to selected users."
                };
            }
            catch (Exception ex)
            {
                return new TicketAssignmentResult
                { ResultType = TicketAssignmentResultType.Error, ResultMessage = ex.Message };
            }
        }

        public ActionResult TicketAssignment(long id, long projectid, long skillid, int status, int quotedtime)
        {
            try
            {
                bool isallassigned = true;
                string userid = User.Identity.GetUserId();
                long tid = id;
                long pid = projectid;
                long sid = skillid;
                int statusid = status;
                Ticket ticket = db.Ticket.Find(tid);
                List<TicketItem> TicketItem = db.TicketItem.Where(t => t.ticketid == tid).ToList();
                foreach (TicketItem item in TicketItem)
                {
                    TicketItemLog tlog = db.TicketItemLog.Where(i => i.ticketitemid == item.id && i.assignedtousersid == userid)
                        .FirstOrDefault();
                    if (tlog == null)
                    {
                        isallassigned = false;
                    }
                }

                if (isallassigned)
                {
                    return Json(new { error = true, errortext = "This ticket already assigned to you." });
                }

                ticket.projectid = projectid;
                ticket.skillid = skillid;
                ticket.statusid = statusid;
                ticket.statusupdatedbyusersid = User.Identity.GetUserId();
                ticket.updatedonutc = DateTime.Now;
                ticket.statusupdatedon = DateTime.Now;
                ticket.LastActivityDate = DateTime.Now;
                ticket.ipused = Request.UserHostAddress;
                ticket.userid = User.Identity.GetUserId();
                db.Entry(ticket).State = EntityState.Modified;
                db.SaveChanges();

                foreach (TicketItem items in TicketItem)
                {
                    TicketItem ticketitem = db.TicketItem.Where(i => i.id == items.id).FirstOrDefault();
                    ticketitem.projectid = pid;
                    ticketitem.skillid = sid;
                    if (status == 4)
                    {
                        ticketitem.statusid = 4;
                    }
                    else if (status == 3)
                    {
                        ticketitem.statusid = 3;
                    }
                    else
                    {
                        ticketitem.statusid = statusid;
                    }

                    ticketitem.statusupdatedbyusersid = User.Identity.GetUserId();
                    ticketitem.updatedonutc = DateTime.Now;
                    ticketitem.statusupdatedon = DateTime.Now;
                    ticketitem.ipused = Request.UserHostAddress;
                    if (items.statusid == 1)
                    {
                        items.quotedtimeinminutes = quotedtime;
                    }

                    ticketitem.userid = User.Identity.GetUserId();
                    db.Entry(ticketitem).State = EntityState.Modified;
                    db.SaveChanges();

                    TicketItemLog ticketitemlog = db.TicketItemLog
                        .Where(i => i.ticketitemid == items.id && i.assignedtousersid == userid).FirstOrDefault();
                    if (ticketitemlog == null)
                    {
                        ticketitemlog = new TicketItemLog();
                        if (status == 4)
                        {
                            ticketitemlog.statusid = 4;
                        }
                        else if (status == 3)
                        {
                            ticketitemlog.statusid = 3;
                        }
                        else
                        {
                            ticketitemlog.statusid = statusid;
                        }

                        long? count = db.TicketItemLog
                            .Where(ti => ti.assignedtousersid == userid && ti.displayorder != null && ti.statusid == 2)
                            .Max(d => d.displayorder);
                        ticketitemlog.displayorder = count != null ? count + 1 : 1;
                        ticketitemlog.ticketitemid = items.id;
                        ticketitemlog.assignedbyusersid = User.Identity.GetUserId();
                        ticketitemlog.assignedtousersid = User.Identity.GetUserId();
                        ticketitemlog.assignedon = DateTime.Now;
                        ticketitemlog.statusupdatedbyusersid = User.Identity.GetUserId();
                        ticketitemlog.statusupdatedon = DateTime.Now;
                        db.TicketItemLog.Add(ticketitemlog);
                        db.SaveChanges();
                    }
                }

                return Json(new { success = true, successtext = "The Ticket Successfully Assigned to you." });
            }
            catch (Exception ex)
            {
                return Json(new { error = true, errortext = ex.Message });
            }
        }

        public ActionResult IsAlreadyAssigned(long id)
        {
            try
            {
                bool isallassigned = true;
                string userid = User.Identity.GetUserId();
                long tid = id;
                Ticket ticket = db.Ticket.Find(tid);
                List<TicketItem> TicketItem = db.TicketItem.Where(t => t.ticketid == tid).ToList();
                foreach (TicketItem item in TicketItem)
                {
                    TicketItemLog tlog = db.TicketItemLog.Where(i => i.ticketitemid == item.id && i.assignedtousersid == userid)
                        .FirstOrDefault();
                    if (tlog == null)
                    {
                        isallassigned = false;
                    }
                }

                if (isallassigned)
                {
                    return Json(new { error = true, errortext = "This ticket already assigned to you." });
                }

                return Json(new { success = true, successtext = "success." });
            }
            catch (Exception ex)
            {
                return Json(new { error = true, errortext = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult AssignTicket(long ticketid, long projectid, long skillid, string users, string shiftteamID)
        {
            try
            {
                // Validate that required information has been provided.
                if (ticketid < 1)
                {
                    return Json(new { error = true, errortext = "Sorry, ticket is requied." });
                }

                if (projectid < 1)
                {
                    return Json(new { error = true, errortext = "Sorry, project is requied." });
                }

                if (skillid < 1)
                {
                    return Json(new { error = true, errortext = "Sorry, skill is requied." });
                }
                // if (string.IsNullOrEmpty(users)) return Json(new { error = true, errortext = "Please select at least one team member for assignment." });

                TicketAssignmentResult result = AssignTicketToUsers(ticketid, projectid, skillid, users, shiftteamID);

                if (result.ResultType == TicketAssignmentResultType.Success)
                {
                    db.SaveChanges();

                    return Json(new
                    {
                        success = true,
                        successtext = "The selected ticket has been assigned to the users successfully.",
                        ticketid = ticketid.ToString()
                    });
                }

                return Json(new { error = true, errortext = result.ResultMessage });
            }
            catch (Exception ex)
            {
                return Json(new { error = true, errortext = ex.Message });
            }
        }

        public ActionResult AssignMultipleTickets(long projectid, long skillid, string ticketcsv, string usercsv,
            string shiftteamID)
        {
            try
            {
                // Validate that required information has been provided.
                if (projectid < 1)
                {
                    return Json(new { error = true, errortext = "Sorry, project is required." });
                }

                if (skillid < 1)
                {
                    return Json(new { error = true, errortext = "Sorry, skill is required." });
                }

                if (string.IsNullOrEmpty(ticketcsv))
                {
                    return Json(new { error = true, errortext = "Please select at least one ticket for assignment." });
                }
                //if (string.IsNullOrEmpty(usercsv)) return Json(new { error = true, errortext = "Please select at least one team member for assignment." });

                // Make sure at least one ticket has been selected.
                string[] Tickets = ticketcsv.Split(',');

                if (Tickets == null)
                {
                    return Json(new { error = true, errortext = "Sorry, looks like no ticket selected to assign." });
                }

                if (usercsv.Equals("") || shiftteamID.Equals(""))
                {
                    return Json(new
                    { error = true, errortext = "Sorry, user and team both must be selected to assign." });
                }

                TicketAssignmentResult ticketResult = new TicketAssignmentResult
                { ResultType = TicketAssignmentResultType.Error, ResultMessage = "Sorry, no tickets processed." };
                foreach (string ticketitem in Tickets)
                {
                    long ticketid = Convert.ToInt64(ticketitem);
                    ticketResult = AssignTicketToUsers(ticketid, projectid, skillid, usercsv, shiftteamID);

                    if (ticketResult.ResultType == TicketAssignmentResultType.Error)
                    {
                        break;
                    }
                }

                // Check latest result, if no error occured it should be success.
                if (ticketResult.ResultType == TicketAssignmentResultType.Success)
                {
                    /*Muhammad Nasir 05-12-2018*/

                    string[] users = usercsv.Split(',');
                    string strusercsv = string.Empty;
                    foreach (string userId in users)
                    {
                        strusercsv = strusercsv + UserManager.FindById(userId).FirstName + " " +
                                     UserManager.FindById(userId).LastName + ",";
                    }

                    strusercsv = strusercsv.Trim(',');

                    string strTickets = string.Empty;
                    foreach (string ticketitem in Tickets)
                    {
                        TicketLogs TicketLogs = new TicketLogs
                        {
                            ticketid = Convert.ToInt64(ticketitem),
                            actiontypeid = 3,
                            actiondate = DateTime.Now,
                            actionbyuserId = User.Identity.GetUserId()
                        };
                        ApplicationUser userAdmin = UserManager.FindById(Convert.ToString(User.Identity.GetUserId()));
                        TicketLogs.ActionDescription = "The <a href='/tickets/ticketitem/" +
                                                       Convert.ToString(ticketitem) + "'>ticket #" +
                                                       Convert.ToString(ticketitem) + "</a> is assigned to <b>[" +
                                                       strusercsv + "]</b> by [" + userAdmin.FirstName + " " +
                                                       userAdmin.LastName + "] on " + Convert.ToString(DateTime.Now);
                        db.TicketLogs.Add(TicketLogs);
                        db.SaveChanges();
                    }

                    db.SaveChanges();
                    return Json(new
                    {
                        success = true,
                        successtext = "The selected tickets have been assigned to the users successfully.",
                        tickets = Tickets
                    });
                }

                return Json(new { error = true, errortext = ticketResult.ResultMessage });
            }
            catch (Exception ex)
            {
                return Json(new { error = true, errortext = ex.Message });
            }
        }

        private TicketAssignmentResult AssignTicketToUsers(long ticketid, long projectid, long skillid, string userscsv,
            string shiftteamID)
        {
            try
            {
                // Step 1. Validate if ticket exists.
                Ticket ticket = db.Ticket.Where(i => i.id == ticketid).FirstOrDefault();
                if (ticket == null)
                {
                    return new TicketAssignmentResult
                    {
                        ResultType = TicketAssignmentResultType.Error,
                        ResultMessage = "Sorry, the ticket you are trying to assign was not found."
                    };
                }

                // Step 2. Validate if ticket emails exists.
                db.Entry(ticket).Collection(p => p.TicketItems).Load();
                if (ticket.TicketItems == null)
                {
                    return new TicketAssignmentResult
                    {
                        ResultType = TicketAssignmentResultType.Error,
                        ResultMessage =
                            "Sorry, the ticket you are trying to assign has no emails that can be assigned to selected users."
                    };
                }

                // Step 3. Validate at least one team Member has been selected to assign.
                string[] teamMembers = userscsv.Split(',');
                if (teamMembers.Length == 0)
                {
                    return new TicketAssignmentResult
                    {
                        ResultType = TicketAssignmentResultType.Error,
                        ResultMessage = "Sorry, no user has been assigned to the ticket."
                    };
                }

                string[] teams = shiftteamID.Split(',');

                if (teamMembers.Length == 0 && teams.Length == 0)
                {
                    return new TicketAssignmentResult
                    {
                        ResultType = TicketAssignmentResultType.Error,
                        ResultMessage = "Sorry, no user has been assigned to the ticket."
                    };
                }
                //Step 4 validate users and teame both are selected
                if (teams.Count() == 1 && teams[0].Equals("") || teamMembers.Count() == 1 && teamMembers[0].Equals(""))
                {
                    return new TicketAssignmentResult
                    {
                        ResultType = TicketAssignmentResultType.Error,
                        ResultMessage = "Sorry, user or teams has not been assigned to the ticket."
                    };
                }
                //removed on ajax call
                //db.TicketTeamLogs.RemoveRange(db.TicketTeamLogs.Where(x=>x.ticketid == ticket.id).ToList());
                foreach (string team in teams)
                {
                    long teamid = Convert.ToInt64(team);
                    TicketTeamLogs teamLog = db.TicketTeamLogs.FirstOrDefault(x => x.teamid == teamid && x.ticketid == ticket.id);
                    if (teamLog == null)
                    {
                        TicketTeamLogs newteamLog = new TicketTeamLogs
                        {
                            displayorder = 1,
                            teamid = teamid,
                            ticketid = ticket.id,
                            assignedbyusersid = User.Identity.GetUserId(),
                            assignedon = DateTime.Now,
                            statusid = 6,
                            statusupdatedbyusersid = User.Identity.GetUserId(),
                            statusupdatedon = DateTime.Now
                        };
                        db.TicketTeamLogs.Add(newteamLog);
                    }
                    else
                    {
                        teamLog.statusupdatedbyusersid = User.Identity.GetUserId();
                        teamLog.statusupdatedon = DateTime.Now;
                        teamLog.assignedon = DateTime.Now;
                    }
                }

                //// Passing shift Id from front end and getting all userid against the shift
                //string[] shift_teams = new string[] { };
                //string userString = string.Empty;
                //if (shiftteamID.Length != 0)
                //{
                //    string[] shiftTeam_Array = shiftteamID.Split(',');
                //    foreach (string shiftTeam in shiftTeam_Array)
                //    {
                //        var teamid = int.Parse(shiftTeam);
                //        var shiftTeam_List = db.TeamMember.Where(f => f.teamid == teamid).ToList();
                //        foreach (TeamMember shiftteammember in shiftTeam_List)
                //        {
                //            userString = string.Concat(userString, shiftteammember.usersid) + ',';
                //        }
                //    }
                //    shift_teams = userString.Remove(userString.Length - 1).Split(',');
                //}

                // merging two array to make one array and find out the distinct ones to asssign the task
                //var z = new string[teamMembers.Length + shift_teams.Length];
                //teamMembers.CopyTo(z, 0);
                //shift_teams.CopyTo(z, teamMembers.Length);
                string[] distinct_teammember = teamMembers.Distinct().ToArray();

                // Step 4. Iterate through all the available Ticket Items and assign users to these tickets.
                foreach (TicketItem ticketItem in ticket.TicketItems)
                {
                    //Notification variables
                    long ticketidfornotification = ticketItem.ticketid;
                    List<string> usertonotify = new List<string>();
                    // Make sure ticket item status is 1 or 2.
                    if (ticketItem.statusid < 3 || ticketItem.statusid == 6)
                    {
                        // Validate if user is assigned to current ticketitem.
                        db.Entry(ticketItem).Collection(p => p.TicketItemLog).Load();

                        // Initialize TicketItemLog if not available.
                        if (ticketItem.TicketItemLog == null)
                        {
                            ticketItem.TicketItemLog = new List<TicketItemLog>();
                        }

                        // Iterate current email for all the users assigned.
                        foreach (string teamMember in distinct_teammember)
                        {
                            if (!string.IsNullOrEmpty(teamMember.Trim()))
                            {
                                // If current user is not found.
                                if (ticketItem.TicketItemLog.Where(t =>
                                            t.ticketitemid == ticketItem.id && t.assignedtousersid == teamMember)
                                        .FirstOrDefault() == null)
                                {
                                    long? count = db.TicketItemLog.Where(ti =>
                                        ti.assignedtousersid == teamMember && ti.displayorder != null &&
                                        ti.statusid == 2).Max(d => d.displayorder);
                                    TicketItemLog newItemLog = new TicketItemLog
                                    {
                                        displayorder = count != null ? count + 1 : 1,
                                        ticketitemid = ticketItem.id,
                                        assignedbyusersid = User.Identity.GetUserId(),
                                        assignedtousersid = teamMember,
                                        assignedon = DateTime.Now,
                                        statusid = 6,
                                        statusupdatedbyusersid = User.Identity.GetUserId(),
                                        statusupdatedon = DateTime.Now
                                    };
                                    db.TicketItemLog.Add(newItemLog);
                                    ticketidfornotification = newItemLog.TicketItem.ticketid;
                                    usertonotify.Add(teamMember);
                                }
                            }
                        }

                        // Update TicketItem Skill and Project
                        if (ticketItem.statusid == 1 || ticketItem.projectid == null || ticketItem.skillid == null)
                        {
                            ticketItem.projectid = projectid;
                            ticketItem.skillid = skillid;
                        }

                        ticketItem.statusid = 6;
                        ticketItem.statusupdatedbyusersid = User.Identity.GetUserId();
                        ticketItem.statusupdatedon = DateTime.Now;
                        ticketItem.updatedonutc = DateTime.Now;
                        ticketItem.ipused = Request.UserHostAddress;
                        db.Entry(ticketItem).State = EntityState.Modified;
                        db.SaveChanges();
                        if (usertonotify.Count() > 0)
                        {
                            AddNotification(ticketidfornotification, 6, usertonotify);
                        }
                    }
                }

                // Update Ticket.
                ticket.projectid = projectid;
                ticket.skillid = skillid;
                ticket.statusid = 6;
                ticket.statusupdatedbyusersid = User.Identity.GetUserId();
                ticket.statusupdatedon = DateTime.Now;
                ticket.LastActivityDate = DateTime.Now;
                ticket.updatedonutc = DateTime.Now;
                ticket.ipused = Request.UserHostAddress;
                db.Entry(ticket).State = EntityState.Modified;
                db.SaveChanges();
                return new TicketAssignmentResult
                {
                    ResultType = TicketAssignmentResultType.Success,
                    ResultMessage = "The ticket has been assigned successfully to selected users."
                };
            }
            catch (Exception ex)
            {
                return new TicketAssignmentResult
                { ResultType = TicketAssignmentResultType.Error, ResultMessage = ex.Message };
            }
        }

        #endregion Inbox

        #region MyTickets

        public ActionResult MyTickets(long id, long? clientid = 0)
        {
            // If no id is provided.
            if (id < 0)
            {
                return RedirectToAction("index", "home");
            }

            /****************************************************
             * If user is logged in, get user's guid.
             ****************************************************/
            string userid = User.Identity.GetUserId();

            /****************************************************
             * Fetch all statuses with count of tickets.
             ****************************************************/
            List<TicketStatusViewModel> ticketStatusCounts = db.Database
                .SqlQuery<TicketStatusViewModel>("exec TicketStatus_GetTicketCountByUser @userid",
                    new SqlParameter("@userid", userid)).ToList();

            List<TicketStatusViewModel> TicketUserStatus = new List<TicketStatusViewModel>();
            TicketUserStatus.Insert(0, new TicketStatusViewModel
            {
                id = 0,
                isactive = true,
                name = "All Tickets",
                ticketcount = TicketUserStatus.Sum(x => x.ticketcount)
            });

            TicketUserStatus.Insert(1, ticketStatusCounts.SingleOrDefault(x => x.name.Equals("Assigned")));
            TicketUserStatus.Insert(2, ticketStatusCounts.SingleOrDefault(x => x.name.Equals("In Progress")));
            TicketUserStatus.Insert(3, ticketStatusCounts.SingleOrDefault(x => x.name.Equals("On Hold")));
            TicketUserStatus.Insert(4, ticketStatusCounts.SingleOrDefault(x => x.name.Equals("QC")));
            TicketUserStatus.Insert(5, ticketStatusCounts.SingleOrDefault(x => x.name.Equals("In Review")));
            TicketUserStatus.Insert(6, ticketStatusCounts.SingleOrDefault(x => x.name.Equals("Done")));

            /****************************************************
             * Fetch all clients even assigned to the user.
             ****************************************************/
            List<UserClientViewModel> TicketUserClients = db.Database
                .SqlQuery<UserClientViewModel>("exec TicketClient_GetTicketCountByUser @userid",
                    new SqlParameter("@userid", userid)).ToList();

            TicketUserClients.Insert(0, new UserClientViewModel
            {
                clientid = 0,
                clientname = "All Clients",
                userfavouriteid = -1,
                ispinned = false
            });

            /****************************************************
             * Fetch Tickets by status, client and/or user.
             ****************************************************/
            IQueryable<Ticket> ticketsQuery = null;

            //IQueryable<Project> projectQuery = null;

            if (id == 0 && clientid == 0)
            {
                ticketsQuery = (from tickets in db.Ticket
                                join items in db.TicketItem on tickets.id equals items.ticketid
                                join logs in db.TicketItemLog on items.id equals logs.ticketitemid
                                where logs.assignedtousersid == userid
                                select tickets).Distinct().OrderByDescending(t => t.lastmodifiedtime).Take(recordsPerPage);
            }

            //projectQuery = (from tickets in db.Ticket
            //                join items in db.TicketItem on tickets.id equals items.ticketid
            //                join logs in db.TicketItemLog on items.id equals logs.ticketitemid
            //                join proj in db.Project on items.projectid equals proj.id
            //                where logs.assignedtousersid == userid && proj.clientid == clientid && tickets.statusid == id
            //                select proj).Where(p => p.iswarning == true);
            else if (id == 0 && clientid > 0)
            {
                ticketsQuery = (from tickets in db.Ticket
                                join items in db.TicketItem on tickets.id equals items.ticketid
                                join logs in db.TicketItemLog on items.id equals logs.ticketitemid
                                join proj in db.Project on items.projectid equals proj.id
                                where logs.assignedtousersid == userid && proj.clientid == clientid
                                select tickets).Distinct().OrderByDescending(t => t.lastmodifiedtime).Take(recordsPerPage);
            }

            //projectQuery = (from tickets in db.Ticket
            //                     join items in db.TicketItem on tickets.id equals items.ticketid
            //                     join logs in db.TicketItemLog on items.id equals logs.ticketitemid
            //                     join proj in db.Project on items.projectid equals proj.id
            //                     where logs.assignedtousersid == userid && proj.clientid == clientid && tickets.statusid == id
            //                     select proj).Where(p => p.iswarning == true);
            else if (id > 0 && clientid == 0)
            {
                ticketsQuery = (from tickets in db.Ticket
                                join items in db.TicketItem on tickets.id equals items.ticketid
                                join logs in db.TicketItemLog on items.id equals logs.ticketitemid
                                where logs.assignedtousersid == userid && tickets.statusid == id
                                select tickets).Distinct().OrderByDescending(t => t.lastmodifiedtime).Take(recordsPerPage);
            }

            //projectQuery = (from tickets in db.Ticket
            //                     join items in db.TicketItem on tickets.id equals items.ticketid
            //                     join logs in db.TicketItemLog on items.id equals logs.ticketitemid
            //                     join proj in db.Project on items.projectid equals proj.id
            //                     where logs.assignedtousersid == userid && proj.clientid == clientid && tickets.statusid == id
            //                     select proj).Where(p => p.iswarning == true);
            else if (id > 0 && clientid > 0)
            {
                ticketsQuery = (from tickets in db.Ticket
                                join items in db.TicketItem on tickets.id equals items.ticketid
                                join logs in db.TicketItemLog on items.id equals logs.ticketitemid
                                join proj in db.Project on items.projectid equals proj.id
                                where logs.assignedtousersid == userid && proj.clientid == clientid && tickets.statusid == id
                                select tickets).Distinct().OrderByDescending(t => t.lastmodifiedtime).Take(recordsPerPage);
            }

            //projectQuery = (from tickets in db.Ticket
            //                     join items in db.TicketItem on tickets.id equals items.ticketid
            //                     join logs in db.TicketItemLog on items.id equals logs.ticketitemid
            //                     join proj in db.Project on items.projectid equals proj.id
            //                     where logs.assignedtousersid == userid && proj.clientid == clientid && tickets.statusid == id
            //                     select proj).Where(p => p.iswarning == true);

            /****************************************************
             * Prepare MyTickets View Model.
             ****************************************************/
            MyTicketsViewModel myTicketsVM = new MyTicketsViewModel
            {
                statusidparam = id,
                clientidparam = clientid,
                TicketUserStatusCollection = TicketUserStatus,
                TicketUserClientCollection = TicketUserClients,
                MyTicketCollection = ticketsQuery.ToList()
                //ProjectDataCollection = projectQuery.ToList()
            };

            return View(myTicketsVM);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult MyTickets(long id, long? clientid = 0, int? pagenum = 0, string topic = "")
        {
            /****************************************************
            * Get Paginated Tickets.
            ****************************************************/
            int pagenumber = pagenum ?? 0;
            long clientidval = clientid ?? 0;

            if (Request.IsAjaxRequest())
            {
                string userid = User.Identity.GetUserId();
                List<Ticket> mytickets = FetchPaginatedMyTickets(id, clientidval, pagenumber, topic);

                ViewBag.statusid = id;
                ViewBag.pagenum = pagenumber;
                string ticketitems = PartialView("_MyTicketItem", mytickets).RenderToString();
                return Json(new { ticketitems, totalcount = mytickets.Count, itemcount = mytickets.Count },
                    JsonRequestBehavior.AllowGet);
            }

            return Json(new { ticketitems = "", totalcount = 0, itemcount = 0 }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Flagged()
        {
            List<TicketStatusViewModel> ticketstatuses = db.Database.SqlQuery<TicketStatusViewModel>("exec ticketstatus_loadticketscount")
                .ToList();
            List<TicketStatusViewModel> ticketstatusesCountAll =
                db.Database.SqlQuery<TicketStatusViewModel>("exec ticketstatus_loadticketscountAll").ToList();
            int totaltask = 0;
            foreach (TicketStatusViewModel status in ticketstatusesCountAll)
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
                ticketstatuses.SingleOrDefault(x => x.name.Equals("In Review")),
                ticketstatuses.SingleOrDefault(x => x.name.Equals("QC")),
                ticketstatuses.SingleOrDefault(x => x.name.Equals("Done")),
                ticketstatuses.SingleOrDefault(x => x.name.Equals("Trash")),
                ticketstatuses.SingleOrDefault(x => x.name.Equals("IsArchive"))
            };
            ViewBag.conversationstatus = newticketstatusesOrder;
            List<Ticket> tickets = new List<Ticket>();
            List<TicketItem> ticketitems = new List<TicketItem>();
            TicketViewModel ticketvm = new TicketViewModel();
            string userid = User.Identity.GetUserId();
            IQueryable<TicketUserFlagged> flaggedTickets = db.TicketUserFlagged.Where(u => u.userid == userid && u.isactive == true).ToList().AsQueryable();
            if (flaggedTickets.Count() > 0)
            {
                foreach (TicketUserFlagged item in flaggedTickets)
                {
                    Ticket ticket = db.Ticket.Where(t => t.id == item.ticketid).FirstOrDefault();
                    tickets.Add(ticket);
                }
            }

            foreach (Ticket item in tickets)
            {
                TicketItem ticketitem = db.TicketItem.Where(t => t.ticketid == item.id).OrderByDescending(t => t.createdonutc)
                    .FirstOrDefault();
                ticketitems.Add(ticketitem);
            }

            IQueryable<Team> ActiveTeams = db.Team.OrderBy(f => f.name).Where(f => f.isactive == true).ToList().AsQueryable();

            //ViewBag.status = db.ConversationStatus.Where(i => i.id != 2);
            List<SelectListItem> projectlist = db.Database.SqlQuery<CombinedEntity>("exec projects_loadcombined").Select(x =>
                new SelectListItem
                {
                    Text = x.name,
                    Value = x.id.ToString()
                }).ToList();
            List<SelectListItem> ClientSelectList = new SelectList(db.Client.Where(x => x.isactive).ToList(), "id", "name").ToList();
            ViewBag.clients = ClientSelectList;
            ViewBag.projects = new SelectList(projectlist, "Value", "Text");
            ViewBag.skills = db.Skill.ToList().AsQueryable();
            //ViewBag.tickettype = 0;
            //ViewBag.counter = db.Ticket.Include(t => t.FlagStatus).Include(t => t.StatusUpdatedByUser).Include(t => t.ConversationStatus).Where(t => t.statusid == id).OrderByDescending(t => t.lastdeliverytime).Where(i => i.statusid == id).Count();
            ViewBag.users = UserManager.Users.OrderBy(f => f.FirstName).Where(u => u.isactive == true).ToList().AsQueryable();
            ViewBag.teams = ActiveTeams;

            ticketvm.tickets = tickets.AsQueryable();
            ticketvm.flaggeditems = flaggedTickets;
            ticketvm.ticketitems = ticketitems;
            ticketvm.teams = ActiveTeams;
            return View(ticketvm);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AddFavouriteClient(long id)
        {
            if (id > 0)
            {
                UserFavourite userFavourite = new UserFavourite
                {
                    userfavouritetypeid = 1,
                    userfavouriteid = id,
                    userid = User.Identity.GetUserId()
                };

                db.UserFavourite.Add(userFavourite);
                db.SaveChanges();
                return Json(new { success = 1 }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { success = 0 }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult RemoveFavouriteClient(long id)
        {
            if (id > 0)
            {
                UserFavourite favClient = db.UserFavourite.SingleOrDefault(x => x.id == id);

                if (favClient != null)
                {
                    db.UserFavourite.Remove(favClient);
                    db.SaveChanges();
                    return Json(new { success = 1 }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { success = 0 }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { success = 0 }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ChnageTicketStatus(string id, string status, bool isExternal = false)
        {
            /*Muhammad Nasir 12-06-2018*/
            TicketStatus Oldticketstaus = db.TicketStatus.Find(db.Ticket.Find(Convert.ToInt64(id)).statusid);
            try
            {
                if (Oldticketstaus.id ==Convert.ToInt32("8") && isExternal== true)
                {
                    TempData["DeleteMessage"] = "Already Deleted";
                    return RedirectToAction("GetAllOrphanedTickets", "Orphan");
                    //return RedirectToAction("GetAllOrphanedTickets", "Orphan", new { deleted = "warning" });
                }
                long tid = Convert.ToInt64(id);
                int statusid = Convert.ToInt32(status);
                Ticket ticket = db.Ticket.Find(tid);
                if (ticket.statusid == 2 && statusid == 2)
                {
                    return Json(new { error = true, errortext = "Sorry this task already in progress." });
                }

                ticket.statusid = statusid;
                ticket.statusupdatedbyusersid = User.Identity.GetUserId();
                ticket.updatedonutc = DateTime.Now;
                ticket.statusupdatedon = DateTime.Now;
                ticket.LastActivityDate = DateTime.Now;
                ticket.ipused = Request.UserHostAddress;
                ticket.userid = User.Identity.GetUserId();
                ticket.LastActivityDate = DateTime.Now;
                db.Entry(ticket).State = EntityState.Modified;
                db.SaveChanges();

                List<TicketItem> TicketItem = db.TicketItem.Where(t => t.ticketid == ticket.id).ToList();
                if (TicketItem.Count > 0 && TicketItem != null)
                {
                    foreach (TicketItem items in TicketItem)
                    {
                        if (items.statusid == 1 || items.statusid == 2)
                        {
                            TicketItem ticketitem = db.TicketItem.Where(i => i.id == items.id).FirstOrDefault();
                            if (statusid == 4)
                            {
                                ticketitem.statusid = 4;
                            }
                            else if (statusid == 3)
                            {
                                ticketitem.statusid = 3;
                            }
                            else
                            {
                                ticketitem.statusid = statusid;
                            }

                            ticketitem.statusupdatedbyusersid = User.Identity.GetUserId();
                            ticketitem.updatedonutc = DateTime.Now;
                            ticketitem.statusupdatedon = DateTime.Now;
                            ticketitem.ipused = Request.UserHostAddress;
                            ticketitem.userid = User.Identity.GetUserId();
                            db.Entry(ticketitem).State = EntityState.Modified;
                            db.SaveChanges();
                        }

                        List<TicketItemLog> ticketitemlog = db.TicketItemLog.Where(t => t.ticketitemid == items.id).ToList();
                        if (ticketitemlog.Count > 0 && ticketitemlog != null)
                        {
                            foreach (TicketItemLog logs in ticketitemlog)
                            {
                                if (logs.statusid == 1 || logs.statusid == 2)
                                {
                                    if (statusid == 4)
                                    {
                                        logs.statusid = 4;
                                    }
                                    else if (statusid == 3)
                                    {
                                        logs.statusid = 3;
                                    }
                                    else
                                    {
                                        logs.statusid = statusid;
                                    }

                                    logs.ticketitemid = items.id;
                                    logs.statusupdatedbyusersid = User.Identity.GetUserId();
                                    logs.statusupdatedon = DateTime.Now;
                                    db.Entry(logs).State = EntityState.Modified;
                                    db.SaveChanges();
                                }
                            }
                        }
                    }
                }

                /*By Muhammad Nasir on 28-11-2018*/
                TicketLogs TicketLogs = new TicketLogs
                {
                    ticketid = Convert.ToInt64(id),
                    actiontypeid = 9,
                    actiondate = DateTime.Now,
                    actionbyuserId = User.Identity.GetUserId()
                };
                //The ticket # 160101 is assigned to [Shaban Sarfraz] by [Bratislav].

                TicketStatus Newticketstatus = db.TicketStatus.Find(Convert.ToInt64(status));
                ApplicationUser userAdmin = UserManager.FindById(Convert.ToString(User.Identity.GetUserId()));

                TicketLogs.ActionDescription = "The <a href='/tickets/ticketitem/" + Convert.ToString(id) +
                                               "'>ticket #" + Convert.ToString(id) +
                                               "</a> status is changed from <b>[" + Oldticketstaus.name +
                                               "]</b> to <b>[" + Newticketstatus.name + "]</b> by [" +
                                               userAdmin.FirstName + " " + userAdmin.LastName + "] on " +
                                               Convert.ToString(DateTime.Now);
                ;
                db.TicketLogs.Add(TicketLogs);
                db.SaveChanges();
                AddNotification(tid, 1);
                if (isExternal == false)
                {
                    return Json(new { success = true, successtext = "The ticket status updated." });

                }
                else
                {
                    TempData["DeleteMessage"] = "Deleted succesfully";
                    return RedirectToAction("GetAllOrphanedTickets", "Orphan");
                    //return RedirectToAction("GetAllOrphanedTickets", "Orphan", new { deleted = "success" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { error = true, errortext = ex.Message });
            }
        }

        public JsonResult t()
        {
            return Json(new { ticketitems = new List<int> { 1, 2 }, statusid = 5 }, JsonRequestBehavior.AllowGet);
        }

        // Close multiple task
        public ActionResult Closemultiplestatus(int statustype, int statusid, List<long> ticketitems)
        {
            if (ticketitems == null || ticketitems.Count == 0)
            {
                return Json(new { error = true, errortext = "Sorry at least one task must be selected" });
            }

            if (statustype == 0)
            {
                return Json(new { error = true, errortext = "Status type is required" });
            }

            if (statustype == 1)
            {
                bool status = false;
                foreach (long itemid in ticketitems)
                {
                    status = CloseMultipleTicket(statusid, itemid);
                }

                if (status)
                {
                    return Json(new
                    { success = true, successtext = "Ticket:The selected task statuses has been updated." });
                }

                return Json(new { error = true, errortext = "The selected task statuses could not updated." });
            }

            if (statustype == 2)
            {
                bool isdone = false;
                foreach (long itemid in ticketitems)
                {
                    isdone = ChangeTicketItemStatus(statusid, itemid);
                }

                if (isdone)
                {
                    return Json(
                        new { success = true, successtext = "Email:The selected task statuses has been updated." });
                }

                return Json(new { error = true, errortext = "Sorry The selected task statuses could not updated" });
            }

            if (statustype == 3)
            {
                bool isdone = false;
                foreach (long itemid in ticketitems)
                {
                    isdone = ChangeUserTaskStatus(statusid, itemid);
                }

                if (isdone)
                {
                    return Json(new { success = true, successtext = "The selected task statuses has been updated" });
                }

                return Json(new { error = true, errortext = "Sorry The selected task statuses could not been updated" });
            }

            return Json(new { });
        }

        private bool CloseMultipleTicket(int statusid, long itemid)
        {
            Ticket ticket = db.Ticket.Where(i => i.id == itemid).FirstOrDefault();
            TicketStatus Oldticketstaus = db.TicketStatus.Find(ticket.statusid);
            ticket.statusid = statusid;
            ticket.statusupdatedbyusersid = User.Identity.GetUserId();
            ticket.updatedonutc = DateTime.Now;
            ticket.statusupdatedon = DateTime.Now;
            ticket.LastActivityDate = DateTime.Now;
            ticket.ipused = Request.UserHostAddress;
            ticket.userid = User.Identity.GetUserId();
            db.Entry(ticket).State = EntityState.Modified;
            db.SaveChanges();
            List<TicketItem> TicketItem = db.TicketItem.Where(t => t.ticketid == ticket.id).ToList();
            if (TicketItem.Count > 0 && TicketItem != null)
            {
                foreach (TicketItem items in TicketItem)
                {
                    if (items.statusid == 1 || items.statusid == 2)
                    {
                        TicketItem ticketitem = db.TicketItem.Where(i => i.id == items.id).FirstOrDefault();
                        if (ticketitem.statusid < 3)
                        {
                            ticketitem.statusid = statusid;
                            ticketitem.statusupdatedbyusersid = User.Identity.GetUserId();
                            ticketitem.updatedonutc = DateTime.Now;
                            ticketitem.statusupdatedon = DateTime.Now;
                            ticketitem.ipused = Request.UserHostAddress;
                            ticketitem.userid = User.Identity.GetUserId();
                            db.Entry(ticketitem).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }

                    List<TicketItemLog> ticketitemlog = db.TicketItemLog.Where(t => t.ticketitemid == items.id).ToList();
                    if (ticketitemlog.Count > 0 && ticketitemlog != null)
                    {
                        foreach (TicketItemLog logs in ticketitemlog)
                        {
                            if (logs.statusid < 3)
                            {
                                logs.statusid = statusid;
                                logs.ticketitemid = items.id;
                                logs.statusupdatedbyusersid = User.Identity.GetUserId();
                                logs.statusupdatedon = DateTime.Now;
                                db.Entry(logs).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                        }
                    }
                }
            }

            TicketLogs TicketLogs = new TicketLogs
            {
                ticketid = itemid,
                actiontypeid = 9,
                actiondate = DateTime.Now,
                actionbyuserId = User.Identity.GetUserId()
            };
            TicketStatus Newticketstatus = db.TicketStatus.Find(statusid);
            ApplicationUser userAdmin = UserManager.FindById(Convert.ToString(User.Identity.GetUserId()));
            TicketLogs.ActionDescription = "The <a href='/tickets/ticketitem/" + itemid + "'>ticket #" + itemid +
                                           "</a> status is changed from <b>[" + Oldticketstaus.name + "]</b> to <b>[" +
                                           Newticketstatus.name + "]</b> by [" + userAdmin.FirstName + " " +
                                           userAdmin.LastName + "] on " + Convert.ToString(DateTime.Now);
            ;
            db.TicketLogs.Add(TicketLogs);
            db.SaveChanges();

            return true;
        }

        private bool ChangeTicketItemStatus(int statusid, long emailid)
        {
            try
            {
                TicketItem TicketItem = db.TicketItem.Where(t => t.id == emailid).FirstOrDefault();

                Ticket ticket = db.Ticket.Where(i => i.id == TicketItem.ticketid).FirstOrDefault();
                TicketStatus Oldticketstaus = db.TicketStatus.Find(ticket.statusid);
                if (ticket.statusid == 1)
                {
                    ticket.statusid = 2;
                    ticket.statusupdatedbyusersid = User.Identity.GetUserId();
                    ticket.statusupdatedon = DateTime.Now;
                    ticket.LastActivityDate = DateTime.Now;
                    db.Entry(ticket).State = EntityState.Modified;
                    db.SaveChanges();

                    TicketLogs TicketLogs = new TicketLogs
                    {
                        ticketid = ticket.id,
                        actiontypeid = 9,
                        actiondate = DateTime.Now,
                        actionbyuserId = User.Identity.GetUserId()
                    };
                    TicketStatus Newticketstatus = db.TicketStatus.Find(statusid);
                    ApplicationUser userAdmin = UserManager.FindById(Convert.ToString(User.Identity.GetUserId()));
                    TicketLogs.ActionDescription = "The <a href='/tickets/ticketitem/" + ticket.id + "'>ticket #" +
                                                   ticket.id + "</a> status is changed from <b>[" +
                                                   Oldticketstaus.name + "]</b> to <b>[" + Newticketstatus.name +
                                                   "]</b> by [" + userAdmin.FirstName + " " + userAdmin.LastName +
                                                   "] on " + Convert.ToString(DateTime.Now);
                    ;
                    db.TicketLogs.Add(TicketLogs);
                    db.SaveChanges();
                }

                TicketItem.statusid = statusid;
                TicketItem.statusupdatedbyusersid = User.Identity.GetUserId();
                TicketItem.updatedonutc = DateTime.Now;
                TicketItem.statusupdatedon = DateTime.Now;
                TicketItem.ipused = Request.UserHostAddress;
                TicketItem.userid = User.Identity.GetUserId();
                db.Entry(TicketItem).State = EntityState.Modified;
                db.SaveChanges();

                List<TicketItemLog> ticketitemlog = db.TicketItemLog.Where(t => t.ticketitemid == emailid).ToList();
                if (ticketitemlog.Count > 0 && ticketitemlog != null)
                {
                    foreach (TicketItemLog items in ticketitemlog)
                    {
                        if (items.statusid < 3)
                        {
                            TicketItemLog ticketitem = db.TicketItemLog.Where(t => t.id == items.id).FirstOrDefault();
                            ticketitem.statusid = statusid;
                            ticketitem.assignedbyusersid = User.Identity.GetUserId();
                            ticketitem.statusupdatedbyusersid = User.Identity.GetUserId();
                            ticketitem.statusupdatedon = DateTime.Now;
                            db.Entry((object)ticketitem).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private bool ChangeUserTaskStatus(int statusid, long ticketitemid)
        {
            try
            {
                TicketItem ticketItem = db.TicketItem.Find(ticketitemid);
                if (ticketItem == null)
                {
                    return false;
                }

                // Fetch ticket assigned to the current user.
                string currentUser = User.Identity.GetUserId();
                TicketItemLog ticketItemLog = db.TicketItemLog
                    .Where(til => til.ticketitemid == ticketitemid && til.assignedtousersid == currentUser)
                    .FirstOrDefault();

                if (ticketItemLog == null)
                {
                    return false;
                }

                ticketItemLog.statusid = statusid;
                ticketItemLog.assignedon = DateTime.Now;
                ticketItemLog.statusupdatedbyusersid = User.Identity.GetUserId();
                ticketItemLog.statusupdatedon = DateTime.Now;
                db.Entry(ticketItemLog).State = EntityState.Modified;
                db.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion MyTickets

        #region TicketItems

        [HttpGet]
        public ActionResult TicketAllocation(long id)
        {
            // If no id is provided.
            if (id < 0)
            {
                return RedirectToAction("index", "home");
            }

            /****************************************************
            * Fetch users assigned to the ticket.
            ****************************************************/
            var ticketusers = (from ticketItem in db.TicketItem.Where(x => x.ticketid == id)
                               join project in db.Project on ticketItem.projectid equals project.id
                               join skill in db.Skill on ticketItem.skillid equals skill.id
                               select new
                               {
                                   ticketItemID = ticketItem.id,
                                   projectid = project.id,
                                   projectname = project.name,
                                   Warningtext = project.warningtext,
                                   WarningStatus = project.iswarning,
                                   skillid = skill.id,
                                   skillname = skill.name
                               }).FirstOrDefault();

            return Json(ticketusers, JsonRequestBehavior.AllowGet);
        }

        public JsonResult TicketItemAssigned(long id)
        {
            // If user is not logged in,
            //if (!IsLoggedIn()) return RedirectToAction("login", "account");

            //// If no id is provided.
            //if (id < 0) return RedirectToAction("index", "home");

            List<TicketItemLog> ticketusers = (from items in db.TicketItem
                                               join logs in db.TicketItemLog on items.id equals logs.ticketitemid
                                               join users in db.Users on logs.assignedtousersid equals users.Id
                                               where items.ticketid == id
                                               select logs
                ).DistinctBy(l => l.assignedtousersid).ToList();

            List<TicketTeamLogs> ticketTeam = (from teamlog in db.TicketTeamLogs
                                               join team in db.Team on teamlog.teamid equals team.id
                                               where teamlog.ticketid == id
                                               select teamlog).DistinctBy(x => x.teamid).ToList();

            return Json(new { ticketusers, ticketTeam }, JsonRequestBehavior.AllowGet);
        }

        #region Ticket Item Pages

        public ActionResult TicketItem(long? id)
        {
            string userId = User.Identity.GetUserId();
            ViewBag.orphan = db.SuppressTickets.Where(x => x.TicketId == id && x.UsersId == userId)
                .Select(x => x.UsersId).FirstOrDefault();
            ViewBag.subscribeTeam = db.SubscribeTeams.Where(x => x.UsersId == userId).Select(x => x.UsersId)
                .FirstOrDefault();
            if (id == null)
            {
                return RedirectToAction("Index", "Tickets", new { id = 1 });
            }
            // If no id is provided.
            if (id.Value < 0)
            {
                return RedirectToAction("index", "home");
            }

            /****************************************************
            * Fetch EmailReplySignature of loged in user
            ****************************************************/
            string LoginUserId = User.Identity.GetUserId();
            ApplicationUser LoginUser = db.Users.Find(LoginUserId);
            ViewBag.EmailSignature = LoginUser.EmailReplySignature;

            /****************************************************
            * Fetch Ticket & TicketItems
            ****************************************************/
            ViewBag.id = id.Value;
            string firstitem = null;
            //var pageSize = 10; // set your page size, which is number of records per page
            //int pagenum = 1;
            //var skip = pageSize * (pagenum - 1);

            Ticket ticket = db.Ticket.Where(t => t.id == id.Value).Include(t => t.TicketItems)
                .Include(t => t.TicketComment).Include(t => t.TicketItems.Select(ti => ti.TicketItemLog))
                .Include(t => t.TicketItems.Select(ti => ti.TicketItemAttachment)).SingleOrDefault();
            //db.tic
            //if (ticket != null)
            //{
            //    var ticket1 = ticket.TicketComment.Count();
            //}

            TicketItem ticketItem = new TicketItem();
            if (ticket != null)
            {
                ticketItem = ticket.TicketItems.Where(ti => ti.projectid > 0 && ti.skillid > 0).OrderBy(ti => ti.id)
                    .FirstOrDefault();
                //}
                //else
                //{
                //    return RedirectToAction("index", "tickets", new { id = 1 });
                //}

                //if (ticket != null)
                //{
                firstitem = ticket.TicketItems.OrderBy(t => t.id).FirstOrDefault().id.ToString();
                ticket.TicketItems = ticket.TicketItems.OrderByDescending(ti => ti.lastmodifiedtime).Skip(0).Take(10)
                    .ToList();
            }
            else
            {
                return RedirectToAction("index", "tickets", new { id = 1 });
            }

            /****************************************************
            * Fetch projects & skills
            ****************************************************/
            List<SelectListItem> projects = db.Database.SqlQuery<CombinedEntity>("exec projects_loadcombined").Select(x =>
                new SelectListItem
                {
                    Text = x.name,
                    Value = x.id.ToString()
                }).ToList();
            List<Skill> skills = db.Skill.ToList();
            if (ticketItem != null)
            {
                projects.Where(x => x.Value == ticketItem.projectid.ToString()).Single().Selected = true;
            }
            /****************************************************
* Fetch users assigned to the ticket.
****************************************************/
            List<TicketItemLog> ticketusers = (from items in db.TicketItem
                                               join logs in db.TicketItemLog on items.id equals logs.ticketitemid
                                               join users in db.Users on logs.assignedtousersid equals users.Id
                                               where items.ticketid == id.Value
                                               select logs
                ).DistinctBy(l => l.assignedtousersid).ToList();

            List<TicketTeamLogs> ticketTeam = (from teamlog in db.TicketTeamLogs
                                               join team in db.Team on teamlog.teamid equals team.id
                                               where teamlog.ticketid == id.Value
                                               select teamlog).DistinctBy(x => x.teamid).ToList();

            Project project = null;
            if (ticket.projectid != null)
            {
                project = db.Project.Where(p => p.id == ticket.projectid).FirstOrDefault();
            }

            //var ClientSelectList = new SelectList(db.Client.Where(x => x.isactive).ToList(), "id", "name").ToList();
            //ViewBag.clients = ClientSelectList;
            ViewBag.clients = new SelectList(db.Client.Where(x => x.isactive).ToList(), "id", "name").ToList();
            TicketItemViewModel ticketItemVM = new TicketItemViewModel
            {
                Ticket = ticket,
                TicketItem = ticketItem,
                Projects = projects,
                Skills = skills,
                TicketUsers = ticketusers,
                TicketTeam = ticketTeam,
                FirstTicketItemID = firstitem,
                TicketProject = project,
                IsWarning = project != null ? project.iswarning : false,
                IsRestrict = LoginUser.IsRestrictEntertime,
                WarningText = project != null ? project.warningtext : string.Empty
            };
            ViewBag.freedCampUrl =
                (from f in db.freedCampTask where f.ticketid == id.Value select f.url).FirstOrDefault();
            return View(ticketItemVM);
        }

        public ActionResult Comments(long? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Tickets", new { id = 1 });
            }
            // If no id is provided.
            if (id.Value < 0)
            {
                return RedirectToAction("index", "home");
            }

            /****************************************************
            * Fetch Ticket & TicketItems
            ****************************************************/
            ViewBag.id = id.Value;
            string firstitem = null;
            int pageSize = 3; // set your page size, which is number of records per page
            int pagenum = 1;
            int skip = pageSize * (pagenum - 1);

            Ticket ticket = db.Ticket.Where(t => t.id == id.Value).Include(t => t.TicketItems)
                .Include(t => t.TicketComment).Include(t => t.TicketItems.Select(ti => ti.TicketItemLog))
                .Include(t => t.TicketItems.Select(ti => ti.TicketItemAttachment)).SingleOrDefault();
            //db.tic
            if (ticket != null)
            {
                int ticket1 = ticket.TicketComment.Count();
            }

            TicketItem ticketItem = new TicketItem();
            if (ticket != null)
            {
                ticketItem = ticket.TicketItems.Where(ti => ti.projectid > 0 && ti.skillid > 0).OrderBy(ti => ti.id)
                    .FirstOrDefault();
            }
            else
            {
                return RedirectToAction("index", "tickets", new { id = 1 });
            }

            if (ticket != null)
            {
                firstitem = ticket.TicketItems.OrderBy(t => t.id).FirstOrDefault().id.ToString();
                ticket.TicketItems = ticket.TicketItems.OrderByDescending(ti => ti.lastmodifiedtime).Skip(skip)
                    .Take(pageSize).ToList();
            }
            else
            {
                return RedirectToAction("index", "tickets", new { id = 1 });
            }

            /****************************************************
            * Fetch projects & skills
            ****************************************************/
            List<SelectListItem> projects = db.Database.SqlQuery<CombinedEntity>("exec projects_loadcombined").Select(x =>
                new SelectListItem
                {
                    Text = x.name,
                    Value = x.id.ToString()
                }).ToList();
            List<Skill> skills = db.Skill.ToList();
            if (ticketItem != null)
            {
                projects.Where(x => x.Value == ticketItem.projectid.ToString()).Single().Selected = true;
            }
            /****************************************************
* Fetch users assigned to the ticket.
****************************************************/
            List<TicketItemLog> ticketusers = (from items in db.TicketItem
                                               join logs in db.TicketItemLog on items.id equals logs.ticketitemid
                                               join users in db.Users on logs.assignedtousersid equals users.Id
                                               where items.ticketid == id.Value
                                               select logs
                ).DistinctBy(l => l.assignedtousersid).ToList();

            List<TicketTeamLogs> ticketTeam = (from teamlog in db.TicketTeamLogs
                                               join team in db.Team on teamlog.teamid equals team.id
                                               where teamlog.ticketid == id.Value
                                               select teamlog).DistinctBy(x => x.teamid).ToList();

            Project project = null;
            if (ticket.projectid != null)
            {
                project = db.Project.Where(p => p.id == ticket.projectid).FirstOrDefault();
            }

            List<SelectListItem> ClientSelectList = new SelectList(db.Client.Where(x => x.isactive).ToList(), "id", "name").ToList();
            ViewBag.clients = ClientSelectList;
            /****************************************************
            * Get Ticket Comment.
            ****************************************************/
            List<TicketComment> ticketcommentlist = db.TicketComment.Where(x => x.ticketid == id).ToList();
            int count = ticketcommentlist.Count();

            TicketItemViewModel ticketItemVM = new TicketItemViewModel
            {
                Ticket = ticket,
                TicketItem = ticketItem,
                Projects = projects,
                Skills = skills,
                TicketUsers = ticketusers,
                TicketTeam = ticketTeam,
                TicketComment = ticketcommentlist.OrderByDescending(c => c.createdonutc).ToList(),
                FirstTicketItemID = ticket.TicketItems.OrderBy(ti => ti.id).FirstOrDefault().id.ToString(),
                TicketProject = project,
                IsWarning = project != null ? project.iswarning : false,
                WarningText = project != null ? project.warningtext : string.Empty
            };
            return View(ticketItemVM);
        }

        public ActionResult Comment(long? id, long commentid)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Tickets", new { id = 1 });
            }
            // If no id is provided.
            if (id.Value < 0)
            {
                return RedirectToAction("index", "home");
            }

            /****************************************************
            * Fetch Ticket & TicketItems
            ****************************************************/
            ViewBag.id = id.Value;
            ViewBag.commentid = commentid;
            string firstitem = null;
            int pageSize = 3; // set your page size, which is number of records per page
            int pagenum = 1;
            int skip = pageSize * (pagenum - 1);

            Ticket ticket = db.Ticket.Where(t => t.id == id.Value).Include(t => t.TicketItems)
                .Include(t => t.TicketComment).Include(t => t.TicketItems.Select(ti => ti.TicketItemLog))
                .Include(t => t.TicketItems.Select(ti => ti.TicketItemAttachment)).SingleOrDefault();
            //db.tic
            if (ticket != null)
            {
                int ticket1 = ticket.TicketComment.Count();
            }

            TicketItem ticketItem = new TicketItem();
            if (ticket != null)
            {
                ticketItem = ticket.TicketItems.Where(ti => ti.projectid > 0 && ti.skillid > 0).OrderBy(ti => ti.id)
                    .FirstOrDefault();
            }
            else
            {
                return RedirectToAction("index", "tickets", new { id = 1 });
            }

            if (ticket != null)
            {
                firstitem = ticket.TicketItems.OrderBy(t => t.id).FirstOrDefault().id.ToString();
                ticket.TicketItems = ticket.TicketItems.OrderByDescending(ti => ti.lastmodifiedtime).Skip(skip)
                    .Take(pageSize).ToList();
            }
            else
            {
                return RedirectToAction("index", "tickets", new { id = 1 });
            }

            /****************************************************
            * Fetch projects & skills
            ****************************************************/
            List<SelectListItem> projects = db.Database.SqlQuery<CombinedEntity>("exec projects_loadcombined").Select(x =>
                new SelectListItem
                {
                    Text = x.name,
                    Value = x.id.ToString()
                }).ToList();

            List<Skill> skills = db.Skill.ToList();
            if (ticketItem != null)
            {
                projects.Where(x => x.Value == ticketItem.projectid.ToString()).Single().Selected = true;
            }
            /****************************************************
* Fetch users assigned to the ticket.
****************************************************/
            List<TicketItemLog> ticketusers = (from items in db.TicketItem
                                               join logs in db.TicketItemLog on items.id equals logs.ticketitemid
                                               join users in db.Users on logs.assignedtousersid equals users.Id
                                               where items.ticketid == id.Value
                                               select logs
                ).DistinctBy(l => l.assignedtousersid).ToList();

            List<TicketTeamLogs> ticketTeam = (from teamlog in db.TicketTeamLogs
                                               join team in db.Team on teamlog.teamid equals team.id
                                               where teamlog.ticketid == id.Value
                                               select teamlog).DistinctBy(x => x.teamid).ToList();

            List<SelectListItem> ClientSelectList = new SelectList(db.Client.Where(x => x.isactive).ToList(), "id", "name").ToList();
            ViewBag.clients = ClientSelectList;

            Project project = null;
            if (ticket.projectid != null)
            {
                project = db.Project.Where(p => p.id == ticket.projectid).FirstOrDefault();
            }
            /****************************************************
* Get Ticket Comment.
****************************************************/
            List<TicketComment> ticketcommentlist = db.TicketComment.Where(x => x.ticketid == id).ToList();
            int count = ticketcommentlist.Count();

            TicketItemViewModel ticketItemVM = new TicketItemViewModel
            {
                Ticket = ticket,
                TicketItem = ticketItem,
                Projects = projects,
                Skills = skills,
                TicketUsers = ticketusers,
                TicketTeam = ticketTeam,
                TicketComment = ticketcommentlist.OrderByDescending(c => c.createdonutc).ToList(),
                FirstTicketItemID = ticket.TicketItems.OrderBy(ti => ti.id).FirstOrDefault().id.ToString(),
                TicketProject = project,
                IsWarning = project != null ? project.iswarning : false,
                WarningText = project != null ? project.warningtext : string.Empty
            };
            return View(ticketItemVM);
        }

        public ActionResult Credentials(long? id, long projectid = 0)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Tickets", new { id = 1 });
            }
            // If no id is provided.
            if (id.Value < 0)
            {
                return RedirectToAction("index", "home");
            }

            /****************************************************
            * Fetch Ticket & TicketItems
            ****************************************************/
            ViewBag.id = id.Value;
            string firstitem = null;
            int pageSize = 3; // set your page size, which is number of records per page
            int pagenum = 1;
            int skip = pageSize * (pagenum - 1);

            Ticket ticket = db.Ticket.Where(t => t.id == id.Value).Include(t => t.TicketItems)
                .Include(t => t.TicketComment).Include(t => t.TicketItems.Select(ti => ti.TicketItemLog))
                .Include(t => t.TicketItems.Select(ti => ti.TicketItemAttachment)).SingleOrDefault();
            //db.tic
            if (ticket != null)
            {
                int ticket1 = ticket.TicketComment.Count();
            }

            TicketItem ticketItem = new TicketItem();
            if (ticket != null)
            {
                ticketItem = ticket.TicketItems.Where(ti => ti.projectid > 0 && ti.skillid > 0).OrderBy(ti => ti.id)
                    .FirstOrDefault();
            }
            else
            {
                return RedirectToAction("index", "tickets", new { id = 1 });
            }

            if (ticket != null)
            {
                firstitem = ticket.TicketItems.OrderBy(t => t.id).FirstOrDefault().id.ToString();
                ticket.TicketItems = ticket.TicketItems.OrderByDescending(ti => ti.lastmodifiedtime).Skip(skip)
                    .Take(pageSize).ToList();
            }
            else
            {
                return RedirectToAction("index", "tickets", new { id = 1 });
            }

            /****************************************************
            * Fetch projects & skills
            ****************************************************/
            List<SelectListItem> projects = db.Database.SqlQuery<CombinedEntity>("exec projects_loadcombined").Select(x =>
                new SelectListItem
                {
                    Text = x.name,
                    Value = x.id.ToString()
                }).ToList();
            List<Skill> skills = db.Skill.ToList();
            if (ticketItem != null)
            {
                projects.Where(x => x.Value == ticketItem.projectid.ToString()).Single().Selected = true;
            }
            /****************************************************
* Fetch users assigned to the ticket.
****************************************************/
            List<TicketItemLog> ticketusers = (from items in db.TicketItem
                                               join logs in db.TicketItemLog on items.id equals logs.ticketitemid
                                               join users in db.Users on logs.assignedtousersid equals users.Id
                                               where items.ticketid == id.Value
                                               select logs
                ).DistinctBy(l => l.assignedtousersid).ToList();

            List<TicketTeamLogs> ticketTeam = (from teamlog in db.TicketTeamLogs
                                               join team in db.Team on teamlog.teamid equals team.id
                                               where teamlog.ticketid == id.Value
                                               select teamlog).DistinctBy(x => x.teamid).ToList();

            Project project = null;
            if (ticket.projectid != null)
            {
                project = db.Project.Where(p => p.id == ticket.projectid).FirstOrDefault();
            }

            List<SelectListItem> ClientSelectList = new SelectList(db.Client.Where(x => x.isactive).ToList(), "id", "name").ToList();
            ViewBag.clients = ClientSelectList;

            List<Credentials> Credentials = new List<Credentials>();
            //var credentials = db.Credentials.Include(c => c.CredentialCategory).Include(c => c.CredentialLevel).Include(c => c.CredentialType).Include(c => c.Project).Where(pi => pi.projectid==id).Where(cl => cl.credentiallevelid<=user.Levelid).ToList();
            if (projectid > 0)
            {
                Credentials = db.Credentials.Include(c => c.CredentialCategory).Include(c => c.CredentialLevel)
                    .Include(c => c.CredentialType).Include(c => c.Project).Where(pi => pi.projectid == projectid)
                    .Distinct().OrderBy(t => t.title).ThenBy(t => t.credentiallevelid)
                    .ThenBy(t => t.credentialcategoryid).ToList();
            }

            ViewBag.IsProjectCredentials = true;
            ViewBag.projectid = id;

            ViewBag.credentialcategoryid = db.CredentialCategories.ToList();
            //ViewBag.credentiallevelid = db.CredentialLevels.Where(x => x.id == 2 || x.id == 3 || x.id == 4).ToList();
            ViewBag.credentiallevelid =
                new SelectList(db.Database.SqlQuery<CombinedEntity>("exec GetCredentialsLevel_sp").ToList(), "id",
                    "name");
            ViewBag.crendentialtypeid = db.CredentialTypes.ToList();

            TicketItemViewModel ticketItemVM = new TicketItemViewModel
            {
                Ticket = ticket,
                TicketItem = ticketItem,
                Projects = projects,
                Skills = skills,
                TicketUsers = ticketusers,
                TicketTeam = ticketTeam,
                FirstTicketItemID = ticket.TicketItems.OrderBy(ti => ti.id).FirstOrDefault().id.ToString(),
                TicketProject = project,
                Credentials = Credentials,
                IsWarning = project != null ? project.iswarning : false,
                WarningText = project != null ? project.warningtext : string.Empty
            };
            return View(ticketItemVM);
        }

        [HttpPost]
        public ActionResult CreateCredential(string url, string username, string password, long projectid,
            long catogeryid, long typeid, long levelid, long skillid, string title = "", string host = "",
            string port = "", string comments = "")
        {
            Credentials credentials = new Credentials();
            try
            {
                credentials.url = url;
                credentials.username = username;
                credentials.password = password;
                credentials.projectid = projectid;
                credentials.title = title;
                credentials.comments = comments;
                credentials.host = host;
                credentials.port = port;
                byte[] key = EncryptionHelper.GenerateKey();
                credentials.passwordhash = EncryptionHelper.EncryptString(credentials.password, key);
                credentials.passwordsalt = Convert.ToBase64String(key);
                credentials.password = EncryptionHelper.GuidPassword();
                credentials.credentiallevelid = 4;
                credentials.credentialcategoryid = catogeryid;
                credentials.credentiallevelid = levelid;
                credentials.crendentialtypeid = typeid;
                credentials.userid = User.Identity.GetUserId();
                credentials.ipused = Request.UserHostAddress;
                credentials.createdonutc = DateTime.Now;
                credentials.updatedonutc = DateTime.Now;
                credentials.isactive = true;
                db.Credentials.Add(credentials);
                db.SaveChanges();

                return Json(new { error = false, msg = "Successfully added", credentials.id },
                    JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = true, msg = ex.Message }, JsonRequestBehavior.AllowGet);
            }
            //return Json(new { error = false, msg = "Successfully added" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Ticketitemtimelog(long id)
        {
            /****************************************************
            * Ticket All Time Entries
            ****************************************************/
            List<TicketTimeLog> tickettimelog = (from t in db.Ticket
                                                 join ti in db.TicketItem on t.id equals ti.ticketid
                                                 join ttl in db.TicketTimeLog on ti.id equals ttl.ticketitemid
                                                 where t.id == id
                                                 select ttl).OrderByDescending(t => t.workdate).Include(t => t.TeamUser).ToList();

            ViewBag.totalspent =
                CommonFunctions.RoundTwoDecimalPlaces((double)tickettimelog.Sum(t => t.timespentinminutes) / 60);
            ViewBag.totalbillable =
                CommonFunctions.RoundTwoDecimalPlaces((double)tickettimelog.Sum(t => t.billabletimeinminutes) / 60);

            /****************************************************
            * Fetch EmailReplySignature of loged in user
            ****************************************************/
            string LoginUserId = User.Identity.GetUserId();
            ApplicationUser LoginUser = db.Users.Find(LoginUserId);
            ViewBag.EmailSignature = LoginUser.EmailReplySignature;

            /****************************************************
            * Fetch Ticket & TicketItems
            ****************************************************/
            ViewBag.id = id;
            string firstitem = null;
            int pageSize = 3; // set your page size, which is number of records per page
            int pagenum = 1;
            int skip = pageSize * (pagenum - 1);

            Ticket ticket = db.Ticket.Where(t => t.id == id).Include(t => t.TicketItems).Include(t => t.TicketComment)
                .Include(t => t.TicketItems.Select(ti => ti.TicketItemLog))
                .Include(t => t.TicketItems.Select(ti => ti.TicketItemAttachment)).SingleOrDefault();
            //db.tic
            if (ticket != null)
            {
                int ticket1 = ticket.TicketComment.Count();
            }

            TicketItem ticketItem = new TicketItem();
            if (ticket != null)
            {
                ticketItem = ticket.TicketItems.Where(ti => ti.projectid > 0 && ti.skillid > 0).OrderBy(ti => ti.id)
                    .FirstOrDefault();
            }
            else
            {
                return RedirectToAction("index", "tickets", new { id = 1 });
            }

            if (ticket != null)
            {
                firstitem = ticket.TicketItems.OrderBy(t => t.id).FirstOrDefault().id.ToString();
                ticket.TicketItems = ticket.TicketItems.OrderByDescending(ti => ti.lastmodifiedtime).Skip(skip)
                    .Take(pageSize).ToList();
            }
            else
            {
                return RedirectToAction("index", "tickets", new { id = 1 });
            }

            /****************************************************
            * Fetch projects & skills
            ****************************************************/
            List<SelectListItem> projects = db.Database.SqlQuery<CombinedEntity>("exec projects_loadcombined").Select(x =>
                new SelectListItem
                {
                    Text = x.name,
                    Value = x.id.ToString()
                }).ToList();
            List<Skill> skills = db.Skill.ToList();
            if (ticketItem != null)
            {
                projects.Where(x => x.Value == ticketItem.projectid.ToString()).Single().Selected = true;
            }
            /****************************************************
* Fetch users assigned to the ticket.
****************************************************/
            List<TicketItemLog> ticketusers = (from items in db.TicketItem
                                               join logs in db.TicketItemLog on items.id equals logs.ticketitemid
                                               join users in db.Users on logs.assignedtousersid equals users.Id
                                               where items.ticketid == id
                                               select logs
                ).DistinctBy(l => l.assignedtousersid).ToList();

            List<TicketTeamLogs> ticketTeam = (from teamlog in db.TicketTeamLogs
                                               join team in db.Team on teamlog.teamid equals team.id
                                               where teamlog.ticketid == id
                                               select teamlog).DistinctBy(x => x.teamid).ToList();

            List<SelectListItem> ClientSelectList = new SelectList(db.Client.Where(x => x.isactive).ToList(), "id", "name").ToList();
            ViewBag.clients = ClientSelectList;

            Project project = null;
            if (ticket.projectid != null)
            {
                project = db.Project.Where(p => p.id == ticket.projectid).FirstOrDefault();
            }

            TicketItemViewModel ticketItemVM = new TicketItemViewModel
            {
                Ticket = ticket,
                TicketItem = ticketItem,
                Projects = projects,
                Skills = skills,
                Timelog = tickettimelog,
                TicketUsers = ticketusers,
                TicketTeam = ticketTeam,
                FirstTicketItemID = ticket.TicketItems.OrderBy(ti => ti.id).FirstOrDefault().id.ToString(),
                TicketProject = project,
                IsWarning = project != null ? project.iswarning : false,
                IsRestrict = LoginUser.IsRestrictEntertime,
                WarningText = project != null ? project.warningtext : string.Empty
            };
            return View(ticketItemVM);
        }

        ////Download Attachment
        //public ActionResult DownloadAttachment(int ticketatachmentID)
        //{
        //    var ID = db.TicketItemAttachment.FirstOrDefault(t => t.id == ticketatachmentID);
        //    string path = ID.path;
        //    Response.Clear();
        //    Response.ContentType = "application/octect-stream";
        //    Response.AppendHeader("content-disposition", "filename="+ path);
        //    Response.TransmitFile(Server.MapPath(path));
        //    Response.End();

        //    return Json(new { successs = true }, JsonRequestBehavior.AllowGet);
        //}

        //Remove Attachment

        #endregion Ticket Item Pages

        public ActionResult RemoveAttachment(int ticketatachmentID)
        {
            TicketItemAttachment ID = db.TicketItemAttachment.FirstOrDefault(t => t.id == ticketatachmentID);
            db.TicketItemAttachment.Remove(ID);
            db.SaveChanges();
            return Json(new { successs = true }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult TicketItemPart(long id, int pagenum)
        {
            int pageSize = 10; // set your page size, which is number of records per page
            int skip = pageSize * (pagenum - 1);
            Ticket ticket = db.Ticket.Where(t => t.id == id).Include(t => t.TicketItems)
                .Include(t => t.TicketItems.Select(ti => ti.TicketItemLog)).SingleOrDefault();
            ticket.TicketItems = ticket.TicketItems.OrderByDescending(ti => ti.lastmodifiedtime).Skip(skip)
                .Take(pageSize).ToList();
            return View(ticket.TicketItems);
        }

        [HttpPost]
        public ActionResult UpdateTicketProject(long id, long ticketitemid, long projectid)
        {
            // Make sure ticket is provided.
            if (id < 1)
            {
                return Json(new { success = false, messagetext = "Project could not be updated." });
            }

            UpdateClientProjectStatus(projectid);

            // Fetch Ticket.
            TicketItem ticekitem = null;
            Ticket ticket = null;
            if (ticketitemid > 0)
            {
                ticekitem = db.TicketItem.Where(i => i.id == ticketitemid).FirstOrDefault();
            }
            else
            {
                List<TicketItem> temp = db.TicketItem.Where(t => t.ticketid == id).ToList();
                ticekitem = db.TicketItem.Where(t => t.ticketid == id).OrderBy(t => t.lastmodifiedtime)
                    .FirstOrDefault();
            }

            ticket = db.Ticket.SingleOrDefault(x => x.id == id);
            if (ticket != null)
            {
                ticket.projectid = projectid;
                db.Entry(ticket).State = EntityState.Modified;
            }

            if (ticekitem != null)
            {
                ticekitem.projectid = projectid;
                ticekitem.updatedonutc = DateTime.Now;
                ticekitem.ipused = Request.UserHostAddress;
                ticekitem.userid = User.Identity.GetUserId();
                db.Entry(ticekitem).State = EntityState.Modified;
                db.SaveChanges();

                return Json(new { success = true, messagetext = "Project has been updated." });
            }

            return Json(new { success = false, messagetext = "Project could not be updated." });
        }

        private void UpdateClientProjectStatus(long id)
        {
            try
            {
                Project project = db.Project.Find(id);
                if (!project.isactive)
                {
                    project.isactive = true;
                    project.updatedonutc = DateTime.UtcNow;
                    db.Entry(project).State = EntityState.Modified;
                    db.SaveChanges();
                    Client client = db.Client.Find(project.clientid);
                    if (!client.isactive)
                    {
                        client.isactive = true;
                        client.updatedonutc = DateTime.UtcNow;
                        db.Entry(client).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult UpdateTicketSkill(long id, long ticketitemid, long skillid)
        {
            // Make sure ticket is provided.
            if (id < 1)
            {
                return Json(new { success = false, messagetext = "Skill could not be updated." });
            }

            // Fetch Ticket.
            TicketItem ticekitem = null;

            if (ticketitemid > 0)
            {
                ticekitem = db.TicketItem.Where(i => i.id == ticketitemid).FirstOrDefault();
            }
            else
            {
                ticekitem = db.TicketItem.Where(t => t.ticketid == id).OrderBy(t => t.lastmodifiedtime)
                    .FirstOrDefault();
            }

            Ticket ticket = db.Ticket.SingleOrDefault(x => x.id == id);
            if (ticket != null)
            {
                ticket.skillid = skillid;
                db.Entry(ticket).State = EntityState.Modified;
            }

            if (ticekitem != null)
            {
                ticekitem.skillid = skillid;
                ticekitem.updatedonutc = DateTime.Now;
                ticekitem.ipused = Request.UserHostAddress;
                ticekitem.userid = User.Identity.GetUserId();
                db.Entry(ticekitem).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = true, messagetext = "Skill has been updated." });
            }

            return Json(new { success = false, messagetext = "Skill could not be updated." });
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AddTime(long tcktitemid, long pid, long sid, int spenttime, int? billtime,
            DateTime workdate, string title, string description, string comments)
        {
            string userid = User.Identity.GetUserId();
            ApplicationUser user = db.Users.Find(userid);
            DateTime date = DateTimeExtensions.AddWorkingDays(-2);
            if (date.Date <= workdate.Date || user.IsRestrictEntertime)
            {
                try
                {
                    TicketItem ticketitem = db.TicketItem.Where(t => t.id == tcktitemid).FirstOrDefault();
                    if (ticketitem != null)
                    {
                        if (ticketitem.projectid == null)
                        {
                            ticketitem.projectid = pid;
                        }

                        if (ticketitem.skillid == null)
                        {
                            ticketitem.skillid = sid;
                        }

                        ticketitem.updatedonutc = DateTime.Now;
                        ticketitem.ipused = Request.UserHostAddress;
                        ticketitem.userid = User.Identity.GetUserId();
                        db.Entry(ticketitem).State = EntityState.Modified;
                        db.SaveChanges();
                    }

                    TicketTimeLog timelog = new TicketTimeLog
                    {
                        ticketitemid = tcktitemid,
                        teamuserid = User.Identity.GetUserId(),
                        projectid = pid,
                        skillid = sid,
                        timespentinminutes = spenttime
                    };
                    if (billtime == null)
                    {
                        timelog.billabletimeinminutes = 0;
                    }
                    else
                    {
                        timelog.billabletimeinminutes = billtime;
                    }

                    timelog.workdate = workdate.Add(DateTime.Now.TimeOfDay);

                    timelog.title = Server.UrlDecode(title);
                    timelog.description = description;
                    timelog.comments = comments;
                    timelog.createdonutc = DateTime.Now;
                    timelog.updatedonutc = DateTime.Now;
                    timelog.ipused = Request.UserHostAddress;
                    timelog.userid = User.Identity.GetUserId();

                    db.TicketTimeLog.Add(timelog);
                    db.SaveChanges();
                    Ticket ticket = db.Ticket.Where(x => x.id == ticketitem.ticketid).FirstOrDefault();
                    ticket.LastActivityDate = DateTime.Now;
                    db.SaveChanges();
                    string username = user.FullName;
                    int? timespentinminutes = timelog.timespentinminutes;
                    DateTime Workdate = timelog.workdate;
                    return Json(new
                    {
                        success = true,
                        Successtext = "The Ticket Item Time Added.",
                        username,
                        timespentinminutes,
                        Workdate
                    });
                }
                catch (Exception ex)
                {
                    return Json(new
                    {
                        error = true,
                        errortext = ex.Message
                    });
                }
            }

            return Json(new
            {
                error = true,
                errortext = "Time entry is restricted !"
            });
        }

        public void AddTimeWithEmail(long tcktitemid, string pid, string sid, int spenttime, int? billtime,
            DateTime workdate, string title, string description, string comments)
        {
            try
            {
                string userid = User.Identity.GetUserId();

                //TicketTimeLog timelog = db.TicketTimeLog.Where(til => til.ticketitemid == tcktitemid).Where(til => til.teamuserid == userid).Where(til => DbFunctions.TruncateTime(til.workdate) == workdate).FirstOrDefault();
                //if (timelog == null)
                //{
                //    timelog = new TicketTimeLog();
                //}

                TicketItem ticketitem = db.TicketItem.Where(t => t.id == tcktitemid).FirstOrDefault();
                if (ticketitem != null)
                {
                    if (ticketitem.projectid == null)
                    {
                        ticketitem.projectid = Convert.ToInt64(pid);
                    }

                    if (ticketitem.skillid == null)
                    {
                        ticketitem.skillid = Convert.ToInt64(sid);
                    }

                    ticketitem.updatedonutc = DateTime.Now;
                    ticketitem.ipused = Request.UserHostAddress;
                    ticketitem.userid = User.Identity.GetUserId();
                    db.Entry(ticketitem).State = EntityState.Modified;
                    db.SaveChanges();
                }

                TicketTimeLog timelog = new TicketTimeLog
                {
                    ticketitemid = tcktitemid,
                    teamuserid = User.Identity.GetUserId(),
                    projectid = Convert.ToInt64(pid),
                    skillid = Convert.ToInt64(sid),
                    timespentinminutes = spenttime
                };
                if (billtime == null || billtime == 0)
                {
                    timelog.billabletimeinminutes = spenttime;
                }
                else
                {
                    timelog.billabletimeinminutes = billtime;
                }

                timelog.workdate = workdate.Add(DateTime.Now.TimeOfDay);
                timelog.title = title;
                timelog.description = description;
                timelog.comments = comments;
                timelog.createdonutc = DateTime.Now;
                timelog.updatedonutc = DateTime.Now;
                timelog.ipused = Request.UserHostAddress;
                timelog.userid = User.Identity.GetUserId();
                db.TicketTimeLog.Add(timelog);
                db.SaveChanges();
            }
            catch (Exception)
            {
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Startworking(string id, string status, string project, string skill, int quotedtime)
        {
            try
            {
                long ticketItemLogId = 0;

                long.TryParse(id, out long ticketid);

                int.TryParse(status, out int statusid);

                long.TryParse(project, out long projectid);

                long.TryParse(skill, out long skillid);

                // Load ticket.
                Ticket ticket = db.Ticket.Where(i => i.id == ticketid).FirstOrDefault();

                if (ticket == null)
                {
                    return Json(new { success = false, successtext = "Sorry, the ticket is not found." });
                }

                if (ticket.statusid == 1) // new ticket.
                {
                    ticket.projectid = projectid;
                    ticket.skillid = skillid;
                    ticket.statusid = statusid; // in progress
                    ticket.statusupdatedbyusersid = User.Identity.GetUserId();
                    ticket.statusupdatedon = DateTime.Now;
                    ticket.LastActivityDate = DateTime.Now;
                    db.Entry(ticket).State = EntityState.Modified;
                }

                // Fetch first ticket item.
                TicketItem ticketItem = db.TicketItem.Where(t => t.ticketid == ticket.id).OrderBy(t => t.lastmodifiedtime)
                    .FirstOrDefault();
                ticketItem.projectid = projectid;
                ticketItem.skillid = skillid;
                ticketItem.statusid = statusid;
                ticketItem.statusupdatedbyusersid = User.Identity.GetUserId();
                ticketItem.statusupdatedon = DateTime.Now;
                ticketItem.quotedtimeinminutes = quotedtime;
                ticketItem.updatedonutc = DateTime.Now;
                ticketItem.ipused = Request.UserHostAddress;
                ticketItem.userid = User.Identity.GetUserId();
                db.Entry(ticketItem).State = EntityState.Modified;

                // add current user's log.
                string userid = User.Identity.GetUserId();
                TicketItemLog ticketItemLog = db.TicketItemLog
                    .Where(i => i.ticketitemid == ticketItem.id && i.assignedtousersid == userid).FirstOrDefault();

                if (ticketItemLog == null)
                {
                    long? count = db.TicketItemLog
                        .Where(ti =>
                            ti.assignedtousersid == userid && ti.displayorder != null && ti.statusid == statusid)
                        .Max(d => d.displayorder);

                    ticketItemLog = new TicketItemLog
                    {
                        displayorder = count != null ? count + 1 : 1,
                        ticketitemid = ticketItem.id,
                        statusid = statusid,
                        assignedbyusersid = userid,
                        assignedtousersid = userid,
                        assignedon = DateTime.Now,
                        statusupdatedbyusersid = userid,
                        statusupdatedon = DateTime.Now
                    };
                    db.TicketItemLog.Add(ticketItemLog);
                }
                else
                {
                    return Json(new { success = false, successtext = "The ticket is already assign to you." });
                }

                // Save all changes.
                db.SaveChanges();
                ticketItemLogId = ticketItemLog.id;
                string username = "";
                ApplicationUser user = UserManager.FindById(userid);
                if (user != null)
                {
                    username = user.FirstName + " " + user.LastName;
                }

                return Json(new
                {
                    success = true,
                    successtext = "The ticket item has been assigned to you.",
                    assignmentid = ticketItemLogId,
                    userid,
                    username
                });
            }
            catch (Exception ex)
            {
                return Json(new { error = true, errortext = ex.Message });
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AssignTicketUser(long id, string userid, string ticketID)
        {
            TicketItemLog usertoassign = db.TicketItemLog.Where(t => t.ticketitemid == id)
                .Where(t => t.assignedtousersid == userid).FirstOrDefault();
            if (usertoassign == null)
            {
                TicketItemLog ticketItemLog = new TicketItemLog
                {
                    ticketitemid = id,
                    assignedbyusersid = User.Identity.GetUserId(),
                    assignedtousersid = userid,
                    assignedon = DateTime.Now,
                    statusid = 6, // Assigned
                    statusupdatedbyusersid = User.Identity.GetUserId(),
                    statusupdatedon = DateTime.Now,
                    displayorder = 0
                };
                Ticket ticket = new Ticket();
                if (ticketID != null)
                {
                    long ID = Convert.ToInt64(ticketID);
                    ticket = db.Ticket.SingleOrDefault(x => x.id == ID);
                    if (ticket != null)
                    {
                        ticket.statusid = 6;
                        ticket.updatedonutc = DateTime.Now;
                    }
                }

                db.TicketItemLog.Add(ticketItemLog);
                db.SaveChanges();

                return Json(new { success = true, messagetext = "The user has been assigned successfully." });
            }

            return Json(new { success = false, messagetext = "The user is already assigned." });
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult RemoveTicketUser(long id, string userid)
        {
            List<TicketItem> ticketitems = db.TicketItem.Where(x => x.ticketid == id).OrderBy(x => x.id).ToList();
            if (ticketitems.Count > 0)
            {
                foreach (TicketItem item in ticketitems)
                {
                    TicketItemLog usertoassign = db.TicketItemLog.Where(t => t.ticketitemid == item.id)
                        .Where(t => t.assignedtousersid == userid).FirstOrDefault();
                    if (usertoassign != null)
                    {
                        db.TicketItemLog.Remove(usertoassign);
                    }
                }

                db.SaveChanges();

                TicketItem ticketitem = db.TicketItem.Where(x => x.ticketid == id).OrderBy(x => x.id).ToList()
                    .FirstOrDefault();
                TicketItemLog ticketitemlog = db.TicketItemLog.Where(t => t.ticketitemid == ticketitem.id).FirstOrDefault();
                if (ticketitemlog == null)
                {
                    Ticket ticket = db.Ticket.Where(t => t.id == id).FirstOrDefault();
                    ticket.statusid = 1;
                    db.SaveChanges();
                }

                // Updated By Muhmmad Nasir on 27-11-2018
                ApplicationUser user = UserManager.FindById(userid);
                TicketLogs TicketLogs = new TicketLogs
                {
                    ticketid = Convert.ToInt64(id),
                    actiontypeid = 11,
                    actiondate = DateTime.Now,
                    actionbyuserId = User.Identity.GetUserId()
                };
                //The ticket # 160101 is assigned to [Shaban Sarfraz] by [Bratislav].

                ApplicationUser userAdmin = UserManager.FindById(Convert.ToString(User.Identity.GetUserId()));
                TicketLogs.ActionDescription = "[" + user.FirstName + " " + user.LastName +
                                               "] is removed from <a href='/tickets/ticketitem/" +
                                               Convert.ToString(id) + "'>ticket #" + Convert.ToString(id) + "</a> by " +
                                               "[" + userAdmin.FirstName + " " + userAdmin.LastName + "] on " +
                                               Convert.ToString(DateTime.Now);
                ;
                db.TicketLogs.Add(TicketLogs);
                db.SaveChanges();

                return Json(new { success = true, messagetext = "The user has been removed successfully." });
            }

            return Json(new { success = false, messagetext = "User not found." });
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult RemoveTicketteam(long id, long teamid)
        {
            TicketTeamLogs teamtoassign = db.TicketTeamLogs.Where(t => t.ticketid == id).Where(t => t.teamid == teamid)
                .FirstOrDefault();
            if (teamtoassign != null)
            {
                db.TicketTeamLogs.Remove(teamtoassign);
                db.SaveChanges();

                // Updated By Muhmmad Nasir on 28-11-2018
                ApplicationUser user = UserManager.FindById(User.Identity.GetUserId());
                TicketLogs TicketLogs = new TicketLogs
                {
                    ticketid = Convert.ToInt64(id),
                    actiontypeid = 12,
                    actiondate = DateTime.Now,
                    actionbyuserId = User.Identity.GetUserId()
                };
                //The ticket # 160101 is assigned to [Shaban Sarfraz] by [Bratislav].

                Team Team = db.Team.Find(teamid);
                TicketLogs.ActionDescription = "Team [" + Team.name + "] is removed from <a>ticket #" +
                                               Convert.ToString(id) + "</a> by " + "[" + user.FirstName + " " +
                                               user.LastName + "] on " + Convert.ToString(DateTime.Now);
                ;
                db.TicketLogs.Add(TicketLogs);
                db.SaveChanges();

                return Json(new { success = true, messagetext = "The team has been removed successfully." });
            }

            return Json(new { success = false, messagetext = "team not found." });
        }

        #endregion TicketItems

        #region Custom Methods

        public JsonResult PrefetchContactEmails()
        {
            IQueryable<EmailSendToModel> contacts = db.Contact.Where(u => u.isactive == true).Select(x => new EmailSendToModel
            {
                text = x.DisplayName + " <" + x.Email + ">",
                value = x.Email
            }).ToList().AsQueryable();
            return Json(contacts, JsonRequestBehavior.AllowGet);
        }

        public JsonResult PrefetchUsers()
        {
            IQueryable<AssignedUserModel> users = db.Users.Where(u => u.isactive == true).OrderBy(u => u.FirstName).ThenBy(u => u.LastName)
                .Select(x => new AssignedUserModel
                {
                    text = x.FirstName + " " + x.LastName,
                    value = x.Id
                }).ToList().AsQueryable();
            return Json(users, JsonRequestBehavior.AllowGet);
        }

        public JsonResult PrefetchTeams()
        {
            IQueryable<AssignedUserModel> teams = db.Team.Where(u => u.isactive == true).Select(x => new AssignedUserModel
            {
                text = x.name,
                value = x.id.ToString()
            }).ToList().AsQueryable();
            return Json(teams, JsonRequestBehavior.AllowGet);
        }

        public JsonResult PrefetchSingleTeams(string UserID)
        {
            ///this is test of feature
            ApplicationUser user = db.Users.Where(x => x.Id == UserID).ToList().FirstOrDefault();
            List<TeamMember> teamMember = db.TeamMember.Where(x => x.usersid == UserID).Include(x => x.Team).ToList();
            if (teamMember != null && teamMember.Count() > 0)
            {
                return Json(new { error = false, teamid = teamMember[0].Team.id, teamName = teamMember[0].Team.name },
                    JsonRequestBehavior.AllowGet);
            }

            return Json(
                new
                {
                    error = true,
                    errortext = user.FirstName + " " + user.LastName + " is not related to any Team  please assign a team and try again!"
                }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult LoadTicketMeta(long TicketId)
        {
            ///this is test of feature
            List<TicketLogs> ticket = db.TicketLogs.Where(x => x.ticketid == TicketId).OrderBy(x => x.id).ToList();
            Ticket ticketdate = db.Ticket.Find(TicketId);

            if (ticket != null && ticket.Count() > 0)
            {
                return Json(
                    new
                    {
                        error = false,
                        TicketMeta = ticket,
                        ticketdate = ticketdate.createdonutc.ToString("yyyy-MM-dd hh:mm tt")
                    }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { error = true, errortext = "Sorry! No Ticket Meta found" }, JsonRequestBehavior.AllowGet);
        }

        //public JsonResult PrefetchSingleTeams(string Username)
        //{
        //      string[] names = Username.Split(' ');
        //      string Fname = names[0];
        //      string Lname = names[1];

        //        var user = db.Users.Where(u => u.FirstName == Fname && u.LastName == Lname).FirstOrDefault();

        //    ///this is test of feature
        //    var teamMember = db.TeamMember.Where(x => x.usersid == user.Id).Include(x => x.Team).ToList();

        //    if (teamMember != null && teamMember.Count() > 0)
        //    {
        //        return Json(new { error = false, teamid = teamMember[0].Team.id, teamName = teamMember[0].Team.name }, JsonRequestBehavior.AllowGet);
        //    }

        //    return Json(new { error = true, errortext = "This User in not related to any Team  please assign a team and try again!" }, JsonRequestBehavior.AllowGet);
        //}

        public long GetTicketCount(int sid)
        {
            long count = 0;
            if (sid == 0)
            {
                count = db.Ticket.Count();
            }
            else
            {
                count = db.Ticket.Where(s => s.statusid == sid).Count();
            }

            return count;
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
                                        currentSubTab = "Done";
                                        break;

                                    case "4":
                                        currentSubTab = "On Hold";
                                        break;

                                    case "5":
                                        currentSubTab = "QC";
                                        break;

                                    case "6":
                                        currentSubTab = "Assigned";
                                        break;

                                    case "7":
                                        currentSubTab = "In Review";
                                        break;

                                    case "8":
                                        currentSubTab = "Trash";
                                        break;

                                    case "9":
                                        currentSubTab = "Archived";
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

        private List<Ticket> FetchPaginatedMyTickets(long id, long clientid, int page, string topic)
        {
            // Calculate records to skip.
            int skipRecords = page * recordsPerPage;
            string userid = User.Identity.GetUserId();

            // Start writing the query.
            IQueryable<Ticket> ticketsQuery = null;

            if (id == 0 && clientid == 0)
            {
                ticketsQuery = from tickets in db.Ticket
                               join items in db.TicketItem on tickets.id equals items.ticketid
                               join logs in db.TicketItemLog on items.id equals logs.ticketitemid
                               where logs.assignedtousersid == userid
                               orderby tickets.lastmodifiedtime descending, tickets.topic
                               select tickets;
            }
            else if (id == 0 && clientid > 0)
            {
                ticketsQuery = from tickets in db.Ticket
                               join items in db.TicketItem on tickets.id equals items.ticketid
                               join logs in db.TicketItemLog on items.id equals logs.ticketitemid
                               join proj in db.Project on items.projectid equals proj.id
                               where logs.assignedtousersid == userid && proj.clientid == clientid
                               orderby tickets.lastmodifiedtime descending, tickets.topic
                               select tickets;
            }
            else if (id > 0 && clientid == 0)
            {
                ticketsQuery = from tickets in db.Ticket
                               join items in db.TicketItem on tickets.id equals items.ticketid
                               join logs in db.TicketItemLog on items.id equals logs.ticketitemid
                               where logs.assignedtousersid == userid && tickets.statusid == id
                               orderby tickets.lastmodifiedtime descending, tickets.topic
                               select tickets;
            }
            else if (id > 0 && clientid > 0)
            {
                ticketsQuery = from tickets in db.Ticket
                               join items in db.TicketItem on tickets.id equals items.ticketid
                               join logs in db.TicketItemLog on items.id equals logs.ticketitemid
                               join proj in db.Project on items.projectid equals proj.id
                               where logs.assignedtousersid == userid && proj.clientid == clientid && tickets.statusid == id
                               orderby tickets.lastmodifiedtime descending, tickets.topic
                               select tickets;
            }

            if (!string.IsNullOrEmpty(topic))
            {
                ticketsQuery = ticketsQuery.Where(t => t.topic.Contains(topic) || t.uniquesenders.Contains(topic));
            }

            return ticketsQuery.Distinct().OrderByDescending(t => t.lastmodifiedtime).Skip(skipRecords)
                .Take(recordsPerPage).ToList();
        }

        private TicketViewModel GetPaginatedTickets(int id, string topic, int page = 1)
        {
            int skipRecords = page * recordsPerPage;
            IQueryable<Ticket> listOfProducts = null;
            if (id == 0)
            {
                listOfProducts = (from tt in db.Ticket
                                  join ticketi in db.TicketItem on tt.id equals ticketi.ticketid
                                  select tt
                    ).Include(t => t.FlagStatus).Include(tt => tt.TicketType).Include(t => t.StatusUpdatedByUser)
                    .Include(t => t.ConversationStatus).GroupBy(t => t.id).Select(t => t.FirstOrDefault());
            }
            //temprarily assign 9 to Archived ..just to avoid not create new action in controller
            else if (id == 9)
            {
                listOfProducts = (from tt in db.Ticket
                                  join ticketi in db.TicketItem on tt.id equals ticketi.ticketid
                                  select tt
                    ).Include(t => t.FlagStatus).Include(tt => tt.TicketType).Include(t => t.StatusUpdatedByUser)
                    .Include(t => t.ConversationStatus).Where(t => t.IsArchieved == true).GroupBy(t => t.id)
                    .Select(t => t.FirstOrDefault());
            }
            else
            {
                listOfProducts = (from tt in db.Ticket
                                  join ticketi in db.TicketItem on tt.id equals ticketi.ticketid
                                  select tt
                    ).Include(t => t.FlagStatus).Include(tt => tt.TicketType).Include(t => t.StatusUpdatedByUser)
                    .Include(t => t.ConversationStatus).Where(t => t.statusid == id && t.IsArchieved == false)
                    .GroupBy(t => t.id).Select(t => t.FirstOrDefault());
            }

            if (!string.IsNullOrEmpty(topic))
            {
                topic = topic.Trim();
                listOfProducts = listOfProducts.Where(t => t.topic.Contains(topic));
            }

            //ViewBag.status = db.ConversationStatus;
            //List<SelectListItem> projectlist = db.Database.SqlQuery<CombinedEntity>("exec projects_loadcombined").Select(x =>
            //    new SelectListItem
            //    {
            //        Text = x.name,
            //        Value = x.id.ToString()
            //    }).ToList();
            //ViewBag.projects = new SelectList(projectlist, "Value", "Text");
            //ViewBag.skills = db.Skill.ToList();
            List<TicketItem> ti = new List<TicketItem>();
            TicketViewModel tvm = new TicketViewModel();
            //string userid = User.Identity.GetUserId();
            //IQueryable<TicketUserFlagged> flag = db.TicketUserFlagged.Where(f => f.isactive && f.userid == userid).ToList().AsQueryable();
            //tvm.flaggeditems = db.TicketUserFlagged.Where(f => f.isactive && f.userid == userid).ToList().AsQueryable(); ;
            tvm.tickets = listOfProducts.OrderByDescending(t => t.lastdeliverytime).Skip(skipRecords)
                .Take(recordsPerPage).ToList().AsQueryable();
            foreach (Ticket items in tvm.tickets)
            {
                TicketItem ticketitem = db.TicketItem.Where(t => t.ticketid == items.id).OrderByDescending(t => t.createdonutc)
                    .FirstOrDefault();
                ti.Add(ticketitem);
            }
            tvm.ticketitems = ti;

            return tvm;
        }

        #endregion Custom Methods
    }
}