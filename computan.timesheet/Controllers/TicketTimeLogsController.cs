using computan.timesheet.core;
using computan.timesheet.Extensions;
using computan.timesheet.Helpers;
using computan.timesheet.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace computan.timesheet.Controllers
{
    [CustomeAuthorizeAttribute]
    public class TicketTimeLogsController : BaseController
    {
        private ApplicationUserManager _userManager;

        public ApplicationUserManager UserManager
        {
            get => _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            private set => _userManager = value;
        }

        // GET: TicketTimeLogs
        public ActionResult Index()
        {
            TicketTimeViewModel timeVM = new TicketTimeViewModel
            {
                SearchTicketTime = new TicketTimeSearchViewModel
                {
                    StartDate = DateTime.Now.Date.Add(TimeSpan.Parse("00:00:00")),
                    EndDate = DateTime.Now.Date.Add(TimeSpan.Parse("23:59:59")),
                    projectid = null,
                    teamuserid = null,
                    ProjectCollection = db.Database.SqlQuery<CombinedEntity>("exec projects_loadcombined").Select(x =>
                        new SelectListItem
                        {
                            Text = x.name,
                            Value = x.id.ToString()
                        }).ToList()
                }
            };
            string currentUserId = User.Identity.GetUserId();
            timeVM.SearchResults = db.TicketTimeLog.Include(t => t.Project).Include(t => t.Skill)
                .Include(t => t.TicketItem).Include(t => t.TeamUser).Where(t => t.teamuserid == currentUserId)
                .Where(ttl =>
                    DbFunctions.TruncateTime(ttl.workdate) >= timeVM.SearchTicketTime.StartDate &&
                    DbFunctions.TruncateTime(ttl.workdate) <= timeVM.SearchTicketTime.EndDate)
                .Where(t => t.timespentinminutes != 0 || t.billabletimeinminutes != 0)
                .OrderByDescending(w => w.workdate).ToList();
            ViewBag.totalspent =
                CommonFunctions.RoundTwoDecimalPlaces((double)timeVM.SearchResults.Sum(t => t.timespentinminutes) /
                                                      60);
            ViewBag.totalbillable =
                CommonFunctions.RoundTwoDecimalPlaces((double)timeVM.SearchResults.Sum(t => t.billabletimeinminutes) /
                                                      60);
            return View(timeVM);
        }

        // POST: TicketTimeLogs/Index
        [HttpPost]
        public ActionResult Index(
            [Bind(Include = "StartDate,EndDate,projectid")]
            TicketTimeSearchViewModel TicketTimeSearch)
        {
            if (ModelState.IsValid)
            {
                TicketTimeViewModel searchTimeVM = new TicketTimeViewModel
                {
                    SearchTicketTime = new TicketTimeSearchViewModel
                    {
                        StartDate = TicketTimeSearch.StartDate.Add(TimeSpan.Parse("00:00:00")),
                        EndDate = TicketTimeSearch.EndDate.Add(TimeSpan.Parse("23:59:59")),
                        projectid = TicketTimeSearch.projectid,
                        ProjectCollection = db.Database.SqlQuery<CombinedEntity>("exec projects_loadcombined").Select(
                            x => new SelectListItem
                            {
                                Text = x.name,
                                Value = x.id.ToString()
                            }).ToList()
                    }
                };
                string searchCurrentUserId = User.Identity.GetUserId();

                if (TicketTimeSearch.projectid != null)
                {
                    searchTimeVM.SearchResults = db.TicketTimeLog.Include(t => t.Project).Include(t => t.Skill)
                        .Include(t => t.TicketItem).Include(t => t.TeamUser)
                        .Where(t => t.teamuserid == searchCurrentUserId)
                        .Where(ttl =>
                            DbFunctions.TruncateTime(ttl.workdate) >= searchTimeVM.SearchTicketTime.StartDate &&
                            DbFunctions.TruncateTime(ttl.workdate) <= searchTimeVM.SearchTicketTime.EndDate)
                        .Where(ttl => ttl.projectid == TicketTimeSearch.projectid)
                        .Where(t => t.timespentinminutes != 0 || t.billabletimeinminutes != 0).OrderBy(d => d.workdate)
                        .ToList();
                }
                else
                {
                    searchTimeVM.SearchResults = db.TicketTimeLog.Include(t => t.Project).Include(t => t.Skill)
                        .Include(t => t.TicketItem).Include(t => t.TeamUser)
                        .Where(t => t.teamuserid == searchCurrentUserId).Where(ttl =>
                            DbFunctions.TruncateTime(ttl.workdate) >= searchTimeVM.SearchTicketTime.StartDate &&
                            DbFunctions.TruncateTime(ttl.workdate) <= searchTimeVM.SearchTicketTime.EndDate)
                        .Where(t => t.timespentinminutes != 0 || t.billabletimeinminutes != 0).OrderBy(d => d.workdate)
                        .ToList();
                }

                ViewBag.totalspent =
                    CommonFunctions.RoundTwoDecimalPlaces(
                        (double)searchTimeVM.SearchResults.Sum(t => t.timespentinminutes) / 60);
                ViewBag.totalbillable =
                    CommonFunctions.RoundTwoDecimalPlaces(
                        (double)searchTimeVM.SearchResults.Sum(t => t.billabletimeinminutes) / 60);
                return View("Index", searchTimeVM);
            }

            TicketTimeViewModel timeVM = new TicketTimeViewModel
            {
                SearchTicketTime = new TicketTimeSearchViewModel
                {
                    StartDate = DateTime.Now.Date.Add(TimeSpan.Parse("00:00:00")),
                    EndDate = DateTime.Now.Date.Add(TimeSpan.Parse("23:59:59")),
                    projectid = null,
                    ProjectCollection = db.Database.SqlQuery<CombinedEntity>("exec projects_loadcombined").Select(x =>
                        new SelectListItem
                        {
                            Text = x.name,
                            Value = x.id.ToString()
                        }).ToList()
                }
            };
            string currentUserId = User.Identity.GetUserId();
            timeVM.SearchResults = db.TicketTimeLog.Include(t => t.Project).Include(t => t.Skill)
                .Include(t => t.TicketItem).Include(t => t.TeamUser).Where(t => t.teamuserid == currentUserId)
                .Where(ttl =>
                    DbFunctions.TruncateTime(ttl.workdate) >= timeVM.SearchTicketTime.StartDate &&
                    DbFunctions.TruncateTime(ttl.workdate) <= timeVM.SearchTicketTime.EndDate)
                .Where(t => t.timespentinminutes != 0 || t.billabletimeinminutes != 0).OrderBy(d => d.workdate)
                .ToList();
            ViewBag.totalspent =
                CommonFunctions.RoundTwoDecimalPlaces((double)timeVM.SearchResults.Sum(t => t.timespentinminutes) /
                                                      60);
            ViewBag.totalbillable =
                CommonFunctions.RoundTwoDecimalPlaces((double)timeVM.SearchResults.Sum(t => t.billabletimeinminutes) /
                                                      60);
            return View("Index", timeVM);
        }

        //GEt:my daily bucket 
        public ActionResult MyBuckets()
        {
            TicketTimeViewModel timeVM = new TicketTimeViewModel
            {
                SearchTicketTime = new TicketTimeSearchViewModel
                {
                    StartDate = DateTime.Now.Date.Add(TimeSpan.Parse("00:00:00")),
                    EndDate = DateTime.Now.Date.Add(TimeSpan.Parse("23:59:59")),
                    projectid = null,
                    teamuserid = null,
                    ProjectCollection = db.Database.SqlQuery<CombinedEntity>("exec projects_loadcombined").Select(x =>
                        new SelectListItem
                        {
                            Text = x.name,
                            Value = x.id.ToString()
                        }).ToList()
                }
            };
            string currentUserId = User.Identity.GetUserId();
            timeVM.SearchResults = db.TicketTimeLog.Include(t => t.Project).Include(t => t.Skill)
                .Include(t => t.TicketItem).Include(t => t.TeamUser).Where(t => t.teamuserid == currentUserId)
                .Where(ttl =>
                    DbFunctions.TruncateTime(ttl.workdate) >= timeVM.SearchTicketTime.StartDate &&
                    DbFunctions.TruncateTime(ttl.workdate) <= timeVM.SearchTicketTime.EndDate)
                .Where(t => t.timespentinminutes == 0 && t.billabletimeinminutes == 0).ToList();
            List<SelectListItem> timespentinminutes = new List<SelectListItem>();
            int timespentinminute = 0;
            timespentinminutes.Add(new SelectListItem
            { Text = timespentinminute.ToString(), Value = timespentinminute.ToString() });
            int Counter = 1;
            for (Counter = 1; Counter <= 120; Counter++)
            {
                timespentinminute = timespentinminute + 5;
                timespentinminutes.Add(new SelectListItem
                { Text = timespentinminute.ToString(), Value = timespentinminute.ToString() });
            }

            ViewData["item.timespentinminutes"] = timespentinminutes;
            ViewData["item.billabletimeinminutes"] = timespentinminutes;
            return View("MyBuckets", timeVM);
        }

        //POST: MY daily bucket
        [HttpPost]
        public ActionResult MyBuckets(
            [Bind(Include = "StartDate,projectid")]
            TicketTimeSearchViewModel TicketTimeSearch)
        {
            if (ModelState.IsValid)
            {
                TicketTimeViewModel searchTimeVM = new TicketTimeViewModel
                {
                    SearchTicketTime = new TicketTimeSearchViewModel
                    {
                        StartDate = TicketTimeSearch.StartDate.Add(TimeSpan.Parse("00:00:00")),
                        EndDate = TicketTimeSearch.StartDate.Add(TimeSpan.Parse("23:59:59")),
                        projectid = TicketTimeSearch.projectid,
                        ProjectCollection = db.Database.SqlQuery<CombinedEntity>("exec projects_loadcombined").Select(
                            x => new SelectListItem
                            {
                                Text = x.name,
                                Value = x.id.ToString()
                            }).ToList()
                    }
                };
                string searchCurrentUserId = User.Identity.GetUserId();

                if (TicketTimeSearch.projectid != null)
                {
                    searchTimeVM.SearchResults = db.TicketTimeLog.Include(t => t.Project).Include(t => t.Skill)
                        .Include(t => t.TicketItem).Include(t => t.TeamUser)
                        .Where(t => t.teamuserid == searchCurrentUserId)
                        .Where(ttl =>
                            DbFunctions.TruncateTime(ttl.workdate) >= searchTimeVM.SearchTicketTime.StartDate &&
                            DbFunctions.TruncateTime(ttl.workdate) <= searchTimeVM.SearchTicketTime.EndDate)
                        .Where(ttl => ttl.projectid == TicketTimeSearch.projectid)
                        .Where(t => t.timespentinminutes == 0 && t.billabletimeinminutes == 0)
                        .OrderByDescending(w => w.workdate).ToList();
                }
                else
                {
                    searchTimeVM.SearchResults = db.TicketTimeLog.Include(t => t.Project).Include(t => t.Skill)
                        .Include(t => t.TicketItem).Include(t => t.TeamUser)
                        .Where(t => t.teamuserid == searchCurrentUserId)
                        .Where(ttl =>
                            DbFunctions.TruncateTime(ttl.workdate) >= searchTimeVM.SearchTicketTime.StartDate &&
                            DbFunctions.TruncateTime(ttl.workdate) <= searchTimeVM.SearchTicketTime.EndDate)
                        .Where(t => t.timespentinminutes == 0 && t.billabletimeinminutes == 0)
                        .OrderByDescending(w => w.workdate).ToList();
                }

                ViewBag.totalspent =
                    CommonFunctions.RoundTwoDecimalPlaces(
                        (double)searchTimeVM.SearchResults.Sum(t => t.timespentinminutes) / 60);
                ViewBag.totalbillable =
                    CommonFunctions.RoundTwoDecimalPlaces(
                        (double)searchTimeVM.SearchResults.Sum(t => t.billabletimeinminutes) / 60);
                List<SelectListItem> timespentinminutes = new List<SelectListItem>();
                int timespentinminute = 0;
                timespentinminutes.Add(new SelectListItem
                { Text = timespentinminute.ToString(), Value = timespentinminute.ToString() });
                int Counter = 1;
                for (Counter = 1; Counter <= 120; Counter++)
                {
                    timespentinminute = timespentinminute + 5;
                    timespentinminutes.Add(new SelectListItem
                    { Text = timespentinminute.ToString(), Value = timespentinminute.ToString() });
                }

                ViewData["item.timespentinminutes"] = timespentinminutes;
                ViewData["item.billabletimeinminutes"] = timespentinminutes;
                return View("Mybuckets", searchTimeVM);
            }

            TicketTimeViewModel timeVM = new TicketTimeViewModel
            {
                SearchTicketTime = new TicketTimeSearchViewModel
                {
                    StartDate = DateTime.Now.Add(TimeSpan.Parse("00:00:00")),
                    EndDate = DateTime.Now.Add(TimeSpan.Parse("23:59:59")),
                    projectid = null,
                    ProjectCollection = db.Database.SqlQuery<CombinedEntity>("exec projects_loadcombined").Select(x =>
                        new SelectListItem
                        {
                            Text = x.name,
                            Value = x.id.ToString()
                        }).ToList()
                }
            };
            string currentUserId = User.Identity.GetUserId();
            timeVM.SearchResults = db.TicketTimeLog.Include(t => t.Project).Include(t => t.Skill)
                .Include(t => t.TicketItem).Include(t => t.TeamUser).Where(t => t.teamuserid == currentUserId)
                .Where(ttl =>
                    DbFunctions.TruncateTime(ttl.workdate) >= timeVM.SearchTicketTime.StartDate &&
                    DbFunctions.TruncateTime(ttl.workdate) <= timeVM.SearchTicketTime.EndDate)
                .Where(t => t.timespentinminutes == 0 && t.billabletimeinminutes == 0)
                .OrderByDescending(w => w.workdate).ToList();
            ViewBag.totalspent =
                CommonFunctions.RoundTwoDecimalPlaces((double)timeVM.SearchResults.Sum(t => t.timespentinminutes) /
                                                      60);
            ViewBag.totalbillable =
                CommonFunctions.RoundTwoDecimalPlaces((double)timeVM.SearchResults.Sum(t => t.billabletimeinminutes) /
                                                      60);
            return View("MyBuckets", timeVM);
        }

        public ActionResult UpdateMyBuckets(DateTime StartDate)
        {
            long? projectid = null;
            if (ModelState.IsValid)
            {
                TicketTimeViewModel searchTimeVM = new TicketTimeViewModel
                {
                    SearchTicketTime = new TicketTimeSearchViewModel
                    {
                        StartDate = StartDate.Add(TimeSpan.Parse("00:00:00")),
                        EndDate = StartDate.Add(TimeSpan.Parse("23:59:59")),
                        projectid = projectid,
                        ProjectCollection = db.Database.SqlQuery<CombinedEntity>("exec projects_loadcombined").Select(
                            x => new SelectListItem
                            {
                                Text = x.name,
                                Value = x.id.ToString()
                            }).ToList()
                    }
                };
                string searchCurrentUserId = User.Identity.GetUserId();

                if (projectid != null)
                {
                    searchTimeVM.SearchResults = db.TicketTimeLog.Include(t => t.Project).Include(t => t.Skill)
                        .Include(t => t.TicketItem).Include(t => t.TeamUser)
                        .Where(t => t.teamuserid == searchCurrentUserId)
                        .Where(ttl =>
                            DbFunctions.TruncateTime(ttl.workdate) >= searchTimeVM.SearchTicketTime.StartDate &&
                            DbFunctions.TruncateTime(ttl.workdate) <= searchTimeVM.SearchTicketTime.EndDate)
                        .Where(ttl => ttl.projectid == projectid)
                        .Where(t => t.timespentinminutes == 0 && t.billabletimeinminutes == 0)
                        .OrderByDescending(w => w.workdate).ToList();
                }
                else
                {
                    searchTimeVM.SearchResults = db.TicketTimeLog.Include(t => t.Project).Include(t => t.Skill)
                        .Include(t => t.TicketItem).Include(t => t.TeamUser)
                        .Where(t => t.teamuserid == searchCurrentUserId)
                        .Where(ttl =>
                            DbFunctions.TruncateTime(ttl.workdate) >= searchTimeVM.SearchTicketTime.StartDate &&
                            DbFunctions.TruncateTime(ttl.workdate) <= searchTimeVM.SearchTicketTime.EndDate)
                        .Where(t => t.timespentinminutes == 0 && t.billabletimeinminutes == 0)
                        .OrderByDescending(w => w.workdate).ToList();
                }

                ViewBag.totalspent =
                    CommonFunctions.RoundTwoDecimalPlaces(
                        (double)searchTimeVM.SearchResults.Sum(t => t.timespentinminutes) / 60);
                ViewBag.totalbillable =
                    CommonFunctions.RoundTwoDecimalPlaces(
                        (double)searchTimeVM.SearchResults.Sum(t => t.billabletimeinminutes) / 60);
                List<SelectListItem> timespentinminutes = new List<SelectListItem>();
                int timespentinminute = 0;
                timespentinminutes.Add(new SelectListItem
                { Text = timespentinminute.ToString(), Value = timespentinminute.ToString() });
                int Counter = 1;
                for (Counter = 1; Counter <= 120; Counter++)
                {
                    timespentinminute = timespentinminute + 5;
                    timespentinminutes.Add(new SelectListItem
                    { Text = timespentinminute.ToString(), Value = timespentinminute.ToString() });
                }

                ViewData["item.timespentinminutes"] = timespentinminutes;
                ViewData["item.billabletimeinminutes"] = timespentinminutes;
                return PartialView("_mybucket", searchTimeVM);
            }

            TicketTimeViewModel timeVM = new TicketTimeViewModel
            {
                SearchTicketTime = new TicketTimeSearchViewModel
                {
                    StartDate = DateTime.Now.Add(TimeSpan.Parse("00:00:00")),
                    EndDate = DateTime.Now.Add(TimeSpan.Parse("23:59:59")),
                    projectid = null,
                    ProjectCollection = db.Database.SqlQuery<CombinedEntity>("exec projects_loadcombined").Select(x =>
                        new SelectListItem
                        {
                            Text = x.name,
                            Value = x.id.ToString()
                        }).ToList()
                }
            };
            string currentUserId = User.Identity.GetUserId();
            timeVM.SearchResults = db.TicketTimeLog.Include(t => t.Project).Include(t => t.Skill)
                .Include(t => t.TicketItem).Include(t => t.TeamUser).Where(t => t.teamuserid == currentUserId)
                .Where(ttl =>
                    DbFunctions.TruncateTime(ttl.workdate) >= timeVM.SearchTicketTime.StartDate &&
                    DbFunctions.TruncateTime(ttl.workdate) <= timeVM.SearchTicketTime.EndDate)
                .Where(t => t.timespentinminutes == 0 && t.billabletimeinminutes == 0)
                .OrderByDescending(w => w.workdate).ToList();
            ViewBag.totalspent =
                CommonFunctions.RoundTwoDecimalPlaces((double)timeVM.SearchResults.Sum(t => t.timespentinminutes) /
                                                      60);
            ViewBag.totalbillable =
                CommonFunctions.RoundTwoDecimalPlaces((double)timeVM.SearchResults.Sum(t => t.billabletimeinminutes) /
                                                      60);
            return PartialView("_mybucket", timeVM);
        }

        //GET :Load Porjects By Client
        public ActionResult LoadProjectsByClient(long id)
        {
            List<Client> subclients = db.Client.Where(p => p.parentid == id).ToList();
            List<CombinedEntity> projects = db.Database.SqlQuery<CombinedEntity>("exec projects_loadcombinedfortimelog @clientid",
                new SqlParameter("@clientid", id)).ToList();
            if (subclients != null && subclients.Count > 0)
            {
                foreach (Client item in subclients)
                {
                    List<CombinedEntity> sublcientprojects = db.Database
                        .SqlQuery<CombinedEntity>("exec projects_loadcombinedfortimelog @clientid",
                            new SqlParameter("@clientid", item.id)).ToList();
                    if (sublcientprojects != null && sublcientprojects.Count > 0)
                    {
                        projects.AddRange(sublcientprojects);
                    }
                }
            }

            IEnumerable<SelectListItem> projectlist = projects.Select(m => new SelectListItem
            {
                Text = m.name,
                Value = m.id.ToString()
            });
            return Json(projectlist, JsonRequestBehavior.AllowGet);
        }

        //GET: Load All Projects
        public ActionResult LoadallProjects()
        {
            List<SelectListItem> ProjectCollection = db.Database.SqlQuery<CombinedEntity>("exec projects_loadcombined").Select(x =>
                new SelectListItem
                {
                    Text = x.name,
                    Value = x.id.ToString()
                }).ToList();
            return Json(ProjectCollection, JsonRequestBehavior.AllowGet);
        }
      
        // GET: TicketTimeLogs/Team
        public ActionResult Team()
        {
            TicketTimeViewModel timeVM = new TicketTimeViewModel
            {
                SearchTicketTime = new TicketTimeSearchViewModel
                {
                    StartDate = DateTime.Now.Date.Add(TimeSpan.Parse("00:00:00")),
                    EndDate = DateTime.Now.Date.Add(TimeSpan.Parse("23:59:59")),
                    projectid = null,
                    clientid = null,
                    ProjectCollection = db.Database.SqlQuery<CombinedEntity>("exec projects_loadcombined").Select(x =>
                        new SelectListItem
                        {
                            Text = x.name,
                            Value = x.id.ToString()
                        }).ToList(),
                    UsersCollection = UserManager.Users.OrderBy(u => u.FirstName).Select(u => new SelectListItem
                    {
                        Text = u.FirstName + " " + u.LastName + " - " + u.Email,
                        Value = u.Id
                    }).ToList(),
                    ClientCollection = new SelectList(db.Client.ToList(), "id", "name").ToList(),
                    TeamList = new SelectList(db.Team.Where(x => x.isactive && x.name != "Quality Control").ToList(), "id", "name").ToList()
                }
            };

            //timeVM.SearchResult = db.Database.SqlQuery<TeamTimeLogSearchResult>("exec FilterTeamTimeLog_sp @teamuserid, @TeamId, @projectid, @clientid, @startDate, @endDate",
            //                  new SqlParameter("teamuserid", DBNull.Value),
            //                  new SqlParameter("TeamId", DBNull.Value),
            //                  new SqlParameter("projectid", DBNull.Value),
            //                  new SqlParameter("clientid", DBNull.Value),
            //                  new SqlParameter("startDate", DateTime.Now.Date.Add(TimeSpan.Parse("00:00:00"))),
            //                  new SqlParameter("endDate", DateTime.Now.Date.Add(TimeSpan.Parse("23:59:59")))
            //                  ).Select(x => new TeamTimeLogSearchResult
            //                  {
            //                      Client = x.Client,
            //                      ClientType = x.ClientType,
            //                      ProjectName = x.ProjectName,
            //                      description = x.description,
            //                      title = x.title,
            //                      workdate = x.workdate,
            //                      timespentinminutes = x.timespentinminutes,
            //                      billabletimeinminutes = x.billabletimeinminutes,
            //                      SkillName = x.SkillName,
            //                      UserName = x.UserName,
            //                       maxbillablehours = x.maxbillablehours ?? "Not Specified",
            //                  }).ToList();
            timeVM.SearchResults = db.TicketTimeLog.Include(t => t.Project).Include(t => t.Skill)
                .Include(t => t.TicketItem).Include(t => t.TeamUser)
                .Where(ttl =>
                    DbFunctions.TruncateTime(ttl.workdate) >= timeVM.SearchTicketTime.StartDate &&
                    DbFunctions.TruncateTime(ttl.workdate) <= timeVM.SearchTicketTime.EndDate)
                .Where(t => t.timespentinminutes != 0 || t.billabletimeinminutes != 0)
                .OrderByDescending(w => w.workdate).ToList();
            timeVM.SearchResults = timeVM.SearchResults.Where(x => x.Skill.name != "Quality Control").ToList();
            ViewBag.totalspent =
                CommonFunctions.RoundTwoDecimalPlaces((double)timeVM.SearchResult.Sum(t => t.timespentinminutes) /
                                                      60);
            ViewBag.totalbillable =
                CommonFunctions.RoundTwoDecimalPlaces((double)timeVM.SearchResult.Sum(t => t.billabletimeinminutes) /
                                                      60);
            Session["startdate"] = timeVM.SearchTicketTime.StartDate;
            Session["endate"] = timeVM.SearchTicketTime.EndDate;
            Session["TicketTimeViewModelS"] = timeVM;
            Session["totalspent"] = ViewBag.totalspent;
            Session["totalbillable"] = ViewBag.totalbillable;
            Session["clientid"] = null;
            return View(timeVM);
        }

        // POST: TicketTimeLogs/Index
        [HttpPost]
        public ActionResult Team(
            [Bind(Include = "StartDate,EndDate,projectid,teamuserid,clientid,TeamId")]
            TicketTimeSearchViewModel TicketTimeSearch)
        {
            if (ModelState.IsValid)
            {
                TicketTimeViewModel searchTimeVM = new TicketTimeViewModel
                {
                    SearchResults = new List<TicketTimeLog>(),
                    SearchTicketTime = new TicketTimeSearchViewModel
                    {
                        StartDate = TicketTimeSearch.StartDate.Date.Add(TimeSpan.Parse("00:00:00")),
                        EndDate = TicketTimeSearch.EndDate.Date.Add(TimeSpan.Parse("23:59:59")),
                        projectid = TicketTimeSearch.projectid,
                        teamuserid = TicketTimeSearch.teamuserid,
                        clientid = TicketTimeSearch.clientid,
                        ProjectCollection = db.Database.SqlQuery<CombinedEntity>("exec projects_loadcombined").Select(
                            x => new SelectListItem
                            {
                                Text = x.name,
                                Value = x.id.ToString()
                            }).ToList(),
                        UsersCollection = UserManager.Users.OrderBy(u => u.FirstName).Select(u => new SelectListItem
                        {
                            Text = u.FirstName + " " + u.LastName + " - " + u.Email,
                            Value = u.Id
                        }).ToList(),
                        ClientCollection = new SelectList(db.Client.ToList(), "id", "name").ToList(),
                        TeamList = new SelectList(db.Team.Where(x => x.isactive && x.name != "Quality Control").ToList(), "id", "name").ToList()
                    }
                };
                //searchTimeVM.SearchResult = db.Database.SqlQuery<TeamTimeLogSearchResult>("exec FilterTeamTimeLog_sp @teamuserid, @TeamId, @projectid, @clientid, @startDate, @endDate",
                //            new SqlParameter("teamuserid", TicketTimeSearch.teamuserid ?? (object)DBNull.Value),
                //            new SqlParameter("TeamId", TicketTimeSearch.TeamId ?? (object)DBNull.Value),
                //            new SqlParameter("projectid", TicketTimeSearch.projectid ?? (object)DBNull.Value),
                //            new SqlParameter("clientid", TicketTimeSearch.clientid ?? (object)DBNull.Value),
                //            new SqlParameter("startDate", TicketTimeSearch.StartDate),
                //            new SqlParameter("endDate", TicketTimeSearch.EndDate)
                //            ).Select(x => new TeamTimeLogSearchResult
                //            {
                //                Client = x.Client,
                //                ClientType = x.ClientType,
                //                ProjectName = x.ProjectName,
                //                description = x.description,
                //                title = x.title,
                //                workdate = x.workdate,
                //                timespentinminutes = x.timespentinminutes,
                //                billabletimeinminutes = x.billabletimeinminutes,
                //                SkillName = x.SkillName,
                //                UserName = x.UserName,
                //                id = x.id,
                //                TeamName = x.TeamName,
                //                maxbillablehours = x.maxbillablehours ?? "Not Specified",
                //            }).ToList();
                if (TicketTimeSearch.projectid != null && TicketTimeSearch.teamuserid != null &&
                    TicketTimeSearch.clientid == null)
                {
                    searchTimeVM.SearchResults = db.TicketTimeLog.Include(t => t.User).Include(t => t.Project)
                        .Include(t => t.Skill).Include(t => t.TicketItem).Include(t => t.TeamUser)
                        .Where(ttl =>
                            DbFunctions.TruncateTime(ttl.workdate) >= searchTimeVM.SearchTicketTime.StartDate &&
                            DbFunctions.TruncateTime(ttl.workdate) <= searchTimeVM.SearchTicketTime.EndDate)
                        .Where(ttl => ttl.projectid == TicketTimeSearch.projectid)
                        .Where(ttl => ttl.teamuserid == TicketTimeSearch.teamuserid)
                        .Where(t => t.timespentinminutes != 0 || t.billabletimeinminutes != 0)
                        .OrderByDescending(w => w.workdate).ToList();
                }
                else if (TicketTimeSearch.projectid != null && TicketTimeSearch.teamuserid == null &&
                         (TicketTimeSearch.clientid == null || TicketTimeSearch.clientid != null))
                {
                    searchTimeVM.SearchResults = db.TicketTimeLog.Include(t => t.User).Include(t => t.Project)
                        .Include(t => t.Skill).Include(t => t.TicketItem).Include(t => t.TeamUser)
                        .Where(ttl =>
                            DbFunctions.TruncateTime(ttl.workdate) >= searchTimeVM.SearchTicketTime.StartDate &&
                            DbFunctions.TruncateTime(ttl.workdate) <= searchTimeVM.SearchTicketTime.EndDate)
                        .Where(ttl => ttl.projectid == TicketTimeSearch.projectid)
                        .Where(t => t.timespentinminutes != 0 || t.billabletimeinminutes != 0)
                        .OrderByDescending(w => w.workdate).ToList();
                }
                else if (TicketTimeSearch.projectid == null && TicketTimeSearch.teamuserid != null &&
                         TicketTimeSearch.clientid == null)
                {
                    searchTimeVM.SearchResults = db.TicketTimeLog.Include(t => t.User).Include(t => t.Project)
                        .Include(t => t.Skill).Include(t => t.TicketItem).Include(t => t.TeamUser)
                        .Where(ttl =>
                            DbFunctions.TruncateTime(ttl.workdate) >= searchTimeVM.SearchTicketTime.StartDate &&
                            DbFunctions.TruncateTime(ttl.workdate) <= searchTimeVM.SearchTicketTime.EndDate)
                        .Where(ttl => ttl.teamuserid == TicketTimeSearch.teamuserid)
                        .Where(t => t.timespentinminutes != 0 || t.billabletimeinminutes != 0)
                        .OrderByDescending(w => w.workdate).ToList();
                }
                else if (TicketTimeSearch.projectid == null && TicketTimeSearch.clientid != null &&
                         TicketTimeSearch.teamuserid == null)
                {
                    List<Client> clientwithsubclients = db.Client.Where(p => p.id == TicketTimeSearch.clientid)
                        .Include(c => c.SubClients).ToList();
                    if (clientwithsubclients != null && clientwithsubclients.Count > 0)
                    {
                        foreach (Client item in clientwithsubclients)
                        {
                            List<CombinedEntity> projects = db.Database
                                .SqlQuery<CombinedEntity>("exec projects_loadcombinedfortimelog @clientid",
                                    new SqlParameter("@clientid", item.id)).ToList();
                            if (projects != null && projects.Count > 0)
                            {
                                foreach (CombinedEntity items in projects)
                                {
                                    List<TicketTimeLog> timelog = db.TicketTimeLog.Include(t => t.User).Include(t => t.Project)
                                        .Include(t => t.Skill).Include(t => t.TicketItem).Include(t => t.TeamUser)
                                        .Where(ttl =>
                                            DbFunctions.TruncateTime(ttl.workdate) >=
                                            searchTimeVM.SearchTicketTime.StartDate &&
                                            DbFunctions.TruncateTime(ttl.workdate) <=
                                            searchTimeVM.SearchTicketTime.EndDate)
                                        .Where(ttl => ttl.projectid == items.id)
                                        .Where(t => t.timespentinminutes != 0 || t.billabletimeinminutes != 0)
                                        .OrderByDescending(w => w.workdate).ToList();
                                    if (timelog != null && timelog.Count > 0)
                                    {
                                        searchTimeVM.SearchResults.AddRange(timelog);
                                    }
                                }
                            }

                            if (item.SubClients != null && item.SubClients.Count > 0)
                            {
                                foreach (Client subitems in item.SubClients)
                                {
                                    List<CombinedEntity> projectsforsubclients = db.Database
                                        .SqlQuery<CombinedEntity>("exec projects_loadcombinedfortimelog @clientid",
                                            new SqlParameter("@clientid", subitems.id)).ToList();
                                    if (projectsforsubclients != null && projectsforsubclients.Count > 0)
                                    {
                                        foreach (CombinedEntity items in projectsforsubclients)
                                        {
                                            List<TicketTimeLog> timelog = db.TicketTimeLog.Include(t => t.User).Include(t => t.Project)
                                                .Include(t => t.Skill).Include(t => t.TicketItem)
                                                .Include(t => t.TeamUser)
                                                .Where(ttl =>
                                                    DbFunctions.TruncateTime(ttl.workdate) >=
                                                    searchTimeVM.SearchTicketTime.StartDate &&
                                                    DbFunctions.TruncateTime(ttl.workdate) <=
                                                    searchTimeVM.SearchTicketTime.EndDate)
                                                .Where(ttl => ttl.projectid == items.id)
                                                .Where(t => t.timespentinminutes != 0 || t.billabletimeinminutes != 0)
                                                .OrderByDescending(w => w.workdate).ToList();
                                            if (timelog != null && timelog.Count > 0)
                                            {
                                                searchTimeVM.SearchResults.AddRange(timelog);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else if (TicketTimeSearch.projectid == null && TicketTimeSearch.clientid != null &&
                         TicketTimeSearch.teamuserid != null)
                {
                    List<Client> clientwithsubclients = db.Client.Where(p => p.id == TicketTimeSearch.clientid)
                        .Include(c => c.SubClients).ToList();
                    if (clientwithsubclients != null && clientwithsubclients.Count > 0)
                    {
                        foreach (Client item in clientwithsubclients)
                        {
                            List<CombinedEntity> projects = db.Database
                                .SqlQuery<CombinedEntity>("exec projects_loadcombinedfortimelog @clientid",
                                    new SqlParameter("@clientid", item.id)).ToList();
                            if (projects != null && projects.Count > 0)
                            {
                                foreach (CombinedEntity items in projects)
                                {
                                    List<TicketTimeLog> timelog = db.TicketTimeLog.Include(t => t.User).Include(t => t.Project)
                                        .Include(t => t.Skill).Include(t => t.TicketItem).Include(t => t.TeamUser)
                                        .Where(ttl =>
                                            DbFunctions.TruncateTime(ttl.workdate) >=
                                            searchTimeVM.SearchTicketTime.StartDate &&
                                            DbFunctions.TruncateTime(ttl.workdate) <=
                                            searchTimeVM.SearchTicketTime.EndDate)
                                        .Where(ttl =>
                                            ttl.projectid == items.id && ttl.teamuserid == TicketTimeSearch.teamuserid)
                                        .Where(t => t.timespentinminutes != 0 || t.billabletimeinminutes != 0)
                                        .OrderByDescending(w => w.workdate).ToList();
                                    if (timelog != null && timelog.Count > 0)
                                    {
                                        searchTimeVM.SearchResults.AddRange(timelog);
                                    }
                                }
                            }

                            if (item.SubClients != null && item.SubClients.Count > 0)
                            {
                                foreach (Client subitems in item.SubClients)
                                {
                                    List<CombinedEntity> projectsforsubclients = db.Database
                                        .SqlQuery<CombinedEntity>("exec projects_loadcombinedfortimelog @clientid",
                                            new SqlParameter("@clientid", subitems.id)).ToList();
                                    if (projectsforsubclients != null && projectsforsubclients.Count > 0)
                                    {
                                        foreach (CombinedEntity items in projectsforsubclients)
                                        {
                                            List<TicketTimeLog> timelog = db.TicketTimeLog.Include(t => t.User).Include(t => t.Project)
                                                .Include(t => t.Skill).Include(t => t.TicketItem)
                                                .Include(t => t.TeamUser)
                                                .Where(ttl =>
                                                    DbFunctions.TruncateTime(ttl.workdate) >=
                                                    searchTimeVM.SearchTicketTime.StartDate &&
                                                    DbFunctions.TruncateTime(ttl.workdate) <=
                                                    searchTimeVM.SearchTicketTime.EndDate)
                                                .Where(ttl =>
                                                    ttl.projectid == subitems.id &&
                                                    ttl.teamuserid == TicketTimeSearch.teamuserid)
                                                .Where(t => t.timespentinminutes != 0 || t.billabletimeinminutes != 0)
                                                .OrderByDescending(w => w.workdate).ToList();
                                            if (timelog != null && timelog.Count > 0)
                                            {
                                                searchTimeVM.SearchResults.AddRange(timelog);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else if (TicketTimeSearch.projectid != null && TicketTimeSearch.clientid != null &&
                         TicketTimeSearch.teamuserid != null)
                {
                    searchTimeVM.SearchResults = db.TicketTimeLog.Include(t => t.User).Include(t => t.Project)
                        .Include(t => t.Skill).Include(t => t.TicketItem).Include(t => t.TeamUser)
                        .Where(ttl =>
                            DbFunctions.TruncateTime(ttl.workdate) >= searchTimeVM.SearchTicketTime.StartDate &&
                            DbFunctions.TruncateTime(ttl.workdate) <= searchTimeVM.SearchTicketTime.EndDate)
                        .Where(ttl => ttl.projectid == TicketTimeSearch.projectid)
                        .Where(ttl => ttl.teamuserid == TicketTimeSearch.teamuserid)
                        .Where(t => t.timespentinminutes != 0 || t.billabletimeinminutes != 0)
                        .OrderByDescending(w => w.workdate).ToList();
                }
                else
                {
                    searchTimeVM.SearchResults = db.TicketTimeLog.Include(t => t.User).Include(t => t.Project)
                        .Include(t => t.Skill).Include(t => t.TicketItem).Include(t => t.TeamUser).Where(ttl =>
                            DbFunctions.TruncateTime(ttl.workdate) >= searchTimeVM.SearchTicketTime.StartDate &&
                            DbFunctions.TruncateTime(ttl.workdate) <= searchTimeVM.SearchTicketTime.EndDate)
                        .Where(t => t.timespentinminutes != 0 || t.billabletimeinminutes != 0)
                        .OrderByDescending(w => w.workdate).ToList();
                }

                double stime = 0.00;
                double btime = 0.00;
                if (searchTimeVM.SearchResult != null && searchTimeVM.SearchResult.Count > 0)
                {
                    searchTimeVM.SearchResults = searchTimeVM.SearchResults
                        .Where(x => x.Skill.name != "Quality Control").ToList();
                    stime = CommonFunctions.RoundTwoDecimalPlaces(
                        (double)searchTimeVM.SearchResult.Sum(t => t.timespentinminutes) / 60);
                    btime = CommonFunctions.RoundTwoDecimalPlaces(
                        (double)searchTimeVM.SearchResult.Sum(t => t.billabletimeinminutes) / 60);
                }

                Session["startdate"] = searchTimeVM.SearchTicketTime.StartDate;
                Session["endate"] = searchTimeVM.SearchTicketTime.EndDate;
                Session["TicketTimeViewModelS"] = searchTimeVM;
                Session["totalspent"] = ViewBag.totalspent = stime;
                Session["totalbillable"] = ViewBag.totalbillable = btime;
                Session["clientid"] = TicketTimeSearch.clientid;
                string queryString = ViewExtensions.BaseUrl+ "/TicketTimeLogs/TeamTimeReport?startdate=" +
                                    TicketTimeSearch.StartDate.ToShortDateString()+ 
                                    "&endDate=" +TicketTimeSearch.EndDate.ToShortDateString()+ "&clientId=" + TicketTimeSearch.clientid +
                                    "&projectid=" + TicketTimeSearch.projectid+
                                    "&teamuserid=" + TicketTimeSearch.teamuserid ;
                ViewBag.Link = queryString;
                return View(searchTimeVM);
            }

            TicketTimeViewModel timeVM = new TicketTimeViewModel
            {
                SearchTicketTime = new TicketTimeSearchViewModel
                {
                    StartDate = DateTime.Now.Date.Add(TimeSpan.Parse("00:00:00")),
                    EndDate = DateTime.Now.Date.Add(TimeSpan.Parse("23:59:59")),
                    projectid = null,
                    clientid = null,
                    ProjectCollection = db.Database.SqlQuery<CombinedEntity>("exec projects_loadcombined").Select(x =>
                        new SelectListItem
                        {
                            Text = x.name,
                            Value = x.id.ToString()
                        }).ToList(),
                    UsersCollection =
                        new SelectList(
                            UserManager.Users.OrderBy(u => u.FirstName).Where(us => us.isactive == true).ToList(), "Id",
                            "Email").ToList(),
                    ClientCollection = new SelectList(db.Client.ToList(), "id", "name").ToList()
                }
            };
            Session["startdate"] = timeVM.SearchTicketTime.StartDate;
            Session["endate"] = timeVM.SearchTicketTime.EndDate;
            timeVM.SearchResults = db.TicketTimeLog.Include(t => t.User).Include(t => t.Project).Include(t => t.Skill)
                .Include(t => t.TicketItem).Include(t => t.TeamUser)
                .Where(ttl =>
                    DbFunctions.TruncateTime(ttl.workdate) >= timeVM.SearchTicketTime.StartDate &&
                    DbFunctions.TruncateTime(ttl.workdate) <= timeVM.SearchTicketTime.EndDate)
                .Where(t => t.timespentinminutes != 0 || t.billabletimeinminutes != 0)
                .OrderByDescending(w => w.workdate).ToList();
            timeVM.SearchResults = timeVM.SearchResults.Where(x => x.Skill.name != "Quality Control").ToList();
            ViewBag.totalspent =
                CommonFunctions.RoundTwoDecimalPlaces((double)timeVM.SearchResults.Sum(t => t.timespentinminutes) /
                                                      60);
            ViewBag.totalbillable =
                CommonFunctions.RoundTwoDecimalPlaces((double)timeVM.SearchResults.Sum(t => t.billabletimeinminutes) /
                                                      60);

            Session["TicketTimeViewModelS"] = timeVM;
            Session["totalspent"] = ViewBag.totalspent;
            Session["totalbillable"] = ViewBag.totalbillable;
            Session["clientid"] = null;
            DataTable dtlist = ToDataTable(timeVM.SearchResults);
           
            return View(timeVM);
        }
        public ActionResult TeamTimeReport(DateTime StartDate, DateTime EndDate, long? projectid, string teamuserid, long? clientid)
        {
            if (ModelState.IsValid)
            {
                TicketTimeSearchViewModel TicketTimeSearch = new TicketTimeSearchViewModel()
                {
                    StartDate = StartDate,
                    EndDate = EndDate,
                    projectid = projectid,
                    teamuserid = teamuserid,
                    clientid = clientid
                };
                TicketTimeViewModel searchTimeVM = new TicketTimeViewModel
                {
                    SearchResults = new List<TicketTimeLog>(),
                    SearchTicketTime = new TicketTimeSearchViewModel
                    {
                        StartDate = TicketTimeSearch.StartDate.Date.Add(TimeSpan.Parse("00:00:00")),
                        EndDate = TicketTimeSearch.EndDate.Date.Add(TimeSpan.Parse("23:59:59")),
                        projectid = TicketTimeSearch.projectid,
                        teamuserid = TicketTimeSearch.teamuserid,
                        clientid = TicketTimeSearch.clientid,
                        ProjectCollection = db.Database.SqlQuery<CombinedEntity>("exec projects_loadcombined").Select(
                            x => new SelectListItem
                            {
                                Text = x.name,
                                Value = x.id.ToString()
                            }).ToList(),
                        UsersCollection = UserManager.Users.OrderBy(u => u.FirstName).Select(u => new SelectListItem
                        {
                            Text = u.FirstName + " " + u.LastName + " - " + u.Email,
                            Value = u.Id
                        }).ToList(),
                        ClientCollection = new SelectList(db.Client.ToList(), "id", "name").ToList()
                    }
                };

                if (TicketTimeSearch.projectid != null && TicketTimeSearch.teamuserid != null &&
                    TicketTimeSearch.clientid == null)
                {
                    searchTimeVM.SearchResults = db.TicketTimeLog.Include(t => t.User).Include(t => t.Project)
                        .Include(t => t.Skill).Include(t => t.TicketItem).Include(t => t.TeamUser)
                        .Where(ttl =>
                            DbFunctions.TruncateTime(ttl.workdate) >= searchTimeVM.SearchTicketTime.StartDate &&
                            DbFunctions.TruncateTime(ttl.workdate) <= searchTimeVM.SearchTicketTime.EndDate)
                        .Where(ttl => ttl.projectid == TicketTimeSearch.projectid)
                        .Where(ttl => ttl.teamuserid == TicketTimeSearch.teamuserid)
                        .Where(t => t.timespentinminutes != 0 || t.billabletimeinminutes != 0)
                        .OrderByDescending(w => w.workdate).ToList();
                }
                else if (TicketTimeSearch.projectid != null && TicketTimeSearch.teamuserid == null &&
                         (TicketTimeSearch.clientid == null || TicketTimeSearch.clientid != null))
                {
                    searchTimeVM.SearchResults = db.TicketTimeLog.Include(t => t.User).Include(t => t.Project)
                        .Include(t => t.Skill).Include(t => t.TicketItem).Include(t => t.TeamUser)
                        .Where(ttl =>
                            DbFunctions.TruncateTime(ttl.workdate) >= searchTimeVM.SearchTicketTime.StartDate &&
                            DbFunctions.TruncateTime(ttl.workdate) <= searchTimeVM.SearchTicketTime.EndDate)
                        .Where(ttl => ttl.projectid == TicketTimeSearch.projectid)
                        .Where(t => t.timespentinminutes != 0 || t.billabletimeinminutes != 0)
                        .OrderByDescending(w => w.workdate).ToList();
                }
                else if (TicketTimeSearch.projectid == null && TicketTimeSearch.teamuserid != null &&
                         TicketTimeSearch.clientid == null)
                {
                    searchTimeVM.SearchResults = db.TicketTimeLog.Include(t => t.User).Include(t => t.Project)
                        .Include(t => t.Skill).Include(t => t.TicketItem).Include(t => t.TeamUser)
                        .Where(ttl =>
                            DbFunctions.TruncateTime(ttl.workdate) >= searchTimeVM.SearchTicketTime.StartDate &&
                            DbFunctions.TruncateTime(ttl.workdate) <= searchTimeVM.SearchTicketTime.EndDate)
                        .Where(ttl => ttl.teamuserid == TicketTimeSearch.teamuserid)
                        .Where(t => t.timespentinminutes != 0 || t.billabletimeinminutes != 0)
                        .OrderByDescending(w => w.workdate).ToList();
                }
                else if (TicketTimeSearch.projectid == null && TicketTimeSearch.clientid != null &&
                         TicketTimeSearch.teamuserid == null)
                {
                    List<Client> clientwithsubclients = db.Client.Where(p => p.id == TicketTimeSearch.clientid)
                        .Include(c => c.SubClients).ToList();
                    if (clientwithsubclients != null && clientwithsubclients.Count > 0)
                    {
                        foreach (Client item in clientwithsubclients)
                        {
                            List<CombinedEntity> projects = db.Database
                                .SqlQuery<CombinedEntity>("exec projects_loadcombinedfortimelog @clientid",
                                    new SqlParameter("@clientid", item.id)).ToList();
                            if (projects != null && projects.Count > 0)
                            {
                                foreach (CombinedEntity items in projects)
                                {
                                    List<TicketTimeLog> timelog = db.TicketTimeLog.Include(t => t.User).Include(t => t.Project)
                                        .Include(t => t.Skill).Include(t => t.TicketItem).Include(t => t.TeamUser)
                                        .Where(ttl =>
                                            DbFunctions.TruncateTime(ttl.workdate) >=
                                            searchTimeVM.SearchTicketTime.StartDate &&
                                            DbFunctions.TruncateTime(ttl.workdate) <=
                                            searchTimeVM.SearchTicketTime.EndDate)
                                        .Where(ttl => ttl.projectid == items.id)
                                        .Where(t => t.timespentinminutes != 0 || t.billabletimeinminutes != 0)
                                        .OrderByDescending(w => w.workdate).ToList();
                                    if (timelog != null && timelog.Count > 0)
                                    {
                                        searchTimeVM.SearchResults.AddRange(timelog);
                                    }
                                }
                            }

                            if (item.SubClients != null && item.SubClients.Count > 0)
                            {
                                foreach (Client subitems in item.SubClients)
                                {
                                    List<CombinedEntity> projectsforsubclients = db.Database
                                        .SqlQuery<CombinedEntity>("exec projects_loadcombinedfortimelog @clientid",
                                            new SqlParameter("@clientid", subitems.id)).ToList();
                                    if (projectsforsubclients != null && projectsforsubclients.Count > 0)
                                    {
                                        foreach (CombinedEntity items in projectsforsubclients)
                                        {
                                            List<TicketTimeLog> timelog = db.TicketTimeLog.Include(t => t.User).Include(t => t.Project)
                                                .Include(t => t.Skill).Include(t => t.TicketItem)
                                                .Include(t => t.TeamUser)
                                                .Where(ttl =>
                                                    DbFunctions.TruncateTime(ttl.workdate) >=
                                                    searchTimeVM.SearchTicketTime.StartDate &&
                                                    DbFunctions.TruncateTime(ttl.workdate) <=
                                                    searchTimeVM.SearchTicketTime.EndDate)
                                                .Where(ttl => ttl.projectid == items.id)
                                                .Where(t => t.timespentinminutes != 0 || t.billabletimeinminutes != 0)
                                                .OrderByDescending(w => w.workdate).ToList();
                                            if (timelog != null && timelog.Count > 0)
                                            {
                                                searchTimeVM.SearchResults.AddRange(timelog);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else if (TicketTimeSearch.projectid == null && TicketTimeSearch.clientid != null &&
                         TicketTimeSearch.teamuserid != null)
                {
                    List<Client> clientwithsubclients = db.Client.Where(p => p.id == TicketTimeSearch.clientid)
                        .Include(c => c.SubClients).ToList();
                    if (clientwithsubclients != null && clientwithsubclients.Count > 0)
                    {
                        foreach (Client item in clientwithsubclients)
                        {
                            List<CombinedEntity> projects = db.Database
                                .SqlQuery<CombinedEntity>("exec projects_loadcombinedfortimelog @clientid",
                                    new SqlParameter("@clientid", item.id)).ToList();
                            if (projects != null && projects.Count > 0)
                            {
                                foreach (CombinedEntity items in projects)
                                {
                                    List<TicketTimeLog> timelog = db.TicketTimeLog.Include(t => t.User).Include(t => t.Project)
                                        .Include(t => t.Skill).Include(t => t.TicketItem).Include(t => t.TeamUser)
                                        .Where(ttl =>
                                            DbFunctions.TruncateTime(ttl.workdate) >=
                                            searchTimeVM.SearchTicketTime.StartDate &&
                                            DbFunctions.TruncateTime(ttl.workdate) <=
                                            searchTimeVM.SearchTicketTime.EndDate)
                                        .Where(ttl =>
                                            ttl.projectid == items.id && ttl.teamuserid == TicketTimeSearch.teamuserid)
                                        .Where(t => t.timespentinminutes != 0 || t.billabletimeinminutes != 0)
                                        .OrderByDescending(w => w.workdate).ToList();
                                    if (timelog != null && timelog.Count > 0)
                                    {
                                        searchTimeVM.SearchResults.AddRange(timelog);
                                    }
                                }
                            }

                            if (item.SubClients != null && item.SubClients.Count > 0)
                            {
                                foreach (Client subitems in item.SubClients)
                                {
                                    List<CombinedEntity> projectsforsubclients = db.Database
                                        .SqlQuery<CombinedEntity>("exec projects_loadcombinedfortimelog @clientid",
                                            new SqlParameter("@clientid", subitems.id)).ToList();
                                    if (projectsforsubclients != null && projectsforsubclients.Count > 0)
                                    {
                                        foreach (CombinedEntity items in projectsforsubclients)
                                        {
                                            List<TicketTimeLog> timelog = db.TicketTimeLog.Include(t => t.User).Include(t => t.Project)
                                                .Include(t => t.Skill).Include(t => t.TicketItem)
                                                .Include(t => t.TeamUser)
                                                .Where(ttl =>
                                                    DbFunctions.TruncateTime(ttl.workdate) >=
                                                    searchTimeVM.SearchTicketTime.StartDate &&
                                                    DbFunctions.TruncateTime(ttl.workdate) <=
                                                    searchTimeVM.SearchTicketTime.EndDate)
                                                .Where(ttl =>
                                                    ttl.projectid == subitems.id &&
                                                    ttl.teamuserid == TicketTimeSearch.teamuserid)
                                                .Where(t => t.timespentinminutes != 0 || t.billabletimeinminutes != 0)
                                                .OrderByDescending(w => w.workdate).ToList();
                                            if (timelog != null && timelog.Count > 0)
                                            {
                                                searchTimeVM.SearchResults.AddRange(timelog);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else if (TicketTimeSearch.projectid != null && TicketTimeSearch.clientid != null &&
                         TicketTimeSearch.teamuserid != null)
                {
                    searchTimeVM.SearchResults = db.TicketTimeLog.Include(t => t.User).Include(t => t.Project)
                        .Include(t => t.Skill).Include(t => t.TicketItem).Include(t => t.TeamUser)
                        .Where(ttl =>
                            DbFunctions.TruncateTime(ttl.workdate) >= searchTimeVM.SearchTicketTime.StartDate &&
                            DbFunctions.TruncateTime(ttl.workdate) <= searchTimeVM.SearchTicketTime.EndDate)
                        .Where(ttl => ttl.projectid == TicketTimeSearch.projectid)
                        .Where(ttl => ttl.teamuserid == TicketTimeSearch.teamuserid)
                        .Where(t => t.timespentinminutes != 0 || t.billabletimeinminutes != 0)
                        .OrderByDescending(w => w.workdate).ToList();
                }
                else
                {
                    searchTimeVM.SearchResults = db.TicketTimeLog.Include(t => t.User).Include(t => t.Project)
                        .Include(t => t.Skill).Include(t => t.TicketItem).Include(t => t.TeamUser).Where(ttl =>
                            DbFunctions.TruncateTime(ttl.workdate) >= searchTimeVM.SearchTicketTime.StartDate &&
                            DbFunctions.TruncateTime(ttl.workdate) <= searchTimeVM.SearchTicketTime.EndDate)
                        .Where(t => t.timespentinminutes != 0 || t.billabletimeinminutes != 0)
                        .OrderByDescending(w => w.workdate).ToList();
                }

                double stime = 0.00;
                double btime = 0.00;
                if (searchTimeVM.SearchResults != null && searchTimeVM.SearchResults.Count > 0)
                {
                    searchTimeVM.SearchResults = searchTimeVM.SearchResults
                        .Where(x => x.Skill.name != "Quality Control").ToList();
                    stime = CommonFunctions.RoundTwoDecimalPlaces(
                        (double)searchTimeVM.SearchResults.Sum(t => t.timespentinminutes) / 60);
                    btime = CommonFunctions.RoundTwoDecimalPlaces(
                        (double)searchTimeVM.SearchResults.Sum(t => t.billabletimeinminutes) / 60);
                }

                Session["startdate"] = searchTimeVM.SearchTicketTime.StartDate;
                Session["endate"] = searchTimeVM.SearchTicketTime.EndDate;
                Session["TicketTimeViewModelS"] = searchTimeVM;
                Session["totalspent"] = ViewBag.totalspent = stime;
                Session["totalbillable"] = ViewBag.totalbillable = btime;
                Session["clientid"] = TicketTimeSearch.clientid;
                string test = ViewExtensions.BaseUrl + "/TicketTimeLogs/TeamTimeReport?startdate=" + TicketTimeSearch.StartDate.ToShortDateString() + "&endDate=" + TicketTimeSearch.EndDate.ToShortDateString();
                //if (TicketTimeSearch.clientid != null)
                //{
                //    test += "&clientId=" + TicketTimeSearch.clientid;
                //}
                //if (TicketTimeSearch.projectid != null)
                //{
                //    test += "&projectid=" + TicketTimeSearch.projectid;
                //}
                //if (TicketTimeSearch.teamuserid != null)
                //{
                //    test += "&teamuserid=" + TicketTimeSearch.teamuserid;
                //}
                //ViewBag.Link = test;
                //ViewBag.totalspent = stime;
                //ViewBag.totalbillable = btime;
                return View(searchTimeVM);
            }

            TicketTimeViewModel timeVM = new TicketTimeViewModel
            {
                SearchTicketTime = new TicketTimeSearchViewModel
                {
                    StartDate = DateTime.Now.Date.Add(TimeSpan.Parse("00:00:00")),
                    EndDate = DateTime.Now.Date.Add(TimeSpan.Parse("23:59:59")),
                    projectid = null,
                    clientid = null,
                    ProjectCollection = db.Database.SqlQuery<CombinedEntity>("exec projects_loadcombined").Select(x =>
                        new SelectListItem
                        {
                            Text = x.name,
                            Value = x.id.ToString()
                        }).ToList(),
                    UsersCollection =
                        new SelectList(
                            UserManager.Users.OrderBy(u => u.FirstName).Where(us => us.isactive == true).ToList(), "Id",
                            "Email").ToList(),
                    ClientCollection = new SelectList(db.Client.ToList(), "id", "name").ToList()
                }
            };
            Session["startdate"] = timeVM.SearchTicketTime.StartDate;
            Session["endate"] = timeVM.SearchTicketTime.EndDate;
            timeVM.SearchResults = db.TicketTimeLog.Include(t => t.User).Include(t => t.Project).Include(t => t.Skill)
                .Include(t => t.TicketItem).Include(t => t.TeamUser)
                .Where(ttl =>
                    DbFunctions.TruncateTime(ttl.workdate) >= timeVM.SearchTicketTime.StartDate &&
                    DbFunctions.TruncateTime(ttl.workdate) <= timeVM.SearchTicketTime.EndDate)
                .Where(t => t.timespentinminutes != 0 || t.billabletimeinminutes != 0)
                .OrderByDescending(w => w.workdate).ToList();
            timeVM.SearchResults = timeVM.SearchResults.Where(x => x.Skill.name != "Quality Control").ToList();
            ViewBag.totalspent =
                CommonFunctions.RoundTwoDecimalPlaces((double)timeVM.SearchResults.Sum(t => t.timespentinminutes) /
                                                      60);
            ViewBag.totalbillable =
                CommonFunctions.RoundTwoDecimalPlaces((double)timeVM.SearchResults.Sum(t => t.billabletimeinminutes) /
                                                      60);

            Session["TicketTimeViewModelS"] = timeVM;
            Session["totalspent"] = ViewBag.totalspent;
            Session["totalbillable"] = ViewBag.totalbillable;
            Session["clientid"] = null;
            DataTable dtlist = ToDataTable(timeVM.SearchResults);

            return View(timeVM);
        }
        public ActionResult DetailTimeReport()
        {
            try
            {
                DateTime startofmnoth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                DateTime endofmonth = startofmnoth.AddMonths(1).AddDays(-1);
                endofmonth = endofmonth.Add(TimeSpan.Parse("23:59:59"));
                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"]
                           .ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("EmployeeSpentTimeByDateRange_sp", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@StartDate", SqlDbType.Date).Value = startofmnoth;
                        cmd.Parameters.Add("@EndDate", SqlDbType.Date).Value = endofmonth;
                        con.Open();
                        dt.Load(cmd.ExecuteReader());
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }

                return View(dt);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult DetailTimeReport(DateTime startdate, DateTime enddate, string TimeType)
        {
            try
            {
                string sp;
                switch (TimeType)
                {
                    case "spent":
                        sp = "EmployeeSpentTimeByDateRange_sp";
                        break;
                    case "bill":
                        sp = "EmployeeBillableTimeByDateRange_sp";
                        break;
                    case "both":
                        sp = "EmployeeSpentAndBillTimeByDateRange_sp";
                        break;
                    default:
                        sp = "EmployeeSpentTimeByDateRange_sp";
                        break;
                }
                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"]
                           .ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(sp, con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@StartDate", SqlDbType.Date).Value = startdate;
                        cmd.Parameters.Add("@EndDate", SqlDbType.Date).Value = enddate;
                        con.Open();
                        dt.Load(cmd.ExecuteReader());
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
                if(TimeType == "both")
                {
                    return PartialView("_SpentAndBillTimeReport", dt);
                }
                return PartialView("_DetailTimeReport", dt);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult QualityControl()
        {
            TicketTimeViewModel timeVM = new TicketTimeViewModel
            {
                SearchTicketTime = new TicketTimeSearchViewModel
                {
                    StartDate = DateTime.Now.Date.Add(TimeSpan.Parse("00:00:00")),
                    EndDate = DateTime.Now.Date.Add(TimeSpan.Parse("23:59:59")),
                    projectid = null,
                    clientid = null,
                    ProjectCollection = db.Database.SqlQuery<CombinedEntity>("exec projects_loadcombined").Select(x =>
                        new SelectListItem
                        {
                            Text = x.name,
                            Value = x.id.ToString()
                        }).ToList(),
                    UsersCollection = UserManager.Users.OrderBy(u => u.FirstName).Where(us => us.isactive == true)
                        .Select(u => new SelectListItem
                        {
                            Text = u.FirstName + " " + u.LastName + " - " + u.Email,
                            Value = u.Id
                        }).ToList(),
                    ClientCollection = new SelectList(db.Client.ToList(), "id", "name").ToList()
                }
            };
            timeVM.SearchResults = db.TicketTimeLog.Include(t => t.Project).Include(t => t.Skill)
                .Include(t => t.TicketItem).Include(t => t.TeamUser)
                .Where(ttl =>
                    DbFunctions.TruncateTime(ttl.workdate) >= timeVM.SearchTicketTime.StartDate &&
                    DbFunctions.TruncateTime(ttl.workdate) <= timeVM.SearchTicketTime.EndDate)
                .Where(t => t.timespentinminutes != 0 && t.billabletimeinminutes != 0)
                .OrderByDescending(w => w.workdate).ToList();
            timeVM.SearchResults = timeVM.SearchResults.Where(x => x.Skill.name == "Quality Control").ToList();
            ViewBag.totalspent =
                CommonFunctions.RoundTwoDecimalPlaces((double)timeVM.SearchResults.Sum(t => t.timespentinminutes) /
                                                      60);
            ViewBag.totalbillable =
                CommonFunctions.RoundTwoDecimalPlaces((double)timeVM.SearchResults.Sum(t => t.billabletimeinminutes) /
                                                      60);
            return View(timeVM);
        }

        [HttpPost]
        public ActionResult QualityControl(
            [Bind(Include = "StartDate,EndDate,projectid,teamuserid,clientid")]
            TicketTimeSearchViewModel TicketTimeSearch)
        {
            if (ModelState.IsValid)
            {
                TicketTimeViewModel searchTimeVM = new TicketTimeViewModel
                {
                    SearchResults = new List<TicketTimeLog>(),
                    SearchTicketTime = new TicketTimeSearchViewModel
                    {
                        StartDate = TicketTimeSearch.StartDate.Date.Add(TimeSpan.Parse("00:00:00")),
                        EndDate = TicketTimeSearch.EndDate.Date.Add(TimeSpan.Parse("23:59:59")),
                        projectid = TicketTimeSearch.projectid,
                        teamuserid = TicketTimeSearch.teamuserid,
                        clientid = TicketTimeSearch.clientid,
                        ProjectCollection = db.Database.SqlQuery<CombinedEntity>("exec projects_loadcombined").Select(
                            x => new SelectListItem
                            {
                                Text = x.name,
                                Value = x.id.ToString()
                            }).ToList(),
                        UsersCollection = UserManager.Users.OrderBy(u => u.FirstName).Where(us => us.isactive == true)
                            .Select(u => new SelectListItem
                            {
                                Text = u.FirstName + " " + u.LastName + " - " + u.Email,
                                Value = u.Id
                            }).ToList(),
                        ClientCollection = new SelectList(db.Client.ToList(), "id", "name").ToList()
                    }
                };

                if (TicketTimeSearch.projectid != null && TicketTimeSearch.teamuserid != null &&
                    TicketTimeSearch.clientid == null)
                {
                    searchTimeVM.SearchResults = db.TicketTimeLog.Include(t => t.Project).Include(t => t.Skill)
                        .Include(t => t.TicketItem).Include(t => t.TeamUser)
                        .Where(ttl =>
                            DbFunctions.TruncateTime(ttl.workdate) >= searchTimeVM.SearchTicketTime.StartDate &&
                            DbFunctions.TruncateTime(ttl.workdate) <= searchTimeVM.SearchTicketTime.EndDate)
                        .Where(ttl => ttl.projectid == TicketTimeSearch.projectid)
                        .Where(ttl => ttl.teamuserid == TicketTimeSearch.teamuserid)
                        .Where(t => t.timespentinminutes != 0 || t.billabletimeinminutes != 0)
                        .OrderByDescending(w => w.workdate).ToList();
                }
                else if (TicketTimeSearch.projectid != null && TicketTimeSearch.teamuserid == null &&
                         (TicketTimeSearch.clientid == null || TicketTimeSearch.clientid != null))
                {
                    searchTimeVM.SearchResults = db.TicketTimeLog.Include(t => t.Project).Include(t => t.Skill)
                        .Include(t => t.TicketItem).Include(t => t.TeamUser)
                        .Where(ttl =>
                            DbFunctions.TruncateTime(ttl.workdate) >= searchTimeVM.SearchTicketTime.StartDate &&
                            DbFunctions.TruncateTime(ttl.workdate) <= searchTimeVM.SearchTicketTime.EndDate)
                        .Where(ttl => ttl.projectid == TicketTimeSearch.projectid)
                        .Where(t => t.timespentinminutes != 0 || t.billabletimeinminutes != 0)
                        .OrderByDescending(w => w.workdate).ToList();
                }
                else if (TicketTimeSearch.projectid == null && TicketTimeSearch.teamuserid != null &&
                         TicketTimeSearch.clientid == null)
                {
                    searchTimeVM.SearchResults = db.TicketTimeLog.Include(t => t.Project).Include(t => t.Skill)
                        .Include(t => t.TicketItem).Include(t => t.TeamUser)
                        .Where(ttl =>
                            DbFunctions.TruncateTime(ttl.workdate) >= searchTimeVM.SearchTicketTime.StartDate &&
                            DbFunctions.TruncateTime(ttl.workdate) <= searchTimeVM.SearchTicketTime.EndDate)
                        .Where(ttl => ttl.teamuserid == TicketTimeSearch.teamuserid)
                        .Where(t => t.timespentinminutes != 0 || t.billabletimeinminutes != 0)
                        .OrderByDescending(w => w.workdate).ToList();
                }
                else if (TicketTimeSearch.projectid == null && TicketTimeSearch.clientid != null &&
                         TicketTimeSearch.teamuserid == null)
                {
                    List<Client> clientwithsubclients = db.Client.Where(p => p.id == TicketTimeSearch.clientid)
                        .Include(c => c.SubClients).ToList();
                    if (clientwithsubclients != null && clientwithsubclients.Count > 0)
                    {
                        foreach (Client item in clientwithsubclients)
                        {
                            List<CombinedEntity> projects = db.Database
                                .SqlQuery<CombinedEntity>("exec projects_loadcombinedfortimelog @clientid",
                                    new SqlParameter("@clientid", item.id)).ToList();
                            if (projects != null && projects.Count > 0)
                            {
                                foreach (CombinedEntity items in projects)
                                {
                                    List<TicketTimeLog> timelog = db.TicketTimeLog.Include(t => t.Project).Include(t => t.Skill)
                                        .Include(t => t.TicketItem).Include(t => t.TeamUser)
                                        .Where(ttl =>
                                            DbFunctions.TruncateTime(ttl.workdate) >=
                                            searchTimeVM.SearchTicketTime.StartDate &&
                                            DbFunctions.TruncateTime(ttl.workdate) <=
                                            searchTimeVM.SearchTicketTime.EndDate)
                                        .Where(ttl => ttl.projectid == items.id)
                                        .Where(t => t.timespentinminutes != 0 || t.billabletimeinminutes != 0)
                                        .OrderByDescending(w => w.workdate).ToList();
                                    if (timelog != null && timelog.Count > 0)
                                    {
                                        searchTimeVM.SearchResults.AddRange(timelog);
                                    }
                                }
                            }

                            if (item.SubClients != null && item.SubClients.Count > 0)
                            {
                                foreach (Client subitems in item.SubClients)
                                {
                                    List<CombinedEntity> projectsforsubclients = db.Database
                                        .SqlQuery<CombinedEntity>("exec projects_loadcombinedfortimelog @clientid",
                                            new SqlParameter("@clientid", subitems.id)).ToList();
                                    if (projectsforsubclients != null && projectsforsubclients.Count > 0)
                                    {
                                        foreach (CombinedEntity items in projectsforsubclients)
                                        {
                                            List<TicketTimeLog> timelog = db.TicketTimeLog.Include(t => t.Project).Include(t => t.Skill)
                                                .Include(t => t.TicketItem).Include(t => t.TeamUser)
                                                .Where(ttl =>
                                                    DbFunctions.TruncateTime(ttl.workdate) >=
                                                    searchTimeVM.SearchTicketTime.StartDate &&
                                                    DbFunctions.TruncateTime(ttl.workdate) <=
                                                    searchTimeVM.SearchTicketTime.EndDate)
                                                .Where(ttl => ttl.projectid == items.id)
                                                .Where(t => t.timespentinminutes != 0 || t.billabletimeinminutes != 0)
                                                .OrderByDescending(w => w.workdate).ToList();
                                            if (timelog != null && timelog.Count > 0)
                                            {
                                                searchTimeVM.SearchResults.AddRange(timelog);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else if (TicketTimeSearch.projectid == null && TicketTimeSearch.clientid != null &&
                         TicketTimeSearch.teamuserid != null)
                {
                    List<Client> clientwithsubclients = db.Client.Where(p => p.id == TicketTimeSearch.clientid)
                        .Include(c => c.SubClients).ToList();
                    if (clientwithsubclients != null && clientwithsubclients.Count > 0)
                    {
                        foreach (Client item in clientwithsubclients)
                        {
                            List<CombinedEntity> projects = db.Database
                                .SqlQuery<CombinedEntity>("exec projects_loadcombinedfortimelog @clientid",
                                    new SqlParameter("@clientid", item.id)).ToList();
                            if (projects != null && projects.Count > 0)
                            {
                                foreach (CombinedEntity items in projects)
                                {
                                    List<TicketTimeLog> timelog = db.TicketTimeLog.Include(t => t.Project).Include(t => t.Skill)
                                        .Include(t => t.TicketItem).Include(t => t.TeamUser)
                                        .Where(ttl =>
                                            DbFunctions.TruncateTime(ttl.workdate) >=
                                            searchTimeVM.SearchTicketTime.StartDate &&
                                            DbFunctions.TruncateTime(ttl.workdate) <=
                                            searchTimeVM.SearchTicketTime.EndDate)
                                        .Where(ttl =>
                                            ttl.projectid == items.id && ttl.teamuserid == TicketTimeSearch.teamuserid)
                                        .Where(t => t.timespentinminutes != 0 || t.billabletimeinminutes != 0)
                                        .OrderByDescending(w => w.workdate).ToList();
                                    if (timelog != null && timelog.Count > 0)
                                    {
                                        searchTimeVM.SearchResults.AddRange(timelog);
                                    }
                                }
                            }

                            if (item.SubClients != null && item.SubClients.Count > 0)
                            {
                                foreach (Client subitems in item.SubClients)
                                {
                                    List<CombinedEntity> projectsforsubclients = db.Database
                                        .SqlQuery<CombinedEntity>("exec projects_loadcombinedfortimelog @clientid",
                                            new SqlParameter("@clientid", subitems.id)).ToList();
                                    if (projectsforsubclients != null && projectsforsubclients.Count > 0)
                                    {
                                        foreach (CombinedEntity items in projectsforsubclients)
                                        {
                                            List<TicketTimeLog> timelog = db.TicketTimeLog.Include(t => t.Project).Include(t => t.Skill)
                                                .Include(t => t.TicketItem).Include(t => t.TeamUser)
                                                .Where(ttl =>
                                                    DbFunctions.TruncateTime(ttl.workdate) >=
                                                    searchTimeVM.SearchTicketTime.StartDate &&
                                                    DbFunctions.TruncateTime(ttl.workdate) <=
                                                    searchTimeVM.SearchTicketTime.EndDate)
                                                .Where(ttl =>
                                                    ttl.projectid == subitems.id &&
                                                    ttl.teamuserid == TicketTimeSearch.teamuserid)
                                                .Where(t => t.timespentinminutes != 0 || t.billabletimeinminutes != 0)
                                                .OrderByDescending(w => w.workdate).ToList();
                                            if (timelog != null && timelog.Count > 0)
                                            {
                                                searchTimeVM.SearchResults.AddRange(timelog);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else if (TicketTimeSearch.projectid != null && TicketTimeSearch.clientid != null &&
                         TicketTimeSearch.teamuserid != null)
                {
                    searchTimeVM.SearchResults = db.TicketTimeLog.Include(t => t.Project).Include(t => t.Skill)
                        .Include(t => t.TicketItem).Include(t => t.TeamUser)
                        .Where(ttl =>
                            DbFunctions.TruncateTime(ttl.workdate) >= searchTimeVM.SearchTicketTime.StartDate &&
                            DbFunctions.TruncateTime(ttl.workdate) <= searchTimeVM.SearchTicketTime.EndDate)
                        .Where(ttl => ttl.projectid == TicketTimeSearch.projectid)
                        .Where(ttl => ttl.teamuserid == TicketTimeSearch.teamuserid)
                        .Where(t => t.timespentinminutes != 0 || t.billabletimeinminutes != 0)
                        .OrderByDescending(w => w.workdate).ToList();
                }
                else
                {
                    searchTimeVM.SearchResults = db.TicketTimeLog.Include(t => t.Project).Include(t => t.Skill)
                        .Include(t => t.TicketItem).Include(t => t.TeamUser).Where(ttl =>
                            DbFunctions.TruncateTime(ttl.workdate) >= searchTimeVM.SearchTicketTime.StartDate &&
                            DbFunctions.TruncateTime(ttl.workdate) <= searchTimeVM.SearchTicketTime.EndDate)
                        .Where(t => t.timespentinminutes != 0 || t.billabletimeinminutes != 0)
                        .OrderByDescending(w => w.workdate).ToList();
                }

                double stime = 0.00;
                double btime = 0.00;
                if (searchTimeVM.SearchResults != null && searchTimeVM.SearchResults.Count > 0)
                {
                    searchTimeVM.SearchResults = searchTimeVM.SearchResults
                        .Where(x => x.Skill.name == "Quality Control").ToList();
                    stime = CommonFunctions.RoundTwoDecimalPlaces(
                        (double)searchTimeVM.SearchResults.Sum(t => t.timespentinminutes) / 60);
                    btime = CommonFunctions.RoundTwoDecimalPlaces(
                        (double)searchTimeVM.SearchResults.Sum(t => t.billabletimeinminutes) / 60);
                }

                ViewBag.totalspent = stime;
                ViewBag.totalbillable = btime;
                return View(searchTimeVM);
            }

            TicketTimeViewModel timeVM = new TicketTimeViewModel
            {
                SearchTicketTime = new TicketTimeSearchViewModel
                {
                    StartDate = DateTime.Now.Date.Add(TimeSpan.Parse("00:00:00")),
                    EndDate = DateTime.Now.Date.Add(TimeSpan.Parse("23:59:59")),
                    projectid = null,
                    clientid = null,
                    ProjectCollection = db.Database.SqlQuery<CombinedEntity>("exec projects_loadcombined").Select(x =>
                        new SelectListItem
                        {
                            Text = x.name,
                            Value = x.id.ToString()
                        }).ToList(),
                    UsersCollection =
                        new SelectList(
                            UserManager.Users.OrderBy(u => u.FirstName).Where(us => us.isactive == true).ToList(), "Id",
                            "Email").ToList(),
                    ClientCollection = new SelectList(db.Client.ToList(), "id", "name").ToList()
                }
            };
            timeVM.SearchResults = db.TicketTimeLog.Include(t => t.Project).Include(t => t.Skill)
                .Include(t => t.TicketItem).Include(t => t.TeamUser)
                .Where(ttl =>
                    DbFunctions.TruncateTime(ttl.workdate) >= timeVM.SearchTicketTime.StartDate &&
                    DbFunctions.TruncateTime(ttl.workdate) <= timeVM.SearchTicketTime.EndDate)
                .Where(t => t.timespentinminutes != 0 || t.billabletimeinminutes != 0)
                .OrderByDescending(w => w.workdate).ToList();
            timeVM.SearchResults = timeVM.SearchResults.Where(x => x.Skill.name == "Quality Control").ToList();
            ViewBag.totalspent =
                CommonFunctions.RoundTwoDecimalPlaces((double)timeVM.SearchResults.Sum(t => t.timespentinminutes) /
                                                      60);
            ViewBag.totalbillable =
                CommonFunctions.RoundTwoDecimalPlaces((double)timeVM.SearchResults.Sum(t => t.billabletimeinminutes) /
                                                      60);
            return View(timeVM);
        }

        // GET: TicketTimeLogs/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            TicketTimeLog ticketTimeLog = db.TicketTimeLog.Find(id);
            if (ticketTimeLog == null)
            {
                return HttpNotFound();
            }

            return View(ticketTimeLog);
        }

        // GET: TicketTimeLogs/Create
        public ActionResult Create(long id)
        {
            List<SelectListItem> timespentinminutes = new List<SelectListItem>();
            int timespentinminute = 0;
            int Counter = 1;
            for (Counter = 1; Counter <= 120; Counter++)
            {
                timespentinminute = timespentinminute + 5;
                timespentinminutes.Add(new SelectListItem
                { Text = timespentinminute.ToString(), Value = timespentinminute.ToString() });
            }

            ViewBag.timespan = timespentinminutes;
            List<SelectListItem> projectlist = db.Database.SqlQuery<CombinedEntity>("exec projects_loadcombined").Select(x =>
                new SelectListItem
                {
                    Text = x.name,
                    Value = x.id.ToString()
                }).ToList();
            ViewBag.projectid = new SelectList(projectlist, "Value", "Text");
            ViewBag.skillid = new SelectList(db.Skill, "id", "name");
            ViewBag.ticketitemid = new SelectList(db.TicketItem, "id", "emailmessageid");
            return View();
        }

        [HttpPost]
        public ActionResult Addbuckettime(List<AddTImeBucketViewModel> model)
        {
            if (model != null && model.Count > 0)
            {
                string userid = User.Identity.GetUserId();
                ApplicationUser user = db.Users.Find(userid);
                DateTime date = DateTimeExtensions.AddWorkingDays(-2);
                foreach (AddTImeBucketViewModel items in model)
                {
                    TicketTimeLog tickettimelog = db.TicketTimeLog.Where(li => li.id == items.id).FirstOrDefault();
                    if (items == null)
                    {
                        continue;
                    }

                    if (date.Date <= tickettimelog.workdate.Date || user.IsRestrictEntertime)
                    {
                        tickettimelog.timespentinminutes = items.timespent;
                        if (items.billtime == null || items.billtime == 0)
                        {
                            tickettimelog.billabletimeinminutes = items.timespent;
                        }
                        else
                        {
                            tickettimelog.billabletimeinminutes = items.billtime;
                        }

                        tickettimelog.updatedonutc = DateTime.Now;
                        tickettimelog.description = items.description;
                        tickettimelog.ipused = Request.UserHostAddress;
                        tickettimelog.userid = User.Identity.GetUserId();
                        db.Entry(tickettimelog).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        return Json(new { error = true, errortext = "Time Entry Restricted!" });
                    }
                }

                return Json(new { error = false });
            }

            return Json(new { error = true, errortext = "" });
        }

        public ActionResult AddTime(long tcktitemid, long pid, long sid, int spenttime, int? billtime,
            DateTime workdate, string title, string description, string comments)
        {
            try
            {
                string userid = User.Identity.GetUserId();
                TicketTimeLog timelog = db.TicketTimeLog.Where(til => til.ticketitemid == tcktitemid)
                    .Where(til => til.teamuserid == userid)
                    .Where(til => DbFunctions.TruncateTime(til.workdate) == workdate).FirstOrDefault();

                if (timelog == null)
                {
                    timelog = new TicketTimeLog();
                }
                else
                {
                    return Json(new { success = true, Successtext = "The Ticket Item Time Added." });
                }

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

                timelog.ticketitemid = tcktitemid;
                timelog.teamuserid = User.Identity.GetUserId();
                timelog.projectid = pid;
                timelog.skillid = sid;
                timelog.timespentinminutes = spenttime;
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
                ApplicationUser user = UserManager.FindById(userid);
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

        // POST: TicketTimeLogs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Create(
            [Bind(Include =
                "id,ticketitemid,teamuserid,projectid,skillid,workdate,title,description,comments,timespentinminutes,billabletimeinminutes,createdonutc,updatedonutc,ipused,userid")]
            TicketTimeLog ticketTimeLog)
        {
            if (ModelState.IsValid)
            {
                ticketTimeLog.createdonutc = DateTime.Now;
                ticketTimeLog.updatedonutc = DateTime.Now;
                ticketTimeLog.ipused = Request.UserHostAddress;
                ticketTimeLog.userid = User.Identity.GetUserId();
                ticketTimeLog.teamuserid = User.Identity.GetUserId();
                db.TicketTimeLog.Add(ticketTimeLog);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            List<SelectListItem> projectlist = db.Database.SqlQuery<CombinedEntity>("exec projects_loadcombined").Select(x =>
                new SelectListItem
                {
                    Text = x.name,
                    Value = x.id.ToString()
                }).ToList();
            ViewBag.projectid = new SelectList(projectlist, "Value", "Text", ticketTimeLog.projectid);
            ViewBag.skillid = new SelectList(db.Skill, "id", "name", ticketTimeLog.skillid);
            ViewBag.ticketitemid = new SelectList(db.TicketItem, "id", "emailmessageid", ticketTimeLog.ticketitemid);
            return View(ticketTimeLog);
        }

        // GET: TicketTimeLogs/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            TicketTimeLog ticketTimeLog = db.TicketTimeLog.Find(id);
            if (ticketTimeLog == null)
            {
                return HttpNotFound();
            }

            List<SelectListItem> projectlist = db.Database.SqlQuery<CombinedEntity>("exec projects_loadcombined").Select(x =>
                new SelectListItem
                {
                    Text = x.name,
                    Value = x.id.ToString()
                }).ToList();
            ViewBag.projectid = new SelectList(projectlist, "Value", "Text", ticketTimeLog.projectid);
            ViewBag.skillid = new SelectList(db.Skill, "id", "name", ticketTimeLog.skillid);
            ViewBag.ticketitemid = new SelectList(db.TicketItem, "id", "emailmessageid", ticketTimeLog.ticketitemid);
            return View(ticketTimeLog);
        }

        // POST: TicketTimeLogs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Edit(
            [Bind(Include =
                "id,ticketitemid,teamuserid,projectid,skillid,workdate,title,description,comments,timespentinminutes,billabletimeinminutes,createdonutc,updatedonutc,ipused,userid")]
            TicketTimeLog ticketTimeLog)
        {
            if (ModelState.IsValid)
            {
                string userid = User.Identity.GetUserId();
                ApplicationUser user = db.Users.Find(userid);
                DateTime date = DateTimeExtensions.AddWorkingDays(-2);
                if (date.Date <= ticketTimeLog.workdate.Date || user.IsRestrictEntertime)
                {
                    if (ticketTimeLog.billabletimeinminutes == null)
                    {
                        ticketTimeLog.billabletimeinminutes = ticketTimeLog.timespentinminutes;
                    }

                    ticketTimeLog.updatedonutc = DateTime.Now;
                    ticketTimeLog.ipused = Request.UserHostAddress;
                    ticketTimeLog.userid = User.Identity.GetUserId();
                    db.Entry(ticketTimeLog).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                ModelState.AddModelError("", "Time Entry is Restricted!");
                List<SelectListItem> projectlist = db.Database.SqlQuery<CombinedEntity>("exec projects_loadcombined").Select(x =>
                    new SelectListItem
                    {
                        Text = x.name,
                        Value = x.id.ToString()
                    }).ToList();
                ViewBag.projectid = new SelectList(projectlist, "Value", "Text", ticketTimeLog.projectid);
                ViewBag.skillid = new SelectList(db.Skill, "id", "name", ticketTimeLog.skillid);
                ViewBag.ticketitemid =
                    new SelectList(db.TicketItem, "id", "emailmessageid", ticketTimeLog.ticketitemid);
                return View(ticketTimeLog);
            }
            else
            {
                List<SelectListItem> projectlist = db.Database.SqlQuery<CombinedEntity>("exec projects_loadcombined").Select(x =>
                    new SelectListItem
                    {
                        Text = x.name,
                        Value = x.id.ToString()
                    }).ToList();
                ViewBag.projectid = new SelectList(projectlist, "Value", "Text", ticketTimeLog.projectid);
                ViewBag.skillid = new SelectList(db.Skill, "id", "name", ticketTimeLog.skillid);
                ViewBag.ticketitemid =
                    new SelectList(db.TicketItem, "id", "emailmessageid", ticketTimeLog.ticketitemid);
                return View(ticketTimeLog);
            }
        }

        // POST: TicketTimeLogs/Delete/5
        [HttpGet]
        [ActionName("Delete")]
        public ActionResult DeleteConfirmed(long id)
        {
            try
            {
                TicketTimeLog ticketTimeLog = db.TicketTimeLog.Find(id);
                db.TicketTimeLog.Remove(ticketTimeLog);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return Json(new { error = true, errortext = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [AllowAnonymous]
        public ActionResult GetTimelog()
        {
            DateTime startofmnoth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            DateTime endofmonth = startofmnoth.AddMonths(1).AddDays(-1);
            endofmonth = endofmonth.Add(TimeSpan.Parse("23:59:59"));
            try
            {
                List<TimelogViewModel> tlvm = db.TicketTimeLog.Where(tl => tl.workdate >= startofmnoth && tl.workdate <= endofmonth)
                    .GroupBy(l => l.TeamUser.UserName)
                    .Select(cl => new TimelogViewModel
                    {
                        fullname = cl.FirstOrDefault().TeamUser.FirstName + " " + cl.FirstOrDefault().TeamUser.LastName,
                        username = cl.FirstOrDefault().TeamUser.UserName,
                        timespent = Math.Round((double)cl.Sum(c => c.timespentinminutes) / 60, 2),
                        billabletime = Math.Round((double)cl.Sum(c => c.billabletimeinminutes) / 60, 2)
                    }).Where(h => h.timespent != 0).ToList();
                db.SaveChanges();
                return View("timelog", tlvm.OrderBy(n => n.fullname));
            }
            catch (Exception ex)
            {
                return View("timelog", ex.Message);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult GetTimelog(DateTime startofmnoth, DateTime endofmonth)
        {
            endofmonth = endofmonth.Add(TimeSpan.Parse("23:59:59"));
            try
            {
                List<TimelogViewModel> tlvm = db.TicketTimeLog.Where(tl => tl.workdate >= startofmnoth && tl.workdate <= endofmonth)
                    .GroupBy(l => l.TeamUser.UserName)
                    .Select(cl => new TimelogViewModel
                    {
                        fullname = cl.FirstOrDefault().TeamUser.FirstName + " " + cl.FirstOrDefault().TeamUser.LastName,
                        username = cl.FirstOrDefault().TeamUser.UserName,
                        timespent = Math.Round((double)cl.Sum(c => c.timespentinminutes) / 60, 2),
                        billabletime = Math.Round((double)cl.Sum(c => c.billabletimeinminutes) / 60, 2)
                    }).Where(h => h.timespent != 0).ToList();
                db.SaveChanges();
                return PartialView("_timelog", tlvm.OrderBy(n => n.fullname));
            }
            catch (Exception ex)
            {
                return PartialView("_timelog", ex.Message);
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


        #region Bucket Methods

        public ActionResult SaveMultipleBuckets(DateTime? bucketdate, List<string> ticketlist)
        {
            // validate requried information.
            if (!bucketdate.HasValue)
            {
                return Json(new { error = true, errortext = "The bucket date is required." });
            }

            if (ticketlist == null || ticketlist.Count == 0)
            {
                return Json(new { error = true, errortext = "Sorry no ticket has been selected." });
            }

            try
            {
                // Iterate through all the ticket items and add bucket entry.
                foreach (string ticketitem in ticketlist)
                {
                    long ticketid = Convert.ToInt64(ticketitem);
                    Ticket ticket = db.Ticket.Where(x => x.id == ticketid).Include(x => x.TicketItems).SingleOrDefault();

                    if (ticket == null)
                    {
                        return Json(new { error = true, errortext = "Sorry no ticket found." });
                    }

                    if (ticket.TicketItems == null)
                    {
                        return Json(new { error = true, errortext = "Sorry no ticket email found." });
                    }

                    TicketItem ticketItem = ticket.TicketItems.OrderBy(x => x.datetimereceived)
                        .Where(x => x.projectid > 0 && x.skillid > 0).FirstOrDefault();

                    if (ticketItem == null)
                    {
                        return Json(new { error = true, errortext = "Sorry, project and skill missing." });
                    }

                    string teamuserid = User.Identity.GetUserId();

                    if (ticketItem != null)
                    {
                        TicketTimeLog ticketTimeLog = db.TicketTimeLog.Where(til => til.ticketitemid == ticketItem.id)
                            .Where(til => til.teamuserid == teamuserid)
                            .Where(til => DbFunctions.TruncateTime(til.workdate) == bucketdate.Value).FirstOrDefault();

                        if (ticketTimeLog == null)
                        {
                            ticketTimeLog = new TicketTimeLog();
                        }
                        else
                        {
                            continue;
                        }

                        if (ticketItem.projectid == null)
                        {
                            continue;
                        }

                        ticketTimeLog.ticketitemid = ticketItem.id;
                        ticketTimeLog.teamuserid = teamuserid;
                        ticketTimeLog.projectid = ticketItem.projectid;
                        ticketTimeLog.skillid = ticketItem.skillid;
                        ticketTimeLog.timespentinminutes = 0;
                        ticketTimeLog.billabletimeinminutes = 0;
                        ticketTimeLog.workdate = bucketdate.Value.Add(DateTime.Now.TimeOfDay);
                        ticketTimeLog.title = ticketItem.subject;
                        ticketTimeLog.description = string.Empty;
                        ticketTimeLog.comments = string.Empty;
                        ticketTimeLog.createdonutc = DateTime.Now;
                        ticketTimeLog.updatedonutc = DateTime.Now;
                        ticketTimeLog.ipused = Request.UserHostAddress;
                        ticketTimeLog.userid = User.Identity.GetUserId();
                        db.TicketTimeLog.Add(ticketTimeLog);
                    }
                }

                db.SaveChanges();

                return Json(new
                { success = true, Successtext = "The tickets have been added to the bucket successfully." });
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

        #endregion

        #region ExportMethod

        public DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);
            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name);
            }

            foreach (T item in items)
            {
                object[] values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }

                dataTable.Rows.Add(values);
            }

            //put a breakpoint here and check datatable
            return dataTable;
        }

        [HttpPost]
        public ActionResult Export()
        {
            TicketTimeViewModel timeVM = new TicketTimeViewModel();
            timeVM = (TicketTimeViewModel)Session["TicketTimeViewModelS"];

            if (timeVM.SearchResults != null && timeVM.SearchResults.Count > 0)
            {
                DataTable dtlist1 = ToDataTable(timeVM.SearchResults);

                DataView view = new DataView(dtlist1);
                DataTable distinctValues = view.ToTable(true, "projectid");

                DataTable newdtwithclient = new DataTable();
                newdtwithclient.Columns.Add("projectid", typeof(long));
                newdtwithclient.Columns.Add("clientid", typeof(long));
                newdtwithclient.Columns.Add("projectname", typeof(string));
                newdtwithclient.Columns.Add("maxhours", typeof(float));
                newdtwithclient.Columns.Add("workingdate", typeof(string));

                double btime = 0.00;
                double overallTotal = 0.00;

                foreach (DataRow dtrow in distinctValues.Rows)
                {
                    Project project = db.Project.Find(Convert.ToInt64(dtrow["projectid"]));
                    Client client = db.Client.Find(Convert.ToInt64(project.clientid));
                    DataRow row = newdtwithclient.NewRow();
                    row["projectid"] = Convert.ToInt64(dtrow["projectid"]);
                    row["clientid"] = project.clientid;
                    row["projectname"] = project.name;

                    newdtwithclient.Rows.Add(row);
                }


                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                ExcelPackage excel = new ExcelPackage();
                ExcelWorksheet workSheet = excel.Workbook.Worksheets.Add("Sheet1");
                workSheet.Columns[1].Width = 50;
                workSheet.Columns[2].Width = 65;
                workSheet.Columns[3].Width = 30;
                workSheet.Columns[4].Width = 25;
                workSheet.TabColor = Color.Black;
                workSheet.DefaultRowHeight = 12;
                //Header of table  
                workSheet.Row(1).Height = 25;
                workSheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                workSheet.Row(1).Style.Font.Bold = true;
                workSheet.Cells[1, 1].Value = "Title";
                workSheet.Cells[1, 2].Value = "Description";
                workSheet.Cells[1, 3].Value = "Work Date";
                workSheet.Cells[1, 4].Value = "Billable Time[Minutes]";
                //  
                int recordIndex = 3;

                foreach (DataRow row in newdtwithclient.Rows)
                {
                    workSheet.Cells[recordIndex, 1].Value = row["projectname"].ToString();
                    workSheet.Row(recordIndex).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    workSheet.Row(recordIndex).Style.Font.Bold = true;
                    recordIndex++;
                    List<TicketTimeLog> searchTimeVMlist = new List<TicketTimeLog>();
                    searchTimeVMlist = timeVM.SearchResults.Where(x => x.projectid == Convert.ToInt64(row["projectid"]))
                        .OrderBy(x => x.workdate).ToList();
                    btime = CommonFunctions.RoundTwoDecimalPlaces(
                        (double)searchTimeVMlist.Sum(t => t.billabletimeinminutes) / 60);

                    foreach (TicketTimeLog item in searchTimeVMlist)
                    {
                        workSheet.Cells[recordIndex++, 1].Value = item.title;
                        recordIndex--;
                        workSheet.Cells[recordIndex, 2].Value = item.description;
                        workSheet.Cells[recordIndex, 3].Value = item.workdate.ToString("MM/dd/yyyy");
                        workSheet.Cells[recordIndex, 4].Value = item.billabletimeinminutes;
                        recordIndex++;
                    }

                    recordIndex++;
                    workSheet.Cells[recordIndex, 3].Value = "Total billable time";
                    workSheet.Cells[recordIndex, 4].Value = btime;
                    workSheet.Row(recordIndex).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    workSheet.Row(recordIndex).Style.Font.Bold = true;
                    recordIndex++;
                    overallTotal += btime;
                }

                recordIndex++;
                recordIndex++;
                workSheet.Cells[recordIndex, 3].Value = "Overall Total";
                workSheet.Cells[recordIndex, 4].Value = overallTotal;
                workSheet.Row(recordIndex).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                workSheet.Row(recordIndex).Style.Font.Bold = true;
                workSheet.Row(recordIndex).Style.Font.Size = 15;
                string excelName = "timesheetbilling";

                if (!string.IsNullOrEmpty(Convert.ToString(Session["clientid"])))
                {
                    Client client = db.Client.Find((long)Session["clientid"]);
                    excelName = client.name;
                    excelName = excelName.Replace(" ", "_");
                }
                else
                {
                    excelName = "Hours_" + "from_" + ((DateTime)Session["startdate"]).ToString("MMddyyyy") + "_To_" +
                                ((DateTime)Session["endate"]).ToString("MMddyyyy");
                }

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition", "attachment; filename=" + excelName + ".xlsx");
                    excel.SaveAs(memoryStream);
                    memoryStream.WriteTo(Response.OutputStream);
                    Response.Flush();
                    Response.End();
                }

                return View();
            }

            return RedirectToAction("Index", "tickettimelogs", new { id = 1 });
        }

        #endregion
    }
}