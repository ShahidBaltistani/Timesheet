using computan.timesheet.core.common;
using computan.timesheet.core;
using computan.timesheet.Helpers;
using computan.timesheet.Models;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web;
using System;

namespace computan.timesheet.Controllers
{
    [CustomeAuthorizeAttribute]
    public class HomeController : BaseController
    {
        private ApplicationRoleManager _roleManager;

        private ApplicationUserManager _userManager;

        public HomeController()
        {
        }

        public HomeController(ApplicationUserManager userManager, ApplicationRoleManager roleManager)
        {
            UserManager = userManager;
            RoleManager = roleManager;
        }

        public ApplicationUserManager UserManager
        {
            get => _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            private set => _userManager = value;
        }

        public ApplicationRoleManager RoleManager
        {
            get => _roleManager ?? HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
            private set => _roleManager = value;
        }

        public ActionResult Index()
        {
            if (User.IsInRole(Role.Admin.ToString()))
            {
                return View("IndexAdmin", db.Team.OrderBy(t => t.name).ToList().AsQueryable());
            }

            return RedirectToAction("MyTasks"); //????
        }

        //public ActionResult NonReplytickets(int? id)
        //{
        //ViewBag.status = db.ConversationStatus.Where(i => i.id != 2);
        //List<SelectListItem> projectlist = db.Database.SqlQuery<CombinedEntity>("exec projects_loadcombined").Select(x =>
        //    new SelectListItem
        //    {
        //        Text = x.name,
        //        Value = x.id.ToString()
        //    }).ToList();
        //ViewBag.projects = new SelectList(projectlist, "Value", "Text");
        //ViewBag.skills = db.Skill.ToList();
        //ViewBag.statusid = id;
        //ViewBag.tickettype = 0;
        //List<TicketStatusViewModel> ticketstatuses = db.Database.SqlQuery<TicketStatusViewModel>("exec ticketstatus_loadticketscount")
        //    .ToList();
        //int totaltask = 0;
        //foreach (TicketStatusViewModel status in ticketstatuses)
        //{
        //    totaltask += status.ticketcount;
        //}

        //TicketStatusViewModel tsvm = new TicketStatusViewModel
        //{
        //    id = 0,
        //    isactive = true,
        //    name = "All",
        //    ticketcount = totaltask
        //};
        //ticketstatuses.Add(tsvm);
        //List<TicketStatusViewModel> newticketstatusesOrder = new List<TicketStatusViewModel>
        //{
        //    ticketstatuses.SingleOrDefault(x => x.name.Equals("All")),
        //    ticketstatuses.SingleOrDefault(x => x.name.Equals("New Task")),
        //    ticketstatuses.SingleOrDefault(x => x.name.Equals("Assigned")),

        //    ticketstatuses.SingleOrDefault(x => x.name.Equals("In Progress")),
        //    ticketstatuses.SingleOrDefault(x => x.name.Equals("On Hold")),
        //    ticketstatuses.SingleOrDefault(x => x.name.Equals("QC")),
        //    ticketstatuses.SingleOrDefault(x => x.name.Equals("In Review")),
        //    ticketstatuses.SingleOrDefault(x => x.name.Equals("Done")),
        //    ticketstatuses.SingleOrDefault(x => x.name.Equals("Trash"))
        //};
        //ViewBag.conversationstatus = newticketstatusesOrder;

        //    IQueryable<Ticket> ticket;

        //    ticket = (from tt in db.Ticket
        //              join ticketi in db.TicketItem on tt.id equals ticketi.ticketid
        //              select tt
        //        ).Include(t => t.FlagStatus).Include(t => t.StatusUpdatedByUser).Include(tt => tt.TicketType)
        //        .Include(t => t.ConversationStatus).GroupBy(t => t.id).Select(t => t.FirstOrDefault())
        //        .OrderByDescending(t => t.lastdeliverytime).Take(30).ToList().AsQueryable();

        //    List<TicketItem> ti = new List<TicketItem>();
        //    foreach (Ticket items in ticket)
        //    {
        //        TicketItem ticketitem = db.TicketItem.Where(t => t.ticketid == items.id).OrderByDescending(t => t.createdonutc)
        //            .FirstOrDefault();
        //        ti.Add(ticketitem);
        //    }

        //    //ViewBag.counter = db.Ticket.Include(t => t.FlagStatus).Include(t => t.StatusUpdatedByUser)
        //    //    .Include(t => t.ConversationStatus).Where(t => t.statusid == id)
        //    //    .OrderByDescending(t => t.lastdeliverytime).Where(i => i.statusid == id).Count();
        //    //ViewBag.users = UserManager.Users.OrderBy(f => f.FirstName).Where(u => u.isactive == true).ToList();
        //    //List<Team> ActiveTeams = db.Team.OrderBy(f => f.name).Where(f => f.isactive == true).ToList().AsQueryable();
        //    //ViewBag.teams = ActiveTeams;

        //    TicketViewModel tvm = new TicketViewModel
        //    {
        //        tickets = ticket,
        //        ticketitems = ti,
        //        teams = db.Team.OrderBy(f => f.name).Where(f => f.isactive == true).ToList().AsQueryable()
        //    };
        //    return View("Index", tvm);
        //}

        public ActionResult team(long? id)
        {
            // If id is not provided, redirect to homepage.
            if (id == null)
            {
                return RedirectToAction("Index");
            }

            // If user is not admin, redirect to my timelog action.
            //if (!User.IsInRole(Role.Admin.ToString())) return RedirectToAction("MyTasks");

            // 1. Fetch Active users.
            List<ApplicationUser> UsersList = db.Users.Where(u => u.isactive == true).ToList();

            string usersid = User.Identity.GetUserId();

            // 2. Filter tickets based on primary selection along with one month date range.
            DateTime assignedFromDate =
                new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0).AddMonths(-1);
            DateTime assignedToDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);
            List<Ticket> dashboardTickets = new List<Ticket>();

            if (User.IsInRole(Role.Admin.ToString()))
            {
                dashboardTickets = fetchTickets(id.Value, assignedFromDate, assignedToDate);
            }
            else
            {
                dashboardTickets = fetchTicketsToUserId(id.Value, assignedFromDate, assignedToDate, usersid);
            }

            // 3. Fetch tickets that are pending assignment.
            List<Ticket> pendingAssignmentTickets = (from t in db.Ticket
                                                     join ttl in db.TicketTeamLogs on t.id equals ttl.ticketid
                                                     where t.IsArchieved == false && t.statusid == 1 && ttl.teamid == id.Value
                                                     select t).Include(t => t.TicketItems).Include(t => t.TicketItems.Select(ti => ti.TicketItemLog))
                .OrderByDescending(t => t.id).ToList();
            // Start preparing the Dashboard Model.
            TeamDashboardViewModel dashboardModel = new TeamDashboardViewModel
            {
                DashboardTickets = dashboardTickets,
                PendingAssignmentTickets = pendingAssignmentTickets
            };

            //Teams For DropDownlist
            ViewBag.Teams = db.Team.ToList();

            //Getting the data of loggedin User
            ViewBag.userid = User.Identity.GetUserId();

            //Checking Admin or User
            ViewBag.Admin = User.IsInRole(Role.Admin.ToString()) ? 1 : 0;

            // 1. Fetch dashboard name.
            Team Team = db.Team.SingleOrDefault(x => x.id == id.Value);
            dashboardModel.teamName = Team != null ? Team.name : "Not Found";

            // 7. Prepare select list for active users.
            List<SelectListItem> UsersSelectList = UsersList.OrderBy(f => f.FirstName).Select(X => new SelectListItem
            {
                Text = X.FirstName + " " + X.LastName + " - " + X.Email,
                Value = X.Id
            }).ToList();

            // 8. Prepare clients select list.
            List<SelectListItem> ClientSelectList = new SelectList(db.Client.Where(x => x.isactive).ToList(), "id", "name").ToList();

            // 9. prepare projects select list.
            List<SelectListItem> ProjectSelectList = db.Database.SqlQuery<CombinedEntity>("exec projects_loadcombined").Select(x =>
                new SelectListItem
                {
                    Text = x.name,
                    Value = x.id.ToString()
                }).ToList();
            string userid = User.Identity.GetUserId();
            List<TicketUserFlagged> flaggedtickets = db.TicketUserFlagged.Where(f => f.isactive && f.userid == userid).ToList();
            dashboardModel.flaggeditems = flaggedtickets;
            ViewBag.skills = db.Skill.ToList();
            ViewBag.teamid = id.Value;
            ViewBag.ByUsers = UsersSelectList;
            ViewBag.ToUsers = UsersSelectList;
            ViewBag.fromdate = assignedFromDate.ToShortDateString();
            ViewBag.todate = assignedToDate.ToShortDateString();
            ViewBag.clients = ClientSelectList;
            ViewBag.projects = ProjectSelectList;
            ViewBag.users = UserManager.Users.OrderBy(f => f.FirstName).Where(u => u.isactive == true).ToList();
            ViewBag.teams = db.Team.OrderBy(f => f.name).Where(f => f.isactive == true).ToList();
            ViewBag.CurrentTeam = Team;

            return View(dashboardModel);
        }

        public ActionResult userdashoard(long? id, string userid)
        {
            // If id is not provided, redirect to homepage.
            if (id == null)
            {
                return RedirectToAction("Index");
            }

            // If user is not admin, redirect to my timelog action.
            //if (!User.IsInRole(Role.Admin.ToString())) return RedirectToAction("MyTasks");

            // 1. Fetch Active users.
            List<ApplicationUser> UsersList = db.Users.Where(u => u.isactive == true).ToList();

            // 2. Filter tickets based on primary selection along with one month date range.
            DateTime assignedFromDate =
                new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0).AddMonths(-1);
            DateTime assignedToDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);
            List<Ticket> dashboardTickets = new List<Ticket>();

            dashboardTickets = fetchTicketsToUserId(id.Value, assignedFromDate, assignedToDate, userid);

            // 3. Fetch tickets that are pending assignment.
            List<Ticket> pendingAssignmentTickets = (from t in db.Ticket
                                                     join ttl in db.TicketTeamLogs on t.id equals ttl.ticketid
                                                     where t.IsArchieved == false && t.statusid == 1 && ttl.teamid == id.Value
                                                     select t).Include(t => t.TicketItems).Include(t => t.TicketItems.Select(ti => ti.TicketItemLog))
                .OrderByDescending(t => t.id).ToList();

            // Start preparing the Dashboard Model.
            TeamDashboardViewModel dashboardModel = new TeamDashboardViewModel
            {
                DashboardTickets = dashboardTickets,
                PendingAssignmentTickets = pendingAssignmentTickets
            };

            //Teams For DropDownlist
            ViewBag.Teams = db.Team.ToList();

            //Getting the data of loggedin User
            ViewBag.userid = User.Identity.GetUserId();

            //Checking Admin or User
            ViewBag.Admin = User.IsInRole(Role.Admin.ToString()) ? 1 : 0;

            // 1. Fetch dashboard name.
            Team Team = db.Team.SingleOrDefault(x => x.id == id.Value);
            dashboardModel.teamName = Team != null ? Team.name : "Not Found";

            // 7. Prepare select list for active users.
            List<SelectListItem> UsersSelectList = UsersList.OrderBy(f => f.FirstName).Select(X => new SelectListItem
            {
                Text = X.FirstName + " " + X.LastName + " - " + X.Email,
                Value = X.Id
            }).ToList();

            // 8. Prepare clients select list.
            List<SelectListItem> ClientSelectList = new SelectList(db.Client.Where(x => x.isactive).ToList(), "id", "name").ToList();

            // 9. prepare projects select list.
            List<SelectListItem> ProjectSelectList = db.Database.SqlQuery<CombinedEntity>("exec projects_loadcombined").Select(x =>
                new SelectListItem
                {
                    Text = x.name,
                    Value = x.id.ToString()
                }).ToList();

            List<TicketUserFlagged> flaggedtickets = db.TicketUserFlagged.Where(f => f.isactive && f.userid == userid).ToList();
            dashboardModel.flaggeditems = flaggedtickets;
            ViewBag.skills = db.Skill.ToList();
            ViewBag.teamid = id.Value;
            ViewBag.ByUsers = UsersSelectList;
            ViewBag.ToUsers = UsersSelectList;
            ViewBag.fromdate = assignedFromDate.ToShortDateString();
            ViewBag.todate = assignedToDate.ToShortDateString();
            ViewBag.clients = ClientSelectList;
            ViewBag.projects = ProjectSelectList;
            ViewBag.users = UserManager.Users.OrderBy(f => f.FirstName).Where(u => u.isactive == true).ToList();
            ViewBag.teams = db.Team.OrderBy(f => f.name).Where(f => f.isactive == true).ToList();
            ViewBag.CurrentTeam = Team;

            return View(dashboardModel);
        }

        public ActionResult teamAjax(long teamid, string ByUsers, string ToUsers, string AssignmentFromDate,
            string AssignmentToDate, string clientid, string projectid, string ticketTitle)
        {
            // If user is not admin, redirect to my timelog action.
            //if (!User.IsInRole(Role.Admin.ToString())) return RedirectToAction("Index");
            List<Ticket> dashboardTickets = null;

            //var assignedFromDate = Convert.ToDateTime(AssignmentFromDate);
            //var assignedToDate = Convert.ToDateTime(AssignmentToDate);
            DateTime assignedFromDate = new DateTime(Convert.ToDateTime(AssignmentFromDate).Year,
                Convert.ToDateTime(AssignmentFromDate).Month, Convert.ToDateTime(AssignmentFromDate).Day, 0, 0, 0);
            DateTime assignedToDate = new DateTime(Convert.ToDateTime(AssignmentToDate).Year,
                Convert.ToDateTime(AssignmentToDate).Month, Convert.ToDateTime(AssignmentToDate).Day, 23, 59, 59);

            if (teamid > 0 && !string.IsNullOrEmpty(AssignmentFromDate) && !string.IsNullOrEmpty(AssignmentToDate) &&
                string.IsNullOrEmpty(clientid) && string.IsNullOrEmpty(projectid) && string.IsNullOrEmpty(ByUsers) &&
                string.IsNullOrEmpty(ToUsers) && string.IsNullOrEmpty(ticketTitle))
            {
                // No Requirement.
                dashboardTickets = fetchTickets(teamid, assignedFromDate, assignedToDate);
            }
            else if (teamid > 0 && !string.IsNullOrEmpty(AssignmentFromDate) &&
                     !string.IsNullOrEmpty(AssignmentToDate) && string.IsNullOrEmpty(clientid) &&
                     string.IsNullOrEmpty(projectid) && string.IsNullOrEmpty(ByUsers) &&
                     !string.IsNullOrEmpty(ToUsers) && string.IsNullOrEmpty(ticketTitle))
            {
                // To Users
                dashboardTickets = fetchTicketsToUserId(teamid, assignedFromDate, assignedToDate, ToUsers);
            }
            else if (teamid > 0 && !string.IsNullOrEmpty(AssignmentFromDate) &&
                     !string.IsNullOrEmpty(AssignmentToDate) && string.IsNullOrEmpty(clientid) &&
                     string.IsNullOrEmpty(projectid) && !string.IsNullOrEmpty(ByUsers) &&
                     string.IsNullOrEmpty(ToUsers) && string.IsNullOrEmpty(ticketTitle))
            {
                // By Users
                dashboardTickets = fetchTicketsByUserId(teamid, assignedFromDate, assignedToDate, ByUsers);
            }
            else if (teamid > 0 && !string.IsNullOrEmpty(AssignmentFromDate) &&
                     !string.IsNullOrEmpty(AssignmentToDate) && string.IsNullOrEmpty(clientid) &&
                     string.IsNullOrEmpty(projectid) && !string.IsNullOrEmpty(ByUsers) &&
                     !string.IsNullOrEmpty(ToUsers) && string.IsNullOrEmpty(ticketTitle))
            {
                // To Users and By Users
                dashboardTickets =
                    fetchTicketsToUserIdAndByUserId(teamid, assignedFromDate, assignedToDate, ToUsers, ByUsers);
            }
            else if (teamid > 0 && !string.IsNullOrEmpty(AssignmentFromDate) &&
                     !string.IsNullOrEmpty(AssignmentToDate) && string.IsNullOrEmpty(clientid) &&
                     !string.IsNullOrEmpty(projectid) && string.IsNullOrEmpty(ByUsers) &&
                     string.IsNullOrEmpty(ToUsers) && string.IsNullOrEmpty(ticketTitle))
            {
                // Project Id
                dashboardTickets =
                    fetchTicketsByProjectId(teamid, assignedFromDate, assignedToDate, Convert.ToInt64(projectid));
            }
            else if (teamid > 0 && !string.IsNullOrEmpty(AssignmentFromDate) &&
                     !string.IsNullOrEmpty(AssignmentToDate) && string.IsNullOrEmpty(clientid) &&
                     !string.IsNullOrEmpty(projectid) && string.IsNullOrEmpty(ByUsers) &&
                     !string.IsNullOrEmpty(ToUsers) && string.IsNullOrEmpty(ticketTitle))
            {
                // Project Id and To User
                dashboardTickets = fetchTicketsByProjectIdAndToUserId(teamid, assignedFromDate, assignedToDate,
                    Convert.ToInt64(projectid), ToUsers);
            }
            else if (teamid > 0 && !string.IsNullOrEmpty(AssignmentFromDate) &&
                     !string.IsNullOrEmpty(AssignmentToDate) && string.IsNullOrEmpty(clientid) &&
                     !string.IsNullOrEmpty(projectid) && !string.IsNullOrEmpty(ByUsers) &&
                     string.IsNullOrEmpty(ToUsers) && string.IsNullOrEmpty(ticketTitle))
            {
                // Project Id and By User
                dashboardTickets = fetchTicketsByProjectIdAndByUserId(teamid, assignedFromDate, assignedToDate,
                    Convert.ToInt64(projectid), ByUsers);
            }
            else if (teamid > 0 && !string.IsNullOrEmpty(AssignmentFromDate) &&
                     !string.IsNullOrEmpty(AssignmentToDate) && string.IsNullOrEmpty(clientid) &&
                     !string.IsNullOrEmpty(projectid) && !string.IsNullOrEmpty(ByUsers) &&
                     string.IsNullOrEmpty(ToUsers) && string.IsNullOrEmpty(ticketTitle))
            {
                // Project Id and By User and To User
                dashboardTickets = fetchTicketsByProjectIdAndByUserIdAndToUserId(teamid, assignedFromDate,
                    assignedToDate, Convert.ToInt64(projectid), ByUsers, ToUsers);
            }
            else if (teamid > 0 && !string.IsNullOrEmpty(AssignmentFromDate) &&
                     !string.IsNullOrEmpty(AssignmentToDate) && !string.IsNullOrEmpty(clientid) &&
                     string.IsNullOrEmpty(projectid) && string.IsNullOrEmpty(ByUsers) &&
                     string.IsNullOrEmpty(ToUsers) && string.IsNullOrEmpty(ticketTitle))
            {
                // Client Id
                dashboardTickets =
                    fetchTicketsByClientId(teamid, assignedFromDate, assignedToDate, Convert.ToInt64(clientid));
            }
            else if (teamid > 0 && !string.IsNullOrEmpty(AssignmentFromDate) &&
                     !string.IsNullOrEmpty(AssignmentToDate) && !string.IsNullOrEmpty(clientid) &&
                     string.IsNullOrEmpty(projectid) && string.IsNullOrEmpty(ByUsers) &&
                     !string.IsNullOrEmpty(ToUsers) && string.IsNullOrEmpty(ticketTitle))
            {
                // Client Id and To User
                dashboardTickets = fetchTicketsByClientIdAndToUserId(teamid, assignedFromDate, assignedToDate,
                    Convert.ToInt64(clientid), ToUsers);
            }
            else if (teamid > 0 && !string.IsNullOrEmpty(AssignmentFromDate) &&
                     !string.IsNullOrEmpty(AssignmentToDate) && !string.IsNullOrEmpty(clientid) &&
                     string.IsNullOrEmpty(projectid) && !string.IsNullOrEmpty(ByUsers) &&
                     string.IsNullOrEmpty(ToUsers) && string.IsNullOrEmpty(ticketTitle))
            {
                // Client Id and By User
                dashboardTickets = fetchTicketsByClientIdAndByUserId(teamid, assignedFromDate, assignedToDate,
                    Convert.ToInt64(clientid), ByUsers);
            }
            else if (teamid > 0 && !string.IsNullOrEmpty(AssignmentFromDate) &&
                     !string.IsNullOrEmpty(AssignmentToDate) && !string.IsNullOrEmpty(clientid) &&
                     string.IsNullOrEmpty(projectid) && string.IsNullOrEmpty(ByUsers) &&
                     !string.IsNullOrEmpty(ToUsers) && string.IsNullOrEmpty(ticketTitle))
            {
                // Client Id and To User and by user
                dashboardTickets = fetchTicketsByClientIdAndByUserIdAndToUserId(teamid, assignedFromDate,
                    assignedToDate, Convert.ToInt64(clientid), ByUsers, ToUsers);
            }
            else if (teamid > 0 && !string.IsNullOrEmpty(AssignmentFromDate) &&
                     !string.IsNullOrEmpty(AssignmentToDate) && !string.IsNullOrEmpty(clientid) &&
                     !string.IsNullOrEmpty(projectid) && string.IsNullOrEmpty(ByUsers) &&
                     string.IsNullOrEmpty(ToUsers) && string.IsNullOrEmpty(ticketTitle))
            {
                // Client Id and ProjectId
                dashboardTickets = fetchTicketsByClientIdAndProjectId(teamid, assignedFromDate, assignedToDate,
                    Convert.ToInt64(clientid), Convert.ToInt64(projectid));
            }
            else if (teamid > 0 && !string.IsNullOrEmpty(AssignmentFromDate) &&
                     !string.IsNullOrEmpty(AssignmentToDate) && !string.IsNullOrEmpty(clientid) &&
                     !string.IsNullOrEmpty(projectid) && string.IsNullOrEmpty(ByUsers) &&
                     !string.IsNullOrEmpty(ToUsers) && string.IsNullOrEmpty(ticketTitle))
            {
                // Client Id and ProjectId and To User
                dashboardTickets = fetchTicketsByClientIdAndProjectIdAndToUserId(teamid, assignedFromDate,
                    assignedToDate, Convert.ToInt64(clientid), Convert.ToInt64(projectid), ToUsers);
            }
            else if (teamid > 0 && !string.IsNullOrEmpty(AssignmentFromDate) &&
                     !string.IsNullOrEmpty(AssignmentToDate) && !string.IsNullOrEmpty(clientid) &&
                     string.IsNullOrEmpty(projectid) && !string.IsNullOrEmpty(ByUsers) &&
                     string.IsNullOrEmpty(ToUsers) && string.IsNullOrEmpty(ticketTitle))
            {
                // Client Id and projectid and By User
                dashboardTickets = fetchTicketsByClientIdAndProjectIdAndByUserId(teamid, assignedFromDate,
                    assignedToDate, Convert.ToInt64(clientid), Convert.ToInt64(projectid), ByUsers);
            }
            else if (teamid > 0 && !string.IsNullOrEmpty(AssignmentFromDate) &&
                     !string.IsNullOrEmpty(AssignmentToDate) && !string.IsNullOrEmpty(clientid) &&
                     !string.IsNullOrEmpty(projectid) && !string.IsNullOrEmpty(ByUsers) &&
                     !string.IsNullOrEmpty(ToUsers) && string.IsNullOrEmpty(ticketTitle))
            {
                // ClientId And ProjectId And ByUserId And ToUserId
                dashboardTickets = fetchTicketsByClientIdAndProjectIdAndByUserIdAndToUserId(teamid, assignedFromDate,
                    assignedToDate, Convert.ToInt64(clientid), Convert.ToInt64(projectid), ByUsers, ToUsers);
            }
            //TicketTitle Check is added from here
            else if (teamid > 0 && !string.IsNullOrEmpty(AssignmentFromDate) &&
                     !string.IsNullOrEmpty(AssignmentToDate) && string.IsNullOrEmpty(clientid) &&
                     string.IsNullOrEmpty(projectid) && string.IsNullOrEmpty(ByUsers) &&
                     string.IsNullOrEmpty(ToUsers) && !string.IsNullOrEmpty(ticketTitle))
            {
                // TicketTitle
                dashboardTickets = fetchTicketsByTicketTitle(teamid, assignedFromDate, assignedToDate, ticketTitle);
            }
            else if (teamid > 0 && !string.IsNullOrEmpty(AssignmentFromDate) &&
                     !string.IsNullOrEmpty(AssignmentToDate) && string.IsNullOrEmpty(clientid) &&
                     string.IsNullOrEmpty(projectid) && !string.IsNullOrEmpty(ByUsers) &&
                     string.IsNullOrEmpty(ToUsers) && !string.IsNullOrEmpty(ticketTitle))
            {
                // TicketTitle and ByUser
                dashboardTickets =
                    fetchTicketsByTicketTitleAndByUserId(teamid, assignedFromDate, assignedToDate, ticketTitle,
                        ByUsers);
            }
            else if (teamid > 0 && !string.IsNullOrEmpty(AssignmentFromDate) &&
                     !string.IsNullOrEmpty(AssignmentToDate) && string.IsNullOrEmpty(clientid) &&
                     string.IsNullOrEmpty(projectid) && string.IsNullOrEmpty(ByUsers) &&
                     !string.IsNullOrEmpty(ToUsers) && !string.IsNullOrEmpty(ticketTitle))
            {
                // TicketTitle and ToUser
                dashboardTickets =
                    fetchTicketsByTicketTitleAndToUserId(teamid, assignedFromDate, assignedToDate, ticketTitle,
                        ToUsers);
            }
            else if (teamid > 0 && !string.IsNullOrEmpty(AssignmentFromDate) &&
                     !string.IsNullOrEmpty(AssignmentToDate) && string.IsNullOrEmpty(clientid) &&
                     string.IsNullOrEmpty(projectid) && !string.IsNullOrEmpty(ByUsers) &&
                     !string.IsNullOrEmpty(ToUsers) && !string.IsNullOrEmpty(ticketTitle))
            {
                // TicketTitle and To User and by user
                dashboardTickets = fetchTicketsByTicketTitleAndByUserIdAndToUserId(teamid, assignedFromDate,
                    assignedToDate, ticketTitle, ByUsers, ToUsers);
            }
            else if (teamid > 0 && !string.IsNullOrEmpty(AssignmentFromDate) &&
                     !string.IsNullOrEmpty(AssignmentToDate) && string.IsNullOrEmpty(clientid) &&
                     !string.IsNullOrEmpty(projectid) && string.IsNullOrEmpty(ByUsers) &&
                     string.IsNullOrEmpty(ToUsers) && !string.IsNullOrEmpty(ticketTitle))
            {
                // TicketTitle and ProjectId
                dashboardTickets = fetchTicketsByTicketTitleAndProjectId(teamid, assignedFromDate, assignedToDate,
                    ticketTitle, Convert.ToInt64(projectid));
            }
            else if (teamid > 0 && !string.IsNullOrEmpty(AssignmentFromDate) &&
                     !string.IsNullOrEmpty(AssignmentToDate) && string.IsNullOrEmpty(clientid) &&
                     !string.IsNullOrEmpty(projectid) && string.IsNullOrEmpty(ByUsers) &&
                     !string.IsNullOrEmpty(ToUsers) && !string.IsNullOrEmpty(ticketTitle))
            {
                // TicketTitle and ProjectId and ToUser
                dashboardTickets = fetchTicketsByTicketTitleAndProjectIdAndToUserId(teamid, assignedFromDate,
                    assignedToDate, ticketTitle, Convert.ToInt64(projectid), ToUsers);
            }
            else if (teamid > 0 && !string.IsNullOrEmpty(AssignmentFromDate) &&
                     !string.IsNullOrEmpty(AssignmentToDate) && string.IsNullOrEmpty(clientid) &&
                     !string.IsNullOrEmpty(projectid) && !string.IsNullOrEmpty(ByUsers) &&
                     string.IsNullOrEmpty(ToUsers) && !string.IsNullOrEmpty(ticketTitle))
            {
                // TicketTitle and ProjectId and ByUser
                dashboardTickets = fetchTicketsByTicketTitleAndProjectIdAndByUserId(teamid, assignedFromDate,
                    assignedToDate, ticketTitle, Convert.ToInt64(projectid), ByUsers);
            }
            else if (teamid > 0 && !string.IsNullOrEmpty(AssignmentFromDate) &&
                     !string.IsNullOrEmpty(AssignmentToDate) && string.IsNullOrEmpty(clientid) &&
                     !string.IsNullOrEmpty(projectid) && !string.IsNullOrEmpty(ByUsers) &&
                     !string.IsNullOrEmpty(ToUsers) && !string.IsNullOrEmpty(ticketTitle))
            {
                // TicketTitle and ProjectId And ByUserId And ToUserId
                dashboardTickets = fetchTicketsByTicketTitleAndProjectIdAndByUserIdAndToUserId(teamid, assignedFromDate,
                    assignedToDate, ticketTitle, Convert.ToInt64(projectid), ByUsers, ToUsers);
            }
            else if (teamid > 0 && !string.IsNullOrEmpty(AssignmentFromDate) &&
                     !string.IsNullOrEmpty(AssignmentToDate) && !string.IsNullOrEmpty(clientid) &&
                     string.IsNullOrEmpty(projectid) && string.IsNullOrEmpty(ByUsers) &&
                     string.IsNullOrEmpty(ToUsers) && !string.IsNullOrEmpty(ticketTitle))
            {
                // TicketTitle and ClientId
                dashboardTickets = fetchTicketsByTicketTitleAndClientId(teamid, assignedFromDate, assignedToDate,
                    ticketTitle, Convert.ToInt64(clientid));
            }
            else if (teamid > 0 && !string.IsNullOrEmpty(AssignmentFromDate) &&
                     !string.IsNullOrEmpty(AssignmentToDate) && !string.IsNullOrEmpty(clientid) &&
                     string.IsNullOrEmpty(projectid) && !string.IsNullOrEmpty(ByUsers) &&
                     string.IsNullOrEmpty(ToUsers) && !string.IsNullOrEmpty(ticketTitle))
            {
                // TicketTitle and ClientId and ByUser
                dashboardTickets = fetchTicketsByTicketTitleAndClientIdAndByUserId(teamid, assignedFromDate,
                    assignedToDate, ticketTitle, Convert.ToInt64(clientid), ByUsers);
            }
            else if (teamid > 0 && !string.IsNullOrEmpty(AssignmentFromDate) &&
                     !string.IsNullOrEmpty(AssignmentToDate) && !string.IsNullOrEmpty(clientid) &&
                     string.IsNullOrEmpty(projectid) && string.IsNullOrEmpty(ByUsers) &&
                     !string.IsNullOrEmpty(ToUsers) && !string.IsNullOrEmpty(ticketTitle))
            {
                // TicketTitle and ClientId and ToUser
                dashboardTickets = fetchTicketsByTicketTitleAndClientIdAndToUserId(teamid, assignedFromDate,
                    assignedToDate, ticketTitle, Convert.ToInt64(clientid), ToUsers);
            }
            else if (teamid > 0 && !string.IsNullOrEmpty(AssignmentFromDate) &&
                     !string.IsNullOrEmpty(AssignmentToDate) && !string.IsNullOrEmpty(clientid) &&
                     string.IsNullOrEmpty(projectid) && !string.IsNullOrEmpty(ByUsers) &&
                     !string.IsNullOrEmpty(ToUsers) && !string.IsNullOrEmpty(ticketTitle))
            {
                // TicketTitle and ClientId and ByUser and ToUser
                dashboardTickets = fetchTicketsByTicketTitleAndClientIdAndByUserIdAndToUserId(teamid, assignedFromDate,
                    assignedToDate, ticketTitle, Convert.ToInt64(clientid), ByUsers, ToUsers);
            }
            else if (teamid > 0 && !string.IsNullOrEmpty(AssignmentFromDate) &&
                     !string.IsNullOrEmpty(AssignmentToDate) && !string.IsNullOrEmpty(clientid) &&
                     !string.IsNullOrEmpty(projectid) && string.IsNullOrEmpty(ByUsers) &&
                     string.IsNullOrEmpty(ToUsers) && !string.IsNullOrEmpty(ticketTitle))
            {
                // TicketTitle and ClientId and ProjectId
                dashboardTickets = fetchTicketsByTicketTitleAndClientIdAndProjectId(teamid, assignedFromDate,
                    assignedToDate, ticketTitle, Convert.ToInt64(clientid), Convert.ToInt64(projectid));
            }
            else if (teamid > 0 && !string.IsNullOrEmpty(AssignmentFromDate) &&
                     !string.IsNullOrEmpty(AssignmentToDate) && !string.IsNullOrEmpty(clientid) &&
                     !string.IsNullOrEmpty(projectid) && !string.IsNullOrEmpty(ByUsers) &&
                     string.IsNullOrEmpty(ToUsers) && !string.IsNullOrEmpty(ticketTitle))
            {
                // TicketTitle and ClientId and ProjectId and ByUser
                dashboardTickets = fetchTicketsByTicketTitleAndClientIdAndProjectIdAndByUserId(teamid, assignedFromDate,
                    assignedToDate, ticketTitle, Convert.ToInt64(clientid), Convert.ToInt64(projectid), ByUsers);
            }
            else if (teamid > 0 && !string.IsNullOrEmpty(AssignmentFromDate) &&
                     !string.IsNullOrEmpty(AssignmentToDate) && !string.IsNullOrEmpty(clientid) &&
                     !string.IsNullOrEmpty(projectid) && string.IsNullOrEmpty(ByUsers) &&
                     !string.IsNullOrEmpty(ToUsers) && !string.IsNullOrEmpty(ticketTitle))
            {
                // TicketTitle and ClientId and ProjectId and ToUser
                dashboardTickets = fetchTicketsByTicketTitleAndClientIdAndProjectIdAndToUserId(teamid, assignedFromDate,
                    assignedToDate, ticketTitle, Convert.ToInt64(clientid), Convert.ToInt64(projectid), ToUsers);
            }
            else if (teamid > 0 && !string.IsNullOrEmpty(AssignmentFromDate) &&
                     !string.IsNullOrEmpty(AssignmentToDate) && !string.IsNullOrEmpty(clientid) &&
                     !string.IsNullOrEmpty(projectid) && !string.IsNullOrEmpty(ByUsers) &&
                     !string.IsNullOrEmpty(ToUsers) && !string.IsNullOrEmpty(ticketTitle))
            {
                // TicketTitle and ClientId and ProjectId and ToUser
                dashboardTickets = fetchTicketsByTicketTitleAndClientIdAndProjectIdAndByUserIdAndToUserId(teamid,
                    assignedFromDate, assignedToDate, ticketTitle, Convert.ToInt64(clientid),
                    Convert.ToInt64(projectid), ByUsers, ToUsers);
            }

            if (dashboardTickets == null)
            {
                dashboardTickets = new List<Ticket>();
            }

            // Start preparing the Dashboard Model.
            TeamDashboardViewModel dashboardModel = new TeamDashboardViewModel();

            // 3. Fetch tickets that are pending assignment.

            if (string.IsNullOrEmpty(ticketTitle))
            {
                List<Ticket> pendingAssignmentTickets = (from t in db.Ticket
                                                         join ttl in db.TicketTeamLogs on t.id equals ttl.ticketid
                                                         where t.IsArchieved == false && t.statusid == 1 && ttl.teamid == teamid
                                                         select t).Include(t => t.TicketItems).Include(t => t.TicketItems.Select(ti => ti.TicketItemLog))
                    .OrderByDescending(t => t.id).ToList();

                dashboardModel.PendingAssignmentTickets = pendingAssignmentTickets;
            }
            else
            {
                List<Ticket> pendingAssignmentTickets = (from t in db.Ticket
                                                         join ttl in db.TicketTeamLogs on t.id equals ttl.ticketid
                                                         where t.IsArchieved == false && t.statusid == 1 && ttl.teamid == teamid &&
                                                               t.topic.Contains(ticketTitle)
                                                         select t).Include(t => t.TicketItems).Include(t => t.TicketItems.Select(ti => ti.TicketItemLog))
                    .OrderByDescending(t => t.id).ToList();
                dashboardModel.PendingAssignmentTickets = pendingAssignmentTickets;
            }

            dashboardModel.DashboardTickets = dashboardTickets;

            // 1. Fetch dashboard name.
            Team Team = db.Team.SingleOrDefault(x => x.id == teamid);
            dashboardModel.teamName = Team != null ? Team.name : "Not Found";

            ViewBag.teamid = teamid;

            // 2. Fetch Active users.
            //var UsersList = db.Users.Where(u => u.isactive == true).ToList();
            string userid = User.Identity.GetUserId();
            List<TicketUserFlagged> flaggedtickets = db.TicketUserFlagged.Where(f => f.isactive && f.userid == userid).ToList();
            dashboardModel.flaggeditems = flaggedtickets;
            return PartialView(dashboardModel);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult RemoveTicketUser(long ticketid, string userid)
        {
            List<TicketItem> ticketitems = db.TicketItem.Where(x => x.ticketid == ticketid).OrderBy(x => x.id).ToList();
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

                TicketItem ticketitem = db.TicketItem.Where(x => x.ticketid == ticketid).OrderBy(x => x.id).ToList()
                    .FirstOrDefault();
                TicketItemLog ticketitemlog = db.TicketItemLog.Where(t => t.ticketitemid == ticketitem.id).FirstOrDefault();
                if (ticketitemlog == null)
                {
                    Ticket ticket = db.Ticket.Where(t => t.id == ticketid).FirstOrDefault();
                    ticket.statusid = 1;
                    db.SaveChanges();
                }

                return Json(new { success = true, messagetext = "The user has been removed successfully." });
            }

            return Json(new { success = false, messagetext = "User not found." });
        }

        public ActionResult RemoveTicketteam(long ticketid, long teamid)
        {
            TicketTeamLogs teamtoassign = db.TicketTeamLogs.Where(t => t.ticketid == ticketid).Where(t => t.teamid == teamid)
                .FirstOrDefault();
            if (teamtoassign != null)
            {
                db.TicketTeamLogs.Remove(teamtoassign);
                db.SaveChanges();
                return Json(new { success = true, messagetext = "The team has been removed successfully." });
            }

            return Json(new { success = false, messagetext = "team not found." });
        }

        private List<Ticket> fetchTickets(long teamid, DateTime assignedFromDate, DateTime assignedToDate)
        {
            List<Ticket> dashboardTickets = (from t in db.Ticket
                                             join ttl in db.TicketTeamLogs on t.id equals ttl.ticketid
                                             join p in db.Project on t.projectid equals p.id
                                             join ti in db.TicketItem on t.id equals ti.ticketid
                                             join til in db.TicketItemLog on ti.id equals til.ticketitemid
                                             where t.IsArchieved == false && ttl.teamid == teamid && t.projectid > 0 && t.skillid > 0 &&
                                                   t.updatedonutc >= assignedFromDate && t.updatedonutc <= assignedToDate
                                             select t).Include(t => t.TicketItems).Include(t => t.TicketItems.Select(ti => ti.TicketItemLog))
                .Distinct().OrderByDescending(t => t.id).ToList();

            return dashboardTickets;
        }

        private List<Ticket> fetchTicketsToUserId(long teamid, DateTime assignedFromDate, DateTime assignedToDate,
            string ToUserId)
        {
            List<Ticket> dashboardTickets = (from t in db.Ticket
                                             join ttl in db.TicketTeamLogs on t.id equals ttl.ticketid
                                             join p in db.Project on t.projectid equals p.id
                                             join ti in db.TicketItem on t.id equals ti.ticketid
                                             join til in db.TicketItemLog on ti.id equals til.ticketitemid
                                             where t.IsArchieved == false && ttl.teamid == teamid && t.projectid > 0 && t.skillid > 0 &&
                                                   t.updatedonutc >= assignedFromDate && t.updatedonutc <= assignedToDate &&
                                                   til.assignedtousersid == ToUserId
                                             select t).Include(t => t.TicketItems).Include(t => t.TicketItems.Select(ti => ti.TicketItemLog))
                .Distinct().OrderByDescending(t => t.id).ToList();

            return dashboardTickets;
        }

        private List<Ticket> fetchTicketsByUserId(long teamid, DateTime assignedFromDate, DateTime assignedToDate,
            string ByUserId)
        {
            List<Ticket> dashboardTickets = (from t in db.Ticket
                                             join ttl in db.TicketTeamLogs on t.id equals ttl.ticketid
                                             join p in db.Project on t.projectid equals p.id
                                             join ti in db.TicketItem on t.id equals ti.ticketid
                                             join til in db.TicketItemLog on ti.id equals til.ticketitemid
                                             where t.IsArchieved == false && ttl.teamid == teamid && t.projectid > 0 && t.skillid > 0 &&
                                                   t.updatedonutc >= assignedFromDate && t.updatedonutc <= assignedToDate &&
                                                   til.assignedbyusersid == ByUserId
                                             select t).Include(t => t.TicketItems).Include(t => t.TicketItems.Select(ti => ti.TicketItemLog))
                .Distinct().OrderByDescending(t => t.id).ToList();

            return dashboardTickets;
        }

        private List<Ticket> fetchTicketsToUserIdAndByUserId(long teamid, DateTime assignedFromDate,
            DateTime assignedToDate, string ToUserId, string ByUserId)
        {
            List<Ticket> dashboardTickets = (from t in db.Ticket
                                             join ttl in db.TicketTeamLogs on t.id equals ttl.ticketid
                                             join p in db.Project on t.projectid equals p.id
                                             join ti in db.TicketItem on t.id equals ti.ticketid
                                             join til in db.TicketItemLog on ti.id equals til.ticketitemid
                                             where t.IsArchieved == false && ttl.teamid == teamid && t.projectid > 0 && t.skillid > 0 &&
                                                   t.updatedonutc >= assignedFromDate && t.updatedonutc <= assignedToDate &&
                                                   til.assignedtousersid == ToUserId && til.assignedbyusersid == ByUserId
                                             select t).Include(t => t.TicketItems).Include(t => t.TicketItems.Select(ti => ti.TicketItemLog))
                .Distinct().OrderByDescending(t => t.id).ToList();

            return dashboardTickets;
        }

        private List<Ticket> fetchTicketsByProjectId(long teamid, DateTime assignedFromDate, DateTime assignedToDate,
            long projectid)
        {
            List<Ticket> dashboardTickets = (from t in db.Ticket
                                             join ttl in db.TicketTeamLogs on t.id equals ttl.ticketid
                                             join p in db.Project on t.projectid equals p.id
                                             join ti in db.TicketItem on t.id equals ti.ticketid
                                             join til in db.TicketItemLog on ti.id equals til.ticketitemid
                                             where t.IsArchieved == false && ttl.teamid == teamid && t.projectid > 0 && t.skillid > 0 &&
                                                   t.updatedonutc >= assignedFromDate && t.updatedonutc <= assignedToDate && p.id == projectid
                                             select t).Include(t => t.TicketItems).Include(t => t.TicketItems.Select(ti => ti.TicketItemLog))
                .Distinct().OrderByDescending(t => t.id).ToList();

            return dashboardTickets;
        }

        private List<Ticket> fetchTicketsByProjectIdAndToUserId(long teamid, DateTime assignedFromDate,
            DateTime assignedToDate, long projectid, string ToUserId)
        {
            List<Ticket> dashboardTickets = (from t in db.Ticket
                                             join ttl in db.TicketTeamLogs on t.id equals ttl.ticketid
                                             join p in db.Project on t.projectid equals p.id
                                             join ti in db.TicketItem on t.id equals ti.ticketid
                                             join til in db.TicketItemLog on ti.id equals til.ticketitemid
                                             where t.IsArchieved == false && ttl.teamid == teamid && t.projectid > 0 && t.skillid > 0 &&
                                                   t.updatedonutc >= assignedFromDate && t.updatedonutc <= assignedToDate && p.id == projectid &&
                                                   til.assignedtousersid == ToUserId
                                             select t).Include(t => t.TicketItems).Include(t => t.TicketItems.Select(ti => ti.TicketItemLog))
                .Distinct().OrderByDescending(t => t.id).ToList();

            return dashboardTickets;
        }

        private List<Ticket> fetchTicketsByProjectIdAndByUserId(long teamid, DateTime assignedFromDate,
            DateTime assignedToDate, long projectid, string ByUserId)
        {
            List<Ticket> dashboardTickets = (from t in db.Ticket
                                             join ttl in db.TicketTeamLogs on t.id equals ttl.ticketid
                                             join p in db.Project on t.projectid equals p.id
                                             join ti in db.TicketItem on t.id equals ti.ticketid
                                             join til in db.TicketItemLog on ti.id equals til.ticketitemid
                                             where t.IsArchieved == false && ttl.teamid == teamid && t.projectid > 0 && t.skillid > 0 &&
                                                   t.updatedonutc >= assignedFromDate && t.updatedonutc <= assignedToDate && p.id == projectid &&
                                                   til.assignedbyusersid == ByUserId
                                             select t).Include(t => t.TicketItems).Include(t => t.TicketItems.Select(ti => ti.TicketItemLog))
                .Distinct().OrderByDescending(t => t.id).ToList();

            return dashboardTickets;
        }

        private List<Ticket> fetchTicketsByProjectIdAndByUserIdAndToUserId(long teamid, DateTime assignedFromDate,
            DateTime assignedToDate, long projectid, string ByUserId, string ToUserId)
        {
            List<Ticket> dashboardTickets = (from t in db.Ticket
                                             join ttl in db.TicketTeamLogs on t.id equals ttl.ticketid
                                             join p in db.Project on t.projectid equals p.id
                                             join ti in db.TicketItem on t.id equals ti.ticketid
                                             join til in db.TicketItemLog on ti.id equals til.ticketitemid
                                             where t.IsArchieved == false && ttl.teamid == teamid && t.projectid > 0 && t.skillid > 0 &&
                                                   t.updatedonutc >= assignedFromDate && t.updatedonutc <= assignedToDate && p.id == projectid &&
                                                   til.assignedbyusersid == ByUserId && til.assignedtousersid == ToUserId
                                             select t).Include(t => t.TicketItems).Include(t => t.TicketItems.Select(ti => ti.TicketItemLog))
                .Distinct().OrderByDescending(t => t.id).ToList();

            return dashboardTickets;
        }

        private List<Ticket> fetchTicketsByClientId(long teamid, DateTime assignedFromDate, DateTime assignedToDate,
            long clientid)
        {
            List<Ticket> dashboardTickets = (from t in db.Ticket
                                             join ttl in db.TicketTeamLogs on t.id equals ttl.ticketid
                                             join p in db.Project on t.projectid equals p.id
                                             join ti in db.TicketItem on t.id equals ti.ticketid
                                             join til in db.TicketItemLog on ti.id equals til.ticketitemid
                                             where t.IsArchieved == false && ttl.teamid == teamid && t.projectid > 0 && t.skillid > 0 &&
                                                   t.updatedonutc >= assignedFromDate && t.updatedonutc <= assignedToDate &&
                                                   p.clientid == clientid
                                             select t).Include(t => t.TicketItems).Include(t => t.TicketItems.Select(ti => ti.TicketItemLog))
                .Distinct().OrderByDescending(t => t.id).ToList();

            return dashboardTickets;
        }

        private List<Ticket> fetchTicketsByClientIdAndToUserId(long teamid, DateTime assignedFromDate,
            DateTime assignedToDate, long clientid, string ToUserId)
        {
            List<Ticket> dashboardTickets = (from t in db.Ticket
                                             join ttl in db.TicketTeamLogs on t.id equals ttl.ticketid
                                             join p in db.Project on t.projectid equals p.id
                                             join ti in db.TicketItem on t.id equals ti.ticketid
                                             join til in db.TicketItemLog on ti.id equals til.ticketitemid
                                             where t.IsArchieved == false && ttl.teamid == teamid && t.projectid > 0 && t.skillid > 0 &&
                                                   t.updatedonutc >= assignedFromDate && t.updatedonutc <= assignedToDate &&
                                                   p.clientid == clientid && til.assignedtousersid == ToUserId
                                             select t).Include(t => t.TicketItems).Include(t => t.TicketItems.Select(ti => ti.TicketItemLog))
                .Distinct().OrderByDescending(t => t.id).ToList();

            return dashboardTickets;
        }

        private List<Ticket> fetchTicketsByClientIdAndByUserId(long teamid, DateTime assignedFromDate,
            DateTime assignedToDate, long clientid, string ByUserId)
        {
            List<Ticket> dashboardTickets = (from t in db.Ticket
                                             join ttl in db.TicketTeamLogs on t.id equals ttl.ticketid
                                             join p in db.Project on t.projectid equals p.id
                                             join ti in db.TicketItem on t.id equals ti.ticketid
                                             join til in db.TicketItemLog on ti.id equals til.ticketitemid
                                             where t.IsArchieved == false && ttl.teamid == teamid && t.projectid > 0 && t.skillid > 0 &&
                                                   t.updatedonutc >= assignedFromDate && t.updatedonutc <= assignedToDate &&
                                                   p.clientid == clientid && til.assignedbyusersid == ByUserId
                                             select t).Include(t => t.TicketItems).Include(t => t.TicketItems.Select(ti => ti.TicketItemLog))
                .Distinct().OrderByDescending(t => t.id).ToList();

            return dashboardTickets;
        }

        private List<Ticket> fetchTicketsByClientIdAndByUserIdAndToUserId(long teamid, DateTime assignedFromDate,
            DateTime assignedToDate, long clientid, string ByUserId, string ToUserId)
        {
            List<Ticket> dashboardTickets = (from t in db.Ticket
                                             join ttl in db.TicketTeamLogs on t.id equals ttl.ticketid
                                             join p in db.Project on t.projectid equals p.id
                                             join ti in db.TicketItem on t.id equals ti.ticketid
                                             join til in db.TicketItemLog on ti.id equals til.ticketitemid
                                             where t.IsArchieved == false && ttl.teamid == teamid && t.projectid > 0 && t.skillid > 0 &&
                                                   t.updatedonutc >= assignedFromDate && t.updatedonutc <= assignedToDate &&
                                                   p.clientid == clientid && til.assignedbyusersid == ByUserId &&
                                                   til.assignedtousersid == ToUserId
                                             select t).Include(t => t.TicketItems).Include(t => t.TicketItems.Select(ti => ti.TicketItemLog))
                .Distinct().OrderByDescending(t => t.id).ToList();

            return dashboardTickets;
        }

        private List<Ticket> fetchTicketsByClientIdAndProjectId(long teamid, DateTime assignedFromDate,
            DateTime assignedToDate, long clientid, long projectid)
        {
            List<Ticket> dashboardTickets = (from t in db.Ticket
                                             join ttl in db.TicketTeamLogs on t.id equals ttl.ticketid
                                             join p in db.Project on t.projectid equals p.id
                                             join ti in db.TicketItem on t.id equals ti.ticketid
                                             join til in db.TicketItemLog on ti.id equals til.ticketitemid
                                             where t.IsArchieved == false && ttl.teamid == teamid && t.projectid > 0 && t.skillid > 0 &&
                                                   t.updatedonutc >= assignedFromDate && t.updatedonutc <= assignedToDate &&
                                                   p.clientid == clientid && p.id == projectid
                                             select t).Include(t => t.TicketItems).Include(t => t.TicketItems.Select(ti => ti.TicketItemLog))
                .Distinct().OrderByDescending(t => t.id).ToList();

            return dashboardTickets;
        }

        private List<Ticket> fetchTicketsByClientIdAndProjectIdAndToUserId(long teamid, DateTime assignedFromDate,
            DateTime assignedToDate, long clientid, long projectid, string ToUserId)
        {
            List<Ticket> dashboardTickets = (from t in db.Ticket
                                             join ttl in db.TicketTeamLogs on t.id equals ttl.ticketid
                                             join p in db.Project on t.projectid equals p.id
                                             join ti in db.TicketItem on t.id equals ti.ticketid
                                             join til in db.TicketItemLog on ti.id equals til.ticketitemid
                                             where t.IsArchieved == false && ttl.teamid == teamid && t.projectid > 0 && t.skillid > 0 &&
                                                   t.updatedonutc >= assignedFromDate && t.updatedonutc <= assignedToDate &&
                                                   p.clientid == clientid && p.id == projectid && til.assignedtousersid == ToUserId
                                             select t).Include(t => t.TicketItems).Include(t => t.TicketItems.Select(ti => ti.TicketItemLog))
                .Distinct().OrderByDescending(t => t.id).ToList();

            return dashboardTickets;
        }

        private List<Ticket> fetchTicketsByClientIdAndProjectIdAndByUserId(long teamid, DateTime assignedFromDate,
            DateTime assignedToDate, long clientid, long projectid, string ByUserId)
        {
            List<Ticket> dashboardTickets = (from t in db.Ticket
                                             join ttl in db.TicketTeamLogs on t.id equals ttl.ticketid
                                             join p in db.Project on t.projectid equals p.id
                                             join ti in db.TicketItem on t.id equals ti.ticketid
                                             join til in db.TicketItemLog on ti.id equals til.ticketitemid
                                             where t.IsArchieved == false && ttl.teamid == teamid && t.projectid > 0 && t.skillid > 0 &&
                                                   t.updatedonutc >= assignedFromDate && t.updatedonutc <= assignedToDate &&
                                                   p.clientid == clientid && p.id == projectid && til.assignedbyusersid == ByUserId
                                             select t).Include(t => t.TicketItems).Include(t => t.TicketItems.Select(ti => ti.TicketItemLog))
                .Distinct().OrderByDescending(t => t.id).ToList();

            return dashboardTickets;
        }

        private List<Ticket> fetchTicketsByClientIdAndProjectIdAndByUserIdAndToUserId(long teamid,
            DateTime assignedFromDate, DateTime assignedToDate, long clientid, long projectid, string ByUserId,
            string ToUserId)
        {
            List<Ticket> dashboardTickets = (from t in db.Ticket
                                             join ttl in db.TicketTeamLogs on t.id equals ttl.ticketid
                                             join p in db.Project on t.projectid equals p.id
                                             join ti in db.TicketItem on t.id equals ti.ticketid
                                             join til in db.TicketItemLog on ti.id equals til.ticketitemid
                                             where t.IsArchieved == false && ttl.teamid == teamid && t.projectid > 0 && t.skillid > 0 &&
                                                   t.updatedonutc >= assignedFromDate && t.updatedonutc <= assignedToDate &&
                                                   p.clientid == clientid && p.id == projectid && til.assignedbyusersid == ByUserId &&
                                                   til.assignedtousersid == ToUserId
                                             select t).Include(t => t.TicketItems).Include(t => t.TicketItems.Select(ti => ti.TicketItemLog))
                .Distinct().OrderByDescending(t => t.id).ToList();

            return dashboardTickets;
        }

        private List<Ticket> fetchTicketsByTicketTitle(long teamid, DateTime assignedFromDate, DateTime assignedToDate,
            string ticketTitle)
        {
            List<Ticket> dashboardTickets = (from t in db.Ticket
                                             join ttl in db.TicketTeamLogs on t.id equals ttl.ticketid
                                             join p in db.Project on t.projectid equals p.id
                                             join ti in db.TicketItem on t.id equals ti.ticketid
                                             join til in db.TicketItemLog on ti.id equals til.ticketitemid
                                             where t.IsArchieved == false && ttl.teamid == teamid && t.projectid > 0 && t.skillid > 0 &&
                                                   t.updatedonutc >= assignedFromDate && t.updatedonutc <= assignedToDate &&
                                                   t.topic.Contains(ticketTitle)
                                             select t).Include(t => t.TicketItems).Include(t => t.TicketItems.Select(ti => ti.TicketItemLog))
                .Distinct().OrderByDescending(t => t.id).ToList();

            return dashboardTickets;
        }

        private List<Ticket> fetchTicketsByTicketTitleAndToUserId(long teamid, DateTime assignedFromDate,
            DateTime assignedToDate, string ticketTitle, string ToUserId)
        {
            List<Ticket> dashboardTickets = (from t in db.Ticket
                                             join ttl in db.TicketTeamLogs on t.id equals ttl.ticketid
                                             join p in db.Project on t.projectid equals p.id
                                             join ti in db.TicketItem on t.id equals ti.ticketid
                                             join til in db.TicketItemLog on ti.id equals til.ticketitemid
                                             where t.IsArchieved == false && ttl.teamid == teamid && t.projectid > 0 && t.skillid > 0 &&
                                                   t.updatedonutc >= assignedFromDate && t.updatedonutc <= assignedToDate &&
                                                   t.topic.Contains(ticketTitle) && til.assignedtousersid == ToUserId
                                             select t).Include(t => t.TicketItems).Include(t => t.TicketItems.Select(ti => ti.TicketItemLog))
                .Distinct().OrderByDescending(t => t.id).ToList();

            return dashboardTickets;
        }

        private List<Ticket> fetchTicketsByTicketTitleAndByUserId(long teamid, DateTime assignedFromDate,
            DateTime assignedToDate, string ticketTitle, string ByUserId)
        {
            List<Ticket> dashboardTickets = (from t in db.Ticket
                                             join ttl in db.TicketTeamLogs on t.id equals ttl.ticketid
                                             join p in db.Project on t.projectid equals p.id
                                             join ti in db.TicketItem on t.id equals ti.ticketid
                                             join til in db.TicketItemLog on ti.id equals til.ticketitemid
                                             where t.IsArchieved == false && ttl.teamid == teamid && t.projectid > 0 && t.skillid > 0 &&
                                                   t.updatedonutc >= assignedFromDate && t.updatedonutc <= assignedToDate &&
                                                   t.topic.Contains(ticketTitle) && til.assignedbyusersid == ByUserId
                                             select t).Include(t => t.TicketItems).Include(t => t.TicketItems.Select(ti => ti.TicketItemLog))
                .Distinct().OrderByDescending(t => t.id).ToList();

            return dashboardTickets;
        }

        private List<Ticket> fetchTicketsByTicketTitleAndByUserIdAndToUserId(long teamid, DateTime assignedFromDate,
            DateTime assignedToDate, string ticketTitle, string ByUserId, string ToUserId)
        {
            List<Ticket> dashboardTickets = (from t in db.Ticket
                                             join ttl in db.TicketTeamLogs on t.id equals ttl.ticketid
                                             join p in db.Project on t.projectid equals p.id
                                             join ti in db.TicketItem on t.id equals ti.ticketid
                                             join til in db.TicketItemLog on ti.id equals til.ticketitemid
                                             where t.IsArchieved == false && ttl.teamid == teamid && t.projectid > 0 && t.skillid > 0 &&
                                                   t.updatedonutc >= assignedFromDate && t.updatedonutc <= assignedToDate &&
                                                   t.topic.Contains(ticketTitle) && til.assignedbyusersid == ByUserId &&
                                                   til.assignedtousersid == ToUserId
                                             select t).Include(t => t.TicketItems).Include(t => t.TicketItems.Select(ti => ti.TicketItemLog))
                .Distinct().OrderByDescending(t => t.id).ToList();

            return dashboardTickets;
        }

        private List<Ticket> fetchTicketsByTicketTitleAndProjectId(long teamid, DateTime assignedFromDate,
            DateTime assignedToDate, string ticketTitle, long projectid)
        {
            List<Ticket> dashboardTickets = (from t in db.Ticket
                                             join ttl in db.TicketTeamLogs on t.id equals ttl.ticketid
                                             join p in db.Project on t.projectid equals p.id
                                             join ti in db.TicketItem on t.id equals ti.ticketid
                                             join til in db.TicketItemLog on ti.id equals til.ticketitemid
                                             where t.IsArchieved == false && ttl.teamid == teamid && t.projectid > 0 && t.skillid > 0 &&
                                                   t.updatedonutc >= assignedFromDate && t.updatedonutc <= assignedToDate &&
                                                   t.topic.Contains(ticketTitle) && p.id == projectid
                                             select t).Include(t => t.TicketItems).Include(t => t.TicketItems.Select(ti => ti.TicketItemLog))
                .Distinct().OrderByDescending(t => t.id).ToList();

            return dashboardTickets;
        }

        private List<Ticket> fetchTicketsByTicketTitleAndProjectIdAndToUserId(long teamid, DateTime assignedFromDate,
            DateTime assignedToDate, string ticketTitle, long projectid, string ToUserId)
        {
            List<Ticket> dashboardTickets = (from t in db.Ticket
                                             join ttl in db.TicketTeamLogs on t.id equals ttl.ticketid
                                             join p in db.Project on t.projectid equals p.id
                                             join ti in db.TicketItem on t.id equals ti.ticketid
                                             join til in db.TicketItemLog on ti.id equals til.ticketitemid
                                             where t.IsArchieved == false && ttl.teamid == teamid && t.projectid > 0 && t.skillid > 0 &&
                                                   t.updatedonutc >= assignedFromDate && t.updatedonutc <= assignedToDate &&
                                                   t.topic.Contains(ticketTitle) && p.id == projectid && til.assignedtousersid == ToUserId
                                             select t).Include(t => t.TicketItems).Include(t => t.TicketItems.Select(ti => ti.TicketItemLog))
                .Distinct().OrderByDescending(t => t.id).ToList();

            return dashboardTickets;
        }

        private List<Ticket> fetchTicketsByTicketTitleAndProjectIdAndByUserId(long teamid, DateTime assignedFromDate,
            DateTime assignedToDate, string ticketTitle, long projectid, string ByUserId)
        {
            List<Ticket> dashboardTickets = (from t in db.Ticket
                                             join ttl in db.TicketTeamLogs on t.id equals ttl.ticketid
                                             join p in db.Project on t.projectid equals p.id
                                             join ti in db.TicketItem on t.id equals ti.ticketid
                                             join til in db.TicketItemLog on ti.id equals til.ticketitemid
                                             where t.IsArchieved == false && ttl.teamid == teamid && t.projectid > 0 && t.skillid > 0 &&
                                                   t.updatedonutc >= assignedFromDate && t.updatedonutc <= assignedToDate &&
                                                   t.topic.Contains(ticketTitle) && p.id == projectid && til.assignedbyusersid == ByUserId
                                             select t).Include(t => t.TicketItems).Include(t => t.TicketItems.Select(ti => ti.TicketItemLog))
                .Distinct().OrderByDescending(t => t.id).ToList();

            return dashboardTickets;
        }

        private List<Ticket> fetchTicketsByTicketTitleAndProjectIdAndByUserIdAndToUserId(long teamid,
            DateTime assignedFromDate, DateTime assignedToDate, string ticketTitle, long projectid, string ByUserId,
            string ToUserId)
        {
            List<Ticket> dashboardTickets = (from t in db.Ticket
                                             join ttl in db.TicketTeamLogs on t.id equals ttl.ticketid
                                             join p in db.Project on t.projectid equals p.id
                                             join ti in db.TicketItem on t.id equals ti.ticketid
                                             join til in db.TicketItemLog on ti.id equals til.ticketitemid
                                             where t.IsArchieved == false && ttl.teamid == teamid && t.projectid > 0 && t.skillid > 0 &&
                                                   t.updatedonutc >= assignedFromDate && t.updatedonutc <= assignedToDate &&
                                                   t.topic.Contains(ticketTitle) && p.id == projectid && til.assignedbyusersid == ByUserId &&
                                                   til.assignedtousersid == ToUserId
                                             select t).Include(t => t.TicketItems).Include(t => t.TicketItems.Select(ti => ti.TicketItemLog))
                .Distinct().OrderByDescending(t => t.id).ToList();

            return dashboardTickets;
        }

        private List<Ticket> fetchTicketsByTicketTitleAndClientId(long teamid, DateTime assignedFromDate,
            DateTime assignedToDate, string ticketTitle, long clientid)
        {
            List<Ticket> dashboardTickets = (from t in db.Ticket
                                             join ttl in db.TicketTeamLogs on t.id equals ttl.ticketid
                                             join p in db.Project on t.projectid equals p.id
                                             join ti in db.TicketItem on t.id equals ti.ticketid
                                             join til in db.TicketItemLog on ti.id equals til.ticketitemid
                                             where t.IsArchieved == false && ttl.teamid == teamid && t.projectid > 0 && t.skillid > 0 &&
                                                   t.updatedonutc >= assignedFromDate && t.updatedonutc <= assignedToDate &&
                                                   t.topic.Contains(ticketTitle) && p.clientid == clientid
                                             select t).Include(t => t.TicketItems).Include(t => t.TicketItems.Select(ti => ti.TicketItemLog))
                .Distinct().OrderByDescending(t => t.id).ToList();

            return dashboardTickets;
        }

        private List<Ticket> fetchTicketsByTicketTitleAndClientIdAndByUserId(long teamid, DateTime assignedFromDate,
            DateTime assignedToDate, string ticketTitle, long clientid, string ByUserId)
        {
            List<Ticket> dashboardTickets = (from t in db.Ticket
                                             join ttl in db.TicketTeamLogs on t.id equals ttl.ticketid
                                             join p in db.Project on t.projectid equals p.id
                                             join ti in db.TicketItem on t.id equals ti.ticketid
                                             join til in db.TicketItemLog on ti.id equals til.ticketitemid
                                             where t.IsArchieved == false && ttl.teamid == teamid && t.projectid > 0 && t.skillid > 0 &&
                                                   t.updatedonutc >= assignedFromDate && t.updatedonutc <= assignedToDate &&
                                                   t.topic.Contains(ticketTitle) && p.clientid == clientid && til.assignedbyusersid == ByUserId
                                             select t).Include(t => t.TicketItems).Include(t => t.TicketItems.Select(ti => ti.TicketItemLog))
                .Distinct().OrderByDescending(t => t.id).ToList();

            return dashboardTickets;
        }

        private List<Ticket> fetchTicketsByTicketTitleAndClientIdAndToUserId(long teamid, DateTime assignedFromDate,
            DateTime assignedToDate, string ticketTitle, long clientid, string ToUserId)
        {
            List<Ticket> dashboardTickets = (from t in db.Ticket
                                             join ttl in db.TicketTeamLogs on t.id equals ttl.ticketid
                                             join p in db.Project on t.projectid equals p.id
                                             join ti in db.TicketItem on t.id equals ti.ticketid
                                             join til in db.TicketItemLog on ti.id equals til.ticketitemid
                                             where t.IsArchieved == false && ttl.teamid == teamid && t.projectid > 0 && t.skillid > 0 &&
                                                   t.updatedonutc >= assignedFromDate && t.updatedonutc <= assignedToDate &&
                                                   t.topic.Contains(ticketTitle) && p.clientid == clientid && til.assignedtousersid == ToUserId
                                             select t).Include(t => t.TicketItems).Include(t => t.TicketItems.Select(ti => ti.TicketItemLog))
                .Distinct().OrderByDescending(t => t.id).ToList();

            return dashboardTickets;
        }

        private List<Ticket> fetchTicketsByTicketTitleAndClientIdAndByUserIdAndToUserId(long teamid,
            DateTime assignedFromDate, DateTime assignedToDate, string ticketTitle, long clientid, string ByUserId,
            string ToUserId)
        {
            List<Ticket> dashboardTickets = (from t in db.Ticket
                                             join ttl in db.TicketTeamLogs on t.id equals ttl.ticketid
                                             join p in db.Project on t.projectid equals p.id
                                             join ti in db.TicketItem on t.id equals ti.ticketid
                                             join til in db.TicketItemLog on ti.id equals til.ticketitemid
                                             where t.IsArchieved == false && ttl.teamid == teamid && t.projectid > 0 && t.skillid > 0 &&
                                                   t.updatedonutc >= assignedFromDate && t.updatedonutc <= assignedToDate &&
                                                   t.topic.Contains(ticketTitle) && p.clientid == clientid &&
                                                   til.assignedbyusersid == ByUserId &&
                                                   til.assignedtousersid == ToUserId
                                             select t).Include(t => t.TicketItems).Include(t => t.TicketItems.Select(ti => ti.TicketItemLog))
                .Distinct().OrderByDescending(t => t.id).ToList();

            return dashboardTickets;
        }

        private List<Ticket> fetchTicketsByTicketTitleAndClientIdAndProjectId(long teamid, DateTime assignedFromDate,
            DateTime assignedToDate, string ticketTitle, long clientid, long projectid)
        {
            List<Ticket> dashboardTickets = (from t in db.Ticket
                                             join ttl in db.TicketTeamLogs on t.id equals ttl.ticketid
                                             join p in db.Project on t.projectid equals p.id
                                             join ti in db.TicketItem on t.id equals ti.ticketid
                                             join til in db.TicketItemLog on ti.id equals til.ticketitemid
                                             where t.IsArchieved == false && ttl.teamid == teamid && t.projectid > 0 && t.skillid > 0 &&
                                                   t.updatedonutc >= assignedFromDate && t.updatedonutc <= assignedToDate &&
                                                   t.topic.Contains(ticketTitle) && p.clientid == clientid && p.id == projectid
                                             select t).Include(t => t.TicketItems).Include(t => t.TicketItems.Select(ti => ti.TicketItemLog))
                .Distinct().OrderByDescending(t => t.id).ToList();

            return dashboardTickets;
        }

        private List<Ticket> fetchTicketsByTicketTitleAndClientIdAndProjectIdAndByUserId(long teamid,
            DateTime assignedFromDate, DateTime assignedToDate, string ticketTitle, long clientid, long projectid,
            string ByUserId)
        {
            List<Ticket> dashboardTickets = (from t in db.Ticket
                                             join ttl in db.TicketTeamLogs on t.id equals ttl.ticketid
                                             join p in db.Project on t.projectid equals p.id
                                             join ti in db.TicketItem on t.id equals ti.ticketid
                                             join til in db.TicketItemLog on ti.id equals til.ticketitemid
                                             where t.IsArchieved == false && ttl.teamid == teamid && t.projectid > 0 && t.skillid > 0 &&
                                                   t.updatedonutc >= assignedFromDate && t.updatedonutc <= assignedToDate &&
                                                   t.topic.Contains(ticketTitle) && p.clientid == clientid && p.id == projectid &&
                                                   til.assignedbyusersid == ByUserId
                                             select t).Include(t => t.TicketItems).Include(t => t.TicketItems.Select(ti => ti.TicketItemLog))
                .Distinct().OrderByDescending(t => t.id).ToList();

            return dashboardTickets;
        }

        private List<Ticket> fetchTicketsByTicketTitleAndClientIdAndProjectIdAndToUserId(long teamid,
            DateTime assignedFromDate, DateTime assignedToDate, string ticketTitle, long clientid, long projectid,
            string ToUserId)
        {
            List<Ticket> dashboardTickets = (from t in db.Ticket
                                             join ttl in db.TicketTeamLogs on t.id equals ttl.ticketid
                                             join p in db.Project on t.projectid equals p.id
                                             join ti in db.TicketItem on t.id equals ti.ticketid
                                             join til in db.TicketItemLog on ti.id equals til.ticketitemid
                                             where t.IsArchieved == false && ttl.teamid == teamid && t.projectid > 0 && t.skillid > 0 &&
                                                   t.updatedonutc >= assignedFromDate && t.updatedonutc <= assignedToDate &&
                                                   t.topic.Contains(ticketTitle) && p.clientid == clientid && p.id == projectid &&
                                                   til.assignedtousersid == ToUserId
                                             select t).Include(t => t.TicketItems).Include(t => t.TicketItems.Select(ti => ti.TicketItemLog))
                .Distinct().OrderByDescending(t => t.id).ToList();

            return dashboardTickets;
        }

        private List<Ticket> fetchTicketsByTicketTitleAndClientIdAndProjectIdAndByUserIdAndToUserId(long teamid,
            DateTime assignedFromDate, DateTime assignedToDate, string ticketTitle, long clientid, long projectid,
            string ByUserId, string ToUserId)
        {
            List<Ticket> dashboardTickets = (from t in db.Ticket
                                             join ttl in db.TicketTeamLogs on t.id equals ttl.ticketid
                                             join p in db.Project on t.projectid equals p.id
                                             join ti in db.TicketItem on t.id equals ti.ticketid
                                             join til in db.TicketItemLog on ti.id equals til.ticketitemid
                                             where t.IsArchieved == false && ttl.teamid == teamid && t.projectid > 0 && t.skillid > 0 &&
                                                   t.updatedonutc >= assignedFromDate && t.updatedonutc <= assignedToDate &&
                                                   t.topic.Contains(ticketTitle) && p.clientid == clientid && p.id == projectid &&
                                                   til.assignedbyusersid == ByUserId && til.assignedtousersid == ToUserId
                                             select t).Include(t => t.TicketItems).Include(t => t.TicketItems.Select(ti => ti.TicketItemLog))
                .Distinct().OrderByDescending(t => t.id).ToList();

            return dashboardTickets;
        }

        public ActionResult Workloadreport()
        {
            List<WorkloadReport> workloadlist = new List<WorkloadReport>();
            List<TeamMember> teamMembers = (from t in db.Team
                                            join tm in db.TeamMember on t.id equals tm.teamid
                                            where t.isactive == true && tm.IsActive == true && t.id != 2 && t.id != 3
                                            select tm).ToList();
            foreach (TeamMember teamUser in teamMembers)
            {
                WorkloadReport workload = db.Database.SqlQuery<WorkloadReport>(
                    "GetUserWorkLoad_sp @TeamId, @UserId",
                    new SqlParameter("TeamId", teamUser.teamid),
                    new SqlParameter("UserId", teamUser.usersid)
                ).FirstOrDefault();
                workloadlist.Add(workload);
            }

            //List<core.Team> teamlist = db.Team.Where(x => x.isactive == true && x.name != "Management" && x.id != 2).ToList();
            //// 2. Filter tickets based on primary selection along with one month date range.
            //DateTime assignedFromDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0).AddMonths(-2);
            //DateTime assignedToDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);

            //if (teamlist != null)
            //{
            //    foreach (core.Team teamitem in teamlist)
            //    {//&& x.IsActive== true
            //        List<TeamMember> teammemberlist = db.TeamMember.Where(x => x.teamid == teamitem.id && x.IsActive == true).OrderBy(x => x.User.FirstName).ToList();
            //        //List<TeamMember> teammemberlist = db.TeamMember.Where(x => x.teamid == teamitem.id).OrderBy(x => x.User.FirstName).ToList();
            //        if (teammemberlist != null)
            //        {
            //            foreach (TeamMember memberitem in teammemberlist)
            //            {
            //                WorkloadReport workload = new WorkloadReport();
            //                int Inprogress = (from t in db.Ticket
            //                                  join ttl in db.TicketTeamLogs on t.id equals ttl.ticketid
            //                                  join p in db.Project on t.projectid equals p.id
            //                                  join ti in db.TicketItem on t.id equals ti.ticketid
            //                                  join til in db.TicketItemLog on ti.id equals til.ticketitemid
            //                                  where ttl.teamid == teamitem.id && t.statusid == 2 && til.assignedtousersid == memberitem.usersid// && (til.assignedon >= assignedFromDate && til.assignedon <= assignedToDate)
            //                                  select t).Count();
            //                //var done = (from t in db.Ticket
            //                //                  join ttl in db.TicketTeamLogs on t.id equals ttl.ticketid
            //                //                  join p in db.Project on t.projectid equals p.id
            //                //                  join ti in db.TicketItem on t.id equals ti.ticketid
            //                //                  join til in db.TicketItemLog on ti.id equals til.ticketitemid
            //                //                  where ttl.teamid == teamitem.id && t.statusid == 3 && til.assignedtousersid == memberitem.usersid //&& (til.assignedon >= assignedFromDate && til.assignedon <= assignedToDate)
            //                //                  select t).Count();
            //                //var OnHold = (from t in db.Ticket
            //                //            join ttl in db.TicketTeamLogs on t.id equals ttl.ticketid
            //                //            join p in db.Project on t.projectid equals p.id
            //                //            join ti in db.TicketItem on t.id equals ti.ticketid
            //                //            join til in db.TicketItemLog on ti.id equals til.ticketitemid
            //                //            where ttl.teamid == teamitem.id && t.statusid == 4 && til.assignedtousersid == memberitem.usersid && (til.assignedon >= assignedFromDate && til.assignedon <= assignedToDate)
            //                //            select t).Count();
            //                //var QC = (from t in db.Ticket
            //                //              join ttl in db.TicketTeamLogs on t.id equals ttl.ticketid
            //                //              join p in db.Project on t.projectid equals p.id
            //                //              join ti in db.TicketItem on t.id equals ti.ticketid
            //                //              join til in db.TicketItemLog on ti.id equals til.ticketitemid
            //                //              where ttl.teamid == teamitem.id && t.statusid == 5 && til.assignedtousersid == memberitem.usersid && (til.assignedon >= assignedFromDate && til.assignedon <= assignedToDate)
            //                //  select t).Count();
            //                int Assigned = (from t in db.Ticket
            //                                join ttl in db.TicketTeamLogs on t.id equals ttl.ticketid
            //                                join p in db.Project on t.projectid equals p.id
            //                                join ti in db.TicketItem on t.id equals ti.ticketid
            //                                join til in db.TicketItemLog on ti.id equals til.ticketitemid
            //                                where ttl.teamid == teamitem.id && t.statusid == 6 && til.assignedtousersid == memberitem.usersid// && (til.assignedon >= assignedFromDate && til.assignedon <= assignedToDate)
            //                                select t).Count();
            //                //var InReview = (from t in db.Ticket
            //                //                join ttl in db.TicketTeamLogs on t.id equals ttl.ticketid
            //                //                join p in db.Project on t.projectid equals p.id
            //                //                join ti in db.TicketItem on t.id equals ti.ticketid
            //                //                join til in db.TicketItemLog on ti.id equals til.ticketitemid
            //                //                where ttl.teamid == teamitem.id && t.statusid == 7 && til.assignedtousersid == memberitem.usersid && (til.assignedon >= assignedFromDate && til.assignedon <= assignedToDate)
            //                //                select t).Count();
            //                //var Trash = (from t in db.Ticket
            //                //                join ttl in db.TicketTeamLogs on t.id equals ttl.ticketid
            //                //                join p in db.Project on t.projectid equals p.id
            //                //                join ti in db.TicketItem on t.id equals ti.ticketid
            //                //                join til in db.TicketItemLog on ti.id equals til.ticketitemid
            //                //                where ttl.teamid == teamitem.id && t.statusid == 8 && til.assignedtousersid == memberitem.usersid && (til.assignedon >= assignedFromDate && til.assignedon <= assignedToDate)
            //                //                select t).Count();

            //                //(from til in db.TicketItemLog
            //                //                   join user in db.Users on til.assignedtousersid equals user.Id
            //                //                   where user.isactive == true && til.statusid == 2 & til.assignedtousersid== memberitem.usersid
            //                //                   select til).Count();
            //                // db.TicketItemLog.Where(x => x.assignedtousersid == memberitem.usersid && x.statusid== 2).Count();//&& x.assignedon >= assignedFromDate && x.assignedon <= assignedToDate
            //                //var ticketclosecount = (from til in db.TicketItemLog
            //                //                        join user in db.Users on til.assignedtousersid equals user.Id
            //                //                        where til.statusupdatedbyusersid == memberitem.usersid &&  user.isactive == true && til.statusid == 3 && til.statusupdatedon >= assignedFromDate && til.statusupdatedon <= assignedToDate
            //                //                        select til).Count();
            //                // db.TicketItemLog.Where(x => x.assignedbyusersid == memberitem.usersid && x.statusid == 3 && x.statusupdatedon >= assignedFromDate && x.statusupdatedon <= assignedToDate).Count();
            //                ApplicationUser userAdmin = UserManager.FindById(Convert.ToString(memberitem.usersid));
            //                core.Team reportteamid = db.Team.Where(x => x.id == memberitem.teamid).FirstOrDefault();
            //                workload.Inprogress = Inprogress;
            //                //  workload.Done = done;
            //                //workload.OnHold = OnHold;
            //                //workload.QC = QC;
            //                workload.Assigned = Assigned;
            //                //workload.InReview = InReview;
            //                //workload.Trash = Trash;
            //                workload.username = userAdmin.FirstName + " " + userAdmin.LastName;
            //                workload.teamname = reportteamid.name;
            //                workload.teamid = reportteamid.id;
            //                workloadlist.Add(workload);
            //            }
            //        }
            //    }
            //}
            FinalWorkloadRepot report = new FinalWorkloadRepot
            {
                WorkloadReport = workloadlist.OrderBy(x => x.username).ToList(),
                Teams = db.Team.Where(x => x.id != 3 && x.id != 2 && x.isactive).ToList()
            };
            return View(report);
        }

        public ActionResult FetchEstimatetasktime(long ticketid)
        {
            int estimatetime = db.TicketEstimateTimeLog.Where(x => x.ticketid == ticketid)
                .OrderByDescending(x => x.updatedonutc).Select(x => x.timeestimateinminutes).FirstOrDefault();
            return Json(new { estimatetime }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult LoadTicketEstimateTimeMeta(long TicketId)
        {
            List<TicketEstimateTimeLog> ticketTimeEstimates = db.TicketEstimateTimeLog.Where(x => x.ticketid == TicketId)
                .OrderBy(x => x.updatedonutc).ToList();
            Ticket ticketdate = db.Ticket.Find(TicketId);
            if (ticketTimeEstimates != null && ticketTimeEstimates.Count() > 0)
            {
                return Json(
                    new
                    {
                        error = false,
                        TicketMeta = ticketTimeEstimates,
                        ticketdate = ticketdate.createdonutc.ToString("yyyy-MM-dd hh:mm tt")
                    }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { error = true, errortext = "Sorry! No Ticket Estimate Time Meta found" },
                JsonRequestBehavior.AllowGet);
        }

        public ActionResult Ticketitemtimelog(long ticketid)
        {
            List<TicketTimeLog> tickettimelog = (from t in db.Ticket
                                                 join ti in db.TicketItem on t.id equals ti.ticketid
                                                 join ttl in db.TicketTimeLog on ti.id equals ttl.ticketitemid
                                                 where t.id == ticketid
                                                 select ttl).OrderByDescending(t => t.workdate).Include(t => t.TeamUser).ToList();
            return Json(new { error = 0, errortext = "Successfully!", Logs = tickettimelog },
                JsonRequestBehavior.AllowGet);
        }

        public ActionResult TicketEstimatTimeReport(long ticketid)
        {
            List<TicketTimeLog> tickettimelogs = (from t in db.Ticket
                                                  join ti in db.TicketItem on t.id equals ti.ticketid
                                                  join ttl in db.TicketTimeLog on ti.id equals ttl.ticketitemid
                                                  where t.id == ticketid
                                                  select ttl).OrderBy(t => t.workdate).Include(t => t.TeamUser).ToList();
            List<TicketEstimateTimeLog> ticketTimeEstimates = db.TicketEstimateTimeLog.Where(x => x.ticketid == ticketid)
                .OrderBy(x => x.updatedonutc).ToList();
            List<TicketTimeReportViewModels> timeReportList = new List<TicketTimeReportViewModels>();
            for (int i = 0; i < ticketTimeEstimates.Count; i++)
            {
                DateTime? EstimateDate1 = ticketTimeEstimates[i].updatedonutc;
                DateTime? EstimateDate2 = i + 1 < ticketTimeEstimates.Count ? ticketTimeEstimates[i + 1].updatedonutc : null;
                if (i == 0)
                {
                    List<TicketTimeLog> ttl = tickettimelogs.Where(x => x.workdate < EstimateDate1).ToList();
                    foreach (TicketTimeLog ticketTime in ttl)
                    {
                        TicketTimeReportViewModels ticketTimeReport = new TicketTimeReportViewModels
                        {
                            WorkDate = ticketTime.workdate,
                            FullName = ticketTime.TeamUser.FullName,
                            SpendTime = ticketTime.timespentinminutes,
                            BillableTime = ticketTime.billabletimeinminutes
                        };
                        timeReportList.Add(ticketTimeReport);
                    }
                }

                TicketTimeReportViewModels ttr = new TicketTimeReportViewModels
                {
                    EstimateDate = EstimateDate1,
                    EstimateTime = ticketTimeEstimates[i].timeestimateinminutes
                };
                timeReportList.Add(ttr);
                List<TicketTimeLog> ticketTimeLog = new List<TicketTimeLog>();
                if (EstimateDate2 != null)
                {
                    ticketTimeLog = tickettimelogs.Where(x => x.workdate >= EstimateDate1 && x.workdate < EstimateDate2)
                        .ToList();
                }
                else
                {
                    ticketTimeLog = tickettimelogs.Where(x => x.workdate >= EstimateDate1).ToList();
                }

                foreach (TicketTimeLog ticketTime in ticketTimeLog)
                {
                    TicketTimeReportViewModels ticketTimeReport = new TicketTimeReportViewModels
                    {
                        WorkDate = ticketTime.workdate,
                        FullName = ticketTime.TeamUser.FullName,
                        SpendTime = ticketTime.timespentinminutes,
                        BillableTime = ticketTime.billabletimeinminutes
                    };
                    timeReportList.Add(ticketTimeReport);
                }
            }

            int totalSpendTime = tickettimelogs.Sum(x => Convert.ToInt32(x.timespentinminutes));
            int totalBillableTime = tickettimelogs.Sum(x => Convert.ToInt32(x.billabletimeinminutes));
            if (timeReportList != null && timeReportList.Count() > 0)
            {
                return Json(new { error = false, timeReportList, totalSpendTime, totalBillableTime },
                    JsonRequestBehavior.AllowGet);
            }

            return Json(new { error = true, errortext = "Sorry! Ticket Estimate Time record not found" },
                JsonRequestBehavior.AllowGet);
        }

        public ActionResult Estimatetasktime()
        {
            EstimateTimeViewModels usersTaskTime = new EstimateTimeViewModels
            {
                searchViewModels = new TaskSearchViewModels
                {
                    StartDate = DateTime.Now.Date.Add(TimeSpan.Parse("00:00:00")),
                    EndDate = DateTime.Now.Date.Add(TimeSpan.Parse("23:59:59")),
                    userid = null,
                    UsersCollection = UserManager.Users.Where(x => x.isactive == true).OrderBy(u => u.FirstName).Select(
                        u => new SelectListItem
                        {
                            Text = u.FirstName + " " + u.LastName + " - " + u.Email,
                            Value = u.Id
                        }).ToList()
                }
            };
            usersTaskTime.taskTimeView = db.Database.SqlQuery<TaskTimeViewModels>(
                "GetTicketEstimateTime_sp @StartDate, @EndDate, @userId",
                new SqlParameter("StartDate", usersTaskTime.searchViewModels.StartDate),
                new SqlParameter("EndDate", usersTaskTime.searchViewModels.EndDate),
                new SqlParameter("userId", DBNull.Value)
            ).ToList();
            return View(usersTaskTime);
        }

        [HttpPost]
        public ActionResult Estimatetasktime(
            [Bind(Include = "StartDate,EndDate,userid")] TaskSearchViewModels searchViewModels)
        {
            if (ModelState.IsValid)
            {
                EstimateTimeViewModels timeVM = new EstimateTimeViewModels
                {
                    searchViewModels = new TaskSearchViewModels
                    {
                        StartDate = searchViewModels.StartDate,
                        EndDate = searchViewModels.EndDate,
                        userid = searchViewModels.userid,
                        UsersCollection = UserManager.Users.Where(x => x.isactive == true).OrderBy(u => u.FirstName)
                            .Select(u => new SelectListItem
                            {
                                Text = u.FirstName + " " + u.LastName + " - " + u.Email,
                                Value = u.Id
                            }).ToList()
                    }
                };
                timeVM.taskTimeView = db.Database.SqlQuery<TaskTimeViewModels>(
                    "GetTicketEstimateTime_sp @StartDate, @EndDate, @userId",
                    new SqlParameter("StartDate", searchViewModels.StartDate),
                    new SqlParameter("EndDate", searchViewModels.EndDate),
                    new SqlParameter("userId", searchViewModels.userid ?? (object)DBNull.Value)
                ).ToList();
                return View(timeVM);
            }

            EstimateTimeViewModels usersTaskTime = new EstimateTimeViewModels
            {
                searchViewModels = new TaskSearchViewModels
                {
                    StartDate = DateTime.Now.Date.Add(TimeSpan.Parse("00:00:00")),
                    EndDate = DateTime.Now.Date.Add(TimeSpan.Parse("23:59:59")),
                    userid = null,
                    UsersCollection = UserManager.Users.Where(x => x.isactive == true).OrderBy(u => u.FirstName).Select(
                        u => new SelectListItem
                        {
                            Text = u.FirstName + " " + u.LastName + " - " + u.Email,
                            Value = u.Id
                        }).ToList()
                }
            };
            usersTaskTime.taskTimeView = db.Database.SqlQuery<TaskTimeViewModels>(
                "GetTicketEstimateTime_sp @StartDate, @EndDate, @userId",
                new SqlParameter("StartDate", usersTaskTime.searchViewModels.StartDate),
                new SqlParameter("EndDate", usersTaskTime.searchViewModels.EndDate),
                new SqlParameter("userId", DBNull.Value)
            ).ToList();
            return View(usersTaskTime);
        }

        public ActionResult FetchUsersAndTeams(long? Ticketid)
        {
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
            List<SelectListItem> ProjectSelectList = db.Database.SqlQuery<CombinedEntity>("exec projects_loadcombined").Select(x =>
                new SelectListItem
                {
                    Text = x.name,
                    Value = x.id.ToString()
                }).ToList();
            List<Skill> skills = db.Skill.ToList();
            if (users != null || teams != null)
            {
                return Json(new { error = 0, Users = users, Teams = teams, skill = skills, project = ProjectSelectList },
                    JsonRequestBehavior.AllowGet);
            }

            return Json(new { error = 1, TextContext = "Sorry! no ticket found with this User or Team" },
                JsonRequestBehavior.AllowGet);
        }

        public JsonResult PrefetchSingleTeams(string UserID)
        {
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
                    errortext = user.FirstName +" "+ user.LastName + "is not related to any Team  please assign a team and try again!"
                }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AddProject(long clientid, string projectname)
        {
            Project project = new Project();
            try
            {
                Project projectdup = db.Project.Where(t => t.name == projectname).FirstOrDefault();
                if (projectdup != null)
                {
                    return Json(new { error = true, message = "Sorry,project already exist." });
                }

                project.createdonutc = DateTime.Now;
                project.updatedonutc = DateTime.Now;
                project.ipused = Request.UserHostAddress;
                project.userid = User.Identity.GetUserId();
                project.clientid = clientid;
                project.name = projectname;
                project.isactive = true;
                project.iswarning = false;
                project.startdate = DateTime.Now;
                db.Project.Add(project);
                db.SaveChanges();

                return Json(new
                { error = false, message = "Successfully Added", prid = project.id, prname = project.name });
            }
            catch (Exception ex)
            {
                return Json(new { error = true, message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult AddSkill(string name)
        {
            Skill skill = new Skill();
            try
            {
                Skill skillup = db.Skill.Where(t => t.name == name).FirstOrDefault();
                if (skillup != null)
                {
                    return Json(new { error = true, message = "Sorry,Skill already exist." });
                }

                skill.createdonutc = DateTime.Now;
                skill.updatedonutc = DateTime.Now;
                skill.ipused = Request.UserHostAddress;
                skill.userid = User.Identity.GetUserId();
                skill.name = name;
                skill.isactive = true;

                db.Skill.Add(skill);
                db.SaveChanges();

                return Json(new { error = false, message = "Successfully Added", skid = skill.id, skname = skill.name });
            }
            catch (Exception ex)
            {
                return Json(new { error = true, message = ex.Message });
            }
        }

        //Changing the team of user
        public ActionResult ChangeTeam(int teamid, long ticketid, int newteamid)
        {
            // Make sure the ticket is not already in new team.
            TicketTeamLogs NewTeamLogs = db.TicketTeamLogs.Where(ttl => ttl.teamid == newteamid && ttl.ticketid == ticketid)
                .FirstOrDefault();
            if (NewTeamLogs != null)
            {
                return Json(new { error = true, TextContext = "Already assigned to this team" },
                    JsonRequestBehavior.AllowGet);
            }

            Ticket ticket = db.Ticket.Where(t => t.id == ticketid).Include(t => t.TicketItems)
                .Include(t => t.TicketItems.Select(ti => ti.TicketItemLog)).FirstOrDefault();

            if (ticket != null)
            {
                ticket.statusid = 1; //Assigned
                ticket.lastmodifiedtime = DateTime.Now;
                ticket.statusupdatedbyusersid = User.Identity.GetUserId();
                ticket.statusupdatedon = DateTime.Now;
                ticket.updatedonutc = DateTime.Now;
                ticket.ipused = Request.UserHostAddress;
                ticket.userid = User.Identity.GetUserId();
                if (ticket.TicketAssignedToCollection != null)
                {
                    db.TicketItemLog.RemoveRange(ticket.TicketAssignedToCollection);
                }

                // update current team record.
                TicketTeamLogs CurrentTeamLog = db.TicketTeamLogs.Where(ttl => ttl.teamid == teamid && ttl.ticketid == ticketid)
                    .FirstOrDefault();
                CurrentTeamLog.teamid = newteamid;
                CurrentTeamLog.assignedbyusersid = User.Identity.GetUserId();
                CurrentTeamLog.assignedon = DateTime.Now;
                CurrentTeamLog.statusupdatedbyusersid = User.Identity.GetUserId();
                CurrentTeamLog.statusupdatedon = DateTime.Now;
                db.SaveChanges();

                return Json(new { error = false, TextContext = "Ticket has been moved to selected team successfully." },
                    JsonRequestBehavior.AllowGet);
            }

            return Json(new { error = true, TextContext = "Sorry, the ticket does not exists." },
                JsonRequestBehavior.AllowGet);
        }

        public ActionResult MyTasks()
        {
            string usersid = User.Identity.GetUserId();
            TeamMember teammemberObject = db.TeamMember.Where(tm => tm.usersid == usersid).FirstOrDefault();
            long teamid = teammemberObject.teamid;
            return RedirectToAction("team", new { id = teamid });
        }

        public ActionResult MyTasksAjax(string AssignmentFromDate, string AssignmentToDate)
        {
            if (!User.IsInRole(Role.Admin.ToString()))
            {
                return RedirectToAction("Index");
            }

            List<ApplicationUser> users = db.Users.ToList();
            TeamDashboardViewModel model = new TeamDashboardViewModel();

            string userid = User.Identity.GetUserId();

            var tempdata = (from t in db.Ticket
                            join ti in db.TicketItem on t.id equals ti.ticketid
                            join til in db.TicketItemLog on ti.id equals til.ticketitemid
                            join ttl in db.TicketTeamLogs on t.id equals ttl.ticketid
                            select new
                            {
                                ttl.teamid,
                                ti.projectid,
                                ti.skillid,
                                ttl.assignedbyusersid,
                                til.assignedtousersid, //????
                                ttl.assignedon,
                                t.id,
                                t.topic,
                                t.statusid,
                                t.createdonutc,
                                t.startdate,
                                t.enddate
                            }).Distinct().OrderByDescending(t => t.id).ToList()
                .Where(x => x.projectid > 0 && x.skillid > 0 && x.assignedtousersid.Equals(userid)).ToList();

            List<TeamDashboardDataViewModel> tickets = (from td in tempdata
                                                        join assingedBy in users on td.assignedbyusersid equals assingedBy.Id
                                                        join assignedTo in users on td.assignedtousersid equals assignedTo.Id
                                                        select new TeamDashboardDataViewModel
                                                        {
                                                            id = td.id,
                                                            topic = td.topic,
                                                            assignedbyusersid = td.assignedbyusersid,
                                                            assignedtousersid = td.assignedtousersid,
                                                            assignedbyuserName = assingedBy.FirstName,
                                                            assignedtouserName = assignedTo.FirstName,
                                                            statusid = td.statusid,
                                                            createdonutc = td.createdonutc,
                                                            assignedon = td.assignedon,
                                                            startdate = td.startdate,
                                                            enddate = td.enddate
                                                        }).ToList();

            if (tickets.Count() == 0)
            {
                model.tickets = new List<TeamDashboardDataViewModel>();
            }
            else
            {
                model.tickets = tickets.ToList();
            }

            if (tickets.Count() > 0)
            {
                model.tickets = model.tickets.GroupBy(x => x.id).Select(x => x.First()).ToList();
            }

            if (!string.IsNullOrEmpty(AssignmentFromDate) && !string.IsNullOrEmpty(AssignmentToDate))
            {
                DateTime from = Convert.ToDateTime(AssignmentFromDate);
                DateTime to = Convert.ToDateTime(AssignmentToDate);
                model.tickets = model.tickets.Where(x => x.assignedon.Date >= from.Date && x.assignedon.Date <= to.Date)
                    .ToList();
            }

            return PartialView(model);
        }

        public ActionResult Showfullname()
        {
            UserFullName name = new UserFullName();
            ApplicationUser userinfo = (ApplicationUser)Session[Role.User.ToString()];
            name.fullname = userinfo.FullName;
            name.image = userinfo.ProfileImage;
            return PartialView("_Userfullname", name);
        }

        public ActionResult Showfullnamewithcity()
        {
            UserFullName name = new UserFullName();
            ApplicationUser userinfo = (ApplicationUser)Session[Role.User.ToString()];
            if (!string.IsNullOrEmpty(userinfo.City))
            {
                name.city = userinfo.City;
            }

            name.fullname = userinfo.FullName;
            long countryid = 0;
            if (userinfo.CountryId != null)
            {
                name.city = userinfo.City;
                countryid = Convert.ToInt64(userinfo.CountryId);
                Country country = db.Country.Find(countryid);
                name.country = country.name;
                name.image = userinfo.ProfileImage;
            }

            return PartialView("_Userfullnamewithcityaddress", name);
        }

        public ActionResult MyProfile()
        {
            string userid = User.Identity.GetUserId();
            ApplicationUser user = UserManager.FindById(userid);
            
            if (user.DateOfBirth != null)
            {
                user.DateOfBirths = user.DateOfBirth.GetValueOrDefault().ToString("MM-dd-yyyy");
            }

            if (user.SpouseDateOfBirth != null)
            {
                user.SpouseDateOfBirths = user.SpouseDateOfBirth.GetValueOrDefault().ToString("MM-dd-yyyy");
            }

            if (user.DateOfJoining != null)
            {
                user.DateOfJoin = user.DateOfJoining.GetValueOrDefault().ToString("MM-dd-yyyy");
            }

            if (user != null)
            {
                ViewBag.ProfileImage = user.ProfileImage;
                ViewBag.designation = user.Designation;
            }

            ViewBag.countrylist = new SelectList(db.Country, "id", "nicename", user.CountryId);
            ViewBag.statelist = new SelectList(db.State, "id", "name", user.StateId);
            if (TempData["upload"] != null)
            {
                ViewBag.success = TempData["upload"].ToString();
                TempData.Remove("upload");
            }
            if (TempData["IsUpdated"] == "updated") //Is for show notification 
            {
                ViewBag.success = "Your profile has been updated.";
            }
            //string repotedId = db.TeamMember.Where(x => x.usersid == userid).Select(x => x.Reported).FirstOrDefault();
            //ViewBag.RepotedTo =
            //    new SelectList(UserManager.Users.OrderBy(f => f.FirstName).Where(u => u.isactive == true), "Id",
            //        "NameEmail", repotedId);
            ViewBag.ShiftTimePK = new SelectList(CommonFunctions.ShiftTimingsPKT(), "Value", "Text", user.ShiftTimePK);
            ViewBag.ShiftTimeEST = new SelectList(CommonFunctions.ShiftTimingsEST(), "Value", "Text", user.ShiftTimeEST);
            ViewBag.TeamLead = new SelectList(CommonFunctions.TeamLeadList(), "Value", "Text", user.TeamLead);
            ViewBag.ProjectManager = new SelectList(CommonFunctions.ProjectManagerList(), "Value", "Text", user.ProjectManager);
            return View(user);
        }

        [HttpPost]
        public async Task<ActionResult> MyProfile(ApplicationUser editUser)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = await UserManager.FindByIdAsync(editUser.Id);
                if (user == null)
                {
                    return HttpNotFound();
                }

                //user.UserName = editUser.Email;
                //user.Email = editUser.Email;
                //user.FirstName = editUser.FirstName;
                //user.LastName = editUser.LastName;
                //user.Address = editUser.Address;
                //user.City = editUser.City;
                //user.StateId = editUser.StateId;
                //user.CountryId = editUser.CountryId;
                //user.Zip = editUser.Zip;
                //user.Phone = editUser.Phone;
                //user.Mobile = editUser.Mobile;
                //user.Designation = editUser.Designation;

                //Later Changes

                //user.DateOfBirth=editUser.DateOfBirth;
                //user.NationalIdentificationNumber = editUser.NationalIdentificationNumber;
                //user.PersonalEmailAddress= editUser.PersonalEmailAddress;
                //user.PersonNameEmergency = editUser.PersonNameEmergency;
                //user.EmergencyPhoneNumber = editUser.EmergencyPhoneNumber;
                //user.SpouseName = editUser.SpouseName;
                //user.SpouseDateOfBirth = editUser.SpouseDateOfBirth;
                //user.ChildrenNames = editUser.ChildrenNames;
                //user.DateOfJoining = editUser.DateOfJoining;
                //user.OfficialEmailAddress = editUser.OfficialEmailAddress;
                //user.ShiftTimings = editUser.ShiftTimings;
                //user.Expertise = editUser.Expertise;
                //user.Experience = editUser.Experience;
                //user.AccountNumber = editUser.AccountNumber;
                //user.BranchName = editUser.BranchName;
                user.IsNotifyManagerOnTaskAssignment = editUser.IsNotifyManagerOnTaskAssignment;
                //if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "/Images/UserProfileImage/"))
                //{
                //    Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "/Images/UserProfileImage/");
                //}

                //if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "/Images/UserProfileImage/deleted"))
                //{
                //    Directory.CreateDirectory(
                //        AppDomain.CurrentDomain.BaseDirectory + "/Images/UserProfileImage/deleted");
                //}

                //if (Request.Files.Count > 0)
                //{
                //    HttpPostedFileBase postedFile = Request.Files[0];
                //    if (postedFile.ContentLength > 0)
                //    {
                //        if (postedFile.ContentType.ToLower() != "image/jpg" &&
                //            postedFile.ContentType.ToLower() != "image/png" &&
                //            postedFile.ContentType.ToLower() != "image/gif" &&
                //            postedFile.ContentType.ToLower() != "image/jpeg")
                //        {
                //            ModelState.AddModelError("user.ProfileImage",
                //                "Sorry, file format must be jpg or jpeg or png or gif");
                //            ViewBag.countrylist = new SelectList(db.Country, "id", "nicename");
                //            ViewBag.statelist = new SelectList(db.State, "id", "name");
                //            return View(editUser);
                //        }

                //        if (ModelState.IsValid)
                //        {
                //            string imagapath = ConfigurationManager.AppSettings["userimagepath"];
                //            string deletedimagepath = ConfigurationManager.AppSettings["deleteduserimagepath"];
                //            string oldfile = user.ProfileImage;
                //            string oldfileExt = Path.GetExtension(oldfile);
                //            string fileExt = Path.GetExtension(postedFile.FileName);
                //            string oldfileName = user.ProfileImage;
                //            string oldpath = Path.Combine(Server.MapPath(imagapath) + oldfileName + oldfileExt);
                //            string fileName = user.Id;
                //            string deleltedfielname = fileName + DateTime.Now.ToString("yyyyMMddHHmmssfff");
                //            string path = Path.Combine(Server.MapPath(imagapath) + fileName + fileExt);
                //            deletedimagepath =
                //                Path.Combine(Server.MapPath(deletedimagepath + deleltedfielname + fileExt));
                //            if (System.IO.File.Exists(oldpath))
                //            {
                //                System.IO.File.Move(oldpath, deletedimagepath);
                //            }

                //            postedFile.SaveAs(path);
                //            user.ProfileImage = fileName + fileExt;
                //            IdentityResult updateuserwithimg = await UserManager.UpdateAsync(user);
                //            ViewBag.countrylist = new SelectList(db.Country, "id", "nicename");
                //            ViewBag.statelist = new SelectList(db.State, "id", "name");
                //            ViewBag.success = "Your profile has been updated.";
                //            ViewBag.ProfileImage = user.ProfileImage;
                //            ViewBag.designation = user.Designation;
                //            Session[Role.User.ToString()] = editUser;
                //            return View(editUser);
                //        }
                //    }
                //}

                IdentityResult updateuser = await UserManager.UpdateAsync(user);
                TempData["IsUpdated"] = "updated";
                return RedirectToAction("MyProfile");
            }
            var errors = ModelState.Select(x => x.Value.Errors)
                    .Where(y => y.Count > 0)
                    .ToList();
            ModelState.AddModelError("", "All Fields with * are required.");
            ViewBag.countrylist = new SelectList(db.Country, "id", "nicename");
            ViewBag.statelist = new SelectList(db.State, "id", "name");
            ViewBag.TeamLead = new SelectList(CommonFunctions.TeamLeadList(), "Value", "Text");
            ViewBag.ProjectManager = new SelectList(CommonFunctions.ProjectManagerList(), "Value", "Text");


            ViewBag.ShiftTimePK = new SelectList(CommonFunctions.ShiftTimingsPKT(), "Value", "Text");
            ViewBag.ShiftTimeEST = new SelectList(CommonFunctions.ShiftTimingsEST(), "Value", "Text");
            return View(editUser);
        }

        [HttpPost]
        public async Task<ActionResult> Upload()
        {
            try
            {
                string userid = User.Identity.GetUserId();
                ApplicationUser user = UserManager.FindById(userid);
                if (user != null)
                {
                    if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "/Images/UserProfileImage/"))
                    {
                        Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "/Images/UserProfileImage/");
                    }

                    if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "/Images/UserProfileImage/deleted"))
                    {
                        Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory +
                                                  "/Images/UserProfileImage/deleted");
                    }

                    if (Request.Files.Count > 0)
                    {
                        HttpPostedFileBase postedFile = Request.Files[0];
                        if (postedFile.ContentLength > 0)
                        {
                            if (postedFile.ContentType.ToLower() != "image/jpg" &&
                                postedFile.ContentType.ToLower() != "image/png" &&
                                postedFile.ContentType.ToLower() != "image/gif" &&
                                postedFile.ContentType.ToLower() != "image/jpeg")
                            {
                                ModelState.AddModelError("user.ProfileImage",
                                    "Sorry, file format must be jpg or jpeg or png or gif");
                            }

                            if (ModelState.IsValid)
                            {
                                string imagapath = ConfigurationManager.AppSettings["userimagepath"];
                                string deletedimagepath = ConfigurationManager.AppSettings["deleteduserimagepath"];
                                //var oldfile = user.ProfileImage;
                                //var oldfileExt = Path.GetExtension(oldfile);
                                string fileExt = Path.GetExtension(postedFile.FileName);
                                //var oldfileName = user.ProfileImage;
                                string oldpath = Path.Combine(Server.MapPath(imagapath) + user.ProfileImage);
                                string fileName = user.Id;
                                string deleltedfielname = fileName + DateTime.Now.ToString("yyyyMMddHHmmssfff");
                                string path = Path.Combine(Server.MapPath(imagapath) + fileName + fileExt);
                                deletedimagepath = Path.Combine(Server.MapPath(deletedimagepath + deleltedfielname +
                                                                               Path.GetExtension(user.ProfileImage)));
                                if (System.IO.File.Exists(oldpath))
                                {
                                    System.IO.File.Move(oldpath, deletedimagepath);
                                }

                                using (Image image = Image.FromStream(postedFile.InputStream))
                                {
                                    Bitmap thumbnailImg = new Bitmap(300, 300);
                                    Graphics thumbGraph = Graphics.FromImage(thumbnailImg);
                                    thumbGraph.CompositingQuality = CompositingQuality.HighQuality;
                                    thumbGraph.SmoothingMode = SmoothingMode.HighQuality;
                                    thumbGraph.InterpolationMode = InterpolationMode.HighQualityBicubic;
                                    Rectangle imageRectangle = new Rectangle(0, 0, 300, 300);
                                    thumbGraph.DrawImage(image, imageRectangle);
                                    thumbnailImg.Save(path, image.RawFormat);
                                }

                                //postedFile.SaveAs(path);
                                user.ProfileImage = fileName + fileExt;
                                IdentityResult updateuser = await UserManager.UpdateAsync(user);
                                TempData["upload"] = "Your profile photo has been updated.";
                                return RedirectToAction("myprofile");
                            }
                        }
                    }

                    return RedirectToAction("myprofile");
                }

                return RedirectToAction("myprofile");
            }
            catch (Exception)
            {
                return RedirectToAction("myprofile");
            }
        }
    }
}