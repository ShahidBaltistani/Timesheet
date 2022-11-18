using computan.timesheet.core;
using System.Collections.Generic;

namespace computan.timesheet.Models
{
    public class ManualTicketViewModelForList
    {
        public ICollection<Ticket> ticket { get; set; }
        public ICollection<TicketItem> ticketitem { get; set; }
    }
}