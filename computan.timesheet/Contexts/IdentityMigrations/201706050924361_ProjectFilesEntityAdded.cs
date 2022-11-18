using System.Data.Entity.Migrations;

namespace computan.timesheet.Contexts.IdentityMigrations
{
    public partial class ProjectFilesEntityAdded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                    "dbo.ProjectFiles",
                    c => new
                    {
                        id = c.Long(false, true),
                        projectid = c.Long(false),
                        filename = c.String(),
                        filetypeid = c.Long(),
                        createdonutc = c.DateTime(false),
                        updatedonutc = c.DateTime(),
                        ipused = c.String(maxLength: 20),
                        userid = c.String()
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.FileTypes", t => t.filetypeid)
                .ForeignKey("dbo.Projects", t => t.projectid)
                .Index(t => t.projectid)
                .Index(t => t.filetypeid);

            CreateTable(
                    "dbo.FileTypes",
                    c => new
                    {
                        id = c.Long(false, true),
                        name = c.Long(false),
                        isative = c.Long(false),
                        createdonutc = c.DateTime(false),
                        updatedonutc = c.DateTime(),
                        ipused = c.String(maxLength: 20),
                        userid = c.String()
                    })
                .PrimaryKey(t => t.id);
        }

        public override void Down()
        {
            DropForeignKey("dbo.ProjectFiles", "projectid", "dbo.Projects");
            DropForeignKey("dbo.ProjectFiles", "filetypeid", "dbo.FileTypes");
            DropIndex("dbo.ProjectFiles", new[] { "filetypeid" });
            DropIndex("dbo.ProjectFiles", new[] { "projectid" });
            DropTable("dbo.FileTypes");
            DropTable("dbo.ProjectFiles");
        }
    }
}