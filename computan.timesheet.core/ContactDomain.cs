using System.Collections.Generic;

namespace computan.timesheet.core
{
    public class ContactCompany
    {
        public long id { get; set; }
        public string name { get; set; }

        public long? clientid { get; set; }

        public virtual ICollection<Contact> ContactCollection { get; set; }
    }
}