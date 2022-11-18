using System.ComponentModel;

namespace computan.timesheet.core
{
    public class CredentialLevel : BaseEntity
    {
        public long id { get; set; }

        [DisplayName("Name")] public string name { get; set; }

        [DisplayName("Level Number")] public int LevelNumber { get; set; }

        [DisplayName("Status")] public bool isactive { get; set; }
    }
}