using System.Data.Entity.Migrations;

namespace computan.timesheet.Contexts.IdentityMigrations
{
    public partial class FreedcampProjectTableUpdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.FreedcampProjects", "createdonutc", c => c.DateTime(false));
            AddColumn("dbo.FreedcampProjects", "updatedonutc", c => c.DateTime());
            AddColumn("dbo.FreedcampProjects", "ipused", c => c.String(maxLength: 20));
            AddColumn("dbo.FreedcampProjects", "userid", c => c.String());
            AlterColumn("dbo.FreedcampProjects", "tsprojectid", c => c.Long());
            AlterColumn("dbo.FreedcampProjects", "skill", c => c.Int());
        }

        public override void Down()
        {
            AlterColumn("dbo.FreedcampProjects", "skill", c => c.String());
            AlterColumn("dbo.FreedcampProjects", "tsprojectid", c => c.Long(false));
            DropColumn("dbo.FreedcampProjects", "userid");
            DropColumn("dbo.FreedcampProjects", "ipused");
            DropColumn("dbo.FreedcampProjects", "updatedonutc");
            DropColumn("dbo.FreedcampProjects", "createdonutc");
        }
    }
}