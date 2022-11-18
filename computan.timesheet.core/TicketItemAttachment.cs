using System.ComponentModel.DataAnnotations.Schema;
using System.Net;

namespace computan.timesheet.core
{
    public class TicketItemAttachment
    {
        public long id { get; set; }
        public string attachmentid { get; set; }
        public long ticketitemid { get; set; }
        public string contentid { get; set; }
        public string contentlocation { get; set; }
        public string contenttype { get; set; }
        public string name { get; set; }
        public string path { get; set; }

        [NotMapped] public string EncodePath => WebUtility.UrlEncode(path);

        [ForeignKey("ticketitemid")] public TicketItem TicketItem { get; set; }
    }
}