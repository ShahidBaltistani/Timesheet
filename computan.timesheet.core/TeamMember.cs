using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace computan.timesheet.core
{
    public class TeamMember : BaseEntity
    {
        public long id { get; set; }

        public long teamid { get; set; }

        [Required(ErrorMessage = "User Name Is Required.")]
        [MaxLength(128)]
        [DisplayName("Username")]
        public string usersid { get; set; }

        [DisplayName("IsActive")] public bool IsActive { get; set; }

        [DisplayName("IsManager")] public bool IsManager { get; set; }

        [DisplayName("TeamLead")] public bool IsTeamLead { get; set; }

        [DisplayName("Report To")] public string Reported { get; set; }

        [ForeignKey("teamid")] public Team Team { get; set; }

        [ForeignKey("usersid")] public virtual ApplicationUser User { get; set; }
        //[ForeignKey("Reported")]
        //public virtual ApplicationUser Manager { get; set; }
    }
}