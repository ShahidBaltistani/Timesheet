using System.Data.Entity.Migrations;

namespace computan.timesheet.Contexts.IdentityMigrations
{
    public partial class PersonIdAddedInIdentityUser : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "Personid", c => c.Long());
        }

        public override void Down()
        {
            DropColumn("dbo.Users", "Personid");
        }
    }
}