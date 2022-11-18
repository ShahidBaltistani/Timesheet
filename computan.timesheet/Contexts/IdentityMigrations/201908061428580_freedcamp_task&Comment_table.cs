using System.Data.Entity.Migrations;

namespace computan.timesheet.Contexts.IdentityMigrations
{
    public partial class freedcamp_taskComment_table : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                    "dbo.FreedcampComments",
                    c => new
                    {
                        id = c.Long(false, true),
                        freedcamp_commentid = c.Long(false),
                        freedcamp_taskid = c.Long(false),
                        ticketitemid = c.Long(false)
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.FreedCampTasks", t => t.freedcamp_taskid)
                .Index(t => t.freedcamp_taskid);

            CreateTable(
                    "dbo.FreedCampTasks",
                    c => new
                    {
                        id = c.Long(false, true),
                        freedcamp_taskid = c.Long(false),
                        freedcamp_projectid = c.Long(false),
                        ticketid = c.Long(true),
                        title = c.String(),
                        description = c.String(),
                        statusid = c.Int(false),
                        createddate = c.DateTime(false),
                        url = c.String(),
                        createdon = c.DateTime(false)
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.FreedcampProjects", t => t.freedcamp_projectid)
                .Index(t => t.freedcamp_projectid);
        }

        public override void Down()
        {
            DropForeignKey("dbo.FreedcampComments", "freedcamp_taskid", "dbo.FreedCampTasks");
            DropForeignKey("dbo.FreedCampTasks", "freedcamp_projectid", "dbo.FreedcampProjects");
            DropIndex("dbo.FreedCampTasks", new[] { "freedcamp_projectid" });
            DropIndex("dbo.FreedcampComments", new[] { "freedcamp_taskid" });
            DropTable("dbo.FreedCampTasks");
            DropTable("dbo.FreedcampComments");
        }
    }
}