using computan.timesheet.Contexts;
using computan.timesheet.core;
using computan.timesheet.Helpers;
using computan.timesheet.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace computan.timesheet.Controllers
{
    [CustomeAuthorizeAttribute]
    public class ClientManagerController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        // GET: ClientManager
        public ActionResult Index()
        {
            DateTime datetime = DateTime.Now.AddMonths(-2);
            //List<Client> Clients = db.Client.Include(i => i.ProjectCollection).Include(i => i.SubClients).Include(i=> i.SubClients.Select(sc=> sc.ProjectCollection)).Where(p => p.parentid == null).OrderBy(n => n.name).ToList();
            List<ActiveClientsViewModel> acvm = (from c in db.Client.Include(i => i.ProjectCollection).Include(i => i.SubClients)
                        .Include(i => i.SubClients.Select(sc => sc.ProjectCollection)).Where(p => p.parentid == null)
                        .OrderBy(n => n.name).Distinct()
                                                 join p in db.Project
                                                     on c.id equals p.clientid
                                                 join t in db.TicketTimeLog
                                                     on p.id equals t.projectid
                                                 where t.workdate >= datetime && t.workdate <= DateTime.Now && t.billabletimeinminutes != null &&
                                                       t.billabletimeinminutes != 0
                                                 orderby t.workdate descending
                                                 select new ActiveClientsViewModel { client = c, workdate = t.workdate }
                ).Distinct().ToList();
            List<Client> clients = new List<Client>();
            if (acvm != null && acvm.Count > 0)
            {
                foreach (ActiveClientsViewModel item in acvm.OrderByDescending(t => t.workdate))
                {
                    clients.Add(item.client);
                }
            }

            ClientManagerViewModel clientVM = new ClientManagerViewModel
            {
                Clients = clients.Distinct().ToList()
            };
            ViewBag.isall = false;
            List<SelectListItem> projectlist = db.Database.SqlQuery<CombinedEntity>("exec projects_loadcombined").Select(x =>
                new SelectListItem
                {
                    Text = x.name,
                    Value = x.id.ToString()
                }).ToList();
            ViewBag.projects = new SelectList(projectlist, "Value", "Text");
            ViewBag.skills = db.Skill.ToList();
            ViewBag.users = db.Users.OrderBy(f => f.FirstName).Where(u => u.isactive == true).ToList();
            return View(clientVM);
        }

        public ActionResult All()
        {
            List<Client> Clients = db.Client.Include(i => i.ProjectCollection).Include(i => i.SubClients)
                .Include(i => i.SubClients.Select(sc => sc.ProjectCollection)).Where(p => p.parentid == null)
                .OrderBy(n => n.name).ToList();

            ClientManagerViewModel clientVM = new ClientManagerViewModel
            {
                Clients = Clients
            };
            ViewBag.isall = true;
            List<SelectListItem> projectlist = db.Database.SqlQuery<CombinedEntity>("exec projects_loadcombined").Select(x =>
                new SelectListItem
                {
                    Text = x.name,
                    Value = x.id.ToString()
                }).ToList();
            ViewBag.projects = new SelectList(projectlist, "Value", "Text");
            ViewBag.skills = db.Skill.ToList();
            ViewBag.users = db.Users.OrderBy(f => f.FirstName).Where(u => u.isactive == true).ToList();
            return View("index", clientVM);
        }

        public ActionResult ClientCredentials(long id)
        {
            string userid = User.Identity.GetUserId();
            ApplicationUser user = db.Users.Where(ui => ui.Id == userid).FirstOrDefault();
            Client client = db.Client.Find(id);
            List<Project> projectlist = new List<Project>();
            if (client != null)
            {
                projectlist = db.Project.Where(p => p.clientid == client.id).ToList();
            }

            List<Credentials> credentials = new List<Credentials>();
            if (projectlist != null && projectlist.Count > 0)
            {
                foreach (Project item in projectlist)
                {
                    List<Credentials> cred = db.Credentials.Include(c => c.CredentialCategory).Include(c => c.CredentialLevel)
                        .Include(c => c.CredentialType).Include(c => c.Project).Where(pi => pi.projectid == item.id)
                        .Distinct().OrderBy(t => t.title)
                        .ToList(); //.ThenBy(t => t.credentiallevelid).ThenBy(t => t.credentialcategoryid)
                    credentials.AddRange(cred);
                }
            }
            //var credentials = db.Credentials.Include(c => c.CredentialCategory).Include(c => c.CredentialLevel).Include(c => c.CredentialType).Include(c => c.Project).Where(pi => pi.projectid==id).Where(cl => cl.credentiallevelid<=user.Levelid).ToList();

            ViewBag.IsProjectCredentials = true;
            ViewBag.projectid = id;
            ViewBag.credentialcategoryid = new SelectList(db.CredentialCategories, "id", "name");
            //ViewBag.credentiallevelid = new SelectList(db.CredentialLevels, "id", "name");
            ViewBag.credentiallevelid =
                new SelectList(db.Database.SqlQuery<CombinedEntity>("exec GetCredentialsLevel_sp").ToList(), "id",
                    "name");
            ViewBag.crendentialtypeid = new SelectList(db.CredentialTypes, "id", "name");
            ViewBag.Skills = db.Skill.ToList();
            ViewBag.projectid = new SelectList(db.Project, "id", "name");
            ViewBag.isclient = true;
            return PartialView("~/views/projectdashboard/_ProjectCredentials.cshtml", credentials);
        }
    }
}