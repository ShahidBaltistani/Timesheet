namespace computan.timesheet.Models
{
    public class UserClientViewModel
    {
        public long clientid { get; set; }
        public string clientname { get; set; }
        public long? userfavouriteid { get; set; }
        public bool ispinned { get; set; } = false;

        public string iconfile
        {
            get
            {
                if (userfavouriteid == null)
                {
                    return "fa-star-o";
                }

                if (userfavouriteid == -1)
                {
                    return "fa-users";
                }

                if (userfavouriteid > 0)
                {
                    return "fa-star";
                }

                return "fa-star-o";
            }
        }

        public string actionclass
        {
            get
            {
                if (userfavouriteid == null)
                {
                    return "addfavclient";
                }

                if (userfavouriteid == -1)
                {
                    return "";
                }

                if (userfavouriteid > 0)
                {
                    return "removefavclient";
                }

                return "addfavclient";
            }
        }
    }
}