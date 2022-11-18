using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace computan.timesheet.core
{
    public class Skill : BaseEntity
    {
        public long id { get; set; }

        [Required(ErrorMessage = "Skill Name Is Required.")]
        [MaxLength(255)]
        [DisplayName("Skill Name")]
        public string name { get; set; }

        [DisplayName("Active?")] public bool isactive { get; set; }
    }
}