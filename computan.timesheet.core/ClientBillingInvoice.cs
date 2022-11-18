using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace computan.timesheet.core
{
    public class ClientBillingInvoice : BaseEntity
    {
        public long Id { get; set; }

        [DisplayName("Client Name")] public long clientid { get; set; }

        [DisplayName("Billing Date")] public DateTime billingdate { get; set; }

        [DisplayName("Hours Consumed")] public double hoursconsumed { get; set; }

        [DisplayName("Is Paid?")] public bool ispaid { get; set; }

        [DisplayName("Is Approved?")] public bool isapproved { get; set; }

        [DisplayName("Billing Cycle Type")] public long billingcyletypeid { get; set; }

        [ForeignKey("clientid")] public Client Client { get; set; }

        [ForeignKey("billingcyletypeid")] public BillingCycleType BillingcyleType { get; set; }
    }
}