using computan.timesheet.core;
using System.Collections.Generic;

namespace computan.timesheet.Models
{
    public class DashboardViewModel
    {
        public List<Ticket> LatestTickets { get; set; }

        public List<TicketItem> MyLatestTasks { get; set; }

        public TicketTimeViewModel TicketTimeViewModel { get; set; }
    }
}