using computan.timesheet.core;
using System.Collections.Generic;
using System.Web.Mvc;

namespace computan.timesheet.Models
{
    public class TicketItemViewModel
    {
        public Ticket Ticket { get; set; }
        public TicketItem TicketItem { get; set; }
        public List<SelectListItem> Projects { get; set; }
        public List<Skill> Skills { get; set; }
        public List<TicketItemLog> TicketUsers { get; set; }
        public List<TicketTeamLogs> TicketTeam { get; set; }
        public List<TicketComment> TicketComment { get; set; }
        public List<Team> Teams { get; set; }
        public Project TicketProject { get; set; }
        public List<TicketItemAttachment> ticketitemattachment { get; set; }
        public bool IsWarning { get; set; }
        public bool IsRestrict { get; set; }
        public string WarningText { get; set; }
        public List<Credentials> Credentials { get; set; }
        public List<TicketTimeLog> Timelog { get; set; }
        public string FirstTicketItemID { get; set; }
    }
}