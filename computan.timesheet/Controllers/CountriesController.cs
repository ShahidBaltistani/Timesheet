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
    public class CountriesController : BaseController
    {
        // GET: Countries
        public ActionResult Index()
        {
            return View(db.Country.ToList());
        }

        // GET: Countries/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Country country = db.Country.Find(id);
            if (country == null)
            {
                return HttpNotFound();
            }

            return View(country);
        }

        // GET: Countries/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Countries/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(
            [Bind(Include = "id,iso,iso3,name,nicename,numcode,phonecode,createdonutc,updatedonutc,ipused,userid")]
            Country country)
        {
            // Validate if Entity is not duplicate.
            Country ecountry = db.Country.Where(c => c.name == country.name).FirstOrDefault();
            if (ecountry != null)
            {
                ModelState.AddModelError("name", "Sorry, country name already exist.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    country.createdonutc = DateTime.Now;
                    country.updatedonutc = DateTime.Now;
                    country.ipused = Request.UserHostAddress;
                    country.userid = User.Identity.GetUserId();
                    db.Country.Add(country);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                    return View(country);
                }
            }

            return View(country);
        }

        // GET: Countries/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Country country = db.Country.Find(id);
            if (country == null)
            {
                return HttpNotFound();
            }

            return View(country);
        }

        // POST: Countries/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(
            [Bind(Include = "id,iso,iso3,name,nicename,numcode,phonecode,createdonutc,updatedonutc,ipused,userid")]
            Country country)
        {
            int ecountry = db.Country.Where(c => c.name == country.name).Count();
            if (ecountry > 1)
            {
                ModelState.AddModelError("name", "Sorry, country name already exist.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    country.updatedonutc = DateTime.Now;
                    country.ipused = Request.UserHostAddress;
                    country.userid = User.Identity.GetUserId();
                    db.Entry(country).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                    return View(country);
                }
            }

            return View(country);
        }

        // GET: Countries/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Country country = db.Country.Find(id);
            if (country == null)
            {
                return HttpNotFound();
            }

            return View(country);
        }

        // POST: Countries/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            Country country = db.Country.Find(id);
            db.Country.Remove(country);
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