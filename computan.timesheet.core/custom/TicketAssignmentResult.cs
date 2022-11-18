namespace computan.timesheet.core.custom
{
    public class TicketAssignmentResult
    {
        public TicketAssignmentResultType ResultType { get; set; }
        public string ResultMessage { get; set; }
    }

    public enum TicketAssignmentResultType
    {
        Success,
        Error
    }
}