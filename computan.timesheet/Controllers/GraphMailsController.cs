using computan.graphapi;
using computan.graphapi.DTO;
using computan.timesheet.core;
using computan.timesheet.core.common;
using computan.timesheet.Helpers;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace computan.timesheet.Controllers
{
    [CustomeAuthorizeAttribute]
    public class GraphMailsController : BaseController
    {
        private readonly IGraphMail _graphMail;

        public GraphMailsController(IGraphMail graphMail)
        {
            _graphMail = graphMail;
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> SendMail(string data)
        {
            try
            {
                //Get attached files
                HttpFileCollectionBase files = Request.Files;
                List<HttpPostedFileBase> file = new List<HttpPostedFileBase>();
                for (int i = 0; i < files.Count; i++)
                {
                    file.Add(files[i]);
                }

                GraphMailModel mailModel = JsonConvert.DeserializeObject<GraphMailModel>(data);
                mailModel.RefreshToken = db.integration
                    .Where(x => x.name == IntegrationSystem.GraphApi.ToString() && x.isenabled).FirstOrDefault()
                    .appsettings;
                //Save email
                if (mailModel.SentItemID == 0)
                {
                    SentItemLog sentitem = new SentItemLog
                    {
                        To = mailModel.TO,
                        Cc = mailModel.CC,
                        Bcc = mailModel.BCC,
                        ticketId = mailModel.TicketID,
                        ticket_title = mailModel.TicketTitle,
                        subject = mailModel.Subject,
                        body = mailModel.body,
                        updatedonutc = DateTime.Now,
                        createdonutc = DateTime.Now,
                        Sentdate = DateTime.Now,
                        IsSent = false,
                        ipused = Request.UserHostAddress,
                        userid = User.Identity.GetUserId()
                    };
                    db.SentItemLog.Add(sentitem);
                    db.SaveChanges();
                    mailModel.SentItemID = sentitem.id;
                }

                //Send email
                await _graphMail.SendAsync(mailModel, file);
                //Update LastActivety Date of Ticket
                if (mailModel.type.Equals("Reply") || mailModel.type.Equals("ReplyAll"))
                {
                    Ticket ticketData = db.Ticket.Find(mailModel.TicketID);
                    ticketData.LastActivityDate = DateTime.Now;
                    db.SaveChanges();
                }

                // Update mail sent status
                SentItemLog sentItemLog = db.SentItemLog.Find(mailModel.SentItemID);
                sentItemLog.IsSent = true;
                db.Entry(sentItemLog).State = EntityState.Modified;
                db.SaveChanges();
                // Save files
                if (file.Count > 0)
                {
                    UploadAttachments(file, mailModel);
                }

                return Json(new { error = false, response = "Email Send Successfully" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(
                    new
                    {
                        error = true,
                        response =
                            "An unknown error occurred while sending the email message. Please try again later or connect to the admin."
                    }, JsonRequestBehavior.AllowGet);
            }
        }

        private void UploadAttachments(List<HttpPostedFileBase> files, GraphMailModel mailModel)
        {
            // Create Outgoing directory, if not exists.
            if (!Directory.Exists(Server.MapPath("~/Attachments/Outgoing")))
            {
                Directory.CreateDirectory(Server.MapPath("~/Attachments/Outgoing"));
            }
            // Create ticket directory if not exists.
            if (!Directory.Exists(Server.MapPath("~/Attachments/Outgoing/" + mailModel.TicketID)))
            {
                Directory.CreateDirectory(Server.MapPath("~/Attachments/Outgoing/" + mailModel.TicketID));
            }

            foreach (HttpPostedFileBase file in files)
            {
                string filename, physicalpath;
                if (mailModel.TicketID > 0)
                {
                    filename = "/Attachments/Outgoing/" + mailModel.TicketID + "/" + file.FileName;
                    physicalpath = Server.MapPath("~/Attachments/Outgoing/" + mailModel.TicketID + "/") + file.FileName;

                    if (System.IO.File.Exists(physicalpath))
                    {
                        string uid = Guid.NewGuid().ToString().Replace("-", "");
                        // create new guid folder
                        if (!Directory.Exists(
                                Server.MapPath("~/Attachments/Outgoing/" + mailModel.TicketID + "/" + uid)))
                        {
                            Directory.CreateDirectory(
                                Server.MapPath("~/Attachments/Outgoing/" + mailModel.TicketID + "/" + uid));
                        }

                        filename = "/Attachments/Outgoing/" + mailModel.TicketID + "/" + uid + "/" + file.FileName;
                        physicalpath =
                            Server.MapPath("~/Attachments/Outgoing/" + mailModel.TicketID + "/" + uid + "/") +
                            file.FileName;
                    }
                }
                else
                {
                    filename = "/Attachments/Outgoing/" + file.FileName;
                    physicalpath = Server.MapPath("~/Attachments/Outgoing/") + file.FileName;
                    if (System.IO.File.Exists(physicalpath))
                    {
                        string uid = Guid.NewGuid().ToString().Replace("-", "");
                        // create new guid folder
                        if (!Directory.Exists(Server.MapPath("~/Attachments/Outgoing/" + uid)))
                        {
                            Directory.CreateDirectory(Server.MapPath("~/Attachments/Outgoing/" + uid));
                        }

                        filename = "/Attachments/Outgoing/" + uid + "/" + file.FileName;
                        physicalpath = Server.MapPath("~/Attachments/Outgoing/" + uid + "/") + file.FileName;
                    }
                }

                file.SaveAs(physicalpath);
                TicketReplay obj = new TicketReplay
                {
                    UserID = User.Identity.GetUserId(),
                    createdon = DateTime.Now,
                    Attatchment = filename,
                    TicketID = mailModel.TicketID,
                    Type = string.IsNullOrEmpty(mailModel.type) ? "New" : mailModel.type,
                    To = string.Empty,
                    CC = string.Empty,
                    BCC = string.Empty,
                    Body = string.Empty
                };
                db.TicketReplay.Add(obj);
                db.SaveChanges();
            }
        }
    }
}