using System;
using System.ComponentModel;
using System.Web.Mvc;

namespace computan.timesheet.core

{
    public class SentItemLog : BaseEntity
    {
        public long id { get; set; }
        public long ticketId { get; set; }
        public string ticket_title { get; set; }

        [DisplayName("To")] public string To { get; set; }

        [DisplayName("Cc")] public string Cc { get; set; }

        [DisplayName("Bcc")] public string Bcc { get; set; }

        [DisplayName("Subject")] public string subject { get; set; }

        [AllowHtml][DisplayName("Body")] public string body { get; set; }

        public string attachments { get; set; }
        public bool IsSent { get; set; }

        public DateTime Sentdate { get; set; }
    }
}