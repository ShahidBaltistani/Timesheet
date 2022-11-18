using System.Data.Entity.Migrations;

namespace computan.timesheet.Contexts.IdentityMigrations
{
    public partial class FiletypeNameFieldChangedToString : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.FileTypes", "name", c => c.String());
        }

        public override void Down()
        {
            AlterColumn("dbo.FileTypes", "name", c => c.Long(false));
        }
    }
}