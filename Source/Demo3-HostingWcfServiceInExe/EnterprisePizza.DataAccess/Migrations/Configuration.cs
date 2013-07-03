namespace EnterprisePizza.DataAccess.Migrations
{
    using EnterprisePizza.Domain;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<EnterprisePizza.DataAccess.EnterprisePizzaDataContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(EnterprisePizza.DataAccess.EnterprisePizzaDataContext context)
        {
            context.AvailableIngredients.AddOrUpdate(x => x.Name,
                new AvailableIngredient { Name = "Extra Cheese", IsInStock = true },
                new AvailableIngredient { Name = "Onions", IsInStock = true },
                new AvailableIngredient { Name = "Green Peppers", IsInStock = true },
                new AvailableIngredient { Name = "Jalapenos", IsInStock = true },
                new AvailableIngredient { Name = "Mushrooms", IsInStock = true },
                new AvailableIngredient { Name = "Pepperoni", IsInStock = true },
                new AvailableIngredient { Name = "Sun Dried Tomatoes", IsInStock = true },
                new AvailableIngredient { Name = "Bacon", IsInStock = true },
                new AvailableIngredient { Name = "Ham", IsInStock = true },
                new AvailableIngredient { Name = "Anchovies", IsInStock = true },
                new AvailableIngredient { Name = "Pineapple", IsInStock = true }
            );
        }
    }
}
