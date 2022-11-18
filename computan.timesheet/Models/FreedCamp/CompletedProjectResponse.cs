using System.Collections.Generic;

namespace computan.timesheet.Models.FreedCamp
{
    public class CompleteProjectResponse
    {
        public int http_code { get; set; }
        public string msg { get; set; }
        public int error_id { get; set; }
        public CompleteProjectResponseData data { get; set; }
    }

    public class FcProject
    {
        public string project_id { get; set; }
        public string group_id { get; set; }
        public string group_name { get; set; }
        public string role_name { get; set; }
        public string role_type { get; set; }
        public string project_name { get; set; }
        public string project_description { get; set; }
        public string project_unique_name { get; set; }
        public string project_color { get; set; }
        public int order { get; set; }
        public bool f_active { get; set; }
        public bool f_favorite { get; set; }
        public bool f_can_delete { get; set; }
        public bool f_can_manage { get; set; }
        public bool f_can_leave { get; set; }
        public int notifications_count { get; set; }
        public object features { get; set; }
        public List<string> users { get; set; }
        public List<string> applications { get; set; }
    }

    public class FcUser
    {
        public string user_id { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string avatar_url { get; set; }
        public string full_name { get; set; }
        public string email { get; set; }
        public string timezone { get; set; }
    }

    public class FcGroup
    {
        public string group_id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string group_unique_name { get; set; }
        public string owner_id { get; set; }
        public int order { get; set; }
        public IList<string> projects { get; set; }
        public IList<object> applications { get; set; }
        public bool f_managed { get; set; }
    }

    public class FcPermissions
    {
        public bool f_can_create_groups { get; set; }
        public bool f_can_create_projects { get; set; }
        public bool f_can_clone_projects { get; set; }
        public bool f_using_ms { get; set; }
    }

    public class CompleteProjectResponseData
    {
        public List<FcProject> projects { get; set; }
        public List<FcUser> users { get; set; }
        public List<FcGroup> groups { get; set; }
        public FcPermissions permissions { get; set; }
        public int notifications_count { get; set; }
    }
}