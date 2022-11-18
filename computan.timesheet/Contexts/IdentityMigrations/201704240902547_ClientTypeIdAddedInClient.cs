using System.Data.Entity.Migrations;

namespace computan.timesheet.Contexts.IdentityMigrations
{
    public partial class ClientTypeIdAddedInClient : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Clients", "clienttypeid", c => c.Long());
            CreateIndex("dbo.Clients", "clienttypeid");
            AddForeignKey("dbo.Clients", "clienttypeid", "dbo.ClientTypes", "id");
        }

        public override void Down()
        {
            DropForeignKey("dbo.Clients", "clienttypeid", "dbo.ClientTypes");
            DropIndex("dbo.Clients", new[] { "clienttypeid" });
            DropColumn("dbo.Clients", "clienttypeid");
        }
    }
}