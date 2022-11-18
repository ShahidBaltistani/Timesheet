using System.Data.Entity.Migrations;

namespace computan.timesheet.Contexts.IdentityMigrations
{
    public partial class ConversationStatusEntityAdded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                    "dbo.ConversationStatus",
                    c => new
                    {
                        id = c.Int(false, true),
                        name = c.String(maxLength: 255),
                        isactive = c.Boolean(false),
                        createdonutc = c.DateTime(false),
                        updatedonutc = c.DateTime(),
                        ipused = c.String(maxLength: 20),
                        userid = c.String()
                    })
                .PrimaryKey(t => t.id);
        }

        public override void Down()
        {
            DropTable("dbo.ConversationStatus");
        }
    }
}