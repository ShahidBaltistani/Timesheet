using System.Data.Entity.Migrations;

namespace computan.timesheet.Contexts.IdentityMigrations
{
    public partial class ConversationIdUniqueIndex : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.Tickets", "conversationid", true, "conversationid");
        }

        public override void Down()
        {
            DropIndex("dbo.Tickets", "conversationid");
        }
    }
}