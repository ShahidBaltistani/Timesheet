using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace computan.timesheet.core
{
    public class FreedCampTask
    {
        public long id { get; set; }
        public long freedcamp_taskid { get; set; }
        public long freedcamp_projectid { get; set; }
        public long ticketid { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public int commentscount { get; set; }
        public int filescount { get; set; }
        public int statusid { get; set; }
        public DateTime createddate { get; set; }
        public string url { get; set; }
        public DateTime createdon { get; set; }

        [ForeignKey("freedcamp_projectid")] public FreedcampProject project { get; set; }
    }
}