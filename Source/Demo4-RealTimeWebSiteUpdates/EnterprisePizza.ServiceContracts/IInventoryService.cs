using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace EnterprisePizza.ServiceContracts
{
    [ServiceContract]
    public interface IInventoryService
    {
        [OperationContract]
        void ToggleIngredientAvailability(ToggleIngredientAvailabilityRequest request);

        [OperationContract]
        IngredientAvailability[] GetIngredientList();
    }

    public class ToggleIngredientAvailabilityRequest
    {
        public string IngredientName { get; set; }
        public bool IsAvailable { get; set; }
    }

    public class IngredientAvailability
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsAvailable { get; set; }
    }
}
