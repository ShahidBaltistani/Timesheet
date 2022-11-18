using System;

namespace computan.timesheet.Models
{
    public class NotificationMessageViewModel
    {
        public long id { get; set; }
        public DateTime createdDate { get; set; }
        public string Message { get; set; }
        public long entityId { get; set; }
        public bool status { get; set; }
        public long EntityActionId { get; set; }

        public long commentid { get; set; }
    }
}