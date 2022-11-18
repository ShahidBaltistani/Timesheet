using computan.timesheet.core;
using computan.timesheet.core.common;
using computan.timesheet.core.OrphanTickets;
using computan.timesheet.Helpers;
using computan.timesheet.Models;
using computan.timesheet.Models.Settings;
using Hangfire;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace computan.timesheet.Controllers
{
    [CustomeAuthorize]
    public class SettingsController : BaseController
    {
        // GET: Integrations
        private ApplicationUserManager _userManager;

        public ApplicationUserManager UserManager
        {
            get => _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            private set => _userManager = value;
        }

        // GET: Setting
        [CustomeAuthorize(Role.Admin, Role.TeamLead)]
        public ActionResult TimeEntry()
        {
            ViewBag.userlist = db.Users.Where(u => u.isactive).ToList();

            return View();
        }

        public ActionResult AllowUser(string userid)
        {
            if (!string.IsNullOrEmpty(userid))
            {
                try
                {
                    db.Database.ExecuteSqlCommand(
                        "TimeEntryRestriction_sp @isRestricted, @unrestricteduserid, @ipUsed, @userId",
                        new SqlParameter("isRestricted", true),
                        new SqlParameter("unrestricteduserid", userid),
                        new SqlParameter("ipUsed", Request.UserHostAddress),
                        new SqlParameter("userId", User.Identity.GetUserId())
                    );

                    string jobId = BackgroundJob.Schedule(
                        () => RestrictUser(userid),
                        TimeSpan.FromHours(3));

                    // TODO: Add Log and send notification
                    AddNotification(9, userid);
                }
                catch (Exception ex)
                {
                    return Json(new { error = 1, errortext = ex.Message }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { error = 0, errortext = "Successfully!" }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { error = 1, errortext = "No user found!" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult RestrictUser(string userid)
        {
            if (!string.IsNullOrEmpty(userid))
            {
                try
                {
                    db.Database.ExecuteSqlCommand(
                        "TimeEntryRestriction_sp @isRestricted, @unrestricteduserid, @ipUsed, @userId",
                        new SqlParameter("isRestricted", false),
                        new SqlParameter("unrestricteduserid", userid),
                        new SqlParameter("ipUsed", DBNull.Value),
                        new SqlParameter("userId", DBNull.Value)
                    );

                    // TODO: Add Log and send notification
                    //AddNotification(10, userid);
                }
                catch (Exception ex)
                {
                    return Json(new { error = 1, errortext = ex.Message }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { error = 0, errortext = "Successfully!" }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { error = 1, errortext = "No user found!" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult RestrictEnabledLog(string userid)
        {
            if (!string.IsNullOrEmpty(userid))
            {
                List<TimeEntryLogs> entryLogs = db.TimeEntryLogs.Where(x => x.unrestricteduserid == userid)
                    .OrderByDescending(x => x.createdonutc).ToList();
                return Json(new { error = 0, errortext = "Successfully!", Logs = entryLogs },
                    JsonRequestBehavior.AllowGet);
            }

            return Json(new { error = 1, errortext = "No user found!" }, JsonRequestBehavior.AllowGet);
        }

        private void AddNotification(long actionid, string userid)
        {
            try
            {
                List<string> Managerslist = new List<string>();
                SendNotificationViewModel send = new SendNotificationViewModel();
                Notification notification = new Notification
                {
                    commentid = 0,
                    entityid = 0
                };
                Managerslist = (from u in db.Users
                                join tm in db.TeamMember on u.Id equals tm.usersid
                                where u.IsNotifyManagerOnTaskAssignment && tm.IsManager
                                select u.Id).Distinct().ToList();

                ApplicationUser user = db.Users.Find(userid);
                string actorid = User.Identity.GetUserId();
                ApplicationUser actor = db.Users.Find(actorid);
                switch (actionid)
                {
                    case 10:
                        // ticket status change
                        send.title = "Time entry restriction update";
                        notification.description = user.FullName + " time entry restricted by " + actor.FullName;
                        break;

                    case 9:
                        //Assign to user
                        send.title = "Time entry restriction update";
                        notification.description = user.FullName + " time entry restriction remove by " + actor.FullName;
                        break;
                }

                notification.actorid = actorid;
                notification.entityactionid = actionid;
                notification.createdon = DateTime.Now;
                db.Notification.Add(notification);
                db.SaveChanges();

                foreach (string item in Managerslist)
                {
                    NotificationUsers notificationuser = new NotificationUsers
                    {
                        notification_Id = notification.id,
                        notifierid = item,
                        status = false
                    };
                    db.NotificationUsers.Add(notificationuser);
                    db.SaveChanges();
                }

                if (Managerslist != null && Managerslist.Count() > 0)
                {
                    send.users = Managerslist;
                    send.notification = notification;
                    NotificatonViewmodel.SendPushNotification(send);
                }
            }
            catch (Exception)
            {
            }
        }

        public ActionResult OrphanedTicketSetting()
        {
            try
            {
                List<SubscribeTeamViewModel> teams = new List<SubscribeTeamViewModel>();
                string userId = User.Identity.GetUserId();
                List<long> checkedIdsold = db.SubscribeTeams.Where(x => x.UsersId.Equals(userId)).Select(x => x.TeamId)
                    .ToList();
                string checkedIds = string.Join(",", checkedIdsold);
                IList<Team> teamList = db.Team.Where(x => x.isactive).ToList();
                teamList.Add(new Team { id = 0, name = "Unassigned Tickets" });
                foreach (Team item in teamList)
                {
                    SubscribeTeamViewModel team = new SubscribeTeamViewModel
                    {
                        name = item.name,
                        Id = item.id,
                        Ischecked = checkedIds != null ? checkedIds.Split(',').Contains(item.id.ToString()) : false
                    };
                    teams.Add(team);
                }

                return View(teams);
            }
            catch (Exception)
            {
                return Json(false);
            }
        }

        public ActionResult SubscribeTeam(string teamIds)
        {
            try
            {
                if (!string.IsNullOrEmpty(teamIds))
                {
                    long teamId = Convert.ToInt64(teamIds);
                    string userId = User.Identity.GetUserId();
                    SubscribeTeam data = db.SubscribeTeams.Where(x => x.TeamId == teamId && x.UsersId == userId).FirstOrDefault();
                    if (data != null)
                    {
                        db.SubscribeTeams.Remove(data);
                        db.SaveChanges();
                        return Json("Unsubscribe", JsonRequestBehavior.AllowGet);
                    }

                    SubscribeTeam subscribeTeams = new SubscribeTeam
                    {
                        UsersId = userId,
                        TeamId = teamId,
                        updatedonutc = DateTime.Now,
                        createdonutc = DateTime.Now,
                        ipused = Request.UserHostAddress
                    };
                    db.SubscribeTeams.Add(subscribeTeams);
                    db.SaveChanges();
                    return Json("Subscribe", JsonRequestBehavior.AllowGet);
                }

                return Json(false, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        [CustomeAuthorize(Role.Admin, Role.TeamLead)]
        public ActionResult OrphanedTicketAge()
        {
            return View(db.ConversationStatus.Where(x => x.id != 3 && x.id != 8 && x.isactive).ToList());
        }

        [CustomeAuthorize(Role.Admin)]
        public ActionResult EditOrphanAge(ConversationStatus data)
        {
            try
            {
                ConversationStatus conversationStatus = db.ConversationStatus.Find(data.id);
                if (conversationStatus != null)
                {
                    conversationStatus.OrphanAge = data.OrphanAge;
                    conversationStatus.updatedonutc = DateTime.Now;
                    db.SaveChanges();
                }

                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult TwoFAPermissionSettings()
        {
            var modelList = db.Users.Where(x => x.isactive).ToList();
            return View(modelList);
        } 
        
        
        public ActionResult ChangeTwoFA(string values)
        {
            var UserId = values.Split(',')[0];
            var Type = Convert.ToInt32(values.Split(',')[2]);
            var value = bool.Parse(values.Split(',')[1]);
            var user = db.Users.Where(x => x.Id == UserId).FirstOrDefault();
            if (Type == 1)
            {
                user.IsAppAuthenticatorEnabled = user.IsAppAuthenticatorEnabled ? false : true;
            }
            else if (Type == 2)
            {
                user.EmailConfirmed = user.EmailConfirmed ? false : true;
            }
            else
            {
                user.IsRocketAuthenticatorEnabled = user.IsRocketAuthenticatorEnabled ? false : true; ;  
            }
            db.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
    }
}