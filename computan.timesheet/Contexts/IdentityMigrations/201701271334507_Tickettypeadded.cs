using System.Data.Entity.Migrations;

namespace computan.timesheet.Contexts.IdentityMigrations
{
    public partial class Tickettypeadded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                    "dbo.TicketTypes",
                    c => new
                    {
                        id = c.Long(false, true),
                        name = c.String(false),
                        createdonutc = c.DateTime(false),
                        updatedonutc = c.DateTime(),
                        ipused = c.String(maxLength: 20),
                        userid = c.String()
                    })
                .PrimaryKey(t => t.id);

            AddColumn("dbo.Tickets", "tickettypeid", c => c.Long(false));
            CreateIndex("dbo.Tickets", "tickettypeid");
            AddForeignKey("dbo.Tickets", "tickettypeid", "dbo.TicketTypes", "id");
        }

        public override void Down()
        {
            DropForeignKey("dbo.Tickets", "tickettypeid", "dbo.TicketTypes");
            DropIndex("dbo.Tickets", new[] { "tickettypeid" });
            DropColumn("dbo.Tickets", "tickettypeid");
            DropTable("dbo.TicketTypes");
        }
    }
}