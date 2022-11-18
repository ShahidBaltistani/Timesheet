using System.Collections.Generic;

namespace computan.timesheet.Models
{
    public class FcmNotificationResult
    {
        public long multicast_id { get; set; }
        public int success { get; set; }
        public int failure { get; set; }
        public int canonical_ids { get; set; }
        public IList<Result> results { get; set; }
    }

    public class Result
    {
        public string message_id { get; set; }
    }
}