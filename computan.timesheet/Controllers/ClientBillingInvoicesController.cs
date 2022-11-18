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
    public class ClientBillingInvoicesController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        // GET: ClientBillingInvoices
        public ActionResult Index()
        {
            IQueryable<ClientBillingInvoice> clientBillingInvoice = db.ClientBillingInvoice.Include(c => c.BillingcyleType).Include(c => c.Client);
            return View(clientBillingInvoice.ToList());
        }

        // GET: ClientBillingInvoices/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ClientBillingInvoice clientBillingInvoice = db.ClientBillingInvoice.Find(id);
            if (clientBillingInvoice == null)
            {
                return HttpNotFound();
            }

            return View(clientBillingInvoice);
        }

        // GET: ClientBillingInvoices/Create
        public ActionResult Create()
        {
            ViewBag.billingcyletypeid = new SelectList(db.BillingCyleType, "Id", "name");
            ViewBag.clientid = new SelectList(db.Client, "id", "name");
            return View();
        }

        // POST: ClientBillingInvoices/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(
            [Bind(Include =
                "Id,clientid,billingdate,hoursconsumed,ispaid,isapproved,billingcyletypeid,createdonutc,updatedonutc,ipused,userid")]
            ClientBillingInvoice clientBillingInvoice)
        {
            if (ModelState.IsValid)
            {
                clientBillingInvoice.createdonutc = DateTime.Now;
                clientBillingInvoice.updatedonutc = DateTime.Now;
                clientBillingInvoice.ipused = Request.UserHostAddress;
                clientBillingInvoice.userid = User.Identity.GetUserId();
                db.ClientBillingInvoice.Add(clientBillingInvoice);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.billingcyletypeid =
                new SelectList(db.BillingCyleType, "Id", "name", clientBillingInvoice.billingcyletypeid);
            ViewBag.clientid = new SelectList(db.Client, "id", "name", clientBillingInvoice.clientid);
            return View(clientBillingInvoice);
        }

        // GET: ClientBillingInvoices/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ClientBillingInvoice clientBillingInvoice = db.ClientBillingInvoice.Find(id);
            if (clientBillingInvoice == null)
            {
                return HttpNotFound();
            }

            ViewBag.billingcyletypeid =
                new SelectList(db.BillingCyleType, "Id", "name", clientBillingInvoice.billingcyletypeid);
            ViewBag.clientid = new SelectList(db.Client, "id", "name", clientBillingInvoice.clientid);
            return View(clientBillingInvoice);
        }

        // POST: ClientBillingInvoices/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(
            [Bind(Include =
                "Id,clientid,billingdate,hoursconsumed,ispaid,isapproved,billingcyletypeid,createdonutc,updatedonutc,ipused,userid")]
            ClientBillingInvoice clientBillingInvoice)
        {
            if (ModelState.IsValid)
            {
                clientBillingInvoice.updatedonutc = DateTime.Now;
                clientBillingInvoice.ipused = Request.UserHostAddress;
                clientBillingInvoice.userid = User.Identity.GetUserId();
                db.Entry(clientBillingInvoice).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.billingcyletypeid =
                new SelectList(db.BillingCyleType, "Id", "name", clientBillingInvoice.billingcyletypeid);
            ViewBag.clientid = new SelectList(db.Client, "id", "name", clientBillingInvoice.clientid);
            return View(clientBillingInvoice);
        }

        // GET: ClientBillingInvoices/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ClientBillingInvoice clientBillingInvoice = db.ClientBillingInvoice.Find(id);
            if (clientBillingInvoice == null)
            {
                return HttpNotFound();
            }

            return View(clientBillingInvoice);
        }

        // POST: ClientBillingInvoices/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            ClientBillingInvoice clientBillingInvoice = db.ClientBillingInvoice.Find(id);
            db.ClientBillingInvoice.Remove(clientBillingInvoice);
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