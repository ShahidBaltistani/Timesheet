using computan.timesheet.core;
using System.Collections.Generic;

namespace computan.timesheet.Models
{
    public class SideMenuViewModel
    {
        public List<TicketStatus> TicketStatus { get; set; }
        public List<TicketStatus> MyTicketStatus { get; set; }
    }
}