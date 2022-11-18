using computan.timesheet.Contexts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Script.Serialization;

namespace computan.timesheet.Models
{
    public static class NotificatonViewmodel
    {
        private const string serverKey =
            "AAAACs9hgBc:APA91bGvDNEEexOPHUlq3C6mpdEGA2fVxAulFiGbfK4ic3tAK0q1nOUZFAYnAY5ADhneB2JpXtreE2LK-FbBuFFfV6yTkVsGE2_UWR5a0plyAEOGVuMJf5cVt9FuoXxKi8Cb1H6lrDAp";

        private static readonly string baseurl = "https://timesheet.computan.com";
        private static readonly ApplicationDbContext db = new ApplicationDbContext();

        public static string SendPushNotification(SendNotificationViewModel notification)
        {
            string clickaction = null;
            try
            {
                FcmNotificationResult result = new FcmNotificationResult();
                List<string> tokens = new List<string>();
                foreach (string item in notification.users)
                {
                    core.UserBrowserinfo token = (from x in db.UserBrowserinfo where x.userId == item && x.isActive select x)
                        .OrderByDescending(b => b.id).FirstOrDefault();
                    if (token != null && !string.IsNullOrEmpty(token.token))
                    {
                        tokens.Add(token.token);
                    }
                }

                if (tokens.Count > 0)
                {
                    if (notification.notification.entityactionid == 7)
                    {
                        clickaction = baseurl + "/tickets/comment/" + notification.notification.entityid;
                        if (notification.notification.commentid != 0)
                        {
                            clickaction += "/" + notification.notification.commentid;
                        }
                    }
                    else if (notification.notification.entityactionid == 10 &&
                             notification.notification.entityactionid == 9)
                    {
                        clickaction = baseurl + "/Settings/TimeEntry/";
                    }
                    else
                    {
                        clickaction = baseurl + "/tickets/ticketitem/" + notification.notification.entityid;
                    }

                    WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                    tRequest.Method = "post";
                    tRequest.Headers.Add(string.Format("Authorization: key={0}", serverKey));
                    //tRequest.Headers.Add(string.Format("Sender: id={0}", senderId));
                    tRequest.ContentType = "application/json";
                    //string postData = "{\"collapse_key\":\"score_update\",\"time_to_live\":108,\"delay_while_idle\":true,\"data\": { \"message\" : " + "here is test notification" + ",\"time\": " + "\"" + System.DateTime.Now.ToString() + "\"},\"registration_ids\":[\"" + regIds + "\"]}";
                    var payload = new
                    {
                        registration_ids = tokens,
                        collapse_key = "type_c",
                        notification = new
                        {
                            body = notification.notification.description,
                            notification.title,
                            badge = 0,
                            icon = "computanLogo.png",
                            click_action = clickaction,
                            sound = "notification sound.mp3"
                        },
                        data = new
                        {
                            ticketid = notification.notification.entityid,
                            comment = notification.notification.commentid,
                            action = notification.notification.entityactionid,
                            title = "",
                            color = " #rrggbb"
                        }
                    };
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    string postbody = JsonConvert.SerializeObject(payload);
                    byte[] byteArray = Encoding.UTF8.GetBytes(postbody);
                    tRequest.ContentLength = byteArray.Length;
                    using (Stream dataStream = tRequest.GetRequestStream())
                    {
                        dataStream.Write(byteArray, 0, byteArray.Length);
                        using (WebResponse tResponse = tRequest.GetResponse())
                        {
                            //result = tResponse;
                            using (Stream dataStreamResponse = tResponse.GetResponseStream())
                            {
                                if (dataStreamResponse != null)
                                {
                                    using (StreamReader tReader = new StreamReader(dataStreamResponse))
                                    {
                                        string Response = tReader.ReadToEnd();
                                        result = JsonConvert.DeserializeObject<FcmNotificationResult>(Response);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }

            return "";
        }
    }
}