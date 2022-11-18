using System.ComponentModel.DataAnnotations.Schema;

namespace computan.timesheet.core
{
    public class BillingNotification : BaseEntity
    {
        public long id { get; set; }
        public long clientid { get; set; }
        public long notificationtypeid { get; set; }
        public string billingmonth { get; set; }
        public string billingweek { get; set; }
        public string billingyear { get; set; }
        public double? maxbillablehours { get; set; }
        public double? hoursconsumed { get; set; }
        public string body { get; set; }
        public string torecipients { get; set; }
        public bool issent { get; set; }
        public bool issentagain { get; set; }

        [ForeignKey("clientid")] public Client Client { get; set; }

        [ForeignKey("notificationtypeid")] public BillingNotificationType BillingNotificationType { get; set; }
    }
}