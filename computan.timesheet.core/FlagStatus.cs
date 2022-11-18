using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace computan.timesheet.core
{
    public class FlagStatus : BaseEntity
    {
        public int id { get; set; }

        [MaxLength(255)][DisplayName("Name")] public string name { get; set; }

        [DisplayName("Active?")] public bool isactive { get; set; }
    }
}