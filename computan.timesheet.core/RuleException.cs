using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace computan.timesheet.core
{
    public class RuleException
    {
        public long id { get; set; }
        public long ruleid { get; set; }
        public long ruleexceptiontypeid { get; set; }

        [Required] public string ruleexceptionvalue { get; set; }

        public bool isrequired { get; set; }

        [ForeignKey("ruleid")] public Rule Rule { get; set; }

        [ForeignKey("ruleexceptiontypeid")] public virtual RuleExceptionType RuleConditionType { get; set; }
    }
}