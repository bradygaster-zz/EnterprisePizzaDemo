using EnterprisePizza.DataAccess;
using EnterprisePizza.ServiceContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EnterprisePizza.Web.Infrastructure
{
    public class WebSiteInventoryService : IInventoryService
    {
        EnterprisePizzaDataContext _dbContext = new EnterprisePizzaDataContext();

        public void ToggleIngredientAvailability(ToggleIngredientAvailabilityRequest request)
        {
            var ingredient = _dbContext.AvailableIngredients
                .First(x => x.Name == request.IngredientName);

            ingredient.IsInStock = request.IsAvailable;
            _dbContext.AvailableIngredients.Attach(ingredient);

            var entry = _dbContext.Entry(ingredient);
            entry.Property(e => e.IsInStock).IsModified = true;

            _dbContext.SaveChanges();
        }

        public IngredientAvailability[] GetIngredientList()
        {
            var ret = _dbContext.AvailableIngredients.Select(x =>
                new IngredientAvailability
                {
                    IsAvailable = x.IsInStock,
                    Name = x.Name,
                    Id = x.Id
                }).ToArray();

            return ret;
        }
    }
}