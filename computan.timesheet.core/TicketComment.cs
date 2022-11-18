using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Script.Serialization;

namespace computan.timesheet.core
{
    public class TicketComment : BaseEntity
    {
        public long id { get; set; }
        public long ticketid { get; set; }

        [DisplayName("Comment On")] public DateTime commenton { get; set; }

        [MaxLength(128)]
        [DisplayName("Comment By")]
        public string commentbyuserid { get; set; }

        [MaxLength(200)]
        [DisplayName("User Name")]
        public string commentbyusername { get; set; }

        [DisplayName("body")] public string commentbody { get; set; }

        public long? parentid { get; set; }

        [ForeignKey("ticketid")]
        [ScriptIgnore]
        public Ticket Ticket { get; set; }

        [ForeignKey("commentbyuserid")]
        [ScriptIgnore]
        public ApplicationUser CommentByUser { get; set; }

        [ForeignKey("parentid")]
        [ScriptIgnore]
        public TicketComment ParentComment { get; set; }
    }
}