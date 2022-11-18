using System.Data.Entity.Migrations;

namespace computan.timesheet.Contexts.IdentityMigrations
{
    public partial class UserFavouriteTableAdded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                    "dbo.UserFavourites",
                    c => new
                    {
                        id = c.Long(false, true),
                        userfavouritetypeid = c.Int(false),
                        userfavouriteid = c.Long(false),
                        userid = c.String()
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.UserFavouriteTypes", t => t.userfavouritetypeid)
                .Index(t => t.userfavouritetypeid);

            CreateTable(
                    "dbo.UserFavouriteTypes",
                    c => new
                    {
                        id = c.Int(false, true),
                        name = c.String()
                    })
                .PrimaryKey(t => t.id);
        }

        public override void Down()
        {
            DropForeignKey("dbo.UserFavourites", "userfavouritetypeid", "dbo.UserFavouriteTypes");
            DropIndex("dbo.UserFavourites", new[] { "userfavouritetypeid" });
            DropTable("dbo.UserFavouriteTypes");
            DropTable("dbo.UserFavourites");
        }
    }
}