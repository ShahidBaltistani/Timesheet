using System.Data.Entity.Migrations;

namespace computan.timesheet.Contexts.IdentityMigrations
{
    public partial class TicketLogTable : DbMigration
    {
        public override void Up()
        {
            RenameTable("dbo.TicketLogTypes", "ActionTypes");
            CreateTable(
                    "dbo.TicketLogs",
                    c => new
                    {
                        id = c.Long(false, true),
                        ticketid = c.Long(false),
                        actiontypeid = c.Long(false),
                        actiondate = c.DateTime(false),
                        actionbyuserId = c.String(),
                        ActionDescription = c.String()
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.ActionTypes", t => t.actiontypeid)
                .Index(t => t.actiontypeid);

            AddColumn("dbo.Clients", "customersource", c => c.String());
        }

        public override void Down()
        {
            DropForeignKey("dbo.TicketLogs", "actiontypeid", "dbo.ActionTypes");
            DropIndex("dbo.TicketLogs", new[] { "actiontypeid" });
            DropColumn("dbo.Clients", "customersource");
            DropTable("dbo.TicketLogs");
            RenameTable("dbo.ActionTypes", "TicketLogTypes");
        }
    }
}