using System;
using Hse.CqrsWorkShop.Domain.Events;

namespace Hse.CqrsWorkShop.Domain.Aggrates
{
    public class ProductAggregate : AggregateBase
    {
        public ProductAggregate()
        {
            RegisterTransition<ProductCreated>(Apply);
        }

        internal string Name { get; private set; }
        internal int Price { get; private set; }

        private void Apply(ProductCreated productCreated)
        {
            Id = productCreated.Id;
            Name = productCreated.Name;
            Price = productCreated.Price;
        }

        private ProductAggregate(Guid id, string name, int price) : this()
        {
            RaiseEvent(new ProductCreated(id, name, price));
        }

        public static IAggregate Create(Guid id, string name, int price)
        {
            return new ProductAggregate(id, name, price);
        }
    }
}