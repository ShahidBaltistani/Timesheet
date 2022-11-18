using System;
using System.Collections.Generic;

namespace computan.timesheet.Models
{
    public class ClientDashboardViewModel
    {
        public long client_id { get; set; }
        public string name { get; set; }

        public List<ProjectViewModel> projects { get; set; }
        //public Client client { get; set; }
        //public List<ClientProject> projects { get; set; }
        //public List<Team> teams { get; set; }
    }

    public class ProjectViewModel
    {
        public long project_id { get; set; }
        public string name { get; set; }
        public int tickets_count { get; set; }
        public List<TicketsViewModel> tickets { get; set; }
        public List<StatusCount> status_count { set; get; }
    }

    public class TicketsViewModel
    {
        public long ticket_id { get; set; }
        public int statusid { get; set; }
        public string status { get; set; }
        public string sender { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string minibody { get; set; }
        public string shortbody { get; set; }
        public string uniqubody { get; set; }
        public DateTime last_updated { get; set; }
        public List<UsersViewModel> assigned_user { get; set; }

        public string color
        {
            get
            {
                if (statusid != 0)
                {
                    switch (statusid)
                    {
                        case 1:
                            return "blue";
                        case 2:
                            return "green";
                        case 4:
                            return "grey";
                        case 5:
                            return "brown";
                        case 6:
                            return "red";
                        case 7:
                            return "indigo";
                    }
                }

                return "";
            }
        }
    }

    public class UsersViewModel
    {
        public string user_name { get; set; }
        public string user_id { get; set; }
        public string short_name { get; set; }
    }

    public class StatusCount
    {
        public StatusCount(int id, int count)
        {
            statusid = id;
            this.count = count;
        }

        public int statusid { get; set; }

        public string name
        {
            get
            {
                switch (statusid)
                {
                    case 1:
                        return "New Task";
                    case 2:
                        return "In Progress";
                    case 4:
                        return "On Hold";
                    case 5:
                        return "Qualtiy Control";
                    case 6:
                        return "Assigned";
                    case 7:
                        return "In Review";
                }

                return "";
            }
        }

        public int count { get; set; }

        public string status_color
        {
            get
            {
                if (statusid != 0)
                {
                    switch (statusid)
                    {
                        case 1:
                            return "blue";
                        case 2:
                            return "green";
                        case 4:
                            return "grey";
                        case 5:
                            return "brown";
                        case 6:
                            return "danger";
                        case 7:
                            return "indigo";
                    }
                }

                return "";
            }
        }
    }
    //public enum StatusColor
    //{
    //    blue = 1, 
    //    green = 2,
    //    grey = 4, 
    //    brown = 5,
    //    red = 6,
    //    yellow = 7,
    //}
}