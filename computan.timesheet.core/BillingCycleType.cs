using System.ComponentModel;

namespace computan.timesheet.core
{
    public class BillingCycleType : BaseEntity
    {
        public long Id { get; set; }

        [DisplayName("Name")] public string name { get; set; }

        [DisplayName("Status")] public bool isactive { get; set; }
    }
}