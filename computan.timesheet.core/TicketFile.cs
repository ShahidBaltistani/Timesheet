using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace computan.timesheet.core
{
    public class TicketFile : BaseEntity
    {
        public long id { get; set; }
        public long ticketid { get; set; }

        [MaxLength(255)] public string filename { get; set; }

        public long? filetypeid { get; set; }
        public string filepath { get; set; }
        public long? ticketcommentid { get; set; }

        [ForeignKey("ticketid")] public Ticket Ticket { get; set; }

        [ForeignKey("filetypeid")] public FileType FileType { get; set; }

        [ForeignKey("ticketcommentid")] public TicketComment TicketComment { get; set; }
    }
}