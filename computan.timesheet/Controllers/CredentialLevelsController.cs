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
    public class CredentialLevelsController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        // GET: CredentialLevels
        public ActionResult Index()
        {
            return View(db.CredentialLevels.ToList());
        }

        // GET: CredentialLevels/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            CredentialLevel credentialLevel = db.CredentialLevels.Find(id);
            if (credentialLevel == null)
            {
                return HttpNotFound();
            }

            return View(credentialLevel);
        }

        // GET: CredentialLevels/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CredentialLevels/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(
            [Bind(Include = "id,name,LevelNumber,isactive,createdonutc,updatedonutc,ipused,userid")]
            CredentialLevel credentialLevel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    credentialLevel.name = credentialLevel.name.Trim();
                    credentialLevel.userid = User.Identity.GetUserId();
                    credentialLevel.ipused = Request.UserHostAddress;
                    credentialLevel.createdonutc = DateTime.Now;
                    credentialLevel.updatedonutc = DateTime.Now;
                    db.CredentialLevels.Add(credentialLevel);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                return View(credentialLevel);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(credentialLevel);
            }
        }

        // GET: CredentialLevels/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            CredentialLevel credentialLevel = db.CredentialLevels.Find(id);
            if (credentialLevel == null)
            {
                return HttpNotFound();
            }

            return View(credentialLevel);
        }

        // POST: CredentialLevels/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(
            [Bind(Include = "id,name,LevelNumber,isactive,createdonutc,updatedonutc,ipused,userid")]
            CredentialLevel credentialLevel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    credentialLevel.name = credentialLevel.name.Trim();
                    credentialLevel.userid = User.Identity.GetUserId();
                    credentialLevel.ipused = Request.UserHostAddress;
                    credentialLevel.updatedonutc = DateTime.Now;
                    db.Entry(credentialLevel).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                return View(credentialLevel);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(credentialLevel);
            }
        }

        // GET: CredentialLevels/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            CredentialLevel credentialLevel = db.CredentialLevels.Find(id);
            if (credentialLevel == null)
            {
                return HttpNotFound();
            }

            return View(credentialLevel);
        }

        // POST: CredentialLevels/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            CredentialLevel credentialLevel = db.CredentialLevels.Find(id);
            db.CredentialLevels.Remove(credentialLevel);
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