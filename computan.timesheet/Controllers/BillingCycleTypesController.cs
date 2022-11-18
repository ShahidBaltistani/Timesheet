using computan.timesheet.Contexts;
using computan.timesheet.core;
using computan.timesheet.Helpers;
using Microsoft.AspNet.Identity;
using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace computan.timesheet.Controllers
{
    [CustomeAuthorizeAttribute]
    public class BillingCycleTypesController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        // GET: BillingCyleTypes
        public ActionResult Index()
        {
            return View(db.BillingCyleType.ToList());
        }

        // GET: BillingCyleTypes/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            BillingCycleType billingCyleType = db.BillingCyleType.Find(id);
            if (billingCyleType == null)
            {
                return HttpNotFound();
            }

            return View(billingCyleType);
        }

        // GET: BillingCyleTypes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: BillingCyleTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(
            [Bind(Include = "Id,name,isactive,createdonutc,updatedonutc,ipused,userid")]
            BillingCycleType billingCyleType)
        {
            if (ModelState.IsValid)
            {
                billingCyleType.createdonutc = DateTime.Now;
                billingCyleType.updatedonutc = DateTime.Now;
                billingCyleType.ipused = Request.UserHostAddress;
                billingCyleType.userid = User.Identity.GetUserId();
                db.BillingCyleType.Add(billingCyleType);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(billingCyleType);
        }

        // GET: BillingCyleTypes/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            BillingCycleType billingCyleType = db.BillingCyleType.Find(id);
            if (billingCyleType == null)
            {
                return HttpNotFound();
            }

            return View(billingCyleType);
        }

        // POST: BillingCyleTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(
            [Bind(Include = "Id,name,isactive,createdonutc,updatedonutc,ipused,userid")]
            BillingCycleType billingCyleType)
        {
            if (ModelState.IsValid)
            {
                billingCyleType.updatedonutc = DateTime.Now;
                billingCyleType.ipused = Request.UserHostAddress;
                billingCyleType.userid = User.Identity.GetUserId();
                db.Entry(billingCyleType).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(billingCyleType);
        }

        // GET: BillingCyleTypes/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            BillingCycleType billingCyleType = db.BillingCyleType.Find(id);
            if (billingCyleType == null)
            {
                return HttpNotFound();
            }

            return View(billingCyleType);
        }

        // POST: BillingCyleTypes/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            BillingCycleType billingCyleType = db.BillingCyleType.Find(id);
            db.BillingCyleType.Remove(billingCyleType);
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