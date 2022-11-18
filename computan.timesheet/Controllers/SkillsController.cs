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
    public class SkillsController : BaseController
    {
        // GET: Skills
        public ActionResult Index()
        {
            return View(db.Skill.ToList());
        }

        // GET: Skills/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Skill skill = db.Skill.Find(id);
            if (skill == null)
            {
                return HttpNotFound();
            }

            return View(skill);
        }

        // GET: Skills/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Skills/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(
            [Bind(Include = "id,name,isactive,createdonutc,updatedonutc,ipused,userid")]
            Skill skill)
        {
            Skill skil = db.Skill.Where(c => c.name == skill.name).FirstOrDefault();
            if (skil != null)
            {
                ModelState.AddModelError("name", "Sorry, skill name already exist.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    skill.createdonutc = DateTime.Now;
                    skill.updatedonutc = DateTime.Now;
                    skill.ipused = Request.UserHostAddress;
                    skill.userid = User.Identity.GetUserId();
                    db.Skill.Add(skill);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                    return View(skill);
                }
            }

            return View(skill);
        }

        // GET: Skills/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Skill skill = db.Skill.Find(id);
            if (skill == null)
            {
                return HttpNotFound();
            }

            return View(skill);
        }

        // POST: Skills/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(
            [Bind(Include = "id,name,isactive,createdonutc,updatedonutc,ipused,userid")]
            Skill skill)
        {
            int skil = db.Skill.Where(c => c.name == skill.name).Count();
            if (skil > 1)
            {
                ModelState.AddModelError("name", "Sorry, skill name already exist.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    skill.updatedonutc = DateTime.Now;
                    skill.ipused = Request.UserHostAddress;
                    skill.userid = User.Identity.GetUserId();
                    db.Entry(skill).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                    return View(skill);
                }
            }

            return View(skill);
        }

        // GET: Skills/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Skill skill = db.Skill.Find(id);
            if (skill == null)
            {
                return HttpNotFound();
            }

            return View(skill);
        }

        // POST: Skills/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            Skill skill = db.Skill.Find(id);
            db.Skill.Remove(skill);
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