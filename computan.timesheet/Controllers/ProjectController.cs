using computan.timesheet.Contexts;
using computan.timesheet.core;
using computan.timesheet.Extensions;
using computan.timesheet.Helpers;
using computan.timesheet.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

// this controller is added to simplfy the  project dashboard controller
// so delete ProjectDashboardController after shifting this controller
namespace computan.timesheet.Controllers
{
    [CustomeAuthorizeAttribute]
    public class ProjectController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();
        private ApplicationUserManager _userManager;

        public ApplicationUserManager UserManager
        {
            get => _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            private set => _userManager = value;
        }

        public ActionResult Index(long id, int? statusid)
        {
            IQueryable<Ticket> ticket;
            List<SelectListItem> projectlist = db.Database.SqlQuery<CombinedEntity>("exec projects_loadcombined").Select(x =>
                new SelectListItem
                {
                    Text = x.name,
                    Value = x.id.ToString()
                }).ToList();
            ViewBag.users = UserManager.Users.OrderBy(f => f.FirstName).Where(u => u.isactive == true).ToList().AsQueryable();
            //List<Team> ActTeams = db.Team.OrderBy(f => f.name).Where(f => f.isactive == true).ToList();
            ViewBag.teams = db.Team.OrderBy(f => f.name).Where(f => f.isactive == true).ToList().AsQueryable();
            ViewBag.projects = new SelectList(projectlist, "Value", "Text");
            IQueryable<SelectListItem> ClientSelectList = new SelectList(db.Client.Where(x => x.isactive).ToList(), "id", "name").ToList().AsQueryable();
            ViewBag.clients = ClientSelectList;
            ViewBag.skills = db.Skill.ToList().AsQueryable();

            #region get tickets

            if (statusid == null)
            {
                ticket = (from tt in db.Ticket
                          join ticketi in db.TicketItem on tt.id equals ticketi.ticketid
                          where tt.projectid == id && tt.statusid == 2
                          select tt
                    ).Include(t => t.FlagStatus).Include(t => t.StatusUpdatedByUser).Include(tt => tt.TicketType)
                    .Include(t => t.ConversationStatus).GroupBy(t => t.id).Select(t => t.FirstOrDefault())
                    .OrderByDescending(t => t.lastdeliverytime).ToList().AsQueryable();
                ViewBag.ActiveStatus = 2;
            }
            else if (statusid == 0)
            {
                ticket = (from tt in db.Ticket
                          join ticketi in db.TicketItem on tt.id equals ticketi.ticketid
                          where tt.projectid == id
                          select tt
                    ).Include(t => t.FlagStatus).Include(t => t.StatusUpdatedByUser).Include(tt => tt.TicketType)
                    .Include(t => t.ConversationStatus).GroupBy(t => t.id).Select(t => t.FirstOrDefault())
                    .OrderByDescending(t => t.lastdeliverytime).ToList().AsQueryable();
                ViewBag.ActiveStatus = 0;
            }
            else
            {
                ticket = (from tt in db.Ticket
                          join ticketi in db.TicketItem on tt.id equals ticketi.ticketid
                          where tt.projectid == id && tt.statusid == statusid
                          select tt
                    ).Include(t => t.FlagStatus).Include(t => t.StatusUpdatedByUser).Include(tt => tt.TicketType)
                    .Include(t => t.ConversationStatus).GroupBy(t => t.id).Select(t => t.FirstOrDefault())
                    .OrderByDescending(t => t.lastdeliverytime).ToList().AsQueryable();
                ViewBag.ActiveStatus = statusid;
            }

            List<TicketItem> ti = new List<TicketItem>();
            foreach (Ticket items in ticket)
            {
                TicketItem ticketitem = db.TicketItem.Where(t => t.ticketid == items.id).OrderByDescending(t => t.createdonutc)
                    .FirstOrDefault();
                ti.Add(ticketitem);
            }

            #endregion get tickets

            string userid = User.Identity.GetUserId();
            ProDashboardViewModel pdvm = new ProDashboardViewModel();
            Project project = db.Project.Include(i => i.SubProjects).Where(i => i.id == id).AsQueryable().FirstOrDefault();
            IQueryable<TicketUserFlagged> flag = db.TicketUserFlagged.Where(f => f.isactive && f.userid == userid).ToList().AsQueryable();
            IQueryable<Team> ActiveTeams = db.Team.OrderBy(f => f.name).Where(f => f.isactive == true).ToList().AsQueryable();
            IQueryable<ProjectMembers> membersworkedon = (from user in db.Users
                                                          join time in db.TicketTimeLog on user.Id equals time.teamuserid
                                                          where time.projectid == id
                                                          select new ProjectMembers { username = user.FirstName + " " + user.LastName, active = user.isactive }
                ).Distinct().ToList().AsQueryable();

            #region sidebar menue creation

            List<Sidebarstatus> sidebarstatuses = new List<Sidebarstatus>();
            List<Sidebarstatus> orderstatus = new List<Sidebarstatus>();
            List<ConversationStatus> statuses = db.ConversationStatus.Where(x => x.isactive).AsQueryable().ToList();
            int allcount = 0;
            foreach (ConversationStatus status in statuses)
            {
                Sidebarstatus sidebarstatus = new Sidebarstatus();
                int count = db.Ticket.Where(x => x.projectid == id && x.statusid == status.id).Count();
                sidebarstatus.id = status.id;
                sidebarstatus.name = status.name;
                sidebarstatus.count = count;
                sidebarstatuses.Add(sidebarstatus);
                allcount += count;
            }

            Sidebarstatus all = new Sidebarstatus
            {
                id = 0,
                name = "All",
                count = allcount
            };
            sidebarstatuses.Add(all);

            orderstatus.Add(sidebarstatuses.SingleOrDefault(x => x.name.Equals("All")));
            orderstatus.Add(sidebarstatuses.SingleOrDefault(x => x.name.Equals("New Task")));
            orderstatus.Add(sidebarstatuses.SingleOrDefault(x => x.name.Equals("Assigned")));
            orderstatus.Add(sidebarstatuses.SingleOrDefault(x => x.name.Equals("In Progress")));
            orderstatus.Add(sidebarstatuses.SingleOrDefault(x => x.name.Equals("On Hold")));
            orderstatus.Add(sidebarstatuses.SingleOrDefault(x => x.name.Equals("In Review")));
            orderstatus.Add(sidebarstatuses.SingleOrDefault(x => x.name.Equals("QC")));
            orderstatus.Add(sidebarstatuses.SingleOrDefault(x => x.name.Equals("Done")));
            orderstatus.Add(sidebarstatuses.SingleOrDefault(x => x.name.Equals("Trash")));
            //sidebarstatuses = sidebarstatuses.OrderBy(x => x.id).ToList();

            #endregion sidebar menue creation

            TicketViewModel ticketView = new TicketViewModel
            {
                tickets = ticket,
                ticketitems = ti,
                teams = ActiveTeams,
                flaggeditems = flag
            };
            pdvm.sidebarstatus = orderstatus;
            pdvm.ticketViewModel = ticketView;
            pdvm.projectMembers = membersworkedon;
            pdvm.project = project;
            return View(pdvm);
        }

        public ActionResult Credentials(long id)
        {
            ProDashboardViewModel pdvm = new ProDashboardViewModel();
            string userid = User.Identity.GetUserId();
            ApplicationUser user = db.Users.Where(ui => ui.Id == userid).FirstOrDefault();
            //var credentials = db.Credentials.Include(c => c.CredentialCategory).Include(c => c.CredentialLevel).Include(c => c.CredentialType).Include(c => c.Project).Where(pi => pi.projectid==id).Where(cl => cl.credentiallevelid<=user.Levelid).ToList();
            List<Credentials> credentials = db.Credentials.Include(c => c.CredentialCategory).Include(c => c.CredentialLevel)
                .Include(c => c.CredentialType).Include(c => c.Project).Where(pi => pi.projectid == id).Distinct()
                .OrderBy(t => t.title).ThenBy(t => t.credentiallevelid).ThenBy(t => t.credentialcategoryid).ToList();

            Project project = db.Project.Include(i => i.SubProjects).Where(i => i.id == id).FirstOrDefault();

            IQueryable<ProjectMembers> membersworkedon = (from users in db.Users
                                                          join time in db.TicketTimeLog on users.Id equals time.teamuserid
                                                          where time.projectid == id
                                                          select new ProjectMembers { username = user.FirstName + " " + user.LastName, active = user.isactive }
                ).Distinct().ToList().AsQueryable();

            #region sidebar menue creation

            List<Sidebarstatus> sidebarstatuses = new List<Sidebarstatus>();
            List<Sidebarstatus> orderstatus = new List<Sidebarstatus>();
            List<ConversationStatus> statuses = db.ConversationStatus.Where(x => x.isactive).ToList();
            int allcount = 0;
            foreach (ConversationStatus status in statuses)
            {
                Sidebarstatus sidebarstatus = new Sidebarstatus();
                int count = db.Ticket.Where(x => x.projectid == id && x.statusid == status.id).Count();
                sidebarstatus.id = status.id;
                sidebarstatus.name = status.name;
                sidebarstatus.count = count;
                sidebarstatuses.Add(sidebarstatus);
                allcount += count;
            }

            Sidebarstatus all = new Sidebarstatus
            {
                id = 0,
                name = "All",
                count = allcount
            };
            sidebarstatuses.Add(all);

            orderstatus.Add(sidebarstatuses.SingleOrDefault(x => x.name.Equals("All")));
            orderstatus.Add(sidebarstatuses.SingleOrDefault(x => x.name.Equals("New Task")));
            orderstatus.Add(sidebarstatuses.SingleOrDefault(x => x.name.Equals("Assigned")));
            orderstatus.Add(sidebarstatuses.SingleOrDefault(x => x.name.Equals("In Progress")));
            orderstatus.Add(sidebarstatuses.SingleOrDefault(x => x.name.Equals("On Hold")));
            orderstatus.Add(sidebarstatuses.SingleOrDefault(x => x.name.Equals("In Review")));
            orderstatus.Add(sidebarstatuses.SingleOrDefault(x => x.name.Equals("QC")));
            orderstatus.Add(sidebarstatuses.SingleOrDefault(x => x.name.Equals("Done")));
            orderstatus.Add(sidebarstatuses.SingleOrDefault(x => x.name.Equals("Trash")));
            //sidebarstatuses = sidebarstatuses.OrderBy(x => x.id).ToList();

            #endregion sidebar menue creation

            pdvm.sidebarstatus = orderstatus;
            pdvm.credentials = credentials;
            pdvm.projectMembers = membersworkedon;
            pdvm.project = project;
            ViewBag.isclient = false;
            return View(pdvm);
        }

        public ActionResult Notes(long id)
        {
            ProDashboardViewModel pdvm = new ProDashboardViewModel();
            Project project = db.Project.Include(i => i.SubProjects).Where(i => i.id == id).FirstOrDefault();
            IQueryable<ProjectMembers> membersworkedon = (from user in db.Users
                                                          join time in db.TicketTimeLog on user.Id equals time.teamuserid
                                                          where time.projectid == id
                                                          select new ProjectMembers { username = user.FirstName + " " + user.LastName, active = user.isactive }
                ).Distinct().ToList().AsQueryable();
            List<ProjectNotes> notes = db.ProjectNotes.Where(i => i.projectid == id).Include(u => u.addedbyuser).ToList();

            #region sidebar menue creation

            List<Sidebarstatus> sidebarstatuses = new List<Sidebarstatus>();
            List<Sidebarstatus> orderstatus = new List<Sidebarstatus>();
            List<ConversationStatus> statuses = db.ConversationStatus.Where(x => x.isactive).ToList();
            int allcount = 0;
            foreach (ConversationStatus status in statuses)
            {
                Sidebarstatus sidebarstatus = new Sidebarstatus();
                int count = db.Ticket.Where(x => x.projectid == id && x.statusid == status.id).Count();
                sidebarstatus.id = status.id;
                sidebarstatus.name = status.name;
                sidebarstatus.count = count;
                sidebarstatuses.Add(sidebarstatus);
                allcount += count;
            }

            Sidebarstatus all = new Sidebarstatus
            {
                id = 0,
                name = "All",
                count = allcount
            };
            sidebarstatuses.Add(all);

            orderstatus.Add(sidebarstatuses.SingleOrDefault(x => x.name.Equals("All")));
            orderstatus.Add(sidebarstatuses.SingleOrDefault(x => x.name.Equals("New Task")));
            orderstatus.Add(sidebarstatuses.SingleOrDefault(x => x.name.Equals("Assigned")));
            orderstatus.Add(sidebarstatuses.SingleOrDefault(x => x.name.Equals("In Progress")));
            orderstatus.Add(sidebarstatuses.SingleOrDefault(x => x.name.Equals("On Hold")));
            orderstatus.Add(sidebarstatuses.SingleOrDefault(x => x.name.Equals("In Review")));
            orderstatus.Add(sidebarstatuses.SingleOrDefault(x => x.name.Equals("QC")));
            orderstatus.Add(sidebarstatuses.SingleOrDefault(x => x.name.Equals("Done")));
            orderstatus.Add(sidebarstatuses.SingleOrDefault(x => x.name.Equals("Trash")));
            //sidebarstatuses = sidebarstatuses.OrderBy(x => x.id).ToList();

            #endregion sidebar menue creation

            pdvm.sidebarstatus = orderstatus;
            pdvm.notes = notes;
            pdvm.projectMembers = membersworkedon;
            pdvm.project = project;
            ViewBag.currentuser = User.Identity.GetUserId();
            return View(pdvm);
        }

        public ActionResult Files(long id)
        {
            ProDashboardViewModel pdvm = new ProDashboardViewModel();
            Project project = db.Project.Include(i => i.SubProjects).Where(i => i.id == id).FirstOrDefault();
            IQueryable<ProjectMembers> membersworkedon = (from user in db.Users
                                                          join time in db.TicketTimeLog on user.Id equals time.teamuserid
                                                          where time.projectid == id
                                                          select new ProjectMembers { username = user.FirstName + " " + user.LastName, active = user.isactive }
                ).Distinct().ToList().AsQueryable();
            List<ProjectFiles> files = db.ProjectFiles.Where(pf => pf.projectid == id).ToList();

            #region sidebar menue creation

            List<Sidebarstatus> sidebarstatuses = new List<Sidebarstatus>();
            List<Sidebarstatus> orderstatus = new List<Sidebarstatus>();
            List<ConversationStatus> statuses = db.ConversationStatus.Where(x => x.isactive).ToList();
            int allcount = 0;
            foreach (ConversationStatus status in statuses)
            {
                Sidebarstatus sidebarstatus = new Sidebarstatus();
                int count = db.Ticket.Where(x => x.projectid == id && x.statusid == status.id).Count();
                sidebarstatus.id = status.id;
                sidebarstatus.name = status.name;
                sidebarstatus.count = count;
                sidebarstatuses.Add(sidebarstatus);
                allcount += count;
            }

            Sidebarstatus all = new Sidebarstatus
            {
                id = 0,
                name = "All",
                count = allcount
            };
            sidebarstatuses.Add(all);

            orderstatus.Add(sidebarstatuses.SingleOrDefault(x => x.name.Equals("All")));
            orderstatus.Add(sidebarstatuses.SingleOrDefault(x => x.name.Equals("New Task")));
            orderstatus.Add(sidebarstatuses.SingleOrDefault(x => x.name.Equals("Assigned")));
            orderstatus.Add(sidebarstatuses.SingleOrDefault(x => x.name.Equals("In Progress")));
            orderstatus.Add(sidebarstatuses.SingleOrDefault(x => x.name.Equals("On Hold")));
            orderstatus.Add(sidebarstatuses.SingleOrDefault(x => x.name.Equals("In Review")));
            orderstatus.Add(sidebarstatuses.SingleOrDefault(x => x.name.Equals("QC")));
            orderstatus.Add(sidebarstatuses.SingleOrDefault(x => x.name.Equals("Done")));
            orderstatus.Add(sidebarstatuses.SingleOrDefault(x => x.name.Equals("Trash")));
            //sidebarstatuses = sidebarstatuses.OrderBy(x => x.id).ToList();

            #endregion sidebar menue creation

            pdvm.sidebarstatus = orderstatus;
            pdvm.files = files;
            pdvm.projectMembers = membersworkedon;
            pdvm.project = project;
            return View(pdvm);
        }

        public ActionResult GetNotes(long id)
        {
            try
            {
                ProjectNotes pn = db.ProjectNotes.Where(i => i.projectid == id).Include(u => u.addedbyuser)
                    .OrderByDescending(x => x.createdonutc).First();
                ProjectNotesViewModel model = new ProjectNotesViewModel
                {
                    id = pn.id,
                    FullName = pn.addedbyuser.FullName,
                    comments = pn.comments,
                    createdonutc = string.Format("{0:f}", pn.createdonutc)
                };

                dynamic data = JsonConvert.SerializeObject(model);
                return Json(model, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = true, response = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult DeleteFile(long id)
        {
            try
            {
                ProjectFiles pf = db.ProjectFiles.Find(id);
                db.ProjectFiles.Remove(pf);
                db.SaveChanges();
                return Json(new { error = false, response = "The selected file has been deleted successfully!" },
                    JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = true, response = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult TimeLog(long id)
        {
            ProDashboardViewModel pdvm = new ProDashboardViewModel();
            Project project = db.Project.Include(i => i.SubProjects).Where(i => i.id == id).FirstOrDefault();
            IQueryable<ProjectMembers> membersworkedon = (from user in db.Users
                                                          join time in db.TicketTimeLog on user.Id equals time.teamuserid
                                                          where time.projectid == id
                                                          select new ProjectMembers { username = user.FirstName + " " + user.LastName, active = user.isactive }
                ).Distinct().ToList().AsQueryable();
            List<ProjectTimelog> timelog = (from x in db.TicketTimeLog
                                            join u in db.Users on x.teamuserid equals u.Id
                                            where x.projectid == id
                                            select new ProjectTimelog
                                            {
                                                id = x.id,
                                                ticketitemid = x.ticketitemid,
                                                tickettitle = x.title,
                                                workdate = x.workdate,
                                                description = x.description,
                                                spent = x.timespentinminutes,
                                                billable = x.billabletimeinminutes,
                                                username = u.FirstName + " " + u.LastName
                                            }).ToList();

            #region sidebar menue creation

            List<Sidebarstatus> sidebarstatuses = new List<Sidebarstatus>();
            List<Sidebarstatus> orderstatus = new List<Sidebarstatus>();
            List<ConversationStatus> statuses = db.ConversationStatus.Where(x => x.isactive).ToList();
            int allcount = 0;
            foreach (ConversationStatus status in statuses)
            {
                Sidebarstatus sidebarstatus = new Sidebarstatus();
                int count = db.Ticket.Where(x => x.projectid == id && x.statusid == status.id).Count();
                sidebarstatus.id = status.id;
                sidebarstatus.name = status.name;
                sidebarstatus.count = count;
                sidebarstatuses.Add(sidebarstatus);
                allcount += count;
            }

            Sidebarstatus all = new Sidebarstatus
            {
                id = 0,
                name = "All",
                count = allcount
            };
            sidebarstatuses.Add(all);

            orderstatus.Add(sidebarstatuses.SingleOrDefault(x => x.name.Equals("All")));
            orderstatus.Add(sidebarstatuses.SingleOrDefault(x => x.name.Equals("New Task")));
            orderstatus.Add(sidebarstatuses.SingleOrDefault(x => x.name.Equals("Assigned")));
            orderstatus.Add(sidebarstatuses.SingleOrDefault(x => x.name.Equals("In Progress")));
            orderstatus.Add(sidebarstatuses.SingleOrDefault(x => x.name.Equals("On Hold")));
            orderstatus.Add(sidebarstatuses.SingleOrDefault(x => x.name.Equals("In Review")));
            orderstatus.Add(sidebarstatuses.SingleOrDefault(x => x.name.Equals("QC")));
            orderstatus.Add(sidebarstatuses.SingleOrDefault(x => x.name.Equals("Done")));
            orderstatus.Add(sidebarstatuses.SingleOrDefault(x => x.name.Equals("Trash")));
            //sidebarstatuses = sidebarstatuses.OrderBy(x => x.id).ToList();

            #endregion sidebar menue creation

            //var stime2 = (double)(timelog.Sum(t => t.spent)) / 60;
            //stime2 = Math.Round(stime2, 2);
            ViewBag.totalspent = CommonFunctions.RoundTwoDecimalPlaces((double)timelog.Sum(t => t.spent) / 60);
            //var btime2 = (double)(timelog.Sum(t => t.billable)) / 60;
            //btime2 = Math.Round(btime2, 2);
            ViewBag.totalbillable = CommonFunctions.RoundTwoDecimalPlaces((double)timelog.Sum(t => t.billable) / 60);
            pdvm.sidebarstatus = orderstatus;
            pdvm.ProjectTimelog = timelog;
            pdvm.projectMembers = membersworkedon;
            pdvm.project = project;
            return View(pdvm);
        }

        [HttpPost]
        public ActionResult UploadFile(long id)
        {
            HttpPostedFileBase file = Request.Files[0];
            if (file != null && file.ContentLength != 0)
            {
                if (file.ContentType != "image/jpeg" && file.ContentType != "image/png" &&
                    file.ContentType != "image/jpg"
                    && !file.FileName.EndsWith(".xls")
                    && !file.FileName.EndsWith(".xlt") && !file.FileName.EndsWith(".xlm") &&
                    !file.FileName.EndsWith(".xlsx") && !file.FileName.EndsWith(".xlsm") &&
                    !file.FileName.EndsWith(".xltx")
                    && !file.FileName.EndsWith(".xltm") && !file.FileName.EndsWith(".xlsb") &&
                    !file.FileName.EndsWith(".xla") && !file.FileName.EndsWith(".xlam") &&
                    !file.FileName.EndsWith(".xll") && !file.FileName.EndsWith(".xlw")
                    && !file.FileName.EndsWith(".ppt") && !file.FileName.EndsWith(".pot") &&
                    !file.FileName.EndsWith(".pps") && !file.FileName.EndsWith(".pptx") &&
                    !file.FileName.EndsWith(".pptm") && !file.FileName.EndsWith(".potx")
                    && !file.FileName.EndsWith(".potm") && !file.FileName.EndsWith(".ppam") &&
                    !file.FileName.EndsWith(".ppsx") && !file.FileName.EndsWith(".ppsm") &&
                    !file.FileName.EndsWith(".sldx") && !file.FileName.EndsWith(".sldm")
                    && !file.FileName.EndsWith(".doc") && !file.FileName.EndsWith(".dot") &&
                    !file.FileName.EndsWith(".wbk") && !file.FileName.EndsWith(".docx") &&
                    !file.FileName.EndsWith(".docm") && !file.FileName.EndsWith(".dotm")
                    && !file.FileName.EndsWith(".docb") && !file.FileName.EndsWith(".pdf") &&
                    !file.FileName.EndsWith(".rar") && !file.FileName.EndsWith(".zip") &&
                    !file.FileName.EndsWith(".txt") && !file.FileName.EndsWith(".csv")
                   )
                {
                    return Json(new
                    {
                        error = true,
                        errortext =
                            "Sorry, the posted file is not in valid fomrat. only word, excel, power point, pdf, text, zip, rar, csv and image files are allowed."
                    });
                }
            }
            else
            {
                return Json(new { error = true, errortext = "Sorry, please select a file." });
            }

            if (file != null && file.ContentLength != 0)
            {
                string fileExt = Path.GetExtension(file.FileName);
                string fileName = Path.GetFileNameWithoutExtension(file.FileName);
                fileName += DateTime.Now.ToString("yyyyMMddHHmmssfff") + fileExt;
                string path = Path.Combine(Server.MapPath(ConfigurationManager.AppSettings["projectfilepath"]), fileName);
                file.SaveAs(path);
                ProjectFiles pf = new ProjectFiles
                {
                    filename = fileName,
                    projectid = id,
                    createdonutc = DateTime.Now,
                    updatedonutc = DateTime.Now,
                    ipused = Request.UserHostAddress,
                    userid = User.Identity.GetUserId()
                };
                db.ProjectFiles.Add(pf);
                db.SaveChanges();
                return Json(new { success = true, successtext = "File has been uploaded succesfully." });
            }

            return Json(new { error = true, errortext = "Sorry, please select a file." });
        }

        [HttpGet]
        public ActionResult AddProjectNotes(long id, string text)
        {
            try
            {
                ProjectNotes pn = new ProjectNotes
                {
                    projectid = id,
                    comments = text,
                    addedbyuserid = User.Identity.GetUserId(),
                    userid = User.Identity.GetUserId(),
                    createdonutc = DateTime.Now,
                    updatedonutc = DateTime.Now,
                    ipused = Request.UserHostAddress
                };
                db.ProjectNotes.Add(pn);
                db.SaveChanges();
                return Json(new { error = false, response = "Note added succesfully!" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = true, response = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult DeleteProjectNotes(long id)
        {
            try
            {
                ProjectNotes projectnotes = db.ProjectNotes.Find(id);
                db.ProjectNotes.Remove(projectnotes);
                db.SaveChanges();
                return Json(new { error = false, response = "The selected note has been deleted successfully!" },
                    JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = true, response = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult EditProjectNotes(long id, string comment)
        {
            try
            {
                ProjectNotes projectnotes = db.ProjectNotes.Find(id);
                projectnotes.comments = comment;
                db.Entry(projectnotes).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { error = false, response = "The selected note has been edited successfully!" },
                    JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = true, response = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult ProjectCredential(long id)
        {
            List<Credentials> credentials = db.Credentials.Include(c => c.CredentialCategory).Include(c => c.CredentialLevel)
                .Include(c => c.CredentialType).Include(c => c.Project).Where(pi => pi.projectid == id).Distinct()
                .OrderBy(t => t.title).ThenBy(t => t.credentiallevelid).ThenBy(t => t.credentialcategoryid).ToList();
            string credentiallist = PartialView("_CredentialPartial", credentials).RenderToString();
            return Json(new { Credential = credentiallist }, JsonRequestBehavior.AllowGet);
        }
    }
}