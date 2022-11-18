using System.Data.Entity.Migrations;

namespace computan.timesheet.Contexts.IdentityMigrations
{
    public partial class ClientTypeEntityAdded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                    "dbo.ClientTypes",
                    c => new
                    {
                        id = c.Long(false, true),
                        name = c.String(),
                        isactive = c.Boolean(false)
                    })
                .PrimaryKey(t => t.id);

            AddColumn("dbo.RuleActions", "statusid", c => c.Int());
        }

        public override void Down()
        {
            DropColumn("dbo.RuleActions", "statusid");
            DropTable("dbo.ClientTypes");
        }
    }
}