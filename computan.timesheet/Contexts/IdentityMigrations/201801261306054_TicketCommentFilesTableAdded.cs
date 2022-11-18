using System.Data.Entity.Migrations;

namespace computan.timesheet.Contexts.IdentityMigrations
{
    public partial class TicketCommentFilesTableAdded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                    "dbo.TicketComments",
                    c => new
                    {
                        id = c.Long(false, true),
                        ticketid = c.Long(false),
                        commenton = c.DateTime(false),
                        commentbyuserid = c.String(maxLength: 128),
                        commentbyusername = c.String(maxLength: 200),
                        commentbody = c.String(),
                        parentid = c.Long(),
                        createdonutc = c.DateTime(false),
                        updatedonutc = c.DateTime(),
                        ipused = c.String(maxLength: 20),
                        userid = c.String()
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Users", t => t.commentbyuserid)
                .ForeignKey("dbo.TicketComments", t => t.parentid)
                .ForeignKey("dbo.Tickets", t => t.ticketid)
                .Index(t => t.ticketid)
                .Index(t => t.commentbyuserid)
                .Index(t => t.parentid);

            CreateTable(
                    "dbo.TicketCommentUserReads",
                    c => new
                    {
                        id = c.Long(false, true),
                        ticketcommentid = c.Long(false),
                        userid = c.String()
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.TicketComments", t => t.ticketcommentid)
                .Index(t => t.ticketcommentid);

            CreateTable(
                    "dbo.TicketFiles",
                    c => new
                    {
                        id = c.Long(false, true),
                        ticketid = c.Long(false),
                        filename = c.String(maxLength: 255),
                        filetypeid = c.Long(),
                        filepath = c.String(),
                        ticketcommentid = c.Long(),
                        createdonutc = c.DateTime(false),
                        updatedonutc = c.DateTime(),
                        ipused = c.String(maxLength: 20),
                        userid = c.String()
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.FileTypes", t => t.filetypeid)
                .ForeignKey("dbo.Tickets", t => t.ticketid)
                .ForeignKey("dbo.TicketComments", t => t.ticketcommentid)
                .Index(t => t.ticketid)
                .Index(t => t.filetypeid)
                .Index(t => t.ticketcommentid);
        }

        public override void Down()
        {
            DropForeignKey("dbo.TicketFiles", "ticketcommentid", "dbo.TicketComments");
            DropForeignKey("dbo.TicketFiles", "ticketid", "dbo.Tickets");
            DropForeignKey("dbo.TicketFiles", "filetypeid", "dbo.FileTypes");
            DropForeignKey("dbo.TicketCommentUserReads", "ticketcommentid", "dbo.TicketComments");
            DropForeignKey("dbo.TicketComments", "ticketid", "dbo.Tickets");
            DropForeignKey("dbo.TicketComments", "parentid", "dbo.TicketComments");
            DropForeignKey("dbo.TicketComments", "commentbyuserid", "dbo.Users");
            DropIndex("dbo.TicketFiles", new[] { "ticketcommentid" });
            DropIndex("dbo.TicketFiles", new[] { "filetypeid" });
            DropIndex("dbo.TicketFiles", new[] { "ticketid" });
            DropIndex("dbo.TicketCommentUserReads", new[] { "ticketcommentid" });
            DropIndex("dbo.TicketComments", new[] { "parentid" });
            DropIndex("dbo.TicketComments", new[] { "commentbyuserid" });
            DropIndex("dbo.TicketComments", new[] { "ticketid" });
            DropTable("dbo.TicketFiles");
            DropTable("dbo.TicketCommentUserReads");
            DropTable("dbo.TicketComments");
        }
    }
}