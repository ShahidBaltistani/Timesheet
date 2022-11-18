using System.Data.Entity.Migrations;

namespace computan.timesheet.Contexts.IdentityMigrations
{
    public partial class AddWarningTextAtClietnLevel : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                    "dbo.CustomerSources",
                    c => new
                    {
                        id = c.Long(false, true),
                        name = c.String(),
                        isactive = c.Boolean(false),
                        createdonutc = c.DateTime(false),
                        updatedonutc = c.DateTime(),
                        ipused = c.String(maxLength: 20),
                        userid = c.String()
                    })
                .PrimaryKey(t => t.id);

            AddColumn("dbo.Clients", "customersourceid", c => c.Long(false));
            AddColumn("dbo.Clients", "iswarning", c => c.Boolean(false));
            AddColumn("dbo.Clients", "warningtext", c => c.String());
            CreateIndex("dbo.Clients", "customersourceid");
            AddForeignKey("dbo.Clients", "customersourceid", "dbo.CustomerSources", "id");
            DropColumn("dbo.Clients", "customersource");
        }

        public override void Down()
        {
            AddColumn("dbo.Clients", "customersource", c => c.String());
            DropForeignKey("dbo.Clients", "customersourceid", "dbo.CustomerSources");
            DropIndex("dbo.Clients", new[] { "customersourceid" });
            DropColumn("dbo.Clients", "warningtext");
            DropColumn("dbo.Clients", "iswarning");
            DropColumn("dbo.Clients", "customersourceid");
            DropTable("dbo.CustomerSources");
        }
    }
}