using System.Data.Entity.Migrations;

namespace computan.timesheet.Contexts.IdentityMigrations
{
    public partial class RulesTablesAdded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                    "dbo.Rules",
                    c => new
                    {
                        id = c.Long(false, true),
                        name = c.String(),
                        runorder = c.Int(false),
                        isactive = c.Boolean(false),
                        createdonutc = c.DateTime(false),
                        updatedonutc = c.DateTime(),
                        ipused = c.String(maxLength: 20),
                        userid = c.String()
                    })
                .PrimaryKey(t => t.id);

            CreateTable(
                    "dbo.RuleActions",
                    c => new
                    {
                        id = c.Long(false, true),
                        ruleid = c.Long(false),
                        ruleactiontypeid = c.Long(false),
                        ruleactionvalue = c.String(),
                        projectid = c.Long(),
                        skillid = c.Long()
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Rules", t => t.ruleid)
                .ForeignKey("dbo.RuleActionTypes", t => t.ruleactiontypeid)
                .Index(t => t.ruleid)
                .Index(t => t.ruleactiontypeid);

            CreateTable(
                    "dbo.RuleActionTypes",
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
                    "dbo.RuleConditions",
                    c => new
                    {
                        id = c.Long(false, true),
                        ruleid = c.Long(false),
                        ruleconditiontypeid = c.Long(false),
                        ruleconditionvalue = c.String()
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Rules", t => t.ruleid)
                .ForeignKey("dbo.RuleConditionTypes", t => t.ruleconditiontypeid)
                .Index(t => t.ruleid)
                .Index(t => t.ruleconditiontypeid);

            CreateTable(
                    "dbo.RuleConditionTypes",
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
                    "dbo.RuleExceptions",
                    c => new
                    {
                        id = c.Long(false, true),
                        ruleid = c.Long(false),
                        ruleexceptiontypeid = c.Long(false),
                        ruleexceptionvalue = c.String()
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Rules", t => t.ruleid)
                .ForeignKey("dbo.RuleExceptionTypes", t => t.ruleexceptiontypeid)
                .Index(t => t.ruleid)
                .Index(t => t.ruleexceptiontypeid);

            CreateTable(
                    "dbo.RuleExceptionTypes",
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
        }

        public override void Down()
        {
            DropForeignKey("dbo.RuleExceptions", "ruleexceptiontypeid", "dbo.RuleExceptionTypes");
            DropForeignKey("dbo.RuleExceptions", "ruleid", "dbo.Rules");
            DropForeignKey("dbo.RuleConditions", "ruleconditiontypeid", "dbo.RuleConditionTypes");
            DropForeignKey("dbo.RuleConditions", "ruleid", "dbo.Rules");
            DropForeignKey("dbo.RuleActions", "ruleactiontypeid", "dbo.RuleActionTypes");
            DropForeignKey("dbo.RuleActions", "ruleid", "dbo.Rules");
            DropIndex("dbo.RuleExceptions", new[] { "ruleexceptiontypeid" });
            DropIndex("dbo.RuleExceptions", new[] { "ruleid" });
            DropIndex("dbo.RuleConditions", new[] { "ruleconditiontypeid" });
            DropIndex("dbo.RuleConditions", new[] { "ruleid" });
            DropIndex("dbo.RuleActions", new[] { "ruleactiontypeid" });
            DropIndex("dbo.RuleActions", new[] { "ruleid" });
            DropTable("dbo.RuleExceptionTypes");
            DropTable("dbo.RuleExceptions");
            DropTable("dbo.RuleConditionTypes");
            DropTable("dbo.RuleConditions");
            DropTable("dbo.RuleActionTypes");
            DropTable("dbo.RuleActions");
            DropTable("dbo.Rules");
        }
    }
}