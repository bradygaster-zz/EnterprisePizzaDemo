using EnterprisePizza.DataAccess;
using EnterprisePizza.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnterprisePizza.OrderBillboard.ViewModels
{
    public class OrderItem
    {
        public OrderItem()
        {
            this.Pizzas = new List<PizzaItem>();
        }

        public int OrderId { get; set; }
        public Guid ClientIdentifier { get; set; }
        public List<PizzaItem> Pizzas { get; set; }

        public static OrderItem ConvertOrderToViewModel(PizzaOrder order)
        {
            // get the ingredients from the database
            var dataContext = new EnterprisePizzaDataContext();
            var ingredients = dataContext.AvailableIngredients.ToList();

            // build the object we'll throw into the service bus topic
            var item = new OrderItem
            {
                OrderId = order.Id,
                ClientIdentifier = order.ClientIdentifier.Value
            };

            // add the pizzas to the order
            order.Pizzas.ForEach(p =>
            {
                var pizzaItem = new PizzaItem
                {
                    PizzaId = p.Id
                };

                // add the sections to the pizza
                p.Sections.ForEach(s =>
                {
                    var sectionItem = new SectionItem();

                    s.Ingredients.ForEach(i =>
                    {
                        sectionItem.Toppings.Add(
                            ingredients.First(x =>
                                x.Id == i.AvailableIngredientId).Name);
                    });

                    pizzaItem.Sections.Add(sectionItem);
                });

                item.Pizzas.Add(pizzaItem);
            });

            return item;
        }
    }

    public class SectionItem
    {
        public SectionItem()
        {
            this.Toppings = new List<string>();
        }

        public int SectionId { get; set; }
        public List<string> Toppings { get; set; }
    }

    public class PizzaItem
    {
        public PizzaItem()
        {
            this.Sections = new List<SectionItem>();
        }

        public int PizzaId { get; set; }
        public List<SectionItem> Sections { get; set; }
    }
}
