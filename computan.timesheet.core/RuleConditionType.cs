using System.ComponentModel;

namespace computan.timesheet.core
{
    public class RuleConditionType : BaseEntity
    {
        public long id { get; set; }

        [DisplayName("Condition")] public string name { get; set; }

        public bool isactive { get; set; }
    }
}