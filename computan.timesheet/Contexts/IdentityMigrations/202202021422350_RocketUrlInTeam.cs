namespace computan.timesheet.Contexts.IdentityMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RocketUrlInTeam : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Teams", "RocketUrl", c => c.String());
            AlterColumn("dbo.Tickets", "LastActivityDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Tickets", "LastActivityDate", c => c.DateTime(nullable: false));
            DropColumn("dbo.Teams", "RocketUrl");
        }
    }
}
