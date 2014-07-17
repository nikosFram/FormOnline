namespace FormOnline.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Answers",
                c => new
                    {
                        AnswerId = c.Int(nullable: false, identity: true),
                        QuestionId = c.Int(nullable: false),
                        AnswerLabel = c.String(),
                    })
                .PrimaryKey(t => t.AnswerId)
                .ForeignKey("dbo.Questions", t => t.QuestionId, cascadeDelete: true)
                .Index(t => t.QuestionId);
            
            CreateTable(
                "dbo.Questions",
                c => new
                    {
                        QuestionId = c.Int(nullable: false, identity: true),
                        FormId = c.Int(nullable: false),
                        QuestionLabel = c.String(),
                    })
                .PrimaryKey(t => t.QuestionId)
                .ForeignKey("dbo.Forms", t => t.FormId, cascadeDelete: true)
                .Index(t => t.FormId);
            
            CreateTable(
                "dbo.Forms",
                c => new
                    {
                        FormId = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        Description = c.String(),
                        ClosingDate = c.DateTime(nullable: false),
                        Hash = c.String(),
                        Url = c.String(),
                        Closed = c.String(),
                    })
                .PrimaryKey(t => t.FormId);
            
            CreateTable(
                "dbo.Stats",
                c => new
                    {
                        StatId = c.Int(nullable: false, identity: true),
                        FormId = c.Int(nullable: false),
                        QuestionId = c.Int(nullable: false),
                        AnswerId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.StatId)
                .Index(t => t.FormId)
                .Index(t => t.QuestionId)
                .Index(t => t.AnswerId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Stats", "QuestionId", "dbo.Questions");
            DropForeignKey("dbo.Stats", "FormId", "dbo.Forms");
            DropForeignKey("dbo.Stats", "AnswerId", "dbo.Answers");
            DropForeignKey("dbo.Questions", "FormId", "dbo.Forms");
            DropForeignKey("dbo.Answers", "QuestionId", "dbo.Questions");
            DropIndex("dbo.Stats", new[] { "AnswerId" });
            DropIndex("dbo.Stats", new[] { "QuestionId" });
            DropIndex("dbo.Stats", new[] { "FormId" });
            DropIndex("dbo.Questions", new[] { "FormId" });
            DropIndex("dbo.Answers", new[] { "QuestionId" });
            DropTable("dbo.Stats");
            DropTable("dbo.Forms");
            DropTable("dbo.Questions");
            DropTable("dbo.Answers");
        }
    }
}
