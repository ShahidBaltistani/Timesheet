using computan.graphapi.DTO;
using Microsoft.Graph;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using File = System.IO.File;

namespace computan.graphapi
{
    public class GraphMail : IGraphMail
    {
        private static readonly string client_secret = ConfigurationManager.AppSettings["client_secret"];
        private static readonly string client_id = ConfigurationManager.AppSettings["client_id"];
        private static readonly string grant_type = ConfigurationManager.AppSettings["grant_type"];
        private static readonly string redirect_uri = ConfigurationManager.AppSettings["redirect_uri"];
        private readonly string constr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();

        public async Task SendAsync(GraphMailModel mailModel, List<HttpPostedFileBase> file)
        {
            GraphServiceClient gclient = GetAuthenticatedClient(AccessToken(mailModel.RefreshToken));
            Message email = CreateMessage(mailModel);
            if (!string.IsNullOrEmpty(mailModel.type) && !string.IsNullOrEmpty(mailModel.MessageId))
            {
                Message graphMessage = await gclient.Me.Messages[mailModel.MessageId].Request().GetAsync();
                if (graphMessage != null && graphMessage.Id != null)
                {
                    switch (mailModel.type)
                    {
                        case "Reply":
                            await Reply(gclient, graphMessage.Id, email, mailModel.Attach, file);
                            break;

                        case "ReplyAll":
                            await ReplyAll(gclient, graphMessage.Id, email, mailModel.Attach, file);
                            break;

                        case "Forward":
                            await ForwardTo(gclient, graphMessage.Id, email, mailModel.Attach, file);
                            break;

                        default:
                            await SendNew(gclient, email, mailModel.Attach, file);
                            break;
                    }
                }
                else
                {
                    await SendNew(gclient, email, mailModel.Attach, file);
                }
            }
            else
            {
                await SendNew(gclient, email, mailModel.Attach, file);
            }
        }

        private async Task SendNew(GraphServiceClient graph, Message msg, string Attach, List<HttpPostedFileBase> file)
        {
            Message response = await graph.Me.Messages.Request().AddAsync(msg);
            await AttachFile(graph, Attach, file, response.Id);
            await graph.Me.Messages[response.Id].Request().UpdateAsync(msg);
            await graph.Me.Messages[response.Id].Send().Request().PostAsync();
        }

        private async Task Reply(GraphServiceClient graph, string Messageid, Message msg, string Attach,
            List<HttpPostedFileBase> file)
        {
            try
            {
                Message response = await graph.Me.Messages[Messageid].CreateReply().Request().PostAsync();
                await AttachFile(graph, Attach, file, response.Id);
                await graph.Me.Messages[response.Id].Request().UpdateAsync(msg);
                await graph.Me.Messages[response.Id].Send().Request().PostAsync();
            }
            catch
            {
                await SendNew(graph, msg, Attach, file);
            }
        }

        private async Task ReplyAll(GraphServiceClient graph, string Messageid, Message msg, string Attach,
            List<HttpPostedFileBase> file)
        {
            try
            {
                Message response = await graph.Me.Messages[Messageid].CreateReplyAll().Request().PostAsync();
                await AttachFile(graph, Attach, file, response.Id);
                await graph.Me.Messages[response.Id].Request().UpdateAsync(msg);
                await graph.Me.Messages[response.Id].Send().Request().PostAsync();
            }
            catch
            {
                await SendNew(graph, msg, Attach, file);
            }
        }

        private async Task ForwardTo(GraphServiceClient graph, string Messageid, Message msg, string Attach,
            List<HttpPostedFileBase> file)
        {
            Message response = await graph.Me.Messages[Messageid].CreateForward().Request().PostAsync();
            await AttachFile(graph, Attach, file, response.Id);
            await graph.Me.Messages[response.Id].Request().UpdateAsync(msg);
            await graph.Me.Messages[response.Id].Send().Request().PostAsync();
        }

        private string AccessToken(string RefreshToken)
        {
            //Sent request to microsoft online to get bearer token against our App
            RestClient client = new RestClient("https://login.microsoftonline.com/computan.onmicrosoft.com/oauth2/token");
            RestRequest request = new RestRequest(Method.POST);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddParameter("client_id", client_id);
            request.AddParameter("client_secret", client_secret);
            request.AddParameter("grant_type", grant_type);
            request.AddParameter("redirect_uri", redirect_uri);
            request.AddParameter("refresh_token", RefreshToken);
            IRestResponse response = client.Execute(request);
            //convert token response to json to get token
            dynamic Content = JsonConvert.DeserializeObject(response.Content);
            return Content.access_token;
        }

        public List<Recipient> ReplyTo()
        {
            List<Recipient> replyTo = new List<Recipient>
            {
                new Recipient
                {
                    EmailAddress = new EmailAddress
                    {
                        Address = "websupport@computan.com"
                    }
                }
            };
            return replyTo;
        }

        public List<Recipient> EmailRecipient(string value)
        {
            List<Recipient> recipient = new List<Recipient>();
            if (!string.IsNullOrEmpty(value))
            {
                string[] emails = value.Split(',');
                foreach (string email in emails)
                {
                    recipient.Add(new Recipient
                    {
                        EmailAddress = new EmailAddress
                        {
                            Address = email
                        }
                    });
                }
            }

            return recipient;
        }

        private Message CreateMessage(GraphMailModel mailModel)
        {
            if (mailModel.Subject.Substring(0, 3).ToUpper() == "RE:")
            {
                mailModel.Subject = mailModel.Subject.Remove(0, 3);
            }
            // Prepare mailmessage.
            Message email = new Message
            {
                Body = new ItemBody
                {
                    Content = mailModel.body,
                    ContentType = BodyType.Html
                },
                HasAttachments = true,
                Subject = mailModel.Subject,
                ToRecipients = EmailRecipient(mailModel.TO),
                CcRecipients = EmailRecipient(mailModel.CC),
                BccRecipients = EmailRecipient(mailModel.BCC),
                ReplyTo = ReplyTo()
            };
            return email;
        }

        private async Task AttachFile(GraphServiceClient graph, string Attach, List<HttpPostedFileBase> file,
            string MsgID = "")
        {
            //For new attachments
            foreach (HttpPostedFileBase files in file)
            {
                if (files.ContentLength < 3145700)
                {
                    FileAttachment attachfile = new FileAttachment
                    {
                        ODataType = "#microsoft.graph.fileAttachment",
                        ContentBytes = ReadFully(files.InputStream),
                        IsInline = false,
                        Name = files.FileName,
                        Size = files.ContentLength
                    };
                    await graph.Me.Messages[MsgID].Attachments.Request().AddAsync(attachfile);
                }
                else
                {
                    int maxChunkSize = 327680;
                    AttachmentItem attachmentItem = new AttachmentItem
                    {
                        AttachmentType = AttachmentType.File,
                        Name = files.FileName,
                        Size = files.ContentLength
                    };
                    UploadSession uploadSession = await graph.Me.Messages[MsgID].Attachments.CreateUploadSession(attachmentItem)
                        .Request().PostAsync();
                    LargeFileUploadTask<AttachmentItem> provider =
                        new LargeFileUploadTask<AttachmentItem>(uploadSession, files.InputStream, maxChunkSize);
                    await provider.UploadAsync();
                }
            }

            //For existing attachments
            if (Attach != "")
            {
                Attach = Attach.Remove(Attach.Length - 1);
                string[] Attachments = Attach.Split(',');
                foreach (string attachment in Attachments)
                {
                    string[] attachinfo = attachment.Split('_');
                    if (attachinfo.Length > 0)
                    {
                        if (attachinfo[1] == "E")
                        {
                            string existingAttachment;
                            int id = Convert.ToInt32(attachinfo[0]);
                            using (SqlConnection con = new SqlConnection(constr))
                            {
                                SqlCommand cmd = new SqlCommand(
                                    "SELECT path FROM [timesheetdb].[dbo].[TicketItemAttachments] where id = " + id,
                                    con);
                                con.Open();
                                existingAttachment = cmd.ExecuteScalar().ToString();
                                con.Close();
                            }

                            if (existingAttachment != null &&
                                File.Exists(AppDomain.CurrentDomain.BaseDirectory + existingAttachment))
                            {
                                byte[] contentBytes =
                                    File.ReadAllBytes(AppDomain.CurrentDomain.BaseDirectory + existingAttachment);
                                string attachmentname = existingAttachment.Split('/').Last();
                                if (contentBytes.Length < 3145700)
                                {
                                    FileAttachment attachfile = new FileAttachment
                                    {
                                        ODataType = "#microsoft.graph.fileAttachment",
                                        ContentBytes = contentBytes,
                                        IsInline = false,
                                        Name = attachmentname,
                                        Size = contentBytes.Length
                                    };
                                    await graph.Me.Messages[MsgID].Attachments.Request().AddAsync(attachfile);
                                }
                                else
                                {
                                    using (Stream fs = new FileStream(
                                               AppDomain.CurrentDomain.BaseDirectory + existingAttachment,
                                               FileMode.Open, FileAccess.Read))
                                    {
                                        int maxChunkSize = 327680;
                                        AttachmentItem attachmentItem = new AttachmentItem
                                        {
                                            AttachmentType = AttachmentType.File,
                                            Name = attachmentname,
                                            Size = fs.Length
                                        };
                                        UploadSession uploadSession = await graph.Me.Messages[MsgID].Attachments
                                            .CreateUploadSession(attachmentItem).Request().PostAsync();
                                        LargeFileUploadTask<AttachmentItem> provider =
                                            new LargeFileUploadTask<AttachmentItem>(uploadSession, fs, maxChunkSize);
                                        await provider.UploadAsync();
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public static byte[] ReadFully(Stream input)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                input.CopyTo(ms);
                return ms.ToArray();
            }
        }

        private static GraphServiceClient GetAuthenticatedClient(string accessToken)
        {
            GraphServiceClient graphClient = new GraphServiceClient(
                new DelegateAuthenticationProvider(
#pragma warning disable 1998
                    async requestMessage =>
#pragma warning restore 1998
                    {
                        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    }));
            return graphClient;
        }
    }
}