using System.Data.Entity.Migrations;

namespace computan.timesheet.Contexts.IdentityMigrations
{
    public partial class BillingNotificatointypeAndBillingNotificationEntityAded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                    "dbo.BillingNotifications",
                    c => new
                    {
                        id = c.Long(false, true),
                        clientid = c.Long(false),
                        notificationtypeid = c.Long(false),
                        billingmonth = c.String(),
                        billingweek = c.String(),
                        body = c.String(),
                        torecipients = c.String(),
                        createdonutc = c.DateTime(false),
                        updatedonutc = c.DateTime(),
                        ipused = c.String(maxLength: 20),
                        userid = c.String()
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.BillingNotificationTypes", t => t.notificationtypeid)
                .ForeignKey("dbo.Clients", t => t.clientid)
                .Index(t => t.clientid)
                .Index(t => t.notificationtypeid);

            CreateTable(
                    "dbo.BillingNotificationTypes",
                    c => new
                    {
                        id = c.Long(false, true),
                        name = c.String(),
                        isactive = c.Boolean(false)
                    })
                .PrimaryKey(t => t.id);
        }

        public override void Down()
        {
            DropForeignKey("dbo.BillingNotifications", "clientid", "dbo.Clients");
            DropForeignKey("dbo.BillingNotifications", "notificationtypeid", "dbo.BillingNotificationTypes");
            DropIndex("dbo.BillingNotifications", new[] { "notificationtypeid" });
            DropIndex("dbo.BillingNotifications", new[] { "clientid" });
            DropTable("dbo.BillingNotificationTypes");
            DropTable("dbo.BillingNotifications");
        }
    }
}