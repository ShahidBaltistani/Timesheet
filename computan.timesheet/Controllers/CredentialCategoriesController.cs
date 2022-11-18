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
    public class CredentialCategoriesController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        // GET: CredentialCategories
        public ActionResult Index()
        {
            return View(db.CredentialCategories.ToList());
        }

        // GET: CredentialCategories/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            CredentialCategory credentialCategory = db.CredentialCategories.Find(id);
            if (credentialCategory == null)
            {
                return HttpNotFound();
            }

            return View(credentialCategory);
        }

        // GET: CredentialCategories/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CredentialCategories/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(
            [Bind(Include = "id,name,isactive,createdonutc,updatedonutc,ipused,userid")]
            CredentialCategory credentialCategory)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    credentialCategory.name = credentialCategory.name.Trim();
                    credentialCategory.createdonutc = DateTime.Now;
                    credentialCategory.updatedonutc = DateTime.Now;
                    credentialCategory.ipused = Request.UserHostAddress;
                    credentialCategory.userid = User.Identity.GetUserId();
                    db.CredentialCategories.Add(credentialCategory);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                return View(credentialCategory);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(credentialCategory);
            }
        }

        // GET: CredentialCategories/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            CredentialCategory credentialCategory = db.CredentialCategories.Find(id);
            if (credentialCategory == null)
            {
                return HttpNotFound();
            }

            return View(credentialCategory);
        }

        // POST: CredentialCategories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(
            [Bind(Include = "id,name,isactive,createdonutc,updatedonutc,ipused,userid")]
            CredentialCategory credentialCategory)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    credentialCategory.name = credentialCategory.name.Trim();
                    credentialCategory.updatedonutc = DateTime.Now;
                    credentialCategory.ipused = Request.UserHostAddress;
                    credentialCategory.userid = User.Identity.GetUserId();
                    db.Entry(credentialCategory).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                return View(credentialCategory);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(credentialCategory);
            }
        }

        // GET: CredentialCategories/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            CredentialCategory credentialCategory = db.CredentialCategories.Find(id);
            if (credentialCategory == null)
            {
                return HttpNotFound();
            }

            return View(credentialCategory);
        }

        // POST: CredentialCategories/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            CredentialCategory credentialCategory = db.CredentialCategories.Find(id);
            db.CredentialCategories.Remove(credentialCategory);
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