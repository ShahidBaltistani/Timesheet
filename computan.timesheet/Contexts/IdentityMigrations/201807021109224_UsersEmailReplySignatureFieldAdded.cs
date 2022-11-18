using System.Data.Entity.Migrations;

namespace computan.timesheet.Contexts.IdentityMigrations
{
    public partial class UsersEmailReplySignatureFieldAdded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "EmailReplySignature", c => c.String());
        }

        public override void Down()
        {
            DropColumn("dbo.Users", "EmailReplySignature");
        }
    }
}