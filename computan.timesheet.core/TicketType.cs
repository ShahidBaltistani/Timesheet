using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace computan.timesheet.core
{
    public class TicketType : BaseEntity
    {
        public long id { get; set; }

        [DisplayName("Name")]
        [Required(ErrorMessage = "Ticket type name is required.")]
        public string name { get; set; }
    }
}