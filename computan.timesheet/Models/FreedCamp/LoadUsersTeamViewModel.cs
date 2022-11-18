using System.Collections.Generic;

namespace computan.timesheet.Models.FreedCamp
{
    public class LoadUsersTeamViewModel
    {
        public long projectid { get; set; }
        public List<TagViewModel> users { get; set; }
        public List<TagViewModel> teams { get; set; }
    }

    public class TagViewModel
    {
        public string userid { get; set; }
        public long teamid { get; set; }
        public string name { get; set; }
    }
}