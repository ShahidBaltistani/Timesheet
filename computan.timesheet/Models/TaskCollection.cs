using Newtonsoft.Json;
using System.Collections.Generic;

namespace computan.timesheet.Models
{
    public class TaskCollection
    {
        [JsonProperty("todo-items")] public List<TodoItem> TodoItem { get; set; }
    }
}