using System.Collections.Generic;
using System.ComponentModel;

namespace computan.timesheet.core
{
    public class Rule : BaseEntity
    {
        public long id { get; set; }

        [DisplayName("Rule Name")] public string name { get; set; }

        [DisplayName("Run Order")] public int runorder { get; set; }

        [DisplayName("Status")] public bool isactive { get; set; }

        public virtual ICollection<RuleCondition> RuleConditions { get; set; }
        public virtual ICollection<RuleAction> RuleActions { get; set; }
        public virtual ICollection<RuleException> RuleExceptions { get; set; }
    }
}