using computan.timesheet.core;
using computan.timesheet.core.OrphanTickets;
using computan.timesheet.Extensions;
using computan.timesheet.Helpers;
using computan.timesheet.Infrastructure;
using computan.timesheet.Models.OrphanTickets;
using computan.timesheet.Services.Orphan;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;

namespace computan.timesheet.Controllers
{
    [CustomeAuthorizeAttribute]
    public class OrphanController : BaseController
    {
        private readonly OrphanService service = new OrphanService();

        public ActionResult GetAllOrphanedTickets(string deleted = null)
        {
            try
            {
                OrphanViewModel orphanTickets = new OrphanViewModel
                {
                    OrphanSearch = new OrphanSearchViewModel()
                    {
                        StartDate = DateTime.Now.AddDays(-30),
                        EndDate = DateTime.Now.AddDays(-1),
                        TeamId = null,
                        StatusId = null,
                        AgeId = null,
                        Teams = db.Team.Where(x => x.isactive).Select(x => new SelectListItem
                        {
                            Text = x.name,
                            Value = x.id.ToString()
                        }).ToList(),
                        Status = db.ConversationStatus.Where(x => x.id != 3 && x.id != 8 && x.isactive).Select(x => new SelectListItem
                        {
                            Text = x.name,
                            Value = x.id.ToString()
                        }).ToList(),
                        Ages = ViewExtensions.Ages.Select(x => new SelectListItem
                        {
                            Text = x.ToString(),
                            Value = x.ToString()
                        }).ToList()
                    },
                    OrphanTickets = db.Database.SqlQuery<OrphanTicketViewModel>("exec FilterOrphanTickets_sp @userId, @teamId, @statusId, @startDate, @endDate",
                                     new SqlParameter("userId", User.Identity.GetUserId()),
                                     new SqlParameter("teamId", DBNull.Value),
                                     new SqlParameter("statusId", DBNull.Value),
                                     new SqlParameter("startDate", DateTime.Now.AddDays(-30)),
                                     new SqlParameter("endDate", DateTime.Now.AddDays(-1))
                                     ).Select(x => new OrphanTicketViewModel
                                     {
                                         id = x.id,
                                         topic = x.topic,
                                         StatusName = x.StatusName,
                                         LastActivityDate = x.LastActivityDate,
                                         TeamName = x.TeamName,
                                         Age = x.Age
                                     }).ToList()
                };
                ViewBag.message = deleted;
                string userId = User.Identity.GetUserId();
                ViewBag.Subscribe = db.SubscribeTeams.Where(x => x.UsersId == userId).FirstOrDefault();
                return View(orphanTickets);
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
                return View();
            }
        }

        [HttpPost]
        public ActionResult GetAllOrphanedTickets([Bind(Include = "StartDate,EndDate,TeamId,StatusId,AgeId")] OrphanSearchViewModel model)
        {
            try
            {
                OrphanViewModel orphanTickets = new OrphanViewModel
                {
                    OrphanSearch = new OrphanSearchViewModel()
                    {
                        StartDate = model.StartDate,
                        EndDate = model.EndDate,
                        TeamId = model.TeamId,
                        StatusId = model.StatusId,
                        AgeId = model.AgeId,
                        Teams = db.Team.Where(x => x.isactive).Select(x => new SelectListItem
                        {
                            Text = x.name,
                            Value = x.id.ToString()
                        }).ToList(),
                        Status = db.ConversationStatus.Where(x => x.id != 3 && x.id != 8 && x.isactive).Select(x => new SelectListItem
                        {
                            Text = x.name,
                            Value = x.id.ToString()
                        }).ToList(),
                        Ages = ViewExtensions.Ages.Select(x => new SelectListItem
                        {
                            Text = x.ToString(),
                            Value = x.ToString()
                        }).ToList()
                    },
                    OrphanTickets = db.Database.SqlQuery<OrphanTicketViewModel>("exec FilterOrphanTickets_sp @userId, @teamId, @statusId, @startDate, @endDate",
                                    new SqlParameter("userId", User.Identity.GetUserId()),
                                    new SqlParameter("teamId", (object)model.TeamId ?? DBNull.Value),
                                    new SqlParameter("statusId", (object)model.StatusId ?? DBNull.Value),
                                    new SqlParameter("startDate", model.StartDate),
                                    new SqlParameter("endDate", model.EndDate)
                                    ).Select(x => new OrphanTicketViewModel
                                    {
                                        id = x.id,
                                        topic = x.topic,
                                        StatusName = x.StatusName,
                                        LastActivityDate = x.LastActivityDate,
                                        TeamName = x.TeamName,
                                        Age = x.Age
                                    }).ToList()
                };
                //foreach (OrphanTicketViewModel item in orphanTickets.OrphanTickets)
                //{
                //    switch (item.StatusName)
                //    {
                //        case "New Task":
                //            item.Age = service.CalculateOrphanAge((int)TicketsStatus.NewTask, item.LastActivityDate);
                //            break;

                //        case "In Progress":
                //            item.Age = service.CalculateOrphanAge((int)TicketsStatus.InProgress, item.LastActivityDate);
                //            break;

                //        case "On Hold":
                //            item.Age = service.CalculateOrphanAge((int)TicketsStatus.OnHold, item.LastActivityDate);
                //            break;

                //        case "QC":
                //            item.Age = service.CalculateOrphanAge((int)TicketsStatus.QC, item.LastActivityDate);
                //            break;

                //        case "Assigned":
                //            item.Age = service.CalculateOrphanAge((int)TicketsStatus.Assigned, item.LastActivityDate);
                //            break;

                //        case "In Review":
                //            item.Age = service.CalculateOrphanAge((int)TicketsStatus.InReview, item.LastActivityDate);
                //            break;

                //        default:
                //            break;
                //    }
                //}
                if (model.AgeId != null)
                {
                    orphanTickets.OrphanTickets = orphanTickets.OrphanTickets.Where(x => x.Age >= Convert.ToInt32(model.AgeId)).ToList();
                }
                string userId = User.Identity.GetUserId();
                ViewBag.Subscribe = db.SubscribeTeams.Where(x => x.UsersId == userId).FirstOrDefault();
                return View(orphanTickets);
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
                return View();
            }
        }

        public ActionResult GetAllSuppressTickets(string suppressed = null)
        {
            ViewBag.message = suppressed;
            string userId = User.Identity.GetUserId();
            List<OrphanTicketViewModel> tickets = (from t in db.Ticket
                                                   join st in db.SuppressTickets on t.id equals st.TicketId
                                                   join c in db.ConversationStatus on t.statusid equals c.id
                                                   //where st.UsersId == userId
                                                   select new OrphanTicketViewModel
                                                   {
                                                       id = t.id,
                                                       topic = t.topic,
                                                       LastActivityDate = t.LastActivityDate.Value,
                                                       StatusName = c.name,
                                                   }).ToList();
            return View(tickets);
        }

        public ActionResult SuppressTicket(int id, bool isExternal = false)
        {
            string userId = User.Identity.GetUserId();
            SuppressTicket suppressTicket = db.SuppressTickets.Where(x => x.TicketId == id).FirstOrDefault();
            if (suppressTicket == null)
            {
                SuppressTicket supress = new SuppressTicket()
                {
                    TicketId = id,
                    UsersId = userId,
                    createdonutc = DateTime.Now,
                    updatedonutc = DateTime.Now,
                    userid = userId,
                    ipused = Request.UserHostAddress
                };
                db.SuppressTickets.Add(supress);
                db.SaveChanges();
                if (isExternal == false)
                {
                    return Json(new
                    {
                        error = false,
                        message = "Un-Suppress",
                        text = "Suppressed"
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return RedirectToAction("GetAllSuppressTickets", new { suppressed = "success" });
                }
            }
            else
            {
                if (isExternal == false)
                {
                    db.SuppressTickets.Remove(suppressTicket);
                    db.SaveChanges();
                    return Json(new
                    {
                        error = false,
                        message = "Suppress",
                        text="Un-Suppressed"
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return RedirectToAction("GetAllSuppressTickets", new { suppressed = "warning" });
                }
            }
        }

        public ActionResult UnAssignedTickets()
        {
            List<OrphanTicketViewModel> unAssignedtickets = db.Database.SqlQuery<OrphanTicketViewModel>("exec GetUnAssignedOrphanTickets_sp @userId",
                   new SqlParameter("userId", User.Identity.GetUserId())
                       ).ToList();
            return View(unAssignedtickets);
        }
        [AllowAnonymous]
        public void SendEmail()
        {
            try
            {
                IEnumerable<string> userList = db.SubscribeTeams.Select(x => x.UsersId).Distinct().ToList();
                if (userList.Count() != 0)
                {
                    foreach (string item in userList)
                    {
                        string mailbody = string.Empty;
                        string userEmail = db.Users.Where(x => x.Id == item).Select(x => x.Email).FirstOrDefault();

                        //Store Procedures called
                        List<OrphanTicketViewModel> tickets = db.Database.SqlQuery<OrphanTicketViewModel>("exec GetOrphanTicketsByUserId_sp @userId",
                        new SqlParameter("userId", item)
                        ).ToList();
                        List<OrphanTicketViewModel> unAssignedtickets = db.Database.SqlQuery<OrphanTicketViewModel>("exec GetUnAssignedOrphanTickets_sp @userId",
                        new SqlParameter("userId", item)
                            ).ToList();

                        var teamList = tickets.Select(x => x.TeamName).Distinct().ToList();
                        if (unAssignedtickets.Count() != 0)
                        {
                            teamList.Insert(0, "Unassigned Tickets");
                            tickets.AddRange(unAssignedtickets);
                        }
                        if ( tickets.Count() > 0)
                        {
                            mailbody += service.MailHeadHTML();
                            foreach (string team in teamList)
                            {
                                List<OrphanTicketViewModel> teamTickets = tickets.Where(x => x.TeamName.Equals(team)).OrderBy(x => x.LastActivityDate).ToList();
                                mailbody += service.MailTableHeaders(team);
                                mailbody += service.OrphanMailBody(teamTickets);
                                mailbody += service.EmailFooterHTML();
                            }
                            //-----Signature-------------
                            if (mailbody != "")
                            {
                                mailbody += "</br></br></br> <div>Kind Regards,</br>Timesheet Management Team</div></section>";
                                MailService.SendClientEmail(userEmail, "Orphaned tickets report", mailbody, null, true);
                            }
                        }
                    }
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
            }
        }
    }
}