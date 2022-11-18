using computan.timesheet.core.common;
using computan.timesheet.Helpers;
using Hangfire;
using Hangfire.Storage;
using System;
using System.Net;
using System.Web.Mvc;

namespace computan.timesheet.Controllers
{
    [CustomeAuthorizeAttribute(Role.Admin)]
    public class JobsController : Controller
    {
        [Obsolete]
        public ActionResult CronJob()
        {
            using (IStorageConnection connection = JobStorage.Current.GetConnection())
            {
                foreach (RecurringJobDto recurringJob in connection.GetRecurringJobs())
                {
                    RecurringJob.RemoveIfExists(recurringJob.Id);
                }

                if (connection.GetRecurringJobs().Count == 0)
                {
                    FreedCampApiController freedCampApi = new FreedCampApiController();
                    RecurringJob.AddOrUpdate(() => freedCampApi.DoJob(), Cron.MinuteInterval(10));
                    GraphAPIController graphAPI = new GraphAPIController();
                    RecurringJob.AddOrUpdate(() => graphAPI.Mails(), Cron.MinuteInterval(10));
                }
            }

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        [Obsolete]
        public ActionResult FreedCampJob()
        {
            FreedCampApiController freedCampApi = new FreedCampApiController();
            RecurringJob.AddOrUpdate(() => freedCampApi.DoJob(), Cron.MinuteInterval(10));
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        [Obsolete]
        public ActionResult GraphMailJob()
        {
            GraphAPIController graphAPI = new GraphAPIController();
            RecurringJob.AddOrUpdate(() => graphAPI.Mails(), Cron.MinuteInterval(10));
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }
    }
}