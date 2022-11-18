using computan.timesheet.Contexts;
using computan.timesheet.core;
using computan.timesheet.Helpers;
using LinqKit;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace computan.timesheet.Controllers
{
    [CustomeAuthorizeAttribute]
    public class ExecuteRulesController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            return View();
        }

        // GET: ExecuteRules
        public string ExecuteRule()
        {
            try
            {
                // Fetch rule information
                List<Rule> rules = db.Rule.OrderBy(o => o.runorder).Where(s => s.isactive == true).ToList();
                if (rules == null || rules.Count == 0)
                {
                    return "No rules found";
                }

                foreach (Rule rule in rules)
                {
                    if (rule.RuleConditions == null || rule.RuleConditions.Count == 0)
                    {
                        continue;
                    }

                    if (rule.RuleActions == null || rule.RuleActions.Count == 0)
                    {
                        continue;
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
                                    predicate = predicate.And(t =>
                                        t.uniquesenders.Contains(condition.ruleconditionvalue));
                                    break;
                                case 2:
                                    predicate = predicate.And(t => t.topic.Contains(condition.ruleconditionvalue));
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
                                    predicate = predicate.And(t =>
                                        !t.uniquesenders.Contains(exception.ruleexceptionvalue));
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
                                    // Make sure ticket item status is 1 or 2.
                                    if (ticketItem.statusid < 3)
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
                                                                    t.ticketitemid == ticketItem.id &&
                                                                    t.assignedtousersid == ruleAction.ruleactionvalue)
                                                                .FirstOrDefault() == null)
                                                        {
                                                            string assigntouserid = ruleAction.ruleactionvalue.Trim();
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
                                                            long? count = db.TicketItemLog
                                                                .Where(ti =>
                                                                    ti.assignedtousersid == assigntouserid &&
                                                                    ti.displayorder != null && ti.statusid == 2)
                                                                .Max(d => d.displayorder);
                                                            newItemLog.displayorder = count != null ? count + 1 : 1;

                                                            if (ruleAction.projectid != null)
                                                            {
                                                                projectid = ruleAction.projectid.Value;
                                                            }

                                                            if (ruleAction.skillid != null)
                                                            {
                                                                skillid = ruleAction.skillid.Value;
                                                            }

                                                            db.TicketItemLog.Add(newItemLog);
                                                        }
                                                    }

                                                    break;
                                                case 2:
                                                    if (!string.IsNullOrEmpty(ruleAction.ruleactionvalue.Trim()))
                                                    {
                                                        // If current user is not found.
                                                        if (ticketItem.TicketItemLog.Where(t =>
                                                                    t.ticketitemid == ticketItem.id &&
                                                                    t.assignedtousersid == ruleAction.ruleactionvalue)
                                                                .FirstOrDefault() == null)
                                                        {
                                                            string assigntouserid = ruleAction.ruleactionvalue.Trim();
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
                                                            long? count = db.TicketItemLog
                                                                .Where(ti =>
                                                                    ti.assignedtousersid == assigntouserid &&
                                                                    ti.displayorder != null && ti.statusid == 2)
                                                                .Max(d => d.displayorder);
                                                            newItemLog.displayorder = count != null ? count + 1 : 1;

                                                            if (ruleAction.projectid == null)
                                                            {
                                                                projectid = 0;
                                                            }

                                                            if (ruleAction.skillid == null)
                                                            {
                                                                skillid = 0;
                                                            }

                                                            db.TicketItemLog.Add(newItemLog);
                                                        }
                                                    }

                                                    break;
                                                case 3:
                                                    IsIgnorable = true;
                                                    statusid = ruleAction.statusid.Value;
                                                    break;
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
                    }
                }

                return "done";
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
                return ex.ToString();
            }
        }
    }
}