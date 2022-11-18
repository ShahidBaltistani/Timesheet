using System.ComponentModel.DataAnnotations.Schema;

namespace computan.timesheet.core
{
    public class ProjectNotes : BaseEntity
    {
        public long id { get; set; }
        public string comments { get; set; }
        public long projectid { get; set; }
        public string addedbyuserid { get; set; }

        [ForeignKey("addedbyuserid")] public virtual ApplicationUser addedbyuser { get; set; }

        [ForeignKey("projectid")] public virtual Project Project { get; set; }
    }
}