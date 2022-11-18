using System.Data.Entity.Migrations;

namespace computan.timesheet.Contexts.IdentityMigrations
{
    public partial class SkypeidAddedInUserTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "Skypeid", c => c.String());
            DropColumn("dbo.Users", "Personid");
        }

        public override void Down()
        {
            AddColumn("dbo.Users", "Personid", c => c.Long());
            DropColumn("dbo.Users", "Skypeid");
        }
    }
}