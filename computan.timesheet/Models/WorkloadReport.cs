using computan.timesheet.core;
using System.Collections.Generic;

namespace computan.timesheet.Models
{
    public class WorkloadReport
    {
        public long Inprogress { get; set; }
        public long Done { get; set; }
        public long OnHold { get; set; }
        public long QC { get; set; }
        public long Assigned { get; set; }
        public long InReview { get; set; }
        public long Trash { get; set; }
        public string username { get; set; }
        public string teamname { get; set; }
        public long teamid { get; set; }
        public List<Team> Team { get; set; }
    }

    public class FinalWorkloadRepot
    {
        public List<WorkloadReport> WorkloadReport { get; set; }
        public List<Team> Teams { get; set; }
    }
}