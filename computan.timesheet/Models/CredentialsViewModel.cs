namespace computan.timesheet.Models
{
    public class CredentialsViewModel
    {
        public long id { get; set; }
        public string title { get; set; }
        public string link { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string port { get; set; }
        public string comments { get; set; }
        public string host { get; set; }
        public string networkdomain { get; set; }
        public string ctypename { get; set; }
        public long ctypeid { get; set; }
        public string clevelname { get; set; }
        public long clevelid { get; set; }
        public string ccategoryname { get; set; }
        public long ccategoryid { get; set; }
        public string projectname { get; set; }
        public string filename { get; set; }
        public string passwordhash { get; set; }
        public string passwordsalt { get; set; }
    }
}