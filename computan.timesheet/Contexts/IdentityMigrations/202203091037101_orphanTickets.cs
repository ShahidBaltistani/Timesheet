using System.Data.Entity.Migrations;

namespace computan.timesheet.Contexts.IdentityMigrations
{
    public partial class orphanTickets : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                    "dbo.SubscribeTeams",
                    c => new
                    {
                        Id = c.Int(false, true),
                        TeamId = c.Long(false),
                        UsersId = c.String(),
                        createdonutc = c.DateTime(false),
                        updatedonutc = c.DateTime(),
                        ipused = c.String(maxLength: 20),
                        userid = c.String()
                    })
                .PrimaryKey(t => t.Id);

            CreateTable(
                    "dbo.SuppressTickets",
                    c => new
                    {
                        Id = c.Int(false, true),
                        TicketId = c.Long(false),
                        UsersId = c.String(),
                        createdonutc = c.DateTime(false),
                        updatedonutc = c.DateTime(),
                        ipused = c.String(maxLength: 20),
                        userid = c.String()
                    })
                .PrimaryKey(t => t.Id);

            AddColumn("dbo.ConversationStatus", "OrphanAge", c => c.Int(false));
            AddColumn("dbo.Teams", "RocketUrl", c => c.String());
            AddColumn("dbo.Tickets", "LastActivityDate", c => c.DateTime());
            AlterColumn("dbo.Teams", "Manager", c => c.String(maxLength: 128));
            AlterColumn("dbo.Teams", "CSM", c => c.String(maxLength: 128));
            CreateIndex("dbo.Teams", "Manager");
            CreateIndex("dbo.Teams", "CSM");
            AddForeignKey("dbo.Teams", "CSM", "dbo.Users", "UsersId");
            AddForeignKey("dbo.Teams", "Manager", "dbo.Users", "UsersId");
        }

        public override void Down()
        {
            DropForeignKey("dbo.Teams", "Manager", "dbo.Users");
            DropForeignKey("dbo.Teams", "CSM", "dbo.Users");
            DropIndex("dbo.Teams", new[] { "CSM" });
            DropIndex("dbo.Teams", new[] { "Manager" });
            AlterColumn("dbo.Teams", "CSM", c => c.String());
            AlterColumn("dbo.Teams", "Manager", c => c.String());
            DropColumn("dbo.Tickets", "LastActivityDate");
            DropColumn("dbo.Teams", "RocketUrl");
            DropColumn("dbo.ConversationStatus", "OrphanAge");
            DropTable("dbo.SuppressTickets");
            DropTable("dbo.SubscribeTeams");
        }
    }
}