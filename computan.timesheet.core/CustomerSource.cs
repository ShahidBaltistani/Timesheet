namespace computan.timesheet.core
{
    public class CustomerSource : BaseEntity
    {
        public long id { get; set; }
        public string name { get; set; }
        public bool isactive { get; set; }
    }
}