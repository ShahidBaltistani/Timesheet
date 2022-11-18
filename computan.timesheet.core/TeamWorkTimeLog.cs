namespace computan.timesheet.core
{
    public class TeamWorkTimeLog : BaseEntity
    {
        public long id { get; set; }
        public long ticketid { get; set; }
        public long teamworktaskid { get; set; }
        public string timeaddedinminuts { get; set; }
        public bool isaddedsuccessfully { get; set; }
    }
}