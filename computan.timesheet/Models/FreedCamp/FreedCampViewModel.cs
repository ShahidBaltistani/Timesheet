using System;

namespace computan.timesheet.Models.FreedCamp
{
    public class FreedCampViewModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string BaseUrl { get; set; }
        public string Publickey { get; set; }
        public string Privatekey { get; set; }
        public bool IsEnabled { get; set; }
        public DateTime createdonutc { get; set; }
        public DateTime? updatedonutc { get; set; }
        public string ipused { get; set; }
        public string userid { get; set; }
    }
}