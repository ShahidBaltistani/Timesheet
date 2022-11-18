namespace computan.timesheet.Contexts.IdentityMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class twoFA : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "IsAppAuthenticatorEnabled", c => c.Boolean(nullable: false));
            AddColumn("dbo.Users", "AppAuthenticatorSecretKey", c => c.String());
            DropColumn("dbo.Users", "IsGoogleAuthenticatorEnabled");
            DropColumn("dbo.Users", "GoogleAuthenticatorSecretKey");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Users", "GoogleAuthenticatorSecretKey", c => c.String());
            AddColumn("dbo.Users", "IsGoogleAuthenticatorEnabled", c => c.Boolean(nullable: false));
            DropColumn("dbo.Users", "AppAuthenticatorSecretKey");
            DropColumn("dbo.Users", "IsAppAuthenticatorEnabled");
        }
    }
}
