namespace computan.timesheet.core.integrations
{
    public class FreedcampSetting : IAppSetting
    {
        public string publickey { get; set; }
        public string privatekey { get; set; }
        public string baseurl { get; set; }
    }
}