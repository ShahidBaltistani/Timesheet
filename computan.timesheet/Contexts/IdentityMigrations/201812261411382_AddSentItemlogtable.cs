using System.Data.Entity.Migrations;

namespace computan.timesheet.Contexts.IdentityMigrations
{
    public partial class AddSentItemlogtable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SentItemLogs", "Sentdate", c => c.DateTime(false));
        }

        public override void Down()
        {
            DropColumn("dbo.SentItemLogs", "Sentdate");
        }
    }
}