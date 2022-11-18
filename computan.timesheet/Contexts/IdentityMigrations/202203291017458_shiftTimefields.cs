namespace computan.timesheet.Contexts.IdentityMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class shiftTimefields : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "IsPkHoliday", c => c.Boolean(nullable: false));
            AddColumn("dbo.Users", "IsRemoteJob", c => c.Boolean(nullable: false));
            AddColumn("dbo.Users", "ProjectManager", c => c.String());
            AddColumn("dbo.Users", "TeamLead", c => c.String());
            AddColumn("dbo.Users", "ShiftTimePK", c => c.String(maxLength: 25));
            AddColumn("dbo.Users", "ShiftTimeEST", c => c.String(maxLength: 25));
            DropColumn("dbo.Users", "ShiftTimings");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Users", "ShiftTimings", c => c.String());
            DropColumn("dbo.Users", "ShiftTimeEST");
            DropColumn("dbo.Users", "ShiftTimePK");
            DropColumn("dbo.Users", "TeamLead");
            DropColumn("dbo.Users", "ProjectManager");
            DropColumn("dbo.Users", "IsRemoteJob");
            DropColumn("dbo.Users", "IsPkHoliday");
        }
    }
}
