using computan.timesheet.Contexts;
using computan.timesheet.core;
using computan.timesheet.Helpers;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace computan.timesheet.Controllers
{
    [CustomeAuthorizeAttribute]
    public class NotificationEntityActionsController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        // GET: NotificationEntityActions
        public ActionResult Index(long id)
        {
            IQueryable<NotificationEntityAction> notificationAction = db.NotificationAction.Include(n => n.notificationentity)
                .Where(na => na.entityid == id && na.isActive == true);
            return View(notificationAction.ToList());
        }

        // GET: NotificationEntityActions/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            NotificationEntityAction notificationEntityAction = db.NotificationAction.Find(id);
            if (notificationEntityAction == null)
            {
                return HttpNotFound();
            }

            return View(notificationEntityAction);
        }

        // GET: NotificationEntityActions/Create
        public ActionResult Create(long id)
        {
            ViewBag.actentityid = id;
            ViewBag.entityid = new SelectList(db.NotificationEntity, "id", "name", id);
            return View();
        }

        // POST: NotificationEntityActions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(
            [Bind(Include = "id,name,entityid,isActive")]
            NotificationEntityAction notificationEntityAction)
        {
            if (ModelState.IsValid)
            {
                db.NotificationAction.Add(notificationEntityAction);
                db.SaveChanges();
                return RedirectToAction("Index", new { id = notificationEntityAction.entityid });
            }

            ViewBag.entityid = new SelectList(db.NotificationEntity, "id", "name", notificationEntityAction.entityid);
            return View(notificationEntityAction);
        }

        // GET: NotificationEntityActions/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            NotificationEntityAction notificationEntityAction = db.NotificationAction.Find(id);
            if (notificationEntityAction == null)
            {
                return HttpNotFound();
            }

            ViewBag.entityid = new SelectList(db.NotificationEntity, "id", "name", notificationEntityAction.entityid);
            return View(notificationEntityAction);
        }

        // POST: NotificationEntityActions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(
            [Bind(Include = "id,name,entityid,isActive")]
            NotificationEntityAction notificationEntityAction)
        {
            if (ModelState.IsValid)
            {
                db.Entry(notificationEntityAction).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", new { id = notificationEntityAction.entityid });
            }

            ViewBag.entityid = new SelectList(db.NotificationEntity, "id", "name", notificationEntityAction.entityid);
            return View(notificationEntityAction);
        }

        // GET: NotificationEntityActions/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            NotificationEntityAction notificationEntityAction = db.NotificationAction.Find(id);
            if (notificationEntityAction == null)
            {
                return HttpNotFound();
            }

            return View(notificationEntityAction);
        }

        // POST: NotificationEntityActions/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            NotificationEntityAction notificationEntityAction = db.NotificationAction.Find(id);
            db.NotificationAction.Remove(notificationEntityAction);
            db.SaveChanges();
            return RedirectToAction("Index", new { id = notificationEntityAction.entityid });
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