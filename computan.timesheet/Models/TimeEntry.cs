using Newtonsoft.Json;
using System;

namespace computan.timesheet.Models
{
    [Serializable]
    public class TimeEntry
    {
        public string description { get; set; }

        [JsonProperty("person-id")] public string personid { get; set; }

        public string date { get; set; }
        public string time { get; set; }
        public string hours { get; set; }
        public string minutes { get; set; }
        public string isbillable { get; set; }
        public string tags { get; set; }
    }
}