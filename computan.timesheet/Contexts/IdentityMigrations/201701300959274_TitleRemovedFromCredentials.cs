using System.Data.Entity.Migrations;

namespace computan.timesheet.Contexts.IdentityMigrations
{
    public partial class TitleRemovedFromCredentials : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Tickets", new[] { "tickettypeid" });
            AlterColumn("dbo.Tickets", "tickettypeid", c => c.Long());
            CreateIndex("dbo.Tickets", "tickettypeid");
            DropColumn("dbo.Credentials", "title");
        }

        public override void Down()
        {
            AddColumn("dbo.Credentials", "title", c => c.String(false));
            DropIndex("dbo.Tickets", new[] { "tickettypeid" });
            AlterColumn("dbo.Tickets", "tickettypeid", c => c.Long(false));
            CreateIndex("dbo.Tickets", "tickettypeid");
        }
    }
}