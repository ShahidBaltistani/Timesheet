using System.Collections.Generic;

namespace computan.timesheet.Models.FreedCamp
{
    public class FreedCampProjectViewModel
    {
        public long id { get; set; }
        public long fcprojectid { get; set; }
        public long tsporjecid { get; set; }
        public string Name { get; set; }
        public bool isActive { get; set; }
        public long skill { get; set; }
        public List<long> teams { get; set; }
    }
}