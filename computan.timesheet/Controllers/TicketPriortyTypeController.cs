using computan.timesheet.core;
using computan.timesheet.core.common;
using computan.timesheet.Helpers;
using Microsoft.AspNet.Identity;
using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace computan.timesheet.Controllers
{
    [CustomeAuthorizeAttribute(Role.Admin)]
    public class TicketPriortyTypeController : BaseController
    {
        // GET: TicketPriortyType
        public ActionResult Index()
        {
            return View(db.TicketPriorty.ToList());
        }

        public ActionResult Detail(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            TicketPriorty tpriorty = db.TicketPriorty.Find(id);
            if (tpriorty == null)
            {
                return HttpNotFound();
            }

            return View(tpriorty);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(
            [Bind(Include = "id,name,isactive,createdonutc,updatedonutc,ipused,userid")]
            TicketPriorty ticketpriorty)
        {
            TicketPriorty tickpriorty = db.TicketPriorty.Where(c => c.name == ticketpriorty.name).FirstOrDefault();
            if (tickpriorty != null)
            {
                ModelState.AddModelError("name", "Sorry, priorty Name Already exist.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    ticketpriorty.createdonutc = DateTime.Now;
                    ticketpriorty.updatedonutc = DateTime.Now;
                    ticketpriorty.ipused = Request.UserHostAddress;
                    ticketpriorty.userid = User.Identity.GetUserId();
                    db.TicketPriorty.Add(ticketpriorty);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                    return View(ticketpriorty);
                }
            }

            return View(ticketpriorty);
        }

        // GET: TciketPriortyType/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            TicketPriorty tprioty = db.TicketPriorty.Find(id);
            if (tprioty == null)
            {
                return HttpNotFound();
            }

            return View(tprioty);
        }

        // POST: TicketPriorty/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(
            [Bind(Include = "id,name,isactive,createdonutc,updatedonutc,ipused,userid")]
            TicketPriorty tpriorty)
        {
            int ticpriorty = db.TicketPriorty.Where(c => c.name == tpriorty.name).Count();
            if (ticpriorty > 1)
            {
                ModelState.AddModelError("name", "Sorry, Ticket Priorty Name already exist.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    tpriorty.updatedonutc = DateTime.Now;
                    tpriorty.ipused = Request.UserHostAddress;
                    tpriorty.userid = User.Identity.GetUserId();
                    db.Entry(tpriorty).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                    return View(tpriorty);
                }
            }

            return View(tpriorty);
        }

        // GET: Skills/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            TicketPriorty tpriorty = db.TicketPriorty.Find(id);
            if (tpriorty == null)
            {
                return HttpNotFound();
            }

            return View(tpriorty);
        }

        // POST: Skills/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            TicketPriorty tpriorty = db.TicketPriorty.Find(id);
            db.TicketPriorty.Remove(tpriorty);
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