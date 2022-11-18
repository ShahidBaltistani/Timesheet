using System.Data.Entity.Migrations;

namespace computan.timesheet.Contexts.IdentityMigrations
{
    public partial class TimeEntryLogs : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                    "dbo.TimeEntryLogs",
                    c => new
                    {
                        id = c.Long(false, true),
                        unrestricteduserid = c.String(maxLength: 128),
                        createdonutc = c.DateTime(false),
                        updatedonutc = c.DateTime(),
                        ipused = c.String(maxLength: 20),
                        userid = c.String()
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Users", t => t.unrestricteduserid)
                .Index(t => t.unrestricteduserid);
        }

        public override void Down()
        {
            DropForeignKey("dbo.TimeEntryLogs", "unrestricteduserid", "dbo.Users");
            DropIndex("dbo.TimeEntryLogs", new[] { "unrestricteduserid" });
            DropTable("dbo.TimeEntryLogs");
        }
    }
}