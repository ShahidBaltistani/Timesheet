using System.Data.Entity.Migrations;

namespace computan.timesheet.Contexts.IdentityMigrations
{
    public partial class NotificationLimitForBillingEntityAddedForBilling : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                    "dbo.NotificationLimitForBillings",
                    c => new
                    {
                        id = c.Long(false, true),
                        NotificationLimit = c.Double(false),
                        createdonutc = c.DateTime(false),
                        updatedonutc = c.DateTime(),
                        ipused = c.String(maxLength: 20),
                        userid = c.String()
                    })
                .PrimaryKey(t => t.id);
        }

        public override void Down()
        {
            DropTable("dbo.NotificationLimitForBillings");
        }
    }
}