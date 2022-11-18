using System.ComponentModel;

namespace computan.timesheet.core
{
    public class CredentialType : BaseEntity
    {
        public long id { get; set; }

        [DisplayName("Name")] public string name { get; set; }

        [DisplayName("Status")] public bool isactive { get; set; }
    }
}