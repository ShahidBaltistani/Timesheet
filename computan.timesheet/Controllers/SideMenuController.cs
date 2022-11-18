using computan.timesheet.Contexts;
using computan.timesheet.Helpers;
using System.Web.Mvc;

namespace computan.timesheet.Controllers
{
    [CustomeAuthorizeAttribute]
    public class SideMenuController : Controller
    {
        // GET: SideMenu 
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        [AllowAnonymous]
        [ChildActionOnly]
        public ActionResult Index()
        {
            //SideMenuViewModel Smvm = new SideMenuViewModel();
            //Smvm.TicketStatus = db.TicketStatus.ToList();

            //var MyStatus = db.TicketStatus.ToList();
            //MyStatus.RemoveAt(0);
            //Smvm.MyTicketStatus = MyStatus;
            ViewBag.currentTab = GetCurrentActiveTab();
            ViewBag.currentSubTab = GetCurrentActiveSubTab();
            //return PartialView("_LayoutSideMenu", Smvm);
            return PartialView("_LayoutSideMenu");
        }

        private string GetCurrentActiveTab()
        {
            System.Web.Routing.RouteData rd = ControllerContext.ParentActionViewContext.RouteData;
            string currentAction = rd.GetRequiredString("action");
            string currentController = rd.GetRequiredString("controller");
            string currentTab = string.Empty;

            switch (currentController.ToLower())
            {
                case "home":
                    switch (currentAction.ToLower())
                    {
                        case "index":
                            currentTab = "home";
                            break;
                        case "myprofile":
                            currentTab = "home";
                            break;
                        case "Workloadreport":
                            currentTab = "Workloadreport";
                            break;
                    }

                    break;
                case "sentitemlogs":
                    switch (currentAction.ToLower())
                    {
                        case "index":
                            currentTab = "sentitems";
                            break;
                    }

                    break;
                case "tickets":
                    switch (currentAction.ToLower())
                    {
                        case "index":
                            object id = rd.Values["id"];
                            if (id != null)
                            {
                                switch (id.ToString())
                                {
                                    case "0":
                                        currentTab = "loadalltickets";
                                        break;
                                    case "1":
                                        currentTab = "taskbystatus";
                                        break;
                                    default:
                                        currentTab = "taskbystatus";
                                        break;
                                }
                            }

                            break;
                        case "mytickets":
                            currentTab = "mytasks";
                            break;
                    }

                    break;
                case "ticketitems":
                    switch (currentAction.ToLower())
                    {
                        case "mytasks":
                            currentTab = "mytasks";
                            break;
                    }

                    break;
                case "orphan":
                    currentTab = "Getallorphantickets";
                    break;

                case "tickettimelogs":
                    currentTab = "timelog";
                    break;
                case "clients":
                case "clientsandprojects":
                case "projects":
                case "rules":
                case "teams":
                case "skills":
                case "states":
                case "countries":
                case "credentialtypes":
                case "credentialcategories":
                case "credentiallevels":
                case "credentials":
                case "rolesadmin":
                case "usersadmin":
                case "integrations":
                    currentTab = "features";
                    break;
            }

            return currentTab;
        }

        private string GetCurrentActiveSubTab()
        {
            System.Web.Routing.RouteData rd = ControllerContext.ParentActionViewContext.RouteData;
            string currentAction = rd.GetRequiredString("action");
            string currentController = rd.GetRequiredString("controller");
            string currentSubTab = string.Empty;

            switch (currentController.ToLower())
            {
                case "home":
                    currentSubTab = "";
                    break;
                case "tickets":
                    switch (currentAction.ToLower())
                    {
                        case "index":
                            object id = rd.Values["id"];
                            if (id != null)
                            {
                                switch (id.ToString())
                                {
                                    case "1":
                                        currentSubTab = "Not Assigned";
                                        break;
                                    case "2":
                                        currentSubTab = "In Progress";
                                        break;
                                    case "3":
                                        currentSubTab = "On Hold";
                                        break;
                                    case "4":
                                        currentSubTab = "Resolved";
                                        break;
                                    case "5":
                                        currentSubTab = "Closed";
                                        break;
                                    case "6":
                                        currentSubTab = "Duplicate";
                                        break;
                                    case "7":
                                        currentSubTab = "Invalid";
                                        break;
                                    case "8":
                                        currentSubTab = "Wont Fix";
                                        break;
                                }
                            }
                            else
                            {
                                currentSubTab = "";
                            }

                            break;
                    }

                    break;
                case "ticketitems":
                    switch (currentAction.ToLower())
                    {
                        case "mytasks":
                            object id = rd.Values["id"];
                            if (id != null)
                            {
                                switch (id.ToString())
                                {
                                    case "1":
                                        currentSubTab = "Not Assigned";
                                        break;
                                    case "2":
                                        currentSubTab = "In Progress";
                                        break;
                                    case "3":
                                        currentSubTab = "On Hold";
                                        break;
                                    case "4":
                                        currentSubTab = "Resolved";
                                        break;
                                    case "5":
                                        currentSubTab = "Closed";
                                        break;
                                    case "6":
                                        currentSubTab = "Duplicate";
                                        break;
                                    case "7":
                                        currentSubTab = "Invalid";
                                        break;
                                    case "8":
                                        currentSubTab = "Wont Fix";
                                        break;
                                }
                            }
                            else
                            {
                                currentSubTab = "";
                            }

                            break;
                    }

                    break;

                case "tickettimelogs":
                    switch (currentAction.ToLower())
                    {
                        case "index":
                            currentSubTab = "My Time Log";
                            break;
                        case "team":
                            currentSubTab = "Team Time Log";
                            break;
                    }

                    break;
                case "clientsandprojects":
                    currentSubTab = "ClientsAndProject";
                    break;
                case "clients":
                    currentSubTab = "Clients";
                    break;

                case "projects":
                    currentSubTab = "Projects";
                    break;
                case "rules":
                    currentSubTab = "Rules";
                    break;
                case "teams":
                    currentSubTab = "Teams";
                    break;

                case "skills":
                    currentSubTab = "Skills";
                    break;

                case "states":
                    currentSubTab = "States";
                    break;

                case "countries":
                    currentSubTab = "Countries";
                    break;

                case "rolesadmin":
                    currentSubTab = "Manage Roles";
                    break;
                case "usersadmin":
                    currentSubTab = "Manage Users";
                    break;
                case "credentials":
                    currentSubTab = "credentials";
                    break;
                case "credentialtypes":
                    currentSubTab = "credential type";
                    break;
                case "credentialcategories":
                    currentSubTab = "credential categories";
                    break;
                case "credentiallevels":
                    currentSubTab = "credential levels";
                    break;
            }

            return currentSubTab;
        }
    }
}