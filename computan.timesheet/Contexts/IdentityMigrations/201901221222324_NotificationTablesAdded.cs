using System.Data.Entity.Migrations;

namespace computan.timesheet.Contexts.IdentityMigrations
{
    public partial class NotificationTablesAdded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                    "dbo.Notifications",
                    c => new
                    {
                        id = c.Long(false, true),
                        entityid = c.Long(false),
                        entityactionid = c.Long(false),
                        createdon = c.DateTime(false),
                        actorid = c.String()
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.NotificationEntityActions", t => t.entityactionid)
                .Index(t => t.entityactionid);

            CreateTable(
                    "dbo.NotificationEntityActions",
                    c => new
                    {
                        id = c.Long(false, true),
                        name = c.String(),
                        entityid = c.Long(false),
                        isActive = c.Boolean(false)
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.NotificationEntities", t => t.entityid)
                .Index(t => t.entityid);

            CreateTable(
                    "dbo.NotificationEntities",
                    c => new
                    {
                        id = c.Long(false, true),
                        name = c.String(),
                        isActive = c.Boolean(false)
                    })
                .PrimaryKey(t => t.id);

            CreateTable(
                    "dbo.NotifiedUsers",
                    c => new
                    {
                        Id = c.Long(false, true),
                        notification_Id = c.Long(false),
                        notifierid = c.String(),
                        status = c.Boolean(false)
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Notifications", t => t.notification_Id)
                .Index(t => t.notification_Id);
        }

        public override void Down()
        {
            DropForeignKey("dbo.NotifiedUsers", "notification_Id", "dbo.Notifications");
            DropForeignKey("dbo.Notifications", "entityactionid", "dbo.NotificationEntityActions");
            DropForeignKey("dbo.NotificationEntityActions", "entityid", "dbo.NotificationEntities");
            DropIndex("dbo.NotifiedUsers", new[] { "notification_Id" });
            DropIndex("dbo.NotificationEntityActions", new[] { "entityid" });
            DropIndex("dbo.Notifications", new[] { "entityactionid" });
            DropTable("dbo.NotifiedUsers");
            DropTable("dbo.NotificationEntities");
            DropTable("dbo.NotificationEntityActions");
            DropTable("dbo.Notifications");
        }
    }
}