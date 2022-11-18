using System.Data.Entity.Migrations;

namespace computan.timesheet.Contexts.IdentityMigrations
{
    public partial class nextlinkfieldadded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.NextLinks", "createdat", c => c.DateTime(false));
        }

        public override void Down()
        {
            DropColumn("dbo.NextLinks", "createdat");
        }
    }
}