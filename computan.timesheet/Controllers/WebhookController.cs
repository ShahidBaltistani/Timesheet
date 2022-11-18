using computan.timesheet.Contexts;
using computan.timesheet.core;
using computan.timesheet.Helpers;
using computan.timesheet.Models.OrphanTickets;
using computan.timesheet.Services.Orphan;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace computan.timesheet.Controllers
{
    [CustomeAuthorizeAttribute]
    public class WebhookController : Controller
    {
        public readonly ApplicationDbContext db = new ApplicationDbContext();
        public readonly OrphanService service = new OrphanService();
        [AllowAnonymous]
        public async Task SendOrphanNotificationInRocket()
        {
            try
            {
                // send orphan tickets which are assingned and have rocket URL
                List<OrphanTicketViewModel> tickets = db.Database.SqlQuery<OrphanTicketViewModel>("exec GetOrphanTicketsForRocket_sp")
                    .Select(t => new OrphanTicketViewModel
                    {
                        id = t.id,
                        topic = t.topic,
                        TeamName = t.TeamName,
                        TeamId = t.TeamId,
                        LastActivityDate = t.LastActivityDate,
                        StatusId = t.StatusId,
                        RocketUrl = t.RocketUrl,
                        Age = t.Age,
                    }).ToList();

                foreach (var ticket in tickets)
                {
                    if (!string.IsNullOrEmpty(ticket.RocketUrl))
                    {
                        string topic = ticket.topic.Replace("]", string.Empty).Replace("[", string.Empty);
                        string baseUrl = string.Format("{0}://{1}{2}", Request.Url.Scheme, Request.Url.Authority, Url.Content("~"));
                        OrphanWebhookViewModel model = new OrphanWebhookViewModel()
                        {
                            //    attachments = new List<Attachment>
                            //{
                            //    new Attachment
                            //    {
                            //        color = "#FFF",
                            //        author_name = ticket.topic,
                            //        author_link = baseUrl+"tickets/ticketitem/" + ticket.id,
                            //        title=ticket.TeamName,
                            //        title_link=baseUrl+"Home/team/" + ticket.TeamId,
                            //    },
                            //      new Attachment
                            //    {
                            //        author_name = "Suppress Ticket",
                            //        author_link= baseUrl +"orphan/SuppressTicket/" + ticket.id + "/?isExternal=true",
                            //        color = "#FFF",
                            //    },
                            //      new Attachment
                            //    {
                            //        author_name = "Trash",
                            //        author_link= baseUrl +"Tickets/ChnageTicketStatus/" + ticket.id +"/?status="+8+ "&isExternal=true",
                            //        color = "#FFF",
                            //    }
                            //},

                            //text = ticket.Age.ToString() + " days old ticket"
                            text = "12 days old ticket | " +
                        "[" + topic + "]" + "(" + baseUrl + "tickets/ticketitem/" + ticket.id + ")" + " | " +
                        "[ Suppress ]" + "(" + baseUrl + "orphan/SuppressTicket/" + ticket.id + "/?isExternal=true)" + " | " +
                        "[ Trash ]" + "(" + baseUrl + "tickets/ChnageTicketStatus/" + ticket.id + "/?status=8&isExternal=true)",
                        };
                        var client = new HttpClient();
                        client.DefaultRequestHeaders.Accept.Clear();
                        var myContent = JsonConvert.SerializeObject(model);
                        var buffer = System.Text.Encoding.UTF8.GetBytes(myContent);
                        var byteContent = new ByteArrayContent(buffer);
                        byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/Json");
                        var responseTask = client.PostAsync(ticket.RocketUrl, byteContent);
                        responseTask.Wait();
                    }
                }
                await SendUnAssignedTicketsNotificationInRocket();
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

        public async Task SendUnAssignedTicketsNotificationInRocket()
        {
            try
            {
                List<OrphanTicketViewModel> tickets = await db.Database.SqlQuery<OrphanTicketViewModel>("exec GetUnAssignedOrphanTickets_sp @userId",
                        new SqlParameter("userId", DBNull.Value)).ToListAsync();

                foreach (var ticket in tickets)
                {
                    string topic = ticket.topic.Replace("]", string.Empty).Replace("[", string.Empty);

                    string baseUrl = string.Format("{0}://{1}{2}", Request.Url.Scheme, Request.Url.Authority, Url.Content("~"));
                    OrphanWebhookViewModel model = new OrphanWebhookViewModel()
                    {
                        //attachments = new List<Attachment>
                        //{
                        //    new Attachment
                        //    {
                        //        color = "#FFF",
                        //        author_name = ticket.topic,
                        //        author_link = baseUrl+"tickets/ticketitem/" + ticket.id
                        //    },
                        //      new Attachment
                        //    {
                        //        author_name = "Suppress Ticket",
                        //        author_link= baseUrl +"orphan/SuppressTicket/" + ticket.id + "/?isExternal=true",
                        //        color = "#FFF",
                        //    },
                        //        new Attachment
                        //    {
                        //        author_name = "Trash ",
                        //        author_link= baseUrl +"Tickets/ChnageTicketStatus/" + ticket.id +"/?status="+8+ "&isExternal=true",
                        //        color = "#FFF",
                        //    }
                        //},
                        
                       
                        //text = ticket.Age.ToString() + " days old ticket",
                        text = "12 days old ticket | " +
                        "[" + topic + "]" + "(" +baseUrl + "tickets/ticketitem/"+ ticket.id+ ")" + " | "  +
                        "[ Suppress ]" + "(" + baseUrl + "orphan/SuppressTicket/" + ticket.id + "/?isExternal=true)" + " | " +
                        "[ Trash ]" + "(" + baseUrl + "tickets/ChnageTicketStatus/" + ticket.id + "/?status=8&isExternal=true)" ,


                    };
                    var client = new HttpClient();
                    client.DefaultRequestHeaders.Accept.Clear();
                    var myContent = JsonConvert.SerializeObject(model);
                    var buffer = System.Text.Encoding.UTF8.GetBytes(myContent);
                    var byteContent = new ByteArrayContent(buffer);
                    byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/Json");
                    var responseTask = client.PostAsync(ConfigurationManager.AppSettings["UnAssignedRocketWebhook"].ToString(), byteContent);
                    responseTask.Wait();
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