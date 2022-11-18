using System.Data.Entity.Migrations;

namespace computan.timesheet.Contexts.IdentityMigrations
{
    public partial class CredentialSkillEntityAdded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                    "dbo.CredentialSkills",
                    c => new
                    {
                        id = c.Long(false, true),
                        skillid = c.Long(false),
                        credentialid = c.Long(false)
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Skills", t => t.skillid)
                .Index(t => t.skillid);
        }

        public override void Down()
        {
            DropForeignKey("dbo.CredentialSkills", "skillid", "dbo.Skills");
            DropIndex("dbo.CredentialSkills", new[] { "skillid" });
            DropTable("dbo.CredentialSkills");
        }
    }
}