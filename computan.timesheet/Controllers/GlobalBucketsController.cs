using computan.timesheet.Contexts;
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
    public class GlobalBucketsController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        // GET: GlobalBuckets
        //[AllowAnonymous]
        //[ChildActionOnly]
        public ActionResult Index()
        {
            string currentUserId = User.Identity.GetUserId();
            TicketTimeViewModel timeVM = new TicketTimeViewModel();
            DateTime currentdatetimestart = DateTime.Now.Date;
            TimeSpan time = new TimeSpan(00, 00, 00);
            currentdatetimestart = currentdatetimestart.Add(time);
            DateTime currentdatetimeend = DateTime.Now.Date;
            currentdatetimeend = currentdatetimeend.Add(TimeSpan.Parse("23:59:59"));
            timeVM.SearchResults = db.TicketTimeLog.Include(t => t.Project).Include(t => t.Skill)
                .Include(t => t.TicketItem).Include(t => t.User).Where(t => t.teamuserid == currentUserId)
                .Where(ttl => ttl.workdate >= currentdatetimestart && ttl.workdate <= currentdatetimeend)
                .Where(t => t.timespentinminutes == 0 && t.billabletimeinminutes == 0).ToList();
            timeVM.TimeAddedResults = db.TicketTimeLog.Include(t => t.Project).Include(t => t.Skill)
                .Include(t => t.TicketItem).Include(t => t.User).Where(t => t.teamuserid == currentUserId)
                .Where(ttl => ttl.workdate >= currentdatetimestart && ttl.workdate <= currentdatetimeend).Where(t =>
                    t.timespentinminutes != 0 && t.timespentinminutes != null && t.billabletimeinminutes != 0 &&
                    t.billabletimeinminutes != null).ToList();
            List<SelectListItem> timespentinminutes = new List<SelectListItem>();
            int timespentinminute = 0;
            int Counter = 1;
            for (Counter = 1; Counter <= 120; Counter++)
            {
                timespentinminutes.Add(new SelectListItem
                { Text = timespentinminute.ToString(), Value = timespentinminute.ToString() });
                timespentinminute += 5;
            }

            ViewData["item.timespentinminutes"] = timespentinminutes;
            ViewData["item.billabletimeinminutes"] = timespentinminutes;
            return PartialView("~/views/shared/_GlobalBuckets.cshtml", timeVM);
        }

        public string getbucktscount()
        {
            string currentUserId = User.Identity.GetUserId();
            DateTime currentdatetimestart = DateTime.Now.Date;
            TimeSpan time = new TimeSpan(00, 00, 00);
            currentdatetimestart = currentdatetimestart.Add(time);
            DateTime currentdatetimeend = DateTime.Now.Date;
            currentdatetimeend = currentdatetimeend.Add(TimeSpan.Parse("23:59:59"));
            int items = db.TicketTimeLog.Include(t => t.Project).Include(t => t.Skill).Include(t => t.TicketItem)
                .Include(t => t.User).Where(t => t.teamuserid == currentUserId)
                .Where(ttl => ttl.workdate >= currentdatetimestart && ttl.workdate <= currentdatetimeend)
                .Where(t => t.timespentinminutes == 0 && t.billabletimeinminutes == 0).Count();
            return items.ToString();
        }

        //[AllowAnonymous]
        //[ChildActionOnly]
        public ActionResult BucketSearchFilter()
        {
            TicketTimeViewModel timeVM = new TicketTimeViewModel
            {
                SearchTicketTime = new TicketTimeSearchViewModel
                {
                    StartDate = DateTime.Now.Date.Add(TimeSpan.Parse("00:00:00")),
                    EndDate = DateTime.Now.Date.Add(TimeSpan.Parse("23:59:59")),
                    projectid = null,
                    ProjectCollection = null
                }
            };

            return PartialView("_GlobalBucketSearchFilter", timeVM.SearchTicketTime);
        }

        public ActionResult MyBucketsGlobal(DateTime StartDate)
        {
            string currentUserId = User.Identity.GetUserId();

            if (ModelState.IsValid)
            {
                //long? projectid = null;
                TicketTimeViewModel searchTimeVM = new TicketTimeViewModel
                {
                    SearchTicketTime = new TicketTimeSearchViewModel
                    {
                        StartDate = StartDate.Add(TimeSpan.Parse("00:00:00")),
                        EndDate = StartDate.Add(TimeSpan.Parse("23:59:59")),
                        projectid = null,
                        ProjectCollection = null
                        //ProjectCollection = db.Database.SqlQuery<CombinedEntity>("exec projects_loadcombined").Select(x => new SelectListItem
                        //{
                        //    Text = x.name,
                        //    Value = x.id.ToString()
                        //}).ToList()
                    }
                };
                //string searchCurrentUserId = User.Identity.GetUserId();

                //if (projectid != null)
                //{
                //    searchTimeVM.SearchResults = db.TicketTimeLog.Include(t => t.Project).Include(t => t.Skill).Include(t => t.TicketItem).Include(t => t.TeamUser).Where(t => t.teamuserid == searchCurrentUserId).Where(ttl => DbFunctions.TruncateTime(ttl.workdate) >= searchTimeVM.SearchTicketTime.StartDate && DbFunctions.TruncateTime(ttl.workdate) <= searchTimeVM.SearchTicketTime.EndDate).Where(ttl => ttl.projectid == projectid).Where(t => t.timespentinminutes == 0 && t.billabletimeinminutes == 0).OrderByDescending(w => w.workdate).ToList();
                //    searchTimeVM.TimeAddedResults = db.TicketTimeLog.Include(t => t.Project).Include(t => t.Skill).Include(t => t.TicketItem).Include(t => t.TeamUser).Where(t => t.teamuserid == searchCurrentUserId).Where(ttl => DbFunctions.TruncateTime(ttl.workdate) >= searchTimeVM.SearchTicketTime.StartDate && DbFunctions.TruncateTime(ttl.workdate) <= searchTimeVM.SearchTicketTime.EndDate).Where(ttl => ttl.projectid == projectid).Where(t => (t.timespentinminutes != 0 && t.timespentinminutes != null) && (t.billabletimeinminutes !=0 && t.billabletimeinminutes != null)).OrderByDescending(w => w.workdate).ToList();
                //}
                //else
                //{
                //    searchTimeVM.SearchResults = db.TicketTimeLog.Include(t => t.Project).Include(t => t.Skill).Include(t => t.TicketItem).Include(t => t.TeamUser).Where(t => t.teamuserid == searchCurrentUserId).Where(ttl => DbFunctions.TruncateTime(ttl.workdate) >= searchTimeVM.SearchTicketTime.StartDate && DbFunctions.TruncateTime(ttl.workdate) <= searchTimeVM.SearchTicketTime.EndDate).Where(t => t.timespentinminutes == 0 && t.billabletimeinminutes == 0).OrderByDescending(w => w.workdate).ToList();
                //    searchTimeVM.TimeAddedResults = db.TicketTimeLog.Include(t => t.Project).Include(t => t.Skill).Include(t => t.TicketItem).Include(t => t.TeamUser).Where(t => t.teamuserid == searchCurrentUserId).Where(ttl => DbFunctions.TruncateTime(ttl.workdate) >= searchTimeVM.SearchTicketTime.StartDate && DbFunctions.TruncateTime(ttl.workdate) <= searchTimeVM.SearchTicketTime.EndDate).Where(t => (t.timespentinminutes != 0 && t.timespentinminutes != null) && (t.billabletimeinminutes != 0 && t.billabletimeinminutes != null)).OrderByDescending(w => w.workdate).ToList();
                //}

                searchTimeVM.SearchResults = db.TicketTimeLog.Include(t => t.Project).Include(t => t.Skill)
                    .Include(t => t.TicketItem).Include(t => t.TeamUser).Where(t => t.teamuserid == currentUserId)
                    .Where(ttl =>
                        DbFunctions.TruncateTime(ttl.workdate) >= searchTimeVM.SearchTicketTime.StartDate &&
                        DbFunctions.TruncateTime(ttl.workdate) <= searchTimeVM.SearchTicketTime.EndDate)
                    .Where(t => t.timespentinminutes == 0 && t.billabletimeinminutes == 0)
                    .OrderByDescending(w => w.workdate).ToList();
                searchTimeVM.TimeAddedResults = db.TicketTimeLog.Include(t => t.Project).Include(t => t.Skill)
                    .Include(t => t.TicketItem).Include(t => t.TeamUser).Where(t => t.teamuserid == currentUserId)
                    .Where(ttl =>
                        DbFunctions.TruncateTime(ttl.workdate) >= searchTimeVM.SearchTicketTime.StartDate &&
                        DbFunctions.TruncateTime(ttl.workdate) <= searchTimeVM.SearchTicketTime.EndDate).Where(t =>
                        t.timespentinminutes != 0 && t.timespentinminutes != null && t.billabletimeinminutes != 0 &&
                        t.billabletimeinminutes != null).OrderByDescending(w => w.workdate).ToList();

                //var stime = (double)(searchTimeVM.SearchResults.Sum(t => t.timespentinminutes)) / 60;
                //stime = Math.Round(stime, 2);
                //ViewBag.totalspent = stime;
                //var btime = (double)(searchTimeVM.SearchResults.Sum(t => t.billabletimeinminutes)) / 60;
                //btime = Math.Round(btime, 2);
                //ViewBag.totalbillable = btime;
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
                return PartialView("_GlobalBuckets", searchTimeVM);
            }

            TicketTimeViewModel timeVM = new TicketTimeViewModel
            {
                SearchTicketTime = new TicketTimeSearchViewModel
                {
                    StartDate = DateTime.Now.Add(TimeSpan.Parse("00:00:00")),
                    EndDate = DateTime.Now.Add(TimeSpan.Parse("23:59:59")),
                    projectid = null,
                    ProjectCollection = null
                    //ProjectCollection = db.Database.SqlQuery<CombinedEntity>("exec projects_loadcombined").Select(x => new SelectListItem
                    //{
                    //    Text = x.name,
                    //    Value = x.id.ToString()
                    //}).ToList()
                }
            };
            //string currentUserId = User.Identity.GetUserId();
            timeVM.SearchResults = db.TicketTimeLog.Include(t => t.Project).Include(t => t.Skill)
                .Include(t => t.TicketItem).Include(t => t.TeamUser).Where(t => t.teamuserid == currentUserId)
                .Where(ttl =>
                    DbFunctions.TruncateTime(ttl.workdate) >= timeVM.SearchTicketTime.StartDate &&
                    DbFunctions.TruncateTime(ttl.workdate) <= timeVM.SearchTicketTime.EndDate)
                .Where(t => t.timespentinminutes == 0 && t.billabletimeinminutes == 0)
                .OrderByDescending(w => w.workdate).ToList();
            //var stime2 = (double)(timeVM.SearchResults.Sum(t => t.timespentinminutes)) / 60;
            //stime2 = Math.Round(stime2, 2);
            //ViewBag.totalspent = stime2;
            //var btime2 = (double)(timeVM.SearchResults.Sum(t => t.billabletimeinminutes)) / 60;
            //btime2 = Math.Round(btime2, 2);
            //ViewBag.totalbillable = btime2;
            //List<SelectListItem> timespentinminutes = new List<SelectListItem>();
            //int timespentinminute = 0;
            //int Counter = 1;
            //for (Counter = 1; Counter <= 120; Counter++)
            //{
            //    timespentinminute = timespentinminute + 5;
            //    timespentinminutes.Add(new SelectListItem { Text = timespentinminute.ToString(), Value = timespentinminute.ToString() });
            //}
            //ViewData["item.timespentinminutes"] = timespentinminutes;
            //ViewData["item.billabletimeinminutes"] = timespentinminutes;
            return PartialView("_GlobalBuckets", timeVM);
        }
    }
}