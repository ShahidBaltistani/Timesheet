using System.ComponentModel.DataAnnotations.Schema;

namespace computan.timesheet.core
{
    public class NotificationUsers
    {
        public long Id { get; set; }
        public long notification_Id { get; set; }
        public string notifierid { get; set; }
        public bool status { get; set; }

        [ForeignKey("notification_Id")] public virtual Notification notification { get; set; }
    }
}