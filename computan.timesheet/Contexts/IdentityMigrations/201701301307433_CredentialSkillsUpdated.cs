using System.Data.Entity.Migrations;

namespace computan.timesheet.Contexts.IdentityMigrations
{
    public partial class CredentialSkillsUpdated : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CredentialSkills", "credentailid", c => c.Long(false));
            AddColumn("dbo.CredentialSkills", "createdonutc", c => c.DateTime(false));
            AddColumn("dbo.CredentialSkills", "updatedonutc", c => c.DateTime());
            AddColumn("dbo.CredentialSkills", "ipused", c => c.String(maxLength: 20));
            AddColumn("dbo.CredentialSkills", "userid", c => c.String());
            DropColumn("dbo.CredentialSkills", "credentialid");
        }

        public override void Down()
        {
            AddColumn("dbo.CredentialSkills", "credentialid", c => c.Long(false));
            DropColumn("dbo.CredentialSkills", "userid");
            DropColumn("dbo.CredentialSkills", "ipused");
            DropColumn("dbo.CredentialSkills", "updatedonutc");
            DropColumn("dbo.CredentialSkills", "createdonutc");
            DropColumn("dbo.CredentialSkills", "credentailid");
        }
    }
}