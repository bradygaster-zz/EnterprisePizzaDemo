using EnterprisePizza.DataAccess;
using EnterprisePizza.Domain;
using EnterprisePizza.Infrastructure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EnterprisePizza.Web.Controllers
{
    public class HomeController : Controller
    {
        EnterprisePizzaDataContext _dbContext = new EnterprisePizzaDataContext();

        public ActionResult Index()
        {
            return View();
        }

        PizzaOrder CreateRandomOrder()
        {
            // create a new guid to remember this client by
            var clientId = Guid.NewGuid();

            // attach this client id to this order
            var randomOrder = new PizzaOrder
            {
                ClientIdentifier = clientId,
                OrderedTimeStamp = DateTime.Now,
                IsOrdered = false,
                IsReceivedByStore = false
            };

            var toppings = _dbContext.AvailableIngredients.ToList();

            var randomPizzaCount = new Random(Guid.NewGuid().GetHashCode()).Next(1, 5);

            for (int i = 0; i < randomPizzaCount; i++)
            {
                var p = new Pizza
                {
                    Order = randomOrder
                };

                var randomSectionCount = new Random(Guid.NewGuid().GetHashCode()).Next(1, 3);

                for (int x = 0; x < randomSectionCount; x++)
                {
                    var section = new Section
                    {
                        Pizza = p
                    };

                    for (int y = 0; y < randomSectionCount; y++)
                    {
                        var randomToppingCount = new Random(Guid.NewGuid().GetHashCode()).Next(0, toppings.Count() - 1);
                        var topping = toppings[randomToppingCount];

                        section.Ingredients.Add(new IngredientSelection
                        {
                            Section = section,
                            AvailableIngredientId = topping.Id
                        });
                    }
                    p.Sections.Add(section);
                }

                randomOrder.Pizzas.Add(p);
            }

            return randomOrder;
        }

        public ActionResult SampleOrder()
        {
            // create a random order
            var randomOrder = CreateRandomOrder();

            // serialize the order into JSON
            var json = JsonConvert.SerializeObject(randomOrder, Formatting.None, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });

            // send the message into the topic
            ServiceBusTopicHelper
                .Setup(SubscriptionInitializer.Initialize())
                    .Publish<PizzaOrder>(randomOrder, (m) =>
                    {
                        m.Properties["IsOrdered"] = false;
                        m.Properties["IsReceivedByStore"] = false;
                    });

            // queue up the message
            //StorageQueueHelper.OpenQueue("incomingorders").Enqueue(json);

            // remember the clientId for the client
            ViewBag.clientId = randomOrder.ClientIdentifier.ToString();

            return View();
        }
    }
}
