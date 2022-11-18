using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace computan.timesheet.core
{
    public class Team : BaseEntity
    {
        public long id { get; set; }

        [Required(ErrorMessage = "Team Name Is Required.")]
        [MaxLength(255)]
        [DisplayName("Name")]
        public string name { get; set; }

        [DisplayName("Project Manager")] public string Manager { get; set; }

        [DisplayName("Customer Success Manager")]
        public string CSM { get; set; }

        [MaxLength(3)][DisplayName("Code")] public string code { get; set; }

        [DisplayName("Active?")] public bool isactive { get; set; }

        [DisplayName("Display Order")] public int? displayorder { get; set; }

        public string RocketUrl { get; set; }
        public ICollection<TeamMember> TeamMember { get; set; }

        [ForeignKey("Manager")] public virtual ApplicationUser Managers { get; set; }

        [ForeignKey("CSM")] public virtual ApplicationUser CSMS { get; set; }
    }
}