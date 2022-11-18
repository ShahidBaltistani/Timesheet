using System;

namespace computan.timesheet.core
{
    public class TicketReplay
    {
        public long id { get; set; }

        public string UserID { get; set; }

        public long TicketID { get; set; }

        public string Type { get; set; }

        public string To { get; set; }

        public string CC { get; set; }

        public string BCC { get; set; }

        public string Body { get; set; }

        public string Attatchment { get; set; }

        public DateTime createdon { get; set; }
    }
}