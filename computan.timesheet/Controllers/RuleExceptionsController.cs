using computan.timesheet.Contexts;
using computan.timesheet.core;
using computan.timesheet.Extensions;
using computan.timesheet.Helpers;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace computan.timesheet.Controllers
{
    [CustomeAuthorizeAttribute]
    public class RuleExceptionsController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        // GET: RuleExceptions
        public ActionResult Index()
        {
            IQueryable<RuleException> ruleException = db.RuleException.Include(r => r.Rule).Include(r => r.RuleConditionType);
            return View(ruleException.ToList());
        }

        // GET: RuleExceptions/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            RuleException ruleException = db.RuleException.Find(id);
            if (ruleException == null)
            {
                return HttpNotFound();
            }

            return View(ruleException);
        }

        // GET: RuleExceptions/Create
        public ActionResult Create()
        {
            ViewBag.ruleid = new SelectList(db.Rule, "id", "name");
            ViewBag.ruleexceptiontypeid = new SelectList(db.RuleExceptionType, "id", "name");
            return View();
        }

        // POST: RuleExceptions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Create(
            [Bind(Include = "id,ruleid,ruleexceptiontypeid,ruleexceptionvalue,isrequired")]
            RuleException ruleException)
        {
            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(ruleException.ruleexceptionvalue))
                {
                    ruleException.ruleexceptionvalue = ruleException.ruleexceptionvalue.Trim();
                }

                db.RuleException.Add(ruleException);
                db.SaveChanges();
                System.Collections.Generic.List<RuleException> exceptionlist = db.RuleException.Where(rc => rc.ruleid == ruleException.ruleid)
                    .Include(rt => rt.RuleConditionType).ToList();
                string ruleexceptionlist =
                    PartialView("~/Views/Rules/_RuleExceptions.cshtml", exceptionlist).RenderToString();
                return Json(new
                {
                    success = true,
                    response = "Exception has been added successfully",
                    exceptionlist = ruleexceptionlist
                });
            }

            ViewBag.ruleid = new SelectList(db.Rule, "id", "name", ruleException.ruleid);
            ViewBag.ruleexceptiontypeid =
                new SelectList(db.RuleExceptionType, "id", "name", ruleException.ruleexceptiontypeid);
            return View(ruleException);
        }

        // GET: RuleExceptions/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            RuleException ruleException = db.RuleException.Find(id);
            if (ruleException == null)
            {
                return HttpNotFound();
            }

            ViewBag.ruleid = new SelectList(db.Rule, "id", "name", ruleException.ruleid);
            ViewBag.ruleexceptiontypeid =
                new SelectList(db.RuleExceptionType, "id", "name", ruleException.ruleexceptiontypeid);
            return PartialView("~/Views/Rules/_NewRuleException.cshtml", ruleException);
        }

        // POST: RuleExceptions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Edit(
            [Bind(Include = "id,ruleid,ruleexceptiontypeid,ruleexceptionvalue,isrequired")]
            RuleException ruleException)
        {
            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(ruleException.ruleexceptionvalue))
                {
                    ruleException.ruleexceptionvalue = ruleException.ruleexceptionvalue.Trim();
                }

                db.Entry(ruleException).State = EntityState.Modified;
                db.SaveChanges();
                System.Collections.Generic.List<RuleException> exceptionlist = db.RuleException.Where(rc => rc.ruleid == ruleException.ruleid)
                    .Include(rt => rt.RuleConditionType).ToList();
                string ruleexceptionlist =
                    PartialView("~/Views/Rules/_RuleExceptions.cshtml", exceptionlist).RenderToString();
                return Json(new
                {
                    success = true,
                    response = "Exception has been updated successfully",
                    exceptionlist = ruleexceptionlist
                });
            }

            ViewBag.ruleid = new SelectList(db.Rule, "id", "name", ruleException.ruleid);
            ViewBag.ruleexceptiontypeid =
                new SelectList(db.RuleExceptionType, "id", "name", ruleException.ruleexceptiontypeid);
            return View(ruleException);
        }

        // GET: RuleExceptions/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            RuleException ruleException = db.RuleException.Find(id);
            if (ruleException == null)
            {
                return HttpNotFound();
            }

            return View(ruleException);
        }

        // POST: RuleExceptions/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        public ActionResult DeleteConfirmed(long id)
        {
            RuleException ruleException = db.RuleException.Find(id);
            db.RuleException.Remove(ruleException);
            db.SaveChanges();
            return Json(new { success = true });
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