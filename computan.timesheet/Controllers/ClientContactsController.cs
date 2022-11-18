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
    [CustomeAuthorize(Role.Admin)]
    public class ClientContactsController : BaseController
    {
        // GET: ClientContacts
        public ActionResult Index(long id)
        {
            return View(db.ClientContact.Where(a => a.clientid == id));
        }

        // GET: ClientContacts/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ClientContact clientContact = db.ClientContact.Find(id);
            if (clientContact == null)
            {
                return HttpNotFound();
            }

            return View(clientContact);
        }

        // GET: ClientContacts/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ClientContacts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "clientid,name,email,title,isactive")] ClientContact clientContact)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    clientContact.createdonutc = DateTime.Now;
                    clientContact.updatedonutc = DateTime.Now;
                    clientContact.ipused = Request.UserHostAddress;
                    clientContact.userid = User.Identity.GetUserId();
                    db.ClientContact.Add(clientContact);
                    db.SaveChanges();
                    return RedirectToAction("Index", new { id = clientContact.clientid });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                    return View(clientContact);
                }
            }

            return View(clientContact);
        }

        // GET: ClientContacts/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ClientContact clientContact = db.ClientContact.Find(id);
            if (clientContact == null)
            {
                return HttpNotFound();
            }

            return View(clientContact);
        }

        // POST: ClientContacts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,name,email,title,isactive")] ClientContact clientContact)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    ClientContact clientContacts = db.ClientContact.Find(clientContact.id);
                    clientContacts.updatedonutc = DateTime.Now;
                    clientContacts.ipused = Request.UserHostAddress;
                    clientContacts.userid = User.Identity.GetUserId();
                    clientContacts.name = clientContact.name;
                    clientContacts.email = clientContact.email;
                    clientContacts.title = clientContact.title;
                    clientContacts.isactive = clientContact.isactive;
                    db.Entry(clientContacts).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index", new { id = clientContacts.clientid });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                    return View(clientContact);
                }
            }

            return View(clientContact);
        }

        // GET: ClientContacts/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ClientContact clientContact = db.ClientContact.Find(id);
            if (clientContact == null)
            {
                return HttpNotFound();
            }

            return View(clientContact);
        }

        // POST: ClientContacts/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            ClientContact clientContact = db.ClientContact.Find(id);
            db.ClientContact.Remove(clientContact);
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