using computan.timesheet.core;
using computan.timesheet.Extensions;
using computan.timesheet.Helpers;
using computan.timesheet.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace computan.timesheet.Controllers
{
    [CustomeAuthorizeAttribute]
    public class TicketItemsController : BaseController
    {
        // Max records per page for ajax requets.
        private readonly int recordsPerPage = 100;

        private ApplicationUserManager _userManager;

        // Create constructors.
        public TicketItemsController()
        {
        }

        public TicketItemsController(ApplicationUserManager userManager)
        {
            UserManager = userManager;
        }

        public ApplicationUserManager UserManager
        {
            get => _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            private set => _userManager = value;
        }

        // GET: TicketItems
        public ActionResult Index(long id)
        {
            IQueryable<TicketItem> ticketItem = db.TicketItem.Include(t => t.Project).Include(t => t.Skill)
                .Include(t => t.StatusUpdatedByUser).Include(t => t.TicketStatus).Include(t => t.TicketItemAttachment)
                .Include(t => t.TicketItemLog.Select(s => s.TicketStatus)).Include(t => t.TicketTimeLog)
                .Where(t => t.ticketid == id);
            ViewBag.ticketstatuslist = db.TicketStatus.ToList();
            ViewBag.currentuserid = User.Identity.GetUserId();
            List<SelectListItem> projectlist = db.Database.SqlQuery<CombinedEntity>("exec projects_loadcombined").Select(x =>
                new SelectListItem
                {
                    Text = x.name,
                    Value = x.id.ToString()
                }).ToList();
            ViewBag.projects = projectlist;
            ViewBag.skills = db.Skill.ToList();
            List<SelectListItem> timespentinminutes = new List<SelectListItem>();
            int timespentinminute = 0;
            int Counter = 1;
            for (Counter = 1; Counter <= 120; Counter++)
            {
                timespentinminute = timespentinminute + 5;
                timespentinminutes.Add(new SelectListItem
                { Text = timespentinminute.ToString(), Value = timespentinminute.ToString() });
            }

            ViewBag.timespentinminutes = timespentinminutes;
            ViewBag.billabletimeinminutes = timespentinminutes;
            ViewBag.users = UserManager.Users.OrderBy(f => f.FirstName).Where(us => us.isactive == true).ToList();
            ViewBag.currentuser = User.Identity.GetUserId();
            return View(ticketItem.ToList().OrderByDescending(ti => ti.lastmodifiedtime));
        }

        public ActionResult TicketItem(long id)
        {
            IQueryable<TicketItem> ticketItem = db.TicketItem.Include(t => t.Project).Include(t => t.Skill)
                .Include(t => t.StatusUpdatedByUser).Include(t => t.TicketStatus).Include(t => t.TicketItemAttachment)
                .Include(t => t.TicketItemLog.Select(s => s.TicketStatus)).Include(t => t.TicketTimeLog)
                .Where(t => t.id == id);
            ViewBag.ticketstatuslist = db.TicketStatus.ToList();
            ViewBag.currentuserid = User.Identity.GetUserId();
            List<SelectListItem> projectlist = db.Database.SqlQuery<CombinedEntity>("exec projects_loadcombined").Select(x =>
                new SelectListItem
                {
                    Text = x.name,
                    Value = x.id.ToString()
                }).ToList();
            ViewBag.projects = projectlist;
            ViewBag.skills = db.Skill.ToList();
            List<SelectListItem> timespentinminutes = new List<SelectListItem>();
            int timespentinminute = 0;
            int Counter = 1;
            for (Counter = 1; Counter <= 120; Counter++)
            {
                timespentinminute = timespentinminute + 5;
                timespentinminutes.Add(new SelectListItem
                { Text = timespentinminute.ToString(), Value = timespentinminute.ToString() });
            }

            ViewBag.timespentinminutes = timespentinminutes;
            ViewBag.billabletimeinminutes = timespentinminutes;
            ViewBag.users = UserManager.Users.OrderBy(f => f.FirstName).Where(us => us.isactive == true).ToList();
            return View("index", ticketItem.ToList().OrderByDescending(ti => ti.lastmodifiedtime));
        }


        [HttpGet]
        [ValidateInput(false)]
        public ActionResult SearchTask(string searchstring, int statusid, int pagenum)
        {
            int skipRecords = pagenum * recordsPerPage;
            try
            {
                TicketStatusAction ticketStatusAction = new TicketStatusAction();
                List<ConversationStatus> TicketStatuses = db.ConversationStatus.ToList();
                foreach (ConversationStatus status in TicketStatuses)
                {
                    switch (status.name.ToLower())
                    {
                        case "new task":
                            break;
                        case "in progress":
                            break;
                        case "closed":
                            status.name = "Close";
                            ticketStatusAction.CloseTicketStatus = status;
                            break;
                        default:
                            if (ticketStatusAction.OtherTicketStatusCollection == null)
                            {
                                ticketStatusAction.OtherTicketStatusCollection = new List<ConversationStatus>();
                            }

                            ticketStatusAction.OtherTicketStatusCollection.Add(status);
                            break;
                    }
                }

                TicketItemStatusAction ticketItemStatusAction = new TicketItemStatusAction();
                List<TicketStatus> TicketItemStatuses = db.TicketStatus.ToList();
                foreach (TicketStatus status in TicketItemStatuses)
                {
                    switch (status.name.ToLower())
                    {
                        case "not assigned":
                            break;
                        case "in progress":
                            break;
                        case "closed":
                            status.name = "Close";
                            ticketItemStatusAction.CloseTicketStatus = status;
                            break;
                        default:
                            if (ticketItemStatusAction.OtherTicketStatusCollection == null)
                            {
                                ticketItemStatusAction.OtherTicketStatusCollection = new List<TicketStatus>();
                            }

                            ticketItemStatusAction.OtherTicketStatusCollection.Add(status);
                            break;
                    }
                }

                // trim user search string.
                searchstring = searchstring.Trim();
                string userid = User.Identity.GetUserId();
                // Fetch Ticek items.
                List<TicketItem> SearchResults = null;
                if (!string.IsNullOrEmpty(searchstring))
                {
                    if (statusid == 0)
                    {
                        SearchResults = (from task in db.TicketItem
                                         join tl in db.TicketItemLog on task.id equals tl.ticketitemid
                                         where tl.assignedtousersid == userid && (task.subject.Contains(searchstring) ||
                                                                                  task.@from.Contains(searchstring) ||
                                                                                  task.lastmodifiedname.Contains(searchstring))
                                         orderby tl.displayorder descending
                                         orderby tl.assignedon descending
                                         select task).Include(t => t.Ticket).Include(t => t.TicketItemLog).Skip(skipRecords)
                            .Take(recordsPerPage).ToList();
                    }
                    else
                    {
                        SearchResults = (from task in db.TicketItem
                                         join tl in db.TicketItemLog on task.id equals tl.ticketitemid
                                         where tl.statusid == statusid && tl.assignedtousersid == userid &&
                                               (task.subject.Contains(searchstring) || task.@from.Contains(searchstring) ||
                                                task.lastmodifiedname.Contains(searchstring))
                                         orderby tl.displayorder descending
                                         orderby tl.assignedon descending
                                         select task).Include(t => t.Ticket).Include(t => t.TicketItemLog).Skip(skipRecords)
                            .Take(recordsPerPage).ToList();
                    }
                }
                else
                {
                    if (statusid == 0)
                    {
                        SearchResults = (from task in db.TicketItem
                                         join tl in db.TicketItemLog on task.id equals tl.ticketitemid
                                         where tl.assignedtousersid == userid
                                         orderby tl.displayorder descending
                                         orderby tl.assignedon descending
                                         select task).Include(t => t.Ticket).Include(t => t.TicketItemLog).Skip(skipRecords)
                            .Take(recordsPerPage).ToList();
                    }
                    else
                    {
                        SearchResults = (from task in db.TicketItem
                                         join tl in db.TicketItemLog on task.id equals tl.ticketitemid
                                         where tl.statusid == statusid && tl.assignedtousersid == userid
                                         orderby tl.displayorder descending
                                         orderby tl.assignedon descending
                                         select task).Include(t => t.Ticket).Include(t => t.TicketItemLog).Skip(skipRecords)
                            .Take(recordsPerPage).ToList();
                    }
                }

                ViewBag.status = db.TicketStatus;
                ViewBag.ticketstatuslist = db.TicketStatus.ToList();
                ViewBag.conversationsatus = db.ConversationStatus.ToList();
                ViewBag.currentuserid = User.Identity.GetUserId();
                List<SelectListItem> projectlist = db.Database.SqlQuery<CombinedEntity>("exec projects_loadcombined").Select(x =>
                    new SelectListItem
                    {
                        Text = x.name,
                        Value = x.id.ToString()
                    }).ToList();
                ViewBag.projects = projectlist;
                ViewBag.skills = db.Skill.ToList();
                ViewBag.projectid_addbucket = projectlist;
                ViewBag.skillid_addbicket = db.Skill.ToList();
                List<SelectListItem> timespentinminutes = new List<SelectListItem>();
                int timespentinminute = 0;
                int Counter = 1;
                for (Counter = 1; Counter <= 120; Counter++)
                {
                    timespentinminute = timespentinminute + 5;
                    timespentinminutes.Add(new SelectListItem
                    { Text = timespentinminute.ToString(), Value = timespentinminute.ToString() });
                }

                int totalcount = 0;
                if (!string.IsNullOrEmpty(searchstring))
                {
                    if (statusid == 0)
                    {
                        totalcount = (from task in db.TicketItem
                                      join tl in db.TicketItemLog on task.id equals tl.ticketitemid
                                      where tl.assignedtousersid == userid && (task.subject.Contains(searchstring) ||
                                                                               task.@from.Contains(searchstring) ||
                                                                               task.lastmodifiedname.Contains(searchstring))
                                      orderby tl.displayorder descending
                                      orderby tl.assignedon descending
                                      select task).Include(t => t.Ticket).Include(t => t.TicketItemLog).Count();
                    }
                    else
                    {
                        totalcount = (from task in db.TicketItem
                                      join tl in db.TicketItemLog on task.id equals tl.ticketitemid
                                      where tl.statusid == statusid && tl.assignedtousersid == userid &&
                                            (task.subject.Contains(searchstring) || task.@from.Contains(searchstring) ||
                                             task.lastmodifiedname.Contains(searchstring))
                                      orderby tl.displayorder descending
                                      orderby tl.assignedon descending
                                      select task).Include(t => t.Ticket).Include(t => t.TicketItemLog).Skip(skipRecords).Count();
                    }
                }
                else
                {
                    if (statusid == 0)
                    {
                        totalcount = (from task in db.TicketItem
                                      join tl in db.TicketItemLog on task.id equals tl.ticketitemid
                                      where tl.assignedtousersid == userid
                                      orderby tl.displayorder descending
                                      orderby tl.assignedon descending
                                      select task).Include(t => t.Ticket).Include(t => t.TicketItemLog).Count();
                    }
                    else
                    {
                        totalcount = (from task in db.TicketItem
                                      join tl in db.TicketItemLog on task.id equals tl.ticketitemid
                                      where tl.statusid == statusid && tl.assignedtousersid == userid
                                      orderby tl.displayorder descending
                                      orderby tl.assignedon descending
                                      select task).Include(t => t.Ticket).Include(t => t.TicketItemLog).Skip(skipRecords).Count();
                    }
                }

                ViewBag.ticketstatusaction = ticketStatusAction;
                ViewBag.ticketitemstatusaction = ticketItemStatusAction;
                ViewBag.timespentinminutes = timespentinminutes;
                ViewBag.billabletimeinminutes = timespentinminutes;
                ViewBag.users = UserManager.Users.OrderBy(u => u.FirstName).Where(us => us.isactive == true).ToList();
                ViewBag.statusid = statusid;
                System.Data.Entity.Infrastructure.DbRawSqlQuery<TicketStatusViewModel> MyStatus = db.Database.SqlQuery<TicketStatusViewModel>("exec ticketstatus_loadtaskscount @userid",
                    new SqlParameter("@userid", userid));
                ViewBag.MyTicketStatus = MyStatus;
                string tasks = PartialView("_mytask", SearchResults).RenderToString();
                return Json(new { tasks, itemcount = SearchResults.Count(), totalcount }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return PartialView("");
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
                case "home":
                    currentSubTab = "";
                    break;
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
                                        currentSubTab = "Not Assigned";
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
                case "ticketitems":
                    switch (currentAction.ToLower())
                    {
                        case "mytasks":
                            object id = rd.Values["id"];
                            if (id != null)
                            {
                                switch (id.ToString())
                                {
                                    case "0":
                                        currentSubTab = "All";
                                        break;
                                    case "1":
                                        currentSubTab = "Not Assigned";
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

                case "tickettimelogs":
                    switch (currentAction.ToLower())
                    {
                        case "index":
                            currentSubTab = "My Time Log";
                            break;
                        case "team":
                            currentSubTab = "Team Time Log";
                            break;
                    }

                    break;

                case "clients":
                    currentSubTab = "Clients";
                    break;

                case "projects":
                    currentSubTab = "Projects";
                    break;
                case "rules":
                    currentSubTab = "Rules";
                    break;
                case "teams":
                    currentSubTab = "Teams";
                    break;

                case "skills":
                    currentSubTab = "Skills";
                    break;

                case "states":
                    currentSubTab = "States";
                    break;

                case "countries":
                    currentSubTab = "Countries";
                    break;

                case "rolesadmin":
                    currentSubTab = "Manage Roles";
                    break;
                case "usersadmin":
                    currentSubTab = "Manage Users";
                    break;
            }

            return currentSubTab;
        }


        //public ActionResult MytaskRow(int? pageNum)
        //{
        //    if (!IsLoggedIn())
        //    {
        //        return RedirectToAction("login", "account");
        //    }
        //    string userid = User.Identity.GetUserId();
        //    var page = pageNum ?? 0;
        //    if (Request.IsAjaxRequest())
        //    {
        //        return PartialView("_mytask", GetPaginatedTasks(page));
        //    }
        //    var ticketItem = (from task in db.TicketItem
        //                      join tl in db.TicketItemLog on task.id equals tl.ticketitemid
        //                      where tl.statusid == id && tl.assignedtousersid == userid
        //                      select task).Include(t => t.TicketStatus).Include(t => t.TicketItemLog).Include(t => t.TicketTimeLog).Include(t => t.TicketItemAttachment).Take(recordsPerPage).ToList();
        //    return View("Index", ticketItem);
        //}

        // GET: TicketItems/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            string userid = User.Identity.GetUserId();
            TicketItem ticketItem = db.TicketItem.Find(id);
            if (ticketItem == null)
            {
                return HttpNotFound();
            }

            return Json(ticketItem, JsonRequestBehavior.AllowGet);
        }

        // GET: TicketItems/Create
        public ActionResult Create()
        {
            List<SelectListItem> projectlist = db.Database.SqlQuery<CombinedEntity>("exec projects_loadcombined").Select(x =>
                new SelectListItem
                {
                    Text = x.name,
                    Value = x.id.ToString()
                }).ToList();
            ViewBag.projectid = new SelectList(projectlist, "Value", "Text");
            ViewBag.skillid = new SelectList(db.Skill, "id", "name");
            ViewBag.statusupdatedbyusersid =
                new SelectList(UserManager.Users.OrderBy(u => u.FirstName).Where(us => us.isactive == true).ToList(),
                    "Id", "Email");
            ViewBag.statusid = new SelectList(db.TicketStatus, "id", "name");
            return View();
        }

        // POST: TicketItems/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(
            [Bind(Include =
                "id,supportconversationid,emailmessageid,bccrecipients,body,conversationid,conversationindex,conversationtopic,datetimecreated,datetimereceived,datetimesent,displaycc,displayto,from,hasattachments,importance,inreplyto,internetmessageheaders,internetmessageid,lastmodifiedname,lastmodifiedtime,mimecontent,replyto,sensitivity,size,subject,torecipients,uniquebody,projectid,skillid,statusid,statusupdatedbyusersid,statusupdatedon,createdonutc,updatedonutc,ipused,userid")]
            TicketItem ticketItem)
        {
            if (ModelState.IsValid)
            {
                db.TicketItem.Add(ticketItem);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            List<SelectListItem> projectlist = db.Database.SqlQuery<CombinedEntity>("exec projects_loadcombined").Select(x =>
                new SelectListItem
                {
                    Text = x.name,
                    Value = x.id.ToString()
                }).ToList();
            ViewBag.projectid = new SelectList(projectlist, "Value", "Text", ticketItem.projectid);
            ViewBag.skillid = new SelectList(db.Skill, "id", "name", ticketItem.skillid);
            ViewBag.statusupdatedbyusersid =
                new SelectList(UserManager.Users.OrderBy(u => u.FirstName).Where(us => us.isactive == true).ToList(),
                    "Id", "Email", ticketItem.statusupdatedbyusersid);
            ViewBag.statusid = new SelectList(db.TicketStatus, "id", "name", ticketItem.statusid);
            return View(ticketItem);
        }

        // GET: TicketItems/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            TicketItem ticketItem = db.TicketItem.Find(id);
            if (ticketItem == null)
            {
                return HttpNotFound();
            }

            List<SelectListItem> projectlist = db.Database.SqlQuery<CombinedEntity>("exec projects_loadcombined").Select(x =>
                new SelectListItem
                {
                    Text = x.name,
                    Value = x.id.ToString()
                }).ToList();
            ViewBag.projectid = new SelectList(projectlist, "Value", "Text", ticketItem.projectid);
            ViewBag.skillid = new SelectList(db.Skill, "id", "name", ticketItem.skillid);
            ViewBag.statusupdatedbyusersid =
                new SelectList(UserManager.Users.OrderBy(u => u.FirstName).Where(us => us.isactive == true).ToList(),
                    "Id", "Email", ticketItem.statusupdatedbyusersid);
            ViewBag.statusid = new SelectList(db.TicketStatus, "id", "name", ticketItem.statusid);
            return View(ticketItem);
        }

        // POST: TicketItems/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(
            [Bind(Include =
                "id,supportconversationid,emailmessageid,bccrecipients,body,conversationid,conversationindex,conversationtopic,datetimecreated,datetimereceived,datetimesent,displaycc,displayto,from,hasattachments,importance,inreplyto,internetmessageheaders,internetmessageid,lastmodifiedname,lastmodifiedtime,mimecontent,replyto,sensitivity,size,subject,torecipients,uniquebody,projectid,skillid,statusid,statusupdatedbyusersid,statusupdatedon,createdonutc,updatedonutc,ipused,userid")]
            TicketItem ticketItem)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ticketItem).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            List<SelectListItem> projectlist = db.Database.SqlQuery<CombinedEntity>("exec projects_loadcombined").Select(x =>
                new SelectListItem
                {
                    Text = x.name,
                    Value = x.id.ToString()
                }).ToList();
            ViewBag.projectid = new SelectList(projectlist, "Value", "Text", ticketItem.projectid);
            ViewBag.skillid = new SelectList(db.Skill, "id", "name", ticketItem.skillid);
            ViewBag.statusupdatedbyusersid =
                new SelectList(UserManager.Users.OrderBy(u => u.FirstName).Where(us => us.isactive == true).ToList(),
                    "Id", "Email", ticketItem.statusupdatedbyusersid);
            ViewBag.statusid = new SelectList(db.TicketStatus, "id", "name", ticketItem.statusid);
            return View(ticketItem);
        }

        // GET: TicketItems/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            TicketItem ticketItem = db.TicketItem.Find(id);
            if (ticketItem == null)
            {
                return HttpNotFound();
            }

            return View(ticketItem);
        }

        // POST: TicketItems/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            TicketItem ticketItem = db.TicketItem.Find(id);
            db.TicketItem.Remove(ticketItem);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult TicketItemAssignment(string id, string status, string projectid, string skillid,
            int quotedtime)
        {
            try
            {
                long pid = 0;
                if (!string.IsNullOrEmpty(projectid))
                {
                    pid = Convert.ToInt64(projectid);
                }

                long sid = 0;
                if (!string.IsNullOrEmpty(skillid))
                {
                    sid = Convert.ToInt64(skillid);
                }

                long tid = 0;
                if (!string.IsNullOrEmpty(id))
                {
                    tid = Convert.ToInt64(id);
                }

                int statusid = 0;
                if (!string.IsNullOrEmpty(status))
                {
                    statusid = Convert.ToInt32(status);
                }

                TicketItem TicketItem = db.TicketItem.Where(t => t.id == tid).FirstOrDefault();

                Ticket ticket = db.Ticket.Where(i => i.id == TicketItem.ticketid).FirstOrDefault();
                if (ticket.statusid == 1)
                {
                    ticket.statusid = 2;
                    ticket.statusupdatedbyusersid = User.Identity.GetUserId();
                    ticket.statusupdatedon = DateTime.Now;
                    ticket.LastActivityDate = DateTime.Now;
                    db.Entry(ticket).State = EntityState.Modified;
                    db.SaveChanges();
                }

                if (TicketItem.statusid == 1 || TicketItem.statusid == 2 || TicketItem.statusid == 3)
                {
                    TicketItem.projectid = pid;
                    TicketItem.skillid = sid;
                }

                TicketItem.statusid = statusid;
                TicketItem.statusupdatedbyusersid = User.Identity.GetUserId();
                TicketItem.updatedonutc = DateTime.Now;
                TicketItem.statusupdatedon = DateTime.Now;
                if (TicketItem.quotedtimeinminutes == 0 || TicketItem.quotedtimeinminutes < 1)
                {
                    TicketItem.quotedtimeinminutes = quotedtime;
                }

                TicketItem.ipused = Request.UserHostAddress;
                TicketItem.userid = User.Identity.GetUserId();
                db.Entry(TicketItem).State = EntityState.Modified;
                db.SaveChanges();

                List<TicketItemLog> ticketitemlog = db.TicketItemLog.Where(t => t.ticketitemid == tid).ToList();
                if (ticketitemlog.Count > 0 && ticketitemlog != null)
                {
                    foreach (TicketItemLog items in ticketitemlog)
                    {
                        if (items.statusid == 1 || items.statusid == 2)
                        {
                            TicketItemLog ticketitem = db.TicketItemLog.Where(t => t.id == items.id).FirstOrDefault();
                            ticketitem.statusid = statusid;
                            ticketitem.assignedbyusersid = User.Identity.GetUserId();
                            ticketitem.assignedon = DateTime.Now;
                            ticketitem.statusupdatedbyusersid = User.Identity.GetUserId();
                            ticketitem.statusupdatedon = DateTime.Now;
                            db.Entry((object)ticketitem).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                }
                else
                {
                    string userid = User.Identity.GetUserId();
                    TicketItemLog ticketitem = new TicketItemLog();
                    long? count = db.TicketItemLog
                        .Where(ti => ti.assignedtousersid == userid && ti.displayorder != null && ti.statusid == 2)
                        .Max(d => d.displayorder);
                    ticketitem.ticketitemid = TicketItem.id;
                    ticketitem.statusid = statusid;
                    ticketitem.displayorder = count != null ? count + 1 : 1;
                    ticketitem.assignedbyusersid = userid;
                    ticketitem.assignedtousersid = userid;
                    ticketitem.assignedon = DateTime.Now;
                    ticketitem.statusupdatedbyusersid = userid;
                    ticketitem.statusupdatedon = DateTime.Now;
                    db.TicketItemLog.Add(ticketitem);
                    db.SaveChanges();
                }

                return Json(new { success = true, successtext = "The Ticket item status has been updated." });
            }
            catch (Exception ex)
            {
                return Json(new { error = true, errortext = ex.Message });
            }
        }

        public ActionResult Startworking(string id, string status, string projectid, string skillid, int quotedtime)
        {
            try

            {
                long pid = 0;
                if (!string.IsNullOrEmpty(projectid))
                {
                    pid = Convert.ToInt64(projectid);
                }

                long sid = 0;
                if (!string.IsNullOrEmpty(skillid))
                {
                    sid = Convert.ToInt64(skillid);
                }

                long tid = 0;
                if (!string.IsNullOrEmpty(id))
                {
                    tid = Convert.ToInt64(id);
                }

                int statusid = 0;
                if (!string.IsNullOrEmpty(status))
                {
                    statusid = Convert.ToInt32(status);
                }

                TicketItem TicketItem = db.TicketItem.Where(t => t.id == tid).FirstOrDefault();

                Ticket ticket = db.Ticket.Where(i => i.id == TicketItem.ticketid).FirstOrDefault();
                if (ticket.statusid == 1)
                {
                    ticket.statusid = 2;
                    ticket.statusupdatedbyusersid = User.Identity.GetUserId();
                    ticket.statusupdatedon = DateTime.Now;
                    db.Entry(ticket).State = EntityState.Modified;
                    db.SaveChanges();
                }

                if (TicketItem.statusid == 1 || TicketItem.statusid == 2 || TicketItem.statusid == 3)
                {
                    TicketItem.projectid = pid;
                    TicketItem.skillid = sid;
                    TicketItem.statusid = 2;
                    TicketItem.statusupdatedbyusersid = User.Identity.GetUserId();
                    TicketItem.statusupdatedon = DateTime.Now;
                }

                if (TicketItem.quotedtimeinminutes == 0 || TicketItem.quotedtimeinminutes < 1)
                {
                    TicketItem.quotedtimeinminutes = quotedtime;
                }

                TicketItem.updatedonutc = DateTime.Now;
                TicketItem.ipused = Request.UserHostAddress;
                TicketItem.userid = User.Identity.GetUserId();
                db.Entry(TicketItem).State = EntityState.Modified;
                db.SaveChanges();
                string userid = User.Identity.GetUserId();
                TicketItemLog ticketitemlog = db.TicketItemLog.Where(i => i.ticketitemid == tid && i.assignedtousersid == userid)
                    .FirstOrDefault();
                long assignmentid = 0;
                if (ticketitemlog == null)
                {
                    ticketitemlog = new TicketItemLog();
                    long? count = db.TicketItemLog
                        .Where(ti => ti.assignedtousersid == userid && ti.displayorder != null && ti.statusid == 2)
                        .Max(d => d.displayorder);
                    ticketitemlog.displayorder = count != null ? count + 1 : 1;
                    ticketitemlog.ticketitemid = TicketItem.id;
                    ticketitemlog.statusid = statusid;
                    ticketitemlog.assignedbyusersid = userid;
                    ticketitemlog.assignedtousersid = userid;
                    ticketitemlog.assignedon = DateTime.Now;
                    ticketitemlog.statusupdatedbyusersid = userid;
                    ticketitemlog.statusupdatedon = DateTime.Now;
                    db.TicketItemLog.Add(ticketitemlog);
                    db.SaveChanges();
                    assignmentid = ticketitemlog.id;
                }
                else
                {
                    return Json(new { success = true, successtext = "The ticket item already assign to you." });
                }

                string username = "";
                ApplicationUser user = UserManager.FindById(userid);
                TicketStatus statusname = db.TicketStatus.Find(2);
                string useritemstatus = "";
                if (statusname != null)
                {
                    useritemstatus = statusname.name;
                }

                string profileimage = "";
                if (user != null)
                {
                    profileimage = user.ProfileImage;
                    username = user.FullName;
                }

                bool hasprofileimage = false;

                if (!string.IsNullOrEmpty(profileimage))
                {
                    hasprofileimage = true;
                }

                return Json(new
                {
                    success = true,
                    successtext = "The ticket item has been assigned to you.",
                    profileimage,
                    username,
                    useritemstatus,
                    hasprofileimage,
                    userid = user.Id,
                    assignmentid,
                    TicketItem.subject,
                    TicketItem.projectid,
                    TicketItem.skillid,
                    TicketItem.id
                });
            }
            catch (Exception ex)
            {
                return Json(new { error = true, errortext = ex.Message });
            }
        }

        public ActionResult MutiUsersAssignment(long pid, long sid, long ticketitemid, string users)
        {
            try
            {
                List<AdminMultipleUsersViewModel> amuvmlist = new List<AdminMultipleUsersViewModel>();
                string[] values = users.Split(';');
                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = values[i].Trim();
                    if (!string.IsNullOrEmpty(values[i]))
                    {
                        string userid = values[i];
                        ApplicationUser user = UserManager.FindById(userid);
                        AdminMultipleUsersViewModel amuvm = new AdminMultipleUsersViewModel
                        {
                            userid = user.Id,
                            username = user.FullName,
                            profileimage = user.ProfileImage
                        };
                        if (string.IsNullOrEmpty(amuvm.profileimage))
                        {
                            amuvm.hasprofileimage = false;
                        }
                        else
                        {
                            amuvm.hasprofileimage = true;
                        }

                        TicketItemLog ticketitemlog = db.TicketItemLog
                            .Where(t => t.ticketitemid == ticketitemid && t.assignedtousersid == userid)
                            .FirstOrDefault();
                        if (ticketitemlog == null)
                        {
                            ticketitemlog = new TicketItemLog();
                            long? count = db.TicketItemLog.Where(ti =>
                                    ti.assignedtousersid == userid && ti.displayorder != null && ti.statusid == 2)
                                .Max(d => d.displayorder);
                            ticketitemlog.displayorder = count != null ? count + 1 : 1;
                            ticketitemlog.statusid = 2;
                            ticketitemlog.assignedon = DateTime.Now;
                            ticketitemlog.assignedtousersid = userid;
                            ticketitemlog.assignedbyusersid = User.Identity.GetUserId();
                            ticketitemlog.statusupdatedbyusersid = User.Identity.GetUserId();
                            ticketitemlog.statusupdatedon = DateTime.Now;
                            ticketitemlog.ticketitemid = ticketitemid;
                            db.TicketItemLog.Add(ticketitemlog);
                            db.SaveChanges();
                            TicketStatus statusname = db.TicketStatus.Find(2);
                            amuvm.assignmentid = ticketitemlog.id;
                            amuvm.useritemstatus = statusname.name;
                            amuvmlist.Add(amuvm);
                        }
                    }
                }

                TicketItem ticketitem = db.TicketItem.Where(i => i.id == ticketitemid).FirstOrDefault();
                if (ticketitem != null && ticketitem.statusid == 1)
                {
                    ticketitem.statusid = 2;
                    ticketitem.projectid = pid;
                    ticketitem.skillid = sid;
                    ticketitem.statusupdatedbyusersid = User.Identity.GetUserId();
                    ticketitem.statusupdatedon = DateTime.Now;
                    ticketitem.updatedonutc = DateTime.Now;
                    ticketitem.ipused = Request.UserHostAddress;
                    db.Entry(ticketitem).State = EntityState.Modified;
                    db.SaveChanges();
                }

                if (ticketitem != null && ticketitem.statusid != 1)
                {
                    ticketitem.updatedonutc = DateTime.Now;
                    db.Entry(ticketitem).State = EntityState.Modified;
                    db.SaveChanges();
                }

                Ticket ticket = db.Ticket.Where(i => i.id == ticketitem.ticketid).FirstOrDefault();
                if (ticket != null && ticket.statusid == 1)
                {
                    ticket.statusid = 2;
                    ticket.statusupdatedbyusersid = User.Identity.GetUserId();
                    ticket.statusupdatedon = DateTime.Now;
                    ticket.LastActivityDate = DateTime.Now;
                    ticket.updatedonutc = DateTime.Now;
                    ticket.ipused = Request.UserHostAddress;
                    db.Entry(ticket).State = EntityState.Modified;
                    db.SaveChanges();
                }

                return Json(new
                { success = true, successtext = "The users have been assigned successfully.", userlist = amuvmlist });
            }
            catch (Exception ex)
            {
                return Json(new { error = true, errortext = ex.Message });
            }
        }


        //assign task to users using Inherit  feature

        public ActionResult InheritTaskAssignment(long ticketitemid)
        {
            try
            {
                long sid = 0;
                long pid = 0;
                List<AdminMultipleUsersViewModel> amuvmlist = new List<AdminMultipleUsersViewModel>();


                string userString = string.Empty;

                long ticketid = db.TicketItem.Where(c => c.id == ticketitemid).Select(c => c.ticketid).FirstOrDefault();
                long tickid = Convert.ToInt64(ticketid);
                long ticketitem_with_InprogressOrCompleteStatus = db.TicketItem
                    .Where(c => c.ticketid == tickid && (c.statusid == 2 || c.statusid == 3)).Select(c => c.id)
                    .FirstOrDefault();

                if (ticketitem_with_InprogressOrCompleteStatus == 0)
                {
                    return Json(new
                    {
                        error = true,
                        errortext =
                            "Sorry, It may be first item of ticket or No assigned user found for that ticket, please use the Plus Sign Button to assign the task."
                    });
                }

                long titemid = Convert.ToInt64(ticketitem_with_InprogressOrCompleteStatus);
                List<TicketItemLog> itemloglist = db.TicketItemLog.Where(c => c.ticketitemid == titemid).ToList();


                sid = Convert.ToInt64(db.TicketItem.Where(c => c.id == titemid).Select(c => c.skillid)
                    .FirstOrDefault());
                pid = Convert.ToInt64(db.TicketItem.Where(c => c.id == titemid).Select(c => c.projectid)
                    .FirstOrDefault());

                if (sid == 0)
                {
                    return Json(new { error = true, errortext = "Sorry, skill not found." });
                }

                if (pid == 0)
                {
                    return Json(new { error = true, errortext = "Sorry, project not found." });
                }

                foreach (TicketItemLog itemlog in itemloglist)
                {
                    userString = string.Concat(userString, itemlog.assignedtousersid) + ',';
                }

                string[] values = userString.Remove(userString.Length - 1).Split(',');


                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = values[i].Trim();
                    if (!string.IsNullOrEmpty(values[i]))
                    {
                        string userid = values[i];
                        ApplicationUser user = UserManager.FindById(userid);
                        AdminMultipleUsersViewModel amuvm = new AdminMultipleUsersViewModel
                        {
                            userid = user.Id,
                            username = user.FullName,
                            profileimage = user.ProfileImage
                        };
                        if (string.IsNullOrEmpty(amuvm.profileimage))
                        {
                            amuvm.hasprofileimage = false;
                        }
                        else
                        {
                            amuvm.hasprofileimage = true;
                        }

                        TicketItemLog ticketitemlog = db.TicketItemLog
                            .Where(t => t.ticketitemid == ticketitemid && t.assignedtousersid == userid)
                            .FirstOrDefault();
                        if (ticketitemlog == null)
                        {
                            ticketitemlog = new TicketItemLog();
                            long? count = db.TicketItemLog.Where(ti =>
                                    ti.assignedtousersid == userid && ti.displayorder != null && ti.statusid == 2)
                                .Max(d => d.displayorder);
                            ticketitemlog.displayorder = count != null ? count + 1 : 1;
                            ticketitemlog.statusid = 2;
                            ticketitemlog.assignedon = DateTime.Now;
                            ticketitemlog.assignedtousersid = userid;
                            ticketitemlog.assignedbyusersid = User.Identity.GetUserId();
                            ticketitemlog.statusupdatedbyusersid = User.Identity.GetUserId();
                            ticketitemlog.statusupdatedon = DateTime.Now;
                            ticketitemlog.ticketitemid = ticketitemid;
                            db.TicketItemLog.Add(ticketitemlog);
                            db.SaveChanges();
                            TicketStatus statusname = db.TicketStatus.Find(2);
                            amuvm.assignmentid = ticketitemlog.id;
                            amuvm.useritemstatus = statusname.name;
                            amuvmlist.Add(amuvm);
                        }
                    }
                }

                TicketItem ticketitem = db.TicketItem.Where(i => i.id == ticketitemid).FirstOrDefault();
                if (ticketitem != null && ticketitem.statusid == 1)
                {
                    ticketitem.statusid = 2;
                    ticketitem.projectid = pid;
                    ticketitem.skillid = sid;
                    ticketitem.statusupdatedbyusersid = User.Identity.GetUserId();
                    ticketitem.statusupdatedon = DateTime.Now;
                    ticketitem.updatedonutc = DateTime.Now;
                    ticketitem.ipused = Request.UserHostAddress;
                    db.Entry(ticketitem).State = EntityState.Modified;
                    db.SaveChanges();
                }

                if (ticketitem != null && ticketitem.statusid != 1)
                {
                    ticketitem.updatedonutc = DateTime.Now;
                    db.Entry(ticketitem).State = EntityState.Modified;
                    db.SaveChanges();
                }

                Ticket ticket = db.Ticket.Where(i => i.id == ticketitem.ticketid).FirstOrDefault();
                if (ticket != null && ticket.statusid == 1)
                {
                    ticket.statusid = 2;
                    ticket.statusupdatedbyusersid = User.Identity.GetUserId();
                    ticket.statusupdatedon = DateTime.Now;
                    ticket.LastActivityDate = DateTime.Now;
                    ticket.updatedonutc = DateTime.Now;
                    ticket.ipused = Request.UserHostAddress;
                    db.Entry(ticket).State = EntityState.Modified;
                    db.SaveChanges();
                }

                return Json(new
                { success = true, successtext = "The users have been assigned successfully.", userlist = amuvmlist });
            }
            catch (Exception ex)
            {
                return Json(new { error = true, errortext = ex.Message });
            }
        }

        public ActionResult LoadConversation(long id)
        {
            try
            {
                List<TicketItem> ti = db.TicketItem.Where(t => t.ticketid == id).OrderByDescending(c => c.createdonutc).ToList();
                return PartialView("~/Views/Tickets/_LoadMessages.cshtml", ti);
            }
            catch
            {
                return PartialView("~/Views/Shared/_error.cshtml");
            }
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }

            base.Dispose(disposing);
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
            TicketItem item = db.TicketItem.Where(i => i.id == itemid).FirstOrDefault();
            Ticket ticket = db.Ticket.Where(i => i.id == item.ticketid).FirstOrDefault();
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

            return true;
        }

        private bool ChangeTicketItemStatus(int statusid, long emailid)
        {
            try
            {
                TicketItem TicketItem = db.TicketItem.Where(t => t.id == emailid).FirstOrDefault();

                Ticket ticket = db.Ticket.Where(i => i.id == TicketItem.ticketid).FirstOrDefault();
                if (ticket.statusid == 1)
                {
                    ticket.statusid = 2;
                    ticket.statusupdatedbyusersid = User.Identity.GetUserId();
                    ticket.statusupdatedon = DateTime.Now;
                    ticket.LastActivityDate = DateTime.Now;
                    db.Entry(ticket).State = EntityState.Modified;
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
                List<TicketItemStatusChnageViewModel> tiscvm = new List<TicketItemStatusChnageViewModel>();
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
                            TicketItemStatusChnageViewModel tis = new TicketItemStatusChnageViewModel
                            {
                                userid = ticketitem.assignedtousersid,
                                assignmentid = ticketitem.id
                            };
                            TicketStatus statusname = db.TicketStatus.Find(statusid);
                            tis.statusname = statusname.name;
                            tis.statusid = statusname.id;
                            tiscvm.Add(tis);
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

        [HttpPost]
        public ActionResult ChangeProject(long ticketitemid, long projectid)
        {
            TicketItem ticekitem = db.TicketItem.Where(i => i.id == ticketitemid).FirstOrDefault();
            if (ticekitem != null)
            {
                ticekitem.projectid = projectid;
                ticekitem.updatedonutc = DateTime.Now;
                ticekitem.ipused = Request.UserHostAddress;
                ticekitem.userid = User.Identity.GetUserId();
                db.Entry(ticekitem).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = true });
            }

            return Json(new { error = true });
        }

        [HttpPost]
        public ActionResult Changeskill(long ticketitemid, long skillid)
        {
            TicketItem ticekitem = db.TicketItem.Where(i => i.id == ticketitemid).FirstOrDefault();
            if (ticekitem != null)
            {
                ticekitem.skillid = skillid;
                ticekitem.updatedonutc = DateTime.Now;
                ticekitem.ipused = Request.UserHostAddress;
                ticekitem.userid = User.Identity.GetUserId();
                db.Entry(ticekitem).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = true });
            }

            return Json(new { error = true });
        }

        #region MyTasks-Tab

        //public ActionResult MyTasks(long id)
        //{
        //    /****************************************************
        //    * redirect to login page.
        //    ****************************************************/
        //    // If user is not logged in, 
        //    if (!IsLoggedIn()) return RedirectToAction("login", "account");

        //    // If no id is provided.
        //    if (id < 0) return RedirectToAction("index", "home");

        //    /****************************************************
        //     * If user is logged in, get user's guid.
        //     ****************************************************/
        //    string userid = User.Identity.GetUserId();

        //    /****************************************************
        //     * Prepare TicketStatusAction instance.
        //     ****************************************************/
        //    var TicketStatuses = db.ConversationStatus.ToList();

        //    TicketStatusAction ticketStatusAction = new TicketStatusAction();
        //    foreach (ConversationStatus status in TicketStatuses)
        //    {
        //        switch (status.name.ToLower())
        //        {
        //            case "new task":
        //                break;
        //            case "in progress":
        //                break;
        //            case "closed":
        //                status.name = "Close";
        //                ticketStatusAction.CloseTicketStatus = status;
        //                break;
        //            default:
        //                if (ticketStatusAction.OtherTicketStatusCollection == null) ticketStatusAction.OtherTicketStatusCollection = new List<ConversationStatus>();
        //                ticketStatusAction.OtherTicketStatusCollection.Add(status);
        //                break;
        //        }
        //    }

        //    /****************************************************
        //     * Prepare TicketItemStatusAction instance.
        //     ****************************************************/
        //    var TicketItemStatuses = db.TicketStatus.ToList();

        //    TicketItemStatusAction ticketItemStatusAction = new TicketItemStatusAction();
        //    foreach (TicketStatus status in TicketItemStatuses)
        //    {
        //        switch (status.name.ToLower())
        //        {
        //            case "not assigned":
        //                break;
        //            case "in progress":
        //                break;
        //            case "closed":
        //                status.name = "Close";
        //                ticketItemStatusAction.CloseTicketStatus = status;
        //                break;
        //            default:
        //                if (ticketItemStatusAction.OtherTicketStatusCollection == null) ticketItemStatusAction.OtherTicketStatusCollection = new List<TicketStatus>();
        //                ticketItemStatusAction.OtherTicketStatusCollection.Add(status);
        //                break;
        //        }
        //    }

        //    /****************************************************
        //     * Get User Tickets All/Based on status.
        //     ****************************************************/
        //    List<TicketItem> ticketItem = new List<TicketItem>();

        //    if (id == 0)
        //    {

        //        ticketItem = (from task in db.TicketItem
        //                      join tl in db.TicketItemLog on task.id equals tl.ticketitemid
        //                      where tl.assignedtousersid == userid
        //                      orderby tl.displayorder descending, tl.assignedon descending
        //                      select task).Include(t => t.Ticket).Include(t => t.TicketItemLog).Take(recordsPerPage).ToList();
        //    }
        //    else
        //    {
        //        ticketItem = (from task in db.TicketItem
        //                      join tl in db.TicketItemLog on task.id equals tl.ticketitemid
        //                      where tl.statusid == id && tl.assignedtousersid == userid
        //                      orderby tl.displayorder descending, tl.assignedon descending
        //                      select task).Include(t => t.Ticket).Include(t => t.TicketItemLog).Take(recordsPerPage).ToList();
        //    }

        //    /****************************************************
        //     * Load Combined list of all the porjects.
        //     ****************************************************/
        //    var projectlist = db.Database.SqlQuery<CombinedEntity>("exec projects_loadcombined").Select(x => new SelectListItem
        //    {
        //        Text = x.name,
        //        Value = x.id.ToString(),
        //    }).ToList();

        //    /****************************************************
        //     * Prepare time spent in minutes values from dropdown.
        //     ****************************************************/
        //    List<SelectListItem> timespentinminutes = new List<SelectListItem>();
        //    int timespentinminute = 0;
        //    int Counter = 1;
        //    for (Counter = 1; Counter <= 120; Counter++)
        //    {
        //        timespentinminute = timespentinminute + 5;
        //        timespentinminutes.Add(new SelectListItem { Text = timespentinminute.ToString(), Value = timespentinminute.ToString() });
        //    }

        //    /****************************************************
        //     * Fetch all users that are currently active in system.
        //     ****************************************************/
        //    List<ApplicationUser> ActiveUsers = UserManager.Users.OrderBy(u => u.FirstName).Where(us => us.isactive == true).ToList();

        //    /****************************************************
        //     * Fetch all statuses with count of tickets.
        //     ****************************************************/
        //    var MyStatus = db.Database.SqlQuery<TicketStatusViewModel>("exec ticketstatus_loadtaskscount @userid", new System.Data.SqlClient.SqlParameter("@userid", userid)).ToList();

        //    // Add All Tasks entry manually.
        //    TicketStatusViewModel tsvm = new TicketStatusViewModel();

        //    int totaltask = 0;
        //    foreach (var status in MyStatus)
        //    {
        //        totaltask += status.ticketcount;
        //    }
        //    tsvm.id = 0;
        //    tsvm.isactive = true;
        //    tsvm.name = "All Tasks";
        //    tsvm.ticketcount = totaltask;
        //    MyStatus.Add(tsvm);

        //    /**************************************************************
        //     * Fetch all projects that were ever assigned to current user.
        //     **************************************************************/
        //    var MyClients = db.Database.SqlQuery<UserClientViewModel>("exec projects_loadclientsbyuser @userid", new System.Data.SqlClient.SqlParameter("@userid", userid)).ToList();

        //    /****************************************************
        //     * Get All Skills
        //     ****************************************************/
        //    var SkillList = db.Skill.ToList();

        //    /****************************************************
        //     * Prepare ViewBags.
        //     ****************************************************/
        //    ViewBag.ticketstatuslist = TicketItemStatuses;
        //    ViewBag.conversationsatus = TicketStatuses;
        //    ViewBag.currentuserid = userid;
        //    ViewBag.projects = projectlist;
        //    ViewBag.skills = SkillList;
        //    ViewBag.projectid_addbucket = projectlist;
        //    ViewBag.skillid_addbicket = SkillList;
        //    ViewBag.ticketstatusaction = ticketStatusAction;
        //    ViewBag.ticketitemstatusaction = ticketItemStatusAction;
        //    ViewBag.timespentinminutes = timespentinminutes;
        //    ViewBag.billabletimeinminutes = timespentinminutes;
        //    ViewBag.users = ActiveUsers;
        //    ViewBag.statusid = id;
        //    ViewBag.MyTicketStatus = MyStatus.OrderBy(i => i.id);
        //    ViewBag.currentSubTab = GetCurrentActiveSubTab();
        //    ViewBag.MyClients = MyClients;
        //    return View(ticketItem.ToList());
        //}

        public ActionResult MyTasks(long id, long? clientid = 0)
        {
            //return RedirectToAction("mytickets", "tickets", new { id = 2 });
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
             * Prepare TicketStatusAction instance.
             ****************************************************/
            List<ConversationStatus> TicketStatuses = db.ConversationStatus.ToList();

            TicketStatusAction ticketStatusAction = new TicketStatusAction();
            foreach (ConversationStatus status in TicketStatuses)
            {
                switch (status.name.ToLower())
                {
                    case "new task":
                        break;
                    case "in progress":
                        break;
                    case "closed":
                        status.name = "Close";
                        ticketStatusAction.CloseTicketStatus = status;
                        break;
                    default:
                        if (ticketStatusAction.OtherTicketStatusCollection == null)
                        {
                            ticketStatusAction.OtherTicketStatusCollection = new List<ConversationStatus>();
                        }

                        ticketStatusAction.OtherTicketStatusCollection.Add(status);
                        break;
                }
            }

            /****************************************************
             * Prepare TicketItemStatusAction instance.
             ****************************************************/
            List<TicketStatus> TicketItemStatuses = db.TicketStatus.ToList();

            TicketItemStatusAction ticketItemStatusAction = new TicketItemStatusAction();
            foreach (TicketStatus status in TicketItemStatuses)
            {
                switch (status.name.ToLower())
                {
                    case "not assigned":
                        break;
                    case "in progress":
                        break;
                    case "closed":
                        status.name = "Close";
                        ticketItemStatusAction.CloseTicketStatus = status;
                        break;
                    default:
                        if (ticketItemStatusAction.OtherTicketStatusCollection == null)
                        {
                            ticketItemStatusAction.OtherTicketStatusCollection = new List<TicketStatus>();
                        }

                        ticketItemStatusAction.OtherTicketStatusCollection.Add(status);
                        break;
                }
            }

            /****************************************************
             * Get User Tickets All/Based on status.
             ****************************************************/
            List<TicketItem> ticketItem = new List<TicketItem>();

            if (id == 0 && clientid == 0)
            {
                ticketItem = (from task in db.TicketItem
                              join tl in db.TicketItemLog on task.id equals tl.ticketitemid
                              where tl.assignedtousersid == userid
                              orderby tl.displayorder descending, tl.assignedon descending
                              select task).Include(t => t.Ticket).Include(t => t.TicketItemLog).Take(recordsPerPage).ToList();
            }
            else if (id == 0 && clientid > 0)
            {
                ticketItem = (from task in db.TicketItem
                              join tl in db.TicketItemLog on task.id equals tl.ticketitemid
                              join pr in db.Project on task.projectid equals pr.id
                              where tl.assignedtousersid == userid && pr.clientid == clientid
                              orderby tl.displayorder descending, tl.assignedon descending
                              select task).Include(t => t.Ticket).Include(t => t.TicketItemLog).Take(recordsPerPage).ToList();
            }
            else if (id > 0 && clientid == 0)
            {
                ticketItem = (from task in db.TicketItem
                              join tl in db.TicketItemLog on task.id equals tl.ticketitemid
                              where tl.statusid == id && tl.assignedtousersid == userid
                              orderby tl.displayorder descending, tl.assignedon descending
                              select task).Include(t => t.Ticket).Include(t => t.TicketItemLog).Take(recordsPerPage).ToList();
            }
            else if (id > 0 && clientid > 0)
            {
                ticketItem = (from task in db.TicketItem
                              join tl in db.TicketItemLog on task.id equals tl.ticketitemid
                              join pr in db.Project on task.projectid equals pr.id
                              where tl.statusid == id && tl.assignedtousersid == userid && pr.clientid == clientid
                              orderby tl.displayorder descending, tl.assignedon descending
                              select task).Include(t => t.Ticket).Include(t => t.TicketItemLog).Take(recordsPerPage).ToList();
            }

            /****************************************************
             * Load Combined list of all the porjects.
             ****************************************************/
            List<SelectListItem> projectlist = db.Database.SqlQuery<CombinedEntity>("exec projects_loadcombined").Select(x =>
                new SelectListItem
                {
                    Text = x.name,
                    Value = x.id.ToString()
                }).ToList();

            /****************************************************
             * Prepare time spent in minutes values from dropdown.
             ****************************************************/
            List<SelectListItem> timespentinminutes = new List<SelectListItem>();
            int timespentinminute = 0;
            int Counter = 1;
            for (Counter = 1; Counter <= 120; Counter++)
            {
                timespentinminute = timespentinminute + 5;
                timespentinminutes.Add(new SelectListItem
                { Text = timespentinminute.ToString(), Value = timespentinminute.ToString() });
            }

            /****************************************************
             * Fetch all users that are currently active in system.
             ****************************************************/
            List<ApplicationUser> ActiveUsers = UserManager.Users.OrderBy(u => u.FirstName).Where(us => us.isactive == true).ToList();

            /****************************************************
             * Fetch all statuses with count of tickets.
             ****************************************************/
            List<TicketStatusViewModel> MyStatus = db.Database.SqlQuery<TicketStatusViewModel>("exec ticketstatus_loadtaskscount @userid",
                new SqlParameter("@userid", userid)).ToList();

            // Add All Tasks entry manually.
            TicketStatusViewModel tsvm = new TicketStatusViewModel();

            int totaltask = 0;
            foreach (TicketStatusViewModel status in MyStatus)
            {
                totaltask += status.ticketcount;
            }

            tsvm.id = 0;
            tsvm.isactive = true;
            tsvm.name = "All Tasks";
            tsvm.ticketcount = totaltask;
            MyStatus.Add(tsvm);

            /**************************************************************
             * Fetch all projects that were ever assigned to current user.
             **************************************************************/
            List<UserClientViewModel> MyClients = db.Database.SqlQuery<UserClientViewModel>("exec projects_loadclientsbyuser @userid",
                new SqlParameter("@userid", userid)).ToList();

            UserClientViewModel allClientsVM = new UserClientViewModel
            {
                clientid = 0,
                clientname = "All Clients",
                userfavouriteid = -1,
                ispinned = false
            };
            MyClients.Insert(0, allClientsVM);

            /****************************************************
             * Get All Skills
             ****************************************************/
            List<Skill> SkillList = db.Skill.ToList();

            /****************************************************
             * Prepare ViewBags.
             ****************************************************/
            ViewBag.ticketstatuslist = TicketItemStatuses;
            ViewBag.conversationsatus = TicketStatuses;
            ViewBag.currentuserid = userid;
            ViewBag.projects = projectlist;
            ViewBag.skills = SkillList;
            ViewBag.projectid_addbucket = projectlist;
            ViewBag.skillid_addbicket = SkillList;
            ViewBag.ticketstatusaction = ticketStatusAction;
            ViewBag.ticketitemstatusaction = ticketItemStatusAction;
            ViewBag.timespentinminutes = timespentinminutes;
            ViewBag.billabletimeinminutes = timespentinminutes;
            ViewBag.users = ActiveUsers;
            ViewBag.statusid = id;
            ViewBag.MyTicketStatus = MyStatus.OrderBy(i => i.id);
            ViewBag.currentSubTab = GetCurrentActiveSubTab();
            ViewBag.MyClients = MyClients;

            // User Selections
            ViewBag.CurrentStatusId = id;
            ViewBag.CurrentClientId = clientid;

            // Return View.
            return View(ticketItem.ToList());
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult MyTaskAjax(int id, int? pagenum, string topic)
        {
            TicketStatusAction ticketStatusAction = new TicketStatusAction();
            List<ConversationStatus> TicketStatuses = db.ConversationStatus.ToList();
            foreach (ConversationStatus status in TicketStatuses)
            {
                switch (status.name.ToLower())
                {
                    case "new task":
                        ticketStatusAction.CloseTicketStatus = status;
                        break;
                    case "in progress":
                        ticketStatusAction.CloseTicketStatus = status;
                        break;
                    case "closed":
                        status.name = "Close";
                        ticketStatusAction.CloseTicketStatus = status;
                        break;
                    default:
                        if (ticketStatusAction.OtherTicketStatusCollection == null)
                        {
                            ticketStatusAction.OtherTicketStatusCollection = new List<ConversationStatus>();
                        }

                        ticketStatusAction.OtherTicketStatusCollection.Add(status);
                        break;
                }
            }

            TicketItemStatusAction ticketItemStatusAction = new TicketItemStatusAction();
            List<TicketStatus> TicketItemStatuses = db.TicketStatus.ToList();
            foreach (TicketStatus status in TicketItemStatuses)
            {
                switch (status.name.ToLower())
                {
                    case "not assigned":
                        ticketItemStatusAction.CloseTicketStatus = status;
                        break;
                    case "in progress":
                        ticketItemStatusAction.CloseTicketStatus = status;
                        break;
                    case "closed":
                        status.name = "Close";
                        ticketItemStatusAction.CloseTicketStatus = status;
                        break;
                    default:
                        if (ticketItemStatusAction.OtherTicketStatusCollection == null)
                        {
                            ticketItemStatusAction.OtherTicketStatusCollection = new List<TicketStatus>();
                        }

                        ticketItemStatusAction.OtherTicketStatusCollection.Add(status);
                        break;
                }
            }

            string userid = User.Identity.GetUserId();
            int page = pagenum ?? 0;

            if (Request.IsAjaxRequest())
            {
                ViewBag.conversationsatus = db.ConversationStatus.ToList();
                ViewBag.ticketstatusaction = ticketStatusAction;
                ViewBag.ticketitemstatusaction = ticketItemStatusAction;
                List<TicketItem> items = GetPaginatedTasks(id, page, topic);
                int totalcount = 0;
                if (!string.IsNullOrEmpty(topic))
                {
                    if (id == 0)
                    {
                        totalcount = (from task in db.TicketItem
                                      join tl in db.TicketItemLog on task.id equals tl.ticketitemid
                                      where tl.assignedtousersid == userid && (task.subject.Contains(topic) ||
                                                                               task.@from.Contains(topic) ||
                                                                               task.lastmodifiedname.Contains(topic))
                                      orderby tl.displayorder descending
                                      orderby tl.assignedon descending
                                      select task).Count();
                    }
                    else
                    {
                        totalcount = (from task in db.TicketItem
                                      join tl in db.TicketItemLog on task.id equals tl.ticketitemid
                                      where tl.statusid == id && tl.assignedtousersid == userid &&
                                            (task.subject.Contains(topic) || task.@from.Contains(topic) ||
                                             task.lastmodifiedname.Contains(topic))
                                      orderby tl.displayorder descending
                                      orderby tl.assignedon descending
                                      select task).Count();
                    }
                }
                else
                {
                    if (id == 0)
                    {
                        totalcount = (from task in db.TicketItem
                                      join tl in db.TicketItemLog on task.id equals tl.ticketitemid
                                      where tl.assignedtousersid == userid
                                      orderby tl.displayorder descending
                                      orderby tl.assignedon descending
                                      select task).Count();
                    }
                    else
                    {
                        totalcount = (from task in db.TicketItem
                                      join tl in db.TicketItemLog on task.id equals tl.ticketitemid
                                      where tl.statusid == id && tl.assignedtousersid == userid
                                      orderby tl.displayorder descending
                                      orderby tl.assignedon descending
                                      select task).Count();
                    }
                }

                string ticketitems = PartialView("_mytask", items).RenderToString();
                return Json(new { ticketitems, totalcount, itemcount = items.Count() }, JsonRequestBehavior.AllowGet);
            }

            ViewBag.status = db.TicketStatus;
            List<TicketItem> ticketItem = (from task in db.TicketItem
                                           join tl in db.TicketItemLog on task.id equals tl.ticketitemid
                                           where tl.statusid == id && tl.assignedtousersid == userid
                                           orderby tl.assignedon descending
                                           select task).Include(t => t.Ticket).Include(t => t.TicketItemLog).Take(recordsPerPage).ToList();
            ViewBag.ticketstatuslist = db.TicketStatus.ToList();
            ViewBag.currentuserid = User.Identity.GetUserId();
            List<SelectListItem> projectlist = db.Database.SqlQuery<CombinedEntity>("exec projects_loadcombined").Select(x =>
                new SelectListItem
                {
                    Text = x.name,
                    Value = x.id.ToString()
                }).ToList();
            ViewBag.projects = projectlist;
            ViewBag.skills = db.Skill.ToList();
            List<SelectListItem> timespentinminutes = new List<SelectListItem>();
            int timespentinminute = 0;
            int Counter = 1;
            for (Counter = 1; Counter <= 120; Counter++)
            {
                timespentinminute = timespentinminute + 5;
                timespentinminutes.Add(new SelectListItem
                { Text = timespentinminute.ToString(), Value = timespentinminute.ToString() });
            }

            ViewBag.timespentinminutes = timespentinminutes;
            ViewBag.billabletimeinminutes = timespentinminutes;
            ViewBag.users = UserManager.Users.OrderBy(u => u.FirstName).Where(us => us.isactive == true).ToList();
            System.Data.Entity.Infrastructure.DbRawSqlQuery<TicketStatusViewModel> MyStatus = db.Database.SqlQuery<TicketStatusViewModel>("exec ticketstatus_loadtaskscount @userid",
                new SqlParameter("@userid", userid));
            ViewBag.MyTicketStatus = MyStatus;
            return View("Index", ticketItem);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult MyTasksAsync(int id, long? clientid = 0, int? pagenum = 0, string topic = "")
        {
            /****************************************************
            * Get Paginated Tickets.
            ****************************************************/
            int pagenumber = pagenum ?? 0;
            long clientidval = clientid ?? 0;

            if (Request.IsAjaxRequest())
            {
                string userid = User.Identity.GetUserId();
                List<TicketItem> mytasks = FetchPaginatedMyTasks(id, clientidval, pagenumber, topic);

                /****************************************************
                 * Prepare TicketItemStatusAction instance.
                 ****************************************************/
                List<TicketStatus> TicketItemStatuses = db.TicketStatus.ToList();

                TicketItemStatusAction ticketItemStatusAction = new TicketItemStatusAction();
                foreach (TicketStatus status in TicketItemStatuses)
                {
                    switch (status.name.ToLower())
                    {
                        case "not assigned":
                            break;
                        case "in progress":
                            break;
                        case "closed":
                            status.name = "Close";
                            ticketItemStatusAction.CloseTicketStatus = status;
                            break;
                        default:
                            if (ticketItemStatusAction.OtherTicketStatusCollection == null)
                            {
                                ticketItemStatusAction.OtherTicketStatusCollection = new List<TicketStatus>();
                            }

                            ticketItemStatusAction.OtherTicketStatusCollection.Add(status);
                            break;
                    }
                }

                /****************************************************
                 * Prepare ViewBags.
                 ****************************************************/
                ViewBag.currentuserid = userid;
                ViewBag.ticketitemstatusaction = ticketItemStatusAction;

                string ticketitems = PartialView("_mytask", mytasks).RenderToString();
                return Json(new { ticketitems, totalcount = mytasks.Count, itemcount = mytasks.Count },
                    JsonRequestBehavior.AllowGet);
            }

            return Json(new { ticketitems = "", totalcount = 0, itemcount = 0 }, JsonRequestBehavior.AllowGet);
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

        private List<TicketItem> GetPaginatedTasks(int id, int page, string topic)
        {
            string userid = User.Identity.GetUserId();
            int skipRecords = page * recordsPerPage;
            IQueryable<TicketItem> ticketItem = null;
            if (id == 0)
            {
                ticketItem = (from task in db.TicketItem
                              join tl in db.TicketItemLog on task.id equals tl.ticketitemid
                              where tl.assignedtousersid == userid
                              orderby tl.displayorder descending
                              orderby tl.assignedon descending
                              select task).Include(t => t.Ticket).Include(t => t.TicketItemLog);
            }
            else
            {
                ticketItem = (from task in db.TicketItem
                              join tl in db.TicketItemLog on task.id equals tl.ticketitemid
                              where tl.statusid == id && tl.assignedtousersid == userid
                              orderby tl.displayorder descending
                              orderby tl.assignedon descending
                              select task).Include(t => t.Ticket).Include(t => t.TicketItemLog);
            }

            if (!string.IsNullOrEmpty(topic))
            {
                ticketItem = ticketItem.Where(t =>
                    t.subject.Contains(topic) || t.from.Contains(topic) || t.lastmodifiedname.Contains(topic));
            }

            ViewBag.status = db.TicketStatus;
            ViewBag.ticketstatuslist = db.TicketStatus.ToList();
            ViewBag.currentuserid = User.Identity.GetUserId();
            List<SelectListItem> projectlist = db.Database.SqlQuery<CombinedEntity>("exec projects_loadcombined").Select(x =>
                new SelectListItem
                {
                    Text = x.name,
                    Value = x.id.ToString()
                }).ToList();
            ViewBag.projects = projectlist;
            ViewBag.skills = db.Skill.ToList();
            List<SelectListItem> timespentinminutes = new List<SelectListItem>();
            int timespentinminute = 0;
            int Counter = 1;
            for (Counter = 1; Counter <= 120; Counter++)
            {
                timespentinminute = timespentinminute + 5;
                timespentinminutes.Add(new SelectListItem
                { Text = timespentinminute.ToString(), Value = timespentinminute.ToString() });
            }

            ViewBag.timespentinminutes = timespentinminutes;
            ViewBag.billabletimeinminutes = timespentinminutes;
            ViewBag.users = UserManager.Users.OrderBy(u => u.FirstName).Where(us => us.isactive == true).ToList();
            return ticketItem.Skip(skipRecords).Take(recordsPerPage).ToList();
        }

        private List<TicketItem> FetchPaginatedMyTasks(int id, long clientid, int page, string topic)
        {
            // Calculate records to skip.
            int skipRecords = page * recordsPerPage;

            // Start writing the query.
            IQueryable<TicketItem> tasks = null;
            string userid = User.Identity.GetUserId();

            if (id == 0 && clientid == 0)
            {
                tasks = (from task in db.TicketItem
                         join tl in db.TicketItemLog on task.id equals tl.ticketitemid
                         where tl.assignedtousersid == userid
                         orderby tl.displayorder descending, tl.assignedon descending
                         select task).Include(t => t.Ticket).Include(t => t.TicketItemLog);
            }
            else if (id == 0 && clientid > 0)
            {
                tasks = (from task in db.TicketItem
                         join tl in db.TicketItemLog on task.id equals tl.ticketitemid
                         join pr in db.Project on task.projectid equals pr.id
                         where tl.assignedtousersid == userid && pr.clientid == clientid
                         orderby tl.displayorder descending, tl.assignedon descending
                         select task).Include(t => t.Ticket).Include(t => t.TicketItemLog);
            }
            else if (id > 0 && clientid == 0)
            {
                tasks = (from task in db.TicketItem
                         join tl in db.TicketItemLog on task.id equals tl.ticketitemid
                         where tl.statusid == id && tl.assignedtousersid == userid
                         orderby tl.displayorder descending, tl.assignedon descending
                         select task).Include(t => t.Ticket).Include(t => t.TicketItemLog);
            }
            else if (id > 0 && clientid > 0)
            {
                tasks = (from task in db.TicketItem
                         join tl in db.TicketItemLog on task.id equals tl.ticketitemid
                         join pr in db.Project on task.projectid equals pr.id
                         where tl.statusid == id && tl.assignedtousersid == userid && pr.clientid == clientid
                         orderby tl.displayorder descending, tl.assignedon descending
                         select task).Include(t => t.Ticket).Include(t => t.TicketItemLog);
            }

            if (!string.IsNullOrEmpty(topic))
            {
                tasks = tasks.Where(t =>
                    t.subject.Contains(topic) || t.from.Contains(topic) || t.lastmodifiedname.Contains(topic));
            }

            return tasks.Skip(skipRecords).Take(recordsPerPage).ToList();
        }

        #endregion

        #region Change Email Status Methods

        public ActionResult ChangeEmailStatus(string id, string status)
        {
            try
            {
                long tid = 0;
                if (!string.IsNullOrEmpty(id))
                {
                    tid = Convert.ToInt64(id);
                }

                int statusid = 0;
                if (!string.IsNullOrEmpty(status))
                {
                    statusid = Convert.ToInt32(status);
                }

                TicketItem TicketItem = db.TicketItem.Where(t => t.id == tid).FirstOrDefault();

                Ticket ticket = db.Ticket.Where(i => i.id == TicketItem.ticketid).FirstOrDefault();
                if (ticket.statusid == 1)
                {
                    ticket.statusid = 2;
                    ticket.statusupdatedbyusersid = User.Identity.GetUserId();
                    ticket.statusupdatedon = DateTime.Now;
                    ticket.LastActivityDate = DateTime.Now;
                    db.Entry(ticket).State = EntityState.Modified;
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

                List<TicketItemLog> ticketitemlog = db.TicketItemLog.Where(t => t.ticketitemid == tid).ToList();
                List<TicketItemStatusChnageViewModel> tiscvm = new List<TicketItemStatusChnageViewModel>();
                if (ticketitemlog.Count > 0 && ticketitemlog != null)
                {
                    foreach (TicketItemLog items in ticketitemlog)
                    {
                        if (items.statusid == 1 || items.statusid == 2)
                        {
                            TicketItemLog ticketitem = db.TicketItemLog.Where(t => t.id == items.id).FirstOrDefault();
                            ticketitem.statusid = statusid;
                            ticketitem.assignedbyusersid = User.Identity.GetUserId();
                            ticketitem.statusupdatedbyusersid = User.Identity.GetUserId();
                            ticketitem.statusupdatedon = DateTime.Now;
                            db.Entry((object)ticketitem).State = EntityState.Modified;
                            db.SaveChanges();
                            TicketItemStatusChnageViewModel tis = new TicketItemStatusChnageViewModel
                            {
                                userid = ticketitem.assignedtousersid,
                                assignmentid = ticketitem.id
                            };
                            TicketStatus statusname = db.TicketStatus.Find(statusid);
                            tis.statusname = statusname.name;
                            tis.statusid = statusname.id;
                            tiscvm.Add(tis);
                        }
                    }
                }

                return Json(new
                { success = true, successtext = "The Ticket item status has been updated.", updatedusers = tiscvm });
            }
            catch (Exception ex)
            {
                return Json(new { error = true, errortext = ex.Message });
            }
        }

        public ActionResult ChangeUserStatus(long ticketitemid, int statusid)
        {
            try
            {
                // Fetch and verify if ticket is available.
                ChangeUserTaskStatus(statusid, ticketitemid);
                TicketStatus userstatus = db.TicketStatus.Find(statusid);
                string statusname = userstatus.name;
                return Json(new { success = true, Successtext = "The user status has been updated.", statusname },
                    JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = true, errortext = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        //TODO: delete these methods after verifying it is not used anywhere.
        public ActionResult TicketItemstatusupdate(string id, string status)
        {
            try
            {
                long tid = 0;
                if (!string.IsNullOrEmpty(id))
                {
                    tid = Convert.ToInt64(id);
                }

                int statusid = 0;
                if (!string.IsNullOrEmpty(status))
                {
                    statusid = Convert.ToInt32(status);
                }

                bool isdone = ChangeTicketItemStatus(statusid, tid);
                if (isdone)
                {
                    return Json(new { success = true, successtext = "The Ticket item status has been updated." });
                }

                return Json(new { error = true, errortext = "Sorry the Ticket item status could not updated." });
            }
            catch (Exception ex)
            {
                return Json(new { error = true, errortext = ex.Message });
            }
        }

        //All Task for particlur date for admin
        public ActionResult AllTasksList()
        {
            DateTime fromdate = DateTime.Now.Add(TimeSpan.Parse("00:00:00")).AddDays(-7);
            DateTime midnight = fromdate.Subtract(fromdate.TimeOfDay);
            fromdate = midnight;
            DateTime todate = DateTime.Now.Add(TimeSpan.Parse("23:59:59"));
            midnight = todate.Subtract(todate.TimeOfDay);
            todate = midnight;
            List<AllTaskViewModel> log = (from task in db.TicketItemLog
                                          join ti in db.TicketItem on task.ticketitemid equals ti.id
                                          where (task.statusid == 2 || task.statusid == 3 || task.statusid == 4) && task.assignedon >= fromdate &&
                                                task.assignedon <= todate
                                          orderby task.user.UserName, task.user.FirstName
                                          select new AllTaskViewModel
                                          {
                                              FirstName = task.user.FirstName,
                                              LastName = task.user.LastName,
                                              assignedon = task.assignedon,
                                              id = ti.id,
                                              subject = ti.subject,
                                              uniquebody = ti.uniquebody,
                                              status = task.TicketStatus.name
                                          }).ToList();
            List<ClientsListViewModel> clients = db.Database.SqlQuery<ClientsListViewModel>("exec clients_loadforlist").ToList();
            ViewBag.clientid = new SelectList(clients.OrderBy(n => n.name).ToList(), "id", "name").ToList();
            ViewBag.projectid = db.Database.SqlQuery<CombinedEntity>("exec projects_loadcombined").Select(x =>
                new SelectListItem
                {
                    Text = x.name,
                    Value = x.id.ToString()
                }).ToList();
            return View(log);
        }

        //All Task for particlur date for admin
        [HttpGet]
        public ActionResult AllTasksListAjax(DateTime fromdate, DateTime todate, long? clientid, long? projectid)
        {
            fromdate = fromdate.Add(TimeSpan.Parse("00:00:00"));
            todate = todate.Add(TimeSpan.Parse("23:59:59"));
            List<AllTaskViewModel> atvm = new List<AllTaskViewModel>();
            if (clientid != 0 && projectid == 0)
            {
                Client client = db.Client.Include(s => s.SubClients).Where(i => i.id == clientid.Value).FirstOrDefault();
                List<Project> projectlist = new List<Project>();
                if (client != null)
                {
                    List<Project> projects = db.Project.Where(i => i.clientid == client.id).ToList();
                    projectlist.AddRange(projects);
                    if (client.SubClients != null && client.SubClients.Count > 0)
                    {
                        foreach (Client item in client.SubClients)
                        {
                            List<Project> subclient_projects = db.Project.Where(i => i.clientid == item.id).ToList();
                            projectlist.AddRange(subclient_projects);
                        }
                    }
                }

                if (projectlist != null && projectlist.Count > 0)
                {
                    foreach (Project items in projectlist)
                    {
                        List<AllTaskViewModel> tasklist = (from task in db.TicketItemLog
                                                           join ti in db.TicketItem on task.ticketitemid equals ti.id
                                                           where (task.statusid == 2 || task.statusid == 3 || task.statusid == 4) &&
                                                                 task.assignedon >= fromdate && task.assignedon <= todate && ti.projectid == items.id
                                                           orderby task.user.UserName, task.user.FirstName
                                                           select new AllTaskViewModel
                                                           {
                                                               FirstName = task.user.FirstName,
                                                               LastName = task.user.LastName,
                                                               assignedon = task.assignedon,
                                                               id = ti.id,
                                                               subject = ti.subject,
                                                               uniquebody = ti.uniquebody,
                                                               status = task.TicketStatus.name
                                                           }).ToList();
                        atvm.AddRange(tasklist);
                    }
                }
            }
            else if (projectid != 0)
            {
                atvm = (from task in db.TicketItemLog
                        join ti in db.TicketItem on task.ticketitemid equals ti.id
                        where (task.statusid == 2 || task.statusid == 3 || task.statusid == 4) &&
                              task.assignedon >= fromdate && task.assignedon <= todate && ti.projectid == projectid
                        orderby task.user.UserName, task.user.FirstName
                        select new AllTaskViewModel
                        {
                            FirstName = task.user.FirstName,
                            LastName = task.user.LastName,
                            assignedon = task.assignedon,
                            id = ti.id,
                            subject = ti.subject,
                            uniquebody = ti.uniquebody,
                            status = task.TicketStatus.name
                        }).ToList();
            }
            else
            {
                atvm = (from task in db.TicketItemLog
                        join ti in db.TicketItem on task.ticketitemid equals ti.id
                        where (task.statusid == 2 || task.statusid == 3 || task.statusid == 4) &&
                              task.assignedon >= fromdate && task.assignedon <= todate
                        orderby task.user.UserName, task.user.FirstName
                        select new AllTaskViewModel
                        {
                            FirstName = task.user.FirstName,
                            LastName = task.user.LastName,
                            assignedon = task.assignedon,
                            id = ti.id,
                            subject = ti.subject,
                            uniquebody = ti.uniquebody,
                            status = task.TicketStatus.name
                        }).ToList();
            }

            return PartialView("_AllTasks", atvm);
        }

        #endregion
    }
}