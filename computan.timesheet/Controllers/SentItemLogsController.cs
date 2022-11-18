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
    public class SentItemLogsController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        // GET: SentItemLogs
        public ActionResult Index()
        {
            string userid = User.Identity.GetUserId();
            return View(db.SentItemLog.Where(x => x.userid == userid).ToList());
        }

        // GET: SentItemLogs/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            SentItemLog sentItemLog = db.SentItemLog.Find(id);
            if (sentItemLog == null)
            {
                return HttpNotFound();
            }

            return View(sentItemLog);
        }

        // GET: SentItemLogs/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: SentItemLogs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(
            [Bind(Include =
                "id,ticketId,ticket_title,To,Cc,Bcc,subject,body,attachments,IsSent,createdonutc,updatedonutc,ipused,userid")]
            SentItemLog sentItemLog)
        {
            if (ModelState.IsValid)
            {
                db.SentItemLog.Add(sentItemLog);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(sentItemLog);
        }

        // GET: SentItemLogs/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            SentItemLog sentItemLog = db.SentItemLog.Find(id);
            if (sentItemLog == null)
            {
                return HttpNotFound();
            }

            return View(sentItemLog);
        }

        // POST: SentItemLogs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(
            [Bind(Include =
                "id,ticketId,ticket_title,To,Cc,Bcc,subject,body,attachments,IsSent,createdonutc,updatedonutc,ipused,userid")]
            SentItemLog sentItemLog)
        {
            if (ModelState.IsValid)
            {
                db.Entry(sentItemLog).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(sentItemLog);
        }

        // GET: SentItemLogs/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            SentItemLog sentItemLog = db.SentItemLog.Find(id);
            if (sentItemLog == null)
            {
                return HttpNotFound();
            }

            return View(sentItemLog);
        }

        // POST: SentItemLogs/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            SentItemLog sentItemLog = db.SentItemLog.Find(id);
            db.SentItemLog.Remove(sentItemLog);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult DeleteSentItemLogs(long id)
        {
            try
            {
                SentItemLog sentItemLog = db.SentItemLog.Find(id);
                db.SentItemLog.Remove(sentItemLog);
                db.SaveChanges();
                return Json(new { error = false, response = "Deleted successfully!" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = true, response = ex.Message }, JsonRequestBehavior.AllowGet);
            }
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