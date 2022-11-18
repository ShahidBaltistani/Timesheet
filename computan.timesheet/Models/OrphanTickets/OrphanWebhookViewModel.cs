using System.Collections.Generic;

namespace computan.timesheet.Models.OrphanTickets
{
    public class Attachment
    {
        public string title { get; set; }
        public string title_link { get; set; }
        public string text { get; set; }
        public string text_link { get; set; }
        public string author_link { get; set; }
        public string author_icon { get; set; }
        public string author_name { get; set; }
        public string color { get; set; }
    }

    public class OrphanWebhookViewModel
    {
        public string text { get; set; }
        public string channel { get; set; }
        public List<Attachment> attachments { get; set; }
    }
}