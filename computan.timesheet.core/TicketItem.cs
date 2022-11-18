using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web;

namespace computan.timesheet.core
{
    public class TicketItem : BaseEntity
    {
        public long id { get; set; }
        public long ticketid { get; set; }

        [DisplayName("Email Message Id")] public string emailmessageid { get; set; }

        [DisplayName("Bcc")] public string bccrecipients { get; set; }

        [DisplayName("Cc")] public string ccrecipients { get; set; }

        [DisplayName("Body")] public string body { get; set; }

        [DisplayName("Conversation Id")] public string conversationid { get; set; }

        [DisplayName("Conversation Index")] public string conversationindex { get; set; }

        [DisplayName("Conversation Topic")] public string conversationtopic { get; set; }

        [DisplayName("Created On")] public DateTime datetimecreated { get; set; }

        [DisplayName("Received On")] public DateTime datetimereceived { get; set; }

        [DisplayName("Sent On")] public DateTime datetimesent { get; set; }

        [DisplayName("Display CC")] public string displaycc { get; set; }

        [DisplayName("Display To")] public string displayto { get; set; }

        [DisplayName("From")] public string from { get; set; }

        [DisplayName("Has Attachment?")] public bool hasattachments { get; set; }

        [DisplayName("Importance")] public int importance { get; set; }

        [DisplayName("In Reply To")] public string inreplyto { get; set; }

        [DisplayName("Headers")] public string internetmessageheaders { get; set; }

        [DisplayName("Message Id")] public string internetmessageid { get; set; }

        [DisplayName("Last Modified Name")] public string lastmodifiedname { get; set; }

        [DisplayName("Last Modified Time")] public DateTime lastmodifiedtime { get; set; }

        [DisplayName("Mime Content")] public string mimecontent { get; set; }

        [DisplayName("Reply To")] public string replyto { get; set; }

        [DisplayName("Sensitivity")] public int sensitivity { get; set; }

        [DisplayName("Size")] public int size { get; set; }

        [DisplayName("Subject")] public string subject { get; set; }

        [DisplayName("To")] public string torecipients { get; set; }

        [DisplayName("Unique Body")] public string uniquebody { get; set; }

        [DisplayName("Ticket Project")] public long? projectid { get; set; }

        [DisplayName("Ticket Skill")] public long? skillid { get; set; }

        [DisplayName("Quoted Time [Minutes]")] public int quotedtimeinminutes { get; set; }

        [DisplayName("Status")] public int statusid { get; set; }

        [MaxLength(128)]
        [DisplayName("Status Updated By")]
        public string statusupdatedbyusersid { get; set; }

        [DisplayName("Status Updated On")] public DateTime statusupdatedon { get; set; }

        public string shortbody
        {
            get
            {
                if (uniquebody != null)
                {
                    HtmlDocument htmlDoc = new HtmlDocument();
                    htmlDoc.LoadHtml(uniquebody);
                    string InnerText = htmlDoc.DocumentNode.InnerText;

                    if (InnerText.Length > 150)
                    {
                        InnerText = InnerText.Substring(0, 150);
                    }

                    return HttpUtility.HtmlDecode(InnerText);
                }

                return uniquebody;
            }
        }

        public string minibody
        {
            get
            {
                if (body != null)
                {
                    HtmlDocument htmlDoc = new HtmlDocument();
                    htmlDoc.LoadHtml(body);
                    string InnerText = htmlDoc.DocumentNode.InnerText;

                    if (InnerText.Length > 50)
                    {
                        InnerText = InnerText.Substring(0, 50);
                    }

                    return HttpUtility.HtmlDecode(InnerText);
                }

                return body;
            }
        }

        public string ProjectName
        {
            get
            {
                if (Project != null)
                {
                    return Project.name;
                }

                return string.Empty;
            }
        }

        public string SkillName
        {
            get
            {
                if (Skill != null)
                {
                    return Skill.name;
                }

                return string.Empty;
            }
        }

        [NotMapped]
        public string sender
        {
            get
            {
                string tempstring = "";
                if (!string.IsNullOrEmpty(lastmodifiedname))
                {
                    tempstring = lastmodifiedname + " <" + from + ">";
                }
                else
                {
                    tempstring = from;
                }

                return tempstring;
            }
        }

        [NotMapped]
        public long? tickettypeid
        {
            get
            {
                if (Ticket != null)
                {
                    return Ticket.tickettypeid;
                }

                return 1;
            }
        }

        [NotMapped]
        public string ToShortDateTimeReceived
        {
            get
            {
                if (datetimereceived.Year != DateTime.Now.Year)
                {
                    return string.Format("{0}/{1}/{2}", datetimereceived.Month, datetimereceived.Day,
                        datetimereceived.Year);
                }

                if (datetimereceived.Day == DateTime.Now.Day)
                {
                    return string.Format("{0}:{1}", datetimereceived.Hour.ToString().PadLeft(2, '0'),
                        datetimereceived.Minute.ToString().PadLeft(2, '0'));
                }

                return string.Format("{0}/{1} {2}:{3}", datetimereceived.Month, datetimereceived.Day,
                    datetimereceived.Hour.ToString().PadLeft(2, '0'),
                    datetimereceived.Minute.ToString().PadLeft(2, '0'));

                //, datetimereceived.Hour.ToString().PadLeft(2, '0'), datetimereceived.Minute.ToString().PadLeft(2, '0')
            }
        }

        public string dashboardbody
        {
            get
            {
                if (body != null)
                {
                    HtmlDocument htmlDoc = new HtmlDocument();
                    htmlDoc.LoadHtml(body);
                    string InnerText = htmlDoc.DocumentNode.InnerText;

                    if (InnerText.Length > 90)
                    {
                        InnerText = InnerText.Substring(0, 90);
                    }

                    return HttpUtility.HtmlDecode(InnerText);
                }

                return body;
            }
        }

        // Foreign Relationships.
        [ForeignKey("ticketid")] public Ticket Ticket { get; set; }

        [ForeignKey("projectid")] public Project Project { get; set; }

        [ForeignKey("skillid")] public Skill Skill { get; set; }

        [ForeignKey("statusid")] public virtual TicketStatus TicketStatus { get; set; }

        [ForeignKey("statusupdatedbyusersid")] public virtual ApplicationUser StatusUpdatedByUser { get; set; }

        public ICollection<TicketItemAttachment> TicketItemAttachment { get; set; }
        public ICollection<TicketItemLog> TicketItemLog { get; set; }
        public ICollection<TicketTimeLog> TicketTimeLog { get; set; }

        public override string ToString()
        {
            return string.Format("{0}", from);
        }
    }
}