using System.Data.Entity.Migrations;

namespace computan.timesheet.Contexts.IdentityMigrations
{
    public partial class jobqueue : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                    "dbo.JobQueues",
                    c => new
                    {
                        id = c.Long(false, true),
                        type = c.String(),
                        addtime = c.DateTime(false),
                        completetime = c.DateTime(),
                        status = c.Int(false),
                        url = c.String(),
                        response = c.String()
                    })
                .PrimaryKey(t => t.id);

            AddColumn("dbo.FreedCampTasks", "commentscount", c => c.Int(false));
            AddColumn("dbo.FreedCampTasks", "filescount", c => c.Int(false));
        }

        public override void Down()
        {
            DropColumn("dbo.FreedCampTasks", "filescount");
            DropColumn("dbo.FreedCampTasks", "commentscount");
            DropTable("dbo.JobQueues");
        }
    }
}