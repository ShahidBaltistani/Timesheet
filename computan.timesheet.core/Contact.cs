using System.ComponentModel.DataAnnotations.Schema;

namespace computan.timesheet.core
{
    public class Contact : BaseEntity
    {
        public long Id { get; set; }

        public long? contactdomainid { get; set; }

        [ForeignKey("contactdomainid")] public ContactCompany ContactCompany { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string DisplayName { get; set; }

        public string Email { get; set; }

        public bool isactive { get; set; }
    }
}