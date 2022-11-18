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
            AddColumn("dbo.Users", "IsRocketAuthenticatorEnabled", c => c.Boolean(nullable: false));
            AlterColumn("dbo.Users", "FirstName", c => c.String(maxLength: 100));
            AlterColumn("dbo.Users", "LastName", c => c.String(maxLength: 100));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Users", "LastName", c => c.String(nullable: false, maxLength: 100));
            AlterColumn("dbo.Users", "FirstName", c => c.String(nullable: false, maxLength: 100));
            DropColumn("dbo.Users", "IsRocketAuthenticatorEnabled");
            DropColumn("dbo.Users", "AppAuthenticatorSecretKey");
            DropColumn("dbo.Users", "IsAppAuthenticatorEnabled");
        }
    }
}
