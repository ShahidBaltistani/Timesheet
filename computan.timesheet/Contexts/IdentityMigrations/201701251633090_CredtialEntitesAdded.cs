using System.Data.Entity.Migrations;

namespace computan.timesheet.Contexts.IdentityMigrations
{
    public partial class CredtialEntitesAdded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                    "dbo.CredentialCategories",
                    c => new
                    {
                        id = c.Long(false, true),
                        name = c.String(),
                        isactive = c.Boolean(false),
                        createdonutc = c.DateTime(false),
                        updatedonutc = c.DateTime(),
                        ipused = c.String(maxLength: 20),
                        userid = c.String()
                    })
                .PrimaryKey(t => t.id);

            CreateTable(
                    "dbo.CredentialLevels",
                    c => new
                    {
                        id = c.Long(false, true),
                        name = c.String(),
                        isactive = c.Boolean(false),
                        createdonutc = c.DateTime(false),
                        updatedonutc = c.DateTime(),
                        ipused = c.String(maxLength: 20),
                        userid = c.String()
                    })
                .PrimaryKey(t => t.id);

            CreateTable(
                    "dbo.Credentials",
                    c => new
                    {
                        id = c.Long(false, true),
                        crendentialtypeid = c.Long(false),
                        credentiallevelid = c.Long(false),
                        projectid = c.Long(),
                        credentialcategoryid = c.Long(false),
                        url = c.String(),
                        username = c.String(false),
                        password = c.String(false),
                        port = c.String(),
                        comments = c.String(),
                        host = c.String(),
                        networkdomain = c.String(),
                        createdonutc = c.DateTime(false),
                        updatedonutc = c.DateTime(),
                        ipused = c.String(maxLength: 20),
                        userid = c.String()
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.CredentialCategories", t => t.credentialcategoryid)
                .ForeignKey("dbo.CredentialLevels", t => t.credentiallevelid)
                .ForeignKey("dbo.CredentialTypes", t => t.crendentialtypeid)
                .ForeignKey("dbo.Projects", t => t.projectid)
                .Index(t => t.crendentialtypeid)
                .Index(t => t.credentiallevelid)
                .Index(t => t.projectid)
                .Index(t => t.credentialcategoryid);

            CreateTable(
                    "dbo.CredentialTypes",
                    c => new
                    {
                        id = c.Long(false, true),
                        name = c.String(),
                        createdonutc = c.DateTime(false),
                        updatedonutc = c.DateTime(),
                        ipused = c.String(maxLength: 20),
                        userid = c.String()
                    })
                .PrimaryKey(t => t.id);
        }

        public override void Down()
        {
            DropForeignKey("dbo.Credentials", "projectid", "dbo.Projects");
            DropForeignKey("dbo.Credentials", "crendentialtypeid", "dbo.CredentialTypes");
            DropForeignKey("dbo.Credentials", "credentiallevelid", "dbo.CredentialLevels");
            DropForeignKey("dbo.Credentials", "credentialcategoryid", "dbo.CredentialCategories");
            DropIndex("dbo.Credentials", new[] { "credentialcategoryid" });
            DropIndex("dbo.Credentials", new[] { "projectid" });
            DropIndex("dbo.Credentials", new[] { "credentiallevelid" });
            DropIndex("dbo.Credentials", new[] { "crendentialtypeid" });
            DropTable("dbo.CredentialTypes");
            DropTable("dbo.Credentials");
            DropTable("dbo.CredentialLevels");
            DropTable("dbo.CredentialCategories");
        }
    }
}