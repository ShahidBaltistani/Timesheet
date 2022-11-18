using computan.timesheet.Contexts;
using computan.timesheet.Extensions;
using computan.timesheet.Helpers;
using computan.timesheet.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace computan.timesheet.Controllers
{
    [CustomeAuthorizeAttribute]
    public class NotificationController : Controller
    {
        private const int recordPerPage = 20;

        private readonly ApplicationDbContext db = new ApplicationDbContext();

        // GET: Notification
        public ActionResult Index()
        {
            string user = User.Identity.GetUserId();
            List<core.NotificationUsers> NotificationList = db.NotificationUsers.Where(n => n.notifierid == user)
                .OrderByDescending(n => n.notification.createdon).Take(recordPerPage).ToList();
            List<NotificationMessageViewModel> messgaelist = new List<NotificationMessageViewModel>();
            foreach (core.NotificationUsers item in NotificationList)
            {
                NotificationMessageViewModel msg = new NotificationMessageViewModel();
                string entityname = item.notification.notificationentityaction.notificationentity.name;
                string entityAction = item.notification.notificationentityaction.name;
                msg.entityId = item.notification.entityid;
                msg.Message = item.notification.description;
                msg.commentid = item.notification.commentid;
                msg.EntityActionId = item.notification.entityactionid;
                msg.id = item.Id;
                msg.status = item.status;
                msg.createdDate = item.notification.createdon;
                messgaelist.Add(msg);
            }

            ViewBag.pagenum = 0;
            return View(messgaelist);
        }

        public ActionResult HeaderNotification()
        {
            string userid = User.Identity.GetUserId();
            List<core.NotificationUsers> notificationlist = db.NotificationUsers.Where(nu => nu.notifierid == userid && !nu.status)
                .OrderByDescending(nu => nu.notification.createdon).ToList();
            List<NotificationMessageViewModel> messgaelist = new List<NotificationMessageViewModel>();
            foreach (core.NotificationUsers item in notificationlist)
            {
                NotificationMessageViewModel msg = new NotificationMessageViewModel();
                string entityname = item.notification.notificationentityaction.notificationentity.name;
                string entityAction = item.notification.notificationentityaction.name;
                msg.entityId = item.notification.entityid;
                msg.id = item.Id;
                msg.EntityActionId = item.notification.entityactionid;
                msg.commentid = item.notification.commentid;
                msg.status = item.status;
                msg.createdDate = item.notification.createdon;
                msg.Message = item.notification.description;
                messgaelist.Add(msg);
            }

            //string Messages = PartialView("_MyNotification", messgaelist).RenderToString();
            return PartialView("_HeaderNotification", messgaelist);
        }

        public ActionResult paginatedNotification(int? page = 0)
        {
            int pagenumber = page ?? 0;
            List<NotificationMessageViewModel> messgaelist = new List<NotificationMessageViewModel>();
            string userid = User.Identity.GetUserId();
            int skipRecords = pagenumber * recordPerPage;
            List<core.NotificationUsers> notification = db.NotificationUsers.Where(n => n.notifierid == userid)
                .OrderByDescending(n => n.notification.createdon).Skip(skipRecords).Take(recordPerPage).ToList();
            ViewBag.pagenum = pagenumber;

            foreach (core.NotificationUsers item in notification)
            {
                NotificationMessageViewModel msg = new NotificationMessageViewModel();
                string entityname = item.notification.notificationentityaction.notificationentity.name;
                string entityAction = item.notification.notificationentityaction.name;
                msg.entityId = item.notification.entityid;
                msg.id = item.Id;
                msg.EntityActionId = item.notification.entityactionid;
                msg.commentid = item.notification.commentid;
                msg.Message = item.notification.description;
                msg.status = item.status;
                msg.createdDate = item.notification.createdon;
                messgaelist.Add(msg);
            }

            string notificationItem = PartialView("_MyNotification", messgaelist).RenderToString();
            return Json(new { NotificationItem = notificationItem }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult readNotification(string id)
        {
            try
            {
                long nid = Convert.ToInt64(id);
                core.NotificationUsers notification = db.NotificationUsers.Find(nid);
                notification.status = true;
                db.Entry(notification).State = EntityState.Modified;
                db.SaveChanges();

                return Json(new
                {
                    error = false,
                    message = "Successfull"
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new
                {
                    error = true,
                    message = "something missing"
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult unReadAll()
        {
            try
            {
                string user = User.Identity.GetUserId();
                List<core.NotificationUsers> notificationlist = db.NotificationUsers.Where(x => x.notifierid == user && x.status).ToList();
                foreach (core.NotificationUsers notification in notificationlist)
                {
                    notification.status = false;
                    db.Entry(notification).State = EntityState.Modified;
                    db.SaveChanges();
                }

                return Json(new { error = false, message = "Successfull" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = true, errortext = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ReadAll()
        {
            try
            {
                string user = User.Identity.GetUserId();
                if (!string.IsNullOrEmpty(user))
                {
                    List<core.NotificationUsers> notificationlist = db.NotificationUsers.Where(x => x.notifierid == user && x.status == false)
                        .ToList();
                    if (notificationlist.Count > 0)
                    {
                        foreach (core.NotificationUsers notification in notificationlist)
                        {
                            notification.status = true;
                            db.Entry(notification).State = EntityState.Modified;
                            db.SaveChanges();
                        }

                        return Json(new { error = false, message = "Successfully read all" },
                            JsonRequestBehavior.AllowGet);
                    }

                    return Json(new { error = true, errortext = "Soory no notification found" },
                        JsonRequestBehavior.AllowGet);
                }

                return Json(new { error = true, errortext = "Soory user not found" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = true, errortext = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult UnreadNotification(string id)
        {
            try
            {
                long nid = Convert.ToInt64(id);
                core.NotificationUsers notification = db.NotificationUsers.Find(nid);
                notification.status = false;
                db.Entry(notification).State = EntityState.Modified;
                db.SaveChanges();

                return Json(new
                {
                    error = false,
                    message = "Successfull"
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new
                {
                    error = true,
                    message = "something missing"
                }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ReadMultipleNotification(List<string> notification)
        {
            if (notification == null || notification.Count == 0)
            {
                return Json(new { error = true, errortext = "Sorry at least one task must be selected" });
            }

            foreach (string item in notification)
            {
                long x = Convert.ToInt64(item);
                core.NotificationUsers noti = db.NotificationUsers.Find(x);
                noti.status = true;
                db.Entry(noti).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new { error = false });
        }

        public string NotificationCount()
        {
            string userid = User.Identity.GetUserId();
            string count = db.NotificationUsers.Where(nu => nu.notifierid == userid && nu.status == false).Count()
                .ToString();
            return count;
        }

        public ActionResult UnreadMultipleNotification(List<string> notification)
        {
            if (notification == null || notification.Count == 0)
            {
                return Json(new { error = true, errortext = "Sorry at least one task must be selected" });
            }

            foreach (string item in notification)
            {
                long x = Convert.ToInt64(item);
                core.NotificationUsers noti = db.NotificationUsers.Find(x);
                noti.status = false;
                db.Entry(noti).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new { error = false });
        }

        [HttpGet]
        public void notificationOn()
        {
            try
            {
                string userid = User.Identity.GetUserId();
                core.ApplicationUser user = db.Users.Where(u => u.Id == userid).SingleOrDefault();
                user.IsNotifyManagerOnTaskAssignment = true;
                db.SaveChanges();
            }
            catch (Exception)
            {
            }
        }

        [HttpGet]
        public void notificationOff()
        {
            try
            {
                string userid = User.Identity.GetUserId();
                core.ApplicationUser user = db.Users.Where(u => u.Id == userid).SingleOrDefault();
                user.IsNotifyManagerOnTaskAssignment = false;
                db.SaveChanges();
            }
            catch (Exception)
            {
            }
        }
    }
}