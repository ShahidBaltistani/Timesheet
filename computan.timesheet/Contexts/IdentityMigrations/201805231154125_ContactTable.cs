using System.Data.Entity.Migrations;

namespace computan.timesheet.Contexts.IdentityMigrations
{
    public partial class ContactTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                    "dbo.Contacts",
                    c => new
                    {
                        Id = c.Long(false, true),
                        FirstName = c.String(),
                        LastName = c.String(),
                        DisplayName = c.String(),
                        Email = c.String(),
                        isactive = c.Boolean(false),
                        createdonutc = c.DateTime(false),
                        updatedonutc = c.DateTime(),
                        ipused = c.String(maxLength: 20),
                        userid = c.String()
                    })
                .PrimaryKey(t => t.Id);
        }

        public override void Down()
        {
            DropTable("dbo.Contacts");
        }
    }
}