using System;

namespace computan.timesheet.Models
{
    public class ContactsViewModels
    {
        public long id { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public bool isactive { get; set; }
        public DateTime createdonutc { get; set; }
    }
}