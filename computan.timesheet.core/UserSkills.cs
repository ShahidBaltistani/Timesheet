using System.ComponentModel.DataAnnotations.Schema;

namespace computan.timesheet.core
{
    public class UserSkills
    {
        public long id { get; set; }
        public long skillid { get; set; }
        public string userid { get; set; }

        [ForeignKey("skillid")] public Skill Skill { get; set; }
    }
}