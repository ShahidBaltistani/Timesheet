using computan.timesheet.Contexts;
using computan.timesheet.core;
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
    public class ManualTicketController : Controller
    {
        // GET: ManualTicket
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index(long id)
        {
            List<SelectListItem> projectlist = db.Database.SqlQuery<CombinedEntity>("exec projects_loadcombined").Select(x =>
                new SelectListItem
                {
                    Text = x.name,
                    Value = x.id.ToString()
                }).ToList();
            ViewBag.users = db.Users.ToList();
            System.Data.Entity.Infrastructure.DbRawSqlQuery<TicketStatusViewModel> ticketstatus = db.Database.SqlQuery<TicketStatusViewModel>("exec Manualticketstatus_loadticketscount");
            ViewBag.ticketstatus = ticketstatus;
            ViewBag.currentsubtab = ticketstatus.Where(x => x.id == id).FirstOrDefault().name;
            List<TicketItem> ticket = (from ti in db.TicketItem
                                       join t in db.Ticket
                                           on ti.ticketid equals t.id
                                       where t.tickettypeid == 2 && t.statusid == id
                                       select ti).Include(ti => ti.Ticket).Include(p => p.Project).Include(s => s.Skill).ToList();
            return View(ticket);
        }

        public ActionResult Create()
        {
            List<SelectListItem> projectlist = db.Database.SqlQuery<CombinedEntity>("exec projects_loadcombined").Select(x =>
                new SelectListItem
                {
                    Text = x.name,
                    Value = x.id.ToString()
                }).ToList();
            ViewBag.projectid = new SelectList(projectlist, "Value", "Text");
            ViewBag.skillid = new SelectList(db.Skill, "id", "name");
            return View();
        }

        public ActionResult CheckUsersTeam(string usersid)
        {
            if (!string.IsNullOrEmpty(usersid))
            {
                usersid = usersid.Split(',').Last();
                long teamid = (from x in db.TeamMember where usersid == x.usersid select x.teamid).FirstOrDefault();
                if (teamid == 0)
                {
                    string name = db.Users.Find(usersid).FullName;
                    return Json(new
                    {
                        error = true,
                        response = name + " user not assign to any team"
                    }, JsonRequestBehavior.AllowGet);
                }
            }

            return Json(new
            {
                error = false,
                response = ""
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(ManualTicketViewModel mtvc, params string[] users)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    mtvc.ticket.uniquesenders = User.Identity.Name;
                    mtvc.ticket.conversationid = DateTime.Now.ToString();
                    mtvc.ticket.createdonutc = DateTime.Now;
                    mtvc.ticket.flagstatusid = 1;
                    mtvc.ticket.hasattachments = false;
                    mtvc.ticket.importance = false;
                    mtvc.ticket.ipused = Request.UserHostAddress;
                    mtvc.ticket.lastdeliverytime = DateTime.Now;
                    mtvc.ticket.lastmodifiedtime = DateTime.Now;
                    mtvc.ticket.messagecount = 1;
                    mtvc.ticket.size = 1;
                    users = string.IsNullOrEmpty(users[0]) ? null : users[0].Split(',');
                    if (users != null && users.Count() > 0)
                    {
                        mtvc.ticket.statusid = 2;
                    }
                    else
                    {
                        mtvc.ticket.statusid = 1;
                    }

                    mtvc.ticket.statusupdatedbyusersid = User.Identity.GetUserId();
                    mtvc.ticket.tickettypeid = 2;
                    mtvc.ticket.updatedonutc = DateTime.Now;
                    mtvc.ticket.userid = User.Identity.GetUserId();
                    mtvc.ticket.statusupdatedon = DateTime.Now;
                    mtvc.ticket.LastActivityDate = DateTime.Now;
                    mtvc.ticket.projectid = mtvc.ticketitem.projectid;
                    mtvc.ticket.skillid = mtvc.ticketitem.skillid;
                    mtvc.ticket.LastActivityDate = DateTime.Now;
                    db.Ticket.Add(mtvc.ticket);
                    db.SaveChanges();
                    mtvc.ticketitem.ticketid = mtvc.ticket.id;
                    mtvc.ticketitem.importance = 1;
                    mtvc.ticketitem.bccrecipients = string.Empty;
                    mtvc.ticketitem.body = string.Empty;
                    mtvc.ticketitem.conversationid = string.Empty;
                    mtvc.ticketitem.conversationindex = string.Empty;
                    mtvc.ticketitem.conversationtopic = string.Empty;
                    mtvc.ticketitem.createdonutc = DateTime.Now;
                    mtvc.ticketitem.updatedonutc = DateTime.Now;
                    mtvc.ticketitem.ipused = Request.UserHostAddress;
                    mtvc.ticketitem.userid = User.Identity.GetUserId();
                    mtvc.ticketitem.datetimecreated = DateTime.Now;
                    mtvc.ticketitem.datetimereceived = DateTime.Now;
                    mtvc.ticketitem.datetimesent = DateTime.Now;
                    mtvc.ticketitem.displaycc = string.Empty;
                    mtvc.ticketitem.displayto = string.Empty;
                    mtvc.ticketitem.emailmessageid = string.Empty;
                    mtvc.ticketitem.from = User.Identity.Name;
                    mtvc.ticketitem.hasattachments = false;
                    mtvc.ticketitem.inreplyto = string.Empty;
                    mtvc.ticketitem.internetmessageheaders = string.Empty;
                    mtvc.ticketitem.internetmessageid = string.Empty;
                    mtvc.ticketitem.lastmodifiedname = string.Empty;
                    mtvc.ticketitem.lastmodifiedtime = DateTime.Now;
                    mtvc.ticketitem.mimecontent = string.Empty;
                    mtvc.ticketitem.replyto = string.Empty;
                    mtvc.ticketitem.sensitivity = 1;
                    ;
                    mtvc.ticketitem.size = 1;
                    mtvc.ticketitem.statusid = 1;
                    if (users != null && users.Count() > 0)
                    {
                        mtvc.ticketitem.statusid = 2;
                    }
                    else
                    {
                        mtvc.ticketitem.statusid = 1;
                    }

                    mtvc.ticketitem.statusupdatedbyusersid = User.Identity.GetUserId();
                    mtvc.ticketitem.statusupdatedon = DateTime.Now;
                    db.TicketItem.Add(mtvc.ticketitem);
                    db.SaveChanges();
                    List<string> teams = new List<string>();
                    if (users != null && users.Count() > 0)
                    {
                        foreach (string userid in users)
                        {
                            long teamid = (from x in db.TeamMember where userid == x.usersid select x.teamid)
                                .FirstOrDefault();
                            if (!teams.Contains(teamid.ToString()))
                            {
                                teams.Add(teamid.ToString());
                            }

                            TicketItemLog ticketitemlog = new TicketItemLog();
                            long? count = db.TicketItemLog.Where(ti =>
                                    ti.assignedtousersid == userid && ti.displayorder != null && ti.statusid == 2)
                                .Max(d => d.displayorder);
                            ticketitemlog.ticketitemid = mtvc.ticketitem.id;
                            ticketitemlog.statusid = 2;
                            ticketitemlog.displayorder = count != null ? count + 1 : 1;
                            ticketitemlog.assignedbyusersid = User.Identity.GetUserId();
                            ticketitemlog.assignedtousersid = userid;
                            ticketitemlog.assignedon = DateTime.Now;
                            ticketitemlog.statusupdatedbyusersid = User.Identity.GetUserId();
                            ;
                            ticketitemlog.statusupdatedon = DateTime.Now;
                            db.TicketItemLog.Add(ticketitemlog);
                            db.SaveChanges();
                        }
                    }

                    if (teams != null && teams.Count() > 0)
                    {
                        foreach (string team in teams)
                        {
                            if (!string.IsNullOrEmpty(team))
                            {
                                long teamid = Convert.ToInt64(team);
                                TicketTeamLogs teamLog = db.TicketTeamLogs.FirstOrDefault(x =>
                                    x.teamid == teamid && x.ticketid == mtvc.ticket.id);
                                if (teamLog == null)
                                {
                                    TicketTeamLogs newteamLog = new TicketTeamLogs
                                    {
                                        displayorder = 1,
                                        teamid = teamid,
                                        ticketid = mtvc.ticket.id,
                                        assignedbyusersid = User.Identity.GetUserId(),
                                        assignedon = DateTime.Now,
                                        statusid = 6,
                                        statusupdatedbyusersid = User.Identity.GetUserId(),
                                        statusupdatedon = DateTime.Now
                                    };
                                    db.TicketTeamLogs.Add(newteamLog);
                                    db.SaveChanges();
                                    /*Muhammad Nasir 30-11-2018*/
                                    //Teams = Teams + db.Team.Find(teamid).name + ",";
                                }
                                else
                                {
                                    teamLog.statusupdatedbyusersid = User.Identity.GetUserId();
                                    teamLog.statusupdatedon = DateTime.Now;
                                    teamLog.assignedon = DateTime.Now;
                                    db.SaveChanges();
                                }
                            }
                        }
                    }

                    if (users != null && users.Count() > 0)
                    {
                        return RedirectToAction("mytickets", "tickets", new { id = "2" });
                    }

                    return RedirectToAction("index", "tickets", new { id = "1" });
                }

                List<SelectListItem> projectlist = db.Database.SqlQuery<CombinedEntity>("exec projects_loadcombined").Select(x =>
                    new SelectListItem
                    {
                        Text = x.name,
                        Value = x.id.ToString()
                    }).ToList();
                ViewBag.projectid = new SelectList(projectlist, "Value", "Text");
                ViewBag.skillid = new SelectList(db.Skill, "id", "name");
                //ViewBag.users = db.Users.OrderBy(u => u.FirstName).Where(us => us.isactive == true).ToList();
                return View(mtvc);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                List<SelectListItem> projectlist = db.Database.SqlQuery<CombinedEntity>("exec projects_loadcombined").Select(x =>
                    new SelectListItem
                    {
                        Text = x.name,
                        Value = x.id.ToString()
                    }).ToList();
                ViewBag.projectid = new SelectList(projectlist, "Value", "Text");
                ViewBag.skillid = new SelectList(db.Skill, "id", "name");
                //ViewBag.users = db.Users.OrderBy(u => u.FirstName).Where(us => us.isactive == true).ToList();
                return View(mtvc);
            }
        }

        public ActionResult Edit(long id)
        {
            Ticket manualTicket = db.Ticket.Find(id);
            TicketItem ticket = db.TicketItem.Include(ti => ti.Ticket).Include(tl => tl.TicketItemLog)
                .Where(i => i.ticketid == id).FirstOrDefault();

            ManualTicketViewModel mtvm = new ManualTicketViewModel
            {
                ticket = ticket.Ticket,
                ticketitem = ticket
            };

            List<SelectListItem> projectlist = db.Database.SqlQuery<CombinedEntity>("exec projects_loadcombined").Select(x =>
                new SelectListItem
                {
                    Text = x.name,
                    Value = x.id.ToString()
                }).ToList();
            ViewBag.users = db.Users.OrderBy(u => u.FirstName).Where(us => us.isactive == true).ToList();
            ViewBag.projectid = new SelectList(projectlist, "Value", "Text", ticket.projectid);
            ViewBag.skillid = new SelectList(db.Skill, "id", "name", ticket.skillid);
            return View(mtvm);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(ManualTicketViewModel mtvc, params string[] users)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Ticket ticket = db.Ticket.Where(i => i.id == mtvc.ticket.id).FirstOrDefault();
                    if (ticket != null)
                    {
                        ticket.topic = mtvc.ticket.topic;
                        ticket.ipused = Request.UserHostAddress;
                        ticket.userid = User.Identity.GetUserId();
                        ticket.updatedonutc = DateTime.Now;
                        db.Entry(ticket).State = EntityState.Modified;
                        db.SaveChanges();
                        TicketItem ticketitem = db.TicketItem.Where(i => i.ticketid == ticket.id).FirstOrDefault();
                        if (ticketitem != null)
                        {
                            ticketitem.updatedonutc = DateTime.Now;
                            ticketitem.ipused = Request.UserHostAddress;
                            ticketitem.userid = User.Identity.GetUserId();
                            ticketitem.uniquebody = mtvc.ticketitem.uniquebody;
                            ticketitem.subject = mtvc.ticketitem.subject;
                            ticketitem.projectid = mtvc.ticketitem.projectid;
                            ticketitem.skillid = mtvc.ticketitem.skillid;
                            db.Entry(ticketitem).State = EntityState.Modified;
                            db.SaveChanges();
                            List<long> teams = new List<long>();
                            if (users != null && users.Count() > 0)
                            {
                                foreach (string userid in users)
                                {
                                    long teamid = (from x in db.TeamMember where userid == x.usersid select x.teamid)
                                        .FirstOrDefault();
                                    if (!teams.Contains(teamid))
                                    {
                                        teams.Add(teamid);
                                    }

                                    TicketItemLog ticketitemlogs = db.TicketItemLog.Where(ti =>
                                            ti.ticketitemid == ticketitem.id && ti.assignedtousersid == userid)
                                        .FirstOrDefault();
                                    if (ticketitemlogs == null)
                                    {
                                        TicketItemLog ticketitemlog = new TicketItemLog();
                                        long? count = db.TicketItemLog.Where(ti =>
                                            ti.assignedtousersid == userid && ti.displayorder != null &&
                                            ti.statusid == 2).Max(d => d.displayorder);
                                        ticketitemlog.displayorder = count != null ? count + 1 : 1;
                                        ticketitemlog.ticketitemid = mtvc.ticketitem.id;
                                        ticketitemlog.statusid = 2;
                                        ticketitemlog.assignedbyusersid = User.Identity.GetUserId();
                                        ticketitemlog.assignedtousersid = userid;
                                        ticketitemlog.assignedon = DateTime.Now;
                                        ticketitemlog.statusupdatedbyusersid = User.Identity.GetUserId();
                                        ;
                                        ticketitemlog.statusupdatedon = DateTime.Now;
                                        db.TicketItemLog.Add(ticketitemlog);
                                        db.SaveChanges();
                                    }
                                }
                            }

                            if (teams != null && teams.Count() > 0)
                            {
                                foreach (long teamid in teams)
                                {
                                    TicketTeamLogs teamLog = db.TicketTeamLogs.FirstOrDefault(x =>
                                        x.teamid == teamid && x.ticketid == mtvc.ticket.id);
                                    if (teamLog == null)
                                    {
                                        TicketTeamLogs newteamLog = new TicketTeamLogs
                                        {
                                            displayorder = 1,
                                            teamid = teamid,
                                            ticketid = mtvc.ticket.id,
                                            assignedbyusersid = User.Identity.GetUserId(),
                                            assignedon = DateTime.Now,
                                            statusid = 6,
                                            statusupdatedbyusersid = User.Identity.GetUserId(),
                                            statusupdatedon = DateTime.Now
                                        };
                                        db.TicketTeamLogs.Add(newteamLog);
                                        db.SaveChanges();
                                        /*Muhammad Nasir 30-11-2018*/
                                        //Teams = Teams + db.Team.Find(teamid).name + ",";
                                    }
                                    else
                                    {
                                        teamLog.statusupdatedbyusersid = User.Identity.GetUserId();
                                        teamLog.statusupdatedon = DateTime.Now;
                                        teamLog.assignedon = DateTime.Now;
                                        db.Entry(teamLog).State = EntityState.Modified;
                                        db.SaveChanges();
                                    }
                                }
                            }
                        }
                    }

                    return RedirectToAction("index", new { id = "2" });
                }

                List<SelectListItem> projectlist = db.Database.SqlQuery<CombinedEntity>("exec projects_loadcombined").Select(x =>
                    new SelectListItem
                    {
                        Text = x.name,
                        Value = x.id.ToString()
                    }).ToList();
                ViewBag.projectid = new SelectList(projectlist, "Value", "Text");
                ViewBag.users = db.Users.OrderBy(u => u.FirstName).Where(us => us.isactive == true).ToList();
                ViewBag.skillid = new SelectList(db.Skill, "id", "name");
                return View(mtvc);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                List<SelectListItem> projectlist = db.Database.SqlQuery<CombinedEntity>("exec projects_loadcombined").Select(x =>
                    new SelectListItem
                    {
                        Text = x.name,
                        Value = x.id.ToString()
                    }).ToList();
                ViewBag.projectid = new SelectList(projectlist, "Value", "Text");
                ViewBag.skillid = new SelectList(db.Skill, "id", "name");
                ViewBag.users = db.Users.OrderBy(u => u.FirstName).Where(us => us.isactive == true).ToList();
                return View(mtvc);
            }
        }

        [ValidateInput(false)]
        public ActionResult EmailReplaySignature(string emailsignature)
        {
            if (emailsignature != string.Empty)
            {
                string logedinuserid = User.Identity.GetUserId();
                ApplicationUser logedinuser = db.Users.Find(logedinuserid);

                if (logedinuser != null)
                {
                    logedinuser.EmailReplySignature = emailsignature;
                    db.SaveChanges();
                    return Json(new { error = 1, contexttext = "Signature Updated Successfully" },
                        JsonRequestBehavior.AllowGet);
                }

                return Json(new { error = 2, contexttext = "Unknown User Identify" }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { error = 3, contexttext = "Cannot Update Empty Signature" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FetchingReplaySignature()
        {
            string logedinuserid = User.Identity.GetUserId();
            ApplicationUser logedinuser = db.Users.Find(logedinuserid);

            if (logedinuser != null)
            {
                string emailsignature = logedinuser.EmailReplySignature;
                return Json(new { error = 1, contexttext = emailsignature }, JsonRequestBehavior.DenyGet);
            }

            return Json(new { error = 2, contexttext = "Unknown User Identify" }, JsonRequestBehavior.DenyGet);
        }
    }
}