using System.Data.Entity.Migrations;

namespace computan.timesheet.Contexts.IdentityMigrations
{
    public partial class LevelnumberAddInCredentialLevel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CredentialLevels", "LevelNumber", c => c.Int(false));
        }

        public override void Down()
        {
            DropColumn("dbo.CredentialLevels", "LevelNumber");
        }
    }
}