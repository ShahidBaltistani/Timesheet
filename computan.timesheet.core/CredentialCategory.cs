using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace computan.timesheet.core
{
    public class CredentialCategory : BaseEntity
    {
        public long id { get; set; }

        [DisplayName("Name")]
        [Required(ErrorMessage = "Credential category name is required.")]
        public string name { get; set; }

        [DisplayName("Status")] public bool isactive { get; set; }
    }
}