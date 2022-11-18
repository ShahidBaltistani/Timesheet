using computan.timesheet.core;
using System.Collections.Generic;

namespace computan.timesheet.Models
{
    public class TicketStatusAction
    {
        public ConversationStatus CloseTicketStatus { get; set; }
        public List<ConversationStatus> OtherTicketStatusCollection { get; set; }
    }
}