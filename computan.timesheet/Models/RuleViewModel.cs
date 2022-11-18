using computan.timesheet.core;
using System.Collections.Generic;

namespace computan.timesheet.Models
{
    public class RuleViewModel
    {
        public Rule Rule { get; set; }
        public List<RuleCondition> RuleConditions { get; set; }
        public List<RuleAction> RuleActions { get; set; }
        public List<RuleException> RuleExceptions { get; set; }
    }
}