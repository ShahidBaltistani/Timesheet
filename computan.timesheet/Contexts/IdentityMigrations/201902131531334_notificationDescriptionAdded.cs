using System.Data.Entity.Migrations;

namespace computan.timesheet.Contexts.IdentityMigrations
{
    public partial class notificationDescriptionAdded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Notifications", "description", c => c.String());
        }

        public override void Down()
        {
            DropColumn("dbo.Notifications", "description");
        }
    }
}