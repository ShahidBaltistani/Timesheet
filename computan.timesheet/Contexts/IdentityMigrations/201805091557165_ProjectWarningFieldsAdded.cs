using System.Data.Entity.Migrations;

namespace computan.timesheet.Contexts.IdentityMigrations
{
    public partial class ProjectWarningFieldsAdded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Projects", "iswarning", c => c.Boolean(false));
            AddColumn("dbo.Projects", "warningtext", c => c.String());
            AddColumn("dbo.Users", "ExchangeUsername", c => c.String());
        }

        public override void Down()
        {
            DropColumn("dbo.Users", "ExchangeUsername");
            DropColumn("dbo.Projects", "warningtext");
            DropColumn("dbo.Projects", "iswarning");
        }
    }
}