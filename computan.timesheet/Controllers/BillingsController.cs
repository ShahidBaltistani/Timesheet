using computan.timesheet.Contexts;
using computan.timesheet.core;
using computan.timesheet.Helpers;
using computan.timesheet.Infrastructure;
using computan.timesheet.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace computan.timesheet.Controllers
{
    [CustomeAuthorizeAttribute]
    public class BillingsController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();
        private List<Project> nthprojects = new List<Project>();

        // GET: Billings
        public ActionResult Index()
        {
            /// GEt billable time consumed for all clients
            List<BillingViewModel> bvm = CalculateBillableTime();
            return View(bvm);
        }

        // Send Notications to Team and clients on over 70% usage
        public ActionResult SendinNotification()
        {
            try
            {
                List<BillingViewModel> model = new List<BillingViewModel>();
                /// GEt billable time consumed for all clients
                List<BillingViewModel> bvm = CalculateBillableTime();
                if (bvm == null || bvm.Count == 0)
                {
                    bvm = new List<BillingViewModel>();
                    ModelState.AddModelError("", "Sorry, An error Occurred.");
                    return View("index", bvm);
                }

                bool hasatleast = false;
                string Emailtext = string.Empty;
                Emailtext = "Hi Computan team,<br><br>";
                Emailtext = Emailtext + "The billing periods for the following clients are ending after 2 day.<br><br>";
                Emailtext = Emailtext + "<ul>";
                List<string> ccEmails = new List<string>();
                Team team = db.Team.Include(tm => tm.TeamMember).Where(n => n.name == "Management").FirstOrDefault();
                if (team != null)
                {
                    if (team.TeamMember != null && team.TeamMember.Count > 0)
                    {
                        foreach (TeamMember ccemail in team.TeamMember)
                        {
                            ccEmails.Add(ccemail.User.Email);
                        }
                    }
                }
                else
                {
                    team = new Team();
                }

                foreach (BillingViewModel client in bvm)
                {
                    //if (client.clienttypeid == 1)
                    //{
                    //    if (client.maxbillablehours != null)
                    //    {
                    //        double t = Math.Round(((client.BillableTime / client.maxbillablehours.Value) * 100), 2);
                    //        if (t >= 70)
                    //        {
                    //            model.Add(client);
                    //        }
                    //    }
                    //}
                    if (client.isEndingPeroid)
                    {
                        model.Add(client);
                        BillingNotification billnotification = null;
                        string year = DateTime.Now.Year.ToString();
                        string month = DateTime.Now.ToString("MMMM");
                        string weeknumber = GetWeekNumberOfMonth(DateTime.Now).ToString();
                        if (client.Billcyletypeid == 1)
                        {
                            billnotification = db.BillingNotification.Where(c =>
                                c.notificationtypeid == 2 && c.clientid == client.clientid && c.billingyear == year &&
                                c.billingmonth == month && c.billingweek == weeknumber).FirstOrDefault();
                        }

                        if (client.Billcyletypeid == 2)
                        {
                            billnotification = db.BillingNotification.Where(c =>
                                c.notificationtypeid == 2 && c.clientid == client.clientid && c.billingyear == year &&
                                c.billingmonth == month).FirstOrDefault();
                        }

                        if (client.Billcyletypeid == 3)
                        {
                            billnotification = db.BillingNotification.Where(c =>
                                c.notificationtypeid == 2 && c.clientid == client.clientid && c.billingyear == year &&
                                c.billingmonth == month).FirstOrDefault();
                        }

                        if (billnotification == null)
                        {
                            hasatleast = true;
                            EmailTemplate EmailTemplate = db.EmailTemplate.FirstOrDefault();
                            string EmailBody = EmailTemplate.body.Replace("{token}",
                                "Hi Computan team,<br>" + client.ClientName + " billing period ending after 2 days.");
                            Emailtext = Emailtext + "<li>" + client.ClientName + "</li>";
                            billnotification = new BillingNotification
                            {
                                clientid = client.clientid,
                                issent = true,
                                issentagain = false,
                                billingmonth = month,
                                billingyear = year
                            };
                            if (client.Billcyletypeid == 1)
                            {
                                billnotification.billingweek = GetWeekNumberOfMonth(DateTime.Now).ToString();
                            }

                            billnotification.notificationtypeid = 2;
                            billnotification.body = EmailBody;
                            billnotification.maxbillablehours = client.maxbillablehours;
                            billnotification.hoursconsumed = client.BillableTime;
                            billnotification.createdonutc = DateTime.Now;
                            billnotification.updatedonutc = DateTime.Now;
                            billnotification.ipused = Request.UserHostAddress;
                            billnotification.userid = User.Identity.GetUserId();
                            db.BillingNotification.Add(billnotification);
                        }
                    }
                }

                if (hasatleast)
                {
                    Emailtext = Emailtext + "</ul>";
                    string To = ConfigurationManager.AppSettings["ToNotificatoins"];
                    EmailTemplate temp = db.EmailTemplate.FirstOrDefault();
                    string body = temp.body.Replace("{token}", Emailtext);
                    MailService.SendClientEmail(To, "Computan Billing Alert", body, ccEmails);
                    db.SaveChanges();
                }

                return View("index", model);
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
                ViewBag.exception = ex.Message;
                return null;
            }
        }

        public ActionResult SendToNotificationOnUsage()
        {
            try
            {
                List<BillingViewModel> model = new List<BillingViewModel>();
                /// GEt billable time consumed for all clients
                List<BillingViewModel> bvm = CalculateBillableTime();
                if (bvm == null || bvm.Count == 0)
                {
                    bvm = new List<BillingViewModel>();
                    ModelState.AddModelError("", "Sorry, An error Occurred.");
                    return View("index", bvm);
                }

                bool hasatleast = false;
                string Emailtext = string.Empty;
                Emailtext = "Hi Computan team,<br><br>";
                Emailtext = Emailtext + "The billable hours for the following clients are exceed to 70%.<br><br>";
                Emailtext = Emailtext + "<ul>";
                List<string> ccEmails = new List<string>();
                Team team = db.Team.Include(tm => tm.TeamMember).Where(n => n.name == "Management").FirstOrDefault();
                if (team != null)
                {
                    if (team.TeamMember != null && team.TeamMember.Count > 0)
                    {
                        foreach (TeamMember ccemail in team.TeamMember)
                        {
                            ccEmails.Add(ccemail.User.Email);
                        }
                    }
                }
                else
                {
                    team = new Team();
                }

                foreach (BillingViewModel client in bvm)
                {
                    if (client.clienttypeid == 1)
                    {
                        if (client.maxbillablehours != null)
                        {
                            string year = DateTime.Now.Year.ToString();
                            //double t = Math.Round(((client.BillableTime / client.maxbillablehours.Value) * 100), 2);
                            double t = CommonFunctions.RoundTwoDecimalPlaces(client.BillableTime /
                                client.maxbillablehours.Value * 100);
                            NotificationLimitForBilling nlb = db.NotificationLimitForBilling.FirstOrDefault();
                            if (nlb == null)
                            {
                                nlb = new NotificationLimitForBilling
                                {
                                    NotificationLimit = 70
                                };
                            }

                            if (t >= nlb.NotificationLimit)
                            {
                                BillingNotification b_n = null;
                                string month = DateTime.Now.ToString("MMMM");
                                string weeknumber = GetWeekNumberOfMonth(DateTime.Now).ToString();
                                if (client.Billcyletypeid == 1)
                                {
                                    b_n = db.BillingNotification.Where(c =>
                                            c.notificationtypeid == 1 && c.clientid == client.clientid &&
                                            c.billingyear == year && c.billingmonth == month &&
                                            c.billingweek == weeknumber)
                                        .FirstOrDefault();
                                }

                                if (client.Billcyletypeid == 2)
                                {
                                    b_n = db.BillingNotification.Where(c =>
                                        c.notificationtypeid == 1 && c.clientid == client.clientid &&
                                        c.billingmonth == month && c.billingyear == year).FirstOrDefault();
                                }

                                if (client.Billcyletypeid == 3)
                                {
                                    b_n = db.BillingNotification.Where(c =>
                                        c.notificationtypeid == 1 && c.clientid == client.clientid &&
                                        c.billingmonth == month && c.billingyear == year).FirstOrDefault();
                                }

                                int a = 1;
                                if (a == 1)
                                {
                                    hasatleast = true;
                                    EmailTemplate EmailTemplate = db.EmailTemplate.FirstOrDefault();
                                    string EmailBody = EmailTemplate.body.Replace("{token}",
                                        "Hi Computan team,<br> " + client.ClientName +
                                        "  billable hours has been consumed upto " + client.BillableTime +
                                        " Hours out of " + client.maxbillablehours + " Hours (" + t + "%).");
                                    //string body2 = "Hi Computan team, " + client.ClientName + "  billable hours has been consumed upto " + t + "%";
                                    Emailtext = Emailtext + "<li>" + client.ClientName +
                                                "  billable hours has been consumed upto " + client.BillableTime +
                                                " Hours out of " + client.maxbillablehours + " Hours (" + t + "%)." +
                                                "</li>";
                                    b_n = new BillingNotification
                                    {
                                        clientid = client.clientid,
                                        issent = true,
                                        issentagain = false,
                                        billingmonth = DateTime.Now.ToString("MMMM"),
                                        billingyear = year
                                    };
                                    if (client.Billcyletypeid == 1)
                                    {
                                        b_n.billingweek = weeknumber;
                                    }

                                    b_n.notificationtypeid = 1;
                                    b_n.body = EmailBody;
                                    b_n.maxbillablehours = client.maxbillablehours;
                                    b_n.hoursconsumed = client.BillableTime;
                                    b_n.createdonutc = DateTime.Now;
                                    b_n.updatedonutc = DateTime.Now;
                                    b_n.ipused = Request.UserHostAddress;
                                    b_n.userid = User.Identity.GetUserId();
                                    db.BillingNotification.Add(b_n);
                                }
                            }
                        }
                    }
                }

                if (hasatleast)
                {
                    Emailtext = Emailtext + "</ul>";
                    string To = ConfigurationManager.AppSettings["ToNotificatoins"];
                    EmailTemplate temp = db.EmailTemplate.FirstOrDefault();
                    string body = temp.body.Replace("{token}", Emailtext);
                    //  MailService.SendClientEmail(To, "Computan Billing Alert", body, ccEmails);
                    // db.SaveChanges();
                }

                return View("index", bvm);
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
                ViewBag.exception = ex.Message;
                return null;
            }
        }

        // GET: Billings/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Billing billing = db.Billing.Find(id);
            if (billing == null)
            {
                return HttpNotFound();
            }

            return View(billing);
        }

        // Calculate Billable Time Consumed for All Clients
        public List<BillingViewModel> CalculateBillableTime()
        {
            try
            {
                bool IsEnding = false;
                List<Client> clients = (from c in db.Client.Include(s => s.SubClients)
                                        join cb in db.ClientBillingCycle
                                            on c.id equals cb.clientid
                                        where c.parentid == null && c.clienttypeid == 1
                                        select c
                    ).ToList();
                List<BillingViewModel> bvmlist = new List<BillingViewModel>();
                double totaltime = 0;
                if (clients != null && clients.Count > 0)
                {
                    foreach (Client client in clients)
                    {
                        IsEnding = false;
                        ClientBillingCycle cbc = db.ClientBillingCycle.Where(c => c.clientid == client.id).FirstOrDefault();
                        if (cbc == null)
                        {
                            continue;
                        }

                        totaltime = 0;
                        nthprojects = new List<Project>();
                        List<Project> projects = db.Project.Where(i => i.clientid == client.id).ToList();
                        nthprojects.AddRange(projects);
                        if (projects != null && projects.Count > 0)
                        {
                            foreach (Project project in projects)
                            {
                                getsubprojects(project.id);
                            }
                        }

                        if (client.SubClients != null && client.SubClients.Count > 0)
                        {
                            foreach (Client subclients in client.SubClients)
                            {
                                List<Project> projectsofsubclient = db.Project.Where(i => i.clientid == subclients.id).ToList();
                                nthprojects.AddRange(projectsofsubclient);
                                foreach (Project project in projectsofsubclient)
                                {
                                    getsubprojects(project.id);
                                }
                            }
                        }

                        if (nthprojects != null && nthprojects.Count > 0)
                        {
                            foreach (Project items in nthprojects)
                            {
                                List<TicketTimeLog> timelog = new List<TicketTimeLog>();
                                IQueryable<TicketTimeLog> query = db.TicketTimeLog.Where(i =>
                                    i.billabletimeinminutes != 0 && i.billabletimeinminutes != null &&
                                    i.projectid == items.id);
                                if (cbc.billingcyletypeid == 1)
                                {
                                    DayOfWeek day = DateTime.Now.DayOfWeek;
                                    int days = day - DayOfWeek.Monday;
                                    DateTime weekstart =
                                        new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day).AddDays(
                                            -days);
                                    DateTime weekend = weekstart.AddDays(6);
                                    weekend = weekend.Add(TimeSpan.Parse("23:59:59"));
                                    timelog = query.Where(i => i.workdate >= weekstart && i.workdate <= weekend)
                                        .ToList();
                                    if (weekend.AddDays(-2).Subtract(TimeSpan.Parse("23:59:59")) >=
                                        new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day) &&
                                        weekend.AddDays(-2) <=
                                        new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day).Add(
                                            TimeSpan.Parse("23:59:59")))
                                    {
                                        IsEnding = true;
                                    }
                                }

                                if (cbc.billingcyletypeid == 2)
                                {
                                    DateTime startofmnoth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                                    DateTime endofmonth = startofmnoth.AddMonths(1).AddDays(-1);
                                    endofmonth = endofmonth.Add(TimeSpan.Parse("23:59:59"));
                                    if (endofmonth.AddDays(-2).Subtract(TimeSpan.Parse("23:59:59")) >=
                                        new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day) &&
                                        endofmonth.AddDays(-2) <=
                                        new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day).Add(
                                            TimeSpan.Parse("23:59:59")))
                                    {
                                        IsEnding = true;
                                    }

                                    timelog = query.Where(i => i.workdate >= startofmnoth && i.workdate <= endofmonth)
                                        .ToList();
                                }

                                if (cbc.billingcyletypeid == 3)
                                {
                                    if (cbc.date.Value == 31 || cbc.date.Value == 30 || cbc.date.Value == 29)
                                    {
                                        if (DateTime.Now.Month == 2)
                                        {
                                            if (DateTime.IsLeapYear(DateTime.Now.Year))
                                            {
                                                cbc.date = 29;
                                            }
                                            else
                                            {
                                                cbc.date = 28;
                                            }
                                        }
                                    }

                                    DateTime EndingPeroid = new DateTime(DateTime.Now.Year, DateTime.Now.Month,
                                        cbc.date.Value);
                                    DateTime starting_peroid = EndingPeroid.AddMonths(-1).AddDays(1);
                                    EndingPeroid = EndingPeroid.Add(TimeSpan.Parse("23:59:59"));
                                    if (EndingPeroid.AddDays(-2).Subtract(TimeSpan.Parse("23:59:59")) >=
                                        new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day) &&
                                        EndingPeroid.AddDays(-2) <=
                                        new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day).Add(
                                            TimeSpan.Parse("23:59:59")))
                                    {
                                        IsEnding = true;
                                    }

                                    timelog = query.Where(i =>
                                        i.workdate >= starting_peroid && i.workdate <= EndingPeroid).ToList();
                                }

                                if (timelog != null && timelog.Count > 0)
                                {
                                    totaltime = totaltime + timelog.Sum(s => s.billabletimeinminutes.Value);
                                }
                            }
                        }

                        BillingViewModel bvm = new BillingViewModel
                        {
                            BillableTime = totaltime
                        };
                        //bvm.BillableTime = Math.Round((bvm.BillableTime / 60), 2);
                        bvm.BillableTime = CommonFunctions.RoundTwoDecimalPlaces(bvm.BillableTime / 60);
                        bvm.clientid = client.id;
                        bvm.clienttypeid = client.clienttypeid;
                        bvm.ClientName = client.name;
                        if (client.clienttypeid == 1)
                        {
                            bvm.maxbillablehours = client.maxbillablehours.Value;
                            //bvm.Percentageconsumed = (bvm.BillableTime / client.maxbillablehours.Value) * 100;
                            //bvm.Percentageconsumed = Math.Round(bvm.Percentageconsumed, 2);
                            bvm.Percentageconsumed =
                                CommonFunctions.RoundTwoDecimalPlaces(bvm.BillableTime / client.maxbillablehours.Value *
                                                                      100);
                        }

                        bvm.Billcyletype = cbc.BillingcyleType.name;
                        bvm.Billcyletypeid = cbc.BillingcyleType.Id;
                        bvm.isEndingPeroid = IsEnding;
                        bvmlist.Add(bvm);
                    }
                }

                return bvmlist;
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
                ViewBag.exception = ex.Message;
                List<BillingViewModel> bvm = new List<BillingViewModel>();
                return bvm;
            }
        }

        // GET: Billings/Create
        public ActionResult Create()
        {
            ViewBag.clientid = new SelectList(db.Client, "id", "name");
            return View();
        }

        // POST: Billings/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(
            [Bind(Include = "id,clientid,billabletime,workdate,createdonutc,updatedonutc,ipused,userid")]
            Billing billing)
        {
            if (ModelState.IsValid)
            {
                db.Billing.Add(billing);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.clientid = new SelectList(db.Client, "id", "name", billing.clientid);
            return View(billing);
        }

        // GET: Billings/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Billing billing = db.Billing.Find(id);
            if (billing == null)
            {
                return HttpNotFound();
            }

            ViewBag.clientid = new SelectList(db.Client, "id", "name", billing.clientid);
            return View(billing);
        }

        // POST: Billings/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(
            [Bind(Include = "id,clientid,billabletime,workdate,createdonutc,updatedonutc,ipused,userid")]
            Billing billing)
        {
            if (ModelState.IsValid)
            {
                db.Entry(billing).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.clientid = new SelectList(db.Client, "id", "name", billing.clientid);
            return View(billing);
        }

        // GET: Billings/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Billing billing = db.Billing.Find(id);
            if (billing == null)
            {
                return HttpNotFound();
            }

            return View(billing);
        }

        // POST: Billings/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            Billing billing = db.Billing.Find(id);
            db.Billing.Remove(billing);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }

            base.Dispose(disposing);
        }

        private void getsubprojects(long projectid)
        {
            List<Project> subprojects = db.Project.Where(p => p.parentid == projectid).ToList();
            if (subprojects == null && subprojects.Count > 0)
            {
                nthprojects.AddRange(subprojects);
                foreach (Project items in subprojects)
                {
                    getsubprojects(items.id);
                }
            }
        }

        private int GetWeekNumberOfMonth(DateTime date)
        {
            date = date.Date;
            DateTime firstMonthDay = new DateTime(date.Year, date.Month, 1);
            DateTime firstMonthMonday = firstMonthDay.AddDays((DayOfWeek.Monday + 7 - firstMonthDay.DayOfWeek) % 7);
            if (firstMonthMonday > date)
            {
                firstMonthDay = firstMonthDay.AddMonths(-1);
                firstMonthMonday = firstMonthDay.AddDays((DayOfWeek.Monday + 7 - firstMonthDay.DayOfWeek) % 7);
            }

            return (date - firstMonthMonday).Days / 7 + 1;
        }

        public ActionResult NotificationLimit()
        {
            NotificationLimitForBilling NLB = db.NotificationLimitForBilling.FirstOrDefault();
            if (NLB != null)
            {
                return View(NLB);
            }

            return View();
        }

        public ActionResult UpdateNotificationLimit(long id)
        {
            NotificationLimitForBilling NLB = db.NotificationLimitForBilling.FirstOrDefault();
            if (NLB != null)
            {
                return View(NLB);
            }

            return View();
        }

        [HttpPost]
        public ActionResult UpdateNotificationLimit(NotificationLimitForBilling model)
        {
            model.updatedonutc = DateTime.Now;
            model.ipused = Request.UserHostAddress;
            model.userid = User.Identity.GetUserId();
            db.Entry(model).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("NotificationLimit");
        }

        public string Email()
        {
            EmailTemplate EmailBody = db.EmailTemplate.FirstOrDefault();
            string body = EmailBody.body.Replace("{token}", "HIII");
            return body;
        }
    }
}