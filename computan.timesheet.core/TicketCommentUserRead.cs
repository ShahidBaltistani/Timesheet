using System.ComponentModel.DataAnnotations.Schema;

namespace computan.timesheet.core
{
    public class TicketCommentUserRead
    {
        public long id { get; set; }
        public long ticketcommentid { get; set; }
        public string userid { get; set; }

        [ForeignKey("ticketcommentid")] public TicketComment TicketComment { get; set; }
    }
}