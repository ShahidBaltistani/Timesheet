using System.Data.Entity.Migrations;

namespace computan.timesheet.Contexts.IdentityMigrations
{
    public partial class clientandprojectModify : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Clients", "accountmanager", c => c.String(maxLength: 128));
            AddColumn("dbo.Projects", "projectmanager", c => c.String(maxLength: 128));
            CreateIndex("dbo.Clients", "accountmanager");
            CreateIndex("dbo.Projects", "projectmanager");
            AddForeignKey("dbo.Projects", "projectmanager", "dbo.Users", "UsersId");
            AddForeignKey("dbo.Clients", "accountmanager", "dbo.Users", "UsersId");
        }

        public override void Down()
        {
            DropForeignKey("dbo.Clients", "accountmanager", "dbo.Users");
            DropForeignKey("dbo.Projects", "projectmanager", "dbo.Users");
            DropIndex("dbo.Projects", new[] { "projectmanager" });
            DropIndex("dbo.Clients", new[] { "accountmanager" });
            DropColumn("dbo.Projects", "projectmanager");
            DropColumn("dbo.Clients", "accountmanager");
        }
    }
}