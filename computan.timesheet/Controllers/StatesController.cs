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
    public class StatesController : BaseController
    {
        // GET: States
        public ActionResult Index()
        {
            IQueryable<State> state = db.State.Include(s => s.Country);
            return View(state.ToList());
        }

        // GET: States/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            State state = db.State.Find(id);
            if (state == null)
            {
                return HttpNotFound();
            }

            return View(state);
        }

        // GET: States/Create
        public ActionResult Create()
        {
            ViewBag.countryid = new SelectList(db.Country, "id", "iso");
            return View();
        }

        // POST: States/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(
            [Bind(Include = "id,name,abbreviation,countryid,createdonutc,updatedonutc,ipused,userid")]
            State state)
        {
            State stat = db.State.Where(c => c.name == state.name).FirstOrDefault();
            if (stat != null)
            {
                ModelState.AddModelError("name", "Sorry, state name already exist.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    state.createdonutc = DateTime.Now;
                    state.updatedonutc = DateTime.Now;
                    state.ipused = Request.UserHostAddress;
                    state.userid = User.Identity.GetUserId();
                    db.State.Add(state);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                    ViewBag.countryid = new SelectList(db.Country, "id", "iso", state.countryid);
                    return View(state);
                }
            }

            ViewBag.countryid = new SelectList(db.Country, "id", "iso", state.countryid);
            return View(state);
        }

        // GET: States/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            State state = db.State.Find(id);
            if (state == null)
            {
                return HttpNotFound();
            }

            ViewBag.countryid = new SelectList(db.Country, "id", "iso", state.countryid);
            return View(state);
        }

        // POST: States/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(
            [Bind(Include = "id,name,abbreviation,countryid,createdonutc,updatedonutc,ipused,userid")]
            State state)
        {
            int stat = db.State.Where(c => c.name == state.name).Count();
            if (stat > 1)
            {
                ModelState.AddModelError("name", "Sorry, state name already exist.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    state.updatedonutc = DateTime.Now;
                    state.ipused = Request.UserHostAddress;
                    state.userid = User.Identity.GetUserId();
                    db.Entry(state).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                    ViewBag.countryid = new SelectList(db.Country, "id", "iso", state.countryid);
                    return View(state);
                }
            }

            ViewBag.countryid = new SelectList(db.Country, "id", "iso", state.countryid);
            return View(state);
        }

        // GET: States/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            State state = db.State.Find(id);
            if (state == null)
            {
                return HttpNotFound();
            }

            return View(state);
        }

        // POST: States/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            State state = db.State.Find(id);
            db.State.Remove(state);
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