using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace computan.timesheet.Models.OrphanTickets
{
    public class OrphanSearchViewModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public long? TeamId { get; set; }
        public long? StatusId { get; set; }
        public long? AgeId { get; set; }
        public List<SelectListItem> Ages { get; set; }
        public List<SelectListItem> Teams { get; set; }
        public List<SelectListItem> Status { get; set; }
    }
}