using System.Data.Entity.Migrations;

namespace computan.timesheet.Contexts.IdentityMigrations
{
    public partial class tablenamechange : DbMigration
    {
        public override void Up()
        {
            RenameTable("dbo.NotifiedUsers", "NotificationUsers");
        }

        public override void Down()
        {
            RenameTable("dbo.NotificationUsers", "NotifiedUsers");
        }
    }
}