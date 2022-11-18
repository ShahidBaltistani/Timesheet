namespace computan.timesheet.Models
{
    public class AddTImeBucketViewModel
    {
        public long id { get; set; }
        public string description { get; set; }
        public int timespent { get; set; }
        public int? billtime { get; set; }
    }
}