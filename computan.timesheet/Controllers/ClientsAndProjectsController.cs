using computan.timesheet.Extensions;
using computan.timesheet.Helpers;
using computan.timesheet.Models;
using Microsoft.AspNet.Identity.Owin;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace computan.timesheet.Controllers
{
    [CustomeAuthorizeAttribute]
    public class ClientsAndProjectsController : BaseController
    {
        private ApplicationUserManager _userManager;

        // GET: ClientsAndProjects
        public ClientsAndProjectsController()
        {
        }

        public ClientsAndProjectsController(ApplicationUserManager userManager)
        {
            UserManager = userManager;
        }

        public ApplicationUserManager UserManager
        {
            get => _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            private set => _userManager = value;
        }

        // GET: Clients
        public ActionResult Index()
        {
            ViewBag.countrylist = new SelectList(db.Country, "id", "nicename");
            ViewBag.statelist = new SelectList(db.State, "id", "name");
            System.Collections.Generic.List<core.Client> Clients = db.Client.Include(i => i.SubClients).Where(p => p.parentid == null && p.isactive == true)
                .OrderBy(n => n.name).ToList();
            return View(Clients);
        }

        public ActionResult Clients()
        {
            ViewBag.countrylist = new SelectList(db.Country, "id", "nicename");
            ViewBag.statelist = new SelectList(db.State, "id", "name");
            System.Collections.Generic.List<core.Client> Clients = db.Client.Include(i => i.SubClients).Where(p => p.parentid == null && p.isactive == true)
                .OrderBy(n => n.name).ToList();
            return PartialView("_clients", Clients);
        }

        public ActionResult SubClients(long id)
        {
            ViewBag.countrylist = new SelectList(db.Country, "id", "nicename");
            ViewBag.statelist = new SelectList(db.State, "id", "name");
            System.Collections.Generic.List<core.Client> Clients = db.Client.Include(i => i.SubClients).Where(p => p.parentid != null && p.parentid == id)
                .OrderBy(n => n.name).ToList();
            System.Collections.Generic.List<core.Project> projects = db.Project.Include(i => i.SubProjects).Where(p => p.parentid == null && p.clientid == id)
                .OrderBy(n => n.name).ToList();
            string clientlist = PartialView("_clients", Clients).RenderToString();
            string projectlist = PartialView("_Projects", projects).RenderToString();
            return Json(new { ResponseType = 1, clientlist, projectlist }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Projects(long id)
        {
            ViewBag.countrylist = new SelectList(db.Country, "id", "nicename");
            ViewBag.statelist = new SelectList(db.State, "id", "name");
            System.Collections.Generic.List<core.Project> projects = db.Project.Include(i => i.SubProjects).Where(p => p.parentid == null && p.clientid == id)
                .OrderBy(n => n.name).ToList();
            return PartialView("_Projects", projects);
        }

        public ActionResult SubProjects(long id)
        {
            ViewBag.countrylist = new SelectList(db.Country, "id", "nicename");
            ViewBag.statelist = new SelectList(db.State, "id", "name");
            System.Collections.Generic.List<core.Project> projects = db.Project.Include(i => i.SubProjects).Where(p => p.parentid != null && p.parentid == id)
                .OrderBy(n => n.name).ToList();
            return PartialView("_Projects", projects);
        }

        //Load task by project
        public ActionResult LoadTasks(long id)
        {
            System.Collections.Generic.List<core.TicketItem> ticketItem = db.TicketItem.Include(t => t.TicketItemLog)
                .Where(t => t.statusid == 2 && t.projectid == id).ToList();
            //var ticketItem = (from task in db.TicketItem.Include(t => t.Ticket).Include(t => t.TicketStatus).Include(t => t.Project).Include(t => t.Skill).Include(t => t.TicketItemLog)
            //                  join tl in db.TicketItemLog.Include(u => u.user) on task.id equals tl.ticketitemid
            //                  where tl.statusid == 2 && task.projectid==id
            //                  orderby tl.assignedon descending
            //                  select task).ToList();
            //ViewBag.users =UserManager.Users.Where(i => i.Id != "07f86888-069b-4c83-a4c2-8994ed4b6933" && i.Id != "e99fec1c-541e-42e4-bc1f-0f36478dd37a" && i.Id != "353ae07d-e937-4c22-879b-b9ff6b9a86b0" && i.Id != "53998caf-7860-48d3-b17c-e0bfba7ec15a" && i.Id != "d22fb43f-f95d-4e26-a895-723059b5524d" && i.Id != "5a4628da-67b5-4e9b-b1c5-9d53686ffb0b" && i.Id != "ae2a7c3d-fd6a-4ecb-a1c5-cf0e53c8ec3c" && i.Id != "03b91e3c-7e60-42ff-998c-bd2b18254731" && i.Id != "09c17fcb-4a63-4f15-b4fe-46eb25ebc381" && i.Id != "1792f740-7bd7-42cc-846f-4326fa2318f4").ToList();
            return PartialView("_LoadTasks", ticketItem);
        }
    }
}