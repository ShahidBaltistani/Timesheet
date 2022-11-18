using ClosedXML.Excel;
using computan.timesheet.Contexts;
using computan.timesheet.core;
using computan.timesheet.core.common;
using computan.timesheet.Helpers;
using computan.timesheet.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace computan.timesheet.Controllers
{
    [CustomeAuthorizeAttribute]
    public class CredentialsController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        // GET: Credentials      
        public ActionResult Index(string s)
        {
            ViewBag.projects = new SelectList(db.Project.ToList(), "id", "name");
            if (!string.IsNullOrEmpty(s))
            {
                ViewBag.projectid = db.Project.Where(x => x.name == s).FirstOrDefault()?.id;
            }

            return View();
        }

        [HttpGet]
        public ActionResult GetData(long? id, bool? filter)
        {
            try
            {
                string draw = Request.Params.GetValues("draw")[0];
                int start = Convert.ToInt32(Request.Params.GetValues("start")[0]);
                int lenght = Convert.ToInt32(Request.Params.GetValues("length")[0]);
                string searchValue = Request["search[value]"];
                string userid = User.Identity.GetUserId();
                ApplicationUser user = db.Users.Where(ui => ui.Id == userid).FirstOrDefault();
                List<CredentialsViewModel> credentials = db.Database.SqlQuery<CredentialsViewModel>(
                    "GetCredentialListByFilter_sp @ProjectId, @UserLevelId, @Active",
                    new SqlParameter("ProjectId", id ?? (object)DBNull.Value),
                    new SqlParameter("Active", filter ?? (object)DBNull.Value),
                    new SqlParameter("UserLevelId", user.Levelid)
                ).ToList();
                int TotalRecordsCount = credentials.Count();
                if (!string.IsNullOrEmpty(searchValue))
                {
                    if (searchValue.Contains("+"))
                    {
                        string[] searcharray1 = searchValue.Split('+').ToArray();
                        foreach (string item in searcharray1)
                        {
                            credentials = (from c in credentials
                                           where c.username.ToLower().Contains(item.ToLower())
                                                 || c.comments != null && c.comments.ToLower().Contains(item.ToLower())
                                                 || c.ccategoryname != null && c.ccategoryname.ToLower().Contains(item.ToLower())
                                                 || c.link != null && c.link.ToLower().Contains(item.ToLower())
                                                 || c.title != null && c.title.ToLower().Contains(item.ToLower())
                                                 || c.projectname != null && c.projectname.ToLower().Contains(item.ToLower())
                                                 || c.host != null && c.host.ToLower().Contains(item.ToLower())
                                                 || c.networkdomain != null && c.networkdomain.ToLower().Contains(item.ToLower())
                                           select c).ToList();
                        }
                    }
                    else
                    {
                        string[] searcharray = searchValue.Split(' ').ToArray();
                        credentials = (from c in credentials
                                       where searcharray.Any(val => c.username.ToLower().Contains(val.ToLower()))
                                             || c.comments != null &&
                                             searcharray.Any(val => c.comments.ToLower().Contains(val.ToLower()))
                                             || c.ccategoryname != null && searcharray.Any(val =>
                                                 c.ccategoryname.ToLower().Contains(val.ToLower()))
                                             || c.link != null && searcharray.Any(val => c.link.ToLower().Contains(val.ToLower()))
                                             || c.title != null &&
                                             searcharray.Any(val => c.title.ToLower().Contains(val.ToLower()))
                                             || c.projectname != null && searcharray.Any(val =>
                                                 c.projectname.ToLower().Contains(val.ToLower()))
                                             || c.host != null && searcharray.Any(val => c.host.ToLower().Contains(val.ToLower()))
                                             || c.networkdomain != null && searcharray.Any(val =>
                                                 c.networkdomain.ToLower().Contains(val.ToLower()))
                                       select c).ToList();
                    }
                }

                int FilteredRecordCount = credentials.Count();
                credentials = credentials.Skip(start).Take(lenght).ToList();
                foreach (CredentialsViewModel credential in credentials)
                {
                    byte[] salt = Convert.FromBase64String(credential.passwordsalt).ToArray();
                    credential.password = EncryptionHelper.DecryptString(credential.passwordhash, salt);
                }

                TempData["CredentialsList"] = credentials;
                return Json(new
                {
                    draw = Convert.ToInt32(draw),
                    recordsTotal = TotalRecordsCount,
                    recordsFiltered = FilteredRecordCount,
                    data = credentials
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

        public ActionResult IndexDev(string s)
        {
            ViewBag.projects = new SelectList(db.Project.ToList(), "id", "name");
            if (!string.IsNullOrEmpty(s))
            {
                ViewBag.projectid = db.Project.Where(x => x.name == s).FirstOrDefault()?.id;
            }

            return View();
        }

        //Get: Project Credentials
        public ActionResult ProjectCredentials(long id)
        {
            ViewBag.IsProjectCredentials = true;
            ViewBag.projectid = id;
            ViewBag.projects = new SelectList(db.Project.ToList(), "id", "name");
            if (User.IsInRole(Role.Admin.ToString()))
            {
                return View("Index");
            }

            return View("IndexDev");
        }

        // GET: Credentials/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Credentials credentials = db.Credentials.Include("Project").Include("CredentialCategory").Include("CredentialLevel")
                .Include("CredentialType").Where(c => c.id == id).FirstOrDefault();
            if (credentials == null)
            {
                return HttpNotFound();
            }

            if (!string.IsNullOrEmpty(credentials.linkedCredential))
            {
                Credentials credential = db.Credentials.Find(int.Parse(credentials.linkedCredential));
                credentials.username = credential.username;
                credentials.password = credential.password;
                credentials.host = credential.host;
                credentials.port = credential.port;
                credentials.networkdomain = credential.networkdomain;
            }

            return View(credentials);
        }

        // GET: Credentials/Create
        public ActionResult Create()
        {
            ViewBag.credentialcategoryid = new SelectList(db.CredentialCategories, "id", "name");
            ViewBag.credentiallevelid =
                new SelectList(db.Database.SqlQuery<CombinedEntity>("exec GetCredentialsLevel_sp").ToList(), "id",
                    "name", 2);
            ViewBag.crendentialtypeid = new SelectList(db.CredentialTypes, "id", "name", 1);
            ViewBag.projectid = new SelectList(db.Project, "id", "name");
            ViewBag.Readonly = false;
            if (Request.IsAjaxRequest())
            {
                return PartialView("_AddCredentialForm");
            }

            return View();
        }

        public ActionResult CreateForProject(long id)
        {
            ViewBag.credentialcategoryid = new SelectList(db.CredentialCategories, "id", "name");
            ViewBag.credentiallevelid =
                new SelectList(db.Database.SqlQuery<CombinedEntity>("exec GetCredentialsLevel_sp").ToList(), "id",
                    "name", 2);
            ViewBag.crendentialtypeid = new SelectList(db.CredentialTypes, "id", "name");
            ViewBag.projectid = new SelectList(db.Project, "id", "name", id);
            ViewBag.IsProjectCredentials = true;
            ViewBag.proid = id;
            ViewBag.Readonly = false;
            return View("create");
        }

        // POST: Credentials/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Create(
            [Bind(Include =
                "id,title,crendentialtypeid,credentiallevelid,projectid,credentialcategoryid,url,username,password,port,comments,host,networkdomain,createdonutc,updatedonutc,ipused,userid,linkedCredential")]
            Credentials credentials, bool? ispro, HttpPostedFileBase file, bool linkedCred = false)
        {
            try
            {
                if (file != null)
                {
                    string[] AllowedFileExtensions = { ".txt", ".ppk", ".pem" };
                    if (file.ContentLength > 102400)
                    {
                        ModelState.AddModelError("", "Your file is too large, maximum allowed size is: 100 KB");
                    }
                    else if (!AllowedFileExtensions.Contains(file.FileName.Substring(file.FileName.LastIndexOf('.'))))
                    {
                        ModelState.AddModelError("",
                            "Please file of type: " + string.Join(", ", AllowedFileExtensions));
                    }
                }

                if (!string.IsNullOrEmpty(credentials.linkedCredential))
                {
                    credentials.linkedCredential = credentials.linkedCredential.Split('/').Last();
                    if (int.TryParse(credentials.linkedCredential, out int id))
                    {
                        Credentials credential = db.Credentials.Find(id);
                        if (credential != null)
                        {
                            credentials.username = credential.username;
                            credentials.password = credential.password;
                            ModelState["username"].Errors.Clear();
                            ModelState["password"].Errors.Clear();
                        }
                        else
                        {
                            ModelState.AddModelError("linkedCredential", "Linked credential not found.");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("linkedCredential", "Please enter correct linked credential url");
                    }
                }

                if (ModelState.IsValid)
                {
                    credentials.username = credentials.username.Trim();
                    credentials.userid = User.Identity.GetUserId();
                    credentials.ipused = Request.UserHostAddress;
                    byte[] key = EncryptionHelper.GenerateKey();
                    credentials.passwordhash = EncryptionHelper.EncryptString(credentials.password, key);
                    credentials.passwordsalt = Convert.ToBase64String(key);
                    credentials.password = EncryptionHelper.GuidPassword();
                    credentials.createdonutc = DateTime.Now;
                    credentials.updatedonutc = DateTime.Now;
                    credentials.isactive = true;
                    if (file != null)
                    {
                        byte[] bytes;
                        using (BinaryReader br = new BinaryReader(file.InputStream))
                        {
                            bytes = br.ReadBytes(file.ContentLength);
                        }

                        credentials.crendentialfile = bytes;
                        credentials.filename = file.FileName;
                    }

                    db.Credentials.Add(credentials);
                    db.SaveChanges();
                    TempData["newcredential"] = credentials.id;
                    if (Request.IsAjaxRequest())
                    {
                        return Json(new { success = true });
                    }

                    if (ispro != null && ispro == true)
                    {
                        return RedirectToAction("ProjectCredentials", new { id = credentials.projectid });
                    }

                    if (User.IsInRole(Role.Admin.ToString()))
                    {
                        return RedirectToAction("Index");
                    }

                    return RedirectToAction("IndexDev");
                }

                ViewBag.credentialcategoryid = new SelectList(db.CredentialCategories, "id", "name",
                    credentials.credentialcategoryid);
                ViewBag.credentiallevelid =
                    new SelectList(db.Database.SqlQuery<CombinedEntity>("exec GetCredentialsLevel_sp").ToList(), "id",
                        "name", credentials.credentiallevelid);
                ViewBag.crendentialtypeid = new SelectList(db.CredentialTypes, "id", "name");
                ViewBag.projectid = new SelectList(db.Project, "id", "name", credentials.projectid);
                ViewBag.Readonly = linkedCred;
                if (linkedCred)
                {
                    ModelState["username"].Errors.Clear();
                    ModelState["password"].Errors.Clear();
                    if (string.IsNullOrEmpty(credentials.linkedCredential))
                    {
                        ModelState.AddModelError("linkedCredential", "Linked credential is required.");
                    }
                }

                if (ispro != null && ispro == true)
                {
                    ViewBag.IsProjectCredentials = true;
                }

                return View(credentials);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                ViewBag.credentialcategoryid = new SelectList(db.CredentialCategories, "id", "name",
                    credentials.credentialcategoryid);
                ViewBag.credentiallevelid =
                    new SelectList(db.Database.SqlQuery<CombinedEntity>("exec GetCredentialsLevel_sp").ToList(), "id",
                        "name", credentials.credentiallevelid);
                ViewBag.projectid = new SelectList(db.Project, "id", "name", credentials.projectid);
                ViewBag.crendentialtypeid = new SelectList(db.CredentialTypes, "id", "name");
                ViewBag.Readonly = linkedCred;
                return View(credentials);
            }
        }

        [HttpPost]
        public FileResult DownloadFile(int? fileId)
        {
            Credentials credentials = db.Credentials.ToList().Find(p => p.id == fileId.Value);
            string contentType = credentials.filename.Split('.').Last();
            return File(credentials.crendentialfile, contentType, credentials.filename);
        }

        [HttpPost]
        public ActionResult ExportCredentials()
        {
            List<CredentialsViewModel> credentialVM = (List<CredentialsViewModel>)TempData.Peek("CredentialsList");
            DataTable dt = new DataTable("Credentials");
            dt.Columns.AddRange(new DataColumn[12]
            {
                new DataColumn("Username"),
                new DataColumn("Password"),
                new DataColumn("title"),
                new DataColumn("URL"),
                new DataColumn("Port"),
                new DataColumn("Host/IP"),
                new DataColumn("Network Domain"),
                new DataColumn("Credential Level"),
                new DataColumn("Credential Category"),
                new DataColumn("Project Name"),
                new DataColumn("File Name"),
                new DataColumn("Comment")
            });
            foreach (CredentialsViewModel credential in credentialVM)
            {
                dt.Rows.Add(
                    credential.username,
                    credential.password,
                    credential.title,
                    credential.link,
                    credential.port,
                    credential.host,
                    credential.networkdomain,
                    credential.clevelname,
                    credential.ccategoryname,
                    credential.projectname,
                    credential.filename,
                    credential.comments
                );
            }

            using (XLWorkbook wb = new XLWorkbook())
            {
                IXLWorksheet wbDetail = wb.Worksheets.Add(dt, "Credentials");
                wbDetail.Columns().AdjustToContents();
                wbDetail.Rows().AdjustToContents();
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "Credentials.xlsx");
                }
            }
        }

        // GET: Credentials/Edit/5
        public ActionResult Edit(long? id, bool? IsProjectCredentials)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Credentials credentials = db.Credentials.Find(id);
            if (credentials == null)
            {
                return HttpNotFound();
            }

            byte[] salt = Convert.FromBase64String(credentials.passwordsalt).ToArray();
            credentials.password = EncryptionHelper.DecryptString(credentials.passwordhash, salt);
            ViewBag.credentialcategoryid =
                new SelectList(db.CredentialCategories, "id", "name", credentials.credentialcategoryid);
            ViewBag.credentiallevelid =
                new SelectList(db.Database.SqlQuery<CombinedEntity>("exec GetCredentialsLevel_sp").ToList(), "id",
                    "name", credentials.credentiallevelid);
            ViewBag.projectid = new SelectList(db.Project, "id", "name", credentials.projectid);
            ViewBag.crendentialtypeid = new SelectList(db.CredentialTypes, "id", "name", credentials.crendentialtypeid);
            credentials.ck = db.CredentialSkills.Where(ci => ci.credentailid == credentials.id).ToList();
            ViewBag.Readonly = !string.IsNullOrEmpty(credentials.linkedCredential);
            if (IsProjectCredentials == true)
            {
                ViewBag.IsProjectCredentials = true;
            }

            if (Request.IsAjaxRequest())
            {
                return PartialView("_EditCredentialForm", credentials);
            }

            return View(credentials);
        }

        // POST: Credentials/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit(
            [Bind(Include =
                "id,title,crendentialtypeid,credentiallevelid,projectid,credentialcategoryid,url,username,password,port,comments,host,networkdomain,createdonutc,updatedonutc,ipused,userid,linkedCredential,isactive")]
            Credentials credentials, bool? IsProjectCredentials, HttpPostedFileBase file, bool linkedCred = false)
        {
            try
            {
                if (file != null)
                {
                    string[] AllowedFileExtensions = { ".txt", ".ppk", ".pem" };
                    if (file.ContentLength > 102400)
                    {
                        ModelState.AddModelError("", "Your file is too large, maximum allowed size is: 100 KB");
                    }
                    else if (!AllowedFileExtensions.Contains(file.FileName.Substring(file.FileName.LastIndexOf('.'))))
                    {
                        ModelState.AddModelError("",
                            "Please file of type: " + string.Join(", ", AllowedFileExtensions));
                    }
                }

                if (!string.IsNullOrEmpty(credentials.linkedCredential))
                {
                    credentials.linkedCredential = credentials.linkedCredential.Split('/').Last();
                    if (int.TryParse(credentials.linkedCredential, out int id))
                    {
                        Credentials credential = db.Credentials.Find(id);
                        if (credential != null)
                        {
                            credentials.username = credential.username;
                            credentials.password = credential.password;
                            ModelState["username"].Errors.Clear();
                            ModelState["password"].Errors.Clear();
                        }
                        else
                        {
                            ModelState.AddModelError("linkedCredential", "Linked credential not found.");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("linkedCredential", "Please enter correct linked credential url");
                    }
                }

                if (ModelState.IsValid)
                {
                    credentials.username = credentials.username.Trim();
                    credentials.userid = User.Identity.GetUserId();
                    credentials.ipused = Request.UserHostAddress;
                    credentials.updatedonutc = DateTime.Now;
                    byte[] key = EncryptionHelper.GenerateKey();
                    credentials.passwordhash = EncryptionHelper.EncryptString(credentials.password, key);
                    credentials.passwordsalt = Convert.ToBase64String(key);
                    credentials.password = EncryptionHelper.GuidPassword();
                    if (file != null)
                    {
                        byte[] bytes;
                        using (BinaryReader br = new BinaryReader(file.InputStream))
                        {
                            bytes = br.ReadBytes(file.ContentLength);
                        }

                        credentials.crendentialfile = bytes;
                        credentials.filename = file.FileName;
                    }
                    else
                    {
                        Credentials credential = db.Credentials.Find(credentials.id);
                        db.Entry(credential).State = EntityState.Detached;
                        credentials.crendentialfile = credential.crendentialfile;
                        credentials.filename = credential.filename;
                    }

                    db.Entry(credentials).State = EntityState.Modified;
                    db.SaveChanges();
                    if (Request.IsAjaxRequest())
                    {
                        return Json(new { success = true });
                    }

                    if (IsProjectCredentials == true)
                    {
                        return RedirectToAction("ProjectCredentials", new { id = credentials.projectid });
                    }

                    if (User.IsInRole(Role.Admin.ToString()))
                    {
                        return RedirectToAction("Index");
                    }

                    return RedirectToAction("IndexDev");
                }

                ViewBag.credentialcategoryid = new SelectList(db.CredentialCategories, "id", "name",
                    credentials.credentialcategoryid);
                ViewBag.credentiallevelid =
                    new SelectList(db.Database.SqlQuery<CombinedEntity>("exec GetCredentialsLevel_sp").ToList(), "id",
                        "name", credentials.credentiallevelid);
                ViewBag.crendentialtypeid =
                    new SelectList(db.CredentialTypes, "id", "name", credentials.crendentialtypeid);
                ViewBag.projectid = new SelectList(db.Project, "id", "name", credentials.projectid);
                ViewBag.Readonly = linkedCred;
                if (linkedCred)
                {
                    ModelState["username"].Errors.Clear();
                    ModelState["password"].Errors.Clear();
                    if (string.IsNullOrEmpty(credentials.linkedCredential))
                    {
                        ModelState.AddModelError("linkedCredential", "Linked credential is required.");
                    }
                }

                credentials.ck = db.CredentialSkills.Where(ci => ci.credentailid == credentials.id).ToList();
                return View(credentials);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                ViewBag.credentialcategoryid = new SelectList(db.CredentialCategories, "id", "name",
                    credentials.credentialcategoryid);
                ViewBag.credentiallevelid =
                    new SelectList(db.Database.SqlQuery<CombinedEntity>("exec GetCredentialsLevel_sp").ToList(), "id",
                        "name", credentials.credentiallevelid);
                ViewBag.crendentialtypeid =
                    new SelectList(db.CredentialTypes, "id", "name", credentials.crendentialtypeid);
                ViewBag.projectid = new SelectList(db.Project, "id", "name", credentials.projectid);
                credentials.ck = db.CredentialSkills.Where(ci => ci.credentailid == credentials.id).ToList();
                return View(credentials);
            }
        }

        // GET: Credentials/Delete/5
        //public ActionResult Delete(long? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Credentials credentials = db.Credentials.Include(x => x.Project).Where(x => x.id == id).FirstOrDefault();
        //    if (credentials == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(credentials);
        //}

        //// POST: Credentials/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(long id)
        //{
        //    Credentials credentials = db.Credentials.Find(id);
        //    db.Credentials.Remove(credentials);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

        [Authorize(Roles = "Admin")]
        public ActionResult DeleteCredential(long id)
        {
            Credentials credentials = db.Credentials.Find(id);
            if (credentials != null)
            {
                db.Credentials.Remove(credentials);
                db.SaveChanges();
                return Json(true, JsonRequestBehavior.AllowGet);
            }

            return Json(false, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public ActionResult Encryption()
        {
            byte[] key = EncryptionHelper.GenerateKey();
            string plaintext = "this is the password";

            string encryptedpass = EncryptionHelper.EncryptString(plaintext, key);
            string decrptedpass = EncryptionHelper.DecryptString(encryptedpass, key);
            Random num = new Random();
            int x = num.Next();

            return Json(new { encrypt = encryptedpass, decrypt = decrptedpass }, JsonRequestBehavior.AllowGet);
        }

        public void ConvertAll()
        {
            List<Credentials> credentials = db.Credentials.ToList();
            foreach (Credentials credential in credentials)
            {
                if (string.IsNullOrEmpty(credential.passwordhash))
                {
                    try
                    {
                        byte[] key = EncryptionHelper.GenerateKey();
                        credential.passwordhash = EncryptionHelper.EncryptString(credential.password, key);
                        credential.passwordsalt = Convert.ToBase64String(key);
                        db.Entry(credential).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }
        }

        public void UpdateOldPassword()
        {
            List<Credentials> credentials = db.Credentials.ToList();
            foreach (Credentials credential in credentials)
            {
                if (!string.IsNullOrEmpty(credential.passwordhash))
                {
                    try
                    {
                        credential.password = EncryptionHelper.GuidPassword();
                        db.Entry(credential).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }
        }

        [HttpGet]
        public JsonResult GetPassword(long id)
        {
            try
            {
                Credentials credential = db.Credentials.Find(id);
                if (!string.IsNullOrEmpty(credential.linkedCredential))
                {
                    Credentials cred = db.Credentials.Find(int.Parse(credential.linkedCredential));
                    credential.passwordsalt = cred.passwordsalt;
                    credential.passwordhash = cred.passwordhash;
                }

                byte[] salt = Convert.FromBase64String(credential.passwordsalt).ToArray();
                string pass = EncryptionHelper.DecryptString(credential.passwordhash, salt);

                return Json(new { error = 0, password = pass, message = "Successfully" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { error = 1, password = "", message = e.Message }, JsonRequestBehavior.AllowGet);
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