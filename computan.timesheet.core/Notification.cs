using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace computan.timesheet.core
{
    public class Notification
    {
        public long id { get; set; }
        public long entityid { get; set; }
        public string description { get; set; }
        public long entityactionid { get; set; }
        public DateTime createdon { get; set; }
        public long commentid { get; set; }
        public string actorid { get; set; }

        [ForeignKey("entityactionid")] public virtual NotificationEntityAction notificationentityaction { get; set; }

        [ForeignKey("actorid")] public virtual ApplicationUser users { get; set; }
    }
}