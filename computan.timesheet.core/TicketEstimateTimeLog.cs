using System.ComponentModel.DataAnnotations.Schema;

namespace computan.timesheet.core
{
    public class TicketEstimateTimeLog : BaseEntity
    {
        public long id { get; set; }
        public long ticketid { get; set; }
        public int ticketitemcount { get; set; }
        public int timeestimateinminutes { get; set; }
        public string ticketusers { get; set; }

        [ForeignKey("userid")] public virtual ApplicationUser updatedBy { get; set; }
    }
}