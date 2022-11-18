using computan.timesheet.Contexts;
using computan.timesheet.core;
using computan.timesheet.Extensions;
using computan.timesheet.Helpers;
using computan.timesheet.Models;
using Microsoft.AspNet.Identity.Owin;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace computan.timesheet.Controllers
{
    [CustomeAuthorizeAttribute]
    public class RuleActionsController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();
        private ApplicationUserManager _userManager;

        public ApplicationUserManager UserManager
        {
            get => _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            private set => _userManager = value;
        }

        // GET: RuleActions
        public ActionResult Index()
        {
            IQueryable<RuleAction> ruleAction = db.RuleAction.Include(r => r.Rule).Include(r => r.RuleActionType);
            return View(ruleAction.ToList());
        }

        // GET: RuleActions/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            RuleAction ruleAction = db.RuleAction.Find(id);
            if (ruleAction == null)
            {
                return HttpNotFound();
            }

            return View(ruleAction);
        }

        // GET: RuleActions/Create
        public ActionResult Create()
        {
            ViewBag.ruleid = new SelectList(db.Rule, "id", "name");
            ViewBag.ruleactiontypeid = new SelectList(db.RuleActionType, "id", "name");
            ViewBag.projectid = new SelectList(db.Project, "id", "name");
            ViewBag.skillid = new SelectList(db.Skill, "id", "name");
            ViewBag.statusid = new SelectList(db.TicketStatus.Where(i => i.id != 1).ToList(), "id", "name");
            return View();
        }

        // POST: RuleActions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Create(
            [Bind(Include = "id,ruleid,ruleactiontypeid,ruleactionvalue,projectid,skillid,statusid")]
            RuleAction ruleAction)
        {
            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(ruleAction.ruleactionvalue))
                {
                    ruleAction.ruleactionvalue = ruleAction.ruleactionvalue.Trim();
                }

                db.RuleAction.Add(ruleAction);
                db.SaveChanges();
                System.Collections.Generic.List<RuleAction> RuleActionList = db.RuleAction.Where(ra => ra.ruleid == ruleAction.ruleid)
                    .Include(rt => rt.RuleActionType).ToList();

                foreach (RuleAction action in RuleActionList)
                {
                    ApplicationUser user = db.Users.Find(action.ruleactionvalue);
                    if (user != null)
                    {
                        action.fullname = user.FirstName + " " + user.LastName;
                    }
                }

                string ruleslist = PartialView("~/Views/Rules/_RuleActions.cshtml",
                    db.RuleAction.Where(rc => rc.ruleid == ruleAction.ruleid).ToList()).RenderToString();
                return Json(new { success = true, ruleslist });
            }

            ViewBag.ruleid = new SelectList(db.Rule, "id", "name", ruleAction.ruleid);
            ViewBag.ruleactiontypeid = new SelectList(db.RuleActionType, "id", "name", ruleAction.ruleactiontypeid);
            ViewBag.projectid = new SelectList(db.Project, "id", "name");
            ViewBag.skillid = new SelectList(db.Skill, "id", "name");
            ViewBag.statusid =
                new SelectList(db.TicketStatus.Where(i => i.id != 1 && i.id != 2).ToList(), "id", "name");
            return View(ruleAction);
        }

        // GET: RuleActions/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            RuleAction ruleAction = db.RuleAction.Find(id);
            if (ruleAction == null)
            {
                return HttpNotFound();
            }

            ViewBag.ruleid = new SelectList(db.Rule, "id", "name", ruleAction.ruleid);
            ViewBag.ruleactiontypeid = new SelectList(db.RuleActionType, "id", "name", ruleAction.ruleactiontypeid);
            ViewBag.ruleactionvalue =
                new SelectList(UserManager.Users.OrderBy(f => f.FirstName).Where(u => u.isactive == true).ToList(),
                    "Id", "FullName", ruleAction.ruleactionvalue);
            ViewBag.projectid = new SelectList(db.Project, "id", "name", ruleAction.projectid);
            ViewBag.skillid = new SelectList(db.Skill, "id", "name", ruleAction.skillid);
            ViewBag.statusid =
                new SelectList(db.TicketStatus.Where(i => i.id != 1 && i.id != 2).ToList(), "id", "name");
            return PartialView("~/Views/Rules/_NewRuleAction.cshtml", ruleAction);
        }

        // POST: RuleActions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Edit(
            [Bind(Include = "id,ruleid,ruleactiontypeid,ruleactionvalue,projectid,skillid,statusid")]
            RuleAction ruleAction)
        {
            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(ruleAction.ruleactionvalue))
                {
                    ruleAction.ruleactionvalue = ruleAction.ruleactionvalue.Trim();
                }

                if (ruleAction.ruleactiontypeid == 2)
                {
                    ruleAction.projectid = null;
                    ruleAction.skillid = null;
                }

                db.Entry(ruleAction).State = EntityState.Modified;
                db.SaveChanges();
                System.Collections.Generic.List<RuleAction> RuleActionList = db.RuleAction.Where(ra => ra.ruleid == ruleAction.ruleid)
                    .Include(rt => rt.RuleActionType).ToList();

                foreach (RuleAction action in RuleActionList)
                {
                    ApplicationUser user = db.Users.Find(action.ruleactionvalue);
                    if (user != null)
                    {
                        action.fullname = user.FirstName + " " + user.LastName;
                    }
                }

                string ruleActionlist = PartialView("~/Views/Rules/_RuleActions.cshtml",
                    db.RuleAction.Where(rc => rc.ruleid == ruleAction.ruleid).ToList()).RenderToString();
                return Json(new
                {
                    success = true,
                    response = "Action has been updated successfully.",
                    ruleactionlist = ruleActionlist
                });
            }

            ViewBag.ruleid = new SelectList(db.Rule, "id", "name", ruleAction.ruleid);
            ViewBag.ruleactiontypeid = new SelectList(db.RuleActionType, "id", "name", ruleAction.ruleactiontypeid);
            return View(ruleAction);
        }

        // GET: RuleActions/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            RuleAction ruleAction = db.RuleAction.Find(id);
            if (ruleAction == null)
            {
                return HttpNotFound();
            }

            return View(ruleAction);
        }

        // POST: RuleActions/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        public ActionResult DeleteConfirmed(long id)
        {
            RuleAction ruleAction = db.RuleAction.Find(id);
            db.RuleAction.Remove(ruleAction);
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