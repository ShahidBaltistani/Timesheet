using System.ComponentModel.DataAnnotations.Schema;

namespace computan.timesheet.core
{
    public class TimeEntryLogs : BaseEntity
    {
        public long id { get; set; }
        public string unrestricteduserid { get; set; }

        [ForeignKey("unrestricteduserid")] public virtual ApplicationUser unrestricteduser { get; set; }

        [ForeignKey("userid")] public virtual ApplicationUser unRestrictedBy { get; set; }
    }
}