using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace computan.timesheet.Models
{
    public class AdminTiaskbyUserViewModel
    {
        public List<AllTaskViewModel> tasks { get; set; }
        public List<AllTaskViewModel> todaytasks { get; set; }
        public TaskSearchFilterViewModel SearchTask { get; set; }
    }

    public class TaskSearchFilterViewModel
    {
        [Required] public DateTime StartDate { get; set; }

        [Required] public DateTime EndDate { get; set; }

        public string teamuserid { get; set; }

        public SelectList UsersCollection { get; set; }
    }
}