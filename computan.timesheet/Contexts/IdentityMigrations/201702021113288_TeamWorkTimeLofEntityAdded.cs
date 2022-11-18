using System.Data.Entity.Migrations;

namespace computan.timesheet.Contexts.IdentityMigrations
{
    public partial class TeamWorkTimeLofEntityAdded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                    "dbo.TeamWorkTimeLogs",
                    c => new
                    {
                        id = c.Long(false, true),
                        ticketid = c.Long(false),
                        teamworktaskid = c.Long(false),
                        timeaddedinminuts = c.String(),
                        isaddedsuccessfully = c.Boolean(false),
                        createdonutc = c.DateTime(false),
                        updatedonutc = c.DateTime(),
                        ipused = c.String(maxLength: 20),
                        userid = c.String()
                    })
                .PrimaryKey(t => t.id);
        }

        public override void Down()
        {
            DropTable("dbo.TeamWorkTimeLogs");
        }
    }
}