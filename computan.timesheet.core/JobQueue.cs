using System;

namespace computan.timesheet.core
{
    public class JobQueue
    {
        public long id { get; set; }
        public string type { get; set; }
        public DateTime addtime { get; set; }
        public DateTime? completetime { get; set; }
        public int status { get; set; }
        public string url { get; set; }
        public string response { get; set; }
    }
}