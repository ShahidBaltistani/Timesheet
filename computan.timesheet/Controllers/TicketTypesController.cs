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
    public class TicketTypesController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        // GET: TicketTypes
        public ActionResult Index()
        {
            return View(db.TicketTypes.ToList());
        }

        // GET: TicketTypes/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            TicketType ticketType = db.TicketTypes.Find(id);
            if (ticketType == null)
            {
                return HttpNotFound();
            }

            return View(ticketType);
        }

        // GET: TicketTypes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TicketTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(
            [Bind(Include = "id,name,createdonutc,updatedonutc,ipused,userid")]
            TicketType ticketType)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    ticketType.name = ticketType.name.Trim();
                    ticketType.createdonutc = DateTime.Now;
                    ticketType.updatedonutc = DateTime.Now;
                    ticketType.ipused = Request.UserHostAddress;
                    ticketType.userid = User.Identity.GetUserId();
                    db.TicketTypes.Add(ticketType);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                return View(ticketType);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(ticketType);
            }
        }

        // GET: TicketTypes/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            TicketType ticketType = db.TicketTypes.Find(id);
            if (ticketType == null)
            {
                return HttpNotFound();
            }

            return View(ticketType);
        }

        // POST: TicketTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(
            [Bind(Include = "id,name,createdonutc,updatedonutc,ipused,userid")]
            TicketType ticketType)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    ticketType.name = ticketType.name.Trim();
                    ticketType.updatedonutc = DateTime.Now;
                    ticketType.ipused = Request.UserHostAddress;
                    ticketType.userid = User.Identity.GetUserId();
                    db.Entry(ticketType).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                return View(ticketType);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(ticketType);
            }
        }

        // GET: TicketTypes/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            TicketType ticketType = db.TicketTypes.Find(id);
            if (ticketType == null)
            {
                return HttpNotFound();
            }

            return View(ticketType);
        }

        // POST: TicketTypes/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            TicketType ticketType = db.TicketTypes.Find(id);
            db.TicketTypes.Remove(ticketType);
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