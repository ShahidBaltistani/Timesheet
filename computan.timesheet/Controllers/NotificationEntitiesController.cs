using computan.timesheet.core;
using computan.timesheet.core.common;
using computan.timesheet.Helpers;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace computan.timesheet.Controllers
{
    [CustomeAuthorizeAttribute(Role.Admin)]
    public class NotificationEntitiesController : BaseController
    {
        // GET: NotificationEntities
        public ActionResult Index()
        {
            return View(db.NotificationEntity.ToList());
        }

        // GET: NotificationEntities/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            NotificationEntity notificationEntity = db.NotificationEntity.Find(id);
            if (notificationEntity == null)
            {
                return HttpNotFound();
            }

            return View(notificationEntity);
        }

        // GET: NotificationEntities/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: NotificationEntities/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,name,isActive")] NotificationEntity notificationEntity)
        {
            if (ModelState.IsValid)
            {
                db.NotificationEntity.Add(notificationEntity);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(notificationEntity);
        }

        // GET: NotificationEntities/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            NotificationEntity notificationEntity = db.NotificationEntity.Find(id);
            if (notificationEntity == null)
            {
                return HttpNotFound();
            }

            return View(notificationEntity);
        }

        // POST: NotificationEntities/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,name,isActive")] NotificationEntity notificationEntity)
        {
            if (ModelState.IsValid)
            {
                db.Entry(notificationEntity).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(notificationEntity);
        }

        // GET: NotificationEntities/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            NotificationEntity notificationEntity = db.NotificationEntity.Find(id);
            if (notificationEntity == null)
            {
                return HttpNotFound();
            }

            return View(notificationEntity);
        }

        // POST: NotificationEntities/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            NotificationEntity notificationEntity = db.NotificationEntity.Find(id);
            db.NotificationEntity.Remove(notificationEntity);
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