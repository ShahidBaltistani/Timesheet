using System.Data.Entity.Migrations;

namespace computan.timesheet.Contexts.IdentityMigrations
{
    public partial class ClientPmToolFieldAdded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Clients", "pmplateformlink", c => c.String());
        }

        public override void Down()
        {
            DropColumn("dbo.Clients", "pmplateformlink");
        }
    }
}