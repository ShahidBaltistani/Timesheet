using computan.timesheet.Contexts;
using computan.timesheet.core;
using computan.timesheet.core.common;
using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace computan.timesheet.Controllers
{
    public class BaseController : Controller
    {
        protected readonly ApplicationDbContext db = new ApplicationDbContext();

        protected int GetOpenTicketsCount()
        {
            int TicketCount = db.Ticket.Where(t => t.statusid == 1).Count();
            return TicketCount;
        }
       
        protected int GetBucketItemCount()
        {
            string currentUserId = User.Identity.GetUserId();
            int bucketItemCount = db.TicketTimeLog.Where(t => t.teamuserid == currentUserId)
                .Where(t => t.timespentinminutes == 0 && t.billabletimeinminutes == 0)
                .Count();
            return bucketItemCount;
        }

        protected string GetUserFullname()
        {
            ApplicationUser userinfo = (ApplicationUser)Session[Role.User.ToString()];
            return userinfo.FullName;
        }

        protected string GetUserProfileImageURL()
        {
            ApplicationUser userinfo = (ApplicationUser)Session[Role.User.ToString()];
            return userinfo.ProfileImage;
        }
    }
}