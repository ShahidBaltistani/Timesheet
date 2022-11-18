using System.Data.Entity.Migrations;

namespace computan.timesheet.Contexts.IdentityMigrations
{
    public partial class alterClientTables : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.Clients", "customersourceid");
            AddForeignKey("dbo.Clients", "customersourceid", "dbo.CustomerSources", "id");
        }

        public override void Down()
        {
            DropForeignKey("dbo.Clients", "customersourceid", "dbo.CustomerSources");
            DropIndex("dbo.Clients", new[] { "customersourceid" });
        }
    }
}