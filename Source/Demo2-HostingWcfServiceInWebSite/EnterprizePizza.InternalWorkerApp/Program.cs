using EnterprisePizza.DataAccess;
using EnterprisePizza.Domain;
using EnterprisePizza.Infrastructure;
using EnterprisePizza.ServiceContracts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EnterprizePizza.InternalWorkerApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var program = new Program();

            Console.WriteLine("Subscribing to the Service Bus Topic");
            program.Subscribe();

            Console.WriteLine("Type Q to End");

            var input = Console.ReadLine();

            while (input != "Q")
            {
                Console.WriteLine("Working", "Information");
                Thread.Sleep(10000);
                input = Console.ReadLine();
            }

            Console.WriteLine("Exiting");
        }

        private void Subscribe()
        {
            // create an instance of the topic helper
            var helper = ServiceBusTopicHelper.Setup(SubscriptionInitializer.Initialize());

            // send the message into the topic
            helper.Subscribe<PizzaOrder>((order) =>
                {
                    // save the order
                    var context = new EnterprisePizzaDataContext();
                    context.Orders.Add(order);
                    context.SaveChanges();

                    // write out a note
                    Console.WriteLine("Order {0} just taken with {1} pizza(s)", 
                        order.Id, 
                        order.Pizzas.Count);

                    // now notify the store of the new order
                    order.IsOrdered = true;

                    // publish the messages as saved but not received yet
                    helper.Publish<PizzaOrder>(order, (m) =>
                        {
                            m.Properties["IsOrdered"] = true;
                            m.Properties["IsReceivedByStore"] = false;
                        });
                }
                , "(IsOrdered = false) AND (IsReceivedByStore = false)",
                "NewPizzaOrders"
            );
        }
    }
}
