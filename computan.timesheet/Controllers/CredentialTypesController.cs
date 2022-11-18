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
    public class CredentialTypesController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        // GET: CredentialTypes
        public ActionResult Index()
        {
            return View(db.CredentialTypes.ToList());
        }

        // GET: CredentialTypes/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            CredentialType credentialType = db.CredentialTypes.Find(id);
            if (credentialType == null)
            {
                return HttpNotFound();
            }

            return View(credentialType);
        }

        // GET: CredentialTypes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CredentialTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(
            [Bind(Include = "id,name,isactive,createdonutc,updatedonutc,ipused,userid")]
            CredentialType credentialType)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    credentialType.name = credentialType.name.Trim();
                    credentialType.createdonutc = DateTime.Now;
                    credentialType.userid = User.Identity.GetUserId();
                    credentialType.updatedonutc = DateTime.Now;
                    credentialType.ipused = Request.UserHostAddress;
                    db.CredentialTypes.Add(credentialType);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                return View(credentialType);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(credentialType);
            }
        }

        // GET: CredentialTypes/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            CredentialType credentialType = db.CredentialTypes.Find(id);
            if (credentialType == null)
            {
                return HttpNotFound();
            }

            return View(credentialType);
        }

        // POST: CredentialTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(
            [Bind(Include = "id,name,isactive,createdonutc,updatedonutc,ipused,userid")]
            CredentialType credentialType)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    credentialType.name = credentialType.name.Trim();
                    credentialType.userid = User.Identity.GetUserId();
                    credentialType.updatedonutc = DateTime.Now;
                    credentialType.ipused = Request.UserHostAddress;
                    db.Entry(credentialType).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                return View(credentialType);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(credentialType);
            }
        }

        // GET: CredentialTypes/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            CredentialType credentialType = db.CredentialTypes.Find(id);
            if (credentialType == null)
            {
                return HttpNotFound();
            }

            return View(credentialType);
        }

        // POST: CredentialTypes/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            CredentialType credentialType = db.CredentialTypes.Find(id);
            db.CredentialTypes.Remove(credentialType);
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