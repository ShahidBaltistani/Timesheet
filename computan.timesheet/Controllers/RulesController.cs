using computan.timesheet.Contexts;
using computan.timesheet.core;
using computan.timesheet.Helpers;
using computan.timesheet.Models;
using LinqKit;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace computan.timesheet.Controllers
{
    [CustomeAuthorizeAttribute]
    public class RulesController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        private ApplicationUserManager _userManager;

        public ApplicationUserManager UserManager
        {
            get => _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            private set => _userManager = value;
        }

        // GET: Rules
        public ActionResult Index()
        {
            return View(db.Rule.OrderBy(r => r.runorder).ToList());
        }

        public ActionResult IsUnique(int runorder)
        {
            try
            {
                Rule tag = db.Rule.Single(m => m.runorder == runorder);
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: Rules/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Rule rule = db.Rule.Find(id);
            if (rule == null)
            {
                return HttpNotFound();
            }

            return View(rule);
        }

        // GET: Rules/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Rules/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(
            [Bind(Include = "id,name,runorder,isactive,createdonutc,updatedonutc,ipused,userid")]
            Rule rule)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    List<Rule> rules = db.Rule.ToList();
                    if (rules != null && rules.Count > 0)
                    {
                        foreach (Rule item in rules)
                        {
                            item.runorder = item.runorder + 1;
                            item.updatedonutc = DateTime.Now;
                            item.updatedonutc = DateTime.Now;
                            item.ipused = Request.UserHostAddress;
                            item.userid = User.Identity.GetUserId();
                            db.Entry(item).State = EntityState.Modified;
                        }
                    }

                    rule.runorder = 1;
                    if (!string.IsNullOrEmpty(rule.name))
                    {
                        rule.name = rule.name.Trim();
                    }

                    rule.createdonutc = DateTime.Now;
                    rule.updatedonutc = DateTime.Now;
                    rule.ipused = Request.UserHostAddress;
                    rule.userid = User.Identity.GetUserId();
                    db.Rule.Add(rule);
                    db.SaveChanges();
                    return RedirectToAction("edit", new { rule.id });
                }

                return View(rule);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(rule);
            }
        }

        // GET: Rules/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Rule rule = db.Rule.Find(id);
            if (rule == null)
            {
                return HttpNotFound();
            }

            List<RuleAction> RuleActionList = db.RuleAction.Where(ra => ra.ruleid == id.Value).ToList();

            foreach (RuleAction action in RuleActionList)
            {
                ApplicationUser user = db.Users.Find(action.ruleactionvalue);
                if (user != null)
                {
                    action.fullname = user.FirstName + " " + user.LastName;
                }
            }

            RuleViewModel ruleViewModel = new RuleViewModel
            {
                Rule = rule,
                RuleConditions = db.RuleCondition.Where(rc => rc.ruleid == id.Value).ToList(),
                RuleActions = db.RuleAction.Where(ra => ra.ruleid == id.Value).ToList(),
                RuleExceptions = db.RuleException.Where(re => re.ruleid == id.Value).ToList()
            };


            ViewBag.ruleid = new SelectList(db.Rule, "id", "name");
            ViewBag.ruleconditiontypeid = new SelectList(db.RuleConditionType, "id", "name");
            ViewBag.projectid = new SelectList(db.Project, "id", "name");
            ViewBag.skillid = new SelectList(db.Skill, "id", "name");
            ViewBag.statusid =
                new SelectList(db.TicketStatus.Where(i => i.id != 1 && i.id != 2).ToList(), "id", "name");
            ViewBag.ruleactiontypeid = new SelectList(db.RuleActionType, "id", "name");
            ViewBag.ruleexceptiontypeid = new SelectList(db.RuleExceptionType, "id", "name");
            ViewBag.ruleactionvalue =
                new SelectList(UserManager.Users.OrderBy(f => f.FirstName).Where(u => u.isactive == true).ToList(),
                    "Id", "FullName");
            ViewBag.currentruleid = id.Value;
            return View(ruleViewModel);
        }

        // POST: Rules/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(
            [Bind(Include = "id,name,runorder,isactive,createdonutc,updatedonutc,ipused,userid")]
            Rule rule)
        {
            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(rule.name))
                {
                    rule.name = rule.name.Trim();
                }

                rule.updatedonutc = DateTime.Now;
                rule.ipused = Request.UserHostAddress;
                rule.userid = User.Identity.GetUserId();

                db.Entry(rule).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(rule);
        }

        [HttpPost]
        public ActionResult status(long id, bool statusid)
        {
            Rule rule = db.Rule.Where(i => i.id == id).FirstOrDefault();
            if (rule != null)
            {
                rule.isactive = statusid;
                rule.updatedonutc = DateTime.Now;
                rule.ipused = Request.UserHostAddress;
                rule.userid = User.Identity.GetUserId();
                db.Entry(rule).State = EntityState.Modified;
                db.SaveChanges();

                return Json(new { success = true, successtext = "Done!" }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { error = true, errortext = "Failed rule not found!" }, JsonRequestBehavior.AllowGet);
        }

        // GET: Rules/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Rule rule = db.Rule.Find(id);
            if (rule == null)
            {
                return HttpNotFound();
            }

            return View(rule);
        }

        // POST: Rules/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            Rule rule = db.Rule.Find(id);
            db.Rule.Remove(rule);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Preview(long? id)
        {
            // Fetch rule information
            Rule rule = db.Rule.Find(id.Value);
            if (rule.RuleConditions == null || rule.RuleConditions.Count == 0)
            {
                return RedirectToAction("index");
            }
            // start building the query.
            IQueryable<Ticket> query = db.Ticket;
            ExpressionStarter<Ticket> predicate = PredicateBuilder.New<Ticket>();
            if (rule.RuleConditions != null)
            {
                foreach (RuleCondition condition in rule.RuleConditions)
                {
                    switch (condition.ruleconditiontypeid)
                    {
                        case 1:
                            if (condition.isrequired)
                            {
                                predicate = predicate.And(t => t.uniquesenders.Contains(condition.ruleconditionvalue));
                            }
                            else
                            {
                                predicate = predicate.Or(t => t.uniquesenders.Contains(condition.ruleconditionvalue));
                            }

                            break;
                        case 2:
                            if (condition.isrequired)
                            {
                                predicate = predicate.And(t => t.topic.Contains(condition.ruleconditionvalue));
                            }
                            else
                            {
                                predicate = predicate.Or(t => t.topic.Contains(condition.ruleconditionvalue));
                            }

                            break;
                    }
                }
            }

            if (rule.RuleExceptions != null)
            {
                foreach (RuleException exception in rule.RuleExceptions)
                {
                    switch (exception.ruleexceptiontypeid)
                    {
                        case 1:
                            predicate = predicate.And(t => !t.uniquesenders.Contains(exception.ruleexceptionvalue));
                            break;
                        case 2:
                            predicate = predicate.And(t => !t.topic.Contains(exception.ruleexceptionvalue));
                            break;
                    }
                }
            }

            predicate = predicate.And(t => t.statusid == 1);

            IQueryable<Ticket> newquery = query.AsExpandable().Where(predicate);
            // Execute the query
            List<Ticket> queryResult = newquery.ToList();

            ViewBag.ruleid = id.Value;
            return View(queryResult);
        }

        public ActionResult ExecuteRule(long? id)
        {
            try
            {
                // Fetch rule information               
                Rule rule = db.Rule.Find(id.Value);
                if (rule.RuleConditions == null || rule.RuleConditions.Count == 0)
                {
                    return RedirectToAction("index");
                }

                if (rule.RuleActions == null || rule.RuleActions.Count == 0)
                {
                    return RedirectToAction("index");
                }
                // start building the query.
                IQueryable<Ticket> query = db.Ticket;
                ExpressionStarter<Ticket> predicate = PredicateBuilder.New<Ticket>();
                if (rule.RuleConditions != null && rule.RuleConditions.Count > 0)
                {
                    foreach (RuleCondition condition in rule.RuleConditions)
                    {
                        switch (condition.ruleconditiontypeid)
                        {
                            case 1:
                                if (condition.isrequired)
                                {
                                    predicate = predicate.And(t =>
                                        t.uniquesenders.Contains(condition.ruleconditionvalue));
                                }
                                else
                                {
                                    predicate = predicate.Or(
                                        t => t.uniquesenders.Contains(condition.ruleconditionvalue));
                                }

                                break;
                            case 2:
                                if (condition.isrequired)
                                {
                                    predicate = predicate.And(t => t.topic.Contains(condition.ruleconditionvalue));
                                }
                                else
                                {
                                    predicate = predicate.Or(t => t.topic.Contains(condition.ruleconditionvalue));
                                }

                                break;
                        }
                    }
                }

                if (rule.RuleExceptions != null && rule.RuleExceptions.Count > 0)
                {
                    foreach (RuleException exception in rule.RuleExceptions)
                    {
                        switch (exception.ruleexceptiontypeid)
                        {
                            case 1:
                                predicate = predicate.And(t => !t.uniquesenders.Contains(exception.ruleexceptionvalue));
                                break;
                            case 2:
                                predicate = predicate.And(t => !t.topic.Contains(exception.ruleexceptionvalue));
                                break;
                        }
                    }
                }

                predicate = predicate.And(t => t.statusid == 1);
                query = query.AsExpandable().Where(predicate);
                // Execute the query
                List<Ticket> tickets = query.ToList();

                // make sure at least one ticket is available.
                if (tickets != null && tickets.Count > 0)
                {
                    // make sure at least one assignment has been provided.
                    if (rule.RuleActions != null && rule.RuleActions.Count > 0)
                    {
                        foreach (Ticket ticket in tickets)
                        {
                            bool IsIgnorable = false;
                            bool IsPRojectOnly = false;
                            int statusid = 0;
                            // Load Tikcet Items.
                            db.Entry(ticket).Collection(p => p.TicketItems).Load();

                            // If not ticket items found, skip
                            if (ticket.TicketItems == null || ticket.TicketItems.Count == 0)
                            {
                                continue;
                            }

                            // Step 4. Iterate through all the available Ticket Items and assign users to these tickets.
                            foreach (TicketItem ticketItem in ticket.TicketItems)
                            {
                                // Make sure ticket item status is 1,2 or 3.
                                if (ticketItem.statusid < 4)
                                {
                                    long projectid = 0;
                                    long skillid = 0;
                                    // Validate if user is assigned to current ticketitem.
                                    db.Entry(ticketItem).Collection(p => p.TicketItemLog).Load();

                                    // Initialize TicketItemLog if not available.
                                    if (ticketItem.TicketItemLog == null || ticketItem.TicketItemLog.Count == 0)
                                    {
                                        ticketItem.TicketItemLog = new List<TicketItemLog>();
                                    }

                                    // Iterate current email for all the users assigned.
                                    foreach (RuleAction ruleAction in rule.RuleActions)
                                    {
                                        switch (ruleAction.ruleactiontypeid)
                                        {
                                            case 1:
                                                if (!string.IsNullOrEmpty(ruleAction.ruleactionvalue.Trim()))
                                                {
                                                    // If current user is not found.
                                                    if (ticketItem.TicketItemLog.Where(t =>
                                                            t.ticketitemid == ticketItem.id && t.assignedtousersid ==
                                                            ruleAction.ruleactionvalue).FirstOrDefault() == null)
                                                    {
                                                        string userid = ruleAction.ruleactionvalue.Trim();
                                                        ;
                                                        TicketItemLog newItemLog = new TicketItemLog
                                                        {
                                                            ticketitemid = ticketItem.id,
                                                            assignedbyusersid = User.Identity.GetUserId(),
                                                            assignedtousersid = ruleAction.ruleactionvalue.Trim(),
                                                            assignedon = DateTime.Now,
                                                            statusid = 2,
                                                            statusupdatedbyusersid = User.Identity.GetUserId(),
                                                            statusupdatedon = DateTime.Now
                                                        };

                                                        if (ruleAction.projectid != null)
                                                        {
                                                            projectid = ruleAction.projectid.Value;
                                                        }

                                                        if (ruleAction.skillid != null)
                                                        {
                                                            skillid = ruleAction.skillid.Value;
                                                        }

                                                        long? count = db.TicketItemLog.Where(ti =>
                                                            ti.assignedtousersid == userid && ti.displayorder != null &&
                                                            ti.statusid == 2).Max(d => d.displayorder);
                                                        newItemLog.displayorder = count != null ? count + 1 : 1;
                                                        db.TicketItemLog.Add(newItemLog);
                                                    }
                                                }

                                                break;
                                            case 2:
                                                if (!string.IsNullOrEmpty(ruleAction.ruleactionvalue.Trim()))
                                                {
                                                    // If current user is not found.
                                                    if (ticketItem.TicketItemLog.Where(t =>
                                                            t.ticketitemid == ticketItem.id && t.assignedtousersid ==
                                                            ruleAction.ruleactionvalue).FirstOrDefault() == null)
                                                    {
                                                        string userid = ruleAction.ruleactionvalue.Trim();
                                                        TicketItemLog newItemLog = new TicketItemLog
                                                        {
                                                            ticketitemid = ticketItem.id,
                                                            assignedbyusersid = User.Identity.GetUserId(),
                                                            assignedtousersid = ruleAction.ruleactionvalue.Trim(),
                                                            assignedon = DateTime.Now,
                                                            statusid = 2,
                                                            statusupdatedbyusersid = User.Identity.GetUserId(),
                                                            statusupdatedon = DateTime.Now
                                                        };

                                                        if (ruleAction.projectid == null)
                                                        {
                                                            projectid = 0;
                                                        }

                                                        if (ruleAction.skillid == null)
                                                        {
                                                            skillid = 0;
                                                        }

                                                        long? count = db.TicketItemLog.Where(ti =>
                                                            ti.assignedtousersid == userid && ti.displayorder != null &&
                                                            ti.statusid == 2).Max(d => d.displayorder);
                                                        newItemLog.displayorder = count != null ? count + 1 : 1;
                                                        db.TicketItemLog.Add(newItemLog);
                                                    }
                                                }

                                                break;
                                            case 3:
                                                IsIgnorable = true;
                                                statusid = ruleAction.statusid.Value;
                                                break;
                                            case 4:
                                                IsPRojectOnly = true;
                                                if (ruleAction.projectid != null)
                                                {
                                                    projectid = ruleAction.projectid.Value;
                                                }

                                                if (ruleAction.skillid != null)
                                                {
                                                    skillid = ruleAction.skillid.Value;
                                                }

                                                break;

                                                //case 5:
                                                //    var start = DateTime.Now.Date.Add(TimeSpan.Parse("00:00:00"));
                                                //    var end = DateTime.Now.Date.Add(TimeSpan.Parse("23:59:59"));
                                                //    var tick = db.TicketItem.Where(c => c.createdonutc >= start && c.createdonutc <= end && c.conversationtopic == ticketItem.conversationtopic).FirstOrDefault();
                                                //    if (tick != null)
                                                //    {
                                                //        TicketItem newticketitem = new TicketItem();
                                                //        newticketitem.ticketid = tick.ticketid;
                                                //        newticketitem.emailmessageid = ticketItem.emailmessageid;
                                                //        newticketitem.bccrecipients = ticketItem.bccrecipients;
                                                //        newticketitem.body = ticketItem.body;
                                                //        newticketitem.conversationid = tick.conversationid;
                                                //        newticketitem.conversationindex = tick.conversationindex;
                                                //        newticketitem.conversationtopic = tick.conversationtopic;
                                                //        newticketitem.datetimecreated = ticketItem.datetimecreated;
                                                //        newticketitem.datetimereceived = ticketItem.datetimereceived;
                                                //        newticketitem.datetimesent = ticketItem.datetimesent;
                                                //        newticketitem.displaycc = ticketItem.displaycc;
                                                //        newticketitem.displayto = ticketItem.displayto;
                                                //        newticketitem.from = ticketItem.from;
                                                //        newticketitem.hasattachments = ticketItem.hasattachments;
                                                //        newticketitem.importance = ticketItem.importance;
                                                //        newticketitem.inreplyto = ticketItem.inreplyto;
                                                //        newticketitem.internetmessageheaders = ticketItem.internetmessageheaders;
                                                //        newticketitem.internetmessageid = ticketItem.internetmessageid;
                                                //        newticketitem.lastmodifiedname = ticketItem.lastmodifiedname;
                                                //        newticketitem.lastmodifiedtime = ticketItem.lastmodifiedtime;
                                                //        newticketitem.mimecontent = ticketItem.mimecontent;
                                                //        newticketitem.replyto = ticketItem.replyto;
                                                //        newticketitem.sensitivity = ticketItem.sensitivity;
                                                //        newticketitem.size = ticketItem.size;
                                                //        newticketitem.subject = ticketItem.subject;
                                                //        newticketitem.torecipients = ticketItem.torecipients;
                                                //        newticketitem.uniquebody = ticketItem.uniquebody;
                                                //        newticketitem.projectid = tick.projectid;
                                                //        newticketitem.skillid = tick.skillid;
                                                //        newticketitem.quotedtimeinminutes = ticketItem.quotedtimeinminutes;
                                                //        newticketitem.statusid = 1;
                                                //        newticketitem.createdonutc = DateTime.Now;

                                                //        db.TicketItem.Add(newticketitem);
                                                //    }
                                                //    break;
                                        }
                                    }

                                    // Update TicketItem Skill and Project
                                    if ((ticketItem.statusid == 1 || ticketItem.projectid == null ||
                                         ticketItem.skillid == null) && projectid != 0 && skillid != 0 &&
                                        IsIgnorable == false)
                                    {
                                        ticketItem.projectid = projectid;
                                        ticketItem.skillid = skillid;
                                    }

                                    if (IsIgnorable)
                                    {
                                        ticketItem.statusid = statusid;
                                        ticketItem.projectid = null;
                                        ticketItem.skillid = null;
                                    }
                                    else if (IsPRojectOnly)
                                    {
                                        ticketItem.statusid = 1;
                                    }
                                    else
                                    {
                                        ticketItem.statusid = 2;
                                    }

                                    ticketItem.statusupdatedbyusersid = User.Identity.GetUserId();
                                    ticketItem.statusupdatedon = DateTime.Now;
                                    ticketItem.updatedonutc = DateTime.Now;
                                    ticketItem.ipused = Request.UserHostAddress;
                                    db.Entry(ticketItem).State = EntityState.Modified;
                                }
                            }

                            // Update Ticket.
                            if (IsIgnorable)
                            {
                                ticket.statusid = statusid;
                            }
                            else if (IsPRojectOnly)
                            {
                                ticket.statusid = 1;
                            }
                            else
                            {
                                ticket.statusid = 2;
                            }

                            ticket.statusupdatedbyusersid = User.Identity.GetUserId();
                            ticket.statusupdatedon = DateTime.Now;
                            ticket.LastActivityDate = DateTime.Now;
                            ticket.updatedonutc = DateTime.Now;
                            ticket.ipused = Request.UserHostAddress;
                            db.Entry(ticket).State = EntityState.Modified;
                        }
                    }

                    // Save All changes to the database.
                    db.SaveChanges();

                    return RedirectToAction("index");
                }

                return RedirectToAction("index");
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
                ViewBag.exception = ex.Message;
                ViewBag.exerror = new HandleErrorInfo(ex, myex.controller, myex.action);
                return View("Error");
            }
        }

        //Sorting rules by run order
        public ActionResult sortingrow(long id, string value)
        {
            Rule rule = db.Rule.Find(id);
            if (rule == null)
            {
                return Json(new { error = true, errortext = "Rule not found." }, JsonRequestBehavior.AllowGet);
            }

            int current_runorder = rule.runorder;
            int pre_runorder = 0;
            if (value == "up")
            {
                pre_runorder = rule.runorder - 1;
            }
            else
            {
                pre_runorder = rule.runorder + 1;
            }
            // get next/previous rule
            Rule pre_rule = db.Rule.Where(r => r.runorder == pre_runorder).FirstOrDefault();
            if (pre_rule != null)
            {
                // update previous row
                pre_rule.runorder = current_runorder;
                pre_rule.updatedonutc = DateTime.Now;
                pre_rule.userid = User.Identity.GetUserId();
                pre_rule.ipused = Request.UserHostAddress;
                db.Entry(pre_rule).State = EntityState.Modified;
                // update select row
                rule.runorder = pre_runorder;
                rule.updatedonutc = DateTime.Now;
                rule.userid = User.Identity.GetUserId();
                rule.ipused = Request.UserHostAddress;
                db.Entry(pre_rule).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new { success = true, successtext = "Done!" }, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}