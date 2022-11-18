using System.Data.Entity.Migrations;

namespace computan.timesheet.Contexts.IdentityMigrations
{
    public partial class UserSkillsAdded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                    "dbo.UserSkills",
                    c => new
                    {
                        id = c.Long(false, true),
                        skillid = c.Long(false),
                        userid = c.String()
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Skills", t => t.skillid)
                .Index(t => t.skillid);
        }

        public override void Down()
        {
            DropForeignKey("dbo.UserSkills", "skillid", "dbo.Skills");
            DropIndex("dbo.UserSkills", new[] { "skillid" });
            DropTable("dbo.UserSkills");
        }
    }
}