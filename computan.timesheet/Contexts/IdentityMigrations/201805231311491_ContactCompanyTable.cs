using System.Data.Entity.Migrations;

namespace computan.timesheet.Contexts.IdentityMigrations
{
    public partial class ContactCompanyTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                    "dbo.ContactCompanies",
                    c => new
                    {
                        id = c.Long(false, true),
                        name = c.String(),
                        clientid = c.Long()
                    })
                .PrimaryKey(t => t.id);

            AddColumn("dbo.Contacts", "contactdomainid", c => c.Long());
            CreateIndex("dbo.Contacts", "contactdomainid");
            AddForeignKey("dbo.Contacts", "contactdomainid", "dbo.ContactCompanies", "id");
        }

        public override void Down()
        {
            DropForeignKey("dbo.Contacts", "contactdomainid", "dbo.ContactCompanies");
            DropIndex("dbo.Contacts", new[] { "contactdomainid" });
            DropColumn("dbo.Contacts", "contactdomainid");
            DropTable("dbo.ContactCompanies");
        }
    }
}