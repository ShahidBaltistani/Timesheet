using System.Data.Entity.Migrations;

namespace computan.timesheet.Contexts.IdentityMigrations
{
    public partial class BillingEntitesAdded : DbMigration
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

            CreateTable(
                    "dbo.BillingCyleTypes",
                    c => new
                    {
                        Id = c.Long(false, true),
                        name = c.String(),
                        isactive = c.Boolean(false),
                        createdonutc = c.DateTime(false),
                        updatedonutc = c.DateTime(),
                        ipused = c.String(maxLength: 20),
                        userid = c.String()
                    })
                .PrimaryKey(t => t.Id);

            CreateTable(
                    "dbo.ClientBillingCycles",
                    c => new
                    {
                        Id = c.Long(false, true),
                        clientid = c.Long(false),
                        billingcyletypeid = c.Long(false),
                        createdonutc = c.DateTime(false),
                        updatedonutc = c.DateTime(),
                        ipused = c.String(maxLength: 20),
                        userid = c.String()
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.BillingCyleTypes", t => t.billingcyletypeid)
                .ForeignKey("dbo.Clients", t => t.clientid)
                .Index(t => t.clientid)
                .Index(t => t.billingcyletypeid);

            CreateTable(
                    "dbo.ClientBillingInvoices",
                    c => new
                    {
                        Id = c.Long(false, true),
                        clientid = c.Long(false),
                        billingdate = c.DateTime(false),
                        hoursconsumed = c.Double(false),
                        ispaid = c.Boolean(false),
                        isapproved = c.Boolean(false),
                        billingcyletypeid = c.Long(false),
                        createdonutc = c.DateTime(false),
                        updatedonutc = c.DateTime(),
                        ipused = c.String(maxLength: 20),
                        userid = c.String()
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.BillingCyleTypes", t => t.billingcyletypeid)
                .ForeignKey("dbo.Clients", t => t.clientid)
                .Index(t => t.clientid)
                .Index(t => t.billingcyletypeid);

            AddColumn("dbo.Clients", "maxbillablehours", c => c.Double());
            AddColumn("dbo.ClientContacts", "isnotify", c => c.Boolean(false));
            DropColumn("dbo.RuleActions", "statusid");
        }

        public override void Down()
        {
            AddColumn("dbo.RuleActions", "statusid", c => c.Int());
            DropForeignKey("dbo.ClientBillingInvoices", "clientid", "dbo.Clients");
            DropForeignKey("dbo.ClientBillingInvoices", "billingcyletypeid", "dbo.BillingCyleTypes");
            DropForeignKey("dbo.ClientBillingCycles", "clientid", "dbo.Clients");
            DropForeignKey("dbo.ClientBillingCycles", "billingcyletypeid", "dbo.BillingCyleTypes");
            DropForeignKey("dbo.Billings", "clientid", "dbo.Clients");
            DropIndex("dbo.ClientBillingInvoices", new[] { "billingcyletypeid" });
            DropIndex("dbo.ClientBillingInvoices", new[] { "clientid" });
            DropIndex("dbo.ClientBillingCycles", new[] { "billingcyletypeid" });
            DropIndex("dbo.ClientBillingCycles", new[] { "clientid" });
            DropIndex("dbo.Billings", new[] { "clientid" });
            DropColumn("dbo.ClientContacts", "isnotify");
            DropColumn("dbo.Clients", "maxbillablehours");
            DropTable("dbo.ClientBillingInvoices");
            DropTable("dbo.ClientBillingCycles");
            DropTable("dbo.BillingCyleTypes");
            DropTable("dbo.Billings");
        }
    }
}