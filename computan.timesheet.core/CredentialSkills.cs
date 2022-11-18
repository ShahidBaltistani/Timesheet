using System.ComponentModel.DataAnnotations.Schema;

namespace computan.timesheet.core
{
    public class CredentialSkills : BaseEntity
    {
        public long id { get; set; }
        public long skillid { get; set; }
        public long credentailid { get; set; }

        [ForeignKey("skillid")] public Skill Skill { get; set; }
    }
}