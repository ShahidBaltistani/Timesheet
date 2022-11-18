using computan.timesheet.Contexts;
using computan.timesheet.core;
using computan.timesheet.Helpers;
using computan.timesheet.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace computan.timesheet.Controllers
{
    [CustomeAuthorizeAttribute]
    public class ProjectDashboardController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        // GET: ProjectDashboard
        public ActionResult Index(long id)
        {
            Project project = db.Project.Include(i => i.SubProjects).Where(i => i.id == id).FirstOrDefault();
            List<AllTaskViewModel> Project_tickets = (from ticket in db.Ticket
                                                      join ticketitem in db.TicketItem on ticket.id equals ticketitem.ticketid
                                                      join teamlog in db.TicketTeamLogs on ticket.id equals teamlog.ticketid
                                                      join team in db.Team on teamlog.teamid equals team.id
                                                      join itemlog in db.TicketItemLog on ticketitem.id equals itemlog.ticketitemid
                                                      where ticket.projectid == id && ticket.statusid == 2
                                                      select new AllTaskViewModel
                                                      {
                                                          FirstName = itemlog.user.FirstName,
                                                          LastName = itemlog.user.LastName,
                                                          assignedon = itemlog.assignedon,
                                                          id = ticketitem.id,
                                                          subject = ticketitem.subject,
                                                          uniquebody = ticketitem.uniquebody,
                                                          team = team.name,
                                                          status = itemlog.TicketStatus.name
                                                      }).OrderByDescending(a => a.assignedon).ToList();

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

            #endregion

            ProjectDashBoardViewModel pdvm = new ProjectDashBoardViewModel();
            List<string> membersworkedon = (from user in db.Users
                                            join time in db.TicketTimeLog on user.Id equals time.teamuserid
                                            where time.projectid == id
                                            select user.FirstName + " " + user.LastName
                ).Distinct().ToList();

            pdvm.sidebarstatus = orderstatus;
            pdvm.project = project;
            pdvm.tasks = Project_tickets;
            pdvm.usernames = membersworkedon;
            return View(pdvm);
        }

        public ActionResult Get_Task(long? id, int? statusid)
        {
            List<AllTaskViewModel> atvm = new List<AllTaskViewModel>();
            if (statusid == 0)
            {
                List<AllTaskViewModel> log = (from ticket in db.Ticket
                                              join ticketitem in db.TicketItem on ticket.id equals ticketitem.ticketid
                                              join teamlog in db.TicketTeamLogs on ticket.id equals teamlog.ticketid
                                              join team in db.Team on teamlog.teamid equals team.id
                                              join itemlog in db.TicketItemLog on ticketitem.id equals itemlog.ticketitemid
                                              where ticket.projectid == id
                                              select new AllTaskViewModel
                                              {
                                                  FirstName = itemlog.user.FirstName,
                                                  LastName = itemlog.user.LastName,
                                                  assignedon = itemlog.assignedon,
                                                  id = ticketitem.id,
                                                  subject = ticketitem.subject,
                                                  uniquebody = ticketitem.uniquebody,
                                                  team = team.name,
                                                  status = itemlog.TicketStatus.name
                                              }).OrderByDescending(a => a.assignedon).ToList();
                atvm = log;
            }
            else
            {
                List<AllTaskViewModel> log = (from ticket in db.Ticket
                                              join ticketitem in db.TicketItem on ticket.id equals ticketitem.ticketid
                                              join teamlog in db.TicketTeamLogs on ticket.id equals teamlog.ticketid
                                              join team in db.Team on teamlog.teamid equals team.id
                                              join itemlog in db.TicketItemLog on ticketitem.id equals itemlog.ticketitemid
                                              where ticket.projectid == id && ticket.statusid == statusid
                                              select new AllTaskViewModel
                                              {
                                                  FirstName = itemlog.user.FirstName,
                                                  LastName = itemlog.user.LastName,
                                                  assignedon = itemlog.assignedon,
                                                  id = ticketitem.id,
                                                  subject = ticketitem.subject,
                                                  uniquebody = ticketitem.uniquebody,
                                                  team = team.name,
                                                  status = itemlog.TicketStatus.name
                                              }).OrderByDescending(a => a.assignedon).ToList();
                atvm = log;
            }

            return PartialView("_Tasks", atvm);
        }

        public ActionResult get_projectfiles(long? id)
        {
            List<ProjectFiles> files = db.ProjectFiles.Where(pf => pf.projectid == id.Value).ToList();
            return PartialView("_ProjectFiles", files);
        }

        public ActionResult get_tasklist(long? id)
        {
            Project project = db.Project.Include(i => i.SubProjects).Where(i => i.id == id).FirstOrDefault();
            List<AllTaskViewModel> log = (from ticket in db.Ticket
                                          join ticketitem in db.TicketItem on ticket.id equals ticketitem.ticketid
                                          join teamlog in db.TicketTeamLogs on ticket.id equals teamlog.ticketid
                                          join team in db.Team on teamlog.teamid equals team.id
                                          join itemlog in db.TicketItemLog on ticketitem.id equals itemlog.ticketitemid
                                          where ticket.projectid == id && ticket.statusid == 2
                                          select new AllTaskViewModel
                                          {
                                              FirstName = itemlog.user.FirstName,
                                              LastName = itemlog.user.LastName,
                                              assignedon = itemlog.assignedon,
                                              id = ticketitem.id,
                                              subject = ticketitem.subject,
                                              uniquebody = ticketitem.uniquebody,
                                              team = team.name,
                                              status = itemlog.TicketStatus.name
                                          }).OrderByDescending(a => a.assignedon).ToList();

            ProjectDashBoardViewModel pdvm = new ProjectDashBoardViewModel
            {
                project = project,
                tasks = log
            };
            return PartialView("_Tasklist", pdvm);
        }

        [HttpPost]
        public ActionResult UploadFile(long id)
        {
            System.Web.HttpPostedFileBase file = Request.Files[0];
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
                    createdonutc = DateTime.UtcNow,
                    updatedonutc = DateTime.UtcNow,
                    ipused = Request.UserHostAddress,
                    userid = User.Identity.GetUserId()
                };
                db.ProjectFiles.Add(pf);
                db.SaveChanges();
                return Json(new { success = true, successtext = "File has been uploaded succesfully." });
            }

            return Json(new { error = true, errortext = "Sorry, please select a file." });
        }

        public ActionResult ProjectCredentials(long id)
        {
            string userid = User.Identity.GetUserId();
            ApplicationUser user = db.Users.Where(ui => ui.Id == userid).FirstOrDefault();
            List<Credentials> credentials = db.Credentials.Include(c => c.CredentialCategory).Include(c => c.CredentialLevel)
                .Include(c => c.CredentialType).Include(c => c.Project).Where(pi => pi.projectid == id).Distinct()
                .OrderBy(t => t.title).ThenBy(t => t.credentiallevelid).ThenBy(t => t.credentialcategoryid).ToList();

            ProjectDashBoardViewModel pdvm = new ProjectDashBoardViewModel();
            List<string> membersworkedon = (from u in db.Users
                                            join time in db.TicketTimeLog on u.Id equals time.teamuserid
                                            where time.projectid == id
                                            select u.FirstName + " " + u.LastName
                ).Distinct().ToList();
            Project project = db.Project.Include(i => i.SubProjects).Where(i => i.id == id).FirstOrDefault();

            pdvm.credentials = credentials;
            pdvm.usernames = membersworkedon;
            pdvm.project = project;
            return View(pdvm);
        }

        public ActionResult PartialProjectCredentials(long id)
        {
            string userid = User.Identity.GetUserId();
            ApplicationUser user = db.Users.Where(ui => ui.Id == userid).FirstOrDefault();
            List<Credentials> credentials = db.Credentials.Include(c => c.CredentialCategory).Include(c => c.CredentialLevel)
                .Include(c => c.CredentialType).Include(c => c.Project).Where(pi => pi.projectid == id).Distinct()
                .OrderBy(t => t.title).ThenBy(t => t.credentiallevelid).ThenBy(t => t.credentialcategoryid).ToList();
            return PartialView("_ProjectCredentials", credentials);
        }

        public ActionResult ProjectNotes(long id)
        {
            List<ProjectNotes> notes = db.ProjectNotes.Where(i => i.projectid == id).Include(u => u.addedbyuser).ToList();
            ViewBag.projectid = id;
            ViewBag.currentuser = User.Identity.GetUserId();
            return PartialView("_Notes", notes);
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
                    createdonutc = DateTime.UtcNow,
                    updatedonutc = DateTime.UtcNow,
                    ipused = Request.UserHostAddress
                };
                db.ProjectNotes.Add(pn);
                db.SaveChanges();
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = true, erorrtext = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult DeleteProjectNotes(long id)
        {
            try
            {
                ProjectNotes projectnotes = db.ProjectNotes.Find(id);
                db.ProjectNotes.Remove(projectnotes);
                db.SaveChanges();
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = true, erorrtext = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}