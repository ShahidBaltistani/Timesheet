using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace computan.timesheet.core
{
    public class ClientBillingCycle : BaseEntity
    {
        public long Id { get; set; }

        [DisplayName("Client Name")] public long clientid { get; set; }

        [DisplayName("Billing Cycle Type")] public long billingcyletypeid { get; set; }

        public int? date { get; set; }
        public string day { get; set; }

        [ForeignKey("clientid")] public Client Client { get; set; }

        [ForeignKey("billingcyletypeid")] public virtual BillingCycleType BillingcyleType { get; set; }
    }
}