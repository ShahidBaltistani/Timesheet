using System.Data.Entity.Migrations;

namespace computan.timesheet.Contexts.IdentityMigrations
{
    public partial class EmailTemplateEntityAdded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                    "dbo.EmailTemplates",
                    c => new
                    {
                        id = c.Long(false, true),
                        name = c.String(),
                        body = c.String()
                    })
                .PrimaryKey(t => t.id);
        }

        public override void Down()
        {
            DropTable("dbo.EmailTemplates");
        }
    }
}