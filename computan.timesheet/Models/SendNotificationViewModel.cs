using computan.timesheet.core;
using System.Collections.Generic;

namespace computan.timesheet.Models
{
    public class SendNotificationViewModel
    {
        public string title { get; set; }
        public List<string> users { get; set; }
        public Notification notification { get; set; }
    }
}