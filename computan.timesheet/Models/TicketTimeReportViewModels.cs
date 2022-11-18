using System;

namespace computan.timesheet.Models
{
    public class TicketTimeReportViewModels
    {
        public DateTime? EstimateDate { get; set; }
        public int? EstimateTime { get; set; }
        public DateTime? WorkDate { get; set; }
        public string FullName { get; set; }
        public int? SpendTime { get; set; }
        public int? BillableTime { get; set; }
    }
}