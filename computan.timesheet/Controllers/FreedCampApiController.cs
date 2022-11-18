using computan.timesheet.Contexts;
using computan.timesheet.core;
using computan.timesheet.core.integrations;
using computan.timesheet.Infrastructure;
using computan.timesheet.Models;
using computan.timesheet.Models.FreedCamp;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;

namespace computan.timesheet.Controllers
{
    public class FreedCampApiController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        // GET: FreedCampApi
        public ActionResult Index()
        {
            return View();
        }

        //public async Task<ActionResult> Freedcamp()
        public ActionResult Freedcamp()
        {
            // Get Setting for integration
            Integration freedcamp = db.integration.Where(x => x.name == "freedcamp" && x.isenabled).FirstOrDefault();
            // Check syncing process is active
            if (freedcamp != null)
            {
                // Generate url for calling
                FreedcampSetting setting = new FreedcampSetting();
                setting = JsonConvert.DeserializeObject<FreedcampSetting>(freedcamp.appsettings);
                try
                {
                    //Sync all projects
                    SyncAllProjects(setting);

                    //sync all task
                    SyncAllTaskByProjects(setting);

                    //Sync all comments
                    SyncComments(setting);

                    ViewBag.Message = "All the task has been successfully sync!";
                    return View();
                }
                catch (Exception ex)
                {
                    MailService.SendClientEmail(ConfigurationManager.AppSettings["ToNotificatoins"], "Timesheet-Error",
                        "There is an error on timesheet while syncing freed camp projects<br>Here is the stack strace:<br>" +
                        ex, null);
                    throw ex;
                }
            }

            ViewBag.Message = "Syncing is stop please enable and try again!";
            return View();
        }

        // Sync all projects in Ts
        public void SyncAllProjects(FreedcampSetting setting)
        {
            // Get Setting for integration
            long ts = DateTimeOffset.Now.ToUnixTimeSeconds();
            byte[] keyByte = Encoding.Default.GetBytes(setting.privatekey);
            string url = setting.baseurl + "/projects?api_key=" + setting.publickey + "&timestamp=" + ts;
            using (HMACSHA1 hmacsha1 = new HMACSHA1(keyByte))
            {
                hmacsha1.ComputeHash(Encoding.Default.GetBytes(string.Concat(setting.publickey, ts)));
                string hashstr = ByteToString(hmacsha1.Hash);
                url += "&hash=" + hashstr;
            }

            try
            {
                // Sending Request
                CompleteProjectResponse response = new CompleteProjectResponse();
                RestClient client = new RestClient(url);
                RestRequest request = new RestRequest(Method.GET);
                request.AddHeader("cache-control", "no-cache");
                request.AddHeader("Connection", "keep-alive");
                request.AddHeader("Accept-Encoding", "gzip, deflate");
                request.AddHeader("Host", "freedcamp.com");
                if (client != null)
                {
                    IRestResponse webresponse = client.Execute(request);
                    response = JsonConvert.DeserializeObject<CompleteProjectResponse>(webresponse.Content);
                }

                //response conditions on response code
                if (response.http_code == 200)
                {
                    List<FcProject> projectlist = response.data.projects;
                    foreach (FcProject item in projectlist)
                    {
                        long prid = Convert.ToInt64(item.project_id);
                        int proje = db.FreedcampProject.Where(f => f.fcprojectid == prid).Count();
                        if (proje <= 0)
                        {
                            // Save project in database
                            FreedcampProject project = new FreedcampProject
                            {
                                fcprojectid = prid,
                                name = item.project_name,
                                isactive = true,
                                createdonutc = DateTime.Now,
                                updatedonutc = DateTime.Now,
                                ipused = Request.UserHostAddress
                            };
                            db.FreedcampProject.Add(project);
                            db.SaveChanges();
                        }
                    }
                }
                else if (response.http_code == 429)
                {
                    throw new Exception("Limit exceed please stop syncing for 30 minutes!");
                }
                else
                {
                    throw new Exception(response.msg);
                }
                //var webRequest = WebRequest.Create(new Uri(url)) as HttpWebRequest;
                //if (webRequest != null)
                //{
                //    webRequest.Method = "GET";
                //    webRequest.ContentType = "application/json";
                //    WebResponse webResponse = webRequest.GetResponse();

                //    using (Stream dataStream = webResponse.GetResponseStream())
                //    {
                //        StreamReader reader = new StreamReader(dataStream);
                //        string responseFromServer = reader.ReadToEnd();
                //        response = JsonConvert.DeserializeObject<CompleteProjectResponse>(responseFromServer);
                //    }
                //}
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
                throw ex;
            }
        }

        public void SyncAllTaskByProjects(FreedcampSetting setting)
        {
            long ts = DateTimeOffset.Now.ToUnixTimeSeconds();
            byte[] keyByte = Encoding.Default.GetBytes(setting.privatekey);
            // Get Project List
            List<FreedcampProject> projects = db.FreedcampProject.Where(x => x.isactive).ToList();
            foreach (FreedcampProject item in projects)
            {
                // Generate url for api
                string url = setting.baseurl + "/tasks?project_id=" + item.fcprojectid + "&api_key=" + setting.publickey +
                          "&timestamp=" + ts;
                using (HMACSHA1 hmacsha1 = new HMACSHA1(keyByte))
                {
                    hmacsha1.ComputeHash(Encoding.Default.GetBytes(string.Concat(setting.publickey, ts)));
                    string hashstr = ByteToString(hmacsha1.Hash);
                    url += "&hash=" + hashstr;
                }

                string date = DateTime.Now.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
                url = url + "&created_date[from]=" + date;
                CompleteTaskResponse response = new CompleteTaskResponse();
                try
                {
                    // Sending Request
                    RestClient client = new RestClient(url);
                    RestRequest request = new RestRequest(Method.GET);
                    request.AddHeader("cache-control", "no-cache");
                    request.AddHeader("Connection", "keep-alive");
                    request.AddHeader("Accept-Encoding", "gzip, deflate");
                    request.AddHeader("Host", "freedcamp.com");
                    if (client != null)
                    {
                        IRestResponse webresponse = client.Execute(request);
                        response = JsonConvert.DeserializeObject<CompleteTaskResponse>(webresponse.Content);
                    }

                    //response conditions on response code
                    if (response.http_code == 200)
                    {
                        //Condition if not in database
                        foreach (Task task in response.data.tasks)
                        {
                            long taskid = Convert.ToInt64(task.id);
                            FreedCampTask dbtasklist = db.freedCampTask
                                .Where(x => x.freedcamp_projectid == item.id && x.freedcamp_taskid == taskid)
                                .FirstOrDefault();
                            if (dbtasklist == null)
                            {
                                GetTaskById(setting, Convert.ToInt64(task.id), item.id, item.tsprojectid, item.skill,
                                    item.userid, item.assignedto, item.team, item.name, false);
                            }
                        }
                    }
                    else if (response.http_code == 404)
                    {
                        throw new Exception("Project not found projectid = " + item.fcprojectid);
                    }
                    else
                    {
                        throw new Exception(response.msg);
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
                    if (response.http_code == 429)
                    {
                        throw ex;
                    }
                }
            }
        }

        public void SyncComments(FreedcampSetting setting)
        {
            try
            {
                // Get Setting for integration
                List<FreedcampProject> projects = db.FreedcampProject.Where(x => x.isactive).ToList();
                foreach (FreedcampProject project in projects)
                {
                    long projectid = project.id;
                    List<FreedCampTask> tasks = db.freedCampTask.Where(x => x.freedcamp_projectid == projectid && x.statusid != 5)
                        .ToList();
                    foreach (FreedCampTask task in tasks)
                    {
                        GetTaskById(setting, task.freedcamp_taskid, projectid, project.tsprojectid, project.skill,
                            project.userid, project.assignedto, project.team, project.name, true);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void GetTaskById(FreedcampSetting setting, long taskid, long project_id, long? ts_project_id,
            int? skill_id, string statusupdatedby, string userscsv, string teamcsv, string projectName,
            bool isAlreadySynced)
        {
            //Generate users and team list
            List<string> userlist = new List<string>();
            List<long> teamlist = new List<long>();
            if (!string.IsNullOrEmpty(userscsv))
            {
                string[] users = userscsv.Split(',');
                foreach (string user in users)
                {
                    if (!string.IsNullOrEmpty(user))
                    {
                        userlist.Add(user);
                    }
                }
            }

            if (!string.IsNullOrEmpty(teamcsv))
            {
                string[] teams = teamcsv.Split(',');
                foreach (string team in teams)
                {
                    if (!string.IsNullOrEmpty(team))
                    {
                        teamlist.Add(Convert.ToInt64(team));
                    }
                }
            }

            CompleteTaskResponse response = new CompleteTaskResponse();
            long ts = DateTimeOffset.Now.ToUnixTimeSeconds();
            byte[] keyByte = Encoding.Default.GetBytes(setting.privatekey);
            string url = setting.baseurl + "/tasks/" + taskid + "?&api_key=" + setting.publickey + "&timestamp=" + ts;
            using (HMACSHA1 hmacsha1 = new HMACSHA1(keyByte))
            {
                hmacsha1.ComputeHash(Encoding.Default.GetBytes(string.Concat(setting.publickey, ts)));
                string hashstr = ByteToString(hmacsha1.Hash);
                url += "&hash=" + hashstr;
            }

            try
            {
                RestClient client = new RestClient(url);
                RestRequest request = new RestRequest(Method.GET);
                request.AddHeader("cache-control", "no-cache");
                request.AddHeader("Connection", "keep-alive");
                request.AddHeader("Accept-Encoding", "gzip, deflate");
                request.AddHeader("Host", "freedcamp.com");
                if (client != null)
                {
                    IRestResponse webresponse = client.Execute(request);
                    response = JsonConvert.DeserializeObject<CompleteTaskResponse>(webresponse.Content);
                    if (response.http_code == 200)
                    {
                        foreach (Task task in response.data.tasks)
                        {
                            if (!isAlreadySynced)
                            {
                                try
                                {
                                    // Create tickets from task
                                    Ticket ticket = new Ticket
                                    {
                                        topic = "[" + projectName + "] " + task.title,
                                        lastdeliverytime = UnixTimeStampToDateTime(task.created_ts),
                                        uniquesenders = "FreedCamp"
                                    };
                                    if (task.files_count > 0)
                                    {
                                        ticket.hasattachments = true;
                                    }

                                    ticket.flagstatusid = 1;
                                    ticket.conversationid = task.id;
                                    ticket.ipused = "162.144.159.171";
                                    ticket.lastmodifiedtime = UnixTimeStampToDateTime(task.created_ts);
                                    ticket.statusupdatedon = DateTime.Now;
                                    if (userlist.Count() > 0 || teamlist.Count() > 0)
                                    {
                                        ticket.statusid = 6;
                                    }
                                    else
                                    {
                                        ticket.statusid = 1;
                                    }

                                    ticket.createdonutc = DateTime.Now;
                                    ticket.updatedonutc = DateTime.Now;
                                    ticket.tickettypeid = 1;
                                    ticket.fromEmail = "do-not-reply@freedcamp.com";
                                    ticket.projectid = ts_project_id;
                                    ticket.skillid = skill_id;
                                    ticket.messagecount = task.comments_count + 1;
                                    ticket.IsArchieved = false;
                                    ticket.importance = false;
                                    ticket.LastActivityDate = DateTime.Now;
                                    if (task.priority_title == "High")
                                    {
                                        ticket.importance = true;
                                    }

                                    db.Ticket.Add(ticket);
                                    db.SaveChanges();

                                    // ticketTeamLog added
                                    foreach (long team in teamlist)
                                    {
                                        TicketTeamLogs teamlog = new TicketTeamLogs
                                        {
                                            ticketid = ticket.id,
                                            teamid = team,
                                            statusid = 6,
                                            assignedbyusersid = statusupdatedby,
                                            statusupdatedbyusersid = statusupdatedby,
                                            statusupdatedon = DateTime.Now,
                                            assignedon = DateTime.Now,
                                            displayorder = 1
                                        };

                                        db.TicketTeamLogs.Add(teamlog);
                                        db.SaveChanges();
                                    }

                                    // Save task in database
                                    FreedCampTask fctask = new FreedCampTask
                                    {
                                        title = task.title,
                                        description = task.description_processed,
                                        freedcamp_taskid = Convert.ToInt64(task.id),
                                        createddate = UnixTimeStampToDateTime(task.created_ts),
                                        ticketid = ticket.id,
                                        url = task.url,
                                        freedcamp_projectid = project_id,
                                        createdon = DateTime.Now,
                                        statusid = task.status
                                    };

                                    db.freedCampTask.Add(fctask);
                                    db.SaveChanges();

                                    // Create ticket item
                                    CreateTicketItem(ticket.id, task.title, task.description_processed, task.created_ts,
                                        task.priority_title, ts_project_id, skill_id, statusupdatedby, userlist,
                                        task.files);

                                    // Sync Comments as a ticketitem
                                    SaveComments(task.comments, ticket.id, fctask.id, task.title, ts_project_id,
                                        skill_id, statusupdatedby, userlist);

                                    //Send Notification
                                    if (!string.IsNullOrEmpty(statusupdatedby))
                                    {
                                        AddNotification(ticket.id, statusupdatedby, userlist, teamlist, 6);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    MailService.SendClientEmail(ConfigurationManager.AppSettings["ToNotificatoins"],
                                        "Timesheet-Error",
                                        "There is an error on timesheet while saving freedcamp tasks <br>Here is the stack strace:<br>" +
                                        ex, null);
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
                                        exception_targetsite = ex.TargetSite.DeclaringType.FullName + ", " +
                                                               ex.TargetSite.Name,
                                        ipused = Request.UserHostAddress,
                                        userid = User.Identity.GetUserId()
                                    };
                                    db.MyExceptions.Add(myex);
                                    db.SaveChanges();
                                }
                            }
                            else
                            {
                                try
                                {
                                    FreedCampTask fctask = db.freedCampTask.Where(x => x.freedcamp_taskid == taskid)
                                        .FirstOrDefault();
                                    Ticket ticket = db.Ticket.Find(fctask.ticketid);
                                    if (fctask.statusid != 1)
                                    {
                                        bool commentadded = SaveComments(task.comments, fctask.ticketid, fctask.id,
                                            task.title, ts_project_id, skill_id, statusupdatedby, userlist);
                                        //Send Notification and update ticket status
                                        if (commentadded)
                                        {
                                            ticket.statusid = 2;
                                            if (!string.IsNullOrEmpty(statusupdatedby))
                                            {
                                                AddNotification(fctask.ticketid, statusupdatedby, userlist, teamlist,
                                                    8);
                                            }
                                        }
                                    }

                                    //else
                                    //    ticket.statusid = 3;
                                    try
                                    {
                                        fctask.statusid = task.status;
                                        db.Entry(fctask).State = EntityState.Modified;
                                        db.SaveChanges();
                                        db.Entry(ticket).State = EntityState.Modified;
                                        db.SaveChanges();
                                    }
                                    catch (Exception ex)
                                    {
                                        MailService.SendClientEmail(ConfigurationManager.AppSettings["ToNotificatoins"],
                                            "Timesheet-Error",
                                            "There is an error on timesheet while updating freedcamp tasks and ticket status on comment <br>Here is the stack strace:<br>" +
                                            ex, null);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    MailService.SendClientEmail(ConfigurationManager.AppSettings["ToNotificatoins"],
                                        "Timesheet-Error",
                                        "There is an error on timesheet while saving freedcamp tasks comments <br>Here is the stack strace:<br>" +
                                        ex, null);
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
                                        exception_targetsite = ex.TargetSite.DeclaringType.FullName + ", " +
                                                               ex.TargetSite.Name,
                                        ipused = Request.UserHostAddress,
                                        userid = User.Identity.GetUserId()
                                    };
                                    db.MyExceptions.Add(myex);
                                    db.SaveChanges();
                                }
                            }
                        }
                    }
                    else if (response.http_code == 404)
                    {
                        FreedCampTask fctask = db.freedCampTask.Where(x => x.freedcamp_taskid == taskid).FirstOrDefault();
                        fctask.statusid = 5;
                        db.Entry(fctask).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        throw new Exception(response.msg);
                    }
                }
            }
            catch (Exception ex)
            {
                MailService.SendClientEmail(ConfigurationManager.AppSettings["ToNotificatoins"], "Timesheet-Error",
                    "There is an error on timesheet while freedcamp task syncing <br>Here is the stack strace:<br>" +
                    ex, null);
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
                if (response.http_code == 429)
                {
                    throw ex;
                }
            }
        }

        public bool SaveComments(List<TaskComment> Comments, long ticketid, long taskid, string title, long? project_id,
            int? skill_id, string statusupdatedby, List<string> userslist)
        {
            bool isadded = false;
            foreach (TaskComment comment in Comments)
            {
                long commentid = Convert.ToInt64(comment.id);
                FreedcampComment dbcomment = db.freedcampComment.Where(x => x.freedcamp_commentid == commentid).FirstOrDefault();
                if (dbcomment == null)
                {
                    long ticketitemid = CreateTicketItem(ticketid, title, comment.description_processed,
                        comment.created_ts, "Medium", project_id, skill_id, statusupdatedby, userslist, comment.files);
                    if (ticketitemid != -1)
                    {
                        FreedcampComment fccomment = new FreedcampComment
                        {
                            freedcamp_commentid = commentid,
                            freedcamp_taskid = taskid,
                            ticketitemid = ticketitemid
                        };

                        db.freedcampComment.Add(fccomment);
                        db.SaveChanges();
                        isadded = true;
                    }
                }
            }

            return isadded;
        }

        public void onetimesync()
        {
            Integration freedcamp = db.integration.Where(x => x.name == "freedcamp" && x.isenabled).FirstOrDefault();
            // Check syncing process is active
            if (freedcamp != null)
            {
                // Generate url for calling
                FreedcampSetting setting = new FreedcampSetting();
                setting = JsonConvert.DeserializeObject<FreedcampSetting>(freedcamp.appsettings);
                long ts = DateTimeOffset.Now.ToUnixTimeSeconds();
                byte[] keyByte = Encoding.Default.GetBytes(setting.privatekey);

                List<FreedcampProject> projects = db.FreedcampProject.Where(x => x.isactive).ToList();
                foreach (FreedcampProject project in projects)
                {
                    string url = setting.baseurl + "/tasks?api_key=" + setting.publickey + "&timestamp=" + ts;
                    using (HMACSHA1 hmacsha1 = new HMACSHA1(keyByte))
                    {
                        hmacsha1.ComputeHash(Encoding.Default.GetBytes(string.Concat(setting.publickey, ts)));
                        string hashstr = ByteToString(hmacsha1.Hash);
                        url += "&hash=" + hashstr;
                    }

                    url += "&project_id=" + project.fcprojectid;
                    try
                    {
                        CompleteTaskResponse response = new CompleteTaskResponse();
                        // Api calling to get task list for specific project
                        HttpWebRequest webRequest = WebRequest.Create(new Uri(url)) as HttpWebRequest;
                        if (webRequest != null)
                        {
                            webRequest.Method = "GET";
                            webRequest.ContentType = "application/json";
                            WebResponse webResponse = webRequest.GetResponse();
                            using (Stream dataStream = webResponse.GetResponseStream())
                            {
                                StreamReader reader = new StreamReader(dataStream);
                                string responseFromServer = reader.ReadToEnd();
                                response = JsonConvert.DeserializeObject<CompleteTaskResponse>(responseFromServer);
                            }

                            //Condition if not in database
                            foreach (Task task in response.data.tasks)
                            {
                                long taskid = Convert.ToInt64(task.id);
                                FreedCampTask dbtasklist = db.freedCampTask.Where(x =>
                                        x.freedcamp_projectid == project.id && x.freedcamp_taskid == taskid)
                                    .FirstOrDefault();
                                if (dbtasklist == null && task.status != 1)
                                {
                                    GetTaskById(setting, Convert.ToInt64(task.id), project.id, project.tsprojectid,
                                        project.skill, project.userid, project.assignedto, project.team, project.name,
                                        false);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }
        }

        #region Latest CronJob

        public void AddJob()
        {
            try
            {
                JobQueue queue = new JobQueue
                {
                    status = 1,
                    type = "Project",
                    url = "https://freedcamp.com/api/v1/projects",
                    addtime = DateTime.Now
                };
                db.jobQueue.Add(queue);
                db.SaveChanges();

                JobQueue queue2 = new JobQueue
                {
                    status = 1,
                    type = "ProjectTask",
                    url = "https://freedcamp.com/api/v1/tasks?status[]=2&offset=0&limit=150&order[created_date]=desc",
                    addtime = DateTime.Now
                };
                db.jobQueue.Add(queue2);
                db.SaveChanges();

                JobQueue queue3 = new JobQueue
                {
                    status = 1,
                    type = "ProjectTask",
                    url = "https://freedcamp.com/api/v1/tasks?status[]=0&offset=0&limit=150&order[created_date]=desc",
                    addtime = DateTime.Now
                };
                db.jobQueue.Add(queue3);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                MailService.SendClientEmail(ConfigurationManager.AppSettings["ToNotificatoins"], "Timesheet-Error",
                    "There is an error on timesheet while freedcamp task syncing <br>Here is the stack strace:<br>" +
                    ex, null);
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

        public void DoJob()
        {
            DateTime src = DateTime.Now;
            DateTime stratdate = new DateTime(src.Year, src.Month, src.Day, src.Hour, 0, 0);
            src = src.AddHours(1);
            DateTime enddate = new DateTime(src.Year, src.Month, src.Day, src.Hour, 0, 0);
            // Get time pool and request count between
            int requestcallscount = db.jobQueue
                .Where(x => x.status != 1 && x.completetime >= stratdate && x.completetime <= enddate).Count();
            requestcallscount += 50;
            if (requestcallscount < 290)
            {
                // Get upper 50 jobs and send
                List<JobQueue> requests = db.jobQueue.Where(x => x.status == 1).OrderBy(x => x.addtime).Take(50).ToList();
                foreach (JobQueue request in requests)
                {
                    try
                    {
                        switch (request.type)
                        {
                            case "Project":
                                try
                                {
                                    bool issuccess = FetchProject(request.url);
                                    //update request queue
                                    if (!issuccess)
                                    {
                                        break;
                                    }

                                    request.status = 2;
                                    request.response = "Successful";
                                }
                                catch (Exception ex)
                                {
                                    //request.status = 3;
                                    request.response = ex.Message;
                                }

                                request.completetime = DateTime.Now;
                                db.Entry(request).State = EntityState.Modified;
                                db.SaveChanges();
                                break;

                            case "ProjectTask":
                                try
                                {
                                    bool issuccess = FetchTask(request.url);
                                    if (!issuccess)
                                    {
                                        break;
                                    }

                                    request.status = 2;
                                    request.response = "Successful";
                                }
                                catch (Exception ex)
                                {
                                    //request.status = 3;
                                    request.response = ex.Message;
                                }

                                request.completetime = DateTime.Now;
                                db.Entry(request).State = EntityState.Modified;
                                db.SaveChanges();
                                break;

                            case "TaskComment":
                                try
                                {
                                    bool issuccess = FetchTaskComment(request.url);
                                    if (!issuccess)
                                    {
                                        break;
                                    }

                                    request.status = 2;
                                    request.response = "Successful";
                                }
                                catch (Exception ex)
                                {
                                    //request.status = 3;
                                    request.response = ex.Message;
                                }

                                request.completetime = DateTime.Now;
                                db.Entry(request).State = EntityState.Modified;
                                db.SaveChanges();
                                break;
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
                        MailService.SendClientEmail(ConfigurationManager.AppSettings["ToNotificatoins"],
                            "Timesheet-Error",
                            "There is an error on timesheet while freedcamp task syncing. At " + myex.controller + "/" +
                            myex.action + " <br>Here is the stack strace:<br>" + ex, null);
                    }
                }
            }
        }

        #endregion Latest CronJob

        #region Latest API Calls Methords

        public bool FetchProject(string requesturl)
        {
            // "url": "https://freedcamp.com/api/v1/projects"
            Integration freedcamp = db.integration.Where(x => x.name == "freedcamp" && x.isenabled).FirstOrDefault();
            if (freedcamp != null)
            {
                FreedcampSetting setting = new FreedcampSetting();
                setting = JsonConvert.DeserializeObject<FreedcampSetting>(freedcamp.appsettings);
                // Get Setting for integration
                long ts = DateTimeOffset.Now.ToUnixTimeSeconds();
                byte[] keyByte = Encoding.Default.GetBytes(setting.privatekey);
                string url = requesturl + "?api_key=" + setting.publickey + "&timestamp=" + ts;
                using (HMACSHA1 hmacsha1 = new HMACSHA1(keyByte))
                {
                    hmacsha1.ComputeHash(Encoding.Default.GetBytes(string.Concat(setting.publickey, ts)));
                    string hashstr = ByteToString(hmacsha1.Hash);
                    url += "&hash=" + hashstr;
                }

                try
                {
                    // Request API call send
                    CompleteProjectResponse response = new CompleteProjectResponse();
                    RestClient client = new RestClient(url);
                    RestRequest request = new RestRequest(Method.GET);
                    request.AddHeader("cache-control", "no-cache");
                    request.AddHeader("Connection", "keep-alive");
                    request.AddHeader("Accept-Encoding", "gzip, deflate");
                    request.AddHeader("Host", "freedcamp.com");
                    if (client != null)
                    {
                        IRestResponse webresponse = client.Execute(request);
                        response = JsonConvert.DeserializeObject<CompleteProjectResponse>(webresponse.Content);
                    }

                    if (response.http_code == 200 && response.data != null)
                    {
                        //If new project added than save the project
                        List<FcProject> projectlist = response.data.projects;
                        foreach (FcProject item in projectlist)
                        {
                            long prid = Convert.ToInt64(item.project_id);
                            int proje = db.FreedcampProject.Where(f => f.fcprojectid == prid).Count();
                            if (proje <= 0)
                            {
                                // Save project in database
                                FreedcampProject project = new FreedcampProject
                                {
                                    fcprojectid = prid,
                                    name = item.project_name,
                                    isactive = true,
                                    createdonutc = DateTime.Now,
                                    updatedonutc = DateTime.Now,
                                    ipused = Request.UserHostAddress
                                };
                                db.FreedcampProject.Add(project);
                                db.SaveChanges();
                            }
                        }

                        // Add New Request For Project Sync
                        AddJob("Project", requesturl);

                        return true;
                    }

                    if (response.http_code == 401)
                    {
                        // msg: Wrong credentials
                        AddJob("Project", requesturl);
                        throw new Exception(response.msg);
                    }

                    if (response.http_code == 404)
                    {
                        // msg: Unknown method
                        throw new Exception(response.msg);
                    }

                    if (response.http_code == 429)
                    {
                        // msg: limit exceed
                        AddJob("Project", requesturl);
                        freedcamp.isenabled = false;
                        db.Entry(freedcamp).State = EntityState.Modified;
                        db.SaveChanges();

                        throw new Exception(response.msg);
                    }

                    // 500 internal server error try again
                    AddJob("Project", requesturl);
                    throw new Exception("internal server error");
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
                    MailService.SendClientEmail(ConfigurationManager.AppSettings["ToNotificatoins"], "Timesheet-Error",
                        "There is an error on timesheet while freedcamp task syncing. At " + myex.controller + "/" +
                        myex.action + " <br>Here is the stack strace:<br>" + ex, null);
                    throw ex;
                }
            }

            return false;
        }

        public bool FetchTask(string requesturl)
        {
            //requesturl =  "https://freedcamp.com/api/v1/tasks?status[]=0&offset=0&limit=150"
            Integration freedcamp = db.integration.Where(x => x.name == "freedcamp" && x.isenabled).FirstOrDefault();
            if (freedcamp != null)
            {
                FreedcampSetting setting = new FreedcampSetting();
                setting = JsonConvert.DeserializeObject<FreedcampSetting>(freedcamp.appsettings);
                // Get Setting for integration
                long ts = DateTimeOffset.Now.ToUnixTimeSeconds();
                byte[] keyByte = Encoding.Default.GetBytes(setting.privatekey);
                string url = requesturl + "&api_key=" + setting.publickey + "&timestamp=" + ts;
                using (HMACSHA1 hmacsha1 = new HMACSHA1(keyByte))
                {
                    hmacsha1.ComputeHash(Encoding.Default.GetBytes(string.Concat(setting.publickey, ts)));
                    string hashstr = ByteToString(hmacsha1.Hash);
                    url += "&hash=" + hashstr;
                }

                try
                {
                    CompleteTaskResponse response = new CompleteTaskResponse();
                    RestClient client = new RestClient(url);
                    RestRequest request = new RestRequest(Method.GET);
                    request.AddHeader("cache-control", "no-cache");
                    request.AddHeader("Connection", "keep-alive");
                    request.AddHeader("Accept-Encoding", "gzip, deflate");
                    request.AddHeader("Host", "freedcamp.com");
                    if (client != null)
                    {
                        IRestResponse webresponse = client.Execute(request);
                        response = JsonConvert.DeserializeObject<CompleteTaskResponse>(webresponse.Content);
                    }

                    if (response.http_code == 200 && response.data != null)
                    {
                        // sync all the task and collect comments count and fie count
                        List<Task> responsetasklist = response.data.tasks;
                        foreach (Task task in responsetasklist)
                        {
                            long taskid = Convert.ToInt64(task.id);
                            //Check if task already have in TS
                            FreedCampTask datatask = db.freedCampTask.Where(x => x.freedcamp_taskid == taskid).FirstOrDefault();
                            if (datatask != null)
                            {
                                if (datatask.commentscount < task.comments_count
                                    || datatask.filescount < task.files_count)
                                {
                                    string newurl = "https://freedcamp.com/api/v1/tasks/" + task.id;
                                    AddJob("TaskComment", newurl);
                                }
                                //Add job for task comment
                                // url = https://freedcamp.com/api/v1/tasks/"taskid"
                            }
                            else
                            {
                                // Create ticket
                                CreateTicket(task);
                            }
                        }

                        TaskMeta reponsemeta = response.data.meta;
                        string nexturl = TaskNextUrl(requesturl, reponsemeta.has_more);
                        AddJob("ProjectTask", nexturl);
                        return true;
                    }

                    if (response.http_code == 401)
                    {
                        AddJob("ProjectTask", requesturl);
                        // msg: Wrong credentials
                        throw new Exception(response.msg);
                    }

                    if (response.http_code == 404)
                    {
                        AddJob("ProjectTask", requesturl);
                        // msg: Unknown method
                        throw new Exception(response.msg);
                    }

                    if (response.http_code == 429)
                    {
                        // msg: limit exceed
                        AddJob("ProjectTask", requesturl);
                        freedcamp.isenabled = false;
                        db.Entry(freedcamp).State = EntityState.Modified;
                        db.SaveChanges();
                        throw new Exception(response.msg);
                    }

                    // 500 internal server error try again
                    AddJob("ProjectTask", requesturl);
                    throw new Exception(response.msg);
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
                    MailService.SendClientEmail(ConfigurationManager.AppSettings["ToNotificatoins"], "Timesheet-Error",
                        "There is an error on timesheet while freedcamp task syncing. At " + myex.controller + "/" +
                        myex.action + " <br>Here is the stack strace:<br>" + ex, null);
                    throw ex;
                }
            }

            return false;
        }

        public bool FetchTaskComment(string requesturl)
        {
            //"url" : "https://freedcamp.com/api/v1/tasks/25515583"
            Integration freedcamp = db.integration.Where(x => x.name == "freedcamp" && x.isenabled).FirstOrDefault();
            if (freedcamp != null)
            {
                FreedcampSetting setting = new FreedcampSetting();
                setting = JsonConvert.DeserializeObject<FreedcampSetting>(freedcamp.appsettings);
                // Get Setting for integration
                long ts = DateTimeOffset.Now.ToUnixTimeSeconds();
                byte[] keyByte = Encoding.Default.GetBytes(setting.privatekey);
                string url = requesturl + "?api_key=" + setting.publickey + "&timestamp=" + ts;
                using (HMACSHA1 hmacsha1 = new HMACSHA1(keyByte))
                {
                    hmacsha1.ComputeHash(Encoding.Default.GetBytes(string.Concat(setting.publickey, ts)));
                    string hashstr = ByteToString(hmacsha1.Hash);
                    url += "&hash=" + hashstr;
                }

                try
                {
                    CompleteTaskResponse response = new CompleteTaskResponse();
                    RestClient client = new RestClient(url);
                    RestRequest request = new RestRequest(Method.GET);
                    request.AddHeader("cache-control", "no-cache");
                    request.AddHeader("Connection", "keep-alive");
                    request.AddHeader("Accept-Encoding", "gzip, deflate");
                    request.AddHeader("Host", "freedcamp.com");
                    if (client != null)
                    {
                        IRestResponse webresponse = client.Execute(request);
                        response = JsonConvert.DeserializeObject<CompleteTaskResponse>(webresponse.Content);
                    }

                    if (response.http_code == 200 && response.data != null)
                    {
                        List<Task> tasklist = response.data.tasks;
                        foreach (Task task in tasklist)
                        {
                            long taskid = Convert.ToInt64(task.id);
                            //Check if task already have in TS
                            FreedCampTask taskdata = db.freedCampTask.Where(x => x.freedcamp_taskid == taskid).FirstOrDefault();
                            if (taskdata != null)
                            {
                                SaveTaskComments(task.comments, taskdata.ticketid, taskdata.id,
                                    taskdata.freedcamp_projectid, task.title);
                                if (taskdata.filescount < task.files_count)
                                {
                                    TicketItem ticketitem = db.TicketItem.Where(x => x.ticketid == taskdata.ticketid)
                                        .FirstOrDefault();
                                    List<TaskFile> syncfiles = new List<TaskFile>();
                                    foreach (TaskFile file in task.files)
                                    {
                                        TicketItemAttachment filedata = db.TicketItemAttachment.Where(x => x.attachmentid == file.id)
                                            .FirstOrDefault();
                                        if (filedata == null)
                                        {
                                            syncfiles.Add(file);
                                        }
                                        //Identify and download single file here
                                    }

                                    DownloadAttachments(syncfiles, ticketitem.id);
                                }

                                taskdata.commentscount = task.comments_count;
                                taskdata.filescount = task.files_count;
                                db.Entry(taskdata).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                        }

                        return true;
                    }

                    if (response.http_code == 401)
                    {
                        // msg: Wrong credentials
                        AddJob("TaskComment", requesturl);
                        throw new Exception(response.msg);
                    }

                    if (response.http_code == 404)
                    {
                        // msg: Unknown method

                        throw new Exception(response.msg);
                    }

                    if (response.http_code == 429)
                    {
                        // msg: limit exceed
                        AddJob("TaskComment", requesturl);
                        freedcamp.isenabled = false;
                        db.Entry(freedcamp).State = EntityState.Modified;
                        db.SaveChanges();
                        throw new Exception(response.msg);
                    }

                    // 500 internal server error try again
                    AddJob("TaskComment", requesturl);
                    throw new Exception(response.msg);
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
                    MailService.SendClientEmail(ConfigurationManager.AppSettings["ToNotificatoins"], "Timesheet-Error",
                        "There is an error on timesheet while freedcamp task syncing. At " + myex.controller + "/" +
                        myex.action + " <br>Here is the stack strace:<br>" + ex, null);
                    throw ex;
                }
            }

            return false;
        }

        // helper Methord to add data in database
        private void CreateTicket(Task task)
        {
            long projectid = Convert.ToInt64(task.project_id);
            FreedcampProject project = db.FreedcampProject.Where(x => x.fcprojectid == projectid).FirstOrDefault();
            //Generate users and team list
            List<string> userlist = new List<string>();
            List<long> teamlist = new List<long>();
            if (!string.IsNullOrEmpty(project.assignedto))
            {
                string[] users = project.assignedto.Split(',');
                foreach (string user in users)
                {
                    if (!string.IsNullOrEmpty(user))
                    {
                        userlist.Add(user);
                    }
                }
            }

            if (!string.IsNullOrEmpty(project.team))
            {
                string[] teams = project.team.Split(',');
                foreach (string team in teams)
                {
                    if (!string.IsNullOrEmpty(team))
                    {
                        teamlist.Add(Convert.ToInt64(team));
                    }
                }
            }

            try
            {
                // Create tickets from task
                Ticket ticket = new Ticket
                {
                    topic = "[" + project.name + "] " + task.title,
                    lastdeliverytime = UnixTimeStampToDateTime(task.created_ts),
                    uniquesenders = "FreedCamp"
                };
                if (task.files_count > 0)
                {
                    ticket.hasattachments = true;
                }

                ticket.flagstatusid = 1;
                ticket.conversationid = task.id;
                ticket.ipused = Request.UserHostAddress;
                ticket.lastmodifiedtime = UnixTimeStampToDateTime(task.created_ts);
                ticket.statusupdatedon = DateTime.Now;
                ticket.createdonutc = DateTime.Now;
                ticket.updatedonutc = DateTime.Now;
                ticket.tickettypeid = 1;
                ticket.fromEmail = "do-not-reply@freedcamp.com";
                ticket.projectid = project.tsprojectid;
                ticket.skillid = project.skill;
                ticket.messagecount = task.comments_count + 1;
                ticket.IsArchieved = false;
                ticket.importance = false;
                ticket.LastActivityDate = DateTime.Now;
                if (task.priority_title == "High")
                {
                    ticket.importance = true;
                }

                if (userlist.Count() > 0)
                {
                    ticket.statusid = 2;
                }
                else
                {
                    ticket.statusid = 1;
                }

                db.Ticket.Add(ticket);
                db.SaveChanges();
                // ticketTeamLog added
                foreach (long team in teamlist)
                {
                    TicketTeamLogs teamlog = new TicketTeamLogs
                    {
                        ticketid = ticket.id,
                        teamid = team,
                        statusid = 2,
                        assignedbyusersid = project.userid,
                        statusupdatedbyusersid = project.userid,
                        statusupdatedon = DateTime.Now,
                        assignedon = DateTime.Now,
                        displayorder = 1
                    };

                    db.TicketTeamLogs.Add(teamlog);
                    db.SaveChanges();
                }

                SaveTask(task, ticket.id, project.id);
                CreateTicketItem(ticket.id, task.title, task.description, task.created_ts,
                    task.priority_title, project.tsprojectid, project.skill, project.userid, userlist, task.files);
                if (task.files_count > 0 || task.comments_count > 0)
                {
                    string url = "https://freedcamp.com/api/v1/tasks/" + task.id;
                    AddJob("TaskComment", url);
                }

                if (project.userid != null)
                {
                    AddNotification(ticket.id, project.userid, userlist, teamlist, 6);
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
                MailService.SendClientEmail(ConfigurationManager.AppSettings["ToNotificatoins"], "Timesheet-Error",
                    "There is an error on timesheet while freedcamp task syncing. At " + myex.controller + "/" +
                    myex.action + " <br>Here is the stack strace:<br>" + ex, null);
            }
        }

        private void SaveTask(Task task, long ticketid, long projectid)
        {
            try
            {
                // Save task in database
                FreedCampTask fctask = new FreedCampTask
                {
                    title = task.title,
                    description = task.description,
                    freedcamp_taskid = Convert.ToInt64(task.id),
                    createddate = UnixTimeStampToDateTime(task.created_ts),
                    ticketid = ticketid,
                    url = task.url,
                    freedcamp_projectid = projectid,
                    createdon = DateTime.Now,
                    statusid = task.status,
                    filescount = task.files_count,
                    commentscount = task.comments_count
                };

                db.freedCampTask.Add(fctask);
                db.SaveChanges();
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
                MailService.SendClientEmail(ConfigurationManager.AppSettings["ToNotificatoins"], "Timesheet-Error",
                    "There is an error on timesheet while freedcamp task syncing. At " + myex.controller + "/" +
                    myex.action + " <br>Here is the stack strace:<br>" + ex, null);
            }
        }

        private long CreateTicketItem(long ticketid, string title, string description, long created_ts,
            string priority_title,
            long? project_id, int? skill_id, string statusupdatedby, List<string> userlist, List<TaskFile> files)
        {
            try
            {
                Ticket ticket = db.Ticket.Find(ticketid);
                ticket.messagecount++;
                ticket.lastdeliverytime = UnixTimeStampToDateTime(created_ts);
                ticket.lastmodifiedtime = UnixTimeStampToDateTime(created_ts);
                if (files != null && files.Count() > 0)
                {
                    ticket.hasattachments = true;
                }

                ticket.updatedonutc = DateTime.Now;

                if (ticket.statusid == 2 || ticket.statusid == 0 || ticket.statusid == 8)
                {
                    ticket.statusid = ticket.statusid;
                }
                else if (ticket.statusid != 1)
                {
                    if (userlist.Count() > 0)
                    {
                        ticket.statusid = 2;
                    }
                    else
                    {
                        ticket.statusid = 1;
                    }
                }

                db.Entry(ticket).State = EntityState.Modified;
                db.SaveChanges();

                //Create ticketitem
                TicketItem ticketitem = new TicketItem
                {
                    ticketid = ticketid,
                    conversationtopic = title,
                    subject = title,
                    body = description,
                    uniquebody = description,
                    from = "do-not-reply@freedcamp.com",
                    lastmodifiedname = "FreedCamp",
                    lastmodifiedtime = DateTime.Now,
                    sensitivity = 0,
                    size = 0,
                    quotedtimeinminutes = 0,
                    torecipients = "WebSupport@Computan.net"
                };
                if (skill_id != null && skill_id != 0)
                {
                    ticketitem.skillid = skill_id;
                }

                if (project_id != null && project_id != 0)
                {
                    ticketitem.projectid = project_id;
                }

                if (userlist.Count() > 0)
                {
                    ticketitem.statusid = 2;
                }
                else
                {
                    ticketitem.statusid = 1;
                }

                ticketitem.statusupdatedbyusersid = statusupdatedby;
                ticketitem.statusupdatedon = DateTime.Now;
                ticketitem.datetimesent = DateTime.Now;
                ticketitem.createdonutc = DateTime.Now;
                ticketitem.datetimereceived = DateTime.Now;
                ticketitem.datetimecreated = UnixTimeStampToDateTime(created_ts);

                switch (priority_title)
                {
                    case "High":
                        ticketitem.importance = 1;
                        break;

                    case "Medium":
                        ticketitem.importance = 2;
                        break;

                    case "Low":
                        ticketitem.importance = 3;
                        break;

                    default:
                        ticketitem.importance = 2;
                        break;
                }

                db.TicketItem.Add(ticketitem);
                db.SaveChanges();

                // Save ticketitemlog
                if (skill_id != null || project_id != null)
                {
                    foreach (string user in userlist)
                    {
                        TicketItemLog itemlog = new TicketItemLog
                        {
                            ticketitemid = ticketitem.id,
                            assignedtousersid = user,
                            assignedbyusersid = statusupdatedby,
                            statusid = 2,
                            assignedon = DateTime.Now,
                            statusupdatedon = DateTime.Now,
                            statusupdatedbyusersid = statusupdatedby
                        };

                        db.TicketItemLog.Add(itemlog);
                        db.SaveChanges();
                    }
                }

                //Sync file as ticket attachment
                if (files != null && files.Count() > 0)
                {
                    DownloadAttachments(files, ticketitem.id);
                }

                return ticketitem.id;
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
                MailService.SendClientEmail(ConfigurationManager.AppSettings["ToNotificatoins"], "Timesheet-Error",
                    "There is an error on timesheet while freedcamp task syncing. At " + myex.controller + "/" +
                    myex.action + " <br>Here is the stack strace:<br>" + ex, null);
                return -1;
            }
        }

        private void SaveTaskComments(List<TaskComment> Comments, long ticketid, long taskid, long projectid,
            string title)
        {
            bool isnewcommentadded = false;
            try
            {
                FreedcampProject project = db.FreedcampProject.Where(x => x.id == projectid).FirstOrDefault();
                List<string> userlist = new List<string>();
                if (!string.IsNullOrEmpty(project.assignedto))
                {
                    string[] users = project.assignedto.Split(',');
                    foreach (string user in users)
                    {
                        if (!string.IsNullOrEmpty(user))
                        {
                            userlist.Add(user);
                        }
                    }
                }

                foreach (TaskComment comment in Comments)
                {
                    long commentid = Convert.ToInt64(comment.id);
                    FreedcampComment dbcomment = db.freedcampComment.Where(x => x.freedcamp_commentid == commentid).FirstOrDefault();
                    if (dbcomment == null)
                    {
                        long ticketitemid = CreateTicketItem(ticketid, title, comment.description_processed,
                            comment.created_ts, "Medium", project.tsprojectid, project.skill, project.userid, userlist,
                            comment.files);
                        if (ticketitemid != -1)
                        {
                            isnewcommentadded = true;
                            FreedcampComment fccomment = new FreedcampComment
                            {
                                freedcamp_commentid = commentid,
                                freedcamp_taskid = taskid,
                                ticketitemid = ticketitemid
                            };

                            db.freedcampComment.Add(fccomment);
                            db.SaveChanges();

                            //Add Notification for new comment added
                            AddNotification(ticketid, project.userid, userlist, null, 8);
                        }
                    }
                }

                if (isnewcommentadded)
                {
                    Ticket ticket = db.Ticket.Find(ticketid);
                    if (userlist.Count() > 0)
                    {
                        ticket.statusid = 2;
                    }
                    else
                    {
                        ticket.statusid = 1;
                    }

                    db.Entry(ticket).State = EntityState.Modified;
                    db.SaveChanges();
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
                MailService.SendClientEmail(ConfigurationManager.AppSettings["ToNotificatoins"], "Timesheet-Error",
                    "There is an error on timesheet while freedcamp task syncing. At " + myex.controller + "/" +
                    myex.action + " <br>Here is the stack strace:<br>" + ex, null);
            }
        }

        private void AddJob(string type, string reuesturl)
        {
            try
            {
                DateTime addtime = DateTime.Now;
                if (type == "Project")
                {
                    addtime = addtime.AddHours(1);
                }

                JobQueue queue = new JobQueue
                {
                    status = 1,
                    type = type,
                    url = reuesturl,
                    addtime = addtime
                };
                db.jobQueue.Add(queue);
                db.SaveChanges();
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
                MailService.SendClientEmail(ConfigurationManager.AppSettings["ToNotificatoins"], "Timesheet-Error",
                    "There is an error on timesheet while freedcamp task syncing. At " + myex.controller + "/" +
                    myex.action + " <br>Here is the stack strace:<br>" + ex, null);
            }
        }

        public void DownloadAttachments(List<TaskFile> files, long itemid)
        {
            string directory = AppDomain.CurrentDomain.BaseDirectory + "/Attachments/" + itemid;
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            foreach (TaskFile file in files)
            {
                TicketItemAttachment attachment = new TicketItemAttachment();

                WebClient wc = new WebClient();
                wc.DownloadFile(file.url, directory + "/" + file.name);

                attachment.name = file.name;
                attachment.ticketitemid = itemid;
                if (!string.IsNullOrEmpty(file.file_type))
                {
                    attachment.contenttype = file.file_type;
                }
                //file.file_type.Replace("\/", "/");
                attachment.attachmentid = file.id;
                attachment.path = "/Attachments/" + itemid + "/" + file.name;
                db.TicketItemAttachment.Add(attachment);
                db.SaveChanges();
            }
        }

        #endregion Latest API Calls Methords

        #region Helper Methords

        private static string ByteToString(byte[] buff)
        {
            string sbinary = "";
            for (int i = 0; i < buff.Length; i++)
            {
                sbinary += buff[i].ToString("x2");
            }

            return sbinary;
        }

        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return origin.AddSeconds(unixTimeStamp);
        }

        private string TaskNextUrl(string url, bool isadd)
        {
            string returnurl = "https://freedcamp.com/api/v1/tasks?status[]=";
            int offset = 0;
            string status = "0";
            if (isadd)
            {
                string parameters = Fetchurlparams(url);
                string[] seperatedpara = parameters.Split('&');
                foreach (string parameter in seperatedpara)
                {
                    string[] values = parameter.Split('=');
                    if (values[0] == "status[]")
                    {
                        status = values[1];
                    }

                    if (values[0] == "offset")
                    {
                        offset = Convert.ToInt32(values[1]);
                    }
                }

                offset += 150;
                returnurl += status + "&offset=" + offset + "&limit=150&order[created_date]=desc";
            }
            else
            {
                string parameters = Fetchurlparams(url);
                string[] seperatedpara = parameters.Split('&');
                foreach (string parameter in seperatedpara)
                {
                    string[] values = parameter.Split('=');
                    if (values[0] == "status[]")
                    {
                        status = values[1];
                        break;
                    }
                }

                returnurl += status + "&offset=0&limit=150&order[created_date]=desc";
            }

            return returnurl;
        }

        private string Fetchurlparams(string url)
        {
            int pTo = url.LastIndexOf("?") + 1;
            string result = url.Substring(0, pTo);
            result = url.Remove(0, pTo);
            return result;
        }

        private void AddNotification(long ticketid, string statusupdatebyuser, List<string> users, List<long> teams,
            int actionid)
        {
            try
            {
                Ticket ticket = (from x in db.Ticket where x.id == ticketid select x).FirstOrDefault();
                ApplicationUser activeuser = db.Users.Where(u => u.Id == statusupdatebyuser).FirstOrDefault();
                List<string> Managerslist = new List<string>();
                SendNotificationViewModel pushmodel = new SendNotificationViewModel();
                Notification notification = new Notification();
                // ADD Message Notification
                if (users != null && users.Count() > 0)
                {
                    switch (actionid)
                    {
                        case 6:
                            pushmodel.title = "New Ticket Assigned";
                            notification.entityactionid = 6;
                            notification.description = activeuser.FullName + " assigned you a new ticket. '" +
                                                       ticket.topic + "'";
                            break;

                        case 8:
                            pushmodel.title = "Assigned Ticketitem Added";
                            notification.entityactionid = 8;
                            notification.description = "ticketitems is updated. '" + ticket.topic + "'";
                            break;
                    }

                    notification.commentid = 0;
                    notification.entityid = ticket.id;

                    notification.actorid = statusupdatebyuser;

                    notification.createdon = DateTime.Now;
                    db.Notification.Add(notification);
                    db.SaveChanges();
                    // ADD Message Notification

                    // ADD Notification user
                    foreach (string user in users)
                    {
                        TeamMember team = db.TeamMember.Where(x => x.usersid == user).FirstOrDefault();
                        if (team != null)
                        {
                            TeamMember manager =
                                (from t in db.TeamMember where t.teamid == team.teamid && t.IsManager select t)
                                .FirstOrDefault();
                            if (manager != null && !string.IsNullOrEmpty(manager.usersid) &&
                                !Managerslist.Contains(manager.usersid))
                            {
                                if (manager.User.IsNotifyManagerOnTaskAssignment)
                                {
                                    Managerslist.Add(manager.usersid);
                                }
                            }
                        }

                        NotificationUsers notificationuser = new NotificationUsers
                        {
                            notification_Id = notification.id,
                            notifierid = user,
                            status = false
                        };
                        db.NotificationUsers.Add(notificationuser);
                        db.SaveChanges();
                    }
                    //notificationid(Message notification),notifierid(person to send),status
                    // ADD Notification user

                    // Create object for Push Notification
                    pushmodel.users = users;
                    pushmodel.notification = notification;
                    // Title(string),List<string>users,notification object
                    // Create object for Push Notification
                    NotificatonViewmodel.SendPushNotification(pushmodel);
                }
                else if (teams != null && teams.Count() > 0)
                {
                    foreach (long team in teams)
                    {
                        TeamMember manager = (from m in db.TeamMember where m.id == team && m.IsManager select m)
                            .FirstOrDefault();
                        if (manager != null && !string.IsNullOrEmpty(manager.usersid) &&
                            !Managerslist.Contains(manager.usersid))
                        {
                            if (manager.User.IsNotifyManagerOnTaskAssignment)
                            {
                                Managerslist.Add(manager.usersid);
                            }
                        }
                    }
                }

                if (Managerslist != null && Managerslist.Count() > 0)
                {
                    AddManagerNotification(ticket, statusupdatebyuser, Managerslist, actionid);
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
                MailService.SendClientEmail(ConfigurationManager.AppSettings["ToNotificatoins"], "Timesheet-Error",
                    "There is an error on timesheet while freedcamp task syncing. At " + myex.controller + "/" +
                    myex.action + " <br>Here is the stack strace:<br>" + ex, null);
            }
        }

        private void AddManagerNotification(Ticket ticket, string statusupdatebyuser, List<string> managers,
            int actionid)
        {
            try
            {
                ApplicationUser activeuser = db.Users.Where(u => u.Id == statusupdatebyuser).FirstOrDefault();
                List<string> Managerslist = new List<string>();
                SendNotificationViewModel pushmodel = new SendNotificationViewModel();
                Notification notification = new Notification();

                switch (actionid)
                {
                    case 6:
                        pushmodel.title = "New Ticket Assigned To Team";
                        notification.entityactionid = 6;
                        notification.description =
                            activeuser.FullName + " assigned you a new ticket. '" + ticket.topic + "'";
                        break;

                    case 8:
                        pushmodel.title = "New Ticketitem Added To Assigned Ticket.";
                        notification.entityactionid = 8;
                        notification.description = "New ticketitem added. '" + ticket.topic + "'";
                        break;
                }

                // ADD Message Notification
                notification.commentid = 0;
                notification.entityid = ticket.id;
                notification.actorid = statusupdatebyuser;
                notification.createdon = DateTime.Now;
                db.Notification.Add(notification);
                db.SaveChanges();
                // ADD Message Notification

                // ADD Notification user
                foreach (string user in managers)
                {
                    string managerid = "";
                    if (!string.IsNullOrEmpty(managerid) && !Managerslist.Contains(managerid))
                    {
                        Managerslist.Add(managerid);
                    }

                    NotificationUsers notificationuser = new NotificationUsers
                    {
                        notification_Id = notification.id,
                        notifierid = user,
                        status = false
                    };
                    db.NotificationUsers.Add(notificationuser);
                    db.SaveChanges();
                }
                //notificationid(Message notification),notifierid(person to send),status
                // ADD Notification user

                // Create object for Push Notification
                pushmodel.users = managers;
                pushmodel.notification = notification;
                // Title(string),List<string>users,notification object
                // Create object for Push Notification
                NotificatonViewmodel.SendPushNotification(pushmodel);
                // Send notification to Project Manager
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
                MailService.SendClientEmail(ConfigurationManager.AppSettings["ToNotificatoins"], "Timesheet-Error",
                    "There is an error on timesheet while freedcamp task syncing. At " + myex.controller + "/" +
                    myex.action + " <br>Here is the stack strace:<br>" + ex, null);
            }
        }

        #endregion Helper Methords
    }
}