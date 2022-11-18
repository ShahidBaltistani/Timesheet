using computan.timesheet.core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace computan.timesheet.Models
{
    public class ProDashboardViewModel
    {
        public TicketViewModel ticketViewModel { get; set; }

        //public List<Ticket> tickets { get; set; }
        //public List<Team> teams { get; set; }
        public List<Sidebarstatus> sidebarstatus { get; set; }

        public IQueryable<ProjectMembers> projectMembers { get; set; }
        public List<Credentials> credentials { get; set; }
        public Project project { get; set; }
        public List<ProjectNotes> notes { get; set; }

        public List<ProjectFiles> files { get; set; }

        //public List<TicketUserFlagged> flaggeditems { get; set; }
        public List<ProjectTimelog> ProjectTimelog { get; set; }

        //public bool IsFlagged(long? ticketid)
        //{
        //    bool flag = false;
        //    if (flaggeditems == null) return flag;

        //    if (ticketid == null) return flag;

        //    foreach (var item in flaggeditems)
        //    {
        //        if (item.ticketid == ticketid)
        //        {
        //            flag = true;
        //            break;
        //        }
        //    }

        //    return flag;
        //}
    }

    public class ProjectMembers
    {
        public string username { get; set; }
        public bool active { get; set; }
    }

    public class ProjectTimelog
    {
        public long id { get; set; }
        public long ticketitemid { get; set; }
        public string tickettitle { get; set; }
        public string username { get; set; }
        public DateTime workdate { get; set; }
        public string description { get; set; }
        public int? spent { get; set; }
        public int? billable { get; set; }

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