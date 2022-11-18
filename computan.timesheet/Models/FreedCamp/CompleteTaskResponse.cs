using System.Collections.Generic;

namespace computan.timesheet.Models.FreedCamp
{
    public class CompleteTaskResponse
    {
        public int http_code { get; set; }
        public string msg { get; set; }
        public int error_id { get; set; }
        public CompleteTaskResponseData data { get; set; }
    }

    public class TaskFile
    {
        public string id { get; set; }
        public string name { get; set; }
        public string title { get; set; }
        public string url { get; set; }
        public string thumb_url { get; set; }
        public int size { get; set; }
        public int version_id { get; set; }
        public int first_version_id { get; set; }
        public bool is_last_version { get; set; }
        public string last_version_id { get; set; }
        public string project_id { get; set; }
        public string files_group_id { get; set; }
        public string file_type { get; set; }
        public string item_app_id { get; set; }
        public string item_id { get; set; }
        public string comment_id { get; set; }
        public string user_id { get; set; }
        public bool f_temporary { get; set; }
        public string location { get; set; }
        public string created_by_id { get; set; }
        public int created_ts { get; set; }
        public bool f_image { get; set; }
        public bool can_delete { get; set; }
        public bool can_edit { get; set; }
        public string app_id { get; set; }
    }

    public class TaskComment
    {
        public string id { get; set; }
        public string item_id { get; set; }
        public string item_app_id { get; set; }
        public string description { get; set; }
        public string description_processed { get; set; }
        public string user_id { get; set; }
        public string created_by_id { get; set; }
        public int created_ts { get; set; }
        public List<TaskFile> files { get; set; }
        public bool can_edit { get; set; }
        public bool f_unread { get; set; }
        public string user_full_name { get; set; }
        public string url { get; set; }
        public string app_id { get; set; }
    }

    public class Task
    {
        public string id { get; set; }
        public string h_parent_id { get; set; }
        public string h_top_id { get; set; }
        public int h_level { get; set; }
        public bool f_adv_subtask { get; set; }
        public string assigned_to_id { get; set; }
        public string created_by_id { get; set; }
        public int created_ts { get; set; }
        public string task_group_id { get; set; }
        public string project_id { get; set; }
        public int priority { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string description_processed { get; set; }
        public int status { get; set; }
        public int order { get; set; }
        public int comments_count { get; set; }
        public int files_count { get; set; }
        public int? completed_ts { get; set; }
        public int? start_ts { get; set; }
        public int? due_ts { get; set; }
        public string task_group_name { get; set; }
        public bool f_archived_list { get; set; }
        public string priority_title { get; set; }
        public string status_title { get; set; }
        public string assigned_to_fullname { get; set; }
        public string r_rule { get; set; }
        public bool can_delete { get; set; }
        public bool can_edit { get; set; }
        public bool can_assign { get; set; }
        public bool can_progress { get; set; }
        public bool can_comment { get; set; }
        public bool f_matched { get; set; }
        public bool f_visible { get; set; }
        public List<TaskComment> comments { get; set; }
        public List<TaskFile> files { get; set; }
        public string cf_tpl_id { get; set; }
        public string url { get; set; }
        public string app_id { get; set; }
        public List<CustomField> custom_fields { get; set; }
    }

    public class CompleteTaskResponseData
    {
        public List<Task> tasks { get; set; }
        public TaskMeta meta { get; set; }
        public Error error { get; set; }
    }

    public class TaskMeta
    {
        public bool has_more { get; set; }
        public int? total_count { get; set; }
    }

    public class CustomField
    {
        public string cf_id { get; set; }
        public string value { get; set; }
    }

    public class Error
    {
        public string general { get; set; }
    }
}