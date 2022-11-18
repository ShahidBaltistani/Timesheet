using computan.timesheet.core;
using System;
using System.Collections.Generic;

namespace computan.timesheet.Models
{
    public class TeamDashboardViewModel
    {
        public string teamName { get; set; }
        public List<Ticket> PendingAssignmentTickets { get; set; }
        public List<Ticket> DashboardTickets { get; set; }
        public List<TeamDashboardDataViewModel> tickets { get; set; }
        public List<TeamDashboardDataViewModel> NewTickets { get; set; }
        public List<SingleUser> users { get; set; }
        public List<TicketUserFlagged> flaggeditems { get; set; }

        public bool IsFlagged(long? ticketid)
        {
            bool flag = false;
            if (flaggeditems == null)
            {
                return flag;
            }

            if (ticketid == null)
            {
                return flag;
            }

            foreach (TicketUserFlagged item in flaggeditems)
            {
                if (item.ticketid == ticketid)
                {
                    flag = true;
                    break;
                }
            }

            return flag;
        }
    }

    public class SingleUser
    {
        public string FullName { get; set; }
        public string ID { get; set; }
    }

    public class TeamDashboardDataViewModel
    {
        public long id { get; set; }
        public string topic { get; set; }
        public long? clientid { get; set; }

        public long? projectid { get; set; }

        //public string projectname { get; set; }
        public long? skillid { get; set; }

        //public string skillname { get; set; }
        public string assignedbyusersid { get; set; }
        public string assignedbyuserName { get; set; }
        public string assignedtousersid { get; set; }
        public string assignedtouserName { get; set; }
        public int statusid { get; set; }
        public DateTime createdonutc { get; set; }
        public DateTime assignedon { get; set; }
        public DateTime? startdate { get; set; }
        public DateTime? enddate { get; set; }
    }
}