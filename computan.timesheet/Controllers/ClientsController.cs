using computan.timesheet.core;
using computan.timesheet.core.common;
using computan.timesheet.Extensions;
using computan.timesheet.Helpers;
using computan.timesheet.Models;
using computan.timesheet.Models.ClientDashboard;
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
    [CustomeAuthorize]
    public class ClientsController : BaseController
    {
        private ApplicationUserManager _userManager;

        public ClientsController()
        {
        }

        public ClientsController(ApplicationUserManager userManager)
        {
            UserManager = userManager;
        }

        public ApplicationUserManager UserManager
        {
            get => _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            private set => _userManager = value;
        }

        public ActionResult Dashboard(long id, string type, string userId)
        {
            ViewBag.clientName = db.Client.Find(id).name;
            ViewBag.clientId = id;
            ViewBag.userId = userId ?? User.Identity.GetUserId();
            ViewBag.type = type == null ? ProjectCategory.ActiveProjects.ToString() : type;

            ViewBag.users = db.Database.SqlQuery<ClientDashboardDropdownVM>(
                    "exec GetUsersByClientId_sp @clientId, @userId",
                    new SqlParameter("clientId", id),
                    new SqlParameter("userId", userId ?? User.Identity.GetUserId())
                )
                .Select(x => new SelectListItem
                {
                    Text = x.Fullname,
                    Value = x.Id.ToString()
                }).ToList();
            return View();
        }

        public ActionResult GetSkills()
        {
            return Json(db.Skill.Where(s => s.isactive == true).ToList(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetProjects()
        {
            List<SelectListItem> projectlist = db.Database.SqlQuery<CombinedEntity>("exec projects_loadcombined").Select(x =>
                new SelectListItem
                {
                    Text = x.name,
                    Value = x.id.ToString()
                }).ToList();
            return Json(projectlist, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ClientDashboard(long id, string type, string userId)
        {
            ClientDashboardViewModel model = new ClientDashboardViewModel();
            Client client = db.Client.Find(id);
            userId = userId ?? User.Identity.GetUserId();
            if (client != null)
            {
                model.client_id = client.id;
                model.name = client.name;
                List<ProjectVM> projects = db.Database.SqlQuery<ProjectVM>(
                    "exec GetClientProjectsByUsers_sp @clientId, @userId, @type",
                    new SqlParameter("clientId", id),
                    new SqlParameter("userId", userId),
                    new SqlParameter("type",
                        User.IsInRole(Role.Admin.ToString()) ? type : ProjectCategory.MyProjects.ToString())
                ).ToList();
                List<ProjectViewModel> pvmlist = new List<ProjectViewModel>();
                foreach (ProjectVM project in projects)
                {
                    List<TicketsViewModel> ticketsList = new List<TicketsViewModel>();
                    List<StatusCount> statuscount = new List<StatusCount>();
                    ProjectViewModel projectmodel = new ProjectViewModel
                    {
                        project_id = project.id,
                        name = project.name
                    };
                    List<Ticket> tickets = new List<Ticket>();
                    if (type == ProjectCategory.MyProjects.ToString())
                    {
                        tickets = (from t in db.Ticket
                                   join ti in db.TicketItem on t.id equals ti.ticketid
                                   join til in db.TicketItemLog on ti.id equals til.ticketitemid
                                   where t.projectid == project.id && t.statusid != 3 && t.statusid != 8
                                         && til.assignedtousersid == userId
                                   select t).Distinct().Include(t => t.TicketItems.Select(ti => ti.TicketItemLog))
                            .Include(c => c.ConversationStatus).ToList();
                    }
                    else
                    {
                        tickets = db.Ticket.Where(t => t.projectid == project.id && t.statusid != 3 && t.statusid != 8)
                            .Include(t => t.TicketItems)
                            .Include(t => t.TicketItems.Select(ti => ti.TicketItemLog))
                            .Include(c => c.ConversationStatus).ToList();
                    }

                    statuscount.Add(new StatusCount(1, tickets.Where(t => t.statusid == 1).Count()));
                    statuscount.Add(new StatusCount(2, tickets.Where(t => t.statusid == 2).Count()));
                    statuscount.Add(new StatusCount(4, tickets.Where(t => t.statusid == 4).Count()));
                    statuscount.Add(new StatusCount(5, tickets.Where(t => t.statusid == 5).Count()));
                    statuscount.Add(new StatusCount(6, tickets.Where(t => t.statusid == 6).Count()));
                    statuscount.Add(new StatusCount(7, tickets.Where(t => t.statusid == 7).Count()));

                    projectmodel.tickets_count = tickets.Count();
                    foreach (Ticket ticket in tickets)
                    {
                        TicketsViewModel ticketmodel = new TicketsViewModel
                        {
                            ticket_id = ticket.id,
                            statusid = ticket.statusid,
                            sender = ticket.uniquesenders,
                            status = ticket.ConversationStatus?.name,
                            title = ticket.topic,
                            last_updated = ticket.lastmodifiedtime,
                            minibody = ticket.TicketItems.Select(x => x.minibody).FirstOrDefault(),
                            shortbody = ticket.TicketItems.Select(x => x.shortbody).FirstOrDefault(),
                            description = ticket.TicketItems.Select(x => x.dashboardbody).FirstOrDefault(),
                            uniqubody = ticket.TicketItems.Select(x => x.uniquebody).FirstOrDefault()
                        };
                        List<UsersViewModel> userslist = new List<UsersViewModel>();
                        if (ticket.TicketAssignedToCollection != null)
                        {
                            foreach (ApplicationUser item in ticket.TicketAssignedToCollection.Select(x => x.user).Distinct())
                            {
                                UsersViewModel usermodel = new UsersViewModel
                                {
                                    user_id = item.Id,
                                    user_name = item.FullName,
                                    short_name = item.GetInitials
                                };
                                userslist.Add(usermodel);
                            }
                        }

                        ticketmodel.assigned_user = userslist;
                        ticketsList.Add(ticketmodel);
                    }

                    projectmodel.tickets = ticketsList;
                    projectmodel.status_count = statuscount;
                    if (type == ProjectCategory.AllProjects.ToString())
                    {
                        pvmlist.Add(projectmodel);
                    }
                    else
                    {
                        if (projectmodel.tickets_count > 0)
                        {
                            pvmlist.Add(projectmodel);
                        }
                    }
                }

                model.projects = pvmlist;
            }

            List<SelectListItem> projectlist = db.Database.SqlQuery<CombinedEntity>("exec projects_loadcombined").Select(x =>
                new SelectListItem
                {
                    Text = x.name,
                    Value = x.id.ToString()
                }).ToList();
            return PartialView("_ClientDashboard", model);
        }

        public ActionResult PMToolReport()
        {
            List<Client> Clients = db.Client.Where(p => p.isactive == true).ToList();
            return View(Clients);
        }

        // GET: Clients
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GetData(bool? filter)
        {
            try
            {
                string draw = Request.Params.GetValues("draw")[0];
                int start = Convert.ToInt32(Request.Params.GetValues("start")[0]);
                int lenght = Convert.ToInt32(Request.Params.GetValues("length")[0]);
                string searchValue = Request["search[value]"];
                List<Client> clients = new List<Client>();
                switch (filter)
                {
                    case true:
                        clients = db.Client.Include(x => x.CustomerSource).Where(x => x.isactive == true).ToList();
                        break;
                    case false:
                        clients = db.Client.Include(x => x.CustomerSource).Where(x => x.isactive == false).ToList();
                        break;
                    default:
                        clients = db.Client.Include(x => x.CustomerSource).ToList();
                        break;
                }

                int TotalRecordsCount = clients.Count();
                if (!string.IsNullOrEmpty(searchValue))
                {
                    string[] searcharray = searchValue.Split(' ').ToArray();
                    clients = (from c in clients
                               where c.name != null && searcharray.Any(val => c.name.ToLower().Contains(val.ToLower()))
                                     || c.email != null && searcharray.Any(val => c.email.ToLower().Contains(val.ToLower()))
                                     || c.User != null &&
                                     searcharray.Any(val => c.User.FullName.ToLower().Contains(val.ToLower()))
                                     || c.website != null &&
                                     searcharray.Any(val => c.website.ToLower().Contains(val.ToLower()))
                                     || c.pmplateformlink != null && searcharray.Any(val =>
                                         c.pmplateformlink.ToLower().Contains(val.ToLower()))
                                     || c.CustomerSource.name != null && searcharray.Any(val =>
                                         c.CustomerSource.name.ToLower().Contains(val.ToLower()))
                               select c).ToList();
                }

                int FilteredRecordCount = clients.Count();
                if (lenght > 0)
                {
                    clients = clients.Skip(start).Take(lenght).ToList();
                }

                List<ClientsViewModels> listClients = new List<ClientsViewModels>();
                foreach (Client client in clients)
                {
                    ClientsViewModels cvm = new ClientsViewModels
                    {
                        id = client.id,
                        name = client.name,
                        email = client.email,
                        maxbillablehours = client.maxbillablehours,
                        website = client.website,
                        pmplateformlink = client.pmplateformlink,
                        CustomerName = client.CustomerSource.name,
                        User = client.User,
                        isactive = client.isactive
                    };
                    listClients.Add(cvm);
                }

                return Json(new
                {
                    draw = Convert.ToInt32(draw),
                    recordsTotal = TotalRecordsCount,
                    recordsFiltered = FilteredRecordCount,
                    data = listClients
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    data = ex.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Clients()
        {
            ViewBag.countrylist = new SelectList(db.Country, "id", "nicename");
            ViewBag.statelist = new SelectList(db.State, "id", "name");
            List<Client> Clients = db.Client.Include(i => i.SubClients).Where(p => p.parentid == null).ToList();
            return PartialView("_clients", Clients);
        }

        public ActionResult SubClients(long id)
        {
            ViewBag.countrylist = new SelectList(db.Country, "id", "nicename");
            ViewBag.statelist = new SelectList(db.State, "id", "name");
            List<Client> Clients = db.Client.Include(i => i.SubClients).Where(p => p.parentid != null && p.parentid == id)
                .ToList();
            List<Project> projects = db.Project.Include(i => i.SubProjects).Where(p => p.parentid == null && p.clientid == id)
                .ToList();
            string clientlist = PartialView("_clients", Clients).RenderToString();
            string projectlist = PartialView("_Projects", projects).RenderToString();
            return Json(new { ResponseType = 1, clientlist, projectlist }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Projects(long id)
        {
            ViewBag.countrylist = new SelectList(db.Country, "id", "nicename");
            ViewBag.statelist = new SelectList(db.State, "id", "name");
            List<Project> projects = db.Project.Include(i => i.SubProjects).Where(p => p.parentid == null && p.clientid == id)
                .ToList();
            return PartialView("_Projects", projects);
        }

        public ActionResult SubProjects(long id)
        {
            ViewBag.countrylist = new SelectList(db.Country, "id", "nicename");
            ViewBag.statelist = new SelectList(db.State, "id", "name");
            List<Project> projects = db.Project.Include(i => i.SubProjects).Where(p => p.parentid != null && p.parentid == id)
                .ToList();
            return PartialView("_Projects", projects);
        }

        //Load task by project
        public ActionResult LoadTasks(long id)
        {
            List<TicketItem> ticketItem = db.TicketItem.Include(t => t.TicketItemLog)
                .Where(t => t.statusid == 2 && t.projectid == id).ToList();
            return PartialView("_LoadTasks", ticketItem);
        }

        // GET: Clients/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Client client = db.Client.Find(id);
            if (client == null)
            {
                return HttpNotFound();
            }

            return View(client);
        }

        // GET: Clients/Create
        public ActionResult Create(long? id)
        {
            ViewBag.countrylist = new SelectList(db.Country, "id", "nicename");
            ViewBag.statelist = new SelectList(db.State, "id", "name");
            ViewBag.clienttypes = new SelectList(db.ClientType, "id", "name");
            ViewBag.customersources = new SelectList(db.CustomerSource, "id", "name");
            ViewBag.users =
                new SelectList(UserManager.Users.OrderBy(f => f.FirstName).Where(u => u.isactive == true).ToList(),
                    "Id", "FullName");
            List<SelectListItem> clienttlist = db.Database.SqlQuery<CombinedEntity>("exec clients_loadcombined").Select(x =>
                new SelectListItem
                {
                    Text = x.name,
                    Value = x.id.ToString()
                }).ToList();
            if (id == null)
            {
                ViewBag.parentlist = new SelectList(clienttlist, "Value", "Text");
                ViewBag.hasparent = false;
            }
            else
            {
                ViewBag.parentlist = new SelectList(clienttlist, "Value", "Text", id.Value);
                ViewBag.hasparent = true;
                ViewBag.parentid = id.Value;
            }

            return View();
        }

        // POST: Clients/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(
            [Bind(Include =
                "id,parentid,name,clienttypeid,customersourceid,accountmanager,email,maxbillablehours,address,city,stateid,countryid,zip,phone,mobile,website,isactive,pmplateformlink,usersid,createdonutc,updatedonutc,ipused,userid,iswarning,warningtext")]
            Client client)
        {
            Client clientname = db.Client.Where(c => c.name == client.name).FirstOrDefault();
            if (clientname != null)
            {
                ModelState.AddModelError("name", "Sorry, client name already exist.");
            }

            if (client.customersourceid.ToString() == "")
            {
                ModelState.AddModelError("customersourceid", "Please select customer source.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    client.id = 0;
                    client.createdonutc = DateTime.Now;
                    client.updatedonutc = DateTime.Now;
                    client.ipused = Request.UserHostAddress;
                    client.userid = User.Identity.GetUserId();
                    db.Client.Add(client);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ViewBag.countrylist = new SelectList(db.Country, "id", "nicename");
                    ViewBag.statelist = new SelectList(db.State, "id", "name");
                    ViewBag.clienttypes = new SelectList(db.ClientType, "id", "name");
                    ViewBag.customersources = new SelectList(db.CustomerSource, "id", "name");
                    ViewBag.users =
                        new SelectList(
                            UserManager.Users.OrderBy(f => f.FirstName).Where(u => u.isactive == true).ToList(), "Id",
                            "FullName");
                    List<SelectListItem> clienttlist = db.Database.SqlQuery<CombinedEntity>("exec clients_loadcombined").Select(x =>
                        new SelectListItem
                        {
                            Text = x.name,
                            Value = x.id.ToString()
                        }).ToList();
                    ViewBag.parentlist = new SelectList(clienttlist, "Value", "Text");
                    ModelState.AddModelError("", ex.Message);
                    return View(client);
                }
            }

            ViewBag.countrylist = new SelectList(db.Country, "id", "nicename");
            ViewBag.statelist = new SelectList(db.State, "id", "name");
            ViewBag.clienttypes = new SelectList(db.ClientType, "id", "name");
            ViewBag.customersources = new SelectList(db.CustomerSource, "id", "name");
            List<SelectListItem> Clienttlist = db.Database.SqlQuery<CombinedEntity>("exec clients_loadcombined").Select(x =>
                new SelectListItem
                {
                    Text = x.name,
                    Value = x.id.ToString()
                }).ToList();
            ViewBag.parentlist = new SelectList(Clienttlist, "Value", "Text");
            ViewBag.users =
                new SelectList(UserManager.Users.OrderBy(f => f.FirstName).Where(u => u.isactive == true).ToList(),
                    "Id", "FullName");
            return View(client);
        }

        // GET: Clients/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Client client = db.Client.Find(id);
            if (client == null)
            {
                return HttpNotFound();
            }

            ViewBag.countrylist = new SelectList(db.Country, "id", "nicename");
            ViewBag.statelist = new SelectList(db.State, "id", "name");
            ViewBag.clienttypes = new SelectList(db.ClientType, "id", "name");
            ViewBag.customersources = new SelectList(db.CustomerSource, "id", "name");
            ViewBag.users =
                new SelectList(UserManager.Users.OrderBy(f => f.FirstName).Where(u => u.isactive == true).ToList(),
                    "Id", "FullName");
            List<SelectListItem> clienttlist = db.Database.SqlQuery<CombinedEntity>("exec clients_loadcombined").Select(x =>
                new SelectListItem
                {
                    Text = x.name,
                    Value = x.id.ToString()
                }).ToList();
            clienttlist.RemoveAll(c => c.Value == id.ToString());
            ViewBag.parentlist = new SelectList(clienttlist, "Value", "Text");
            return View(client);
        }

        // POST: Clients/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(
            [Bind(Include =
                "id,parentid,name,clienttypeid,customersourceid,accountmanager,email,maxbillablehours,address,city,stateid,countryid,zip,phone,mobile,website,isactive,pmplateformlink,usersid,createdonutc,updatedonutc,ipused,userid,iswarning,warningtext")]
            Client client)
        {
            if (client.customersourceid.ToString() == "")
            {
                ModelState.AddModelError("customersourceid", "Please select customer source.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    client.updatedonutc = DateTime.Now;
                    client.ipused = Request.UserHostAddress;
                    client.userid = User.Identity.GetUserId();
                    if (!client.isactive)
                    {
                        List<Project> projectslist = db.Project.Where(p => p.clientid == client.id && p.isactive).ToList();
                        foreach (Project project in projectslist)
                        {
                            Project pro = project;
                            pro.isactive = false;
                            db.Entry(pro).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }

                    db.Entry(client).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ViewBag.countrylist = new SelectList(db.Country, "id", "nicename");
                    ViewBag.statelist = new SelectList(db.State, "id", "name");
                    ViewBag.clienttypes = new SelectList(db.ClientType, "id", "name");
                    ViewBag.customersources = new SelectList(db.CustomerSource, "id", "name");
                    ViewBag.users =
                        new SelectList(
                            UserManager.Users.OrderBy(f => f.FirstName).Where(u => u.isactive == true).ToList(), "Id",
                            "FullName");
                    List<SelectListItem> clienttlist = db.Database.SqlQuery<CombinedEntity>("exec clients_loadcombined").Select(x =>
                        new SelectListItem
                        {
                            Text = x.name,
                            Value = x.id.ToString()
                        }).ToList();
                    ViewBag.parentlist = new SelectList(clienttlist, "Value", "Text");
                    ModelState.AddModelError("", ex.Message);
                    return View(client);
                }
            }

            ViewBag.countrylist = new SelectList(db.Country, "id", "nicename");
            ViewBag.statelist = new SelectList(db.State, "id", "name");
            ViewBag.clienttypes = new SelectList(db.ClientType, "id", "name");
            ViewBag.customersources = new SelectList(db.CustomerSource, "id", "name");
            ViewBag.users =
                new SelectList(UserManager.Users.OrderBy(f => f.FirstName).Where(u => u.isactive == true).ToList(),
                    "Id", "FullName");
            List<SelectListItem> Clienttlist = db.Database.SqlQuery<CombinedEntity>("exec clients_loadcombined").Select(x =>
                new SelectListItem
                {
                    Text = x.name,
                    Value = x.id.ToString()
                }).ToList();
            Clienttlist.RemoveAll(c => c.Value == client.id.ToString());
            ViewBag.parentlist = new SelectList(Clienttlist, "Value", "Text");
            return View(client);
        }

        // GET: Clients/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Client client = db.Client.Find(id);
            if (client == null)
            {
                return HttpNotFound();
            }

            return View(client);
        }

        // POST: Clients/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            Client client = db.Client.Find(id);
            db.Client.Remove(client);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult UpdateStatus()
        {
            try
            {
                InActiveProject();
                InActiveClient();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void InActiveClient()
        {
            try
            {
                List<Client> clients = db.Client.Where(x => x.isactive).ToList();
                foreach (Client client in clients)
                {
                    int project_count = db.Project.Where(p => p.clientid == client.id && p.isactive).Count();
                    if (project_count <= 0)
                    {
                        client.isactive = false;
                        client.updatedonutc = DateTime.UtcNow;
                        db.Entry(client).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
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
                throw ex;
            }
        }

        public void InActiveProject()
        {
            try
            {
                List<Project> projectslist = db.Project.Where(p => p.isactive).ToList();
                foreach (Project project in projectslist)
                {
                    //var ticketscount = db.Ticket
                    //        .Where(p => p.projectid == project.id && p.statusid != 3 && p.statusid != 8)
                    //        .Count();
                    //if (ticketscount == 0)
                    //{
                    Ticket lastticket = db.Ticket.Where(p => p.projectid == project.id)
                        .OrderByDescending(t => t.lastdeliverytime).FirstOrDefault();
                    if (lastticket == null || DateTime.Now.Subtract(lastticket.lastmodifiedtime).TotalDays >= 30)
                    {
                        project.isactive = false;
                        project.updatedonutc = DateTime.UtcNow;
                        db.Entry(project).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    //}
                }
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

                throw ex;
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
    }
}