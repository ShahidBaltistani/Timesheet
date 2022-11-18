using computan.timesheet.core;
using System.Collections.Generic;

namespace computan.timesheet.Models
{
    public class TicketItemStatusAction
    {
        public TicketStatus CloseTicketStatus { get; set; }
        public List<TicketStatus> OtherTicketStatusCollection { get; set; }
    }
}