using System.Data.Entity.Migrations;

namespace computan.timesheet.Contexts.IdentityMigrations
{
    public partial class attachmentid__myexceptionsentityadded : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Projects", new[] { "clientid" });
            DropIndex("dbo.TeamMembers", new[] { "usersid" });
            CreateTable(
                    "dbo.MyExceptions",
                    c => new
                    {
                        id = c.Long(false, true),
                        exceptiondate = c.DateTime(false),
                        controller = c.String(),
                        action = c.String(),
                        exception_message = c.String(),
                        exception_source = c.String(),
                        exception_stracktrace = c.String(),
                        exception_targetsite = c.String(),
                        ipused = c.String(),
                        userid = c.String()
                    })
                .PrimaryKey(t => t.id);

            AddColumn("dbo.TicketItemAttachments", "attachmentid", c => c.String());
            AlterColumn("dbo.Clients", "name", c => c.String(false, 255));
            AlterColumn("dbo.Projects", "clientid", c => c.Long(false));
            AlterColumn("dbo.Projects", "name", c => c.String(false, 255));
            AlterColumn("dbo.Skills", "name", c => c.String(false, 255));
            AlterColumn("dbo.States", "name", c => c.String(false, 40));
            AlterColumn("dbo.Teams", "name", c => c.String(false, 255));
            AlterColumn("dbo.TeamMembers", "usersid", c => c.String(false, 128));
            AlterColumn("dbo.Users", "FirstName", c => c.String(false, 100));
            AlterColumn("dbo.Users", "LastName", c => c.String(false, 100));
            CreateIndex("dbo.Projects", "clientid");
            CreateIndex("dbo.TeamMembers", "usersid");
        }

        public override void Down()
        {
            DropIndex("dbo.TeamMembers", new[] { "usersid" });
            DropIndex("dbo.Projects", new[] { "clientid" });
            AlterColumn("dbo.Users", "LastName", c => c.String(maxLength: 100));
            AlterColumn("dbo.Users", "FirstName", c => c.String(maxLength: 100));
            AlterColumn("dbo.TeamMembers", "usersid", c => c.String(maxLength: 128));
            AlterColumn("dbo.Teams", "name", c => c.String(maxLength: 255));
            AlterColumn("dbo.States", "name", c => c.String(maxLength: 40));
            AlterColumn("dbo.Skills", "name", c => c.String(maxLength: 255));
            AlterColumn("dbo.Projects", "name", c => c.String(maxLength: 255));
            AlterColumn("dbo.Projects", "clientid", c => c.Long());
            AlterColumn("dbo.Clients", "name", c => c.String(maxLength: 255));
            DropColumn("dbo.TicketItemAttachments", "attachmentid");
            DropTable("dbo.MyExceptions");
            CreateIndex("dbo.TeamMembers", "usersid");
            CreateIndex("dbo.Projects", "clientid");
        }
    }
}