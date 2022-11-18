using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace computan.timesheet.core
{
    public class ClientContact : BaseEntity
    {
        public long id { get; set; }

        public long clientid { get; set; }

        [MaxLength(255)][DisplayName("Name")] public string name { get; set; }

        [MaxLength(255)]
        [DisplayName("Email")]
        public string email { get; set; }

        [DisplayName("Is notify?")] public bool isnotify { get; set; }

        [MaxLength(100)]
        [DisplayName("Title")]
        public string title { get; set; }

        [DisplayName("Active?")] public bool isactive { get; set; }

        [ForeignKey("clientid")] public Client Client { get; set; }
    }
}