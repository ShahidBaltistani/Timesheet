using System.Data.Entity.Migrations;

namespace computan.timesheet.Contexts.IdentityMigrations
{
    public partial class newTableForMergedTicketAdded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                    "dbo.TicketMergeLogs",
                    c => new
                    {
                        id = c.Long(false, true),
                        ticketid = c.Long(false),
                        conversationid = c.String(maxLength: 255),
                        uniquesenders = c.String(),
                        topic = c.String(),
                        lastdeliverytime = c.DateTime(false),
                        size = c.Long(false),
                        messagecount = c.Int(false),
                        hasattachments = c.Boolean(false),
                        importance = c.Boolean(false),
                        flagstatusid = c.Int(false),
                        lastmodifiedtime = c.DateTime(false),
                        statusid = c.Int(false),
                        IsArchieved = c.Boolean(false),
                        ticketpriortyid = c.Int(false),
                        tickettypeid = c.Long(),
                        statusupdatedbyusersid = c.String(maxLength: 128),
                        statusupdatedon = c.DateTime(false),
                        fromEmail = c.String(),
                        projectid = c.Long(),
                        skillid = c.Long(),
                        startdate = c.DateTime(),
                        enddate = c.DateTime(),
                        mergebyuserid = c.String(),
                        mergedinticketid = c.Long(false),
                        createdonutc = c.DateTime(false),
                        updatedonutc = c.DateTime(),
                        ipused = c.String(maxLength: 20),
                        userid = c.String()
                    })
                .PrimaryKey(t => t.id)
                .Index(t => t.conversationid, unique: true, name: "conversationid");
        }

        public override void Down()
        {
            DropIndex("dbo.TicketMergeLogs", "conversationid");
            DropTable("dbo.TicketMergeLogs");
        }
    }
}