using System.Data.Entity.Migrations;

namespace computan.timesheet.Contexts.IdentityMigrations
{
    public partial class ProjectidAddedInProjectNotesEntity : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProjectNotes", "projectid", c => c.Long(false));
            CreateIndex("dbo.ProjectNotes", "projectid");
            AddForeignKey("dbo.ProjectNotes", "projectid", "dbo.Projects", "id");
        }

        public override void Down()
        {
            DropForeignKey("dbo.ProjectNotes", "projectid", "dbo.Projects");
            DropIndex("dbo.ProjectNotes", new[] { "projectid" });
            DropColumn("dbo.ProjectNotes", "projectid");
        }
    }
}