using computan.timesheet.core;
using computan.timesheet.Helpers;
using computan.timesheet.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace computan.timesheet.Controllers
{
    [CustomeAuthorizeAttribute]
    public class ProjectsController : BaseController
    {
        private ApplicationUserManager _userManager;

        public ApplicationUserManager UserManager
        {
            get => _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            private set => _userManager = value;
        }

        // GET: Projects
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult GetData(bool? filter)
        {
            try
            {
                string draw = Request.Params.GetValues("draw")[0];
                int start = Convert.ToInt32(Request.Params.GetValues("start")[0]);
                int lenght = Convert.ToInt32(Request.Params.GetValues("length")[0]);
                string searchValue = Request["search[value]"];
                System.Collections.Generic.List<ProjectsViewModels> projects = db.Database.SqlQuery<ProjectsViewModels>("exec GetProjectListByFilter_sp " + filter)
                    .ToList();
                int TotalRecordsCount = projects.Count();
                if (!string.IsNullOrEmpty(searchValue))
                {
                    string[] searcharray = searchValue.Split(' ').ToArray();
                    projects = (from c in projects
                                where c.name != null && searcharray.Any(val => c.name.ToLower().Contains(val.ToLower()))
                                      || c.clientname != null &&
                                      searcharray.Any(val => c.clientname.ToLower().Contains(val.ToLower()))
                                      || c.projectmanager != null && searcharray.Any(val =>
                                          c.projectmanager.ToLower().Contains(val.ToLower()))
                                      || c.description != null &&
                                      searcharray.Any(val => c.description.ToLower().Contains(val.ToLower()))
                                select c).ToList();
                }

                int FilteredRecordCount = projects.Count();
                if (lenght > 0)
                {
                    projects = projects.Skip(start).Take(lenght).ToList();
                }

                return Json(new
                {
                    draw = Convert.ToInt32(draw),
                    recordsTotal = TotalRecordsCount,
                    recordsFiltered = FilteredRecordCount,
                    data = projects
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    data = ex.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }
        //public ActionResult ProjectCredentials(long id)
        //{
        //    if (!IsLoggedIn())
        //    {
        //        return RedirectToAction("login", "account");
        //    }
        //    var credentials = db.Credentials.Where(cd => cd.projectid == id).FirstOrDefault();
        //    if (credentials != null)
        //    {
        //        ViewBag.credentialcategoryid = new SelectList(db.CredentialCategories, "id", "name", credentials.credentialcategoryid);
        //        ViewBag.credentiallevelid = new SelectList(db.CredentialLevels, "id", "name", credentials.credentiallevelid);
        //        ViewBag.crendentialtypeid = new SelectList(db.CredentialTypes, "id", "name", credentials.crendentialtypeid);
        //        ViewBag.projectid = new SelectList(db.Project, "id", "name", credentials.projectid);
        //    }
        //    else
        //    {
        //        ViewBag.credentialcategoryid = new SelectList(db.CredentialCategories, "id", "name");
        //        ViewBag.credentiallevelid = new SelectList(db.CredentialLevels, "id", "name");
        //        ViewBag.crendentialtypeid = new SelectList(db.CredentialTypes, "id", "name");
        //        ViewBag.projectid = new SelectList(db.Project, "id", "name");
        //    }
        //    return View("~/views/credentials/edit.cshtml", credentials);
        //}
        //[HttpPost]
        //public ActionResult ProjectCredentials([Bind(Include = "id,crendentialtypeid,credentiallevelid,projectid,credentialcategoryid,url,username,password,port,comments,host,networkdomain,createdonutc,updatedonutc,ipused,userid")] Credentials credentials)
        //{
        //    try
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            var credential = db.Credentials.Where(p => p.projectid == credentials.projectid).FirstOrDefault();
        //            if (credential != null)
        //            {
        //                credential.username = credentials.username.Trim();
        //                credential.userid = User.Identity.GetUserId();
        //                credential.ipused = Request.UserHostAddress;
        //                credential.updatedonutc = DateTime.Now;
        //                db.Entry(credential).State = EntityState.Modified;
        //                db.SaveChanges();
        //                return RedirectToAction("Index");
        //            }
        //            else
        //            {
        //                credentials.username = credentials.username.Trim();
        //                credentials.userid = User.Identity.GetUserId();
        //                credentials.ipused = Request.UserHostAddress;
        //                credentials.createdonutc = DateTime.Now;
        //                credentials.updatedonutc = DateTime.Now;
        //                db.Credentials.Add(credentials);
        //                db.SaveChanges();
        //                return RedirectToAction("Index");
        //            }
        //        }
        //        ViewBag.credentialcategoryid = new SelectList(db.CredentialCategories, "id", "name", credentials.credentialcategoryid);
        //        ViewBag.credentiallevelid = new SelectList(db.CredentialLevels, "id", "name", credentials.credentiallevelid);
        //        ViewBag.crendentialtypeid = new SelectList(db.CredentialTypes, "id", "name", credentials.crendentialtypeid);
        //        ViewBag.projectid = new SelectList(db.Project, "id", "name", credentials.projectid);
        //        return View("~/views/credentials/Index.cshtml", credentials);
        //    }
        //    catch (Exception ex)
        //    {
        //        ModelState.AddModelError("", ex.Message);
        //        ViewBag.credentialcategoryid = new SelectList(db.CredentialCategories, "id", "name", credentials.credentialcategoryid);
        //        ViewBag.credentiallevelid = new SelectList(db.CredentialLevels, "id", "name", credentials.credentiallevelid);
        //        ViewBag.crendentialtypeid = new SelectList(db.CredentialTypes, "id", "name", credentials.crendentialtypeid);
        //        ViewBag.projectid = new SelectList(db.Project, "id", "name", credentials.projectid);
        //        return View("~/views/credentials/edit.cshtml", credentials);
        //    }
        //}

        // GET: Projects/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Project project = db.Project.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }

            return View(project);
        }

        // GET: Projects/Create
        public ActionResult Create(long? id)
        {
            System.Collections.Generic.List<SelectListItem> clienttlist = db.Database.SqlQuery<CombinedEntity>("exec clients_loadcombined").Select(x =>
                new SelectListItem
                {
                    Text = x.name,
                    Value = x.id.ToString()
                }).ToList();
            if (id != null)
            {
                ViewBag.clientlist = new SelectList(clienttlist, "Value", "Text", id.Value);
                ViewBag.hasclient = true;
                ViewBag.clientid = id.Value;
            }
            else
            {
                ViewBag.clientlist = new SelectList(clienttlist, "Value", "Text");
                ViewBag.hasclient = false;
            }

            System.Collections.Generic.List<SelectListItem> projectlist = db.Database.SqlQuery<CombinedEntity>("exec projects_loadcombined").Select(x =>
                new SelectListItem
                {
                    Text = x.name,
                    Value = x.id.ToString()
                }).ToList();
            ViewBag.users =
                new SelectList(UserManager.Users.OrderBy(f => f.FirstName).Where(u => u.isactive == true).ToList(),
                    "Id", "FullName");
            ViewBag.parentlist = new SelectList(projectlist, "Value", "Text");
            return View();
        }

        // POST: Projects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        [AllowAnonymous]
        public ActionResult Create(
            [Bind(Include =
                "id,parentid,clientid,name,projectmanager,description,startdate,completiondate,isactive,createdonutc,updatedonutc,ipused,userid,iswarning,warningtext")]
            Project project)
        {
            Project projectdup = db.Project.Where(t => t.name == project.name).FirstOrDefault();
            if (projectdup != null)
            {
                ModelState.AddModelError("name", "Sorry,project already exist.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    project.createdonutc = DateTime.Now;
                    project.updatedonutc = DateTime.Now;
                    project.ipused = Request.UserHostAddress;
                    project.userid = User.Identity.GetUserId();
                    db.Project.Add(project);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                    return View(project);
                }
            }

            ViewBag.users =
                new SelectList(UserManager.Users.OrderBy(f => f.FirstName).Where(u => u.isactive == true).ToList(),
                    "Id", "FullName");
            System.Collections.Generic.List<SelectListItem> clienttlist = db.Database.SqlQuery<CombinedEntity>("exec clients_loadcombined").Select(x =>
                new SelectListItem
                {
                    Text = x.name,
                    Value = x.id.ToString()
                }).ToList();
            ViewBag.clientlist = new SelectList(clienttlist, "Value", "Text", project.clientid);
            System.Collections.Generic.List<SelectListItem> projectlist = db.Database.SqlQuery<CombinedEntity>("exec projects_loadcombined").Select(x =>
                new SelectListItem
                {
                    Text = x.name,
                    Value = x.id.ToString()
                }).ToList();
            ViewBag.parentlist = new SelectList(projectlist, "Value", "Text", project.parentid);
            return View(project);
        }

        // GET: Projects/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Project project = db.Project.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }

            System.Collections.Generic.List<SelectListItem> clienttlist = db.Database.SqlQuery<CombinedEntity>("exec clients_loadcombined").Select(x =>
                new SelectListItem
                {
                    Text = x.name,
                    Value = x.id.ToString()
                }).ToList();
            ViewBag.clientlist = new SelectList(clienttlist, "Value", "Text", project.clientid);
            ViewBag.users =
                new SelectList(UserManager.Users.OrderBy(f => f.FirstName).Where(u => u.isactive == true).ToList(),
                    "Id", "FullName"); //, project.clientid
            System.Collections.Generic.List<SelectListItem> projectlist = db.Database.SqlQuery<CombinedEntity>("exec projects_loadcombined").Select(x =>
                new SelectListItem
                {
                    Text = x.name,
                    Value = x.id.ToString()
                }).ToList();
            projectlist.RemoveAll(c => c.Value == id.ToString());
            ViewBag.parentlist = new SelectList(projectlist, "Value", "Text", project.parentid);
            return View(project);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit(
            [Bind(Include =
                "id,parentid,clientid,name,projectmanager,description,startdate,completiondate,isactive,createdonutc,updatedonutc,ipused,userid,iswarning,warningtext")]
            Project project)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    project.updatedonutc = DateTime.Now;
                    project.ipused = Request.UserHostAddress;
                    project.userid = User.Identity.GetUserId();
                    db.Entry(project).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }


                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                    return View(project);
                }
            }

            System.Collections.Generic.List<SelectListItem> clienttlist = db.Database.SqlQuery<CombinedEntity>("exec clients_loadcombined").Select(x =>
                new SelectListItem
                {
                    Text = x.name,
                    Value = x.id.ToString()
                }).ToList();
            ViewBag.clientlist = new SelectList(clienttlist, "Value", "Text", project.clientid); //, project.clientid
            System.Collections.Generic.List<SelectListItem> projectlist = db.Database.SqlQuery<CombinedEntity>("exec projects_loadcombined").Select(x =>
                new SelectListItem
                {
                    Text = x.name,
                    Value = x.id.ToString()
                }).ToList();
            projectlist.RemoveAll(c => c.Value == project.id.ToString());
            ViewBag.parentlist = new SelectList(projectlist, "Value", "Text", project.parentid);
            ViewBag.users =
                new SelectList(UserManager.Users.OrderBy(f => f.FirstName).Where(u => u.isactive == true).ToList(),
                    "Id", "FullName");
            return View(project);
        }

        // GET: Projects/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Project project = db.Project.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }

            return View(project);
        }

        // POST: Projects/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            Project project = db.Project.Find(id);
            db.Project.Remove(project);
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