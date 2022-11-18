using System;

namespace computan.timesheet.Models
{
    public class ProjectsViewModels
    {
        public long id { get; set; }
        public string name { get; set; }
        public string clientname { get; set; }
        public string projectmanager { get; set; }
        public string description { get; set; }
        public DateTime? startdate { get; set; }
        public DateTime? completiondate { get; set; }
        public bool isactive { get; set; }
    }
}