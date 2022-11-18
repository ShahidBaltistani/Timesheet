using Newtonsoft.Json;

namespace computan.timesheet.Models
{
    public class TodoItem
    {
        [JsonProperty("project-id")] public string projectid { get; set; }

        public string order { get; set; }
        public bool canEdit { get; set; }
        public string id { get; set; }
        public bool completed { get; set; }
        public string description { get; set; }
        public string progress { get; set; }

        [JsonProperty("company-id")] public string companyid { get; set; }

        [JsonProperty("creator-avatar-url")] public string creatoravatarurl { get; set; }

        [JsonProperty("creator-id")] public string creatorid { get; set; }

        [JsonProperty("project-name")] public string projectname { get; set; }

        [JsonProperty("start-date")] public string startdate { get; set; }

        [JsonProperty("tasklist-private")] public string tasklistprivate { get; set; }

        public string lockdownId { get; set; }
        public bool canComplete { get; set; }

        [JsonProperty("todo-list-name")] public string todolistname { get; set; }

        public string @private { get; set; }
        public string status { get; set; }

        [JsonProperty("todo-list-id")] public string todolistid { get; set; }

        public string content { get; set; }

        [JsonProperty("company-name")] public string companyname { get; set; }

        [JsonProperty("creator-firstname")] public string creatorfirstname { get; set; }

        public bool canLogTime { get; set; }
        public string timeIsLogged { get; set; }
    }
}