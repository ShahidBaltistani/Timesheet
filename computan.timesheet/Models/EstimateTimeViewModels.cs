using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace computan.timesheet.Models
{
    public class EstimateTimeViewModels
    {
        public TaskSearchViewModels searchViewModels { get; set; }
        public List<TaskTimeViewModels> taskTimeView { get; set; }
    }

    public class TaskTimeViewModels
    {
        public long TicketId { get; set; }

        [DisplayFormat(NullDisplayText = "No Title")]
        public string TicketTitle { get; set; }

        public int EstimatedTime { get; set; }
        public int SpentTime { get; set; }
        public int BillTime { get; set; }
        public int TimeDifference => EstimatedTime - SpentTime;
    }

    public class TaskSearchViewModels
    {
        [Required] public DateTime StartDate { get; set; }

        [Required] public DateTime EndDate { get; set; }

        public string userid { get; set; }
        public List<SelectListItem> UsersCollection { get; set; }
    }
}