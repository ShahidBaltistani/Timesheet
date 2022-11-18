namespace computan.timesheet.core
{
    public class UserBrowserinfo
    {
        public long id { get; set; }
        public string browser { get; set; }
        public string userId { get; set; }
        public string token { get; set; }
        public bool isActive { get; set; }
    }
}