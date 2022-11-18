using computan.timesheet.Contexts;
using computan.timesheet.core;
using computan.timesheet.Helpers;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace computan.timesheet.Controllers
{
    [CustomeAuthorizeAttribute]
    public class ClientBillingCyclesController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        // GET: ClientBillingCycles
        public ActionResult Index()
        {
            IQueryable<ClientBillingCycle> clientBillingCycle = db.ClientBillingCycle.Include(c => c.BillingcyleType).Include(c => c.Client);
            return View(clientBillingCycle.ToList());
        }

        // GET: ClientBillingCycles/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ClientBillingCycle clientBillingCycle = db.ClientBillingCycle.Find(id);
            if (clientBillingCycle == null)
            {
                return HttpNotFound();
            }

            return View(clientBillingCycle);
        }

        // GET: ClientBillingCycles/Create
        public ActionResult Create()
        {
            ViewBag.billingcyletypeid = new SelectList(db.BillingCyleType, "Id", "name");
            ViewBag.clientid = new SelectList(db.Client, "id", "name");
            List<int> dates = new List<int>();
            for (int i = 1; i <= 30; i++)
            {
                dates.Add(i);
            }

            SelectList weekdays = new SelectList(new[]
            {
                new SelectListItem {Text = "Monday", Value = "1"},
                new SelectListItem {Text = "Tuesday", Value = "2"},
                new SelectListItem {Text = "Wednesday", Value = "3"},
                new SelectListItem {Text = "Thursday", Value = "4"},
                new SelectListItem {Text = "Friday", Value = "5"},
                new SelectListItem {Text = "Saturday", Value = "6"},
                new SelectListItem {Text = "Sunday", Value = "7"}
            }, "Value", "Text");
            ViewBag.dates = new SelectList(dates);
            ViewBag.days = weekdays;
            return View();
        }

        // POST: ClientBillingCycles/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(
            [Bind(Include = "Id,clientid,billingcyletypeid,day,date,createdonutc,updatedonutc,ipused,userid")]
            ClientBillingCycle clientBillingCycle, int? date)
        {
            if (ModelState.IsValid)
            {
                if (date != null)
                {
                    clientBillingCycle.date = date;
                }

                clientBillingCycle.createdonutc = DateTime.Now;
                clientBillingCycle.updatedonutc = DateTime.Now;
                clientBillingCycle.ipused = Request.UserHostAddress;
                clientBillingCycle.userid = User.Identity.GetUserId();
                db.ClientBillingCycle.Add(clientBillingCycle);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.billingcyletypeid =
                new SelectList(db.BillingCyleType, "Id", "name", clientBillingCycle.billingcyletypeid);
            ViewBag.clientid = new SelectList(db.Client, "id", "name", clientBillingCycle.clientid);
            List<int> dates = new List<int>();
            for (int i = 1; i <= 30; i++)
            {
                dates.Add(i);
            }

            SelectList weekdays = new SelectList(new[]
            {
                new SelectListItem {Text = "Monday", Value = "1"},
                new SelectListItem {Text = "Tuesday", Value = "2"},
                new SelectListItem {Text = "Wednesday", Value = "3"},
                new SelectListItem {Text = "Thursday", Value = "4"},
                new SelectListItem {Text = "Friday", Value = "5"},
                new SelectListItem {Text = "Saturday", Value = "6"},
                new SelectListItem {Text = "Sunday", Value = "7"}
            }, "Value", "Text");
            ViewBag.dates = new SelectList(dates);
            ViewBag.days = weekdays;
            return View(clientBillingCycle);
        }

        // GET: ClientBillingCycles/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ClientBillingCycle clientBillingCycle = db.ClientBillingCycle.Find(id);
            if (clientBillingCycle == null)
            {
                return HttpNotFound();
            }

            ViewBag.billingcyletypeid =
                new SelectList(db.BillingCyleType, "Id", "name", clientBillingCycle.billingcyletypeid);
            ViewBag.clientid = new SelectList(db.Client, "id", "name", clientBillingCycle.clientid);
            List<int> dates = new List<int>();
            for (int i = 1; i <= 30; i++)
            {
                dates.Add(i);
            }

            SelectList weekdays = new SelectList(new[]
            {
                new SelectListItem {Text = "Monday", Value = "1"},
                new SelectListItem {Text = "Tuesday", Value = "2"},
                new SelectListItem {Text = "Wednesday", Value = "3"},
                new SelectListItem {Text = "Thursday", Value = "4"},
                new SelectListItem {Text = "Friday", Value = "5"},
                new SelectListItem {Text = "Saturday", Value = "6"},
                new SelectListItem {Text = "Sunday", Value = "7"}
            }, "Value", "Text");
            ViewBag.dates = new SelectList(dates);
            ViewBag.days = weekdays;
            return View(clientBillingCycle);
        }

        // POST: ClientBillingCycles/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(
            [Bind(Include = "Id,clientid,billingcyletypeid,day,date,createdonutc,updatedonutc,ipused,userid")]
            ClientBillingCycle clientBillingCycle)
        {
            if (ModelState.IsValid)
            {
                clientBillingCycle.updatedonutc = DateTime.Now;
                clientBillingCycle.ipused = Request.UserHostAddress;
                clientBillingCycle.userid = User.Identity.GetUserId();
                db.Entry(clientBillingCycle).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.billingcyletypeid =
                new SelectList(db.BillingCyleType, "Id", "name", clientBillingCycle.billingcyletypeid);
            ViewBag.clientid = new SelectList(db.Client, "id", "name", clientBillingCycle.clientid);
            List<int> dates = new List<int>();
            for (int i = 1; i <= 30; i++)
            {
                dates.Add(i);
            }

            SelectList weekdays = new SelectList(new[]
            {
                new SelectListItem {Text = "Monday", Value = "1"},
                new SelectListItem {Text = "Tuesday", Value = "2"},
                new SelectListItem {Text = "Wednesday", Value = "3"},
                new SelectListItem {Text = "Thursday", Value = "4"},
                new SelectListItem {Text = "Friday", Value = "5"},
                new SelectListItem {Text = "Saturday", Value = "6"},
                new SelectListItem {Text = "Sunday", Value = "7"}
            }, "Value", "Text");
            ViewBag.dates = new SelectList(dates);
            ViewBag.days = weekdays;
            return View(clientBillingCycle);
        }

        // GET: ClientBillingCycles/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ClientBillingCycle clientBillingCycle = db.ClientBillingCycle.Find(id);
            if (clientBillingCycle == null)
            {
                return HttpNotFound();
            }

            return View(clientBillingCycle);
        }

        // POST: ClientBillingCycles/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            ClientBillingCycle clientBillingCycle = db.ClientBillingCycle.Find(id);
            db.ClientBillingCycle.Remove(clientBillingCycle);
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
    }
}