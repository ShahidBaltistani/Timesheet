using computan.timesheet.Contexts;
using computan.timesheet.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace computan.timesheet.Infrastructure
{
    public class MangeTaskService
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        public string ChangeEmailStatus(long id, int statusid)
        {
            try
            {
                core.TicketItem TicketItem = db.TicketItem.Where(t => t.id == id).FirstOrDefault();
                core.Ticket ticket = db.Ticket.Where(i => i.id == TicketItem.ticketid).FirstOrDefault();
                if (ticket.statusid == 1)
                {
                    ticket.statusid = 2;
                    ticket.statusupdatedbyusersid = HttpContext.Current.User.Identity.GetUserId();
                    ticket.statusupdatedon = DateTime.Now;
                    ticket.LastActivityDate = DateTime.Now;
                    db.Entry(ticket).State = EntityState.Modified;
                    db.SaveChanges();
                }

                TicketItem.statusid = statusid;
                TicketItem.statusupdatedbyusersid = HttpContext.Current.User.Identity.GetUserId();
                TicketItem.updatedonutc = DateTime.Now;
                TicketItem.statusupdatedon = DateTime.Now;
                TicketItem.ipused = HttpContext.Current.Request.UserHostAddress;
                TicketItem.userid = HttpContext.Current.User.Identity.GetUserId();
                db.Entry(TicketItem).State = EntityState.Modified;
                db.SaveChanges();

                List<core.TicketItemLog> ticketitemlog = db.TicketItemLog.Where(t => t.ticketitemid == id).ToList();
                List<TicketItemStatusChnageViewModel> tiscvm = new List<TicketItemStatusChnageViewModel>();
                if (ticketitemlog.Count > 0 && ticketitemlog != null)
                {
                    foreach (core.TicketItemLog items in ticketitemlog)
                    {
                        if (items.statusid == 1 || items.statusid == 2)
                        {
                            core.TicketItemLog ticketitem = db.TicketItemLog.Where(t => t.id == items.id).FirstOrDefault();
                            ticketitem.statusid = statusid;
                            ticketitem.assignedbyusersid = HttpContext.Current.User.Identity.GetUserId();
                            ticketitem.statusupdatedbyusersid = HttpContext.Current.User.Identity.GetUserId();
                            ticketitem.statusupdatedon = DateTime.Now;
                            db.Entry((object)ticketitem).State = EntityState.Modified;
                            db.SaveChanges();
                            TicketItemStatusChnageViewModel tis = new TicketItemStatusChnageViewModel
                            {
                                userid = ticketitem.assignedtousersid,
                                assignmentid = ticketitem.id
                            };
                            core.TicketStatus statusname = db.TicketStatus.Find(statusid);
                            tis.statusname = statusname.name;
                            tis.statusid = statusname.id;
                            tiscvm.Add(tis);
                        }
                    }
                }

                return "The Ticket item status has been updated.";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public bool ChangeUserTaskStatus(int statusid, long taskid)
        {
            try
            {
                // Fetch ticket assigned to the current user.
                string currentUser = HttpContext.Current.User.Identity.GetUserId();
                core.TicketItemLog ticketItemLog = db.TicketItemLog.Where(til => til.id == taskid).FirstOrDefault();
                if (ticketItemLog == null)
                {
                    return false;
                }

                ticketItemLog.statusid = statusid;
                ticketItemLog.assignedon = DateTime.Now;
                ticketItemLog.statusupdatedbyusersid = HttpContext.Current.User.Identity.GetUserId();
                ticketItemLog.statusupdatedon = DateTime.Now;
                db.Entry(ticketItemLog).State = EntityState.Modified;
                db.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}