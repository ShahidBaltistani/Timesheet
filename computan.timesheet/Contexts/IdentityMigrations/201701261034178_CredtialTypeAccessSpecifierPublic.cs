using System.Data.Entity.Migrations;

namespace computan.timesheet.Contexts.IdentityMigrations
{
    public partial class CredtialTypeAccessSpecifierPublic : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CredentialTypes", "isactive", c => c.Boolean(false));
            AlterColumn("dbo.CredentialCategories", "name", c => c.String(false));
        }

        public override void Down()
        {
            AlterColumn("dbo.CredentialCategories", "name", c => c.String());
            DropColumn("dbo.CredentialTypes", "isactive");
        }
    }
}