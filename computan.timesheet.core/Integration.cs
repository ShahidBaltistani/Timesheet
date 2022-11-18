namespace computan.timesheet.core
{
    public class Integration : BaseEntity
    {
        public long id { get; set; }
        public string name { get; set; }
        public string appsettings { get; set; }
        public bool isenabled { get; set; }
    }
}