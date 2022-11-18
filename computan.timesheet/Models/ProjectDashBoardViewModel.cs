using computan.timesheet.core;
using System.Collections.Generic;

namespace computan.timesheet.Models
{
    public class ProjectDashBoardViewModel
    {
        public Project project { get; set; }

        public List<AllTaskViewModel> tasks { get; set; }
        public List<Credentials> credentials { get; set; }
        public List<string> usernames { get; set; }
        public List<Sidebarstatus> sidebarstatus { get; set; }
    }

    public class Sidebarstatus
    {
        public int id { get; set; }
        public string name { get; set; }
        public int count { get; set; }
    }
}