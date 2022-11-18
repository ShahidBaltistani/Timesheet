using System.Data.Entity.Migrations;

namespace computan.timesheet.Contexts.IdentityMigrations
{
    public partial class ProjectNotesEntityAdded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                    "dbo.ProjectNotes",
                    c => new
                    {
                        id = c.Long(false, true),
                        comments = c.String(),
                        addedbyuserid = c.String(maxLength: 128),
                        createdonutc = c.DateTime(false),
                        updatedonutc = c.DateTime(),
                        ipused = c.String(maxLength: 20),
                        userid = c.String()
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Users", t => t.addedbyuserid)
                .Index(t => t.addedbyuserid);
        }

        public override void Down()
        {
            DropForeignKey("dbo.ProjectNotes", "addedbyuserid", "dbo.Users");
            DropIndex("dbo.ProjectNotes", new[] { "addedbyuserid" });
            DropTable("dbo.ProjectNotes");
        }
    }
}