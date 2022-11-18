namespace computan.timesheet.Models
{
    public class UserProjectViewModel
    {
        public long projectid { get; set; }
        public string projectname { get; set; }
        public bool ispinned { get; set; } = false;
    }
}