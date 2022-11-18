using System.Data.Entity.Migrations;

namespace computan.timesheet.Contexts.IdentityMigrations
{
    public partial class FreedCampProjectTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                    "dbo.FreedcampProjects",
                    c => new
                    {
                        id = c.Long(false, true),
                        fcprojectid = c.Long(false),
                        tsprojectid = c.Long(),
                        name = c.String(),
                        skill = c.String(),
                        team = c.String(),
                        assignedto = c.String(),
                        isactive = c.Boolean(false)
                    })
                .PrimaryKey(t => t.id);
        }

        public override void Down()
        {
            DropTable("dbo.FreedcampProjects");
        }
    }
}