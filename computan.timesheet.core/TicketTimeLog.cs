using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace computan.timesheet.core
{
    public class TicketTimeLog : BaseEntity
    {
        public long id { get; set; }

        public long ticketitemid { get; set; }

        [DisplayName("Member Name")] public string teamuserid { get; set; }

        [DisplayName("Project Name")] public long? projectid { get; set; }

        [DisplayName("Skill Name")] public long? skillid { get; set; }

        [DisplayName("Work Date")] public DateTime workdate { get; set; }

        [DisplayName("Title")] public string title { get; set; }

        [DisplayName("Description")] public string description { get; set; }

        [DisplayName("Comments")] public string comments { get; set; }

        [DisplayName("Time Spent [Minutes]")] public int? timespentinminutes { get; set; }

        [DisplayName("Billable Time [Minutes]")]
        public int? billabletimeinminutes { get; set; }

        // Foreign Key Relationships
        [ForeignKey("ticketitemid")] public TicketItem TicketItem { get; set; }

        [ForeignKey("projectid")] public Project Project { get; set; }

        [ForeignKey("skillid")] public Skill Skill { get; set; }

        [ForeignKey("teamuserid")] public virtual ApplicationUser TeamUser { get; set; }

        [ForeignKey("userid")] public virtual ApplicationUser User { get; set; }

        [NotMapped]
        public string GetWorkDate
        {
            get
            {
                if (this != null && workdate != null)
                {
                    string workdatewithouttime = workdate.ToString("MM/dd/yyyy");

                    return workdatewithouttime;
                }

                return string.Empty;
            }
        }
    }
}