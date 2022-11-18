using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace computan.timesheet.core
{
    public class TicketLogs
    {
        public long id { get; set; }

        public long ticketid { get; set; }

        public long actiontypeid { get; set; }

        [ForeignKey("actiontypeid")] public ActionType ActionType { get; set; }

        public DateTime actiondate { get; set; }

        public string actionbyuserId { get; set; }

        public string ActionDescription { get; set; }
    }
}