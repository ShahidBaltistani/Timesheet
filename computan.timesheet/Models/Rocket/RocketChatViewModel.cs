using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace computan.timesheet.Models.Rocket
{
    public class RocketCreateViewModel
    {
        public string name { get; set; }

        public string pass { get; set; }

        public string email { get; set; }

        public string username { get; set; }
        public bool verified { get; set; }
        public string secretURL { get; set; }
    }
    public class RocketStatusViewModel
    {
        public string userId { get; set; }
        public bool activeStatus { get; set; }
        public bool active { get; set; }
        public string name { get; set; }

        public string pass { get; set; }

        public string email { get; set; }

        public string username { get; set; }
        public bool verified { get; set; }
    }

    public class RocketUserViewModel
    {
        public string _id { get; set; }
        public string userId { get; set; }
        public bool activeStatus { get; set; }

    }
    public class Root
    {
        public RocketUserViewModel user { get; set; }
        public bool success { get; set; }
    }

    public class RocketSetting 
    {
        public string Baseurl { get; set; }
        public string AuthToken { get; set; }
        public string UserId { get; set; }
        public string SecretURL { get; set; }
    } 
    
    public class RocketUpdateViewModel 
    {
        public RocketStatusViewModel data { get; set; }
        public string userId { get; set; }
    }

}