using EnterprisePizza.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnterprisePizza.DataAccess
{
    public class EnterprisePizzaDataContext
        : DbContext
    {
        public EnterprisePizzaDataContext()
            : base("EnterprisePizza")
        {
        }

        public DbSet<PizzaOrder> Orders { get; set; }
        public DbSet<Pizza> Pizzas { get; set; }
        public DbSet<Section> Sections { get; set; }
        public DbSet<IngredientSelection> IngredientSelections { get; set; }
        public DbSet<AvailableIngredient> AvailableIngredients { get; set; }
    }
}
