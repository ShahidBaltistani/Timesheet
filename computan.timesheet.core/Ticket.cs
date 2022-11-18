using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace computan.timesheet.core
{
    public class Ticket : BaseEntity
    {
        public long id { get; set; }

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

        [DisplayName("Ticket Priorty")] public int ticketpriortyid { get; set; }

        [DisplayName("Ticket Type")] public long? tickettypeid { get; set; }

        [MaxLength(128)]
        [DisplayName("Status Updated By")]
        public string statusupdatedbyusersid { get; set; }

        [DisplayName("Status Updated On")] public DateTime statusupdatedon { get; set; }

        // Foreign Relationships.
        [ForeignKey("flagstatusid")] public FlagStatus FlagStatus { get; set; }

        [ForeignKey("statusid")] public virtual ConversationStatus ConversationStatus { get; set; }

        [ForeignKey("tickettypeid")] public virtual TicketType TicketType { get; set; }

        [ForeignKey("statusupdatedbyusersid")] public virtual ApplicationUser StatusUpdatedByUser { get; set; }

        public ICollection<TicketItem> TicketItems { get; set; }

        public virtual ICollection<TicketTeamLogs> TicketTeamLogs { get; set; }

        [NotMapped]
        public string LastModifiedToShortDateTime
        {
            get
            {
                if (lastmodifiedtime.Year != DateTime.Now.Year)
                {
                    return string.Format("{0}-{1}-{2} {3}:{4}", lastmodifiedtime.Month.ToString().PadLeft(2, '0'),
                        lastmodifiedtime.Day.ToString().PadLeft(2, '0'),
                        lastmodifiedtime.Year.ToString().Substring(2, 2),
                        lastmodifiedtime.Hour.ToString().PadLeft(2, '0'),
                        lastmodifiedtime.Minute.ToString().PadLeft(2, '0'));
                }

                return string.Format("{0}-{1} {2}:{3}", lastmodifiedtime.Month.ToString().PadLeft(2, '0'),
                    lastmodifiedtime.Day.ToString().PadLeft(2, '0'), lastmodifiedtime.Hour.ToString().PadLeft(2, '0'),
                    lastmodifiedtime.Minute.ToString().PadLeft(2, '0'));
            }
        }

        [NotMapped]
        public string LastModifiedToDateTimeWithMonthName =>
            string.Format("{0}", lastmodifiedtime.ToString("dd-MMM-yyyy"));

        [NotMapped]
        public string TicketTeamName
        {
            get
            {
                string teamName = string.Empty;

                if (TicketTeamLogs != null)
                {
                    foreach (TicketTeamLogs ticketTeam in TicketTeamLogs)
                    {
                        if (ticketTeam.team != null)
                        {
                            teamName += ticketTeam.team.name + " | ";
                        }
                    }

                    return teamName.Remove(teamName.Length - 3);
                }

                return teamName;
            }
        }

        [NotMapped]
        public List<TicketItemLog> TicketAssignedToCollection
        {
            get
            {
                List<TicketItemLog> ticketAssignedList = null;
                if (TicketItems != null)
                {
                    foreach (TicketItem item in TicketItems)
                    {
                        if (item.TicketItemLog != null)
                        {
                            foreach (TicketItemLog log in item.TicketItemLog)
                            {
                                if (ticketAssignedList == null)
                                {
                                    ticketAssignedList = new List<TicketItemLog>();
                                }

                                ticketAssignedList.Add(log);
                            }
                        }
                    }
                }

                return ticketAssignedList;
            }
        }

        public string fromEmail { get; set; }
        public long? projectid { get; set; }
        public long? skillid { get; set; }

        public DateTime? startdate { get; set; }
        public DateTime? enddate { get; set; }
        public DateTime? LastActivityDate { get; set; }

        public bool IsArchieved { get; set; }
        public virtual ICollection<TicketComment> TicketComment { get; set; }
    }
}