using System.Data.Entity.Migrations;

namespace computan.timesheet.Contexts.IdentityMigrations
{
    public partial class BillingEntityAdded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                    "dbo.Billings",
                    c => new
                    {
                        id = c.Long(false, true),
                        clientid = c.Long(false),
                        billabletime = c.Int(false),
                        workdate = c.DateTime(false),
                        createdonutc = c.DateTime(false),
                        updatedonutc = c.DateTime(),
                        ipused = c.String(maxLength: 20),
                        userid = c.String()
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Clients", t => t.clientid)
                .Index(t => t.clientid);
        }

        public override void Down()
        {
            DropForeignKey("dbo.Billings", "clientid", "dbo.Clients");
            DropIndex("dbo.Billings", new[] { "clientid" });
            DropTable("dbo.Billings");
        }
    }
}