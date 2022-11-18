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
    public class CredentialSkillsController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        // GET: CredentialSkills
        public ActionResult Index()
        {
            IQueryable<CredentialSkills> credentialSkills = db.CredentialSkills.Include(c => c.Skill);
            return View(credentialSkills.ToList());
        }

        // GET: CredentialSkills/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            CredentialSkills credentialSkills = db.CredentialSkills.Find(id);
            if (credentialSkills == null)
            {
                return HttpNotFound();
            }

            return View(credentialSkills);
        }

        // GET: CredentialSkills/Create
        public ActionResult Create()
        {
            ViewBag.skillid = new SelectList(db.Skill, "id", "name");
            return View();
        }

        // POST: CredentialSkills/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(
            [Bind(Include = "id,skillid,credentailid,createdonutc,updatedonutc,ipused,userid")]
            CredentialSkills credentialSkills)
        {
            if (ModelState.IsValid)
            {
                credentialSkills.createdonutc = DateTime.Now;
                credentialSkills.updatedonutc = DateTime.Now;
                credentialSkills.ipused = Request.UserHostAddress;
                credentialSkills.userid = User.Identity.GetUserId();
                db.CredentialSkills.Add(credentialSkills);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.skillid = new SelectList(db.Skill, "id", "name", credentialSkills.skillid);
            return View(credentialSkills);
        }

        // GET: CredentialSkills/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            CredentialSkills credentialSkills = db.CredentialSkills.Find(id);
            if (credentialSkills == null)
            {
                return HttpNotFound();
            }

            ViewBag.skillid = new SelectList(db.Skill, "id", "name", credentialSkills.skillid);
            return View(credentialSkills);
        }

        // POST: CredentialSkills/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(
            [Bind(Include = "id,skillid,credentailid,createdonutc,updatedonutc,ipused,userid")]
            CredentialSkills credentialSkills)
        {
            if (ModelState.IsValid)
            {
                credentialSkills.updatedonutc = DateTime.Now;
                credentialSkills.ipused = Request.UserHostAddress;
                credentialSkills.userid = User.Identity.GetUserId();
                db.Entry(credentialSkills).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.skillid = new SelectList(db.Skill, "id", "name", credentialSkills.skillid);
            return View(credentialSkills);
        }

        // GET: CredentialSkills/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            CredentialSkills credentialSkills = db.CredentialSkills.Find(id);
            if (credentialSkills == null)
            {
                return HttpNotFound();
            }

            return View(credentialSkills);
        }

        // POST: CredentialSkills/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            CredentialSkills credentialSkills = db.CredentialSkills.Find(id);
            db.CredentialSkills.Remove(credentialSkills);
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