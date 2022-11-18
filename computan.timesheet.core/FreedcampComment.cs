using System.ComponentModel.DataAnnotations.Schema;

namespace computan.timesheet.core
{
    public class FreedcampComment
    {
        public long id { get; set; }
        public long freedcamp_commentid { get; set; }
        public long freedcamp_taskid { get; set; }
        public long ticketitemid { get; set; }

        [ForeignKey("freedcamp_taskid")] public FreedCampTask task { get; set; }
    }
}