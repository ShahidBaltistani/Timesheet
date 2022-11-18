namespace computan.timesheet.core.common
{
    public enum Role
    {
        Admin,
        TeamLead,
        User
    }

    public enum ProjectCategory
    {
        ActiveProjects,
        MyProjects,
        AllProjects
    }

    public enum TicketsStatus
    {
        NewTask = 1,
        InProgress = 2,
        Done = 3,
        OnHold = 4,
        QC = 5,
        Assigned = 6,
        InReview = 7,
        Trash = 8
    }

    public enum IntegrationSystem
    {
        Freedcamp,
        GitHub,
        GraphApi,
        Asana
    }
    
    public enum APIResponse
    {
        OK=200,
        BadRequest=400,
        Unauthorized =401,
        NotFound = 404
    }
}