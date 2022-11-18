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
    public class RuleConditionsController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        // GET: RuleConditions
        public ActionResult Index()
        {
            IQueryable<RuleCondition> ruleCondition = db.RuleCondition.Include(r => r.Rule).Include(r => r.RuleConditionType);
            return View(ruleCondition.ToList());
        }

        // GET: RuleConditions/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            RuleCondition ruleCondition = db.RuleCondition.Find(id);
            if (ruleCondition == null)
            {
                return HttpNotFound();
            }

            return View(ruleCondition);
        }

        // GET: RuleConditions/Create
        public ActionResult Create()
        {
            ViewBag.ruleid = new SelectList(db.Rule, "id", "name");
            ViewBag.ruleconditiontypeid = new SelectList(db.RuleConditionType, "id", "name");
            return View();
        }

        // POST: RuleConditions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Create(
            [Bind(Include = "id,ruleid,isrequired,ruleconditiontypeid,ruleconditionvalue")]
            RuleCondition ruleCondition)
        {
            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(ruleCondition.ruleconditionvalue))
                {
                    ruleCondition.ruleconditionvalue = ruleCondition.ruleconditionvalue.Trim();
                }

                db.RuleCondition.Add(ruleCondition);
                db.SaveChanges();
                string ruleConditionlist = PartialView("~/Views/Rules/_RuleConditions.cshtml",
                    db.RuleCondition.Where(rc => rc.ruleid == ruleCondition.ruleid).Include(rt => rt.RuleConditionType)
                        .ToList()).RenderToString();
                return Json(new
                {
                    success = true,
                    response = "Condition has been added successfully.",
                    ruleconditionlist = ruleConditionlist
                });
            }

            ViewBag.ruleid = new SelectList(db.Rule, "id", "name", ruleCondition.ruleid);
            ViewBag.ruleconditiontypeid =
                new SelectList(db.RuleConditionType, "id", "name", ruleCondition.ruleconditiontypeid);
            return View(ruleCondition);
        }

        // GET: RuleConditions/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            RuleCondition ruleCondition = db.RuleCondition.Where(i => i.id == id).Include(r => r.RuleConditionType)
                .FirstOrDefault();
            if (ruleCondition == null)
            {
                return HttpNotFound();
            }

            ViewBag.ruleid = new SelectList(db.Rule, "id", "name", ruleCondition.ruleid);
            ViewBag.ruleconditiontypeid =
                new SelectList(db.RuleConditionType, "id", "name", ruleCondition.ruleconditiontypeid);
            return PartialView("~/Views/Rules/_NewRuleCondition.cshtml", ruleCondition);
        }

        // POST: RuleConditions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Edit(
            [Bind(Include = "id,ruleid,isrequired,ruleconditiontypeid,ruleconditionvalue")]
            RuleCondition ruleCondition)
        {
            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(ruleCondition.ruleconditionvalue))
                {
                    ruleCondition.ruleconditionvalue = ruleCondition.ruleconditionvalue.Trim();
                }

                db.Entry(ruleCondition).State = EntityState.Modified;
                db.SaveChanges();
                System.Collections.Generic.List<RuleCondition> conditionList = db.RuleCondition.Where(rc => rc.ruleid == ruleCondition.ruleid)
                    .Include(r => r.RuleConditionType).ToList();
                string ruleConditionlist =
                    PartialView("~/Views/Rules/_RuleConditions.cshtml", conditionList).RenderToString();
                return Json(new
                {
                    success = true,
                    response = "Condition has been updated successfully.",
                    ruleconditionlist = ruleConditionlist
                });
            }

            ViewBag.ruleid = new SelectList(db.Rule, "id", "name", ruleCondition.ruleid);
            ViewBag.ruleconditiontypeid =
                new SelectList(db.RuleConditionType, "id", "name", ruleCondition.ruleconditiontypeid);
            return View(ruleCondition);
        }

        // GET: RuleConditions/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            RuleCondition ruleCondition = db.RuleCondition.Find(id);
            if (ruleCondition == null)
            {
                return HttpNotFound();
            }

            return View(ruleCondition);
        }

        // POST: RuleConditions/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        public ActionResult DeleteConfirmed(long id)
        {
            RuleCondition ruleCondition = db.RuleCondition.Find(id);
            db.RuleCondition.Remove(ruleCondition);
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