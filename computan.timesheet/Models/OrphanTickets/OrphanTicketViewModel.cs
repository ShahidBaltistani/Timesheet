using System;
using System.Collections.Generic;

namespace computan.timesheet.Models.OrphanTickets
{
    public class OrphanViewModel
    {
        public List<OrphanTicketViewModel> OrphanTickets { get; set; }
        public OrphanSearchViewModel OrphanSearch { get; set; }
    }

    public class OrphanTicketViewModel
    {
        public long id { get; set; }
        public string topic { get; set; }
        public DateTime LastActivityDate { get; set; }
        public string StatusName { get; set; }
        public int StatusId { get; set; }
        public string TeamName { get; set; }
        public long TeamId { get; set; }
        public int Age { get; set; }
        public string RocketUrl { get; set; }
    }
}