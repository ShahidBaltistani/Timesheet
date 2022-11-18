using System.ComponentModel;

namespace computan.timesheet.Models
{
    public class TeamMemberViewModel
    {
        public long id { get; set; }
        public string userid { get; set; }

        [DisplayName("Name")] public string name { get; set; }

        [DisplayName("Email")] public string email { get; set; }

        [DisplayName("Reported To")] public string lead { get; set; }

        public bool manager { get; set; }
        public bool teamlead { get; set; }
        public bool active { get; set; }
    }

    public class ReportableUsers
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public bool Manager { get; set; }
    }
}