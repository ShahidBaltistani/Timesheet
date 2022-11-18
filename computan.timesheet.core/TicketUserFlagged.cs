namespace computan.timesheet.core
{
    public class TicketUserFlagged
    {
        public long id { get; set; }
        public long ticketid { get; set; }
        public string userid { get; set; }
        public bool isactive { get; set; }
    }
}