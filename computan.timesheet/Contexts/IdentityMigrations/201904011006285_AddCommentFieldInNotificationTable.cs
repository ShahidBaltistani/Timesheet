using System.Data.Entity.Migrations;

namespace computan.timesheet.Contexts.IdentityMigrations
{
    public partial class AddCommentFieldInNotificationTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Notifications", "commentid", c => c.Long(false));
        }

        public override void Down()
        {
            DropColumn("dbo.Notifications", "commentid");
        }
    }
}