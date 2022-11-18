namespace computan.timesheet.core.OrphanTickets
{
    public class SuppressTicket : BaseEntity
    {
        public int Id { get; set; }
        public long TicketId { get; set; }
        public string UsersId { get; set; }
    }
}