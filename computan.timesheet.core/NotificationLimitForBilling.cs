using System.ComponentModel;

namespace computan.timesheet.core
{
    public class NotificationLimitForBilling : BaseEntity
    {
        public long id { get; set; }

        [DisplayName("Notification Limit")] public double NotificationLimit { get; set; }
    }
}