using computan.timesheet.Contexts;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Linq;


namespace computan.timesheet.Helpers
{
    public static class CommonFunctions
    {
        public static ApplicationDbContext db = new ApplicationDbContext();
        public static double RoundTwoDecimalPlaces(double value)
        {
            return Math.Round(value, 2);
        }
        public static List<SelectListItem> ShiftTimingsPKT()
        {
            List<SelectListItem> shiftTimes = new List<SelectListItem>();

            shiftTimes.Add(new SelectListItem
            {
                Text = "Select ShiftTime",
                Value = "0",
            });
            shiftTimes.Add(new SelectListItem
            {
                Text = "12:00 PM - 09:00 PM",
                Value = "1",
            });
            shiftTimes.Add(new SelectListItem
            {
                Text = "03:00 PM - 12:00 AM",
                Value = "2",
            });
            shiftTimes.Add(new SelectListItem
            {
                Text = "06:00 PM - 03:00 AM",
                Value = "3",
            });
            return shiftTimes;
        }
        
        public static List<SelectListItem> ShiftTimingsEST()
        {
            List<SelectListItem> shiftTimes = new List<SelectListItem>();

            shiftTimes.Add(new SelectListItem
            {
                Text = "Select ShiftTime",
                Value = "0",
            });
            shiftTimes.Add(new SelectListItem
            {
                Text = "03:00 PM - 12:00 PM",
                Value = "1",
            });
            shiftTimes.Add(new SelectListItem
            {
                Text = "06:00 AM - 03:00 PM",
                Value = "2",
            });
            shiftTimes.Add(new SelectListItem
            {
                Text = "09:00 AM - 06:00 PM",
                Value = "3",
            });
            return shiftTimes;
        }
        public static List<SelectListItem> TeamLeadList()
        {
            return (from u in db.Users
            join t in db.TeamMember
            on u.Id equals t.usersid
            where t.IsTeamLead == true &&
            t.IsActive == true
            select new SelectListItem
            {
                Value = u.Id.ToString(),
                Text = u.FirstName + " " + u.LastName,
               }).ToList();
        } 
        
        public static List<SelectListItem> ProjectManagerList()
        {
            return (from u in db.Users
                    join t in db.TeamMember
                    on u.Id equals t.usersid
                    where t.IsManager == true &&
                    t.IsActive == true
                    select new SelectListItem
                    {
                        Value = u.Id.ToString(),
                        Text = u.FirstName + " " + u.LastName,
                    }).ToList();
        }
     
    }
}