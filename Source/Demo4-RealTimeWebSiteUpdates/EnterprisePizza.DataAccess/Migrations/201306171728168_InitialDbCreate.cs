namespace EnterprisePizza.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialDbCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PizzaOrders",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        OrderedTimeStamp = c.DateTime(nullable: false),
                        PreppedTimeStamp = c.DateTime(),
                        LeftBuildingTimeStamp = c.DateTime(),
                        DeliveredTimeStamp = c.DateTime(),
                        IsOrdered = c.Boolean(nullable: false),
                        IsReceivedByStore = c.Boolean(nullable: false),
                        ClientIdentifier = c.Guid(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Pizzas",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        OrderId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.PizzaOrders", t => t.OrderId, cascadeDelete: true)
                .Index(t => t.OrderId);
            
            CreateTable(
                "dbo.Sections",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PizzaId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Pizzas", t => t.PizzaId, cascadeDelete: true)
                .Index(t => t.PizzaId);
            
            CreateTable(
                "dbo.IngredientSelections",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SectionId = c.Int(nullable: false),
                        AvailableIngredientId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Sections", t => t.SectionId, cascadeDelete: true)
                .Index(t => t.SectionId);
            
            CreateTable(
                "dbo.AvailableIngredients",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        IsInStock = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.IngredientSelections", new[] { "SectionId" });
            DropIndex("dbo.Sections", new[] { "PizzaId" });
            DropIndex("dbo.Pizzas", new[] { "OrderId" });
            DropForeignKey("dbo.IngredientSelections", "SectionId", "dbo.Sections");
            DropForeignKey("dbo.Sections", "PizzaId", "dbo.Pizzas");
            DropForeignKey("dbo.Pizzas", "OrderId", "dbo.PizzaOrders");
            DropTable("dbo.AvailableIngredients");
            DropTable("dbo.IngredientSelections");
            DropTable("dbo.Sections");
            DropTable("dbo.Pizzas");
            DropTable("dbo.PizzaOrders");
        }
    }
}
