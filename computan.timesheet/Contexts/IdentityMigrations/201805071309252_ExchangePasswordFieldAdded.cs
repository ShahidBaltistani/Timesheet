using System.Data.Entity.Migrations;

namespace computan.timesheet.Contexts.IdentityMigrations
{
    public partial class ExchangePasswordFieldAdded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                    "dbo.TicketReplays",
                    c => new
                    {
                        id = c.Long(false, true),
                        UserID = c.String(),
                        TicketID = c.Long(false),
                        Type = c.String(),
                        To = c.String(),
                        CC = c.String(),
                        BCC = c.String(),
                        Body = c.String(),
                        Attatchment = c.String(),
                        createdon = c.DateTime(false)
                    })
                .PrimaryKey(t => t.id);

            CreateTable(
                    "dbo.TicketTeamLogs",
                    c => new
                    {
                        id = c.Long(false, true),
                        ticketid = c.Long(false),
                        teamid = c.Long(false),
                        assignedbyusersid = c.String(maxLength: 128),
                        assignedon = c.DateTime(false),
                        displayorder = c.Long(),
                        statusid = c.Int(false),
                        statusupdatedbyusersid = c.String(maxLength: 128),
                        statusupdatedon = c.DateTime(false)
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Teams", t => t.teamid)
                .Index(t => t.teamid);

            AddColumn("dbo.Users", "ExchangePassword", c => c.String());
            AddColumn("dbo.Tickets", "fromEmail", c => c.String());
            AddColumn("dbo.Tickets", "projectid", c => c.Long());
            AddColumn("dbo.Tickets", "skillid", c => c.Long());
            AddColumn("dbo.Tickets", "startdate", c => c.DateTime());
            AddColumn("dbo.Tickets", "enddate", c => c.DateTime());
        }

        public override void Down()
        {
            DropForeignKey("dbo.TicketTeamLogs", "teamid", "dbo.Teams");
            DropIndex("dbo.TicketTeamLogs", new[] { "teamid" });
            DropColumn("dbo.Tickets", "enddate");
            DropColumn("dbo.Tickets", "startdate");
            DropColumn("dbo.Tickets", "skillid");
            DropColumn("dbo.Tickets", "projectid");
            DropColumn("dbo.Tickets", "fromEmail");
            DropColumn("dbo.Users", "ExchangePassword");
            DropTable("dbo.TicketTeamLogs");
            DropTable("dbo.TicketReplays");
        }
    }
}