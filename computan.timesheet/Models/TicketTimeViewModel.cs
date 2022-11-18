using computan.timesheet.core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace computan.timesheet.Models
{
    public class TicketTimeViewModel
    {
        public TicketTimeSearchViewModel SearchTicketTime { get; set; }
        public List<TicketTimeLog> SearchResults { get; set; }
        public List<TeamTimeLogSearchResult> SearchResult{ get; set; }
        public List<TicketTimeLog> TimeAddedResults { get; set; }
    }

    public class TicketTimeSearchViewModel
    {
        [Required] public DateTime StartDate { get; set; }

        [Required] public DateTime EndDate { get; set; }

        public long? projectid { get; set; }
        public long? clientid { get; set; }
        public long? TeamId { get; set; }
        public string teamuserid { get; set; }
        public List<SelectListItem> ProjectCollection { get; set; }
        public List<SelectListItem> UsersCollection { get; set; }
        public List<SelectListItem> ClientCollection { get; set; }
        public List<SelectListItem> TeamList{ get; set; }
    }
    
    public class TeamTimeLogSearchResult
    {
 

        public long id { get; set; }
        public string Client { get; set; }
        public string ClientType { get; set; }
        public string ProjectName { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string maxbillablehours { get; set; }

        public DateTime workdate { get; set; }
        public int? timespentinminutes { get; set; }
        public int? billabletimeinminutes { get; set; }
        public string UserName { get; set; }
        public string SkillName { get; set; }
        public string TeamName { get; set; }
        public string GetWorkDate
        {
            get
            {
                if (this != null && workdate != null)
                {
                    string workdatewithouttime = workdate.ToString("MM/dd/yyyy");

                    return workdatewithouttime;
                }

                return string.Empty;
            }
        }

    }
}