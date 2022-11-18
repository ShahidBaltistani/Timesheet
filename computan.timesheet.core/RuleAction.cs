using System.ComponentModel.DataAnnotations.Schema;

namespace computan.timesheet.core
{
    public class RuleAction
    {
        public long id { get; set; }
        public long ruleid { get; set; }
        public long ruleactiontypeid { get; set; }
        public string ruleactionvalue { get; set; }
        public long? projectid { get; set; }
        public long? skillid { get; set; }
        public int? statusid { get; set; }

        [NotMapped] public string fullname { get; set; }

        [ForeignKey("ruleid")] public Rule Rule { get; set; }

        [ForeignKey("ruleactiontypeid")] public virtual RuleActionType RuleActionType { get; set; }
    }
}