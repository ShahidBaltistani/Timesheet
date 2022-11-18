using System.ComponentModel.DataAnnotations.Schema;

namespace computan.timesheet.core
{
    public class NotificationEntityAction
    {
        public long id { get; set; }
        public string name { get; set; }
        public long entityid { get; set; }
        public bool isActive { get; set; }

        [ForeignKey("entityid")] public virtual NotificationEntity notificationentity { get; set; }
    }
}