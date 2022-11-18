using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace computan.timesheet.core
{
    public class Billing : BaseEntity
    {
        public long id { get; set; }
        public long clientid { get; set; }
        public int billabletime { get; set; }
        public DateTime workdate { get; set; }

        [ForeignKey("clientid")] public Client client { get; set; }
    }
}