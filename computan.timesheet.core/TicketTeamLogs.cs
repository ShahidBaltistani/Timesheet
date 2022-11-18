using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace computan.timesheet.core
{
    public class TicketTeamLogs
    {
        public long id { get; set; }

        public long ticketid { get; set; }

        public long teamid { get; set; }

        [MaxLength(128)]
        [DisplayName("Assigned By")]
        public string assignedbyusersid { get; set; }

        [DisplayName("Assigned On")] public DateTime assignedon { get; set; }

        [DisplayName("Display Order")] public long? displayorder { get; set; }

        [DisplayName("Status")] public int statusid { get; set; }

        [MaxLength(128)]
        [DisplayName("Status Updated By")]
        public string statusupdatedbyusersid { get; set; }

        [DisplayName("Status Updated On")] public DateTime statusupdatedon { get; set; }

        [ForeignKey("teamid")] public virtual Team team { get; set; }

        [ForeignKey("ticketid")] public Ticket Ticket { get; set; }

        //[ForeignKey("assignedtousersid")]
        //public virtual ApplicationUser user { get; set; }
        //[ForeignKey("assignedbyusersid")]
        //public virtual ApplicationUser assignbyuser { get; set; }

        //[ForeignKey("ticketitemid")]
        //public TicketItem TicketItem { get; set; }

        //[ForeignKey("statusid")]
        //public virtual TicketStatus TicketStatus { get; set; }
    }
}