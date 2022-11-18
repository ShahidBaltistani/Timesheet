namespace computan.timesheet.core.OrphanTickets
{
    public class SubscribeTeam : BaseEntity
    {
        public int Id { get; set; }
        public long TeamId { get; set; }
        public string UsersId { get; set; }
    }
}