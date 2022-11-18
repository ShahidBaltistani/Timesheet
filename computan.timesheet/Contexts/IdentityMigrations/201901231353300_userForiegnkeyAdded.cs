using System.Data.Entity.Migrations;

namespace computan.timesheet.Contexts.IdentityMigrations
{
    public partial class userForiegnkeyAdded : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Notifications", "actorid", c => c.String(maxLength: 128));
            CreateIndex("dbo.Notifications", "actorid");
            AddForeignKey("dbo.Notifications", "actorid", "dbo.Users", "UsersId");
        }

        public override void Down()
        {
            DropForeignKey("dbo.Notifications", "actorid", "dbo.Users");
            DropIndex("dbo.Notifications", new[] { "actorid" });
            AlterColumn("dbo.Notifications", "actorid", c => c.String());
        }
    }
}