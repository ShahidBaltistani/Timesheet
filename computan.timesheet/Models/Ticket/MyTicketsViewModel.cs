using computan.timesheet.core;
using System.Collections.Generic;

namespace computan.timesheet.Models
{
    public class MyTicketsViewModel
    {
        public long statusidparam { get; set; }
        public long? clientidparam { get; set; }
        public List<TicketStatusViewModel> TicketUserStatusCollection { get; set; }
        public List<UserClientViewModel> TicketUserClientCollection { get; set; }
        public List<Ticket> MyTicketCollection { get; set; }

        public List<Project> ProjectDataCollection { get; set; }
        public bool WarningStatus { get; set; }
        public string WarningText { get; set; }
    }
}