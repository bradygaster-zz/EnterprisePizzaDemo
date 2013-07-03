using EnterprisePizza.ServiceContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnterprizePizza.InternalWorkerApp
{
    public class CorporateOfficeInventoryService : IInventoryService
    {
        public void ToggleIngredientAvailability(ToggleIngredientAvailabilityRequest request)
        {
            var msgTemplate = !request.IsAvailable 
                ? "The store is OUT OF {0}" 
                : "The store been replenished with {0}";

            var msg = string.Format(msgTemplate, request.IngredientName);

            Console.WriteLine(msg);
        }

        public IngredientAvailability[] GetIngredientList()
        {
            throw new ApplicationException("Central office doesn't provide ingredients list, stores do");
        }
    }
}
