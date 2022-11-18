using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace computan.timesheet.Models.Settings
{
    public class TwoFAViewModel
    {
        public bool IsAppAuthenticatorEnable { get; set; }
        public bool IsAppRocketEnable { get; set; }
        public bool IsEmailAuthenticatorEnable { get; set; }
        public string Name { get; set; }
    }
} 