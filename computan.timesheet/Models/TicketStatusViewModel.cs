namespace computan.timesheet.Models
{
    public class TicketStatusViewModel
    {
        public int id { get; set; }
        public string name { get; set; }
        public bool isactive { get; set; }
        public int ticketcount { get; set; }

        public string iconname
        {
            get
            {
                string icon = "icon-files-empty";
                if (!string.IsNullOrEmpty(name))
                {
                    switch (name)
                    {
                        case "All Tickets":
                            icon = "icon-files-empty";
                            break;
                        case "In Progress":
                            icon = "icon-file-plus";
                            break;
                        case "Closed":
                            icon = "icon-file-check";
                            break;
                        case "Wont Fix":
                            icon = "icon-file-minus";
                            break;
                        case "Need Review":
                            icon = "icon-file-minus2";
                            break;
                        default:
                            icon = "icon-files-empty";
                            break;
                    }
                }

                return icon;
            }
        }
    }
}