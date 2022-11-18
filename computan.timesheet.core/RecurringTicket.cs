namespace computan.timesheet.core
{
    public class RecurringTicket : BaseEntity
    {
        public long id { get; set; }
        public string Recurringtitle { get; set; }
        public string usersid { get; set; }
        public string teamsid { get; set; }
        public long projectid { get; set; }
        public long clientid { get; set; }
        public long recurringtype { get; set; }
    }
}