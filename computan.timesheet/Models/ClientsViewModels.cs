using computan.timesheet.core;

namespace computan.timesheet.Models
{
    public class ClientsViewModels
    {
        public long id { get; set; }
        public string name { get; set; }
        public double? maxbillablehours { get; set; }
        public string email { get; set; }
        public string website { get; set; }
        public bool isactive { get; set; }
        public string pmplateformlink { get; set; }
        public string CustomerName { get; set; }
        public ApplicationUser User { get; set; }
    }
}