using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace computan.timesheet.core
{
    public class TicketMergeLog : BaseEntity
    {
        public long id { get; set; }
        public long ticketid { get; set; }

        [MaxLength(255)]
        [DisplayName("Conversation Id")]
        [Index("conversationid", 1, IsUnique = true)]
        public string conversationid { get; set; }

        [DisplayName("Unique Senders")] public string uniquesenders { get; set; }

        [DisplayName("Topic")] public string topic { get; set; }

        [DisplayName("Last Delivery Time")] public DateTime lastdeliverytime { get; set; }

        [DisplayName("Size")] public long size { get; set; }

        [DisplayName("Message Count")] public int messagecount { get; set; }

        [DisplayName("Has Attachment?")] public bool hasattachments { get; set; }

        [DisplayName("Importance")] public bool importance { get; set; }

        [DisplayName("Flag Status")] public int flagstatusid { get; set; }

        [DisplayName("Last Modified Time")] public DateTime lastmodifiedtime { get; set; }

        [DisplayName("Status")] public int statusid { get; set; }

        [DisplayName("IsArchieved")] public bool IsArchieved { get; set; }

        [DisplayName("Ticket Priorty")] public int ticketpriortyid { get; set; }

        [DisplayName("Ticket Type")] public long? tickettypeid { get; set; }

        [MaxLength(128)]
        [DisplayName("Status Updated By")]
        public string statusupdatedbyusersid { get; set; }

        [DisplayName("Status Updated On")] public DateTime statusupdatedon { get; set; }

        public string fromEmail { get; set; }
        public long? projectid { get; set; }
        public long? skillid { get; set; }

        public DateTime? startdate { get; set; }
        public DateTime? enddate { get; set; }

        public string mergebyuserid { get; set; }
        public long mergedinticketid { get; set; }
    }
}