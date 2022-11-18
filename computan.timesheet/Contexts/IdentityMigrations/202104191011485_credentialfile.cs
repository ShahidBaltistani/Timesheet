using System.Data.Entity.Migrations;

namespace computan.timesheet.Contexts.IdentityMigrations
{
    public class credentialfile : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.TicketComments", "parentid", "dbo.TicketComments");
            DropIndex("dbo.TicketComments", new[] { "parentid" });
            AddColumn("dbo.Credentials", "crendentialfile", c => c.Binary());
            AddColumn("dbo.Credentials", "filename", c => c.String());
            AddColumn("dbo.TicketComments", "ParentComment_id", c => c.Long());
            CreateIndex("dbo.TicketComments", "ParentComment_id");
            AddForeignKey("dbo.TicketComments", "ParentComment_id", "dbo.TicketComments", "id");
        }

        public override void Down()
        {
            DropForeignKey("dbo.TicketComments", "ParentComment_id", "dbo.TicketComments");
            DropIndex("dbo.TicketComments", new[] { "ParentComment_id" });
            DropColumn("dbo.TicketComments", "ParentComment_id");
            DropColumn("dbo.Credentials", "filename");
            DropColumn("dbo.Credentials", "crendentialfile");
            CreateIndex("dbo.TicketComments", "parentid");
            AddForeignKey("dbo.TicketComments", "parentid", "dbo.TicketComments", "id");
        }
    }
}