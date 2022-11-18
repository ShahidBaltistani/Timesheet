using System.ComponentModel;

namespace computan.timesheet.Models
{
    public class BillingViewModel
    {
        public long clientid { get; set; }
        public long? clienttypeid { get; set; }

        [DisplayName("Client Name")] public string ClientName { get; set; }

        public double? maxbillablehours { get; set; }
        public double BillableTime { get; set; }
        public bool isEndingPeroid { get; set; }
        public double Percentageconsumed { get; set; }
        public string Billcyletype { get; set; }
        public long? Billcyletypeid { get; set; }
    }
}