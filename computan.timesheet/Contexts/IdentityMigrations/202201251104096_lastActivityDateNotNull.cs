namespace computan.timesheet.Contexts.IdentityMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class lastActivityDateNotNull : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Tickets", "LastActivityDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Tickets", "LastActivityDate", c => c.DateTime());
        }
    }
}
