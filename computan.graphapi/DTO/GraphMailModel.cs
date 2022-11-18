namespace computan.graphapi.DTO
{
    public class GraphMailModel
    {
        public string type { get; set; }
        public string TO { get; set; }
        public string CC { get; set; }
        public string BCC { get; set; }
        public string Subject { get; set; }
        public string body { get; set; }
        public string Attach { get; set; }
        public string MessageId { get; set; }
        public long TicketID { get; set; }
        public string TicketTitle { get; set; }
        public long SentItemID { get; set; }
        public string RefreshToken { get; set; }
    }
}