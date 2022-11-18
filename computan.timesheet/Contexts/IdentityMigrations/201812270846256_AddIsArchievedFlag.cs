using System.Data.Entity.Migrations;

namespace computan.timesheet.Contexts.IdentityMigrations
{
    public partial class AddIsArchievedFlag : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tickets", "IsArchieved", c => c.Boolean(false));
        }

        public override void Down()
        {
            DropColumn("dbo.Tickets", "IsArchieved");
        }
    }
}