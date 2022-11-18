using computan.timesheet.core;
using computan.timesheet.core.common;
using computan.timesheet.Helpers;
using computan.timesheet.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace computan.timesheet.Controllers
{
    [CustomeAuthorizeAttribute(Role.Admin)]
    public class TeamMembersController : BaseController
    {
        private ApplicationUserManager _userManager;

        public TeamMembersController()
        {
        }

        public TeamMembersController(ApplicationUserManager userManager)
        {
            UserManager = userManager;
        }

        public ApplicationUserManager UserManager
        {
            get => _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            private set => _userManager = value;
        }


        // GET: TeamMembers
        public ActionResult Index(long id)
        {
            System.Collections.Generic.List<TeamMemberViewModel> teammembers = db.Database
                .SqlQuery<TeamMemberViewModel>("exec TeamMember_loadbyteamid @teamid", new SqlParameter("@teamid", id))
                .ToList();
            //var teamMember = db.TeamMember.Include(t => t.Team).Include(t => t.User).Where(t => t.teamid==id && t.IsActive== true);
            ViewBag.team = db.Team.Find(id).name;
            ViewBag.teams = new SelectList(db.Team, "id", "name"); // db.Team.ToList();
            return View(teammembers);
        }

        //Move user to other team
        public ActionResult MoveUser(int teamid, string usersid, int teamMemberid)
        {
            TeamMember sameteamUser = db.TeamMember.Where(tm => tm.teamid == teamid && tm.usersid == usersid)
                .SingleOrDefault();
            TeamMember usermanager = db.TeamMember.Where(t => t.id == teamMemberid).SingleOrDefault();
            if (sameteamUser != null)
            {
                return Json(new { error = 1, contextText = "User is alreaady in this team" },
                    JsonRequestBehavior.AllowGet);
            }

            if (usermanager.IsManager)
            {
                TeamMember manager = db.TeamMember.Where(t => t.teamid == teamid && t.IsManager).SingleOrDefault();
                if (manager != null)
                {
                    return Json(new { error = 1, contextText = "Team alreaady contains manager" },
                        JsonRequestBehavior.AllowGet);
                }

                TeamMember teammember = db.TeamMember.Where(tm => tm.id == teamMemberid).SingleOrDefault();
                teammember.teamid = teamid;
                teammember.usersid = usersid;
                db.SaveChanges();
                return Json(new { error = 0, contextText = "User team changed successfully" },
                    JsonRequestBehavior.AllowGet);
            }
            else
            {
                TeamMember teammember = db.TeamMember.Where(tm => tm.id == teamMemberid).SingleOrDefault();
                teammember.teamid = teamid;
                teammember.usersid = usersid;
                db.SaveChanges();
                return Json(new { error = 0, contextText = "User team changed successfully" },
                    JsonRequestBehavior.AllowGet);
            }
        }

        // GET: TeamMembers/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            TeamMember teamMember = db.TeamMember.Find(id);
            if (teamMember == null)
            {
                return HttpNotFound();
            }

            return View(teamMember);
        }

        // GET: TeamMembers/Create
        public ActionResult Create(long id)
        {
            ViewBag.MemberTeamId = id.ToString();
            ViewBag.teamid = new SelectList(db.Team, "id", "name", id);
            ViewBag.usersid =
                new SelectList(UserManager.Users.OrderBy(f => f.FirstName).Where(u => u.isactive == true).ToList(),
                    "Id", "NameEmail");
            ViewBag.reported =
                new SelectList(UserManager.Users.OrderBy(f => f.FirstName).Where(u => u.isactive == true).ToList(),
                    "Id", "NameEmail");
            return View();
        }

        // POST: TeamMembers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TeamMember teamMember)
        {
            TeamMember teammem = db.TeamMember.Where(c => c.usersid == teamMember.usersid && c.teamid == teamMember.teamid)
                .FirstOrDefault();

            if (teammem != null)
            {
                ModelState.AddModelError("", "Sorry, Team Member already exist in team.");
            }
            //if(teamMember.IsManager)
            //{
            //    var teamManager = db.TeamMember.Where(t => t.teamid == teamMember.teamid && t.IsManager).FirstOrDefault();
            //    if(teamManager !=null)
            //    {
            //        ModelState.AddModelError("IsManager", "Sorry, Team manager is already exist in team.");
            //    }
            //}

            if (ModelState.IsValid)
            {
                try
                {
                    teamMember.createdonutc = DateTime.Now;
                    teamMember.updatedonutc = DateTime.Now;
                    teamMember.ipused = Request.UserHostAddress;
                    teamMember.userid = User.Identity.GetUserId();

                    db.TeamMember.Add(teamMember);
                    db.SaveChanges();
                    return RedirectToAction("Index", new { id = teamMember.teamid });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                    ViewBag.teamid = new SelectList(db.Team, "id", "name", teamMember.teamid);
                    ViewBag.usersid =
                        new SelectList(
                            UserManager.Users.OrderBy(f => f.FirstName).Where(u => u.isactive == true).ToList(), "Id",
                            "NameEmail");
                    ViewBag.reported =
                        new SelectList(
                            UserManager.Users.OrderBy(f => f.FirstName).Where(u => u.isactive == true).ToList(), "Id",
                            "NameEmail");
                    return View(teamMember);
                }
            }

            ViewBag.reported =
                new SelectList(UserManager.Users.OrderBy(f => f.FirstName).Where(u => u.isactive == true).ToList(),
                    "Id", "NameEmail");
            ViewBag.teamid = new SelectList(db.Team, "id", "name", teamMember.teamid);
            ViewBag.usersid =
                new SelectList(UserManager.Users.OrderBy(f => f.FirstName).Where(u => u.isactive == true).ToList(),
                    "Id", "NameEmail");
            return View(teamMember);
        }

        // GET: TeamMembers/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            TeamMember teamMember = db.TeamMember.Find(id);
            if (teamMember == null)
            {
                return HttpNotFound();
            }

            ViewBag.teamid = new SelectList(db.Team, "id", "name", teamMember.teamid);
            ViewBag.usersid =
                new SelectList(UserManager.Users.OrderBy(f => f.FirstName).Where(u => u.isactive == true).ToList(),
                    "Id", "NameEmail");
            ViewBag.reported =
                new SelectList(UserManager.Users.OrderBy(f => f.FirstName).Where(u => u.isactive == true).ToList(),
                    "Id", "NameEmail");
            return View(teamMember);
        }

        // POST: TeamMembers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(TeamMember teamMember)
        {
            //if (teamMember.IsManager)
            //{
            //    var teamManager = db.TeamMember.Where(t => t.teamid == teamMember.teamid && t.IsManager).FirstOrDefault();
            //    if (teamManager != null && teamManager.id != teamMember.id)
            //        ModelState.AddModelError("IsManager", "Sorry, Team manager is already exist in team.");                                        
            //    else if(teamMember.Reported != null)                
            //        ModelState.AddModelError("Reported", "Sorry, Manager is checked!");
            //    if (teamMember.IsTeamLead)
            //        ModelState.AddModelError("IsTeamLead", "Sorry, Manager is checked!");
            //    if(teamManager != null)
            //        db.Entry(teamManager).State = EntityState.Detached;
            //}
            if (ModelState.IsValid)
            {
                try
                {
                    teamMember.updatedonutc = DateTime.Now;
                    teamMember.ipused = Request.UserHostAddress;
                    teamMember.userid = User.Identity.GetUserId();
                    db.Entry(teamMember).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index", new { id = teamMember.teamid });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                    ViewBag.teamid = new SelectList(db.Team, "id", "name", teamMember.teamid);
                    ViewBag.usersid =
                        new SelectList(
                            UserManager.Users.OrderBy(f => f.FirstName).Where(u => u.isactive == true).ToList(), "Id",
                            "NameEmail");
                    ViewBag.reported =
                        new SelectList(
                            UserManager.Users.OrderBy(f => f.FirstName).Where(u => u.isactive == true).ToList(), "Id",
                            "NameEmail");
                    return View(teamMember);
                }
            }

            ViewBag.teamid = new SelectList(db.Team, "id", "name", teamMember.teamid);
            ViewBag.usersid =
                new SelectList(UserManager.Users.OrderBy(f => f.FirstName).Where(u => u.isactive == true).ToList(),
                    "Id", "NameEmail");
            ViewBag.reported =
                new SelectList(UserManager.Users.OrderBy(f => f.FirstName).Where(u => u.isactive == true).ToList(),
                    "Id", "NameEmail");
            return View(teamMember);
        }

        // GET: TeamMembers/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            TeamMember teamMember = db.TeamMember.Find(id);
            if (teamMember == null)
            {
                return HttpNotFound();
            }

            return View(teamMember);
        }

        // POST: TeamMembers/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            TeamMember teamMember = db.TeamMember.Find(id);
            db.TeamMember.Remove(teamMember);
            db.SaveChanges();
            return RedirectToAction("Index", "Teammembers", new { id = teamMember.teamid });
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