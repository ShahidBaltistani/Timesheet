namespace computan.timesheet.Contexts.IdentityMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedFileFields : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "LatestResume", c => c.String());
            AddColumn("dbo.Users", "LastDegree", c => c.String());
            AddColumn("dbo.Users", "CNIC_Front", c => c.String());
            AddColumn("dbo.Users", "CNIC_Back", c => c.String());
            AddColumn("dbo.Users", "ExperienceLetter", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "ExperienceLetter");
            DropColumn("dbo.Users", "CNIC_Back");
            DropColumn("dbo.Users", "CNIC_Front");
            DropColumn("dbo.Users", "LastDegree");
            DropColumn("dbo.Users", "LatestResume");
        }
    }
}
