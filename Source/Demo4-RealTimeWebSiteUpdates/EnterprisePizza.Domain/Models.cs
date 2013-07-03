using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EnterprisePizza.Domain
{
    [DataContract]
    public class PizzaOrder
    {
        public PizzaOrder()
        {
            this.Pizzas = new List<Pizza>();
        }

        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public DateTime OrderedTimeStamp { get; set; }

        [DataMember]
        public DateTime? PreppedTimeStamp { get; set; }

        [DataMember]
        public DateTime? LeftBuildingTimeStamp { get; set; }

        [DataMember]
        public DateTime? DeliveredTimeStamp { get; set; }

        [DataMember]
        public List<Pizza> Pizzas { get; set; }

        [DataMember]
        public bool IsOrdered { get; set; }

        [DataMember]
        public bool IsReceivedByStore { get; set; }

        [DataMember]
        public Guid? ClientIdentifier { get; set; }
    }

    [DataContract(IsReference = true)]
    public class Pizza
    {
        public Pizza()
        {
            this.Sections = new List<Section>();
        }

        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public List<Section> Sections { get; set; }

        [DataMember]
        public int OrderId { get; set; }

        [ForeignKey("OrderId")]
        public PizzaOrder Order { get; set; }
    }

    [DataContract(IsReference = true)]
    public class Section
    {
        public Section()
        {
            this.Ingredients = new List<IngredientSelection>();
        }

        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public int PizzaId { get; set; }

        [ForeignKey("PizzaId")]
        public Pizza Pizza { get; set; }

        [DataMember]
        public List<IngredientSelection> Ingredients { get; set; }
    }

    [DataContract(IsReference = true)]
    public class IngredientSelection
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public int SectionId { get; set; }

        [ForeignKey("SectionId")]
        public Section Section { get; set; }

        [DataMember]
        public int AvailableIngredientId { get; set; }
    }

    [DataContract(IsReference = true)]
    public class AvailableIngredient
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public bool IsInStock { get; set; }
    }
}
