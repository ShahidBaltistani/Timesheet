namespace computan.timesheet.core
{
    public class FreedcampProject : BaseEntity
    {
        public long id { get; set; }
        public long fcprojectid { get; set; }
        public long? tsprojectid { get; set; }
        public string name { get; set; }
        public int? skill { get; set; }
        public string team { get; set; }
        public string assignedto { get; set; }
        public bool isactive { get; set; }
    }
}