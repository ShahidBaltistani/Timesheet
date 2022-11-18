using System.Web.Mvc;
using System.Web.Routing;

namespace computan.timesheet
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            #region TicketRoutes

            //  routes.MapRoute(
            //    "MyTicketsComment",                                              // Route name
            //    "Tickets/loadCommentByTicketId/{id}",                           // URL with parameters
            //    new { controller = "Tickets", action = "loadCommentByTicketId" }  // Parameter defaults
            //);
            routes.MapRoute(
                "UserDashboardRoute", // Route name
                "Home/userdashoard/{id}/{userid}", // URL with parameters
                new { controller = "Home", action = "userdashoard" } // Parameter defaults
            );
            routes.MapRoute(
                "MyTicketsRoute", // Route name
                "Tickets/MyTickets/{id}/{clientid}", // URL with parameters
                new { controller = "Tickets", action = "MyTickets" } // Parameter defaults
            );
            routes.MapRoute(
                "SearchIndex", // Route name
                "Tickets/Index/{id}/{searchvalue}", // URL with parameters
                new { controller = "Tickets", action = "Index" } // Parameter defaults
            );

            #endregion TicketRoutes

            #region MyTasks-Routes

            routes.MapRoute(
                "MyTasks", // Route name
                "ticketitems/mytaskajax/{id}/{pagenum}/{topic}", // URL with parameters
                new
                {
                    controller = "TicketItems",
                    action = "MyTaskAjax",
                    id = 1,
                    pagenum = 0,
                    topic = UrlParameter.Optional
                } // Parameter defaults
            );
            routes.MapRoute(
                "Alltaskajax", // Route name
                "ticketitems/AllTasksListAjax/{fromdate}/{todate}/{clientid}/{projectid}", // URL with parameters
                new
                {
                    controller = "TicketItems",
                    action = "AllTasksListAjax",
                    clientid = UrlParameter.Optional,
                    projectid = UrlParameter.Optional
                } // Parameter defaults
            );

            routes.MapRoute(
                "userstatusupdate", // Route name
                "ticketitems/ChangeUserStatus/{ticketitemid}/{statusid}", // URL with parameters
                new { controller = "ticketitems", action = "ChangeUserStatus" } // Parameter defaults
            );

            routes.MapRoute(
                "MyTasks_ByStatusAndClient", // Route name
                "ticketitems/mytasks/{id}/{clientid}", // URL with parameters
                new { controller = "TicketItems", action = "mytasks" } // Parameter defaults
            );

            //routes.MapRoute(
            //    "mytasks_noparam",                               // route name
            //    "ticketitems/mytasks",                           // url without parameters
            //    new { controller = "ticketitems", action = "mytasks", id = 2 }  // parameter defaults
            //);

            #endregion MyTasks-Routes

            routes.MapRoute(
                "Search", // Route name
                "Tickets/SearchTickets/{searchstring}/{statusid}", // URL with parameters
                new { controller = "Tickets", action = "SearchTickets", statusid = 1 } // Parameter defaults
            );
            routes.MapRoute(
                "TicketComments", // Route name
                "Tickets/Comment/{id}/{commentid}", // URL with parameters
                new { controller = "Tickets", action = "Comment" } // Parameter defaults
            );
            routes.MapRoute(
                "TicketCredential", // Route name
                "Tickets/Credentials/{id}/{projectid}", // URL with parameters
                new { controller = "Tickets", action = "Credentials" } // Parameter defaults
            );
            routes.MapRoute(
                "Tickets", // Route name
                "Tickets/IndexAjax/{id}/{pagenum}/{topic}", // URL with parameters
                new
                {
                    controller = "Tickets",
                    action = "IndexAjax",
                    id = 1,
                    pagenum = 0,
                    topic = UrlParameter.Optional
                } // Parameter defaults
            );
            routes.MapRoute(
                "TicketsByClient", // Route name
                "TicketsByClient/Index/{id}/{clientid}", // URL with parameters
                new { controller = "TicketsByClient", action = "index" } // Parameter defaults
            );
            routes.MapRoute(
                "MyTickets", // Route name
                "Tickets/MyTicketsAjax/{id}/{pagenum}", // URL with parameters
                new { controller = "Tickets", action = "MyTicketsAjax", id = 1, pagenum = 0 } // Parameter defaults
            );

            routes.MapRoute(
                "swaping", // Route name
                "rules/sortingrow/{id}/{value}", // URL with parameters
                new { controller = "rules", action = "sortingrow" } // Parameter defaults
            );

            routes.MapRoute(
                "GlobalBucketSearch", // Route name
                "GlobalBuckets/MyBucketsGlobal/{StartDate}", // URL with parameters
                new { controller = "GlobalBuckets", action = "MyBucketsGlobal" } // Parameter defaults
            );
            routes.MapRoute(
                "ProjectDashboard", // Route name
                "projectdashboard/{id}", // URL with parameters
                new { controller = "projectdashboard", action = "index" } // Parameter defaults
            );
            routes.MapRoute(
                "ProjectDashboardfilter", // Route name
                "projectdashboard/get_task/{id}/{statusid}", // URL with parameters
                new { controller = "projectdashboard", action = "get_task" } // Parameter defaults
            );
            routes.MapRoute(
                "ProjectDashboardnotes", // Route name
                "projectdashboard/addprojectnotes/{id}/{text}", // URL with parameters
                new { controller = "projectdashboard", action = "addprojectnotes" } // Parameter defaults
            );
            routes.MapRoute(
                "Default",
                "{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                "ClientsDashboard", // Route name
                "Clients/Dashboard/{id}/{type}/{userId}", // URL with parameters
                new
                {
                    controller = "Clients",
                    action = "Dashboard",
                    id = 1,
                    type = "active",
                    userId = UrlParameter.Optional
                } // Parameter defaults
            );

            routes.MapRoute(
                "ProjectDashboardIndexWithFilter", // Route name
                "Project/Index/{id}/{statusid}", // URL with parameters
                new { controller = "Project", action = "Index" } // Parameter defaults
            );
        }
    }
}