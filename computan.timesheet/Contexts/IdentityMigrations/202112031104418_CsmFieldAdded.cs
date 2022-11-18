using System.Data.Entity.Migrations;

namespace computan.timesheet.Contexts.IdentityMigrations
{
    public partial class CsmFieldAdded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Teams", "CSM", c => c.String());
        }

        public override void Down()
        {
            DropColumn("dbo.Teams", "CSM");
        }
    }
}