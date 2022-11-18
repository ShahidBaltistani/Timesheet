using computan.timesheet.core;
using computan.timesheet.Helpers;
using computan.timesheet.Models;
using Microsoft.AspNet.Identity.Owin;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace computan.timesheet.Controllers
{
    [CustomeAuthorizeAttribute]
    public class TicketsByClientController : BaseController
    {
        // GET: TicketsByClient
        private const int recordsPerPage = 50;

        private ApplicationUserManager _userManager;

        public TicketsByClientController()
        {
        }

        public TicketsByClientController(ApplicationUserManager userManager)
        {
            UserManager = userManager;
        }

        public ApplicationUserManager UserManager
        {
            get => _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            private set => _userManager = value;
        }

        public ActionResult Index(int id, long clientid, string topic)
        {
            if (!string.IsNullOrEmpty(topic))
            {
                topic = topic.Trim();
            }
            //if (!IsLoggedIn())
            //{
            //    return RedirectToAction("login", "account");
            //}
            ViewBag.status = db.ConversationStatus.ToList();
            List<SelectListItem> projectlist = db.Project.Where(c => c.clientid == clientid).Select(x => new SelectListItem
            {
                Text = x.name,
                Value = x.id.ToString()
            }).ToList();
            ViewBag.projects = new SelectList(projectlist, "Value", "Text");
            ViewBag.skills = db.Skill.ToList();
            ViewBag.statusid = id;
            ViewBag.tickettype = 0;
            ViewBag.clientid = clientid;
            List<Project> Project = db.Project.Where(c => c.clientid == clientid).ToList();
            SqlParameter client_id = new SqlParameter("@clientid", clientid);
            SqlParameter para_topic = new SqlParameter("@topic", topic);
            List<TicketStatusViewModel> ticketstatuses = db.Database
                .SqlQuery<TicketStatusViewModel>("exec ticketstatus_loadticketscount_projectid @clientid,@topic",
                    client_id, para_topic).ToList();
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
            ViewBag.conversationstatus = ticketstatuses.OrderBy(i => i.id);
            List<Ticket> ticket = new List<Ticket>();
            if (id == 0)
            {
                if (Project != null)
                {
                    foreach (Project item in Project)
                    {
                        IQueryable<Ticket> items = (from tt in db.Ticket
                                                    join ticketi in db.TicketItem on tt.id equals ticketi.ticketid
                                                    where ticketi.projectid == item.id && tt.topic.Contains(topic)
                                                    select tt
                            ).Include(t => t.FlagStatus).Include(t => t.StatusUpdatedByUser)
                            .Include(tt => tt.TicketType).Include(t => t.ConversationStatus).GroupBy(t => t.id)
                            .Select(t => t.FirstOrDefault()).OrderByDescending(t => t.lastdeliverytime).AsQueryable();
                        ticket.AddRange(items);
                    }
                }
            }
            else
            {
                if (Project != null)
                {
                    foreach (Project item in Project)
                    {
                        IQueryable<Ticket> items = (from tt in db.Ticket
                                                    join ticketi in db.TicketItem on tt.id equals ticketi.ticketid
                                                    where ticketi.projectid == item.id && tt.topic.Contains(topic)
                                                    select tt
                            ).Include(t => t.FlagStatus).Include(t => t.StatusUpdatedByUser)
                            .Include(tt => tt.TicketType).Include(t => t.ConversationStatus)
                            .Where(t => t.statusid == id).GroupBy(t => t.id).Select(t => t.FirstOrDefault())
                            .OrderByDescending(t => t.lastdeliverytime).ToList().AsQueryable();
                        ticket.AddRange(items);
                    }
                }
            }

            List<TicketItem> ti = new List<TicketItem>();
            foreach (Ticket items in ticket)
            {
                TicketItem ticketitem = db.TicketItem.Where(t => t.ticketid == items.id).OrderByDescending(t => t.createdonutc)
                    .FirstOrDefault();
                ti.Add(ticketitem);
            }

            ViewBag.counter = ticket.Count();
            ViewBag.users = UserManager.Users.OrderBy(f => f.FirstName).Where(u => u.isactive == true).ToList();
            TicketViewModel tvm = new TicketViewModel
            {
                tickets = ticket.AsQueryable(),
                ticketitems = ti
            };
            if (!string.IsNullOrEmpty(topic))
            {
                return PartialView("_index", tvm);
            }

            return PartialView("_tickets", tvm);
        }

        public ActionResult GetTicketsByProject(int id, long? projectid, long? clientid, string topic)
        {
            if (!string.IsNullOrEmpty(topic))
            {
                topic = topic.Trim();
            }
            //if (!IsLoggedIn())
            //{
            //    return RedirectToAction("login", "account");
            //}
            ViewBag.status = db.ConversationStatus.ToList();
            Project project = db.Project.Where(i => i.id == projectid).FirstOrDefault();
            List<SelectListItem> projectlist = db.Project.Where(c => c.clientid == clientid).Select(x => new SelectListItem
            {
                Text = x.name,
                Value = x.id.ToString()
            }).ToList();
            ViewBag.clientid = clientid;
            ViewBag.projects = new SelectList(projectlist, "Value", "Text");
            ViewBag.skills = db.Skill.ToList();
            ViewBag.statusid = id;
            ViewBag.tickettype = 0;
            if (project != null)
            {
                clientid = project.clientid;
            }

            SqlParameter client_id = new SqlParameter("@clientid", clientid);
            SqlParameter para_topic = new SqlParameter("@topic", topic);
            List<TicketStatusViewModel> ticketstatuses = db.Database
                .SqlQuery<TicketStatusViewModel>("exec ticketstatus_loadticketscount_projectid @clientid,@topic",
                    client_id, para_topic).ToList();
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
            ViewBag.conversationstatus = ticketstatuses.OrderBy(i => i.id);
            List<Ticket> ticket = new List<Ticket>();
            if (projectid != null)
            {
                if (id == 0)
                {
                    Project Project = db.Project.Where(c => c.id == projectid).FirstOrDefault();
                    if (Project != null)
                    {
                        IQueryable<Ticket> items = (from tt in db.Ticket
                                                    join ticketi in db.TicketItem on tt.id equals ticketi.ticketid
                                                    where ticketi.projectid == projectid && tt.topic.Contains(topic)
                                                    select tt
                            ).Include(t => t.FlagStatus).Include(t => t.StatusUpdatedByUser)
                            .Include(tt => tt.TicketType).Include(t => t.ConversationStatus).GroupBy(t => t.id)
                            .Select(t => t.FirstOrDefault()).OrderByDescending(t => t.lastdeliverytime).AsQueryable();
                        ticket.AddRange(items);
                    }
                }
                else
                {
                    List<Project> Project = db.Project.Where(c => c.id == projectid).ToList();
                    if (Project != null)
                    {
                        IQueryable<Ticket> items = (from tt in db.Ticket
                                                    join ticketi in db.TicketItem on tt.id equals ticketi.ticketid
                                                    where ticketi.projectid == projectid && tt.topic.Contains(topic)
                                                    select tt
                            ).Include(t => t.FlagStatus).Include(t => t.StatusUpdatedByUser)
                            .Include(tt => tt.TicketType).Include(t => t.ConversationStatus)
                            .Where(t => t.statusid == id).GroupBy(t => t.id).Select(t => t.FirstOrDefault())
                            .OrderByDescending(t => t.lastdeliverytime).ToList().AsQueryable();
                        ticket.AddRange(items);
                    }
                }
            }
            else
            {
                if (id == 0)
                {
                    List<Project> Project = db.Project.Where(c => c.clientid == clientid).ToList();
                    if (Project != null)
                    {
                        foreach (Project item in Project)
                        {
                            IQueryable<Ticket> items = (from tt in db.Ticket
                                                        join ticketi in db.TicketItem on tt.id equals ticketi.ticketid
                                                        where ticketi.projectid == item.id && tt.topic.Contains(topic)
                                                        select tt
                                ).Include(t => t.FlagStatus).Include(t => t.StatusUpdatedByUser)
                                .Include(tt => tt.TicketType).Include(t => t.ConversationStatus).GroupBy(t => t.id)
                                .Select(t => t.FirstOrDefault()).OrderByDescending(t => t.lastdeliverytime).AsQueryable();
                            ticket.AddRange(items);
                        }
                    }
                }
                else
                {
                    List<Project> Project = db.Project.Where(c => c.clientid == clientid).ToList();
                    if (Project != null)
                    {
                        foreach (Project item in Project)
                        {
                            IQueryable<Ticket> items = (from tt in db.Ticket
                                                        join ticketi in db.TicketItem on tt.id equals ticketi.ticketid
                                                        where ticketi.projectid == item.id && tt.topic.Contains(topic)
                                                        select tt
                                ).Include(t => t.FlagStatus).Include(t => t.StatusUpdatedByUser)
                                .Include(tt => tt.TicketType).Include(t => t.ConversationStatus)
                                .Where(t => t.statusid == id).GroupBy(t => t.id).Select(t => t.FirstOrDefault())
                                .OrderByDescending(t => t.lastdeliverytime).ToList().AsQueryable();
                            ticket.AddRange(items);
                        }
                    }
                }
            }

            List<TicketItem> ti = new List<TicketItem>();
            foreach (Ticket items in ticket)
            {
                TicketItem ticketitem = db.TicketItem.Where(t => t.ticketid == items.id).OrderByDescending(t => t.createdonutc)
                    .FirstOrDefault();
                ti.Add(ticketitem);
            }

            ViewBag.counter = ticket.Count();
            ViewBag.users = UserManager.Users.OrderBy(f => f.FirstName).Where(u => u.isactive == true).ToList();
            TicketViewModel tvm = new TicketViewModel
            {
                tickets = ticket.AsQueryable(),
                ticketitems = ti
            };
            return PartialView("_index", tvm);
        }

        public ActionResult getprojects(long clientid)
        {
            List<Project> projects = db.Project.Where(c => c.clientid == clientid).ToList();
            return PartialView("_GetProjects", projects);
        }

        public ActionResult AssignUsersModel(long clientid)
        {
            List<Project> projects = db.Project.Where(c => c.clientid == clientid).ToList();
            ViewBag.users = UserManager.Users.OrderBy(f => f.FirstName).Where(u => u.isactive == true).ToList();
            List<SelectListItem> projectlist = db.Project.Where(c => c.clientid == clientid).Select(x => new SelectListItem
            {
                Text = x.name,
                Value = x.id.ToString()
            }).ToList();
            ViewBag.clientid = clientid;
            ViewBag.projects = new SelectList(projectlist, "Value", "Text");
            ViewBag.skills = db.Skill.ToList();
            return PartialView("_AssignUsersModel");
        }

        public ActionResult StartWorkingModel(long clientid)
        {
            List<Project> projects = db.Project.Where(c => c.clientid == clientid).ToList();
            List<SelectListItem> projectlist = db.Project.Where(c => c.clientid == clientid).Select(x => new SelectListItem
            {
                Text = x.name,
                Value = x.id.ToString()
            }).ToList();
            ViewBag.clientid = clientid;
            ViewBag.projects = new SelectList(projectlist, "Value", "Text");
            ViewBag.skills = db.Skill.ToList();
            return PartialView("_StartWorkingModel");
        }

        public ActionResult GetRules(long clientid)
        {
            List<Project> projects = db.Project.Where(c => c.clientid == clientid).ToList();
            List<Rule> rules = new List<Rule>();
            if (projects != null && projects.Count > 0)
            {
                foreach (Project item in projects)
                {
                    List<Rule> ruleelist = (from r in db.Rule
                                            join ra in db.RuleAction
                                                on r.id equals ra.ruleid
                                            where ra.projectid == item.id
                                            select r
                        ).ToList();
                    if (ruleelist != null && ruleelist.Count > 0)
                    {
                        rules.AddRange(ruleelist);
                    }
                }
            }

            return PartialView("_GetRules", rules);
        }
    }
}