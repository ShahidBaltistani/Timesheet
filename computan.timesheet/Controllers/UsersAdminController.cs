using computan.timesheet.core;
using computan.timesheet.core.common;
using computan.timesheet.Extensions;
using computan.timesheet.Helpers;
using computan.timesheet.Infrastructure;
using computan.timesheet.Models;
using computan.timesheet.Models.Rocket;
using computan.timesheet.Services.Rocket;
using computan.timesheet.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace computan.timesheet.Controllers
{
    [CustomeAuthorizeAttribute(Role.Admin)]
    public class UsersAdminController : BaseController
    {
        private ApplicationRoleManager _roleManager;

        private ApplicationUserManager _userManager;

        public UsersAdminController()
        {
        }

        public UsersAdminController(ApplicationUserManager userManager, ApplicationRoleManager roleManager)
        {
            UserManager = userManager;
            RoleManager = roleManager;
        }

        public ApplicationUserManager UserManager
        {
            get => _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            private set => _userManager = value;
        }

        public ApplicationRoleManager RoleManager
        {
            get => _roleManager ?? HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
            private set => _roleManager = value;
        }

        //
        // GET: /Users/
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult GetData(bool? filter)
        {
            try
            {
                List<UsersViewModels> users = db.Database.SqlQuery<UsersViewModels>("exec GetUsersListWithRole_sp " + filter).ToList();
                return Json(new
                {
                    data = users
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

        public ActionResult GetTask(string id)
        {
            DateTime start = DateTime.Now.Date.Add(TimeSpan.Parse("00:00:00"));
            DateTime end = DateTime.Now.Date.Add(TimeSpan.Parse("23:59:59"));
            List<AllTaskViewModel> tasks = (from t in db.Ticket
                                            join ti in db.TicketItem on t.id equals ti.ticketid
                                            join task in db.TicketItemLog on ti.id equals task.ticketitemid
                                            where task.assignedtousersid == id && t.statusid == 2
                                            orderby task.displayorder descending, task.assignedon descending
                                            select new AllTaskViewModel
                                            {
                                                assignedon = task.assignedon,
                                                id = ti.id,
                                                taskid = task.id,
                                                subject = ti.subject,
                                                uniquebody = ti.uniquebody,
                                                displayorder = task.displayorder,
                                                statusid = t.statusid,
                                                ticketid = t.id
                                            }).ToList();
            List<AllTaskViewModel> todaytasks = (from t in db.Ticket
                                                 join ti in db.TicketItem on t.id equals ti.ticketid
                                                 join task in db.TicketItemLog on ti.id equals task.ticketitemid
                                                 where task.assignedtousersid == id && t.statusid == 2 && t.statusupdatedon >= start &&
                                                       t.statusupdatedon <= end
                                                 orderby task.displayorder descending, task.assignedon descending
                                                 select new AllTaskViewModel
                                                 {
                                                     assignedon = task.assignedon,
                                                     id = ti.id,
                                                     taskid = task.id,
                                                     subject = ti.subject,
                                                     uniquebody = ti.uniquebody,
                                                     status = task.TicketStatus.name,
                                                     displayorder = task.displayorder
                                                 }).ToList();

            AdminTiaskbyUserViewModel model = new AdminTiaskbyUserViewModel
            {
                tasks = tasks,
                todaytasks = todaytasks,
                SearchTask = new TaskSearchFilterViewModel
                {
                    UsersCollection = new SelectList(db.Users.Where(i => i.isactive == true).OrderBy(u => u.FirstName)
                        .Select(u => new
                        {
                            Text = u.FirstName + " " + u.LastName + " - " + u.Email,
                            Value = u.Id
                        }).ToList(), "Value", "Text", id)
                }
            };
            ViewBag.ticketstatuslist = db.TicketStatus.Where(i => i.id != 1).ToList();
            return View(model);
        }

        [HttpPost]
        public ActionResult GetTaskAjax(string teamuserid, long ticketstatus)
        {
            List<AllTaskViewModel> tasks = (from t in db.Ticket
                                            join ti in db.TicketItem on t.id equals ti.ticketid
                                            join task in db.TicketItemLog on ti.id equals task.ticketitemid
                                            where task.assignedtousersid == teamuserid && t.statusid == ticketstatus
                                            orderby task.displayorder descending, task.assignedon descending
                                            select new AllTaskViewModel
                                            {
                                                assignedon = task.assignedon,
                                                id = ti.id,
                                                taskid = task.id,
                                                subject = ti.subject,
                                                uniquebody = ti.uniquebody,
                                                displayorder = task.displayorder,
                                                statusid = t.statusid,
                                                ticketid = t.id
                                            }).ToList();

            ViewBag.ticketstatuslist = db.TicketStatus.Where(i => i.id != 1).ToList();
            return PartialView("_GetTasklist", tasks);
        }

        [HttpPost]
        public ActionResult GetTodayTaskAjax(string teamuserid)
        {
            DateTime start = DateTime.Now.Date.Add(TimeSpan.Parse("00:00:00"));
            DateTime end = DateTime.Now.Date.Add(TimeSpan.Parse("23:59:59"));
            List<AllTaskViewModel> todaytasks = (from t in db.Ticket
                                                 join ti in db.TicketItem on t.id equals ti.ticketid
                                                 join task in db.TicketItemLog on ti.id equals task.ticketitemid
                                                 where task.assignedtousersid == teamuserid && t.statusid == 2 && t.statusupdatedon >= start &&
                                                       t.statusupdatedon <= end
                                                 orderby task.displayorder descending, task.assignedon descending
                                                 select new AllTaskViewModel
                                                 {
                                                     assignedon = task.assignedon,
                                                     id = ti.id,
                                                     taskid = task.id,
                                                     subject = ti.subject,
                                                     uniquebody = ti.uniquebody,
                                                     status = task.TicketStatus.name,
                                                     displayorder = task.displayorder
                                                 }).ToList();

            ViewBag.ticketstatuslist = db.TicketStatus.Where(i => i.id != 1).ToList();
            return PartialView("_TodayTasks", todaytasks);
        }

        [HttpPost]
        public ActionResult sortingrow(List<TaskSortingViewModel> model)
        {
            if (model == null || model.Count == 0)
            {
                return Json(new { error = true, errortext = "no data found." });
            }

            foreach (TaskSortingViewModel item in model)
            {
                TicketItemLog task = db.TicketItemLog.Where(r => r.id == item.id).FirstOrDefault();
                if (task != null)
                {
                    task.displayorder = item.displayorder;
                    db.Entry(task).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }

            return Json(new { success = true, successtext = "Done!" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ChangerUserStatus(int statusid, long taskid)
        {
            try
            {
                MangeTaskService Mts = new MangeTaskService();
                bool result = Mts.ChangeUserTaskStatus(statusid, taskid);
                if (result)
                {
                    return Json(new { success = true, successtext = "The user status has been updated." });
                }

                return Json(new { error = true, errortext = "The user status has not been updated." });
            }
            catch (Exception ex)
            {
                return Json(new { error = true, errortext = ex.Message });
            }
        }

        public ActionResult GetStates(int CountryId)
        {
            List<State> data = db.State.Where(x => x.countryid == CountryId).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        } 
        
        public ActionResult ResetTwoFA(string id)
        {
            var user = db.Users.Where(x => x.Id == id).FirstOrDefault();
            user.AppAuthenticatorSecretKey = null;
            user.IsAppAuthenticatorEnabled = false;
            user.EmailConfirmed = true;
            user.IsRocketAuthenticatorEnabled = true;
            db.SaveChanges();
            if (user != null)
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            return Json(false ,JsonRequestBehavior.AllowGet);
        }

        #region user Add/edit/reset paswword

        //
        // GET: /Users/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ApplicationUser user = await UserManager.FindByIdAsync(id);

            ViewBag.RoleNames = await UserManager.GetRolesAsync(user.Id);

            return View(user);
        }

        //
        // GET: /Users/Create
        public async Task<ActionResult> Create()
        {
            ViewBag.countrylist = new SelectList(db.Country, "id", "nicename");
            ViewBag.statelist = new SelectList(db.State, "id", "name");
            ViewBag.teams = new SelectList(db.Team, "id", "name");
            ViewBag.RoleId = new SelectList(await RoleManager.Roles.ToListAsync(), "Name", "Name");
            ViewBag.Skills = db.Skill.ToList();
            ViewBag.levels = new SelectList(db.Database.SqlQuery<CombinedEntity>("exec GetCredentialsLevel_sp").ToList(), "id","name");
            ViewBag.reported =  new SelectList(UserManager.Users.OrderBy(f => f.FirstName).Where(u => u.isactive == true).ToList(),  "Id", "NameEmail");
            ViewBag.TeamLead = new SelectList(CommonFunctions.TeamLeadList(), "Value", "Text");
            ViewBag.ProjectManager = new SelectList(CommonFunctions.ProjectManagerList(), "Value", "Text");

            ViewBag.ShiftTimePK = new SelectList(CommonFunctions.ShiftTimingsPKT(), "Value", "Text", "0");
            ViewBag.ShiftTimeEST = new SelectList(CommonFunctions.ShiftTimingsEST(), "Value", "Text", "0");
            return View();
        }
        //
        // POST: /Users/Create
        [HttpPost]
        public async Task<ActionResult> Create(RegisterViewModel userViewModel, HttpPostedFileBase filename, params string[] selectedRoles)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = new ApplicationUser();
                if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "/Images/UserProfileImage/"))
                {
                    Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "/Images/UserProfileImage/");
                }

                if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "/Images/UserProfileImage/deleted"))
                {
                    Directory.CreateDirectory(
                        AppDomain.CurrentDomain.BaseDirectory + "/Images/UserProfileImage/deleted");
                }

                HttpPostedFileBase postedFile = Request.Files[0];
                HttpPostedFileBase LatestResume = Request.Files[1];
                HttpPostedFileBase lastDegree = Request.Files[2];
                HttpPostedFileBase CNIC_front = Request.Files[3];
                HttpPostedFileBase CNIC_back = Request.Files[4];
                HttpPostedFileBase experiencLetter = Request.Files[5];
                string imagapath = ConfigurationManager.AppSettings["userimagepath"];
                
                if (postedFile.ContentLength > 0)
                {
                    if (postedFile.ContentType.ToLower() != "image/jpg" &&
                        postedFile.ContentType.ToLower() != "image/png" &&
                        postedFile.ContentType.ToLower() != "image/gif" &&
                        postedFile.ContentType.ToLower() != "image/jpeg")
                    {
                        ModelState.AddModelError("customer.profileimage",
                            "Sorry, file format must be jpg or jpeg or png or gif");
                    }

                    if (ModelState.IsValid)
                    {
                        string deletedimagepath = ConfigurationManager.AppSettings["deleteduserimagepath"];
                        string oldfile = user.ProfileImage;
                        string oldfileExt = Path.GetExtension(oldfile);
                        string fileExt = Path.GetExtension(postedFile.FileName);
                        string oldfileName = user.ProfileImage;
                        string oldpath = Path.Combine(Server.MapPath(imagapath) + oldfileName + oldfileExt);
                        string fileName = user.Id;
                        string deleltedfielname = fileName + DateTime.Now.ToString("yyyyMMddHHmmssfff");
                        string path = Path.Combine(Server.MapPath(imagapath) + fileName + fileExt);
                        deletedimagepath = Path.Combine(Server.MapPath(deletedimagepath + deleltedfielname + fileExt));
                        if (System.IO.File.Exists(oldpath))
                        {
                            System.IO.File.Move(oldpath, deletedimagepath);
                        }

                        postedFile.SaveAs(path);
                        user.ProfileImage = fileName + fileExt;
                    }
                }
                if (LatestResume.ContentLength > 0)
                {
                    string file = Path.GetFileNameWithoutExtension(LatestResume.FileName);
                    string fileExt = Path.GetExtension(LatestResume.FileName);
                    string newFileName = file + DateTime.Now.ToString("yyyyMMddHHmmssff") + fileExt;
                    string path = Path.Combine(Server.MapPath(imagapath) + newFileName);
                    LatestResume.SaveAs(path);
                    user.LatestResume = newFileName;
                }
                if (lastDegree.ContentLength > 0)
                {
                    string file = Path.GetFileNameWithoutExtension(lastDegree.FileName);
                    string fileExt = Path.GetExtension(lastDegree.FileName);
                    string newFileName = file + DateTime.Now.ToString("yyyyMMddHHmmssff") + fileExt;
                    string path = Path.Combine(Server.MapPath(imagapath) + newFileName);
                    lastDegree.SaveAs(path);
                    user.LastDegree = newFileName;
                }
                if (CNIC_front.ContentLength > 0)
                {
                    string file = Path.GetFileNameWithoutExtension(CNIC_front.FileName);
                    string fileExt = Path.GetExtension(CNIC_front.FileName);
                    string newFileName = file + DateTime.Now.ToString("yyyyMMddHHmmssff") + fileExt;
                    string path = Path.Combine(Server.MapPath(imagapath) + newFileName);
                    CNIC_front.SaveAs(path);
                    user.CNIC_Front = newFileName;
                }
                if (CNIC_back.ContentLength > 0)
                {
                    string file = Path.GetFileNameWithoutExtension(CNIC_back.FileName);
                    string fileExt = Path.GetExtension(CNIC_back.FileName);
                    string newFileName = file + DateTime.Now.ToString("yyyyMMddHHmmssff") + fileExt;
                    string path = Path.Combine(Server.MapPath(imagapath) + newFileName);
                    CNIC_back.SaveAs(path);
                    user.CNIC_Back = newFileName;
                }
                if (experiencLetter.ContentLength > 0)
                {
                    string file = Path.GetFileNameWithoutExtension(experiencLetter.FileName);
                    string fileExt = Path.GetExtension(experiencLetter.FileName);
                    string newFileName = file + DateTime.Now.ToString("yyyyMMddHHmmssff") + fileExt;
                    string path = Path.Combine(Server.MapPath(imagapath) + newFileName);
                    experiencLetter.SaveAs(path);
                    user.ExperienceLetter = newFileName;
                }

                user.UserName = userViewModel.Email;
                user.Email = userViewModel.Email;
                user.FirstName = userViewModel.FirstName;
                user.LastName = userViewModel.LastName;
                user.Address = userViewModel.Address;
                user.City = userViewModel.City;
                user.StateId = userViewModel.StateId;
                user.CountryId = userViewModel.CountryId;
                user.Levelid = userViewModel.Levelid;
                user.Zip = userViewModel.Zip;
                user.Phone = userViewModel.Phone;
                user.Mobile = userViewModel.Mobile;
                user.Designation = userViewModel.Designation;
                user.Skypeid = userViewModel.Skypeid;
                user.isactive = true;
                user.createdonutc = DateTime.Now;
                user.updatedonutc = DateTime.Now;
                user.ipused = Request.UserHostAddress;
                user.userid = User.Identity.GetUserId();
                user.DateOfBirth = userViewModel.DateOfBirth;
                user.NationalIdentificationNumber = userViewModel.NationalIdentificationNumber;
                user.PersonalEmailAddress = userViewModel.PersonalEmailAddress;
                user.PersonNameEmergency = userViewModel.PersonNameEmergency;
                user.EmergencyPhoneNumber = userViewModel.EmergencyPhoneNumber;
                user.SpouseName = userViewModel.SpouseName;
                user.SpouseDateOfBirth = userViewModel.SpouseDateOfBirth;
                user.ChildrenNames = userViewModel.ChildrenNames;
                user.DateOfJoining = userViewModel.DateOfJoining;
                user.Experience = userViewModel.Experience;
                user.AccountNumber = userViewModel.AccountNumber;
                user.BranchName = userViewModel.BranchName;
                user.TeamLead = userViewModel.TeamLead;
                user.ProjectManager = userViewModel.ProjectManager;
                user.IsPkHoliday = userViewModel.IsPkHoliday;
                user.IsRemoteJob = userViewModel.IsRemoteJob;
                user.ShiftTimePK = userViewModel.ShiftTimePK;
                user.ShiftTimeEST = userViewModel.ShiftTimeEST;
                IdentityResult adminresult = await UserManager.CreateAsync(user, userViewModel.Password);

                if (adminresult.Succeeded)
                {
                    MessageViewModel message1 = new MessageViewModel()
                    {
                        IsSuccess = true,
                        Message = "User created in Timesheet successfully"
                    };
                    TempData["isTimesheetUserCreated"] = JsonConvert.SerializeObject(message1);
                    ApplicationUser usersByEmail = UserManager.FindByEmail(userViewModel.Email);
                    TeamMember teamMember = new TeamMember
                    {
                        teamid = userViewModel.teamId,
                        usersid = usersByEmail.Id,
                        Reported = userViewModel.Reported,
                        IsTeamLead = selectedRoles.Contains("TeamLead"),
                        IsManager = selectedRoles.Contains("Admin"),
                        IsActive = true,
                        createdonutc = DateTime.Now,
                        updatedonutc = DateTime.Now,
                        ipused = Request.UserHostAddress,
                        userid = User.Identity.GetUserId()
                    };
                    db.TeamMember.Add(teamMember);
                    db.SaveChanges();
                    //Assign general tickets
                    db.Database.ExecuteSqlCommand("CommonTicket_sp @userId, @Active",
                        new SqlParameter("userId", usersByEmail.Id),
                        new SqlParameter("Active", 1)
                    );

                    if (userViewModel.skills != null && userViewModel.skills.Count > 0)
                    {
                        foreach (long items in userViewModel.skills)
                        {
                            UserSkills us = new UserSkills
                            {
                                skillid = items,
                                userid = user.Id
                            };
                            db.UserSkills.Add(us);
                            db.SaveChanges();
                        }
                    }

                    if (selectedRoles != null)
                    {
                        IdentityResult result = await UserManager.AddToRolesAsync(user.Id, selectedRoles);
                        if (!result.Succeeded)
                        {
                            ModelState.AddModelError("", result.Errors.First());
                            ViewBag.RoleId = new SelectList(await RoleManager.Roles.ToListAsync(), "Name", "Name");
                            return View();
                        }
                    }
                }
                else
                {
                    ViewBag.countrylist = new SelectList(db.Country, "id", "nicename");
                    ViewBag.statelist = new SelectList(db.State, "id", "name");
                    foreach (string res in adminresult.Errors)
                    {
                        ModelState.AddModelError("", res);
                    }

                    ViewBag.RoleId = new SelectList(RoleManager.Roles, "Name", "Name");
                    ViewBag.Skills = db.Skill.ToList();
                    ViewBag.teams = new SelectList(db.Team, "id", "name");
                    ViewBag.levels =
                        new SelectList(db.Database.SqlQuery<CombinedEntity>("exec GetCredentialsLevel_sp").ToList(),
                            "id", "name");
                    ViewBag.TeamLead = new SelectList(CommonFunctions.TeamLeadList(), "Value", "Text");
                    ViewBag.ProjectManager = new SelectList(CommonFunctions.ProjectManagerList(), "Value", "Text");
                    ViewBag.ShiftTimePK = new SelectList(CommonFunctions.ShiftTimingsPKT(), "Value", "Text", "0");
                    ViewBag.ShiftTimeEST = new SelectList(CommonFunctions.ShiftTimingsEST(), "Value", "Text", "0");
                    return View();
                }
                string rocketUser = CreateRocketUser(userViewModel);
                    MessageViewModel message = new MessageViewModel()
                    {
                        IsSuccess = rocketUser == APIResponse.OK.ToString() ? true : false,
                        Message = rocketUser == APIResponse.OK.ToString() ? "User created in Rocket successfully" :
                            rocketUser == APIResponse.BadRequest.ToString() ? "Username already exists in Rocket" :
                            rocketUser == APIResponse.Unauthorized.ToString() ? "You do not have the necessary permissions" :
                            "Something went wrong, please try again"
                    };

                TempData["isRocketUserCreated"] = JsonConvert.SerializeObject(message);

                return RedirectToAction("Index");
            }

            ViewBag.countrylist = new SelectList(db.Country, "id", "nicename");
            ViewBag.statelist = new SelectList(db.State, "id", "name");
            ViewBag.RoleId = new SelectList(RoleManager.Roles, "Name", "Name");
            ViewBag.Skills = db.Skill.ToList();
            ViewBag.teams = new SelectList(db.Team, "id", "name");
            ViewBag.levels =
                new SelectList(db.Database.SqlQuery<CombinedEntity>("exec GetCredentialsLevel_sp").ToList(), "id",
                    "name");
            ViewBag.TeamLead = new SelectList(CommonFunctions.TeamLeadList(), "Value", "Text");
            ViewBag.TeamLead = new SelectList(CommonFunctions.TeamLeadList(), "Value", "Text");
            ViewBag.ProjectManager = new SelectList(CommonFunctions.ProjectManagerList(), "Value", "Text");
            ViewBag.ShiftTimePK = new SelectList(CommonFunctions.ShiftTimingsPKT(), "Value", "Text", "0");
            ViewBag.ShiftTimeEST = new SelectList(CommonFunctions.ShiftTimingsEST(), "Value", "Text", "0");
            return View();
        }
        [AllowAnonymous]
        public FileResult DownloadFile(string fileName)
        {
            if (fileName != null)
            {
                string path = Server.MapPath("~/Images/UserProfileImage/") + fileName;
                byte[] bytes = System.IO.File.ReadAllBytes(path);
                return File(bytes, "application/octet-stream", fileName);
            }
            return null;
            
        }
        
        //
        // GET: /Users/Edit/1
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ApplicationUser user = await UserManager.FindByIdAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            DateTime? D = user.DateOfBirth.GetValueOrDefault().Date;
            IList<string> userRoles = await UserManager.GetRolesAsync(user.Id);
            List<UserSkills> userskills = db.UserSkills.Where(ui => ui.userid == user.Id).ToList();
            List<long> userskillid = new List<long>();
            foreach (UserSkills items in userskills)
            {
                userskillid.Add(items.skillid);
            }

            ViewBag.countrylist = new SelectList(db.Country, "id", "nicename", user.CountryId);
            ViewBag.statelist = new SelectList(db.State, "id", "name", user.StateId);
            ViewBag.levels =
                new SelectList(db.Database.SqlQuery<CombinedEntity>("exec GetCredentialsLevel_sp").ToList(), "id",
                    "name", user.Levelid);
            string repotedId = db.TeamMember.Where(x => x.usersid == user.Id).Select(x => x.Reported).FirstOrDefault();
            //ViewBag.RepotedTo =
            //    new SelectList(UserManager.Users.OrderBy(f => f.FirstName).Where(u => u.isactive == true), "Id",
            //        "NameEmail", repotedId);

            ViewBag.ShiftTimePK = new SelectList(CommonFunctions.ShiftTimingsPKT(), "Value", "Text", user.ShiftTimePK);
            ViewBag.ShiftTimeEST = new SelectList(CommonFunctions.ShiftTimingsEST(), "Value", "Text", user.ShiftTimeEST);
            ViewBag.TeamLead = new SelectList(CommonFunctions.TeamLeadList(), "Value", "Text",user.TeamLead);
            ViewBag.ProjectManager = new SelectList(CommonFunctions.ProjectManagerList(), "Value", "Text", user.ProjectManager);
            return View(new EditUserViewModel
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Address = user.Address,
                City = user.City,
                StateId = user.StateId,
                CountryId = user.CountryId,
                Levelid = user.Levelid,
                Zip = user.Zip,
                Phone = user.Phone,
                Mobile = user.Mobile,
                Designation = user.Designation,
                Skypeid = user.Skypeid,
                isactive = user.isactive,
                ProfileImage = user.ProfileImage,
                //Later Changes
                DateOfBirth = user.DateOfBirth?.ToString("MM-dd-yyyy"),
                NationalIdentificationNumber = user.NationalIdentificationNumber,
                PersonalEmailAddress = user.PersonalEmailAddress,
                PersonNameEmergency = user.PersonNameEmergency,
                EmergencyPhoneNumber = user.EmergencyPhoneNumber,
                SpouseName = user.SpouseName,
                SpouseDateOfBirth = user.SpouseDateOfBirth?.ToString("MM-dd-yyyy"),
                ChildrenNames = user.ChildrenNames,
                DateOfJoining = user.DateOfJoining?.ToString("MM-dd-yyyy"),
                Experience = user.Experience,
                AccountNumber = user.AccountNumber,
                BranchName = user.BranchName,
                IsPkHoliday = user.IsPkHoliday,
                IsRemoteJob = user.IsRemoteJob,
                //Documents
                LatestResume = user.LatestResume,
                ExperienceLetter = user.ExperienceLetter,
                CNIC_Front = user.CNIC_Front,
                CNIC_Back = user.CNIC_Back,
                LastDegree = user.LastDegree,
                RolesList = RoleManager.Roles.ToList().Select(x => new SelectListItem
                {
                    Selected = userRoles.Contains(x.Name),
                    Text = x.Name,
                    Value = x.Name
                }),
                skills = db.Skill.ToList().Select(x => new SelectListItem
                {
                    Selected = userskillid.Contains(x.id),
                    Text = x.name,
                    Value = x.id.ToString()
                }) 
        });
        }

        //
        // POST: /Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(EditUserViewModel editUser, HttpPostedFileBase filename,
            params string[] selectedRole)
        {
            if (ModelState.IsValid)
            {
                string oldUsername = string.Empty;
                ApplicationUser user = await UserManager.FindByIdAsync(editUser.Id);
                if (user == null)
                {
                    return HttpNotFound();
                }
                // check isActive before update the user
                //bool IsChanged = false;
                //if (user.isactive != editUser.isactive)
                //{
                //    IsChanged = true;
                //}
                oldUsername = user.Email;
                bool oldStatus = user.isactive;
                user.UserName = editUser.Email;
                user.Email = editUser.Email;
                user.FirstName = editUser.FirstName;
                user.LastName = editUser.LastName;
                user.Address = editUser.Address;
                user.City = editUser.City;
                user.StateId = editUser.StateId;
                user.CountryId = editUser.CountryId;
                user.Levelid = editUser.Levelid;
                user.Zip = editUser.Zip;
                user.Phone = editUser.Phone;
                user.Mobile = editUser.Mobile;
                user.Designation = editUser.Designation;
                user.Skypeid = editUser.Skypeid;
                user.isactive = editUser.isactive;
                user.updatedonutc = DateTime.Now;
                user.ipused = Request.UserHostAddress;
                user.userid = User.Identity.GetUserId();
                user.NationalIdentificationNumber = editUser.NationalIdentificationNumber;
                user.PersonalEmailAddress = editUser.PersonalEmailAddress;
                user.PersonNameEmergency = editUser.PersonNameEmergency;
                user.EmergencyPhoneNumber = editUser.EmergencyPhoneNumber;
                user.SpouseName = editUser.SpouseName;
                
                if (editUser.SpouseDateOfBirth != null)
                {
                    user.SpouseDateOfBirth = DateTime.Parse(editUser.SpouseDateOfBirth);
                }
                else
                {
                    user.SpouseDateOfBirth = null;
                } 
                if (editUser.DateOfBirth != null)
                {
                    user.DateOfBirth = DateTime.Parse(editUser.DateOfBirth);
                }
                else
                {
                    user.DateOfBirth = null;
                } 
                
                if (editUser.DateOfJoining != null)
                {
                    user.DateOfJoining = DateTime.Parse(editUser.DateOfJoining);
                }
                else
                {
                    user.DateOfJoining = null;
                }

                user.ChildrenNames = editUser.ChildrenNames;
                user.Experience = editUser.Experience;
                user.AccountNumber = editUser.AccountNumber;
                user.BranchName = editUser.BranchName;
                user.IsPkHoliday = editUser.IsPkHoliday;
                user.IsRemoteJob = editUser.IsRemoteJob;
                user.ShiftTimePK = editUser.ShiftTimePK;
                user.ShiftTimeEST = editUser.ShiftTimeEST;
                user.TeamLead=editUser.TeamLead;
                user.ProjectManager = editUser.ProjectManager;
                if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "/Images/UserProfileImage/"))
                {
                    Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "/Images/UserProfileImage/");
                }

                if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "/Images/UserProfileImage/deleted"))
                {
                    Directory.CreateDirectory(
                        AppDomain.CurrentDomain.BaseDirectory + "/Images/UserProfileImage/deleted");
                }
                HttpPostedFileBase postedFile = Request.Files[0];
                HttpPostedFileBase LatestResume = Request.Files[1];
                HttpPostedFileBase lastDegree = Request.Files[2];
                HttpPostedFileBase CNIC_front = Request.Files[3];
                HttpPostedFileBase CNIC_back = Request.Files[4];
                HttpPostedFileBase experiencLetter = Request.Files[5];
                string imagapath = ConfigurationManager.AppSettings["userimagepath"];
             
                if (Request.Files.Count > 0)
                {
                    //HttpPostedFileBase postedFile = Request.Files[0];
                    if (postedFile.ContentLength > 0)
                    {
                        if (postedFile.ContentType.ToLower() != "image/jpg" &&
                            postedFile.ContentType.ToLower() != "image/png" &&
                            postedFile.ContentType.ToLower() != "image/gif" &&
                            postedFile.ContentType.ToLower() != "image/jpeg")
                        {
                            ModelState.AddModelError("user.ProfileImage",
                                "Sorry, file format must be jpg or jpeg or png or gif");
                        }

                        if (ModelState.IsValid)
                        {
                            string deletedimagepath = ConfigurationManager.AppSettings["deleteduserimagepath"];
                            string oldfile = user.ProfileImage;
                            //string oldfileExt = Path.GetExtension(oldfile);
                            string fileExt = Path.GetExtension(postedFile.FileName);
                            string oldfileName = user.ProfileImage;
                            string oldpath = Path.Combine(Server.MapPath(imagapath) + oldfileName );
                            string fileName = user.Id;
                            string deleltedfielname = fileName + DateTime.Now.ToString("yyyyMMddHHmmssfff");
                            string path = Path.Combine(Server.MapPath(imagapath) + fileName + fileExt);
                            deletedimagepath = Path.Combine(Server.MapPath(deletedimagepath + deleltedfielname + fileExt));
                            if (System.IO.File.Exists(oldpath))
                            {
                                System.IO.File.Move(oldpath, deletedimagepath);
                            }

                            postedFile.SaveAs(path);
                            user.ProfileImage = fileName + fileExt;
                        }
                    }
                    if (LatestResume.ContentLength > 0)
                    {
                        string file = Path.GetFileNameWithoutExtension(LatestResume.FileName);
                        string fileExt = Path.GetExtension(LatestResume.FileName);
                        string newFileName = file + DateTime.Now.ToString("yyyyMMddHHmmssff") + fileExt;
                        string path = Path.Combine(Server.MapPath(imagapath) + newFileName);
                        LatestResume.SaveAs(path);
                        user.LatestResume = newFileName;
                    }
                    if (lastDegree.ContentLength > 0)
                    {
                        string file = Path.GetFileNameWithoutExtension(lastDegree.FileName);
                        string fileExt = Path.GetExtension(lastDegree.FileName);
                        string newFileName = file + DateTime.Now.ToString("yyyyMMddHHmmssff") + fileExt;
                        string path = Path.Combine(Server.MapPath(imagapath) + newFileName);
                        lastDegree.SaveAs(path);
                        user.LastDegree = newFileName;
                    }
                    if (CNIC_front.ContentLength > 0)
                    {
                        string file = Path.GetFileNameWithoutExtension(CNIC_front.FileName);
                        string fileExt = Path.GetExtension(CNIC_front.FileName);
                        string newFileName = file + DateTime.Now.ToString("yyyyMMddHHmmssff") + fileExt;
                        string path = Path.Combine(Server.MapPath(imagapath) + newFileName);
                        CNIC_front.SaveAs(path);
                        user.CNIC_Front = newFileName;
                    }
                    if (CNIC_back.ContentLength > 0)
                    {
                        string file = Path.GetFileNameWithoutExtension(CNIC_back.FileName);
                        string fileExt = Path.GetExtension(CNIC_back.FileName);
                        string newFileName = file + DateTime.Now.ToString("yyyyMMddHHmmssff") + fileExt;
                        string path = Path.Combine(Server.MapPath(imagapath) + newFileName);
                        CNIC_back.SaveAs(path);
                        user.CNIC_Back = newFileName;
                    }
                    if (experiencLetter.ContentLength > 0)
                    {
                        string file = Path.GetFileNameWithoutExtension(experiencLetter.FileName);
                        string fileExt = Path.GetExtension(experiencLetter.FileName);
                        string newFileName = file + DateTime.Now.ToString("yyyyMMddHHmmssff") + fileExt;
                        string path = Path.Combine(Server.MapPath(imagapath) + newFileName);
                        experiencLetter.SaveAs(path);
                        user.ExperienceLetter = newFileName;
                    }
                }
                IdentityResult updateuser = await UserManager.UpdateAsync(user);
                
                MessageViewModel message = new MessageViewModel()
                {
                    IsSuccess = updateuser.Succeeded,
                    Message = updateuser.Succeeded? "User modified in Timesheet successfully": "Something went wrong in Timesheet please try again"
                };
                TempData["isTimesheetUserCreated"] = JsonConvert.SerializeObject(message);
                //if (IsChanged)
                //{
                editUser.OldEmail = oldUsername;
                    string editRocketUser = ChangeRocketUserStatus(editUser);
                    MessageViewModel updatedMessage = new MessageViewModel()
                    {
                        IsSuccess = editRocketUser == APIResponse.OK.ToString() ?true : false,
                        Message = editRocketUser == APIResponse.OK.ToString() ? "User modified in Rocket successfully" :
                            editRocketUser == APIResponse.Unauthorized.ToString() ? "You do not have the necessary permissions. " :
                            editRocketUser == APIResponse.NotFound.ToString() ? "User not found in Rocket " :
                            editRocketUser == APIResponse.BadRequest.ToString() ? "User not found in Rocket " :
                            "Something went wrong please try again"
                    };
                    TempData["isRocketUserCreated"] = JsonConvert.SerializeObject(updatedMessage);
                //}

                List<TeamMember> tm = db.TeamMember.Where(x => x.usersid == editUser.Id).ToList();
                foreach (TeamMember teamMember in tm)
                {
                    teamMember.IsActive = editUser.isactive;
                    teamMember.IsTeamLead = selectedRole.Contains("TeamLead");
                    teamMember.Reported = editUser.RepotedId;
                    teamMember.IsManager = selectedRole.Contains("Admin");
                    db.Entry(teamMember).State = EntityState.Modified;
                    db.SaveChanges();
                }

                //Assign or remove general tickets
                if (oldStatus != editUser.isactive)
                {
                    db.Database.ExecuteSqlCommand("CommonTicket_sp @userId, @Active",
                        new SqlParameter("userId", editUser.Id),
                        new SqlParameter("Active", editUser.isactive));
                }

                string ismyprofile = Request.Form["myprofileid"];
                if (ismyprofile == "yes")
                {
                    return RedirectToAction("myprofile", "home");
                }

                IList<string> userRoles = await UserManager.GetRolesAsync(user.Id);

                selectedRole = selectedRole ?? new string[] { };

                IdentityResult result = await UserManager.AddToRolesAsync(user.Id, selectedRole.Except(userRoles).ToArray());

                if (!result.Succeeded)
                {
                    ViewBag.countrylist = new SelectList(db.Country, "id", "nicename");
                    ViewBag.statelist = new SelectList(db.State, "id", "name");
                    ViewBag.levels =
                        new SelectList(db.Database.SqlQuery<CombinedEntity>("exec GetCredentialsLevel_sp").ToList(),
                            "id", "name", user.Levelid);
                    ModelState.AddModelError("", result.Errors.First());
                    return View();
                }

                result = await UserManager.RemoveFromRolesAsync(user.Id, userRoles.Except(selectedRole).ToArray());

                if (!result.Succeeded)
                {
                    ViewBag.countrylist = new SelectList(db.Country, "id", "nicename");
                    ViewBag.statelist = new SelectList(db.State, "id", "name");
                    ViewBag.levels =
                        new SelectList(db.Database.SqlQuery<CombinedEntity>("exec GetCredentialsLevel_sp").ToList(),
                            "id", "name", user.Levelid);
                    ModelState.AddModelError("", result.Errors.First());
                    return View();
                }

                if (result.Succeeded)
                {
                    List<UserSkills> userskills = db.UserSkills.Where(ui => ui.userid == user.Id).ToList();
                    foreach (UserSkills item in userskills)
                    {
                        db.UserSkills.Remove(item);
                        db.SaveChanges();
                    }

                    if (Request.Form["userskills"] != null)
                    {
                        string userkills = Request.Form["userskills"];
                        string[] values = userkills.Split(',');
                        for (int i = 0; i < values.Length; i++)
                        {
                            values[i] = values[i].Trim();
                            string sid = "";
                            if (!string.IsNullOrEmpty(values[i]))
                            {
                                sid = values[i];
                            }

                            UserSkills us = new UserSkills
                            {
                                skillid = Convert.ToInt64(sid),
                                userid = user.Id
                            };
                            db.UserSkills.Add(us);
                            db.SaveChanges();
                        }
                    }
                }

                return RedirectToAction("Index");
            }
            var errors = ModelState.Select(x => x.Value.Errors)
                          .Where(y => y.Count > 0)
                          .ToList();
            string Ismyprofile = Request.Form["myprofileid"];
            if (Ismyprofile == "yes")
            {
                ModelState.AddModelError("", "All Fields with * are required.");
                return RedirectToAction("myprofile", "home");
            }

            return RedirectToAction("Edit", new { id = editUser.Id });
        }

        
        private string ChangeRocketUserStatus(EditUserViewModel editUser)
        {
            try
            {
                var rocketSetting = db.integration.Where(x => x.name == "Rocket").FirstOrDefault();
                RocketSetting setting = JsonConvert.DeserializeObject<RocketSetting>(rocketSetting.appsettings);
                string modifiedUser =  RocketService.ChangeUserStatus(setting, editUser);
                return modifiedUser;
            }
            catch (Exception ex)
            {
                System.Web.Routing.RouteData rd = ControllerContext.RouteData;
                string currentAction = rd.GetRequiredString("action");
                string currentController = rd.GetRequiredString("controller");
                MyExceptions myex = new MyExceptions
                {
                    action = currentAction,
                    exceptiondate = DateTime.Now,
                    controller = currentController,
                    exception_message = ex.Message,
                    exception_source = ex.Source,
                    exception_stracktrace = ex.StackTrace,
                    exception_targetsite = ex.TargetSite.DeclaringType.FullName + ", " + ex.TargetSite.Name,
                    ipused = Request.UserHostAddress,
                    userid = User.Identity.GetUserId()
                };
                db.MyExceptions.Add(myex);
                db.SaveChanges();
                return null;
            }
        }



        private string CreateRocketUser(RegisterViewModel model)
        {
            try
            {
                string username = model.Email.Split('@')[0];
                var userExist = RocketService.GetRocketUserId(GetRocketSettings(), username);
                if (userExist != APIResponse.OK.ToString())
                {
                    string isUserCreated = RocketService.CreateUser(GetRocketSettings(), model);
                   return isUserCreated;
                }
                else if (userExist == APIResponse.Unauthorized.ToString())
                {
                    return userExist;
                }
                else
                {
                    return "BadRequest";
                }
            }
            catch (Exception ex)
            {
                System.Web.Routing.RouteData rd = ControllerContext.RouteData;
                string currentAction = rd.GetRequiredString("action");
                string currentController = rd.GetRequiredString("controller");
                MyExceptions myex = new MyExceptions
                {
                    action = currentAction,
                    exceptiondate = DateTime.Now,
                    controller = currentController,
                    exception_message = ex.Message,
                    exception_source = ex.Source,
                    exception_stracktrace = ex.StackTrace,
                    exception_targetsite = ex.TargetSite.DeclaringType.FullName + ", " + ex.TargetSite.Name,
                    ipused = Request.UserHostAddress,
                    userid = User.Identity.GetUserId()
                };
                db.MyExceptions.Add(myex);
                db.SaveChanges();
                return null;
            }
        }

        public RocketSetting GetRocketSettings()
        {
            var rocketSetting = db.integration.Where(x => x.name == "Rocket").FirstOrDefault();
            return JsonConvert.DeserializeObject<RocketSetting>(rocketSetting.appsettings);
        }
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ApplicationUser user = await UserManager.FindByIdAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }

            return View(user);
        }

        //
        // POST: /Users/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            if (ModelState.IsValid)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                ApplicationUser user = await UserManager.FindByIdAsync(id);
                if (user == null)
                {
                    return HttpNotFound();
                }

                IdentityResult result = await UserManager.DeleteAsync(user);
                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First());
                    return View();
                }

                return RedirectToAction("Index");
            }

            return View();
        }

        // GET: /UserAdmin/ResetPassword
        public ActionResult ResetPassword(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ResetUserPasswordViewModel model = new ResetUserPasswordViewModel { Id = id };
            return View(model);
        }

        //
        // POST: /UserAdmin/ResetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetUserPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            IdentityResult removePassword = UserManager.RemovePassword(model.Id);
            if (removePassword.Succeeded)
            {
                //Removed Password Success
                IdentityResult validPass = await UserManager.PasswordValidator.ValidateAsync(model.NewPassword);
                if (validPass.Succeeded)
                {
                    IdentityResult AddPassword = UserManager.AddPassword(model.Id, model.NewPassword);
                    if (AddPassword.Succeeded)
                    {
                        return View("PasswordResetConfirm");
                    }
                }
                else
                {
                    string errors = string.Empty;
                    foreach (string items in validPass.Errors)
                    {
                        errors += items;
                    }

                    ModelState.AddModelError("", errors);
                    return View(model);
                }
            }

            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        #endregion user Add/edit/reset paswword
    }
}