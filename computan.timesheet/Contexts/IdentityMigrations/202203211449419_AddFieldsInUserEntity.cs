using System.Data.Entity.Migrations;

namespace computan.timesheet.Contexts.IdentityMigrations
{
    public partial class AddFieldsInUserEntity : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "DateOfBirth", c => c.DateTime());
            AddColumn("dbo.Users", "NationalIdentificationNumber", c => c.String());
            AddColumn("dbo.Users", "PersonalEmailAddress", c => c.String());
            AddColumn("dbo.Users", "PersonNameEmergency", c => c.String());
            AddColumn("dbo.Users", "EmergencyPhoneNumber", c => c.String());
            AddColumn("dbo.Users", "SpouseName", c => c.String());
            AddColumn("dbo.Users", "SpouseDateOfBirth", c => c.DateTime());
            AddColumn("dbo.Users", "ChildrenNames", c => c.String());
            AddColumn("dbo.Users", "DateOfJoining", c => c.DateTime());
            AddColumn("dbo.Users", "ShiftTimings", c => c.String());
            AddColumn("dbo.Users", "Experience", c => c.String());
            AddColumn("dbo.Users", "AccountNumber", c => c.String());
            AddColumn("dbo.Users", "BranchName", c => c.String());
        }

        public override void Down()
        {
            DropColumn("dbo.Users", "BranchName");
            DropColumn("dbo.Users", "AccountNumber");
            DropColumn("dbo.Users", "Experience");
            DropColumn("dbo.Users", "ShiftTimings");
            DropColumn("dbo.Users", "DateOfJoining");
            DropColumn("dbo.Users", "ChildrenNames");
            DropColumn("dbo.Users", "SpouseDateOfBirth");
            DropColumn("dbo.Users", "SpouseName");
            DropColumn("dbo.Users", "EmergencyPhoneNumber");
            DropColumn("dbo.Users", "PersonNameEmergency");
            DropColumn("dbo.Users", "PersonalEmailAddress");
            DropColumn("dbo.Users", "NationalIdentificationNumber");
            DropColumn("dbo.Users", "DateOfBirth");
        }
    }
}