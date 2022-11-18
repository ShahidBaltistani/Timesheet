using computan.timesheet.core;
using System.Collections.Generic;

namespace computan.timesheet.Models
{
    public class ClientProject
    {
        public long projectid { get; set; }
        public string name { get; set; }
        public int taskcount { get; set; }
        public bool isActive { get; set; }
        public List<Ticket> tickets { get; set; }
    }
}