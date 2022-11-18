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
    [CustomeAuthorizeAttribute(Role.Admin)]
    public class TeamsController : BaseController
    {
        // GET: Teams
        public ActionResult Index()
        {
            return View(db.Team.ToList());
        }

        // GET: Teams/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Team team = db.Team.Find(id);
            if (team == null)
            {
                return HttpNotFound();
            }

            return View(team);
        }

        // GET: Teams/Create
        public ActionResult Create()
        {
            ViewBag.Manager = new SelectList(db.Users.OrderBy(f => f.FirstName).Where(u => u.isactive == true).ToList(),
                "id", "NameEmail");
            return View();
        }

        // POST: Teams/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(
            [Bind(Include = "id,name,code,isactive,createdonutc,updatedonutc,ipused,userid,Manager,CSM")]
            Team team)
        {
            Team teamcheck = db.Team.Where(c => c.name == team.name).FirstOrDefault();
            if (teamcheck != null)
            {
                ModelState.AddModelError("name", "Sorry, Team name already exist.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    team.displayorder = 1;
                    team.createdonutc = DateTime.Now;
                    team.updatedonutc = DateTime.Now;
                    team.ipused = Request.UserHostAddress;
                    team.userid = User.Identity.GetUserId();
                    db.Team.Add(team);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                    return View(team);
                }
            }

            return View(team);
        }

        // GET: Teams/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Team team = db.Team.Find(id);
            if (team == null)
            {
                return HttpNotFound();
            }

            ViewBag.Manager = new SelectList(db.Users.OrderBy(f => f.FirstName).Where(u => u.isactive == true).ToList(),
                "id", "NameEmail");
            return View(team);
        }

        // POST: Teams/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(
            [Bind(Include =
                "id,name,code,isactive,createdonutc,updatedonutc,ipused,userid,displayorder,Manager,CSM")]
            Team team)
        {
            int teamcheck = db.Team.Where(c => c.name == team.name).Count();
            if (teamcheck > 1)
            {
                ModelState.AddModelError("name", "Sorry, Team name already exist.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    team.updatedonutc = DateTime.Now;
                    team.ipused = Request.UserHostAddress;
                    team.userid = User.Identity.GetUserId();
                    db.Entry(team).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                    return View(team);
                }
            }

            return View(team);
        }

        // GET: Teams/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Team team = db.Team.Find(id);
            if (team == null)
            {
                return HttpNotFound();
            }

            return View(team);
        }

        // POST: Teams/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            Team team = db.Team.Find(id);
            db.Team.Remove(team);
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