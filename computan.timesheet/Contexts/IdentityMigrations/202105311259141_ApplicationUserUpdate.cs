using System.Data.Entity.Migrations;

namespace computan.timesheet.Contexts.IdentityMigrations
{
    public partial class ApplicationUserUpdate : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.TicketComments", "parentid");
            RenameColumn("dbo.TicketComments", "ParentComment_id", "parentid");
            RenameIndex("dbo.TicketComments", "IX_ParentComment_id", "IX_parentid");
            AddColumn("dbo.Users", "createdonutc", c => c.DateTime(false));
            AddColumn("dbo.Users", "updatedonutc", c => c.DateTime());
            AddColumn("dbo.Users", "ipused", c => c.String(maxLength: 20));
            AddColumn("dbo.Users", "userid", c => c.String());
        }

        public override void Down()
        {
            DropColumn("dbo.Users", "userid");
            DropColumn("dbo.Users", "ipused");
            DropColumn("dbo.Users", "updatedonutc");
            DropColumn("dbo.Users", "createdonutc");
            RenameIndex("dbo.TicketComments", "IX_parentid", "IX_ParentComment_id");
            RenameColumn("dbo.TicketComments", "parentid", "ParentComment_id");
            AddColumn("dbo.TicketComments", "parentid", c => c.Long());
        }
    }
}