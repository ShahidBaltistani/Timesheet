using System.Data.Entity.Migrations;

namespace computan.timesheet.Contexts.IdentityMigrations
{
    public partial class AlterClientstables : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Clients", "customersourceid", c => c.Long(false));
            DropColumn("dbo.Clients", "customersource");
        }

        public override void Down()
        {
            AddColumn("dbo.Clients", "customersource", c => c.String());
            DropColumn("dbo.Clients", "customersourceid");
        }
    }
}