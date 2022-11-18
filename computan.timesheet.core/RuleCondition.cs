using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace computan.timesheet.core
{
    public class RuleCondition
    {
        public long id { get; set; }
        public long ruleid { get; set; }
        public long ruleconditiontypeid { get; set; }

        [DisplayName("Condition Value")]
        [Required]
        public string ruleconditionvalue { get; set; }

        public bool isrequired { get; set; }

        [ForeignKey("ruleid")] public Rule Rule { get; set; }

        [ForeignKey("ruleconditiontypeid")] public virtual RuleConditionType RuleConditionType { get; set; }
    }
}